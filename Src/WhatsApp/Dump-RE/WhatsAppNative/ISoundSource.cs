// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISoundSource
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1886368661, 29824, 20121, 166, 21, 232, 126, 61, 188, 54, 188)]
  public interface ISoundSource
  {
    AudioMetadata GetMetadata();

    bool FillBuffer([In] IByteBuffer buf);

    void Seek([In] long Millis);

    long GetPosition();

    long GetDuration();
  }
}
