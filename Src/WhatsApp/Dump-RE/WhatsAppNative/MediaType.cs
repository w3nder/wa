// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.MediaType
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  public struct MediaType
  {
    public MediaContainerType Container;
    public Mp4VideoStreamType VideoStreamType;
    public Mp4AudioStreamType AudioStreamType;
    public Mp4AudioSubType AudioSubtype;
    public bool FormatProblemsFound;
  }
}
