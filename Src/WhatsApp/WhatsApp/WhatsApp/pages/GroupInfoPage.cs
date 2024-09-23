// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupInfoPage
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GroupInfoPage : PhoneApplicationPage
  {
    private static Conversation NextInstanceConvo;
    private static bool NextInstanceExpandDescription;
    private Conversation currentConvo_;
    private bool expandDescription;
    private GlobalProgressIndicator globalProgress_;
    private ObservableCollection<GroupParticipantViewModel> participantViewModels_;
    private int maxParticipantCount = Settings.MaxGroupParticipants - 1;
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private object subscriptionLock = new object();
    private IDisposable updateSubscription;
    private IDisposable imageSubscription;
    private IDisposable jidInfoChangedSub;
    private IDisposable participantsChangedSub;
    private IDisposable emailSub;
    private Microsoft.Phone.Shell.ApplicationBar appBar_;
    private bool pageRemoved;
    private bool initedOnCreation;
    private bool initedOnLoaded;
    private WaContactsListTabData participantPickerTabData;
    private ApplicationBarIconButton pinButton_;
    private RichTextBlock DescriptionTextBlock;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantsListBox;
    internal StackPanel HeaderPanel;
    internal Image GroupImage;
    internal Rectangle GroupImagePendingOverlay;
    internal ProgressBar GroupImagePendingProgressBar;
    internal StackPanel SubjectPanel;
    internal TextBlock SubjectLabel;
    internal RichTextBlock SubjectTextBlock;
    internal StackPanel DescriptionPanel;
    internal TextBlock DescriptionLabel;
    internal TextBlock AddDescriptionLabel;
    internal ChatInfoTabHeader TabHeader;
    internal TextBlock ParticipantsLabel;
    internal TextBlock ParticipantsCountBlock;
    internal Button SearchParticipants;
    internal Image SearchParticipantsIcon;
    internal Button AddParticipantsButton;
    internal TextBlock AddParticipantsButtonBlock;
    internal Button InviteLinkButton;
    internal TextBlock InviteLinkButtonBlock;
    internal TextBlock GroupCreatedTextBlock;
    internal Grid ReadOnlyPanel;
    internal TextBlock ReadOnlyBlock;
    private bool _contentLoaded;

    public GroupInfoPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.currentConvo_ = GroupInfoPage.NextInstanceConvo;
      GroupInfoPage.NextInstanceConvo = (Conversation) null;
      this.expandDescription = GroupInfoPage.NextInstanceExpandDescription;
      GroupInfoPage.NextInstanceExpandDescription = false;
      this.InitOnCreation();
    }

    public static void Start(NavigationService nav, Conversation convo, bool expandDescription = false)
    {
      GroupInfoPage.NextInstanceConvo = convo;
      GroupInfoPage.NextInstanceExpandDescription = expandDescription;
      NavUtils.NavigateToPage(nav, nameof (GroupInfoPage));
    }

    private void InitOnCreation()
    {
      if (this.initedOnCreation || this.currentConvo_ == null)
        return;
      Conversation convo = this.currentConvo_;
      this.initedOnCreation = true;
      this.globalProgress_ = new GlobalProgressIndicator((DependencyObject) this);
      this.ReadOnlyBlock.Text = AppResources.ReadOnlyGroupInfoHelpText;
      this.AddParticipantsButtonBlock.Text = AppResources.GroupAddParticipants.ToLangFriendlyLower();
      this.InviteLinkButtonBlock.Text = AppResources.InviteLinkTitle.ToLangFriendlyLower();
      this.ParticipantsLabel.Text = AppResources.GroupInfoParticipants.ToLangFriendlyLower();
      this.SubjectLabel.Text = AppResources.GroupInfoSubject.ToLangFriendlyLower();
      this.DescriptionLabel.Text = AppResources.GroupInfoDescription.ToLangFriendlyLower();
      this.AddDescriptionLabel.Text = AppResources.GroupInfoDescriptionAdd.ToLangFriendlyLower();
      StackPanel subjectPanel = this.SubjectPanel;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
        Offset = 1.0
      });
      linearGradientBrush.GradientStops = gradientStopCollection;
      linearGradientBrush.StartPoint = new System.Windows.Point(0.8, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(1.0, 0.0);
      subjectPanel.OpacityMask = (Brush) linearGradientBrush;
      this.ParticipantsListBox.ItemsSource = (IList) new List<string>();
      TextWrapping textWrapping = TextWrapping.NoWrap;
      int num = 3;
      if (this.expandDescription)
      {
        textWrapping = TextWrapping.Wrap;
        num = 0;
      }
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Margin = new Thickness(-12.0, 8.0, -12.0, 0.0);
      richTextBlock.TextWrapping = textWrapping;
      richTextBlock.TextWrapLines = num;
      richTextBlock.Expandable = !this.expandDescription;
      richTextBlock.AllowLinks = true;
      richTextBlock.FontFamily = UIUtils.FontFamilySemiLight;
      richTextBlock.FontSize = UIUtils.FontSizeSmall;
      richTextBlock.Foreground = (Brush) UIUtils.ForegroundBrush;
      this.DescriptionTextBlock = richTextBlock;
      this.DescriptionPanel.Children.Insert(2, (UIElement) this.DescriptionTextBlock);
      this.SearchParticipantsIcon.Source = (System.Windows.Media.ImageSource) AssetStore.SearchButtonIcon;
      this.Loaded += (RoutedEventHandler) ((sender, e) =>
      {
        if (this.initedOnLoaded)
          return;
        this.InitOnLoaded();
      });
      bool forceRequest = convo.IsGroupParticipant();
      this.imageSubscription = ChatPictureStore.GetState(convo.Jid, true, forceRequest, false).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (state =>
      {
        if (state.ErrorCode.HasValue)
        {
          string str;
          switch (state.ErrorCode.Value)
          {
            case 401:
              str = AppResources.GroupInfoNotAnAdmin;
              break;
            case 403:
              str = AppResources.SetGroupIconFailureNotAParticipant;
              break;
            default:
              str = AppResources.SetGroupIconFailure;
              break;
          }
          AppState.ClientInstance.ShowMessageBox(str);
        }
        this.GroupImagePendingOverlay.Visibility = this.GroupImagePendingProgressBar.Visibility = state.IsPending.ToVisibility();
        this.GroupImage.Source = (System.Windows.Media.ImageSource) (state.Image ?? AssetStore.DefaultGroupIcon);
      }));
      string jid = convo.Jid;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        this.updateSubscription = db.UpdatedConversationSubject.Where<ConvoAndMessage>((Func<ConvoAndMessage, bool>) (i => i.Conversation.Jid == convo.Jid)).ObserveOnDispatcher<ConvoAndMessage>().Subscribe<ConvoAndMessage>((Action<ConvoAndMessage>) (_ =>
        {
          if (this.initedOnLoaded)
            this.UpdateGroupInfo(false);
          this.TabHeader.UpdateGroupSettingPanel();
        }));
        this.participantsChangedSub = FunEventHandler.Events.GroupMembershipUpdatedSubject.Select<FunEventHandler.Events.ConversationWithFlags, Conversation>((Func<FunEventHandler.Events.ConversationWithFlags, Conversation>) (i => i.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == convo.Jid)).Subscribe<Conversation>((Action<Conversation>) (_ => this.UpdateParticipantsAsync()));
      }));
      this.jidInfoChangedSub = MessagesContext.Events.JidInfoUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => u.UpdatedObj is JidInfo updatedObj && jid == updatedObj.Jid)).Select<DbDataUpdate, JidInfo>((Func<DbDataUpdate, JidInfo>) (u => u.UpdatedObj as JidInfo)).ObserveOnDispatcher<JidInfo>().Subscribe<JidInfo>((Action<JidInfo>) (ji =>
      {
        if (this.initedOnLoaded)
          this.TabHeader.Set(jid);
        this.UpdateDescription(convo);
      }));
      this.UpdateParticipantsAsync();
    }

    private void InitOnLoaded()
    {
      if (this.initedOnLoaded || this.currentConvo_ == null)
        return;
      this.initedOnLoaded = true;
      Conversation currentConvo = this.currentConvo_;
      this.UpdateGroupInfo(true);
      this.UpdateGroupCreationDetails();
      this.TabHeader.Set(currentConvo.Jid);
      if (this.participantViewModels_ == null)
        return;
      this.ParticipantsListBox.ItemsSource = (IList) this.participantViewModels_;
    }

    private void UpdateGroupInfo(bool init)
    {
      Conversation currentConvo = this.currentConvo_;
      if (currentConvo == null)
        return;
      this.ReadOnlyPanel.Visibility = (!currentConvo.IsGroupParticipant()).ToVisibility();
      if (currentConvo.IsLocked() && !currentConvo.UserIsAdmin(Settings.MyJid))
        this.GroupImage.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.GroupImage_Tap);
      else
        this.GroupImage.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.GroupImage_Tap);
      this.UpdateSubject(currentConvo);
      this.UpdateDescription(currentConvo);
      this.UpdateMaxGroupParticipant(currentConvo);
      if (!init)
        this.ResetAppBar();
      if (currentConvo.UserIsAdmin(Settings.MyJid) && currentConvo.IsGroupParticipant())
      {
        this.AddParticipantsButton.Visibility = Visibility.Visible;
        this.InviteLinkButton.Visibility = Visibility.Visible;
      }
      else
      {
        this.AddParticipantsButton.Visibility = Visibility.Collapsed;
        this.InviteLinkButton.Visibility = Visibility.Collapsed;
      }
      this.UpdateParticipantsAsync();
    }

    private void UpdateSubject(Conversation convo)
    {
      this.TitlePanel.SmallTitle = AppResources.GroupInfo;
      string name = convo.GetName();
      this.SubjectTextBlock.Text = new RichTextBlock.TextSet()
      {
        Text = name
      };
      if (convo.IsLocked() && !convo.UserIsAdmin(Settings.MyJid))
        this.SubjectPanel.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.Subject_Tap);
      else
        this.SubjectPanel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Subject_Tap);
    }

    private void UpdateDescription(Conversation convo)
    {
      if (Settings.GroupDescriptionLength <= 0)
      {
        this.DescriptionPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        string groupDescription = convo.GroupDescription;
        bool flag = convo.IsLocked() && !convo.UserIsAdmin(Settings.MyJid);
        if (flag)
          this.DescriptionPanel.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.Description_Tap);
        else
          this.DescriptionPanel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Description_Tap);
        if (string.IsNullOrEmpty(groupDescription))
        {
          if (convo.IsGroupParticipant() && !flag)
          {
            this.AddDescriptionLabel.Visibility = Visibility.Visible;
            this.DescriptionTextBlock.Visibility = Visibility.Collapsed;
            this.DescriptionPanel.Visibility = Visibility.Visible;
          }
          else
            this.DescriptionPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
          bool allowLinks = true;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => allowLinks = SuspiciousJid.ShouldAllowLinksForJid(db, this.currentConvo_.Jid)));
          this.DescriptionTextBlock.AllowLinks = allowLinks;
          this.DescriptionTextBlock.Text = new RichTextBlock.TextSet()
          {
            Text = groupDescription
          };
          this.AddDescriptionLabel.Visibility = Visibility.Collapsed;
          this.DescriptionTextBlock.Visibility = Visibility.Visible;
          this.DescriptionPanel.Visibility = Visibility.Visible;
        }
      }
    }

    private void UpdateMaxGroupParticipant(Conversation convo)
    {
      this.maxParticipantCount = Settings.MaxGroupParticipants - 1;
      this.UpdateParticipantCount();
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
      if (this.currentConvo_.UserIsAdmin(Settings.MyJid) && this.currentConvo_.IsGroupParticipant())
      {
        ApplicationBarIconButton applicationBarIconButton4 = new ApplicationBarIconButton()
        {
          IconUri = new Uri("/Images/icon-add-recipient.png", UriKind.Relative),
          Text = "Add"
        };
        applicationBarIconButton4.Click += new EventHandler(this.AddParticipants_Click);
        bar.Buttons.Add((object) applicationBarIconButton4);
      }
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
      bool flag = this.currentConvo_ != null && !this.currentConvo_.IsGroupParticipant();
      ApplicationBarMenuItem applicationBarMenuItem3 = new ApplicationBarMenuItem()
      {
        Text = flag ? "Delete" : "LeaveGroup"
      };
      applicationBarMenuItem3.Click += new EventHandler(this.LeaveGroupButton_Click);
      bar.MenuItems.Add((object) applicationBarMenuItem3);
      if (this.currentConvo_?.GroupOwner != Settings.MyJid)
      {
        ApplicationBarMenuItem applicationBarMenuItem4 = new ApplicationBarMenuItem()
        {
          Text = "ReportSpamButton"
        };
        applicationBarMenuItem4.Click += new EventHandler(this.ReportSpamButton_Click);
        bar.MenuItems.Add((object) applicationBarMenuItem4);
      }
      Localizable.LocalizeAppBar(bar);
      this.ApplicationBar = (IApplicationBar) bar;
    }

    private void UpdatePinButton()
    {
      if (this.pinButton_ == null)
        return;
      this.pinButton_.IsEnabled = !TileHelper.ChatTileExists(this.currentConvo_.Jid);
    }

    private void AddParticipants(List<UserStatus> users)
    {
      if (users == null || users.Count == 0)
        return;
      this.globalProgress_.Acquire();
      IDisposable addMembersSub = (IDisposable) null;
      Action release = (Action) (() =>
      {
        release = (Action) (() => { });
        this.globalProgress_.Release();
        if (addMembersSub == null)
          return;
        lock (this.subscriptionLock)
        {
          this.subscriptions.Remove(addMembersSub);
          addMembersSub.Dispose();
        }
      });
      addMembersSub = GroupCreationPage.AddMembersAsync(this.currentConvo_.Jid, users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid))).ObserveOnDispatcher<Dictionary<string, int>>().Subscribe<Dictionary<string, int>>((Action<Dictionary<string, int>>) (failures =>
      {
        foreach (UserStatus user in users)
        {
          if (failures.ContainsKey(user.Jid))
          {
            Log.l(nameof (GroupInfoPage), "Error adding user: {0}, {1}", (object) user.Jid, (object) failures[user.Jid]);
            this.AddUserToParticipantsList(user, failures[user.Jid]);
          }
          else
            this.AddUserToParticipantsList(user);
        }
      }), (Action<Exception>) (err => release()), (Action) (() => release()));
      lock (this.subscriptionLock)
        this.subscriptions.Add(addMembersSub);
    }

    private void UpdateGroupCreationDetails()
    {
      string str1 = (string) null;
      string str2 = (string) null;
      if (!string.IsNullOrEmpty(this.currentConvo_.GroupOwner))
      {
        string groupOwner = this.currentConvo_.GroupOwner;
        str1 = !(groupOwner == Settings.MyJid) ? string.Format(AppResources.GroupCreatedByUser, (object) JidHelper.GetDisplayNameForContactJid(groupOwner)) : AppResources.GroupCreatedByYou;
      }
      DateTime? groupCreationT = this.currentConvo_.GroupCreationT;
      if (groupCreationT.HasValue)
        str2 = string.Format(AppResources.TimeCreated, (object) DateTimeUtils.FormatLastSeen(groupCreationT.Value.ToLocalTime()));
      this.GroupCreatedTextBlock.Text = string.Join("\n", ((IEnumerable<string>) new string[2]
      {
        str1,
        str2
      }).Where<string>((Func<string, bool>) (s => s != null)));
    }

    private void UpdateParticipantsAsync()
    {
      List<UserStatus> participants = (List<UserStatus>) null;
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        participants = ((IEnumerable<UserStatus>) this.currentConvo_.GetParticipants(false, false)).Where<UserStatus>((Func<UserStatus, bool>) (u => u != null)).ToList<UserStatus>();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (!this.initedOnLoaded)
            return;
          if (this.participantViewModels_ == null)
          {
            this.participantViewModels_ = new ObservableCollection<GroupParticipantViewModel>(participants.Select<UserStatus, GroupParticipantViewModel>((Func<UserStatus, GroupParticipantViewModel>) (u => new GroupParticipantViewModel(u, this.currentConvo_, this.globalProgress_))));
            this.ParticipantsListBox.ItemsSource = (IList) this.participantViewModels_;
          }
          else
            Utils.UpdateInPlace<GroupParticipantViewModel, UserStatus>((IList<GroupParticipantViewModel>) this.participantViewModels_, (IList<UserStatus>) participants, (Func<GroupParticipantViewModel, string>) (vm => vm.User.Jid), (Func<UserStatus, string>) (us => us.Jid), (Func<UserStatus, GroupParticipantViewModel>) (toAdd => new GroupParticipantViewModel(toAdd, this.currentConvo_, this.globalProgress_)), (Action<GroupParticipantViewModel>) (vm => vm.IsGroupAdmin = this.currentConvo_.UserIsAdmin(vm.User.Jid)));
          this.UpdateParticipantCount();
        }));
      }));
    }

    private void UpdateParticipantCount()
    {
      if (this.participantViewModels_ == null)
        return;
      int num = this.participantViewModels_.Count;
      int participantCount = this.maxParticipantCount;
      if (num > participantCount)
        num = participantCount;
      this.ParticipantsCountBlock.Text = string.Format(AppResources.NthOutOfTotal, (object) num, (object) participantCount);
    }

    private void AddUserToParticipantsList(UserStatus user, int errorCode = 0)
    {
      GroupParticipantViewModel newItem = this.participantViewModels_.FirstOrDefault<GroupParticipantViewModel>((Func<GroupParticipantViewModel, bool>) (vm => vm.User.Jid == user?.Jid && user != null));
      if (user != null && newItem == null)
      {
        newItem = new GroupParticipantViewModel(user, this.currentConvo_, this.globalProgress_);
        this.participantViewModels_.InsertInOrder<GroupParticipantViewModel>(newItem, (Func<GroupParticipantViewModel, GroupParticipantViewModel, bool>) ((a, b) => ParticipantSort.CompareParticipants(a.User, b.User, false, true) < 0));
      }
      if (newItem == null)
        return;
      newItem.Error = errorCode;
    }

    private void RemoveUserFromParticipantsList(string jid)
    {
      int count = this.participantViewModels_.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.participantViewModels_.ElementAt<GroupParticipantViewModel>(index).User.Jid == jid)
        {
          this.participantViewModels_.RemoveAt(index);
          break;
        }
      }
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.emailSub.SafeDispose();
      this.emailSub = (IDisposable) null;
      if (e != null && e.NavigationMode == NavigationMode.Back)
        ProfilePictureChooserPage.ClearPopup();
      if (this.DescriptionTextBlock != null)
        this.DescriptionTextBlock.TextWrapping = TextWrapping.NoWrap;
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
      this.imageSubscription.SafeDispose();
      this.imageSubscription = (IDisposable) null;
      this.updateSubscription.SafeDispose();
      this.updateSubscription = (IDisposable) null;
      this.participantsChangedSub.SafeDispose();
      this.participantsChangedSub = (IDisposable) null;
      lock (this.subscriptionLock)
      {
        this.subscriptions.ForEach((Action<IDisposable>) (d => d.Dispose()));
        this.subscriptions.Clear();
      }
      base.OnRemovedFromJournal(e);
    }

    private void AddParticipants_Click(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      if (this.currentConvo_.GetParticipantCount() >= Settings.MaxGroupParticipants - 1)
      {
        int num = (int) MessageBox.Show(Plurals.Instance.GetString(AppResources.GroupLimitExceededPlural, Settings.MaxGroupParticipants - 1));
      }
      else
      {
        string[] currentParticipantJids = this.currentConvo_.GetParticipantJids(true);
        Func<IEnumerable<string>, IObservable<bool>> confirmSelectionFunc = (Func<IEnumerable<string>, IObservable<bool>>) (selJids =>
        {
          List<UserStatus> source1 = new List<UserStatus>();
          foreach (string selJid in selJids)
          {
            if (!((IEnumerable<string>) currentParticipantJids).Contains<string>(selJid))
              source1.Add(UserCache.Get(selJid, false));
          }
          if (source1.Count == 0)
            return Observable.Return<bool>(false);
          if (source1.Count == 1)
          {
            string displayName = source1.First<UserStatus>().GetDisplayName();
            return UIUtils.Decision(string.Format(AppResources.ConfirmAddParticipantBody, (object) displayName), title: string.Format(AppResources.ConfirmAddParticipantTitle, (object) displayName));
          }
          IEnumerable<string> source2 = source1.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName()));
          int count = source1.Count - 1;
          string str1 = string.Join(", ", source2.Take<string>(count));
          string str2 = source2.Last<string>();
          return UIUtils.Decision(Plurals.Instance.GetStringWithIndex(AppResources.ConfirmAddParticipantsBodyPlural, 2, (object) str1, (object) str2, (object) count), title: Plurals.Instance.GetStringWithIndex(AppResources.ConfirmAddParticipantsTitlePlural, 2, (object) str1, (object) str2, (object) count));
        });
        RecipientListPage.StartContactPicker(AppResources.GroupAddParticipants, (IEnumerable<string>) currentParticipantJids, false, (Brush) UIUtils.SelectionBrush, readOnlySelected: (IEnumerable<string>) currentParticipantJids, confirmSelectionFunc: confirmSelectionFunc, subtitleOverride: AppResources.AlreadyInGroup).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Select<RecipientListPage.RecipientListResults, List<UserStatus>>((Func<RecipientListPage.RecipientListResults, List<UserStatus>>) (recipientListResults =>
        {
          List<string> stringList = recipientListResults?.SelectedJids;
          List<UserStatus> userStatusList = new List<UserStatus>();
          if (stringList == null)
            stringList = new List<string>();
          foreach (string jid in stringList)
          {
            if (!((IEnumerable<string>) currentParticipantJids).Contains<string>(jid))
              userStatusList.Add(UserCache.Get(jid, false));
          }
          return userStatusList;
        })).Subscribe<List<UserStatus>>(new Action<List<UserStatus>>(this.AddParticipants));
      }
    }

    private void InviteLinkButton_Click(object sender, RoutedEventArgs routedEventArgs)
    {
      GroupInviteLinkPage.Start(this.currentConvo_.Jid).ObserveOnDispatcher<string>().Subscribe<string>();
    }

    private void Subject_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      if (!this.currentConvo_.IsGroupParticipant())
      {
        int num1 = (int) MessageBox.Show(AppResources.ReadOnlyGroupMetadataHelpText);
      }
      else if (this.currentConvo_.IsLocked() && !this.currentConvo_.UserIsAdmin(Settings.MyJid))
      {
        int num2 = (int) MessageBox.Show(AppResources.GroupInfoNotAnAdmin);
      }
      else
      {
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddString("jid", this.currentConvo_.Jid);
        NavUtils.NavigateToPage(this.NavigationService, "ChangeSubjectPage", uriParams);
      }
    }

    private void Description_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      if (!this.currentConvo_.IsGroupParticipant())
      {
        int num1 = (int) MessageBox.Show(AppResources.ReadOnlyGroupMetadataHelpText);
      }
      else if (this.currentConvo_.IsLocked() && !this.currentConvo_.UserIsAdmin(Settings.MyJid))
      {
        int num2 = (int) MessageBox.Show(AppResources.GroupInfoNotAnAdmin);
      }
      else
      {
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddString("jid", this.currentConvo_.Jid);
        NavUtils.NavigateToPage(this.NavigationService, "ChangeDescriptionPage", uriParams);
      }
    }

    private void GroupImage_Tap(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      if (!this.currentConvo_.IsGroupParticipant())
      {
        int num1 = (int) MessageBox.Show(AppResources.ReadOnlyGroupMetadataHelpText);
      }
      else if (this.currentConvo_.IsLocked() && !this.currentConvo_.UserIsAdmin(Settings.MyJid))
      {
        int num2 = (int) MessageBox.Show(AppResources.GroupInfoNotAnAdmin);
      }
      else
      {
        if (this.GroupImage.Source != null)
          ProfilePictureChooserPage.PlayEntranceAnimation(this.GroupImage, this.HeaderPanel.TransformToVisual(Application.Current.RootVisual).Transform(new System.Windows.Point(0.0, 0.0)), this.ParticipantsListBox.TransformToVisual(Application.Current.RootVisual).Transform(new System.Windows.Point(0.0, 0.0)), this.Orientation, true);
        SystemTray.Opacity = 0.0;
        IDisposable d = (IDisposable) null;
        d = ProfilePictureChooserPage.Start(this.currentConvo_.Jid, (string) null, this.GroupImage.Source).Subscribe<ProfilePictureChooserPage.ProfilePictureChooserArgs>((Action<ProfilePictureChooserPage.ProfilePictureChooserArgs>) (args => d.SafeDispose()));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.ParticipantsListBox.ListHeader == null)
            return;
          this.ParticipantsListBox.ScrollTo(this.ParticipantsListBox.ListHeader);
        }));
      }
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

    private void ReportSpamButton_Click(object sender, EventArgs e)
    {
      Message[] msgsToReport = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => msgsToReport = db.GetLatestMessages(this.currentConvo_.Jid, this.currentConvo_.MessageLoadingStart(), new int?(5), new int?(0))));
      bool flag = this.currentConvo_.IsGroupParticipant();
      Observable.Return<bool>(true).Decision(flag ? AppResources.ReportSpamAndLeaveConfirmBodyNoHistory : AppResources.ReportSpamConfirmBodyNoHistory, flag ? AppResources.ReportAndLeaveButton : AppResources.ReportSpamButton, AppResources.CancelButton).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        DeleteChat.DeleteSpam(this.currentConvo_.Jid);
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateHome()));
        App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
        {
          try
          {
            App.CurrentApp.Connection.SendSpamReport(msgsToReport, "group_info", this.currentConvo_.Jid, this.currentConvo_.GroupOwner, this.currentConvo_.GroupSubject);
          }
          catch (Exception ex)
          {
            string context = string.Format("Sending spam report for {0}", (object) this.currentConvo_.Jid);
            Log.LogException(ex, context);
          }
        }));
      }));
    }

    private void LeaveGroupButton_Click(object sender, EventArgs e)
    {
      if (this.currentConvo_ == null)
        return;
      DeleteChat.PromptForDelete(Observable.Return<bool>(true), this.currentConvo_).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        int num = this.currentConvo_ == null ? 0 : (!this.currentConvo_.IsGroupParticipant() ? 1 : 0);
        DeleteChat.Delete(this.currentConvo_.Jid);
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_DELETE);
        if (num == 0)
          return;
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

    private void SearchParticipants_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      GroupParticipantPickerPage.Start(this.currentConvo_, "Search Participants", isSearchable: true, counterType: GroupParticipantPickerPage.CounterType.Participant).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (_ => { }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupInfoPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ParticipantsListBox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantsListBox");
      this.HeaderPanel = (StackPanel) this.FindName("HeaderPanel");
      this.GroupImage = (Image) this.FindName("GroupImage");
      this.GroupImagePendingOverlay = (Rectangle) this.FindName("GroupImagePendingOverlay");
      this.GroupImagePendingProgressBar = (ProgressBar) this.FindName("GroupImagePendingProgressBar");
      this.SubjectPanel = (StackPanel) this.FindName("SubjectPanel");
      this.SubjectLabel = (TextBlock) this.FindName("SubjectLabel");
      this.SubjectTextBlock = (RichTextBlock) this.FindName("SubjectTextBlock");
      this.DescriptionPanel = (StackPanel) this.FindName("DescriptionPanel");
      this.DescriptionLabel = (TextBlock) this.FindName("DescriptionLabel");
      this.AddDescriptionLabel = (TextBlock) this.FindName("AddDescriptionLabel");
      this.TabHeader = (ChatInfoTabHeader) this.FindName("TabHeader");
      this.ParticipantsLabel = (TextBlock) this.FindName("ParticipantsLabel");
      this.ParticipantsCountBlock = (TextBlock) this.FindName("ParticipantsCountBlock");
      this.SearchParticipants = (Button) this.FindName("SearchParticipants");
      this.SearchParticipantsIcon = (Image) this.FindName("SearchParticipantsIcon");
      this.AddParticipantsButton = (Button) this.FindName("AddParticipantsButton");
      this.AddParticipantsButtonBlock = (TextBlock) this.FindName("AddParticipantsButtonBlock");
      this.InviteLinkButton = (Button) this.FindName("InviteLinkButton");
      this.InviteLinkButtonBlock = (TextBlock) this.FindName("InviteLinkButtonBlock");
      this.GroupCreatedTextBlock = (TextBlock) this.FindName("GroupCreatedTextBlock");
      this.ReadOnlyPanel = (Grid) this.FindName("ReadOnlyPanel");
      this.ReadOnlyBlock = (TextBlock) this.FindName("ReadOnlyBlock");
    }
  }
}
