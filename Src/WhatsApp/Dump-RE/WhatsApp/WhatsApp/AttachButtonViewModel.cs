// Decompiled with JetBrains decompiler
// Type: WhatsApp.AttachButtonViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class AttachButtonViewModel : PropChangedBase
  {
    private bool isEnabled = true;

    public AttachPanel.ActionType ActionType { get; private set; }

    public Brush ButtonBackground
    {
      get
      {
        return this.ActionType != AttachPanel.ActionType.None ? (Brush) UIUtils.AccentBrush : (Brush) UIUtils.TransparentBrush;
      }
    }

    public System.Windows.Media.ImageSource ButtonThumbnail
    {
      get
      {
        System.Windows.Media.ImageSource buttonThumbnail = (System.Windows.Media.ImageSource) null;
        switch (this.ActionType)
        {
          case AttachPanel.ActionType.RecordVideo:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.CamcorderButtonIcon;
            break;
          case AttachPanel.ActionType.TakePicture:
          case AttachPanel.ActionType.TakePictureOrVideo:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.CameraButtonIcon;
            break;
          case AttachPanel.ActionType.ChooseAudio:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.AudioButtonIcon;
            break;
          case AttachPanel.ActionType.ChooseDocument:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.DocumentButtonIcon;
            break;
          case AttachPanel.ActionType.ChoosePicture:
          case AttachPanel.ActionType.ChoosePictureAndVideo:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.PictureButtonIcon;
            break;
          case AttachPanel.ActionType.ChooseVideo:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.VideoButtonIcon;
            break;
          case AttachPanel.ActionType.ShareContact:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.ContactButtonIcon;
            break;
          case AttachPanel.ActionType.ShareLocation:
            buttonThumbnail = (System.Windows.Media.ImageSource) AssetStore.LocationButtonIcon;
            break;
        }
        return buttonThumbnail;
      }
    }

    public string ButtonText
    {
      get
      {
        string buttonText = (string) null;
        switch (this.ActionType)
        {
          case AttachPanel.ActionType.RecordVideo:
            buttonText = AppResources.AttachButtonCamcorder;
            break;
          case AttachPanel.ActionType.TakePicture:
          case AttachPanel.ActionType.TakePictureOrVideo:
            buttonText = AppResources.AttachButtonCamera;
            break;
          case AttachPanel.ActionType.ChooseAudio:
            buttonText = AppResources.AttachButtonAudio;
            break;
          case AttachPanel.ActionType.ChooseDocument:
            buttonText = AppResources.AttachButtonDocument;
            break;
          case AttachPanel.ActionType.ChoosePicture:
            buttonText = AppResources.AttachButtonExistingPhoto;
            break;
          case AttachPanel.ActionType.ChooseVideo:
            buttonText = AppResources.AttachButtonExistingVideo;
            break;
          case AttachPanel.ActionType.ChoosePictureAndVideo:
            buttonText = AppResources.AttachButtonAlbum;
            break;
          case AttachPanel.ActionType.ShareContact:
            buttonText = AppResources.AttachButtonContact;
            break;
          case AttachPanel.ActionType.ShareLocation:
            buttonText = AppResources.AttachButtonLocation;
            break;
        }
        return buttonText;
      }
    }

    public string ButtonId
    {
      get
      {
        string buttonId = (string) null;
        switch (this.ActionType)
        {
          case AttachPanel.ActionType.RecordVideo:
            buttonId = "AttachButtonCamcorder";
            break;
          case AttachPanel.ActionType.TakePicture:
          case AttachPanel.ActionType.TakePictureOrVideo:
            buttonId = "AttachButtonCamera";
            break;
          case AttachPanel.ActionType.ChooseAudio:
            buttonId = "AttachButtonAudio";
            break;
          case AttachPanel.ActionType.ChooseDocument:
            buttonId = "AttachButtonDocument";
            break;
          case AttachPanel.ActionType.ChoosePicture:
            buttonId = "AttachButtonExistingPhoto";
            break;
          case AttachPanel.ActionType.ChooseVideo:
            buttonId = "AttachButtonExistingVideo";
            break;
          case AttachPanel.ActionType.ChoosePictureAndVideo:
            buttonId = "AttachButtonAlbum";
            break;
          case AttachPanel.ActionType.ShareContact:
            buttonId = "AttachButtonContact";
            break;
          case AttachPanel.ActionType.ShareLocation:
            buttonId = "AttachButtonLocation";
            break;
        }
        return buttonId;
      }
    }

    public double ButtonOpacity => !this.IsEnabled ? 0.5 : 1.0;

    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.NotifyPropertyChanged("ButtonOpacity");
      }
    }

    public double ButtonSize => AttachPanel.ButtonSize;

    public Thickness ButtonMargin => new Thickness(AttachPanel.ButtonGap / 2.0);

    public AttachButtonViewModel(AttachPanel.ActionType actType) => this.ActionType = actType;
  }
}
