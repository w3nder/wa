// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppToast
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp
{
  public class InAppToast : InAppFloatingBanner
  {
    private double timeoutInSeconds;
    private DispatcherTimer timeoutTimer;

    protected InAppToast()
    {
    }

    public static void ShowForMessage(Message msg)
    {
      if (msg == null)
        return;
      Log.d("inapp toast", "keyid:{0},type:{1},jid:{2}", (object) msg.KeyId, (object) msg.MediaWaType, (object) msg.KeyRemoteJid);
      Uri newUri = (Uri) null;
      Uri pageUri = WaUris.ChatPage();
      NavUtils.BackStackOpParams backStackOpParams = (NavUtils.BackStackOpParams) null;
      App.CurrentApp.ValidateNavigation(pageUri, NavigationMode.New, true, true, false, out newUri, out backStackOpParams);
      if (newUri != (Uri) null)
      {
        Log.l("inapp toast", "blocked | msg id:{0}", (object) msg.MessageID);
      }
      else
      {
        string destJid = msg.KeyRemoteJid;
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          new InAppToast()
          {
            view = ((IInAppFloatingBannerView) InAppFloatingBannerView.CreateForMessage(msg)),
            timeoutInSeconds = 5.0,
            onClick = ((Action) (() =>
            {
              if (App.CurrentApp.CurrentPage is ChatPage currentPage2 && currentPage2.Jid == destJid)
                return;
              WaUriParams additionalParams = new WaUriParams();
              additionalParams.AddString("Source", nameof (InAppToast));
              NavUtils.NavigateToChat(destJid, true, additionalParams: additionalParams);
            }))
          }.Show();
        }));
      }
    }

    public static void ShowForError(string errMsg, Action onComplete = null)
    {
      Log.l("inapp toast", string.Format("show err | {0}", (object) errMsg));
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        new InAppToast()
        {
          view = ((IInAppFloatingBannerView) InAppFloatingBannerView.CreateForError(errMsg)),
          timeoutInSeconds = 6.0,
          onComplete = onComplete
        }.Show();
      }));
    }

    public static void ShowForCallBatteryLevel(string msg)
    {
      Log.l("inapp toast", "show call battery level | {0}", (object) msg);
      if (Voip.IsInCall)
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          new InAppToast()
          {
            view = ((IInAppFloatingBannerView) InAppFloatingBannerView.CreateForString(msg)),
            timeoutInSeconds = 6.0
          }.Show();
        }));
      else
        Log.l("inapp toast", "skipping battery level message: Not in call");
    }

    public static void ShowForString(string msg, string destUriStr)
    {
      if (string.IsNullOrEmpty(msg))
        return;
      Log.l("inapp toast", "show | msg:{0},uri:{1}", (object) msg, (object) (destUriStr ?? "n/a"));
      Action onClick = (Action) null;
      if (!string.IsNullOrEmpty(destUriStr))
        onClick = (Action) (() => App.CurrentApp.RootFrame?.Navigate(new Uri(destUriStr, UriKind.Relative)));
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        new InAppToast()
        {
          view = ((IInAppFloatingBannerView) InAppFloatingBannerView.CreateForString(msg)),
          timeoutInSeconds = 6.0,
          onClick = onClick
        }.Show();
      }));
    }

    private void StartTimeoutTimer()
    {
      this.StopTimeoutTimer();
      if (this.timeoutTimer == null)
      {
        this.timeoutTimer = new DispatcherTimer()
        {
          Interval = TimeSpan.FromSeconds(this.timeoutInSeconds)
        };
        this.timeoutTimer.Tick += new EventHandler(this.OnTimeout);
      }
      this.timeoutTimer.Start();
    }

    private void StopTimeoutTimer()
    {
      if (this.timeoutTimer == null)
        return;
      this.timeoutTimer.Stop();
    }

    protected override void OnOpened()
    {
      base.OnOpened();
      this.StartTimeoutTimer();
    }

    protected override void OnClosed()
    {
      base.OnClosed();
      this.StopTimeoutTimer();
    }

    protected override void OnContentDraggingStarted()
    {
      base.OnContentDraggingStarted();
      this.StopTimeoutTimer();
    }

    protected override void OnContentDraggingEnded()
    {
      base.OnContentDraggingEnded();
      this.StartTimeoutTimer();
    }

    private void OnTimeout(object sender, EventArgs e)
    {
      this.StopTimeoutTimer();
      (this.view == null ? Observable.Empty<Unit>() : this.view.HandleTimeout()).Subscribe<Unit>((Action<Unit>) (_ => { }), (Action) (() => this.Close()));
    }
  }
}
