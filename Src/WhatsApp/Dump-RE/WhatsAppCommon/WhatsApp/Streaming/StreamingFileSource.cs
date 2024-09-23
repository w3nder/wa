// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.StreamingFileSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;

#nullable disable
namespace WhatsApp.Streaming
{
  public class StreamingFileSource : IStreamingFileSource, IDisposable
  {
    private Stream fileStream;
    private Action @throw;

    public StreamingFileSource(string fileName)
    {
      this.fileStream = MediaStorage.OpenFile(fileName);
    }

    public int Read(byte[] buf, int pos, int len)
    {
      try
      {
        return this.fileStream.Read(buf, pos, len);
      }
      catch (Exception ex)
      {
        this.OnIoError(ex);
        throw;
      }
    }

    public long GetFullFileSize() => this.fileStream.Length;

    public void Seek(long offset)
    {
      try
      {
        this.fileStream.Seek(offset, SeekOrigin.Begin);
      }
      catch (Exception ex)
      {
        this.OnIoError(ex);
        throw;
      }
    }

    public void Dispose() => this.fileStream.Dispose();

    private void OnIoError(Exception ex) => this.@throw = ex.GetRethrowAction();

    public void CheckIoError()
    {
      Action action = this.@throw;
      if (action == null)
        return;
      action();
    }
  }
}
