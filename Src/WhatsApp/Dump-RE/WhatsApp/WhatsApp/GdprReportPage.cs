// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprReportPage
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

#nullable disable
namespace WhatsApp
{
  public class GdprReportPage : PhoneApplicationPage
  {
    private const string LogHeader = "gdpr";
    private GdprReportPageViewModel viewModel;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal Image TopIcon;
    internal WhatsApp.CompatibilityShims.LongListSelector ContentList;
    internal RichTextBlock TopTooltipBlock;
    internal Button MainButton;
    internal Button SendViaWaButton;
    internal TextBlock RequestStateBlock;
    internal Button DeleteButton;
    internal TextBlock BottomTooltipBlock;
    private bool _contentLoaded;

    public GdprReportPage()
    {
      this.InitializeComponent();
      this.viewModel = new GdprReportPageViewModel(this.Orientation);
      this.viewModel.GetObservable().ObserveOnDispatcher<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>(new Action<KeyValuePair<string, object>>(this.OnViewModelNotified));
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.LargeTitle = this.viewModel.PageTitle;
      this.TopIcon.Source = (System.Windows.Media.ImageSource) AssetStore.GdprReportIcon;
      this.SendViaWaButton.Content = (object) this.viewModel.SendViaWaButtonLabel;
      this.DeleteButton.Content = (object) this.viewModel.DeleteButtonLabel;
      this.ContentList.ItemsSource = (IList) new object[0];
      this.ContentList.OverlapScrollBar = true;
      this.Loaded += (RoutedEventHandler) ((s, e) =>
      {
        if (!this.ContentList.IsScrollRequired)
          return;
        this.TopIcon.Visibility = Visibility.Collapsed;
      });
    }

    private void UpdatePage()
    {
      this.TopTooltipBlock.Text = this.viewModel.TopTooltipRichText;
      this.MainButton.Content = (object) this.viewModel.MainButtonLabel;
      this.MainButton.Opacity = this.viewModel.MainButtonOpacity;
      this.MainButton.IsEnabled = this.viewModel.ShouldEnableMainButton;
      this.RequestStateBlock.Visibility = this.viewModel.RequestStateVisibility;
      this.RequestStateBlock.Text = this.viewModel.RequestStateStr;
      this.SendViaWaButton.Visibility = this.viewModel.SendViaWaButtonVisibility;
      this.DeleteButton.Visibility = this.viewModel.DeleteButtonVisibility;
      this.BottomTooltipBlock.Text = this.viewModel.BottomTooltipStr;
    }

    private void RequestReport()
    {
      Log.l("gdpr", "request report");
      Action promptTryAgain = (Action) (() =>
      {
        Log.l("gdpr", "abort request | try again later");
        UIUtils.ShowMessageBox(" ", AppResources.GdprReportErrRequest).Subscribe<Unit>((Action<Unit>) (_ => GdprReport.SetStateInit()));
      });
      GdprReport.SetStateRequesting();
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection != null && connection.IsConnected)
        connection.SendGdprRequestReport((Action) null, (Action) (() => promptTryAgain()));
      else
        promptTryAgain();
    }

    private void DownloadReport()
    {
      DateTime? expirationTimeUtc = Settings.GdprReportExpirationTimeUtc;
      if (expirationTimeUtc.HasValue && FunRunner.CurrentServerTimeUtc > expirationTimeUtc.Value)
      {
        Log.l("gdpr", "abort download | expired | reset | expire time:{0}(utc),curr time:{1}(utc)", (object) expirationTimeUtc.Value, (object) FunRunner.CurrentServerTimeUtc);
        GdprReport.SetStateInit();
        int num = (int) MessageBox.Show(AppResources.GdprReportErrExpired);
      }
      else
      {
        Action onDownloadError = (Action) (() =>
        {
          Log.l("gdpr", "abort download | revert to ready state");
          int num = (int) MessageBox.Show(AppResources.GdprReportErrDownload);
          GdprReport.RevertStateDownloadingToReady();
        });
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection != null && !connection.IsConnected)
        {
          onDownloadError();
        }
        else
        {
          Message reportMsg = this.viewModel.ReportMsg;
          if (reportMsg == null)
          {
            Log.SendCrashLog((Exception) new ArgumentNullException("invalid gdpr report info"), "couldn't download gdpr report due to invalid report info");
            GdprReport.SetStateInit();
          }
          else
          {
            GdprReport.SetStateDownloading();
            Log.l("gdpr", "start download");
            string filepath = GdprReport.GetOutputFilepath();
            MediaDownloadManager.DownloadEncryptedFile(Settings.GdprReportCreationTimeUtc.Value.ToFileTimeUtc().ToString(), FunXMPP.FMessage.FunMediaType.Document, reportMsg.MediaHash, reportMsg.GetCipherMediaHash(), reportMsg.MediaKey, reportMsg.MediaSize, filepath, MediaHelper.DownloadContextOptions.UserRequest | MediaHelper.DownloadContextOptions.UserWaiting).Subscribe<MediaDownloadProgress>((Action<MediaDownloadProgress>) (ev =>
            {
              Log.l("gdpr", (object) ev.DownloadedSoFar);
              if (ev.DownloadState != MediaDownloadProgress.DownloadStatus.Completed)
                return;
              GdprReport.SetStateDownloaded(filepath);
            }), (Action<Exception>) (ex =>
            {
              int? nullable = new int?();
              if (ex is MediaDownloadException downloadException2 && downloadException2.ErrorCode == MediaDownloadException.DownloadErrorCode.UnexpectedHttpCode)
                nullable = new int?(downloadException2.HttpResponseCode);
              Log.l(ex, string.Format("download gdpr report | http error code:{0}", (object) (nullable ?? -1)));
              this.Dispatcher.BeginInvoke(onDownloadError);
            }), (Action) (() => Log.l("gdpr", "download completed")));
          }
        }
      }
    }

    private void ExportReport()
    {
      string filepath = Settings.GdprReportFilepath;
      bool flag = this.CheckReportFileExists(filepath);
      Log.l("gdpr", "export report | filepath:{0} | exists:{1}", (object) filepath, (object) flag);
      if (!flag)
        return;
      UIUtils.Decision(AppResources.GdprReportExportConfirm, AppResources.Export, AppResources.Cancel).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (toExport =>
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        GdprReportPage.\u003C\u003Ec__DisplayClass6_1 cDisplayClass61 = new GdprReportPage.\u003C\u003Ec__DisplayClass6_1();
        Log.l("gdpr", "user export {0}", (object) toExport);
        if (!toExport)
        {
          Log.l("gdpr", "user cancels export");
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass61.exportFilepath = (string) null;
          try
          {
            string str = string.Format("{0}/{1}.zip", (object) "gdpr_report", (object) AppResources.GdprReportFilename);
            using (IMediaStorage mediaStorage = MediaStorage.Create(filepath))
            {
              using (Stream stream = mediaStorage.OpenFile(filepath))
              {
                using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
                {
                  if (storeForApplication.FileExists(str))
                    storeForApplication.DeleteFile(str);
                  stream.Position = 0L;
                  using (IsolatedStorageFileStream file = storeForApplication.CreateFile(str))
                    stream.CopyTo((Stream) file);
                  stream.Position = 0L;
                }
              }
            }
            // ISSUE: reference to a compiler-generated field
            cDisplayClass61.exportFilepath = str;
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "making gdpr report copy");
            // ISSUE: reference to a compiler-generated field
            cDisplayClass61.exportFilepath = filepath;
          }
          // ISSUE: reference to a compiler-generated field
          cDisplayClass61.dataTransferManager = DataTransferManager.GetForCurrentView();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass61.dataRequestedEventHandler = (TypedEventHandler<DataTransferManager, DataRequestedEventArgs>) null;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          cDisplayClass61.dataRequestedEventHandler = new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object) cDisplayClass61, __methodptr(\u003CExportReport\u003Eb__1));
          // ISSUE: reference to a compiler-generated field
          DataTransferManager dataTransferManager = cDisplayClass61.dataTransferManager;
          // ISSUE: reference to a compiler-generated field
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, EventRegistrationToken>(dataTransferManager.add_DataRequested), new Action<EventRegistrationToken>(dataTransferManager.remove_DataRequested), cDisplayClass61.dataRequestedEventHandler);
          DataTransferManager.ShowShareUI();
        }
      }));
    }

    private void SendReportViaWa(string recipientPickerTitle, string filename)
    {
      string filepath = Settings.GdprReportFilepath;
      bool flag = this.CheckReportFileExists(filepath);
      Log.l("gdpr", "send to Wa report | filepath:{0} | exists:{1}", (object) filepath, (object) flag);
      if (!flag)
        return;
      UIUtils.Decision(AppResources.GdprReportExportToWaConfirm, AppResources.Continue, AppResources.Cancel).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (toSend =>
      {
        Log.l("gdpr", "send via Wa {0}", (object) toSend);
        if (!toSend)
        {
          Log.l("gdpr", "user cancels send");
        }
        else
        {
          Action<Exception> onError = (Action<Exception>) (ex => Log.LogException(ex, "Recipient Picker Exception sending to WA"));
          Action onCompleted = (Action) (() => Log.l("gdpr", "Recipient Picker completed"));
          Action<RecipientListPage.RecipientListResults> onNext = (Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
          {
            List<string> selJids = recipientListResults?.SelectedJids;
            Log.l("gdpr", "Recipients selected: {0}", selJids == null ? (object) -1 : (object) selJids.Count);
            if (selJids == null || selJids.Count < 1)
              return;
            WAThreadPool.QueueUserWorkItem((Action) (() =>
            {
              using (IMediaStorage mediaStorage = MediaStorage.Create(filepath))
              {
                using (Stream stream = mediaStorage.OpenFile(filepath))
                  MediaUpload.SendDocument(selJids, new DocumentMessageUtils.DocumentData()
                  {
                    MimeType = "application/zip",
                    FileExtension = "zip",
                    Thumbnail = (WriteableBitmap) null,
                    Stream = stream,
                    Filename = filename,
                    Title = AppResources.GdprReportFilename
                  }, (Message) null, (string) null, false);
              }
            }));
          });
          RecipientListPage.StartRecipientPicker(recipientPickerTitle, new FunXMPP.FMessage.Type?(FunXMPP.FMessage.Type.Document)).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>(onNext, onError, onCompleted);
        }
      }));
    }

    private bool CheckReportFileExists(string filepath)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(filepath))
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(filepath))
        {
          if (mediaStorage.FileExists(filepath))
            flag = true;
        }
      }
      if (!flag)
      {
        Log.SendCrashLog((Exception) new ArgumentException("report file does not exist"), "export report", logOnlyForRelease: true);
        this.TryDeleteReport((Action) null);
      }
      return flag;
    }

    private void DeleteReport()
    {
      string reportDeleteConfirm = AppResources.GdprReportDeleteConfirm;
      WaRichText.Chunk chunk = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlChunks(reportDeleteConfirm, "b")).SingleOrDefault<WaRichText.Chunk>();
      if (chunk != null)
        chunk.Format = WaRichText.Formats.Bold;
      RichTextBlock.TextSet message = new RichTextBlock.TextSet();
      message.Text = reportDeleteConfirm;
      RichTextBlock.TextSet textSet = message;
      WaRichText.Chunk[] chunkArray;
      if (chunk != null)
        chunkArray = new WaRichText.Chunk[1]{ chunk };
      else
        chunkArray = (WaRichText.Chunk[]) null;
      textSet.PartialFormattings = (IEnumerable<WaRichText.Chunk>) chunkArray;
      UIUtils.Decision(message, AppResources.Delete, AppResources.Cancel).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (toDelete =>
      {
        if (!toDelete)
          return;
        int num;
        this.TryDeleteReport((Action) (() => num = (int) MessageBox.Show(AppResources.GdprReportErrDelete)));
      }));
    }

    private void TryDeleteReport(Action promptTryAgain)
    {
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection != null && connection.IsConnected)
      {
        connection.SendGdprDeleteReport((Action) null, (Action) (() =>
        {
          Action action = promptTryAgain;
          if (action == null)
            return;
          action();
        }));
      }
      else
      {
        Action action = promptTryAgain;
        if (action == null)
          return;
        action();
      }
    }

    private void OnViewModelNotified(KeyValuePair<string, object> p)
    {
      if (p.Key == "Refresh")
        this.UpdatePage();
      else if (p.Key == "ReadyTimeChanged" || p.Key == "CreationTimeChanged" || p.Key == "ReportInfoChanged")
      {
        this.RequestStateBlock.Text = this.viewModel.RequestStateStr;
      }
      else
      {
        if (!(p.Key == "ExpirationTimeChanged"))
          return;
        this.BottomTooltipBlock.Text = this.viewModel.BottomTooltipStr;
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      this.viewModel.RefreshState();
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection != null && connection.IsConnected)
      {
        if (Settings.GdprReportState == GdprReport.States.Requesting)
          GdprReport.SetStateInit();
        this.viewModel.PendingStatusChecking = true;
        connection.SendGdprCheckReportStatus((Action) (() =>
        {
          this.viewModel.PendingStatusChecking = false;
          this.Dispatcher.BeginInvoke(new Action(this.UpdatePage));
        }), (Action) null);
      }
      if (Settings.GdprReportState == GdprReport.States.Downloading)
        this.DownloadReport();
      this.UpdatePage();
    }

    private void MainButton_Click(object sender, RoutedEventArgs e)
    {
      switch (Settings.GdprReportState)
      {
        case GdprReport.States.Init:
          Log.l("gdpr", "user taps request button");
          this.RequestReport();
          break;
        case GdprReport.States.Ready:
          Log.l("gdpr", "user taps download button");
          this.DownloadReport();
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.GDPR_REPORT_DOWNLOAD);
          break;
        case GdprReport.States.Downloaded:
          Log.l("gdpr", "user taps export button");
          this.ExportReport();
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.GDPR_REPORT_EXPORT);
          break;
      }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      Log.l("gdpr", "user taps delete button");
      this.DeleteReport();
    }

    private void SendViaWaButton_Click(object sender, RoutedEventArgs e)
    {
      Log.l("gdpr", "user taps send to wa button");
      this.SendReportViaWa(this.viewModel.SendViaWaButtonLabel, AppResources.GdprReportFilename);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/GdprReportPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.TopIcon = (Image) this.FindName("TopIcon");
      this.ContentList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ContentList");
      this.TopTooltipBlock = (RichTextBlock) this.FindName("TopTooltipBlock");
      this.MainButton = (Button) this.FindName("MainButton");
      this.SendViaWaButton = (Button) this.FindName("SendViaWaButton");
      this.RequestStateBlock = (TextBlock) this.FindName("RequestStateBlock");
      this.DeleteButton = (Button) this.FindName("DeleteButton");
      this.BottomTooltipBlock = (TextBlock) this.FindName("BottomTooltipBlock");
    }
  }
}
