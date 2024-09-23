// Decompiled with JetBrains decompiler
// Type: WhatsApp.UpdateVersion
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.System;


namespace WhatsApp
{
  public class UpdateVersion : PhoneApplicationPage
  {
    private bool maybeLaterAllowed;
    private const string AppidMarket = "9WZDNCRDFWBS";
    private const string AppidBeta = "9WZDNCRDFWBT";
    private const string MSStoreDeepLinkWp10 = "ms-windows-store://pdp/?productid=9WZDNCRDFWBT";
    private const string MSStoreDeepLinkWp81 = "ms-windows-store:navigate?appid=6b587088-a2bd-4597-8416-6c77f0a3ec6d&cid=msft_web_appsforwindowsphone_chart";
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Image UpdateIconBlack;
    internal Image UpdateIconWhite;
    internal StackPanel ContentPanel;
    internal TextBlock UpdateText;
    internal RoundButton ButtonIcon;
    internal TextBlock ButtonText;
    internal Button UpdateLaterButton;
    internal RoundButton ButtonIcon2;
    internal TextBlock ButtonText2;
    internal Button ContactSupportButton;
    internal RoundButton ButtonIcon3;
    internal TextBlock ButtonText3;
    internal StackPanel DeprecatedPanel;
    internal RichTextBlock DeprecatedText;
    private bool _contentLoaded;

    public UpdateVersion()
    {
      this.InitializeComponent();
      if (AppState.ShowDeprecationMessaging() && AppState.IsFinalRelease())
      {
        this.ContentPanel.Visibility = Visibility.Collapsed;
        this.DeprecatedPanel.Visibility = Visibility.Visible;
        string s;
        if ((AppState.IsExpired ? 1 : 0) != 0)
        {
          if (AppState.IsFinalRelease())
            FunRunner.ClearSkewValues("final release expired");
          this.TitlePanel.SmallTitle = AppResources.WP7DeprecationPhoneNotSupportedTitle;
          s = AppResources.WP7DeprecationPhoneNotSupportedDeprecation;
        }
        else
        {
          this.TitlePanel.SmallTitle = AppResources.UpdateVersionTitle;
          s = string.Format(AppResources.WP7DeprecationNoMoreUpdates, (object) AppState.GetDeprecationDateString());
        }
        WaRichText.Chunk chunk1 = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(s)).SingleOrDefault<WaRichText.Chunk>();
        if (chunk1 == null)
        {
          this.DeprecatedText.Text = new RichTextBlock.TextSet()
          {
            Text = s
          };
        }
        else
        {
          string learnMoreString = chunk1.Value;
          Func<string> valueFunc = (Func<string>) (() => learnMoreString.Replace(' ', ' '));
          WaRichText.Chunk chunk2 = new WaRichText.Chunk(chunk1.Offset, chunk1.Length, WaRichText.Formats.Link, Constants.SwitchPhonesUrl, valueFunc);
          this.DeprecatedText.Text = new RichTextBlock.TextSet()
          {
            Text = s,
            PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
            {
              chunk2
            }
          };
        }
      }
      else
      {
        this.ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-update.png");
        this.ButtonText.Text = AppResources.UpdateNow;
        this.ButtonIcon2.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/next.png");
        this.ButtonText2.Text = AppResources.UpdateLater;
        this.UpdateLaterButton.Visibility = Visibility.Collapsed;
        this.ButtonIcon3.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-contact.png");
        this.ButtonText3.Text = AppResources.ContactSupportButton;
        this.ContactSupportButton.Visibility = Visibility.Collapsed;
      }
      if (ImageStore.IsDarkTheme())
        this.UpdateIconBlack.Opacity = 0.0;
      else
        this.UpdateIconWhite.Opacity = 0.0;
      App.LogExpiryRelatedVariables("UpdateVersion page");
    }

    private void OnUpdateClick(object sender, EventArgs e) => UpdateVersion.UpdateApp();

    private void OnUpdateLaterClick(object sender, EventArgs e) => NavUtils.NavigateHome();

    private void ContactSupport_Click(object sender, EventArgs e)
    {
      this.ContactSupportButton.Visibility = Visibility.Visible;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddBool("PageReplace", true);
      uriParams.AddString("context", "app update");
      NavUtils.NavigateToPage(this.NavigationService, "ContactSupportPage", uriParams);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      string str;
      if (UriUtils.ParsePageParams(e.Uri).TryGetValue("UpdateLater", out str))
        this.maybeLaterAllowed = str == "true";
      if (this.maybeLaterAllowed)
      {
        this.UpdateText.Text = AppResources.VerificationNeedsUpdateSoon;
        this.UpdateLaterButton.Visibility = Visibility.Visible;
      }
      else
      {
        this.UpdateText.Text = AppResources.VerificationNeedsUpdate;
        if (AppState.GetTimeSinceBuild(new DateTime?(DateTime.UtcNow)).TotalDays < 22.0)
          this.ContactSupportButton.Visibility = Visibility.Visible;
      }
      base.OnNavigatedTo(e);
    }

    public static void UpdateApp()
    {
      string url = UpdateVersion.UpdateAppUrl();
      AppState.Worker.Enqueue((Action) (async () =>
      {
        try
        {
          int num = await Launcher.LaunchUriAsync(new Uri(url)) ? 1 : 0;
        }
        catch (Exception ex)
        {
          Log.l("updateVersion", "exception using {0}", (object) url);
          Log.SendCrashLog(ex, "Using Update Url", logOnlyForRelease: true);
        }
      }));
    }

    public static string UpdateAppUrl()
    {
      string str = Constants.UpdateUrl;
      if (Settings.MSUpdateLinkEnabled)
        str = !AppState.IsWP10OrLater ? "ms-windows-store:navigate?appid=6b587088-a2bd-4597-8416-6c77f0a3ec6d&cid=msft_web_appsforwindowsphone_chart" : "ms-windows-store://pdp/?productid=9WZDNCRDFWBT";
      return str;
    }

    private void OnLearnMoreClick(object sender, RoutedEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri(Constants.SwitchPhonesUrl)
      }.Show();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/UpdateVersion.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.UpdateIconBlack = (Image) this.FindName("UpdateIconBlack");
      this.UpdateIconWhite = (Image) this.FindName("UpdateIconWhite");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.UpdateText = (TextBlock) this.FindName("UpdateText");
      this.ButtonIcon = (RoundButton) this.FindName("ButtonIcon");
      this.ButtonText = (TextBlock) this.FindName("ButtonText");
      this.UpdateLaterButton = (Button) this.FindName("UpdateLaterButton");
      this.ButtonIcon2 = (RoundButton) this.FindName("ButtonIcon2");
      this.ButtonText2 = (TextBlock) this.FindName("ButtonText2");
      this.ContactSupportButton = (Button) this.FindName("ContactSupportButton");
      this.ButtonIcon3 = (RoundButton) this.FindName("ButtonIcon3");
      this.ButtonText3 = (TextBlock) this.FindName("ButtonText3");
      this.DeprecatedPanel = (StackPanel) this.FindName("DeprecatedPanel");
      this.DeprecatedText = (RichTextBlock) this.FindName("DeprecatedText");
    }
  }
}
