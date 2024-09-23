// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatSettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class ChatSettingsPage : PhoneApplicationPage
  {
    private static readonly string logHdr = nameof (ChatSettingsPage);
    private ObservableCollection<SettingsPage.SettingItem> settingItems;
    private SettingsPage.SettingItem backupItem;
    private SettingsPage.SettingItem archiveItem;
    private SettingsPage.SettingItem clearItem;
    private SettingsPage.SettingItem deleteItem;
    private bool showingArchiveAll;
    private IDisposable backupSub;
    private GlobalProgressIndicator globalProgress;
    private bool enableEaseOfAccessLink = true;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal WhatsApp.CompatibilityShims.LongListSelector ItemsList;
    internal TextBlock ChatSectionTitleBlock;
    internal ToggleSwitch EnterKeyIsSendToggle;
    internal ToggleSwitch SaveIncomingMediaToggle;
    internal TextBlock SaveIncomingMediaTooltipBlock;
    internal StackPanel CallSectionPanel;
    internal TextBlock CallSectionTitleBlock;
    internal ToggleSwitch LowVoipDataUsageToggle;
    internal TextBlock LowVoipDataUsageTooltipBlock;
    private bool _contentLoaded;

    public ChatSettingsPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.Settings.ToUpper();
      this.PageTitle.LargeTitle = AppResources.ChatsAndCallsSettingsTitle;
      this.ChatSectionTitleBlock.Text = AppResources.ChatSettingsTitle;
      this.CallSectionTitleBlock.Text = AppResources.CallSettingsTitle;
      this.LowVoipDataUsageToggle.Header = (object) AppResources.LowVoipDataUsageToggleLabel;
      this.LowVoipDataUsageToggle.IsChecked = new bool?(Settings.LowBandwidthVoip);
      this.LowVoipDataUsageTooltipBlock.Text = AppResources.LowVoipDataUsageToggleTooltip;
      this.CallSectionPanel.Visibility = Visibility.Visible;
      this.EnterKeyIsSendToggle.Header = (object) AppResources.EnterKeySetting;
      this.EnterKeyIsSendToggle.IsChecked = new bool?(Settings.EnterKeyIsSend);
      this.SaveIncomingMediaToggle.Header = (object) AppResources.SaveIncomingMediaSetting;
      this.SaveIncomingMediaToggle.IsChecked = new bool?(Settings.SaveIncomingMedia);
      this.SaveIncomingMediaTooltipBlock.Text = AppResources.SaveIncomingMediaSettingTooltip;
      this.InitSettingItems();
      this.UpdateBackupItem((Func<DateTime?>) (() =>
      {
        DateTime? nullable = new DateTime?();
        try
        {
          if (Settings.CorruptDb || !Backup.CanBackup())
            this.settingItems.Remove(this.backupItem);
          else
            nullable = Backup.GetLastBackupTime();
        }
        catch (Exception ex)
        {
        }
        return nullable;
      }));
      this.UpdateClearItem();
      this.UpdateDeleteItem();
      this.globalProgress = new GlobalProgressIndicator((DependencyObject) this);
    }

    private void InitSettingItems()
    {
      this.backupItem = new SettingsPage.SettingItem(AppResources.BackupTitle, (string) null)
      {
        OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ChatBackupPage", folderName: "Pages/Settings")),
        AutomationId = "AIdBackup"
      };
      this.archiveItem = new SettingsPage.SettingItem()
      {
        OnTap = new Action(this.OnArchiveTap),
        AutomationId = "AIdArchive"
      };
      this.clearItem = new SettingsPage.SettingItem()
      {
        OnTap = new Action(this.OnClearTap),
        AutomationId = "AIdClear"
      };
      this.deleteItem = new SettingsPage.SettingItem()
      {
        OnTap = new Action(this.OnDeleteTap),
        AutomationId = "AIdDelete"
      };
      ObservableCollection<SettingsPage.SettingItem> observableCollection = new ObservableCollection<SettingsPage.SettingItem>();
      observableCollection.Add(new SettingsPage.SettingItem(AppResources.DefaultWallpaper, AppResources.WallpaperSettingsExplanation)
      {
        OnTap = (Action) (() => SetWallpaperPage.Start()),
        AutomationId = "AIdWallpaper"
      });
      observableCollection.Add(new SettingsPage.SettingItem(AppResources.FontSizeSetting, AppResources.TextSizeExplanation1)
      {
        OnTap = (Action) (() =>
        {
          if (!this.enableEaseOfAccessLink)
            return;
          this.enableEaseOfAccessLink = false;
          Action release = (Action) (() => this.enableEaseOfAccessLink = true);
          NavUtils.NavigateExternal(DeepLinks.EaseOfAccessUrl, release, (Action<Exception>) (ex =>
          {
            Log.SendCrashLog(ex, "failed to open ease of access");
            release();
          }));
        }),
        AutomationId = "AIdTextSize"
      });
      observableCollection.Add(new SettingsPage.SettingItem(AppResources.AutodownloadSettings, AppResources.AutodownloadExplanation)
      {
        OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "AutodownloadPage", folderName: "Pages/Settings")),
        AutomationId = "AIdAutoDownload"
      });
      observableCollection.Add(this.archiveItem);
      observableCollection.Add(this.clearItem);
      observableCollection.Add(this.deleteItem);
      observableCollection.Add(this.backupItem);
      this.settingItems = observableCollection;
      this.settingItems.Add(new SettingsPage.SettingItem(AppResources.StorageInfoLink, AppResources.StorageInfoLinkDescr)
      {
        OnTap = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ChatCountsPage", folderName: "Pages/Settings")),
        AutomationId = "AIdStorageinfo"
      });
      this.ItemsList.ItemsSource = (IList) this.settingItems;
    }

    private void UpdateArchiveItem()
    {
      int liveChats = 0;
      int allChats = 0;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        liveChats = db.GetConversationsCount(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, false);
        allChats = db.GetConversationsCount(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, true);
      }));
      if (liveChats > 0 || allChats == 0)
      {
        this.showingArchiveAll = true;
        this.archiveItem.Title = AppResources.ArchiveAll;
        this.archiveItem.Details = AppResources.ArchiveAllExplain;
        this.archiveItem.IsEnabled = allChats > 0;
      }
      else
      {
        this.showingArchiveAll = false;
        this.archiveItem.Title = AppResources.UnarchiveAll;
        this.archiveItem.Details = AppResources.UnarchiveAllExplain;
        this.archiveItem.IsEnabled = true;
      }
    }

    private void UpdateClearItem()
    {
      this.clearItem.Title = AppResources.ClearAll;
      this.clearItem.Details = AppResources.ClearAllExplain;
      this.clearItem.IsEnabled = true;
    }

    private void UpdateDeleteItem()
    {
      this.deleteItem.Title = AppResources.DeleteAll;
      this.deleteItem.Details = AppResources.DeleteAllExplain;
      this.deleteItem.IsEnabled = true;
    }

    private void UpdateBackupItem(Func<DateTime?> backupTimeSource)
    {
      Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        Log.d(ChatSettingsPage.logHdr, "querying backups");
        DateTime? nullable = backupTimeSource();
        if (nullable.HasValue)
        {
          string str = DateTimeUtils.FormatCompact(nullable.Value, DateTimeUtils.TimeDisplay.SameWeekOnly, true);
          observer.OnNext(string.Format(AppResources.LastBackup, (object) str));
        }
        else
          observer.OnNext(AppResources.BackupSummary);
        return (Action) (() => { });
      })).Take<string>(1).SubscribeOn<string>((IScheduler) AppState.Worker).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (s => this.backupItem.Details = s.ToLangFriendlyLower()));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.UpdateArchiveItem();
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);
      bool? isChecked1 = this.SaveIncomingMediaToggle.IsChecked;
      if (isChecked1.HasValue && Settings.SaveIncomingMedia != isChecked1.Value)
        Settings.SaveIncomingMedia = isChecked1.Value;
      bool? isChecked2 = this.EnterKeyIsSendToggle.IsChecked;
      if (isChecked2.HasValue && Settings.EnterKeyIsSend != isChecked2.Value)
        Settings.EnterKeyIsSend = isChecked2.Value;
      bool? isChecked3 = this.LowVoipDataUsageToggle.IsChecked;
      if (!isChecked3.HasValue || Settings.LowBandwidthVoip == isChecked3.Value)
        return;
      Settings.LowBandwidthVoip = isChecked3.Value;
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.backupSub.SafeDispose();
      this.backupSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    private void OnArchiveTap()
    {
      Observable.Return<bool>(true).Decision(this.showingArchiveAll ? AppResources.ArchiveAllConfirm : AppResources.UnarchiveAllConfirm).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        this.globalProgress.Acquire();
        this.IsEnabled = false;
        if (this.showingArchiveAll)
        {
          ArchiveChat.ArchiveAll();
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_ARCHIVE);
        }
        else
          ArchiveChat.UnarchiveAll();
        this.UpdateArchiveItem();
        this.IsEnabled = true;
        this.globalProgress.Release();
      }));
    }

    private void OnClearTap()
    {
      this.globalProgress.Acquire();
      this.IsEnabled = false;
      ClearAllChatPicker.Launch(false).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
      this.IsEnabled = true;
      this.globalProgress.Release();
    }

    private void OnDeleteTap()
    {
      this.globalProgress.Acquire();
      this.IsEnabled = false;
      ClearAllChatPicker.Launch(true).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
      this.IsEnabled = true;
      this.globalProgress.Release();
    }

    private void OnBackupTap()
    {
      if (this.backupSub != null)
        return;
      DateTime start = DateTime.Now;
      List<IDisposable> disps = new List<IDisposable>();
      this.backupSub = (IDisposable) new DisposableAction((Action) (() => disps.ForEach((Action<IDisposable>) (d => d.Dispose()))));
      IDisposable disposable = Observable.Return<Unit>(new Unit()).Do<Unit>((Action<Unit>) (_ =>
      {
        this.IsEnabled = false;
        disps.Add((IDisposable) new DisposableAction((Action) (() => this.IsEnabled = true)));
        this.globalProgress.Acquire();
        this.backupItem.IsEnabled = false;
        disps.Add((IDisposable) new DisposableAction((Action) (() =>
        {
          this.globalProgress.Release();
          this.backupItem.IsEnabled = true;
        })));
        disps.Add(BackKeyBroker.Get((PhoneApplicationPage) this, 0).Subscribe<CancelEventArgs>((Action<CancelEventArgs>) (ev => ev.Cancel = true)));
      })).ObserveOn<Unit>((IScheduler) Scheduler.ThreadPool).Do<Unit>((Action<Unit>) (_ =>
      {
        using (ManualResetEvent ev = new ManualResetEvent(false))
        {
          WAThreadPool.QueueUserWorkItem((Action) (() =>
          {
            Action onComplete = Utils.IgnoreMultipleInvokes((Action) (() => ev.Set()));
            try
            {
              Backup.Save(onComplete);
              this.UpdateBackupItem((Func<DateTime?>) (() => Backup.GetLastBackupTime()));
            }
            catch (Exception ex)
            {
              Log.SendCrashLog(ex, "create backup");
              onComplete();
            }
          }));
          TimeSpan timeSpan1 = DateTime.Now - start;
          TimeSpan timeSpan2 = TimeSpan.FromSeconds(3.0);
          if (timeSpan1 < timeSpan2)
            Thread.Sleep(timeSpan2 - timeSpan1);
          ev.WaitOne();
        }
      })).ObserveOnDispatcher<Unit>().Finally<Unit>((Action) (() =>
      {
        this.backupSub.SafeDispose();
        this.backupSub = (IDisposable) null;
      })).Subscribe<Unit>();
      disps.Add(disposable);
      FieldStats.ReportBackupConvo();
    }

    private void OnSettingItemTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is SettingsPage.SettingItem tag))
        return;
      tag.OnTap();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/ChatSettingsPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ItemsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ItemsList");
      this.ChatSectionTitleBlock = (TextBlock) this.FindName("ChatSectionTitleBlock");
      this.EnterKeyIsSendToggle = (ToggleSwitch) this.FindName("EnterKeyIsSendToggle");
      this.SaveIncomingMediaToggle = (ToggleSwitch) this.FindName("SaveIncomingMediaToggle");
      this.SaveIncomingMediaTooltipBlock = (TextBlock) this.FindName("SaveIncomingMediaTooltipBlock");
      this.CallSectionPanel = (StackPanel) this.FindName("CallSectionPanel");
      this.CallSectionTitleBlock = (TextBlock) this.FindName("CallSectionTitleBlock");
      this.LowVoipDataUsageToggle = (ToggleSwitch) this.FindName("LowVoipDataUsageToggle");
      this.LowVoipDataUsageTooltipBlock = (TextBlock) this.FindName("LowVoipDataUsageTooltipBlock");
    }
  }
}
