// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebTokenRequestResult
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [MarshalingBehavior]
  [Activatable(100794368)]
  public sealed class WAWebTokenRequestResult : IWAWebTokenRequestResult
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebTokenRequestResult();

    [Version(100794368)]
    public extern WAWebTokenResponse ResponseData { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern WAWebTokenRequestStatus ResponseStatus { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern WAWebProviderError ResponseError { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncAction InvalidateCacheAsync();
  }
}
