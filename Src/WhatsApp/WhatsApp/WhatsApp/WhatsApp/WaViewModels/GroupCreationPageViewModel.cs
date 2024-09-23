// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.GroupCreationPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;


namespace WhatsApp.WaViewModels
{
  public class GroupCreationPageViewModel : MultiParticipantsChatViewModelBase
  {
    private bool isSIPUp_;
    private System.Windows.Media.ImageSource GroupThumbSource_;

    public bool IsSIPUp
    {
      set
      {
        this.isSIPUp_ = value;
        this.NotifyPropertyChanged("PageMargin");
      }
    }

    public override Thickness PageMargin
    {
      get
      {
        return !this.isSIPUp_ || !this.Orientation.IsLandscape() ? new Thickness(0.0) : new Thickness(0.0, -120.0, 0.0, 0.0);
      }
    }

    public override string PageTitle => AppResources.CreateGroupTitle;

    public GroupCreationPageViewModel(Conversation convo, PageOrientation initialOrientation)
      : base(convo, initialOrientation)
    {
    }

    protected override System.Windows.Media.ImageSource GetPictureSource()
    {
      return this.GroupThumbSource_;
    }

    public void SetPictureSource(System.Windows.Media.ImageSource source)
    {
      this.GroupThumbSource_ = source;
      this.NotifyPropertyChanged("PictureSource");
    }
  }
}
