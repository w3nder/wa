﻿// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.VideoParams
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public struct VideoParams
  {
    public VideoState VideoState;
    public uint EncSupported;
    public VideoCodec Codec;
    public VideoOrientation Orientation;
  }
}