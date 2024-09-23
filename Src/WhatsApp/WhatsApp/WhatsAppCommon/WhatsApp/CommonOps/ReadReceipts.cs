// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ReadReceipts
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.CommonOps
{
  public class ReadReceipts
  {
    public static void Send(MessagesContext db, ReceiptSpec[] receipts)
    {
      LinkedList<PersistentAction> outgoingTasks = (LinkedList<PersistentAction>) null;
      ReadReceipts.ScheduleSend(db, receipts, out outgoingTasks);
      if (outgoingTasks == null || !outgoingTasks.Any<PersistentAction>())
        return;
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        foreach (PersistentAction a in outgoingTasks)
          AppState.AttemptPersistentAction(a);
      }));
    }

    public static void ScheduleSend(
      MessagesContext db,
      ReceiptSpec[] receipts,
      out LinkedList<PersistentAction> savedOutgoingTasks)
    {
      Log.l("read receipt", "schedule sending {0} read receipts", (object) receipts.Length);
      savedOutgoingTasks = new LinkedList<PersistentAction>();
      DateTime dateTime1 = DateTimeUtils.FromUnixTime(1415214000L);
      bool flag = !Settings.EnableReadReceipts;
      LinkedList<ReceiptSpec> source = new LinkedList<ReceiptSpec>();
      LinkedList<PostponedReceipt> linkedList1 = new LinkedList<PostponedReceipt>();
      LinkedList<CipherTextReceipt> linkedList2 = new LinkedList<CipherTextReceipt>();
      long unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime();
      foreach (ReceiptSpec receipt in receipts)
      {
        DateTime? messageTimestamp = receipt.MessageTimestamp;
        if (messageTimestamp.HasValue)
        {
          messageTimestamp = receipt.MessageTimestamp;
          DateTime dateTime2 = dateTime1;
          if ((messageTimestamp.HasValue ? (messageTimestamp.GetValueOrDefault() < dateTime2 ? 1 : 0) : 0) != 0)
            continue;
        }
        if (receipt.IsCipherText)
        {
          Log.d("read receipt", "ciphertext | jid: {0} participant: {1}", (object) receipt.Jid, (object) receipt.Participant);
          linkedList2.AddLast(new CipherTextReceipt(receipt));
        }
        else
        {
          JidHelper.JidTypes jidType = JidHelper.GetJidType(receipt.Jid);
          if (jidType != JidHelper.JidTypes.Psa)
          {
            if (flag && (jidType == JidHelper.JidTypes.User || jidType == JidHelper.JidTypes.Status))
            {
              Log.d("read receipt", "postponed | jid: {0} participant: {1}", (object) receipt.Jid, (object) receipt.Participant);
              linkedList1.AddLast(new PostponedReceipt(receipt, unixTime));
            }
            else
            {
              Log.d("read receipt", "to send | jid: {0} participant: {1}", (object) receipt.Jid, (object) receipt.Participant);
              source.AddLast(receipt);
            }
          }
        }
      }
      if (source.Any<ReceiptSpec>())
      {
        Func<ReceiptSpec, string> keySelector = (Func<ReceiptSpec, string>) (rs => string.Format("{0},{1}", (object) rs.Jid, (object) (rs.Participant ?? "")));
        foreach (IEnumerable<ReceiptSpec> receiptsEnum in source.GroupBy<ReceiptSpec, string>(keySelector))
        {
          PersistentAction a = PersistentAction.SendReceipts(receiptsEnum, "read");
          if (a != null)
          {
            db.StorePersistentAction(a);
            savedOutgoingTasks.AddLast(a);
          }
        }
        Log.d("read receipt", "scheduled {0} read receipts to be sent", (object) source.Count);
      }
      if (linkedList1.Any<PostponedReceipt>())
      {
        db.InsertPostponedReceiptsOnSubmit((IEnumerable<PostponedReceipt>) linkedList1);
        Log.d("read receipt", "postponed {0} read receipts", (object) linkedList1.Count);
      }
      if (!linkedList2.Any<CipherTextReceipt>())
        return;
      db.InsertCipherTextReceiptsOnSubmit((IEnumerable<CipherTextReceipt>) linkedList2);
      Log.d("read receipt", "ciphertext {0} read receipts", (object) linkedList2.Count);
    }

    public static void SendMessageReceipt(
      Message msg,
      FunXMPP.FMessage.Status receiptStatus,
      bool dedup = true)
    {
      AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => ReadReceipts.SendMessageReceipt(db, msg, receiptStatus, dedup)))));
    }

    public static void SendMessageReceipt(
      MessagesContext db,
      Message msg,
      FunXMPP.FMessage.Status receiptStatus,
      bool dedup = true)
    {
      if (msg == null || msg.KeyFromMe || dedup && msg.Status.GetOverrideWeight() >= receiptStatus.GetOverrideWeight())
        return;
      string receiptType;
      switch (receiptStatus)
      {
        case FunXMPP.FMessage.Status.PlayedByTarget:
          receiptType = "played";
          break;
        case FunXMPP.FMessage.Status.ReadByTarget:
          receiptType = "read";
          break;
        default:
          return;
      }
      PersistentAction pa = PersistentAction.SendReceipt(msg, receiptType);
      db.StorePersistentAction(pa);
      msg.Status = receiptStatus;
      db.SubmitChanges();
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }
  }
}
