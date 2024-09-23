// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.MsaAuthenticationProvider
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class MsaAuthenticationProvider : IAuthenticationProvider
  {
    internal readonly string clientId;
    internal string clientSecret;
    internal string returnUrl;
    internal string[] scopes;
    private OAuthHelper oAuthHelper;
    internal ICredentialVault credentialVault;
    internal IWebAuthenticationUi webAuthenticationUi;

    public CredentialCache CredentialCache { get; private set; }

    public AccountSession CurrentAccountSession { get; set; }

    public bool IsAuthenticated => this.CurrentAccountSession != null;

    public async Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
      AccountSession accountSession = await this.ProcessCachedAccountSessionAsync(this.CurrentAccountSession).ConfigureAwait(false);
      if (accountSession == null)
        throw new ServiceException(new Error()
        {
          Code = "authenticationFailure",
          Message = "Unable to retrieve a valid account session for the user. Please call AuthenticateUserAsync to prompt the user to re-authenticate."
        });
      if (string.IsNullOrEmpty(accountSession.AccessToken))
        return;
      request.Headers.Authorization = new AuthenticationHeaderValue(string.IsNullOrEmpty(accountSession.AccessTokenType) ? "bearer" : accountSession.AccessTokenType, accountSession.AccessToken);
    }

    public virtual async Task SignOutAsync()
    {
      if (!this.IsAuthenticated)
        return;
      if (this.webAuthenticationUi != null)
      {
        Uri requestUri = new Uri(this.oAuthHelper.GetSignOutUrl(this.clientId, this.returnUrl));
        try
        {
          IDictionary<string, string> dictionary = await this.webAuthenticationUi.AuthenticateAsync(requestUri, new Uri(this.returnUrl)).ConfigureAwait(false);
        }
        catch (ServiceException ex)
        {
          if (!ex.IsMatch("authenticationCancelled"))
            throw;
        }
      }
      this.DeleteUserCredentialsFromCache(this.CurrentAccountSession);
      this.CurrentAccountSession = (AccountSession) null;
    }

    protected void CacheAuthResult(AccountSession accountSession)
    {
      this.CurrentAccountSession = accountSession;
      if (this.CredentialCache == null)
        return;
      this.CredentialCache.AddToCache(accountSession);
    }

    protected void DeleteUserCredentialsFromCache(AccountSession accountSession)
    {
      if (this.CredentialCache == null)
        return;
      this.CredentialCache.DeleteFromCache(accountSession);
    }

    public async Task RestoreMostRecentFromCacheOrAuthenticateUserAsync(string userName = null)
    {
      using (HttpProvider httpProvider = new HttpProvider())
        await this.RestoreMostRecentFromCacheOrAuthenticateUserAsync((IHttpProvider) httpProvider, userName).ConfigureAwait(false);
    }

    public async Task RestoreMostRecentFromCacheOrAuthenticateUserAsync(
      IHttpProvider httpProvider,
      string userName = null)
    {
      AccountSession authResult = await this.GetMostRecentAuthenticationResultFromCacheAsync(httpProvider).ConfigureAwait(false);
      if (authResult == null)
        await this.AuthenticateUserAsync(httpProvider, userName);
      else
        this.CacheAuthResult(authResult);
    }

    public async Task AuthenticateUserAsync(string userName = null)
    {
      using (HttpProvider httpProvider = new HttpProvider())
        await this.AuthenticateUserAsync((IHttpProvider) httpProvider, userName).ConfigureAwait(false);
    }

    public virtual async Task AuthenticateUserAsync(IHttpProvider httpProvider, string userName = null)
    {
      AccountSession authResult = await this.GetAuthenticationResultFromCacheAsync(userName, httpProvider).ConfigureAwait(false);
      if (authResult == null)
      {
        string authorizationCode = await this.oAuthHelper.GetAuthorizationCodeAsync(this.clientId, this.returnUrl, this.scopes, this.webAuthenticationUi, userName).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(authorizationCode))
          authResult = await this.oAuthHelper.RedeemAuthorizationCodeAsync(authorizationCode, this.clientId, this.clientSecret, this.returnUrl, this.scopes, httpProvider).ConfigureAwait(false);
        if (authResult == null || string.IsNullOrEmpty(authResult.AccessToken))
          throw new ServiceException(new Error()
          {
            Code = "authenticationFailure",
            Message = "Failed to retrieve a valid authentication token for the user."
          });
      }
      this.CacheAuthResult(authResult);
    }

    internal async Task<AccountSession> GetAuthenticationResultFromCacheAsync(
      string userId,
      IHttpProvider httpProvider)
    {
      AccountSession resultFromCacheAsync1 = await this.ProcessCachedAccountSessionAsync(this.CurrentAccountSession, httpProvider).ConfigureAwait(false);
      if (resultFromCacheAsync1 != null)
        return resultFromCacheAsync1;
      if (string.IsNullOrEmpty(userId) && this.CurrentAccountSession != null)
        userId = this.CurrentAccountSession.UserId;
      AccountSession cacheResult = this.CredentialCache.GetResultFromCache(this.clientId, userId);
      AccountSession resultFromCacheAsync2 = await this.ProcessCachedAccountSessionAsync(cacheResult, httpProvider).ConfigureAwait(false);
      if (resultFromCacheAsync2 != null || cacheResult == null)
        return resultFromCacheAsync2;
      this.CredentialCache.DeleteFromCache(cacheResult);
      this.CurrentAccountSession = (AccountSession) null;
      return (AccountSession) null;
    }

    internal async Task<AccountSession> GetMostRecentAuthenticationResultFromCacheAsync(
      IHttpProvider httpProvider)
    {
      AccountSession cacheResult = this.CredentialCache.GetMostRecentlyUsedResultFromCache();
      AccountSession resultFromCacheAsync = await this.ProcessCachedAccountSessionAsync(cacheResult, httpProvider).ConfigureAwait(false);
      if (resultFromCacheAsync != null || cacheResult == null)
        return resultFromCacheAsync;
      this.CredentialCache.DeleteFromCache(cacheResult);
      this.CurrentAccountSession = (AccountSession) null;
      return (AccountSession) null;
    }

    internal async Task<AccountSession> ProcessCachedAccountSessionAsync(
      AccountSession accountSession)
    {
      AccountSession accountSession1;
      using (HttpProvider httpProvider = new HttpProvider())
        accountSession1 = await this.ProcessCachedAccountSessionAsync(accountSession, (IHttpProvider) httpProvider).ConfigureAwait(false);
      return accountSession1;
    }

    internal virtual async Task<AccountSession> ProcessCachedAccountSessionAsync(
      AccountSession accountSession,
      IHttpProvider httpProvider)
    {
      if (accountSession != null)
      {
        bool shouldRefresh = accountSession.ShouldRefresh;
        if (shouldRefresh && accountSession.CanRefresh)
        {
          accountSession = await this.oAuthHelper.RedeemRefreshTokenAsync(accountSession.RefreshToken, this.clientId, this.clientSecret, this.returnUrl, this.scopes, httpProvider).ConfigureAwait(false);
          if (accountSession != null && !string.IsNullOrEmpty(accountSession.AccessToken))
            return accountSession;
        }
        else if (!shouldRefresh)
          return accountSession;
      }
      return (AccountSession) null;
    }
  }
}
