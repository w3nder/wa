// Decompiled with JetBrains decompiler
// Type: WhatsApp.CloudBackupCheckPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsAppNative;
using Windows.Foundation;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace WhatsApp
{
  public class CloudBackupCheckPage : PhoneApplicationPage
  {
    private WAAccountSettingsPane accountSettingsPane;
    private OneDriveRestoreProcessor processor;
    private CancellationTokenSource cancellationSource;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal ProgressBar CheckProgress;
    internal TextBlock BackupLocateErrorText;
    internal Grid Decision;
    internal Button CheckForBackupButton;
    internal Button SkipButton;
    private bool _contentLoaded;

    public CloudBackupCheckPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.PageTitle.SmallTitle = AppResources.RestoreMessageHistory;
      this.PageTitle.LargeTitle = "Microsoft OneDrive";
      this.CheckForBackupButton.Content = (object) AppResources.Yes;
      this.SkipButton.Content = (object) AppResources.No;
      this.BackupLocateErrorText.Visibility = Visibility.Collapsed;
      if (OneDriveUtils.UseWP10AccountInterface())
      {
        if (!OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane = NativeInterfaces.CreateInstance<WAAccountSettingsPane>()), "ctor in CloudBackupCheckPage"))
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
      this.processor = new OneDriveRestoreProcessor();
      this.cancellationSource = new CancellationTokenSource();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane?.OnCurrentViewNavigatedTo()), "accountSettingsPane?.OnCurrentViewNavigatedTo() in CloudBackupCheckPage"))
        return;
      ((IDisposable) this.accountSettingsPane).SafeDispose();
      this.accountSettingsPane = (WAAccountSettingsPane) null;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (!OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane?.OnCurrentViewNavigatedFrom()), "accountSettingsPane?.OnNavigatedFrom() in CloudBackupCheckPage"))
      {
        ((IDisposable) this.accountSettingsPane).SafeDispose();
        this.accountSettingsPane = (WAAccountSettingsPane) null;
      }
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      ((IDisposable) this.accountSettingsPane).SafeDispose();
      this.accountSettingsPane = (WAAccountSettingsPane) null;
    }

    private void GoToNextPage()
    {
      if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.VerifiedPendingBackupCheck)
        Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingHistoryRestore;
      this.NavigationService.Navigate(new Uri("/PageSelect?ClearStack=true", UriKind.Relative));
    }

    private async void CheckForBackupButton_Click(object sender, RoutedEventArgs e)
    {
      this.BackupLocateErrorText.Visibility = Visibility.Collapsed;
      if (this.accountSettingsPane != null && !OneDriveUtils.TryWP10AccountInterface((Action) (() => this.accountSettingsPane.Show()), "accountSettingsPane.Show() in CloudBackupCheckPage"))
      {
        ((IDisposable) this.accountSettingsPane).SafeDispose();
        this.accountSettingsPane = (WAAccountSettingsPane) null;
      }
      if (this.accountSettingsPane != null)
        return;
      await this.AuthenticateAndCheckForCloudBackup();
    }

    private void SkipButton_Click(object sender, RoutedEventArgs e)
    {
      Log.l("cloudcheck", "Skipping");
      this.cancellationSource.Cancel();
      this.GoToNextPage();
    }

    private void ContinueButton_Click(object sender, EventArgs e)
    {
      Log.l("cloudcheck", "Continuing");
      this.cancellationSource.Cancel();
      this.GoToNextPage();
    }

    private async Task AuthenticateAndCheckForCloudBackup(WAWebAccountProvider selectedProvider = null)
    {
      this.CheckForBackupButton.IsEnabled = false;
      this.CheckProgress.Visibility = Visibility.Visible;
      bool authenticated = false;
      bool success = false;
      bool exception = false;
      try
      {
        authenticated = await this.AuthenticateUser(selectedProvider);
        if (authenticated)
          success = await this.CheckForCloudBackup();
      }
      catch (Exception ex)
      {
        Log.l("cloudcheck", "Unable to check for cloud backup");
        Log.LogException(ex, "cloudcheck");
        exception = true;
      }
      if (authenticated & success)
      {
        this.GoToNextPage();
      }
      else
      {
        this.BackupLocateErrorText.Text = !exception ? (success ? AppResources.CloudBackupCheckUnexpectedError : AppResources.CloudBackupCheckNotFound) : AppResources.CloudBackupCheckUnexpectedError;
        this.CheckProgress.Visibility = Visibility.Collapsed;
        this.CheckForBackupButton.IsEnabled = true;
        this.CheckForBackupButton.Content = (object) AppResources.Retry;
        this.BackupLocateErrorText.Visibility = Visibility.Visible;
      }
    }

    private async void AccountSettingsPane_WebAccountProviderInvoked(
      WAAccountSettingsPane sender,
      WAWebAccountProviderInvokedEventArgs args)
    {
      await this.AuthenticateAndCheckForCloudBackup(args.Provider);
    }

    private async Task<bool> AuthenticateUser(WAWebAccountProvider selectedProvider)
    {
      bool reauthenticate = Settings.OneDriveUserReauthenticate;
      await this.processor.Reset();
      if (!await this.processor.Authenticate(new CredentialPromptType?(reauthenticate ? (CredentialPromptType) 1 : (CredentialPromptType) 0), selectedProvider))
        return false;
      bool success = await this.processor.QueryUserMetadata(this.cancellationSource.Token);
      if (success)
      {
        await this.processor.QueryDriveMetadata(this.cancellationSource.Token);
        Settings.OneDriveUserReauthenticate = false;
      }
      return success;
    }

    private async Task<bool> CheckForCloudBackup()
    {
      bool success = false;
      OneDriveManifest manifest = (OneDriveManifest) null;
      try
      {
        Microsoft.OneDrive.Sdk.Item remoteBackup = await this.processor.FindRemoteBackup(this.cancellationSource.Token);
        if (remoteBackup != null)
        {
          Log.l("cc", "Found remote backup: {0}", (object) remoteBackup.Name);
          manifest = await this.processor.GetRemoteBackupManifest(remoteBackup, this.cancellationSource.Token);
          manifest.Open();
          BackupProperties backupProperties = manifest.CurrentOneDriveBackupProperties();
          Log.l("cc", "BackupProperties: Id={0}, StartTime={1}, Size={2}, IncrementalSize={3}, IncompleteSize={4}", (object) backupProperties.BackupId, (object) backupProperties.StartTime.ToString(), (object) backupProperties.Size, (object) backupProperties.IncrementalSize, (object) backupProperties.IncompleteSize);
          success = true;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, nameof (CheckForCloudBackup));
        this.processor.ClearTemporaryData();
        success = false;
      }
      finally
      {
        manifest.SafeDispose();
      }
      return success;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CloudBackupCheckPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.CheckProgress = (ProgressBar) this.FindName("CheckProgress");
      this.BackupLocateErrorText = (TextBlock) this.FindName("BackupLocateErrorText");
      this.Decision = (Grid) this.FindName("Decision");
      this.CheckForBackupButton = (Button) this.FindName("CheckForBackupButton");
      this.SkipButton = (Button) this.FindName("SkipButton");
    }
  }
}
