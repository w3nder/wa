// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebAuthenticationManager
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2491489051, 13294, 19066, 152, 188, 14, 87, 112, 86, 125, 151)]
  public interface IWAWebAuthenticationManager
  {
    IAsyncOperation<WAWebAccountProvider> FindAccountProviderAsync([In] string webAccountProviderId);

    IAsyncOperation<WAWebAccount> FindAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] string webAccountId);

    IAsyncOperation<WAWebTokenRequestResult> RequestTokenAsync(
      [In] WAWebAccountProvider provider,
      [In] string scope);

    IAsyncOperation<WAWebTokenRequestResult> RequestTokenWithWebAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] WAWebAccount account,
      [In] string scope);

    IAsyncOperation<WAWebTokenRequestResult> GetTokenSilentlyAsync(
      [In] WAWebAccountProvider provider,
      [In] string scope);

    IAsyncOperation<WAWebTokenRequestResult> GetTokenSilentlyWithWebAccountAsync(
      [In] WAWebAccountProvider provider,
      [In] WAWebAccount account,
      [In] string scope);
  }
}
