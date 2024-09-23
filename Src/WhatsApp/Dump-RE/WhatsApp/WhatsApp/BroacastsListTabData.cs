// Decompiled with JetBrains decompiler
// Type: WhatsApp.BroacastsListTabData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class BroacastsListTabData : GroupsListTabData
  {
    public BroacastsListTabData() => this.Header = AppResources.BroadcastListsHeader;

    protected override List<Conversation> LoadFromDb(MessagesContext db)
    {
      List<Conversation> conversations = db.GetConversations(new JidHelper.JidTypes[1]
      {
        JidHelper.JidTypes.Broadcast
      }, true);
      conversations.Sort(new Comparison<Conversation>(Conversation.CompareByName));
      return conversations;
    }
  }
}
