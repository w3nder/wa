// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.AxolotlSessionCipherNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class AxolotlSessionCipherNative : IAxolotlSessionCipherNative
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern AxolotlSessionCipherNative();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize(
      [In] string RecipientId,
      [In] IAxolotlSessionCipherCallbacks ManagedCallbacks,
      [In] IAxolotlNative AxolotlNative);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EncryptMessage(
      [In] IByteBuffer PlainText,
      out int CipherTextType,
      out IByteBuffer CipherText);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DecryptPreKeyMessage([In] IByteBuffer CipherText);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DecryptMessage([In] IByteBuffer CipherText);
  }
}
