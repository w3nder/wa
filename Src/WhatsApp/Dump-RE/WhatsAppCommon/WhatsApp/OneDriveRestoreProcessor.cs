// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveRestoreProcessor
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using WhatsAppNative;
using Windows.Security.Authentication.OnlineId;
using Windows.Storage;
using Windows.Storage.FileProperties;

#nullable disable
namespace WhatsApp
{
  public class OneDriveRestoreProcessor : OneDriveProcessor
  {
    private static readonly string RestoreTmpDir = "onedriveTmp";
    public static readonly string RestoreTmpPath = Constants.IsoStorePath + "\\" + OneDriveRestoreProcessor.RestoreTmpDir;
    public const int RESTORE_SELECTION_COUNT = 10;

    public void ClearTemporaryData()
    {
      Log.l("onedrive", "clearing temporary restore data");
      NativeInterfaces.Misc.RemoveDirectoryRecursive(OneDriveRestoreProcessor.RestoreTmpPath, true);
    }

    public async Task<Item> FindRemoteBackup(CancellationToken cancellationToken)
    {
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "checking for remote backups");
      Item obj1 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(this.chatId).Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
      List<Item> source = new List<Item>();
      foreach (Item child in (IEnumerable<Item>) obj1.Children)
      {
        if (child.Name.StartsWith("db_", StringComparison.InvariantCultureIgnoreCase))
        {
          if (child.Name.Length > 3 && long.TryParse(child.Name.Substring(3), out long _))
            source.Add(child);
          Log.l("onedrive", string.Format("Backup subfolder: Id={0}, Name={1}", (object) child.Id, (object) child.Name));
        }
        else if (child.Name.Equals("media", StringComparison.InvariantCultureIgnoreCase))
          Log.l("onedrive", string.Format("Backup subfolder: Id={0}, Name={1}", (object) child.Id, (object) child.Name));
      }
      List<Item> list = source.OrderByDescending<Item, long>((Func<Item, long>) (item => long.Parse(item.Name.Substring(3)))).ToList<Item>();
      cancellationToken.ThrowIfCancellationRequested();
      foreach (Item obj2 in list)
      {
        Item item = obj2;
        Item obj3 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Items[item.Id].Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
        bool flag1 = false;
        bool flag2 = false;
        foreach (Item child in (IEnumerable<Item>) obj3.Children)
        {
          if (child.Name.Equals("messages.db"))
            flag1 = true;
          else if (child.Name.Equals("onedrive_manifest.db"))
            flag2 = true;
          if (flag1 & flag2)
            break;
        }
        if (flag1 & flag2)
        {
          Log.l("onedrive", string.Format("Backup selected: Id={0}, Name={1}", (object) item.Id, (object) item.Name));
          return item;
        }
      }
      return (Item) null;
    }

    public async Task<OneDriveManifest> GetRemoteBackupManifest(
      Item backupItem,
      CancellationToken cancellationToken)
    {
      Log.l("onedrive", "getting remote backup manifest");
      string dbFile = OneDriveRestoreProcessor.RestoreTmpPath + "\\onedrive_manifest.db";
      this.ClearTemporaryData();
      using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
        storeMediaStorage.CreateDirectory(OneDriveRestoreProcessor.RestoreTmpDir);
      using (Stream stream = await this.OneDriveRun<Stream>((Func<Task<Stream>>) (async () => await this.oneDriveClient.Drive.Items[backupItem.Id].ItemWithPath("onedrive_manifest.db").Content.Request().GetAsync(cancellationToken)), cancellationToken))
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(dbFile))
        {
          using (Stream destination = mediaStorage.OpenFile(dbFile, FileMode.Create, FileAccess.Write))
            stream.CopyTo(destination);
        }
      }
      return new OneDriveManifest(dbFile);
    }

    public static OneDriveManifest RemoteBackupManifest()
    {
      OneDriveManifest oneDriveManifest = new OneDriveManifest(OneDriveRestoreProcessor.RestoreTmpPath + "\\onedrive_manifest.db");
      return !oneDriveManifest.Exists ? (OneDriveManifest) null : oneDriveManifest;
    }

    public static bool HasRemoteBackup()
    {
      bool flag = false;
      try
      {
        using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
        {
          if (oneDriveManifest != null)
          {
            BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
            if (backupProperties != null)
            {
              if (!string.IsNullOrEmpty(backupProperties.BackupId))
                flag = true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.l("onedrive", "error checking for downloaded remote backup manifest: " + ex.ToString());
      }
      return flag;
    }

    public long DatabaseRestoreSize(OneDriveManifest manifest)
    {
      long num = 0;
      foreach (RemoteDatabaseFile remoteDatabaseFile in manifest.GetRemoteDatabaseFiles())
        num += remoteDatabaseFile.Size;
      return num;
    }

    public async Task<MediaRestoreEstimate> EstimateMediaRestoreSize(
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      List<RemoteMediaFile> fileList = manifest.RemoteMediaFilesToRestore(0);
      Log.l("onedrive", "manifest has {0} files to check", (object) fileList.Count);
      cancellationToken.ThrowIfCancellationRequested();
      string path = (await KnownFolders.PicturesLibrary.CreateFolderAsync("WhatsApp", (CreationCollisionOption) 3)).Path;
      int num1 = 0;
      int num2 = 0;
      long num3 = 0;
      long num4 = 0;
      long num5 = 0;
      foreach (RemoteMediaFile fileEntry in fileList)
      {
        bool flag = false;
        FileRef fileReference = fileEntry.FileReference;
        string str;
        if (fileReference.Root == FileRoot.PhoneStorageWhatsAppMedia || fileReference.Root == FileRoot.SdCardWhatsAppMedia)
        {
          if (!string.IsNullOrEmpty(fileReference.Subdir))
            str = Path.Combine(path, fileReference.Subdir, fileReference.FilePart);
          else
            str = Path.Combine(path, fileReference.FilePart);
        }
        else
          str = fileReference.ToAbsolutePath();
        using (IMediaStorage mediaStorage = MediaStorage.Create(str))
        {
          if (mediaStorage.FileExists(str))
          {
            using (Stream stream = mediaStorage.OpenFile(str))
            {
              if (stream.Length == fileEntry.Size)
                flag = true;
              else
                Log.l("onedrive", "media file size mismatch: {0} -> {1} != {2}", (object) fileEntry.RemoteFileId, (object) stream.Length, (object) fileEntry.Size);
            }
          }
        }
        if (!flag && OneDriveRestoreProcessor.FileDestinationValid(fileEntry))
        {
          ++num2;
          if (fileEntry.Size > num3)
            num3 = fileEntry.Size;
          if (fileEntry.FileReference.Root == FileRoot.IsoStore)
            num4 += fileEntry.Size;
          else
            num5 += fileEntry.Size;
        }
        ++num1;
        cancellationToken.ThrowIfCancellationRequested();
        if (num1 % 100 == 0)
          Log.l("onedrive", "checked {0} of {1}", (object) num1, (object) fileList.Count);
      }
      MediaRestoreEstimate mediaRestoreEstimate = new MediaRestoreEstimate();
      mediaRestoreEstimate.SizeOnDevice = num3 + num4;
      if (path.StartsWith("D:", StringComparison.InvariantCultureIgnoreCase))
        mediaRestoreEstimate.SizeOnSdCard = num5;
      else
        mediaRestoreEstimate.SizeOnDevice += num5;
      Log.l("onedrive", "estimated {0} files for restore, needing: Device={1} bytes, SdCard={2} bytes", (object) num2, (object) mediaRestoreEstimate.SizeOnDevice, (object) mediaRestoreEstimate.SizeOnSdCard);
      manifest.UpdateRestoreEstimates(mediaRestoreEstimate.Total(), fileList.Count);
      return mediaRestoreEstimate;
    }

    public async Task RestoreRemoteDatabases(
      OneDriveManifest manifest,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      Log.l("onedrive", "restoring remote databases");
      IEnumerable<RemoteDatabaseFile> remoteDatabaseFiles = manifest.GetRemoteDatabaseFiles();
      byte[] buf = new byte[32768];
      long progressBase = 0;
      foreach (RemoteDatabaseFile remoteDatabaseFile in remoteDatabaseFiles)
      {
        RemoteDatabaseFile dbFile = remoteDatabaseFile;
        Log.l("onedrive", "downloading remote DB file: Name={0}, Size={1}, Id={2}", (object) dbFile.FileName, (object) dbFile.Size, (object) dbFile.RemoteFileId);
        string fileName = OneDriveRestoreProcessor.RestoreTmpPath + "\\" + dbFile.FileName;
        int num1 = await this.OneDriveRun<int>((Func<Task<int>>) (async () =>
        {
          long progressValue = 0;
          using (Stream stream = await this.oneDriveClient.Drive.Items[dbFile.RemoteFileId].Content.Request().GetAsync(cancellationToken))
          {
            IMediaStorage fs;
            int num2;
            if (num2 != 1 && num2 != 2)
              fs = MediaStorage.Create(fileName);
            try
            {
              Stream file;
              if (num2 != 1 && num2 != 2)
                file = fs.OpenFile(fileName, FileMode.Create, FileAccess.Write);
              try
              {
                int n = 0;
                do
                {
                  n = await stream.ReadAsync(buf, 0, buf.Length, cancellationToken);
                  if (n > 0)
                  {
                    await file.WriteAsync(buf, 0, n, cancellationToken);
                    progressValue += (long) n;
                    progress?.Report(progressBase + progressValue);
                  }
                }
                while (n > 0);
              }
              finally
              {
                file?.Dispose();
              }
              file = (Stream) null;
            }
            finally
            {
              fs?.Dispose();
            }
            fs = (IMediaStorage) null;
          }
          progressBase += progressValue;
          return 1;
        }), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        byte[] hash;
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          using (Stream inputStream = nativeMediaStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
          {
            using (SHA1Managed shA1Managed = new SHA1Managed())
              hash = shA1Managed.ComputeHash(inputStream);
          }
        }
        if (!dbFile.Sha1Hash.IsEqualBytes(hash))
        {
          Log.l("onedrive", "DB file hash mismatch!");
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(fileName);
          if (dbFile.FileName.Equals("messages.db", StringComparison.InvariantCultureIgnoreCase))
            throw new Exception("Cannot restore backup with corrupted message DB");
        }
        else
          Log.l("onedrive", "DB file hash verified");
      }
    }

    public async Task RestoreMedia(
      OneDriveManifest manifest,
      OneDriveBkupRestTrigger restoreTrigger,
      CancellationToken cancellationToken,
      IProgress<long> progress)
    {
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "performing media restore");
      int restored = 0;
      int existing = 0;
      int downloadfailed = 0;
      bool saveOverlapOk = AppState.IsDecentMemoryDevice && restoreTrigger != OneDriveBkupRestTrigger.BackgroundAgentShort;
      Stats.LogMemoryUsage(gc: true);
      DateTime now = DateTime.Now;
      long restoreStartTime = now.Ticks;
      List<RemoteMediaFile> fileList;
      while ((fileList = manifest.RemoteMediaFilesToRestore(10)) != null && fileList.Count > 0)
      {
        Log.l("onedrive", "manifest has {0} (or more) files to restore", (object) fileList.Count);
        if (AppState.BatterySaverEnabled)
        {
          Log.l("onedrive", "media restore insufficient battery {0}", (object) AppState.BatteryPercentage);
          throw new OneDriveBatterySaverStatexception(string.Format("Battery Saver State stopping OneDrive restore"));
        }
        Task saveTask = (Task) null;
        Exception saveException = (Exception) null;
        long saveTimeTicks = 0;
        long saveStallTimeTicks = 0;
        now = DateTime.Now;
        long saveLoopStartTimeTicks = now.Ticks;
        Action<bool> saveWait = (Action<bool>) (rethrowException =>
        {
          Log.d("onedrive", "media restore waiting");
          long ticks = DateTime.Now.Ticks;
          try
          {
            saveTask.Wait(cancellationToken);
          }
          catch (OperationCanceledException ex)
          {
            Log.l("onedrive", "media restore wait cancelled");
            if (saveException != null)
              saveException = (Exception) ex;
          }
          saveStallTimeTicks += DateTime.Now.Ticks - ticks;
          Log.d("onedrive", "media restore waited");
          saveTask = (Task) null;
          if (saveException == null)
            return;
          Log.l("onedrive", "media restore save exception {0}, throwing {1}", (object) saveException.ToString(), (object) rethrowException);
          if (rethrowException)
          {
            cancellationToken.ThrowIfCancellationRequested();
            throw saveException;
          }
        });
        bool saveExceptionShouldBeRethrown = true;
        int saveCount = 0;
        long progressBase = 0;
        try
        {
          for (int index = 0; index < fileList.Count; ++index)
          {
            cancellationToken.ThrowIfCancellationRequested();
            RemoteMediaFile fileEntry = fileList[index];
            Log.l("onedrive", "media file {0}/{1}, {2} -> {3}", (object) (index + 1), (object) fileList.Count, (object) fileEntry.RemoteFileId, (object) fileEntry.FileReference.ToString());
            if (await this.CheckMediaFileExists(fileEntry) || !OneDriveRestoreProcessor.FileDestinationValid(fileEntry))
            {
              manifest.RemoveDownloadedMediaFile(fileEntry);
              progress.Report(progressBase + fileEntry.Size);
              progressBase += fileEntry.Size;
              ++existing;
            }
            else
            {
              cancellationToken.ThrowIfCancellationRequested();
              string downloadTmpPath = OneDriveRestoreProcessor.CreateDownloadTmpPath(fileEntry);
              DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
              if (diskSpace.FreeBytes < (ulong) fileEntry.Size + 15UL)
              {
                Log.l("onedrive", "media restore insufficient available space for {0}", (object) fileEntry.FileReference.FilePart);
                throw new OneDriveDiskFullException(string.Format("Not enough free space to download file: {0} < {1} + {2}", (object) diskSpace.FreeBytes, (object) fileEntry.Size, (object) 15));
              }
              cancellationToken.ThrowIfCancellationRequested();
              long downloadProgressValue = 0;
              if (!await this.DownloadMediaFile(fileEntry, downloadTmpPath, cancellationToken, (IProgress<long>) new Progress<long>((Action<long>) (p =>
              {
                progress?.Report(progressBase + p);
                downloadProgressValue = p;
              }))))
              {
                Log.l("onedrive", "media restore download failed for {0}", (object) fileEntry.FileReference.FilePart);
                ++downloadfailed;
                manifest.RemoveDownloadedMediaFile(fileEntry);
              }
              else
              {
                progressBase += downloadProgressValue;
                if (saveOverlapOk)
                {
                  if (saveTask != null)
                    saveWait(true);
                  ++saveCount;
                  saveTask = Task.Run((Func<Task>) (async () =>
                  {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                      Log.d("onedrive", "media restore saving");
                      long saveStartTicks = DateTime.Now.Ticks;
                      await this.SaveMediaFile(fileEntry, downloadTmpPath, manifest, cancellationToken);
                      manifest.RemoveDownloadedMediaFile(fileEntry);
                      ++restored;
                      saveTimeTicks += DateTime.Now.Ticks - saveStartTicks;
                      Log.d("onedrive", "media restore saved");
                    }
                    catch (Exception ex)
                    {
                      Log.LogException(ex, "onedrive media restore exception saving media");
                      saveException = ex;
                    }
                  }), cancellationToken);
                }
                else
                {
                  cancellationToken.ThrowIfCancellationRequested();
                  await this.SaveMediaFile(fileEntry, downloadTmpPath, manifest, cancellationToken);
                  manifest.RemoveDownloadedMediaFile(fileEntry);
                  ++restored;
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          saveExceptionShouldBeRethrown = false;
          Log.l("onedrive", "media restore exception");
          throw;
        }
        finally
        {
          try
          {
            if (saveOverlapOk)
            {
              if (saveCount > 0)
              {
                if (saveTask != null)
                  saveWait(saveExceptionShouldBeRethrown);
                Log.l("onedrive", "media restore overlap times for {0} entries: save {1}ms, waited {2}ms, total {3}ms", (object) saveCount, (object) (saveTimeTicks / 10000L), (object) (saveStallTimeTicks / 10000L), (object) ((DateTime.Now.Ticks - saveLoopStartTimeTicks) / 10000L));
                if (!cancellationToken.IsCancellationRequested)
                  Stats.LogMemoryUsage(gc: true);
              }
            }
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "Exception tidying overlapped restores");
          }
          Log.l("onedrive", "media restore progress: restored={0}, existing={1}, downloadfailed={2}, took={3}ms", (object) restored, (object) existing, (object) downloadfailed, (object) ((DateTime.Now.Ticks - restoreStartTime) / 10000L));
        }
        saveWait = (Action<bool>) null;
      }
      object[] objArray = new object[4]
      {
        (object) restored,
        (object) existing,
        (object) downloadfailed,
        null
      };
      now = DateTime.Now;
      objArray[3] = (object) ((now.Ticks - restoreStartTime) / 10000L);
      Log.l("onedrive", "media restore complete: restored={0}, existing={1}, downloadfailed={2}, took={3}ms", objArray);
    }

    private static bool FileDestinationValid(RemoteMediaFile fileEntry)
    {
      switch (fileEntry.FileReference.Root)
      {
        case FileRoot.IsoStore:
        case FileRoot.PhoneStorageWhatsAppMedia:
        case FileRoot.SdCardWhatsAppMedia:
          return true;
        default:
          return false;
      }
    }

    private async Task<bool> CheckMediaFileExists(RemoteMediaFile fileEntry)
    {
      string str = await this.MediaDestinationAbsolutePath(fileEntry.FileReference);
      using (IMediaStorage mediaStorage = MediaStorage.Create(str))
      {
        if (mediaStorage.FileExists(str))
        {
          using (Stream inputStream = mediaStorage.OpenFile(str))
          {
            if (inputStream.Length == fileEntry.Size)
            {
              using (SHA1Managed shA1Managed = new SHA1Managed())
              {
                if (shA1Managed.ComputeHash(inputStream).IsEqualBytes(fileEntry.Sha1Hash))
                {
                  Log.l("onedrive", "media file exists: {0} -> {1}", (object) fileEntry.RemoteFileId, (object) str);
                  return true;
                }
                Log.l("onedrive", "media file hash mismatch: {0}", (object) fileEntry.RemoteFileId);
              }
            }
            else
              Log.l("onedrive", "media file exists size mismatch: {0} -> {1} != {2}", (object) fileEntry.RemoteFileId, (object) inputStream.Length, (object) fileEntry.Size);
          }
        }
      }
      return false;
    }

    private async Task<string> MediaDestinationAbsolutePath(FileRef fileRef)
    {
      string str;
      if (fileRef.Root == FileRoot.PhoneStorageWhatsAppMedia || fileRef.Root == FileRoot.SdCardWhatsAppMedia)
      {
        StorageFolder folderAsync = await KnownFolders.PicturesLibrary.CreateFolderAsync("WhatsApp", (CreationCollisionOption) 3);
        if (!string.IsNullOrEmpty(fileRef.Subdir))
          str = Path.Combine(folderAsync.Path, fileRef.Subdir, fileRef.FilePart);
        else
          str = Path.Combine(folderAsync.Path, fileRef.FilePart);
      }
      else
        str = fileRef.ToAbsolutePath();
      return str;
    }

    private async Task<bool> DownloadMediaFile(
      RemoteMediaFile fileEntry,
      string downloadTmpPath,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      using (NativeMediaStorage fs = new NativeMediaStorage())
      {
        if (fs.FileExists(downloadTmpPath))
        {
          using (Stream inputStream = fs.OpenFile(downloadTmpPath, FileMode.Open, FileAccess.Read))
          {
            if (inputStream.Length == fileEntry.Size)
            {
              using (SHA1Managed shA1Managed = new SHA1Managed())
              {
                if (shA1Managed.ComputeHash(inputStream).IsEqualBytes(fileEntry.Sha1Hash))
                {
                  Log.l("onedrive", "media file already downloaded to tmp path: {0}", (object) downloadTmpPath);
                  return true;
                }
              }
            }
          }
          Log.l("onedrive", "deleting old download tmp file: {0}", (object) downloadTmpPath);
          fs.DeleteFile(downloadTmpPath);
        }
        byte[] buf = new byte[32768];
        int num1 = await this.OneDriveRun<bool>((Func<Task<bool>>) (async () =>
        {
          try
          {
            long progressValue = 0;
            using (Stream stream = await this.oneDriveClient.Drive.Items[fileEntry.RemoteFileId].Content.Request().GetAsync(cancellationToken))
            {
              Stream file;
              int num2;
              if (num2 != 1 && num2 != 2)
                file = fs.OpenFile(downloadTmpPath, FileMode.Create, FileAccess.Write);
              try
              {
                int n = 0;
                do
                {
                  cancellationToken.ThrowIfCancellationRequested();
                  n = await stream.ReadAsync(buf, 0, buf.Length, cancellationToken);
                  if (n > 0)
                  {
                    await file.WriteAsync(buf, 0, n, cancellationToken);
                    progressValue += (long) n;
                    progress?.Report(progressValue);
                  }
                }
                while (n > 0);
              }
              finally
              {
                file?.Dispose();
              }
              file = (Stream) null;
            }
          }
          catch (ServiceException ex)
          {
            if (ex.IsMatch("itemNotFound"))
            {
              Log.l("onedrive", "Media file not found! {0} -> {1}", (object) fileEntry.RemoteFileId, (object) fileEntry.FileReference.FilePart);
              return false;
            }
            throw;
          }
          return true;
        }), cancellationToken) ? 1 : 0;
        cancellationToken.ThrowIfCancellationRequested();
        if (num1 != 0)
        {
          byte[] hash;
          using (Stream inputStream = fs.OpenFile(downloadTmpPath, FileMode.Open, FileAccess.Read))
          {
            using (SHA1Managed shA1Managed = new SHA1Managed())
              hash = shA1Managed.ComputeHash(inputStream);
          }
          if (!fileEntry.Sha1Hash.IsEqualBytes(hash))
          {
            Log.l("onedrive", "Media file hash mismatch! {0} -> {1}", (object) fileEntry.RemoteFileId, (object) fileEntry.FileReference.FilePart);
            fs.DeleteFile(downloadTmpPath);
            return false;
          }
          Log.l("onedrive", "Media file hash verified: {0} -> {1}", (object) fileEntry.RemoteFileId, (object) fileEntry.FileReference.FilePart);
          return true;
        }
        try
        {
          if (fs.FileExists(downloadTmpPath))
            fs.DeleteFile(downloadTmpPath);
        }
        catch (Exception ex)
        {
        }
        return false;
      }
    }

    private async Task SaveMediaFile(
      RemoteMediaFile fileEntry,
      string downloadTmpPath,
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      string destPath = fileEntry.FileReference.ToAbsolutePath();
      FileRef fileRef = fileEntry.FileReference;
      if (fileRef.Root == FileRoot.PhoneStorageWhatsAppMedia || fileRef.Root == FileRoot.SdCardWhatsAppMedia)
        await this.SaveMediaToStorageFolder(fileEntry, downloadTmpPath, manifest, cancellationToken);
      else if (fileRef.Root == FileRoot.IsoStore)
        await this.SaveMediaToIsoStore(fileEntry, downloadTmpPath, cancellationToken);
      else
        Log.l("onedrive", "skipping media for unsupported location: {0}", (object) destPath);
      using (IMediaStorage mediaStorage = MediaStorage.Create(downloadTmpPath))
        mediaStorage.DeleteFile(downloadTmpPath);
    }

    private async Task SaveMediaToStorageFolder(
      RemoteMediaFile fileEntry,
      string downloadTmpPath,
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      StorageFolder destLib = KnownFolders.PicturesLibrary;
      destLib = await destLib.CreateFolderAsync("WhatsApp", (CreationCollisionOption) 3);
      string subdir = fileEntry.FileReference.Subdir;
      if (!string.IsNullOrEmpty(subdir))
      {
        string[] strArray = subdir.Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
          destLib = await destLib.CreateFolderAsync(strArray[index], (CreationCollisionOption) 3);
        strArray = (string[]) null;
      }
      StorageFile srcFile = await StorageFile.GetFileFromPathAsync(downloadTmpPath);
      string destName = fileEntry.FileReference.FilePart;
      BasicProperties basicPropertiesAsync = await srcFile.GetBasicPropertiesAsync();
      DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(destLib.Path.StartsWith("C:", StringComparison.InvariantCultureIgnoreCase) ? "C:" : "D:");
      if (diskSpace.FreeBytes < basicPropertiesAsync.Size + 15UL)
        throw new OneDriveDiskFullException(string.Format("Not enough free space to save file: {0} < {1} + {2}", (object) diskSpace.FreeBytes, (object) basicPropertiesAsync.Size, (object) 15));
      bool tryNewName = true;
      StorageFile fileAsync;
      FileRef dstFileRef;
      do
      {
        fileAsync = await destLib.CreateFileAsync(destName, (CreationCollisionOption) 0);
        dstFileRef = MediaStorage.AnalyzePath(fileAsync.Path);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          LocalFile localFileByUri = db.GetLocalFileByUri(fileEntry.FileReference.ToAbsolutePath());
          if (localFileByUri != null && localFileByUri.Sha1Hash != null && localFileByUri.Sha1Hash.Length != 0 && !localFileByUri.Sha1Hash.IsEqualBytes(fileEntry.Sha1Hash))
            return;
          if (fileEntry.FileReference.Root != dstFileRef.Root)
          {
            if (db.GetLocalFileByUri(dstFileRef.ToAbsolutePath()) == null)
              tryNewName = false;
            else
              Log.l("onedrive", "finding new name for file as selected name is already used on other root: {0}", (object) dstFileRef.ToString());
          }
          else
            tryNewName = false;
        }));
      }
      while (tryNewName);
      if (!string.Equals(fileEntry.FileReference.ToAbsolutePath(), dstFileRef.ToAbsolutePath(), StringComparison.InvariantCultureIgnoreCase))
      {
        Log.l("onedrive", "media file changed from \"{0}\" to \"{1}\"", (object) fileEntry.FileReference.ToString(), (object) dstFileRef.ToString());
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          MediaRenameJournalEntry entry = manifest.InsertMediaRenameIntoJournal(fileEntry.FileReference, dstFileRef, fileEntry.Sha1Hash);
          db.LocalFileRename(fileEntry.FileReference, dstFileRef, fileEntry.Sha1Hash);
          manifest.ApplyMediaRenameFromJournal(entry);
        }));
      }
      if (fileAsync == null)
        return;
      await srcFile.CopyAndReplaceAsync((IStorageFile) fileAsync);
    }

    private async Task SaveMediaToIsoStore(
      RemoteMediaFile fileEntry,
      string downloadTmpPath,
      CancellationToken cancellationToken)
    {
      string absolutePath = fileEntry.FileReference.ToAbsolutePath();
      MediaDownload.CreateIsoStoreDirectory(fileEntry.FileReference.Subdir);
      DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
      using (IMediaStorage srcFs = MediaStorage.Create(downloadTmpPath))
      {
        using (Stream srcStream = srcFs.OpenFile(downloadTmpPath))
        {
          if (diskSpace.FreeBytes < (ulong) srcStream.Length + 15UL)
            throw new OneDriveDiskFullException(string.Format("Not enough free space to save file: {0} < {1} + {2}", (object) diskSpace.FreeBytes, (object) srcStream.Length, (object) 15));
          using (IMediaStorage dstFs = MediaStorage.Create(absolutePath))
          {
            using (Stream dstStream = dstFs.OpenFile(absolutePath, FileMode.Create, FileAccess.Write))
              await srcStream.CopyToAsync(dstStream);
          }
          srcStream.Close();
        }
      }
    }

    private static string CreateDownloadTmpPath(RemoteMediaFile fileEntry)
    {
      return string.Format("{0}\\_{1}.tmp", (object) OneDriveRestoreProcessor.RestoreTmpPath, (object) fileEntry.RemoteFileId.Replace('!', '_'));
    }
  }
}
