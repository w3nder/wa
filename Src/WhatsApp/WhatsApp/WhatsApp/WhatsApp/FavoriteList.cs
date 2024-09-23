// Decompiled with JetBrains decompiler
// Type: WhatsApp.FavoriteList
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class FavoriteList : UserControl
  {
    private IDisposable fetchFavsSub;
    private IDisposable favUpdateSub;
    private IDisposable favSelChangeSub;
    private Dictionary<string, KeyedObservableCollection<string, UserViewModel>> groupedVms;
    private ObservableCollection<UserViewModel> flatSource;
    private List<Pair<UserStatus, DbDataUpdate.Types>> pendingChanges = new List<Pair<UserStatus, DbDataUpdate.Types>>();
    private bool isShowRequested;
    private bool isShown;
    private bool shouldShowAsFlat;
    private Subject<int> dataReadySubject = new Subject<int>();
    internal DataTemplate FavGroupHeaderTemplate;
    internal ProgressBar LoadingProgressBar;
    internal WhatsApp.CompatibilityShims.LongListSelector FavList;
    internal RoundButton InviteFriendsButton;
    internal TextBlock InviteFriendsButtonCaptionBlock;
    internal TextBlock FavoritesCountText;
    private bool _contentLoaded;

    public FavoriteList()
    {
      this.InitializeComponent();
      this.FavList.OverlapScrollBar = true;
      this.Init();
      this.InviteFriendsButton.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/tell-a-friend-icon-black.png", "/Images/tell-a-friend-icon.png");
      this.InviteFriendsButtonCaptionBlock.Text = AppResources.InviteFriendsToWA;
    }

    private void Init()
    {
      DateTime? timeStart = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      this.fetchFavsSub = Observable.Create<UserStatus[]>((Func<IObserver<UserStatus[]>, Action>) (observer =>
      {
        UserStatus[] favs = (UserStatus[]) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => favs = db.GetWaContacts(true)));
        observer.OnNext(favs ?? new UserStatus[0]);
        return (Action) (() => { });
      })).SubscribeOn<UserStatus[]>(WAThreadPool.Scheduler).Catch<UserStatus[]>(Observable.Return<UserStatus[]>(new UserStatus[0])).Take<UserStatus[]>(1).Select<UserStatus[], Dictionary<string, KeyedObservableCollection<string, UserViewModel>>>((Func<UserStatus[], Dictionary<string, KeyedObservableCollection<string, UserViewModel>>>) (favs => ((IEnumerable<UserStatus>) favs).Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u =>
      {
        return new UserViewModel(u)
        {
          Model2MenuItemsFunc = new Func<object, IEnumerable<MenuItem>>(this.GetFavItemMenuItems)
        };
      })).OrderBy<UserViewModel, string>((Func<UserViewModel, string>) (vm => vm.TitleStr)).GroupBy<UserViewModel, string>((Func<UserViewModel, string>) (vm => vm.TitleStr.ToGroupChar())).ToDictionary<IGrouping<string, UserViewModel>, string, KeyedObservableCollection<string, UserViewModel>>((Func<IGrouping<string, UserViewModel>, string>) (g => g.Key), (Func<IGrouping<string, UserViewModel>, KeyedObservableCollection<string, UserViewModel>>) (g => new KeyedObservableCollection<string, UserViewModel>(g))))).ObserveOnDispatcher<Dictionary<string, KeyedObservableCollection<string, UserViewModel>>>().Subscribe<Dictionary<string, KeyedObservableCollection<string, UserViewModel>>>((Action<Dictionary<string, KeyedObservableCollection<string, UserViewModel>>>) (grouped =>
      {
        PerformanceTimer.End("fav list: data loaded", timeStart);
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
        this.groupedVms = grouped;
        this.FlushPendingChanges();
        int itemsCount = this.GetItemsCount();
        this.shouldShowAsFlat = itemsCount < 30;
        this.fetchFavsSub.SafeDispose();
        this.fetchFavsSub = (IDisposable) null;
        if (this.isShowRequested)
          this.Show();
        this.dataReadySubject.OnNext(itemsCount);
      }));
      this.favSelChangeSub = this.FavList.GetSelectionChangedAsync().Where<object>((Func<object, bool>) (o => o != null)).ObserveOnDispatcher<object>().Do<object>((Action<object>) (u => this.FavList.SelectedItem = (object) null)).Select<object, UserViewModel>((Func<object, UserViewModel>) (o => o as UserViewModel)).Where<UserViewModel>((Func<UserViewModel, bool>) (vm => vm != null && vm.User != null)).Subscribe<UserViewModel>((Action<UserViewModel>) (vm => this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToChat(vm.User.Jid, false)))));
      this.favUpdateSub = ContactsContext.Events.UserStatusUpdatedSubject.ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (u => this.OnUserStatusUpdated(u.UpdatedObj as UserStatus, u.UpdateType)));
    }

    public void DisposeAll()
    {
      this.favSelChangeSub.SafeDispose();
      this.favUpdateSub.SafeDispose();
      this.fetchFavsSub.SafeDispose();
      this.favUpdateSub = this.fetchFavsSub = this.favSelChangeSub = (IDisposable) null;
    }

    public void Reload()
    {
      this.DisposeAll();
      this.isShown = false;
      this.flatSource = (ObservableCollection<UserViewModel>) null;
      this.groupedVms = (Dictionary<string, KeyedObservableCollection<string, UserViewModel>>) null;
      this.Init();
    }

    public int GetItemsCount()
    {
      return this.groupedVms != null ? this.groupedVms.Sum<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>>((Func<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, int>) (p => p.Value != null ? p.Value.Count : 0)) : 0;
    }

    public UserStatus[] GetLoadedFavorites()
    {
      if (this.flatSource != null)
        return this.flatSource.Select<UserViewModel, UserStatus>((Func<UserViewModel, UserStatus>) (vm => vm.User)).ToArray<UserStatus>();
      return this.groupedVms != null ? this.groupedVms.Values.SelectMany<KeyedObservableCollection<string, UserViewModel>, UserViewModel>((Func<KeyedObservableCollection<string, UserViewModel>, IEnumerable<UserViewModel>>) (items => (IEnumerable<UserViewModel>) items)).OrderBy<UserViewModel, string>((Func<UserViewModel, string>) (vm => vm.TitleStr)).Select<UserViewModel, UserStatus>((Func<UserViewModel, UserStatus>) (vm => vm.User)).ToArray<UserStatus>() : new UserStatus[0];
    }

    public IObservable<int> GetDataReadyObservable()
    {
      return this.groupedVms != null ? Observable.Return<int>(this.GetItemsCount()) : (IObservable<int>) this.dataReadySubject;
    }

    public void Show()
    {
      this.isShowRequested = true;
      if (this.groupedVms == null || this.isShown)
        return;
      this.isShown = true;
      this.FavList.OverlapScrollBar = true;
      if (this.shouldShowAsFlat)
        this.ShowAsFlat();
      else
        this.ShowAsGrouped();
      this.UpdateCount();
    }

    private void ShowAsFlat()
    {
      this.flatSource = new ObservableCollection<UserViewModel>(this.groupedVms.SelectMany<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, UserViewModel>((Func<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, IEnumerable<UserViewModel>>) (p => p.Value.AsEnumerable<UserViewModel>())));
      this.FavList.IsFlatList = true;
      this.FavList.ItemsSource = (IList) this.flatSource;
    }

    private void ShowAsGrouped()
    {
      List<KeyedObservableCollection<string, UserViewModel>> list = this.groupedVms.OrderBy<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, string>((Func<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, string>) (p => p.Key)).Select<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, KeyedObservableCollection<string, UserViewModel>>((Func<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, KeyedObservableCollection<string, UserViewModel>>) (p => p.Value)).ToList<KeyedObservableCollection<string, UserViewModel>>();
      this.FavList.IsFlatList = false;
      this.FavList.ItemsSource = (IList) list;
    }

    private void UpdateCount()
    {
      int itemsCount = this.GetItemsCount();
      if (itemsCount > 5)
      {
        this.FavoritesCountText.Visibility = Visibility.Visible;
        this.FavoritesCountText.Text = Plurals.Instance.GetString(AppResources.NumOfFavsPlural, itemsCount);
      }
      else
        this.FavoritesCountText.Visibility = Visibility.Collapsed;
    }

    private void AddItemToList(UserStatus user, bool updateCount)
    {
      if (!user.IsInDeviceContactList)
        return;
      if (this.flatSource != null)
        this.InsertItemToGrouping(this.flatSource, user);
      if (this.groupedVms != null)
      {
        string groupChar = user.GetDisplayName().ToGroupChar();
        KeyedObservableCollection<string, UserViewModel> observableCollection = (KeyedObservableCollection<string, UserViewModel>) null;
        if (this.groupedVms.TryGetValue(groupChar, out observableCollection) && observableCollection != null)
        {
          if (!observableCollection.Any<UserViewModel>((Func<UserViewModel, bool>) (vm => vm.User.Jid == user.Jid)))
            this.InsertItemToGrouping((ObservableCollection<UserViewModel>) observableCollection, user);
        }
        else
        {
          string key = groupChar;
          UserViewModel[] items = new UserViewModel[1];
          UserViewModel userViewModel = new UserViewModel(user);
          userViewModel.Model2MenuItemsFunc = new Func<object, IEnumerable<MenuItem>>(this.GetFavItemMenuItems);
          items[0] = userViewModel;
          observableCollection = new KeyedObservableCollection<string, UserViewModel>(key, (IEnumerable<UserViewModel>) items);
          this.groupedVms.Add(groupChar, observableCollection);
          if (!this.shouldShowAsFlat)
            this.ShowAsGrouped();
        }
      }
      if (!updateCount)
        return;
      this.UpdateCount();
    }

    private void RemoveItemFromList(UserStatus user, bool updateCount)
    {
      if (this.flatSource != null)
        this.RemoveItemFromFavGrouping(this.flatSource, user.Jid);
      if (this.groupedVms != null)
      {
        bool flag1 = false;
        KeyedObservableCollection<string, UserViewModel> items1 = (KeyedObservableCollection<string, UserViewModel>) null;
        bool flag2 = false;
        string displayName = user.GetDisplayName(getNumberIfNoName: false, getFormattedNumber: false);
        if (displayName != null)
        {
          string groupChar = displayName.ToGroupChar();
          if (this.groupedVms.TryGetValue(groupChar, out items1) && items1 != null && this.RemoveItemFromFavGrouping((ObservableCollection<UserViewModel>) items1, user.Jid))
          {
            flag1 = true;
            if (items1.Count == 0)
            {
              this.groupedVms.Remove(groupChar);
              flag2 = true;
            }
          }
        }
        if (!flag1)
        {
          try
          {
            KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>> keyValuePair = this.groupedVms.First<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>>((Func<KeyValuePair<string, KeyedObservableCollection<string, UserViewModel>>, bool>) (p => p.Value.Any<UserViewModel>((Func<UserViewModel, bool>) (vm => vm.User.Jid == user.Jid))));
            KeyedObservableCollection<string, UserViewModel> items2 = keyValuePair.Value;
            if (items2 != null)
            {
              if (this.RemoveItemFromFavGrouping((ObservableCollection<UserViewModel>) items2, user.Jid))
              {
                if (items2.Count == 0)
                {
                  this.groupedVms.Remove(keyValuePair.Key);
                  flag2 = true;
                }
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
        if (flag2 && !this.shouldShowAsFlat)
          this.ShowAsGrouped();
      }
      if (!updateCount)
        return;
      this.UpdateCount();
    }

    private void FlushPendingChanges()
    {
      Pair<UserStatus, DbDataUpdate.Types>[] array = this.pendingChanges.ToArray();
      this.pendingChanges.Clear();
      foreach (Pair<UserStatus, DbDataUpdate.Types> pair in array)
        this.OnUserStatusUpdated(pair.First, pair.Second);
    }

    private void InsertItemToGrouping(ObservableCollection<UserViewModel> items, UserStatus user)
    {
      if (user == null || items == null)
        return;
      UserViewModel userViewModel = new UserViewModel(user);
      userViewModel.Model2MenuItemsFunc = new Func<object, IEnumerable<MenuItem>>(this.GetFavItemMenuItems);
      UserViewModel newItem = userViewModel;
      items.InsertInOrder<UserViewModel>(newItem, (Func<UserViewModel, UserViewModel, bool>) ((vm1, vm2) => StringComparer.CurrentCulture.Compare(vm1.User.GetDisplayName(), vm2.User.GetDisplayName()) < 0));
    }

    private bool RemoveItemFromFavGrouping(ObservableCollection<UserViewModel> items, string jid)
    {
      int index = 0;
      bool flag = false;
      foreach (UserViewModel userViewModel in (Collection<UserViewModel>) items)
      {
        if (userViewModel.User.Jid == jid)
        {
          flag = true;
          break;
        }
        ++index;
      }
      if (flag)
        items.RemoveAt(index);
      return flag;
    }

    private IEnumerable<MenuItem> GetFavItemMenuItems(object model)
    {
      if (!(model is UserStatus userStatus))
        return (IEnumerable<MenuItem>) null;
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = (object) AppResources.CreateConversationTile;
      MenuItem menuItem2 = menuItem1;
      if (!TileHelper.ChatTileExists(userStatus.Jid))
      {
        menuItem2.Tag = (object) userStatus;
        menuItem2.Click += new RoutedEventHandler(this.FavItemPin_Click);
      }
      else
        menuItem2.IsEnabled = false;
      return (IEnumerable<MenuItem>) new MenuItem[1]
      {
        menuItem2
      };
    }

    private void OnUserStatusUpdated(UserStatus user, DbDataUpdate.Types updateType)
    {
      if (user == null)
        return;
      Log.d("fav list", "user update | jid={0} type={1}", (object) user.Jid, (object) updateType);
      if (this.flatSource == null && this.groupedVms == null)
      {
        this.pendingChanges.Add(new Pair<UserStatus, DbDataUpdate.Types>(user, updateType));
      }
      else
      {
        switch (updateType)
        {
          case DbDataUpdate.Types.Added:
            this.AddItemToList(user, false);
            break;
          case DbDataUpdate.Types.Deleted:
            this.RemoveItemFromList(user, false);
            break;
          case DbDataUpdate.Types.Modified:
            this.RemoveItemFromList(user, false);
            this.AddItemToList(user, false);
            break;
        }
        this.UpdateCount();
      }
    }

    private void InviteFriendsButton_Click(object sender, EventArgs e)
    {
      NavUtils.NavigateToPage("ShareOptions");
    }

    private void FavItemPin_Click(object sender, RoutedEventArgs e)
    {
      if (!(sender is MenuItem menuItem) || !(menuItem.Tag is UserStatus tag))
        return;
      PinToStart.Pin(tag);
    }

    private void DebugIndexContact_Click(object sender, RoutedEventArgs e)
    {
      if (!(sender is MenuItem menuItem))
        return;
      UserStatus user = menuItem.Tag as UserStatus;
      if (user == null)
        return;
      ContactsContext.Instance((Action<ContactsContext>) (db => db.IndexContactForSearch(user.Jid)));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/FavoriteList.xaml", UriKind.Relative));
      this.FavGroupHeaderTemplate = (DataTemplate) this.FindName("FavGroupHeaderTemplate");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.FavList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("FavList");
      this.InviteFriendsButton = (RoundButton) this.FindName("InviteFriendsButton");
      this.InviteFriendsButtonCaptionBlock = (TextBlock) this.FindName("InviteFriendsButtonCaptionBlock");
      this.FavoritesCountText = (TextBlock) this.FindName("FavoritesCountText");
    }
  }
}
