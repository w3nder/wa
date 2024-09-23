// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SetStatus
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.CommonOps
{
  public static class SetStatus
  {
    public static bool TryGetPendingStatus(out string pendingStatus)
    {
      string s = pendingStatus = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        PersistentAction persistentAction = ((IEnumerable<PersistentAction>) db.GetPersistentActions(PersistentAction.Types.SetStatus)).LastOrDefault<PersistentAction>();
        if (persistentAction == null)
          return;
        s = persistentAction.ActionDataString;
      }));
      return (pendingStatus = s) != null;
    }

    private static void CancelPendingStatus()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (PersistentAction persistentAction in db.GetPersistentActions(PersistentAction.Types.SetStatus))
          db.DeletePersistentActionOnSubmit(persistentAction);
      }));
    }

    public static void Set(string newStatus, string incomingId = null)
    {
      string pendingStatus = (string) null;
      if (SetStatus.TryGetPendingStatus(out pendingStatus))
        SetStatus.CancelPendingStatus();
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(Settings.MyJid);
        if (userStatus == null)
          return;
        userStatus.Status = newStatus;
        db.SubmitChanges();
      }));
      AppState.SchedulePersistentAction(PersistentAction.SetStatus(newStatus, incomingId));
    }

    public static void ResetOnDisabled()
    {
      bool toReset = false;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(Settings.MyJid);
        if (userStatus == null || !(userStatus.Status != "**no status**"))
          return;
        toReset = true;
      }));
      if (!toReset)
        return;
      SetStatus.Set("**no status**");
    }
  }
}
