// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWebWriter
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(1682284871, 51415, 20036, 176, 114, 25, 40, 126, 216, 58, 204)]
  [Version(100794368)]
  public interface IWebWriter
  {
    void Write([In] IByteBuffer Buf);
  }
}
