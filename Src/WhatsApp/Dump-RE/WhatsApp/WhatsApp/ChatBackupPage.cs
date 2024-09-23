// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatBackupPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsAppNative;
using Windows.Foundation;

#nullable disable
namespace WhatsApp
{
  public class ChatBackupPage : PhoneApplicationPage
  {
    private bool autoMode;
    private string autoUserId;
    private string autoUserName;
    private string autoUserEmail;
    private DateTime? autoLocalTime;
    private BackupProperties autoCloudProps;
    private OneDriveBackupState? autoBackupState;
    private OneDriveBkupRestStopReason? autoBackupStopReason;
    private OneDriveBackupStopError? autoBackupStopError;
    private bool? autoBackupIncomplete;
    private OneDriveRestoreState? autoRestoreState;
    private OneDriveBkupRestStopReason? autoRestoreStopReason;
    private OneDriveRestoreStopError? autoRestoreStopError;
    private bool? autoRestoreIncomplete;
    private long? autoProgressMaximum;
    private long? autoProgressValue;
    private bool? autoProgressIndeterminate;
    private OneDriveBackupFrequency? autoOneDriveBackupFrequency;
    private WAAccountSettingsPane accountSettingsPane;
    private GlobalProgressIndicator progressIndicator;
    private IDisposable settingsSub;
    private Brush progressForegroundBrush;
    private bool showingRetryOption;
    private bool readyForRestore;
    private bool readyForBackup;
    private bool authInProgressState;
    private bool backupDisabledState;
    private OneDriveLastBackupState currentOneDriveBackupState;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock LastBackupTitleBlock;
    internal TextBlock LocalTimeBlock;
    internal TextBlock CloudTimeBlock;
    internal TextBlock BackupSizeBlock;
    internal Border OneDriveBackupError;
    internal TextBlock OneDriveLastResultText;
    internal Button BackupRetryButton;
    internal Image BackupRetryButtonIcon;
    internal TextBlock BackupInfoLabel;
    internal Button BackupButton;
    internal Grid BackupProgressArea;
    internal ProgressBar BackupProgressBar;
    internal Button BackupCancelButton;
    internal Image BackupCancelButtonIcon;
    internal TextBlock BackupProgressLabel;
    internal TextBlock BackupSettingsTitleBlock;
    internal StackPanel RestoreNetworkPanel;
    internal ListPicker RestoreNetworkPicker;
    internal ListPicker BackupFrequencyPicker;
    internal TextBlock BackupAccountTitleLabel;
    internal Button BackupAccountButton;
    internal ListPicker BackupNetworkPicker;
    internal ToggleSwitch IncludeVideosToggle;
    internal TextBlock IncludeVideosTooltipBlock;
    internal TextBlock RevokeLabel;
    internal TextBlock RevokeLink;
    private bool _contentLoaded;

    public ChatBackupPage()
    {
      this.InitializeComponent();
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      this.progressForegroundBrush = this.BackupProgressBar.Foreground;
      this.BackupFrequencyPicker.ItemsSource = (IEnumerable) new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>[5]
      {
        new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>()
        {
          Name = AppResources.OneDriveBackupFrequencyNever,
          Value = OneDriveBackupFrequency.Off
        },
        new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>()
        {
          Name = string.Format(AppResources.OneDriveBackupFrequencyManual, (object) AppResources.BackupButton),
          Value = OneDriveBackupFrequency.Manual
        },
        new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>()
        {
          Name = AppResources.OneDriveBackupFrequencyDaily,
          Value = OneDriveBackupFrequency.Daily
        },
        new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>()
        {
          Name = AppResources.OneDriveBackupFrequencyWeekly,
          Value = OneDriveBackupFrequency.Weekly
        },
        new ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency>()
        {
          Name = AppResources.OneDriveBackupFrequencyMonthly,
          Value = OneDriveBackupFrequency.Monthly
        }
      };
      this.BackupNetworkPicker.ItemsSource = (IEnumerable) new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>[2]
      {
        new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>()
        {
          Name = AppResources.OneDriveBackupOnWiFi,
          Value = AutoDownloadSetting.EnabledOnWifi
        },
        new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>()
        {
          Name = AppResources.OneDriveBackupOnData,
          Value = AutoDownloadSetting.Enabled
        }
      };
      this.BackupNetworkPicker.SelectedIndex = 0;
      this.RestoreNetworkPicker.ItemsSource = (IEnumerable) new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>[2]
      {
        new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>()
        {
          Name = AppResources.OneDriveBackupOnWiFi,
          Value = AutoDownloadSetting.EnabledOnWifi
        },
        new ChatBackupPage.BackupSettingOption<AutoDownloadSetting>()
        {
          Name = AppResources.OneDriveBackupOnData,
          Value = AutoDownloadSetting.Enabled
        }
      };
      this.RestoreNetworkPicker.SelectedIndex = 0;
      this.SetBackupFrequencyPicker(Settings.OneDriveBackupFrequency);
      AutoDownloadSetting driveBackupNetwork = Settings.OneDriveBackupNetwork;
      int num1 = 0;
      foreach (object obj in this.BackupNetworkPicker.ItemsSource)
      {
        if (obj is ChatBackupPage.BackupSettingOption<AutoDownloadSetting> backupSettingOption && backupSettingOption.Value == driveBackupNetwork)
        {
          this.BackupNetworkPicker.SelectedIndex = num1;
          break;
        }
        ++num1;
      }
      AutoDownloadSetting driveRestoreNetwork = Settings.OneDriveRestoreNetwork;
      int num2 = 0;
      foreach (object obj in this.RestoreNetworkPicker.ItemsSource)
      {
        if (obj is ChatBackupPage.BackupSettingOption<AutoDownloadSetting> backupSettingOption && backupSettingOption.Value == driveRestoreNetwork)
        {
          this.RestoreNetworkPicker.SelectedIndex = num2;
          break;
        }
        ++num2;
      }
      this.IncludeVideosToggle.IsChecked = new bool?(Settings.OneDriveIncludeVideos);
      try
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          this.BackupInfoLabel.Text = !nativeMediaStorage.SdCardExists() ? AppResources.OneDriveBackupGeneralInfoInternal : AppResources.OneDriveBackupGeneralInfoSd;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "backup settings");
        this.BackupInfoLabel.Text = AppResources.OneDriveBackupGeneralInfoInternal;
      }
      this.BackupFrequencyPicker.SelectionChanged += new SelectionChangedEventHandler(this.BackupFrequencyPicker_SelectionChanged);
      this.BackupNetworkPicker.SelectionChanged += new SelectionChangedEventHandler(this.BackupNetworkPicker_SelectionChanged);
      this.RestoreNetworkPicker.SelectionChanged += new SelectionChangedEventHandler(this.RestoreNetworkPicker_SelectionChanged);
      this.IncludeVideosToggle.Checked += new EventHandler<RoutedEventArgs>(this.IncludeVideosToggle_Checked);
      this.IncludeVideosToggle.Unchecked += new EventHandler<RoutedEventArgs>(this.IncludeVideosToggle_Unchecked);
      this.RevokeLabel.Text = AppResources.OneDriveManageAccessLinkLabel;
      this.RevokeLink.Text = "https://account.live.com/consent/Manage";
      this.settingsSub = Settings.GetSettingsChangedObservable(new Settings.Key[4]
      {
        Settings.Key.OneDriveUserId,
        Settings.Key.OneDriveUserDisplayName,
        Settings.Key.OneDriveBackupStatus,
        Settings.Key.OneDriveRestoreNetwork
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>(new Action<Settings.Key>(this.OnSettingsChanged));
      if (OneDriveUtils.UseWP10AccountInterface())
      {
        if (!OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane = NativeInterfaces.CreateInstance<WAAccountSettingsPane>()), "ctor in ChatBackupPage"))
        {
          ((IDisposable) this.accountSettingsPane).SafeDispose();
          this.accountSettingsPane = (WAAccountSettingsPane) null;
        }
        else
        {
          WAAccountSettingsPane accountSettingsPane = this.accountSettingsPane;
          // ISSUE: method pointer
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<WAAccountSettingsPane, WAWebAccountProviderInvokedEventArgs>>(new Func<TypedEventHandler<WAAccountSettingsPane, WAWebAccountProviderInvokedEventArgs>, EventRegistrationToken>(accountSettingsPane.add_WebAccountProviderInvoked), new Action<EventRegistrationToken>(accountSettingsPane.remove_WebAccountProviderInvoked), new TypedEventHandler<WAAccountSettingsPane, WAWebAccountProviderInvokedEventArgs>((object) this, __methodptr(AccountSettingsPane_WebAccountProviderInvoked)));
        }
      }
      this.BackupRetryButtonIcon.Source = (System.Windows.Media.ImageSource) AssetStore.RetryIcon;
      this.BackupCancelButtonIcon.Source = (System.Windows.Media.ImageSource) AssetStore.CancelIcon;
      if (OneDriveRestoreManager.IsRestoreIncomplete)
        this.InitForRestore();
      else
        this.InitForBackup();
    }

    private void InitForRestore()
    {
      this.DisconnectFromBackup();
      if (!this.readyForRestore)
      {
        this.RestoreNetworkPanel.Visibility = Visibility.Visible;
        OneDriveRestoreManager.Instance.BackupPropertiesChanged += new EventHandler<BackupProperties>(this.OneDrive_BackupPropertiesChanged);
        OneDriveRestoreManager.Instance.ProgressMaximumChanged += new EventHandler<long>(this.OneDrive_ProgressMaximumChanged);
        OneDriveRestoreManager.Instance.ProgressValueChanged += new EventHandler<long>(this.OneDrive_ProgressValueChanged);
        OneDriveRestoreManager.Instance.IsProgressIndeterminateChanged += new EventHandler<bool>(this.OneDrive_IsProgressIndeterminateChanged);
        OneDriveRestoreManager.Instance.StateChanged += new EventHandler<OneDriveRestoreState>(this.OneDrive_RestoreStateChanged);
        OneDriveRestoreManager.Instance.RestoreStopped += new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_RestoreStopped);
        this.readyForRestore = true;
      }
      this.UpdateAccountItem();
      this.UpdateLocalBackupItem();
      this.UpdateCloudBackupItem();
      this.UpdateRestoreStateItem();
    }

    private void DisconnectFromRestore()
    {
      if (!this.readyForRestore)
        return;
      this.RestoreNetworkPanel.Visibility = Visibility.Collapsed;
      OneDriveRestoreManager.Instance.BackupPropertiesChanged -= new EventHandler<BackupProperties>(this.OneDrive_BackupPropertiesChanged);
      OneDriveRestoreManager.Instance.ProgressMaximumChanged -= new EventHandler<long>(this.OneDrive_ProgressMaximumChanged);
      OneDriveRestoreManager.Instance.ProgressValueChanged -= new EventHandler<long>(this.OneDrive_ProgressValueChanged);
      OneDriveRestoreManager.Instance.IsProgressIndeterminateChanged -= new EventHandler<bool>(this.OneDrive_IsProgressIndeterminateChanged);
      OneDriveRestoreManager.Instance.StateChanged -= new EventHandler<OneDriveRestoreState>(this.OneDrive_RestoreStateChanged);
      OneDriveRestoreManager.Instance.RestoreStopped -= new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_RestoreStopped);
      this.readyForRestore = false;
    }

    private void InitForBackup()
    {
      this.DisconnectFromRestore();
      if (!this.readyForBackup)
      {
        OneDriveBackupManager.Instance.IsAuthenticationInProgressChanged += new EventHandler<bool>(this.OneDrive_IsAuthenticationInProgressChanged);
        OneDriveBackupManager.Instance.BackupPropertiesChanged += new EventHandler<BackupProperties>(this.OneDrive_BackupPropertiesChanged);
        OneDriveBackupManager.Instance.ProgressMaximumChanged += new EventHandler<long>(this.OneDrive_ProgressMaximumChanged);
        OneDriveBackupManager.Instance.ProgressValueChanged += new EventHandler<long>(this.OneDrive_ProgressValueChanged);
        OneDriveBackupManager.Instance.IsProgressIndeterminateChanged += new EventHandler<bool>(this.OneDrive_IsProgressIndeterminateChanged);
        OneDriveBackupManager.Instance.StateChanged += new EventHandler<OneDriveBackupState>(this.OneDrive_StateChanged);
        OneDriveBackupManager.Instance.BackupStopped += new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_BackupStopped);
        this.readyForBackup = true;
      }
      this.OneDrive_IsAuthenticationInProgressChanged((object) OneDriveBackupManager.Instance, OneDriveBackupManager.Instance.IsAuthenticationInProgress);
      if (OneDriveBackupManager.Instance.State == OneDriveBackupState.Idle && OneDriveBackupManager.Instance.StopReason != OneDriveBkupRestStopReason.StopError && OneDriveBackupManager.IsBackupIncomplete)
        OneDriveBackupManager.Instance.Start(OneDriveBkupRestTrigger.ForegroundResume);
      this.UpdateAccountItem();
      this.UpdateLocalBackupItem();
      this.UpdateCloudBackupItem();
      this.UpdateBackupStateItem();
      this.UpdateOneDriveBackupState();
    }

    private void DisconnectFromBackup()
    {
      if (!this.readyForBackup)
        return;
      OneDriveBackupManager.Instance.IsAuthenticationInProgressChanged -= new EventHandler<bool>(this.OneDrive_IsAuthenticationInProgressChanged);
      OneDriveBackupManager.Instance.BackupPropertiesChanged -= new EventHandler<BackupProperties>(this.OneDrive_BackupPropertiesChanged);
      OneDriveBackupManager.Instance.ProgressMaximumChanged -= new EventHandler<long>(this.OneDrive_ProgressMaximumChanged);
      OneDriveBackupManager.Instance.ProgressValueChanged -= new EventHandler<long>(this.OneDrive_ProgressValueChanged);
      OneDriveBackupManager.Instance.IsProgressIndeterminateChanged -= new EventHandler<bool>(this.OneDrive_IsProgressIndeterminateChanged);
      OneDriveBackupManager.Instance.StateChanged -= new EventHandler<OneDriveBackupState>(this.OneDrive_StateChanged);
      OneDriveBackupManager.Instance.BackupStopped -= new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_BackupStopped);
      this.readyForBackup = false;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane?.OnCurrentViewNavigatedTo()), "accountSettingsPane?.OnCurrentViewNavigatedTo() in ChatBackupPage"))
        return;
      ((IDisposable) this.accountSettingsPane).SafeDispose();
      this.accountSettingsPane = (WAAccountSettingsPane) null;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (!OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane?.OnCurrentViewNavigatedFrom()), "accountSettingsPane?.OnCurrentViewNavigatedFrom() in ChatBackupPage"))
      {
        ((IDisposable) this.accountSettingsPane).SafeDispose();
        this.accountSettingsPane = (WAAccountSettingsPane) null;
      }
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.BackupFrequencyPicker.SelectionChanged -= new SelectionChangedEventHandler(this.BackupFrequencyPicker_SelectionChanged);
      this.BackupNetworkPicker.SelectionChanged -= new SelectionChangedEventHandler(this.BackupNetworkPicker_SelectionChanged);
      this.RestoreNetworkPicker.SelectionChanged -= new SelectionChangedEventHandler(this.RestoreNetworkPicker_SelectionChanged);
      this.IncludeVideosToggle.Checked -= new EventHandler<RoutedEventArgs>(this.IncludeVideosToggle_Checked);
      this.IncludeVideosToggle.Unchecked -= new EventHandler<RoutedEventArgs>(this.IncludeVideosToggle_Unchecked);
      ((IDisposable) this.accountSettingsPane).SafeDispose();
      this.accountSettingsPane = (WAAccountSettingsPane) null;
      this.settingsSub.SafeDispose();
      this.settingsSub = (IDisposable) null;
      this.DisconnectFromRestore();
      this.DisconnectFromBackup();
    }

    private void BackupButton_Click(object sender, RoutedEventArgs e)
    {
      if (OneDriveBackupManager.Instance.State != OneDriveBackupState.Idle)
        return;
      OneDriveBackupManager.Instance.Start(OneDriveBkupRestTrigger.UserInteraction);
    }

    private void BackupCancelButton_Click(object sender, EventArgs e)
    {
      if (this.readyForRestore)
      {
        Log.l("odm", "User restore cancel {0} {1}", (object) OneDriveRestoreManager.Instance.State, (object) OneDriveRestoreManager.Instance.StopReason);
        if (!OneDriveRestoreManager.IsRestoreIncomplete)
          return;
        UIUtils.Decision(AppResources.OneDriveAbortRestore, AppResources.Yes, AppResources.No).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          try
          {
            OneDriveRestoreManager.Instance.Abort(OneDriveRestoreStopError.CancelledByUser);
          }
          catch (Exception ex)
          {
          }
        }));
      }
      else
      {
        Log.l("odm", "User backup cancel {0} {1}", (object) OneDriveBackupManager.Instance.State, (object) OneDriveBackupManager.Instance.StopReason);
        if (!OneDriveBackupManager.IsBackupIncomplete)
          return;
        UIUtils.Decision(AppResources.OneDriveAbortBackup, AppResources.Yes, AppResources.No).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          try
          {
            OneDriveBackupManager.Instance.Abort(OneDriveBackupStopError.CancelledByUser);
          }
          catch (Exception ex)
          {
          }
        }));
      }
    }

    private void BackupRetryButton_Click(object sender, EventArgs e)
    {
      if (this.readyForRestore)
        OneDriveRestoreManager.Instance.Start(OneDriveBkupRestTrigger.ForegroundResume);
      else
        OneDriveBackupManager.Instance.Start(OneDriveBkupRestTrigger.ForegroundResume);
    }

    private void BackupRetry_Click(object sender, RoutedEventArgs e)
    {
      OneDriveLastBackupState driveBackupState = this.currentOneDriveBackupState;
      int num = driveBackupState != null ? driveBackupState.Id : -1;
      Log.l(nameof (ChatBackupPage), "retry in state {0}", (object) num);
      switch (num)
      {
        case 2:
          if (!this.BackupButton.IsEnabled || this.BackupButton.Visibility != Visibility.Visible)
            break;
          this.BackupButton_Click(sender, e);
          break;
        case 4:
          if (!this.BackupAccountButton.IsEnabled || this.BackupAccountButton.Visibility != Visibility.Visible)
            break;
          this.BackupAccountButton_Click(sender, e);
          break;
        case 7:
          this.BackupRetryButton_Click(sender, (EventArgs) e);
          break;
        default:
          if (!this.BackupButton.IsEnabled || this.BackupButton.Visibility != Visibility.Visible)
            break;
          this.BackupButton_Click(sender, e);
          break;
      }
    }

    private void BackupAccountButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.accountSettingsPane != null && !OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane.Show()), "accountSettingsPane.Show() in ChatBackupPage"))
      {
        ((IDisposable) this.accountSettingsPane).SafeDispose();
        this.accountSettingsPane = (WAAccountSettingsPane) null;
      }
      if (this.accountSettingsPane != null)
        return;
      NavUtils.NavigateExternalByProductId("{AD543082-80EC-45BB-AA02-FFE7F4182BA8}");
    }

    private async void AccountSettingsPane_WebAccountProviderInvoked(
      WAAccountSettingsPane sender,
      WAWebAccountProviderInvokedEventArgs args)
    {
      if (await OneDriveBackupManager.Instance.AuthenticateUser(args.Provider))
        return;
      Settings.OneDriveBackupFrequency = OneDriveBackupFrequency.Off;
      this.SetBackupFrequencyPicker(OneDriveBackupFrequency.Off);
    }

    private void RevokeLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri(this.RevokeLink.Text)
      }.Show();
    }

    private async void BackupFrequencyPicker_SelectionChanged(
      object sender,
      SelectionChangedEventArgs e)
    {
      bool flag = false;
      if (this.BackupFrequencyPicker.SelectedItem is ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency> selectedItem)
      {
        if (Settings.OneDriveBackupFrequency == OneDriveBackupFrequency.Off && selectedItem.Value != OneDriveBackupFrequency.Off)
          flag = true;
        Settings.OneDriveBackupFrequency = selectedItem.Value;
        this.UpdateOneDriveBackupState();
      }
      if (!flag || this.autoMode || this.readyForRestore)
        return;
      if (OneDriveUtils.UseWP10AccountInterface() && this.accountSettingsPane != null && string.IsNullOrEmpty(Settings.OneDriveUserAccountId) && !OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane.Show()), "accountSettingsPane.Show() in ChatBackupPage frequency change"))
      {
        ((IDisposable) this.accountSettingsPane).SafeDispose();
        this.accountSettingsPane = (WAAccountSettingsPane) null;
      }
      if (this.accountSettingsPane != null)
        return;
      if (await OneDriveBackupManager.Instance.AuthenticateUser())
        return;
      Settings.OneDriveBackupFrequency = OneDriveBackupFrequency.Off;
      this.SetBackupFrequencyPicker(OneDriveBackupFrequency.Off);
    }

    private void BackupNetworkPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!(this.BackupNetworkPicker.SelectedItem is ChatBackupPage.BackupSettingOption<AutoDownloadSetting> selectedItem))
        return;
      Settings.OneDriveBackupNetwork = selectedItem.Value;
    }

    private void RestoreNetworkPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!(this.RestoreNetworkPicker.SelectedItem is ChatBackupPage.BackupSettingOption<AutoDownloadSetting> selectedItem))
        return;
      Settings.OneDriveRestoreNetwork = selectedItem.Value;
    }

    private void IncludeVideosToggle_Checked(object sender, RoutedEventArgs e)
    {
      Settings.OneDriveIncludeVideos = ((int) this.IncludeVideosToggle.IsChecked ?? 0) != 0;
    }

    private void IncludeVideosToggle_Unchecked(object sender, RoutedEventArgs e)
    {
      Settings.OneDriveIncludeVideos = ((int) this.IncludeVideosToggle.IsChecked ?? 0) != 0;
    }

    private void OneDrive_IsAuthenticationInProgressChanged(object sender, bool e)
    {
      if (e && !this.authInProgressState)
      {
        this.authInProgressState = true;
        this.progressIndicator.Acquire();
        this.BackupButton.IsEnabled = false;
        this.BackupAccountButton.IsEnabled = false;
        this.BackupFrequencyPicker.IsEnabled = false;
        this.BackupNetworkPicker.IsEnabled = false;
        this.RestoreNetworkPicker.IsEnabled = false;
        this.IncludeVideosToggle.IsEnabled = false;
      }
      else
      {
        if (e || !this.authInProgressState)
          return;
        this.authInProgressState = false;
        this.BackupButton.IsEnabled = !this.backupDisabledState;
        this.BackupAccountButton.IsEnabled = true;
        this.BackupFrequencyPicker.IsEnabled = true;
        this.BackupNetworkPicker.IsEnabled = true;
        this.RestoreNetworkPicker.IsEnabled = false;
        this.IncludeVideosToggle.IsEnabled = true;
        this.progressIndicator.Release();
      }
    }

    private void OneDrive_BackupPropertiesChanged(object sender, BackupProperties e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.UpdateCloudBackupItem()));
    }

    private void OneDrive_ProgressMaximumChanged(object sender, long e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (this.showingRetryOption || this.OneDriveBackupError.Visibility != Visibility.Collapsed)
          return;
        this.BackupProgressBar.Maximum = (double) e;
      }));
    }

    private void OneDrive_ProgressValueChanged(object sender, long e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (!this.showingRetryOption && this.OneDriveBackupError.Visibility == Visibility.Collapsed)
          this.BackupProgressBar.Value = (double) e;
        if (this.readyForRestore)
        {
          if (OneDriveRestoreManager.Instance.State != OneDriveRestoreState.RestoringMedia)
            return;
          this.BackupProgressLabel.Text = this.BuildMediaRestoreProgressMessage();
        }
        else
        {
          switch (OneDriveBackupManager.Instance.State)
          {
            case OneDriveBackupState.LocalBackup:
              this.BackupProgressLabel.Text = this.BuildLocalProgressMessage();
              break;
            case OneDriveBackupState.UploadingDatabases:
            case OneDriveBackupState.UploadingMedia:
              this.BackupProgressLabel.Text = this.BuildUploadProgressMessage();
              break;
          }
        }
      }));
    }

    private void OneDrive_IsProgressIndeterminateChanged(object sender, bool e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (this.showingRetryOption || this.OneDriveBackupError.Visibility != Visibility.Collapsed)
          return;
        this.BackupProgressBar.IsIndeterminate = e;
      }));
    }

    private void OneDrive_RestoreStateChanged(object sender, OneDriveRestoreState e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.UpdateRestoreStateItem();
        this.UpdateLocalBackupItem();
        this.UpdateOneDriveBackupState();
      }));
    }

    private void OneDrive_RestoreStopped(object sender, BkupRestStoppedEventArgs e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.UpdateRestoreStateItem();
        this.UpdateLocalBackupItem();
        this.UpdateCloudBackupItem();
        if (e.Reason != OneDriveBkupRestStopReason.Completed)
          return;
        this.InitForBackup();
      }));
    }

    private void OneDrive_StateChanged(object sender, OneDriveBackupState e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.UpdateBackupStateItem();
        this.UpdateLocalBackupItem();
        this.UpdateOneDriveBackupState();
      }));
    }

    private void OneDrive_BackupStopped(object sender, BkupRestStoppedEventArgs e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.UpdateBackupStateItem();
        this.UpdateLocalBackupItem();
        this.UpdateCloudBackupItem();
        this.UpdateOneDriveBackupState();
      }));
    }

    private void OnSettingsChanged(Settings.Key changedItem)
    {
      if (changedItem == Settings.Key.OneDriveUserId || changedItem == Settings.Key.OneDriveUserDisplayName)
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.UpdateAccountItem()));
      if (changedItem == Settings.Key.OneDriveBackupStatus)
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.UpdateOneDriveBackupState()));
      if (changedItem != Settings.Key.OneDriveRestoreNetwork || !this.readyForRestore || OneDriveRestoreManager.Instance.State != OneDriveRestoreState.Idle)
        return;
      OneDriveRestoreManager.Instance.Start(OneDriveBkupRestTrigger.UserInteraction);
    }

    private void UpdateAccountItem()
    {
      string str1 = this.autoUserId ?? Settings.OneDriveUserId;
      string str2 = this.autoUserName ?? Settings.OneDriveUserDisplayName;
      string str3 = this.autoUserEmail ?? Settings.OneDriveUserAccountEmail;
      if (!string.IsNullOrEmpty(str1))
      {
        TextBlock textBlock = new TextBlock()
        {
          TextWrapping = TextWrapping.Wrap
        };
        this.BackupAccountTitleLabel.Visibility = Visibility.Visible;
        this.BackupAccountButton.Visibility = Visibility.Visible;
        this.RevokeLabel.Visibility = Visibility.Visible;
        this.RevokeLink.Visibility = Visibility.Visible;
        textBlock.Text = string.IsNullOrEmpty(str3) ? str2 : string.Format("{0}\n{1}", (object) str2, (object) str3);
        this.BackupAccountButton.Content = (object) textBlock;
      }
      else
      {
        this.BackupAccountTitleLabel.Visibility = Visibility.Collapsed;
        this.BackupAccountButton.Visibility = Visibility.Collapsed;
        this.RevokeLabel.Visibility = Visibility.Collapsed;
        this.RevokeLink.Visibility = Visibility.Collapsed;
      }
    }

    private void UpdateOneDriveBackupState()
    {
      this.currentOneDriveBackupState = (OneDriveLastBackupState) null;
      OneDriveBackupFrequency? driveBackupFrequency = this.autoOneDriveBackupFrequency;
      if (((int) driveBackupFrequency ?? (int) Settings.OneDriveBackupFrequency) != 0)
        this.currentOneDriveBackupState = OneDriveBackupStatusReporting.GetLastBackupState();
      Visibility visibility = Visibility.Collapsed;
      if (this.currentOneDriveBackupState != null)
      {
        if (this.currentOneDriveBackupState.DisplayFlag == 1)
        {
          visibility = Visibility.Visible;
          this.OneDriveLastResultText.Text = OneDriveBackupStatusReporting.CreateDisplayString(this.currentOneDriveBackupState.Id, this.currentOneDriveBackupState.Parm1, this.currentOneDriveBackupState.Parm2);
        }
        else if (this.currentOneDriveBackupState.Id == 1)
        {
          long num = DateTime.UtcNow.Ticks - this.currentOneDriveBackupState.TimestampTicks;
          bool flag = false;
          driveBackupFrequency = this.autoOneDriveBackupFrequency;
          switch ((int) driveBackupFrequency ?? (int) Settings.OneDriveBackupFrequency)
          {
            case 2:
              flag = num > 1728000000000L;
              break;
            case 3:
              flag = num > 7776000000000L;
              break;
            case 4:
              flag = num > 28512000000000L;
              break;
          }
          if (flag)
          {
            visibility = Visibility.Visible;
            this.OneDriveLastResultText.Text = AppResources.OneDriveBackupErrorNotRunRecently;
          }
        }
      }
      this.OneDriveBackupError.Visibility = visibility;
    }

    private void UpdateLocalBackupItem()
    {
      Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        DateTime? nullable = new DateTime?();
        try
        {
          if (Settings.CorruptDb || !Backup.CanBackup())
          {
            this.backupDisabledState = true;
            this.BackupButton.IsEnabled = false;
          }
          else
            nullable = Backup.GetLastBackupTime();
        }
        catch (Exception ex)
        {
        }
        string str = !this.autoLocalTime.HasValue ? (!nullable.HasValue || this.autoMode ? AppResources.BackupSummary : DateTimeUtils.FormatCompact(nullable.Value, DateTimeUtils.TimeDisplay.SameWeekOnly, true)) : DateTimeUtils.FormatCompact(this.autoLocalTime.Value, DateTimeUtils.TimeDisplay.SameWeekOnly, true);
        observer.OnNext(str);
        return (Action) (() => { });
      })).Take<string>(1).SubscribeOn<string>((IScheduler) AppState.Worker).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (s => this.LocalTimeBlock.Text = string.Format(AppResources.LocalBackupTime, (object) s.ToLangFriendlyLower())));
    }

    private void UpdateCloudBackupItem()
    {
      BackupProperties backupProperties;
      long progressValue;
      long progressMaximum;
      if (this.readyForRestore)
      {
        backupProperties = this.autoCloudProps ?? OneDriveRestoreManager.Instance.BackupProperties;
        progressValue = OneDriveRestoreManager.Instance.ProgressValue;
        progressMaximum = OneDriveRestoreManager.Instance.ProgressMaximum;
      }
      else
      {
        backupProperties = this.autoCloudProps ?? OneDriveBackupManager.Instance.BackupProperties;
        progressValue = OneDriveBackupManager.Instance.ProgressValue;
        progressMaximum = OneDriveBackupManager.Instance.ProgressMaximum;
      }
      string str1 = (string) null;
      string str2 = (string) null;
      if (backupProperties != null && backupProperties.InProgress && !string.IsNullOrEmpty(backupProperties.LastBackupId))
      {
        str1 = DateTimeUtils.FormatCompact(backupProperties.LastStartTime, DateTimeUtils.TimeDisplay.SameWeekOnly, true);
        str2 = Utils.FileSizeFormatter.Format(backupProperties.LastSize);
      }
      else if (backupProperties != null && !backupProperties.InProgress && !string.IsNullOrEmpty(backupProperties.BackupId))
      {
        str1 = DateTimeUtils.FormatCompact(backupProperties.StartTime, DateTimeUtils.TimeDisplay.SameWeekOnly, true);
        str2 = Utils.FileSizeFormatter.Format(backupProperties.Size);
      }
      if (!string.IsNullOrEmpty(str1))
      {
        this.CloudTimeBlock.Text = string.Format(AppResources.OneDriveBackupTime, (object) str1.ToLangFriendlyLower());
        this.CloudTimeBlock.Visibility = Visibility.Visible;
      }
      else
        this.CloudTimeBlock.Visibility = Visibility.Collapsed;
      if (!string.IsNullOrEmpty(str2))
      {
        this.BackupSizeBlock.Text = string.Format(AppResources.OneDriveBackupSize, (object) str2);
        this.BackupSizeBlock.Visibility = Visibility.Visible;
      }
      else
        this.BackupSizeBlock.Visibility = Visibility.Collapsed;
      if (this.showingRetryOption || this.OneDriveBackupError.Visibility != Visibility.Collapsed)
        return;
      this.BackupProgressBar.Maximum = (double) progressMaximum;
      this.BackupProgressBar.Value = (double) progressValue;
    }

    private void UpdateRestoreStateItem()
    {
      OneDriveRestoreState driveRestoreState = (OneDriveRestoreState) ((int) this.autoRestoreState ?? (int) OneDriveRestoreManager.Instance.State);
      OneDriveBkupRestStopReason bkupRestStopReason = (OneDriveBkupRestStopReason) ((int) this.autoRestoreStopReason ?? (int) OneDriveRestoreManager.Instance.StopReason);
      OneDriveRestoreStopError restoreStopError = (OneDriveRestoreStopError) ((int) this.autoRestoreStopError ?? (int) OneDriveRestoreManager.Instance.StopError);
      bool flag = ((int) this.autoRestoreIncomplete ?? (OneDriveRestoreManager.IsRestoreIncomplete ? 1 : 0)) != 0;
      bool showProgress = driveRestoreState != OneDriveRestoreState.Idle && driveRestoreState != OneDriveRestoreState.Complete;
      bool showProgressError = false;
      this.BackupProgressLabel.Text = driveRestoreState != OneDriveRestoreState.RestoringMedia ? "" : this.BuildMediaRestoreProgressMessage();
      if (driveRestoreState == OneDriveRestoreState.Idle & flag)
      {
        string str1 = (string) null;
        switch (bkupRestStopReason)
        {
          case OneDriveBkupRestStopReason.StopError:
            showProgressError = true;
            switch (restoreStopError)
            {
              case OneDriveRestoreStopError.Unauthenticated:
                string str2;
                try
                {
                  str2 = ((TextBlock) this.BackupAccountButton.Content).Text;
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "backup account button text extraction failed");
                  str2 = AppResources.OneDriveBackupAccountTitle;
                }
                str1 = string.Format(AppResources.OneDriveRestoreAuthError, (object) str2);
                break;
              case OneDriveRestoreStopError.ServiceNotAvailable:
                str1 = AppResources.OneDriveNotAvailableForRestore;
                break;
              case OneDriveRestoreStopError.LocalDiskFull:
                str1 = AppResources.OneDriveRestoreErrorDeviceFull;
                break;
              case OneDriveRestoreStopError.LoginFailure:
                str1 = AppResources.OneDriveRestoreLoginFailure;
                break;
              case OneDriveRestoreStopError.StoppedByBattery:
                str1 = AppResources.OneDriveRestoreBatteryTooLow;
                break;
              default:
                str1 = AppResources.OneDriveRestoreProgressError;
                break;
            }
            break;
          case OneDriveBkupRestStopReason.NetworkChange:
            str1 = AppResources.OneDriveBackupProgressWaitingForNetwork;
            showProgress = true;
            break;
        }
        this.BackupProgressLabel.Text = str1;
      }
      this.UpdateProgressBarVisibility(showProgress, showProgressError);
    }

    private void UpdateBackupStateItem()
    {
      OneDriveBackupState driveBackupState = (OneDriveBackupState) ((int) this.autoBackupState ?? (int) OneDriveBackupManager.Instance.State);
      OneDriveBkupRestStopReason bkupRestStopReason = (OneDriveBkupRestStopReason) ((int) this.autoBackupStopReason ?? (int) OneDriveBackupManager.Instance.StopReason);
      OneDriveBackupStopError? autoBackupStopError = this.autoBackupStopError;
      if (!autoBackupStopError.HasValue)
      {
        int stopError = (int) OneDriveBackupManager.Instance.StopError;
      }
      else
      {
        int valueOrDefault = (int) autoBackupStopError.GetValueOrDefault();
      }
      bool flag = ((int) this.autoBackupIncomplete ?? (OneDriveBackupManager.IsBackupIncomplete ? 1 : 0)) != 0;
      bool showProgress = driveBackupState != OneDriveBackupState.Idle && driveBackupState != OneDriveBackupState.Complete;
      bool showProgressError = false;
      switch (driveBackupState)
      {
        case OneDriveBackupState.LocalBackup:
          this.BackupProgressLabel.Text = this.BuildLocalProgressMessage();
          break;
        case OneDriveBackupState.InitializingBackup:
        case OneDriveBackupState.Authenticating:
        case OneDriveBackupState.PreparingBackupPath:
        case OneDriveBackupState.SynchronizingManifest:
          this.BackupProgressLabel.Text = AppResources.OneDriveBackupProgressPreparingRemote;
          break;
        case OneDriveBackupState.UploadingDatabases:
        case OneDriveBackupState.UploadingMedia:
          this.BackupProgressLabel.Text = this.BuildUploadProgressMessage();
          break;
        case OneDriveBackupState.SynchronizingManifestFinal:
          this.BackupProgressLabel.Text = AppResources.OneDriveBackupProgressFinishingRemote;
          break;
        default:
          this.BackupProgressLabel.Text = "";
          break;
      }
      if (driveBackupState == OneDriveBackupState.Idle & flag && (bkupRestStopReason == OneDriveBkupRestStopReason.NetworkChange || bkupRestStopReason == OneDriveBkupRestStopReason.StopError))
      {
        showProgress = false;
        showProgressError = true;
        this.BackupProgressLabel.Text = AppResources.OneDriveBackupProgressError;
      }
      this.UpdateProgressBarVisibility(showProgress, showProgressError);
    }

    private void UpdateProgressBarVisibility(bool showProgress, bool showProgressError)
    {
      if (showProgressError)
      {
        this.BackupProgressBar.IsIndeterminate = false;
        this.BackupProgressBar.Maximum = 100.0;
        this.BackupProgressBar.Value = 100.0;
        this.BackupProgressBar.Foreground = (Brush) UIUtils.RedBrush;
        this.BackupProgressArea.Visibility = Visibility.Visible;
        this.BackupProgressLabel.Visibility = Visibility.Visible;
        this.BackupButton.Visibility = Visibility.Collapsed;
        this.showingRetryOption = true;
      }
      else if (showProgress)
      {
        if (this.BackupProgressArea.Visibility != Visibility.Visible || this.showingRetryOption)
        {
          if (this.readyForRestore)
          {
            this.BackupProgressBar.IsIndeterminate = ((int) this.autoProgressIndeterminate ?? (OneDriveRestoreManager.Instance.IsProgressIndeterminate ? 1 : 0)) != 0;
            ProgressBar backupProgressBar1 = this.BackupProgressBar;
            long? nullable = this.autoProgressMaximum;
            double num1 = (double) (nullable ?? OneDriveRestoreManager.Instance.ProgressMaximum);
            backupProgressBar1.Maximum = num1;
            ProgressBar backupProgressBar2 = this.BackupProgressBar;
            nullable = this.autoProgressValue;
            double num2 = (double) (nullable ?? OneDriveRestoreManager.Instance.ProgressValue);
            backupProgressBar2.Value = num2;
          }
          else
          {
            this.BackupProgressBar.IsIndeterminate = ((int) this.autoProgressIndeterminate ?? (OneDriveBackupManager.Instance.IsProgressIndeterminate ? 1 : 0)) != 0;
            ProgressBar backupProgressBar3 = this.BackupProgressBar;
            long? nullable = this.autoProgressMaximum;
            double num3 = (double) (nullable ?? OneDriveBackupManager.Instance.ProgressMaximum);
            backupProgressBar3.Maximum = num3;
            ProgressBar backupProgressBar4 = this.BackupProgressBar;
            nullable = this.autoProgressValue;
            double num4 = (double) (nullable ?? OneDriveBackupManager.Instance.ProgressValue);
            backupProgressBar4.Value = num4;
          }
          this.BackupProgressBar.Foreground = this.progressForegroundBrush;
          this.BackupProgressArea.Visibility = Visibility.Visible;
        }
        this.BackupProgressLabel.Visibility = Visibility.Visible;
        this.BackupButton.Visibility = Visibility.Collapsed;
        this.showingRetryOption = false;
      }
      else
      {
        this.BackupProgressArea.Visibility = Visibility.Collapsed;
        this.BackupProgressLabel.Visibility = Visibility.Collapsed;
        this.BackupButton.Visibility = Visibility.Visible;
        this.showingRetryOption = false;
      }
    }

    private void SetBackupFrequencyPicker(OneDriveBackupFrequency frequencyValue)
    {
      int num = 0;
      foreach (object obj in this.BackupFrequencyPicker.ItemsSource)
      {
        if (obj is ChatBackupPage.BackupSettingOption<OneDriveBackupFrequency> backupSettingOption && backupSettingOption.Value == frequencyValue)
        {
          if (this.BackupFrequencyPicker.SelectedIndex == num)
            break;
          this.BackupFrequencyPicker.SelectedIndex = num;
          this.UpdateOneDriveBackupState();
          break;
        }
        ++num;
      }
    }

    private string BuildMediaRestoreProgressMessage()
    {
      long? nullable = this.autoProgressMaximum;
      long bytes1 = nullable ?? OneDriveRestoreManager.Instance.ProgressMaximum;
      nullable = this.autoProgressValue;
      long bytes2 = nullable ?? OneDriveRestoreManager.Instance.ProgressValue;
      int num = bytes2 >= bytes1 ? 100 : (int) (100.0 * ((double) bytes2 / (double) bytes1));
      return string.Format(AppResources.OneDriveRestoreProgressDownloading, (object) (Utils.FileSizeFormatter.Format(bytes2) ?? "0"), (object) (Utils.FileSizeFormatter.Format(bytes1) ?? "0"), (object) num);
    }

    private string BuildLocalProgressMessage()
    {
      long? nullable = this.autoProgressMaximum;
      long num1 = nullable ?? OneDriveBackupManager.Instance.ProgressMaximum;
      nullable = this.autoProgressValue;
      long num2 = nullable ?? OneDriveBackupManager.Instance.ProgressValue;
      return string.Format(AppResources.OneDriveBackupProgressLocal, (object) (num2 >= num1 ? 100 : (int) (100.0 * ((double) num2 / (double) num1))));
    }

    private string BuildUploadProgressMessage()
    {
      long? nullable = this.autoProgressMaximum;
      long bytes1 = nullable ?? OneDriveBackupManager.Instance.ProgressMaximum;
      nullable = this.autoProgressValue;
      long bytes2 = nullable ?? OneDriveBackupManager.Instance.ProgressValue;
      int num = bytes2 >= bytes1 ? 100 : (int) (100.0 * ((double) bytes2 / (double) bytes1));
      return string.Format(AppResources.OneDriveBackupProgressUploading, (object) (Utils.FileSizeFormatter.Format(bytes2) ?? "0"), (object) (Utils.FileSizeFormatter.Format(bytes1) ?? "0"), (object) num);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/ChatBackupPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.LastBackupTitleBlock = (TextBlock) this.FindName("LastBackupTitleBlock");
      this.LocalTimeBlock = (TextBlock) this.FindName("LocalTimeBlock");
      this.CloudTimeBlock = (TextBlock) this.FindName("CloudTimeBlock");
      this.BackupSizeBlock = (TextBlock) this.FindName("BackupSizeBlock");
      this.OneDriveBackupError = (Border) this.FindName("OneDriveBackupError");
      this.OneDriveLastResultText = (TextBlock) this.FindName("OneDriveLastResultText");
      this.BackupRetryButton = (Button) this.FindName("BackupRetryButton");
      this.BackupRetryButtonIcon = (Image) this.FindName("BackupRetryButtonIcon");
      this.BackupInfoLabel = (TextBlock) this.FindName("BackupInfoLabel");
      this.BackupButton = (Button) this.FindName("BackupButton");
      this.BackupProgressArea = (Grid) this.FindName("BackupProgressArea");
      this.BackupProgressBar = (ProgressBar) this.FindName("BackupProgressBar");
      this.BackupCancelButton = (Button) this.FindName("BackupCancelButton");
      this.BackupCancelButtonIcon = (Image) this.FindName("BackupCancelButtonIcon");
      this.BackupProgressLabel = (TextBlock) this.FindName("BackupProgressLabel");
      this.BackupSettingsTitleBlock = (TextBlock) this.FindName("BackupSettingsTitleBlock");
      this.RestoreNetworkPanel = (StackPanel) this.FindName("RestoreNetworkPanel");
      this.RestoreNetworkPicker = (ListPicker) this.FindName("RestoreNetworkPicker");
      this.BackupFrequencyPicker = (ListPicker) this.FindName("BackupFrequencyPicker");
      this.BackupAccountTitleLabel = (TextBlock) this.FindName("BackupAccountTitleLabel");
      this.BackupAccountButton = (Button) this.FindName("BackupAccountButton");
      this.BackupNetworkPicker = (ListPicker) this.FindName("BackupNetworkPicker");
      this.IncludeVideosToggle = (ToggleSwitch) this.FindName("IncludeVideosToggle");
      this.IncludeVideosTooltipBlock = (TextBlock) this.FindName("IncludeVideosTooltipBlock");
      this.RevokeLabel = (TextBlock) this.FindName("RevokeLabel");
      this.RevokeLink = (TextBlock) this.FindName("RevokeLink");
    }

    public class BackupSettingOption<T>
    {
      public string Name { get; set; }

      public T Value { get; set; }
    }
  }
}
