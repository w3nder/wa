// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IAxolotlGroupCipherNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(276351650, 12877, 16830, 140, 171, 168, 118, 87, 195, 38, 22)]
  [Version(100794368)]
  public interface IAxolotlGroupCipherNative
  {
    void Initialize(
      [In] string GroupId,
      [In] string SenderId,
      [In] IAxolotlSessionCipherCallbacks ManagedCallbacks,
      [In] IAxolotlNative AxolotlNative);

    IByteBuffer EncryptMessage([In] IByteBuffer PlainText);

    void DecryptMessage([In] IByteBuffer CipherText);
  }
}
