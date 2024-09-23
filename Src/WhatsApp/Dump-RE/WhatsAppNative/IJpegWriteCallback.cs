// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IJpegWriteCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(965439331, 47889, 18943, 165, 116, 162, 218, 38, 24, 214, 34)]
  public interface IJpegWriteCallback
  {
    void Write([In] byte[] bytes);
  }
}
