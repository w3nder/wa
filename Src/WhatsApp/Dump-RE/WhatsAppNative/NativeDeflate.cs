// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.NativeDeflate
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [MarshalingBehavior]
  [Activatable(100794368)]
  public sealed class NativeDeflate : IDeflate
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern NativeDeflate();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize([In] int level);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetInputBuffer([In] IByteBuffer buf);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetOutputBuffer([In] IByteBuffer buf);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetInputLength([In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetOutputLength();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ResetOutputBuffer();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool Deflate([In] bool Flush);
  }
}
