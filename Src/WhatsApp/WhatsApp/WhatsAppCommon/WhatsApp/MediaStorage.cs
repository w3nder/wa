// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaStorage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Text;


namespace WhatsApp
{
  public static class MediaStorage
  {
    public static IMediaStorage Create(string path)
    {
      return !NativeMediaStorage.UriApplicable(path) ? (IMediaStorage) new IsoStoreMediaStorage() : (IMediaStorage) new NativeMediaStorage();
    }

    public static Stream OpenFile(string filename, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
    {
      using (IMediaStorage mediaStorage = MediaStorage.Create(filename))
        return mediaStorage.OpenFile(filename, mode, access);
    }

    public static string GetAbsolutePath(string path)
    {
      if (path == null)
        return (string) null;
      using (IMediaStorage mediaStorage = MediaStorage.Create(path))
        return mediaStorage.GetFullFsPath(path);
    }

    public static FileRoot? DetermineRoot(string path)
    {
      try
      {
        if (!string.IsNullOrEmpty(path))
        {
          string absolutePath = MediaStorage.GetAbsolutePath(path);
          if (absolutePath.StartsWith(Constants.IsoStorePath, StringComparison.OrdinalIgnoreCase))
            return new FileRoot?(FileRoot.IsoStore);
          if (absolutePath.StartsWith("C:\\", StringComparison.OrdinalIgnoreCase))
            return new FileRoot?(FileRoot.PhoneStorage);
          if (absolutePath.StartsWith("D:\\", StringComparison.OrdinalIgnoreCase))
            return new FileRoot?(FileRoot.SdCard);
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception determining root");
      }
      Log.d("root", "Couldn't determine root from '{0}'", (object) path);
      return new FileRoot?();
    }

    public static FileRef AnalyzePath(string path)
    {
      FileRef fileRef = new FileRef();
      path = MediaStorage.GetAbsolutePath(path);
      int startIndex;
      if (path.StartsWith(Constants.IsoStorePath, StringComparison.OrdinalIgnoreCase))
      {
        fileRef.Root = FileRoot.IsoStore;
        startIndex = Constants.IsoStorePath.Length + 1;
      }
      else if (path.StartsWith("C:\\", StringComparison.OrdinalIgnoreCase))
      {
        fileRef.Root = FileRoot.PhoneStorage;
        startIndex = 3;
      }
      else if (path.StartsWith("D:\\", StringComparison.OrdinalIgnoreCase))
      {
        fileRef.Root = FileRoot.SdCard;
        startIndex = 3;
      }
      else
      {
        Log.l(nameof (AnalyzePath), "Path: {0}", (object) path);
        throw new Exception("Unexpected prefix on path");
      }
      string str = path.Substring(startIndex);
      int length = str.LastIndexOf('\\');
      if (length <= 0)
      {
        fileRef.Subdir = "";
        fileRef.FilePart = str;
      }
      else
      {
        fileRef.Subdir = str.Substring(0, length);
        fileRef.FilePart = str.Substring(length + 1);
      }
      foreach (RootMapping mapping in RootMapping.Mappings)
      {
        if (fileRef.Root == mapping.Base && fileRef.Subdir != null && fileRef.Subdir.StartsWith(mapping.Subdir, StringComparison.InvariantCultureIgnoreCase))
        {
          char ch;
          if (mapping.Subdir.Length >= fileRef.Subdir.Length || (ch = fileRef.Subdir[mapping.Subdir.Length]) == '\\' || ch == '/')
          {
            fileRef.Root |= mapping.Value;
            fileRef.Subdir = fileRef.Subdir.Substring(mapping.Subdir.Length + (fileRef.Subdir.Length > mapping.Subdir.Length ? 1 : 0));
            break;
          }
        }
      }
      return fileRef;
    }

    public static string ToAbsolutePath(this FileRef r)
    {
      StringBuilder stringBuilder = new StringBuilder();
      FileRoot fileRoot1 = r.Root & FileRoot.StorageMask;
      switch (fileRoot1)
      {
        case FileRoot.IsoStore:
          stringBuilder.Append(Constants.IsoStorePath);
          stringBuilder.Append('\\');
          break;
        case FileRoot.PhoneStorage:
          stringBuilder.Append("C:\\");
          break;
        case FileRoot.SdCard:
          stringBuilder.Append("D:\\");
          break;
        default:
          throw new NotSupportedException("Unknown root " + (object) r.Root);
      }
      FileRoot fileRoot2 = r.Root & ~FileRoot.StorageMask;
      if (fileRoot2 != FileRoot.IsoStore)
      {
        foreach (RootMapping mapping in RootMapping.Mappings)
        {
          if (mapping.Base == fileRoot1 && mapping.Value == fileRoot2)
          {
            stringBuilder.Append(mapping.Subdir);
            stringBuilder.Append('\\');
            break;
          }
        }
      }
      if (!string.IsNullOrEmpty(r.Subdir))
      {
        stringBuilder.Append(r.Subdir);
        stringBuilder.Append('\\');
      }
      stringBuilder.Append(r.FilePart);
      return stringBuilder.ToString();
    }

    public static FileRef Normalized(this FileRef r)
    {
      return MediaStorage.AnalyzePath(r.ToAbsolutePath());
    }

    public static bool TryNormalize(this FileRef r, out FileRef normalizedRef)
    {
      normalizedRef = r.Normalized();
      return r.Root != normalizedRef.Root || !string.Equals(r.Subdir, normalizedRef.Subdir) || !string.Equals(r.FilePart, normalizedRef.Subdir);
    }

    public static void RenameDir(string fromDirFullPath, string toDirFullPath)
    {
      try
      {
        DirectoryInfo directoryInfo = new DirectoryInfo(fromDirFullPath);
        Log.l("media", "renaming directory from {0} to {1}", (object) fromDirFullPath, (object) toDirFullPath);
        directoryInfo.MoveTo(toDirFullPath);
      }
      catch (Exception ex)
      {
        Log.l("oops", "{0}", (object) ex.GetFriendlyMessage());
      }
    }

    public static void CopyFilesInDir(string fromDirFullPath, string toDirFullPath)
    {
      try
      {
        Log.l("media", "checking output directory for copy from {0} to {1}", (object) fromDirFullPath, (object) toDirFullPath);
        DirectoryInfo directoryInfo1 = new DirectoryInfo(toDirFullPath);
        DirectoryInfo directoryInfo2 = new DirectoryInfo(fromDirFullPath);
        if (!directoryInfo2.Exists)
          Log.l("media", "input directory not found {0}", (object) fromDirFullPath);
        DirectoryInfo directoryInfo3 = new DirectoryInfo(toDirFullPath);
        if (!directoryInfo3.Exists)
          directoryInfo3.Create();
        FileInfo[] files = directoryInfo2.GetFiles();
        foreach (FileInfo fileInfo in files)
        {
          Log.l("media", "copying {0}", (object) fileInfo.Name);
          fileInfo.CopyTo(toDirFullPath + "\\" + fileInfo.Name);
        }
        Log.l("media", "copied {0} files", (object) files.Length);
      }
      catch (Exception ex)
      {
        Log.l("oops", "{0}", (object) ex.GetFriendlyMessage());
      }
    }
  }
}
