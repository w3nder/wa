// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.AxolotlFastRatchetGroupCipherNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Activatable(100794368)]
  [Version(100794368)]
  public sealed class AxolotlFastRatchetGroupCipherNative : IAxolotlGroupCipherNative
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern AxolotlFastRatchetGroupCipherNative();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize(
      [In] string GroupId,
      [In] string SenderId,
      [In] IAxolotlSessionCipherCallbacks ManagedCallbacks,
      [In] IAxolotlNative AxolotlNative);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer EncryptMessage([In] IByteBuffer PlainText);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DecryptMessage([In] IByteBuffer CipherText);
  }
}
