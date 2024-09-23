// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.PinToStart
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class PinToStart
  {
    public static bool Pin(string jid)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
      return PinToStart.Pin(convo);
    }

    public static bool Pin(UserStatus user)
    {
      if (user == null)
        return false;
      if (TileHelper.ChatTileExists(user.Jid))
        return true;
      bool toRequestLargePic = false;
      string imagePath = ChatPictureStore.GetPicturePath(user.Jid, out toRequestLargePic) ?? user.PhotoPath;
      if (!PinToStart.CreateConversationTile(user.Jid, user.GetDisplayName(), imagePath))
        return false;
      if (toRequestLargePic)
        AppState.SchedulePersistentAction(PersistentAction.SendGetImage(user.Jid));
      return true;
    }

    public static bool Pin(Conversation convo)
    {
      if (convo == null)
        return false;
      if (JidHelper.IsUserJid(convo.Jid))
        return PinToStart.Pin(ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (db => db.GetUserStatus(convo.Jid))));
      if (TileHelper.ChatTileExists(convo.Jid))
        return true;
      bool toRequestLargePic = false;
      string picturePath = ChatPictureStore.GetPicturePath(convo.Jid, out toRequestLargePic);
      if (!PinToStart.CreateConversationTile(convo.Jid, convo.GetName(), picturePath))
        return false;
      if (toRequestLargePic)
        AppState.SchedulePersistentAction(PersistentAction.SendGetImage(convo.Jid));
      return true;
    }

    private static bool CreateConversationTile(string jid, string displayName, string imagePath)
    {
      Conversation convo = (Conversation) null;
      Message lastMessage = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = db.GetConversation(jid, CreateOptions.CreateAndSubmitIfNotFound);
        if (convo.GetUnreadMessagesCount() <= 0 || !convo.LastMessageID.HasValue)
          return;
        lastMessage = db.GetMessageById(convo.LastMessageID.Value);
      }));
      TileHelper.CreateChatTile(jid, displayName, imagePath, convo.GetUnreadMessagesCount(), lastMessage);
      return TileHelper.GetChatTile(convo.Jid) != null;
    }
  }
}
