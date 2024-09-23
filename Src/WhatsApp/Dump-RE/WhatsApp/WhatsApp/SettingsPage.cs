// Decompiled with JetBrains decompiler
// Type: WhatsApp.SettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class SettingsPage : PhoneApplicationPage
  {
    public List<SettingsPage.SettingItem> settingItems;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector ItemsList;
    private bool _contentLoaded;

    public SettingsPage()
    {
      this.InitializeComponent();
      this.InitPage();
    }

    private void InitPage()
    {
      this.TitlePanel.SmallTitle = Constants.OffcialNameUpper;
      this.TitlePanel.LargeTitle = AppResources.SettingsTitle;
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.settingItems = new List<SettingsPage.SettingItem>()
      {
        new SettingsPage.SettingItem()
        {
          Title = AppResources.About,
          Details = Settings.IsUpdateAvailable ? AppResources.AboutSettingsDescriptionPlusUpdate : AppResources.AboutSettingsDescription,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "About", folderName: "Pages/Settings")),
          AutomationId = "AIdAbout"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.ContactsSettingsPageTitle,
          Details = AppResources.ContactsSettingsDescription,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ContactsSettingsPage", folderName: "Pages/Settings")),
          AutomationId = "AIdContact"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.ProfileTitle,
          Details = AppResources.ProfileSettingsDescriptionPostStatusV2Revived,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ProfilePage", folderName: "Pages/Settings")),
          AutomationId = "AIdProfile"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.AccountSettingsTitle,
          Details = AppResources.AccountSettingsDescription,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "AccountPage", folderName: "Pages/Settings")),
          AutomationId = "AIdAccount"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.ChatsAndCallsSettingsTitle,
          Details = AppResources.ChatSettingsDescription,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ChatSettingsPage", folderName: "Pages/Settings")),
          AutomationId = "AIdChats"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.LockScreenSettings,
          Details = AppResources.LockScreenSettingsDescription,
          OnTap = (Action) (() => NavUtils.NavigateExternal("ms-settings-lock:")),
          AutomationId = "AIdLock"
        },
        new SettingsPage.SettingItem()
        {
          Title = AppResources.CustomAlertsTitle,
          Details = AppResources.NotificationSettingsExplanation,
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "NotificationSettingsPage", folderName: "Pages/Settings")),
          AutomationId = "AIdAlerts"
        }
      };
      this.settingItems.Add(new SettingsPage.SettingItem()
      {
        Title = AppResources.ScreenRotationSettings,
        Details = AppResources.ScreenRotationSettingsExplanation,
        OnTap = (Action) (() => NavUtils.NavigateExternal("ms-settings-screenrotation:")),
        AutomationId = "AIdRotation"
      });
      if (Settings.IsWaAdmin)
        this.settingItems.Add(new SettingsPage.SettingItem()
        {
          Title = "wadmin menu",
          Details = "waa-de-min!",
          OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "WaAdminPage")),
          AutomationId = "AIdWaAdmin"
        });
      this.ItemsList.ItemsSource = (IList) this.settingItems;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (e.NavigationMode != NavigationMode.New)
        return;
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.SETTINGS);
    }

    private void OnItemTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is Action tag))
        return;
      tag();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/SettingsPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ItemsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ItemsList");
    }

    public class SettingItem : WaViewModelBase
    {
      private string title;
      private string details;
      private bool isEnabled = true;
      private string automationId;

      public string Title
      {
        get => this.title.ToLangFriendlyLower();
        set
        {
          if (!(this.title != value))
            return;
          this.title = value;
          this.NotifyPropertyChanged(nameof (Title));
        }
      }

      public string Details
      {
        get => this.details;
        set
        {
          if (!(this.details != value))
            return;
          this.details = value;
          this.NotifyPropertyChanged(nameof (Details));
        }
      }

      public Action OnTap { get; set; }

      public bool IsEnabled
      {
        get => this.isEnabled;
        set
        {
          if (this.isEnabled == value)
            return;
          this.isEnabled = value;
          this.NotifyPropertyChanged(nameof (IsEnabled));
          this.NotifyPropertyChanged("Opacity");
        }
      }

      public double Opacity => !this.isEnabled ? 0.5 : 1.0;

      public string AutomationId
      {
        get => this.automationId;
        set => this.automationId = value;
      }

      public SettingItem()
      {
      }

      public SettingItem(string titleIn, string detailsIn)
      {
        this.title = titleIn;
        this.details = detailsIn;
      }
    }
  }
}
