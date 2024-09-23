// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMp4UtilsWriteCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1338934024, 11304, 16404, 152, 174, 89, 155, 155, 198, 28, 28)]
  public interface IMp4UtilsWriteCallback
  {
    void Write([In] byte[] bytes, [In] bool isSeekPoint, [In] ulong timestamp);
  }
}
