// Decompiled with JetBrains decompiler
// Type: WhatsApp.About
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class About : PhoneApplicationPage
  {
    private IDisposable serverStatusSub;
    private AboutPageViewModel viewModel;
    internal Grid LayoutRoot;
    internal Grid imagePanel;
    internal RichTextBlock UpdateOsText;
    internal ItemsControl ButtonsList;
    internal TextBlock ViewTermsButton;
    internal TextBlock ViewLicensesButton;
    private bool _contentLoaded;

    public About()
    {
      this.InitializeComponent();
      if (AppState.ShowDeprecationMessaging())
      {
        this.UpdateOsText.Visibility = Visibility.Visible;
        string s = string.Format(AppResources.WP7DeprecationAboutDescription, (object) AppState.GetDeprecationDateString());
        WaRichText.Chunk chunk1 = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(s)).SingleOrDefault<WaRichText.Chunk>();
        if (chunk1 == null)
        {
          this.UpdateOsText.Text = new RichTextBlock.TextSet()
          {
            Text = s
          };
        }
        else
        {
          string learnMoreString = chunk1.Value;
          Func<string> valueFunc = (Func<string>) (() => learnMoreString.Replace(' ', ' '));
          WaRichText.Chunk chunk2 = new WaRichText.Chunk(chunk1.Offset, chunk1.Length, WaRichText.Formats.Link, Constants.SwitchPhonesUrl, valueFunc);
          this.UpdateOsText.Text = new RichTextBlock.TextSet()
          {
            Text = s,
            PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
            {
              chunk2
            }
          };
        }
      }
      this.DataContext = (object) (this.viewModel = new AboutPageViewModel(this.Orientation));
      this.InitButtons();
      this.ViewTermsButton.Text = AppResources.TermsAndPrivacyButton;
      this.ViewTermsButton.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      this.ViewLicensesButton.Text = AppResources.ViewLicenseButton;
      this.ViewLicensesButton.Margin = new Thickness(4.0, 0.0, 12.0, 0.0);
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
    }

    private void InitButtons()
    {
      ObservableCollection<About.ButtonContent> observableCollection1 = new ObservableCollection<About.ButtonContent>();
      observableCollection1.Add(new About.ButtonContent()
      {
        Icon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-faq.png"),
        Text = AppResources.RTFM,
        OnClick = (Action) (() =>
        {
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.FAQ);
          new WebBrowserTask()
          {
            Uri = new Uri("https://faq.whatsapp.com")
          }.Show();
        })
      });
      observableCollection1.Add(new About.ButtonContent()
      {
        Icon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-contact.png"),
        Text = AppResources.ContactSupportButton,
        OnClick = new Action(this.ContactSupport_Click)
      });
      ObservableCollection<About.ButtonContent> observableCollection2 = observableCollection1;
      if (Settings.IsUpdateAvailable && !AppState.IsFinalRelease())
        observableCollection2.Add(new About.ButtonContent()
        {
          Icon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-update.png"),
          Text = AppResources.CheckForUpdates,
          OnClick = new Action(this.UpdateApp_Click)
        });
      this.ButtonsList.ItemsSource = (IEnumerable) observableCollection2;
    }

    private void ViewLicensesButton_Tap(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "ViewLicensePage");
    }

    private void ViewTermsButton_Tap(object sender, RoutedEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri("https://www.whatsapp.com/legal/")
      }.Show();
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      if (this.viewModel != null)
        this.viewModel.Orientation = this.Orientation;
      base.OnOrientationChanged(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (e.NavigationMode != NavigationMode.New)
        return;
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.ABOUT);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.serverStatusSub.SafeDispose();
      this.serverStatusSub = (IDisposable) null;
      base.OnNavigatingFrom(e);
    }

    private void ContactSupport_Click()
    {
      if (AppState.BatterySaverEnabled)
      {
        Nag.NagBatterySaver(AppResources.TurnOffBatterySaverToContactSupport);
      }
      else
      {
        if (this.serverStatusSub != null)
          return;
        this.serverStatusSub = ServerStatus.GetStatus().Timeout<ServerStatus>(TimeSpan.FromSeconds(5.0)).ObserveOnDispatcher<ServerStatus>().Subscribe<ServerStatus>((Action<ServerStatus>) (status =>
        {
          if (status.FailureCount() > 0)
          {
            ContactSupportOutagePage.Start(status).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (allowContact =>
            {
              if (allowContact)
              {
                WaUriParams uriParams = new WaUriParams();
                uriParams.AddBool("PageReplace", true);
                uriParams.AddString("context", "about");
                if (App.CurrentApp.Connection.IsConnected)
                  NavUtils.NavigateToPage(this.NavigationService, "ContactSupportPage", uriParams);
                else
                  NavUtils.NavigateToPage(this.NavigationService, "ConnectionHelp", uriParams);
              }
              else
                this.NavigationService.GoBack();
            }));
          }
          else
          {
            WaUriParams uriParams = new WaUriParams();
            uriParams.AddString("context", ContactSupportHelper.AppendPhoneNumberIfNotLoggedIn("about"));
            if (App.CurrentApp.Connection.IsConnected)
              NavUtils.NavigateToPage(this.NavigationService, "ContactSupportPage", uriParams);
            else
              NavUtils.NavigateToPage(this.NavigationService, "ConnectionHelp", uriParams);
          }
          this.serverStatusSub.SafeDispose();
          this.serverStatusSub = (IDisposable) null;
        }), (Action<Exception>) (ex =>
        {
          NavUtils.NavigateToPage(this.NavigationService, "ConnectionHelp");
          this.serverStatusSub.SafeDispose();
          this.serverStatusSub = (IDisposable) null;
        }), (Action) (() =>
        {
          this.serverStatusSub.SafeDispose();
          this.serverStatusSub = (IDisposable) null;
        }));
      }
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is Action tag))
        return;
      tag();
    }

    private void UpdateApp_Click() => UpdateVersion.UpdateApp();

    private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      ((TextBlock) sender).Foreground = this.Resources[(object) "PhoneChromeBrush"] as Brush;
    }

    private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      ((TextBlock) sender).Foreground = this.Resources[(object) "PhoneAccentBrush"] as Brush;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/About.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.imagePanel = (Grid) this.FindName("imagePanel");
      this.UpdateOsText = (RichTextBlock) this.FindName("UpdateOsText");
      this.ButtonsList = (ItemsControl) this.FindName("ButtonsList");
      this.ViewTermsButton = (TextBlock) this.FindName("ViewTermsButton");
      this.ViewLicensesButton = (TextBlock) this.FindName("ViewLicensesButton");
    }

    public class ButtonContent
    {
      public BitmapSource Icon { get; set; }

      public string Text { get; set; }

      public Action OnClick { get; set; }
    }
  }
}
