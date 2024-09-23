// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.MediaMisc
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class MediaMisc : IMediaMisc
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern MediaMisc();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void StripMetadata([In] string Filename);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IVideoUtils OpenVideo([In] IWAStream Stream, [In] bool Rgb);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ScaleImage(
      [In] IByteBuffer Buffer,
      [In] int Width,
      [In] int Height,
      [In] int Stride,
      [In] int DesiredWidth,
      [In] int DesiredHeight);
  }
}
