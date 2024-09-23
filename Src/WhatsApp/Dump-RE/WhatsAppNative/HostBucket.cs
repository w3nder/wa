// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.HostBucket
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class HostBucket : IHost
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern HostBucket();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void AddHost([In] string HostName);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetResolver([In] IHostResolver Resolver);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetShuffleMode([In] HostShuffleMode mode);
  }
}
