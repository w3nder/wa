// Decompiled with JetBrains decompiler
// Type: WhatsApp.TwoFactorPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class TwoFactorPage : PhoneApplicationPage
  {
    public List<TwoFactorPage.TwoFactorItem> settingItems;
    internal Grid LayoutRoot;
    internal StackPanel ContentPanel;
    internal Image Padlock;
    internal TextBlock Description;
    internal WhatsApp.CompatibilityShims.LongListSelector ItemsList;
    private bool _contentLoaded;

    public TwoFactorPage()
    {
      this.InitializeComponent();
      this.InitSettingsList();
    }

    private void InitSettingsList()
    {
      this.Padlock.Source = (System.Windows.Media.ImageSource) AssetStore.SecurityPadlockIcon;
      if (Settings.TwoFactorAuthEnabled)
      {
        this.Description.Text = AppResources.TwoStepDescriptionEnabled;
        bool flag = !string.IsNullOrEmpty(Settings.TwoFactorAuthEmail);
        this.settingItems = new List<TwoFactorPage.TwoFactorItem>()
        {
          new TwoFactorPage.TwoFactorItem()
          {
            Title = AppResources.ChangePasscode,
            OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "SetupTwoFactor", "start=change", "Pages/TwoFactor"))
          },
          new TwoFactorPage.TwoFactorItem()
          {
            Title = !flag ? AppResources.AddEmail : AppResources.ChangeEmail,
            OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "SetupTwoFactor", "start=email", "Pages/TwoFactor"))
          },
          new TwoFactorPage.TwoFactorItem()
          {
            Title = AppResources.Disable,
            OnTap = (Action) (() => UIUtils.MessageBox(AppResources.TwoStepVerification, AppResources.DisableTwoStepPrompt, (IEnumerable<string>) new string[2]
            {
              AppResources.Disable,
              AppResources.Cancel
            }, (Action<int>) (i =>
            {
              if (i == 0)
                TwoFactorAuthentication.RemoveSetupCode((Action<bool>) (promptReset => this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
                {
                  this.InitSettingsList();
                  if (!promptReset)
                    return;
                  NavUtils.NavigateHome();
                }))), (Action<int>) (err => this.Dispatcher.BeginInvokeIfNeeded((Action) (() => UIUtils.ShowMessageBox(AppResources.TwoStepVerification, AppResources.TwoStepRemoveError).Subscribe<Unit>()))));
            })))
          }
        };
      }
      else
      {
        this.Description.Text = AppResources.TwoStepDescription;
        this.settingItems = new List<TwoFactorPage.TwoFactorItem>()
        {
          new TwoFactorPage.TwoFactorItem()
          {
            Title = AppResources.Enable,
            OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "SetupTwoFactor", folderName: "Pages/TwoFactor"))
          }
        };
      }
      this.ItemsList.ItemsSource = (IList) this.settingItems;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      this.InitSettingsList();
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
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/TwoFactor/TwoFactorPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.Padlock = (Image) this.FindName("Padlock");
      this.Description = (TextBlock) this.FindName("Description");
      this.ItemsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ItemsList");
    }

    public class TwoFactorItem : WaViewModelBase
    {
      private string title;
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

      public TwoFactorItem()
      {
      }

      public TwoFactorItem(string titleIn) => this.title = titleIn;
    }
  }
}
