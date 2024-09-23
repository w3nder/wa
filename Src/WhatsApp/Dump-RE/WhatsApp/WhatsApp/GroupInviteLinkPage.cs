// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupInviteLinkPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.CommonOps;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

#nullable disable
namespace WhatsApp
{
  public class GroupInviteLinkPage : PhoneApplicationPage
  {
    private FunXMPP.Connection conn;
    private static string nextInstanceGroupJid = (string) null;
    private string GroupJid;
    private string link;
    public static string GroupInviteLinkStart = "https://chat.whatsapp.com/";
    private bool shareSet;
    public static HashSet<string> inviteLinksPending = new HashSet<string>();
    public static GlobalProgressIndicator globalProgressBar = (GlobalProgressIndicator) null;
    internal Grid LayoutRoot;
    internal Grid Content;
    internal PageTitlePanel TitlePanel;
    internal TextBlock InviteLink;
    internal TextBlock InviteLinkDescription;
    internal TextBlock ShareViaWhatsApp;
    internal TextBlock CopyLink;
    internal TextBlock RevokeLink;
    internal TextBlock ShareLink;
    private bool _contentLoaded;

    public GroupInviteLinkPage()
    {
      this.InitializeComponent();
      this.GroupJid = GroupInviteLinkPage.nextInstanceGroupJid;
      GroupInviteLinkPage.nextInstanceGroupJid = (string) null;
      if (this.GroupJid == null)
        return;
      this.conn = App.CurrentApp.Connection;
      this.conn.SendGetGroupInviteLink((Action<string>) (code =>
      {
        this.link = code == null || code == "" ? AppResources.InviteLinkDisplayFailed : GroupInviteLinkPage.GroupInviteLinkStart + code;
        this.Dispatcher.BeginInvoke((Action) (() => this.InviteLink.Text = this.link));
      }), (Action<string>) (error => this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.InviteLink.Text = error;
        this.link = (string) null;
      }))), this.GroupJid);
    }

    private void RevokeLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      UIUtils.MessageBox(" ", AppResources.RevokeConfirmation, (IEnumerable<string>) new string[2]
      {
        AppResources.RevokeLink,
        AppResources.CancelButton
      }, (Action<int>) (idx =>
      {
        if (idx != 0)
          return;
        if (!this.conn.IsConnected)
        {
          int num1 = (int) MessageBox.Show(AppResources.ConnectionStateOffline);
        }
        else
          this.conn.SendCreateGroupInviteLink((Action<string>) (code =>
          {
            if (code == null || code == "")
            {
              this.link = AppResources.InviteLinkDisplayFailed;
            }
            else
            {
              int num2;
              this.Dispatcher.BeginInvoke((Action) (() => num2 = (int) MessageBox.Show(AppResources.RevokeComplete)));
              this.link = GroupInviteLinkPage.GroupInviteLinkStart + code;
            }
            this.Dispatcher.BeginInvoke((Action) (() => this.InviteLink.Text = this.link));
          }), (Action<string>) (error => this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.InviteLink.Text = error;
            this.link = (string) null;
          }))), this.GroupJid);
      }));
    }

    public static IObservable<string> Start(string GroupJid = null)
    {
      return Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        GroupInviteLinkPage.nextInstanceGroupJid = GroupJid;
        NavUtils.NavigateToPage(nameof (GroupInviteLinkPage));
        return (Action) (() => { });
      }));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.GroupJid != null)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void ShareLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!this.shareSet)
      {
        DataTransferManager forCurrentView = DataTransferManager.GetForCurrentView();
        // ISSUE: method pointer
        WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, EventRegistrationToken>(forCurrentView.add_DataRequested), new Action<EventRegistrationToken>(forCurrentView.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object) this, __methodptr(ShareTextHandler)));
        this.shareSet = true;
      }
      DataTransferManager.ShowShareUI();
    }

    private void ShareViaWhatsApp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      Message message = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => message = new Message(true)
      {
        KeyFromMe = true,
        KeyRemoteJid = Settings.MyJid,
        KeyId = FunXMPP.GenerateMessageId(),
        Data = this.link,
        Status = FunXMPP.FMessage.Status.Unsent,
        MediaWaType = FunXMPP.FMessage.Type.Undefined
      }));
      if (message == null)
        return;
      SendMessage.ChooseRecipientAndSendNew(new Message[1]
      {
        message
      }, AppResources.SendLinkViaWhatsApp);
    }

    private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
    {
      string link = this.link;
      if (!this.conn.IsConnected)
      {
        int num1 = (int) MessageBox.Show(AppResources.ConnectionStateOffline);
      }
      else if (link != null)
      {
        DataRequest request = e.Request;
        DataRequestDeferral deferral = request.GetDeferral();
        request.Data.Properties.put_Title(AppResources.InviteLinkTitle);
        request.Data.SetText(link);
        deferral.Complete();
      }
      else
      {
        int num2 = (int) MessageBox.Show(AppResources.InviteLinkDisplayFailed);
      }
    }

    private void CopyLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.link != null && this.link != "")
      {
        Clipboard.SetText(this.link);
        int num = (int) MessageBox.Show(AppResources.CopyLinkConfirmation);
      }
      else
      {
        int num1 = (int) MessageBox.Show(AppResources.InviteLinkDisplayFailed);
      }
    }

    public static void JoinGroupWithInviteLink(string inviteLink)
    {
      if (!inviteLink.StartsWith(GroupInviteLinkPage.GroupInviteLinkStart, StringComparison.OrdinalIgnoreCase))
        return;
      if (inviteLink.EndsWith("/"))
        inviteLink.Remove(inviteLink.Length - 1, 1);
      int num = inviteLink.LastIndexOf('/');
      GroupInviteLinkPage.JoinGroupWithInviteCode(inviteLink.Substring(num + 1));
    }

    public static void JoinGroupWithInviteCode(string code)
    {
      if (string.IsNullOrEmpty(code) || GroupInviteLinkPage.inviteLinksPending.Contains(code))
        return;
      GroupInviteLinkPage.InviteLinkTimer inviteLinkTimer = new GroupInviteLinkPage.InviteLinkTimer(code);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupInviteLinkPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Content = (Grid) this.FindName("Content");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.InviteLink = (TextBlock) this.FindName("InviteLink");
      this.InviteLinkDescription = (TextBlock) this.FindName("InviteLinkDescription");
      this.ShareViaWhatsApp = (TextBlock) this.FindName("ShareViaWhatsApp");
      this.CopyLink = (TextBlock) this.FindName("CopyLink");
      this.RevokeLink = (TextBlock) this.FindName("RevokeLink");
      this.ShareLink = (TextBlock) this.FindName("ShareLink");
    }

    public class InviteLinkTimer
    {
      private const int timeoutTime = 5;
      private DispatcherTimer serverTimer;
      public DateTime startTime;
      public bool receivedServerResponse;
      private Action callback;
      private string code;

      public InviteLinkTimer(string code)
      {
        this.code = code;
        this.callback = this.CreateAction();
        this.StartTimer();
        App.CurrentApp.Connection.InvokeWhenConnected(this.callback);
      }

      public Action CreateAction()
      {
        FunXMPP.Connection conn = App.CurrentApp.Connection;
        return (Action) (() => conn.SendGetGroupInfoInviteLink((Action<string>) (message =>
        {
          if (this.receivedServerResponse)
            return;
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            UIUtils.MessageBox(" ", message, (IEnumerable<string>) new string[2]
            {
              AppResources.Yes,
              AppResources.No
            }, (Action<int>) (idx =>
            {
              if (idx != 0)
                return;
              if (!conn.IsConnected)
              {
                int num4 = (int) MessageBox.Show(AppResources.ConnectionStateOffline);
              }
              else
              {
                int num5;
                int num6;
                conn.SendAcceptGroupInfoInviteLink((Action<string>) (gJid => Deployment.Current.Dispatcher.BeginInvoke((Action) (() => num5 = (int) MessageBox.Show(AppResources.YouJoinedGroup)))), (Action<string>) (error => Deployment.Current.Dispatcher.BeginInvoke((Action) (() => num6 = (int) MessageBox.Show(error)))), this.code);
              }
            }));
            this.StopTimer();
          }));
        }), (Action<string>) (error => Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (GroupInviteLinkPage.inviteLinksPending.Contains(this.code))
          {
            int num = (int) MessageBox.Show(error);
          }
          this.StopTimer();
        }))), this.code));
      }

      public void StartTimer()
      {
        this.startTime = FunRunner.CurrentServerTimeUtc;
        this.receivedServerResponse = false;
        this.serverTimer = new DispatcherTimer();
        GroupInviteLinkPage.inviteLinksPending.Add(this.code);
        this.serverTimer.Tick += new EventHandler(this.TimerTick);
        this.serverTimer.Interval = new TimeSpan(0, 0, 1);
        int num = this.serverTimer.IsEnabled ? 1 : 0;
        this.serverTimer.Start();
      }

      public void StopTimer()
      {
        this.receivedServerResponse = true;
        this.serverTimer.Stop();
        GroupInviteLinkPage.inviteLinksPending.Remove(this.code);
      }

      public void TimerTick(object sender, object e)
      {
        if (this.receivedServerResponse)
        {
          this.StopTimer();
        }
        else
        {
          if (!(FunRunner.CurrentServerTimeUtc - this.startTime > TimeSpan.FromSeconds(5.0)))
            return;
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            App.CurrentApp.Connection.actionsPendingConnection.Remove(this.callback);
            int num = (int) MessageBox.Show(AppResources.InviteQueryError);
            this.StopTimer();
          }));
        }
      }
    }
  }
}
