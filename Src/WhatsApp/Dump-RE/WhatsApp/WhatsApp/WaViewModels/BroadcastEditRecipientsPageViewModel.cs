// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.BroadcastEditRecipientsPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class BroadcastEditRecipientsPageViewModel : EditParticipantsPageViewModel
  {
    public override string PageTitleStr => AppResources.BroadcastListAddRecipientTitle;

    public override string AddParticipantButtonStr => AppResources.AddRecipients;

    public BroadcastEditRecipientsPageViewModel(
      Conversation convo,
      PageOrientation initialOrientation)
      : base(convo, initialOrientation)
    {
    }

    protected override void OnParticipantAddSubmitting(
      MessagesContext db,
      UserStatus user,
      bool isConvoInDb)
    {
      if (!isConvoInDb)
        return;
      this.InsertParticipantChangedMessage(db, user.Jid, SystemMessageUtils.ParticipantChange.Join);
    }

    protected override void OnParticipantRemoveSubmitting(
      MessagesContext db,
      UserStatus user,
      bool isConvoInDb)
    {
      if (!isConvoInDb)
        return;
      this.InsertParticipantChangedMessage(db, user.Jid, SystemMessageUtils.ParticipantChange.Removed);
    }
  }
}
