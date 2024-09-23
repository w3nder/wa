// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.IStreamingMp4DemuxCallbacks
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsAppNative;


namespace WhatsApp.Streaming
{
  public interface IStreamingMp4DemuxCallbacks : IMp4UtilsMetadataReceiver
  {
    void OnError(Exception e);

    void OnVideoBytes(byte[] h264, int offset, int length, bool isSeekPoint, ulong timestamp);

    void OnAudioBytes(byte[] aac, int offset, int length, bool isSeekPoint, ulong timestamp);

    void OnEndOfFile();
  }
}
