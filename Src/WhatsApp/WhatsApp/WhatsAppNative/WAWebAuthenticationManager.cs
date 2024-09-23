// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebAuthenticationManager
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Activatable(100794368)]
  [MarshalingBehavior]
  public sealed class WAWebAuthenticationManager : IWAWebAuthenticationManager
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebAuthenticationManager();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebAccountProvider> FindAccountProviderAsync(
      [In] string webAccountProviderId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebAccount> FindAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] string webAccountId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebTokenRequestResult> RequestTokenAsync(
      [In] WAWebAccountProvider provider,
      [In] string scope);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebTokenRequestResult> RequestTokenWithWebAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] WAWebAccount account,
      [In] string scope);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebTokenRequestResult> GetTokenSilentlyAsync(
      [In] WAWebAccountProvider provider,
      [In] string scope);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncOperation<WAWebTokenRequestResult> GetTokenSilentlyWithWebAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] WAWebAccount account,
      [In] string scope);
  }
}
