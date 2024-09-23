// Decompiled with JetBrains decompiler
// Type: WhatsApp.OwnStatusList
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
using WhatsApp.CommonOps;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class OwnStatusList : UserControl, IDisposable
  {
    private IDisposable dbSub;
    private StatusItemViewModel[] initialItemsToShow;
    private ObservableCollection<StatusItemViewModel> listSrc;
    private bool isShowRequested;
    private int? showLimit;
    private HashSet<int> trackedMsgIds = new HashSet<int>();
    internal LongListSelector StatusList;
    internal StackPanel ViewMorePanel;
    internal TextBlock ViewMoreBlock;
    internal TextBlock BottomTooltipBlock;
    private bool _contentLoaded;

    public event EventHandler Emptied;

    public bool UseSmallItemTemplate
    {
      set
      {
        this.StatusList.ItemTemplate = this.Resources[value ? (object) "SmallSizeItemTemplate" : (object) "RegularSizeItemTemplate"] as DataTemplate;
      }
    }

    public OwnStatusList()
    {
      this.InitializeComponent();
      this.ViewMoreBlock.Text = AppResources.ViewMore;
      this.BottomTooltipBlock.Text = AppResources.SentStatusTooltip;
    }

    public void Dispose()
    {
      this.dbSub.SafeDispose();
      this.dbSub = (IDisposable) null;
    }

    public void Show(int? limit)
    {
      if (this.listSrc != null)
        return;
      this.isShowRequested = true;
      this.showLimit = limit;
      this.ViewMorePanel.Visibility = Visibility.Collapsed;
      if (this.initialItemsToShow == null)
      {
        this.Load(false);
      }
      else
      {
        if (limit.HasValue && this.initialItemsToShow.Length > limit.Value)
        {
          this.initialItemsToShow = ((IEnumerable<StatusItemViewModel>) this.initialItemsToShow).Take<StatusItemViewModel>(limit.Value).ToArray<StatusItemViewModel>();
          this.ViewMorePanel.Visibility = Visibility.Visible;
        }
        this.listSrc = new ObservableCollection<StatusItemViewModel>((IEnumerable<StatusItemViewModel>) this.initialItemsToShow);
        this.StatusList.ItemsSource = (IList) this.listSrc;
      }
    }

    public void Load(bool async)
    {
      IObservable<StatusItemViewModel[]> source = this.GetOwnStatusesObservable();
      if (async)
      {
        source = source.SubscribeOn<StatusItemViewModel[]>((IScheduler) AppState.Worker);
      }
      else
      {
        this.dbSub.SafeDispose();
        this.dbSub = (IDisposable) null;
      }
      if (this.dbSub != null)
        return;
      this.dbSub = source.ObserveOnDispatcher<StatusItemViewModel[]>().Subscribe<StatusItemViewModel[]>((Action<StatusItemViewModel[]>) (vms =>
      {
        this.initialItemsToShow = vms;
        if (!this.isShowRequested)
          return;
        this.Show(this.showLimit);
      }));
    }

    private IObservable<StatusItemViewModel[]> GetOwnStatusesObservable()
    {
      return Observable.Create<StatusItemViewModel[]>((Func<IObserver<StatusItemViewModel[]>, Action>) (observer =>
      {
        IDisposable delMsgSub = (IDisposable) null;
        IDisposable newMsgSub = (IDisposable) null;
        IDisposable newReceiptSub = (IDisposable) null;
        IDisposable msgUpdateSub = (IDisposable) null;
        object accessLock = new object();
        bool disposed = false;
        WaStatus[] statuses = (WaStatus[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          statuses = db.GetStatuses(Settings.MyJid, false, false, new TimeSpan?(WaStatus.Expiration));
          lock (accessLock)
          {
            if (disposed)
              return;
            delMsgSub = db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus() && m.KeyFromMe)).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnOutgoingStatusMessageDeleted));
            newMsgSub = db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus() && m.KeyFromMe)).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnNewOutgoingStatusMessage));
            newReceiptSub = MessagesContext.Events.NewReceiptSubject.Where<ReceiptState>((Func<ReceiptState, bool>) (rs => rs.Status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReadByTarget.GetOverrideWeight())).ObserveOnDispatcher<ReceiptState>().Subscribe<ReceiptState>(new Action<ReceiptState>(this.OnNewReadReceipt));
            msgUpdateSub = db.MessageUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => (u.UpdatedObj is Message updatedObj2 ? (updatedObj2.IsStatus() ? 1 : 0) : 0) != 0 && ((IEnumerable<string>) u.ModifiedColumns).Contains<string>("Status"))).ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (u => this.OnMessageStatusChanged(u.UpdatedObj as Message)));
          }
        }));
        this.trackedMsgIds = new HashSet<int>(((IEnumerable<WaStatus>) statuses).Select<WaStatus, int>((Func<WaStatus, int>) (s => s.MessageId)));
        observer.OnNext(((IEnumerable<WaStatus>) statuses).Select<WaStatus, StatusItemViewModel>((Func<WaStatus, StatusItemViewModel>) (s => new StatusItemViewModel(s))).ToArray<StatusItemViewModel>());
        return (Action) (() =>
        {
          lock (accessLock)
          {
            disposed = true;
            newMsgSub.SafeDispose();
            newMsgSub = (IDisposable) null;
            newReceiptSub.SafeDispose();
            newReceiptSub = (IDisposable) null;
            delMsgSub.SafeDispose();
            delMsgSub = (IDisposable) null;
            msgUpdateSub.SafeDispose();
            msgUpdateSub = (IDisposable) null;
          }
        });
      }));
    }

    protected void NotifyEmptied()
    {
      if (this.Emptied == null)
        return;
      this.Emptied((object) this, new EventArgs());
    }

    private void OnOutgoingStatusMessageDeleted(Message msg)
    {
      if (!msg.IsStatus() || !msg.KeyFromMe)
        return;
      this.trackedMsgIds.Remove(msg.MessageID);
      if (this.listSrc == null)
        return;
      this.listSrc.RemoveWhere<StatusItemViewModel>((Func<StatusItemViewModel, bool>) (item => item.Status.MessageId == msg.MessageID));
      if (this.listSrc.Any<StatusItemViewModel>())
        return;
      this.NotifyEmptied();
    }

    private void OnNewOutgoingStatusMessage(Message msg)
    {
      if (!msg.IsStatus() || !msg.KeyFromMe)
        return;
      this.trackedMsgIds.Add(msg.MessageID);
      if (this.listSrc == null)
        return;
      WaStatus status = (WaStatus) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => status = db.GetWaStatus(Settings.MyJid, msg.MessageID)));
      this.listSrc.Insert(0, new StatusItemViewModel(status));
      if (!this.showLimit.HasValue || this.listSrc.Count <= this.showLimit.Value)
        return;
      while (this.listSrc.Count > this.showLimit.Value)
        this.listSrc.RemoveAt(this.listSrc.Count - 1);
      this.ViewMorePanel.Visibility = Visibility.Visible;
    }

    private void OnMessageStatusChanged(Message msg)
    {
      if (!this.trackedMsgIds.Contains(msg.MessageID) || !msg.IsDeliveredToServer())
        return;
      ObservableCollection<StatusItemViewModel> listSrc = this.listSrc;
      (listSrc != null ? listSrc.FirstOrDefault<StatusItemViewModel>((Func<StatusItemViewModel, bool>) (item =>
      {
        int? messageId1 = item.Status?.MessageId;
        int messageId2 = msg.MessageID;
        return messageId1.GetValueOrDefault() == messageId2 && messageId1.HasValue;
      })) : (StatusItemViewModel) null)?.Refresh();
    }

    private void OnNewReadReceipt(ReceiptState rs)
    {
      if (!this.trackedMsgIds.Contains(rs.MessageId))
        return;
      ObservableCollection<StatusItemViewModel> listSrc = this.listSrc;
      (listSrc != null ? listSrc.FirstOrDefault<StatusItemViewModel>((Func<StatusItemViewModel, bool>) (item =>
      {
        int? messageId1 = item.Status?.MessageId;
        int messageId2 = rs.MessageId;
        return messageId1.GetValueOrDefault() == messageId2 && messageId1.HasValue;
      })) : (StatusItemViewModel) null)?.Refresh();
    }

    private void Item_Tap(object sender, GestureEventArgs e)
    {
      WaStatus status = (sender is StatusItemControl statusItemControl ? statusItemControl.ViewModel : (JidItemViewModel) null) is StatusItemViewModel viewModel ? viewModel.Status : (WaStatus) null;
      if (status == null)
        return;
      WaStatusViewPage.Start((WaStatusThread) new SingleWaStatusThread(status), (WaStatusThread[]) null, false);
    }

    private void ViewMore_Tap(object sender, GestureEventArgs e) => OwnStatusesPage.Start();

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
      StatusItemViewModel vm = (sender as FrameworkElement)?.DataContext as StatusItemViewModel;
      if (vm == null)
        return;
      UIUtils.Decision(AppResources.DeleteStatusV3ConfirmBody, AppResources.Delete, AppResources.Cancel, AppResources.Delete).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (toDelete =>
      {
        if (!toDelete)
          return;
        if (this.listSrc != null)
        {
          this.listSrc.Remove(vm);
          if (!this.listSrc.Any<StatusItemViewModel>())
            this.NotifyEmptied();
        }
        if (vm.Status == null)
          return;
        Action postDbAction = (Action) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          db.RevokeStatusOnSubmit(vm.Status, out postDbAction);
          db.SubmitChanges();
        }));
        Action action = postDbAction;
        if (action == null)
          return;
        action();
      }));
    }

    private void Forward_Click(object sender, RoutedEventArgs e)
    {
      StatusItemViewModel vm = (sender as FrameworkElement)?.DataContext as StatusItemViewModel;
      if (vm?.Status == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message messageById = db.GetMessageById(vm.Status.MessageId);
        if (messageById == null)
          return;
        SendMessage.ChooseRecipientAndForwardExisting(new Message[1]
        {
          messageById
        }, true);
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/OwnStatusList.xaml", UriKind.Relative));
      this.StatusList = (LongListSelector) this.FindName("StatusList");
      this.ViewMorePanel = (StackPanel) this.FindName("ViewMorePanel");
      this.ViewMoreBlock = (TextBlock) this.FindName("ViewMoreBlock");
      this.BottomTooltipBlock = (TextBlock) this.FindName("BottomTooltipBlock");
    }
  }
}
