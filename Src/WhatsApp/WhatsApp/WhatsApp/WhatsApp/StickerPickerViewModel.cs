// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerPickerViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class StickerPickerViewModel : PropChangedBase
  {
    public List<TrayItemViewModel> TrayItems;
    private int trayItemIndex = -1;
    private int stickerCount;
    private PageOrientation orientation;

    public int TrayItemIndex
    {
      get => this.trayItemIndex;
      set
      {
        if (this.trayItemIndex == value)
          return;
        if (this.trayItemIndex >= 0)
        {
          TrayItemViewModel trayItem = this.TrayItems[this.trayItemIndex];
          if (trayItem != null)
            trayItem.IsSelected = false;
        }
        this.trayItemIndex = value;
        this.TrayItems[this.trayItemIndex].IsSelected = true;
      }
    }

    public int PivotIndex { get; set; }

    public TrayItemViewModel SelectedTrayItem => this.TrayItems[this.trayItemIndex];

    public string TitleTextText
    {
      get
      {
        switch (this.SelectedTrayItem.GroupType)
        {
          case PickerGroupType.Recent:
            return AppResources.RecentlyUsedEmojis;
          case PickerGroupType.Starred:
            return AppResources.NoSavedStickers;
          default:
            return "";
        }
      }
    }

    public Visibility TitleTextBlockVisibility
    {
      get
      {
        switch (this.SelectedTrayItem.GroupType)
        {
          case PickerGroupType.Recent:
            return Visibility.Visible;
          case PickerGroupType.Starred:
            return this.StickerCount <= 0 ? Visibility.Visible : Visibility.Collapsed;
          default:
            return Visibility.Collapsed;
        }
      }
    }

    public int StickerCount
    {
      get => this.stickerCount;
      set
      {
        if (this.stickerCount == value)
          return;
        this.stickerCount = value;
      }
    }

    public int RecentStickersRows => 2;

    public PageOrientation Orientation
    {
      get => this.orientation;
      set
      {
        if (this.orientation == value)
          return;
        this.orientation = value;
      }
    }

    public StickerPickerViewModel(Button plusButton)
    {
      this.InitTrayItems();
      if (plusButton == null)
        return;
      plusButton.Click += new RoutedEventHandler(this.PlusButton_Tap);
    }

    private void InitTrayItems()
    {
      this.TrayItems = new List<TrayItemViewModel>()
      {
        new TrayItemViewModel(this, PickerGroupType.Recent, 0),
        new TrayItemViewModel(this, PickerGroupType.Starred, 1)
      };
      int num = 2;
      foreach (StickerPack availableStickerPack in SqliteStickerPacks.GetAvailableStickerPacks())
        this.TrayItems.Add(new TrayItemViewModel(this, PickerGroupType.Pack, num++, availableStickerPack));
    }

    private void PlusButton_Tap(object sender, RoutedEventArgs e)
    {
    }
  }
}
