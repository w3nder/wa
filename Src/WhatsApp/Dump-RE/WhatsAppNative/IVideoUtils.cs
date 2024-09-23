// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IVideoUtils
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(1739959225, 31791, 16688, 169, 91, 187, 198, 126, 184, 1, 188)]
  [Version(100794368)]
  public interface IVideoUtils
  {
    void Dispose();

    long GetDuration();

    void Seek([In] long Ticks);

    bool GetFrame([In] ISampleSink Sink);

    FRAME_ATTRIBUTES GetFrameAttributes();

    int GetStride();

    uint GetD3dPointers();

    void SetDirectSampleMode([In] bool Enabled);
  }
}
