// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatusList
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class WaStatusList : UserControl, IDisposable
  {
    private bool isDisposed;
    private IDisposable loadingSub;
    private IDisposable dbResetSub;
    private List<IDisposable> dbUpdateSubs = new List<IDisposable>();
    private bool upToDate;
    private ObservableCollection<KeyedObservableCollection<string, StatusThreadItemViewModel>> listSrc = new ObservableCollection<KeyedObservableCollection<string, StatusThreadItemViewModel>>();
    private KeyedObservableCollection<string, StatusThreadItemViewModel> mutedItems;
    private KeyedObservableCollection<string, StatusThreadItemViewModel> unviewedItems;
    private KeyedObservableCollection<string, StatusThreadItemViewModel> viewedItems;
    private SelfStatusThreadViewModel selfItem;
    private StatusThreadItemViewModel psaItem;
    private List<Pair<DbDataUpdate.Types, Message>> pendingUpdates = new List<Pair<DbDataUpdate.Types, Message>>();
    private Subject<Unit> addNewStatusSubj = new Subject<Unit>();
    internal LongListSelector StatusList;
    internal WaSelfStatusItemControl SelfStatusControl;
    internal Grid ExpandButton;
    internal OwnStatusList OwnStatusList;
    private bool _contentLoaded;

    public WaStatusList()
    {
      this.InitializeComponent();
      this.upToDate = false;
      this.StatusList.OverlapScrollBar = true;
      this.StatusList.JumpListStyle = (Style) null;
      this.StatusList.ItemsSource = (IList) this.listSrc;
      this.OwnStatusList.UseSmallItemTemplate = true;
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.OwnStatusList.Dispose();
      this.loadingSub.SafeDispose();
      this.loadingSub = (IDisposable) null;
      this.dbResetSub.SafeDispose();
      this.dbResetSub = (IDisposable) null;
      this.dbUpdateSubs.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.dbUpdateSubs.Clear();
      this.isDisposed = true;
    }

    public IObservable<Unit> AddNewStatusObservable() => (IObservable<Unit>) this.addNewStatusSubj;

    public void TryReload()
    {
      WaStatusHelper.GenerateSessionId();
      if (this.upToDate)
        return;
      this.loadingSub.SafeDispose();
      this.loadingSub = (IDisposable) null;
      this.dbUpdateSubs.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.dbUpdateSubs.Clear();
      if (this.dbResetSub == null)
        this.dbResetSub = AppState.DbResetSubject.ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
        {
          this.upToDate = false;
          this.TryReload();
        }));
      this.loadingSub = this.GetStatusThreads().SubscribeOn<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>(WAThreadPool.Scheduler).ObserveOnDispatcher<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>().Subscribe<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>((Action<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>) (p =>
      {
        if (this.isDisposed)
        {
          p.Second.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
        }
        else
        {
          if (p.Second != null && p.Second.Any<IDisposable>())
            this.dbUpdateSubs.AddRange((IEnumerable<IDisposable>) p.Second);
          switch (p.First.Key)
          {
            case WaStatusList.StatusGroupings.Self:
              SelfStatusThreadViewModel statusThreadViewModel = p.First.Value.First<StatusThreadItemViewModel>() as SelfStatusThreadViewModel;
              this.SelfStatusControl.ViewModel = (JidItemViewModel) (this.selfItem = statusThreadViewModel);
              this.UpdateOwnStatus(statusThreadViewModel.StatusThread);
              break;
            case WaStatusList.StatusGroupings.HasUnviewed:
              this.ShowUnviewed(p.First.Value);
              break;
            case WaStatusList.StatusGroupings.AllViewed:
              this.ShowViewed(p.First.Value);
              break;
            case WaStatusList.StatusGroupings.Muted:
              this.ShowMuted(p.First.Value);
              break;
            case WaStatusList.StatusGroupings.Psa:
              this.ShowPsa(p.First.Value.FirstOrDefault<StatusThreadItemViewModel>());
              break;
          }
        }
      }), (Action) (() =>
      {
        if (this.pendingUpdates.Any<Pair<DbDataUpdate.Types, Message>>())
        {
          this.pendingUpdates.ForEach((Action<Pair<DbDataUpdate.Types, Message>>) (pu =>
          {
            if (pu.First == DbDataUpdate.Types.Added)
              this.OnNewStatusMessage(pu.Second);
            else if (pu.First == DbDataUpdate.Types.Modified)
            {
              this.OnMessageStatusChanged(pu.Second);
            }
            else
            {
              if (pu.First != DbDataUpdate.Types.Deleted)
                return;
              this.OnStatusMessageDeleted(pu.Second);
            }
          }));
          this.pendingUpdates.Clear();
        }
        this.upToDate = true;
        this.loadingSub = (IDisposable) null;
      }));
    }

    private IObservable<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>> GetStatusThreads()
    {
      return Observable.Create<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>((Func<IObserver<Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>>, Action>) (observer =>
      {
        List<IDisposable> subs = new List<IDisposable>();
        HashSet<string> mutedJids = (HashSet<string>) null;
        List<WaStatusThread> statusThreads = (List<WaStatusThread>) null;
        WaStatusThread psaThread = (WaStatusThread) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          SelfStatusThreadViewModel statusThreadViewModel = new SelfStatusThreadViewModel(db.GetStatusThread(Settings.MyJid), false);
          observer.OnNext(new Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>(new KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>(WaStatusList.StatusGroupings.Self, new List<StatusThreadItemViewModel>()
          {
            (StatusThreadItemViewModel) statusThreadViewModel
          }), (List<IDisposable>) null));
          mutedJids = db.GetStatusMutedJids();
          if (Settings.IsStatusPSAUnseen)
            psaThread = db.GetStatusThread("0@s.whatsapp.net", false);
          statusThreads = db.GetStatusThreads(true);
          subs.Add(db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus())).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnNewStatusMessage)));
          subs.Add(db.MessageUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => (u.UpdatedObj is Message updatedObj2 ? (updatedObj2.IsStatus() ? 1 : 0) : 0) != 0 && ((IEnumerable<string>) u.ModifiedColumns).Contains<string>("Status"))).ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (u => this.OnMessageStatusChanged(u.UpdatedObj as Message))));
          subs.Add(JidInfo.JidInfoUpdatedSubject.ObserveOnDispatcher<JidInfo>().Subscribe<JidInfo>(new Action<JidInfo>(this.OnStatusMuteUpdated)));
          subs.Add(db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus())).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnStatusMessageDeleted)));
          subs.Add(db.UpdatedMessageMediaWaTypeSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus())).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnStatusMessageMediaTypeChanged)));
        }));
        List<StatusThreadItemViewModel> threadItemViewModelList1 = new List<StatusThreadItemViewModel>();
        List<StatusThreadItemViewModel> threadItemViewModelList2 = new List<StatusThreadItemViewModel>();
        List<StatusThreadItemViewModel> threadItemViewModelList3 = new List<StatusThreadItemViewModel>();
        if (statusThreads.Any<WaStatusThread>() || psaThread != null)
        {
          string[] jids = statusThreads.Select<WaStatusThread, string>((Func<WaStatusThread, string>) (t => t.Jid)).Where<string>((Func<string, bool>) (jid => jid != null)).ToArray<string>();
          List<UserStatus> users = (List<UserStatus>) null;
          if (((IEnumerable<string>) jids).Any<string>())
            ContactsContext.Instance((Action<ContactsContext>) (db => users = db.GetUserStatuses((IEnumerable<string>) jids, true, true)));
          else
            users = new List<UserStatus>();
          Dictionary<string, UserStatus> dictionary = users.ToDictionary<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid));
          dictionary.Remove(Settings.MyJid);
          if (psaThread == null && dictionary.ContainsKey("0@s.whatsapp.net"))
            psaThread = statusThreads.FirstOrDefault<WaStatusThread>((Func<WaStatusThread, bool>) (t => JidHelper.IsPsaJid(t.Jid)));
          dictionary.Remove("0@s.whatsapp.net");
          UserStatus sender = (UserStatus) null;
          foreach (WaStatusThread statusThread in statusThreads)
          {
            if (dictionary.TryGetValue(statusThread.Jid, out sender) && sender != null)
            {
              StatusThreadItemViewModel threadItemViewModel = new StatusThreadItemViewModel(statusThread, sender);
              if (mutedJids.Contains(statusThread.Jid))
              {
                threadItemViewModel.IsMuted = true;
                threadItemViewModelList3.Add(threadItemViewModel);
              }
              else if (statusThread.LatestStatus.IsViewed)
                threadItemViewModelList2.Add(threadItemViewModel);
              else
                threadItemViewModelList1.Add(threadItemViewModel);
            }
          }
        }
        observer.OnNext(new Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>(new KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>(WaStatusList.StatusGroupings.HasUnviewed, threadItemViewModelList1), subs));
        threadItemViewModelList2.Sort((Comparison<StatusThreadItemViewModel>) ((vm1, vm2) => StringComparer.CurrentCulture.Compare(vm1.Sender.GetDisplayName(), vm2.Sender.GetDisplayName())));
        observer.OnNext(new Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>(new KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>(WaStatusList.StatusGroupings.AllViewed, threadItemViewModelList2), (List<IDisposable>) null));
        observer.OnNext(new Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>(new KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>(WaStatusList.StatusGroupings.Muted, threadItemViewModelList3), (List<IDisposable>) null));
        if (psaThread != null && psaThread.Count > 0)
        {
          StatusThreadItemViewModel threadItemViewModel = new StatusThreadItemViewModel(psaThread, UserCache.Get("0@s.whatsapp.net", true))
          {
            EnableContextMenu = false
          };
          observer.OnNext(new Pair<KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>, List<IDisposable>>(new KeyValuePair<WaStatusList.StatusGroupings, List<StatusThreadItemViewModel>>(WaStatusList.StatusGroupings.Psa, new List<StatusThreadItemViewModel>()
          {
            threadItemViewModel
          }), (List<IDisposable>) null));
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private bool IsPending()
    {
      return this.unviewedItems == null || this.viewedItems == null || this.mutedItems == null;
    }

    private void UpdateOwnStatus(WaStatusThread statusThread = null)
    {
      if (statusThread == null || !JidHelper.IsSelfJid(statusThread.Jid))
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => statusThread = db.GetStatusThread(Settings.MyJid)));
      if (statusThread == null)
        return;
      if (this.selfItem != null)
        this.selfItem.StatusThread = statusThread;
      this.ExpandButton.Visibility = (statusThread.Count > 0).ToVisibility();
    }

    private void ShowUnviewed(List<StatusThreadItemViewModel> items)
    {
      if (items == null)
        return;
      if (this.unviewedItems == null)
      {
        this.unviewedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.RecentStatusUpdates, (IEnumerable<StatusThreadItemViewModel>) items);
        if (!items.Any<StatusThreadItemViewModel>())
          return;
        this.listSrc.Insert(0, this.unviewedItems);
      }
      else if (items.Any<StatusThreadItemViewModel>())
      {
        Utils.UpdateInPlace<StatusThreadItemViewModel>((IList<StatusThreadItemViewModel>) this.unviewedItems, (IList<StatusThreadItemViewModel>) items, (Func<StatusThreadItemViewModel, string>) (vm => vm.Jid), (Action<StatusThreadItemViewModel>) (vm => vm.Refresh()));
        if (this.listSrc.Contains(this.unviewedItems))
          return;
        this.listSrc.Insert(0, this.unviewedItems);
      }
      else
      {
        this.listSrc.Remove(this.unviewedItems);
        this.unviewedItems.Clear();
      }
    }

    private void ShowViewed(List<StatusThreadItemViewModel> items)
    {
      if (items == null)
        return;
      if (this.viewedItems == null)
      {
        this.viewedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.ViewedStatusUpdates, (IEnumerable<StatusThreadItemViewModel>) items);
        if (!items.Any<StatusThreadItemViewModel>())
          return;
        int index = this.mutedItems == null ? -1 : this.listSrc.IndexOf(this.mutedItems);
        if (index < 0)
          this.listSrc.Add(this.viewedItems);
        else
          this.listSrc.Insert(index, this.viewedItems);
      }
      else if (items.Any<StatusThreadItemViewModel>())
      {
        Utils.UpdateInPlace<StatusThreadItemViewModel>((IList<StatusThreadItemViewModel>) this.viewedItems, (IList<StatusThreadItemViewModel>) items, (Func<StatusThreadItemViewModel, string>) (vm => vm.Jid), (Action<StatusThreadItemViewModel>) (vm => vm.Refresh()));
        if (this.listSrc.Contains(this.viewedItems))
          return;
        int index = this.mutedItems == null ? -1 : this.listSrc.IndexOf(this.mutedItems);
        if (index < 0)
          this.listSrc.Add(this.viewedItems);
        else
          this.listSrc.Insert(index, this.viewedItems);
      }
      else
      {
        this.listSrc.Remove(this.viewedItems);
        this.viewedItems.Clear();
      }
    }

    private void ShowPsa(StatusThreadItemViewModel item)
    {
      if (!JidHelper.IsPsaJid(item.Jid))
        return;
      this.RemoveUnviewedStatusThread("0@s.whatsapp.net");
      this.RemoveViewedStatusThread("0@s.whatsapp.net");
      this.psaItem = item;
      if (((int) item.StatusThread?.LatestStatus?.IsViewed ?? 0) != 0)
        this.AddViewedStatusThread(item);
      else
        this.AddUnviewedStatusThread(item);
    }

    private void ShowMuted(List<StatusThreadItemViewModel> items)
    {
      if (items == null)
        return;
      if (this.mutedItems == null)
      {
        this.mutedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.StatusMutedTitle, (IEnumerable<StatusThreadItemViewModel>) items);
        if (!items.Any<StatusThreadItemViewModel>())
          return;
        this.listSrc.Add(this.mutedItems);
      }
      else if (items.Any<StatusThreadItemViewModel>())
      {
        Utils.UpdateInPlace<StatusThreadItemViewModel>((IList<StatusThreadItemViewModel>) this.mutedItems, (IList<StatusThreadItemViewModel>) items, (Func<StatusThreadItemViewModel, string>) (vm => vm.Jid), (Action<StatusThreadItemViewModel>) (vm => vm.Refresh()));
        if (this.listSrc.Contains(this.mutedItems))
          return;
        this.listSrc.Add(this.mutedItems);
      }
      else
      {
        this.listSrc.Remove(this.mutedItems);
        this.mutedItems.Clear();
      }
    }

    private void AddUnviewedStatusThread(StatusThreadItemViewModel item)
    {
      if (item == null || item.StatusThread?.LatestStatus == null)
        return;
      item.IsMuted = false;
      bool flag;
      if (this.unviewedItems == null)
      {
        this.unviewedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.RecentStatusUpdates, (IEnumerable<StatusThreadItemViewModel>) new StatusThreadItemViewModel[1]
        {
          item
        });
        flag = true;
      }
      else
      {
        if (JidHelper.IsPsaJid(item.Jid))
          this.unviewedItems.Insert(0, item);
        else
          this.unviewedItems.InsertInOrder<StatusThreadItemViewModel>(item, (Func<StatusThreadItemViewModel, StatusThreadItemViewModel, bool>) ((vm1, vm2) => vm1.StatusThread.LatestStatus.Timestamp > vm2.StatusThread.LatestStatus.Timestamp && !JidHelper.IsPsaJid(vm2.Jid) || JidHelper.IsPsaJid(vm1.Jid)));
        flag = !this.listSrc.Contains(this.unviewedItems);
      }
      if (!flag)
        return;
      this.listSrc.Insert(0, this.unviewedItems);
    }

    private void RemoveFromAll(string jid)
    {
      this.RemoveUnviewedStatusThread(jid);
      this.RemoveViewedStatusThread(jid);
      this.RemoveMutedStatusThread(jid);
    }

    private void RemoveUnviewedStatusThread(string jid)
    {
      if (this.unviewedItems != null)
        this.unviewedItems.RemoveWhere<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == jid));
      if (this.unviewedItems.Any<StatusThreadItemViewModel>())
        return;
      this.listSrc.Remove(this.unviewedItems);
    }

    private void AddViewedStatusThread(StatusThreadItemViewModel item)
    {
      if (item == null || item.StatusThread?.LatestStatus == null)
        return;
      item.IsMuted = false;
      bool flag;
      if (this.viewedItems == null)
      {
        this.viewedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.PreviousStatusUpdates, (IEnumerable<StatusThreadItemViewModel>) new StatusThreadItemViewModel[1]
        {
          item
        });
        flag = true;
      }
      else
      {
        if (JidHelper.IsPsaJid(item.Jid))
          this.viewedItems.Insert(0, item);
        else
          this.viewedItems.InsertInOrder<StatusThreadItemViewModel>(item, (Func<StatusThreadItemViewModel, StatusThreadItemViewModel, bool>) ((vm1, vm2) => StringComparer.CurrentCulture.Compare(vm1.TitleStr, vm2.TitleStr) < 0 && !JidHelper.IsPsaJid(vm2.Jid) || JidHelper.IsPsaJid(vm1.Jid)));
        flag = !this.listSrc.Contains(this.viewedItems);
      }
      if (!flag)
        return;
      int index = this.mutedItems == null ? -1 : this.listSrc.IndexOf(this.mutedItems);
      if (index < 0)
        this.listSrc.Add(this.viewedItems);
      else
        this.listSrc.Insert(index, this.viewedItems);
    }

    private void RemoveViewedStatusThread(string jid)
    {
      if (this.viewedItems != null)
        this.viewedItems.RemoveWhere<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == jid));
      if (this.viewedItems.Any<StatusThreadItemViewModel>())
        return;
      this.listSrc.Remove(this.viewedItems);
    }

    private void AddMutedStatusThread(StatusThreadItemViewModel item)
    {
      if (item == null || item.StatusThread?.LatestStatus == null)
        return;
      item.IsMuted = true;
      bool flag;
      if (this.mutedItems == null)
      {
        this.mutedItems = new KeyedObservableCollection<string, StatusThreadItemViewModel>(AppResources.StatusMutedTitle, (IEnumerable<StatusThreadItemViewModel>) new StatusThreadItemViewModel[1]
        {
          item
        });
        flag = true;
      }
      else
      {
        this.mutedItems.InsertInOrder<StatusThreadItemViewModel>(item, (Func<StatusThreadItemViewModel, StatusThreadItemViewModel, bool>) ((vm1, vm2) => vm1.StatusThread.LatestStatus.Timestamp > vm2.StatusThread.LatestStatus.Timestamp));
        flag = !this.listSrc.Contains(this.mutedItems);
      }
      if (!flag)
        return;
      this.listSrc.Add(this.mutedItems);
    }

    private void RemoveMutedStatusThread(string jid)
    {
      if (this.mutedItems != null)
        this.mutedItems.RemoveWhere<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == jid));
      if (this.mutedItems.Any<StatusThreadItemViewModel>())
        return;
      this.listSrc.Remove(this.mutedItems);
    }

    private void UpdateStatusThread(string senderJid)
    {
      if (JidHelper.IsSelfJid(senderJid))
      {
        this.UpdateOwnStatus();
      }
      else
      {
        WaStatusThread statusThread = (WaStatusThread) null;
        bool isMuted = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          statusThread = db.GetStatusThread(senderJid);
          JidInfo jidInfo = db.GetJidInfo(senderJid, CreateOptions.None);
          isMuted = jidInfo != null && jidInfo.IsStatusMuted;
        }));
        if (statusThread == null || statusThread.Count <= 0)
        {
          this.RemoveFromAll(senderJid);
        }
        else
        {
          if (isMuted)
            return;
          bool flag = ((int) statusThread?.LatestStatus?.IsViewed ?? 0) != 0;
          StatusThreadItemViewModel threadItemViewModel1 = this.unviewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (item => item.Jid == senderJid));
          if (threadItemViewModel1 != null)
          {
            threadItemViewModel1.StatusThread = statusThread;
            if (flag)
              this.RemoveUnviewedStatusThread(senderJid);
          }
          StatusThreadItemViewModel threadItemViewModel2 = this.viewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (item => item.Jid == senderJid));
          if (threadItemViewModel2 == null)
          {
            if (!flag)
              return;
            UserStatus sender = UserCache.Get(senderJid, true);
            this.AddViewedStatusThread(new StatusThreadItemViewModel(statusThread, sender));
          }
          else
            threadItemViewModel2.StatusThread = statusThread;
        }
      }
    }

    private void OnStatusMuteUpdated(JidInfo ji)
    {
      if (ji == null)
        return;
      StatusThreadItemViewModel matchedItem = (StatusThreadItemViewModel) null;
      if (ji.IsStatusMuted)
      {
        StatusThreadItemViewModel threadItemViewModel1 = (StatusThreadItemViewModel) null;
        StatusThreadItemViewModel threadItemViewModel2;
        StatusThreadItemViewModel threadItemViewModel3 = threadItemViewModel2 = this.unviewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid));
        if (threadItemViewModel2 == null)
          threadItemViewModel2 = threadItemViewModel1 = this.viewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid));
        matchedItem = threadItemViewModel2;
        if (matchedItem != null)
        {
          if (threadItemViewModel3 != null)
            this.RemoveUnviewedStatusThread(ji.Jid);
          if (threadItemViewModel1 != null)
            this.RemoveViewedStatusThread(ji.Jid);
          if (this.mutedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid)) == null)
            this.AddMutedStatusThread(matchedItem);
        }
      }
      else
      {
        matchedItem = this.mutedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid));
        if (matchedItem != null)
        {
          this.RemoveMutedStatusThread(ji.Jid);
          if ((this.unviewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid)) ?? this.viewedItems.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == ji.Jid))) == null)
          {
            WaStatusThread statusThread = (WaStatusThread) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => statusThread = db.GetStatusThread(matchedItem.Jid)));
            if (statusThread != null)
              matchedItem.StatusThread = statusThread;
            if (matchedItem.StatusThread.LatestStatus.IsViewed)
              this.AddViewedStatusThread(matchedItem);
            else
              this.AddUnviewedStatusThread(matchedItem);
          }
        }
      }
      if (matchedItem == null)
        return;
      matchedItem.Refresh();
    }

    private void OnMessageStatusChanged(Message msg)
    {
      if (this.IsPending())
      {
        this.pendingUpdates.Add(new Pair<DbDataUpdate.Types, Message>(DbDataUpdate.Types.Modified, msg));
      }
      else
      {
        if (msg == null || !msg.IsStatus())
          return;
        string senderJid = msg.GetSenderJid();
        if (JidHelper.IsSelfJid(senderJid))
          this.selfItem.Refresh();
        else
          this.UpdateStatusThread(senderJid);
      }
    }

    private void OnStatusMessageMediaTypeChanged(Message msg)
    {
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
        this.OnStatusMessageDeleted(msg);
      else
        this.OnNewStatusMessage(msg);
    }

    private void OnStatusMessageDeleted(Message msg)
    {
      if (this.IsPending())
      {
        this.pendingUpdates.RemoveWhere<Pair<DbDataUpdate.Types, Message>>((Func<Pair<DbDataUpdate.Types, Message>, bool>) (p => p.Second.MessageID == msg.MessageID));
        this.pendingUpdates.Add(new Pair<DbDataUpdate.Types, Message>(DbDataUpdate.Types.Deleted, msg));
      }
      else
      {
        if (msg == null || !msg.IsStatus())
          return;
        Log.l("statusv3", "update status list on revoke | {0}", (object) msg.LogInfo());
        this.UpdateStatusThread(msg.GetSenderJid());
      }
    }

    private void OnNewStatusMessage(Message msg)
    {
      if (msg == null)
        return;
      if (this.IsPending())
        this.pendingUpdates.Add(new Pair<DbDataUpdate.Types, Message>(DbDataUpdate.Types.Added, msg));
      else if (!msg.MediaWaType.IsSupportedStatusType())
      {
        Log.l("statusv3", "status list | skip new status msg | unsupported status type: {0}", (object) msg.MediaWaType);
      }
      else
      {
        string senderJid = msg.GetSenderJid();
        if (JidHelper.IsSelfJid(senderJid))
        {
          this.UpdateOwnStatus();
        }
        else
        {
          bool isMuted = false;
          WaStatusThread statusThread = (WaStatusThread) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            statusThread = db.GetStatusThread(senderJid);
            JidInfo jidInfo = db.GetJidInfo(senderJid, CreateOptions.None);
            isMuted = jidInfo != null && jidInfo.IsStatusMuted;
          }));
          if (statusThread == null || statusThread.Count <= 0)
            return;
          bool flag = false;
          KeyedObservableCollection<string, StatusThreadItemViewModel> source = isMuted ? this.mutedItems : this.unviewedItems;
          StatusThreadItemViewModel threadItemViewModel = source != null ? source.FirstOrDefault<StatusThreadItemViewModel>((Func<StatusThreadItemViewModel, bool>) (vm => vm.Jid == senderJid)) : (StatusThreadItemViewModel) null;
          UserStatus sender;
          if (threadItemViewModel == null)
          {
            sender = UserCache.Get(senderJid, true);
            threadItemViewModel = new StatusThreadItemViewModel(statusThread, sender);
          }
          else
          {
            sender = threadItemViewModel.Sender;
            threadItemViewModel.StatusThread = statusThread;
            if (source[0].Jid == senderJid)
              flag = true;
            else
              source.Remove(threadItemViewModel);
          }
          if (sender != null && sender.IsInDeviceContactList && !flag)
          {
            if (isMuted)
              this.AddMutedStatusThread(threadItemViewModel);
            else
              this.AddUnviewedStatusThread(threadItemViewModel);
          }
          threadItemViewModel.Refresh();
          this.RemoveViewedStatusThread(senderJid);
        }
      }
    }

    private void Item_Tap(object sender, GestureEventArgs e)
    {
      if (!((sender is StatusItemControl statusItemControl ? statusItemControl.ViewModel : (JidItemViewModel) null) is StatusThreadItemViewModel viewModel))
        return;
      WaStatus latestStatus = viewModel.StatusThread?.LatestStatus;
      if (latestStatus == null)
        return;
      WaStatusThread[] allThreads = (WaStatusThread[]) null;
      if (!latestStatus.IsViewed && !viewModel.IsMuted && !JidHelper.IsPsaJid(latestStatus.Jid))
      {
        KeyedObservableCollection<string, StatusThreadItemViewModel> unviewedItems = this.unviewedItems;
        allThreads = unviewedItems != null ? unviewedItems.Select<StatusThreadItemViewModel, WaStatusThread>((Func<StatusThreadItemViewModel, WaStatusThread>) (vm => vm.StatusThread)).ToArray<WaStatusThread>() : (WaStatusThread[]) null;
      }
      WaStatusViewPage.Start(viewModel.StatusThread, allThreads, !latestStatus.IsViewed);
    }

    private void SelfStatusControl_Tap(object sender, GestureEventArgs e)
    {
      WaStatusThread statusThread = this.selfItem?.StatusThread;
      if (statusThread?.LatestStatus == null)
        this.addNewStatusSubj.OnNext(new Unit());
      else
        WaStatusViewPage.Start(statusThread, (WaStatusThread[]) null, false);
    }

    private void ExpandButton_Tap(object sender, GestureEventArgs e) => OwnStatusesPage.Start();

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/WaStatusList.xaml", UriKind.Relative));
      this.StatusList = (LongListSelector) this.FindName("StatusList");
      this.SelfStatusControl = (WaSelfStatusItemControl) this.FindName("SelfStatusControl");
      this.ExpandButton = (Grid) this.FindName("ExpandButton");
      this.OwnStatusList = (OwnStatusList) this.FindName("OwnStatusList");
    }

    private enum StatusGroupings
    {
      Self,
      HasUnviewed,
      AllViewed,
      Muted,
      Psa,
    }
  }
}
