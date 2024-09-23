// Decompiled with JetBrains decompiler
// Type: WhatsApp.IMediaStorage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;

#nullable disable
namespace WhatsApp
{
  public interface IMediaStorage : IDisposable
  {
    string GetFullFsPath(string localPath);

    Stream OpenFile(string filename, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read);

    bool FileExists(string filename);

    void MoveFile(string src, string dst);

    void DeleteFile(string path);

    void CreateDirectory(string path);

    void RemoveDirectory(string path);
  }
}
