// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupParticipants
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class GroupParticipants
  {
    private string LogHeader;
    private Dictionary<string, GroupParticipantState> users = new Dictionary<string, GroupParticipantState>();
    private Dictionary<string, bool> pendingAdditions = new Dictionary<string, bool>();
    private Dictionary<string, bool> pendingRemovals = new Dictionary<string, bool>();
    private Dictionary<string, bool> pendingElevations = new Dictionary<string, bool>();
    private Dictionary<string, bool> pendingDemotions = new Dictionary<string, bool>();
    private bool wasCleared;
    private int cacheVerison;
    private string gjid;
    private SqliteMessagesContext oldContext_;
    private bool loggedBefore;

    public IEnumerable<string> Keys
    {
      get
      {
        return this.users.Keys.Where<string>((Func<string, bool>) (jid => !this.pendingRemovals.ContainsKey(jid)));
      }
    }

    public IEnumerable<string> Admins
    {
      get
      {
        return this.users.Where<KeyValuePair<string, GroupParticipantState>>((Func<KeyValuePair<string, GroupParticipantState>, bool>) (kv => this.IsAdmin(kv.Value))).Select<KeyValuePair<string, GroupParticipantState>, string>((Func<KeyValuePair<string, GroupParticipantState>, string>) (kv => kv.Key));
      }
    }

    public IEnumerable<string> NonAdmins
    {
      get
      {
        return this.users.Where<KeyValuePair<string, GroupParticipantState>>((Func<KeyValuePair<string, GroupParticipantState>, bool>) (kv => !this.IsAdmin(kv.Value))).Select<KeyValuePair<string, GroupParticipantState>, string>((Func<KeyValuePair<string, GroupParticipantState>, string>) (kv => kv.Key));
      }
    }

    public int Count => this.users.Count - this.pendingRemovals.Count;

    public GroupParticipants(MessagesContext db, string jid)
    {
      this.gjid = jid;
      this.LogHeader = string.Format("participants[{0}]", (object) jid);
      this.cacheVerison = db.GetCacheVersion();
      this.oldContext_ = (SqliteMessagesContext) db;
      this.users = db.GetParticipants(jid, false);
      Log.l(this.LogHeader, "GroupParticipants loaded {0} participants", (object) this.users.Count);
    }

    public void Clear()
    {
      this.wasCleared = true;
      this.users.Clear();
      this.pendingAdditions.Clear();
      this.pendingRemovals.Clear();
      this.pendingElevations.Clear();
      this.pendingDemotions.Clear();
    }

    public bool ContainsKey(string jid)
    {
      return !this.pendingRemovals.ContainsKey(jid) && this.users.ContainsKey(jid);
    }

    public void Remove(string jid)
    {
      this.pendingAdditions.Remove(jid);
      this.pendingElevations.Remove(jid);
      this.pendingDemotions.Remove(jid);
      this.pendingRemovals[jid] = true;
    }

    public void Add(string jid, bool admin)
    {
      this.pendingRemovals.Remove(jid);
      this.pendingAdditions[jid] = true;
      this.users[jid] = new GroupParticipantState()
      {
        GroupJid = this.gjid,
        MemberJid = jid,
        Flags = admin ? 1L : 0L
      };
    }

    public void AddSuperAdmin(string jid)
    {
      if (this.users[jid] == null)
        this.Add(jid, true);
      this.users[jid].Flags |= 4L;
    }

    private bool IsSuperAdmin(GroupParticipantState state)
    {
      return !this.pendingRemovals.ContainsKey(state.MemberJid) && ((ulong) state.Flags & 4UL) > 0UL;
    }

    private bool IsAdmin(GroupParticipantState state)
    {
      string memberJid = state.MemberJid;
      if (this.pendingRemovals.ContainsKey(memberJid))
        return false;
      if (this.pendingElevations.ContainsKey(memberJid))
        return true;
      return ((state.Flags & 1L) != 0L || (state.Flags & 4L) != 0L) && !this.pendingDemotions.ContainsKey(memberJid);
    }

    public bool IsAdmin(string jid)
    {
      GroupParticipantState state = (GroupParticipantState) null;
      return this.users.TryGetValue(jid, out state) && state != null && this.IsAdmin(state);
    }

    public bool IsSuperAdmin(string jid)
    {
      GroupParticipantState state = (GroupParticipantState) null;
      return this.users.TryGetValue(jid, out state) && state != null && this.IsSuperAdmin(state);
    }

    public void SetAdmin(string jid)
    {
      this.pendingDemotions.Remove(jid);
      this.pendingElevations[jid] = true;
    }

    public void RemoveAdmin(string jid)
    {
      this.pendingElevations.Remove(jid);
      this.pendingDemotions[jid] = true;
    }

    public bool Set(string[] participants)
    {
      if (participants == null)
        return false;
      bool flag = false;
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
      List<string> stringList = new List<string>();
      WhatsApp.WaCollections.Set<string> set = new WhatsApp.WaCollections.Set<string>();
      foreach (string participant in participants)
      {
        if (this.ContainsKey(participant))
          set.Add(participant);
        else
          dictionary[participant] = false;
      }
      foreach (string key in this.Keys)
      {
        if (!set.Contains(key))
        {
          this.Remove(key);
          flag = true;
        }
      }
      foreach (string key in dictionary.Keys)
      {
        this.Add(key, false);
        flag = true;
      }
      return flag;
    }

    public void Flush()
    {
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = false;
        if (this.wasCleared)
        {
          Dictionary<string, GroupParticipantState> participants = db.GetParticipants(this.gjid, false);
          if (participants.Count != 0)
          {
            foreach (KeyValuePair<string, GroupParticipantState> keyValuePair in participants)
            {
              db.DeleteGroupParticipantStateOnSubmit(keyValuePair.Value);
              flag = true;
            }
          }
          if (flag)
          {
            db.SubmitChanges();
            flag = false;
          }
          this.wasCleared = false;
        }
        Func<IEnumerable<string>, IEnumerable<string>> func = (Func<IEnumerable<string>, IEnumerable<string>>) (jids => jids.Where<string>((Func<string, bool>) (jid => !this.pendingRemovals.ContainsKey(jid))));
        foreach (string key in func((IEnumerable<string>) this.pendingElevations.Keys))
        {
          GroupParticipantState participantState = (GroupParticipantState) null;
          if (this.users.TryGetValue(key, out participantState) && participantState != null)
          {
            participantState.Flags |= 1L;
            flag = true;
          }
        }
        foreach (string key in func((IEnumerable<string>) this.pendingDemotions.Keys))
        {
          GroupParticipantState participantState = (GroupParticipantState) null;
          if (this.users.TryGetValue(key, out participantState) && participantState != null)
          {
            participantState.Flags &= -2L;
            flag = true;
          }
        }
        this.pendingElevations.Clear();
        this.pendingDemotions.Clear();
        foreach (string key in func((IEnumerable<string>) this.pendingAdditions.Keys))
        {
          GroupParticipantState s = (GroupParticipantState) null;
          if (this.users.TryGetValue(key, out s) && s != null)
          {
            db.InsertGroupParticipantStateOnSubmit(s);
            flag = true;
          }
        }
        this.pendingAdditions.Clear();
        foreach (string key in this.pendingRemovals.Keys)
        {
          GroupParticipantState s = (GroupParticipantState) null;
          if (this.users.TryGetValue(key, out s) && s != null)
          {
            this.users.Remove(key);
            db.DeleteGroupParticipantStateOnSubmit(s);
            flag = true;
          }
        }
        this.pendingRemovals.Clear();
        if (!flag)
          return;
        db.SubmitChanges();
      }));
    }

    public bool NeedsReload
    {
      get
      {
        int num = this.oldContext_.GetCacheVersion() != this.cacheVerison ? 1 : 0;
        if (num == 0)
          return num != 0;
        if (this.loggedBefore)
          return num != 0;
        Log.l(this.LogHeader, "NeedsReload: cache is stale");
        this.loggedBefore = true;
        return num != 0;
      }
    }
  }
}
