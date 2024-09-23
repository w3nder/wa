// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallHistoryList
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
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CallHistoryList : UserControl, IDisposable
  {
    private bool isShown;
    private bool isInitialLoadingFinished;
    private List<CallRecordUpdate> pendingUpdates = new List<CallRecordUpdate>();
    private ObservableCollection<CallLogItemViewModel> items = new ObservableCollection<CallLogItemViewModel>();
    private Dictionary<long, CallLogItemViewModel> callRecordToItemMapping = new Dictionary<long, CallLogItemViewModel>();
    private IDisposable callRecordUpdatedSub;
    private TextBlock tooltipBlock;
    internal Grid LayoutRoot;
    internal ProgressBar LoadingProgressBar;
    internal WhatsApp.CompatibilityShims.LongListSelector CallList;
    private bool _contentLoaded;

    public CallHistoryList()
    {
      this.InitializeComponent();
      this.CallList.ItemsSource = (IList) this.items;
    }

    public void Dispose()
    {
      this.callRecordUpdatedSub.SafeDispose();
      this.callRecordUpdatedSub = (IDisposable) null;
    }

    public void Show()
    {
      if (this.isShown)
        return;
      this.isShown = true;
      this.LoadingProgressBar.Visibility = Visibility.Visible;
      bool isProgressBarVisible = true;
      CallRecord[] sortedRecords = CallLog.Load();
      this.callRecordUpdatedSub = CallLog.GetUpdateObservable().SubscribeOn<CallRecordUpdate[]>((IScheduler) AppState.Worker).ObserveOnDispatcher<CallRecordUpdate[]>().Subscribe<CallRecordUpdate[]>(new Action<CallRecordUpdate[]>(this.OnCallRecordUpdated));
      CallRecord.CoalesceRecords(sortedRecords).SubscribeOn<CallRecord[]>(WAThreadPool.Scheduler).ObserveOnDispatcher<CallRecord[]>().Subscribe<CallRecord[]>((Action<CallRecord[]>) (calls =>
      {
        if (calls == null || !((IEnumerable<CallRecord>) calls).Any<CallRecord>())
          return;
        CallLogItemViewModel logItemViewModel = new CallLogItemViewModel(UserCache.Get(((IEnumerable<CallRecord>) calls).First<CallRecord>().PeerJid, true), calls);
        foreach (CallRecord call in calls)
          this.callRecordToItemMapping[call.CallRecordId] = logItemViewModel;
        this.items.Add(logItemViewModel);
        if (!isProgressBarVisible)
          return;
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
        isProgressBarVisible = false;
      }), (Action) (() =>
      {
        this.isInitialLoadingFinished = true;
        this.ProcessPendingUpdates();
        if (isProgressBarVisible)
        {
          this.LoadingProgressBar.Visibility = Visibility.Collapsed;
          isProgressBarVisible = false;
        }
        this.UpdateEmptyCallLogTooltipVisibility();
        this.items.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) => this.UpdateEmptyCallLogTooltipVisibility());
      }));
    }

    private void UpdateEmptyCallLogTooltipVisibility()
    {
      bool flag = !this.items.Any<CallLogItemViewModel>();
      if (flag && this.tooltipBlock == null)
      {
        TextBlock textBlock = new TextBlock();
        textBlock.Margin = new Thickness(0.0);
        textBlock.Visibility = Visibility.Collapsed;
        textBlock.TextWrapping = TextWrapping.Wrap;
        textBlock.Style = Application.Current.Resources[(object) "PhoneTextLargeStyle"] as Style;
        textBlock.Foreground = UIUtils.SubtleBrush;
        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
        textBlock.VerticalAlignment = VerticalAlignment.Top;
        this.tooltipBlock = textBlock;
        this.LayoutRoot.Children.Insert(0, (UIElement) this.tooltipBlock);
      }
      if (this.tooltipBlock == null)
        return;
      this.tooltipBlock.Visibility = flag.ToVisibility();
    }

    private void AddNewRecord(CallRecord call)
    {
      if (call == null)
        return;
      CallLogItemViewModel logItemViewModel = this.items.FirstOrDefault<CallLogItemViewModel>();
      bool flag = true;
      if (logItemViewModel != null)
        flag = !CallRecord.ShouldMerge(logItemViewModel.MostRecentCall, call);
      if (flag)
      {
        logItemViewModel = new CallLogItemViewModel(UserCache.Get(call.PeerJid, true), new CallRecord[1]
        {
          call
        });
        this.items.Insert(0, logItemViewModel);
      }
      else
      {
        logItemViewModel.AddCallRecord(call);
        logItemViewModel.Refresh();
      }
      this.callRecordToItemMapping[call.CallRecordId] = logItemViewModel;
    }

    private void DeleteRecord(long callRecordId)
    {
      CallLogItemViewModel logItemViewModel = (CallLogItemViewModel) null;
      if (this.callRecordToItemMapping.TryGetValue(callRecordId, out logItemViewModel) && logItemViewModel != null)
        logItemViewModel.RemoveCallRecord(callRecordId);
      this.callRecordToItemMapping.Remove(callRecordId);
      if (logItemViewModel == null || logItemViewModel.Calls.Any<CallRecord>())
        return;
      this.items.Remove(logItemViewModel);
    }

    public void DeleteAll()
    {
      if (!this.isInitialLoadingFinished)
        return;
      Observable.Return<bool>(true).Decision(AppResources.ConfirmDeleteAllCallsBody, AppResources.Delete, AppResources.Cancel, AppResources.ConfirmDeleteAllCallsTitle).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        this.items.Clear();
        CallLog.DeleteAll();
      }));
    }

    private void ProcessPendingUpdates()
    {
      CallRecordUpdate[] array = this.pendingUpdates.ToArray();
      this.pendingUpdates = (List<CallRecordUpdate>) null;
      this.OnCallRecordUpdated(array);
    }

    private void OnCallRecordUpdated(CallRecordUpdate[] updates)
    {
      if (!this.isInitialLoadingFinished)
      {
        this.pendingUpdates.AddRange((IEnumerable<CallRecordUpdate>) updates);
      }
      else
      {
        foreach (CallRecordUpdate update in updates)
        {
          switch (update.UpdateType)
          {
            case DbDataUpdate.Types.Added:
              this.AddNewRecord(update.UpdatedCallRecord);
              break;
            case DbDataUpdate.Types.Deleted:
              CallRecord updatedCallRecord = update.UpdatedCallRecord;
              if (updatedCallRecord != null)
              {
                this.DeleteRecord(updatedCallRecord.CallRecordId);
                break;
              }
              break;
          }
        }
      }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
      if (!(sender is MenuItem menuItem) || !(menuItem.Tag is CallLogItemViewModel tag))
        return;
      this.items.Remove(tag);
      CallLog.Delete(tag.Calls.ToArray(), false);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/CallHistoryList.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.CallList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("CallList");
    }
  }
}
