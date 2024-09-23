// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ITranscoder
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(161171354, 22692, 18048, 135, 71, 241, 181, 253, 54, 223, 107)]
  [Version(100794368)]
  public interface ITranscoder
  {
    void Initialize(
      [In] IVideoUtils video,
      [In] ISoundSource audio,
      [In] TranscoderContainerType outputContainer,
      [In] IWAStream output);

    void Seek([In] long millis);

    void Transcode([In] long durationMillisOrNegative, [In] ITranscoderProgress progress);

    void Cancel();

    void SetEncoderScheduler([In] IWAScheduler scheduler);

    void AddImageTransform([In] IImageTransform transform);

    IImageTransform CreateClipRectangleTransform([In] uint x, [In] uint y, [In] uint w, [In] uint h);

    IImageTransform CreateMaxEdgeTransform([In] uint maxEdge);

    IImageTransform CreateRotateTransform([In] uint exifOrientation);
  }
}
