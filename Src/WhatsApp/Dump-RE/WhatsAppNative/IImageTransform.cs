// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IImageTransform
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2617184648, 62470, 19640, 178, 49, 223, 180, 115, 117, 112, 114)]
  public interface IImageTransform
  {
    void OnMetadata(
      [In] FRAME_ATTRIBUTES InputDimensions,
      [In] IMAGE_OFFSETS InputOffsets,
      out FRAME_ATTRIBUTES Dimensions,
      out IMAGE_OFFSETS OutputOffsets);

    void Transform([In] IByteBuffer Pixels);
  }
}
