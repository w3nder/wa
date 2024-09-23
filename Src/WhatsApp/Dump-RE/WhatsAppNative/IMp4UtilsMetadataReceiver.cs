// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMp4UtilsMetadataReceiver
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(944628575, 45936, 18888, 137, 135, 181, 8, 104, 83, 4, 100)]
  public interface IMp4UtilsMetadataReceiver
  {
    void OnVideoMetadata([In] int Width, [In] int Height, [In] float Fps, [In] float Duration, [In] int RotationAngle);

    void OnAudioMetadata(
      [In] int SampleRate,
      [In] int Channels,
      [In] int BitsPerSample,
      [In] int BytesPerSecond,
      [In] int BlockAlign,
      [In] float Duration);

    void OnMetadataComplete();
  }
}
