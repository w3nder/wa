// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaWebAuthenticationProvider
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using WhatsAppNative;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace WhatsApp
{
  public class WaWebAuthenticationProvider : IWaAuthenticationProvider, IAuthenticationProvider
  {
    private readonly int ticketExpirationTimeInMinutes = 59;
    private readonly string[] scopes;
    private WAWebAuthenticationManager authManager;

    public WaWebAuthenticationProvider(string[] scopes)
      : this(scopes, (CredentialCache) null)
    {
    }

    public WaWebAuthenticationProvider(string[] scopes, CredentialCache credentialCache)
    {
      this.scopes = scopes;
      this.CredentialCache = credentialCache ?? new CredentialCache();
      this.authManager = NativeInterfaces.CreateInstance<WAWebAuthenticationManager>();
    }

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

    public async Task SignOutAsync()
    {
      if (!this.IsAuthenticated)
        return;
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

    public async Task AuthenticateUserAsync(
      CredentialPromptType promptType,
      WAWebAccountProvider selectedProvider = null)
    {
      AccountSession accountSession = await this.GetAuthenticationResultFromCacheAsync().ConfigureAwait(false);
      if (accountSession == null)
        accountSession = await this.GetAccountSessionAsync(promptType, selectedProvider);
      this.CacheAuthResult(accountSession);
    }

    internal async Task<AccountSession> GetAuthenticationResultFromCacheAsync()
    {
      ConfiguredTaskAwaitable<AccountSession> configuredTaskAwaitable = this.ProcessCachedAccountSessionAsync(this.CurrentAccountSession).ConfigureAwait(false);
      AccountSession resultFromCacheAsync1 = await configuredTaskAwaitable;
      if (resultFromCacheAsync1 != null)
        return resultFromCacheAsync1;
      AccountSession cacheResult = this.CredentialCache.GetResultFromCache(this.CurrentAccountSession?.ClientId, this.CurrentAccountSession?.UserId);
      configuredTaskAwaitable = this.ProcessCachedAccountSessionAsync(cacheResult).ConfigureAwait(false);
      AccountSession resultFromCacheAsync2 = await configuredTaskAwaitable;
      if (resultFromCacheAsync2 != null || cacheResult == null)
        return resultFromCacheAsync2;
      this.CredentialCache.DeleteFromCache(cacheResult);
      this.CurrentAccountSession = (AccountSession) null;
      return (AccountSession) null;
    }

    internal async Task<AccountSession> ProcessCachedAccountSessionAsync(
      AccountSession accountSession)
    {
      if (accountSession != null)
      {
        if (!accountSession.ShouldRefresh)
          return accountSession;
        accountSession = await this.GetAccountSessionAsync((CredentialPromptType) 2);
        if (accountSession != null && !string.IsNullOrEmpty(accountSession.AccessToken))
        {
          this.CacheAuthResult(accountSession);
          return accountSession;
        }
      }
      return (AccountSession) null;
    }

    private async Task<AccountSession> GetAccountSessionAsync(
      CredentialPromptType promptType,
      WAWebAccountProvider selectedProvider = null)
    {
      string authScope = string.Join(" ", this.scopes);
      WAWebAccount account;
      WAWebAccountProvider accountProvider;
      if (selectedProvider != null)
      {
        accountProvider = selectedProvider;
        account = (WAWebAccount) null;
      }
      else
      {
        string accountProviderId = Settings.OneDriveUserAccountProviderId;
        string accountId = Settings.OneDriveUserAccountId;
        if (string.IsNullOrEmpty(accountProviderId) || string.IsNullOrEmpty(accountId))
          throw new ServiceException(new Error()
          {
            Code = "authenticationNeverOccured",
            Message = string.Format("Cannot authenticate user without saved account ID")
          });
        WAWebAccountProvider webAccountProvider = accountProvider;
        accountProvider = await this.authManager.FindAccountProviderAsync(accountProviderId);
        if (accountProvider == null)
          throw new ServiceException(new Error()
          {
            Code = "authenticationNeverOccured",
            Message = string.Format("Could not find account provider for authentication")
          });
        WAWebAccount waWebAccount = account;
        account = await this.authManager.FindAccountAsync(accountProvider, accountId);
        if (account == null)
          throw new ServiceException(new Error()
          {
            Code = "authenticationNeverOccured",
            Message = string.Format("Could not find account for authentication")
          });
        accountId = (string) null;
      }
      WAWebTokenRequestResult tokenRequestResult;
      if (promptType != 2)
      {
        TaskCompletionSource<WAWebTokenRequestResult> uiThread = new TaskCompletionSource<WAWebTokenRequestResult>();
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (async () =>
        {
          WAWebTokenRequestResult result = (WAWebTokenRequestResult) null;
          try
          {
            Log.d("webauth", "authenticating user on UI thread: {0}", (object) promptType.ToString());
            if (account != null)
              result = await this.authManager.RequestTokenWithWebAccountAsync(accountProvider, account, authScope);
            else
              result = await this.authManager.RequestTokenAsync(accountProvider, authScope);
          }
          catch (Exception ex)
          {
            uiThread.SetException(ex);
            return;
          }
          uiThread.SetResult(result);
        }));
        tokenRequestResult = await uiThread.Task;
      }
      else
      {
        Log.d("webauth", "authenticating user silently: {0}", (object) promptType.ToString());
        if (account != null)
          tokenRequestResult = await this.authManager.GetTokenSilentlyWithWebAccountAsync(accountProvider, account, authScope);
        else
          tokenRequestResult = await this.authManager.GetTokenSilentlyAsync(accountProvider, authScope);
      }
      if (tokenRequestResult.ResponseStatus != WAWebTokenRequestStatus.Success)
      {
        bool flag = tokenRequestResult.ResponseStatus == WAWebTokenRequestStatus.UserCancel;
        throw new ServiceException(new Error()
        {
          Code = flag ? "authenticationCancelled" : "authenticationFailure",
          Message = string.Format("Failed to request authentication for user: {0}", (object) tokenRequestResult.ResponseStatus.ToString())
        });
      }
      string token = tokenRequestResult?.ResponseData?.Token;
      WAWebAccount webAccount = tokenRequestResult?.ResponseData?.WebAccount;
      WAWebAccountProvider webAccountProvider1 = webAccount?.WebAccountProvider;
      if (string.IsNullOrEmpty(token))
        throw new ServiceException(new Error()
        {
          Code = "authenticationFailure",
          Message = string.Format("Failed to retrieve a valid authentication token for user")
        });
      Log.l("webauth", "Retrieved authentication token for user");
      AccountSession accountSessionAsync = new AccountSession()
      {
        AccessToken = token,
        AccessTokenType = "Bearer",
        ClientId = Utils.ToPpsz((IEnumerable<string>) new string[2]
        {
          webAccountProvider1.Id,
          webAccountProvider1.Authority
        }),
        UserId = webAccount.Id,
        ExpiresOnUtc = DateTimeOffset.UtcNow.AddMinutes((double) this.ticketExpirationTimeInMinutes),
        Scopes = this.scopes
      };
      Settings.OneDriveUserAccountProviderId = webAccountProvider1.Id;
      Settings.OneDriveUserAccountProviderAuthority = webAccountProvider1.Authority;
      Settings.OneDriveUserAccountId = webAccount.Id;
      return accountSessionAsync;
    }
  }
}
