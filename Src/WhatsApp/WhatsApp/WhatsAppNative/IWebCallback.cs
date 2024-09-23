// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWebCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(505767225, 15915, 19363, 161, 144, 110, 33, 125, 190, 190, 214)]
  [Version(100794368)]
  public interface IWebCallback
  {
    void Write([In] IWebWriter Writer);

    void OnResponseCode([In] int Code);

    void OnHeaders([In] string Headers);

    void ResponseBytesIn([In] IByteBuffer ByteBuffer);

    void EndResponse();
  }
}
