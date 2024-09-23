// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.MultiParticipantsChatInfoPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;


namespace WhatsApp.WaViewModels
{
  public abstract class MultiParticipantsChatInfoPageViewModel : MultiParticipantsChatViewModelBase
  {
    public abstract string PageTitleStr { get; }

    public abstract string ChatNameTooltip { get; }

    public abstract string EncryptionStateStr { get; }

    public abstract System.Windows.Media.ImageSource EncryptionStateIcon { get; }

    public MultiParticipantsChatInfoPageViewModel(
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
      this.InsertParticipantChangedMessage(db, user.Jid, SystemMessageUtils.ParticipantChange.Join);
    }

    protected override void OnParticipantRemoveSubmitting(
      MessagesContext db,
      UserStatus user,
      bool isConvoInDb)
    {
      this.InsertParticipantChangedMessage(db, user.Jid, SystemMessageUtils.ParticipantChange.Removed);
    }
  }
}
