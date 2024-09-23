// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.RevokeMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class RevokeMessage
  {
    public static void RevokeMessagesFromMe(Message[] msgs)
    {
      if (msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      Conversation c = (Conversation) null;
      string jid = (string) null;
      foreach (Message msg in msgs)
      {
        Message m = msg;
        if (m != null)
        {
          Action postDbAction = (Action) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            db.RevokeMessageOnSubmit(m, out postDbAction);
            if (c == null)
            {
              jid = m.KeyRemoteJid;
              c = db.GetConversation(jid, CreateOptions.None);
            }
            if (c != null)
            {
              c.UpdateModifyTag();
              AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.ModifyTag);
            }
            db.SubmitChanges();
          }));
          Action action = postDbAction;
          if (action != null)
            action();
        }
      }
    }

    public static void RevokeMessageFromMe(Message msg)
    {
      if (msg == null)
        return;
      RevokeMessage.RevokeMessagesFromMe(new Message[1]
      {
        msg
      });
    }
  }
}
