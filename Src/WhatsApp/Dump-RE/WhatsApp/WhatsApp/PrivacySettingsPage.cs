// Decompiled with JetBrains decompiler
// Type: WhatsApp.PrivacySettingsPage
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

#nullable disable
namespace WhatsApp
{
  public class PrivacySettingsPage : PhoneApplicationPage
  {
    private GlobalProgressIndicator progressIndicator;
    private bool ignoreUpdate;
    private StatusPrivacySettingPickerWrapper statusPickerWrapper;
    private IDisposable timerSub;
    private bool shouldFetchRemote = true;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Grid ErrorMessagePanel;
    internal TextBlock ErrorMessageBlock;
    internal WhatsApp.CompatibilityShims.LongListSelector SettingsList;
    internal StackPanel StatusSection;
    internal TextBlock StatusHeader;
    internal ListPicker StatusPicker;
    internal TextBlock StatusTooltip;
    internal StackPanel LiveLocationSection;
    internal TextBlock LiveLocationHeader;
    internal TextBlock LiveLocationSharingStatus;
    internal TextBlock LiveLocationTooltip;
    internal ToggleSwitch EnableReadReceiptToggle;
    private bool _contentLoaded;

    public PrivacySettingsPage()
    {
      this.InitializeComponent();
      this.TitlePanel.SmallTitle = AppResources.AccountSettingsTitle;
      this.TitlePanel.LargeTitle = AppResources.PrivacySettingsTitle;
      this.SettingsList.OverlapScrollBar = true;
      this.SettingsList.ItemsSource = (IList) this.GetPrivacySettingItems();
      this.EnableReadReceiptToggle.IsChecked = new bool?(Settings.EnableReadReceipts);
      this.StatusSection.Visibility = Visibility.Visible;
      this.StatusHeader.Text = AppResources.StatusV3Title;
      this.StatusTooltip.Text = AppResources.StatusPrivacyTooltip;
      this.statusPickerWrapper = new StatusPrivacySettingPickerWrapper(this.StatusPicker);
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      if (Settings.LiveLocationEnabled)
      {
        this.LiveLocationSection.Visibility = Visibility.Visible;
        this.LiveLocationHeader.Text = AppResources.LiveLocation;
        this.LiveLocationTooltip.Text = AppResources.LiveLocationPrivacyTooltip;
      }
      else
        this.LiveLocationSection.Visibility = Visibility.Collapsed;
    }

    private void EnablePage(bool enable)
    {
      this.SettingsList.IsEnabled = enable;
      this.IsEnabled = enable;
    }

    private List<PrivacySettingItem> GetPrivacySettingItems()
    {
      return new List<PrivacySettingItem>()
      {
        new PrivacySettingItem("profile", Settings.ProfilePhotoVisibility, (Action<string, PrivacyVisibility>) ((item, newVisibility) => this.Dispatcher.BeginInvoke((Action) (() => this.UpdatePrivacySettingRemote(item, newVisibility, (Action) null))))),
        new PrivacySettingItem("last", Settings.LastSeenVisibility, (Action<string, PrivacyVisibility>) ((item, newVisibility) => this.Dispatcher.BeginInvoke((Action) (() => this.UpdatePrivacySettingRemote(item, newVisibility, (Action) (() => App.CurrentApp.Connection.EventHandler.ClearLastSeenCache()))))))
        {
          NoticeStr = AppResources.LastSeenReciprocityNotice
        },
        new PrivacySettingItem("status", Settings.StatusVisibility, (Action<string, PrivacyVisibility>) ((item, newVisibility) => this.Dispatcher.BeginInvoke((Action) (() => this.UpdatePrivacySettingRemote(item, newVisibility, (Action) null)))))
      };
    }

    private void ReloadPrivacySettings()
    {
      this.ignoreUpdate = true;
      this.SettingsList.ItemsSource = (IList) this.GetPrivacySettingItems();
      this.EnableReadReceiptToggle.IsChecked = new bool?(Settings.EnableReadReceipts);
      this.ignoreUpdate = false;
    }

    private void UpdateLiveLocationStatus()
    {
      int jidsSendingToCount = LiveLocationManager.Instance.GetJidsSendingToCount();
      if (jidsSendingToCount > 0)
      {
        this.LiveLocationSharingStatus.Text = Plurals.Instance.GetString(AppResources.LiveLocationCurrentlySharingPlural, jidsSendingToCount);
      }
      else
      {
        this.LiveLocationSharingStatus.Foreground = UIUtils.SubtleBrush;
        this.LiveLocationSharingStatus.Text = AppResources.LiveLocationNotSharing;
      }
    }

    private void UpdatePrivacySettingRemote(
      string privacyItem,
      PrivacyVisibility newVal,
      Action onComplete)
    {
      if (this.ignoreUpdate)
        return;
      Log.l("privacy page", "updating {0} to {1}", (object) privacyItem, (object) newVal);
      if (onComplete == null)
        onComplete = (Action) (() => { });
      this.EnablePage(false);
      this.StartWait((Action) null);
      if (App.CurrentApp.Connection.IsConnected)
      {
        if (privacyItem == "last")
          Settings.LastSeenVisibilityInDoubt = true;
        else if (privacyItem == "status")
          Settings.StatusVisibilityInDoubt = true;
        else if (privacyItem == "profile")
          Settings.ProfilePhotoVisibilityInDoubt = true;
        else if (privacyItem == "readreceipts")
          Settings.EnableReadReceiptInDoubt = true;
        FunXMPP.Connection connection = App.CurrentApp.Connection;
        Dictionary<string, PrivacyVisibility> dict = new Dictionary<string, PrivacyVisibility>();
        dict.Add(privacyItem, newVal);
        Action onComplete1 = (Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.EndWait(true);
          onComplete();
          Log.l("privacy page", "{0} updated to {1}", (object) privacyItem, (object) newVal);
        })));
        Action<int> onError = (Action<int>) (err => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.EndWait(false);
          this.ReloadPrivacySettings();
          this.ShowErrorMessage(AppResources.ChangesSaveError, true);
          Log.l("privacy page", "{0} updated to {1} failed", (object) privacyItem, (object) newVal);
        })));
        connection.SendSetPrivacySettings(dict, onComplete1, onError);
      }
      else
      {
        this.EndWait(false);
        this.ShowErrorMessage(AppResources.CannotUpdateSettingsWhenOffline, false);
        Log.l("privacy page", "update failed while offline");
      }
    }

    private void StartWait(Action onWaitEnd)
    {
      this.progressIndicator.Acquire();
      this.timerSub.SafeDispose();
      this.timerSub = Observable.Timer(TimeSpan.FromSeconds(30.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        this.progressIndicator.ReleaseAll();
        this.timerSub.SafeDispose();
        this.timerSub = (IDisposable) null;
        if (onWaitEnd == null)
          return;
        onWaitEnd();
      }));
    }

    private void EndWait(bool enablePage)
    {
      this.progressIndicator.ReleaseAll();
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.EnablePage(enablePage);
    }

    private void ShowErrorMessage(string errMsg, bool enablePage)
    {
      this.ErrorMessagePanel.Visibility = Visibility.Visible;
      this.ErrorMessageBlock.Text = errMsg;
      this.EnablePage(enablePage);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (e.Uri.OriginalString.StartsWith("app://external/", StringComparison.OrdinalIgnoreCase))
        this.shouldFetchRemote = true;
      this.EndWait(false);
      bool flag = ((int) this.EnableReadReceiptToggle.IsChecked ?? 1) != 0;
      if (flag != Settings.EnableReadReceipts)
      {
        if (flag)
        {
          DateTime? prevEnabledAt = Settings.LastEnableReadReceiptsTimeUtc;
          DateTime funTimeNow = FunRunner.CurrentServerTimeUtc;
          PersistentAction pa = (PersistentAction) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            pa = PersistentAction.SendPostponedReceipts(funTimeNow, prevEnabledAt);
            db.StorePersistentAction(pa);
          }));
          Settings.EnableReadReceipts = true;
          this.UpdatePrivacySettingRemote("readreceipts", PrivacyVisibility.Everyone, (Action) null);
          Settings.LastEnableReadReceiptsTimeUtc = new DateTime?(funTimeNow);
          AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
        }
        else
        {
          Settings.EnableReadReceipts = false;
          this.UpdatePrivacySettingRemote("readreceipts", PrivacyVisibility.None, (Action) null);
        }
      }
      base.OnNavigatedFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      this.ErrorMessagePanel.Visibility = Visibility.Collapsed;
      this.UpdateLiveLocationStatus();
      if (this.shouldFetchRemote)
      {
        this.EnablePage(false);
        this.StartWait((Action) (() =>
        {
          this.EnablePage(false);
          this.ShowErrorMessage(AppResources.CannotUpdateSettingsWhenOffline, false);
        }));
        App.CurrentApp.Connection.InvokeWhenConnected((Action) (() => App.CurrentApp.Connection.SendGetPrivacySettings((Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.EndWait(true);
          this.ReloadPrivacySettings();
          this.shouldFetchRemote = false;
        }))), (Action<int>) (err => this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.EndWait(false);
          this.ShowErrorMessage(AppResources.SettingsRetrieveError, false);
          Log.l("privacy page", "retrieve privacy settings failed | error: {0}", (object) err);
        }))))));
      }
      else
        this.EnablePage(true);
    }

    private void LiveLocationSharingStatus_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "LiveLocationSettingsPage", folderName: "Pages/Settings");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/PrivacySettingsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ErrorMessagePanel = (Grid) this.FindName("ErrorMessagePanel");
      this.ErrorMessageBlock = (TextBlock) this.FindName("ErrorMessageBlock");
      this.SettingsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("SettingsList");
      this.StatusSection = (StackPanel) this.FindName("StatusSection");
      this.StatusHeader = (TextBlock) this.FindName("StatusHeader");
      this.StatusPicker = (ListPicker) this.FindName("StatusPicker");
      this.StatusTooltip = (TextBlock) this.FindName("StatusTooltip");
      this.LiveLocationSection = (StackPanel) this.FindName("LiveLocationSection");
      this.LiveLocationHeader = (TextBlock) this.FindName("LiveLocationHeader");
      this.LiveLocationSharingStatus = (TextBlock) this.FindName("LiveLocationSharingStatus");
      this.LiveLocationTooltip = (TextBlock) this.FindName("LiveLocationTooltip");
      this.EnableReadReceiptToggle = (ToggleSwitch) this.FindName("EnableReadReceiptToggle");
    }
  }
}
