// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.EnterCode
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp.verify
{
  public class EnterCode : PhoneApplicationPage
  {
    private static int editButtonUsed;
    private string codeType;
    private IDisposable timerSubscription;
    private IDisposable registrationSubscription;
    private IDisposable inputResetSubscription;
    private AppBarWrapper appBarWrapper;
    private bool isVoiceButtonEnabled;
    private Registration.CodeEntryMethod currentMethod;
    private System.Windows.Input.Key[] digits = new System.Windows.Input.Key[10]
    {
      System.Windows.Input.Key.D0,
      System.Windows.Input.Key.D1,
      System.Windows.Input.Key.D2,
      System.Windows.Input.Key.D3,
      System.Windows.Input.Key.D4,
      System.Windows.Input.Key.D5,
      System.Windows.Input.Key.D6,
      System.Windows.Input.Key.D7,
      System.Windows.Input.Key.D8,
      System.Windows.Input.Key.D9
    };
    internal Storyboard WaitTime;
    internal DoubleAnimation TimeToWait;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock ExplanationText;
    internal Grid CodeInput;
    internal TextBox HiddenCode;
    internal TextBlock VisibleCode;
    internal TextBlock TimerTextBlock;
    internal Grid VoiceButton;
    internal TextBlock VoiceExplain;
    internal RoundButton CallMeButton;
    internal TextBlock CallButtonCaptionBlock;
    internal Button ContactSupportButton;
    internal TextBlock ProgressBarTitle;
    internal ProgressBar ProgressBar;
    private bool _contentLoaded;

    public EnterCode()
    {
      this.InitializeComponent();
      this.TitlePanel.SmallTitle = AppResources.EnterCodeTitle;
      this.TitlePanel.Mode = PageTitlePanel.Modes.NotZoomed;
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      if (Settings.OldChatID != null && this.ApplicationBar.Buttons[0] is ApplicationBarIconButton button)
      {
        button.Text = "Cancel";
        button.IconUri = new Uri("/Images/assets/notheme/x.png", UriKind.Relative);
      }
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.CallMeButton.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/reg-phone-icon.png");
      this.CallButtonCaptionBlock.Text = AppResources.TryVoiceVerification;
      this.Code_TextChanged((object) null, (TextChangedEventArgs) null);
      this.Loaded += new RoutedEventHandler(this.EnterCode_Loaded);
    }

    private void EnterCode_Loaded(object sender, RoutedEventArgs e) => this.HiddenCode.Focus();

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.timerSubscription.SafeDispose();
      this.timerSubscription = (IDisposable) null;
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.inputResetSubscription.SafeDispose();
      this.inputResetSubscription = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.appBarWrapper == null)
        this.appBarWrapper = new AppBarWrapper(this.ApplicationBar);
      this.appBarWrapper.IsEnabled = EnterCode.editButtonUsed == 0;
      if (EnterCode.editButtonUsed > 0)
        Observable.Timer(TimeSpan.FromSeconds(EnterCode.editButtonUsed > 1 ? 60.0 : 30.0)).Take<long>(1).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ => this.appBarWrapper.IsEnabled = true));
      base.OnNavigatedTo(e);
      this.codeType = this.NavigationContext.QueryString["type"];
      Log.WriteLineDebug("code type {0}", (object) this.codeType);
      string str1 = (string) null;
      this.NavigationContext.QueryString.TryGetValue("code", out str1);
      if (!string.IsNullOrEmpty(str1))
      {
        Log.WriteLineDebug("code came from sms browser link {0}", (object) str1);
        SMSCodeHelper.SaveSMSCode(str1);
        this.currentMethod = Registration.CodeEntryMethod.Link;
      }
      else
      {
        str1 = SMSCodeHelper.GetSavedSMSCode();
        if (!string.IsNullOrEmpty(str1))
          this.currentMethod = Registration.CodeEntryMethod.Retry;
      }
      switch (e.NavigationMode)
      {
        case NavigationMode.New:
        case NavigationMode.Back:
        case NavigationMode.Reset:
          DateTime timeoutUtc = Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.ServerSentVoice ? Settings.PhoneNumberVerificationTimeoutUtc : Settings.PhoneNumberVerificationRetryUtc;
          string str2 = PhoneNumberFormatter.FormatInternationalNumber(Settings.CountryCode + Settings.PhoneNumber);
          string str3 = string.Format("{0} {1}", this.codeType == "voice" ? (object) AppResources.CodeExplanationVoice : (object) AppResources.CodeExplanation, (object) string.Format(AppResources.CodeExplanationTail, (object) Settings.CodeLength));
          InlineCollection inlines = this.ExplanationText.Inlines;
          inlines.Clear();
          string[] strArray = new string[1]{ str2 };
          foreach (Utils.FormatResult formatResult in Utils.Format(str3, strArray))
          {
            Run run = new Run();
            if (formatResult.Index.HasValue)
            {
              run.FontSize = (double) this.Resources[(object) "PhoneFontSizeMediumLarge"];
              run.FontWeight = FontWeights.Bold;
            }
            else
              run.FontSize = (double) this.Resources[(object) "PhoneFontSizeMedium"];
            run.Text = formatResult.Value;
            inlines.Add((Inline) run);
          }
          PhoneNumberVerificationState prevState = Settings.PhoneNumberVerificationState;
          if (timeoutUtc > FunRunner.CurrentServerTimeUtc)
          {
            Log.l("reg", "timeout at {0}", (object) timeoutUtc);
            string timerTextFmt = (string) null;
            if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.ServerSentVoice)
            {
              timerTextFmt = AppResources.TryVoiceVerificationAgainAt;
              this.VoiceExplain.Text = AppResources.TryVoiceCallAgain;
            }
            else
              timerTextFmt = AppResources.TryVoiceVerificationAt;
            this.timerSubscription.SafeDispose();
            this.timerSubscription = Observable.Timer(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(1.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
            {
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              if (timeoutUtc > currentServerTimeUtc)
              {
                this.TimerTextBlock.Text = string.Format(timerTextFmt, (object) (timeoutUtc - currentServerTimeUtc).ToFriendlyString());
              }
              else
              {
                this.timerSubscription.SafeDispose();
                this.timerSubscription = (IDisposable) null;
                this.EnableVoiceButton(prevState);
              }
            }));
          }
          else
            this.EnableVoiceButton(prevState);
          if (Settings.CodeEntryWaitToRetryUtc > FunRunner.CurrentServerTimeUtc)
          {
            this.WaitToReenterCode(smsCodeToTry: str1);
            str1 = (string) null;
          }
          if (str1 != null)
          {
            this.HiddenCode.Text = str1;
            this.Code_TextChanged((object) null, (TextChangedEventArgs) null);
            break;
          }
          break;
      }
      this.Dispatcher.BeginInvoke((Action) (() => this.HiddenCode.Focus()));
    }

    private void EnableVoiceButton(PhoneNumberVerificationState prevState)
    {
      if (this.isVoiceButtonEnabled)
      {
        Log.l("reg", "enable voice button skipped");
      }
      else
      {
        this.isVoiceButtonEnabled = true;
        if (Settings.PhoneNumberVerificationState == prevState && this.codeType != "voice")
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.ServerSendSmsFailed;
        this.TimerTextBlock.Visibility = Visibility.Collapsed;
        this.VoiceButton.Visibility = Visibility.Visible;
        Log.l("reg", "enabled voice button");
      }
    }

    private void Code_KeyDown(object sender, KeyEventArgs e)
    {
      if ((sender as TextBox).Text.Length >= Settings.CodeLength)
        e.Handled = true;
      if (((IEnumerable<System.Windows.Input.Key>) this.digits).Contains<System.Windows.Input.Key>(e.Key))
        return;
      e.Handled = true;
    }

    private void Code_TextChanged(object sender, TextChangedEventArgs e)
    {
      string digits = this.HiddenCode.Text.ExtractDigits();
      int length = digits.Length;
      int num = Settings.CodeLength / 2;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < num; ++index)
      {
        if (index < length)
        {
          stringBuilder.Append(digits[index]);
          stringBuilder.Append(' ');
        }
        else
        {
          stringBuilder.Append("-");
          stringBuilder.Append(' ');
        }
      }
      stringBuilder.Append(' ');
      stringBuilder.Append(' ');
      stringBuilder.Append(' ');
      for (int index = num; index < num * 2; ++index)
      {
        if (index < length)
        {
          stringBuilder.Append(digits[index]);
          stringBuilder.Append(' ');
        }
        else
        {
          stringBuilder.Append("-");
          stringBuilder.Append(' ');
        }
      }
      --stringBuilder.Length;
      this.VisibleCode.Text = stringBuilder.ToString();
      if (length != Settings.CodeLength)
        return;
      this.CheckCode(digits);
    }

    private void CodeInput_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.HiddenCode.Focus();
    }

    private void SetReadOnly()
    {
      this.HiddenCode.IsEnabled = false;
      this.ProgressBar.Visibility = Visibility.Visible;
      this.Dispatcher.BeginInvoke((Action) (() => this.Focus()));
    }

    private void ClearReadOnly()
    {
      this.HiddenCode.IsEnabled = true;
      this.HiddenCode.Text = string.Empty;
      this.ProgressBarTitle.Visibility = Visibility.Collapsed;
      this.ProgressBar.Visibility = Visibility.Collapsed;
    }

    private void NavWorkaround(Action a)
    {
      this.ClearReadOnly();
      a();
    }

    private void Navigate(Uri uri)
    {
      this.NavWorkaround((Action) (() => this.NavigationService.Navigate(uri)));
    }

    private void CheckCode(string code)
    {
      if (this.registrationSubscription != null)
        return;
      Log.WriteLineDebug("trying to reg {0} {1} with code {2}", (object) Settings.CountryCode, (object) Settings.PhoneNumber, (object) code);
      this.SetReadOnly();
      this.registrationSubscription = Registration.Register(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken, code, this.currentMethod).ObserveOnDispatcher<Registration.RegResult>().Finally<Registration.RegResult>((Action) (() => this.registrationSubscription = (IDisposable) null)).Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (resp =>
      {
        if (resp.Reason != "temporarily_unavailable" && resp.Reason != "guessed_too_fast")
          SMSCodeHelper.SaveSMSCode((string) null);
        if (resp.Status == "ok" || resp.Status == "expired")
        {
          Log.WriteLineDebug("registered ok, status {0}", (object) resp.Status);
          Settings.ChatID = resp.ChatID;
          Settings.PhoneNumberAccountCreationType = Registration.GetAccountCreationType(resp);
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingBackupCheck;
          this.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify"));
        }
        else if (resp.PromptForReenter)
        {
          Log.WriteLineDebug("prompted to reenter code");
          if (this.codeType == "voice")
            this.ContactSupportButton.Visibility = Visibility.Visible;
          this.WaitToReenterCode(resp.ErrorString, resp.ActionTitle);
        }
        else if (resp.Reason == "security_code")
        {
          Settings.ChatID = resp.Login;
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingSecurityCode;
          Settings.TwoFactorWipeType = resp.WipeType;
          Settings.TwoFactorWipeToken = resp.WipeToken;
          Settings.TwoFactorWipeTimeUntilTokenValid = resp.WipeWait.HasValue ? new DateTime?(DateTime.Now + TimeSpan.FromSeconds((double) resp.WipeWait.Value)) : new DateTime?();
          SMSCodeHelper.SaveSMSCode(code);
          this.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true;", "verify"));
        }
        else
        {
          if (resp.ErrorString == null)
            return;
          Log.WriteLineDebug("got error {0}", (object) resp.ErrorString);
          if (this.codeType == "voice")
            this.ContactSupportButton.Visibility = Visibility.Visible;
          if (resp.HasSupportAction())
          {
            Log.WriteLineDebug("enabling support action {0}", (object) resp.ActionTitle);
            UIUtils.MessageBox((string) null, resp.ErrorString, (IEnumerable<string>) new string[2]
            {
              AppResources.Cancel,
              resp.ActionTitle
            }, (Action<int>) (buttonIdx =>
            {
              if (buttonIdx != 1)
                return;
              this.NavWorkaround(new Action(((RegResultExtensions) resp).PerformSupportAction));
            }));
          }
          else
          {
            Log.WriteLineDebug("throwing user back to phone number entry page");
            int num = (int) MessageBox.Show(resp.ErrorString);
            if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.NewlyEntered || Settings.OldChatID != null)
              return;
            this.Navigate(UriUtils.CreatePageUri("PhoneNumberEntry", "ClearStack=true"));
          }
        }
      }), (Action<Exception>) (ex =>
      {
        Log.LogException(ex, "register");
        this.ClearReadOnly();
        int num = (int) MessageBox.Show(string.Format(AppResources.VerificationError, (object) ex.GetType().Name));
      }));
      this.currentMethod = Registration.CodeEntryMethod.Retry;
    }

    private void WaitToReenterCode(string errorMessage = "", string title = "", string smsCodeToTry = null)
    {
      this.SetReadOnly();
      DateTime entryWaitToRetryUtc = Settings.CodeEntryWaitToRetryUtc;
      DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
      TimeSpan timeSpan;
      int num1;
      if (!(entryWaitToRetryUtc < currentServerTimeUtc))
      {
        timeSpan = entryWaitToRetryUtc - currentServerTimeUtc;
        num1 = (int) timeSpan.TotalSeconds;
      }
      else
        num1 = 0;
      int seconds = num1;
      if (seconds > 2)
      {
        this.ProgressBarTitle.Visibility = Visibility.Visible;
        int num2 = 0;
        DateTime progressbarStartUtc = Settings.RegProgressbarStartUtc;
        if (progressbarStartUtc != new DateTime(0L) && entryWaitToRetryUtc > progressbarStartUtc && progressbarStartUtc < currentServerTimeUtc)
        {
          timeSpan = entryWaitToRetryUtc - progressbarStartUtc;
          double totalSeconds = timeSpan.TotalSeconds;
          num2 = (int) ((totalSeconds - (double) seconds) * 100.0 / totalSeconds);
        }
        this.ProgressBar.IsIndeterminate = false;
        this.ProgressBar.Maximum = 100.0;
        this.ProgressBar.Value = (double) num2;
        this.TimeToWait.Duration = new Duration(TimeSpan.FromSeconds((double) seconds));
        Storyboarder.Perform(this.WaitTime);
      }
      else
        this.ProgressBar.IsIndeterminate = true;
      this.inputResetSubscription.SafeDispose();
      this.inputResetSubscription = Observable.Timer(TimeSpan.FromSeconds((double) seconds)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        this.ClearReadOnly();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.HiddenCode.Focus();
          if (string.IsNullOrEmpty(smsCodeToTry))
            return;
          this.HiddenCode.Text = smsCodeToTry;
        }));
      }));
      if (Settings.RegProgressbarStartUtc == new DateTime(0L))
      {
        Settings.RegProgressbarStartUtc = FunRunner.CurrentServerTimeUtc;
        if (string.IsNullOrEmpty(errorMessage))
        {
          errorMessage = AppResources.WrongCodeMessage;
          if (seconds > 2)
            errorMessage = string.Format(AppResources.ReenterCodeLaterWithTime, (object) Registration.RegResult.WaitStringFromTime(seconds));
        }
        if (string.IsNullOrEmpty(title))
          title = AppResources.ReenterCode;
        Log.WriteLineDebug("(popup){0}: {1}", (object) title, (object) errorMessage);
        int num3 = (int) MessageBox.Show(errorMessage, title, MessageBoxButton.OK);
      }
      Log.WriteLineDebug("wrong code, wait time {0} seconds", (object) seconds);
    }

    private void Edit_Click(object sender, EventArgs e)
    {
      ++EnterCode.editButtonUsed;
      Log.WriteLineDebug("user hit edit phone number");
      if (Settings.OldChatID == null)
      {
        this.Navigate(UriUtils.CreatePageUri("PhoneNumberEntry", "ClearStack=true"));
      }
      else
      {
        Log.WriteLineDebug("change number: user clicked cancel on EnterCode screen. was about to change from {0} to {1}", (object) Settings.OldChatID, (object) Settings.ChatID);
        AccountManagement.AbortChangePhoneNumber();
        this.Navigate(new Uri("/PageSelect?ClearStack=true", UriKind.Relative));
      }
    }

    private void VoiceButton_Click(object sender, EventArgs e)
    {
      Log.l("reg", "user request code via voice");
      if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.ServerSentVoice && Settings.PhoneNumberVerificationRetryUtc > FunRunner.CurrentServerTimeUtc)
      {
        int num = (int) MessageBox.Show(string.Format(AppResources.ReenterCodeLaterWithTime, (object) Registration.RegResult.WaitStringFromTime((int) (Settings.PhoneNumberVerificationRetryUtc - FunRunner.CurrentServerTimeUtc).TotalSeconds)));
      }
      else
      {
        Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.ServerSendSmsFailed;
        this.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify"));
      }
    }

    private void VoiceExplain_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.Focus();
    }

    private void ContactSupport_Click(object sender, RoutedEventArgs e)
    {
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("context", ContactSupportHelper.AppendPhoneNumberIfNotLoggedIn("code entry page"));
      uriParams.AddBool("ClearStack", true);
      this.NavWorkaround((Action) (() => NavUtils.NavigateToPage("ContactSupportPage", uriParams)));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/verify/EnterCode.xaml", UriKind.Relative));
      this.WaitTime = (Storyboard) this.FindName("WaitTime");
      this.TimeToWait = (DoubleAnimation) this.FindName("TimeToWait");
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ExplanationText = (TextBlock) this.FindName("ExplanationText");
      this.CodeInput = (Grid) this.FindName("CodeInput");
      this.HiddenCode = (TextBox) this.FindName("HiddenCode");
      this.VisibleCode = (TextBlock) this.FindName("VisibleCode");
      this.TimerTextBlock = (TextBlock) this.FindName("TimerTextBlock");
      this.VoiceButton = (Grid) this.FindName("VoiceButton");
      this.VoiceExplain = (TextBlock) this.FindName("VoiceExplain");
      this.CallMeButton = (RoundButton) this.FindName("CallMeButton");
      this.CallButtonCaptionBlock = (TextBlock) this.FindName("CallButtonCaptionBlock");
      this.ContactSupportButton = (Button) this.FindName("ContactSupportButton");
      this.ProgressBarTitle = (TextBlock) this.FindName("ProgressBarTitle");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
    }
  }
}
