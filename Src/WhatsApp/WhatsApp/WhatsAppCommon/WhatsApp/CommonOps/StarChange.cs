// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.StarChange
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.CommonOps
{
  public static class StarChange
  {
    public static void NotifyQrStarChange(Message msg)
    {
      int newModTag = 0;
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
        if (conversation == null)
          return;
        newModTag = (int) conversation.ModifyTag;
      }));
      AppState.QrPersistentAction.NotifyStarred(msg, newModTag);
    }
  }
}
