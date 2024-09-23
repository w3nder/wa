// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWASocketHandler
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(3913214146, 31721, 17060, 175, 166, 224, 67, 47, 30, 88, 15)]
  [Version(100794368)]
  public interface IWASocketHandler
  {
    void Connected();

    void Disconnected([In] uint HResult);

    void BytesIn([In] IByteBuffer Buffer);

    void WriteBufferDrained();
  }
}
