// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.IOAuthRequestStringBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public interface IOAuthRequestStringBuilder
  {
    string GetAuthorizationCodeRequestUrl(
      string appId,
      string returnUrl,
      string[] scopes,
      string userId = null);

    string GetCodeRedemptionRequestBody(
      string code,
      string appId,
      string returnUrl,
      string[] scopes,
      string clientSecret = null);

    string GetRefreshTokenRequestBody(
      string refreshToken,
      string appId,
      string returnUrl,
      string[] scopes,
      string clientSecret = null);
  }
}
