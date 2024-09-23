// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.EditParticipantsPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;


namespace WhatsApp.WaViewModels
{
  public abstract class EditParticipantsPageViewModel : MultiParticipantsChatViewModelBase
  {
    public abstract string PageTitleStr { get; }

    public Visibility SearchVisibility => (!this.Orientation.IsLandscape()).ToVisibility();

    public EditParticipantsPageViewModel(Conversation convo, PageOrientation initialOrientation)
      : base(convo, initialOrientation)
    {
    }

    protected override void OnOrientationChanged()
    {
      this.NotifyPropertyChanged("SearchVisibility");
      base.OnOrientationChanged();
    }
  }
}
