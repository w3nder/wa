// Decompiled with JetBrains decompiler
// Type: WhatsApp.Backup
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class Backup
  {
    private static readonly TimeSpan BackupValidity = TimeSpan.FromDays((double) Math.Max(45, 180));
    private const int MaxBackups = 5;
    private const int MaxRootBackupsWithSdCard = 2;
    private const string SummaryFilename = "summary.bin";
    public const int BackupVersion_Encrypted = 0;
    public const int BackupVersion_Compressed = 1;
    public const int BackupVersion_ChatdKey = 2;
    public const int BackupVerison_MaxSupported = 2;
    private const int IoBufferSize = 65536;
    private const string FinalRestoreDir = "restoreFiles";
    private static readonly byte[] SqliteMagic = Encoding.UTF8.GetBytes("SQLite format 3");
    private static Backup.KeyState[] savedKeys_;

    public static string[] GetBackupDirs()
    {
      WhatsAppNative.Backup instance = NativeInterfaces.CreateInstance<WhatsAppNative.Backup>();
      string backupDirs;
      try
      {
        backupDirs = instance.GetBackupDirs();
      }
      catch (Exception ex)
      {
        return new string[0];
      }
      return Utils.ParsePpsz(backupDirs).ToArray();
    }

    private static T SwallowLog<T>(Func<T> op)
    {
      using (NativeInterfaces.Misc.SquelchLogs())
        return op();
    }

    public static IEnumerable<Backup.BackupInfo> ListBackups(Func<string[]> getBackupDirs = null)
    {
      List<Backup.BackupInfo> backupInfoList = new List<Backup.BackupInfo>();
      NativeMediaStorage fs = new NativeMediaStorage();
      foreach (string str in (getBackupDirs ?? new Func<string[]>(Backup.GetBackupDirs))())
      {
        string dir = str;
        try
        {
          foreach (WIN32_FIND_DATA data in Backup.SwallowLog<IEnumerable<WIN32_FIND_DATA>>((Func<IEnumerable<WIN32_FIND_DATA>>) (() => fs.FindFiles(dir + "\\*"))))
          {
            if (data.IsDirectory() && !string.IsNullOrEmpty(data.cFileName) && !(data.cFileName == "."))
            {
              if (!(data.cFileName == ".."))
              {
                try
                {
                  backupInfoList.Add(new Backup.BackupInfo()
                  {
                    FullPath = dir + "\\" + data.cFileName,
                    CreationTime = DateTime.FromFileTimeUtc(data.ftCreationTime),
                    Incomplete = char.ToLowerInvariant(data.cFileName[0]) == 't'
                  });
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "add to backup list");
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "path enumeration");
        }
      }
      return (IEnumerable<Backup.BackupInfo>) backupInfoList;
    }

    public static BackupSummary GetSavedSummary()
    {
      return Backup.ListBackups().OrderByDescending<Backup.BackupInfo, DateTime>((Func<Backup.BackupInfo, DateTime>) (bi => bi.CreationTime)).Where<Backup.BackupInfo>(new Func<Backup.BackupInfo, bool>(Backup.ValidForRestore)).Select<Backup.BackupInfo, BackupSummary>((Func<Backup.BackupInfo, BackupSummary>) (bi =>
      {
        try
        {
          using (Stream input = Backup.SwallowLog<Stream>((Func<Stream>) (() => MediaStorage.OpenFile(bi.FullPath + "\\summary.bin"))))
          {
            BackupSummary savedSummary = BackupSummary.Deserialize(input);
            savedSummary.OnFinalDirectoryKnown(bi.FullPath);
            savedSummary.Timestamp = new DateTime?(bi.CreationTime);
            return savedSummary;
          }
        }
        catch (Exception ex)
        {
          return (BackupSummary) null;
        }
      })).Where<BackupSummary>((Func<BackupSummary, bool>) (p => p != null)).FirstOrDefault<BackupSummary>();
    }

    public static void RemoveSavedSummary()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        foreach (Backup.BackupInfo listBackup in Backup.ListBackups())
        {
          try
          {
            using (NativeInterfaces.Misc.SquelchLogs())
              nativeMediaStorage.DeleteFile(listBackup.FullPath + "\\summary.bin");
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    private static string[] DumpDatabases(
      string subdir,
      Action<int> onProgressPercentage = null,
      Action<long> onTotalFileSize = null,
      Action<string> onProgressString = null,
      BackupSummary summaryObject = null,
      CancellationToken? cancel = null)
    {
      onProgressPercentage = Backup.SanitizeProgressCallback(onProgressPercentage);
      Backup.DatabaseToBackup[] databaseToBackupArray = new Backup.DatabaseToBackup[3];
      Backup.DatabaseToBackup databaseToBackup1;
      if (!AppState.IsBackgroundAgent)
      {
        Backup.BackupViaSqliteBackupApi viaSqliteBackupApi = new Backup.BackupViaSqliteBackupApi();
        viaSqliteBackupApi.Filename = "messages.db";
        viaSqliteBackupApi.Synchronize = (Action<Action>) (a => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          MessagesContext.Reset(true);
          a();
        })));
        databaseToBackup1 = (Backup.DatabaseToBackup) viaSqliteBackupApi;
      }
      else
      {
        databaseToBackup1 = (Backup.DatabaseToBackup) new Backup.BackupViaFile();
        databaseToBackup1.Filename = "messages.db";
        databaseToBackup1.Synchronize = (Action<Action>) (a => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          MessagesContext.Reset(true);
          a();
        })));
      }
      databaseToBackupArray[0] = databaseToBackup1;
      Backup.BackupViaFile backupViaFile1 = new Backup.BackupViaFile();
      backupViaFile1.Filename = "calls.db";
      backupViaFile1.Synchronize = new Action<Action>(((AsyncExtensions) CallLog.Lock).PerformWithLock);
      databaseToBackupArray[1] = (Backup.DatabaseToBackup) backupViaFile1;
      Backup.BackupViaFile backupViaFile2 = new Backup.BackupViaFile();
      backupViaFile2.Filename = "settings.db";
      backupViaFile2.Synchronize = new Action<Action>(Settings.PerformWithWriteLock);
      databaseToBackupArray[2] = (Backup.DatabaseToBackup) backupViaFile2;
      Backup.DatabaseToBackup[] dbs = databaseToBackupArray;
      Action<int> action1 = onProgressPercentage;
      if (action1 != null)
        action1(0);
      cancel?.ThrowIfCancellationRequested();
      dbs = ((IEnumerable<Backup.DatabaseToBackup>) dbs).Where<Backup.DatabaseToBackup>((Func<Backup.DatabaseToBackup, bool>) (d =>
      {
        try
        {
          return d.FileSize != 0L;
        }
        catch (Exception ex)
        {
          return false;
        }
      })).ToArray<Backup.DatabaseToBackup>();
      using (new DisposableAction((Action) (() =>
      {
        foreach (IDisposable d in dbs)
          d.SafeDispose();
      })))
      {
        long totalBytes = 0;
        long currentBytes = 0;
        int totalPct = 0;
        foreach (Backup.DatabaseToBackup databaseToBackup2 in dbs)
        {
          Log.l("backup", "Opening [{0}]", (object) databaseToBackup2.Filename);
          databaseToBackup2.OpenDb(subdir);
          totalBytes += databaseToBackup2.FileSize;
        }
        if (onTotalFileSize != null)
          onTotalFileSize(totalBytes);
        foreach (Backup.DatabaseToBackup databaseToBackup3 in dbs)
        {
          Backup.DatabaseToBackup db = databaseToBackup3;
          db.Export((Action<long, int>) ((bytes, pct) =>
          {
            currentBytes += bytes;
            int num = (int) (currentBytes * 100L / totalBytes);
            if (num <= totalPct)
              return;
            totalPct = num;
            Action<string> action2 = onProgressString;
            if (action2 != null)
              action2(string.Format("sqlite backup ({1}% of {0}, {2}% of all DBs)", (object) db.Filename, (object) pct, (object) totalPct));
            Action<int> action3 = onProgressPercentage;
            if (action3 == null)
              return;
            action3(totalPct);
          }), cancel);
        }
      }
      cancel?.ThrowIfCancellationRequested();
      using (Sqlite sqlite = new Sqlite(MediaStorage.GetAbsolutePath(subdir + "\\messages.db"), SqliteOpenFlags.READWRITE))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("DELETE FROM WaStatuses"))
          preparedStatement.Step();
        if (summaryObject != null)
        {
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("SELECT LocalFileUri, Sha1Hash, FileSize FROM LocalFiles WHERE FileType = ? AND ReferenceCount <> 0"))
          {
            preparedStatement.Bind(0, 0, false);
            while (preparedStatement.Step())
            {
              string column1 = (string) preparedStatement.Columns[0];
              byte[] column2 = (byte[]) preparedStatement.Columns[1];
              object column3 = preparedStatement.Columns[2];
              long? nullable1 = new long?();
              if (column3 != null)
                nullable1 = new long?((long) column3);
              if (!string.IsNullOrEmpty(column1) && (column2 == null || column2.Length != 0))
              {
                long? nullable2 = nullable1;
                long num = 0;
                if ((nullable2.GetValueOrDefault() < num ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
                {
                  BackupMediaFile backupMediaFile = new BackupMediaFile()
                  {
                    FileRef = MediaStorage.AnalyzePath(column1),
                    Sha1Hash = column2,
                    Size = nullable1
                  };
                  summaryObject.MediaFiles.Add(backupMediaFile);
                  cancel?.ThrowIfCancellationRequested();
                }
              }
            }
          }
        }
      }
      Action<int> action4 = onProgressPercentage;
      if (action4 != null)
        action4(100);
      if (summaryObject != null)
      {
        summaryObject.Timestamp = new DateTime?(DateTime.UtcNow);
        try
        {
          NativeMediaStorage nativeMediaStorage = new NativeMediaStorage();
          long fileTime = 0;
          string absolutePath = MediaStorage.GetAbsolutePath(subdir + "\\*");
          foreach (WIN32_FIND_DATA file in nativeMediaStorage.FindFiles(absolutePath))
          {
            if (!file.IsDirectory() && !string.IsNullOrEmpty(file.cFileName) && !(file.cFileName == ".") && !(file.cFileName == "..") && file.ftCreationTime > fileTime)
              fileTime = file.ftCreationTime;
          }
          if (fileTime > 0L)
          {
            DateTime dateTime1 = DateTime.FromFileTimeUtc(fileTime);
            DateTime dateTime2 = dateTime1;
            DateTime? timestamp = summaryObject.Timestamp;
            if ((timestamp.HasValue ? (dateTime2 < timestamp.GetValueOrDefault() ? 1 : 0) : 0) != 0)
              summaryObject.Timestamp = new DateTime?(dateTime1);
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception extracting creation time from backup");
        }
      }
      return ((IEnumerable<Backup.DatabaseToBackup>) dbs).Select<Backup.DatabaseToBackup, string>((Func<Backup.DatabaseToBackup, string>) (db => db.Filename)).ToArray<string>();
    }

    private static byte[] DeriveKey(string filename, byte[] salt, int version, Backup.KeyState ks)
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange(filename.Select<char, byte>((Func<char, byte>) (ch =>
      {
        if (ch >= 'A' && ch <= 'Z')
          ch = char.ToLowerInvariant(ch);
        return ch >= 'a' && ch <= 'Z' || ch >= '0' && ch <= '9' ? (byte) ch : (byte) 0;
      })).Where<byte>((Func<byte, bool>) (b => b > (byte) 0)));
      if (version >= 2)
      {
        if (ks?.Key == null)
          throw new IOException("Backup key not set");
        byteList.AddRange((IEnumerable<byte>) ks.Key);
      }
      else
      {
        foreach (ushort num in Settings.MyJid)
        {
          byteList.Add((byte) num);
          byteList.Add((byte) ((uint) num >> 8));
        }
      }
      return new Rfc2898DeriveBytes(byteList.ToArray(), salt ?? new byte[8], 4).GetBytes(20);
    }

    private static void EncryptDatabases(
      List<string> files,
      Backup.IWriteCallback callback,
      int version,
      CancellationToken? cancel = null,
      BackupSummary summaryObject = null,
      Action<bool> onReadIoComplete = null)
    {
      Backup.KeyState ks = (Backup.KeyState) null;
      RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
      byte[] data1;
      if (version >= 2)
      {
        byte[] backupKey = Settings.BackupKey;
        ks = backupKey != null ? JsonBase.Deserialize<Backup.KeyState>(backupKey, 0, backupKey.Length) : throw new IOException("No keys exist");
        if (ks == null)
          throw new IOException("Could not deserialize key blob");
        byte[] data2 = new byte[16];
        cryptoServiceProvider.GetBytes(data2);
        ks.LocalSalt = data2;
        data1 = ks.SerializeWithoutKey();
      }
      else
      {
        data1 = new byte[16];
        cryptoServiceProvider.GetBytes(data1);
      }
      Backup.BackupHeader s = new Backup.BackupHeader();
      s.Version = version;
      s.OffsetToPayload = SerializeStruct.SizeOf<Backup.BackupHeader>() + data1.Length;
      byte[] buffer = new byte[SerializeStruct.SizeOf<Backup.BackupHeader>()];
      SerializeStruct.Write<Backup.BackupHeader>(s, buffer, 0, buffer.Length);
      byte[] numArray = new byte[65536];
      byte[] compressorOutput = (byte[]) null;
      try
      {
        foreach (string file in files)
        {
          ref CancellationToken? local1 = ref cancel;
          CancellationToken valueOrDefault;
          if (local1.HasValue)
          {
            valueOrDefault = local1.GetValueOrDefault();
            valueOrDefault.ThrowIfCancellationRequested();
          }
          IDeflate compressor = (IDeflate) null;
          HashAlgorithm hasher = (HashAlgorithm) null;
          long destinationSize = 0;
          if (summaryObject != null)
            hasher = (HashAlgorithm) new SHA1Managed();
          using (IMediaStorage mediaStorage = MediaStorage.Create(file))
          {
            using (Stream stream = mediaStorage.OpenFile(file))
            {
              int num = file.LastIndexOfAny(new char[2]
              {
                '\\',
                '/'
              });
              string str = file;
              if (num >= 0)
                str = file.Substring(num + 1);
              int pendingProgress = 0;
              Action<byte[], int, int, int> write = (Action<byte[], int, int, int>) ((b, o, l, pl) =>
              {
                hasher?.TransformBlock(b, o, l, (byte[]) null, 0);
                callback.Write(b, o, l, pl + pendingProgress);
                pendingProgress = 0;
                destinationSize += (long) l;
              });
              callback.OpenFile(str);
              write(buffer, 0, buffer.Length, 0);
              write(data1, 0, data1.Length, 0);
              RC4 rc4 = new RC4(Backup.DeriveKey(str, ks?.LocalSalt ?? data1, version, ks), 4096);
              bool finishCalled = false;
              Action action = (Action) null;
              Action<byte[], int, int, int> innerWrite = write;
              write = (Action<byte[], int, int, int>) ((b, o, l, pl) =>
              {
                rc4.Cipher(b, o, l);
                innerWrite(b, o, l, pl);
              });
              if (version >= 1)
              {
                if (compressorOutput == null)
                  compressorOutput = new byte[65536];
                compressor = (IDeflate) NativeInterfaces.CreateInstance<NativeDeflate>();
                compressor.Initialize(1);
                IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
                IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
                instance1.Put(numArray, 0, numArray.Length);
                instance2.Put(compressorOutput, 0, compressorOutput.Length);
                compressor.SetInputBuffer(instance1);
                compressor.SetOutputBuffer(instance2);
                Marshal.ReleaseComObject((object) instance1);
                Marshal.ReleaseComObject((object) instance2);
                action = (Action) (() =>
                {
                  bool flag = true;
                  while (flag)
                  {
                    ref CancellationToken? local2 = ref cancel;
                    if (local2.HasValue)
                      local2.GetValueOrDefault().ThrowIfCancellationRequested();
                    flag = compressor.Deflate(finishCalled);
                    int outputLength = compressor.GetOutputLength();
                    if (outputLength != 0)
                    {
                      write(compressorOutput, 0, outputLength, 0);
                      compressor.ResetOutputBuffer();
                    }
                  }
                });
              }
              Stats.LogMemoryUsage(gc: true);
              int Length;
              while ((Length = stream.Read(numArray, 0, numArray.Length)) != 0)
              {
                ref CancellationToken? local3 = ref cancel;
                if (local3.HasValue)
                {
                  valueOrDefault = local3.GetValueOrDefault();
                  valueOrDefault.ThrowIfCancellationRequested();
                }
                if (compressor != null)
                {
                  pendingProgress += Length;
                  compressor.SetInputLength(Length);
                  action();
                }
                else
                  write(numArray, 0, Length, Length);
                ref CancellationToken? local4 = ref cancel;
                if (local4.HasValue)
                {
                  valueOrDefault = local4.GetValueOrDefault();
                  valueOrDefault.ThrowIfCancellationRequested();
                }
              }
              if (compressor != null)
              {
                finishCalled = true;
                action();
                Marshal.ReleaseComObject((object) compressor);
              }
              if (hasher != null)
              {
                hasher.TransformFinalBlock(new byte[0], 0, 0);
                summaryObject.OnDatabaseHashKnown(str, hasher.Hash, destinationSize);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (onReadIoComplete != null)
          onReadIoComplete(false);
        onReadIoComplete = (Action<bool>) null;
        callback.Discard();
        throw;
      }
      finally
      {
        if (onReadIoComplete != null)
          onReadIoComplete(true);
        callback.Close();
      }
      callback.Commit();
    }

    public static void Save(
      Action onComplete = null,
      Action<int> onProgress = null,
      BackupSummary summaryObject = null,
      CancellationToken? cancel = null)
    {
      string progressString = (string) null;
      Action<int> progressSnap = onProgress;
      onProgress = (Action<int>) (pct =>
      {
        Log.l("backup", "{0}% complete > current op: {1}", (object) pct, (object) progressString);
        Action<int> action = progressSnap;
        if (action == null)
          return;
        action(pct);
      });
      onProgress = Backup.SanitizeProgressCallback(onProgress);
      try
      {
        string[] locations = Backup.GetBackupDirs();
        if (locations.Length == 0)
        {
          Log.l("backup", "no backup dirs to speak of.  exiting");
        }
        else
        {
          Backup.CleanupOld();
          cancel?.ThrowIfCancellationRequested();
          string backupDir = "tmp\\backup";
          using (IMediaStorage fs = MediaStorage.Create(backupDir))
          {
            using (NativeInterfaces.Misc.SquelchLogs())
            {
              Action<Action> action1 = (Action<Action>) (a =>
              {
                try
                {
                  a();
                }
                catch (Exception ex)
                {
                }
              });
              Action[] actionArray = new Action[3]
              {
                (Action) (() => fs.RemoveDirectoryRecursive(backupDir, renameFirst: true)),
                (Action) (() => fs.CreateDirectory("tmp")),
                (Action) (() => fs.CreateDirectory(backupDir))
              };
              foreach (Action action2 in actionArray)
                action1(action2);
            }
          }
          long dbSize = 0;
          string[] source = Backup.DumpDatabases(backupDir, (Action<int>) (pct =>
          {
            Action<int> action = onProgress;
            if (action == null)
              return;
            action(pct / (locations.Length + 1));
          }), (Action<long>) (s => dbSize = s), (Action<string>) (str => progressString = str), summaryObject, cancel);
          locations = ((IEnumerable<string>) locations).Where<string>((Func<string, bool>) (path =>
          {
            bool flag = true;
            try
            {
              flag = NativeInterfaces.Misc.GetDiskSpace(path).FreeBytes >= (ulong) dbSize;
            }
            catch (Exception ex)
            {
            }
            if (!flag)
              Log.l("backup", "Excluding backup into [{0}] due to disk space", (object) path);
            return flag;
          })).ToArray<string>();
          if (locations.Length == 0)
            throw new IOException("No backup locations.");
          int callbackIdx = 0;
          long totalBytes = 0;
          object progressLock = new object();
          Backup.SimpleWriteCallback mainCallback = (Backup.SimpleWriteCallback) null;
          Backup.ParallelWriteCallback parallelIo = new Backup.ParallelWriteCallback(((IEnumerable<string>) locations).Select<string, Func<Backup.IWriteCallback>>((Func<string, Func<Backup.IWriteCallback>>) (loc => (Func<Backup.IWriteCallback>) (() =>
          {
            long myTotalBytes = 0;
            int lastPct = 0;
            int dumpDbProgressPart = 100 / (locations.Length + 1);
            Backup.SimpleWriteCallback simpleWriteCallback = new Backup.SimpleWriteCallback(loc + "\\temp", (Action<long>) (bytes =>
            {
              if (onProgress == null)
                return;
              long num3 = bytes;
              bytes -= myTotalBytes;
              myTotalBytes = num3;
              lock (progressLock)
              {
                totalBytes += bytes;
                int num4 = dumpDbProgressPart + (int) (totalBytes * 100L / (dbSize * (long) (locations.Length + 1)));
                if (num4 == lastPct)
                  return;
                progressString = string.Format("encrypt database ({0}%)", (object) (totalBytes * 100L / (dbSize * (long) locations.Length)));
                Action<int> action = onProgress;
                if (action != null)
                  action(num4);
                lastPct = num4;
              }
            }));
            if (callbackIdx++ == 0)
              mainCallback = simpleWriteCallback;
            return (Backup.IWriteCallback) simpleWriteCallback;
          }))).ToArray<Func<Backup.IWriteCallback>>());
          Func<string, string> selector = (Func<string, string>) (s => backupDir + "\\" + s);
          Backup.EncryptDatabases(((IEnumerable<string>) source).Select<string, string>(selector).ToList<string>(), (Backup.IWriteCallback) parallelIo, 2, cancel, summaryObject, (Action<bool>) (succeeded =>
          {
            if (succeeded && summaryObject != null)
              parallelIo.Schedule(0, (Action<Backup.IWriteCallback>) (writer =>
              {
                writer.OpenFile("summary.bin");
                summaryObject.Serialize((Action<byte[], int, int>) ((buf, offset, len) => writer.Write(buf, offset, len)));
              }));
            NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\" + backupDir, true, true);
          }));
          parallelIo.CheckWorkerExceptions();
          summaryObject?.OnFinalDirectoryKnown(mainCallback.FinalDbPath);
          progressString = "commit.";
          Action<int> action3 = onProgress;
          if (action3 == null)
            return;
          action3(100);
        }
      }
      catch (OperationCanceledException ex)
      {
      }
      finally
      {
        if (onComplete != null)
          onComplete();
      }
    }

    private static string Commit(string dir, string temp)
    {
      DateTime date = DateTime.Now.Date;
      // ISSUE: variable of a boxed type
      __Boxed<int> year = (ValueType) date.Year;
      int num1 = date.Month;
      string str1 = num1.ToString().PadLeft(2, '0');
      num1 = date.Day;
      string str2 = num1.ToString().PadLeft(2, '0');
      string prefix = string.Format("{0}-{1}-{2}-", (object) year, (object) str1, (object) str2);
      int num2 = 0;
label_1:
      using (NativeMediaStorage fs = new NativeMediaStorage())
      {
        WIN32_FIND_DATA[] source = new WIN32_FIND_DATA[0];
        try
        {
          source = Backup.SwallowLog<WIN32_FIND_DATA[]>((Func<WIN32_FIND_DATA[]>) (() => fs.FindFiles(dir + "\\" + prefix + "*").ToArray<WIN32_FIND_DATA>()));
        }
        catch (Exception ex)
        {
        }
        int num3 = ((IEnumerable<WIN32_FIND_DATA>) source).Select<WIN32_FIND_DATA, string>((Func<WIN32_FIND_DATA, string>) (fd => fd.cFileName.Substring(prefix.Length))).Select<string, int>((Func<string, int>) (s =>
        {
          int result = -1;
          int.TryParse(s, out result);
          return result;
        })).DefaultIfEmpty<int>(-1).Max();
        string dst = dir + "\\" + prefix + (num3 + 1).ToString().PadLeft(4, '0');
        using (NativeInterfaces.Misc.SquelchLogs())
        {
          try
          {
            fs.MoveFile(temp, dst);
          }
          catch (Exception ex)
          {
            if (num2++ > 3)
              throw;
            else
              goto label_1;
          }
        }
        return dst;
      }
    }

    private static void CleanupOld()
    {
      List<string> toDelete = new List<string>();
      List<List<Backup.BackupInfo>> source1 = new List<List<Backup.BackupInfo>>();
      Action<string> action = (Action<string>) (path =>
      {
        if (Backup.IsCloudBackupInProgressUnderDirectory(path))
          return;
        int num = path.LastIndexOf('\\');
        if (num < 0)
          return;
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          string dst = path.Substring(0, num + 1) + "t_" + path.Substring(num + 1);
          try
          {
            nativeMediaStorage.MoveFile(path, dst);
          }
          catch (Exception ex)
          {
            return;
          }
          toDelete.Add(dst);
        }
      });
      foreach (string backupDir in Backup.GetBackupDirs())
      {
        string dir = backupDir;
        List<Backup.BackupInfo> backupInfoList = new List<Backup.BackupInfo>();
        foreach (Backup.BackupInfo backupInfo in (IEnumerable<Backup.BackupInfo>) Backup.ListBackups((Func<string[]>) (() => new string[1]
        {
          dir
        })).OrderByDescending<Backup.BackupInfo, DateTime>((Func<Backup.BackupInfo, DateTime>) (info => info.CreationTime)))
        {
          if (backupInfo.Incomplete)
            toDelete.Add(backupInfo.FullPath);
          else if (backupInfo.CreationTime > DateTime.UtcNow || backupInfo.CreationTime < DateTime.UtcNow.Add(-Backup.BackupValidity))
            action(backupInfo.FullPath);
          else
            backupInfoList.Add(backupInfo);
        }
        source1.Add(backupInfoList);
      }
      for (int index = 0; index < source1.Count; ++index)
      {
        List<Backup.BackupInfo> source2 = source1[index];
        int max = 5;
        if (index == 0 && source1.Skip<List<Backup.BackupInfo>>(1).Where<List<Backup.BackupInfo>>((Func<List<Backup.BackupInfo>, bool>) (b => b.Count >= max)).Any<List<Backup.BackupInfo>>())
          max = 2;
        if (source2.Count > max)
        {
          foreach (Backup.BackupInfo backupInfo in source2.Skip<Backup.BackupInfo>(max).Reverse<Backup.BackupInfo>())
            action(backupInfo.FullPath);
        }
      }
      foreach (string path in toDelete)
      {
        Log.l("backup", "attempting to delete old backup: {0}", (object) path);
        NativeInterfaces.Misc.RemoveDirectoryRecursive(path, true);
      }
    }

    public static void ResetBackupKeys()
    {
      if (Settings.BackupKey == null)
      {
        Log.l(nameof (Backup), "Refresh keys ignored, already requested");
      }
      else
      {
        Settings.BackupKey = (byte[]) null;
        Settings.RestoreKeys = (byte[]) null;
        Backup.SavedKeys = (Backup.KeyState[]) null;
      }
    }

    public static void MaybeUpdateBackupKey(FunXMPP.Connection conn)
    {
      if (Settings.BackupKey != null && Settings.BackupKeySetUtc.HasValue)
        return;
      Log.l(nameof (Backup), "Backup key being generated, was null:{0}", (object) (Settings.BackupKey == null));
      RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
      byte[] accountHash = new byte[20];
      byte[] data = accountHash;
      cryptoServiceProvider.GetBytes(data);
      conn.SendCreateCipherKey(accountHash).Subscribe<Backup.KeyState>((Action<Backup.KeyState>) (ks =>
      {
        Log.l("backup", "Backup key change processed");
        Settings.BackupKey = ks.Serialize();
        Settings.BackupKeySetUtc = new DateTime?(DateTime.UtcNow);
      }), (Action<Exception>) (ex => Log.LogException(ex, "create cipher")));
    }

    public static void Restore()
    {
      foreach (Backup.BackupInfo backupInfo in Backup.ListBackups().OrderByDescending<Backup.BackupInfo, DateTime>((Func<Backup.BackupInfo, DateTime>) (info => info.CreationTime)).Where<Backup.BackupInfo>(new Func<Backup.BackupInfo, bool>(Backup.ValidForRestore)))
      {
        string str = "restoreTmp";
        NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\" + str, true, renameTarget: "tmp\\restore-" + DateTime.UtcNow.ToUnixTime().ToString());
        using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
          storeMediaStorage.CreateDirectory(str);
        Log.l("backup", "trying restore from: {0}", (object) backupInfo.FullPath);
        try
        {
          Backup.RestoreInto(backupInfo.FullPath, str);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "restore");
          continue;
        }
        Backup.CommitRestore(str);
        return;
      }
      throw new IOException("None of the backups passed the gauntlet!");
    }

    public static void CommitRestore(string restoreDir)
    {
      NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\restoreFiles", true, renameTarget: "tmp\\restoreCommit-" + DateTime.UtcNow.ToUnixTime().ToString());
      using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
        storeMediaStorage.MoveFile(restoreDir, "restoreFiles");
      Backup.OnAppStarted(false);
    }

    private static bool HasSqliteMagic(Stream stream)
    {
      long position = stream.Position;
      try
      {
        byte[] numArray = new byte[Backup.SqliteMagic.Length];
        stream.Read(numArray, 0, numArray.Length);
        return Backup.SqliteMagic.IsEqualBytes(numArray);
      }
      finally
      {
        stream.Position = position;
      }
    }

    public static void RestoreInto(string srcDir, string dstDir, Set<string> filesToIgnore = null)
    {
      Func<string, bool> func = (Func<string, bool>) null;
      if (filesToIgnore != null)
      {
        filesToIgnore = new Set<string>(filesToIgnore.Select<string, string>((Func<string, string>) (p => p.ToLowerInvariant())));
        func = (Func<string, bool>) (k => filesToIgnore.Contains(k.ToLowerInvariant()));
      }
      bool legacy = false;
      if (!srcDir.StartsWith("D:", StringComparison.OrdinalIgnoreCase))
      {
        string str = srcDir + "\\messages.db";
        using (IMediaStorage mediaStorage = MediaStorage.Create(str))
        {
          using (Stream stream = mediaStorage.OpenFile(str))
          {
            if (Backup.HasSqliteMagic(stream))
            {
              legacy = true;
              Log.l("backup", "found legacy backup format");
            }
          }
        }
      }
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        foreach (WIN32_FIND_DATA file in nativeMediaStorage.FindFiles(MediaStorage.GetAbsolutePath(srcDir) + "\\*"))
        {
          if (!(file.cFileName == ".") && !(file.cFileName == "..") && string.Compare(file.cFileName, "summary.bin", StringComparison.OrdinalIgnoreCase) != 0 && !file.IsDirectory() && (func == null || !func(file.cFileName)))
          {
            string filename = srcDir + "\\" + file.cFileName;
            Log.l("backup", "copying [{0}] ...", (object) filename);
            Backup.DecryptFile(filename, dstDir, legacy);
          }
        }
      }
      if (!Backup.ValidateRestore(dstDir))
        throw new IOException("ValidateRestore returned false");
    }

    private static void DecryptFile(
      string filename,
      Stream srcStream,
      Action<byte[], int, int, Action> write,
      int? bufferSize = null,
      Action<Backup.KeyState> onKeyState = null)
    {
      bool cancel = false;
      Action cancelObject = (Action) (() => cancel = true);
      byte[] buffer = new byte[SerializeStruct.SizeOf<Backup.BackupHeader>()];
      if (srcStream.Read(buffer, 0, buffer.Length) != buffer.Length)
        throw new IOException("Short read on backup header");
      Backup.BackupHeader backupHeader = SerializeStruct.Read<Backup.BackupHeader>(buffer, 0, buffer.Length);
      if (backupHeader.Version < 0 || backupHeader.Version > 2)
        throw new IOException("Invalid backup header version");
      if (backupHeader.OffsetToPayload < buffer.Length)
        throw new IOException("Invalid backup header");
      byte[] numArray1 = new byte[backupHeader.OffsetToPayload - buffer.Length];
      if (srcStream.Read(numArray1, 0, numArray1.Length) != numArray1.Length)
        throw new IOException("Short read on backup salt");
      Backup.KeyState ks = (Backup.KeyState) null;
      if (backupHeader.Version >= 2)
      {
        ks = JsonBase.Deserialize<Backup.KeyState>(numArray1, 0, numArray1.Length);
        numArray1 = ks != null ? ks.LocalSalt : throw new Exception("Could not deserialize key blob");
        Backup.BindKeyState(ks);
        if (onKeyState != null)
          onKeyState(ks);
      }
      RC4 rc4 = new RC4(Backup.DeriveKey(filename, numArray1, backupHeader.Version, ks), 4096);
      byte[] numArray2 = new byte[bufferSize ?? 65536];
      Inflater decompressor = (Inflater) null;
      byte[] decompressBuffer = (byte[]) null;
      Action action = (Action) null;
      if (backupHeader.Version >= 1)
      {
        decompressor = new Inflater();
        decompressBuffer = new byte[(bufferSize ?? 65536) * 2];
        action = (Action) (() =>
        {
          while (!decompressor.IsNeedingInput && !decompressor.IsFinished)
          {
            int num = decompressor.Inflate(decompressBuffer);
            if (num != 0)
              write(decompressBuffer, 0, num, cancelObject);
          }
        });
      }
      int num1;
      while ((num1 = srcStream.Read(numArray2, 0, numArray2.Length)) != 0)
      {
        rc4.Cipher(numArray2, 0, num1);
        if (decompressor != null)
        {
          decompressor.SetInput(numArray2, 0, num1);
          action();
        }
        else
          write(numArray2, 0, num1, cancelObject);
        if (cancel)
          return;
      }
      if (action != null)
        action();
      Inflater inflater = decompressor;
      if ((inflater != null ? (inflater.IsFinished ? 1 : 0) : 1) == 0)
        throw new IOException("Unexpected end of file on decompress.");
    }

    private static Backup.KeyState[] SavedKeys
    {
      get
      {
        if (Backup.savedKeys_ == null)
        {
          byte[] restoreKeys = Settings.RestoreKeys;
          if (restoreKeys == null)
            return (Backup.KeyState[]) null;
          Backup.savedKeys_ = JsonBase.Deserialize<Backup.KeyState[]>(restoreKeys, 0, restoreKeys.Length);
        }
        return Backup.savedKeys_;
      }
      set
      {
        MemoryStream memoryStream = new MemoryStream();
        new DataContractJsonSerializer(typeof (Backup.KeyState[])).WriteObject((Stream) memoryStream, (object) value);
        Settings.RestoreKeys = memoryStream.ToArray();
        Backup.savedKeys_ = value;
      }
    }

    public static void BindKeyState(Backup.KeyState ks)
    {
      Backup.KeyState[] storedKeys = Backup.SavedKeys;
      if (storedKeys == null)
        storedKeys = new Backup.KeyState[0];
      string key = ks.DictionaryKey;
      Backup.KeyState keyState = ((IEnumerable<Backup.KeyState>) storedKeys).Where<Backup.KeyState>((Func<Backup.KeyState, bool>) (k => key == k.DictionaryKey)).FirstOrDefault<Backup.KeyState>();
      if (keyState != null)
      {
        ks.Key = keyState.Key;
      }
      else
      {
        if (ks.KeyVersion == null)
          throw new NullReferenceException("KeyVersion");
        if (ks.AccountHash == null)
          throw new NullReferenceException("AccountHash");
        if (ks.Salt == null)
          throw new NullReferenceException("Salt");
        int num1 = 0;
        int num2 = 1000;
        for (int index = 0; index < 5; ++index)
        {
          using (ManualResetEvent ev = new ManualResetEvent(false))
          {
            int? code = new int?();
            AppState.GetConnection().ConnectedObservable().SelectMany<Unit, Backup.KeyState, Backup.KeyState>((Func<Unit, IObservable<Backup.KeyState>>) (connp => AppState.GetConnection().SendGetCipherKey(ks.KeyVersion, ks.AccountHash, ks.Salt)), (Func<Unit, Backup.KeyState, Backup.KeyState>) ((connp, d) => d)).Timeout<Backup.KeyState>(TimeSpan.FromMilliseconds((double) num2)).Take<Backup.KeyState>(1).Subscribe<Backup.KeyState>((Action<Backup.KeyState>) (ksFromServer =>
            {
              ks.Key = ksFromServer.Key;
              Backup.SavedKeys = ((IEnumerable<Backup.KeyState>) new Backup.KeyState[1]
              {
                ksFromServer
              }).Concat<Backup.KeyState>((IEnumerable<Backup.KeyState>) storedKeys).ToArray<Backup.KeyState>();
              ev.Set();
            }), (Action<Exception>) (ex =>
            {
              code = ex is IqException iqException2 ? iqException2.Code : new int?();
              ev.Set();
            }));
            ev.WaitOne();
            if (ks.Key == null)
            {
              int? nullable = code;
              int num3 = 408;
              if ((nullable.GetValueOrDefault() == num3 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
              {
                index = 0;
              }
              else
              {
                int num4 = num1 + num2;
                num1 = num2;
                num2 = num4;
              }
            }
            else
              break;
          }
        }
        if (ks.Key == null)
          throw new Exception("Could not determine key.");
      }
    }

    private static void DecryptFile(string filename, string outDir, bool legacy = false)
    {
      int num = filename.LastIndexOfAny(new char[2]
      {
        '\\',
        '/'
      });
      string filename1 = num < 0 ? filename : filename.Substring(num + 1);
      string str = outDir + "\\" + filename1;
      using (IMediaStorage mediaStorage1 = MediaStorage.Create(filename))
      {
        using (Stream srcStream = mediaStorage1.OpenFile(filename))
        {
          using (IMediaStorage mediaStorage2 = MediaStorage.Create(str))
          {
            using (Stream dstStream = mediaStorage2.OpenFile(str, FileMode.Create, FileAccess.ReadWrite))
            {
              if (legacy)
              {
                srcStream.CopyTo(dstStream);
              }
              else
              {
                Backup.DecryptFile(filename1, srcStream, (Action<byte[], int, int, Action>) ((buf, offset, len, cancel) => dstStream.Write(buf, offset, len)));
                dstStream.Flush();
              }
            }
          }
        }
      }
    }

    private static bool ValidateRestore(string dir)
    {
      using (Sqlite sqlite = new Sqlite(MediaStorage.GetAbsolutePath(dir + "\\messages.db"), SqliteOpenFlags.READONLY))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("SELECT * FROM Conversations"))
        {
          do
            ;
          while (preparedStatement.Step());
        }
      }
      return true;
    }

    public static void OnAppStarted(bool ignoreExceptions = true)
    {
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (!storeForApplication.DirectoryExists("restoreFiles"))
            return;
          foreach (string fileName in storeForApplication.GetFileNames("restoreFiles\\*"))
          {
            if (storeForApplication.FileExists(fileName))
              storeForApplication.DeleteFile(fileName);
            storeForApplication.MoveFile("restoreFiles\\" + fileName, fileName);
          }
          NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\restoreFiles", renameTarget: "tmp\\restoreCommit-" + DateTime.UtcNow.ToUnixTime().ToString());
        }
      }
      catch (Exception ex)
      {
        if (ignoreExceptions)
          return;
        throw;
      }
    }

    private static bool ValidForRestore(Backup.BackupInfo info)
    {
      if (info.Incomplete)
      {
        Log.l("backup", "{0} > not elligble; incomplete backup", (object) info.FullPath);
        return false;
      }
      if (info.CreationTime > DateTime.UtcNow)
      {
        Log.l("backup", "{0} > not elligble; backup is from the future ({1})", (object) info.FullPath, (object) info.CreationTime.ToString());
        return false;
      }
      if (!(info.CreationTime < DateTime.UtcNow.Add(-Backup.BackupValidity)))
        return true;
      Log.l("backup", "{0} > not elligble; backup is old ({1})", (object) info.FullPath, (object) info.CreationTime.ToString());
      return false;
    }

    private static bool ValidForPresentJid(Backup.BackupInfo info)
    {
      try
      {
        using (Stream stream = MediaStorage.OpenFile(info.FullPath + "\\messages.db"))
        {
          if (Backup.HasSqliteMagic(stream))
          {
            Log.l("backup", "{0} > Found legacy backup format", (object) info.FullPath);
            return true;
          }
          MemoryStream mem = new MemoryStream();
          Backup.DecryptFile("messages.db", stream, (Action<byte[], int, int, Action>) ((buffer, offset, len, cancel) =>
          {
            if (mem.Length < (long) Backup.SqliteMagic.Length)
              mem.Write(buffer, offset, len);
            if (mem.Length < (long) Backup.SqliteMagic.Length)
              return;
            cancel();
          }), new int?(32));
          mem.Position = 0L;
          if (!Backup.HasSqliteMagic((Stream) mem))
          {
            Log.l("backup", "{0} > Found unusable backup", (object) info.FullPath);
            return false;
          }
        }
      }
      catch (Exception ex)
      {
        string context = "hit error validating backup: " + info.FullPath;
        Log.LogException(ex, context);
        return false;
      }
      return true;
    }

    public static void DeleteAll()
    {
      string path = ((IEnumerable<string>) Backup.GetBackupDirs()).FirstOrDefault<string>();
      if (path == null)
        return;
      NativeInterfaces.Misc.RemoveDirectoryRecursive(path);
    }

    public static DateTime? GetLastBackupTime(bool validateCrypto = true)
    {
      return Backup.ListBackups().Where<Backup.BackupInfo>((Func<Backup.BackupInfo, bool>) (b =>
      {
        if (!Backup.ValidForRestore(b))
          return false;
        return !validateCrypto || Backup.ValidForPresentJid(b);
      })).Select<Backup.BackupInfo, DateTime?>((Func<Backup.BackupInfo, DateTime?>) (info => new DateTime?(info.CreationTime))).DefaultIfEmpty<DateTime?>(new DateTime?()).Max<DateTime?>();
    }

    public static Backup.BackupInfo? GetLastBackupInfo(bool validateCrypto = false)
    {
      return Backup.ListBackups().Where<Backup.BackupInfo>((Func<Backup.BackupInfo, bool>) (b =>
      {
        if (!Backup.ValidForRestore(b))
          return false;
        return !validateCrypto || Backup.ValidForPresentJid(b);
      })).Select<Backup.BackupInfo, Backup.BackupInfo?>((Func<Backup.BackupInfo, Backup.BackupInfo?>) (info => new Backup.BackupInfo?(info))).DefaultIfEmpty<Backup.BackupInfo?>(new Backup.BackupInfo?()).MaxOfFunc<Backup.BackupInfo?, DateTime>((Func<Backup.BackupInfo?, DateTime>) (b => !b.HasValue ? DateTime.MinValue : b.GetValueOrDefault().CreationTime));
    }

    public static bool CanBackup() => Backup.GetBackupDirs().Length != 0;

    private static bool IsCloudBackupInProgressUnderDirectory(string directory)
    {
      return !directory.StartsWith("D:", StringComparison.InvariantCultureIgnoreCase) && OneDriveBackupManager.Instance.IsDbBackupPathInUse(directory);
    }

    private static Action<int> SanitizeProgressCallback(Action<int> snap)
    {
      if (snap == null)
        return (Action<int>) null;
      int pct = 0;
      return (Action<int>) (newPct =>
      {
        newPct = Math.Min(100, newPct);
        if (newPct <= pct)
          return;
        pct = newPct;
        snap(pct);
      });
    }

    public struct BackupInfo
    {
      public string FullPath;
      public DateTime CreationTime;
      public bool Incomplete;
    }

    private abstract class DatabaseToBackup : IDisposable
    {
      public string Filename;
      public Action<Action> Synchronize;
      private long? fileSize;

      public long FileSize
      {
        get
        {
          if (this.fileSize.HasValue)
            return this.fileSize.Value;
          using (IMediaStorage mediaStorage = MediaStorage.Create(this.Filename))
          {
            using (Stream stream = mediaStorage.OpenFile(this.Filename))
            {
              long length = stream.Length;
              this.fileSize = new long?(length);
              return length;
            }
          }
        }
      }

      public void OpenDb(string targetDir)
      {
        this.Dispose();
        int num = this.Filename.LastIndexOfAny(new char[2]
        {
          '/',
          '\\'
        });
        string str = this.Filename;
        if (num >= 0)
          str = this.Filename.Substring(num + 1);
        string absolutePath = MediaStorage.GetAbsolutePath(targetDir + "\\" + str);
        this.OpenDbImpl(MediaStorage.GetAbsolutePath(this.Filename), absolutePath);
      }

      public abstract void OpenDbImpl(string srcFileName, string dstFileName);

      public abstract void Dispose();

      public abstract void Export(Action<long, int> onBytesWritten, CancellationToken? cancel);
    }

    private class BackupViaSqliteBackupApi : Backup.DatabaseToBackup
    {
      private Sqlite srcDb;
      private Sqlite dstDb;
      private ISqliteBackup backupEngine;

      public ISqliteBackup BackupEngine => this.backupEngine;

      private void OpenSrcDb()
      {
        (this.Synchronize ?? (Action<Action>) (a => a()))((Action) (() => this.srcDb = new Sqlite(MediaStorage.GetAbsolutePath(this.Filename), SqliteOpenFlags.READONLY)));
      }

      public override void OpenDbImpl(string srcFileName, string dstFileName)
      {
        (this.Synchronize ?? (Action<Action>) (a => a()))((Action) (() => this.srcDb = new Sqlite(srcFileName, SqliteOpenFlags.READONLY)));
        this.dstDb = new Sqlite(dstFileName, SqliteOpenFlags.Defaults, syncMode: new SqliteSynchronizeOptions?(SqliteSynchronizeOptions.None), wal: false);
        this.backupEngine = this.srcDb.Backup(this.dstDb);
        this.backupEngine.Step(0);
      }

      private void DisposeBackupEngine()
      {
        ((IDisposable) this.backupEngine).SafeDispose();
        this.backupEngine = (ISqliteBackup) null;
      }

      public override void Dispose()
      {
        this.DisposeBackupEngine();
        this.srcDb.SafeDispose();
        this.dstDb.SafeDispose();
        this.srcDb = this.dstDb = (Sqlite) null;
      }

      public override void Export(Action<long, int> onBytesWritten, CancellationToken? cancel)
      {
        int num1 = 0;
        bool flag;
        do
        {
          cancel?.ThrowIfCancellationRequested();
          flag = this.BackupEngine.Step(Math.Min(Math.Max(1, this.BackupEngine.GetTotalPages() / 100), this.BackupEngine.GetRemainingPages()));
          int totalPages = this.BackupEngine.GetTotalPages();
          int num2 = totalPages - this.BackupEngine.GetRemainingPages();
          int num3 = totalPages == 0 ? 0 : num2 * 100 / totalPages;
          if (num3 > num1)
          {
            int num4 = num3 - num1;
            onBytesWritten(this.FileSize * (long) num4 / 100L, num1);
            num1 = num3;
          }
        }
        while (!flag);
        this.DisposeBackupEngine();
      }
    }

    private class BackupViaFile : Backup.DatabaseToBackup
    {
      private string srcFileName;
      private string dstFileName;

      public override void OpenDbImpl(string srcFileName, string dstFileName)
      {
        this.srcFileName = srcFileName;
        this.dstFileName = dstFileName;
      }

      public override void Export(Action<long, int> onBytesWritten, CancellationToken? cancel)
      {
        long written = 0;
        long pendingProgress = 0;
        int pct = 0;
        (this.Synchronize ?? (Action<Action>) (a => a()))((Action) (() =>
        {
          using (Stream stream1 = MediaStorage.OpenFile(this.srcFileName))
          {
            using (Stream stream2 = MediaStorage.OpenFile(this.dstFileName, FileMode.Create, FileAccess.ReadWrite))
            {
              long fileSize = this.FileSize;
              byte[] buffer = new byte[16384];
              int count;
              while ((count = stream1.Read(buffer, 0, buffer.Length)) > 0)
              {
                stream2.Write(buffer, 0, count);
                written += (long) count;
                pendingProgress += (long) count;
                int num = (int) (written * 100L / fileSize);
                if (num != pct)
                {
                  onBytesWritten(pendingProgress, num);
                  pct = num;
                  pendingProgress = 0L;
                  ref CancellationToken? local = ref cancel;
                  if (local.HasValue)
                    local.GetValueOrDefault().ThrowIfCancellationRequested();
                }
              }
            }
          }
        }));
        if (pendingProgress == 0L)
          return;
        onBytesWritten(pendingProgress, 100);
      }

      public override void Dispose()
      {
      }
    }

    private struct BackupHeader
    {
      public int Version;
      public int OffsetToPayload;
    }

    [DataContract]
    public class KeyState : JsonBase
    {
      [IgnoreDataMember]
      public byte[] Salt
      {
        get => this.FromBase64(this.SaltString);
        set => this.SaltString = this.ToBase64(value);
      }

      [IgnoreDataMember]
      public byte[] AccountHash
      {
        get => this.FromBase64(this.AccountHashString);
        set => this.AccountHashString = this.ToBase64(value);
      }

      [IgnoreDataMember]
      public byte[] Key
      {
        get => this.FromBase64(this.KeyString);
        set => this.KeyString = this.ToBase64(value);
      }

      [IgnoreDataMember]
      public byte[] LocalSalt
      {
        get => this.FromBase64(this.LocalSaltString);
        set => this.LocalSaltString = this.ToBase64(value);
      }

      [DataMember(Name = "v")]
      public string KeyVersion { get; set; }

      [DataMember(Name = "s")]
      public string SaltString { get; set; }

      [DataMember(Name = "ah")]
      public string AccountHashString { get; set; }

      [DataMember(Name = "p")]
      public string KeyString { get; set; }

      [DataMember(Name = "ls")]
      public string LocalSaltString { get; set; }

      public string DictionaryKey
      {
        get
        {
          return string.Format("{0}:{1}:{2}", (object) this.AccountHashString, (object) this.SaltString, (object) this.KeyVersion);
        }
      }

      private byte[] FromBase64(string b64)
      {
        return b64 != null ? Convert.FromBase64String(b64) : (byte[]) null;
      }

      private string ToBase64(byte[] b) => b != null ? Convert.ToBase64String(b) : (string) null;

      public byte[] SerializeWithoutKey()
      {
        byte[] key = this.Key;
        this.Key = (byte[]) null;
        try
        {
          return this.Serialize();
        }
        finally
        {
          this.Key = key;
        }
      }

      public void SerializeWithoutKey(Stream output)
      {
        byte[] key = this.Key;
        this.Key = (byte[]) null;
        try
        {
          this.Serialize(output);
        }
        finally
        {
          this.Key = key;
        }
      }
    }

    private interface IWriteCallback
    {
      void OpenFile(string str);

      void Write(byte[] buffer, int offset, int length, int progressLen = 0);

      void Discard();

      void Close();

      void Commit();
    }

    private class SimpleWriteCallback : Backup.IWriteCallback
    {
      private string dir;
      private Stream currentStream;
      private Action<long> progressFunc;
      private long currentProgress;

      public SimpleWriteCallback(string dir, Action<long> onBytesWritten = null)
      {
        dir += "_beta";
        using (NativeMediaStorage stg = new NativeMediaStorage())
        {
          stg.RemoveDirectoryRecursive(dir, true, true);
          stg.CreateDirectory(dir);
        }
        this.dir = dir;
        this.progressFunc = onBytesWritten;
      }

      public void OpenFile(string relativePath)
      {
        this.Close();
        string str = this.dir + "\\" + relativePath;
        using (IMediaStorage mediaStorage = MediaStorage.Create(str))
          this.currentStream = mediaStorage.OpenFile(str, FileMode.Create, FileAccess.ReadWrite);
      }

      public void Write(byte[] buffer, int offset, int len, int progLen = 0)
      {
        if (this.currentStream == null)
          throw new InvalidOperationException("stream not initialized");
        this.currentStream.Write(buffer, offset, len);
        if (progLen == 0)
          return;
        Action<long> progressFunc = this.progressFunc;
        if (progressFunc == null)
          return;
        progressFunc(this.currentProgress += (long) progLen);
      }

      public void Close()
      {
        this.currentStream.SafeDispose();
        this.currentStream = (Stream) null;
      }

      public void Discard()
      {
      }

      public void Commit()
      {
        string dir = this.dir;
        int length = dir.LastIndexOfAny(new char[2]
        {
          '\\',
          '/'
        });
        string str = Backup.Commit(dir.Substring(0, length), dir);
        Log.l("backup", "[{0}] saved", (object) str);
        this.FinalDbPath = str;
      }

      public string FinalDbPath { get; private set; }
    }

    private class ParallelWriteCallback : Backup.IWriteCallback
    {
      private Backup.ParallelWriteCallback.InnerCallback[] callbacks;
      private bool discarded;
      private List<Pair<Exception, Action>> workerExceptions = new List<Pair<Exception, Action>>();
      private object exceptionLock = new object();

      public ParallelWriteCallback(params Func<Backup.IWriteCallback>[] callbackCtors)
      {
        Func<Func<Backup.IWriteCallback>, Func<Backup.IWriteCallback>> map = (Func<Func<Backup.IWriteCallback>, Func<Backup.IWriteCallback>>) (inFn => (Func<Backup.IWriteCallback>) (() =>
        {
          try
          {
            return inFn();
          }
          catch (Exception ex)
          {
            this.OnWorkerException(ex);
            throw;
          }
        }));
        this.callbacks = ((IEnumerable<Func<Backup.IWriteCallback>>) callbackCtors).Select<Func<Backup.IWriteCallback>, Backup.ParallelWriteCallback.InnerCallback>((Func<Func<Backup.IWriteCallback>, Backup.ParallelWriteCallback.InnerCallback>) (ctor => new Backup.ParallelWriteCallback.InnerCallback(map(ctor)))).ToArray<Backup.ParallelWriteCallback.InnerCallback>();
      }

      private void OnWorkerException(Exception ex)
      {
        lock (this.exceptionLock)
          this.workerExceptions.Add(new Pair<Exception, Action>(ex, ex.GetRethrowAction()));
      }

      public void CheckWorkerExceptions()
      {
        Action action = (Action) null;
        lock (this.exceptionLock)
        {
          switch (this.workerExceptions.Count)
          {
            case 0:
              this.workerExceptions.Clear();
              break;
            case 1:
              action = this.workerExceptions[0].Second;
              goto case 0;
            default:
              AggregateException ex = new AggregateException(this.workerExceptions.Select<Pair<Exception, Action>, Exception>((Func<Pair<Exception, Action>, Exception>) (a => a.First)));
              action = (Action) (() =>
              {
                throw ex;
              });
              goto case 0;
          }
        }
        if (action == null)
          return;
        action();
      }

      private void Enqueue(
        Backup.ParallelWriteCallback.InnerCallback callback,
        Action<Backup.IWriteCallback> op)
      {
        callback.Queue.Enqueue((Action) (() =>
        {
          try
          {
            op(callback.Writer);
          }
          catch (Exception ex)
          {
            this.OnWorkerException(ex);
          }
        }));
      }

      private void ForEach(Action<Backup.IWriteCallback> op)
      {
        this.CheckWorkerExceptions();
        foreach (Backup.ParallelWriteCallback.InnerCallback callback in this.callbacks)
          this.Enqueue(callback, op);
        this.CheckWorkerExceptions();
      }

      public void Schedule(int index, Action<Backup.IWriteCallback> op)
      {
        this.Enqueue(this.callbacks[index], op);
      }

      public void OpenFile(string relativePath)
      {
        this.ForEach((Action<Backup.IWriteCallback>) (writer => writer.OpenFile(relativePath)));
      }

      public void Write(byte[] buffer, int offset, int len, int progLen = 0)
      {
        byte[] copy = new byte[len];
        Array.Copy((Array) buffer, offset, (Array) copy, 0, len);
        this.ForEach((Action<Backup.IWriteCallback>) (writer => writer.Write(copy, 0, copy.Length, progLen)));
      }

      private void StopThreads(bool discard)
      {
        List<ManualResetEvent> manualResetEventList = new List<ManualResetEvent>();
        foreach (Backup.ParallelWriteCallback.InnerCallback callback in this.callbacks)
        {
          ManualResetEvent ev = new ManualResetEvent(false);
          Backup.ParallelWriteCallback.InnerCallback snap = callback;
          callback.Queue.Stop(discard ? WorkQueue.Priority.Interrupt : WorkQueue.Priority.Normal, (Action) (() =>
          {
            try
            {
              if (discard)
                snap.Writer.Discard();
            }
            catch (Exception ex)
            {
              this.OnWorkerException(ex);
              discard = true;
            }
            finally
            {
              try
              {
                snap.Writer.Close();
                if (!discard)
                  snap.Writer.Commit();
              }
              catch (Exception ex)
              {
                this.OnWorkerException(ex);
              }
            }
            ev.Set();
          }));
          manualResetEventList.Add(ev);
        }
        foreach (ManualResetEvent d in manualResetEventList)
        {
          d.WaitOne();
          d.SafeDispose();
        }
        this.CheckWorkerExceptions();
      }

      public void Discard()
      {
        this.StopThreads(true);
        this.discarded = true;
      }

      public void Close()
      {
        if (this.discarded)
          return;
        this.StopThreads(false);
      }

      public void Commit()
      {
      }

      private class InnerCallback
      {
        public WorkQueue Queue;
        public Backup.IWriteCallback Writer;

        public InnerCallback(Func<Backup.IWriteCallback> ctor)
        {
          Backup.ParallelWriteCallback.InnerCallback innerCallback = this;
          this.Queue = new WorkQueue(flags: WorkQueue.StartFlags.Unpausable | WorkQueue.StartFlags.WatchdogExcempt);
          this.Queue.Enqueue((Action) (() => innerCallback.Writer = ctor()));
        }
      }
    }
  }
}
