// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactCallLogView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.CommonOps;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ContactCallLogView : UserControl
  {
    private string peerJid;
    private CallRecord groupCallRecord;
    private bool isShown;
    private bool initialLoadFinished;
    private IDisposable callRecordUpdatedSub;
    private List<CallRecordUpdate> pendingUpdates = new List<CallRecordUpdate>();
    private Dictionary<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>> grouped = new Dictionary<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>();
    private Dictionary<ContactCallLogView.TimeRanges, string> timeRangeStrMapping;
    internal DataTemplate GroupHeaderTemplate;
    internal ZoomBox RootZoomBox;
    internal LongListSelector CallList;
    internal StackPanel FooterPanel;
    internal TextBlock FooterInfoBlock;
    private bool _contentLoaded;

    public ContactCallLogView()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.CallList.IsFlatList = false;
      this.FooterInfoBlock.Text = AppResources.CallLogNoRecentCallsWithContact;
      this.timeRangeStrMapping = new Dictionary<ContactCallLogView.TimeRanges, string>()
      {
        {
          ContactCallLogView.TimeRanges.Today,
          AppResources.Today.ToLangFriendlyLower()
        },
        {
          ContactCallLogView.TimeRanges.Yesterday,
          AppResources.Yesterday.ToLangFriendlyLower()
        },
        {
          ContactCallLogView.TimeRanges.ThisWeek,
          AppResources.GroupingThisWeek.ToLangFriendlyLower()
        },
        {
          ContactCallLogView.TimeRanges.LastWeek,
          AppResources.CallLogLastWeek
        },
        {
          ContactCallLogView.TimeRanges.TwoWeeksAgo,
          AppResources.CallLog2WeekAgo
        },
        {
          ContactCallLogView.TimeRanges.ThreeWeeksAgo,
          AppResources.CallLog3WeekAgo
        },
        {
          ContactCallLogView.TimeRanges.LastMonth,
          AppResources.CallLogLastMonth
        },
        {
          ContactCallLogView.TimeRanges.Older,
          AppResources.CallLogOlder
        }
      };
    }

    public void Show(string jid) => this.Show(jid, (CallRecord) null);

    public void Show(string jid, CallRecord groupCallRecord)
    {
      if (this.isShown)
        return;
      this.peerJid = jid;
      this.groupCallRecord = groupCallRecord;
      this.isShown = true;
      DateTime cachedTimesNow = DateTime.Now;
      this.grouped.Clear();
      this.FooterPanel.Visibility = groupCallRecord == null ? Visibility.Visible : Visibility.Collapsed;
      CallRecord[] callRecordArray;
      if (groupCallRecord == null)
        callRecordArray = CallLog.Load(jid);
      else
        callRecordArray = new CallRecord[1]
        {
          groupCallRecord
        };
      CallRecord[] callRecords = callRecordArray;
      this.callRecordUpdatedSub = CallLog.GetUpdateObservable().SubscribeOn<CallRecordUpdate[]>((IScheduler) AppState.Worker).ObserveOnDispatcher<CallRecordUpdate[]>().Subscribe<CallRecordUpdate[]>(new Action<CallRecordUpdate[]>(this.OnCallRecordUpdated));
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        this.grouped = ((IEnumerable<CallRecord>) callRecords).Select<CallRecord, ContactCallLogItemViewModel>((Func<CallRecord, ContactCallLogItemViewModel>) (cr => new ContactCallLogItemViewModel(cr))).GroupBy<ContactCallLogItemViewModel, ContactCallLogView.TimeRanges>((Func<ContactCallLogItemViewModel, ContactCallLogView.TimeRanges>) (vm => ContactCallLogView.GetGroupingKey(vm.CallRecord, cachedTimesNow))).ToDictionary<IGrouping<ContactCallLogView.TimeRanges, ContactCallLogItemViewModel>, ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>((Func<IGrouping<ContactCallLogView.TimeRanges, ContactCallLogItemViewModel>, ContactCallLogView.TimeRanges>) (ig => ig.Key), (Func<IGrouping<ContactCallLogView.TimeRanges, ContactCallLogItemViewModel>, KeyedObservableCollection<string, ContactCallLogItemViewModel>>) (ig => new KeyedObservableCollection<string, ContactCallLogItemViewModel>(this.GetTimeRangeStr(ig.Key), (IEnumerable<ContactCallLogItemViewModel>) ig)));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.UpdateFooterVisibility();
          this.initialLoadFinished = true;
          this.ProcessPendingCallRecordUpdates();
          this.UpdateList();
        }));
      }));
    }

    private void UpdateList()
    {
      this.CallList.ItemsSource = (IList) this.grouped.OrderBy<KeyValuePair<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>, ContactCallLogView.TimeRanges>((Func<KeyValuePair<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>, ContactCallLogView.TimeRanges>) (p => p.Key)).Select<KeyValuePair<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>, KeyedObservableCollection<string, ContactCallLogItemViewModel>>((Func<KeyValuePair<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>, KeyedObservableCollection<string, ContactCallLogItemViewModel>>) (p => p.Value)).ToList<KeyedObservableCollection<string, ContactCallLogItemViewModel>>();
    }

    private string GetTimeRangeStr(ContactCallLogView.TimeRanges tr)
    {
      string str = (string) null;
      this.timeRangeStrMapping.TryGetValue(tr, out str);
      return str ?? AppResources.CallLogOlder;
    }

    private static ContactCallLogView.TimeRanges GetGroupingKey(CallRecord cr, DateTime localNow)
    {
      DateTime localTime = cr.StartTime.ToLocalTime();
      if (localTime.Year == localNow.Year && localTime.DayOfYear >= localNow.DayOfYear - 1)
        return localTime.DayOfYear != localNow.DayOfYear - 1 ? ContactCallLogView.TimeRanges.Today : ContactCallLogView.TimeRanges.Yesterday;
      TimeSpan timeSpan = localNow - localTime;
      if (timeSpan < TimeSpan.FromDays((double) (localNow.DayOfWeek + 1)))
        return ContactCallLogView.TimeRanges.ThisWeek;
      if (timeSpan < TimeSpan.FromDays((double) (localNow.DayOfWeek + 1 + 7)))
        return ContactCallLogView.TimeRanges.LastWeek;
      if (timeSpan < TimeSpan.FromDays((double) (localNow.DayOfWeek + 1 + 14)))
        return ContactCallLogView.TimeRanges.TwoWeeksAgo;
      if (timeSpan < TimeSpan.FromDays((double) (localNow.DayOfWeek + 1 + 21)))
        return ContactCallLogView.TimeRanges.ThreeWeeksAgo;
      return timeSpan < Constants.ThirtyOneDays || localTime.Year == localNow.Year && localTime.Month == localNow.Month - 1 || localTime.Month == 12 && localNow.Month == 1 && localTime.Year == localNow.Year - 1 ? ContactCallLogView.TimeRanges.LastMonth : ContactCallLogView.TimeRanges.Older;
    }

    private void UpdateFooterVisibility()
    {
      this.FooterInfoBlock.Visibility = (!this.grouped.Any<KeyValuePair<ContactCallLogView.TimeRanges, KeyedObservableCollection<string, ContactCallLogItemViewModel>>>()).ToVisibility();
    }

    private void ProcessPendingCallRecordUpdates()
    {
      CallRecordUpdate[] array = this.pendingUpdates.ToArray();
      this.pendingUpdates = (List<CallRecordUpdate>) null;
      this.OnCallRecordUpdated(array);
    }

    private void OnCallRecordUpdated(CallRecordUpdate[] updates)
    {
      if (!this.initialLoadFinished)
      {
        this.pendingUpdates.AddRange((IEnumerable<CallRecordUpdate>) updates);
      }
      else
      {
        bool flag = false;
        foreach (CallRecordUpdate update in updates)
        {
          if (update.UpdateType == DbDataUpdate.Types.Added && update.UpdatedCallRecord != null && update.UpdatedCallRecord.PeerJid == this.peerJid)
          {
            flag = true;
            ContactCallLogItemViewModel logItemViewModel = new ContactCallLogItemViewModel(update.UpdatedCallRecord);
            ContactCallLogView.TimeRanges groupingKey = ContactCallLogView.GetGroupingKey(update.UpdatedCallRecord, DateTime.Now);
            KeyedObservableCollection<string, ContactCallLogItemViewModel> observableCollection1 = (KeyedObservableCollection<string, ContactCallLogItemViewModel>) null;
            if (this.grouped.TryGetValue(groupingKey, out observableCollection1))
            {
              observableCollection1.Insert(0, logItemViewModel);
            }
            else
            {
              KeyedObservableCollection<string, ContactCallLogItemViewModel> observableCollection2 = new KeyedObservableCollection<string, ContactCallLogItemViewModel>(this.GetTimeRangeStr(groupingKey), (IEnumerable<ContactCallLogItemViewModel>) new ContactCallLogItemViewModel[1]
              {
                logItemViewModel
              });
              this.grouped[groupingKey] = observableCollection2;
            }
          }
        }
        if (!flag || this.groupCallRecord != null)
          return;
        this.UpdateList();
      }
    }

    private void CallList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ContactCallLogItemViewModel selectedItem = this.CallList.SelectedItem as ContactCallLogItemViewModel;
      this.CallList.SelectedItem = (object) null;
      if (selectedItem == null || selectedItem.CallRecord == null)
        return;
      List<CallRecord.CallLogEntryParticipant> participantEntries = selectedItem.CallRecord.ParticipantEntries;
      if ((participantEntries != null ? (!participantEntries.Any<CallRecord.CallLogEntryParticipant>() ? 1 : 0) : 1) == 0)
        return;
      CallContact.Call(selectedItem.CallRecord.PeerJid, context: "from contact page call log");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ContactCallLogView.xaml", UriKind.Relative));
      this.GroupHeaderTemplate = (DataTemplate) this.FindName("GroupHeaderTemplate");
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.CallList = (LongListSelector) this.FindName("CallList");
      this.FooterPanel = (StackPanel) this.FindName("FooterPanel");
      this.FooterInfoBlock = (TextBlock) this.FindName("FooterInfoBlock");
    }

    public enum TimeRanges
    {
      Today,
      Yesterday,
      ThisWeek,
      LastWeek,
      TwoWeeksAgo,
      ThreeWeeksAgo,
      LastMonth,
      Older,
    }
  }
}
