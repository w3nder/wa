// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IHost
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(3775407513, 51294, 17763, 168, 167, 217, 157, 1, 136, 39, 4)]
  [Version(100794368)]
  public interface IHost
  {
    void AddHost([In] string HostName);

    void SetResolver([In] IHostResolver Resolver);

    void SetShuffleMode([In] HostShuffleMode mode);
  }
}
