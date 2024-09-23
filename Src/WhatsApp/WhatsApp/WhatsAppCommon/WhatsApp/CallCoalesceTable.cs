// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallCoalesceTable
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public class CallCoalesceTable
  {
    private Set<string> keysToIgnore = new Set<string>();
    private Dictionary<string, CallCoalesceTable.Entry> entriesByKey = new Dictionary<string, CallCoalesceTable.Entry>();
    private List<CallCoalesceTable.Entry> allEntries = new List<CallCoalesceTable.Entry>();

    public static string KeyForCall(string jid, string callId)
    {
      return string.Format("<{0},{1}>", (object) jid, (object) callId);
    }

    private static string KeyForStruct(ISignalingStruct @struct)
    {
      return CallCoalesceTable.KeyForCall(@struct.GetPeerJid(), @struct.GetCallId());
    }

    public void Offer(
      ISignalingStruct @struct,
      DateTime? dt,
      int elapsedOfferMs,
      Action onComplete = null,
      Action onAccepted = null)
    {
      CallCoalesceTable.Entry entry = (CallCoalesceTable.Entry) null;
      string key = CallCoalesceTable.KeyForStruct(@struct);
      if (this.keysToIgnore.Contains(key))
      {
        if (onComplete == null)
          return;
        onComplete();
      }
      else
      {
        if (!this.entriesByKey.TryGetValue(key, out entry))
        {
          entry = new CallCoalesceTable.Entry();
          this.entriesByKey[key] = entry;
          this.allEntries.Add(entry);
        }
        if (dt.HasValue && !entry.Timestamp.HasValue)
          entry.Timestamp = dt;
        switch (@struct.GetMessageType())
        {
          case SignalingMessageType.Offer:
            entry.elaspedOfferMs = elapsedOfferMs;
            entry.HasVideo = @struct.GetVideoParams().VideoState == VideoState.Enabled;
            entry.Offered = true;
            break;
          case SignalingMessageType.Terminate:
            entry.Terminated = true;
            break;
        }
        if (onComplete != null)
          entry.OnCompleteCallbacks.Add(onComplete);
        entry.Stanzas.Add(new Pair<ISignalingStruct, Action>(@struct, onAccepted));
      }
    }

    private void Process(CallCoalesceTable.Entry e)
    {
      try
      {
        if (e.Offered && e.Terminated)
        {
          ISignalingStruct first = e.Stanzas.First<Pair<ISignalingStruct, Action>>().First;
          this.OnMissedCall(first.GetPeerJid(), first.GetCallId(), e.Timestamp, e.elaspedOfferMs, e.HasVideo);
        }
        else
          e.Stanzas.ForEach((Action<Pair<ISignalingStruct, Action>>) (s => VoipSignaling.OnIncomingSignalData(s.First, s.Second)));
      }
      finally
      {
        e.OnComplete();
      }
    }

    public void ProcessAll()
    {
      this.allEntries.ForEach(new Action<CallCoalesceTable.Entry>(this.Process));
      this.Clear();
    }

    public void OnMissedCall(
      string jid,
      string callId,
      DateTime? t,
      int elapsedMs,
      bool hasVideo)
    {
      this.keysToIgnore.Add(CallCoalesceTable.KeyForCall(jid, callId));
      VoipHandler.Instance.OnMissedCall(jid, callId, t.HasValue ? t.Value.ToFileTimeUtc() : 0L, elapsedMs, true, hasVideo);
    }

    public void Clear()
    {
      this.entriesByKey.Clear();
      this.allEntries.Clear();
      this.keysToIgnore.Clear();
    }

    private class Entry
    {
      public bool Offered;
      public bool Terminated;
      public bool HasVideo;
      public DateTime? Timestamp;
      public int elaspedOfferMs;
      public List<Pair<ISignalingStruct, Action>> Stanzas = new List<Pair<ISignalingStruct, Action>>();
      public List<Action> OnCompleteCallbacks = new List<Action>();

      public void OnComplete()
      {
        this.OnCompleteCallbacks.ForEach((Action<Action>) (a => a()));
        this.OnCompleteCallbacks.Clear();
      }
    }
  }
}
