// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.BroadcastInfoPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;


namespace WhatsApp.WaViewModels
{
  public class BroadcastInfoPageViewModel : MultiParticipantsChatInfoPageViewModel
  {
    public override string PageTitleStr => AppResources.PageTitleBroadcastList;

    protected override System.Windows.Media.ImageSource GetPictureSource()
    {
      return (System.Windows.Media.ImageSource) AssetStore.Broadcast;
    }

    public override string AddParticipantButtonStr => AppResources.AddRecipients;

    public override string ParticipantsStr => AppResources.Recipients;

    public override string ChatNameTooltip => AppResources.BroadcastListName;

    public BroadcastInfoPageViewModel(Conversation convo, PageOrientation initialOrientation)
      : base(convo, initialOrientation)
    {
    }

    public override string EncryptionStateStr => AppResources.EncryptedBroadcast;

    public override System.Windows.Media.ImageSource EncryptionStateIcon
    {
      get => (System.Windows.Media.ImageSource) AssetStore.LoadAsset("encrypted.png");
    }
  }
}
