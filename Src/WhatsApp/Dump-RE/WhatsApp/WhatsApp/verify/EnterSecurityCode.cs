// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.EnterSecurityCode
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp.verify
{
  public class EnterSecurityCode : PhoneApplicationPage
  {
    private IDisposable verificationSubscription;
    private IDisposable timerSubscription;
    private GlobalProgressIndicator indicator;
    private DispatcherTimer doubleCheckTimer;
    private string errorReason;
    private bool codeEntryEnabled = true;
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
    private bool enterMode = true;
    internal Storyboard FadeOutAnimation;
    internal Storyboard FadeInAnimation;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Grid EnterGrid;
    internal Grid CodeInput;
    internal TextBox HiddenCode;
    internal TextBlock VisibleCode;
    internal TextBlock TimerTextBlock;
    internal Button ForgetButton;
    internal Button ContactSupportErrorButton;
    internal Grid ForgetGrid;
    internal TextBlock ExplanationText;
    internal Button WipeButton;
    internal TextBlock WipeText;
    internal Button ContactSupportButton;
    internal Button EmailButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public static EnterSecurityCode.AccountWipeType AccountWipe
    {
      get
      {
        switch (Settings.TwoFactorWipeType)
        {
          case "full":
            return EnterSecurityCode.AccountWipeType.Full;
          case "offline":
            DateTime? timeUntilTokenValid = Settings.TwoFactorWipeTimeUntilTokenValid;
            if (timeUntilTokenValid.HasValue)
            {
              timeUntilTokenValid = Settings.TwoFactorWipeTimeUntilTokenValid;
              DateTime now = DateTime.Now;
              if ((timeUntilTokenValid.HasValue ? (timeUntilTokenValid.GetValueOrDefault() > now ? 1 : 0) : 0) != 0)
                return EnterSecurityCode.AccountWipeType.None;
            }
            return EnterSecurityCode.AccountWipeType.Offline;
          default:
            return EnterSecurityCode.AccountWipeType.None;
        }
      }
    }

    public EnterSecurityCode()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.indicator = new GlobalProgressIndicator((DependencyObject) this);
      this.Code_TextChanged((object) null, (TextChangedEventArgs) null);
      this.Loaded += new RoutedEventHandler(this.EnterSecurityCode_Loaded);
    }

    private void EnterSecurityCode_Loaded(object sender, RoutedEventArgs e)
    {
      this.HiddenCode.Focus();
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
      string text = this.HiddenCode.Text;
      int length = text.Length;
      int num = Settings.CodeLength / 2;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < num; ++index)
      {
        if (index < length)
        {
          stringBuilder.Append("●");
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
          stringBuilder.Append("●");
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
      this.CheckSecurityCode(text);
    }

    private void CodeInput_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!this.IsCodeEntryEnabled)
        return;
      this.HiddenCode.Focus();
    }

    private void ClearCode()
    {
      this.HiddenCode.Text = "";
      this.Focus();
    }

    private void DisableEntry()
    {
      this.HiddenCode.IsEnabled = false;
      this.EmailButton.IsEnabled = false;
      this.CancelButton.IsEnabled = false;
      this.WipeButton.IsEnabled = false;
      this.WipeText.Opacity = 0.4;
    }

    private void EnableEntry()
    {
      this.HiddenCode.IsEnabled = true;
      this.EmailButton.IsEnabled = true;
      this.CancelButton.IsEnabled = true;
      this.WipeButton.IsEnabled = true;
      this.WipeText.Opacity = 1.0;
    }

    private bool IsCodeEntryEnabled
    {
      get => this.codeEntryEnabled;
      set
      {
        this.codeEntryEnabled = value;
        if (value)
          this.VisibleCode.Opacity = 1.0;
        else
          this.VisibleCode.Opacity = 0.4;
      }
    }

    private bool EnterMode
    {
      get => this.enterMode;
      set
      {
        this.enterMode = value;
        this.GoToVisualState(value);
      }
    }

    private void GoToVisualState(bool enterMode)
    {
      EventHandler completed = (EventHandler) null;
      completed = (EventHandler) ((s, e) =>
      {
        this.FadeOutAnimation.Completed -= completed;
        this.GoToVisualStateImpl(enterMode);
      });
      this.FadeOutAnimation.Completed += completed;
      this.FadeOutAnimation.Begin();
    }

    private void GoToVisualStateImpl(bool enterMode)
    {
      if (enterMode)
      {
        this.EnterGrid.Visibility = Visibility.Visible;
        this.ForgetGrid.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.EnterGrid.Visibility = Visibility.Collapsed;
        this.ForgetGrid.Visibility = Visibility.Visible;
        switch (EnterSecurityCode.AccountWipe)
        {
          case EnterSecurityCode.AccountWipeType.None:
            this.ExplanationText.Text = string.Format(AppResources.TwoStepVerificationWipeImmediate, (object) Plurals.Instance.GetString(AppResources.DaysPlural, (int) Math.Ceiling((Settings.TwoFactorWipeTimeUntilTokenValid.Value - DateTime.Now).TotalDays)));
            this.WipeButton.Visibility = Visibility.Collapsed;
            break;
          case EnterSecurityCode.AccountWipeType.Offline:
          case EnterSecurityCode.AccountWipeType.Full:
            this.ExplanationText.Text = AppResources.TwoStepVerificationWipeWarning;
            this.WipeButton.Visibility = Visibility.Visible;
            break;
        }
      }
      this.FadeInAnimation.Begin();
    }

    private void CheckSecurityCode(string code)
    {
      if (this.verificationSubscription != null)
        return;
      this.DisableEntry();
      this.indicator.Acquire();
      this.TimerTextBlock.Text = string.Empty;
      this.timerSubscription.SafeDispose();
      this.timerSubscription = (IDisposable) null;
      this.verificationSubscription = Registration.CheckSecurityCode(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken, code).ObserveOnDispatcher<Registration.RegResult>().Finally<Registration.RegResult>((Action) (() => this.verificationSubscription = (IDisposable) null)).Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (resp =>
      {
        this.EnableEntry();
        if (resp.Status == "ok")
        {
          Log.WriteLineDebug("registered ok, status {0}", (object) resp.Status);
          Settings.ChatID = resp.ChatID;
          Settings.PhoneNumberAccountCreationType = Registration.GetAccountCreationType(resp);
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingBackupCheck;
          TwoFactorAuthentication.CodeValidated(code);
          SMSCodeHelper.SaveSMSCode((string) null);
          this.NavigationService.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify"));
        }
        else if (resp.Reason == "incorrect")
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.SameDeviceFailed;
        else if (resp.ErrorString != null)
        {
          this.ClearCode();
          string messageBoxText = resp.ErrorString;
          if (resp.Reason == "mismatch" && SMSCodeHelper.GetSavedSMSCode() == code)
            messageBoxText = AppResources.TwoStepMismatchSMSUsed;
          int num1 = (int) MessageBox.Show(messageBoxText);
          int? nullable1;
          if (resp.WaitSeconds.HasValue)
          {
            nullable1 = resp.WaitSeconds;
            if (nullable1.Value > 0)
            {
              DateTime currentServerTimeUtc1 = FunRunner.CurrentServerTimeUtc;
              ref DateTime local = ref currentServerTimeUtc1;
              nullable1 = resp.WaitSeconds;
              double num2 = (double) nullable1.Value;
              DateTime waitTill = local.AddSeconds(num2);
              this.timerSubscription.SafeDispose();
              this.timerSubscription = Observable.Timer(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(1.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
              {
                DateTime currentServerTimeUtc2 = FunRunner.CurrentServerTimeUtc;
                if (waitTill > currentServerTimeUtc2)
                {
                  this.TimerTextBlock.Text = string.Format(AppResources.ReenterCodeLaterWithTime, (object) (waitTill - currentServerTimeUtc2).ToFriendlyString());
                }
                else
                {
                  this.timerSubscription.SafeDispose();
                  this.timerSubscription = (IDisposable) null;
                  this.TimerTextBlock.Text = string.Empty;
                  this.IsCodeEntryEnabled = true;
                  this.HiddenCode.Focus();
                }
              }));
              this.IsCodeEntryEnabled = false;
              goto label_11;
            }
          }
          this.HiddenCode.Focus();
label_11:
          Settings.TwoFactorWipeToken = resp.WipeToken;
          Settings.TwoFactorWipeType = resp.WipeType;
          nullable1 = resp.WipeWait;
          DateTime? nullable2;
          if (!nullable1.HasValue)
          {
            nullable2 = new DateTime?();
          }
          else
          {
            DateTime now = DateTime.Now;
            nullable1 = resp.WipeWait;
            TimeSpan timeSpan = TimeSpan.FromSeconds((double) nullable1.Value);
            nullable2 = new DateTime?(now + timeSpan);
          }
          Settings.TwoFactorWipeTimeUntilTokenValid = nullable2;
        }
        this.errorReason = resp.Reason;
        if (!string.IsNullOrEmpty(this.errorReason))
          this.ContactSupportErrorButton.Visibility = Visibility.Visible;
        this.indicator.Release();
      }), (Action<Exception>) (ex =>
      {
        Log.LogException(ex, "register");
        int num = (int) MessageBox.Show(string.Format(AppResources.VerificationError, (object) ex.GetType().Name));
        this.EnableEntry();
        this.HiddenCode.Focus();
        this.indicator.Release();
      }));
    }

    private void Email_Click(object sender, RoutedEventArgs e)
    {
      this.indicator.Acquire();
      this.DisableEntry();
      Registration.UseEmailRecovery(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken).ObserveOnDispatcher<Registration.RegResult>().Finally<Registration.RegResult>((Action) (() => this.verificationSubscription = (IDisposable) null)).Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (resp =>
      {
        int? nullable1;
        if (resp.Status == "sent")
        {
          int num = (int) MessageBox.Show(AppResources.TwoStepVerificationEmail);
          Settings.TwoFactorEmailWipePollExpiryTime = new DateTime?(DateTimeUtils.FromUnixTime(resp.WipeExpiryTime.GetValueOrDefault()));
          nullable1 = resp.MinPoll;
          Settings.TwoFactorWipePollInterval = nullable1 ?? 5;
          this.QueueDoubleCheck((double) Settings.TwoFactorWipePollInterval);
        }
        else if (resp.Reason == "incorrect")
          Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.SameDeviceFailed;
        else if (resp.ErrorString != null)
        {
          if (resp.Reason == "reset_too_soon")
          {
            int num1 = (int) MessageBox.Show(AppResources.TwoStepVerificationEmailTooSoon);
          }
        }
        else
        {
          int num2 = (int) MessageBox.Show(AppResources.TwoStepEmailError);
        }
        this.errorReason = resp.Reason;
        if (!string.IsNullOrEmpty(this.errorReason))
          this.ContactSupportErrorButton.Visibility = Visibility.Visible;
        this.indicator.Release();
        this.EnableEntry();
        this.EnterMode = true;
        Settings.TwoFactorWipeToken = resp.WipeToken;
        Settings.TwoFactorWipeType = resp.WipeType;
        nullable1 = resp.WipeWait;
        DateTime? nullable2;
        if (!nullable1.HasValue)
        {
          nullable2 = new DateTime?();
        }
        else
        {
          DateTime now = DateTime.Now;
          nullable1 = resp.WipeWait;
          TimeSpan timeSpan = TimeSpan.FromSeconds((double) nullable1.Value);
          nullable2 = new DateTime?(now + timeSpan);
        }
        Settings.TwoFactorWipeTimeUntilTokenValid = nullable2;
      }), (Action<Exception>) (ex =>
      {
        Log.LogException(ex, "register");
        int num = (int) MessageBox.Show(string.Format(AppResources.VerificationError, (object) ex.GetType().Name));
        this.indicator.Release();
        this.EnableEntry();
        this.EnterMode = true;
      }));
    }

    private void Forgot_Click(object sender, RoutedEventArgs e) => this.EnterMode = false;

    private void ContactSupport_Click(object sender, RoutedEventArgs e)
    {
      string val = string.Format("{0} {1}", (object) ("2sv:" + this.errorReason), (object) (" " + (Settings.ChatID ?? Settings.CountryCode + Settings.PhoneNumber)));
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("context", val);
      uriParams.AddBool("ClearStack", true);
      NavUtils.NavigateToPage("ContactSupportPage", uriParams);
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
      UIUtils.Decision(EnterSecurityCode.AccountWipe == EnterSecurityCode.AccountWipeType.Offline ? AppResources.TwoStepVerificationWipe7 : AppResources.TwoStepVerificationWipe30, AppResources.ResetAccountButton, AppResources.CancelButton, " ").Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        this.DisableEntry();
        this.indicator.Acquire();
        Registration.WipeAccount(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken, Settings.TwoFactorWipeToken).ObserveOnDispatcher<Registration.RegResult>().Finally<Registration.RegResult>((Action) (() => this.verificationSubscription = (IDisposable) null)).Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (resp =>
        {
          if (resp.Status == "ok")
          {
            Log.WriteLineDebug("Account successfully wiped using 2FacWipeAccount");
            this.AccountUnlocked(resp);
          }
          else if (resp.Reason == "incorrect")
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.SameDeviceFailed;
          else if (resp.Reason == "stale")
          {
            Settings.TwoFactorWipeType = resp.WipeType;
            Settings.TwoFactorWipeToken = resp.WipeToken;
          }
          else
          {
            int num = (int) MessageBox.Show(AppResources.TwoStepVerificationWipeStale);
          }
          this.errorReason = resp.Reason;
          if (!string.IsNullOrEmpty(this.errorReason))
            this.ContactSupportErrorButton.Visibility = Visibility.Visible;
          this.indicator.Release();
          this.EnableEntry();
        }), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "2fa reset wipe");
          int num = (int) MessageBox.Show(string.Format(AppResources.TwoStepVerificationWipeStale, (object) ex.GetType().Name));
          this.indicator.Release();
          this.EnableEntry();
        }));
      }));
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => this.EnterMode = true;

    private void AccountUnlocked(Registration.RegResult resp)
    {
      Settings.ChatID = resp.ChatID;
      Settings.PhoneNumberAccountCreationType = Registration.GetAccountCreationType(resp);
      Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingBackupCheck;
      TwoFactorAuthentication.CodeRemoved();
      SMSCodeHelper.SaveSMSCode((string) null);
      this.NavigationService.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify"));
    }

    private void DoubleCheck()
    {
      if (this.doubleCheckTimer != null)
        this.doubleCheckTimer.Stop();
      Registration.Exists(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken).ObserveOnDispatcher<Registration.RegResult>().Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (status =>
      {
        if (status.Status == "ok")
        {
          this.AccountUnlocked(status);
        }
        else
        {
          if (!(status.Status == "fail"))
            return;
          if (status.Reason == "security_code")
          {
            Settings.TwoFactorWipeType = status.WipeType;
            Settings.TwoFactorWipeToken = status.WipeToken;
            int? nullable1 = status.MinPoll;
            Settings.TwoFactorWipePollInterval = nullable1 ?? 5;
            nullable1 = status.WipeWait;
            DateTime? nullable2;
            if (!nullable1.HasValue)
            {
              nullable2 = new DateTime?();
            }
            else
            {
              DateTime now = DateTime.Now;
              nullable1 = status.WipeWait;
              TimeSpan timeSpan = TimeSpan.FromSeconds((double) nullable1.Value);
              nullable2 = new DateTime?(now + timeSpan);
            }
            Settings.TwoFactorWipeTimeUntilTokenValid = nullable2;
            long? serverTime = status.ServerTime;
            DateTime dateTime1;
            if (!serverTime.HasValue)
            {
              dateTime1 = DateTime.UtcNow;
            }
            else
            {
              serverTime = status.ServerTime;
              dateTime1 = DateTimeUtils.FromUnixTime(serverTime.Value);
            }
            DateTime? wipePollExpiryTime = Settings.TwoFactorEmailWipePollExpiryTime;
            DateTime dateTime2 = dateTime1;
            if ((wipePollExpiryTime.HasValue ? (wipePollExpiryTime.GetValueOrDefault() > dateTime2 ? 1 : 0) : 0) == 0)
              return;
            this.QueueDoubleCheck((double) Settings.TwoFactorWipePollInterval);
          }
          else
          {
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.SameDeviceFailed;
            this.NavigationService.Navigate(UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify"));
          }
        }
      }));
    }

    private void QueueDoubleCheck(double timeInSeconds)
    {
      if (this.doubleCheckTimer == null)
      {
        this.doubleCheckTimer = new DispatcherTimer();
        this.doubleCheckTimer.Tick += (EventHandler) ((s, e) => this.DoubleCheck());
      }
      this.doubleCheckTimer.Interval = TimeSpan.FromSeconds(timeInSeconds);
      this.doubleCheckTimer.Start();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.DoubleCheck();
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (this.doubleCheckTimer != null)
      {
        this.doubleCheckTimer.Stop();
        this.doubleCheckTimer = (DispatcherTimer) null;
      }
      base.OnNavigatedFrom(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/verify/EnterSecurityCode.xaml", UriKind.Relative));
      this.FadeOutAnimation = (Storyboard) this.FindName("FadeOutAnimation");
      this.FadeInAnimation = (Storyboard) this.FindName("FadeInAnimation");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.EnterGrid = (Grid) this.FindName("EnterGrid");
      this.CodeInput = (Grid) this.FindName("CodeInput");
      this.HiddenCode = (TextBox) this.FindName("HiddenCode");
      this.VisibleCode = (TextBlock) this.FindName("VisibleCode");
      this.TimerTextBlock = (TextBlock) this.FindName("TimerTextBlock");
      this.ForgetButton = (Button) this.FindName("ForgetButton");
      this.ContactSupportErrorButton = (Button) this.FindName("ContactSupportErrorButton");
      this.ForgetGrid = (Grid) this.FindName("ForgetGrid");
      this.ExplanationText = (TextBlock) this.FindName("ExplanationText");
      this.WipeButton = (Button) this.FindName("WipeButton");
      this.WipeText = (TextBlock) this.FindName("WipeText");
      this.ContactSupportButton = (Button) this.FindName("ContactSupportButton");
      this.EmailButton = (Button) this.FindName("EmailButton");
      this.CancelButton = (Button) this.FindName("CancelButton");
    }

    public enum AccountWipeType
    {
      None,
      Offline,
      Full,
    }
  }
}
