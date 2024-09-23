// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAdminPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;
using Windows.Security.Authentication.OnlineId;


namespace WhatsApp
{
  public class WaAdminPage : PhoneApplicationPage
  {
    private WaAdminPageViewModel viewModel;
    private static bool doneASync;
    internal ZoomBox RootZoomBox;
    internal WhatsApp.CompatibilityShims.LongListSelector ItemsList;
    private bool _contentLoaded;

    public WaAdminPage()
    {
      this.InitializeComponent();
      this.Init();
    }

    private void Init()
    {
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.DataContext = (object) (this.viewModel = new WaAdminPageViewModel(this.Orientation));
      this.ItemsList.OverlapScrollBar = true;
      this.UpdateItems();
    }

    private void UpdateItems()
    {
      List<WaAdminPage.Item> objList1 = new List<WaAdminPage.Item>();
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "server properties",
        Subtitle = "view and refresh server props",
        OnSelected = (Action) (() => NavUtils.NavigateToPage(this.NavigationService, "ServerPropertiesDebugPage", folderName: "Test"))
      });
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "delete axolotl database",
        Subtitle = "tap to reset your encryption keys",
        OnSelected = (Action) (() =>
        {
          AppState.GetConnection().Encryption.Reset();
          int num = (int) MessageBox.Show("Encryption Database Reset");
        })
      });
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "delete internal message backup",
        Subtitle = "tap to delete backups from OEM storage",
        OnSelected = (Action) (() => UIUtils.Decision("Clear OEM storage backups?", "Yes", "No").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          Backup.DeleteAll();
        })))
      });
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "force unregister push",
        Subtitle = "re-register push notifications at next app launch",
        OnSelected = (Action) (() => PushSystem.ForegroundInstance.RequestNewUri())
      });
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "OneDrive media download",
        Subtitle = "download missing media from OneDrive",
        OnSelected = (Action) (() => UIUtils.Decision("You have a recent OneDrive backup?", "Yes", "No").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (confirmed)
          {
            this.StartOneDriveMediaRestore();
          }
          else
          {
            int num = (int) MessageBox.Show("Please make one!");
          }
        })))
      });
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "purge psa statuses",
        Subtitle = "tap to purge all psa statuses",
        OnSelected = (Action) (() =>
        {
          int n = 0;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Message[] array = ((IEnumerable<WaStatus>) db.GetStatuses("0@s.whatsapp.net", false, true, new TimeSpan?())).Select<WaStatus, Message>((Func<WaStatus, Message>) (s => db.GetMessageById(s.MessageId))).Where<Message>((Func<Message, bool>) (m => m != null)).ToArray<Message>();
            db.DeleteMessages(array);
            n = array.Length;
          }));
          Settings.LastPSAReceived = 0;
          UIUtils.ShowMessageBox("PSA statuses purged", string.Format("total deleted: {0}", (object) n)).Subscribe<Unit>();
        })
      });
      List<WaAdminPage.Item> objList2 = objList1;
      WaAdminPage.Item obj1 = new WaAdminPage.Item();
      obj1.Title = "reg sync";
      DateTime? nextFullSyncUtc = Settings.NextFullSyncUtc;
      string str;
      if (!nextFullSyncUtc.HasValue)
      {
        str = "n/a";
      }
      else
      {
        nextFullSyncUtc = Settings.NextFullSyncUtc;
        str = (nextFullSyncUtc.Value - FunRunner.CurrentServerTimeUtc).ToString("d\\.hh\\:mm");
      }
      obj1.Subtitle = string.Format("initiate reg sync - next full in: {0}", (object) str);
      obj1.OnSelected = (Action) (() =>
      {
        if (WaAdminPage.doneASync)
        {
          UIUtils.ShowMessageBox("Reg Sync", "You have done a reg sync recently").Subscribe<Unit>();
        }
        else
        {
          WaAdminPage.doneASync = true;
          ContactStore.SyncReg();
          UIUtils.ShowMessageBox("Reg Sync", "Sync started").Subscribe<Unit>();
        }
      });
      objList2.Add(obj1);
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "gdpr report: reset state",
        Subtitle = "tap to reset",
        OnSelected = (Action) (() =>
        {
          GdprReport.DeleteReport(false, "wadmin - reset");
          UIUtils.ShowMessageBox((string) null, "gdpr report state has been reset").Subscribe<Unit>();
        })
      });
      WaAdminPage.Item gifProviderItem = (WaAdminPage.Item) null;
      List<WaAdminPage.Item> objList3 = objList1;
      WaAdminPage.Item obj2 = new WaAdminPage.Item();
      obj2.Title = "set gif provider";
      obj2.Subtitle = string.Format("current: {0}", (object) this.GetGifProviderState());
      obj2.OnSelected = (Action) (() => ListPickerPage.Start(this.GetGifProviderOptions(), title: "choose gif provider").ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (sel =>
      {
        switch (sel)
        {
          case 0:
          case 1:
          case 2:
            Settings.WaAdminForceGifProvider = sel;
            break;
        }
        gifProviderItem.Subtitle = string.Format("current: {0}", (object) this.GetGifProviderState());
      })));
      WaAdminPage.Item obj3 = obj2;
      gifProviderItem = obj2;
      WaAdminPage.Item obj4 = obj3;
      objList3.Add(obj4);
      objList1.Add(new WaAdminPage.Item()
      {
        Title = "biz 2 tier upgrade",
        Subtitle = "Caution: may have side effects",
        OnSelected = (Action) (() =>
        {
          string messageBoxText;
          try
          {
            using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            {
              using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile("version", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
              {
                byte[] bytes = Encoding.UTF8.GetBytes("3.99.99");
                storageFileStream.SetLength((long) bytes.Length);
                storageFileStream.Write(bytes, 0, bytes.Length);
              }
            }
            Settings.BizChat2TierOneTimeSysMsgAdded = new DateTime?();
            messageBoxText = "Upgrade forced - please force close and restart";
          }
          catch (Exception ex)
          {
            messageBoxText = "Exception: " + ex.GetFriendlyMessage();
          }
          int num = (int) MessageBox.Show(messageBoxText);
        })
      });
      this.ItemsList.ItemsSource = (IList) objList1;
    }

    private string[] GetGifProviderOptions()
    {
      return new string[3]
      {
        string.Format("per server prop ({0})", (object) GifProviders.Instance.GetCurrentProvider(true).GetName()),
        "force giphy",
        "force tenor"
      };
    }

    private string GetGifProviderState()
    {
      IGifProvider overriddenProvider = GifProviders.Instance.GetWadminOverriddenProvider();
      return overriddenProvider != null ? string.Format("force {0}", (object) overriddenProvider.GetName()) : string.Format("per server prop ({0})", (object) GifProviders.Instance.GetCurrentProvider(true).GetName());
    }

    private async void StartOneDriveMediaRestore()
    {
      GlobalProgressIndicator progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      progressIndicator.Acquire();
      string responseString = (string) null;
      try
      {
        OneDriveRestoreProcessor processor = new OneDriveRestoreProcessor();
        if (!processor.IsAuthenticated)
        {
          if (!await processor.Authenticate(new CredentialPromptType?((CredentialPromptType) 0)))
            responseString = "Auth failed";
        }
        if (responseString == null)
        {
          CancellationTokenSource cancellationSource = new CancellationTokenSource();
          OneDriveManifest manifest = (OneDriveManifest) null;
          try
          {
            Microsoft.OneDrive.Sdk.Item remoteBackup = await processor.FindRemoteBackup(cancellationSource.Token);
            if (remoteBackup != null)
            {
              Log.p("wadmin", "Found backup!");
              manifest = await processor.GetRemoteBackupManifest(remoteBackup, cancellationSource.Token);
              BackupProperties backupProperties = manifest.CurrentOneDriveBackupProperties();
              Log.p("wadmin", "BackupProperties: Id={0}, StartTime={1}, Size={2}, IncrementalSize={3}, IncompleteSize={4}", (object) backupProperties.BackupId, (object) backupProperties.StartTime.ToString(), (object) backupProperties.Size, (object) backupProperties.IncrementalSize, (object) backupProperties.IncompleteSize);
              string str = string.Format("Backup Found:\nId={0}\nStartTime={1}\nSize={2}", (object) backupProperties.BackupId, (object) backupProperties.StartTime, (object) backupProperties.Size);
              OneDriveRestoreManager.Instance.RefreshBackupProperties();
              UIUtils.Decision(str + "\nUse this for media restore?", "Yes", "No").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
              {
                string messageBoxText;
                if (confirmed)
                {
                  try
                  {
                    OneDriveRestoreManager.Instance.Start(OneDriveBkupRestTrigger.UserInteraction);
                    messageBoxText = "Restore started";
                  }
                  catch (Exception ex)
                  {
                    messageBoxText = "Exception starting restore: " + ex.ToString();
                  }
                }
                else
                  messageBoxText = "Restore skipped";
                int num = (int) MessageBox.Show(messageBoxText);
              }));
            }
          }
          catch (Exception ex)
          {
            int num = (int) MessageBox.Show(ex.Message);
          }
          finally
          {
            manifest.SafeDispose();
          }
          cancellationSource = (CancellationTokenSource) null;
          manifest = (OneDriveManifest) null;
        }
        processor = (OneDriveRestoreProcessor) null;
      }
      catch (Exception ex)
      {
        responseString = ex.ToString();
      }
      progressIndicator.Release();
      if (responseString == null)
        return;
      int num1;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => num1 = (int) MessageBox.Show(responseString)));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (Settings.IsWaAdmin)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      WaAdminPage.Item selectedItem = this.ItemsList.SelectedItem as WaAdminPage.Item;
      this.ItemsList.SelectedItem = (object) null;
      if (selectedItem == null || selectedItem.OnSelected == null)
        return;
      selectedItem.OnSelected();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WaAdminPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.ItemsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ItemsList");
    }

    public class Item : WaViewModelBase
    {
      private string subtitle;

      public string Title { get; set; }

      public string Subtitle
      {
        get => this.subtitle;
        set
        {
          this.subtitle = value;
          this.NotifyPropertyChanged(nameof (Subtitle));
        }
      }

      public Action OnSelected { get; set; }
    }
  }
}
