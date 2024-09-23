// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageLoader
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class MessageLoader : WaDisposable
  {
    private const int DbBatchCount = 100;
    private const string logHeaderStr = "msgloader";
    private string logHeader;
    private string jid_;
    private Conversation convo_;
    private Utils.FiboGenerator fiboGen_ = new Utils.FiboGenerator();
    private IMessageLoadingHandler msgListener_;
    private object mainLock_ = new object();
    private object listenerLock_ = new object();
    private IDisposable newMsgSub_;
    private IDisposable updatedMsgSub_;
    private IDisposable delMsgSub_;
    private IDisposable dbResetSub_;
    private IDisposable loadOlderSub;
    private IDisposable loadNewerSub;
    private bool isLoadingOlder_;
    private bool isLoadingNewer_;
    private bool selfRequestOlderOnMsgsLoaded_;
    private bool selfRequestNewerOnMsgsLoaded_;
    private bool isOldestLoaded_;
    private bool isOldestStaged_;
    private bool isLatestLoaded_;
    private bool isLatestStaged_;
    private bool isInitNotified_;
    private List<MessageLoader.Result> pendingResults_ = new List<MessageLoader.Result>();
    private const int InitialRespondBatchCount = 20;
    private int respondBatchCount_ = 20;
    private List<Message> inventoryNewer_;
    private List<Message> stagedNewer_ = new List<Message>();
    private List<Message> inventoryOlder_;
    private List<Message> stagedOlder_ = new List<Message>(100);
    private bool preloadAfterInit_ = true;
    private bool includeMiscInfo_ = true;
    private bool disposed_;
    private WorkQueue newMsgDelayWorker;
    private object delayedNewMsgCountLock_ = new object();
    private int delayedNewMsgCount_;

    private string LogHeader
    {
      get
      {
        return this.Jid != null ? (this.logHeader = string.Format("{0} {1}", (object) "msgloader", (object) this.Jid)) : "msgloader";
      }
    }

    public string Jid => this.jid_;

    public IMessageLoadingHandler MessageListener
    {
      set
      {
        lock (this.listenerLock_)
        {
          this.msgListener_ = this.msgListener_ == null ? value : throw new InvalidOperationException("up to 1 listener permitted");
          this.NotifyNolock(new MessageLoader.Result((Message) null, MessageLoader.Result.ResultType.None));
        }
      }
    }

    private int RespondBatchCount
    {
      get => Math.Min(this.respondBatchCount_, 100);
      set
      {
        if (AppState.IsLowMemoryDevice)
          return;
        this.respondBatchCount_ = Math.Min(value, 100);
      }
    }

    private MessageLoader(string jid) => this.jid_ = jid;

    public static MessageLoader Get(
      string jid,
      int? firstUnreadMsgId,
      int unreadCount,
      bool preloadAfterInit = true,
      bool includeMiscInfo = true,
      int? targetInitialLandingMsgId = null)
    {
      MessageLoader loader = new MessageLoader(jid);
      loader.preloadAfterInit_ = preloadAfterInit;
      loader.includeMiscInfo_ = includeMiscInfo;
      WAThreadPool.QueueUserWorkItem((Action) (() => loader.Init(firstUnreadMsgId, unreadCount, targetInitialLandingMsgId, false)));
      return loader;
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      lock (this.listenerLock_)
        this.msgListener_ = (IMessageLoadingHandler) null;
      lock (this.mainLock_)
      {
        this.DisposeSubscriptions();
        this.stagedNewer_.Clear();
        this.stagedOlder_.Clear();
        this.inventoryNewer_ = (List<Message>) null;
        this.inventoryOlder_ = (List<Message>) null;
        if (this.newMsgDelayWorker != null)
          this.newMsgDelayWorker.Stop();
        this.disposed_ = true;
      }
    }

    private void DisposeSubscriptions()
    {
      this.newMsgSub_.SafeDispose();
      this.newMsgSub_ = (IDisposable) null;
      this.delMsgSub_.SafeDispose();
      this.delMsgSub_ = (IDisposable) null;
      this.dbResetSub_.SafeDispose();
      this.dbResetSub_ = (IDisposable) null;
      this.loadNewerSub.SafeDispose();
      this.loadNewerSub = (IDisposable) null;
      this.loadOlderSub.SafeDispose();
      this.loadOlderSub = (IDisposable) null;
    }

    private int GetStagedCount() => this.stagedNewer_.Count + this.stagedOlder_.Count;

    private int GetInventoryCount(bool older)
    {
      return !older ? (this.inventoryNewer_ != null ? this.inventoryNewer_.Count : 0) : (this.inventoryOlder_ != null ? this.inventoryOlder_.Count : 0);
    }

    private bool IsLowInventory(bool older)
    {
      return this.GetInventoryCount(older) < this.RespondBatchCount;
    }

    private Conversation GetConversation(MessagesContext db, bool createIfNotFound, string context = null)
    {
      if (this.convo_ == null)
      {
        CreateResult result;
        this.convo_ = db.GetConversation(this.jid_, createIfNotFound ? CreateOptions.CreateIfNotFound : CreateOptions.None, out result);
        if (result == CreateResult.Created)
          Log.l(this.LogHeader, "created temp convo obj | jid:{0},context:{1}", (object) this.jid_, (object) (context ?? "n/a"));
      }
      return this.convo_;
    }

    public void ReInit(int? targetLandingMsgId)
    {
      lock (this.mainLock_)
      {
        if (this.disposed_)
          return;
        this.DisposeSubscriptions();
        this.fiboGen_ = new Utils.FiboGenerator();
        this.respondBatchCount_ = 20;
        this.isLoadingOlder_ = this.isLoadingNewer_ = this.selfRequestNewerOnMsgsLoaded_ = this.selfRequestOlderOnMsgsLoaded_ = this.isOldestLoaded_ = this.isOldestStaged_ = this.isLatestLoaded_ = this.isLatestStaged_ = false;
        this.inventoryNewer_ = this.inventoryOlder_ = (List<Message>) null;
        this.stagedNewer_.Clear();
        this.stagedOlder_.Clear();
      }
      this.Init(new int?(), 0, targetLandingMsgId, true);
    }

    private void Init(
      int? firstUnreadMsgId,
      int unreadCount,
      int? targetLandingMsgId,
      bool isReInit)
    {
      string logHeader = this.LogHeader;
      object[] objArray = new object[4]
      {
        (object) this.jid_,
        (object) (firstUnreadMsgId ?? -1),
        (object) unreadCount,
        null
      };
      int? nullable1 = targetLandingMsgId;
      objArray[3] = (object) (nullable1 ?? -1);
      Log.d(logHeader, "init | jid:{0},first unread:{1},unread count:{2},target:{3}", objArray);
      int? nullable2 = new int?();
      int unreadMsgObjsCount = 0;
      int? convoLastMsgId = new int?();
      List<Message> source = (List<Message>) null;
      lock (this.mainLock_)
      {
        if (this.disposed_)
          return;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Conversation conversation = this.GetConversation(db, true, "get initial msgs");
          Message[] messageArray = !targetLandingMsgId.HasValue ? MessageLoader.GetInitialMessages(db, conversation.Jid, conversation.MessageLoadingStart(), firstUnreadMsgId, unreadCount, 10, new int?(AppState.IsDecentMemoryDevice ? 2000 : 500), this.includeMiscInfo_, out unreadMsgObjsCount, out this.isOldestLoaded_) : MessageLoader.GetInitialMessagesWithTargetLanding(db, conversation.Jid, conversation.MessageLoadingStart(), targetLandingMsgId.Value, this.includeMiscInfo_, out this.isOldestLoaded_, out this.isLatestLoaded_);
          Message message = ((IEnumerable<Message>) messageArray).FirstOrDefault<Message>();
          convoLastMsgId = conversation.LastMessageID;
          this.isLatestLoaded_ = !convoLastMsgId.HasValue || message != null && message.MessageID >= convoLastMsgId.Value;
          this.PushInventory(true, messageArray);
          this.newMsgSub_.SafeDispose();
          this.newMsgSub_ = this.TakeNewMessageDelay<Message>(db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyRemoteJid == this.jid_))).ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnDbMessageInserted));
          this.delMsgSub_.SafeDispose();
          this.delMsgSub_ = db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyRemoteJid == this.jid_)).ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnDbMessageDeleted));
        }));
        this.dbResetSub_.SafeDispose();
        this.dbResetSub_ = AppState.DbResetSubject.Subscribe<Unit>((Action<Unit>) (_ => this.OnDbReset()));
        if (targetLandingMsgId.HasValue)
        {
          source = this.StageInventory(true, this.GetInventoryCount(true));
          this.isOldestStaged_ = this.isOldestLoaded_;
          this.isLatestStaged_ = this.isLatestLoaded_;
          int num = 0;
          foreach (Message message in source)
          {
            if (message.MessageID == targetLandingMsgId.Value)
            {
              nullable2 = new int?(num);
              break;
            }
            ++num;
          }
        }
        else if (unreadMsgObjsCount > 0 && unreadCount > 0)
        {
          source = this.StageInventory(true, Math.Max(unreadMsgObjsCount + 3, 10));
          Message message1 = source.FirstOrDefault<Message>();
          Message message2 = source.LastOrDefault<Message>();
          if (message1 != null && message2 != null && firstUnreadMsgId.HasValue)
          {
            nullable1 = firstUnreadMsgId;
            int messageId1 = message1.MessageID;
            if ((nullable1.GetValueOrDefault() <= messageId1 ? (nullable1.HasValue ? 1 : 0) : 0) != 0 && message2.MessageID < firstUnreadMsgId.Value)
            {
              Message message3 = source.Where<Message>((Func<Message, bool>) (m =>
              {
                int messageId2 = m.MessageID;
                int? nullable3 = firstUnreadMsgId;
                int valueOrDefault = nullable3.GetValueOrDefault();
                return (messageId2 >= valueOrDefault ? (nullable3.HasValue ? 1 : 0) : 0) != 0 && m.MediaWaType != FunXMPP.FMessage.Type.System;
              })).OrderBy<Message, int>((Func<Message, int>) (m => m.MessageID)).FirstOrDefault<Message>();
              if (message3 != null)
              {
                DateTime? nullable4 = new DateTime?();
                int index = source.IndexOf(message3) + 1;
                if (index >= 0 && index < source.Count)
                  nullable4 = source[index].FunTimestamp;
                bool flag = source.Any<Message>((Func<Message, bool>) (m =>
                {
                  int messageId3 = m.MessageID;
                  int? nullable5 = firstUnreadMsgId;
                  int valueOrDefault = nullable5.GetValueOrDefault();
                  return (messageId3 >= valueOrDefault ? (nullable5.HasValue ? 1 : 0) : 0) != 0 && !m.IsPtt();
                }));
                source.Insert(unreadMsgObjsCount, new Message()
                {
                  MediaWaType = FunXMPP.FMessage.Type.Divider,
                  Data = ((flag ? 1 : -1) * unreadCount).ToString(),
                  FunTimestamp = nullable4,
                  MessageID = -101
                });
                nullable2 = new int?(unreadMsgObjsCount);
              }
              else
              {
                Log.d(this.LogHeader, "Unexpectedly couldn't find first unread {0}, {1}, {2}", (object) message1.MessageID, (object) message2.MessageID, (object) firstUnreadMsgId.Value);
                Log.SendCrashLog(new Exception("First unread not found"), "First unread not found", logOnlyForRelease: true);
              }
            }
          }
        }
        else
          source = this.StageInventory(true, 10);
        Message message4 = source.FirstOrDefault<Message>();
        this.isLatestStaged_ = !convoLastMsgId.HasValue || message4 != null && message4.MessageID >= convoLastMsgId.Value;
      }
      this.Notify(new MessageLoader.Result(source.ToArray(), MessageLoader.Result.ResultType.Init)
      {
        TargetLandingIndex = nullable2,
        IsReInit = isReInit
      });
      if (!this.preloadAfterInit_)
        return;
      if (this.IsLowInventory(true))
      {
        Log.d(this.LogHeader, "preload older after init  | inventory:{0},next batch:{1}", (object) this.GetInventoryCount(true), (object) this.RespondBatchCount);
        this.selfRequestOlderOnMsgsLoaded_ = true;
        this.loadOlderSub.SafeDispose();
        this.loadOlderSub = this.GetLoadMoreObservable(true, 1500).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          this.loadOlderSub.SafeDispose();
          this.loadOlderSub = (IDisposable) null;
          this.OnMessagesLoadedFromDb(true);
        }));
      }
      if (!this.IsLowInventory(false))
        return;
      Log.d(this.LogHeader, "preload newer after init  | inventory:{0},next batch:{1}", (object) this.GetInventoryCount(false), (object) this.RespondBatchCount);
      this.selfRequestNewerOnMsgsLoaded_ = true;
      this.loadNewerSub.SafeDispose();
      this.loadNewerSub = this.GetLoadMoreObservable(false, 1500).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.loadNewerSub.SafeDispose();
        this.loadNewerSub = (IDisposable) null;
        this.OnMessagesLoadedFromDb(false);
      }));
    }

    private static Message[] GetInitialMessagesWithTargetLanding(
      MessagesContext db,
      string jid,
      int? loadingStart,
      int targetLanding,
      bool includeMiscInfo,
      out bool isOldestLoaded,
      out bool isLatestLoaded)
    {
      Message[] messagesAfter = db.GetMessagesAfter(jid, loadingStart, new int?(targetLanding), true, new int?(15), new int?(), includeMiscInfo, false);
      Message[] messagesBefore = db.GetMessagesBefore(jid, loadingStart, new int?(targetLanding), new int?(15), new int?(), includeMiscInfo);
      Message message1 = ((IEnumerable<Message>) db.GetMessagesAfter(jid, loadingStart, new int?(), true, new int?(1), new int?())).FirstOrDefault<Message>();
      Message message2 = ((IEnumerable<Message>) db.GetLatestMessages(jid, loadingStart, new int?(1), new int?())).FirstOrDefault<Message>();
      isOldestLoaded = ((IEnumerable<Message>) messagesBefore).Any<Message>() && (message1 == null || message1.MessageID >= ((IEnumerable<Message>) messagesBefore).LastOrDefault<Message>().MessageID);
      isLatestLoaded = ((IEnumerable<Message>) messagesAfter).Any<Message>() && (message2 == null || message2.MessageID <= ((IEnumerable<Message>) messagesAfter).FirstOrDefault<Message>().MessageID);
      return ((IEnumerable<Message>) messagesAfter).Concat<Message>((IEnumerable<Message>) messagesBefore).ToArray<Message>();
    }

    public static Message[] GetInitialMessages(
      MessagesContext db,
      string jid,
      int? loadingStart,
      int? firstUnreadMsgId,
      int unreadCount,
      int minToGet,
      int? maxToGet = null,
      bool includeMiscInfo = true)
    {
      int unreadMsgObjsCount = 0;
      bool isOldestLoaded = false;
      return MessageLoader.GetInitialMessages(db, jid, loadingStart, firstUnreadMsgId, unreadCount, minToGet, maxToGet, includeMiscInfo, out unreadMsgObjsCount, out isOldestLoaded);
    }

    public static Message[] GetInitialMessages(
      MessagesContext db,
      string jid,
      int? loadingStart,
      int? firstUnreadMsgId,
      int unreadCount,
      int minToGet,
      int? maxToGet,
      bool includeMiscInfo,
      out int unreadMsgObjsCount,
      out bool isOldestLoaded)
    {
      if (unreadCount < 0)
        unreadCount = 0;
      unreadMsgObjsCount = 0;
      isOldestLoaded = false;
      Func<int, int> func = (Func<int, int>) (n => 10 + (int) ((double) n * 1.1));
      bool flag1 = firstUnreadMsgId.HasValue && unreadCount > 0;
      bool flag2 = false;
      minToGet = Math.Max(10, minToGet);
      int n1 = flag1 ? Math.Max(minToGet, func(unreadCount)) : minToGet;
      if (maxToGet.HasValue && maxToGet.Value > minToGet && n1 > maxToGet.Value)
      {
        n1 = maxToGet.Value;
        flag2 = true;
      }
      int num1 = 0;
      LinkedList<Message[]> source1 = new LinkedList<Message[]>();
      Message[] source2 = MessageLoader.LoadLatestFromDb(db, jid, loadingStart, n1, 0, includeMiscInfo);
      if (source2.Length < n1)
        isOldestLoaded = true;
      Log.d("msgloader", "first batch | jid:{0},loading start:{1},msgs to get:{2},msgs got:{3}", (object) jid, (object) loadingStart, (object) n1, (object) source2.Length);
      source1.AddLast(source2);
      int offset = num1 + source2.Length;
      if (flag1 && ((IEnumerable<Message>) source2).Any<Message>())
      {
        while (((IEnumerable<Message>) source2).LastOrDefault<Message>().MessageID >= firstUnreadMsgId.Value)
        {
          if (isOldestLoaded | flag2)
          {
            unreadMsgObjsCount = offset;
            goto label_18;
          }
          else
          {
            Log.l("msgloader", "fetch more on init | unread count:{0},msgs got:{1},estimation:{2}", (object) unreadCount, (object) offset, (object) func(unreadCount));
            int n2 = 10;
            source2 = MessageLoader.LoadLatestFromDb(db, jid, loadingStart, n2, offset, includeMiscInfo);
            if (source2.Length < n2)
              isOldestLoaded = true;
            if (((IEnumerable<Message>) source2).Any<Message>())
            {
              source1.AddLast(source2);
              offset += source2.Length;
            }
          }
        }
        int num2 = 0;
        for (int index = source2.Length - 1; index >= 0 && source2[index].MessageID < firstUnreadMsgId.Value; --index)
          ++num2;
        unreadMsgObjsCount = offset - num2;
      }
label_18:
      return source1.Count <= 1 ? source1.First<Message[]>() : source1.SelectMany<Message[], Message>((Func<Message[], IEnumerable<Message>>) (batch => (IEnumerable<Message>) batch)).ToArray<Message>();
    }

    public void RequestNewerMessages() => this.RequestMessages(false);

    public void RequestOlderMessages() => this.RequestMessages(true);

    private void RequestMessages(bool older)
    {
      Message[] msgs = (Message[]) null;
      Action action = (Action) null;
      bool flag1 = false;
      bool flag2 = false;
      lock (this.mainLock_)
      {
        if (this.disposed_)
          return;
        if ((older ? (this.isOldestStaged_ ? 1 : 0) : (this.isLatestStaged_ ? 1 : 0)) != 0)
        {
          Log.d(this.LogHeader, "request msgs | skip | already staged towards {0}", older ? (object) "oldest" : (object) "latest");
          return;
        }
        lock (this.listenerLock_)
        {
          if (this.msgListener_ != null)
            this.msgListener_.OnMessagesRequestProcessing();
        }
        bool flag3 = older ? this.isOldestLoaded_ : this.isLatestLoaded_;
        if (!this.IsLowInventory(older) | flag3)
        {
          msgs = this.StageInventory(older, this.RespondBatchCount).ToArray();
          if (msgs.Length != 0)
            action = (Action) (() => this.Notify(new MessageLoader.Result(msgs, older ? MessageLoader.Result.ResultType.Older : MessageLoader.Result.ResultType.Newer)));
          else if (flag3)
          {
            Log.d(this.LogHeader, "request msg | done done done");
            if (older)
              this.isOldestStaged_ = true;
            else
              this.isLatestStaged_ = true;
            action = (Action) (() => this.Notify(new MessageLoader.Result(new Message[0], older ? MessageLoader.Result.ResultType.Older : MessageLoader.Result.ResultType.Newer)));
          }
          this.IncreaseNextRespondBatchCount();
          if (this.IsLowInventory(older))
          {
            Log.d(this.LogHeader, "preload more {0} | inv:{1}, next batch:{2}", older ? (object) nameof (older) : (object) "newer", (object) this.GetInventoryCount(older), (object) this.RespondBatchCount);
            flag2 = true;
          }
        }
        else
        {
          if (older)
            this.selfRequestOlderOnMsgsLoaded_ = true;
          else
            this.selfRequestNewerOnMsgsLoaded_ = true;
          if ((older ? (this.isLoadingOlder_ ? 1 : 0) : (this.isLoadingNewer_ ? 1 : 0)) == 0)
          {
            if (!flag3)
              flag1 = true;
          }
        }
      }
      if (action != null)
        action();
      lock (this.listenerLock_)
      {
        if (this.msgListener_ != null)
          this.msgListener_.OnMessagesRequestProcessed();
      }
      if (!(flag1 | flag2) || (older ? this.loadOlderSub : this.loadNewerSub) != null)
        return;
      IDisposable sub = (IDisposable) null;
      sub = this.GetLoadMoreObservable(older, flag1 ? 25 : 500).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        sub.SafeDispose();
        if (older)
          this.loadOlderSub = sub = (IDisposable) null;
        else
          this.loadNewerSub = sub = (IDisposable) null;
        this.OnMessagesLoadedFromDb(older);
      }));
      if (older)
        this.loadOlderSub = sub;
      else
        this.loadNewerSub = sub;
    }

    private IObservable<T> TakeNewMessageDelay<T>(IObservable<T> source)
    {
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (observer => source.Synchronize<T>(this.delayedNewMsgCountLock_).Subscribe<T>((Action<T>) (nextT =>
      {
        if (this.delayedNewMsgCount_ > 0)
        {
          --this.delayedNewMsgCount_;
          if (this.newMsgDelayWorker == null)
            this.newMsgDelayWorker = new WorkQueue(flags: WorkQueue.StartFlags.Unpausable, identifierString: "MsgDelayWorker");
          this.newMsgDelayWorker.Enqueue((Action) (() =>
          {
            observer.OnNext(nextT);
            Thread.Sleep(950);
          }));
          if (this.delayedNewMsgCount_ != 0)
            return;
          this.newMsgDelayWorker.Stop();
          this.newMsgDelayWorker = (WorkQueue) null;
        }
        else
          observer.OnNext(nextT);
      }), (Action<Exception>) (ex => observer.OnError(ex)), (Action) (() => observer.OnCompleted()))));
    }

    public void IncreaseDelayedNewMessageCount(int n)
    {
      lock (this.delayedNewMsgCountLock_)
        this.delayedNewMsgCount_ += n;
    }

    private void IncreaseNextRespondBatchCount()
    {
      if (this.RespondBatchCount >= 100)
        return;
      this.RespondBatchCount = 10 + this.fiboGen_.Next();
    }

    private IObservable<Unit> GetLoadMoreObservable(bool older, int delayInMs = 500)
    {
      Log.d(this.LogHeader, "load obs | direction:{0},delay:{1}", older ? (object) nameof (older) : (object) "newer", (object) delayInMs);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        bool flag1 = false;
        bool flag2 = false;
        lock (this.mainLock_)
        {
          bool flag3 = older ? this.isOldestLoaded_ : this.isLatestLoaded_;
          int num1 = older ? (this.isLoadingOlder_ ? 1 : 0) : (this.isLoadingNewer_ ? 1 : 0);
          flag2 = this.IsLowInventory(older);
          int num2 = flag3 ? 1 : 0;
          if ((num1 | num2) != 0 || !flag2)
            flag1 = true;
          else if (older)
            this.isLoadingOlder_ = true;
          else
            this.isLoadingNewer_ = true;
        }
        if (flag1)
        {
          if (!flag2)
            observer.OnNext(new Unit());
          observer.OnCompleted();
        }
        else
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds((double) delayInMs), (Action) (() =>
          {
            bool flag4 = false;
            lock (this.mainLock_)
            {
              flag4 = !older || !this.isLatestLoaded_ ? this.LoadByComparingId(older) : this.LoadOlderByOffset();
              if (older)
                this.isLoadingOlder_ = false;
              else
                this.isLoadingNewer_ = false;
            }
            if (flag4)
              observer.OnNext(new Unit());
            observer.OnCompleted();
          }));
        return (Action) (() => { });
      }));
    }

    private bool LoadByComparingId(bool older)
    {
      if ((older ? (this.isOldestLoaded_ ? 1 : 0) : (this.isLatestLoaded_ ? 1 : 0)) != 0 || !this.IsLowInventory(older))
        return false;
      Message[] msgs = (Message[]) null;
      int n = 100;
      Message message = this.GetInventoryCount(older) <= 0 ? (!older ? this.stagedNewer_.LastOrDefault<Message>() ?? this.stagedOlder_.FirstOrDefault<Message>() : this.stagedOlder_.LastOrDefault<Message>() ?? this.stagedNewer_.FirstOrDefault<Message>()) : (older ? this.inventoryOlder_.Last<Message>() : this.inventoryNewer_.Last<Message>());
      if (message == null)
        return false;
      int refMsgId = message.MessageID;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = this.GetConversation(db, false, "load more");
        if (older)
          msgs = db.GetMessagesBefore(conversation.Jid, conversation.MessageLoadingStart(), new int?(refMsgId), new int?(n), new int?(), this.includeMiscInfo_);
        else
          msgs = db.GetMessagesAfter(conversation.Jid, conversation.MessageLoadingStart(), new int?(refMsgId), false, new int?(n), new int?(), this.includeMiscInfo_);
      }));
      if (msgs == null || msgs.Length < n)
      {
        if (older)
          this.isOldestLoaded_ = true;
        else
          this.isLatestLoaded_ = true;
      }
      if (msgs != null)
      {
        this.PushInventory(older, msgs);
        Log.d(this.LogHeader, "loaded {0} {1} msgs | inventory:{2}", (object) msgs.Length, older ? (object) nameof (older) : (object) "newer", (object) this.GetInventoryCount(older));
      }
      return true;
    }

    private bool LoadOlderByOffset()
    {
      if (!this.isLatestLoaded_)
      {
        if (Settings.IsWaAdmin)
          throw new InvalidOperationException("can't calc offset");
        return false;
      }
      if (this.isOldestLoaded_ || !this.IsLowInventory(true))
        return false;
      int offset = this.GetStagedCount() + this.GetInventoryCount(true) + this.GetInventoryCount(false);
      Message[] msgs = (Message[]) null;
      int n = 100;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = this.GetConversation(db, false, "load more");
        msgs = MessageLoader.LoadLatestFromDb(db, conversation.Jid, conversation.MessageLoadingStart(), n, offset, this.includeMiscInfo_);
      }));
      if (msgs == null || msgs.Length < n)
        this.isOldestLoaded_ = true;
      if (msgs != null)
      {
        this.PushInventory(true, msgs);
        Log.d(this.LogHeader, "loaded {0} msgs | inventory:{1}", (object) msgs.Length, (object) this.GetInventoryCount(true));
      }
      return true;
    }

    private static Message[] LoadLatestFromDb(
      MessagesContext db,
      string jid,
      int? loadingStart,
      int n,
      int offset,
      bool includeMiscInfo)
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      Message[] latestMessages = db.GetLatestMessages(jid, loadingStart, new int?(n), new int?(offset), includeMiscInfo);
      PerformanceTimer.End(string.Format("msg loader: loaded {0}/{1} msgs | range {2} -> {3} | jid={4}", (object) latestMessages.Length, (object) n, (object) offset, (object) (offset + n), (object) jid), start);
      return latestMessages;
    }

    public string[] GetJidsMentionedMeRecently()
    {
      int num1 = 15;
      int num2 = this.stagedNewer_.Count - 1;
      string myJid = Settings.MyJid;
      Set<string> source = new Set<string>();
      while (num1 > 0 && num2 >= 0)
      {
        Message m = this.stagedNewer_[num2--];
        --num1;
        if (m.HasJidMentioned(myJid))
          source.Add(m.GetSenderJid());
      }
      int count = this.stagedOlder_.Count;
      int num3 = 0;
      while (num1 > 0 && num3 < count)
      {
        Message m = this.stagedOlder_[num3++];
        --num1;
        if (m.HasJidMentioned(myJid))
          source.Add(m.GetSenderJid());
      }
      return source.ToArray<string>();
    }

    private List<Message> StageInventory(bool older, int n)
    {
      List<Message> messageList = this.PopInventory(older, n);
      if (messageList.Any<Message>())
      {
        if (older)
          this.stagedOlder_.AddRange((IEnumerable<Message>) messageList);
        else
          this.stagedNewer_.AddRange((IEnumerable<Message>) messageList);
      }
      return messageList;
    }

    private List<Message> PopInventory(bool older, int n)
    {
      List<Message> messageList;
      if (this.GetInventoryCount(older) < n)
      {
        if (older)
        {
          messageList = this.inventoryOlder_;
          this.inventoryOlder_ = (List<Message>) null;
        }
        else
        {
          messageList = this.inventoryNewer_;
          this.inventoryNewer_ = (List<Message>) null;
        }
      }
      else if (older)
      {
        messageList = this.inventoryOlder_.Take<Message>(n).ToList<Message>();
        this.inventoryOlder_ = this.inventoryOlder_.Skip<Message>(n).ToList<Message>();
      }
      else
      {
        messageList = this.inventoryNewer_.Take<Message>(n).ToList<Message>();
        this.inventoryNewer_ = this.inventoryNewer_.Skip<Message>(n).ToList<Message>();
      }
      return messageList ?? new List<Message>();
    }

    private void PushInventory(bool older, Message[] msgs)
    {
      if (msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      if (older)
      {
        if (this.inventoryOlder_ == null)
          this.inventoryOlder_ = new List<Message>((IEnumerable<Message>) msgs);
        else
          this.inventoryOlder_.AddRange((IEnumerable<Message>) msgs);
      }
      else if (this.inventoryNewer_ == null)
        this.inventoryNewer_ = new List<Message>((IEnumerable<Message>) msgs);
      else
        this.inventoryNewer_.AddRange((IEnumerable<Message>) msgs);
    }

    private void Notify(MessageLoader.Result res)
    {
      lock (this.listenerLock_)
        this.NotifyNolock(res);
    }

    private void NotifyNolock(MessageLoader.Result res)
    {
      if (res.Type == MessageLoader.Result.ResultType.Init)
      {
        if (this.isInitNotified_)
        {
          Log.l(this.LogHeader, "re-init..");
          this.pendingResults_.Clear();
        }
        this.isInitNotified_ = true;
      }
      if (this.msgListener_ == null || !this.isInitNotified_)
        this.pendingResults_.Add(res);
      else if (this.pendingResults_.Any<MessageLoader.Result>())
      {
        Log.d(this.LogHeader, "flush pending results | count:{0}", (object) this.pendingResults_.Count);
        List<MessageLoader.Result> list = this.pendingResults_.ToList<MessageLoader.Result>();
        list.Add(res);
        this.pendingResults_.Clear();
        MessageLoader.Result res1 = list.Where<MessageLoader.Result>((Func<MessageLoader.Result, bool>) (r => r.Type == MessageLoader.Result.ResultType.Init)).FirstOrDefault<MessageLoader.Result>();
        if (res1 != null)
          list.Remove(res1);
        this.HandleResultByType(res1);
        foreach (MessageLoader.Result res2 in list)
          this.HandleResultByType(res2);
      }
      else
        this.HandleResultByType(res);
    }

    private void HandleResultByType(MessageLoader.Result res)
    {
      switch (res.Type)
      {
        case MessageLoader.Result.ResultType.Init:
          this.msgListener_.OnInitialMessages(res.Messages, res.TargetLandingIndex, res.IsReInit);
          break;
        case MessageLoader.Result.ResultType.Insert:
          foreach (Message message in res.Messages)
            this.msgListener_.OnMessageInsertion(message);
          break;
        case MessageLoader.Result.ResultType.Delete:
          foreach (Message message in res.Messages)
            this.msgListener_.OnMessageDeletion(message);
          break;
        case MessageLoader.Result.ResultType.Older:
          this.msgListener_.OnOlderMessages(res.Messages);
          break;
        case MessageLoader.Result.ResultType.Newer:
          this.msgListener_.OnNewerMessages(res.Messages);
          break;
        case MessageLoader.Result.ResultType.Reset:
          this.msgListener_.OnDbReset(res.Messages);
          break;
        case MessageLoader.Result.ResultType.Updated:
          this.msgListener_.OnUpdatedMessages(res.Messages);
          break;
      }
    }

    private void OnDbMessageInserted(Message m)
    {
      MessageLoader.Result res = (MessageLoader.Result) null;
      if (m.KeyFromMe)
      {
        lock (this.mainLock_)
        {
          if (this.isLatestStaged_)
          {
            this.stagedNewer_.Add(m);
            res = new MessageLoader.Result(m, MessageLoader.Result.ResultType.Insert);
          }
          else
            this.ReInit(new int?());
        }
      }
      else
      {
        lock (this.mainLock_)
        {
          if (this.isLatestStaged_)
          {
            this.stagedNewer_.Add(m);
            res = new MessageLoader.Result(m, MessageLoader.Result.ResultType.Insert);
          }
          else if (this.isLatestLoaded_)
            this.PushInventory(false, new Message[1]{ m });
        }
      }
      if (res == null)
        return;
      this.Notify(res);
    }

    private void OnDbMessageDeleted(Message m)
    {
      this.Notify(new MessageLoader.Result(m, MessageLoader.Result.ResultType.Delete));
      lock (this.mainLock_)
      {
        this.stagedNewer_.Remove(m);
        this.stagedOlder_.Remove(m);
        if (this.inventoryOlder_ != null)
          this.inventoryOlder_.Remove(m);
        if (this.inventoryNewer_ == null)
          return;
        this.inventoryNewer_.Remove(m);
      }
    }

    private void OnDbReset()
    {
      Message[] toInsert = (Message[]) null;
      Message[] source1 = (Message[]) null;
      lock (this.mainLock_)
      {
        source1 = this.stagedNewer_.Any<Message>() ? this.stagedNewer_.Concat<Message>((IEnumerable<Message>) this.stagedOlder_).ToArray<Message>() : this.stagedOlder_.ToArray();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          this.convo_ = (Conversation) null;
          Conversation conversation = this.GetConversation(db, true, "db reset");
          if (this.isLatestStaged_)
          {
            Message message = this.stagedNewer_.LastOrDefault<Message>() ?? this.stagedOlder_.FirstOrDefault<Message>();
            int? nullable1 = message == null ? new int?() : new int?(message.MessageID);
            int? nullable2 = nullable1;
            int? nullable3;
            if (conversation != null)
            {
              nullable3 = conversation.EffectiveFirstMessageID;
              if (nullable3.HasValue)
              {
                Conversation.ConversationStatus? status = conversation.Status;
                Conversation.ConversationStatus conversationStatus = Conversation.ConversationStatus.Clearing;
                if ((status.GetValueOrDefault() == conversationStatus ? (status.HasValue ? 1 : 0) : 0) != 0 && nullable1.HasValue)
                {
                  nullable3 = conversation.EffectiveFirstMessageID;
                  if (nullable3.Value > nullable1.Value)
                  {
                    ref int? local = ref nullable2;
                    nullable3 = conversation.EffectiveFirstMessageID;
                    int num = nullable3.Value - 1;
                    local = new int?(num);
                  }
                }
              }
            }
            MessagesContext messagesContext = db;
            string jid = conversation.Jid;
            int? loadingStart = conversation.MessageLoadingStart();
            int? lowerBoundMsgId = nullable2;
            nullable3 = new int?();
            int? limit = nullable3;
            nullable3 = new int?();
            int? offset = nullable3;
            FunXMPP.FMessage.Type? mediaType = new FunXMPP.FMessage.Type?();
            toInsert = messagesContext.GetMessagesAfter(jid, loadingStart, lowerBoundMsgId, false, limit, offset, true, mediaType: mediaType);
            if (toInsert == null || !((IEnumerable<Message>) toInsert).Any<Message>())
              return;
            this.stagedNewer_.AddRange((IEnumerable<Message>) toInsert);
          }
          else if (this.isLatestLoaded_)
          {
            this.isLatestLoaded_ = false;
            Log.l(this.LogHeader, "db reset | reset lastest loaded flag");
          }
          else
            Log.l(this.LogHeader, "db reset | skip");
        }));
      }
      this.Notify(new MessageLoader.Result(toInsert ?? new Message[0], MessageLoader.Result.ResultType.Reset));
      if (!((IEnumerable<Message>) source1).Any<Message>())
        return;
      LinkedList<Message> toUpdate = new LinkedList<Message>();
      LinkedList<Message> source2 = new LinkedList<Message>();
      foreach (Message m in source1)
      {
        if (m != null)
        {
          if (m.IsRevoked())
          {
            source2.AddLast(m);
          }
          else
          {
            if (m.KeyFromMe)
            {
              if ((m.IsPtt() ? (m.IsPlayedByTarget() ? 1 : 0) : (!m.IsReadByTarget() ? 0 : (m.MediaWaType != FunXMPP.FMessage.Type.Revoked ? 1 : 0))) != 0)
                continue;
            }
            else if (m.MediaWaType != FunXMPP.FMessage.Type.Undefined && m.MediaWaType != FunXMPP.FMessage.Type.ExtendedText && m.MediaWaType != FunXMPP.FMessage.Type.System && m.LocalFileUri != null)
              continue;
            toUpdate.AddLast(m);
          }
        }
      }
      if (toUpdate.Any<Message>())
        WAThreadPool.QueueUserWorkItem((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (Message message in toUpdate)
            db.GetMessageById(message.MessageID);
        }))));
      if (!source2.Any<Message>())
        return;
      this.Notify(new MessageLoader.Result(source2.ToArray<Message>(), MessageLoader.Result.ResultType.Updated));
    }

    private void OnMessagesLoadedFromDb(bool older)
    {
      Log.d(this.LogHeader, "{0} messages was loaded", older ? (object) nameof (older) : (object) "newer");
      bool flag = false;
      lock (this.mainLock_)
      {
        if (older)
        {
          flag = this.selfRequestOlderOnMsgsLoaded_;
          this.selfRequestOlderOnMsgsLoaded_ = false;
        }
        else
        {
          flag = this.selfRequestNewerOnMsgsLoaded_;
          this.selfRequestNewerOnMsgsLoaded_ = false;
        }
      }
      if (!flag)
        return;
      this.RequestMessages(older);
    }

    private class Result
    {
      public MessageLoader.Result.ResultType Type { get; private set; }

      public Message[] Messages { get; private set; }

      public int? TargetLandingIndex { get; set; }

      public bool IsReInit { get; set; }

      public Result(Message[] msgs, MessageLoader.Result.ResultType resType)
      {
        this.Messages = msgs;
        this.Type = resType;
      }

      public Result(Message msg, MessageLoader.Result.ResultType resType)
      {
        this.Messages = new Message[1]{ msg };
        this.Type = resType;
      }

      public enum ResultType
      {
        None,
        Init,
        Insert,
        Delete,
        Older,
        Newer,
        Reset,
        Updated,
      }
    }
  }
}
