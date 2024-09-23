// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4HostSelector
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Threading;
using WhatsApp.Events;


namespace WhatsApp
{
  public class Mms4HostSelector
  {
    private const string LogHdr = "mms4hs";
    private static object createLock = new object();
    private static Mms4HostSelector _selectorInstance;
    private object selectionLock = new object();
    private static int MIN_RE_REQUEST_DELAY = 5;
    private static long LEEWAY_TICKS = TimeSpan.FromSeconds(15.0).Ticks;
    private static long MIN_TIME_LEFT_TICKS = TimeSpan.FromSeconds(5.0).Ticks;
    public Subject<Mms4ConnectionBlock> BlockCreatedSubject = new Subject<Mms4ConnectionBlock>();
    private Mms4ConnectionBlock currentBlock;
    private long requestedUpdateTimeUtcTicks;
    private Mms4FibonacciBackoffCalculator backoffCalculator;
    private long backOffUntilTimeUtcTicks;
    private const int WAIT_TIME_SECS = 15;
    private const long PRE_WARM_MIN_INTERVAL_TIME_TICKS = 150000000;
    private long nextPrewarmRequest;
    private IDisposable stopPrewarm;

    public static Mms4HostSelector GetInstance()
    {
      if (Mms4HostSelector._selectorInstance == null)
      {
        lock (Mms4HostSelector.createLock)
        {
          if (Mms4HostSelector._selectorInstance == null)
          {
            Mms4HostSelector._selectorInstance = new Mms4HostSelector();
            Mms4HostSelector._selectorInstance.currentBlock = (Mms4ConnectionBlock) null;
            Mms4HostSelector._selectorInstance.backOffUntilTimeUtcTicks = Settings.Mms4QueryBackoffUntilTimeUtcTicks;
            Mms4HostSelector._selectorInstance.backoffCalculator = new Mms4FibonacciBackoffCalculator();
          }
        }
      }
      return Mms4HostSelector._selectorInstance;
    }

    public IObservable<Mms4HostSelector.Mms4HostSelection> GetSelectedHostObservable(
      bool downloadFlag,
      FunXMPP.FMessage.FunMediaType mediaType,
      bool waitForPrewarm)
    {
      lock (this.selectionLock)
      {
        Mms4ConnectionBlock currentBlock = this.CurrentBlock;
        if (currentBlock != null)
        {
          string hostname = currentBlock.GetHostname(downloadFlag, mediaType);
          if (hostname != null)
            return Observable.Return<Mms4HostSelector.Mms4HostSelection>(new Mms4HostSelector.Mms4HostSelection(hostname, currentBlock.AuthToken));
        }
        return Observable.Create<Mms4HostSelector.Mms4HostSelection>((Func<IObserver<Mms4HostSelector.Mms4HostSelection>, Action>) (observer =>
        {
          bool completed = false;
          bool waitingForPrewarm = waitForPrewarm;
          IDisposable disp = (IDisposable) null;
          disp = this.BlockCreatedSubject.Subscribe<Mms4ConnectionBlock>((Action<Mms4ConnectionBlock>) (cb =>
          {
            if (completed || cb == null)
              return;
            if (waitingForPrewarm)
            {
              Log.l("mms4hs", "not notifying yet as request wanted to wait for prewarm");
              waitingForPrewarm = false;
            }
            else
            {
              observer.OnNext(new Mms4HostSelector.Mms4HostSelection(cb.GetHostname(downloadFlag, mediaType), cb.AuthToken));
              completed = true;
              disp.SafeDispose();
              disp = (IDisposable) null;
            }
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "exception getting selected host");
            throw ex;
          }));
          return (Action) (() =>
          {
            completed = true;
            disp.SafeDispose();
            disp = (IDisposable) null;
          });
        }));
      }
    }

    private bool RequestRoutingInfoIfNearlyExpired()
    {
      Log.d("mms4hs", nameof (RequestRoutingInfoIfNearlyExpired));
      bool flag = this.IsRoutingInfoExpired(Mms4HostSelector.LEEWAY_TICKS);
      if (flag)
      {
        lock (this.selectionLock)
        {
          if (this.requestedUpdateTimeUtcTicks < DateTime.UtcNow.Ticks)
          {
            Log.l("mms4hs", nameof (RequestRoutingInfoIfNearlyExpired));
            try
            {
              this.RequestUpdatedRoutingInfo(false);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "mms4 exception updating routing information");
            }
          }
        }
      }
      return flag;
    }

    private bool IsRoutingInfoExpired(long leeway)
    {
      bool flag = false;
      lock (this.selectionLock)
        flag = this.currentBlock == null || this.currentBlock.RouteExpiryTime <= DateTime.UtcNow.Ticks + leeway;
      Log.d("mms4hs", "IsRoutingInfoExpired {0} {1}", (object) leeway, (object) flag);
      return flag;
    }

    public bool RequestUpdatedRoutingInfo(bool startedByPersistentAction)
    {
      Log.l("mms4hs", nameof (RequestUpdatedRoutingInfo));
      bool flag = false;
      if (Mms4ServerPropHelper.IsMms4Enabled())
      {
        long updateTimeUtcTicks = this.requestedUpdateTimeUtcTicks;
        DateTime dateTime = DateTime.UtcNow;
        long ticks1 = dateTime.Ticks;
        if (updateTimeUtcTicks <= ticks1)
        {
          if (this.requestedUpdateTimeUtcTicks != 0L)
            Log.l("mms4hs", "Repeating request - assuming there is a problem with the preceding request");
          dateTime = DateTime.UtcNow;
          dateTime = dateTime.AddSeconds((double) Mms4HostSelector.MIN_RE_REQUEST_DELAY);
          this.requestedUpdateTimeUtcTicks = dateTime.Ticks;
          RouteSelection routeSelectionFs = (RouteSelection) null;
          Action<int> onError = (Action<int>) (err =>
          {
            this.requestedUpdateTimeUtcTicks = 0L;
            this.OnMediaRoutingRequestError(err);
          });
          Action<FunXMPP.ProtocolTreeNode> onComplete = (Action<FunXMPP.ProtocolTreeNode>) (mediaConnNode => ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
          {
            try
            {
              Mms4ConnectionBlock cb = new Mms4ConnectionBlock(mediaConnNode, ref routeSelectionFs);
              Log.d("mms4hs", "setting non prewarmed result");
              this.CurrentBlock = cb;
              Mms4HostSelector.PerformRouteChecking(cb, ref routeSelectionFs);
              Log.d("mms4hs", "setting prewarmed result");
              cb.IsUpdated = true;
              this.CurrentBlock = cb;
            }
            catch (Exception ex)
            {
              Log.SendCrashLog(ex, "Processing mediaConnection", logOnlyForRelease: true);
            }
          })));
          FunXMPP.Connection connection = AppState.GetConnection();
          if (connection != null)
          {
            try
            {
              flag = true;
              routeSelectionFs = new RouteSelection();
              dateTime = DateTime.UtcNow;
              long ticks2 = dateTime.Ticks;
              connection.SendMms4EndpointQuery(onComplete, onError);
            }
            catch (Exception ex)
            {
              string context = string.Format("Getting server data");
              Log.LogException(ex, context);
              this.requestedUpdateTimeUtcTicks = 0L;
            }
          }
          if (!flag)
          {
            if (!startedByPersistentAction)
            {
              Mms4Helper.MaybeScheduleMms4RouteSelection();
              goto label_12;
            }
            else
            {
              this.requestedUpdateTimeUtcTicks = 0L;
              goto label_12;
            }
          }
          else
            goto label_12;
        }
      }
      Log.l("mms4hs", "not sending request {0} {1}", (object) Mms4ServerPropHelper.IsMms4Enabled(), (object) this.requestedUpdateTimeUtcTicks);
label_12:
      return flag;
    }

    public Mms4ConnectionBlock CurrentBlock
    {
      get
      {
        this.RequestRoutingInfoIfNearlyExpired();
        lock (this.selectionLock)
        {
          if (this.currentBlock != null && this.currentBlock.RouteExpiryTime < DateTime.UtcNow.Ticks + Mms4HostSelector.MIN_TIME_LEFT_TICKS)
            return (Mms4ConnectionBlock) null;
          Mms4ConnectionBlock currentBlock = this.currentBlock;
          return (currentBlock != null ? currentBlock.HostCount : -1) < 1 ? (Mms4ConnectionBlock) null : this.currentBlock;
        }
      }
      private set
      {
        bool flag = false;
        lock (this.selectionLock)
        {
          if (value == null)
          {
            flag = this.currentBlock != null;
            this.currentBlock = (Mms4ConnectionBlock) null;
          }
          else if (this.currentBlock == null || this.currentBlock.IsUpdated)
          {
            this.currentBlock = value;
            this.currentBlock.IsUpdated = false;
            this.backoffCalculator.Reset();
            this.BackOffUntilTimeUtcTicks = 0L;
            flag = true;
          }
          else
            Log.d("mms4hs", "Not firing {0}", (object) this.currentBlock.IsUpdated);
        }
        if (!flag)
          return;
        Log.l("mms4hs", "New connection block selected");
        this.BlockCreatedSubject.OnNext(this.currentBlock);
      }
    }

    public long BackOffUntilTimeUtcTicks
    {
      get => this.backOffUntilTimeUtcTicks;
      private set
      {
        Settings.Mms4QueryBackoffUntilTimeUtcTicks = value;
        this.backOffUntilTimeUtcTicks = value;
      }
    }

    public void OnMediaRoutingRequestError(int code)
    {
      Log.l("mms4hs", "OnMediaRoutingRequestError code {0}", (object) code);
      if (503 != code)
        return;
      this.BackOffUntilTimeUtcTicks = DateTime.UtcNow.Ticks + this.backoffCalculator.GetSleepTimeMs() * 10000L;
      Mms4Helper.MaybeScheduleMms4RouteSelection();
    }

    public void OnMediaTransferErrorOrResponseCode(int code)
    {
      Log.l("mms4hs", "OnMediaTransferErrorOrResponseCode code {0}", (object) code);
      if (code != 401 && code != 403)
        return;
      try
      {
        Mms4Helper.MaybeScheduleMms4RouteSelection();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "mms4 exception updating routing");
      }
    }

    public void OnMediaUseImminent(bool download, FunXMPP.FMessage.FunMediaType mediaType)
    {
      if (!Mms4ServerPropHelper.IsMms4Enabled() || this.RequestRoutingInfoIfNearlyExpired() || !NetworkStateMonitor.IsDataConnected())
        return;
      lock (this.selectionLock)
        this.MaybePrewarmSelectedHost(this.currentBlock.GetHostname(download, mediaType), this.currentBlock.AuthToken);
    }

    private static void PerformRouteChecking(
      Mms4ConnectionBlock cb,
      ref RouteSelection routeSelectionFs)
    {
      object obj = new object();
      int hostIndexForFs = 0;
      string authToken = cb.AuthToken;
      string[] hostList = cb.GetHostList();
      bool[] flagArray1 = new bool[2];
      foreach (string hostName in hostList)
      {
        IObservable<Mms4HostSelector.PreWarmResut> rightSource = Mms4HostSelector.checkPrewarm(hostName, (string) null, authToken, hostIndexForFs, -1);
        IObservable<Mms4HostSelector.PreWarmResut> source = Observable.Timer(TimeSpan.FromSeconds(15.0)).Select<long, Mms4HostSelector.PreWarmResut>((Func<long, Mms4HostSelector.PreWarmResut>) (endTime => new Mms4HostSelector.PreWarmResut()
        {
          respCode = Mms4HostSelector.PreWarmResut.ResponseCodes.TIMEOUT
        })).Merge<Mms4HostSelector.PreWarmResut>(rightSource);
        int waitingForCount = 1;
        Func<Mms4HostSelector.PreWarmResut, bool> predicate = (Func<Mms4HostSelector.PreWarmResut, bool>) (routeWithCode =>
        {
          if (routeWithCode.respCode == Mms4HostSelector.PreWarmResut.ResponseCodes.TIMEOUT || routeWithCode.respCode == Mms4HostSelector.PreWarmResut.ResponseCodes.UNKNOWN)
          {
            Log.l("mms4hs", "incomplete checks {0} {1}", (object) waitingForCount, (object) routeWithCode.respCode);
            return true;
          }
          --waitingForCount;
          if (routeWithCode.RouteHostName == null)
            return waitingForCount <= 0;
          Log.l("mms4hs", "found {0}", (object) routeWithCode.RouteHostName);
          return true;
        });
        if (source.Where<Mms4HostSelector.PreWarmResut>(predicate).FirstOrDefault<Mms4HostSelector.PreWarmResut>()?.RouteHostName == null)
        {
          cb.MarkHostAsBad(hostName);
        }
        else
        {
          bool[] flagArray2 = cb.MarkHostAsGood(hostName);
          flagArray1[0] |= flagArray2[0];
          flagArray1[1] |= flagArray2[1];
          if (flagArray1[0] && flagArray1[1])
          {
            Log.l("mms4hs", "Stopping checking early - found required support");
            break;
          }
        }
      }
    }

    private static IObservable<Mms4HostSelector.PreWarmResut> checkPrewarm(
      string hostName,
      string ipAddress,
      string authToken,
      int hostIndexForFs,
      int ipIndexForFs)
    {
      Log.d("mms4hs", "checking prewarm on authority {0}, ip:{1}", (object) hostName, (object) (ipAddress != null));
      long checkAuthorizationStartTimeUtcTicks = DateTime.UtcNow.Ticks;
      string authCheckUrl = "https://" + hostName + "/prewarm";
      return Observable.Create<Mms4HostSelector.PreWarmResut>((Func<IObserver<Mms4HostSelector.PreWarmResut>, Action>) (observer =>
      {
        bool cancelled = false;
        IDisposable disp = NativeWeb.Mms4AuthCheck(authCheckUrl, ipAddress).Subscribe<int>((Action<int>) (code =>
        {
          if (cancelled)
            return;
          Mms4HostSelector.PreWarmResut preWarmResut = new Mms4HostSelector.PreWarmResut();
          preWarmResut.CompletedAuthCheck = true;
          if (code == 200)
          {
            Log.d("mms4hs", "prewarmed {0}, ip:{1}", (object) hostName, (object) (ipAddress != null));
            preWarmResut.RouteHostName = hostName;
            preWarmResut.respCode = Mms4HostSelector.PreWarmResut.ResponseCodes.OK;
            preWarmResut.FsHostIndex = hostIndexForFs;
            preWarmResut.FsIpIndex = ipIndexForFs;
            preWarmResut.FsAuthCheckTimeTicks = DateTime.UtcNow.Ticks - checkAuthorizationStartTimeUtcTicks;
          }
          else
          {
            Log.l("mms4hs", "prewarm resp code {0}", (object) code);
            preWarmResut.respCode = Mms4HostSelector.PreWarmResut.ResponseCodes.HTTP_CODE_OTHER;
          }
          observer.OnNext(preWarmResut);
        }), (Action<Exception>) (err => Log.LogException(err, "mms4 Prewarm Exception")), (Action) (() =>
        {
          if (cancelled)
            return;
          observer.OnCompleted();
        }));
        return (Action) (() =>
        {
          cancelled = true;
          disp.SafeDispose();
        });
      }));
    }

    private void MaybePrewarmSelectedHost(string hostname, string authToken)
    {
      if (this.nextPrewarmRequest >= DateTime.UtcNow.Ticks)
        return;
      this.nextPrewarmRequest = DateTime.UtcNow.Ticks + 150000000L;
      this.stopPrewarm.SafeDispose();
      this.stopPrewarm = Mms4HostSelector.checkPrewarm(hostname, (string) null, authToken, -1, -1).Subscribe<Mms4HostSelector.PreWarmResut>((Action<Mms4HostSelector.PreWarmResut>) (route => Log.d("mms4hs", "pre warm finished {0}", (object) route.respCode, (object) (this.stopPrewarm == null))), (Action<Exception>) (ex => Log.LogException(ex, "mms4 prewarm exception")), (Action) (() => this.stopPrewarm = (IDisposable) null));
    }

    public class Mms4HostSelection
    {
      public string HostName { get; private set; }

      public string AuthToken { get; private set; }

      public Mms4HostSelection(string hostName, string authToken)
      {
        this.HostName = hostName;
        this.AuthToken = authToken;
      }
    }

    private class PreWarmResut
    {
      public string RouteHostName;
      public bool CompletedAuthCheck;
      public Mms4HostSelector.PreWarmResut.ResponseCodes respCode;
      public long FsAuthCheckTimeTicks;
      public int FsHostIndex = -1;
      public int FsIpIndex = -1;

      public enum ResponseCodes
      {
        UNKNOWN,
        OK,
        TIMEOUT,
        HTTP_CODE_OTHER,
      }
    }
  }
}
