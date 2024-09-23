// Decompiled with JetBrains decompiler
// Type: WhatsApp.IsoStoreMediaStorage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.IO.IsolatedStorage;

#nullable disable
namespace WhatsApp
{
  public class IsoStoreMediaStorage : IMediaStorage, IDisposable
  {
    private IsolatedStorageFile fs;

    public IsoStoreMediaStorage() => this.fs = IsolatedStorageFile.GetUserStoreForApplication();

    public void Dispose() => this.fs.Dispose();

    public string GetFullFsPath(string localPath)
    {
      return string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) MediaDownload.SanitizeIsoStorePath(localPath));
    }

    public Stream OpenFile(string filename, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
    {
      try
      {
        return (Stream) this.fs.OpenFile(filename, mode, access, FileShare.ReadWrite | FileShare.Delete);
      }
      catch (Exception ex)
      {
        string context = string.Format("Failed to open file: {0}", (object) filename);
        Log.LogException(ex, context, false);
        throw;
      }
    }

    public bool FileExists(string filename) => this.fs.FileExists(filename);

    public void MoveFile(string src, string dst)
    {
      if (this.fs.DirectoryExists(src))
        this.fs.MoveDirectory(src, dst);
      else
        this.fs.MoveFile(src, dst);
    }

    public void DeleteFile(string path) => this.fs.DeleteFile(path);

    public void CreateDirectory(string path) => this.fs.CreateDirectory(path);

    public void RemoveDirectory(string path) => this.fs.DeleteDirectory(path);

    public bool IsDirectory(string path) => this.fs.DirectoryExists(path);
  }
}
