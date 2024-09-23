// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.GroupParticipantsPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;


namespace WhatsApp.WaViewModels
{
  public class GroupParticipantsPageViewModel : EditParticipantsPageViewModel
  {
    public override string PageTitleStr => AppResources.GroupParticipants;

    public GroupParticipantsPageViewModel(Conversation convo, PageOrientation initialOrientation)
      : base(convo, initialOrientation)
    {
    }
  }
}
