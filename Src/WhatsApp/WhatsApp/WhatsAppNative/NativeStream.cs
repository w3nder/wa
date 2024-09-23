// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.NativeStream
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [MarshalingBehavior]
  public sealed class NativeStream : IWAStream
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern StreamFlags GetFlags();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetPosition();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long Seek([In] long Offset, [In] uint Whence);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetLength();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetLength([In] long Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void WriteBase([In] uint Buf, [In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int ReadBase([In] uint Buf, [In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Write([In] IByteBuffer buf, [In] int Offset, [In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int Read([In] IByteBuffer buf, [In] int Offset, [In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Flush();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IWAStream Clone();
  }
}
