// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatCollection
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class ChatCollection : WaDisposable
  {
    private IDisposable convoUpdateSub_;
    private IDisposable messageUpdateSub;
    private IDisposable dbResetSub_;
    private ObservableCollection<ConversationItem> chats_;
    protected Func<Conversation, bool> shouldProcessFilter_;
    private bool enableUpdate = true;
    private LinkedList<Pair<Conversation, Message>> pendingConvoUpdates = new LinkedList<Pair<Conversation, Message>>();
    private Set<string> pendingConvoRemovals = new Set<string>();
    private string context;

    public Func<Conversation, bool> SkipAddingFilter { protected get; set; }

    public Func<MessagesContext, IEnumerable<Conversation>> GetAllFromDb { protected get; set; }

    public int Count => this.chats_.Count;

    public bool EnableUpdate
    {
      get => this.enableUpdate;
      set
      {
        if (this.enableUpdate == value)
          return;
        this.enableUpdate = value;
        if (!this.enableUpdate)
          return;
        this.ResumeConversationUpdate();
      }
    }

    public event EventHandler CollectionChanged;

    protected void NotifyCollectionChanged()
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, new EventArgs());
    }

    public ChatCollection(
      MessagesContext db,
      IEnumerable<ConversationItem> chats,
      Func<Conversation, bool> shouldProcessFilter,
      string context)
    {
      this.chats_ = new ObservableCollection<ConversationItem>(chats);
      this.shouldProcessFilter_ = shouldProcessFilter;
      this.SkipAddingFilter = (Func<Conversation, bool>) (c =>
      {
        if (c.IsArchived)
          return true;
        Conversation.ConversationStatus? status = c.Status;
        Conversation.ConversationStatus conversationStatus = Conversation.ConversationStatus.Deleting;
        return status.GetValueOrDefault() == conversationStatus && status.HasValue;
      });
      this.context = context;
      this.convoUpdateSub_ = db.UpdatedConversationSubject.Where<ConvoAndMessage>((Func<ConvoAndMessage, bool>) (p => this.ShouldProcess(p.Conversation))).ObserveOnDispatcher<ConvoAndMessage>().Subscribe<ConvoAndMessage>((Action<ConvoAndMessage>) (p => this.OnConversationUpdated(p.Conversation, p.LastMessage, p.PinStateUpdated, true)));
      this.messageUpdateSub = db.UpdatedMessageMediaWaTypeSubject.ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnMediaWaTypeUpdated));
      this.dbResetSub_ = AppState.DbResetSubject.ObserveOnDispatcherIfNeeded<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.OnDbReset()));
    }

    protected override void DisposeManagedResources()
    {
      this.convoUpdateSub_.SafeDispose();
      this.convoUpdateSub_ = (IDisposable) null;
      this.messageUpdateSub.SafeDispose();
      this.messageUpdateSub = (IDisposable) null;
      this.dbResetSub_.SafeDispose();
      this.dbResetSub_ = (IDisposable) null;
      base.DisposeManagedResources();
    }

    protected virtual void UpdateChatItem(ConversationItem ci)
    {
      ci.UpdateSubject.OnNext(new Unit());
    }

    public ObservableCollection<ConversationItem> GetItems() => this.chats_;

    public int GetItemIndex(string jid)
    {
      if (!string.IsNullOrEmpty(jid))
      {
        int num = 0;
        foreach (ConversationItem chat in (Collection<ConversationItem>) this.chats_)
        {
          if (!(chat.Jid == jid))
            ++num;
          else
            break;
        }
      }
      return -1;
    }

    protected bool ShouldProcess(Conversation convo)
    {
      if (convo == null)
        return false;
      return this.shouldProcessFilter_ == null || this.shouldProcessFilter_(convo);
    }

    protected bool ShouldSkipAdd(Conversation convo)
    {
      if (convo == null)
        return false;
      return this.SkipAddingFilter == null || this.SkipAddingFilter(convo);
    }

    public virtual void Append(ConversationItem ci) => this.chats_.Add(ci);

    protected virtual void InsertAt(int i, ConversationItem ci) => this.chats_.Insert(i, ci);

    protected virtual void Remove(string[] jids)
    {
      HashSet<string> jidsSet = new HashSet<string>((IEnumerable<string>) jids);
      this.chats_.RemoveWhere<ConversationItem>((Func<ConversationItem, bool>) (ci => jidsSet.Contains(ci.Jid)));
    }

    protected virtual bool RemoveAt(int i, ConversationItem ci, bool cleanup)
    {
      if (!(this.chats_.ElementAt<ConversationItem>(i).Jid == ci.Jid))
        return false;
      this.chats_.RemoveAt(i);
      return true;
    }

    protected void InsertChatInOrder(ConversationItem ci)
    {
      int count = this.chats_.Count;
      ConversationItem conversationItem = (ConversationItem) null;
      bool flag = false;
      for (int index = 0; index < count; ++index)
      {
        ConversationItem ci2 = this.chats_.ElementAt<ConversationItem>(index);
        if (ConversationItem.CompareBySortKeyAndTimestamp(ci, ci2) < 0)
        {
          if (conversationItem == null || conversationItem.Jid != ci.Jid)
            this.InsertAt(index, ci);
          flag = true;
          break;
        }
        conversationItem = ci2;
      }
      if (flag || conversationItem != null && !(conversationItem.Jid != ci.Jid))
        return;
      this.Append(ci);
    }

    private ChatCollection.DbResetState GetDbResetState(Conversation convo, Message msg = null)
    {
      bool? isRevoked = msg != null ? new bool?(msg.IsRevoked()) : new bool?();
      if (msg == null && convo.LastMessageID.HasValue)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Message messageById = db.GetMessageById(convo.LastMessageID.Value);
          if (messageById == null)
            Log.l(nameof (ChatCollection), "Missing last message for {0}", (object) convo.Jid);
          isRevoked = messageById != null ? new bool?(messageById.IsRevoked()) : new bool?();
        }));
      return new ChatCollection.DbResetState()
      {
        LastMessageId = msg != null ? new int?(msg.MessageID) : convo.LastMessageID,
        MuteExpiration = convo.MuteExpiration,
        SortKey = convo.SortKey,
        IsRevoked = isRevoked
      };
    }

    private bool ResetStateEqual(
      ChatCollection.DbResetState a,
      ChatCollection.DbResetState b,
      out bool pinStateUnchanged)
    {
      ref bool local = ref pinStateUnchanged;
      DateTime? nullable = a.SortKey;
      DateTime? sortKey = b.SortKey;
      int num1 = nullable.HasValue == sortKey.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == sortKey.GetValueOrDefault() ? 1 : 0) : 1) : 0;
      local = num1 != 0;
      int? lastMessageId1 = a.LastMessageId;
      int? lastMessageId2 = b.LastMessageId;
      int num2;
      if ((lastMessageId1.GetValueOrDefault() == lastMessageId2.GetValueOrDefault() ? (lastMessageId1.HasValue == lastMessageId2.HasValue ? 1 : 0) : 0) != 0)
      {
        DateTime? muteExpiration = a.MuteExpiration;
        nullable = b.MuteExpiration;
        if ((muteExpiration.HasValue == nullable.HasValue ? (muteExpiration.HasValue ? (muteExpiration.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          bool? isRevoked1 = a.IsRevoked;
          bool? isRevoked2 = b.IsRevoked;
          num2 = isRevoked1.GetValueOrDefault() == isRevoked2.GetValueOrDefault() ? (isRevoked1.HasValue == isRevoked2.HasValue ? 1 : 0) : 0;
          goto label_4;
        }
      }
      num2 = 0;
label_4:
      int num3 = pinStateUnchanged ? 1 : 0;
      return (num2 & num3) != 0;
    }

    private void OnDbReset()
    {
      Log.d("chats", "refresh on db reset");
      ObservableCollection<ConversationItem> chats = this.chats_;
      if (chats == null)
        return;
      Dictionary<string, ChatCollection.DbResetState> oldJids = new Dictionary<string, ChatCollection.DbResetState>();
      foreach (ConversationItem conversationItem in (Collection<ConversationItem>) chats)
      {
        if (oldJids.ContainsKey(conversationItem.Jid))
        {
          Log.l("chats", "jid {0} appeared twice!", (object) conversationItem.Jid);
          Log.SendCrashLog(new Exception("duplicate jid"), "duplicate jid check", logOnlyForRelease: true);
        }
        else
          oldJids.Add(conversationItem.Jid, this.GetDbResetState(conversationItem.Conversation, conversationItem.Message));
      }
      Func<MessagesContext, IEnumerable<Conversation>> loadFromDb = this.GetAllFromDb ?? (Func<MessagesContext, IEnumerable<Conversation>>) (db => (IEnumerable<Conversation>) db.GetConversations(new JidHelper.JidTypes[4]
      {
        JidHelper.JidTypes.User,
        JidHelper.JidTypes.Group,
        JidHelper.JidTypes.Broadcast,
        JidHelper.JidTypes.Psa
      }, true));
      Set<string> newJids = new Set<string>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (newDb =>
      {
        foreach (Conversation convo in loadFromDb(newDb))
        {
          if (this.ShouldProcess(convo) && !this.ShouldSkipAdd(convo))
          {
            newJids.Add(convo.Jid);
            bool pinStateUnchanged = false;
            ChatCollection.DbResetState a;
            if (!oldJids.TryGetValue(convo.Jid, out a) || !this.ResetStateEqual(a, this.GetDbResetState(convo), out pinStateUnchanged))
            {
              Message msg = (Message) null;
              int? lastMessageId = convo.LastMessageID;
              if (lastMessageId.HasValue)
              {
                MessagesContext messagesContext = newDb;
                lastMessageId = convo.LastMessageID;
                int id = lastMessageId.Value;
                msg = messagesContext.GetMessageById(id);
              }
              this.OnConversationUpdated(convo, msg, !pinStateUnchanged, false);
            }
          }
        }
      }));
      string[] array = oldJids.Where<KeyValuePair<string, ChatCollection.DbResetState>>((Func<KeyValuePair<string, ChatCollection.DbResetState>, bool>) (kv => !newJids.Contains(kv.Key))).Select<KeyValuePair<string, ChatCollection.DbResetState>, string>((Func<KeyValuePair<string, ChatCollection.DbResetState>, string>) (kv => kv.Key)).ToArray<string>();
      if (((IEnumerable<string>) array).Any<string>())
      {
        if (this.EnableUpdate)
          this.Remove(array);
        else
          this.pendingConvoRemovals.AddRange((IEnumerable<string>) array);
      }
      this.NotifyCollectionChanged();
    }

    private void ResumeConversationUpdate()
    {
      Pair<Conversation, Message>[] array1 = this.pendingConvoUpdates.ToArray<Pair<Conversation, Message>>();
      string[] array2 = this.pendingConvoRemovals.ToArray<string>();
      this.pendingConvoUpdates.Clear();
      this.pendingConvoRemovals.Clear();
      if (((IEnumerable<string>) array2).Any<string>())
      {
        this.Remove(array2);
        Log.l("chats", "processed {0} pending convo removal(s)", (object) array2.Length);
      }
      if (!((IEnumerable<Pair<Conversation, Message>>) array1).Any<Pair<Conversation, Message>>())
        return;
      foreach (Pair<Conversation, Message> pair in array1)
        this.OnConversationUpdated(pair.First, pair.Second, true, true);
      Log.l("chats", "processed {0} pending convo update(s)", (object) array1.Length);
    }

    private void OnMediaWaTypeUpdated(Message msg)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None)));
      Conversation conversation = convo;
      int? lastMessageId;
      int num;
      if (conversation == null)
      {
        num = 0;
      }
      else
      {
        lastMessageId = conversation.LastMessageID;
        num = lastMessageId.HasValue ? 1 : 0;
      }
      if (num == 0)
        return;
      lastMessageId = convo.LastMessageID;
      if (lastMessageId.Value != msg.MessageID)
        return;
      this.OnConversationUpdated(convo, msg, false, false);
    }

    private void OnConversationUpdated(
      Conversation convo,
      Message msg,
      bool pinStateUpdated,
      bool shouldNotifyChange)
    {
      if (convo == null)
        return;
      if (!this.EnableUpdate)
      {
        this.pendingConvoUpdates.AddLast(new Pair<Conversation, Message>(convo, msg));
      }
      else
      {
        int count = this.chats_.Count;
        ConversationItem ci = (ConversationItem) null;
        bool flag1 = false;
        bool flag2 = !this.ShouldSkipAdd(convo);
        for (int index = 0; index < count; ++index)
        {
          ConversationItem conversationItem = this.chats_.ElementAt<ConversationItem>(index);
          if (conversationItem.Conversation.ConversationID == convo.ConversationID)
          {
            if (flag2)
            {
              if (pinStateUpdated)
                flag1 = false;
              else if (convo.IsPinned())
                flag1 = true;
              else if (msg != null && !msg.IsNoteworthy() && !convo.HasFlag(Conversation.ConversationFlags.ResetDirty))
                flag1 = true;
              else if (count <= 1 || (index == count - 1 || ConversationItem.CompareByTimestamp(conversationItem, this.chats_.ElementAt<ConversationItem>(index + 1)) <= 0) && (index == 0 || ConversationItem.CompareByTimestamp(conversationItem, this.chats_.ElementAt<ConversationItem>(index - 1)) >= 0))
                flag1 = true;
            }
            if (!flag1)
              this.RemoveAt(index, conversationItem, !flag2);
            conversationItem.Message = msg;
            ci = conversationItem;
            break;
          }
        }
        if (flag1)
        {
          Log.d("chats", "in place update | jid: {0}", (object) convo.Jid);
          this.UpdateChatItem(ci);
        }
        else
        {
          if (ci == null && (msg != null || !string.IsNullOrEmpty(convo.ComposingText)))
            ci = new ConversationItem(convo)
            {
              Message = msg
            };
          if (ci != null & flag2)
          {
            this.InsertChatInOrder(ci);
            this.UpdateChatItem(ci);
          }
          if (!shouldNotifyChange)
            return;
          this.NotifyCollectionChanged();
        }
      }
    }

    private struct DbResetState
    {
      public int? LastMessageId;
      public DateTime? MuteExpiration;
      public DateTime? SortKey;
      public bool? IsRevoked;
    }
  }
}
