// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprReportPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GdprReportPageViewModel : PageViewModelBase
  {
    private GdprReport.States reportState;
    private IDisposable settingsChangedSub;

    public bool PendingStatusChecking { get; set; }

    public override string PageTitle => AppResources.GdprReportTitle;

    public RichTextBlock.TextSet TopTooltipRichText
    {
      get
      {
        string str1 = AppResources.GdprReportIntro + " ";
        string learnMoreText = AppResources.LearnMoreText;
        WaRichText.Chunk chunk = new WaRichText.Chunk(str1.Length, learnMoreText.Length, WaRichText.Formats.Link, WaWebUrls.FaqUrlGdprReport);
        string str2 = str1 + learnMoreText;
        IEnumerable<WaRichText.Chunk> chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
        {
          chunk
        };
        return new RichTextBlock.TextSet()
        {
          Text = str2 ?? "",
          PartialFormattings = chunks
        };
      }
    }

    public bool ShouldEnableMainButton
    {
      get
      {
        bool enableMainButton = true;
        switch (this.reportState)
        {
          case GdprReport.States.RequestSent:
          case GdprReport.States.Downloading:
          case GdprReport.States.Requesting:
            enableMainButton = false;
            break;
          case GdprReport.States.Ready:
            enableMainButton = !this.PendingStatusChecking;
            break;
        }
        return enableMainButton;
      }
    }

    public double MainButtonOpacity => !this.ShouldEnableMainButton ? 0.5 : 1.0;

    public string MainButtonLabel
    {
      get
      {
        string str;
        switch (this.reportState)
        {
          case GdprReport.States.RequestSent:
            str = AppResources.GdprButtonRequestSent;
            break;
          case GdprReport.States.Ready:
            str = AppResources.GdprButtonDownloadReport;
            break;
          case GdprReport.States.Downloading:
            str = AppResources.GdprButtonDownloading;
            break;
          case GdprReport.States.Downloaded:
            str = AppResources.GdprButtonExportReport;
            break;
          case GdprReport.States.Requesting:
            str = AppResources.GdprButtonRequesting;
            break;
          default:
            str = AppResources.GdprButtonRequestReport;
            break;
        }
        return str ?? "";
      }
    }

    public Visibility RequestStateVisibility
    {
      get
      {
        bool flag = true;
        switch (this.reportState)
        {
          case GdprReport.States.Init:
          case GdprReport.States.Downloading:
          case GdprReport.States.Requesting:
            flag = false;
            break;
        }
        return flag.ToVisibility();
      }
    }

    public string RequestStateStr
    {
      get
      {
        string str1 = (string) null;
        switch (this.reportState)
        {
          case GdprReport.States.RequestSent:
            DateTime? reportReadyTimeUtc = Settings.GdprReportReadyTimeUtc;
            if (reportReadyTimeUtc.HasValue)
            {
              str1 = string.Format(AppResources.GdprReportReadyDate, (object) DateTimeUtils.FunTimeToPhoneTime(reportReadyTimeUtc.Value).ToMonthDayString());
              break;
            }
            break;
          case GdprReport.States.Ready:
            if (this.ReportMsg != null)
            {
              string str2 = Utils.FileSizeFormatter.Format(this.ReportMsg.MediaSize);
              DateTime? reportCreationTimeUtc = Settings.GdprReportCreationTimeUtc;
              if (reportCreationTimeUtc.HasValue)
              {
                str1 = string.Format("{0} • {1}", (object) DateTimeUtils.FunTimeToPhoneTime(reportCreationTimeUtc.Value).ToMonthDayString(), (object) str2);
                break;
              }
              break;
            }
            break;
          case GdprReport.States.Downloaded:
            if (this.ReportMsg != null)
            {
              DateTime? reportCreationTimeUtc = Settings.GdprReportCreationTimeUtc;
              if (reportCreationTimeUtc.HasValue)
              {
                str1 = DateTimeUtils.FunTimeToPhoneTime(reportCreationTimeUtc.Value).ToMonthDayString();
                break;
              }
              break;
            }
            break;
        }
        return str1 ?? "";
      }
    }

    public Visibility DeleteButtonVisibility
    {
      get => (this.reportState == GdprReport.States.Downloaded).ToVisibility();
    }

    public string DeleteButtonLabel => AppResources.GdprButtonDeleteReport;

    public Visibility SendViaWaButtonVisibility
    {
      get => (this.reportState == GdprReport.States.Downloaded).ToVisibility();
    }

    public string SendViaWaButtonLabel
    {
      get
      {
        return string.Format(AppResources.GdprButtonExportReportToWa, (object) AppResources.OfficialName);
      }
    }

    public string BottomTooltipStr
    {
      get
      {
        string str = (string) null;
        switch (this.reportState)
        {
          case GdprReport.States.Ready:
            DateTime? expirationTimeUtc = Settings.GdprReportExpirationTimeUtc;
            if (expirationTimeUtc.HasValue)
            {
              str = string.Format(AppResources.GdprReportExpiration, (object) DateTimeUtils.FunTimeToPhoneTime(expirationTimeUtc.Value).ToMonthDayString());
              goto case GdprReport.States.Downloading;
            }
            else
              goto case GdprReport.States.Downloading;
          case GdprReport.States.Downloading:
          case GdprReport.States.Downloaded:
            return str ?? "";
          default:
            int number = 3;
            DateTime? reportReadyTimeUtc = Settings.GdprReportReadyTimeUtc;
            if (reportReadyTimeUtc.HasValue)
            {
              number = (int) (reportReadyTimeUtc.Value - FunRunner.CurrentServerTimeUtc).TotalDays;
              if (number < 1)
                number = 1;
            }
            str = string.Format("{0}\n\n{1}", (object) Plurals.Instance.GetString(AppResources.GdprReportReadyCountdownPlural, number), (object) AppResources.GdprReportRequestCancelPolicy);
            goto case GdprReport.States.Downloading;
        }
      }
    }

    public Message ReportMsg { get; private set; }

    public GdprReportPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.ReportMsg = GdprReport.CreateMessageFromReportInfo(Settings.GdprReportInfo);
      this.RefreshState();
      this.settingsChangedSub = Settings.GetSettingsChangedObservable(new Settings.Key[5]
      {
        Settings.Key.GdprReportState,
        Settings.Key.GdprReportCreationTimeUtc,
        Settings.Key.GdprReportExpirationTimeUtc,
        Settings.Key.GdprReportReadyTimeUtc,
        Settings.Key.GdprReportInfo
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>(new Action<Settings.Key>(this.OnSettingsChanged));
    }

    public void RefreshState() => this.reportState = Settings.GdprReportState;

    protected override void DisposeManagedResources()
    {
      this.settingsChangedSub.SafeDispose();
      this.settingsChangedSub = (IDisposable) null;
      base.DisposeManagedResources();
    }

    private void OnSettingsChanged(Settings.Key key)
    {
      switch (key)
      {
        case Settings.Key.GdprReportState:
          this.reportState = Settings.GdprReportState;
          this.Notify("Refresh");
          break;
        case Settings.Key.GdprReportReadyTimeUtc:
          this.Notify("ReadyTimeChanged");
          break;
        case Settings.Key.GdprReportCreationTimeUtc:
          this.Notify("CreationTimeChanged");
          break;
        case Settings.Key.GdprReportInfo:
          this.ReportMsg = GdprReport.CreateMessageFromReportInfo(Settings.GdprReportInfo);
          this.Notify("ReportInfoChanged");
          break;
        case Settings.Key.GdprReportExpirationTimeUtc:
          this.Notify("ExpirationTimeChanged");
          break;
      }
    }
  }
}
