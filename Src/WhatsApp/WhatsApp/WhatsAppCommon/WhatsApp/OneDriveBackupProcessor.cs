// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveBackupProcessor
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using WhatsApp.WaCollections;
using Windows.Security.Authentication.OnlineId;


namespace WhatsApp
{
  public class OneDriveBackupProcessor : OneDriveProcessor
  {
    private const int OneDriveLargeUploadMaxRetry = 2;

    public async Task PrepareBackupPath(
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      string dbDirName = OneDriveBackupProcessor.RemoteDbDirectory(manifest, true);
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "preparing backup path");
      Item obj1 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.Request().GetAsync(cancellationToken)), cancellationToken);
      Log.l("onedrive", string.Format("AppRoot: Id={0}, Name={1}", (object) obj1.Id, (object) obj1.Name));
      Item obj2 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
      bool flag1 = false;
      if (obj2.Children != null && obj2.Children.Count > 0)
      {
        foreach (Item child in (IEnumerable<Item>) obj2.Children)
        {
          if (child.Name.Equals(this.chatId, StringComparison.InvariantCultureIgnoreCase))
          {
            Log.l("onedrive", string.Format("Backup Root: Id={0}, Name={1}", (object) child.Id, (object) child.Name));
            flag1 = true;
          }
        }
      }
      if (!flag1)
      {
        Log.l("onedrive", "creating backup root for {0}", (object) this.chatId);
        Item obj3 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.Children.Request().AddAsync(new Item()
        {
          Name = this.chatId,
          Folder = new Microsoft.OneDrive.Sdk.Folder()
        })), cancellationToken);
        Log.l("onedrive", "subfolder created: Id={0}, Name={1}", (object) obj3.Id, (object) obj3.Name);
      }
      Item backupFolder;
      Item obj4 = backupFolder;
      backupFolder = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(this.chatId).Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
      bool flag2 = false;
      bool hasDirMedia = false;
      List<Item> source = new List<Item>();
      foreach (Item child in (IEnumerable<Item>) backupFolder.Children)
      {
        if (child.Name.Equals(dbDirName, StringComparison.InvariantCultureIgnoreCase))
        {
          Log.l("onedrive", "Backup subfolder: Id={0}, Name={1}", (object) child.Id, (object) child.Name);
          flag2 = true;
        }
        else if (child.Name.Equals("media", StringComparison.InvariantCultureIgnoreCase))
        {
          Log.l("onedrive", "Backup subfolder: Id={0}, Name={1}", (object) child.Id, (object) child.Name);
          hasDirMedia = true;
        }
        else if (child.Name.StartsWith("db_"))
        {
          if (child.Name.Length > 3 && long.TryParse(child.Name.Substring(3), out long _))
            source.Add(child);
          Log.l("onedrive", "Found previous backup folder: Name={0}, ID={1}", (object) child.Name, (object) child.Id);
        }
      }
      List<Item> list = source.OrderBy<Item, long>((Func<Item, long>) (item => long.Parse(item.Name.Substring(3)))).ToList<Item>();
      if (!flag2)
      {
        if (list.Count > 2)
        {
          Log.l("onedrive", "recycling db subfolder");
          Item reuseItem = list[0];
          Item updateItem = new Item() { Name = dbDirName };
          Item obj5 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Items[reuseItem.Id].Request().UpdateAsync(updateItem)), cancellationToken);
          Log.l("onedrive", "subfolder recycled: Id={0}, Name={1}->{2}", (object) reuseItem.Id, (object) reuseItem.Name, (object) obj5.Name);
        }
        else
        {
          Log.l("onedrive", "creating db subfolder");
          Item obj6 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Items[backupFolder.Id].Children.Request().AddAsync(new Item()
          {
            Name = dbDirName,
            Folder = new Microsoft.OneDrive.Sdk.Folder()
          })), cancellationToken);
          Log.l("onedrive", "subfolder created: Id={0}, Name={1}", (object) obj6.Id, (object) obj6.Name);
        }
      }
      if (!hasDirMedia)
      {
        Log.l("onedrive", "creating media subfolder");
        Item obj7 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Items[backupFolder.Id].Children.Request().AddAsync(new Item()
        {
          Name = "media",
          Folder = new Microsoft.OneDrive.Sdk.Folder()
        })), cancellationToken);
        Log.l("onedrive", "subfolder created: Id={0}, Name={1}", (object) obj7.Id, (object) obj7.Name);
      }
      Log.l("onedrive", "backup path prepared");
    }

    public async Task<bool> UploadManifest(
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      bool flag = false;
      string dbDirName = OneDriveBackupProcessor.RemoteDbDirectory(manifest, true);
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "uploading manifest");
      bool wasOpen = manifest.IsOpen;
      string fileName = manifest.FileName;
      try
      {
        if (wasOpen)
          manifest.Close();
        string manifestPath = string.Format("{0}/{1}/onedrive_manifest.db", (object) this.chatId, (object) dbDirName);
        string remoteHash = (string) null;
        string localHash = (string) null;
        try
        {
          Item obj = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(manifestPath).Request().GetAsync(cancellationToken)), cancellationToken);
          Log.l("onedrive", "found remote copy, Id={0}, ETag={1}, CTag={2}", (object) obj.Id, (object) obj.ETag, (object) obj.CTag);
          if (obj.File != null)
          {
            if (obj.File.Hashes != null)
              remoteHash = obj.File.Hashes.Sha1Hash;
          }
        }
        catch (ServiceException ex)
        {
          string errorCode = OneDriveErrorCode.ItemNotFound.ToString();
          if (ex.IsMatch(errorCode))
            Log.l("onedrive", "did not find remote copy");
          else
            throw;
        }
        flag = await this.OneDriveRun<bool>((Func<Task<bool>>) (async () =>
        {
          using (NativeMediaStorage fs = new NativeMediaStorage())
          {
            using (Stream stream = fs.OpenFile(fileName, FileMode.Open, FileAccess.Read))
            {
              if (remoteHash != null)
              {
                using (SHA1Managed shA1Managed = new SHA1Managed())
                  localHash = shA1Managed.ComputeHash(stream).ToHexString();
                stream.Position = 0L;
              }
              if (!string.IsNullOrEmpty(localHash) && !string.IsNullOrEmpty(remoteHash) && localHash.Equals(remoteHash, StringComparison.InvariantCultureIgnoreCase))
              {
                Log.l("onedrive", "hash match on local manifest: {0}", (object) fileName);
                return false;
              }
              Log.l("onedrive", "uploading local manifest: {0}, Size={1}", (object) fileName, (object) stream.Length);
              stream.Position = 0L;
              Item obj = await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(manifestPath).Content.Request().PutAsync<Item>(stream, cancellationToken);
              stream.Close();
              Log.l("onedrive", "manifest upload complete, Id={0}, ETag={1}, CTag={2}", (object) obj.Id, (object) obj.ETag, (object) obj.CTag);
              return true;
            }
          }
        }), cancellationToken);
      }
      finally
      {
        if (wasOpen)
          manifest.Open();
      }
      return flag;
    }

    public async Task BackupDatabases(
      OneDriveManifest manifest,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      string dbDirName = OneDriveBackupProcessor.RemoteDbDirectory(manifest, true);
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      List<LocalBackupFile> localBackupFiles = manifest.LocalDatabaseFilesToUpload();
      string dbPath = string.Format("{0}/{1}", (object) this.chatId, (object) dbDirName);
      Set<string> remoteDbSet = new Set<string>();
      Item obj1 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(dbPath).Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
      if (obj1.Children != null)
      {
        foreach (Item child in (IEnumerable<Item>) obj1.Children)
        {
          byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(child);
          if (fileHash != null)
          {
            remoteDbSet.Add(fileHash.ToHexString());
            manifest.UpdateRemoteDatabaseFile(child);
          }
        }
      }
      foreach (LocalBackupFile localBackupFile in localBackupFiles)
      {
        LocalBackupFile backupFile = localBackupFile;
        cancellationToken.ThrowIfCancellationRequested();
        if (!remoteDbSet.Contains(backupFile.Sha1Hash.ToHexString()))
        {
          string localFile = backupFile.FileReference.ToAbsolutePath();
          string remoteFile = backupFile.FileReference.FilePart;
          string remotePath = string.Format("{0}/{1}/{2}", (object) this.chatId, (object) dbDirName, (object) remoteFile);
          long fileSize = 0;
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            using (Stream inputStream = nativeMediaStorage.OpenFile(localFile, FileMode.Open, FileAccess.Read))
            {
              byte[] b = (byte[]) null;
              using (SHA1Managed shA1Managed = new SHA1Managed())
                b = shA1Managed.ComputeHash(inputStream);
              if (!backupFile.Sha1Hash.IsEqualBytes(b))
              {
                Log.l("onedrive", "local DB does not match hash from backup session: {0}", (object) localFile);
                throw new Exception("local DB does not match hash from backup session");
              }
              fileSize = inputStream.Length;
            }
          }
          Log.l("onedrive", "uploading DB local file: {0}, Size={1}", (object) localFile, (object) fileSize);
          Item fileItem = (Item) null;
          if (fileSize > 1048576L)
          {
            using (NativeMediaStorage fs = new NativeMediaStorage())
            {
              using (Stream stream = fs.OpenFile(localFile, FileMode.Open, FileAccess.Read))
                fileItem = await this.UploadLargeFile(remotePath, remoteFile, stream, cancellationToken, progress);
            }
          }
          else
          {
            fileItem = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () =>
            {
              Item obj2;
              using (NativeMediaStorage fs = new NativeMediaStorage())
              {
                using (Stream stream = fs.OpenFile(localFile, FileMode.Open, FileAccess.Read))
                  obj2 = await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(remotePath).Content.Request().PutAsync<Item>(stream, cancellationToken);
              }
              return obj2;
            }), cancellationToken);
            if (fileItem != null && progress != null)
              progress.Report(fileItem.Size ?? 0L);
          }
          if (fileItem == null)
          {
            if (fileSize > 1048576L)
              OneDriveBackupProcessor.RemoveSavedLargeUploadUrl(remotePath, remoteFile);
            throw new Exception("item not uploaded successfully");
          }
          if (!OneDriveBackupFiles.ExtractFileHash(fileItem).IsEqualBytes(backupFile.Sha1Hash))
          {
            if (fileSize > 1048576L)
              OneDriveBackupProcessor.RemoveSavedLargeUploadUrl(remotePath, remoteFile);
            throw new Exception("item upload hash mismatch");
          }
          Log.l("onedrive", "upload complete, Id={0}, ETag={1}, CTag={2}", (object) fileItem.Id, (object) fileItem.ETag, (object) fileItem.CTag);
          manifest.UpdateRemoteDatabaseFile(fileItem);
          remoteFile = (string) null;
          backupFile = new LocalBackupFile();
        }
      }
    }

    private async Task<Item> UploadLargeFile(
      string path,
      string name,
      Stream stream,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      int retryCount = 0;
      long accumulatedProgress = 0;
      Progress<long> progressWrapper = new Progress<long>((Action<long>) (p =>
      {
        accumulatedProgress += p;
        progress?.Report(p);
      }));
      Item item = (Item) null;
      do
      {
        try
        {
          cancellationToken.ThrowIfCancellationRequested();
          if (retryCount > 0)
            Log.l("onedrive", "large upload retry attempt {0}", (object) retryCount);
          item = await this.UploadLargeFileImpl(path, name, stream, cancellationToken, (IProgress<long>) progressWrapper);
          break;
        }
        catch (ServiceException ex)
        {
          if (ex.IsMatch("invalidRange") || ex.IsMatch("sessionExpired") || ex.IsMatch("invalidRequest"))
          {
            Log.l("onedrive", "restarting large upload due to error: {0}", (object) ex.Error.Code);
            OneDriveBackupProcessor.RemoveSavedLargeUploadUrl(path, name);
            progress?.Report(-accumulatedProgress);
            accumulatedProgress = 0L;
            stream.Position = 0L;
            ++retryCount;
          }
          else
            throw;
        }
      }
      while (retryCount < 2);
      if (item == null && retryCount >= 2)
      {
        Log.l("onedrive", "large file upload failed due to max retry count");
        throw new Exception("maximum number of OneDrive retries");
      }
      return item;
    }

    private async Task<Item> UploadLargeFileImpl(
      string path,
      string name,
      Stream stream,
      CancellationToken cancellationToken,
      IProgress<long> progress)
    {
      Item item = (Item) null;
      string uploadUrl = OneDriveBackupProcessor.GetSavedLargeUploadUrl(path, name);
      DateTimeOffset dateTimeOffset;
      UploadSession session;
      if (!string.IsNullOrEmpty(uploadUrl))
      {
        Log.l("onedrive", "retrieving existing session for file: {0}", (object) name);
        UploadSession uploadSession = session;
        session = await this.OneDriveRun<UploadSession>((Func<Task<UploadSession>>) (async () =>
        {
          HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), uploadUrl);
          await this.oneDriveClient.AuthenticationProvider.AuthenticateRequestAsync(request);
          cancellationToken.ThrowIfCancellationRequested();
          return this.oneDriveClient.HttpProvider.Serializer.DeserializeObject<UploadSession>(await (await this.oneDriveClient.HttpProvider.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)).Content.ReadAsStringAsync());
        }), cancellationToken);
        if (string.IsNullOrEmpty(session.UploadUrl))
          session.UploadUrl = uploadUrl;
        object[] objArray = new object[2];
        DateTimeOffset? expirationDateTime = session.ExpirationDateTime;
        ref DateTimeOffset? local = ref expirationDateTime;
        string str;
        if (!local.HasValue)
        {
          str = (string) null;
        }
        else
        {
          dateTimeOffset = local.GetValueOrDefault();
          str = dateTimeOffset.ToString();
        }
        if (str == null)
          str = "<null>";
        objArray[0] = (object) str;
        objArray[1] = (object) string.Join(", ", session.NextExpectedRanges);
        Log.l("onedrive", "retrieved session: Expiration={0}, NextExpectedRanges=[{1}]", objArray);
      }
      else
      {
        Log.l("onedrive", "creating new session for file: {0}", (object) name);
        UploadSession uploadSession1 = session;
        session = await this.OneDriveRun<UploadSession>((Func<Task<UploadSession>>) (async () =>
        {
          UploadSession uploadSession2;
          try
          {
            uploadSession2 = await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(path).CreateSession(new ChunkedUploadSessionDescriptor()
            {
              Name = name
            }).Request().PostAsync(cancellationToken);
          }
          catch (ServiceException ex)
          {
            if (ex.IsMatch("invalidRequest"))
              throw new OneDriveInvalidUploadException("Unable to create upload session for file", (Exception) ex);
            throw;
          }
          return uploadSession2;
        }), cancellationToken);
        OneDriveBackupProcessor.SaveLargeUploadUrl(path, name, session.UploadUrl);
        object[] objArray = new object[2];
        DateTimeOffset? expirationDateTime = session.ExpirationDateTime;
        ref DateTimeOffset? local = ref expirationDateTime;
        string str;
        if (!local.HasValue)
        {
          str = (string) null;
        }
        else
        {
          dateTimeOffset = local.GetValueOrDefault();
          str = dateTimeOffset.ToString();
        }
        if (str == null)
          str = "<null>";
        objArray[0] = (object) str;
        objArray[1] = (object) string.Join(", ", session.NextExpectedRanges);
        Log.l("onedrive", "created session: Expiration={0}, NextExpectedRanges=[{1}]", objArray);
      }
      cancellationToken.ThrowIfCancellationRequested();
      DateTimeOffset? expirationDateTime1 = session.ExpirationDateTime;
      ref DateTimeOffset? local1 = ref expirationDateTime1;
      long num1;
      if (!local1.HasValue)
      {
        num1 = long.MaxValue;
      }
      else
      {
        dateTimeOffset = local1.GetValueOrDefault();
        num1 = dateTimeOffset.UtcTicks;
      }
      long utcExpires = num1;
      long fragmentSize = this.UploadFragmentSizeBytes;
      KeyValuePair<long, long> nextRange = OneDriveBackupProcessor.ParseUploadRange(session.NextExpectedRanges);
      long currentProgress = nextRange.Key;
      byte[] fragmentBuffer = new byte[fragmentSize];
      if (currentProgress > 0L)
      {
        Log.l("onedrive", "progress: {0} / {1}", (object) currentProgress, (object) stream.Length);
        progress?.Report(currentProgress);
      }
      while (OneDriveBackupProcessor.IsUploadRangeValid(nextRange))
      {
        dateTimeOffset = DateTimeOffset.UtcNow;
        long utcTicks = dateTimeOffset.UtcTicks;
        if (utcTicks >= utcExpires)
        {
          Log.l("onedrive", "large upload session expired: {0} >= {1}", (object) (utcTicks / 10000L), (object) (utcExpires / 10000L));
          throw new ServiceException(new Error()
          {
            Code = "sessionExpired",
            Message = ErrorConstants.Messages.UnexpectedExceptionOnSend
          });
        }
        long startPosition = nextRange.Key;
        long num2 = Math.Min(nextRange.Value + 1L, startPosition + fragmentSize);
        if (num2 > stream.Length)
          num2 = stream.Length;
        stream.Seek(startPosition, SeekOrigin.Begin);
        int len;
        int num3 = len;
        len = await stream.ReadAsync(fragmentBuffer, 0, (int) (num2 - startPosition), cancellationToken);
        HttpResponseMessage response = await this.OneDriveRun<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
        {
          HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PUT"), session.UploadUrl);
          await this.oneDriveClient.AuthenticationProvider.AuthenticateRequestAsync(request);
          request.Content = (HttpContent) new ByteArrayContent(fragmentBuffer, 0, len);
          request.Content.Headers.ContentLength = new long?((long) len);
          request.Content.Headers.ContentRange = new ContentRangeHeaderValue(startPosition, startPosition + (long) len - 1L, stream.Length);
          return await this.oneDriveClient.HttpProvider.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }), cancellationToken);
        string inputString = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Accepted)
        {
          UploadSession uploadSession = this.oneDriveClient.HttpProvider.Serializer.DeserializeObject<UploadSession>(inputString);
          Log.l("onedrive", "chunk accepted: Expiration={0}, NextExpectedRanges=[{1}]", (object) uploadSession.ExpirationDateTime, (object) string.Join(", ", uploadSession.NextExpectedRanges));
          DateTimeOffset? expirationDateTime2 = uploadSession.ExpirationDateTime;
          if (expirationDateTime2.HasValue)
          {
            expirationDateTime2 = uploadSession.ExpirationDateTime;
            dateTimeOffset = expirationDateTime2.Value;
            utcExpires = dateTimeOffset.UtcTicks;
          }
          nextRange = OneDriveBackupProcessor.ParseUploadRange(uploadSession.NextExpectedRanges);
          currentProgress += (long) len;
          Log.l("onedrive", "progress: {0} / {1}", (object) currentProgress, (object) stream.Length);
          progress?.Report((long) len);
          response = (HttpResponseMessage) null;
        }
        else
        {
          if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
          {
            item = this.oneDriveClient.HttpProvider.Serializer.DeserializeObject<Item>(inputString);
            long num4 = stream.Length - currentProgress;
            currentProgress = stream.Length;
            Log.l("onedrive", "progress: {0} / {1}", (object) currentProgress, (object) stream.Length);
            progress?.Report(num4);
            OneDriveBackupProcessor.RemoveSavedLargeUploadUrl(path, name);
            Log.l("onedrive", "large upload complete: {0}", (object) response.StatusCode.ToString());
            break;
          }
          Log.l("onedrive", "unexpected response code: {0}", (object) response.StatusCode.ToString());
          break;
        }
      }
      return item;
    }

    private static void SaveLargeUploadUrl(string path, string name, string uploadUrl)
    {
      string largeUploadUrlKey = OneDriveBackupProcessor.GetLargeUploadUrlKey(path, name);
      Dictionary<string, string> driveLargeUploadUrls = Settings.OneDriveLargeUploadUrls;
      driveLargeUploadUrls.Add(largeUploadUrlKey, uploadUrl);
      Settings.OneDriveLargeUploadUrls = driveLargeUploadUrls;
    }

    private static string GetSavedLargeUploadUrl(string path, string name)
    {
      string str;
      if (!Settings.OneDriveLargeUploadUrls.TryGetValue(OneDriveBackupProcessor.GetLargeUploadUrlKey(path, name), out str))
        return (string) null;
      return string.IsNullOrEmpty(str) ? (string) null : str;
    }

    private static void RemoveSavedLargeUploadUrl(string path, string name)
    {
      string largeUploadUrlKey = OneDriveBackupProcessor.GetLargeUploadUrlKey(path, name);
      Dictionary<string, string> driveLargeUploadUrls = Settings.OneDriveLargeUploadUrls;
      driveLargeUploadUrls.Remove(largeUploadUrlKey);
      Settings.OneDriveLargeUploadUrls = driveLargeUploadUrls;
    }

    private static string GetLargeUploadUrlKey(string path, string name) => path + ":" + name;

    private static KeyValuePair<long, long> ParseUploadRange(IEnumerable<string> nextExpectedRanges)
    {
      long key = 0;
      long num = 9223372036854775806;
      try
      {
        string[] strArray = nextExpectedRanges.First<string>().Split('-');
        if (strArray.Length != 0 && !string.IsNullOrEmpty(strArray[0]))
          key = long.Parse(strArray[0]);
        if (strArray.Length > 1)
        {
          if (!string.IsNullOrEmpty(strArray[1]))
            num = long.Parse(strArray[1]);
        }
      }
      catch (Exception ex)
      {
        Log.l("onedrive", "unable to parse upload range");
        Log.LogException(ex, "onedrive");
      }
      return new KeyValuePair<long, long>(key, num);
    }

    private static bool IsUploadRangeValid(KeyValuePair<long, long> range)
    {
      return range.Key >= 0L && range.Value >= range.Key;
    }

    public async Task BackupMedia(
      OneDriveManifest manifest,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "performing media backup");
      List<LocalBackupFile> fileList = manifest.LocalMediaFilesToUpload();
      Log.l("onedrive", "manifest has {0} files to upload", (object) fileList.Count);
      int index = 0;
      foreach (LocalBackupFile localBackupFile in fileList)
      {
        LocalBackupFile fileEntry = localBackupFile;
        ++index;
        bool blacklistFile = false;
        string absolutePath = fileEntry.FileReference.ToAbsolutePath();
        try
        {
          Item fileItem = (Item) null;
          byte[] localFileHash = (byte[]) null;
          try
          {
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              using (Stream inputStream = nativeMediaStorage.OpenFile(absolutePath, FileMode.Open, FileAccess.Read))
              {
                using (SHA1Managed shA1Managed = new SHA1Managed())
                  localFileHash = shA1Managed.ComputeHash(inputStream);
              }
            }
          }
          catch (Exception ex)
          {
            Log.l("onedrive", "unable to compute hash for local file to upload: {0}", (object) ex.ToString());
          }
          if (localFileHash != null && localFileHash.IsEqualBytes(fileEntry.Sha1Hash))
            fileItem = await this.UploadMediaFile(fileEntry, cancellationToken, progress);
          else if (localFileHash != null && fileEntry.Sha1Hash != null)
          {
            Log.l("onedrive", "local file hash does not match local file entry, cannot upload: {0}", (object) fileEntry.FileReference.ToAbsolutePath());
            blacklistFile = true;
          }
          if (fileItem != null)
          {
            Log.l("onedrive", "uploaded item {0} {1}/{2} => {3}", (object) fileEntry.FileReference.FilePart, (object) index, (object) fileList.Count, (object) fileItem.Id);
            manifest.UpdateRemoteMediaFile(fileEntry.FileReference, fileItem);
          }
          localFileHash = (byte[]) null;
        }
        catch (OneDriveInvalidUploadException ex)
        {
          Log.l("onedrive", "unable to construct valid upload request: {0}", (object) ex.ToString());
          blacklistFile = true;
        }
        catch (DirectoryNotFoundException ex)
        {
          Log.l("onedrive", "unable to find path for local file to upload: {0}", (object) ex.ToString());
          blacklistFile = true;
        }
        catch (FileNotFoundException ex)
        {
          Log.l("onedrive", "unable to find local file to upload: {0}", (object) ex.ToString());
          blacklistFile = true;
        }
        if (blacklistFile)
          manifest.BlacklistLocalMediaFile(fileEntry);
        fileEntry = new LocalBackupFile();
      }
    }

    private async Task<Item> UploadMediaFile(
      LocalBackupFile fileEntry,
      CancellationToken cancellationToken,
      IProgress<long> progress = null)
    {
      Item obj1 = (Item) null;
      string localFileName = fileEntry.FileReference.ToAbsolutePath();
      string remoteFileName = fileEntry.FileReference.ToRemoteFileName();
      string itemPath = string.Format("{0}/media/{1}", (object) this.chatId, (object) remoteFileName);
      long num = 0;
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        using (Stream stream = nativeMediaStorage.OpenFile(localFileName, FileMode.Open, FileAccess.Read))
          num = stream.Length;
      }
      if (num > 1048576L)
      {
        using (NativeMediaStorage fs = new NativeMediaStorage())
        {
          using (Stream stream = fs.OpenFile(localFileName, FileMode.Open, FileAccess.Read))
            obj1 = await this.UploadLargeFile(itemPath, remoteFileName, stream, cancellationToken, progress);
        }
      }
      else
      {
        obj1 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () =>
        {
          Item obj2;
          using (NativeMediaStorage fs = new NativeMediaStorage())
          {
            using (Stream stream = fs.OpenFile(localFileName, FileMode.Open, FileAccess.Read))
            {
              try
              {
                obj2 = await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(itemPath).Content.Request().PutAsync<Item>(stream, cancellationToken);
              }
              catch (ServiceException ex)
              {
                if (ex.IsMatch("invalidRequest"))
                  throw new OneDriveInvalidUploadException("Unable to upload file", (Exception) ex);
                throw;
              }
            }
          }
          return obj2;
        }), cancellationToken);
        if (obj1 != null && progress != null)
          progress.Report(obj1.Size ?? 0L);
      }
      return obj1;
    }

    public async Task PurgeDeletedMedia(
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      int num1 = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "checking for remote media files to purge");
      List<RemoteMediaFile> purge = manifest.RemoteMediaFilesToPurge();
      Log.l("onedrive", "manifest has {0} remote media files to purge", (object) purge.Count);
      int removed = 0;
      int missing = 0;
      foreach (RemoteMediaFile remoteMediaFile in purge)
      {
        RemoteMediaFile fileEntry = remoteMediaFile;
        Log.l("onedrive", "purging file: {0} (Id={1}, Size={2})", (object) fileEntry.FileReference.ToRemoteFileName(), (object) fileEntry.RemoteFileId, (object) fileEntry.Size);
        int num2 = await this.OneDriveRun<int>((Func<Task<int>>) (async () =>
        {
          try
          {
            await this.oneDriveClient.Drive.Items[fileEntry.RemoteFileId].Request().DeleteAsync(cancellationToken);
            Log.l("onedrive", "remote file removed (Id={0})", (object) fileEntry.RemoteFileId);
            ++removed;
          }
          catch (ServiceException ex)
          {
            string errorCode = OneDriveErrorCode.ItemNotFound.ToString();
            if (ex.IsMatch(errorCode))
            {
              Log.l("onedrive", "did not find remote file (Id={0})", (object) fileEntry.RemoteFileId);
              ++missing;
            }
            else
              throw;
          }
          return 1;
        }), cancellationToken);
        manifest.RemoveRemoteMediaFileById(fileEntry.RemoteFileId);
        cancellationToken.ThrowIfCancellationRequested();
      }
      if (removed <= 0 && missing <= 0)
        return;
      Log.l("onedrive", "purge completed: {0} removed, {1} missing", (object) removed, (object) missing);
    }

    public async Task FinalizeBackup(OneDriveManifest manifest, CancellationToken cancellationToken)
    {
      int num1 = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "finalizing backup");
      string pendingDbDirName = OneDriveBackupProcessor.RemoteDbDirectory(manifest, true);
      string finalDbDirName = OneDriveBackupProcessor.RemoteDbDirectory(manifest, false);
      string.Format("{0}/{1}", (object) this.chatId, (object) pendingDbDirName);
      Item obj1 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(this.chatId).Request().Expand("children").GetAsync(cancellationToken)), cancellationToken);
      string dbDirId = (string) null;
      string str1 = (string) null;
      List<string> itemsToRemove = new List<string>();
      List<Item> otherBackups = new List<Item>();
      foreach (Item child in (IEnumerable<Item>) obj1.Children)
      {
        if (child.Folder != null)
        {
          if (child.Name.Equals(pendingDbDirName, StringComparison.InvariantCultureIgnoreCase))
          {
            dbDirId = child.Id;
            Log.l("onedrive", "found backup folder: Name={0}, ID={1}", (object) child.Name, (object) child.Id);
          }
          else if (child.Name.Equals(finalDbDirName, StringComparison.InvariantCultureIgnoreCase))
          {
            str1 = child.Id;
            Log.l("onedrive", "found colliding remnant: Name={0}, ID={1}", (object) child.Name, (object) child.Id);
          }
          else if (child.Name.StartsWith("t_db_"))
          {
            itemsToRemove.Add(child.Id);
            Log.l("onedrive", "found stale incomplete backup: Name={0}, ID={1}", (object) child.Name, (object) child.Id);
          }
          else if (child.Name.StartsWith("db_"))
          {
            if (child.Name.Length > 3 && long.TryParse(child.Name.Substring(3), out long _))
              otherBackups.Add(child);
            Log.l("onedrive", "found previous backup folder: Name={0}, ID={1}", (object) child.Name, (object) child.Id);
          }
        }
      }
      if (str1 != null && dbDirId != null)
        itemsToRemove.Add(str1);
      foreach (string str2 in itemsToRemove)
      {
        string itemId = str2;
        int num2 = await this.OneDriveRun<int>((Func<Task<int>>) (async () =>
        {
          await this.oneDriveClient.Drive.Items[itemId].Request().DeleteAsync(cancellationToken);
          return 1;
        }), cancellationToken);
        Log.l("onedrive", "removed item: ID={0}", (object) itemId);
      }
      itemsToRemove.Clear();
      Item updateItem = new Item() { Name = finalDbDirName };
      Item obj2 = await this.OneDriveRun<Item>((Func<Task<Item>>) (async () => await this.oneDriveClient.Drive.Items[dbDirId].Request().UpdateAsync(updateItem, cancellationToken)), cancellationToken);
      Log.l("onedrive", "remote DB path renamed: Name={0}, ID={1}", (object) obj2.Name, (object) obj2.Id);
      List<Item> list = otherBackups.OrderBy<Item, long>((Func<Item, long>) (item => long.Parse(item.Name.Substring(3)))).ToList<Item>();
      if (list.Count > 2)
      {
        for (int index = 0; index < list.Count - 2; ++index)
          itemsToRemove.Add(list[index].Id);
      }
      foreach (string str3 in itemsToRemove)
      {
        string itemId = str3;
        int num3 = await this.OneDriveRun<int>((Func<Task<int>>) (async () =>
        {
          await this.oneDriveClient.Drive.Items[itemId].Request().DeleteAsync(cancellationToken);
          return 1;
        }), cancellationToken);
        Log.l("onedrive", "removed old backup: ID={0}", (object) itemId);
      }
      manifest.SetOneDriveBackupInProgress(false);
    }

    private static string RemoteDbDirectory(OneDriveManifest manifest, bool inProgress)
    {
      BackupProperties backupProperties = manifest.CurrentOneDriveBackupProperties();
      if (backupProperties == null || string.IsNullOrEmpty(backupProperties.BackupId))
        throw new ArgumentException("Manifest backup properties are invalid");
      return (inProgress ? "t_db_" : "db_") + backupProperties.BackupId;
    }
  }
}
