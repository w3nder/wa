// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.IStreamingFileSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp.Streaming
{
  public interface IStreamingFileSource : IDisposable
  {
    int Read(byte[] buf, int pos, int len);

    void Seek(long pos);

    long GetFullFileSize();

    void CheckIoError();
  }
}
