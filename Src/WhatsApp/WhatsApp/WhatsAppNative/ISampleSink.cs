// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISampleSink
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(907527691, 12939, 19781, 152, 217, 90, 66, 192, 192, 59, 237)]
  [Version(100794368)]
  public interface ISampleSink
  {
    void OnSampleAvailable([In] IByteBuffer buffer, [In] long timestamp, [In] long duration);
  }
}
