// Decompiled with JetBrains decompiler
// Type: WhatsApp.BroadcastListInfoPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class BroadcastListInfoPage : PhoneApplicationPage
  {
    private static string NextInstanceJid;
    private BroadcastInfoPageViewModel viewModel;
    private Conversation broadcastList;
    private GlobalProgressIndicator progressIndicator;
    private Microsoft.Phone.Shell.ApplicationBar infoTabAppBar;
    private Microsoft.Phone.Shell.ApplicationBar currAppBar;
    internal PivotHeaderConverter PivotHeaderConverter;
    internal Grid LayoutRoot;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantList;
    internal ListBoxItem EncryptionPanel;
    internal Image EncryptionIcon;
    internal TextBlock EncryptionTitleBlock;
    internal TextBlock EncryptionStateBlock;
    private bool _contentLoaded;

    public BroadcastListInfoPage()
    {
      this.InitializeComponent();
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      this.infoTabAppBar = this.Resources[(object) "InfoTabAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(this.infoTabAppBar);
      this.ApplicationBar = (IApplicationBar) (this.currAppBar = this.infoTabAppBar);
      if (BroadcastListInfoPage.NextInstanceJid == null)
        return;
      string nextInstanceJid = BroadcastListInfoPage.NextInstanceJid;
      BroadcastListInfoPage.NextInstanceJid = (string) null;
      this.Init(nextInstanceJid);
    }

    public static void Start(NavigationService nav, string jid)
    {
      BroadcastListInfoPage.NextInstanceJid = jid;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString(nameof (jid), jid);
      NavUtils.NavigateToPage(nav, nameof (BroadcastListInfoPage), uriParams);
    }

    private void Init(string jid)
    {
      this.broadcastList = (Conversation) null;
      if (JidHelper.IsBroadcastJid(jid))
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.broadcastList = db.GetConversation(jid, CreateOptions.None)));
      if (this.broadcastList == null)
        return;
      this.DataContext = (object) (this.viewModel = new BroadcastInfoPageViewModel(this.broadcastList, this.Orientation));
      this.viewModel.ParticipantAdded += (EventHandler<MultiParticipantsChatViewModelBase.ParticipantAddedEventArgs>) ((sender, args) =>
      {
        if (args == null || args.AddedParticipantViewModel == null)
          return;
        this.ParticipantList.ScrollTo((object) args.AddedParticipantViewModel);
      });
    }

    private void UpdatePinButton()
    {
      if (this.broadcastList == null)
        return;
      IList buttons = this.infoTabAppBar.Buttons;
      for (int index = 0; index < buttons.Count; ++index)
      {
        if (buttons[index] is ApplicationBarIconButton applicationBarIconButton && applicationBarIconButton.Text == AppResources.CreateConversationTileShort)
        {
          applicationBarIconButton.IsEnabled = !TileHelper.ChatTileExists(this.broadcastList.Jid);
          break;
        }
      }
    }

    private void UpdateEncryptionPanel()
    {
      if (this.broadcastList == null)
        return;
      this.EncryptionTitleBlock.Text = AppResources.Encryption;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      if (this.broadcastList == null || this.viewModel == null)
      {
        string jid = (string) null;
        if (this.NavigationContext.QueryString.TryGetValue("jid", out jid) && !string.IsNullOrEmpty(jid))
        {
          this.Init(jid);
          flag = this.broadcastList == null || this.viewModel == null;
        }
      }
      base.OnNavigatedTo(e);
      if (flag)
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.NavigationService.GoBack()));
      }
      else
      {
        this.UpdatePinButton();
        this.UpdateEncryptionPanel();
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.viewModel.SafeDispose();
      base.OnRemovedFromJournal(e);
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
      if (this.broadcastList != null)
      {
        Conversation convo = this.broadcastList;
        DeleteChat.PromptForDelete(Observable.Return<bool>(true), convo).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
        {
          if (accept)
          {
            DeleteChat.Delete(convo.Jid);
            FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_DELETE);
          }
          this.NavigationService.JumpBackTo("BroadcastListMainPage", fallbackToHome: true);
        }));
      }
      else
        this.Dispatcher.BeginInvoke((Action) (() => this.NavigationService.JumpBackTo("BroadcastListMainPage", fallbackToHome: true)));
    }

    private void PinButton_Click(object sender, EventArgs e)
    {
      if (!PinToStart.Pin(this.broadcastList))
        return;
      this.UpdatePinButton();
    }

    private void ClearChatHistoryButton_Click(object sender, EventArgs e)
    {
      if (this.broadcastList == null)
        return;
      ClearChatPicker.Launch(this.broadcastList.Jid).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.broadcastList == null)
        return;
      NavUtils.NavigateToPage(this.NavigationService, "ChangeSubjectPage", "jid=" + this.broadcastList.Jid);
    }

    private void AddRecipient_Click(object sender, RoutedEventArgs e)
    {
      if (this.viewModel == null)
        return;
      this.viewModel.LaunchParticipantPicker();
    }

    private void EncryptionPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      UIUtils.MessageBox(" ", AppResources.EncryptedBroadcastDetails, (IEnumerable<string>) new string[2]
      {
        AppResources.OkLower,
        AppResources.LearnMoreButton
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        new WebBrowserTask()
        {
          Uri = new Uri(WaWebUrls.FaqUrlGroupE2e)
        }.Show();
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/BroadcastListInfoPage.xaml", UriKind.Relative));
      this.PivotHeaderConverter = (PivotHeaderConverter) this.FindName("PivotHeaderConverter");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ParticipantList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantList");
      this.EncryptionPanel = (ListBoxItem) this.FindName("EncryptionPanel");
      this.EncryptionIcon = (Image) this.FindName("EncryptionIcon");
      this.EncryptionTitleBlock = (TextBlock) this.FindName("EncryptionTitleBlock");
      this.EncryptionStateBlock = (TextBlock) this.FindName("EncryptionStateBlock");
    }
  }
}
