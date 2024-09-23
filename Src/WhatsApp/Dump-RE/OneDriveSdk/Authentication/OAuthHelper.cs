// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.OAuthHelper
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class OAuthHelper
  {
    public async Task<string> GetAuthorizationCodeAsync(
      string clientId,
      string returnUrl,
      string[] scopes,
      IWebAuthenticationUi webAuthenticationUi,
      string userId = null)
    {
      if (webAuthenticationUi != null)
      {
        IDictionary<string, string> responseValues = await webAuthenticationUi.AuthenticateAsync(new Uri(this.GetAuthorizationCodeRequestUrl(clientId, returnUrl, scopes, userId)), new Uri(returnUrl)).ConfigureAwait(false);
        OAuthErrorHandler.ThrowIfError(responseValues);
        string code;
        if (responseValues != null && responseValues.TryGetValue("code", out code))
          return code;
        code = (string) null;
      }
      return (string) null;
    }

    public string GetAuthorizationCodeRequestUrl(
      string clientId,
      string returnUrl,
      string[] scopes,
      string userId = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("https://login.live.com/oauth20_authorize.srf");
      stringBuilder.AppendFormat("?{0}={1}", (object) "redirect_uri", (object) returnUrl);
      stringBuilder.AppendFormat("&{0}={1}", (object) "client_id", (object) clientId);
      if (scopes != null)
        stringBuilder.AppendFormat("&{0}={1}", (object) "scope", (object) WebUtility.UrlEncode(string.Join(" ", scopes)));
      if (!string.IsNullOrEmpty(userId))
        stringBuilder.AppendFormat("&{0}={1}", (object) "user_id", (object) userId);
      stringBuilder.AppendFormat("&{0}={1}", (object) "response_type", (object) "code");
      return stringBuilder.ToString();
    }

    public string GetAuthorizationCodeRedemptionRequestBody(
      string code,
      string clientId,
      string returnUrl,
      string[] scopes,
      string clientSecret = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}={1}", (object) "redirect_uri", (object) returnUrl);
      stringBuilder.AppendFormat("&{0}={1}", (object) "client_id", (object) clientId);
      if (scopes != null)
        stringBuilder.AppendFormat("&{0}={1}", (object) "scope", (object) WebUtility.UrlEncode(string.Join(" ", scopes)));
      stringBuilder.AppendFormat("&{0}={1}", (object) nameof (code), (object) code);
      stringBuilder.AppendFormat("&{0}={1}", (object) "grant_type", (object) "authorization_code");
      if (!string.IsNullOrEmpty(clientSecret))
        stringBuilder.AppendFormat("&client_secret={0}", (object) clientSecret);
      return stringBuilder.ToString();
    }

    public string GetRefreshTokenRequestBody(
      string refreshToken,
      string clientId,
      string returnUrl,
      string[] scopes,
      string clientSecret = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}={1}", (object) "redirect_uri", (object) returnUrl);
      stringBuilder.AppendFormat("&{0}={1}", (object) "client_id", (object) clientId);
      if (scopes != null)
        stringBuilder.AppendFormat("&{0}={1}", (object) "scope", (object) WebUtility.UrlEncode(string.Join(" ", scopes)));
      stringBuilder.AppendFormat("&{0}={1}", (object) "refresh_token", (object) refreshToken);
      stringBuilder.AppendFormat("&{0}={1}", (object) "grant_type", (object) "refresh_token");
      if (!string.IsNullOrEmpty(clientSecret))
        stringBuilder.AppendFormat("&{0}={1}", (object) "client_secret", (object) clientSecret);
      return stringBuilder.ToString();
    }

    public string GetSignOutUrl(string clientId, string returnUrl)
    {
      return string.Format("{0}?client_id={1}&redirect_uri={2}", (object) "https://login.live.com/oauth20_logout.srf", (object) clientId, (object) returnUrl);
    }

    public async Task<AccountSession> RedeemAuthorizationCodeAsync(
      string authorizationCode,
      string clientId,
      string clientSecret,
      string returnUrl,
      string[] scopes)
    {
      AccountSession accountSession;
      using (HttpProvider httpProvider = new HttpProvider())
        accountSession = await this.RedeemAuthorizationCodeAsync(authorizationCode, clientId, clientSecret, returnUrl, scopes, (IHttpProvider) httpProvider).ConfigureAwait(false);
      return accountSession;
    }

    public async Task<AccountSession> RedeemAuthorizationCodeAsync(
      string authorizationCode,
      string clientId,
      string clientSecret,
      string returnUrl,
      string[] scopes,
      IHttpProvider httpProvider)
    {
      if (string.IsNullOrEmpty(authorizationCode))
        throw new ServiceException(new Error()
        {
          Code = "authenticationFailure",
          Message = "Authorization code is required to redeem."
        });
      return await this.SendTokenRequestAsync(this.GetAuthorizationCodeRedemptionRequestBody(authorizationCode, clientId, returnUrl, scopes, clientSecret), httpProvider).ConfigureAwait(false);
    }

    public async Task<AccountSession> RedeemRefreshTokenAsync(
      string refreshToken,
      string clientId,
      string returnUrl,
      string[] scopes)
    {
      return await this.RedeemRefreshTokenAsync(refreshToken, clientId, (string) null, returnUrl, scopes, (IHttpProvider) null).ConfigureAwait(false);
    }

    public async Task<AccountSession> RedeemRefreshTokenAsync(
      string refreshToken,
      string clientId,
      string returnUrl,
      string[] scopes,
      IHttpProvider httpProvider)
    {
      return await this.RedeemRefreshTokenAsync(refreshToken, clientId, (string) null, returnUrl, scopes, httpProvider).ConfigureAwait(false);
    }

    public async Task<AccountSession> RedeemRefreshTokenAsync(
      string refreshToken,
      string clientId,
      string clientSecret,
      string returnUrl,
      string[] scopes)
    {
      return await this.RedeemRefreshTokenAsync(refreshToken, clientId, clientSecret, returnUrl, scopes, (IHttpProvider) null).ConfigureAwait(false);
    }

    public async Task<AccountSession> RedeemRefreshTokenAsync(
      string refreshToken,
      string clientId,
      string clientSecret,
      string returnUrl,
      string[] scopes,
      IHttpProvider httpProvider)
    {
      if (string.IsNullOrEmpty(refreshToken))
        throw new ServiceException(new Error()
        {
          Code = "authenticationFailure",
          Message = "Refresh token is required to redeem."
        });
      string tokenRequestBody = this.GetRefreshTokenRequestBody(refreshToken, clientId, returnUrl, scopes, clientSecret);
      return httpProvider == null ? await this.SendTokenRequestAsync(tokenRequestBody) : await this.SendTokenRequestAsync(tokenRequestBody, httpProvider).ConfigureAwait(false);
    }

    public async Task<AccountSession> SendTokenRequestAsync(string requestBodyString)
    {
      AccountSession accountSession;
      using (HttpProvider httpProvider = new HttpProvider())
        accountSession = await this.SendTokenRequestAsync(requestBodyString, (IHttpProvider) httpProvider).ConfigureAwait(false);
      return accountSession;
    }

    public async Task<AccountSession> SendTokenRequestAsync(
      string requestBodyString,
      IHttpProvider httpProvider)
    {
      AccountSession accountSession;
      using (HttpResponseMessage authResponse = await httpProvider.SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://login.live.com/oauth20_token.srf")
      {
        Content = (HttpContent) new StringContent(requestBodyString, Encoding.UTF8, "application/x-www-form-urlencoded")
      }).ConfigureAwait(false))
      {
        using (Stream stream = await authResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
        {
          IDictionary<string, string> dictionary = httpProvider.Serializer.DeserializeObject<IDictionary<string, string>>(stream);
          if (dictionary != null)
          {
            OAuthErrorHandler.ThrowIfError(dictionary);
            accountSession = new AccountSession(dictionary);
          }
          else
            throw new ServiceException(new Error()
            {
              Code = "authenticationFailure",
              Message = "Authentication failed. No response values returned from authentication flow."
            });
        }
      }
      return accountSession;
    }
  }
}
