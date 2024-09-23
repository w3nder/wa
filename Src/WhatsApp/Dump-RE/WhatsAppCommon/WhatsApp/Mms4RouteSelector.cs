// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4RouteSelector
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WhatsApp.Events;

#nullable disable
namespace WhatsApp
{
  public class Mms4RouteSelector
  {
    private static long LEEWAY_TICKS = TimeSpan.FromSeconds(15.0).Ticks;
    private static long MIN_TIME_LEFT_TICKS = TimeSpan.FromSeconds(5.0).Ticks;
    public Subject<Mms4RouteSelector.SelectedRoute> RouteSelectedSubject = new Subject<Mms4RouteSelector.SelectedRoute>();
    private static object createLock = new object();
    private static Mms4RouteSelector _selectorInstance;
    private object selectionLock = new object();
    private Mms4RouteSelector.SelectedRoute currentlySelectedRoute;
    private long requestedUpdateTimeUtcTicks;
    private Mms4FibonacciBackoffCalculator backoffCalculator;
    private long backOffUntilTimeUtcTicks;
    private static int MIN_RE_REQUEST_DELAY = 5;
    private const int MINTTL_120_SECS = 120;
    private const int MINTTL_DOWNGRADE_PROPORTION = 5;
    private const int WAIT_TIME_SECS = 15;
    private const long PRE_WARM_MIN_INTERVAL_TIME_TICKS = 150000000;
    private long nextPrewarmRequest;
    private IDisposable stopPrewarm;

    public Mms4RouteSelector.SelectedRoute CurrentSelectedRoute
    {
      get
      {
        this.RequestRoutingInfoIfNearlyExpired();
        lock (this.selectionLock)
          return this.currentlySelectedRoute != null && this.currentlySelectedRoute.RouteExpiryTimeUtcTicks < DateTime.UtcNow.Ticks + Mms4RouteSelector.MIN_TIME_LEFT_TICKS || this.currentlySelectedRoute?.RouteHostName == null ? (Mms4RouteSelector.SelectedRoute) null : this.currentlySelectedRoute;
      }
      private set
      {
        bool flag = false;
        lock (this.selectionLock)
        {
          if (value == null)
          {
            flag = this.currentlySelectedRoute != null;
            this.currentlySelectedRoute = (Mms4RouteSelector.SelectedRoute) null;
          }
          else
          {
            if (this.currentlySelectedRoute != null)
            {
              if (this.currentlySelectedRoute.RouteExpiryTimeUtcTicks >= value.RouteExpiryTimeUtcTicks)
                goto label_10;
            }
            this.currentlySelectedRoute = value;
            this.currentlySelectedRoute.ToSettings();
            this.backoffCalculator.Reset();
            this.BackOffUntilTimeUtcTicks = 0L;
            flag = true;
          }
        }
label_10:
        if (!flag)
          return;
        Log.l("mms4", "New route selected");
        this.RouteSelectedSubject.OnNext(this.currentlySelectedRoute);
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

    public static Mms4RouteSelector GetInstance()
    {
      if (Mms4RouteSelector._selectorInstance == null)
      {
        lock (Mms4RouteSelector.createLock)
        {
          if (Mms4RouteSelector._selectorInstance == null)
          {
            Mms4RouteSelector._selectorInstance = new Mms4RouteSelector();
            Mms4RouteSelector._selectorInstance.currentlySelectedRoute = Mms4RouteSelector.SelectedRoute.FromSettings();
            Mms4RouteSelector._selectorInstance.backOffUntilTimeUtcTicks = Settings.Mms4QueryBackoffUntilTimeUtcTicks;
            Mms4RouteSelector._selectorInstance.backoffCalculator = new Mms4FibonacciBackoffCalculator();
          }
        }
      }
      return Mms4RouteSelector._selectorInstance;
    }

    public void OnMediaRoutingRequestError(int code)
    {
      Log.l("mms4", "OnMediaRoutingRequestError code {0}", (object) code);
      if (503 != code)
        return;
      this.BackOffUntilTimeUtcTicks = DateTime.UtcNow.Ticks + this.backoffCalculator.GetSleepTimeMs() * 10000L;
      Mms4Helper.MaybeScheduleMms4HostSelection();
    }

    public void OnMediaTransferErrorOrResponseCode(int code)
    {
      Log.l("mms4", "OnMediaTransferErrorOrResponseCode code {0}", (object) code);
      if (code != 401 && code != 403)
        return;
      try
      {
        Mms4RouteSelector.SelectedRoute.ClearSettings();
        Mms4Helper.MaybeScheduleMms4HostSelection();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "mms4 exception updating routing");
      }
    }

    public void OnMediaUseImminent()
    {
      if (!Mms4ServerPropHelper.IsMms4Enabled() || this.RequestRoutingInfoIfNearlyExpired() || !NetworkStateMonitor.IsDataConnected())
        return;
      lock (this.selectionLock)
        this.MaybePrewarmSelectedRoute(this.currentlySelectedRoute);
    }

    public IObservable<Mms4RouteSelector.SelectedRoute> GetSelectedRouteObservable(
      bool waitForPrewarm)
    {
      lock (this.selectionLock)
      {
        Mms4RouteSelector.SelectedRoute currentSelectedRoute = this.CurrentSelectedRoute;
        return currentSelectedRoute != null ? Observable.Return<Mms4RouteSelector.SelectedRoute>(currentSelectedRoute) : Observable.Create<Mms4RouteSelector.SelectedRoute>((Func<IObserver<Mms4RouteSelector.SelectedRoute>, Action>) (observer =>
        {
          bool completed = false;
          bool waitingForPrewarm = waitForPrewarm;
          IDisposable disp = (IDisposable) null;
          disp = this.RouteSelectedSubject.Subscribe<Mms4RouteSelector.SelectedRoute>((Action<Mms4RouteSelector.SelectedRoute>) (sr =>
          {
            if (completed || sr == null)
              return;
            if (waitingForPrewarm && !sr.CompletedAuthCheck)
            {
              waitingForPrewarm = false;
            }
            else
            {
              observer.OnNext(sr);
              completed = true;
              disp.SafeDispose();
              disp = (IDisposable) null;
            }
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "exception getting selected route");
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
      Log.d("mms4", nameof (RequestRoutingInfoIfNearlyExpired));
      bool flag = this.IsRoutingInfoExpired(Mms4RouteSelector.LEEWAY_TICKS);
      if (flag)
      {
        lock (this.selectionLock)
        {
          if (this.requestedUpdateTimeUtcTicks < DateTime.UtcNow.Ticks)
          {
            Log.l("mms4", nameof (RequestRoutingInfoIfNearlyExpired));
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
        flag = this.currentlySelectedRoute == null || this.currentlySelectedRoute.RouteExpiryTimeUtcTicks <= DateTime.UtcNow.Ticks + leeway;
      Log.d("mms4", "IsRoutingInfoExpired {0} {1}", (object) leeway, (object) flag);
      return flag;
    }

    public bool RequestUpdatedRoutingInfo(bool startedByPersistentAction)
    {
      Log.l("mms4", nameof (RequestUpdatedRoutingInfo));
      bool flag = false;
      if (Mms4ServerPropHelper.IsMms4Enabled())
      {
        long updateTimeUtcTicks = this.requestedUpdateTimeUtcTicks;
        DateTime dateTime = DateTime.UtcNow;
        long ticks = dateTime.Ticks;
        if (updateTimeUtcTicks <= ticks)
        {
          if (this.requestedUpdateTimeUtcTicks != 0L)
            Log.l("mms4", "Repeating request - assuming there is a problem with the preceding request");
          dateTime = DateTime.UtcNow;
          dateTime = dateTime.AddSeconds((double) Mms4RouteSelector.MIN_RE_REQUEST_DELAY);
          this.requestedUpdateTimeUtcTicks = dateTime.Ticks;
          RouteSelection routeSelectionFs = (RouteSelection) null;
          long routeSelectionStartTimeUtcTicks = 0;
          Action<int> onError = (Action<int>) (err =>
          {
            this.requestedUpdateTimeUtcTicks = 0L;
            this.OnMediaRoutingRequestError(err);
          });
          Action<FunXMPP.ProtocolTreeNode> onComplete = (Action<FunXMPP.ProtocolTreeNode>) (mediaConnNode => ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
          {
            if (this.ProcessQueryResult(mediaConnNode, ref routeSelectionFs))
            {
              routeSelectionFs.routeSelectionT = new long?((DateTime.UtcNow.Ticks - routeSelectionStartTimeUtcTicks) / 10000L);
              routeSelectionFs.SaveEvent();
            }
            this.requestedUpdateTimeUtcTicks = 0L;
          })));
          FunXMPP.Connection connection = AppState.GetConnection();
          if (connection != null)
          {
            try
            {
              flag = true;
              routeSelectionFs = new RouteSelection();
              dateTime = DateTime.UtcNow;
              routeSelectionStartTimeUtcTicks = dateTime.Ticks;
              connection.SendMms4EndpointQuery(onComplete, onError);
            }
            catch (Exception ex)
            {
              string context = string.Format("Getting server data");
              Log.LogException(ex, context);
            }
          }
          if (!flag && !startedByPersistentAction)
          {
            Mms4Helper.MaybeScheduleMms4RouteSelection();
            goto label_10;
          }
          else
            goto label_10;
        }
      }
      Log.l("mms4", "not sending request {0} {1}", (object) Mms4ServerPropHelper.IsMms4Enabled(), (object) this.requestedUpdateTimeUtcTicks);
label_10:
      return flag;
    }

    private bool ProcessQueryResult(
      FunXMPP.ProtocolTreeNode mediaConnNode,
      ref RouteSelection routeSelectionFs)
    {
      string attributeValue1 = mediaConnNode.GetAttributeValue("auth");
      string attributeValue2 = mediaConnNode.GetAttributeValue("ttl");
      long num1 = 0;
      ref long local1 = ref num1;
      if (!long.TryParse(attributeValue2, out local1))
      {
        Log.l("mms4", "media connection node has incorrect ttl");
        return false;
      }
      long num2 = num1 - Math.Min(num1 / 5L, 120L);
      DateTime utcNow = DateTime.UtcNow;
      long num3 = utcNow.Ticks + TimeSpan.FromSeconds((double) num2).Ticks;
      string attributeValue3 = mediaConnNode.GetAttributeValue("auth_ttl");
      long num4 = 0;
      ref long local2 = ref num4;
      if (!long.TryParse(attributeValue3, out local2))
      {
        Log.l("mms4", "media connection node has incorrect auth_ttl");
        num4 = num2;
      }
      utcNow = DateTime.UtcNow;
      long num5 = utcNow.Ticks + TimeSpan.FromSeconds((double) num4).Ticks;
      Dictionary<string, List<Mms4RouteSelector.IpEndpointNode>> dictionary = new Dictionary<string, List<Mms4RouteSelector.IpEndpointNode>>();
      foreach (FunXMPP.ProtocolTreeNode allChild in mediaConnNode.GetAllChildren("host"))
      {
        string attributeValue4 = allChild.GetAttributeValue("hostname");
        if (attributeValue4 != null)
        {
          dictionary[attributeValue4] = (List<Mms4RouteSelector.IpEndpointNode>) null;
          List<Mms4RouteSelector.IpEndpointNode> ipEndpointNodeList = new List<Mms4RouteSelector.IpEndpointNode>();
          if (allChild.children != null)
          {
            foreach (FunXMPP.ProtocolTreeNode child in allChild.children)
            {
              if (child.tag == "ip4")
              {
                Mms4RouteSelector.IpEndpointNode ipEndpointNode = new Mms4RouteSelector.IpEndpointNode()
                {
                  IsIp6 = false,
                  IpAddress = child.GetDataString()
                };
                ipEndpointNodeList.Add(ipEndpointNode);
              }
              if (child.tag == "ip6")
              {
                Mms4RouteSelector.IpEndpointNode ipEndpointNode = new Mms4RouteSelector.IpEndpointNode()
                {
                  IsIp6 = true,
                  IpAddress = child.GetDataString()
                };
                ipEndpointNodeList.Add(ipEndpointNode);
              }
            }
          }
          if (ipEndpointNodeList.Count > 0 && attributeValue4 != null)
            dictionary[attributeValue4] = ipEndpointNodeList;
        }
      }
      if (dictionary.Count == 0)
      {
        Log.l("mms4", "auth request failed to find any nodes");
        return false;
      }
      Log.d("mms4", "found {0} nodes", (object) dictionary.Count);
      string key = dictionary.First<KeyValuePair<string, List<Mms4RouteSelector.IpEndpointNode>>>().Key;
      Log.d("mms4", "setting non prewarmed result {0}", (object) key);
      Mms4RouteSelector.SelectedRouteExtended selectedRouteExtended1 = new Mms4RouteSelector.SelectedRouteExtended();
      selectedRouteExtended1.CompletedAuthCheck = false;
      selectedRouteExtended1.RouteHostName = key;
      selectedRouteExtended1.RouteIpAddress = (string) null;
      selectedRouteExtended1.RouteAuthToken = attributeValue1;
      selectedRouteExtended1.respCode = Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.OK;
      selectedRouteExtended1.FsHostIndex = -1;
      selectedRouteExtended1.FsIpIndex = -1;
      selectedRouteExtended1.RouteExpiryTimeUtcTicks = num3 - 1L;
      selectedRouteExtended1.AuthExpiryTimeUtcTicks = num5 - 1L;
      this.CurrentSelectedRoute = (Mms4RouteSelector.SelectedRoute) selectedRouteExtended1;
      Mms4RouteSelector.SelectedRouteExtended selectedRouteExtended2 = Mms4RouteSelector.PerformRouteSelection(attributeValue1, dictionary, ref routeSelectionFs);
      if (selectedRouteExtended2 == null || selectedRouteExtended2.RouteHostName == null)
        return false;
      selectedRouteExtended2.RouteExpiryTimeUtcTicks = num3;
      selectedRouteExtended2.AuthExpiryTimeUtcTicks = num5;
      this.CurrentSelectedRoute = (Mms4RouteSelector.SelectedRoute) selectedRouteExtended2;
      routeSelectionFs.routeHostname = selectedRouteExtended2.RouteHostName;
      routeSelectionFs.routeIp = selectedRouteExtended2.RouteIpAddress;
      if (selectedRouteExtended2.FsHostIndex >= 0)
        routeSelectionFs.routeClassIndex = new long?((long) selectedRouteExtended2.FsHostIndex);
      if (selectedRouteExtended2.FsIpIndex >= 0)
        routeSelectionFs.ipAddressIndex = new long?((long) selectedRouteExtended2.FsIpIndex);
      routeSelectionFs.finalAuthcheckT = new long?(selectedRouteExtended2.FsAuthCheckTimeTicks / 10000L);
      return true;
    }

    private static Mms4RouteSelector.SelectedRouteExtended PerformRouteSelection(
      string authToken,
      Dictionary<string, List<Mms4RouteSelector.IpEndpointNode>> checkNodes,
      ref RouteSelection routeSelectionFs)
    {
      object obj = new object();
      int hostIndexForFs = 0;
      foreach (string key in checkNodes.Keys)
      {
        List<Mms4RouteSelector.IpEndpointNode> checkNode = checkNodes[key];
        IObservable<Mms4RouteSelector.SelectedRouteExtended> rightSource1 = (IObservable<Mms4RouteSelector.SelectedRouteExtended>) null;
        IObservable<Mms4RouteSelector.SelectedRouteExtended> rightSource2 = (IObservable<Mms4RouteSelector.SelectedRouteExtended>) null;
        if (checkNode != null)
        {
          int ipIndexForFs = 0;
          foreach (Mms4RouteSelector.IpEndpointNode ipEndpointNode in checkNode)
          {
            if (ipEndpointNode.IsIp6 && rightSource2 == null)
              rightSource2 = Mms4RouteSelector.checkPrewarm(key, ipEndpointNode.IpAddress, authToken, hostIndexForFs, ipIndexForFs);
            if (!ipEndpointNode.IsIp6 && rightSource1 == null)
              rightSource1 = Mms4RouteSelector.checkPrewarm(key, ipEndpointNode.IpAddress, authToken, hostIndexForFs, ipIndexForFs);
            ++ipIndexForFs;
          }
        }
        IObservable<Mms4RouteSelector.SelectedRouteExtended> rightSource3 = Mms4RouteSelector.checkPrewarm(key, (string) null, authToken, hostIndexForFs, -1);
        IObservable<Mms4RouteSelector.SelectedRouteExtended> observable = Observable.Timer(TimeSpan.FromSeconds(15.0)).Select<long, Mms4RouteSelector.SelectedRouteExtended>((Func<long, Mms4RouteSelector.SelectedRouteExtended>) (endTime => new Mms4RouteSelector.SelectedRouteExtended()
        {
          respCode = Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.TIMEOUT
        })).Merge<Mms4RouteSelector.SelectedRouteExtended>(rightSource3);
        int waitingForCount = 1;
        if (rightSource2 != null)
        {
          observable = observable.Merge<Mms4RouteSelector.SelectedRouteExtended>(rightSource2);
          waitingForCount++;
        }
        if (rightSource1 != null)
        {
          observable = observable.Merge<Mms4RouteSelector.SelectedRouteExtended>(rightSource1);
          waitingForCount++;
        }
        Mms4RouteSelector.SelectedRouteExtended selectedRouteExtended = observable.Where<Mms4RouteSelector.SelectedRouteExtended>((Func<Mms4RouteSelector.SelectedRouteExtended, bool>) (routeWithCode =>
        {
          if (routeWithCode.respCode == Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.TIMEOUT || routeWithCode.respCode == Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.UNKNOWN)
          {
            Log.l("mms4", "incomplete checks {0} {1}", (object) waitingForCount, (object) routeWithCode.respCode);
            return true;
          }
          --waitingForCount;
          if (routeWithCode.RouteHostName == null)
            return waitingForCount <= 0;
          Log.l("mms4", "found {0}", (object) routeWithCode.RouteHostName);
          return true;
        })).FirstOrDefault<Mms4RouteSelector.SelectedRouteExtended>();
        if (selectedRouteExtended != null && selectedRouteExtended.RouteHostName != null)
          return selectedRouteExtended;
      }
      return (Mms4RouteSelector.SelectedRouteExtended) null;
    }

    private static IObservable<Mms4RouteSelector.SelectedRouteExtended> checkPrewarm(
      string hostName,
      string ipAddress,
      string authToken,
      int hostIndexForFs,
      int ipIndexForFs)
    {
      Log.d("mms4", "checking prewarm on authority {0}, ip:{1}", (object) hostName, (object) (ipAddress != null));
      long checkAuthorizationStartTimeUtcTicks = DateTime.UtcNow.Ticks;
      string authCheckUrl = "https://" + hostName + "/prewarm";
      return Observable.Create<Mms4RouteSelector.SelectedRouteExtended>((Func<IObserver<Mms4RouteSelector.SelectedRouteExtended>, Action>) (observer =>
      {
        bool cancelled = false;
        IDisposable disp = NativeWeb.Mms4AuthCheck(authCheckUrl, ipAddress).Subscribe<int>((Action<int>) (code =>
        {
          if (cancelled)
            return;
          Mms4RouteSelector.SelectedRouteExtended selectedRouteExtended = new Mms4RouteSelector.SelectedRouteExtended();
          selectedRouteExtended.CompletedAuthCheck = true;
          if (code == 200)
          {
            Log.d("mms4", "prewarmed {0}, ip:{1}", (object) hostName, (object) (ipAddress != null));
            selectedRouteExtended.RouteHostName = hostName;
            selectedRouteExtended.RouteUploadHostName = hostName;
            selectedRouteExtended.RouteIpAddress = ipAddress;
            selectedRouteExtended.RouteAuthToken = authToken;
            selectedRouteExtended.respCode = Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.OK;
            selectedRouteExtended.FsHostIndex = hostIndexForFs;
            selectedRouteExtended.FsIpIndex = ipIndexForFs;
            selectedRouteExtended.FsAuthCheckTimeTicks = DateTime.UtcNow.Ticks - checkAuthorizationStartTimeUtcTicks;
          }
          else
          {
            Log.l("mms4", "prewarm resp code {0}", (object) code);
            selectedRouteExtended.respCode = Mms4RouteSelector.SelectedRouteExtended.ResponseCodes.HTTP_CODE_OTHER;
          }
          observer.OnNext(selectedRouteExtended);
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

    private void MaybePrewarmSelectedRoute(Mms4RouteSelector.SelectedRoute prewarmRoute)
    {
      if (this.nextPrewarmRequest >= DateTime.UtcNow.Ticks)
        return;
      this.nextPrewarmRequest = DateTime.UtcNow.Ticks + 150000000L;
      this.stopPrewarm.SafeDispose();
      this.stopPrewarm = Mms4RouteSelector.checkPrewarm(prewarmRoute.RouteHostName, prewarmRoute.RouteIpAddress, prewarmRoute.RouteAuthToken, -1, -1).Subscribe<Mms4RouteSelector.SelectedRouteExtended>((Action<Mms4RouteSelector.SelectedRouteExtended>) (route => Log.d("mms4 prewarm", "Finished {0}", (object) route.respCode, (object) (this.stopPrewarm == null))), (Action<Exception>) (ex => Log.LogException(ex, "mms4 prewarm exception")), (Action) (() => this.stopPrewarm = (IDisposable) null));
    }

    public class SelectedRoute
    {
      public string RouteHostName;
      public string RouteIpAddress;
      public long RouteExpiryTimeUtcTicks;
      public string RouteAuthToken;
      public bool CompletedAuthCheck;
      public string RouteUploadHostName;
      public long AuthExpiryTimeUtcTicks;
      private const char SEP_CHAR = '/';

      public void ToSettings()
      {
        string str = (string) null;
        if (this.RouteHostName != null && this.RouteExpiryTimeUtcTicks > DateTime.UtcNow.Ticks)
          str = "1/" + this.RouteHostName + "/" + (this.RouteIpAddress != null ? this.RouteIpAddress : "") + "/" + this.RouteAuthToken + "/" + this.RouteExpiryTimeUtcTicks.ToString() + "/" + (this.CompletedAuthCheck ? "1" : "0") + "/" + this.RouteUploadHostName ?? "/" + this.AuthExpiryTimeUtcTicks.ToString();
        try
        {
          Settings.Mms4CurrentRoute = str;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception storing mms4 route information");
        }
      }

      public static Mms4RouteSelector.SelectedRoute FromSettings()
      {
        Mms4RouteSelector.SelectedRoute selectedRoute = new Mms4RouteSelector.SelectedRoute();
        try
        {
          string mms4CurrentRoute = Settings.Mms4CurrentRoute;
          if (mms4CurrentRoute != null)
          {
            string[] strArray = mms4CurrentRoute.Split('/');
            if (strArray != null)
            {
              if (strArray.Length >= 5)
              {
                if (strArray[0] == "1")
                {
                  if (long.Parse(strArray[4]) > DateTime.UtcNow.Ticks)
                  {
                    selectedRoute.RouteHostName = strArray[1];
                    selectedRoute.RouteIpAddress = (string) null;
                    selectedRoute.RouteAuthToken = strArray[3];
                    selectedRoute.RouteExpiryTimeUtcTicks = long.Parse(strArray[4]);
                    selectedRoute.CompletedAuthCheck = strArray.Length < 6 || strArray[5] == "1";
                    if (strArray.Length >= 7)
                      selectedRoute.RouteUploadHostName = strArray[6];
                    if (strArray.Length >= 8)
                      selectedRoute.AuthExpiryTimeUtcTicks = long.Parse(strArray[7]);
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception retrieving mms4 route information");
        }
        return selectedRoute;
      }

      public static void ClearSettings()
      {
        Log.l("mms4", "Clearing selected route");
        Settings.Mms4CurrentRoute = (string) null;
      }
    }

    private class SelectedRouteExtended : Mms4RouteSelector.SelectedRoute
    {
      public Mms4RouteSelector.SelectedRoute selectedRoute;
      public Mms4RouteSelector.SelectedRouteExtended.ResponseCodes respCode;
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

    public struct IpEndpointNode
    {
      public bool IsIp6;
      public string IpAddress;
    }
  }
}
