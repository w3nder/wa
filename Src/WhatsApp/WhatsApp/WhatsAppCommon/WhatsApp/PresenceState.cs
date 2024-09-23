// Decompiled with JetBrains decompiler
// Type: WhatsApp.PresenceState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class PresenceState
  {
    private static readonly TimeSpan IncomingComposingExpire = TimeSpan.FromSeconds(25.0);
    private static readonly TimeSpan OutgoingComposeRepeatInterval = TimeSpan.FromSeconds(10.0);
    private static PresenceState instance_;
    internal static object presenceLock = new object();
    internal static Dictionary<string, PresenceState.PresenceSubscriptionState> presenseStates = new Dictionary<string, PresenceState.PresenceSubscriptionState>();
    private PresenceState.Cache[] cachedChatStates = new PresenceState.Cache[2];
    private IDisposable blockListUpdateSub;
    private List<IDisposable> nextConnectionCallbacks = new List<IDisposable>();
    private object nextConnLock = new object();
    private Action<IDisposable> disposeAtNextConnection;

    public static PresenceState Instance
    {
      get
      {
        return Utils.LazyInit<PresenceState>(ref PresenceState.instance_, (Func<PresenceState>) (() => new PresenceState()));
      }
    }

    public void DisposeAtNextConnection(IDisposable d)
    {
      if (this.disposeAtNextConnection == null)
      {
        lock (this.nextConnLock)
        {
          if (this.disposeAtNextConnection == null)
          {
            this.nextConnectionCallbacks.Add(d);
            return;
          }
        }
      }
      this.disposeAtNextConnection(d);
    }

    public void SetDisposeAtNextConnectionCallback(Action<IDisposable> a)
    {
      lock (this.nextConnLock)
      {
        this.nextConnectionCallbacks.ForEach((Action<IDisposable>) (d => a(d)));
        this.nextConnectionCallbacks.Clear();
        this.disposeAtNextConnection = a;
      }
    }

    public IObservable<PresenceEventArgs> GetPresence(string jid, bool subscribe = true)
    {
      if (this.blockListUpdateSub == null)
        ContactsContext.Instance((Action<ContactsContext>) (db => this.blockListUpdateSub = db.BlockListUpdateSubject.ObserveOn<IEnumerable<string>>((IScheduler) AppState.Worker).Subscribe<IEnumerable<string>>((Action<IEnumerable<string>>) (_ => this.OnBlockListUpdated()))));
      return Observable.CreateWithDisposable<PresenceState.PresenceSubscriptionState>((Func<IObserver<PresenceState.PresenceSubscriptionState>, IDisposable>) (observer =>
      {
        IDisposable presence = (IDisposable) null;
        PresenceState.PresenceSubscriptionState subscriptionState = (PresenceState.PresenceSubscriptionState) null;
        PresenceState.PresenceSubscriptionState st;
        lock (PresenceState.presenceLock)
        {
          if (PresenceState.presenseStates.TryGetValue(jid, out subscriptionState))
          {
            st = subscriptionState;
            if (!subscriptionState.Subscribed)
              subscriptionState = (PresenceState.PresenceSubscriptionState) null;
          }
          else
          {
            st = new PresenceState.PresenceSubscriptionState(jid);
            PresenceState.presenseStates.Add(jid, st);
          }
          presence = st.Subscribe(observer);
          if (subscribe)
            st.Subscribed = true;
        }
        try
        {
          if (subscriptionState != null)
            observer.OnNext(subscriptionState);
        }
        catch (Exception ex)
        {
          presence.Dispose();
          observer.OnError(ex);
        }
        if (subscriptionState == null & subscribe)
          AppState.InvokeWhenConnected((Action<FunXMPP.Connection>) (conn =>
          {
            lock (PresenceState.presenceLock)
              this.DisposeAtNextConnection(st.Subscribe((IObserver<PresenceState.PresenceSubscriptionState>) new NoOpObserver<PresenceState.PresenceSubscriptionState>()));
            this.SubscribeToJid(jid, conn);
          }));
        return presence;
      })).Select<PresenceState.PresenceSubscriptionState, PresenceEventArgs>((Func<PresenceState.PresenceSubscriptionState, PresenceEventArgs>) (src => new PresenceEventArgs()
      {
        Jid = src.Jid,
        State = src.State,
        LastSeen = src.LastSeen,
        FormattedString = src.FormattedString
      })).DistinctUntilChanged<PresenceEventArgs>((Func<PresenceEventArgs, PresenceEventArgs, bool>) ((a, b) =>
      {
        bool presence = a.Jid == b.Jid && a.State == b.State && a.FormattedString == null == (b.FormattedString == null);
        if (presence && a.FormattedString != null)
          presence = a.FormattedString.Presence == b.FormattedString.Presence && a.FormattedString.PresenceString == b.FormattedString.PresenceString;
        return presence;
      }));
    }

    private void SubscribeToJid(string jid, FunXMPP.Connection conn = null)
    {
      if (conn == null)
        conn = AppState.ClientInstance.GetConnection();
      conn.SendPresenceSubscriptionRequest(jid);
    }

    public void OnLogin()
    {
      lock (PresenceState.presenceLock)
      {
        foreach (PresenceState.PresenceSubscriptionState subscriptionState in PresenceState.presenseStates.Values.Where<PresenceState.PresenceSubscriptionState>((Func<PresenceState.PresenceSubscriptionState, bool>) (st => st.Subscribed)))
        {
          this.DisposeAtNextConnection(subscriptionState.Subscribe((IObserver<PresenceState.PresenceSubscriptionState>) new NoOpObserver<PresenceState.PresenceSubscriptionState>()));
          this.SubscribeToJid(subscriptionState.Jid);
        }
      }
    }

    public void OnConnectionLost()
    {
      this.ClearCaches(false);
      this.DisposeTimer(PresenceState.ChatStateSource.Qr);
    }

    public void ClearCaches(bool clearSubscriptions = true)
    {
      lock (PresenceState.presenceLock)
      {
        foreach (PresenceState.PresenceSubscriptionState st in PresenceState.presenseStates.Values.ToArray<PresenceState.PresenceSubscriptionState>())
          this.Reset(st, false);
        if (!clearSubscriptions)
          return;
        PresenceState.presenseStates.Clear();
      }
    }

    private void Reset(PresenceState.PresenceSubscriptionState st, bool acquireLock = true)
    {
      st.State = Presence.Offline;
      st.LastSeen = new DateTime?();
      if (st.GroupMembers != null)
        st.GroupMembers.Clear();
      st.Notify(acquireLock);
      if (acquireLock)
      {
        lock (PresenceState.presenceLock)
          st.EndTimer();
      }
      else
        st.EndTimer();
    }

    public void ResetForUser(string jid, bool rejoin = true)
    {
      PresenceState.PresenceSubscriptionState st = (PresenceState.PresenceSubscriptionState) null;
      lock (PresenceState.presenceLock)
        PresenceState.presenseStates.TryGetValue(jid, out st);
      if (st == null)
        return;
      this.Reset(st);
      if (!rejoin)
        return;
      this.SubscribeToJid(jid);
    }

    private PresenceState.PresenceSubscriptionState GetValueOrCreateUnlocked(string jid)
    {
      PresenceState.PresenceSubscriptionState orCreateUnlocked;
      if (!PresenceState.presenseStates.TryGetValue(jid, out orCreateUnlocked))
        orCreateUnlocked = new PresenceState.PresenceSubscriptionState(jid);
      return orCreateUnlocked;
    }

    public void OnAvailable(string jid, bool isAvailable, DateTime? dt)
    {
      PresenceState.PresenceSubscriptionState st = (PresenceState.PresenceSubscriptionState) null;
      lock (PresenceState.presenceLock)
      {
        st = this.GetValueOrCreateUnlocked(jid);
        st.State = isAvailable ? Presence.Online : Presence.Offline;
        st.LastSeen = dt;
        bool flag = st.State != Presence.Offline;
        Log.d("presence", "pr:{0}, last:{1}", (object) st.State, (object) st.LastSeen);
        if (JidHelper.IsUserJid(jid))
        {
          if (ContactsContext.Instance<bool>((Func<ContactsContext, bool>) (db => JidHelper.IsJidBlocked(db, jid))))
            this.Reset(st, false);
          else if (!flag & isAvailable)
            this.GetCachedForJid(jid).ToList<PresenceState.Cache>().ForEach((Action<PresenceState.Cache>) (cache =>
            {
              string jid1 = jid;
              string participant = (string) null;
              if (jid1 != cache.Jid)
              {
                participant = jid1;
                jid1 = cache.Jid;
              }
              this.SendComposing(jid1, cache.Presence, true, participant);
            }));
        }
        else if (JidHelper.IsGroupJid(jid) && !isAvailable)
        {
          st.GroupMembers.Clear();
          st.Subscribed = false;
        }
        st.EndTimer();
      }
      st.Notify(true);
    }

    public void OnComposing(
      string jid,
      string participant,
      bool isComposing,
      FunXMPP.FMessage.Type mediaType = FunXMPP.FMessage.Type.Undefined)
    {
      PresenceState.PresenceSubscriptionState subscriptionState = (PresenceState.PresenceSubscriptionState) null;
      bool flag = false;
      if (participant != null && participant == Settings.MyJid)
        return;
      lock (PresenceState.presenceLock)
      {
        subscriptionState = this.GetValueOrCreateUnlocked(jid);
        subscriptionState.LastSeen = new DateTime?();
        Presence p = Presence.Online;
        if (isComposing)
        {
          p = mediaType == FunXMPP.FMessage.Type.Audio ? Presence.OnlineAndRecording : Presence.OnlineAndTyping;
          subscriptionState.StartTimer(participant);
        }
        else
          subscriptionState.EndTimer(participant);
        if (JidHelper.IsGroupJid(subscriptionState.Jid))
        {
          if (participant != null)
          {
            if (subscriptionState.GroupMembers != null)
            {
              if (p == Presence.Online || p == Presence.Offline)
                subscriptionState.GroupMembers.Remove(participant);
              else
                subscriptionState.GroupMembers[participant] = new PresenceWithTimestamp(p);
              flag = true;
            }
          }
        }
        else
        {
          subscriptionState.State = p;
          flag = true;
        }
      }
      if (subscriptionState == null || !flag)
        return;
      subscriptionState.Notify(true);
    }

    private void OnBlockListUpdated()
    {
      lock (PresenceState.presenceLock)
      {
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          foreach (KeyValuePair<string, PresenceState.PresenceSubscriptionState> presenseState in PresenceState.presenseStates)
          {
            if (JidHelper.IsJidBlocked(db, presenseState.Key))
              this.Reset(presenseState.Value, false);
          }
        }));
        foreach (KeyValuePair<string, PresenceState.PresenceSubscriptionState> presenseState in PresenceState.presenseStates)
          presenseState.Value.Notify(false);
      }
    }

    public void OnUserJoinedGroup(string gjid, string jid)
    {
      PresenceState.Cache cache = ((IEnumerable<PresenceState.Cache>) this.cachedChatStates).Where<PresenceState.Cache>((Func<PresenceState.Cache, bool>) (state => state != null && state.Jid == gjid)).FirstOrDefault<PresenceState.Cache>();
      if (cache == null)
        return;
      this.SendComposing(gjid, cache.Presence, true, jid);
    }

    private string MediaTypeForState(Presence p)
    {
      return p == Presence.OnlineAndRecording ? "audio" : (string) null;
    }

    public void SendComposing(
      string jid,
      Presence p = Presence.OnlineAndTyping,
      bool cached = false,
      string participant = null,
      PresenceState.ChatStateSource source = PresenceState.ChatStateSource.Client)
    {
      if (participant != null && participant == Settings.MyJid)
        return;
      if (!cached)
        this.CheckGroupSubscription(jid);
      AppState.ClientInstance.GetConnection().SendComposing(jid, this.MediaTypeForState(p), participant);
      if (cached)
        return;
      lock (PresenceState.presenceLock)
      {
        this.cachedChatStates[(int) source].SafeDispose();
        this.cachedChatStates[(int) source] = new PresenceState.Cache(jid, p);
      }
    }

    public void SendRecordingVoice(string jid)
    {
      this.SendComposing(jid, Presence.OnlineAndRecording);
    }

    public void SendPaused(string jid, bool cached = false, PresenceState.ChatStateSource source = PresenceState.ChatStateSource.Client)
    {
      lock (PresenceState.presenceLock)
      {
        if (!cached)
          AppState.ClientInstance.GetConnection().SendPaused(jid);
        this.cachedChatStates[(int) source].SafeDispose();
        this.cachedChatStates[(int) source] = (PresenceState.Cache) null;
      }
    }

    private IEnumerable<PresenceState.Cache> GetCachedForJid(string jid)
    {
      return ((IEnumerable<PresenceState.Cache>) this.cachedChatStates).Where<PresenceState.Cache>((Func<PresenceState.Cache, bool>) (cs => cs != null)).Where<PresenceState.Cache>((Func<PresenceState.Cache, bool>) (cs =>
      {
        PresenceState.Cache c = cs;
        if (c.Jid.IsGroupJid())
        {
          bool isMember = false;
          Action<GroupParticipants> onSet = (Action<GroupParticipants>) (set => isMember = set.ContainsKey(jid));
          if (cs.Convo == null)
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              cs.Convo = db.GetConversation(cs.Jid, CreateOptions.None);
              if (cs.Convo == null)
                return;
              cs.Convo.ParticipantSetAction(onSet);
            }));
          else
            cs.Convo.ParticipantSetAction(onSet);
          if (!isMember)
            cs = (PresenceState.Cache) null;
        }
        else if (c.Jid != jid || c.Presence == Presence.Offline)
          c = (PresenceState.Cache) null;
        return c != null;
      }));
    }

    public void CheckGroupSubscription(string jid)
    {
      if (!JidHelper.IsGroupJid(jid))
        return;
      this.GetPresence(jid).Take<PresenceEventArgs>(1).Subscribe<PresenceEventArgs>();
    }

    public void DisposeTimer(PresenceState.ChatStateSource source)
    {
      lock (PresenceState.presenceLock)
      {
        this.cachedChatStates[(int) source].SafeDispose();
        this.cachedChatStates[(int) source] = (PresenceState.Cache) null;
      }
    }

    private class Cache : IDisposable
    {
      public string Jid;
      public Presence Presence = Presence.Offline;
      public Conversation Convo;
      private bool wasDisposed;
      private IDisposable timerSub;
      private IDisposable newMessageSub;

      public Cache(string jid, Presence p)
      {
        PresenceState.Cache cache = this;
        this.Jid = jid;
        this.Presence = p;
        WorkQueue worker = AppState.Worker;
        Action<Action> callWithLock = (Action<Action>) (a =>
        {
          lock (PresenceState.presenceLock)
          {
            if (this.wasDisposed)
              return;
            a();
          }
        });
        this.timerSub = Observable.Return<Unit>(new Unit()).ObserveOn<Unit>((IScheduler) new DelayScheduler((IScheduler) worker, PresenceState.OutgoingComposeRepeatInterval)).Repeat<Unit>().Subscribe<Unit>((Action<Unit>) (_ => callWithLock(new Action(closure_0.OnTimerTick))));
        this.newMessageSub = MessagesContext.Events.NewMessagesSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyFromMe && msg.KeyRemoteJid == jid)).ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>((Action<Message>) (_ => callWithLock(new Action(closure_0.OnOutgoingMessage))));
      }

      private void OnTimerTick()
      {
        PresenceState.Instance.SendComposing(this.Jid, this.Presence, true);
      }

      private void OnOutgoingMessage() => PresenceState.Instance.SendPaused(this.Jid, true);

      public void Dispose()
      {
        this.wasDisposed = true;
        this.timerSub.SafeDispose();
        this.newMessageSub.SafeDispose();
      }
    }

    public enum ChatStateSource
    {
      Client,
      Qr,
    }

    public class PresenceSubscriptionState
    {
      public string Jid;
      public Presence State;
      public DateTime? LastSeen;
      public Dictionary<string, PresenceWithTimestamp> GroupMembers;
      private LinkedList<IObserver<PresenceState.PresenceSubscriptionState>> observers = new LinkedList<IObserver<PresenceState.PresenceSubscriptionState>>();
      internal bool Subscribed;
      private Dictionary<string, IDisposable> timers = new Dictionary<string, IDisposable>();

      public PresenceSubscriptionState(string jid)
      {
        this.Jid = jid;
        this.State = Presence.Offline;
        if (!JidHelper.IsGroupJid(jid))
          return;
        this.GroupMembers = new Dictionary<string, PresenceWithTimestamp>();
      }

      public ProcessedPresence FormattedString
      {
        get
        {
          if (JidHelper.IsUserJid(this.Jid))
            return PresenceState.PresenceSubscriptionState.ProcessIndividualPresence(this);
          return JidHelper.IsGroupJid(this.Jid) ? PresenceState.PresenceSubscriptionState.ProcessGroupPresence(this) : (ProcessedPresence) null;
        }
      }

      private static ProcessedPresence ProcessIndividualPresence(
        PresenceState.PresenceSubscriptionState pr)
      {
        string str1 = (string) null;
        string str2 = (string) null;
        switch (pr.State)
        {
          case Presence.Online:
            str1 = AppResources.Online;
            break;
          case Presence.OnlineAndTyping:
            str1 = AppResources.Typing;
            break;
          case Presence.OnlineAndRecording:
            str1 = AppResources.ComposingAudio;
            break;
          default:
            if (pr.LastSeen.HasValue)
            {
              DateTime dateTimeVal = DateTimeUtils.FunTimeToPhoneTime(pr.LastSeen.Value);
              if (dateTimeVal > DateTime.Now)
                dateTimeVal = DateTime.Now;
              str1 = string.Format(AppResources.LastSeen, (object) DateTimeUtils.FormatLastSeen(dateTimeVal));
              int length = AppResources.LastSeen.LastIndexOf("{0}");
              if (length > 0)
              {
                str2 = AppResources.LastSeen.Substring(0, length);
                break;
              }
              break;
            }
            break;
        }
        return new ProcessedPresence()
        {
          PresenceString = str1,
          Presence = pr.State,
          PresencePrefix = str2
        };
      }

      private static ProcessedPresence ProcessGroupPresence(
        PresenceState.PresenceSubscriptionState pr)
      {
        string str1 = (string) null;
        Presence presence = Presence.Offline;
        if (pr.GroupMembers.Count != 0)
        {
          Presence[] interesting = new Presence[2]
          {
            Presence.OnlineAndTyping,
            Presence.OnlineAndRecording
          };
          var data = pr.GroupMembers.Where<KeyValuePair<string, PresenceWithTimestamp>>((Func<KeyValuePair<string, PresenceWithTimestamp>, bool>) (kv => ((IEnumerable<Presence>) interesting).Contains<Presence>(kv.Value.Presence))).Select(kv => new
          {
            Jid = kv.Key,
            Presence = kv.Value.Presence,
            Timestamp = kv.Value.Timestamp
          }).MaxOfFunc(s => s.Timestamp);
          if (data != null)
          {
            string jid = data.Jid;
            presence = data.Presence;
            string format = (string) null;
            switch (presence)
            {
              case Presence.OnlineAndTyping:
                format = AppResources.UserIsTyping;
                break;
              case Presence.OnlineAndRecording:
                format = AppResources.UserIsRecording;
                break;
            }
            if (format != null)
            {
              UserStatus userStatus = UserCache.Get(jid, false);
              string str2 = userStatus == null ? JidHelper.GetPhoneNumber(jid, true) : userStatus.GetDisplayName(true);
              str1 = string.Format(format, (object) str2) + "…";
            }
          }
        }
        return new ProcessedPresence()
        {
          PresenceString = str1,
          Presence = presence
        };
      }

      internal IDisposable Subscribe(
        IObserver<PresenceState.PresenceSubscriptionState> observer)
      {
        LinkedListNode<IObserver<PresenceState.PresenceSubscriptionState>> node = this.observers.AddLast(observer);
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          lock (PresenceState.presenceLock)
          {
            if (node != null)
            {
              this.observers.Remove(node);
              node = (LinkedListNode<IObserver<PresenceState.PresenceSubscriptionState>>) null;
            }
            if (this.observers.Any<IObserver<PresenceState.PresenceSubscriptionState>>())
              return;
            PresenceState.presenseStates.Remove(this.Jid);
          }
        }));
      }

      internal void Notify(bool acquireLock)
      {
        IObserver<PresenceState.PresenceSubscriptionState>[] snapshot = (IObserver<PresenceState.PresenceSubscriptionState>[]) null;
        Action action = (Action) (() => snapshot = this.observers.ToArray<IObserver<PresenceState.PresenceSubscriptionState>>());
        if (acquireLock)
        {
          lock (PresenceState.presenceLock)
            action();
        }
        else
          action();
        foreach (IObserver<PresenceState.PresenceSubscriptionState> observer in snapshot)
        {
          try
          {
            observer.OnNext(this);
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
          }
        }
      }

      internal void StartTimer(string participant = null)
      {
        this.EndTimer(participant);
        bool cancelled = false;
        IDisposable disp = PooledTimer.Instance.Schedule(PresenceState.IncomingComposingExpire, (Action) (() =>
        {
          lock (PresenceState.presenceLock)
          {
            this.timers.Remove(participant ?? "");
            if (cancelled)
              return;
            this.OnTimerTick(participant);
          }
        }));
        DisposableAction disposableAction = new DisposableAction((Action) (() =>
        {
          cancelled = true;
          disp.Dispose();
        }));
        this.timers[participant ?? ""] = (IDisposable) disposableAction;
      }

      private void OnTimerTick(string participant = null)
      {
        Presence presence = Presence.Offline;
        if (participant == null)
        {
          presence = this.State;
        }
        else
        {
          PresenceWithTimestamp presenceWithTimestamp = (PresenceWithTimestamp) null;
          if (this.GroupMembers.TryGetValue(participant, out presenceWithTimestamp) && presenceWithTimestamp != null)
            presence = presenceWithTimestamp.Presence;
        }
        if (presence != Presence.OnlineAndTyping && presence != Presence.OnlineAndRecording)
          return;
        PresenceState.Instance.OnComposing(this.Jid, participant, false);
      }

      internal void EndTimer(string participant = null)
      {
        if (participant == null)
        {
          List<IDisposable> list = this.timers.Values.ToList<IDisposable>();
          this.timers.Clear();
          list.ForEach((Action<IDisposable>) (d_ => d_.Dispose()));
        }
        else
        {
          IDisposable d;
          if (!this.timers.TryGetValue(participant ?? "", out d))
            return;
          this.timers.Remove(participant ?? "");
          d.SafeDispose();
        }
      }
    }
  }
}
