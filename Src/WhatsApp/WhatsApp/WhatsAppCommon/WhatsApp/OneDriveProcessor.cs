// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveProcessor
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using WhatsAppNative;
using Windows.Security.Authentication.OnlineId;


namespace WhatsApp
{
  public abstract class OneDriveProcessor
  {
    private static string[] scopes = new string[4]
    {
      "wl.signin",
      "wl.emails",
      "wl.offline_access",
      "onedrive.appfolder"
    };
    private static string OneDriveBaseUri = "https://api.onedrive.com/v1.0";
    private static string LiveIdUriPrefix = "https://apis.live.net/v5.0/me?access_token=";
    private object initLock = new object();
    private object statsLock = new object();
    protected IWaAuthenticationProvider authProvider;
    protected IOneDriveClient oneDriveClient;
    protected string chatId;
    protected const long FragmentByteAlignmentBytes = 327680;
    protected const long MaxSimpleUploadSize = 1048576;
    protected long UploadFragmentSizeBytes;
    private const int OneDriveMaxRetry = 5;
    private const int OneDriveRetryDelayMs = 2000;
    private int requestCount;

    public OneDriveProcessor()
    {
      this.chatId = Settings.ChatID;
      this.UploadFragmentSizeBytes = !AppState.IsBackgroundAgent ? 3932160L : 327680L;
      this.CredentialPrompt = (CredentialPromptType) 2;
      this.InitializeOneDriveClient(false);
    }

    public bool IsAuthenticated => this.IsClientAuthenticated();

    public string ClientId { get; private set; }

    public string UserId { get; private set; }

    public CredentialPromptType CredentialPrompt { get; set; }

    public int RequestCount
    {
      get
      {
        lock (this.statsLock)
          return this.requestCount;
      }
    }

    private void InitializeOneDriveClient(bool forceWebAuth)
    {
      lock (this.initLock)
      {
        bool flag = false;
        string driveUserAccountId = Settings.OneDriveUserAccountId;
        if (OneDriveUtils.UseWP10AccountInterface() && !string.IsNullOrEmpty(driveUserAccountId) | forceWebAuth)
        {
          if (!(this.authProvider is WaWebAuthenticationProvider))
          {
            Log.l("onedrive", "Using WebAuth authentication provider");
            this.authProvider = (IWaAuthenticationProvider) new WaWebAuthenticationProvider(OneDriveProcessor.scopes);
            flag = true;
          }
        }
        else if (!(this.authProvider is WaOnlineIdAuthenticationProvider))
        {
          Log.l("onedrive", "Using OnlineId authentication provider");
          this.authProvider = (IWaAuthenticationProvider) new WaOnlineIdAuthenticationProvider(OneDriveProcessor.scopes);
          flag = true;
        }
        if (!(this.oneDriveClient == null | flag))
          return;
        Log.l("onedrive", "Instantiating OneDrive client");
        this.oneDriveClient = (IOneDriveClient) new OneDriveClient(OneDriveProcessor.OneDriveBaseUri, (IAuthenticationProvider) this.authProvider, (IHttpProvider) new WaHttpProvider());
      }
    }

    public async Task<bool> Authenticate(
      CredentialPromptType? promptType = null,
      WAWebAccountProvider selectedProvider = null)
    {
      if (selectedProvider != null)
      {
        this.InitializeOneDriveClient(true);
        await this.authProvider.SignOutAsync();
      }
      bool flag;
      if (!this.IsClientAuthenticated())
      {
        try
        {
          if (this.authProvider is WaWebAuthenticationProvider)
          {
            await ((WaWebAuthenticationProvider) this.authProvider).AuthenticateUserAsync(promptType.HasValue ? promptType.Value : this.CredentialPrompt, selectedProvider);
            this.ClientId = this.authProvider.CurrentAccountSession?.ClientId;
            this.UserId = this.authProvider.CurrentAccountSession?.UserId;
          }
          else
          {
            await ((WaOnlineIdAuthenticationProvider) this.authProvider).AuthenticateUserAsync(promptType.HasValue ? promptType.Value : this.CredentialPrompt);
            this.ClientId = this.authProvider.CurrentAccountSession?.ClientId;
            this.UserId = this.authProvider.CurrentAccountSession?.UserId;
            Settings.OneDriveUserAccountId = this.UserId;
          }
          flag = true;
        }
        catch (ServiceException ex)
        {
          Log.l("onedrive", "authentication error {0}", (object) ex.Error.ToString());
          Log.LogException((Exception) ex, "onedrive");
          flag = false;
        }
      }
      else
        flag = true;
      return flag;
    }

    protected bool IsClientAuthenticated()
    {
      return this.oneDriveClient != null && this.authProvider.IsAuthenticated;
    }

    public async Task Reset() => await this.authProvider.SignOutAsync();

    public void ResetCounters()
    {
      lock (this.statsLock)
        this.requestCount = 0;
    }

    public async Task<bool> QueryUserMetadata(CancellationToken cancellationToken)
    {
      string token = (string) null;
      byte[] buffer = await this.OneDriveRun<byte[]>((Func<Task<byte[]>>) (async () =>
      {
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), "");
        if (string.IsNullOrEmpty(token))
        {
          await this.oneDriveClient.AuthenticationProvider.AuthenticateRequestAsync(request);
          token = request.Headers.Authorization.Parameter;
          request.Headers.Authorization = (AuthenticationHeaderValue) null;
        }
        request.RequestUri = new Uri(OneDriveProcessor.LiveIdUriPrefix + token);
        return await (await this.oneDriveClient.HttpProvider.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)).Content.ReadAsByteArrayAsync();
      }), cancellationToken);
      LiveIdUserInfo liveIdUserInfo = (LiveIdUserInfo) null;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          liveIdUserInfo = new DataContractJsonSerializer(typeof (LiveIdUserInfo)).ReadObject((Stream) memoryStream) as LiveIdUserInfo;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "onedrive");
      }
      if (liveIdUserInfo == null)
      {
        Log.l("onedrive", "Unable to deserialize user info");
        return false;
      }
      string id = liveIdUserInfo.Id;
      string name = liveIdUserInfo.Name;
      string str1 = !string.IsNullOrEmpty(liveIdUserInfo.Emails?.Account) ? liveIdUserInfo.Emails?.Account : liveIdUserInfo.Emails?.Preferred;
      string str2 = string.IsNullOrEmpty(id) ? string.Empty : id.ToLowerInvariant();
      string str3 = string.IsNullOrEmpty(name) ? string.Empty : name;
      string str4 = string.IsNullOrEmpty(str1) ? string.Empty : str1;
      if (str2 != string.Empty)
      {
        Log.l("onedrive", "LiveId: UserId={0}, DisplayName=\"{1}\", Email=\"{2}\"", (object) str2, (object) str3, (object) str4);
        Settings.OneDriveUserId = str2;
        Settings.OneDriveUserDisplayName = str3;
        Settings.OneDriveUserAccountEmail = str4;
        return true;
      }
      Log.l("onedrive", "Unable to retrieve user data");
      Settings.OneDriveUserId = string.Empty;
      Settings.OneDriveUserDisplayName = string.Empty;
      Settings.OneDriveUserAccountEmail = string.Empty;
      return false;
    }

    public async Task QueryDriveMetadata(CancellationToken cancellationToken)
    {
      Microsoft.OneDrive.Sdk.Drive drive = await this.OneDriveRun<Microsoft.OneDrive.Sdk.Drive>((Func<Task<Microsoft.OneDrive.Sdk.Drive>>) (async () => await this.oneDriveClient.Drive.Request().GetAsync(cancellationToken)), cancellationToken);
      string id = drive?.Owner?.User?.Id;
      string displayName = drive?.Owner?.User?.DisplayName;
      string str1 = string.IsNullOrEmpty(id) ? string.Empty : id.ToLowerInvariant();
      string str2 = string.IsNullOrEmpty(displayName) ? string.Empty : displayName;
      if (!(str1 != string.Empty))
        return;
      Log.l("onedrive", "OneDrive: UserId={0}, DisplayName=\"{1}\"", (object) str1, (object) str2);
      string oneDriveUserId = Settings.OneDriveUserId;
      if (string.IsNullOrEmpty(oneDriveUserId) || str1.Equals(oneDriveUserId, StringComparison.InvariantCultureIgnoreCase))
        return;
      Log.l("onedrive", "Warning! Saved account user ID does not match OneDrive user ID.");
    }

    public async Task<bool> SynchronizeManifest(
      OneDriveManifest manifest,
      CancellationToken cancellationToken)
    {
      int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
      Log.l("onedrive", "synchronizing backup manifest");
      string deltaToken = manifest.GetMediaDeltaToken();
      Log.l("onedrive", "using delta token: {0}", (object) deltaToken);
      bool resyncRequired = false;
      IItemDeltaCollectionPage deltaResult = (IItemDeltaCollectionPage) null;
      try
      {
        IItemDeltaCollectionPage deltaCollectionPage = deltaResult;
        deltaResult = await this.OneDriveRun<IItemDeltaCollectionPage>((Func<Task<IItemDeltaCollectionPage>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(string.Format("{0}/media", (object) this.chatId)).Delta(deltaToken).Request().GetAsync(cancellationToken)), cancellationToken);
      }
      catch (ServiceException ex)
      {
        if (ex.IsMatch("resyncRequired") || ex.IsMatch("resyncApplyDifferences") || ex.IsMatch("resyncUploadDifferences"))
        {
          Log.l("onedrive", "delta error={0} ({1})", (object) ex.Error.Code, (object) ex.Error.InnerError.Code);
          resyncRequired = true;
        }
        else
          throw;
      }
      if (resyncRequired)
      {
        Log.l("onedrive", "need full resync");
        IItemDeltaCollectionPage deltaCollectionPage = deltaResult;
        deltaResult = await this.OneDriveRun<IItemDeltaCollectionPage>((Func<Task<IItemDeltaCollectionPage>>) (async () => await this.oneDriveClient.Drive.Special.AppRoot.ItemWithPath(string.Format("{0}/media", (object) this.chatId)).Delta().Request().GetAsync(cancellationToken)), cancellationToken);
      }
      deltaToken = deltaResult.Token;
      List<Item> itemList = deltaResult.ToList<Item>();
      while (deltaResult.NextPageRequest != null)
      {
        IItemDeltaCollectionPage deltaCollectionPage = deltaResult;
        deltaResult = await this.OneDriveRun<IItemDeltaCollectionPage>((Func<Task<IItemDeltaCollectionPage>>) (async () => await deltaResult.NextPageRequest.GetAsync(cancellationToken)), cancellationToken);
        if (!string.IsNullOrEmpty(deltaResult.Token))
          deltaToken = deltaResult.Token;
        itemList.AddRange((IEnumerable<Item>) deltaResult);
      }
      Log.l("onedrive", "retrieved {0} changes", (object) itemList.Count);
      return manifest.UpdateRemoteMediaFiles(deltaToken, resyncRequired, itemList.Where<Item>((Func<Item, bool>) (item => item.File != null && item.File.Hashes != null)));
    }

    protected static bool TryGetLength(Stream source, out long length)
    {
      if (source == null)
      {
        length = -1L;
        return false;
      }
      try
      {
        length = source.Length;
        return true;
      }
      catch
      {
        length = -1L;
        return false;
      }
    }

    protected async Task<TResult> OneDriveRun<TResult>(
      Func<Task<TResult>> function,
      CancellationToken cancellationToken)
    {
      TResult result = default (TResult);
      int retryCount = 0;
      bool retryDelay = false;
      do
      {
        bool resetAuth = false;
        try
        {
          cancellationToken.ThrowIfCancellationRequested();
          if (retryCount > 0)
            Log.l("onedrive", "retry attempt {0}", (object) retryCount);
          if (retryDelay)
          {
            retryDelay = false;
            Log.l("onedrive", "waiting {0}ms before retry", (object) 2000);
            await Task.Delay(2000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
          }
          lock (this.statsLock)
            ++this.requestCount;
          result = await Task.Run<TResult>(function, cancellationToken);
          break;
        }
        catch (ServiceException ex)
        {
          if (ex.InnerException is TaskCanceledException)
            Log.l("onedrive", "request failed on task canceled");
          else if (string.IsNullOrEmpty(ex.Error.Code))
            Log.l("onedrive", "request failed on empty server exception");
          else if (ex.IsMatch("Unauthorized") || ex.IsMatch("unauthenticated") || ex.IsMatch("request_token_expired"))
          {
            Log.l("onedrive", "request failed with auth error {0}", (object) ex.Error.Code);
            resetAuth = true;
          }
          else
          {
            if (!ex.IsMatch("timeout"))
            {
              Exception innerException = ex.InnerException;
              if ((innerException != null ? (innerException.GetHResult() == 2147954431U ? 1 : 0) : 0) == 0)
              {
                if (ex.IsMatch("server_internal_error") || ex.IsMatch("serviceNotAvailable"))
                {
                  Log.l("onedrive", "request failed with error {0}", (object) ex.Error.Code);
                  retryDelay = true;
                  goto label_27;
                }
                else if (ex.IsMatch("generalException"))
                {
                  Log.l("onedrive", "request failed with unspecified error");
                  retryDelay = true;
                  goto label_27;
                }
                else
                {
                  Log.l("onedrive", "request failed with error {0}", (object) ex.Error.Code);
                  throw;
                }
              }
            }
            Log.l("onedrive", "request failed with timeout or connection error");
          }
label_27:
          Log.LogException((Exception) ex, "onedrive");
          ++retryCount;
        }
        if (resetAuth)
        {
          await this.Reset();
          int num = await this.Authenticate(new CredentialPromptType?(this.CredentialPrompt)) ? 1 : 0;
        }
      }
      while (retryCount < 5);
      if (retryCount >= 5)
      {
        Log.l("onedrive", "request failed due to max retry count");
        throw new Exception("maximum number of OneDrive retries");
      }
      return result;
    }
  }
}
