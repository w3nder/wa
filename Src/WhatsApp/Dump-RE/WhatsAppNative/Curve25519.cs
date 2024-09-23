// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Curve25519
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class Curve25519 : ICurve25519
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Curve25519();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GenKeyPair(out IByteBuffer PubKey, out IByteBuffer PrivKey);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer Derive([In] IByteBuffer PubKey, [In] IByteBuffer PrivKey);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer Sign([In] IByteBuffer Message, [In] IByteBuffer SignKey);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool Verify([In] IByteBuffer Message, [In] IByteBuffer Signature, [In] IByteBuffer SignKey);
  }
}
