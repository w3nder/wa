// Decompiled with JetBrains decompiler
// Type: WhatsApp.PsaInfoPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class PsaInfoPage : PhoneApplicationPage
  {
    private static Conversation NextInstanceConvo;
    private Conversation currentConvo_;
    private GlobalProgressIndicator globalProgress_;
    private ObservableCollection<GroupParticipantViewModel> participantViewModels_;
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private object subscriptionLock = new object();
    private IDisposable updateSubscription;
    private IDisposable jidInfoChangedSub;
    private IDisposable emailSub;
    private Microsoft.Phone.Shell.ApplicationBar appBar_;
    private bool pageRemoved;
    private bool initedOnCreation;
    private bool initedOnLoaded;
    private ApplicationBarIconButton pinButton_;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantsListBox;
    internal StackPanel HeaderPanel;
    internal Image PsaImage;
    internal StackPanel SubjectPanel;
    internal TextBlock SubjectLabel;
    internal Image VerifiedBadge;
    internal RichTextBlock SubjectTextBlock;
    internal ChatInfoTabHeader TabHeader;
    private bool _contentLoaded;

    public PsaInfoPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.currentConvo_ = PsaInfoPage.NextInstanceConvo;
      PsaInfoPage.NextInstanceConvo = (Conversation) null;
      this.InitOnCreation();
    }

    public static void Start(NavigationService nav, Conversation convo)
    {
      PsaInfoPage.NextInstanceConvo = convo;
      NavUtils.NavigateToPage(nav, nameof (PsaInfoPage));
    }

    private void InitOnCreation()
    {
      if (this.initedOnCreation || this.currentConvo_ == null)
        return;
      Conversation convo = this.currentConvo_;
      this.initedOnCreation = true;
      this.globalProgress_ = new GlobalProgressIndicator((DependencyObject) this);
      this.SubjectLabel.Text = convo.GetName();
      this.ParticipantsListBox.ItemsSource = (IList) new List<string>();
      this.Loaded += (RoutedEventHandler) ((sender, e) =>
      {
        if (this.initedOnLoaded)
          return;
        this.InitOnLoaded();
      });
      this.VerifiedBadge.Source = (System.Windows.Media.ImageSource) AssetStore.InlineVerified;
      this.VerifiedBadge.Width = this.VerifiedBadge.Height = (double) Application.Current.Resources[(object) "PhoneFontSizeLarge"];
      this.VerifiedBadge.VerticalAlignment = VerticalAlignment.Center;
      this.PsaImage.Source = (System.Windows.Media.ImageSource) AssetStore.WhatsAppAvatar;
      string jid = convo.Jid;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.updateSubscription = db.UpdatedConversationSubject.Where<ConvoAndMessage>((Func<ConvoAndMessage, bool>) (i => i.Conversation.Jid == convo.Jid)).ObserveOnDispatcher<ConvoAndMessage>().Subscribe<ConvoAndMessage>((Action<ConvoAndMessage>) (_ =>
      {
        if (!this.initedOnLoaded)
          return;
        this.UpdatePsaInfo(false);
      }))));
      this.jidInfoChangedSub = MessagesContext.Events.JidInfoUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => u.UpdatedObj is JidInfo updatedObj && jid == updatedObj.Jid)).ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (_ =>
      {
        if (!this.initedOnLoaded)
          return;
        this.TabHeader.Set(jid);
      }));
    }

    private void InitOnLoaded()
    {
      if (this.initedOnLoaded || this.currentConvo_ == null)
        return;
      this.initedOnLoaded = true;
      Conversation currentConvo = this.currentConvo_;
      this.UpdatePsaInfo(true);
      this.TabHeader.Set(currentConvo.Jid);
      if (this.participantViewModels_ == null)
        return;
      this.ParticipantsListBox.ItemsSource = (IList) this.participantViewModels_;
    }

    private void UpdatePsaInfo(bool init)
    {
      if (this.currentConvo_ == null)
        return;
      this.TitlePanel.SmallTitle = AppResources.AnnouncementInfo;
      this.SubjectTextBlock.Text = new RichTextBlock.TextSet()
      {
        Text = AppResources.OfficialAnnouncementsDescription
      };
      if (init)
        return;
      this.ResetAppBar();
    }

    private void ResetAppBar()
    {
      if (this.currentConvo_ == null)
        return;
      Microsoft.Phone.Shell.ApplicationBar bar = this.appBar_ ?? (this.appBar_ = new Microsoft.Phone.Shell.ApplicationBar());
      bar.Buttons.Clear();
      ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton();
      applicationBarIconButton1.IconUri = new Uri("/Images/pin-to-start.png", UriKind.Relative);
      applicationBarIconButton1.Text = "CreateConversationTileShort";
      applicationBarIconButton1.IsEnabled = !TileHelper.ChatTileExists(this.currentConvo_.Jid);
      ApplicationBarIconButton applicationBarIconButton2 = applicationBarIconButton1;
      this.pinButton_ = applicationBarIconButton1;
      ApplicationBarIconButton applicationBarIconButton3 = applicationBarIconButton2;
      applicationBarIconButton3.Click += new EventHandler(this.CreateConversationTileButton_Click);
      bar.Buttons.Add((object) applicationBarIconButton3);
      bar.MenuItems.Clear();
      bool hasValue = this.currentConvo_.LastMessageID.HasValue;
      ApplicationBarMenuItem applicationBarMenuItem1 = new ApplicationBarMenuItem()
      {
        Text = "EmailChatHistory",
        IsEnabled = hasValue
      };
      applicationBarMenuItem1.Click += new EventHandler(this.EmailChatHistoryButton_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem1);
      ApplicationBarMenuItem applicationBarMenuItem2 = new ApplicationBarMenuItem()
      {
        Text = "ClearChatHistory",
        IsEnabled = hasValue
      };
      applicationBarMenuItem2.Click += new EventHandler(this.ClearChatHistoryButton_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem2);
      ApplicationBarMenuItem applicationBarMenuItem3 = new ApplicationBarMenuItem()
      {
        Text = "Delete"
      };
      applicationBarMenuItem3.Click += new EventHandler(this.DeleteChatButton_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem3);
      Localizable.LocalizeAppBar(bar);
      this.ApplicationBar = (IApplicationBar) bar;
    }

    private void UpdatePinButton()
    {
      if (this.pinButton_ == null)
        return;
      this.pinButton_.IsEnabled = !TileHelper.ChatTileExists(this.currentConvo_.Jid);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.emailSub.SafeDispose();
      this.emailSub = (IDisposable) null;
      if (e.NavigationMode == NavigationMode.Back)
        ProfilePictureChooserPage.ClearPopup();
      base.OnNavigatingFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      SystemTray.Opacity = 1.0;
      if (this.currentConvo_ == null)
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      else
        this.ResetAppBar();
      base.OnNavigatedTo(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      this.jidInfoChangedSub.SafeDispose();
      this.jidInfoChangedSub = (IDisposable) null;
      this.updateSubscription.SafeDispose();
      this.updateSubscription = (IDisposable) null;
      lock (this.subscriptionLock)
      {
        this.subscriptions.ForEach((Action<IDisposable>) (d => d.Dispose()));
        this.subscriptions.Clear();
      }
      base.OnRemovedFromJournal(e);
    }

    private void EmailChatHistoryButton_Click(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null && this.emailSub != null)
        return;
      this.globalProgress_.Acquire();
      this.emailSub = EmailChatHistory.Create(this.currentConvo_.Jid).SubscribeOn<Unit>(WAThreadPool.Scheduler).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (unit => { }), (Action) (() =>
      {
        this.globalProgress_.Release();
        this.emailSub.SafeDispose();
        this.emailSub = (IDisposable) null;
      }));
    }

    private void ClearChatHistoryButton_Click(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      ClearChatPicker.Launch(this.currentConvo_.Jid).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
    }

    private void DeleteChatButton_Click(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      DeleteChat.PromptForDelete(Observable.Return<bool>(true), this.currentConvo_).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        DeleteChat.Delete(this.currentConvo_.Jid);
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_DELETE);
        this.NavigationService.JumpBackTo("ContactsPage");
      }));
    }

    private void CreateConversationTileButton_Click(object sender, EventArgs e)
    {
      if (!PinToStart.Pin(this.currentConvo_))
        return;
      this.UpdatePinButton();
    }

    private void Participant_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/PsaInfoPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ParticipantsListBox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantsListBox");
      this.HeaderPanel = (StackPanel) this.FindName("HeaderPanel");
      this.PsaImage = (Image) this.FindName("PsaImage");
      this.SubjectPanel = (StackPanel) this.FindName("SubjectPanel");
      this.SubjectLabel = (TextBlock) this.FindName("SubjectLabel");
      this.VerifiedBadge = (Image) this.FindName("VerifiedBadge");
      this.SubjectTextBlock = (RichTextBlock) this.FindName("SubjectTextBlock");
      this.TabHeader = (ChatInfoTabHeader) this.FindName("TabHeader");
    }
  }
}
