// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebTokenResponse
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class WAWebTokenResponse : IWAWebTokenResponse
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebTokenResponse();

    [Version(100794368)]
    public extern string Token { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern WAWebProviderError ProviderError { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern WAWebAccount WebAccount { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern IMap<string, string> Properties { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
