// Decompiled with JetBrains decompiler
// Type: WhatsApp.NativeMediaStorage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public class NativeMediaStorage : IMediaStorage, IDisposable
  {
    private INativeMediaStorage fs;
    private const string UriPrefix = "file:";

    public NativeMediaStorage() => this.fs = NativeInterfaces.Misc.GetFilesystem();

    public void Dispose() => this.fs = (INativeMediaStorage) null;

    public static string MakeUri(string path)
    {
      return !NativeMediaStorage.HasFilePrefix(path) ? "file:" + path : path;
    }

    private static bool HasFilePrefix(string path)
    {
      return path.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool UriApplicable(string path)
    {
      if (string.IsNullOrEmpty(path) || path.StartsWith("\\shared\\", StringComparison.InvariantCultureIgnoreCase))
        return false;
      if (NativeMediaStorage.HasFilePrefix(path) || path[0] == '\\')
        return true;
      return path.Length >= 3 && path[1] == ':' && path[2] == '\\';
    }

    public string GetFullFsPath(string localPath)
    {
      while (NativeMediaStorage.HasFilePrefix(localPath))
        localPath = localPath.Substring("file:".Length);
      return localPath;
    }

    public Stream OpenFile(string filename, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
    {
      return this.OpenFile(filename, mode, access, FileShare.ReadWrite | FileShare.Delete);
    }

    public Stream OpenFile(string filename, FileMode mode, FileAccess access, FileShare share)
    {
      uint DesiredAccess = 0;
      if ((access & FileAccess.Read) != (FileAccess) 0)
        DesiredAccess |= 2147483648U;
      if ((access & FileAccess.Write) != (FileAccess) 0)
        DesiredAccess |= 1073741824U;
      return (Stream) new NativeStream(this.fs.CreateFile(this.GetFullFsPath(filename), DesiredAccess, (uint) share, (uint) mode, 128U));
    }

    public NativeStream GetTempFile() => new NativeStream(this.fs.GetTempFile());

    public bool FileExists(string filename) => this.fs.FileExists(this.GetFullFsPath(filename));

    public void MoveFile(string src, string dst)
    {
      this.fs.MoveFile(this.GetFullFsPath(src), this.GetFullFsPath(dst));
    }

    public void MoveFileWithOverwrite(string src, string dst)
    {
      this.fs.MoveFileWithOverwrite(this.GetFullFsPath(src), this.GetFullFsPath(dst));
    }

    public void DeleteFile(string path) => this.fs.DeleteFile(this.GetFullFsPath(path));

    public void CreateDirectory(string path) => this.fs.CreateDirectory(path);

    public void RemoveDirectory(string path) => this.fs.RemoveDirectory(path);

    public IEnumerable<WIN32_FIND_DATA> FindFiles(string pattern)
    {
      WIN32_FIND_DATA data = new WIN32_FIND_DATA();
      IntPtr handle = this.fs.FindFirstFile(pattern, out data);
      try
      {
        yield return data;
        while (this.fs.FindNextFile(handle, out data))
          yield return data;
      }
      finally
      {
        this.fs.FindClose(handle);
      }
    }

    public bool SdCardExists()
    {
      try
      {
        this.FindFiles("D:\\*").FirstOrDefault<WIN32_FIND_DATA>();
        return true;
      }
      catch (DirectoryNotFoundException ex)
      {
        return false;
      }
    }
  }
}
