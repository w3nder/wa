// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WhatsApp.ProtoBuf;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class QrSession
  {
    private const string LogHeader = "web";
    public const string Version = "0.17.10";
    private const int DropSessionTimeout = 10;
    private object @lock = new object();
    private byte[] cachedBlob;
    private QrSession.SessionState state;
    private bool _available;
    private ObservableCollection<QrSessionInfo> sessionsInfo;
    private QrSession.ActionsCache pendingActions;

    public static ClientPayload.WebInfo.WebdPayload WebdPayload
    {
      get
      {
        return new ClientPayload.WebInfo.WebdPayload()
        {
          UsesParticipantInKey = new bool?(false),
          SupportsStarredMessages = new bool?(true),
          SupportsUrlMessages = new bool?(true),
          SupportsMediaRetry = new bool?(true),
          Features = QrSession.getSerializedWebFeatures()
        };
      }
    }

    public static byte[] getSerializedWebFeatures()
    {
      return WebFeatures.SerializeToBytes(new WebFeatures()
      {
        GroupsV3 = new WebFeatures.Flag?(WebFeatures.Flag.OPTIONAL),
        QueryStatusV3Thumbnail = new WebFeatures.Flag?(WebFeatures.Flag.OPTIONAL),
        ChangeNumberV2 = new WebFeatures.Flag?(WebFeatures.Flag.OPTIONAL),
        LiveLocations = new WebFeatures.Flag?(WebFeatures.Flag.IMPLEMENTED),
        VnameV2 = new WebFeatures.Flag?(WebFeatures.Flag.IMPLEMENTED)
      });
    }

    public event EventHandler Disconnected;

    private QrSession.SessionState State
    {
      get
      {
        byte[] qrBlob = Settings.QrBlob;
        if (qrBlob != null && !qrBlob.IsEqualBytes(this.cachedBlob))
        {
          this.state = this.LoadState(qrBlob);
          this.cachedBlob = qrBlob;
        }
        return this.state;
      }
    }

    public bool Available
    {
      get => this._available;
      set
      {
        if (value == this._available)
          return;
        this._available = value;
        this.UpdateSessionsInfo();
      }
    }

    public bool Active => this.Id != null;

    public bool HasConnections
    {
      get
      {
        this.PurgeExpiredBrowsers();
        QrSession.SessionState state = this.State;
        return state != null && state.Browsers != null && state.Browsers.Length != 0;
      }
    }

    public byte[] Id
    {
      get
      {
        QrSession.SessionState state = this.State;
        byte[] id = (byte[]) null;
        if (state != null)
          id = state.SessionData;
        return id;
      }
    }

    public QrCrypto Crypto
    {
      get
      {
        QrCrypto crypto = (QrCrypto) null;
        QrSession.SessionState state = this.State;
        if (state != null)
        {
          QrSession.PerBrowserData perBrowserData = (QrSession.PerBrowserData) null;
          lock (this.@lock)
            perBrowserData = state.GetActiveBrowser();
          if (perBrowserData != null)
            crypto = perBrowserData.Crypto;
        }
        return crypto;
      }
    }

    public bool QrDemoShown
    {
      get
      {
        QrSession.SessionState state = this.State;
        return state != null && state.QrDemoShown;
      }
      set
      {
        lock (this.@lock)
        {
          QrSession.SessionState state = this.State ?? new QrSession.SessionState();
          state.QrDemoShown = value;
          this.UpdateState(state);
        }
      }
    }

    public static bool IsKnownCode(int code)
    {
      switch (code)
      {
        case 404:
        case 405:
        case 408:
        case 500:
          return true;
        default:
          return false;
      }
    }

    public byte[] ActiveBrowserId
    {
      get
      {
        QrSession.SessionState state = this.State;
        if (state != null && state.ActiveBrowser.HasValue)
        {
          lock (this.@lock)
          {
            QrSession.PerBrowserData activeBrowser = state.GetActiveBrowser();
            if (activeBrowser != null)
              return activeBrowser.BrowserId;
          }
        }
        return (byte[]) null;
      }
    }

    public void Synchronize(Action a)
    {
      lock (this.@lock)
      {
        if (!this.Active)
          return;
        a();
      }
    }

    public void SynchronizeWithMessageContext(MessagesContext.MessagesCallback a)
    {
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        lock (this.@lock)
        {
          if (!this.Active)
            return;
          a(db);
        }
      }));
    }

    public bool OnQrCodeScanned(string code, Action success, Action failure)
    {
      string[] strArray = code.Split(',');
      if (strArray.Length < 3)
        return false;
      byte[] sessionData = Encoding.UTF8.GetBytes(strArray[0]);
      byte[] pubKey = Convert.FromBase64String(strArray[1]);
      byte[] browserId = Convert.FromBase64String(strArray[2]);
      byte[] cryptKey = new byte[32];
      byte[] macKey = new byte[32];
      RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
      cryptoServiceProvider.GetBytes(cryptKey);
      cryptoServiceProvider.GetBytes(macKey);
      QrCrypto crypto = new QrCrypto(cryptKey, macKey);
      byte[] password = crypto.GenPassword(pubKey);
      byte[] clientToken = QrCrypto.GenerateToken();
      if (this.Active)
      {
        if (this.Available)
          this.UpdateLastConnected((byte[]) null, DateTime.Now);
        this.Disconnect();
        AppState.GetConnection().SendQrUnsync(false);
      }
      Log.l("web", nameof (OnQrCodeScanned));
      AppState.GetConnection().SendQrSync(sessionData, clientToken, password: password, onComplete: (Action<string, string, bool>) ((os, browser, timeout) =>
      {
        Log.l("web", "SendQrSync - Success - BrowserId: {0}", browserId != null ? (object) Convert.ToBase64String(browserId) : (object) "");
        this.SetSession(sessionData, cryptKey, macKey, browserId, clientToken, os, browser, crypto);
        AppState.GetConnection().EventHandler.Qr.OnQrOnline(new bool?(timeout));
        AppState.QrPersistentAction.NotifyPreemptiveChats();
        AppState.QrPersistentAction.NotifyPreemptiveContacts();
        AppState.GetConnection().EventHandler.Qr.Session.QrDemoShown = true;
        if (success == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded(success);
      }), onError: (Action<int>) (err =>
      {
        Log.l("web", "SendQrSync - ERROR - Error: {0}", (object) err);
        if (failure == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded(failure);
      }));
      return true;
    }

    private void SetSession(
      byte[] sessionData,
      byte[] cryptKey,
      byte[] macKey,
      byte[] browserId,
      byte[] clientToken,
      string os,
      string browser,
      QrCrypto crypto = null)
    {
      lock (this.@lock)
      {
        QrSession.SessionState state1 = this.State ?? new QrSession.SessionState();
        state1.SessionData = sessionData;
        QrSession.PerBrowserData perBrowserData = new QrSession.PerBrowserData()
        {
          CryptKey = cryptKey,
          MacKey = macKey,
          BrowserId = browserId,
          ClientToken = clientToken,
          Crypto = crypto,
          OperatingSystem = os,
          BrowserName = browser,
          FirstConnected = DateTime.Now,
          LastConnected = DateTime.Now
        };
        state1.Browsers = ((IEnumerable<QrSession.PerBrowserData>) new QrSession.PerBrowserData[1]
        {
          perBrowserData
        }).Concat<QrSession.PerBrowserData>(((IEnumerable<QrSession.PerBrowserData>) (state1.Browsers ?? new QrSession.PerBrowserData[0])).Where<QrSession.PerBrowserData>((Func<QrSession.PerBrowserData, bool>) (pb => !pb.BrowserId.IsEqualBytes(browserId)))).ToArray<QrSession.PerBrowserData>();
        state1.ActiveBrowser = new int?(0);
        this.UpdateState(state1);
        WAThreadPool.QueueUserWorkItem((Action) (async () =>
        {
          GeoCoordinate coords = (GeoCoordinate) null;
          try
          {
            GeoCoordinate geoCoordinate = coords;
            coords = await LocationHelper.GetInaccurateGeoCoordinate();
          }
          catch (Exception ex)
          {
            string context = string.Format("{0} > GetInaccurateGeoCoordinate - FAIL", (object) "web");
            Log.LogException(ex, context);
          }
          if (!(coords != (GeoCoordinate) null))
            return;
          WebServices.Instance.ReverseGeocode(coords.Latitude, coords.Longitude).Take<PlaceSearchResult>(1).Subscribe<PlaceSearchResult>((Action<PlaceSearchResult>) (place =>
          {
            lock (this.@lock)
            {
              QrSession.SessionState state3 = this.State;
              if (state3.Browsers == null || state3.Browsers.Length == 0)
                return;
              for (int index = 0; index < state3.Browsers.Length; ++index)
              {
                if (state3.Browsers[index].BrowserId == browserId)
                  state3.Browsers[index].Location = place.Locality + ", " + place.AdminDistrict;
              }
              this.UpdateState(state3);
            }
          }), (Action<Exception>) (ex => Log.l("web", "ReverseGeocode - FAIL - Coords: {0}", (object) coords)));
        }));
      }
    }

    public void UpdateLastConnected(byte[] browserid, DateTime timestamp)
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state == null || !this.Active && browserid == null)
          return;
        int? nullable = new int?();
        if (browserid == null)
        {
          nullable = state.ActiveBrowser;
        }
        else
        {
          if (state.Browsers == null)
            return;
          for (int index = 0; index < state.Browsers.Length; ++index)
          {
            if (state.Browsers[index].BrowserId.IsEqualBytes(browserid))
            {
              nullable = new int?(index);
              break;
            }
          }
        }
        if (!nullable.HasValue)
          return;
        if (state.Browsers == null)
          return;
        try
        {
          state.Browsers[nullable.Value].LastConnected = timestamp;
          this.UpdateState(state);
        }
        catch
        {
        }
      }
    }

    public ObservableCollection<QrSessionInfo> SessionsInfo
    {
      get
      {
        if (this.sessionsInfo == null)
        {
          this.sessionsInfo = new ObservableCollection<QrSessionInfo>();
          this.UpdateSessionsInfo();
        }
        return this.sessionsInfo;
      }
    }

    private void UpdateSessionsInfo()
    {
      if (this.sessionsInfo == null)
        return;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        QrSession.SessionState state = this.State;
        if (state.Browsers == null)
        {
          this.sessionsInfo.Clear();
        }
        else
        {
          Dictionary<string, QrSessionInfo> dictionary1 = ((IEnumerable<QrSession.PerBrowserData>) state.Browsers).Select<QrSession.PerBrowserData, QrSessionInfo>((Func<QrSession.PerBrowserData, QrSessionInfo>) (b => new QrSessionInfo()
          {
            BrowserId = b.BrowserIdString,
            OperatingSystem = b.OperatingSystem,
            Browser = b.BrowserName,
            LastConnected = new DateTime?(b.LastConnected),
            FirstConnected = b.FirstConnected,
            Location = b.Location
          })).ToDictionary<QrSessionInfo, string, QrSessionInfo>((Func<QrSessionInfo, string>) (i => i.BrowserId), (Func<QrSessionInfo, QrSessionInfo>) (i => i));
          Dictionary<string, QrSessionInfo> dictionary2 = new Dictionary<string, QrSessionInfo>();
          List<QrSessionInfo> qrSessionInfoList = new List<QrSessionInfo>();
          QrSession.PerBrowserData activeBrowser = state.GetActiveBrowser();
          if (activeBrowser != null && this.Available)
            dictionary1[activeBrowser.BrowserIdString].LastConnected = new DateTime?();
          foreach (QrSessionInfo qrSessionInfo1 in (Collection<QrSessionInfo>) this.sessionsInfo)
          {
            if (dictionary1.ContainsKey(qrSessionInfo1.BrowserId))
            {
              QrSessionInfo qrSessionInfo2 = dictionary1[qrSessionInfo1.BrowserId];
              qrSessionInfo1.Browser = qrSessionInfo2.Browser;
              qrSessionInfo1.OperatingSystem = qrSessionInfo2.OperatingSystem;
              qrSessionInfo1.LastConnected = qrSessionInfo2.LastConnected;
              qrSessionInfo1.FirstConnected = qrSessionInfo2.FirstConnected;
              qrSessionInfo1.Location = qrSessionInfo2.Location;
              dictionary2[qrSessionInfo1.BrowserId] = qrSessionInfo1;
            }
            else
              qrSessionInfoList.Add(qrSessionInfo1);
          }
          foreach (QrSessionInfo qrSessionInfo in qrSessionInfoList)
            this.sessionsInfo.Remove(qrSessionInfo);
          foreach (QrSessionInfo newItem in dictionary1.Values)
          {
            if (!dictionary2.ContainsKey(newItem.BrowserId))
              this.sessionsInfo.InsertInOrder<QrSessionInfo>(newItem, (Func<QrSessionInfo, QrSessionInfo, bool>) ((s1, s2) => s1.FirstConnected < s2.FirstConnected));
          }
        }
      }));
    }

    public FunXMPP.VerifySessionResponse VerifyCurrent(
      ref FunXMPP.VerifySessionType type,
      byte[] sessionData,
      byte[] browserId,
      byte[] clientToken,
      string os = null,
      string browser = null)
    {
      FunXMPP.VerifySessionResponse verifySessionResponse = new FunXMPP.VerifySessionResponse();
      verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Failure;
      if (sessionData == null || browserId == null || clientToken == null)
        return verifySessionResponse;
      this.PurgeExpiredBrowsers();
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state == null || state.Browsers == null)
          return verifySessionResponse;
        int? nullable = new int?();
        int index = 0;
        for (int length = state.Browsers.Length; index < length; ++index)
        {
          if (state.Browsers[index].BrowserId.IsEqualBytes(browserId))
          {
            nullable = new int?(index);
            break;
          }
        }
        if (!nullable.HasValue)
        {
          verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Failure;
          return verifySessionResponse;
        }
        QrSession.PerBrowserData browser1 = state.Browsers[nullable.Value];
        if (type == FunXMPP.VerifySessionType.Challenge)
        {
          byte[] a = (byte[]) null;
          using (HMACSHA256 hmacshA256 = new HMACSHA256(browser1.MacKey))
            a = hmacshA256.ComputeHash(browser1.Challenge);
          if (!a.IsEqualBytes(clientToken))
          {
            verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Failure;
            return verifySessionResponse;
          }
          type = state.ChallengeOriginatingType;
        }
        else if (!browser1.ClientToken.IsEqualBytes(clientToken))
        {
          byte[] numArray;
          if (browser1.Challenge != null && type == state.ChallengeOriginatingType)
          {
            numArray = browser1.Challenge;
          }
          else
          {
            numArray = QrCrypto.GenerateToken();
            browser1.Challenge = numArray;
          }
          verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Challenge;
          verifySessionResponse.ChallengeData = numArray;
          state.ChallengeOriginatingType = type;
          this.UpdateState(state);
          return verifySessionResponse;
        }
        if (state.ActiveBrowser.HasValue && type != FunXMPP.VerifySessionType.Takeover && type != FunXMPP.VerifySessionType.ForcedResume && (type != FunXMPP.VerifySessionType.Normal && type != FunXMPP.VerifySessionType.Resume || state.ActiveBrowser.Value != nullable.Value))
        {
          verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Conflict;
          return verifySessionResponse;
        }
        verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Accept;
        byte[] token = QrCrypto.GenerateToken();
        verifySessionResponse.ClientToken = token;
        browser1.ClientToken = token;
        browser1.OperatingSystem = os ?? browser1.OperatingSystem;
        browser1.BrowserName = browser ?? browser1.BrowserName;
        state.ActiveBrowser = nullable;
        state.SessionData = sessionData;
        this.UpdateState(state);
        return verifySessionResponse;
      }
    }

    public FunXMPP.VerifySessionResponse VerifyQuerySync(
      FunXMPP.VerifySessionType type,
      byte[] sessionData)
    {
      FunXMPP.VerifySessionResponse verifySessionResponse = new FunXMPP.VerifySessionResponse();
      verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Failure;
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state != null)
        {
          if (type == FunXMPP.VerifySessionType.Query)
          {
            if (state.SessionData != null)
            {
              if (state.SessionData.IsEqualBytes(sessionData))
                verifySessionResponse.Response = FunXMPP.VerifySessionResponse.ResponseType.Accept;
            }
          }
        }
      }
      return verifySessionResponse;
    }

    public void DiscardChallenge()
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state == null)
          return;
        QrSession.PerBrowserData activeBrowser = state.GetActiveBrowser();
        if (activeBrowser == null || activeBrowser.Challenge == null)
          return;
        activeBrowser.Challenge = (byte[]) null;
        this.UpdateState(state);
      }
    }

    public void LogoutAll()
    {
      this.DeleteBrowser((byte[]) null, (byte[]) null);
      Action onComplete = (Action) (() => Log.l("web", nameof (LogoutAll)));
      AppState.GetConnection().SendQrUnsync(true, onComplete);
      PresenceState.Instance.DisposeTimer(PresenceState.ChatStateSource.Qr);
    }

    public void Disconnect()
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        bool flag = false;
        byte[] inArray = (byte[]) null;
        if (state != null)
        {
          if (state.SessionData != null)
          {
            state.SessionData = (byte[]) null;
            flag = true;
          }
          QrSession.PerBrowserData activeBrowser;
          if ((activeBrowser = state.GetActiveBrowser()) != null)
          {
            if (activeBrowser.ForgetMe)
              activeBrowser.Expiration = new DateTime?(DateTime.Now + TimeSpan.FromMinutes(10.0));
            state.ActiveBrowser = new int?();
            inArray = activeBrowser.BrowserId;
            flag = true;
          }
        }
        this.Available = false;
        if (!flag)
          return;
        Log.l("web", "Session Disconnect - BrowserId: {0}", inArray != null ? (object) Convert.ToBase64String(inArray) : (object) "");
        this.UpdateState(state);
        if (this.Disconnected == null)
          return;
        this.Disconnected((object) null, (EventArgs) null);
      }
    }

    public void DeleteBrowser(byte[] browserid, byte[] code)
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        bool flag = false;
        if (state != null)
        {
          if (browserid == null)
          {
            this.Disconnect();
            if (state.Browsers != null)
            {
              state.Browsers = (QrSession.PerBrowserData[]) null;
              flag = true;
            }
          }
          else if (state.Browsers != null)
          {
            if (code != null)
            {
              QrSession.PerBrowserData perBrowserData = ((IEnumerable<QrSession.PerBrowserData>) state.Browsers).Where<QrSession.PerBrowserData>((Func<QrSession.PerBrowserData, bool>) (pb => pb.BrowserId.IsEqualBytes(browserid))).FirstOrDefault<QrSession.PerBrowserData>();
              if (perBrowserData != null && !perBrowserData.Crypto.GenHmac(perBrowserData.CryptKey, 0, perBrowserData.CryptKey.Length).IsEqualBytes(code))
              {
                Log.l("web", "Session DeleteBrowser FAIL - Code Mismatch");
                return;
              }
            }
            QrSession.PerBrowserData activeBrowser;
            if ((activeBrowser = state.GetActiveBrowser()) != null && activeBrowser.BrowserId.IsEqualBytes(browserid))
            {
              this.UpdateState(state);
              this.Disconnect();
              state = this.State;
            }
            state.RemoveBrowsers((Func<QrSession.PerBrowserData, bool>) (pb => pb.BrowserId.IsEqualBytes(browserid)));
            flag = true;
          }
        }
        if (!flag)
          return;
        Log.l("web", "Session DeleteBrowser - BrowserId: {0}", browserid != null ? (object) Convert.ToBase64String(browserid) : (object) "");
        this.UpdateState(state);
      }
    }

    public void SetActiveForgetMe(bool forgetMe)
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state == null)
          return;
        QrSession.PerBrowserData activeBrowser = state.GetActiveBrowser();
        if (activeBrowser == null)
          return;
        activeBrowser.ForgetMe = forgetMe;
        this.UpdateState(state);
      }
    }

    public void PurgeExpiredBrowsers()
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state == null || state.Browsers == null)
          return;
        state.RemoveBrowsers((Func<QrSession.PerBrowserData, bool>) (b => b.ForgetMe && b.Expiration.HasValue && b.Expiration.Value < DateTime.Now));
        this.UpdateState(state);
      }
    }

    public DateTime? GetFirstExpirationTime()
    {
      lock (this.@lock)
      {
        QrSession.SessionState state = this.State;
        if (state != null && state.Browsers != null)
        {
          QrSession.PerBrowserData perBrowserData = ((IEnumerable<QrSession.PerBrowserData>) state.Browsers).Where<QrSession.PerBrowserData>((Func<QrSession.PerBrowserData, bool>) (b => b.ForgetMe && b.Expiration.HasValue && b.Expiration.Value > DateTime.Now)).OrderBy<QrSession.PerBrowserData, DateTime?>((Func<QrSession.PerBrowserData, DateTime?>) (b => b.Expiration)).FirstOrDefault<QrSession.PerBrowserData>();
          if (perBrowserData != null)
          {
            Log.l("web", "found an expiring browser");
            return perBrowserData.Expiration;
          }
        }
        return new DateTime?();
      }
    }

    private QrSession.SessionState LoadState(byte[] blob)
    {
      IRecToken instance = (IRecToken) NativeInterfaces.CreateInstance<RecToken>();
      IByteBuffer byteBuffer = (IByteBuffer) null;
      try
      {
        byteBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        byteBuffer.PutWithCopy(blob);
        instance.Decode(byteBuffer);
        blob = byteBuffer.Get();
      }
      finally
      {
        byteBuffer?.Reset();
      }
      using (MemoryStream memoryStream = new MemoryStream(blob, 0, blob.Length, false))
        return new DataContractJsonSerializer(typeof (QrSession.SessionState)).ReadObject((Stream) memoryStream) as QrSession.SessionState;
    }

    private void UpdateState(QrSession.SessionState state)
    {
      byte[] numArray = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (QrSession.SessionState)).WriteObject((Stream) memoryStream, (object) state);
        IRecToken instance1 = (IRecToken) NativeInterfaces.CreateInstance<RecToken>();
        IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        try
        {
          instance2.Put(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
          instance1.Encode(instance2);
          numArray = instance2.Get();
        }
        finally
        {
          instance2.Reset();
        }
      }
      this.state = state;
      Settings.QrBlob = this.cachedBlob = numArray;
      this.UpdateSessionsInfo();
    }

    private static byte[] FromBase64(string str)
    {
      byte[] numArray = (byte[]) null;
      if (str != null)
        numArray = Convert.FromBase64String(str);
      return numArray;
    }

    private static string ToBase64(byte[] blob)
    {
      string base64 = (string) null;
      if (blob != null)
        base64 = Convert.ToBase64String(blob);
      return base64;
    }

    private static byte[] HelperGet(ref byte[] cache, Func<string> source)
    {
      return cache ?? (cache = QrSession.FromBase64(source()));
    }

    private static void HelperSet(ref byte[] cache, byte[] value, Action<string> set)
    {
      if (cache.IsEqualBytes(value))
        return;
      cache = value;
      set(QrSession.ToBase64(value));
    }

    public void ProcessEpoch(int epoch)
    {
      if (this.PendingActions.Epoch == epoch)
        return;
      this.PendingActions.Actions.Clear();
      this.SaveActionsCache();
    }

    public QrSession.ActionsCache PendingActions
    {
      get
      {
        if (this.pendingActions == null)
          this.LoadActionsCache();
        return this.pendingActions;
      }
    }

    public void SetActionStatus(string id, int status)
    {
      this.PendingActions.Actions[id] = status;
      this.SaveActionsCache();
    }

    private void LoadActionsCache()
    {
      byte[] qrActionsBlob = Settings.QrActionsBlob;
      if (qrActionsBlob == null)
      {
        this.pendingActions = new QrSession.ActionsCache();
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream(qrActionsBlob, 0, qrActionsBlob.Length, false))
          this.pendingActions = new DataContractJsonSerializer(typeof (QrSession.ActionsCache)).ReadObject((Stream) memoryStream) as QrSession.ActionsCache;
      }
    }

    private void SaveActionsCache()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (QrSession.ActionsCache)).WriteObject((Stream) memoryStream, (object) this.pendingActions);
        Settings.QrActionsBlob = memoryStream.ToArray();
      }
    }

    [DataContract]
    public class SessionState
    {
      private byte[] sessionData;

      public byte[] SessionData
      {
        get
        {
          return QrSession.HelperGet(ref this.sessionData, (Func<string>) (() => this.SessionDataString));
        }
        set
        {
          QrSession.HelperSet(ref this.sessionData, value, (Action<string>) (s => this.SessionDataString = s));
        }
      }

      [DataMember(Name = "s")]
      public string SessionDataString { get; set; }

      [DataMember(Name = "b")]
      public int? ActiveBrowser { get; set; }

      [DataMember(Name = "bs")]
      public QrSession.PerBrowserData[] Browsers { get; set; }

      public QrSession.PerBrowserData GetActiveBrowser()
      {
        int index = this.ActiveBrowser ?? -1;
        return this.Browsers != null && index >= 0 && index < this.Browsers.Length ? this.Browsers[index] : (QrSession.PerBrowserData) null;
      }

      public void RemoveBrowsers(Func<QrSession.PerBrowserData, bool> removePred)
      {
        int length = 0;
        for (int index = 0; index < this.Browsers.Length; ++index)
        {
          if (removePred(this.Browsers[index]))
          {
            int? nullable1 = this.ActiveBrowser;
            if (nullable1.HasValue)
            {
              nullable1 = this.ActiveBrowser;
              int num1 = index;
              if ((nullable1.GetValueOrDefault() == num1 ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
              {
                nullable1 = new int?();
                this.ActiveBrowser = nullable1;
              }
              else
              {
                nullable1 = this.ActiveBrowser;
                int num2 = index;
                if ((nullable1.GetValueOrDefault() > num2 ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
                {
                  nullable1 = this.ActiveBrowser;
                  int? nullable2 = nullable1;
                  int num3 = 1;
                  this.ActiveBrowser = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() - num3) : new int?();
                }
              }
            }
          }
          else
            this.Browsers[length++] = this.Browsers[index];
        }
        if (length == this.Browsers.Length)
          return;
        QrSession.PerBrowserData[] destinationArray = new QrSession.PerBrowserData[length];
        Array.Copy((Array) this.Browsers, (Array) destinationArray, length);
        this.Browsers = destinationArray;
      }

      [DataMember(Name = "d")]
      public bool QrDemoShown { get; set; }

      [DataMember(Name = "v")]
      public FunXMPP.VerifySessionType ChallengeOriginatingType { get; set; }
    }

    [DataContract]
    public class PerBrowserData
    {
      private byte[] browserId;
      private byte[] cryptKey;
      private byte[] macKey;
      private byte[] clientToken;
      private byte[] challenge;
      private QrCrypto crypto;

      public byte[] BrowserId
      {
        get => QrSession.HelperGet(ref this.browserId, (Func<string>) (() => this.BrowserIdString));
        set
        {
          QrSession.HelperSet(ref this.browserId, value, (Action<string>) (s => this.BrowserIdString = s));
        }
      }

      [DataMember(Name = "b")]
      public string BrowserIdString { get; set; }

      public byte[] CryptKey
      {
        get => QrSession.HelperGet(ref this.cryptKey, (Func<string>) (() => this.CryptKeyString));
        set
        {
          QrSession.HelperSet(ref this.cryptKey, value, (Action<string>) (s => this.CryptKeyString = s));
        }
      }

      [DataMember(Name = "c")]
      public string CryptKeyString { get; set; }

      public byte[] MacKey
      {
        get => QrSession.HelperGet(ref this.macKey, (Func<string>) (() => this.MacKeyString));
        set
        {
          QrSession.HelperSet(ref this.macKey, value, (Action<string>) (s => this.MacKeyString = s));
        }
      }

      [DataMember(Name = "m")]
      public string MacKeyString { get; set; }

      public byte[] ClientToken
      {
        get
        {
          return QrSession.HelperGet(ref this.clientToken, (Func<string>) (() => this.ClientTokenString));
        }
        set
        {
          QrSession.HelperSet(ref this.clientToken, value, (Action<string>) (s => this.ClientTokenString = s));
        }
      }

      [DataMember(Name = "t")]
      public string ClientTokenString { get; set; }

      public byte[] Challenge
      {
        get => QrSession.HelperGet(ref this.challenge, (Func<string>) (() => this.ChallengeString));
        set
        {
          QrSession.HelperSet(ref this.challenge, value, (Action<string>) (s => this.ChallengeString = s));
        }
      }

      [DataMember(Name = "ch")]
      public string ChallengeString { get; set; }

      [DataMember(Name = "os")]
      public string OperatingSystem { get; set; }

      [DataMember(Name = "bName")]
      public string BrowserName { get; set; }

      [DataMember(Name = "lc")]
      public DateTime LastConnected { get; set; }

      [DataMember(Name = "fc")]
      public DateTime FirstConnected { get; set; }

      public QrCrypto Crypto
      {
        get => this.crypto ?? (this.crypto = new QrCrypto(this.CryptKey, this.MacKey));
        set => this.crypto = value;
      }

      [DataMember(Name = "l")]
      public string Location { get; set; }

      [DataMember(Name = "f")]
      public bool ForgetMe { get; set; }

      [DataMember(Name = "e")]
      public DateTime? Expiration { get; set; }
    }

    [DataContract]
    public class ActionsCache
    {
      [DataMember(Name = "i")]
      public bool InvalidForQuery;

      public ActionsCache() => this.Actions = new Dictionary<string, int>();

      [DataMember(Name = "e")]
      public int Epoch { get; set; }

      [DataMember(Name = "a")]
      public Dictionary<string, int> Actions { get; set; }
    }
  }
}
