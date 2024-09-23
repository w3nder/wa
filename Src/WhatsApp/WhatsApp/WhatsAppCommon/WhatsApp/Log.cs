// Decompiled with JetBrains decompiler
// Type: WhatsApp.Log
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using ICSharpCode.SharpZipLib.Silverlight.GZip;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;


namespace WhatsApp
{
  public class Log
  {
    public static bool SuppressCrashLog = false;
    private static System.Type[] filteredExceptionTypes = new System.Type[5]
    {
      typeof (WebException),
      typeof (NetworkException),
      typeof (FunXMPP.Connection.UnforwardableMessageException),
      typeof (ThreadAbortException),
      typeof (DatabaseInvalidatedException)
    };
    private static Func<Exception, bool>[] filters = new Func<Exception, bool>[9]
    {
      (Func<Exception, bool>) (ex => ex is TargetInvocationException && Log.IsFiltered(ex.InnerException)),
      (Func<Exception, bool>) (ex => ((IEnumerable<System.Type>) Log.filteredExceptionTypes).Where<System.Type>((Func<System.Type, bool>) (type => type.IsInstanceOfType((object) ex))).Any<System.Type>()),
      (Func<Exception, bool>) (ex => ex.GetHResult() == 2276592645U),
      (Func<Exception, bool>) (ex => (ex.Message ?? "").StartsWith("FrameworkDispatcher.Update has not been called")),
      (Func<Exception, bool>) (ex => (ex.Message ?? "").StartsWith("Navigation already in progress")),
      (Func<Exception, bool>) (ex => (ex.Message ?? "").StartsWith("Navigation is not allowed when the task is not in the foreground")),
      (Func<Exception, bool>) (ex => ex.GetType() == typeof (Exception) && ex.Message == "0x8000ffff"),
      (Func<Exception, bool>) (ex =>
      {
        int hresult = (int) ex.GetHResult();
        uint num1 = 2147942400;
        uint num2 = (uint) (hresult & (int) ushort.MaxValue);
        bool flag = false;
        if ((hresult & -65536) == (int) num1 && num2 > 1200U && num2 < 12160U)
          flag = true;
        return flag;
      }),
      new Func<Exception, bool>(Log.WasUploadedRecently)
    };
    private static object uploadDedupLock = new object();
    private static Dictionary<string, DateTime> lastUploadByExnType = new Dictionary<string, DateTime>();
    private const int GzipBufferSize = 1048576;
    private static ObservableQueue clbUploadQueue = new ObservableQueue();

    public static void Initialize()
    {
      NativeInterfaces.InitLogSink();
      Log.SetNativeCrashlogInfo();
      if (!AppState.IsBackgroundAgent || !Log.ShouldCreateSupportLog())
        return;
      Log.SendSupportLog((string) null, false);
    }

    public static void WriteLineDebug(string s) => Log.l(s);

    public static void WriteLineDebug(string format, params object[] args) => Log.l(format, args);

    public static void l(string s) => NativeInterfaces.Misc.Log(s, false);

    public static void l(string format, params object[] args) => Log.l(string.Format(format, args));

    public static void l(string header, string body)
    {
      Log.l(string.Format("{0} > {1}", (object) header, (object) body));
    }

    public static void l(string header, string format, params object[] args)
    {
      Log.l(header, string.Format(format, args));
    }

    public static void l(Exception ex, string context, bool stacktrace = true)
    {
      Log.l(Log.GetExceptionString(ex, context, stacktrace));
    }

    public static void l(string header, string context, byte[] dumpBytes, int maxBytes = 1000)
    {
      if (dumpBytes == null || dumpBytes.Length < 0)
      {
        Log.l(header, "{0} bytes: {1}", (object) context, dumpBytes == null ? (object) "null" : (object) "0");
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder(80);
        int num1 = maxBytes < 1 ? dumpBytes.Length : Math.Min(dumpBytes.Length, maxBytes);
        Log.l(header, "{0} bytes: {1} of {2}", (object) context, (object) num1, (object) dumpBytes.Length);
        int val2 = 32;
        for (int index = 0; index < num1; index += val2)
        {
          int num2 = Math.Min(num1 - index, val2);
          stringBuilder.Clear();
          for (int start = index; start < index + num2; ++start)
          {
            if (start != index)
              stringBuilder.Append(", 0x");
            else
              stringBuilder.Append("0x");
            stringBuilder.Append(dumpBytes.ToHexString(start, 1));
          }
          Log.l("     " + stringBuilder.ToString() + ",");
        }
      }
    }

    public static void d(string s) => NativeInterfaces.Misc.Log(s, false);

    public static void d(Func<string> headerFunc, string body) => Log.l(headerFunc(), body);

    public static void d(string header, string body) => Log.l(header, body);

    public static void d(Func<string> headerFunc, string format, params object[] args)
    {
      Log.l(headerFunc(), format, args);
    }

    public static void d(string header, string format, params object[] args)
    {
      Log.l(header, format, args);
    }

    public static void d(Exception ex, string context, bool stacktrace = true)
    {
      Log.l(ex, context, stacktrace);
    }

    public static void d(string header, string context, byte[] dumpBytes, int maxBytes = 1000)
    {
      Log.l(header, context, dumpBytes, maxBytes);
    }

    public static void p(string s)
    {
    }

    public static void p(string header, string body)
    {
    }

    public static void p(string header, string format, params object[] args)
    {
    }

    private static bool WasUploadedRecently(Exception e)
    {
      DateTime utcNow = DateTime.UtcNow;
      string key = string.Format("{0}.{1}.{2}", (object) e.GetType().Name, (object) e.GetHResult(), (object) (e.Message ?? "").GetHashCode());
      lock (Log.uploadDedupLock)
      {
        DateTime dateTime;
        if (Log.lastUploadByExnType.TryGetValue(key, out dateTime) && dateTime > utcNow.AddHours(-1.0))
        {
          Log.l("clb", "Suppressing crashlog; we've already sent a log of this type ({0}) recently.", (object) key);
          return true;
        }
        Log.lastUploadByExnType[key] = utcNow;
        return false;
      }
    }

    private static bool IsFiltered(Exception e)
    {
      if (e == null)
        return false;
      for (int index = 0; index < Log.filters.Length; ++index)
      {
        if (Log.filters[index](e))
        {
          Log.l("clb", "Hit exception filter at index {0}", (object) index);
          return true;
        }
      }
      return false;
    }

    private static string GetStackTrace(Exception ex) => ex.StackTrace;

    private static string GetExceptionString(Exception ex, string context, bool stacktrace = true)
    {
      StringBuilder str = new StringBuilder();
      str.AppendFormat("exception > {0}", (object) context);
      str.Append("\n--\n");
      if (AppState.IsBackgroundAgent)
        str.Append("Raised from background agent\n");
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      str.Append("Device language: " + lang + ", locale: " + locale + "\n");
      TimeSpan timeSpan = TimeSpan.FromMilliseconds((double) NativeInterfaces.Misc.GetTickCount());
      str.Append("Device uptime: " + timeSpan.ToString() + "\n");
      foreach (Exception exception in ex.GetInnerExceptions().Reverse<Exception>())
      {
        exception.GetSynopsis(str);
        str.Append("\n");
        if (stacktrace)
        {
          string stackTrace = Log.GetStackTrace(exception);
          if (!string.IsNullOrEmpty(stackTrace))
          {
            str.Append(stackTrace);
            str.Append("\n");
          }
        }
      }
      str.Append("--\n");
      return str.ToString();
    }

    public static void SetCrashLogInfo()
    {
      Log.CrashLogAttributes.SetDefaultChatID();
      Log.SetNativeCrashlogInfo();
    }

    private static void SetNativeCrashlogInfo()
    {
      NativeInterfaces.Misc.SetLogInfo(string.Format("{0}-{1}-{2}", (object) AppState.GetAppVersion(), (object) Log.CrashLogAttributes.DefaultChatID, (object) "1"));
    }

    public static void LogException(Exception e, string context, bool stacktrace = true)
    {
      Log.l(e, context, stacktrace);
    }

    public static void AttachSupportLogs(string token, NativeStream compressedStream)
    {
      if (compressedStream == null)
        return;
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        using (NativeStream tempFile = nativeMediaStorage.GetTempFile())
        {
          NativeInterfaces.Misc.SubmitLog((object) tempFile.GetNative());
          tempFile.Seek(0L, SeekOrigin.Begin);
          GZipOutputStream destination = new GZipOutputStream((Stream) compressedStream);
          tempFile.CopyTo((Stream) destination, 1048576);
          destination.Finish();
        }
      }
    }

    public static void SendSupportLog(string token, bool force = true)
    {
      Log.l("clb", "Device uptime: {0}", (object) TimeSpan.FromMilliseconds((double) NativeInterfaces.Misc.GetTickCount()).ToString());
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        Log.CreateLogQueueDir(storeForApplication);
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        string filename = string.Format("{0}\\logq\\l{1}.txt", (object) Constants.IsoStorePath, (object) Log.CrashLogAttributes.GenerateFileNameExtraFields(string.IsNullOrEmpty(token) ? NativeInterfaces.Misc.GetTickCount().ToString("x") : "v2" + token));
        using (NativeStream nativeStream = (NativeStream) nativeMediaStorage.OpenFile(filename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
          NativeInterfaces.Misc.SubmitLog((object) nativeStream.GetNative());
      }
      Log.SetCreatedSupportLog();
      Log.TryUpload(force);
    }

    private static string BuildCrashlog(Exception e, string context, bool stacktrace = true)
    {
      StringBuilder str = new StringBuilder();
      str.Append("sending crash log for: ");
      e.GetSynopsis(str);
      if (!string.IsNullOrEmpty(e.StackTrace))
      {
        str.Append("\n");
        str.Append(e.StackTrace);
      }
      Log.l(str.ToString());
      return Log.GetExceptionString(e, context, stacktrace);
    }

    private static void WriteCrashLog(Stream outFile, string str)
    {
      byte[] bytes1 = Encoding.UTF8.GetBytes(str);
      outFile.Write(bytes1, 0, bytes1.Length);
      string str1 = NativeInterfaces.Misc.DumpLog();
      if (string.IsNullOrEmpty(str1))
        return;
      byte[] bytes2 = Encoding.UTF8.GetBytes("native log buffer:\n" + str1);
      outFile.Write(bytes2, 0, bytes2.Length);
    }

    public static void SendCrashLog(
      Exception e,
      string context,
      bool stacktrace = true,
      bool filter = true,
      bool logOnlyForRelease = false)
    {
      try
      {
        bool flag = false;
        if (!flag)
        {
          double totalDays = AppState.GetTimeSinceBuild().TotalDays;
          double maxValue = AppState.DaysUntilExpiration();
          double num = 22.0;
          if (maxValue < num && (double) new Random().Next((int) maxValue) < totalDays - num)
          {
            flag = true;
            Log.l("upload throttled");
          }
        }
        if (!flag & filter && Log.IsFiltered(e))
          flag = true;
        if (flag)
        {
          Log.l(e, "filtered: " + context, stacktrace);
        }
        else
        {
          string str1 = Log.BuildCrashlog(e, context, stacktrace);
          if (Log.SuppressCrashLog)
          {
            Log.l("sending crash logs is disabled during shutdown. writing to debug log instead.\n" + str1);
          }
          else
          {
            using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            {
              Log.CreateLogQueueDir(storeForApplication);
              string str2 = "c" + Log.CrashLogAttributes.GenerateFileNameExtraFields();
              using (IsolatedStorageFileStream file = storeForApplication.CreateFile("logq/" + str2))
                Log.WriteCrashLog((Stream) file, str1);
            }
            Log.TryUpload();
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void CreateForensicsFile(Exception e, Action<Stream> onStream)
    {
      string str = "f" + Log.CrashLogAttributes.GenerateFileNameExtraFields();
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        Log.CreateLogQueueDir(storeForApplication);
        try
        {
          storeForApplication.CreateDirectory("tmp");
        }
        catch (Exception ex)
        {
        }
        storeForApplication.CreateDirectory("tmp/" + str);
      }
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        using (NativeStream nativeStream = (NativeStream) nativeMediaStorage.OpenFile(Constants.IsoStorePath + "\\tmp\\" + str + "\\video.fos", FileMode.CreateNew, FileAccess.ReadWrite))
        {
          using (NativeStream outFile = (NativeStream) nativeMediaStorage.OpenFile(Constants.IsoStorePath + "\\tmp\\" + str + "\\wp.log", FileMode.CreateNew, FileAccess.ReadWrite))
          {
            onStream((Stream) nativeStream);
            Log.WriteCrashLog((Stream) outFile, Log.BuildCrashlog(e, "mp4 check"));
          }
        }
      }
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        storeForApplication.MoveDirectory("tmp/" + str, "logq/" + str);
      Log.TryUpload();
    }

    private static void CreateLogQueueDir(IsolatedStorageFile fs)
    {
      if (fs.DirectoryExists("logq"))
        return;
      try
      {
        fs.CreateDirectory("logq");
      }
      catch (Exception ex)
      {
      }
    }

    public static void TryUpload(bool force = false)
    {
      if (!force && (Voip.IsInCall || Utils.TryGetExpression<bool>((Func<bool>) (() => NativeInterfaces.Misc.GetCellInfo().Roaming && !NetworkStateMonitor.IsWifiDataConnected()), false) || AppState.IsBackgroundAgent))
        Log.l("clb", "upload attempt skipped");
      else if (!NetworkStateMonitor.IsDataConnected())
        Log.l("clb", "upload attempt skipped - no network");
      else
        AppState.Worker.Enqueue((Action) (() =>
        {
          IsolatedStorageFile fs = (IsolatedStorageFile) null;
          RefCountAction refCountAction = new RefCountAction((Action) (() => fs = IsolatedStorageFile.GetUserStoreForApplication()), (Action) (() => fs.Dispose()));
          using (refCountAction.Subscribe())
          {
            string[] strArray1 = (string[]) null;
            try
            {
              strArray1 = fs.GetDirectoryNames("logq/*");
              Log.d("clb", "directory count {0}", strArray1 != null ? (object) strArray1.Length : (object) -1);
            }
            catch (Exception ex)
            {
            }
            foreach (string str in ((IEnumerable<string>) (strArray1 ?? new string[0])).Select<string, string>((Func<string, string>) (n => "logq/" + n)))
            {
              Log.CrashLogAttributes attrs = new Log.CrashLogAttributes(str)
              {
                IsSupport = false
              };
              List<Log.CrashLogSpec> crashLogSpecList = new List<Log.CrashLogSpec>();
              List<Stream> streams = new List<Stream>();
              try
              {
                string reName = Log.CrashLogAttributes.IncrementAttempts(str);
                if (reName != null)
                  fs.MoveDirectory(str, reName);
                string name = reName == null ? str : reName;
                foreach (string fileName in fs.GetFileNames(name + "/*"))
                {
                  IsolatedStorageFileStream storageFileStream = fs.OpenFile(name + "/" + fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                  streams.Add((Stream) storageFileStream);
                  crashLogSpecList.Add(new Log.CrashLogSpec(name, fileName, true)
                  {
                    Stream = (Stream) storageFileStream
                  });
                }
                attrs.IsDetailed = crashLogSpecList.Where<Log.CrashLogSpec>((Func<Log.CrashLogSpec, bool>) (s => s.IsAttachment)).Any<Log.CrashLogSpec>();
                Log.UploadCrashLogObservable((IEnumerable<Log.CrashLogSpec>) crashLogSpecList, attrs).ObserveInQueue<bool>(Log.clbUploadQueue).Subscribe<bool>((Action<bool>) (success =>
                {
                  Log.d("clb", "directory {0} - {1}", (object) name, (object) success);
                  streams.ForEach((Action<Stream>) (s => s.SafeDispose()));
                  streams.Clear();
                  if (!success)
                    return;
                  NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\" + reName, true);
                }), (Action<Exception>) (ex =>
                {
                  streams.ForEach((Action<Stream>) (s => s.SafeDispose()));
                  streams.Clear();
                  Log.LogException(ex, "clb upload observable exception");
                }));
              }
              catch (Exception ex)
              {
                streams.ForEach((Action<Stream>) (s => s.SafeDispose()));
                streams.Clear();
                Log.l("clb", "clb upload exception {0} processing {1}", (object) ex.GetFriendlyMessage(), (object) str);
              }
            }
            string[] strArray2 = (string[]) null;
            try
            {
              strArray2 = fs.GetFileNames("logq/*");
              Log.d("clb", "file count {0}", strArray2 != null ? (object) strArray2.Length : (object) -1);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "clb exception finding files to upload");
            }
            foreach (string str in ((IEnumerable<string>) (strArray2 ?? new string[0])).Select<string, string>((Func<string, string>) (n => "logq/" + n)))
            {
              Stream fileStream = (Stream) null;
              try
              {
                string destinationFileName = Log.CrashLogAttributes.IncrementAttempts(str);
                if (destinationFileName != null)
                  fs.MoveFile(str, destinationFileName);
                string name = destinationFileName == null ? str : destinationFileName;
                fileStream = (Stream) fs.OpenFile(name, FileMode.Open, FileAccess.Read, FileShare.None);
                IDisposable fssub = refCountAction.Subscribe();
                Action<bool> onComplete = (Action<bool>) (success =>
                {
                  Log.d("clb", "file {0} - {1}", (object) name, (object) success);
                  fileStream.SafeDispose();
                  if (!success)
                    return;
                  try
                  {
                    fs.DeleteFile(name);
                  }
                  catch (Exception ex)
                  {
                    Log.LogException(ex, "clb upload delete file exception");
                  }
                  fssub.Dispose();
                });
                int length = name.IndexOf('/');
                Log.UploadCrashLogObservable((IEnumerable<Log.CrashLogSpec>) new Log.CrashLogSpec[1]
                {
                  new Log.CrashLogSpec(name.Substring(0, length), name.Substring(length + 1), false)
                  {
                    Stream = fileStream
                  }
                }, new Log.CrashLogAttributes(name.Substring(length + 1))).ObserveInQueue<bool>(Log.clbUploadQueue).Subscribe<bool>((Action<bool>) (success => onComplete(success)), (Action<Exception>) (ex =>
                {
                  Log.LogException(ex, "clb upload observable exception for file");
                  onComplete(false);
                }));
              }
              catch (Exception ex)
              {
                fileStream.SafeDispose();
                Log.l("clb", "ignoring file {0} after exception: {1}", (object) str, (object) ex.GetFriendlyMessage());
              }
            }
          }
        }));
    }

    private static IObservable<bool> UploadCrashLogObservable(
      IEnumerable<Log.CrashLogSpec> files,
      Log.CrashLogAttributes attrs)
    {
      return !attrs.ShouldSendLog() ? Observable.Return<bool>(true) : Observable.Create<bool>((Func<IObserver<bool>, Action>) (o =>
      {
        bool cancelled = false;
        Action<bool> onUploaded = (Action<bool>) (success =>
        {
          o.OnNext(success);
          o.OnCompleted();
        });
        IDisposable disp = NativeWeb.EmptyPost(Constants.FBCrashlogGateUrl + Log.GetAttributeParamString(attrs)).Subscribe<int>((Action<int>) (code =>
        {
          if (cancelled)
            return;
          if (code == 200)
          {
            try
            {
              Log.UploadCrashLogImpl(files, attrs, onUploaded);
            }
            catch (Exception ex)
            {
              Log.l("clb", string.Format("Crashlog gateway request raised exception: {0}: {1}", (object) ex.GetType().Name, (object) (ex.GetFriendlyMessage() ?? "")));
              if (cancelled)
                return;
              onUploaded(false);
            }
          }
          else
            onUploaded(true);
        }), (Action<Exception>) (e =>
        {
          Log.l("clb", string.Format("Crashlog gateway post request raised exception: {0}: {1}", (object) e.GetType().Name, (object) (e.GetFriendlyMessage() ?? "")));
          if (cancelled)
            return;
          onUploaded(true);
        }));
        return (Action) (() =>
        {
          cancelled = true;
          disp.SafeDispose();
          o.OnCompleted();
        });
      }));
    }

    private static void UploadCrashLogImpl(
      IEnumerable<Log.CrashLogSpec> inputFiles,
      Log.CrashLogAttributes attrs,
      Action<bool> onUploaded)
    {
      List<Stream> streamsToRelease = new List<Stream>();
      Action releaseStreams = Utils.IgnoreMultipleInvokes((Action) (() => streamsToRelease.ForEach((Action<Stream>) (s => s.SafeDispose()))));
      Log.CrashLogSpec[] array1 = inputFiles.Where<Log.CrashLogSpec>((Func<Log.CrashLogSpec, bool>) (s => s.Stream != null && s.Stream.Length != 0L)).ToArray<Log.CrashLogSpec>();
      if (!((IEnumerable<Log.CrashLogSpec>) array1).Any<Log.CrashLogSpec>())
      {
        releaseStreams();
        onUploaded(true);
      }
      else
      {
        Action<bool> onComplete = (Action<bool>) (success =>
        {
          releaseStreams();
          onUploaded(success);
        });
        try
        {
          foreach (Log.CrashLogSpec crashLogSpec in ((IEnumerable<Log.CrashLogSpec>) array1).Where<Log.CrashLogSpec>((Func<Log.CrashLogSpec, bool>) (f => !f.IsCompressed)))
          {
            NativeStream tempFile = new NativeMediaStorage().GetTempFile();
            streamsToRelease.Add((Stream) tempFile);
            GZipOutputStream destination = new GZipOutputStream((Stream) tempFile, 1048576);
            crashLogSpec.Stream.CopyTo((Stream) destination);
            destination.Finish();
            tempFile.Position = 0L;
            crashLogSpec.Stream = (Stream) tempFile;
            crashLogSpec.Filename += ".gz";
          }
          List<MultiPartUploader.FormData> formDataList = new List<MultiPartUploader.FormData>();
          string url = Constants.FBCrashlogUrl + Log.GetAttributeParamString(attrs);
          formDataList.AddRange((IEnumerable<MultiPartUploader.FormData>) ((IEnumerable<Log.CrashLogSpec>) array1).Select<Log.CrashLogSpec, MultiPartUploader.FormDataFile>((Func<Log.CrashLogSpec, MultiPartUploader.FormDataFile>) (file =>
          {
            return new MultiPartUploader.FormDataFile()
            {
              Name = file.IsAttachment ? "attachment" : nameof (file),
              ContentType = "application/x-gzip",
              Content = file.Stream,
              Filename = file.FormDataFilename
            };
          })));
          MultiPartUploader.FormData[] array2 = formDataList.ToArray();
          MultiPartUploader.Open(url, array2).Subscribe<MultiPartUploader.Args>((Action<MultiPartUploader.Args>) (args =>
          {
            if (args.Result == null)
              return;
            args.Result.Dispose();
            bool flag = args.ResponseCode >= 200 && args.ResponseCode < 300;
            if (!flag)
              Log.l("clb", "Log upload got response {0}", (object) args.ResponseCode);
            onComplete(flag);
          }), (Action<Exception>) (e =>
          {
            Log.l("clb", string.Format("Crash log request raised exception: {0}: {1}", (object) e.GetType().Name, (object) (e.GetFriendlyMessage() ?? "")));
            onComplete(false);
          }));
        }
        catch (Exception ex)
        {
          releaseStreams();
          throw;
        }
      }
    }

    private static string GetAttributeParamString(Log.CrashLogAttributes attrs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("?from={0}", (object) (attrs.ChatID ?? Log.CrashLogAttributes.DefaultChatID));
      if (attrs.IsSupport)
        stringBuilder.Append("&type=support");
      if (attrs.IsDetailed)
        stringBuilder.Append("&detailed=true");
      stringBuilder.Append("&access_token=" + Constants.FBCrashlogAccessToken);
      return stringBuilder.ToString();
    }

    public static void ProcessLogIntervalSetting(string val)
    {
      Log.d("log", "process log interval server prop:[{0}]", (object) (val ?? "null"));
      Settings.CreateSupportLogIntervalHours = val;
      val = val?.Trim();
      int result = 0;
      if (!string.IsNullOrEmpty(val))
        int.TryParse(val, out result);
      result = result < 1 ? 0 : result;
      if (result == NonDbSettings.CreateSupportLogIntervalHours)
        return;
      NonDbSettings.CreateSupportLogIntervalHours = result;
      NonDbSettings.NextSupportLogUnixTime = new long?();
    }

    private static bool ShouldCreateSupportLog()
    {
      bool supportLog = false;
      int logIntervalHours = NonDbSettings.CreateSupportLogIntervalHours;
      if (logIntervalHours > 0)
      {
        long? supportLogUnixTime = NonDbSettings.NextSupportLogUnixTime;
        if (supportLogUnixTime.HasValue)
        {
          supportLog = DateTime.UtcNow.ToUnixTime() > supportLogUnixTime.Value;
        }
        else
        {
          long unixTime;
          long num = (long) (new Random((int) (unixTime = DateTime.UtcNow.ToUnixTime())).Next(0, logIntervalHours) * 60 * 60);
          NonDbSettings.NextSupportLogUnixTime = new long?(unixTime + num);
        }
      }
      return supportLog;
    }

    private static void SetCreatedSupportLog()
    {
      int logIntervalHours = NonDbSettings.CreateSupportLogIntervalHours;
      if (logIntervalHours <= 0)
        return;
      NonDbSettings.NextSupportLogUnixTime = new long?(DateTime.UtcNow.ToUnixTime() + (long) (logIntervalHours * 60 * 60));
    }

    private class CrashLogSpec
    {
      public string Filepath;
      public string Filename;
      public bool IsCompressed;
      public bool IsAttachment;
      private bool useNameInFormData = true;
      public Stream Stream;

      public string FormDataFilename
      {
        get
        {
          return !this.useNameInFormData ? "wp.log" + (this.IsCompressed ? ".gz" : "") : this.Filename;
        }
      }

      public CrashLogSpec(string filepath, string name, bool useName)
      {
        this.IsCompressed = name.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase);
        this.IsAttachment = name.EndsWith(".fos" + (this.IsCompressed ? ".gz" : ""), StringComparison.InvariantCultureIgnoreCase);
        this.Filename = name;
        this.useNameInFormData = useName;
        this.Filepath = filepath;
      }
    }

    private class CrashLogAttributes
    {
      public string Version;
      public int? Attempts;
      public bool IsSupport;
      public bool IsDetailed;
      public string ChatID;
      private const int maxAttempts = 3;
      private static readonly string UNKNOWN_CHATID = "unknown";
      private static readonly string NATIVE_LOG_CHATID = "native";
      private static string defaultChatID = Log.CrashLogAttributes.UNKNOWN_CHATID;

      public static string DefaultChatID => Log.CrashLogAttributes.defaultChatID;

      public static void SetDefaultChatID()
      {
        string str = Settings.ChatID;
        if (str == null && Settings.PhoneNumber != null)
          str = Settings.CountryCode + Settings.PhoneNumber;
        Log.CrashLogAttributes.defaultChatID = str ?? Log.CrashLogAttributes.UNKNOWN_CHATID;
      }

      public CrashLogAttributes(string name)
      {
        this.IsSupport = name.StartsWith("l", StringComparison.OrdinalIgnoreCase);
        this.ParseFileNameExtraFields(name);
      }

      public static string IncrementAttempts(string name)
      {
        string[] strArray = name.Split(',');
        int result = 0;
        if (strArray.Length < 2 || !int.TryParse(strArray[1], out result))
          return (string) null;
        strArray[1] = (result + 1).ToString();
        return string.Join(",", strArray);
      }

      public static string GenerateFileNameExtraFields(string timestamp = null)
      {
        if (timestamp == null)
          timestamp = string.Format("{0}", (object) DateTime.Now.Ticks);
        return string.Format("{0}-{1}-{2}-{3},0", (object) timestamp, (object) AppState.GetAppVersion(), (object) Uri.EscapeUriString(Log.CrashLogAttributes.DefaultChatID), (object) "1");
      }

      private void ParseFileNameExtraFields(string name)
      {
        string[] strArray1 = name.Split(',');
        if (strArray1.Length >= 2)
        {
          int result = 0;
          if (int.TryParse(strArray1[1], out result))
            this.Attempts = new int?(result);
        }
        string[] strArray2 = strArray1[0].Split('-');
        if (strArray2.Length >= 2)
          this.Version = strArray2[1];
        if (strArray2.Length >= 3)
        {
          this.ChatID = strArray2[2];
          if (string.IsNullOrEmpty(this.ChatID) || this.ChatID == Log.CrashLogAttributes.UNKNOWN_CHATID || this.ChatID == Log.CrashLogAttributes.NATIVE_LOG_CHATID)
            this.ChatID = Log.CrashLogAttributes.DefaultChatID;
        }
        else
          this.ChatID = Log.CrashLogAttributes.DefaultChatID;
        int length = strArray2.Length;
      }

      public bool ShouldSendLog()
      {
        if (!this.IsSupport || this.Version == null || !(this.Version != AppState.GetAppVersion()))
        {
          int? attempts = this.Attempts;
          int num = 3;
          if ((attempts.GetValueOrDefault() > num ? (attempts.HasValue ? 1 : 0) : 0) == 0 && !AppState.IsExpired)
            return true;
        }
        return false;
      }
    }

    public class StringTooLongException : Exception
    {
      public StringTooLongException(string message)
        : base(message)
      {
      }
    }
  }
}
