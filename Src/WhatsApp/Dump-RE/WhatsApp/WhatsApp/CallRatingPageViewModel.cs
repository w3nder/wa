// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallRatingPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CallRatingPageViewModel : PageViewModelBase
  {
    private string peerJid;
    private int ratingVal;
    private bool shouldShowName;

    public string PeerJid => this.peerJid;

    public System.Windows.Media.ImageSource WhatsAppIconSource
    {
      get
      {
        BitmapImage iconSrc = AssetStore.LoadAsset("wa-callscreen-icon.png", AssetStore.ThemeSetting.Dark);
        if (iconSrc == null)
          return (System.Windows.Media.ImageSource) null;
        return ImageStore.IsDarkTheme() ? (System.Windows.Media.ImageSource) iconSrc : (System.Windows.Media.ImageSource) IconUtils.CreateColorIcon((BitmapSource) iconSrc, (Brush) UIUtils.ForegroundBrush, new double?((double) iconSrc.PixelWidth));
      }
    }

    public override string PageTitle => AppResources.WhatsAppCallHeader;

    public Thickness ContentPanelMargin
    {
      get
      {
        double num = 24.0 * ResolutionHelper.ZoomMultiplier;
        return new Thickness(num, 0.0, num, 0.0);
      }
    }

    public double FeedbackBoxMaxHeight => 216.0 * ResolutionHelper.ZoomMultiplier;

    public string FeedbackBoxTooltipStr => AppResources.VoipRatingFeedbackBoxTooltip;

    public string RatingTooltipStr
    {
      get
      {
        if (this.peerJid == null || !this.shouldShowName)
          return AppResources.VoipRatingTooltip;
        UserStatus userStatus = UserCache.Get(this.peerJid, false);
        return string.Format(AppResources.VoipRatingTooltipWith, userStatus == null ? (object) JidHelper.GetPhoneNumber(this.peerJid, true) : (object) userStatus.GetDisplayName(true));
      }
    }

    public double AppBarHeight => 72.0 * ResolutionHelper.ZoomMultiplier;

    public int RatingValue
    {
      get => this.ratingVal;
      set
      {
        if (this.ratingVal == value)
          return;
        this.ratingVal = value;
        this.NotifyPropertyChanged(nameof (RatingValue));
        this.NotifyPropertyChanged("RatingDescriptionStr");
      }
    }

    public string RatingDescriptionStr
    {
      get
      {
        string ratingDescriptionStr;
        switch (this.RatingValue)
        {
          case 1:
            ratingDescriptionStr = AppResources.VoipRatingLevel1;
            break;
          case 2:
            ratingDescriptionStr = AppResources.VoipRatingLevel2;
            break;
          case 3:
            ratingDescriptionStr = AppResources.VoipRatingLevel3;
            break;
          case 4:
            ratingDescriptionStr = AppResources.VoipRatingLevel4;
            break;
          case 5:
            ratingDescriptionStr = AppResources.VoipRatingLevel5;
            break;
          default:
            ratingDescriptionStr = "";
            break;
        }
        return ratingDescriptionStr;
      }
    }

    public CallRatingPageViewModel(string jid, bool showName, PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.peerJid = jid;
      this.shouldShowName = showName;
    }

    public Visibility ShowLowDataNotification
    {
      get => !Settings.LowBandwidthVoip ? Visibility.Collapsed : Visibility.Visible;
    }

    public string LowDataUsageEnabledStr => AppResources.CallRatingScreenLowDataUsageEnabled;

    public string WhatsAppSettingsStr => AppResources.CallRatingScreenLowDataWhatsAppSettings;
  }
}
