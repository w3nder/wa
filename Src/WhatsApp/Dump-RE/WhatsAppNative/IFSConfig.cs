// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IFSConfig
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(3781793151, 39520, 18366, 171, 71, 231, 245, 68, 40, 90, 132)]
  [Version(100794368)]
  public interface IFSConfig
  {
    void GetKnownValues([In] IFieldStats stats);

    uint GetMaxSendInterval();
  }
}
