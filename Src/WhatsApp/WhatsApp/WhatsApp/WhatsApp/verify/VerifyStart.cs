// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.VerifyStart
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.CommonOps;


namespace WhatsApp.verify
{
  public class VerifyStart : PhoneApplicationPage
  {
    private const string LogHeader = "reg";
    private IDisposable verificationSubscription;
    private DispatcherTimer registrationTimeout = new DispatcherTimer();
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal ProgressBar ProgressBar;
    internal TextBlock ErrorTextBlock;
    internal Button EditButton;
    internal Button CustomActionButton;
    private bool _contentLoaded;

    public VerifyStart()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.PageTitle.SmallTitle = AppResources.VerificationTitle;
      this.EditButton.Content = (object) AppResources.EditPhoneNumber;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.NewlyEntered)
      {
        this.registrationTimeout.Interval = TimeSpan.FromMinutes(5.0);
        this.registrationTimeout.Tick += new EventHandler(this.OnRegistrationTimeout);
        this.registrationTimeout.Start();
        ConversionRecordHelper.ClearConversionRecords();
      }
      this.verificationSubscription = this.GetLayoutUpdatedAsync().Take<Unit>(1).SelectMany((Func<Unit, IObservable<bool>>) (layout => this.CheckEncryptionIdentity()), (layout, hasIdentity) => new
      {
        layout = layout,
        hasIdentity = hasIdentity
      }).Where(_param1 => _param1.hasIdentity).SelectMany(_param1 => this.CheckDataConnection(), (_param1, hasData) => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        hasData = hasData
      }).Where(_param1 => _param1.hasData).SelectMany(_param1 => this.SameDeviceCheck().Concat<Uri>(this.CheckPhoneService().Where<bool>((Func<bool, bool>) (phoneService => phoneService)).SelectMany<bool, Uri, Uri>((Func<bool, IObservable<Uri>>) (phoneService => this.GetNextStep()), (Func<bool, Uri, Uri>) ((phoneService, dest2) => dest2))).Take<Uri>(1), (_param1, dest) => dest).ObserveOnDispatcher<Uri>().Do<Uri>((Action<Uri>) (d => this.NavigationService.Navigate(d))).Subscribe<Uri>();
    }

    private void OnRegistrationTimeout(object sender, EventArgs e)
    {
      this.registrationTimeout.Stop();
      this.CustomActionButton.Content = (object) AppResources.ContactSupportButton;
      this.CustomActionButton.Tag = (object) (Action) (() =>
      {
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddString("context", ContactSupportHelper.AppendPhoneNumberIfNotLoggedIn("server_timeout"));
        uriParams.AddBool("ClearStack", true);
        NavUtils.NavigateToPage("ContactSupportPage", uriParams);
      });
      this.CustomActionButton.Visibility = Visibility.Visible;
    }

    private IObservable<bool> CheckPhoneService()
    {
      return Observable.Defer<bool>((Func<IObservable<bool>>) (() => Observable.Return<bool>(DeviceNetworkInformation.IsNetworkAvailable))).Do<bool>((Action<bool>) (b =>
      {
        if (b)
          return;
        this.ReportError(AppResources.NoWireless);
      }));
    }

    private IObservable<bool> CheckDataConnection()
    {
      return Observable.Defer<bool>((Func<IObservable<bool>>) (() => Observable.Return<bool>(DeviceNetworkInformation.IsNetworkAvailable))).Do<bool>((Action<bool>) (b =>
      {
        if (b)
          return;
        this.ReportError(AppResources.NoData);
      }));
    }

    private IObservable<bool> CheckEncryptionIdentity()
    {
      return Observable.Defer<bool>((Func<IObservable<bool>>) (() =>
      {
        bool flag;
        try
        {
          byte[] clientStaticPublicKey = Settings.ClientStaticPublicKey;
          Axolotl encryption = AppState.GetConnection().Encryption;
          int localRegistrationId = (int) encryption.Store.LocalRegistrationId;
          byte[] identityKeyForSending = encryption.Store.IdentityKeyForSending;
          AxolotlPreKey latestSignedPreKey = encryption.Store.LatestSignedPreKey;
          flag = identityKeyForSending != null && latestSignedPreKey != null && clientStaticPublicKey != null && clientStaticPublicKey.Length != 0;
        }
        catch (Exception ex)
        {
          flag = false;
        }
        return Observable.Return<bool>(flag);
      })).Do<bool>((Action<bool>) (b =>
      {
        if (b)
          return;
        this.ReportError(AppResources.EncryptionIdentityGenerationFail);
      }));
    }

    public string VerificationText
    {
      get
      {
        return string.Format(AppResources.VerifyingNumber, (object) PhoneNumberFormatter.FormatInternationalNumber(Settings.CountryCode + Settings.PhoneNumber));
      }
    }

    private void ReportError(string p)
    {
      Log.l("reg", "showing error message: {0}", (object) p);
      this.ProgressBar.Visibility = Visibility.Collapsed;
      this.ErrorTextBlock.Text = p;
      this.ErrorTextBlock.Visibility = Visibility.Visible;
      if (Settings.OldChatID != null)
      {
        Log.l("reg", "change number pending, hit error, provide cancel option | old:{0},new:{1}", (object) Settings.OldChatID, (object) Settings.ChatID);
        this.EditButton.Content = (object) AppResources.CancelButton;
      }
      this.EditButton.Visibility = Visibility.Visible;
    }

    private IObservable<Uri> GetNextStep()
    {
      return Observable.Create<Uri>((Func<IObserver<Uri>, Action>) (o =>
      {
        string nextPage = (string) null;
        bool flag = true;
        Func<string> func = (Func<string>) (() => string.Format("{0}{1}ClearStack=true", (object) nextPage, (object) (char) (nextPage.IndexOf('?') >= 0 ? 38 : 63)));
        PhoneNumberVerificationState verificationState = Settings.PhoneNumberVerificationState;
        Log.l("reg", "get next step for state {0}", (object) verificationState);
        switch (verificationState)
        {
          case PhoneNumberVerificationState.SameDeviceFailed:
            nextPage = UriUtils.CreatePageUriStr("SendCode", "type=sms", "verify");
            break;
          case PhoneNumberVerificationState.ServerSentSms:
            nextPage = UriUtils.CreatePageUriStr("EnterCode", "type=sms", "verify");
            break;
          case PhoneNumberVerificationState.ServerSendSmsFailed:
            nextPage = UriUtils.CreatePageUriStr("SendCode", "type=voice", "verify");
            break;
          case PhoneNumberVerificationState.ServerSentVoice:
            nextPage = UriUtils.CreatePageUriStr("EnterCode", "type=voice", "verify");
            break;
          case PhoneNumberVerificationState.ServerSendVoiceFailed:
            nextPage = (string) null;
            break;
          case PhoneNumberVerificationState.VerifiedPendingBackupCheck:
            if (this.ShouldCheckForCloudBackup())
            {
              flag = false;
              nextPage = UriUtils.CreatePageUriStr("CloudBackupCheckPage");
              break;
            }
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingHistoryRestore;
            goto case PhoneNumberVerificationState.VerifiedPendingHistoryRestore;
          case PhoneNumberVerificationState.VerifiedPendingHistoryRestore:
            if (this.HasHistoryBackup() && !Settings.SuppressRestoreFromBackupAtReg)
            {
              Settings.SuppressRestoreFromBackupAtReg = false;
              flag = false;
              nextPage = UriUtils.CreatePageUriStr("HistoryRestore");
              break;
            }
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.Verified;
            goto case PhoneNumberVerificationState.Verified;
          case PhoneNumberVerificationState.VerifiedPendingSecurityCode:
            nextPage = UriUtils.CreatePageUriStr("EnterSecurityCode", "", "verify");
            break;
          case PhoneNumberVerificationState.Verified:
            nextPage = "/PageSelect";
            break;
        }
        if (nextPage != null)
          o.OnNext(new Uri(func(), UriKind.Relative));
        if (flag)
          o.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private bool ShouldCheckForCloudBackup()
    {
      return !Settings.SuppressRestoreFromBackupAtReg && Settings.PhoneNumberAccountCreationType != PhoneNumberAccountCreationType.New;
    }

    private bool HasHistoryBackup()
    {
      return OneDriveRestoreProcessor.HasRemoteBackup() || Backup.GetLastBackupTime().HasValue;
    }

    private IObservable<Uri> SameDeviceCheck()
    {
      switch (Settings.PhoneNumberVerificationState)
      {
        case PhoneNumberVerificationState.ServerSentSms:
        case PhoneNumberVerificationState.ServerSendSmsFailed:
        case PhoneNumberVerificationState.ServerSentVoice:
        case PhoneNumberVerificationState.VerifiedPendingBackupCheck:
        case PhoneNumberVerificationState.VerifiedPendingHistoryRestore:
        case PhoneNumberVerificationState.VerifiedPendingSecurityCode:
        case PhoneNumberVerificationState.Verified:
          return Observable.Empty<Uri>();
        default:
          return Observable.CreateWithDisposable<Uri>((Func<IObserver<Uri>, IDisposable>) (o =>
          {
            Settings.RecoveryTokenSet = true;
            this.CustomActionButton.Visibility = Visibility.Collapsed;
            Log.l("reg", "exists check");
            return Registration.Exists(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken).Timeout<Registration.RegResult>(TimeSpan.FromSeconds(30.0)).ObserveOnDispatcher<Registration.RegResult>().Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (status =>
            {
              if (status.Status == "ok")
              {
                Settings.ChatID = status.ChatID;
                Settings.PhoneNumberAccountCreationType = Registration.GetAccountCreationType(status);
                Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingBackupCheck;
              }
              else if (status.Status == "fail")
              {
                if (status.Reason == "incorrect")
                  Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.SameDeviceFailed;
                if (status.Reason == "security_code")
                {
                  Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingSecurityCode;
                  Settings.TwoFactorWipeType = status.WipeType;
                  Settings.TwoFactorWipeToken = status.WipeToken;
                }
                else if (status.ErrorString != null)
                {
                  if (status.Reason == "old_version")
                  {
                    this.NavigationService.Navigate(UriUtils.CreatePageUri("UpdateVersion", "ClearStack=true"));
                  }
                  else
                  {
                    this.ReportError(status.ErrorString);
                    if (status.HasSupportAction())
                    {
                      this.CustomActionButton.Content = (object) status.ActionTitle;
                      this.CustomActionButton.Tag = (object) (Action) (() => status.PerformSupportAction());
                      this.CustomActionButton.Visibility = Visibility.Visible;
                    }
                    this.PageTitle.Visibility = status.Reason == "blocked" ? Visibility.Collapsed : Visibility.Visible;
                  }
                }
              }
              o.OnCompleted();
            }), (Action<Exception>) (ex =>
            {
              Log.l(ex, "verify start");
              if (ex.GetHResult() == 2147954437U)
                this.ReportError(string.Format(AppResources.ClockSkew, (object) DateTime.Now));
              else
                this.ReportError(AppResources.CouldNotConnect);
              o.OnCompleted();
            }));
          }));
      }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
      if (Settings.OldChatID == null)
      {
        Log.l("reg", "user hit edit number button");
        NavUtils.NavigateToPage(this.NavigationService, "PhoneNumberEntry", "ClearStack=true");
      }
      else
      {
        Log.l("reg", "user hit cancel change number button");
        AccountManagement.AbortChangePhoneNumber();
        NavUtils.NavigateHome(this.NavigationService);
      }
    }

    private void CustomAction_Click(object sender, RoutedEventArgs e)
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
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/verify/VerifyStart.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
      this.ErrorTextBlock = (TextBlock) this.FindName("ErrorTextBlock");
      this.EditButton = (Button) this.FindName("EditButton");
      this.CustomActionButton = (Button) this.FindName("CustomActionButton");
    }
  }
}
