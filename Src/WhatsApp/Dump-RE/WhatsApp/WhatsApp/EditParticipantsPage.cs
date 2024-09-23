// Decompiled with JetBrains decompiler
// Type: WhatsApp.EditParticipantsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class EditParticipantsPage : PhoneApplicationPage
  {
    private static Conversation nextInstanceConvo_;
    private static IObserver<PageArgs> nextInstanceObserver_;
    private EditParticipantsPageViewModel viewModel_;
    private Conversation convo_;
    private IObserver<PageArgs> observer_;
    private bool isConvoInDb_;
    private IDisposable participantChangeSub_;
    private List<JidItemViewModel> searchSource_;
    private bool pageRemoved_;
    private ApplicationBarIconButton appBarSubmitButton_;
    internal Grid LayoutRoot;
    internal ItemQuickSearchControl QuickSearch;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantList;
    internal TextBlock WarningTextBlock;
    private bool _contentLoaded;

    private ApplicationBarIconButton AppBarSubmitButton
    {
      get
      {
        if (this.appBarSubmitButton_ == null)
          this.appBarSubmitButton_ = (ApplicationBarIconButton) this.ApplicationBar.Buttons[0];
        return this.appBarSubmitButton_;
      }
    }

    public EditParticipantsPage()
    {
      this.InitializeComponent();
      this.convo_ = EditParticipantsPage.nextInstanceConvo_;
      this.observer_ = EditParticipantsPage.nextInstanceObserver_;
      EditParticipantsPage.nextInstanceConvo_ = (Conversation) null;
      EditParticipantsPage.nextInstanceObserver_ = (IObserver<PageArgs>) null;
      if (this.convo_ == null || this.observer_ == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.isConvoInDb_ = db.GetConversation(this.convo_.Jid, CreateOptions.None) != null));
      Microsoft.Phone.Shell.ApplicationBar resource = this.Resources[this.isConvoInDb_ ? (object) "AppBarForExistingChat" : (object) "AppBarForNewChat"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(resource);
      this.ApplicationBar = (IApplicationBar) resource;
      this.InitQuickSearch();
      this.ParticipantList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ParticipantList_ScrollingStarted);
    }

    public static IObservable<PageArgs> Start(Conversation convo)
    {
      return Observable.Create<PageArgs>((Func<IObserver<PageArgs>, Action>) (observer =>
      {
        EditParticipantsPage.nextInstanceConvo_ = convo;
        EditParticipantsPage.nextInstanceObserver_ = observer;
        NavUtils.NavigateToPage(nameof (EditParticipantsPage));
        return (Action) (() => { });
      }));
    }

    private void DisposeAll()
    {
      this.participantChangeSub_.SafeDispose();
      this.participantChangeSub_ = (IDisposable) null;
      this.viewModel_.SafeDispose();
      if (this.searchSource_ == null)
        return;
      List<JidItemViewModel> itemsToDispose = this.searchSource_;
      AppState.Worker.Enqueue((Action) (() =>
      {
        foreach (IDisposable d in itemsToDispose)
          d.SafeDispose();
        itemsToDispose.Clear();
      }));
    }

    private void InitQuickSearch()
    {
      BitmapSource defaultIcon = AssetStore.DefaultContactIconBlack;
      AppState.Worker.Enqueue((Action) (() =>
      {
        List<JidItemViewModel> viewModels = (List<JidItemViewModel>) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => viewModels = ((IEnumerable<UserStatus>) db.GetWaContacts(false)).Select<UserStatus, JidItemViewModel>((Func<UserStatus, JidItemViewModel>) (u => (JidItemViewModel) new UserViewModel(u, false)
        {
          DefaultIcon = (System.Windows.Media.ImageSource) defaultIcon
        })).ToList<JidItemViewModel>()));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.searchSource_ = viewModels;
          if (this.pageRemoved_)
            this.DisposeAll();
          else
            this.QuickSearch.ItemsSource = (IEnumerable<JidItemViewModel>) this.searchSource_;
        }));
      }));
      this.QuickSearch.ItemSelected += (ItemQuickSearchControl.ItemSelectedHandler) (userVM =>
      {
        if (userVM == null || !(userVM.Model is UserStatus model2))
          return;
        this.viewModel_.AddParticipant(model2);
      });
      this.QuickSearch.WatermarkText = AppResources.TypeAName;
      this.QuickSearch.ResultBlackListFilter = (Func<JidItemViewModel, bool>) (userVM =>
      {
        UserStatus model3 = userVM.Model as UserStatus;
        return this.convo_ != null && this.convo_.ContainsParticipant(model3.Jid);
      });
    }

    private void RefreshSubmitButton()
    {
      if (this.AppBarSubmitButton == null)
        return;
      bool excludingSelf = this.convo_.IsBroadcast();
      this.AppBarSubmitButton.IsEnabled = this.convo_.GetParticipantCount(excludingSelf) > (excludingSelf ? 1 : 0);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      this.viewModel_.Orientation = e.Orientation;
      base.OnOrientationChanged(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = this.convo_ == null || this.observer_ == null;
      if (!flag && this.viewModel_ == null)
      {
        if (this.convo_.IsBroadcast())
        {
          this.viewModel_ = (EditParticipantsPageViewModel) new BroadcastEditRecipientsPageViewModel(this.convo_, this.Orientation);
          this.WarningTextBlock.Text = string.Format(AppResources.BroadcastWarning, (object) PhoneNumberFormatter.FormatInternationalNumber(Settings.ChatID));
        }
        else if (this.convo_.IsGroup())
          this.viewModel_ = (EditParticipantsPageViewModel) new GroupParticipantsPageViewModel(this.convo_, this.Orientation);
        else
          flag = true;
        if (!flag && this.viewModel_ != null)
        {
          this.participantChangeSub_ = this.viewModel_.ParticipantViewModels.GetCollectionChangedAsnyc().Subscribe<NotifyCollectionChangedEventArgs>((Action<NotifyCollectionChangedEventArgs>) (_ => this.RefreshSubmitButton()));
          this.RefreshSubmitButton();
          this.viewModel_.ParticipantAdded += (EventHandler<MultiParticipantsChatViewModelBase.ParticipantAddedEventArgs>) ((sender, args) =>
          {
            if (args == null || args.AddedParticipantViewModel == null)
              return;
            this.ParticipantList.ScrollTo((object) args.AddedParticipantViewModel);
          });
          this.DataContext = (object) this.viewModel_;
        }
      }
      base.OnNavigatedTo(e);
      if (!flag)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved_ = true;
      this.DisposeAll();
      if (this.observer_ != null)
        this.observer_.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    private void SubmitButton_Click(object sender, EventArgs e)
    {
      this.ParticipantList.Focus();
      this.ApplicationBar.IsVisible = false;
      this.observer_.OnNext(new PageArgs(this.NavigationService));
    }

    private void ParticipantList_ScrollingStarted(object sender, EventArgs e) => this.Focus();

    private void AddParticipant_Click(object sender, EventArgs e)
    {
      this.viewModel_.LaunchParticipantPicker();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/EditParticipantsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.QuickSearch = (ItemQuickSearchControl) this.FindName("QuickSearch");
      this.ParticipantList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantList");
      this.WarningTextBlock = (TextBlock) this.FindName("WarningTextBlock");
    }
  }
}
