// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Transcoder
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
  public sealed class Transcoder : ITranscoder
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Transcoder();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize(
      [In] IVideoUtils video,
      [In] ISoundSource audio,
      [In] TranscoderContainerType outputContainer,
      [In] IWAStream output);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Seek([In] long millis);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Transcode([In] long durationMillisOrNegative, [In] ITranscoderProgress progress);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Cancel();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetEncoderScheduler([In] IWAScheduler scheduler);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void AddImageTransform([In] IImageTransform transform);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IImageTransform CreateClipRectangleTransform([In] uint x, [In] uint y, [In] uint w, [In] uint h);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IImageTransform CreateMaxEdgeTransform([In] uint maxEdge);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IImageTransform CreateRotateTransform([In] uint exifOrientation);
  }
}
