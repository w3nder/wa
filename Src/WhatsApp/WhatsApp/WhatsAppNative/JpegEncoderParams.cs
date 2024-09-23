// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.JpegEncoderParams
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public struct JpegEncoderParams
  {
    public int SourceWidth;
    public int SourceHeight;
    public int SourceStride;
    public int Quality;
    public bool UseWaProgJpeg;
  }
}
