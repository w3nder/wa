// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneContactPickerPage
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WhatsApp.ContactClasses;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class PhoneContactPickerPage : PhoneApplicationPage
  {
    private static string nextInstanceTitle;
    private static IObserver<Contact> nextInstanceObserver;
    private static Func<Contact, IObservable<bool>> nextInstanceConfirmSelectionFunc;
    private static bool nextInstanceShouldOpenSearchOnEntry;
    private static bool nextInstanceShouldCloseOnSelection;
    private static bool nextInstanceEnableMultiSelect;
    private static Conversation nextInstanceConversation;
    private bool disposed;
    private IObserver<Contact> observer;
    private Func<Contact, IObservable<bool>> confirmSelectionFunc;
    private bool shouldOpenSearchOnEntry;
    private bool shouldCloseOnSelection;
    private bool allLoaded;
    private bool isPageEverLoaded;
    private System.Windows.Media.ImageSource groupImageSource;
    private ObservableCollection<PhoneContactPickerPage.ContactWrapper> resultsSrc;
    private IList<PhoneContactPickerPage.ContactWrapper>[] allItemsSrc;
    private IDisposable loadSub;
    private IDisposable searchSub;
    private RegexSearch regexSearch = new RegexSearch();
    private IDisposable searchTextChangedSub;
    private string searchTermInUse = "";
    private Storyboard slideDownSb;
    private DateTime? lastTextChangedAt;
    private IDisposable delaySub;
    private ApplicationBarIconButton sendButton;
    private Conversation conversation;
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private bool isMultiSelect;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal EmojiTextBox SearchBox;
    internal Pivot Pivot;
    internal WhatsApp.CompatibilityShims.LongListSelector ListControl;
    internal ProgressBar ProgressIndicator;
    internal Grid ShareGroupButton;
    internal Image GroupImage;
    internal TextBlock FooterTextBlock;
    private bool _contentLoaded;

    private void ClearSubscriptions()
    {
      IDisposable[] array = this.subscriptions.ToArray();
      this.subscriptions.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }

    public void Dispose()
    {
      this.disposed = true;
      this.ClearSubscriptions();
      this.GroupImage.Source = (System.Windows.Media.ImageSource) null;
      this.groupImageSource = (System.Windows.Media.ImageSource) null;
      this.searchTextChangedSub.SafeDispose();
      this.searchTextChangedSub = (IDisposable) null;
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
    }

    public PhoneContactPickerPage()
    {
      this.InitializeComponent();
      this.InitAppBar();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.observer = PhoneContactPickerPage.nextInstanceObserver;
      PhoneContactPickerPage.nextInstanceObserver = (IObserver<Contact>) null;
      string nextInstanceTitle = PhoneContactPickerPage.nextInstanceTitle;
      PhoneContactPickerPage.nextInstanceTitle = (string) null;
      this.confirmSelectionFunc = PhoneContactPickerPage.nextInstanceConfirmSelectionFunc;
      PhoneContactPickerPage.nextInstanceConfirmSelectionFunc = (Func<Contact, IObservable<bool>>) null;
      this.shouldOpenSearchOnEntry = PhoneContactPickerPage.nextInstanceShouldOpenSearchOnEntry;
      PhoneContactPickerPage.nextInstanceShouldOpenSearchOnEntry = false;
      this.shouldCloseOnSelection = PhoneContactPickerPage.nextInstanceShouldCloseOnSelection;
      PhoneContactPickerPage.nextInstanceShouldCloseOnSelection = false;
      this.conversation = PhoneContactPickerPage.nextInstanceConversation;
      PhoneContactPickerPage.nextInstanceConversation = (Conversation) null;
      this.isMultiSelect = PhoneContactPickerPage.nextInstanceEnableMultiSelect && Settings.IsWaAdmin;
      PhoneContactPickerPage.nextInstanceEnableMultiSelect = false;
      if (this.conversation != null && this.conversation.IsGroup() && this.isMultiSelect)
      {
        this.ShareGroupButton.Visibility = Visibility.Visible;
        ((Action) (() =>
        {
          if (this.disposed)
            return;
          this.subscriptions.Add(ChatPictureStore.Get(this.conversation.Jid, false, true, false, this.groupImageSource == null ? ChatPictureStore.SubMode.Default : ChatPictureStore.SubMode.TrackChange).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
          {
            if (this.disposed)
              return;
            if (picState == null || picState.Image == null)
            {
              this.groupImageSource = (System.Windows.Media.ImageSource) null;
              this.GroupImage.Visibility = Visibility.Collapsed;
            }
            else
            {
              this.GroupImage.Source = this.groupImageSource = (System.Windows.Media.ImageSource) picState.Image;
              this.GroupImage.Visibility = Visibility.Visible;
            }
          })));
        }))();
      }
      else
        this.ShareGroupButton.Visibility = Visibility.Collapsed;
      if (!string.IsNullOrEmpty(nextInstanceTitle))
      {
        this.TitlePanel.SmallTitle = nextInstanceTitle;
        this.TitlePanel.Visibility = Visibility.Visible;
      }
      this.Pivot.HeaderTemplate = (DataTemplate) null;
      this.searchTextChangedSub = this.SearchBox.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchBox_TextChanged));
      this.ListControl.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ListControl_ManipulationStarted);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.LoadContacts();
    }

    public static IObservable<Contact> Start(
      string title,
      Func<Contact, IObservable<bool>> confirmSelectionFunc = null,
      bool shouldOpenSearchOnEntry = false,
      bool shouldCloseOnSelection = false,
      bool multiSelectEnabled = false,
      Conversation groupConvo = null)
    {
      return Observable.Create<Contact>((Func<IObserver<Contact>, Action>) (observer =>
      {
        PhoneContactPickerPage.nextInstanceObserver = observer;
        PhoneContactPickerPage.nextInstanceTitle = title;
        PhoneContactPickerPage.nextInstanceConfirmSelectionFunc = confirmSelectionFunc;
        PhoneContactPickerPage.nextInstanceShouldOpenSearchOnEntry = shouldOpenSearchOnEntry;
        PhoneContactPickerPage.nextInstanceShouldCloseOnSelection = shouldCloseOnSelection;
        PhoneContactPickerPage.nextInstanceConversation = groupConvo;
        PhoneContactPickerPage.nextInstanceEnableMultiSelect = multiSelectEnabled;
        NavUtils.NavigateToPage(nameof (PhoneContactPickerPage));
        return (Action) (() => { });
      }));
    }

    private void LoadContacts()
    {
      if (this.loadSub != null || this.allLoaded)
        return;
      this.ProgressIndicator.Visibility = Visibility.Visible;
      this.loadSub = ContactStore.GetAllContacts().Catch<Contact[], Exception>((Func<Exception, IObservable<Contact[]>>) (exn =>
      {
        this.FooterTextBlock.Text = string.Format(AppResources.ErrorReadingContacts, (object) exn.Message);
        this.FooterTextBlock.Visibility = Visibility.Visible;
        this.ProgressIndicator.Visibility = Visibility.Collapsed;
        return Observable.Empty<Contact[]>();
      })).SubscribeOn<Contact[]>(WAThreadPool.Scheduler).Do<Contact[]>((Action<Contact[]>) (contacts => this.InitSearch(contacts))).Select<Contact[], IEnumerable<PhoneContactPickerPage.ContactWrapper>>((Func<Contact[], IEnumerable<PhoneContactPickerPage.ContactWrapper>>) (contacts => ((IEnumerable<Contact>) contacts).Select<Contact, PhoneContactPickerPage.ContactWrapper>((Func<Contact, PhoneContactPickerPage.ContactWrapper>) (c => new PhoneContactPickerPage.ContactWrapper(c, this.isMultiSelect))))).Select<IEnumerable<PhoneContactPickerPage.ContactWrapper>, IList<PhoneContactPickerPage.ContactWrapper>[]>((Func<IEnumerable<PhoneContactPickerPage.ContactWrapper>, IList<PhoneContactPickerPage.ContactWrapper>[]>) (contacts => contacts.OrderBy<PhoneContactPickerPage.ContactWrapper, string>((Func<PhoneContactPickerPage.ContactWrapper, string>) (c => c.Contact.DisplayName)).GroupBy<PhoneContactPickerPage.ContactWrapper, string>((Func<PhoneContactPickerPage.ContactWrapper, string>) (c => c.Contact.DisplayName.ToGroupChar())).OrderBy<IGrouping<string, PhoneContactPickerPage.ContactWrapper>, string>((Func<IGrouping<string, PhoneContactPickerPage.ContactWrapper>, string>) (g => g.Key)).Select<IGrouping<string, PhoneContactPickerPage.ContactWrapper>, IList<PhoneContactPickerPage.ContactWrapper>>((Func<IGrouping<string, PhoneContactPickerPage.ContactWrapper>, IList<PhoneContactPickerPage.ContactWrapper>>) (g => g.ToGoodGrouping<string, PhoneContactPickerPage.ContactWrapper>())).ToArray<IList<PhoneContactPickerPage.ContactWrapper>>())).ObserveOnDispatcher<IList<PhoneContactPickerPage.ContactWrapper>[]>().Subscribe<IList<PhoneContactPickerPage.ContactWrapper>[]>((Action<IList<PhoneContactPickerPage.ContactWrapper>[]>) (listSrc =>
      {
        this.allLoaded = true;
        this.ProgressIndicator.Visibility = Visibility.Collapsed;
        this.loadSub.SafeDispose();
        this.loadSub = (IDisposable) null;
        this.allItemsSrc = listSrc;
        if (this.searchSub != null)
          return;
        this.ListControl.IsFlatList = false;
        this.ListControl.GroupHeaderTemplate = this.Resources[(object) "GroupHeaderTemplate"] as DataTemplate;
        this.ListControl.ItemsSource = (IList) this.allItemsSrc;
      }));
    }

    public void SubmitButton_Tap(object sender, EventArgs e)
    {
      if (this.resultsSrc != null)
      {
        foreach (PhoneContactPickerPage.ContactWrapper contactWrapper in (Collection<PhoneContactPickerPage.ContactWrapper>) this.resultsSrc)
        {
          if (contactWrapper.IsChecked)
            this.observer.OnNext(contactWrapper.Contact);
        }
      }
      else
      {
        foreach (IEnumerable<PhoneContactPickerPage.ContactWrapper> contactWrappers in this.allItemsSrc)
        {
          foreach (PhoneContactPickerPage.ContactWrapper contactWrapper in contactWrappers)
          {
            if (contactWrapper.IsChecked)
              this.observer.OnNext(contactWrapper.Contact);
          }
        }
      }
      this.observer.OnCompleted();
    }

    private void InitSearch(Contact[] contacts)
    {
      this.regexSearch.Init(((IEnumerable<Contact>) contacts).Select<Contact, KeyValuePair<string, object>>((Func<Contact, KeyValuePair<string, object>>) (c => new KeyValuePair<string, object>(c.DisplayName ?? "", (object) c))).ToArray<KeyValuePair<string, object>>());
    }

    private void SearchContacts(string rawTerm)
    {
      if (!this.allLoaded)
        return;
      string str = rawTerm == null ? "" : rawTerm.Trim();
      if (str == this.searchTermInUse)
        return;
      Action cleanup = (Action) (() =>
      {
        this.searchSub.SafeDispose();
        this.searchSub = (IDisposable) null;
        this.ProgressIndicator.Visibility = Visibility.Collapsed;
        this.FooterTextBlock.Visibility = Visibility.Collapsed;
      });
      cleanup();
      this.searchTermInUse = str;
      if (string.IsNullOrWhiteSpace(this.searchTermInUse))
      {
        this.ListControl.IsFlatList = false;
        this.ListControl.GroupHeaderTemplate = this.Resources[(object) "GroupHeaderTemplate"] as DataTemplate;
        this.ListControl.ItemsSource = (IList) this.allItemsSrc;
        this.resultsSrc.Clear();
        this.resultsSrc = (ObservableCollection<PhoneContactPickerPage.ContactWrapper>) null;
      }
      else
      {
        this.ProgressIndicator.Visibility = Visibility.Visible;
        this.searchSub = this.GetSearchObservable(this.searchTermInUse).SubscribeOn<KeyValuePair<string, Contact[]>>((IScheduler) AppState.Worker).ObserveOnDispatcher<KeyValuePair<string, Contact[]>>().Subscribe<KeyValuePair<string, Contact[]>>((Action<KeyValuePair<string, Contact[]>>) (p =>
        {
          if (p.Key != this.searchTermInUse)
            return;
          PhoneContactPickerPage.ContactWrapper[] array = ((IEnumerable<Contact>) p.Value).Select<Contact, PhoneContactPickerPage.ContactWrapper>((Func<Contact, PhoneContactPickerPage.ContactWrapper>) (c => new PhoneContactPickerPage.ContactWrapper(c, this.isMultiSelect))).ToArray<PhoneContactPickerPage.ContactWrapper>();
          if (this.resultsSrc == null)
            this.resultsSrc = new ObservableCollection<PhoneContactPickerPage.ContactWrapper>((IEnumerable<PhoneContactPickerPage.ContactWrapper>) array);
          else if (((IEnumerable<PhoneContactPickerPage.ContactWrapper>) array).Any<PhoneContactPickerPage.ContactWrapper>())
            Utils.UpdateInPlace<PhoneContactPickerPage.ContactWrapper>((IList<PhoneContactPickerPage.ContactWrapper>) this.resultsSrc, (IList<PhoneContactPickerPage.ContactWrapper>) array, (Func<PhoneContactPickerPage.ContactWrapper, string>) (c => c.Contact.GetIdentifier()), (Action<PhoneContactPickerPage.ContactWrapper>) null);
          else
            this.resultsSrc.Clear();
          if (this.ListControl.ItemsSource != this.resultsSrc)
          {
            this.ListControl.IsFlatList = true;
            this.ListControl.ItemsSource = (IList) this.resultsSrc;
          }
          cleanup();
          if (this.resultsSrc.Any<PhoneContactPickerPage.ContactWrapper>())
            return;
          this.FooterTextBlock.Text = AppResources.NoResults;
          this.FooterTextBlock.Visibility = Visibility.Visible;
        }), (Action<Exception>) (ex => cleanup()), cleanup);
      }
    }

    private IObservable<KeyValuePair<string, Contact[]>> GetSearchObservable(string searchTerm)
    {
      return Observable.Create<KeyValuePair<string, Contact[]>>((Func<IObserver<KeyValuePair<string, Contact[]>>, Action>) (observer =>
      {
        string[] strArray1;
        if (!string.IsNullOrWhiteSpace(searchTerm))
          strArray1 = ((IEnumerable<string>) searchTerm.ToLower().Split(' ')).Where<string>((Func<string, bool>) (part => !string.IsNullOrEmpty(part))).ToArray<string>();
        else
          strArray1 = new string[0];
        string[] strArray2 = strArray1;
        if (((IEnumerable<string>) strArray2).Any<string>())
        {
          Contact[] array = this.regexSearch.Lookup(strArray2).Select<SearchMethodBase.Match, Contact>((Func<SearchMethodBase.Match, Contact>) (m => m.Tag as Contact)).Where<Contact>((Func<Contact, bool>) (item => item != null)).ToArray<Contact>();
          observer.OnNext(new KeyValuePair<string, Contact[]>(searchTerm, array));
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private void CloseKeyboard() => this.Focus();

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageEverLoaded)
        return;
      this.isPageEverLoaded = true;
      if (!this.shouldOpenSearchOnEntry)
        return;
      this.SearchBox.Focus();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      if (this.observer != null)
        this.observer.OnCompleted();
      this.Dispose();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      if (string.IsNullOrWhiteSpace(this.SearchBox.Text))
        this.SlideDownAndBackOut();
      else
        this.SearchBox.Text = "";
    }

    private void Item_Tap(object sender, RoutedEventArgs e)
    {
      Grid grid = sender as Grid;
      PhoneContactPickerPage.ContactWrapper dataContext = ((FrameworkElement) e.OriginalSource).DataContext as PhoneContactPickerPage.ContactWrapper;
      if (this.isMultiSelect)
      {
        dataContext.IsChecked = !dataContext.IsChecked;
        (grid.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is CheckBox)).FirstOrDefault<UIElement>() as CheckBox).IsChecked = new bool?(dataContext.IsChecked);
      }
      else
      {
        this.observer.OnNext(dataContext.Contact);
        this.observer.OnCompleted();
      }
    }

    private void InitAppBar()
    {
      this.sendButton = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/sendmsg.png", UriKind.Relative),
        Text = "Send"
      };
      this.sendButton.Click += new EventHandler(this.SubmitButton_Tap);
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      bar.Buttons.Add((object) this.sendButton);
      Localizable.LocalizeAppBar(bar);
      this.ApplicationBar = (IApplicationBar) bar;
    }

    private void SearchBox_TextChanged(TextChangedEventArgs e)
    {
      string rawTerm = this.SearchBox.Text ?? "";
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
          this.SearchContacts(this.SearchBox.Text);
        }));
      else
        this.SearchContacts(rawTerm);
    }

    private void ListControl_ManipulationStarted(object sender, EventArgs e)
    {
      this.CloseKeyboard();
    }

    private void ListControl_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (!this.isMultiSelect || e.ItemKind != LongListSelectorItemKind.Item)
        return;
      Grid child = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as Grid;
      PhoneContactPickerPage.ContactWrapper content = e.Container.Content as PhoneContactPickerPage.ContactWrapper;
      if (!(child.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is CheckBox)).FirstOrDefault<UIElement>() is CheckBox checkBox) || content == null)
        Log.SendCrashLog(new Exception("please fix me."), "contact picker");
      else
        checkBox.IsChecked = new bool?(content.IsChecked);
    }

    private void ShareGroup_Click(object sender, RoutedEventArgs e)
    {
      Set<string> jidSet = new Set<string>((IEnumerable<string>) this.conversation.GetParticipantJids());
      string[] rawPhones = (string[]) null;
      ContactsContext.Instance((Action<ContactsContext>) (db => rawPhones = ((IEnumerable<PhoneNumber>) db.GetAllPhoneNumbers()).Where<PhoneNumber>((Func<PhoneNumber, bool>) (p => p.Jid != null && jidSet.Contains(p.Jid))).Select<PhoneNumber, string>((Func<PhoneNumber, string>) (p => p.RawPhoneNumber)).ToArray<string>()));
      Set<string> rawSet = new Set<string>((IEnumerable<string>) rawPhones);
      ContactStore.GetAllContacts().Select<Contact[], IEnumerable<Contact>>((Func<Contact[], IEnumerable<Contact>>) (c => ((IEnumerable<Contact>) c).Where<Contact>((Func<Contact, bool>) (contact =>
      {
        foreach (ContactPhoneNumber phoneNumber in contact.PhoneNumbers)
        {
          if (phoneNumber.PhoneNumber != null && rawSet.Contains(phoneNumber.PhoneNumber))
            return true;
        }
        return false;
      })))).Subscribe<IEnumerable<Contact>>((Action<IEnumerable<Contact>>) (contacts =>
      {
        foreach (Contact contact in contacts)
          this.observer.OnNext(contact);
        this.observer.OnCompleted();
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/PhoneContactPickerPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.SearchBox = (EmojiTextBox) this.FindName("SearchBox");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.ListControl = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ListControl");
      this.ProgressIndicator = (ProgressBar) this.FindName("ProgressIndicator");
      this.ShareGroupButton = (Grid) this.FindName("ShareGroupButton");
      this.GroupImage = (Image) this.FindName("GroupImage");
      this.FooterTextBlock = (TextBlock) this.FindName("FooterTextBlock");
    }

    private class ContactWrapper : PropChangedBase
    {
      private Contact contact;
      private bool isChecked;
      private bool multiSelectEnabled;
      private Visibility multiSelectVisibility = Visibility.Collapsed;

      public ContactWrapper(Contact c, bool mutliSelectEnabled = false)
      {
        this.Contact = c;
        this.IsChecked = false;
        this.MultiSelectEnabled = mutliSelectEnabled;
      }

      public Contact Contact
      {
        get => this.contact;
        set
        {
          if (this.contact != null && this.contact.Equals((object) value))
            return;
          this.contact = value;
          this.NotifyPropertyChanged(nameof (Contact));
        }
      }

      public bool IsChecked
      {
        get => this.isChecked;
        set
        {
          if (this.isChecked == value)
            return;
          this.isChecked = value;
          this.NotifyPropertyChanged(nameof (IsChecked));
        }
      }

      public bool MultiSelectEnabled
      {
        get => this.multiSelectEnabled;
        set
        {
          this.multiSelectEnabled = value;
          this.MultiSelectVisibility = this.multiSelectEnabled.ToVisibility();
        }
      }

      public Visibility MultiSelectVisibility
      {
        get => this.MultiSelectEnabled.ToVisibility();
        set
        {
          this.multiSelectVisibility = value;
          this.NotifyPropertyChanged(nameof (MultiSelectVisibility));
        }
      }
    }
  }
}
