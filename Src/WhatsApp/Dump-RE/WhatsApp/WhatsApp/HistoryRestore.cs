// Decompiled with JetBrains decompiler
// Type: WhatsApp.HistoryRestore
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WhatsApp.WaCollections;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace WhatsApp
{
  public class HistoryRestore : PhoneApplicationPage
  {
    private OneDriveRestoreProcessor processor;
    private bool restoreCloudBackup;
    private CancellationTokenSource cloudDetailsCancellationSource;
    private long cloudDbSize = -1;
    private long cloudMediaSize = -1;
    internal Storyboard StartingAnimation;
    internal DoubleAnimationUsingKeyFrames SDCardAnimationStart;
    internal DoubleAnimationUsingKeyFrames PhoneAnimationStart;
    internal Storyboard EndingAnimation;
    internal DoubleAnimationUsingKeyFrames SDCardAnimationEnd;
    internal DoubleAnimationUsingKeyFrames PhoneAnimationEnd;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal Grid RestoreVisual;
    internal WhiteBlackImage SDCard;
    internal WhiteBlackImage PhonePic;
    internal ProgressBar Progressbar;
    internal WhiteBlackImage SmilingPhone;
    internal PageTitlePanel PageTitle;
    internal Grid Decision;
    internal TextBlock LastBackup;
    internal ProgressBar CloudDetailsProgress;
    internal TextBlock CloudDetailsText;
    internal Grid RestoreCloudDetails;
    internal TextBlock RestoreCloudDetailsText;
    private bool _contentLoaded;

    public HistoryRestore()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.SDCard.Image = ImageStore.SDCardIcon;
      this.PhonePic.Image = ImageStore.PhoneIcon;
      this.SmilingPhone.Image = ImageStore.SmilingPhoneIcon;
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.PageTitle.SmallTitle = AppResources.RestoreMessageHistory;
      this.PageTitle.LargeTitle = AppResources.MessageBackupFound;
      this.processor = new OneDriveRestoreProcessor();
      this.processor.CredentialPrompt = (CredentialPromptType) 0;
      this.InitializeRestoreInfo();
    }

    private void InitializeRestoreInfo()
    {
      if (OneDriveBackupFiles.GetLastBackupKind() == LastBackupKind.Remote)
      {
        using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
        {
          BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
          this.SDCard.Image = ImageStore.OneDriveIcon;
          this.LastBackup.Text = string.Format(AppResources.LastCloudBackup, (object) DateTimeUtils.FormatLastSeen(backupProperties.StartTime, out bool _)).ToLangFriendlyLower();
          this.restoreCloudBackup = true;
        }
      }
      if (this.restoreCloudBackup)
      {
        this.CloudDetailsText.Visibility = Visibility.Collapsed;
        this.CloudDetailsProgress.Visibility = Visibility.Visible;
        this.cloudDetailsCancellationSource = new CancellationTokenSource();
        CancellationToken cancellationToken = this.cloudDetailsCancellationSource.Token;
        Task.Run((Func<Task>) (async () =>
        {
          using (OneDriveManifest manifest = OneDriveRestoreProcessor.RemoteBackupManifest())
          {
            long dbSize = this.processor.DatabaseRestoreSize(manifest);
            MediaRestoreEstimate mediaSize;
            MediaRestoreEstimate mediaRestoreEstimate = mediaSize;
            mediaSize = await this.processor.EstimateMediaRestoreSize(manifest, cancellationToken);
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              if (cancellationToken.IsCancellationRequested)
                return;
              this.cloudDbSize = dbSize;
              this.cloudMediaSize = mediaSize.Total();
              this.UpdateCloudRestoreDetails();
            }));
          }
        }), cancellationToken);
      }
      else
      {
        this.SDCard.Image = ImageStore.SDCardIcon;
        DateTime? lastBackupTime = Backup.GetLastBackupTime();
        if (!lastBackupTime.HasValue)
          return;
        this.LastBackup.Text = string.Format(AppResources.LastBackup, (object) DateTimeUtils.FormatLastSeen(lastBackupTime.Value, out bool _)).ToLangFriendlyLower();
      }
    }

    private void UpdateCloudRestoreDetails()
    {
      if (this.Decision.Visibility == Visibility.Visible)
      {
        if (this.cloudDbSize >= 0L && this.cloudMediaSize >= 0L)
        {
          this.CloudDetailsText.Text = string.Format(AppResources.RestoreCloudDownloadSize, (object) Utils.FileSizeFormatter.Format(this.cloudDbSize + this.cloudMediaSize));
          this.CloudDetailsText.Visibility = Visibility.Visible;
          this.CloudDetailsProgress.Visibility = Visibility.Collapsed;
        }
        else if (this.cloudDetailsCancellationSource == null || this.cloudDetailsCancellationSource.IsCancellationRequested)
        {
          this.CloudDetailsText.Visibility = Visibility.Collapsed;
          this.CloudDetailsProgress.Visibility = Visibility.Collapsed;
        }
        else
        {
          this.CloudDetailsText.Visibility = Visibility.Collapsed;
          this.CloudDetailsProgress.Visibility = Visibility.Visible;
        }
      }
      else if (this.cloudDbSize >= 0L && this.cloudMediaSize >= 0L)
      {
        string str = string.Format(AppResources.RestoreCloudMediaSize, (object) Utils.FileSizeFormatter.Format(Math.Max(this.cloudMediaSize, 1024L)));
        if (Settings.OneDriveRestoreNetwork == AutoDownloadSetting.EnabledOnWifi)
          str = str + " " + AppResources.OneDriveRestoreMediaOverWifi;
        this.RestoreCloudDetailsText.Text = str;
        this.RestoreCloudDetails.Visibility = Visibility.Visible;
      }
      else
        this.RestoreCloudDetails.Visibility = Visibility.Collapsed;
    }

    private void Restore()
    {
      this.Decision.Visibility = Visibility.Collapsed;
      this.PageTitle.LargeTitle = AppResources.RestoringProgress;
      this.UpdateCloudRestoreDetails();
      this.AnimationObservable().CombineLatest<Unit, string, string>(this.RestoreFromBackupObservable(), (Func<Unit, string, string>) ((anim, status) => status)).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (status =>
      {
        this.PageTitle.LargeTitle = status;
        this.ApplicationBar.IsVisible = true;
        Storyboarder.Perform(this.EndingAnimation, false);
      }));
    }

    private IObservable<Unit> AnimationObservable()
    {
      this.SDCardAnimationStart.KeyFrames[1].Value = this.SDCardAnimationEnd.KeyFrames[0].Value = -this.ActualWidth / 4.0;
      this.PhoneAnimationStart.KeyFrames[0].Value = this.PhonePic.ActualWidth;
      this.PhoneAnimationStart.KeyFrames[1].Value = this.PhoneAnimationEnd.KeyFrames[0].Value = -this.ActualWidth / 4.0 + this.PhonePic.ActualWidth / 2.0;
      this.PhoneAnimationEnd.KeyFrames[1].Value = -this.ActualWidth / 2.0 + this.PhonePic.ActualWidth / 2.0;
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Storyboarder.Perform(this.StartingAnimation, false, (Action) (() =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    private IObservable<string> RestoreFromBackupObservable()
    {
      return Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        WAThreadPool.QueueUserWorkItem((Action) (async () =>
        {
          int num;
          if (num == 0 || this.restoreCloudBackup)
          {
            try
            {
              await this.RestoreCloudBackupDatabases();
            }
            catch (Exception ex)
            {
              Log.SendCrashLog(ex, "cloud restore");
              this.restoreCloudBackup = false;
            }
          }
          try
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              MessagesContext.Reset(true);
              if (this.TryCloudRestore())
                return;
              Backup.Restore();
            }));
            Settings.Invalidate();
            long restored = 0;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              db.BackupRestoredPostProcessing(db);
              restored = db.GetMessagesCount();
            }));
            if (Settings.CorruptDb)
              Settings.CorruptDb = false;
            Settings.LastGroupsUpdatedUtc = new DateTime?();
            Settings.LastFullSyncUtc = new DateTime?();
            observer.OnNext(Plurals.Instance.GetString(AppResources.RestoredPlural, (int) restored));
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "restore backup");
            observer.OnNext(AppResources.RestoreFailed);
          }
          this.MarkCompleted();
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    private async Task RestoreCloudBackupDatabases()
    {
      CancellationTokenSource cancellationSource = new CancellationTokenSource();
      OneDriveManifest manifest = (OneDriveManifest) null;
      try
      {
        manifest = OneDriveRestoreProcessor.RemoteBackupManifest();
        int num = await this.processor.Authenticate() ? 1 : 0;
        long progressMax = manifest.GetRemoteDatabaseSizeToRestore();
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          this.Progressbar.Value = 0.0;
          this.Progressbar.Maximum = (double) progressMax;
          this.Progressbar.IsIndeterminate = false;
        }));
        await this.processor.RestoreRemoteDatabases(manifest, cancellationSource.Token, (IProgress<long>) new Progress<long>((Action<long>) (p => Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.Progressbar.Value = p <= progressMax ? (double) p : (double) progressMax)))));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "download from cloud");
        throw;
      }
      finally
      {
        manifest.SafeDispose();
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.Progressbar.IsIndeterminate = true));
      }
    }

    public bool TryCloudRestore()
    {
      if (!this.restoreCloudBackup)
        return false;
      string restoreTmpPath = OneDriveRestoreProcessor.RestoreTmpPath;
      string str = "restoreTmp";
      Set<string> filesToIgnore = new Set<string>();
      filesToIgnore.Add("onedrive_manifest.db");
      NativeInterfaces.Misc.RemoveDirectoryRecursive(Constants.IsoStorePath + "\\" + str, true);
      using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
        storeMediaStorage.CreateDirectory(str);
      Log.l("backup", "trying restore cloud backup from: {0}", (object) restoreTmpPath);
      try
      {
        Backup.RestoreInto(restoreTmpPath, str, filesToIgnore);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "restore cloud");
        this.processor.ClearTemporaryData();
        return false;
      }
      try
      {
        using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
        {
          BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
          if (backupProperties != null)
          {
            if (!string.IsNullOrEmpty(backupProperties.BackupId))
            {
              Settings.OneDriveBackupFrequency = backupProperties.Settings.BackupFrequency;
              Settings.OneDriveBackupNetwork = backupProperties.Settings.BackupNetwork;
              Settings.OneDriveIncludeVideos = backupProperties.Settings.IncludeVideos;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "manifest props");
      }
      Backup.CommitRestore(str);
      return true;
    }

    private void MarkCompleted()
    {
      this.cloudDetailsCancellationSource?.Cancel();
      if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.VerifiedPendingHistoryRestore)
        return;
      Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.Verified;
    }

    private void GoToNextPage()
    {
      this.cloudDetailsCancellationSource?.Cancel();
      NavUtils.NavigateHome(this.NavigationService);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      UIUtils.EnableWakeLock(true);
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      UIUtils.EnableWakeLock(false);
      base.OnNavigatedFrom(e);
    }

    private void OnSkip(object sender, RoutedEventArgs e)
    {
      UIUtils.MessageBox((string) null, AppResources.ConfirmNotRestoring, (IEnumerable<string>) new string[2]
      {
        AppResources.RestoreButton,
        AppResources.SkipButton
      }, (Action<int>) (buttonIdx =>
      {
        if (buttonIdx != 0)
        {
          if (buttonIdx != 1)
            return;
          this.MarkCompleted();
          this.GoToNextPage();
        }
        else
          this.Restore();
      }));
    }

    private void OnRestore(object sender, RoutedEventArgs e) => this.Restore();

    private void OnContinueClick(object sender, EventArgs e) => this.GoToNextPage();

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/HistoryRestore.xaml", UriKind.Relative));
      this.StartingAnimation = (Storyboard) this.FindName("StartingAnimation");
      this.SDCardAnimationStart = (DoubleAnimationUsingKeyFrames) this.FindName("SDCardAnimationStart");
      this.PhoneAnimationStart = (DoubleAnimationUsingKeyFrames) this.FindName("PhoneAnimationStart");
      this.EndingAnimation = (Storyboard) this.FindName("EndingAnimation");
      this.SDCardAnimationEnd = (DoubleAnimationUsingKeyFrames) this.FindName("SDCardAnimationEnd");
      this.PhoneAnimationEnd = (DoubleAnimationUsingKeyFrames) this.FindName("PhoneAnimationEnd");
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.RestoreVisual = (Grid) this.FindName("RestoreVisual");
      this.SDCard = (WhiteBlackImage) this.FindName("SDCard");
      this.PhonePic = (WhiteBlackImage) this.FindName("PhonePic");
      this.Progressbar = (ProgressBar) this.FindName("Progressbar");
      this.SmilingPhone = (WhiteBlackImage) this.FindName("SmilingPhone");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.Decision = (Grid) this.FindName("Decision");
      this.LastBackup = (TextBlock) this.FindName("LastBackup");
      this.CloudDetailsProgress = (ProgressBar) this.FindName("CloudDetailsProgress");
      this.CloudDetailsText = (TextBlock) this.FindName("CloudDetailsText");
      this.RestoreCloudDetails = (Grid) this.FindName("RestoreCloudDetails");
      this.RestoreCloudDetailsText = (TextBlock) this.FindName("RestoreCloudDetailsText");
    }
  }
}
