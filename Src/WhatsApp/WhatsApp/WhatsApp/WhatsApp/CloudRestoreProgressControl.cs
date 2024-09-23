// Decompiled with JetBrains decompiler
// Type: WhatsApp.CloudRestoreProgressControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  public class CloudRestoreProgressControl : UserControl
  {
    private Brush progressForegroundBrush;
    private long? autoMaximum;
    private long? autoValue;
    private bool? autoIndeterminate;
    private OneDriveRestoreState? autoState;
    private OneDriveBkupRestStopReason? autoStopReason;
    private OneDriveRestoreStopError? autoStopError;
    internal Grid LayoutRoot;
    internal Grid IconPanel;
    internal Grid ContentPanel;
    internal TextBlock RestoreTitle;
    internal ProgressBar RestoreProgress;
    internal TextBlock RestoreSubtitle;
    private bool _contentLoaded;

    public CloudRestoreProgressControl()
    {
      this.InitializeComponent();
      this.progressForegroundBrush = this.RestoreProgress.Foreground;
      OneDriveRestoreManager.Instance.BackupPropertiesChanged += new EventHandler<BackupProperties>(this.OneDrive_BackupPropertiesChanged);
      OneDriveRestoreManager.Instance.ProgressMaximumChanged += new EventHandler<long>(this.OneDrive_ProgressMaximumChanged);
      OneDriveRestoreManager.Instance.ProgressValueChanged += new EventHandler<long>(this.OneDrive_ProgressValueChanged);
      OneDriveRestoreManager.Instance.IsProgressIndeterminateChanged += new EventHandler<bool>(this.OneDrive_IsProgressIndeterminateChanged);
      OneDriveRestoreManager.Instance.StateChanged += new EventHandler<OneDriveRestoreState>(this.OneDrive_RestoreStateChanged);
      OneDriveRestoreManager.Instance.RestoreStopped += new EventHandler<BkupRestStoppedEventArgs>(this.OneDrive_RestoreStopped);
      this.UpdateCloudBackupProperties();
      this.UpdateForRestoreState();
    }

    private void OneDrive_BackupPropertiesChanged(object sender, BackupProperties e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.UpdateCloudBackupProperties()));
    }

    private void OneDrive_ProgressMaximumChanged(object sender, long e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.RestoreProgress.Maximum = (double) e));
    }

    private void OneDrive_ProgressValueChanged(object sender, long e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.RestoreProgress.Value = (double) e;
        if (OneDriveRestoreManager.Instance.State != OneDriveRestoreState.RestoringMedia)
          return;
        this.RestoreSubtitle.Text = this.BuildMediaRestoreProgressMessage();
      }));
    }

    private void OneDrive_IsProgressIndeterminateChanged(object sender, bool e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.RestoreProgress.IsIndeterminate = e));
    }

    private void OneDrive_RestoreStateChanged(object sender, OneDriveRestoreState e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.UpdateForRestoreState()));
    }

    private void OneDrive_RestoreStopped(object sender, BkupRestStoppedEventArgs e)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.UpdateForRestoreState();
        this.UpdateCloudBackupProperties();
      }));
    }

    private void UpdateCloudBackupProperties()
    {
      long? nullable = this.autoValue;
      long num = nullable ?? OneDriveRestoreManager.Instance.ProgressValue;
      nullable = this.autoMaximum;
      this.RestoreProgress.Maximum = (double) (nullable ?? OneDriveRestoreManager.Instance.ProgressMaximum);
      this.RestoreProgress.Value = (double) num;
    }

    private void UpdateForRestoreState()
    {
      int num1 = (int) this.autoState ?? (int) OneDriveRestoreManager.Instance.State;
      OneDriveBkupRestStopReason? autoStopReason = this.autoStopReason;
      if (!autoStopReason.HasValue)
      {
        int stopReason = (int) OneDriveRestoreManager.Instance.StopReason;
      }
      else
      {
        int valueOrDefault1 = (int) autoStopReason.GetValueOrDefault();
      }
      OneDriveRestoreStopError? autoStopError = this.autoStopError;
      if (!autoStopError.HasValue)
      {
        int stopError = (int) OneDriveRestoreManager.Instance.StopError;
      }
      else
      {
        int valueOrDefault2 = (int) autoStopError.GetValueOrDefault();
      }
      bool flag = false;
      string str = (string) null;
      if (num1 == 0 && (OneDriveRestoreManager.IsRestoreIncomplete || this.autoState.HasValue))
      {
        flag = true;
        str = AppResources.OneDriveRestoreProgressErrorTapForMore;
      }
      if (str == null)
        str = this.BuildMediaRestoreProgressMessage();
      this.RestoreSubtitle.Text = str;
      if (flag)
      {
        this.RestoreProgress.IsIndeterminate = false;
        this.RestoreProgress.Maximum = 100.0;
        this.RestoreProgress.Value = 100.0;
        this.RestoreProgress.Foreground = (Brush) UIUtils.RedBrush;
      }
      else
      {
        this.RestoreProgress.IsIndeterminate = ((int) this.autoIndeterminate ?? (OneDriveRestoreManager.Instance.IsProgressIndeterminate ? 1 : 0)) != 0;
        ProgressBar restoreProgress1 = this.RestoreProgress;
        long? nullable = this.autoMaximum;
        double num2 = (double) (nullable ?? OneDriveRestoreManager.Instance.ProgressMaximum);
        restoreProgress1.Maximum = num2;
        ProgressBar restoreProgress2 = this.RestoreProgress;
        nullable = this.autoValue;
        double num3 = (double) (nullable ?? OneDriveRestoreManager.Instance.ProgressValue);
        restoreProgress2.Value = num3;
        this.RestoreProgress.Foreground = this.progressForegroundBrush;
      }
    }

    private string BuildMediaRestoreProgressMessage()
    {
      long? nullable = this.autoMaximum;
      long bytes1 = nullable ?? OneDriveRestoreManager.Instance.ProgressMaximum;
      nullable = this.autoValue;
      long bytes2 = nullable ?? OneDriveRestoreManager.Instance.ProgressValue;
      int num = bytes2 >= bytes1 ? 100 : (int) (100.0 * ((double) bytes2 / (double) bytes1));
      return string.Format(AppResources.OneDriveTransferProgress, (object) (Utils.FileSizeFormatter.Format(bytes2) ?? "0"), (object) (Utils.FileSizeFormatter.Format(bytes1) ?? "0"), (object) num);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/CloudRestoreProgressControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.IconPanel = (Grid) this.FindName("IconPanel");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.RestoreTitle = (TextBlock) this.FindName("RestoreTitle");
      this.RestoreProgress = (ProgressBar) this.FindName("RestoreProgress");
      this.RestoreSubtitle = (TextBlock) this.FindName("RestoreSubtitle");
    }
  }
}
