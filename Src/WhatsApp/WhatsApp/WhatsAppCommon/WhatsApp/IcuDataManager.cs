// Decompiled with JetBrains decompiler
// Type: WhatsApp.IcuDataManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatsAppNative;


namespace WhatsApp
{
  public class IcuDataManager
  {
    private static readonly string ICU_DEFAULT_LOCATION = ".";
    private static readonly string ICU_FILE_DIRECTORY = "C:\\Data\\Users\\Public\\Pictures\\WhatsApp\\icu\\";
    private static readonly string ICU_FILE_NAME = "icudt55l.dat";
    private static readonly string ICU_FILE_NAME_TMP = "icudt55l.tmp";
    private static readonly string ICU_FILE_PATH = IcuDataManager.ICU_FILE_DIRECTORY + IcuDataManager.ICU_FILE_NAME;
    private static readonly string ICU_FILE_PATH_TMP = IcuDataManager.ICU_FILE_DIRECTORY + IcuDataManager.ICU_FILE_NAME_TMP;
    private const int SPACE_REQUIRED_FOR_DOWNLOAD = 30000000;
    private static string LogHeader = "IcuDM";
    private static string SERVER_ENDPOINT = "https://static.whatsapp.net/icu?level=55";
    private static object initLock = new object();
    private static IcuDataManager _instance;
    private string specifiedDirectory;
    private bool downloadInProgress;
    private static WorkQueue requestDataThread;
    private List<string> inProgressLanguages = new List<string>();

    public static void InitializeIcu() => IcuDataManager.GetInstance();

    public static bool SetFullData()
    {
      bool flag = false;
      try
      {
        using (IMediaStorage fs = MediaStorage.Create(IcuDataManager.ICU_FILE_DIRECTORY))
        {
          flag = IcuDataManager.DirectoryExists(fs, IcuDataManager.ICU_FILE_DIRECTORY);
          if (!flag)
          {
            fs.CreateDirectory(IcuDataManager.ICU_FILE_DIRECTORY);
            Log.l(IcuDataManager.LogHeader, "Added indicator that full Icu is required");
          }
        }
      }
      catch (Exception ex)
      {
        Log.l(IcuDataManager.LogHeader, "Exception processing icu directory {0}", (object) ex.ToString());
      }
      return flag;
    }

    public static bool IsFullData() => IcuDataManager.GetInstance().HasFullData();

    private static void GetIcuDataFileDetails(out bool downloadExists, out bool downloadShouldExist)
    {
      downloadExists = false;
      downloadShouldExist = false;
      lock (IcuDataManager.initLock)
      {
        try
        {
          downloadExists = false;
          downloadShouldExist = false;
          using (IMediaStorage fs = MediaStorage.Create(IcuDataManager.ICU_FILE_DIRECTORY))
          {
            downloadShouldExist = IcuDataManager.DirectoryExists(fs, IcuDataManager.ICU_FILE_DIRECTORY);
            if (downloadShouldExist)
              downloadExists = fs.FileExists(IcuDataManager.ICU_FILE_PATH);
            else
              downloadExists = false;
          }
        }
        catch (Exception ex)
        {
          Log.l(IcuDataManager.LogHeader, IcuDataManager.LogHeader + " Exception processing file details {0}", (object) ex.GetFriendlyMessage());
          Log.SendCrashLog(ex, "Exception for full icu", logOnlyForRelease: true);
          downloadExists = false;
          downloadShouldExist = false;
        }
      }
    }

    private static IcuDataManager GetInstance()
    {
      if (IcuDataManager._instance == null)
      {
        lock (IcuDataManager.initLock)
        {
          if (IcuDataManager._instance == null)
            IcuDataManager._instance = new IcuDataManager();
        }
      }
      return IcuDataManager._instance;
    }

    private IcuDataManager()
    {
      if (this.specifiedDirectory != null)
        return;
      bool downloadExists = false;
      bool downloadShouldExist = false;
      IcuDataManager.GetIcuDataFileDetails(out downloadExists, out downloadShouldExist);
      this.specifiedDirectory = downloadExists ? IcuDataManager.ICU_FILE_DIRECTORY : IcuDataManager.ICU_DEFAULT_LOCATION;
      if (this.specifiedDirectory.Length > 0)
      {
        try
        {
          NativeInterfaces.Misc.SetIcuDirectory(this.specifiedDirectory);
        }
        catch (Exception ex1)
        {
          string context1 = IcuDataManager.LogHeader + " exception setting Icu directory";
          Log.LogException(ex1, context1);
          if (this.specifiedDirectory == IcuDataManager.ICU_FILE_DIRECTORY)
          {
            this.specifiedDirectory = IcuDataManager.ICU_DEFAULT_LOCATION;
            try
            {
              NativeInterfaces.Misc.SetIcuDirectory(this.specifiedDirectory);
            }
            catch (Exception ex2)
            {
              string context2 = IcuDataManager.LogHeader + " exception resetting Icu directory";
              Log.LogException(ex2, context2);
            }
            downloadShouldExist = true;
            downloadExists = false;
            IcuDataManager.CleanUpIcuDataFile(true, true);
          }
          Log.SendCrashLog(new Exception("Icu setting exception"), "Failed to set Icu data", logOnlyForRelease: true);
        }
      }
      if (!downloadShouldExist || downloadExists)
        return;
      this.StartDownloadIfRequired();
    }

    private static bool DirectoryExists(IMediaStorage fs, string directory)
    {
      NativeMediaStorage nativeMediaStorage = fs as NativeMediaStorage;
      bool flag;
      if (nativeMediaStorage != null)
      {
        try
        {
          IEnumerable<WIN32_FIND_DATA> files = nativeMediaStorage.FindFiles(IcuDataManager.ICU_FILE_DIRECTORY + "*");
          flag = files != null && files.Any<WIN32_FIND_DATA>();
        }
        catch (DirectoryNotFoundException ex)
        {
          Log.l(IcuDataManager.LogHeader, "DirectoryExists - Directory not found");
          flag = false;
        }
      }
      else
        flag = fs is IsoStoreMediaStorage storeMediaStorage && storeMediaStorage.IsDirectory(IcuDataManager.ICU_FILE_DIRECTORY);
      return flag;
    }

    private static void CleanUpIcuDataFile(bool actual, bool temp)
    {
      using (IMediaStorage mediaStorage = MediaStorage.Create(IcuDataManager.ICU_FILE_DIRECTORY))
      {
        if (actual)
        {
          try
          {
            mediaStorage.DeleteFile(IcuDataManager.ICU_FILE_PATH);
          }
          catch (FileNotFoundException ex)
          {
            Log.d(IcuDataManager.LogHeader, "Icu data not found to clean up");
          }
          catch (Exception ex)
          {
            Log.l(IcuDataManager.LogHeader, "Exception cleaning up actual file {0}", (object) ex.ToString());
          }
        }
        if (!temp)
          return;
        try
        {
          mediaStorage.DeleteFile(IcuDataManager.ICU_FILE_PATH_TMP);
        }
        catch (FileNotFoundException ex)
        {
          Log.d(IcuDataManager.LogHeader, "Icu temp data not found to cleanup");
        }
        catch (Exception ex)
        {
          Log.l(IcuDataManager.LogHeader, "Exception cleaning up temp file {0}", (object) ex.ToString());
        }
      }
    }

    private bool HasFullData() => !(this.specifiedDirectory != IcuDataManager.ICU_FILE_DIRECTORY);

    private static WorkQueue RequestDataThread
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref IcuDataManager.requestDataThread, (Func<WorkQueue>) (() => new WorkQueue()));
      }
    }

    private void StartDownloadIfRequired()
    {
      Log.d(IcuDataManager.LogHeader, "Requesting full icu data {0}", (object) this.downloadInProgress);
      if (this.downloadInProgress)
        return;
      this.downloadInProgress = true;
      if (NativeInterfaces.Misc.GetDiskSpace(IcuDataManager.ICU_FILE_DIRECTORY).FreeBytes < 30000000UL)
        Log.l(IcuDataManager.LogHeader, "Insufficient free space");
      else
        IcuDataManager.RequestDataThread.Enqueue((Action) (() =>
        {
          if (this.HasFullData())
          {
            Log.l(IcuDataManager.LogHeader, "Terminating download, data available!");
          }
          else
          {
            bool downloadExists;
            IcuDataManager.GetIcuDataFileDetails(out downloadExists, out bool _);
            if (downloadExists)
            {
              Log.l(IcuDataManager.LogHeader, "Terminating download, data available!");
            }
            else
            {
              try
              {
                using (IMediaStorage fs = MediaStorage.Create(IcuDataManager.ICU_FILE_DIRECTORY))
                {
                  if (!IcuDataManager.DirectoryExists(fs, IcuDataManager.ICU_FILE_DIRECTORY))
                    fs.CreateDirectory(IcuDataManager.ICU_FILE_DIRECTORY);
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "Create directory exception creating icu data directory");
                return;
              }
              long partialDownloadFileSize = NativeInterfaces.Misc.GetFileSizeFast(IcuDataManager.ICU_FILE_PATH_TMP);
              Dictionary<string, string> dictionary = new Dictionary<string, string>();
              dictionary.Add("Content-Type", "application/octet-stream");
              dictionary.Add("Accept-Encoding", "gzip");
              if (partialDownloadFileSize > 0L)
                dictionary.Add("Range:", string.Format("bytes={0}", (object) partialDownloadFileSize));
              StringBuilder stringBuilder = new StringBuilder();
              foreach (KeyValuePair<string, string> keyValuePair in dictionary)
              {
                if (stringBuilder.Length > 0)
                  stringBuilder.Append("\r\n");
                stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
                {
                  keyValuePair.Value
                })));
              }
              string headers = stringBuilder.ToString();
              IcuDataManager.GetIcuData(IcuDataManager.SERVER_ENDPOINT, headers).Subscribe<IcuDataManager.IcuDataDownloadDetails>((Action<IcuDataManager.IcuDataDownloadDetails>) (downloadDetails =>
              {
                int responseCode = downloadDetails.ResponseCode;
                string responseEncoding = downloadDetails.ResponseEncoding;
                Stream input = (Stream) downloadDetails.IcuDataStream;
                Log.l(IcuDataManager.LogHeader, "processing response {0}, {1}, {2}", (object) responseCode, (object) (responseEncoding ?? "null"), (object) (input != null));
                switch (responseCode)
                {
                  case 200:
                    if (input != null)
                    {
                      if (input.Length != 0L)
                      {
                        try
                        {
                          input.Position = 0L;
                          if (responseEncoding == "gzip")
                            input = (Stream) input.Gunzip();
                          using (IMediaStorage mediaStorage = MediaStorage.Create(IcuDataManager.ICU_FILE_DIRECTORY))
                          {
                            FileMode mode = FileMode.Create;
                            if (partialDownloadFileSize > 0L)
                              mode = FileMode.Append;
                            byte[] buffer = new byte[4096];
                            long num = Math.Max(partialDownloadFileSize, 0L);
                            using (Stream stream = mediaStorage.OpenFile(IcuDataManager.ICU_FILE_PATH_TMP, mode, FileAccess.ReadWrite))
                            {
                              int count;
                              while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                              {
                                stream.Write(buffer, 0, count);
                                num += (long) count;
                              }
                              stream.Close();
                            }
                            Log.l(IcuDataManager.LogHeader, "created temp file with {0} bytes - renaming", (object) num);
                            if (mediaStorage.FileExists(IcuDataManager.ICU_FILE_PATH))
                              mediaStorage.DeleteFile(IcuDataManager.ICU_FILE_PATH);
                            mediaStorage.MoveFile(IcuDataManager.ICU_FILE_PATH_TMP, IcuDataManager.ICU_FILE_PATH);
                            Log.l(IcuDataManager.LogHeader, "created icu data file {0}", (object) IcuDataManager.ICU_FILE_PATH);
                            return;
                          }
                        }
                        catch (Exception ex)
                        {
                          Log.LogException(ex, "Exception downloading icu data");
                          IcuDataManager.SaveFetchError(-100, -1);
                          return;
                        }
                      }
                      else
                        break;
                    }
                    else
                      break;
                  case 404:
                    IcuDataManager.SaveFetchError(responseCode, -1);
                    return;
                }
                IcuDataManager.SaveFetchError(responseCode, 0);
              }), (Action<Exception>) (ex =>
              {
                Log.LogException(ex, "Exception downloading icu data");
                IcuDataManager.SaveFetchError(-200, -1);
              }), (Action) (() => { }));
            }
          }
        }));
    }

    private static void SaveFetchError(int responseCode, int dataSize)
    {
      try
      {
        Log.l(IcuDataManager.LogHeader, "Error download - code {0}, size {1}", (object) responseCode, (object) dataSize);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception saving icu data download state info");
      }
    }

    private static IObservable<IcuDataManager.IcuDataDownloadDetails> GetIcuData(
      string url,
      string headers)
    {
      return NativeWeb.Create<IcuDataManager.IcuDataDownloadDetails>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<IcuDataManager.IcuDataDownloadDetails>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        int responseCode = -1;
        bool onCompletedFired = false;
        string responseEncoding = (string) null;
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, returnedHeaders) =>
        {
          responseCode = code;
          foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headers))
          {
            if (header.Key == "Accept-Encoding")
              responseEncoding = header.Value;
          }
          switch (code)
          {
            case 200:
              break;
            case 304:
            case 404:
              observer.OnNext(new IcuDataManager.IcuDataDownloadDetails()
              {
                ResponseCode = responseCode,
                ResponseEncoding = (string) null,
                IcuDataStream = (MemoryStream) null
              });
              observer.OnCompleted();
              onCompletedFired = true;
              break;
            default:
              observer.OnError(new Exception(string.Format("GetIcuData Unexpected response {0}", (object) code)));
              observer.OnCompleted();
              onCompletedFired = true;
              break;
          }
        });
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnEndResponse = (Action) (() =>
        {
          if (!onCompletedFired)
          {
            mem.Position = 0L;
            observer.OnNext(new IcuDataManager.IcuDataDownloadDetails()
            {
              ResponseCode = responseCode,
              ResponseEncoding = responseEncoding,
              IcuDataStream = mem
            });
            observer.OnCompleted();
          }
          else
            mem.SafeDispose();
        });
        string headers1 = headers;
        req1.Open(url1, (IWebCallback) callbackObject, headers: headers1);
      }));
    }

    private class IcuDataDownloadDetails
    {
      public int ResponseCode = -1;
      public string ResponseEncoding;
      public MemoryStream IcuDataStream;
    }
  }
}
