// Decompiled with JetBrains decompiler
// Type: WhatsApp.Conversation
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid", IsUnique = true)]
  [Index(Columns = "JidType", Name = "JidTypeIndex")]
  public class Conversation : PropChangingChangedBase
  {
    private string jid;
    private JidHelper.JidTypes jidType;
    private string composingText;
    private DateTime? timestamp;
    private string groupOwner;
    private DateTime? groupCreationT;
    private string groupSubject;
    private DateTime? groupSubjectT;
    private string groupSubjectOwner;
    private byte[] groupSubjectPerformanceHint;
    private int? lastMessageId;
    private int? effectiveFirstMessageId;
    private int? unreadMessageCount;
    private int? firstUnreadMessageID;
    private DateTime? muteExpiration;
    private bool isArchived;
    private Conversation.ConversationStatus? status;
    private Conversation.ConversationFlags? flags;
    private DateTime? automuteTimer;
    private DateTime? sortKey;
    private long modifyTag;
    private long unreadTileCount;
    private string participantsHash;
    private byte[] internalPropertiesProtoBuf;
    private string groupDescription;
    private DateTime? groupDescriptionT;
    private string groupDescriptionOwner;
    private string groupDescriptionId;
    private const int MaxModifyTag = 999999;
    private object participantsLock = new object();
    private volatile WhatsApp.GroupParticipants participants;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ConversationID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Jid
    {
      get => this.jid;
      set
      {
        if (!(this.jid != value))
          return;
        this.NotifyPropertyChanging(nameof (Jid));
        this.jid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public JidHelper.JidTypes JidType
    {
      get
      {
        if (this.jidType == JidHelper.JidTypes.Undefined)
        {
          this.jidType = JidHelper.GetJidType(this.Jid);
          Log.l("conversation", "undefined jid type for {0}", (object) (this.jid ?? "null"));
        }
        return this.jidType;
      }
      set
      {
        if (this.jidType == value)
          return;
        this.NotifyPropertyChanging(nameof (JidType));
        this.jidType = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ComposingText
    {
      get => this.composingText;
      set
      {
        if (!(this.composingText != value))
          return;
        this.NotifyPropertyChanging(nameof (ComposingText));
        this.composingText = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? Timestamp
    {
      get => this.timestamp;
      set
      {
        DateTime? nullable1 = value;
        if (nullable1.HasValue && nullable1.Value.Kind == DateTimeKind.Local)
          nullable1 = new DateTime?(nullable1.Value.ToUniversalTime());
        DateTime? timestamp = this.timestamp;
        DateTime? nullable2 = nullable1;
        if ((timestamp.HasValue == nullable2.HasValue ? (timestamp.HasValue ? (timestamp.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (Timestamp));
        this.timestamp = nullable1;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupOwner
    {
      get => this.groupOwner;
      set
      {
        if (!(this.groupOwner != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupOwner));
        this.groupOwner = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? GroupCreationT
    {
      get => this.groupCreationT;
      set
      {
        DateTime? groupCreationT = this.groupCreationT;
        DateTime? nullable = value;
        if ((groupCreationT.HasValue == nullable.HasValue ? (groupCreationT.HasValue ? (groupCreationT.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (GroupCreationT));
        this.groupCreationT = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupSubject
    {
      get => this.groupSubject;
      set
      {
        if (!(this.groupSubject != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupSubject));
        this.groupSubject = value;
        this.NotifyPropertyChanged(nameof (GroupSubject));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? GroupSubjectT
    {
      get => this.groupSubjectT;
      set
      {
        DateTime? groupSubjectT = this.groupSubjectT;
        DateTime? nullable = value;
        if ((groupSubjectT.HasValue == nullable.HasValue ? (groupSubjectT.HasValue ? (groupSubjectT.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (GroupSubjectT));
        this.groupSubjectT = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupSubjectOwner
    {
      get => this.groupSubjectOwner;
      set
      {
        if (!(this.groupSubjectOwner != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupSubjectOwner));
        this.groupSubjectOwner = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] GroupSubjectPerformanceHint
    {
      get => this.groupSubjectPerformanceHint;
      set
      {
        if (this.groupSubjectPerformanceHint == value)
          return;
        this.NotifyPropertyChanging(nameof (GroupSubjectPerformanceHint));
        this.groupSubjectPerformanceHint = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public string GroupParticipants { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? LastMessageID
    {
      get => this.lastMessageId;
      set
      {
        int? lastMessageId = this.lastMessageId;
        int? nullable = value;
        if ((lastMessageId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (lastMessageId.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (LastMessageID));
        this.lastMessageId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? EffectiveFirstMessageID
    {
      get => this.effectiveFirstMessageId;
      set
      {
        int? effectiveFirstMessageId = this.effectiveFirstMessageId;
        int? nullable = value;
        if ((effectiveFirstMessageId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (effectiveFirstMessageId.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (EffectiveFirstMessageID));
        this.effectiveFirstMessageId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? UnreadMessageCount
    {
      get => this.unreadMessageCount;
      set
      {
        this.NotifyPropertyChanging(nameof (UnreadMessageCount));
        this.unreadMessageCount = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? FirstUnreadMessageID
    {
      get => this.firstUnreadMessageID;
      set
      {
        this.NotifyPropertyChanging(nameof (FirstUnreadMessageID));
        this.firstUnreadMessageID = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public string PhotoID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? MuteExpiration
    {
      get => this.muteExpiration;
      set
      {
        DateTime? muteExpiration = this.muteExpiration;
        DateTime? nullable = value;
        if ((muteExpiration.HasValue == nullable.HasValue ? (muteExpiration.HasValue ? (muteExpiration.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MuteExpiration));
        this.muteExpiration = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsArchived
    {
      get => this.isArchived;
      set
      {
        if (this.isArchived == value)
          return;
        this.NotifyPropertyChanging(nameof (IsArchived));
        this.isArchived = value;
        this.NotifyPropertyChanged(nameof (IsArchived));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public Conversation.ConversationStatus? Status
    {
      get => this.status;
      set
      {
        Conversation.ConversationStatus? status = this.status;
        Conversation.ConversationStatus? nullable = value;
        if ((status.GetValueOrDefault() == nullable.GetValueOrDefault() ? (status.HasValue != nullable.HasValue ? 1 : 0) : 1) != 0)
        {
          this.NotifyPropertyChanging(nameof (Status));
          this.status = value;
        }
        this.NotifyPropertyChanged(nameof (Status));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public Conversation.ConversationFlags? Flags
    {
      get => this.flags;
      set
      {
        Conversation.ConversationFlags? flags = this.flags;
        Conversation.ConversationFlags? nullable = value;
        if ((flags.GetValueOrDefault() == nullable.GetValueOrDefault() ? (flags.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (Flags));
        this.flags = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? AutomuteTimer
    {
      get => this.automuteTimer;
      set
      {
        this.NotifyPropertyChanging(nameof (AutomuteTimer));
        this.automuteTimer = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Wallpaper { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? SortKey
    {
      get => this.sortKey;
      set
      {
        DateTime? sortKey = this.sortKey;
        DateTime? nullable = value;
        if ((sortKey.HasValue == nullable.HasValue ? (sortKey.HasValue ? (sortKey.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (SortKey));
        this.sortKey = value;
        this.NotifyPropertyChanged(nameof (SortKey));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long ModifyTag
    {
      get => this.modifyTag;
      set
      {
        if (this.modifyTag == value)
          return;
        this.NotifyPropertyChanging(nameof (ModifyTag));
        this.modifyTag = value;
        this.NotifyPropertyChanged(nameof (ModifyTag));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long UnreadTileCount
    {
      get => this.unreadTileCount;
      set
      {
        if (this.unreadTileCount == value)
          return;
        this.NotifyPropertyChanging(nameof (UnreadTileCount));
        this.unreadTileCount = Math.Max(value, 0L);
        this.NotifyPropertyChanged(nameof (UnreadTileCount));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ParticipantsHash
    {
      get
      {
        if (this.participantsHash == null)
          this.UpdateParticipantsHash();
        return this.participantsHash;
      }
      set
      {
        if (!(this.participantsHash != value))
          return;
        this.NotifyPropertyChanging(nameof (ParticipantsHash));
        this.participantsHash = value;
        this.NotifyPropertyChanged(nameof (ParticipantsHash));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] InternalPropertiesProtobuf
    {
      get => this.internalPropertiesProtoBuf;
      set
      {
        if (this.internalPropertiesProtoBuf == value)
          return;
        this.NotifyPropertyChanging(nameof (InternalPropertiesProtobuf));
        this.internalPropertiesProtoBuf = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupDescription
    {
      get => this.groupDescription;
      set
      {
        if (!(this.groupDescription != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupDescription));
        this.groupDescription = value;
        this.NotifyPropertyChanged(nameof (GroupDescription));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? GroupDescriptionT
    {
      get => this.groupDescriptionT;
      set
      {
        DateTime? groupDescriptionT = this.groupDescriptionT;
        DateTime? nullable = value;
        if ((groupDescriptionT.HasValue == nullable.HasValue ? (groupDescriptionT.HasValue ? (groupDescriptionT.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (GroupDescriptionT));
        this.groupDescriptionT = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupDescriptionOwner
    {
      get => this.groupDescriptionOwner;
      set
      {
        if (!(this.groupDescriptionOwner != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupDescriptionOwner));
        this.groupDescriptionOwner = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupDescriptionId
    {
      get => this.groupDescriptionId;
      set
      {
        if (!(this.groupDescriptionId != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupDescriptionId));
        this.groupDescriptionId = value;
      }
    }

    public ConversationProperties InternalProperties
    {
      get
      {
        return this.InternalPropertiesProtobuf == null ? (ConversationProperties) null : ConversationProperties.Deserialize(this.InternalPropertiesProtobuf);
      }
      set
      {
        this.InternalPropertiesProtobuf = value != null ? ConversationProperties.SerializeToBytes(value) : (byte[]) null;
      }
    }

    public bool SkipDeleteNotification { get; set; }

    public DateTime? LocalTimestamp
    {
      get
      {
        DateTime? timestamp = this.Timestamp;
        return !timestamp.HasValue ? new DateTime?() : new DateTime?(DateTimeUtils.FunTimeToPhoneTime(timestamp.Value));
      }
      private set
      {
        this.Timestamp = value.HasValue ? new DateTime?(DateTimeUtils.PhoneTimeToFunTime(value.Value)) : new DateTime?();
      }
    }

    public Conversation()
    {
    }

    public Conversation(string convoJid) => this.jid = convoJid;

    public static int CompareBySortKey(Conversation c1, Conversation c2)
    {
      if (c1 == null)
        return 1;
      return c2 == null ? -1 : Nullable.Compare<DateTime>(c2.SortKey, c1.SortKey);
    }

    public static int CompareByTimestamp(Conversation c1, Conversation c2)
    {
      if (c1 == null)
        return 1;
      return c2 == null ? -1 : Nullable.Compare<DateTime>(c2.Timestamp, c1.Timestamp);
    }

    public static int CompareByName(Conversation c1, Conversation c2)
    {
      if (c1 == null)
        return 1;
      return c2 == null ? -1 : string.Compare(c1.GroupSubject, c2.GroupSubject);
    }

    public long UpdateModifyTag()
    {
      long num = (long) new Random((int) DateTime.UtcNow.ToUnixTime()).Next(1, 999998);
      if (num >= this.ModifyTag)
        ++num;
      return this.ModifyTag = num;
    }

    public bool HasFlag(Conversation.ConversationFlags f)
    {
      int num = (int) f;
      return (num & ((int) this.Flags ?? 0)) == num;
    }

    public void SetFlag(Conversation.ConversationFlags f)
    {
      this.Flags = new Conversation.ConversationFlags?((Conversation.ConversationFlags) ((int) this.Flags ?? 0) | f);
    }

    public void ClearFlag(Conversation.ConversationFlags f)
    {
      this.Flags = new Conversation.ConversationFlags?((Conversation.ConversationFlags) ((int) this.Flags ?? 0) & ~f);
    }

    public bool IsLocked()
    {
      return JidHelper.IsGroupJid(this.Jid) && this.HasFlag(Conversation.ConversationFlags.Locked);
    }

    public void Lock() => this.SetFlag(Conversation.ConversationFlags.Locked);

    public void Unlock() => this.ClearFlag(Conversation.ConversationFlags.Locked);

    public bool IsAnnounceOnly()
    {
      return JidHelper.IsGroupJid(this.Jid) && this.HasFlag(Conversation.ConversationFlags.AnnounceOnly);
    }

    public bool IsAnnounceOnlyForUser()
    {
      return this.IsAnnounceOnly() && !this.UserIsAdmin(Settings.MyJid);
    }

    public void MakeAnnounceOnly() => this.SetFlag(Conversation.ConversationFlags.AnnounceOnly);

    public void MakeNotAnnounceOnly()
    {
      this.ClearFlag(Conversation.ConversationFlags.AnnounceOnly);
    }

    public bool IsConversationSeen() => this.HasFlag(Conversation.ConversationFlags.Seen);

    public void SetConversationSeen() => this.SetFlag(Conversation.ConversationFlags.Seen);

    public void SetConversationNotSeen() => this.ClearFlag(Conversation.ConversationFlags.Seen);

    public bool IsGroupParticipant()
    {
      bool r = false;
      if (!JidHelper.IsGroupJid(this.Jid))
        return false;
      if (this.HasFlag(Conversation.ConversationFlags.Deleted))
        r = false;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (set => r = set.ContainsKey(Settings.MyJid)));
      return r;
    }

    public bool IsReadOnly()
    {
      if (this.IsAnnounceOnlyForUser())
        return true;
      return JidHelper.IsGroupJid(this.Jid) && !this.IsGroupParticipant();
    }

    public int GetUnreadMessagesCount()
    {
      int? unreadMessageCount = this.UnreadMessageCount;
      return !unreadMessageCount.HasValue || unreadMessageCount.Value <= 0 ? 0 : unreadMessageCount.Value;
    }

    public bool IsMarkedAsUnread()
    {
      int? unreadMessageCount = this.UnreadMessageCount;
      return unreadMessageCount.HasValue && unreadMessageCount.Value < 0;
    }

    public bool IsRead()
    {
      int? unreadMessageCount = this.UnreadMessageCount;
      bool flag = !unreadMessageCount.HasValue || unreadMessageCount.Value == 0;
      if (!flag)
        Log.d("Convo", "Not read: {0} {1}", (object) this.Jid, unreadMessageCount.HasValue ? (object) unreadMessageCount.Value.ToString() : (object) "<null>");
      return flag;
    }

    public void ParticipantSetAction(Action<WhatsApp.GroupParticipants> callback)
    {
      WhatsApp.GroupParticipants grp = this.participants;
      WhatsApp.GroupParticipants groupParticipants1 = grp;
      if ((groupParticipants1 != null ? (groupParticipants1.NeedsReload ? 1 : 0) : 1) != 0)
        MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
        {
          WhatsApp.GroupParticipants participants = this.participants;
          if ((participants != null ? (participants.NeedsReload ? 1 : 0) : 1) != 0)
          {
            WhatsApp.GroupParticipants groupParticipants2 = new WhatsApp.GroupParticipants(db, this.Jid);
            Interlocked.MemoryBarrier();
            this.participants = groupParticipants2;
          }
          grp = this.participants;
        }));
      lock (this.participantsLock)
        callback(grp);
    }

    public int GetParticipantCount(bool excludingSelf = false)
    {
      int r = 0;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantSet => r = !excludingSelf || !participantSet.ContainsKey(Settings.MyJid) ? participantSet.Count : participantSet.Count - 1));
      return r;
    }

    public string[] GetParticipantJids(bool excludeSelf = false)
    {
      string[] r = (string[]) null;
      Func<string, bool> filter = (Func<string, bool>) (j => !excludeSelf || j != Settings.MyJid);
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantSet => r = participantSet.Keys.Where<string>(filter).ToArray<string>()));
      return r;
    }

    public string[] GetAdminJids(bool excludeSelf = false)
    {
      string[] r = (string[]) null;
      Func<string, bool> filter = (Func<string, bool>) (j =>
      {
        if (!this.UserIsAdmin(j))
          return false;
        return !excludeSelf || j != Settings.MyJid;
      });
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantSet => r = participantSet.Keys.Where<string>(filter).ToArray<string>()));
      return r;
    }

    public void UpdateParticipantsHash()
    {
      Log.l("Convo", "Updating participants hash");
      string newHash = (string) null;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantSet =>
      {
        using (SHA1Managed shA1Managed = new SHA1Managed())
        {
          foreach (string s in (IEnumerable<string>) participantSet.Keys.OrderBy<string, string>((Func<string, string>) (s => s)))
          {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            shA1Managed.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
          }
          shA1Managed.TransformFinalBlock(new byte[0], 0, 0);
          newHash = 1.ToString() + ":" + Convert.ToBase64String(shA1Managed.Hash).Substring(0, 8);
        }
      }));
      this.ParticipantsHash = newHash;
    }

    public Set<string> GetParticipantsFromHash(MessagesContext db, string participantsHash)
    {
      Set<string> participants = new Set<string>();
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantsSet =>
      {
        foreach (string key in participantsSet.Keys)
          participants.Add(key);
      }));
      if (participantsHash == this.ParticipantsHash)
        return participants;
      foreach (ParticipantsHashHistory participantsHashHistory in db.GetParticipantsHistory(this.Jid))
      {
        switch (participantsHashHistory.ParticipantAction)
        {
          case ParticipantsHashHistory.ParticipantActions.Added:
            participants.Remove(participantsHashHistory.ParticipantJid);
            break;
          case ParticipantsHashHistory.ParticipantActions.Removed:
            participants.Add(participantsHashHistory.ParticipantJid);
            break;
        }
        if (participantsHashHistory.OldHash == participantsHash)
          return participants;
      }
      return (Set<string>) null;
    }

    public bool UpdateParticipants(
      SqliteMessagesContext db,
      Dictionary<string, bool> toAdd,
      IEnumerable<string> toRemove,
      IEnumerable<string> adminsPromoted = null,
      IEnumerable<string> adminsDemoted = null)
    {
      bool dirty = false;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participantSet =>
      {
        if (toRemove != null)
        {
          foreach (string str in toRemove)
          {
            if (participantSet.ContainsKey(str))
            {
              participantSet.Remove(str);
              dirty = true;
              db.AddParticipantsHistory(this, str, ParticipantsHashHistory.ParticipantActions.Removed);
              LiveLocationManager.Instance.ProcessUserRemovedFromGroup(this.Jid, str);
            }
          }
        }
        if (toAdd != null)
        {
          foreach (KeyValuePair<string, bool> keyValuePair in toAdd)
          {
            string key = keyValuePair.Key;
            if (!participantSet.ContainsKey(key))
            {
              participantSet.Add(key, keyValuePair.Value);
              dirty = true;
              db.AddParticipantsHistory(this, key, ParticipantsHashHistory.ParticipantActions.Added);
            }
          }
        }
        if (adminsPromoted != null)
        {
          foreach (string jid in adminsPromoted)
          {
            if (!participantSet.IsAdmin(jid))
            {
              participantSet.SetAdmin(jid);
              dirty = true;
            }
          }
        }
        if (adminsDemoted != null)
        {
          foreach (string jid in adminsDemoted)
          {
            if (participantSet.IsAdmin(jid))
            {
              participantSet.RemoveAdmin(jid);
              dirty = true;
            }
          }
        }
        if (!dirty)
          return;
        participantSet.Flush();
      }));
      if (toRemove != null && toRemove.Any<string>())
        AppState.GetConnection().Encryption.OnParticipantRemovedFromConversation(this.Jid);
      return dirty;
    }

    public bool AddParticipant(MessagesContext db, string jid, bool isAdmin = false)
    {
      return this.UpdateParticipants((SqliteMessagesContext) db, new Dictionary<string, bool>()
      {
        {
          jid,
          isAdmin
        }
      }, (IEnumerable<string>) null);
    }

    public bool RemoveParticipant(MessagesContext db, string jid)
    {
      return this.UpdateParticipants((SqliteMessagesContext) db, (Dictionary<string, bool>) null, (IEnumerable<string>) new string[1]
      {
        jid
      });
    }

    public void ClearParticipants()
    {
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (participants =>
      {
        participants.Clear();
        participants.Flush();
      }));
    }

    public bool ContainsParticipant(string jid)
    {
      bool r = false;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (set => r = set.ContainsKey(jid)));
      return r;
    }

    public bool UserIsAdmin(string jid)
    {
      bool r = false;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (set => r = set.IsAdmin(jid)));
      return r;
    }

    public bool UserIsSuperAdmin(string jid)
    {
      bool r = false;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (set => r = set.IsSuperAdmin(jid)));
      return r;
    }

    public int GetAdminCount()
    {
      int r = 0;
      this.ParticipantSetAction((Action<WhatsApp.GroupParticipants>) (set => r = set.Admins.Count<string>()));
      return r;
    }

    public enum ConversationStatus
    {
      None,
      Deleting,
      Clearing,
      ArchivedObsolete,
    }

    public enum ConversationFlags
    {
      None = 0,
      Locked = 1,
      Deleted = 2,
      ResetDirty = 4,
      AnnounceOnly = 8,
      Seen = 16, // 0x00000010
    }
  }
}
