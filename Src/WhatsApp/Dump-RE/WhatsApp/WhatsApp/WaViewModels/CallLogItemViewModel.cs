// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.CallLogItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class CallLogItemViewModel : UserViewModel
  {
    private Dictionary<string, IDisposable> picFetchSubs = new Dictionary<string, IDisposable>();
    private static bool? isInCall;
    private static IDisposable voipCallGlobalSub = (IDisposable) null;
    private static Subject<bool> voipCallGlobalSubject = new Subject<bool>();

    public List<CallRecord> Calls { get; private set; }

    public override object Model => (object) this.Calls;

    public override string Key
    {
      get
      {
        CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
        return string.Format("{0}-{1}", (object) base.Key, (object) (callRecord == null ? 0L : callRecord.StartTime.ToUnixTime()));
      }
    }

    public bool IsGroupCall
    {
      get
      {
        CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
        return callRecord != null && callRecord.IsGroupCall;
      }
    }

    public string CountStr
    {
      get
      {
        int recordsCount = this.RecordsCount;
        return recordsCount <= 1 ? "" : string.Format("({0})", (object) recordsCount);
      }
    }

    public string GroupCountStr
    {
      get
      {
        CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
        if (callRecord == null)
          return "";
        List<CallRecord.CallLogEntryParticipant> participantEntries = callRecord.ParticipantEntries;
        int num = participantEntries != null ? participantEntries.Count<CallRecord.CallLogEntryParticipant>() : 0;
        return num - 3 <= 0 ? "" : string.Format(", +{0}", (object) (num - 3));
      }
    }

    public List<string> ParticipantFirstNames
    {
      get
      {
        CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
        return callRecord != null ? callRecord.ParticipantEntriesSorted.Select<CallRecord.CallLogEntryParticipant, string>((Func<CallRecord.CallLogEntryParticipant, string>) (n => UserCache.Get(n.jid, false).GetDisplayName(true))).ToList<string>() : new List<string>();
      }
    }

    public override RichTextBlock.TextSet GetRichTitle()
    {
      if (!this.IsGroupCall)
        return base.GetRichTitle();
      string groupCallTitle = this.GetGroupCallTitle();
      return new RichTextBlock.TextSet()
      {
        Text = groupCallTitle,
        SerializedFormatting = LinkDetector.GetMatches(Emoji.ConvertToUnicode(groupCallTitle)),
        PartialFormattings = this.titleStrHighlights
      };
    }

    public List<System.Windows.Media.ImageSource> PictureSources
    {
      get
      {
        List<System.Windows.Media.ImageSource> imageSources = new List<System.Windows.Media.ImageSource>();
        if (this.IsGroupCall)
        {
          foreach (string str in this.Calls.Select<CallRecord, string>((Func<CallRecord, string>) (c => c.PeerJid)))
          {
            System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
            if (this.GetCachedPicSource(str, out cached))
              imageSources.Add(cached ?? this.GetDefaultPicture());
            else if (!this.picFetchSubs.ContainsKey(str))
              this.GetPictureSourceObservable(str, true, true).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (imgSrc => imageSources.Add(imgSrc)));
            imageSources.Add(this.GetDefaultPicture());
          }
          this.NotifyPropertyChanged(nameof (PictureSources));
          this.Notify(nameof (PictureSources), (object) imageSources);
        }
        else
          imageSources.Add(this.PictureSource);
        return imageSources;
      }
    }

    public string GetGroupCallTitle()
    {
      CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
      return callRecord != null && this.IsGroupCall ? string.Join(", ", callRecord.ParticipantEntriesSorted.Take<CallRecord.CallLogEntryParticipant>(3).Select<CallRecord.CallLogEntryParticipant, string>((Func<CallRecord.CallLogEntryParticipant, string>) (p => UserCache.Get(p.jid, true)?.GetDisplayName(true) ?? ""))) : "";
    }

    public override Brush SubtitleBrush
    {
      get
      {
        CallRecord callRecord = this.Calls.LastOrDefault<CallRecord>();
        return callRecord == null || callRecord.Result != CallRecord.CallResult.Missed ? UIUtils.SubtleBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public int RecordsCount => this.Calls.Count;

    public CallRecord MostRecentCall
    {
      get
      {
        return !this.Calls.Any<CallRecord>() ? (CallRecord) null : this.Calls.MaxOfFunc<CallRecord, DateTime>((Func<CallRecord, DateTime>) (r => r.StartTime));
      }
    }

    public CallLogItemViewModel(UserStatus user, CallRecord[] calls)
      : base(user, false)
    {
      this.Calls = ((IEnumerable<CallRecord>) calls).OrderByDescending<CallRecord, DateTime>((Func<CallRecord, DateTime>) (c => c.StartTime)).ToList<CallRecord>();
    }

    public void AddCallRecord(CallRecord call)
    {
      this.Calls.InsertInOrder<CallRecord>(call, (Func<CallRecord, CallRecord, bool>) ((r0, r1) => r0.StartTime > r1.StartTime));
    }

    public void RemoveCallRecord(long callRecordId)
    {
      this.Calls.RemoveWhere<CallRecord>((Func<CallRecord, bool>) (r => r.CallRecordId == callRecordId));
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      CallRecord callRecord = this.Calls.FirstOrDefault<CallRecord>();
      if (callRecord == null)
        return (RichTextBlock.TextSet) null;
      string str = string.Format("{0}, {1}", !callRecord.FromMe ? (callRecord.Result == CallRecord.CallResult.Missed ? (object) AppResources.CallLogItemMissedCall : (object) AppResources.CallLogItemIncoming) : (callRecord.Result == CallRecord.CallResult.Canceled ? (object) AppResources.CallLogItemCanceled : (object) AppResources.CallLogItemOutgoing), (object) DateTimeUtils.FormatCompact(callRecord.StartTime, DateTimeUtils.TimeDisplay.SameWeekOnly));
      return new RichTextBlock.TextSet() { Text = str };
    }

    public override IDisposable ActivateLazySubscriptions()
    {
      return (IDisposable) new DisposableChain(new IDisposable[2]
      {
        CallLogItemViewModel.voipCallGlobalSubject.Subscribe<bool>(new Action<bool>(this.Voip_CallStateChanged)),
        base.ActivateLazySubscriptions()
      });
    }

    public static bool IsInCall
    {
      get
      {
        if (!CallLogItemViewModel.isInCall.HasValue)
        {
          CallLogItemViewModel.voipCallGlobalSub = VoipHandler.CallStateChangedSubject.ObserveOnDispatcher<WaCallStateChangedArgs>().Subscribe<WaCallStateChangedArgs>(new Action<WaCallStateChangedArgs>(CallLogItemViewModel.Voip_GlobalCallStateChanged));
          CallLogItemViewModel.isInCall = new bool?(Voip.IsInCall);
        }
        return CallLogItemViewModel.isInCall.Value;
      }
      private set
      {
        bool? isInCall = CallLogItemViewModel.isInCall;
        bool flag = value;
        if ((isInCall.GetValueOrDefault() == flag ? (!isInCall.HasValue ? 1 : 0) : 1) == 0)
          return;
        CallLogItemViewModel.isInCall = new bool?(value);
        CallLogItemViewModel.voipCallGlobalSubject.OnNext(value);
      }
    }

    private static void Voip_GlobalCallStateChanged(WaCallStateChangedArgs state)
    {
      CallLogItemViewModel.IsInCall = state.CurrState != 0;
    }

    private void Voip_CallStateChanged(bool isInCall) => this.Refresh();
  }
}
