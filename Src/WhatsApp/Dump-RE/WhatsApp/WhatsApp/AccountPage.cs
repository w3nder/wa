// Decompiled with JetBrains decompiler
// Type: WhatsApp.AccountPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class AccountPage : PhoneApplicationPage
  {
    internal Grid LayoutRoot;
    internal Grid Content;
    internal PageTitlePanel TitlePanel;
    internal Button PrivacyButton;
    internal Button SecurityButton;
    internal Button ChangeNumberButton;
    internal Button TwoFacButton;
    internal Button DownloadDataButton;
    internal Button DeleteAccountButton;
    private bool _contentLoaded;

    public AccountPage()
    {
      this.InitializeComponent();
      this.DownloadDataButton.Visibility = Settings.GdprReportEnabled.ToVisibility();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.ACCOUNT);
    }

    private void Privacy_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "PrivacySettingsPage", folderName: "Pages/Settings");
    }

    private void Security_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "SecuritySettingsPage", folderName: "Pages/Settings");
    }

    private void TwoFactorButton_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "TwoFactorPage", folderName: "Pages/TwoFactor");
    }

    private void ChangeNumberButton_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "ChangeNumberStartPage");
    }

    private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "DeleteAccountPage", folderName: "Pages/Settings");
    }

    private void DownloadDataButton_Click(object sender, RoutedEventArgs e)
    {
      if (!Settings.GdprReportEnabled)
        return;
      NavUtils.NavigateToPage(this.NavigationService, "GdprReportPage", folderName: "Pages/Settings");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/AccountPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Content = (Grid) this.FindName("Content");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.PrivacyButton = (Button) this.FindName("PrivacyButton");
      this.SecurityButton = (Button) this.FindName("SecurityButton");
      this.ChangeNumberButton = (Button) this.FindName("ChangeNumberButton");
      this.TwoFacButton = (Button) this.FindName("TwoFacButton");
      this.DownloadDataButton = (Button) this.FindName("DownloadDataButton");
      this.DeleteAccountButton = (Button) this.FindName("DeleteAccountButton");
    }
  }
}
