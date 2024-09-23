// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.AsyncRevoke
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class AsyncRevoke
  {
    public static void AddPendingRevoke(
      MessagesContext db,
      string origJid,
      string origId,
      bool origFromMe,
      string revJid,
      string revId,
      bool revFromMe)
    {
      if (origJid == null || origId == null || revJid == null || revId == null || db == null)
      {
        Log.l("async revoke", "bad arguments");
      }
      else
      {
        Log.l("async revoke", "origJid:{0}, origId:{1}, revJid:{2}, revId:{3}", (object) origJid, (object) origId, (object) revJid, (object) revId);
        if (AsyncRevoke.IsRevokePending(db, origJid, origId))
        {
          Log.l("async revoke", "scheduled task exists already, this is a retry | id:{0}", (object) origId);
        }
        else
        {
          Log.l("async revoke", "create scheduled task, id:{0}", (object) origId);
          BinaryData binaryData = new BinaryData();
          binaryData.AppendStrWithLengthPrefix(origJid);
          binaryData.AppendStrWithLengthPrefix(origId);
          binaryData.AppendInt32(origFromMe ? 1 : 0);
          binaryData.AppendStrWithLengthPrefix(revJid);
          binaryData.AppendStrWithLengthPrefix(revId);
          binaryData.AppendInt32(revFromMe ? 1 : 0);
          WaScheduledTask task = new WaScheduledTask(WaScheduledTask.Types.PendingRevoke, AsyncRevoke.GetLookupKey(origJid, origId), binaryData.Get(), WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(1.0)));
          db.InsertWaScheduledTaskOnSubmit(task);
          db.SubmitChanges();
        }
      }
    }

    public static IObservable<Unit> PerformPendingRevoke(WaScheduledTask task)
    {
      return Observable.Empty<Unit>();
    }

    private static string GetLookupKey(string jid, string keyId)
    {
      return string.Format("{0}-{1}", (object) keyId, (object) jid);
    }

    public static bool IsRevokePending(MessagesContext db, string jid, string keyId)
    {
      return ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
      {
        WaScheduledTask.Types.PendingRevoke
      }, lookupKey: AsyncRevoke.GetLookupKey(jid, keyId))).FirstOrDefault<WaScheduledTask>() != null;
    }

    public static void RemovePendingRevoke(MessagesContext db, string jid, string keyId)
    {
      Log.l("async revoke", "remove pending revoke | jid:{0},id:{0}", (object) jid, (object) keyId);
      db.RemoveScheduledTasks(WaScheduledTask.Types.PendingRevoke, AsyncRevoke.GetLookupKey(jid, keyId));
    }
  }
}
