// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.NativeWebp
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
  [Activatable(100794368)]
  [MarshalingBehavior]
  public sealed class NativeWebp : IWebp
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern NativeWebp();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetVersion();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetWidth();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetHeight();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int HasAlpha();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetCanvasHeight();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetCanvasWidth();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetLoopCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetBgColor();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetFrameCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int Initialise([In] IByteBuffer InputBuffer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int Decode([In] IByteBuffer OutputBuffer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();
  }
}
