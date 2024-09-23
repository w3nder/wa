// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallRecord
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class CallRecord
  {
    private DateTime startTime;
    private DateTime? connectTime;
    private DateTime endTime;

    public long CallRecordId { get; set; }

    public string PeerJid { get; set; }

    public bool FromMe { get; set; }

    public string CallId { get; set; }

    public DateTime StartTime
    {
      get => this.startTime;
      set => this.startTime = value.ToUniversalTime();
    }

    public DateTime? ConnectTime
    {
      get => this.connectTime;
      set
      {
        this.connectTime = value.HasValue ? new DateTime?(value.Value.ToUniversalTime()) : new DateTime?();
      }
    }

    public DateTime EndTime
    {
      get => this.endTime;
      set => this.endTime = value.ToUniversalTime();
    }

    public CallRecord.CallResult Result { get; set; }

    public long DataUsageTx { get; set; }

    public long DataUsageRx { get; set; }

    public bool? VideoCall { get; set; }

    public List<CallRecord.CallLogEntryParticipant> ParticipantEntries { get; set; }

    public TimeSpan Duration
    {
      get
      {
        if (this.ConnectTime.HasValue)
        {
          DateTime endTime1 = this.EndTime;
          DateTime? connectTime = this.ConnectTime;
          DateTime dateTime1 = connectTime.Value;
          if (endTime1 > dateTime1)
          {
            DateTime endTime2 = this.EndTime;
            connectTime = this.ConnectTime;
            DateTime dateTime2 = connectTime.Value;
            return endTime2 - dateTime2;
          }
        }
        return TimeSpan.FromSeconds(0.0);
      }
    }

    public long DataUsage => this.DataUsageRx + this.DataUsageTx;

    public static IObservable<CallRecord[]> CoalesceRecords(CallRecord[] sortedRecords)
    {
      return sortedRecords == null || !((IEnumerable<CallRecord>) sortedRecords).Any<CallRecord>() ? Observable.Empty<CallRecord[]>() : Observable.Create<CallRecord[]>((Func<IObserver<CallRecord[]>, Action>) (observer =>
      {
        List<CallRecord> source = new List<CallRecord>();
        CallRecord callRecord = (CallRecord) null;
        foreach (CallRecord sortedRecord in sortedRecords)
        {
          bool flag = false;
          if (callRecord == null)
            flag = true;
          else if (sortedRecord.IsGroupCall || callRecord.IsGroupCall)
            flag = false;
          else if (sortedRecord.PeerJid == callRecord.PeerJid && sortedRecord.Result == CallRecord.CallResult.Missed == (callRecord.Result == CallRecord.CallResult.Missed))
          {
            DateTime startTime = sortedRecord.StartTime;
            DateTime localTime1 = startTime.ToLocalTime();
            startTime = callRecord.StartTime;
            DateTime localTime2 = startTime.ToLocalTime();
            if (localTime1.Year == localTime2.Year && localTime1.Month == localTime2.Month && localTime1.Day == localTime2.Day)
              flag = true;
          }
          if (!flag)
          {
            CallRecord[] array = source.ToArray();
            source.Clear();
            observer.OnNext(array);
          }
          source.Add(sortedRecord);
          callRecord = sortedRecord;
        }
        if (source.Any<CallRecord>())
          observer.OnNext(source.ToArray());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public List<CallRecord.CallLogEntryParticipant> ParticipantEntriesSorted
    {
      get
      {
        List<CallRecord.CallLogEntryParticipant> participantEntriesSorted = new List<CallRecord.CallLogEntryParticipant>();
        List<CallRecord.CallLogEntryParticipant> participantEntries = this.ParticipantEntries;
        if ((participantEntries != null ? (__nonvirtual (participantEntries.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          participantEntriesSorted.Add(this.ParticipantEntries.First<CallRecord.CallLogEntryParticipant>());
          participantEntriesSorted.AddRange((IEnumerable<CallRecord.CallLogEntryParticipant>) this.ParticipantEntries.Skip<CallRecord.CallLogEntryParticipant>(1).OrderByDescending<CallRecord.CallLogEntryParticipant, bool>((Func<CallRecord.CallLogEntryParticipant, bool>) (p =>
          {
            UserStatus userStatus = UserCache.Get(p.jid, false);
            return userStatus != null && userStatus.IsInDevicePhonebook;
          })));
        }
        return participantEntriesSorted;
      }
    }

    public static bool ShouldMerge(CallRecord r0, CallRecord r1)
    {
      bool flag = false;
      if (r0 != null && r1 != null && !r0.IsGroupCall && !r1.IsGroupCall && r0.PeerJid == r1.PeerJid && r0.Result == CallRecord.CallResult.Missed == (r1.Result == CallRecord.CallResult.Missed))
      {
        DateTime startTime = r0.StartTime;
        DateTime localTime1 = startTime.ToLocalTime();
        startTime = r1.StartTime;
        DateTime localTime2 = startTime.ToLocalTime();
        if (localTime1.Year == localTime2.Year && localTime1.Month == localTime2.Month && localTime1.Day == localTime2.Day)
          flag = true;
      }
      return flag;
    }

    public bool IsGroupCall
    {
      get
      {
        List<CallRecord.CallLogEntryParticipant> participantEntries = this.ParticipantEntries;
        return participantEntries != null && participantEntries.Any<CallRecord.CallLogEntryParticipant>();
      }
    }

    public enum CallResult
    {
      Undefined,
      Connected,
      Missed,
      Declined,
      Canceled,
      Unavailable,
    }

    public struct CallLogEntryParticipant
    {
      public string jid;
      public CallRecord.CallResult res;

      public CallLogEntryParticipant(string jid, CallLogResult res)
      {
        this.jid = jid;
        switch (res)
        {
          case CallLogResult.Missed:
            this.res = CallRecord.CallResult.Missed;
            break;
          case CallLogResult.Unavailable:
            this.res = CallRecord.CallResult.Unavailable;
            break;
          case CallLogResult.Rejected:
            this.res = CallRecord.CallResult.Declined;
            break;
          case CallLogResult.Connected:
            this.res = CallRecord.CallResult.Connected;
            break;
          default:
            this.res = CallRecord.CallResult.Undefined;
            break;
        }
      }
    }
  }
}
