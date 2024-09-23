// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMediaMisc
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(2597102381, 61705, 16688, 139, 137, 224, 3, 78, 73, 42, 95)]
  [Version(100794368)]
  public interface IMediaMisc
  {
    void StripMetadata([In] string Filename);

    IVideoUtils OpenVideo([In] IWAStream Stream, [In] bool Rgb);

    void ScaleImage(
      [In] IByteBuffer Buffer,
      [In] int Width,
      [In] int Height,
      [In] int Stride,
      [In] int DesiredWidth,
      [In] int DesiredHeight);
  }
}
