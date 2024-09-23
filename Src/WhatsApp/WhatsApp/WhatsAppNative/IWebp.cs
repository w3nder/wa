// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWebp
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2765924098, 45226, 18656, 170, 177, 144, 9, 34, 69, 104, 107)]
  public interface IWebp
  {
    int GetVersion();

    int GetWidth();

    int GetHeight();

    int HasAlpha();

    uint GetCanvasHeight();

    uint GetCanvasWidth();

    uint GetLoopCount();

    uint GetBgColor();

    uint GetFrameCount();

    int Initialise([In] IByteBuffer InputBuffer);

    int Decode([In] IByteBuffer OutputBuffer);

    void Dispose();
  }
}
