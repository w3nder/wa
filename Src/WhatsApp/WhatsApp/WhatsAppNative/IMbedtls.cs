// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMbedtls
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2985233154, 16391, 20029, 139, 251, 250, 150, 240, 146, 240, 253)]
  [Version(100794368)]
  public interface IMbedtls
  {
    IByteBuffer AesGcmEncrypt(
      [In] IByteBuffer CipherKeyBuffer,
      [In] IByteBuffer IvBuffer,
      [In] IByteBuffer AddBuffer,
      [In] IByteBuffer PlainTextBuffer);

    IByteBuffer AesGcmDecrypt(
      [In] IByteBuffer CipherKeyBuffer,
      [In] IByteBuffer IvBuffer,
      [In] IByteBuffer AddBuffer,
      [In] IByteBuffer CipherTextBuffer);
  }
}
