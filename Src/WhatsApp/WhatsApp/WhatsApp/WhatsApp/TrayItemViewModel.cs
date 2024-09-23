// Decompiled with JetBrains decompiler
// Type: WhatsApp.TrayItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class TrayItemViewModel : PropChangedBase
  {
    private bool isSelected;

    public PickerGroupType GroupType { get; set; }

    private StickerPack StickerPack { get; set; }

    private StickerPickerViewModel ViewModel { get; set; }

    public int Index { get; set; }

    public TrayItemViewModel(
      StickerPickerViewModel viewModel,
      PickerGroupType type,
      int index,
      StickerPack pack = null)
    {
      this.ViewModel = viewModel;
      this.GroupType = type;
      this.Index = index;
      this.StickerPack = pack;
    }

    public bool IsSelected
    {
      get => this.isSelected;
      set
      {
        if (this.isSelected == value)
          return;
        this.isSelected = value;
        this.NotifyPropertyChanged("TrayButtonBackground");
      }
    }

    public Brush TrayButtonBackground
    {
      get => !this.IsSelected ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.AccentBrush;
    }

    public System.Windows.Media.ImageSource TrayButtonImage
    {
      get
      {
        switch (this.GroupType)
        {
          case PickerGroupType.Recent:
            return (System.Windows.Media.ImageSource) AssetStore.StickerRecentIcon(AssetStore.ThemeSetting.Dark);
          case PickerGroupType.Starred:
            return (System.Windows.Media.ImageSource) AssetStore.StickerSavedIcon(AssetStore.ThemeSetting.Dark);
          case PickerGroupType.Pack:
            byte[] trayImageForPack = SqliteStickerPacks.GetTrayImageForPack(this.StickerPack);
            if (trayImageForPack == null)
              return (System.Windows.Media.ImageSource) AssetStore.StickerRecentIcon(AssetStore.ThemeSetting.Dark);
            MemoryStream streamSource = new MemoryStream(trayImageForPack);
            BitmapImage trayButtonImage = new BitmapImage();
            trayButtonImage.SetSource((Stream) streamSource);
            return (System.Windows.Media.ImageSource) trayButtonImage;
          default:
            return (System.Windows.Media.ImageSource) AssetStore.StickerRecentIcon(AssetStore.ThemeSetting.Dark);
        }
      }
    }
  }
}
