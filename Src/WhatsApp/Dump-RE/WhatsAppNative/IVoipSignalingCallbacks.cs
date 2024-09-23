// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IVoipSignalingCallbacks
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2628790553, 42663, 20063, 128, 211, 232, 214, 182, 103, 109, 198)]
  public interface IVoipSignalingCallbacks
  {
    void OnSignalingData([In] byte[] Ptr, [In] SignalingDataArgs Args);
  }
}
