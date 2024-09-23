// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAudioArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;


namespace WhatsApp
{
  public class WaAudioArgs : EventArgs
  {
    public StreamingUploadContext AudioStreamingUploadContext;
    public Stream Stream;
    public int Duration;
    public string MimeType;
    public string FileExtension;
    public byte[] Thumbnail;
    public string FullPath;
    public ulong FileSize;
    public bool C2cStarted;

    public Message QuotedMessage { get; set; }

    public string QuotedChat { get; set; }
  }
}
