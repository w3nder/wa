// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAdminPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class WaAdminPageViewModel : PageViewModelBase
  {
    public WaAdminPageViewModel(PageOrientation orientation)
      : base(orientation)
    {
    }

    public bool ForceFbPlaceSearch
    {
      get => WebServices.ForceFbPlaceSearch;
      set => WebServices.ForceFbPlaceSearch = value;
    }

    public bool VacationModeEnabled
    {
      get => Settings.VacationModeEnabled;
      set => Settings.VacationModeEnabled = value;
    }

    public bool StickerPickerEnabled
    {
      get => Settings.StickerPickerEnabled;
      set => Settings.StickerPickerEnabled = value;
    }

    public bool FinalReleaseEnabled
    {
      get => AppState.IsFinalRelease();
      set => Settings.DeprecationFinalRelease = value ? 1 : 0;
    }

    public int DaysTillExpiry => (int) AppState.DaysUntilExpiration();
  }
}
