// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupParticipantPickerPage
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsApp.RegularExpressions;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GroupParticipantPickerPage : PhoneApplicationPage
  {
    private static Conversation nextInstanceConvo = (Conversation) null;
    private static string nextInstanceTitle = (string) null;
    private static string nextInstanceSubtitle = (string) null;
    private static IObserver<List<string>> nextInstanceObserver = (IObserver<List<string>>) null;
    private static Func<UserStatus, bool> nextInstanceFitlerFunc = (Func<UserStatus, bool>) null;
    private static GroupParticipantPickerPage.CounterType nextInstanceCounterType = GroupParticipantPickerPage.CounterType.None;
    private static bool nextInstanceisMultiSelect = false;
    private static bool nextInstanceIsSearchable = false;
    private Conversation currentConvo;
    private GlobalProgressIndicator globalProgress;
    private ObservableCollection<GroupParticipantViewModel> participantViewModels;
    private IDisposable participantsChangedSub;
    private bool initedOnCreation;
    private bool initedOnLoaded;
    private IObserver<List<string>> jidObserver;
    private Func<UserStatus, bool> fitlerFunc;
    private List<IDisposable> disposables = new List<IDisposable>();
    private GroupParticipantPickerPage.CounterType counterType;
    private DateTime? lastTextChangedAt;
    private IDisposable delaySub;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock ParticipantsCountBlock;
    internal TextBox SearchBox;
    internal TextBlock SearchFieldTooltipBlock;
    internal Image DeleteIcon;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantsListBox;
    private bool _contentLoaded;

    public GroupParticipantPickerPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.jidObserver = GroupParticipantPickerPage.nextInstanceObserver;
      GroupParticipantPickerPage.nextInstanceObserver = (IObserver<List<string>>) null;
      string nextInstanceTitle = GroupParticipantPickerPage.nextInstanceTitle;
      GroupParticipantPickerPage.nextInstanceTitle = (string) null;
      string instanceSubtitle = GroupParticipantPickerPage.nextInstanceSubtitle;
      GroupParticipantPickerPage.nextInstanceSubtitle = (string) null;
      this.fitlerFunc = GroupParticipantPickerPage.nextInstanceFitlerFunc;
      GroupParticipantPickerPage.nextInstanceFitlerFunc = (Func<UserStatus, bool>) null;
      this.currentConvo = GroupParticipantPickerPage.nextInstanceConvo;
      GroupParticipantPickerPage.nextInstanceConvo = (Conversation) null;
      this.counterType = GroupParticipantPickerPage.nextInstanceCounterType;
      GroupParticipantPickerPage.nextInstanceCounterType = GroupParticipantPickerPage.CounterType.None;
      if (GroupParticipantPickerPage.nextInstanceIsSearchable)
      {
        GroupParticipantPickerPage.nextInstanceIsSearchable = false;
        this.disposables.Add(this.SearchBox.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchBox_TextChanged)));
        this.SearchBox.Visibility = Visibility.Visible;
      }
      else
        this.SearchBox.Visibility = Visibility.Collapsed;
      if (!string.IsNullOrEmpty(nextInstanceTitle))
      {
        this.TitlePanel.SmallTitle = nextInstanceTitle;
        this.TitlePanel.Visibility = Visibility.Visible;
      }
      if (!string.IsNullOrEmpty(instanceSubtitle))
      {
        this.TitlePanel.KeepOriginalSubtitleCase = true;
        this.TitlePanel.Subtitle = instanceSubtitle;
      }
      this.InitOnCreation();
    }

    public static IObservable<List<string>> Start(
      Conversation convo,
      string title,
      string subtitle = null,
      Func<UserStatus, bool> filter = null,
      bool clearStack = false,
      bool isSearchable = false,
      GroupParticipantPickerPage.CounterType counterType = GroupParticipantPickerPage.CounterType.None)
    {
      return Observable.Create<List<string>>((Func<IObserver<List<string>>, Action>) (observer =>
      {
        GroupParticipantPickerPage.nextInstanceObserver = observer;
        GroupParticipantPickerPage.nextInstanceTitle = title;
        GroupParticipantPickerPage.nextInstanceConvo = convo;
        GroupParticipantPickerPage.nextInstanceSubtitle = subtitle;
        GroupParticipantPickerPage.nextInstanceFitlerFunc = filter;
        GroupParticipantPickerPage.nextInstanceIsSearchable = isSearchable;
        GroupParticipantPickerPage.nextInstanceCounterType = counterType;
        WaUriParams uriParams = new WaUriParams();
        if (clearStack)
          uriParams.AddBool("ClearStack", clearStack);
        NavUtils.NavigateToPage(nameof (GroupParticipantPickerPage), uriParams);
        return (Action) (() => { });
      }));
    }

    private void InitOnCreation()
    {
      if (this.initedOnCreation || this.currentConvo == null)
        return;
      Conversation convo = this.currentConvo;
      this.initedOnCreation = true;
      this.globalProgress = new GlobalProgressIndicator((DependencyObject) this);
      this.ParticipantsListBox.ItemsSource = (IList) new List<string>();
      this.DeleteIcon.Source = AssetStore.KeypadCancelIcon;
      this.DeleteIcon.Visibility = Visibility.Collapsed;
      this.SearchFieldTooltipBlock.Visibility = Visibility.Visible;
      this.SearchFieldTooltipBlock.Text = AppResources.SearchCamel;
      this.SearchFieldTooltipBlock.Foreground = (Brush) UIUtils.BlackBrush;
      this.SearchFieldTooltipBlock.Opacity = ImageStore.IsDarkTheme() ? 0.3 : 0.5;
      this.ParticipantsListBox.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ParticipantsListBox_OnManipulationStarted);
      this.Loaded += (RoutedEventHandler) ((sender, e) =>
      {
        if (this.initedOnLoaded)
          return;
        this.InitOnLoaded();
      });
      this.participantsChangedSub = FunEventHandler.Events.GroupMembershipUpdatedSubject.Select<FunEventHandler.Events.ConversationWithFlags, Conversation>((Func<FunEventHandler.Events.ConversationWithFlags, Conversation>) (i => i.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == convo.Jid)).Subscribe<Conversation>((Action<Conversation>) (_ => this.UpdateParticipantsAsync()));
      this.disposables.Add(this.participantsChangedSub);
    }

    private void ParticipantsListBox_OnManipulationStarted(
      object o,
      ManipulationStartedEventArgs manipulationStartedEventArgs)
    {
      this.ParticipantsListBox.Focus();
    }

    private void InitOnLoaded()
    {
      if (this.initedOnLoaded || this.currentConvo == null)
        return;
      this.initedOnLoaded = true;
      this.UpdateParticipantsAsync();
      if (this.participantViewModels == null)
        return;
      this.ParticipantsListBox.ItemsSource = (IList) this.participantViewModels;
    }

    private void UpdateParticipantsAsync()
    {
      List<UserStatus> participants = (List<UserStatus>) null;
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        participants = ((IEnumerable<UserStatus>) this.currentConvo.GetParticipants(true, false)).Where<UserStatus>((Func<UserStatus, bool>) (u =>
        {
          if (u == null)
            return false;
          Func<UserStatus, bool> fitlerFunc = this.fitlerFunc;
          return fitlerFunc == null || fitlerFunc(u);
        })).ToList<UserStatus>();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (!this.initedOnLoaded)
            return;
          if (this.participantViewModels == null)
          {
            this.participantViewModels = new ObservableCollection<GroupParticipantViewModel>(participants.Select<UserStatus, GroupParticipantViewModel>((Func<UserStatus, GroupParticipantViewModel>) (u => new GroupParticipantViewModel(u, this.currentConvo, this.globalProgress))));
            this.ParticipantsListBox.ItemsSource = (IList) this.participantViewModels;
          }
          else
            Utils.UpdateInPlace<GroupParticipantViewModel, UserStatus>((IList<GroupParticipantViewModel>) this.participantViewModels, (IList<UserStatus>) participants, (Func<GroupParticipantViewModel, string>) (vm => vm.User.Jid), (Func<UserStatus, string>) (us => us.Jid), (Func<UserStatus, GroupParticipantViewModel>) (toAdd => new GroupParticipantViewModel(toAdd, this.currentConvo, this.globalProgress)), (Action<GroupParticipantViewModel>) (vm => vm.IsGroupAdmin = this.currentConvo.UserIsAdmin(vm.User.Jid)));
          this.UpdateParticipantCount();
        }));
      }));
    }

    private void UpdateParticipantCount()
    {
      if (this.participantViewModels == null)
        return;
      int count = this.participantViewModels.Count;
      switch (this.counterType)
      {
        case GroupParticipantPickerPage.CounterType.Admin:
          this.ParticipantsCountBlock.Visibility = Visibility.Visible;
          this.ParticipantsCountBlock.Text = Plurals.Instance.GetString(AppResources.NumAdminsPlural, count);
          break;
        case GroupParticipantPickerPage.CounterType.Participant:
          this.ParticipantsCountBlock.Visibility = Visibility.Visible;
          this.ParticipantsCountBlock.Text = Plurals.Instance.GetString(AppResources.NumParticipantsPlural, count);
          break;
        default:
          this.ParticipantsCountBlock.Visibility = Visibility.Collapsed;
          break;
      }
    }

    private void OnJidItemSelected(JidItemViewModel selItem)
    {
      if (this.jidObserver == null)
        return;
      this.jidObserver.OnNext(new List<string>()
      {
        selItem.Jid
      });
      this.jidObserver.OnCompleted();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      SystemTray.Opacity = 1.0;
      if (this.currentConvo == null)
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      base.OnNavigatedTo(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      foreach (IDisposable disposable in this.disposables)
        disposable.SafeDispose();
      this.disposables.Clear();
      base.OnRemovedFromJournal(e);
    }

    private void Participant_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is UserItemControl userItemControl))
        return;
      this.OnJidItemSelected(userItemControl.ViewModel);
    }

    private void SearchBox_TextChanged(TextChangedEventArgs e)
    {
      bool flag = string.IsNullOrEmpty(this.SearchBox.Text);
      this.SearchFieldTooltipBlock.Visibility = flag.ToVisibility();
      this.DeleteIcon.Visibility = (!flag).ToVisibility();
      string rawTerm = this.SearchBox.Text ?? "";
      this.fitlerFunc = (Func<UserStatus, bool>) (u => this.FilterByText(u, rawTerm));
      DateTime utcNow = DateTime.UtcNow;
      int num = rawTerm.Length < 3 || !this.lastTextChangedAt.HasValue ? 1 : (utcNow - this.lastTextChangedAt.Value < TimeSpan.FromMilliseconds(500.0) ? 1 : 0);
      this.lastTextChangedAt = new DateTime?(utcNow);
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      if (num != 0)
        this.delaySub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
        {
          this.delaySub.SafeDispose();
          this.delaySub = (IDisposable) null;
          this.UpdateParticipantsAsync();
        }));
      else
        this.UpdateParticipantsAsync();
    }

    private bool FilterByText(UserStatus user, string searchTerm)
    {
      if (user == null)
        return false;
      if (string.IsNullOrEmpty(searchTerm))
        return true;
      string query = (string) null;
      string ftsQuery = (string) null;
      ChatSearchPage.ProcessSearchTerm(searchTerm, out query, out ftsQuery);
      string displayName = user.GetDisplayName(getFormattedNumber: false);
      string input = user.PushName ?? "";
      Regex regex = new Regex(".*" + searchTerm + ".*", RegexOptions.IgnoreCase);
      return regex.IsMatch(displayName) || regex.IsMatch(input);
    }

    private void DeleteIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.SearchBox.Text = "";
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupParticipantPickerPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ParticipantsCountBlock = (TextBlock) this.FindName("ParticipantsCountBlock");
      this.SearchBox = (TextBox) this.FindName("SearchBox");
      this.SearchFieldTooltipBlock = (TextBlock) this.FindName("SearchFieldTooltipBlock");
      this.DeleteIcon = (Image) this.FindName("DeleteIcon");
      this.ParticipantsListBox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantsListBox");
    }

    public enum CounterType
    {
      Admin,
      Participant,
      None,
    }
  }
}
