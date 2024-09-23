// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveBackupFiles
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public static class OneDriveBackupFiles
  {
    private static string RootToRemoteString(FileRoot root)
    {
      switch (root)
      {
        case FileRoot.IsoStore:
          return "iso";
        case FileRoot.PhoneStorage:
          return "ph";
        case FileRoot.SdCard:
          return "sd";
        case FileRoot.PhoneStorageWhatsAppMedia:
          return "phm";
        case FileRoot.SdCardWhatsAppMedia:
          return "sdm";
        case FileRoot.PhoneStorageBackup:
          return "phb";
        default:
          throw new ArgumentOutOfRangeException(nameof (root));
      }
    }

    private static FileRoot RootFromRemoteString(string remoteString)
    {
      if (remoteString.Equals("phm", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.PhoneStorageWhatsAppMedia;
      if (remoteString.Equals("sdm", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.SdCardWhatsAppMedia;
      if (remoteString.Equals("phb", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.PhoneStorageBackup;
      if (remoteString.Equals("iso", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.IsoStore;
      if (remoteString.Equals("ph", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.PhoneStorage;
      if (remoteString.Equals("sd", StringComparison.InvariantCultureIgnoreCase))
        return FileRoot.SdCard;
      throw new ArgumentOutOfRangeException(nameof (remoteString));
    }

    public static string ToRemoteFileName(this FileRef r)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(OneDriveBackupFiles.RootToRemoteString(r.Root));
      if (!string.IsNullOrWhiteSpace(r.Subdir))
      {
        string subdir = r.Subdir;
        char[] separator = new char[1]{ '\\' };
        foreach (string str1 in subdir.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          string str2 = str1.Replace(",", ",,");
          stringBuilder.Append(',');
          stringBuilder.Append(str2);
        }
      }
      string str = r.FilePart.Replace(",", ",,");
      stringBuilder.Append(',');
      stringBuilder.Append(str);
      return stringBuilder.ToString();
    }

    public static FileRef FromRemoteFileName(string remoteFileName)
    {
      if (string.IsNullOrEmpty(remoteFileName))
        return new FileRef();
      List<string> source = new List<string>();
      int startIndex = 0;
      for (int index = 0; index < remoteFileName.Length; ++index)
      {
        if (remoteFileName[index] == ',')
        {
          ++index;
          if (index < remoteFileName.Length && remoteFileName[index] == ',')
          {
            ++index;
          }
          else
          {
            source.Add(remoteFileName.Substring(startIndex, index - startIndex - 1).Replace(",,", ","));
            startIndex = index;
          }
        }
      }
      if (startIndex < remoteFileName.Length)
        source.Add(remoteFileName.Substring(startIndex, remoteFileName.Length - startIndex).Replace(",,", ","));
      FileRef r = new FileRef();
      if (source.Count >= 2)
      {
        r.Root = OneDriveBackupFiles.RootFromRemoteString(source[0]);
        r.FilePart = source.Last<string>();
        if (source.Count > 2)
        {
          StringBuilder stringBuilder = new StringBuilder();
          for (int index = 1; index < source.Count - 1; ++index)
          {
            if (index != 1)
              stringBuilder.Append('\\');
            stringBuilder.Append(source[index]);
          }
          r.Subdir = stringBuilder.ToString();
        }
        else
          r.Subdir = "";
      }
      return r.Normalized();
    }

    public static byte[] ExtractFileHash(Item item)
    {
      if (item == null || item.File == null || item.File.Hashes == null || item.File.Hashes.Sha1Hash == null)
        throw new Exception("Invalid file facet!");
      return OneDriveBackupFiles.FromHexString(item.File.Hashes.Sha1Hash);
    }

    public static byte[] FromHexString(string hexString)
    {
      if (hexString == null)
        return (byte[]) null;
      return hexString.Length == 0 ? new byte[0] : Enumerable.Range(0, hexString.Length).Where<int>((Func<int, bool>) (x => x % 2 == 0)).Select<int, byte>((Func<int, byte>) (x => Convert.ToByte(hexString.Substring(x, 2), 16))).ToArray<byte>();
    }

    public static LastBackupKind GetLastBackupKind()
    {
      BackupProperties backupProperties = (BackupProperties) null;
      RemoteDatabaseFile? nullable1 = new RemoteDatabaseFile?();
      using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        backupProperties = oneDriveManifest?.CurrentOneDriveBackupProperties();
        if (backupProperties != null)
        {
          foreach (RemoteDatabaseFile remoteDatabaseFile in oneDriveManifest.GetRemoteDatabaseFiles())
          {
            if (remoteDatabaseFile.FileName.Equals("messages.db", StringComparison.InvariantCultureIgnoreCase))
            {
              nullable1 = new RemoteDatabaseFile?(remoteDatabaseFile);
              break;
            }
          }
        }
        if (nullable1.HasValue && nullable1.Value.Size >= 0L && nullable1.Value.Sha1Hash != null)
        {
          if (nullable1.Value.Sha1Hash.Length != 0)
            goto label_16;
        }
        backupProperties = (BackupProperties) null;
      }
label_16:
      Backup.BackupInfo? nullable2 = Backup.GetLastBackupInfo(true);
      string str = (string) null;
      long num = -1;
      if (nullable2.HasValue)
      {
        str = nullable2.Value.FullPath + "\\messages.db";
        using (IMediaStorage mediaStorage = MediaStorage.Create(str))
        {
          if (mediaStorage.FileExists(str))
          {
            using (Stream stream = mediaStorage.OpenFile(str))
              num = stream.Length;
          }
        }
      }
      if (str == null || num < 0L)
        nullable2 = new Backup.BackupInfo?();
      if (backupProperties != null)
        Log.d("odfiles", "Found remote backup: Date={0}, DB_Size={1}", (object) backupProperties.StartTime, (object) nullable1.Value.Size);
      if (nullable2.HasValue)
        Log.d("odfiles", "Found local backup: Date={0}, DB_Size={1}", (object) nullable2.Value.CreationTime, (object) num);
      if (backupProperties == null && nullable2.HasValue)
        return LastBackupKind.Local;
      if (backupProperties != null && !nullable2.HasValue)
        return LastBackupKind.Remote;
      if (backupProperties == null || !nullable2.HasValue)
        return LastBackupKind.None;
      if (nullable1.Value.Size == num)
      {
        Log.d("odfiles", "Local and remote DB files have the same size, comparing hashes");
        byte[] a = (byte[]) null;
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(str))
          {
            using (Stream inputStream = mediaStorage.OpenFile(str))
            {
              using (SHA1Managed shA1Managed = new SHA1Managed())
                a = shA1Managed.ComputeHash(inputStream);
            }
          }
        }
        catch (Exception ex)
        {
          Log.WriteLineDebug("local file hash error: " + ex.ToString());
          Log.LogException(ex, "local file hash");
        }
        if (a != null)
        {
          if (a.IsEqualBytes(nullable1.Value.Sha1Hash))
          {
            Log.d("odfiles", "Local and remote DB hashes match, picking local");
            return LastBackupKind.Local;
          }
          Log.d("odfiles", "Local and remote DB hashes differ, picking newer");
          return backupProperties.StartTime.Ticks > nullable2.Value.CreationTime.Ticks ? LastBackupKind.Remote : LastBackupKind.Local;
        }
        Log.d("odfiles", "Local DB hash error, picking remote");
        return LastBackupKind.Remote;
      }
      Log.d("odfiles", "Local and remote DB files are a different size, picking the newer backup");
      return backupProperties.StartTime.Ticks > nullable2.Value.CreationTime.Ticks ? LastBackupKind.Remote : LastBackupKind.Local;
    }
  }
}
