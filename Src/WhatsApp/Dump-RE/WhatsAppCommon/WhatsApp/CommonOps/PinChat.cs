// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.PinChat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class PinChat
  {
    public const int MaxPinnedChats = 3;

    public static void Pin(string jid, bool notifyQr = true)
    {
      PinChat.Pin(new string[1]{ jid }, notifyQr);
    }

    public static void Pin(string[] jids, bool notifyQr)
    {
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> list = ((IEnumerable<string>) jids).Select<string, Conversation>((Func<string, Conversation>) (jid => db.GetConversation(jid, CreateOptions.None))).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).ToList<Conversation>();
        PinChat.Pin(db, list, notifyQr, out postDbAction);
        db.SubmitChanges();
      }));
      Action action = postDbAction;
      if (action == null)
        return;
      action();
    }

    public static void Pin(Conversation convo, bool notifyQr = true)
    {
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        MessagesContext db1 = db;
        List<Conversation> convos = new List<Conversation>();
        convos.Add(convo);
        int num = notifyQr ? 1 : 0;
        ref Action local = ref postDbAction;
        PinChat.Pin(db1, convos, num != 0, out local);
        db.SubmitChanges();
      }));
      Action action = postDbAction;
      if (action == null)
        return;
      action();
    }

    private static void Pin(
      MessagesContext db,
      List<Conversation> convos,
      bool notifyQr,
      out Action postDbAction)
    {
      postDbAction = (Action) null;
      int num1 = ((IEnumerable<Conversation>) db.GetPinnedConversations()).Count<Conversation>();
      int num2 = convos.Count<Conversation>();
      Dictionary<string, DateTime> pinTimes = new Dictionary<string, DateTime>();
      int num3 = num2;
      if (num1 + num3 > 3)
      {
        AppState.ClientInstance.ShowMessageBox(Plurals.Instance.GetString(AppResources.PinErrorPlural, 3));
      }
      else
      {
        DateTime pinTime = DateTime.UtcNow;
        int chatCount = 0;
        convos.ForEach((Action<Conversation>) (convo =>
        {
          DateTime dateTime = pinTime.AddSeconds((double) chatCount++);
          Log.d("pin chats", "pinning | jid: {0}, with timestamp: {1}", (object) convo.Jid, (object) dateTime);
          convo.SortKey = new DateTime?(pinTimes[convo.Jid] = dateTime);
        }));
        Log.d("pin chats", "pinning | pinned {0} conversations", (object) chatCount);
      }
      if (!(pinTimes.Count<KeyValuePair<string, DateTime>>() > 0 & notifyQr) || !AppState.QrPersistentAction.ShouldAttemptQrPersistentAction)
        return;
      postDbAction = (Action) (() =>
      {
        foreach (string key in pinTimes.Keys)
          AppState.QrPersistentAction.NotifyChatStatus(key, FunXMPP.ChatStatusForwardAction.Pin, new DateTime?(pinTimes[key]));
      });
    }

    public static void Unpin(string jid, bool notifyQr = true)
    {
      PinChat.Unpin(new string[1]{ jid }, notifyQr);
    }

    public static void Unpin(string[] jids, bool notifyQr)
    {
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> list = ((IEnumerable<string>) jids).Select<string, Conversation>((Func<string, Conversation>) (jid => db.GetConversation(jid, CreateOptions.None))).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).ToList<Conversation>();
        PinChat.Unpin(db, list, notifyQr, out postDbAction);
        db.SubmitChanges();
      }));
      Action action = postDbAction;
      if (action == null)
        return;
      action();
    }

    public static void Unpin(Conversation convo, bool notifyQr = true)
    {
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        MessagesContext db1 = db;
        List<Conversation> convos = new List<Conversation>();
        convos.Add(convo);
        int num = notifyQr ? 1 : 0;
        ref Action local = ref postDbAction;
        PinChat.Unpin(db1, convos, num != 0, out local);
        db.SubmitChanges();
      }));
      Action action = postDbAction;
      if (action == null)
        return;
      action();
    }

    public static void Unpin(
      MessagesContext db,
      List<Conversation> convos,
      bool notifyQr,
      out Action postDbAction)
    {
      postDbAction = (Action) null;
      List<string> unpinnedJid = new List<string>();
      convos.ForEach((Action<Conversation>) (convo =>
      {
        unpinnedJid.Add(convo.Jid);
        if (!convo.SortKey.HasValue)
          return;
        convo.SortKey = new DateTime?();
      }));
      if (!notifyQr || !AppState.QrPersistentAction.ShouldAttemptQrPersistentAction)
        return;
      postDbAction = (Action) (() =>
      {
        foreach (string jid in unpinnedJid)
          AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.Pin);
      });
    }
  }
}
