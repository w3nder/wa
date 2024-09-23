// Decompiled with JetBrains decompiler
// Type: WhatsApp.BlockListPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class BlockListPage : PhoneApplicationPage
  {
    private bool loaded;
    private IDisposable blockListLoadSub;
    private ObservableCollection<UserViewModel> observableCollection = new ObservableCollection<UserViewModel>();
    private WaContactsListTabData contactPickerData;
    private Dictionary<string, bool> blockedJids;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector BlockedListBox;
    internal StackPanel LoadingPanel;
    internal StackPanel NoContactsPanel;
    internal TextBlock NoContactsTextBlock;
    internal StackPanel ContactsPanel;
    internal TextBlock ContactsTextBlock;
    private bool _contentLoaded;

    public BlockListPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.NoContactsTextBlock.Text = AppResources.BlockListExplanation;
      this.ContactsTextBlock.Text = this.NoContactsTextBlock.Text + "\n\n" + AppResources.BlockListRemoveInstructions;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (!this.loaded)
      {
        this.blockListLoadSub.SafeDispose();
        this.blockListLoadSub = this.LoadBlockList().ObserveOnDispatcher<IEnumerable<string>>().Subscribe<IEnumerable<string>>((Action<IEnumerable<string>>) (list =>
        {
          this.blockListLoadSub.SafeDispose();
          this.blockListLoadSub = (IDisposable) null;
          ContactsContext.Instance((Action<ContactsContext>) (db => this.observableCollection = new ObservableCollection<UserViewModel>(db.GetUserStatuses(list, true, false).Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u =>
          {
            return new UserViewModel(u)
            {
              Model2MenuItemsFunc = new Func<object, IEnumerable<MenuItem>>(this.GetBlockedContactMenuItems)
            };
          })))));
          this.LoadingPanel.Visibility = Visibility.Collapsed;
          this.ApplicationBar.IsVisible = true;
          this.observableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnBlockListCollectionChanged);
          this.UpdateInfoText();
          this.BlockedListBox.ItemsSource = (IList) this.observableCollection;
        }));
        this.loaded = true;
      }
      base.OnNavigatedTo(e);
    }

    private IObservable<IEnumerable<string>> LoadBlockList()
    {
      IObservable<IEnumerable<string>> r = (IObservable<IEnumerable<string>>) null;
      ContactsContext.Instance((Action<ContactsContext>) (contacts =>
      {
        DateTime? blockListLastUpdate = contacts.BlockListLastUpdate;
        DateTime dateTime = DateTime.Now.AddDays(-1.0);
        if (!blockListLastUpdate.HasValue || blockListLastUpdate.Value < dateTime)
          r = Observable.CreateWithDisposable<IEnumerable<string>>((Func<IObserver<IEnumerable<string>>, IDisposable>) (observer =>
          {
            IDisposable disposable = contacts.BlockListUpdateSubject.Subscribe(observer);
            App app = App.CurrentApp;
            app.Connection.InvokeWhenConnected((Action) (() => app.Connection.SendGetPrivacyList()));
            return disposable;
          }));
        else
          r = ((IEnumerable<IEnumerable<string>>) new IEnumerable<string>[1]
          {
            (IEnumerable<string>) contacts.BlockListSet.Keys.ToArray<string>()
          }).ToObservable<IEnumerable<string>>();
      }));
      return r;
    }

    private void UpdateInfoText()
    {
      if (this.observableCollection.Any<UserViewModel>())
      {
        this.NoContactsPanel.Visibility = Visibility.Collapsed;
        this.ContactsPanel.Visibility = Visibility.Visible;
      }
      else
      {
        this.NoContactsPanel.Visibility = Visibility.Visible;
        this.ContactsPanel.Visibility = Visibility.Collapsed;
      }
    }

    private IEnumerable<MenuItem> GetBlockedContactMenuItems(object model)
    {
      if (!(model is UserStatus userStatus))
        return (IEnumerable<MenuItem>) null;
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = (object) AppResources.UnblockContact;
      menuItem1.Tag = (object) userStatus;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.Delete_Click);
      return (IEnumerable<MenuItem>) new MenuItem[1]
      {
        menuItem2
      };
    }

    private void Add_Click(object sender, EventArgs e)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db => this.blockedJids = db.BlockListSet));
      ListTabData[] tabs = new ListTabData[1];
      WaContactsListTabData contactsListTabData1 = this.contactPickerData;
      if (contactsListTabData1 == null)
      {
        WaContactsListTabData contactsListTabData2 = new WaContactsListTabData();
        contactsListTabData2.EnableCache = true;
        contactsListTabData2.ItemVisibleFilter = (Func<JidItemViewModel, bool>) (item => !this.blockedJids.ContainsKey(item.Jid));
        contactsListTabData2.Header = (string) null;
        WaContactsListTabData contactsListTabData3 = contactsListTabData2;
        this.contactPickerData = contactsListTabData2;
        contactsListTabData1 = contactsListTabData3;
      }
      tabs[0] = (ListTabData) contactsListTabData1;
      JidItemPickerPage.Start(tabs, AppResources.Block, shouldCloseOnSelection: true).ObserveOnDispatcher<List<string>>().Select<List<string>, List<UserStatus>>((Func<List<string>, List<UserStatus>>) (selJids =>
      {
        List<UserStatus> userStatusList = new List<UserStatus>();
        foreach (string selJid in selJids)
          userStatusList.Add(UserCache.Get(selJid, true));
        return userStatusList;
      })).Subscribe<List<UserStatus>>((Action<List<UserStatus>>) (users =>
      {
        foreach (UserStatus user1 in users)
        {
          UserStatus user = user1;
          bool isAlreadyBlocked = false;
          ContactsContext.Instance((Action<ContactsContext>) (db => isAlreadyBlocked = db.BlockListSet.ContainsKey(user.Jid)));
          if (!isAlreadyBlocked)
          {
            ObservableCollection<UserViewModel> observableCollection = this.observableCollection;
            observableCollection.Add(new UserViewModel(user)
            {
              Model2MenuItemsFunc = new Func<object, IEnumerable<MenuItem>>(this.GetBlockedContactMenuItems)
            });
            this.UpdateInfoText();
          }
        }
      }));
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
      if (!(sender is MenuItem menuItem) || !(menuItem.Tag is UserStatus tag))
        return;
      for (int index = 0; index < this.observableCollection.Count; ++index)
      {
        if (tag.Jid == this.observableCollection[index].Jid)
        {
          this.observableCollection.RemoveAt(index);
          break;
        }
      }
      this.UpdateInfoText();
    }

    private void BlockedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.BlockedListBox.SelectedItem = (object) null;
    }

    private void OnBlockListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Remove)
        return;
      BlockContact.SetBlockedContacts(this.observableCollection.Select<UserViewModel, string>((Func<UserViewModel, string>) (item => item.Jid)).ToArray<string>());
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.observableCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnBlockListCollectionChanged);
      this.blockListLoadSub.SafeDispose();
      this.blockListLoadSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/BlockListPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.BlockedListBox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("BlockedListBox");
      this.LoadingPanel = (StackPanel) this.FindName("LoadingPanel");
      this.NoContactsPanel = (StackPanel) this.FindName("NoContactsPanel");
      this.NoContactsTextBlock = (TextBlock) this.FindName("NoContactsTextBlock");
      this.ContactsPanel = (StackPanel) this.FindName("ContactsPanel");
      this.ContactsTextBlock = (TextBlock) this.FindName("ContactsTextBlock");
    }
  }
}
