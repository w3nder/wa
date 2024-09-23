// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IOpusEnc
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(3472854978, 5173, 17064, 181, 65, 30, 201, 223, 246, 1, 192)]
  [Version(100794368)]
  public interface IOpusEnc
  {
    void Initialize([In] OpusEncoderParams EncoderParams);

    void OnSamples([In] IByteBuffer buffer);

    void Flush();

    IByteBuffer GetBuffer();

    void Dispose();
  }
}
