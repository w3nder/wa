// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaOnlineIdAuthenticationProvider
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace WhatsApp
{
  public class WaOnlineIdAuthenticationProvider : IWaAuthenticationProvider, IAuthenticationProvider
  {
    private readonly int ticketExpirationTimeInMinutes = 59;
    private readonly string[] scopes;
    private OnlineIdAuthenticator authenticator;

    public WaOnlineIdAuthenticationProvider(string[] scopes)
      : this(scopes, (CredentialCache) null)
    {
    }

    public WaOnlineIdAuthenticationProvider(string[] scopes, CredentialCache credentialCache)
    {
      this.scopes = scopes;
      this.CredentialCache = credentialCache ?? new CredentialCache();
      this.authenticator = new OnlineIdAuthenticator();
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
      Log.l("onlineid", "attempting to sign out");
      if (!this.IsAuthenticated)
        return;
      if (this.authenticator.CanSignOut)
      {
        TaskCompletionSource<int> uiThread = new TaskCompletionSource<int>();
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (async () =>
        {
          try
          {
            Log.l("onlineid", "signing out user on UI thread");
            await (IAsyncAction) this.authenticator.SignOutUserAsync();
          }
          catch (Exception ex)
          {
            uiThread.SetException(ex);
            return;
          }
          uiThread.SetResult(1);
        }));
        int task = await uiThread.Task;
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

    public async Task AuthenticateUserAsync(CredentialPromptType promptType)
    {
      AccountSession accountSession = await this.GetAuthenticationResultFromCacheAsync().ConfigureAwait(false);
      if (accountSession == null)
        accountSession = await this.GetAccountSessionAsync(promptType);
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

    private async Task<AccountSession> GetAccountSessionAsync(CredentialPromptType promptType)
    {
      OnlineIdServiceTicketRequest serviceTicketRequest = new OnlineIdServiceTicketRequest(string.Join(" ", this.scopes), "DELEGATION");
      List<OnlineIdServiceTicketRequest> ticketRequests = new List<OnlineIdServiceTicketRequest>();
      ticketRequests.Add(serviceTicketRequest);
      TaskCompletionSource<UserIdentity> uiThread = new TaskCompletionSource<UserIdentity>();
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (async () =>
      {
        UserIdentity userIdentity = (UserIdentity) null;
        try
        {
          Log.l("onlineid", "authenticating user on UI thread: {0}", (object) promptType.ToString());
          userIdentity = await (IAsyncOperation<UserIdentity>) this.authenticator.AuthenticateUserAsync((IEnumerable<OnlineIdServiceTicketRequest>) ticketRequests, promptType);
        }
        catch (Exception ex)
        {
          if (ex is TaskCanceledException)
          {
            uiThread.SetException((Exception) new ServiceException(new Error()
            {
              Code = "authenticationCancelled",
              Message = "Failed to authenticate due to task cancellation"
            }, ex));
            return;
          }
          if (ex.GetHResult() == 2156265484U)
          {
            uiThread.SetException((Exception) new ServiceException(new Error()
            {
              Code = "authenticationFailure",
              Message = "User interaction is required for authentication."
            }, ex));
            return;
          }
          uiThread.SetException(ex);
          return;
        }
        uiThread.SetResult(userIdentity);
      }));
      UserIdentity task = await uiThread.Task;
      OnlineIdServiceTicket onlineIdServiceTicket = task.Tickets.FirstOrDefault<OnlineIdServiceTicket>();
      if (onlineIdServiceTicket == null || string.IsNullOrEmpty(onlineIdServiceTicket.Value))
        throw new ServiceException(new Error()
        {
          Code = "authenticationFailure",
          Message = "Failed to retrieve a valid authentication token from OnlineIdAuthenticator for user"
        });
      return new AccountSession()
      {
        AccessToken = onlineIdServiceTicket == null ? (string) null : onlineIdServiceTicket.Value,
        AccessTokenType = "Bearer",
        ClientId = this.authenticator.ApplicationId.ToString(),
        UserId = task.SafeCustomerId,
        ExpiresOnUtc = DateTimeOffset.UtcNow.AddMinutes((double) this.ticketExpirationTimeInMinutes),
        Scopes = this.scopes
      };
    }
  }
}
