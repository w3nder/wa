// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.RecToken
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class RecToken : IRecToken
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern RecToken();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer GetToken([In] int TokenOffset);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetSalt([In] IByteBuffer Salt);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Encode([In] IByteBuffer Bytes);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Decode([In] IByteBuffer Bytes);
  }
}
