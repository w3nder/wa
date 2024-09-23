// Decompiled with JetBrains decompiler
// Type: WhatsApp.SetupTwoFactor
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
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class SetupTwoFactor : PhoneApplicationPage
  {
    private ApplicationBarIconButton ForwardButton;
    private GlobalProgressIndicator indicator;
    private string code;
    private string email;
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
    private SetupTwoFactor.CodeEntryState state;
    internal Storyboard FadeOutAnimation;
    internal Storyboard FadeInAnimation;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock ExplanationText;
    internal Grid CodeInput;
    internal TextBox HiddenCode;
    internal TextBlock VisibleCode;
    internal StackPanel EmailInput;
    internal TextBlock EmailHeader;
    internal TextBox EmailPart1;
    internal Button SkipButton;
    internal TextBlock ErrorText;
    private bool _contentLoaded;

    public SetupTwoFactor()
    {
      this.InitializeComponent();
      this.ForwardButton = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      if (Settings.OldChatID != null)
      {
        this.ForwardButton.Text = "Cancel";
        this.ForwardButton.IconUri = new Uri("/Images/assets/notheme/x.png", UriKind.Relative);
      }
      this.indicator = new GlobalProgressIndicator((DependencyObject) this);
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.CodeState = SetupTwoFactor.CodeEntryState.EnterCode;
      this.Code_TextChanged((object) null, (TextChangedEventArgs) null);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      string str = (string) null;
      if (this.NavigationContext.QueryString.TryGetValue("start", out str))
      {
        switch (str)
        {
          case "email":
            this.CodeState = SetupTwoFactor.CodeEntryState.ChangeEmail;
            this.code = Settings.TwoFactorAuthCodeLocal;
            break;
          case "change":
            this.CodeState = SetupTwoFactor.CodeEntryState.ChangeCode;
            this.email = Settings.TwoFactorAuthEmail;
            break;
        }
      }
      else
        this.CodeState = SetupTwoFactor.CodeEntryState.EnterCode;
      base.OnNavigatedTo(e);
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
      if (length == Settings.CodeLength)
      {
        if (this.CodeState == SetupTwoFactor.CodeEntryState.VerifyCode && text != this.code)
        {
          this.ErrorText.Visibility = Visibility.Visible;
          this.ErrorText.Text = AppResources.PasscodeMismatch;
          this.ForwardButton.IsEnabled = false;
        }
        else
        {
          this.ErrorText.Visibility = Visibility.Collapsed;
          this.ForwardButton.IsEnabled = true;
          this.Next_Click(sender, (EventArgs) e);
        }
      }
      else
      {
        this.ErrorText.Visibility = Visibility.Collapsed;
        this.ForwardButton.IsEnabled = false;
      }
    }

    private void CodeInput_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.HiddenCode.Focus();
    }

    private void Email_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.ErrorText.Visibility = Visibility.Collapsed;
      if (string.IsNullOrEmpty(this.EmailPart1.Text))
        return;
      this.ForwardButton.IsEnabled = true;
    }

    private SetupTwoFactor.CodeEntryState CodeState
    {
      get => this.state;
      set
      {
        this.state = value;
        this.GoToVisualState(value);
      }
    }

    private void GoToVisualState(SetupTwoFactor.CodeEntryState newState, bool useTransitions = true)
    {
      EventHandler completed = (EventHandler) null;
      if (useTransitions)
      {
        completed = (EventHandler) ((s, e) =>
        {
          this.FadeOutAnimation.Completed -= completed;
          this.GoToVisualStateImpl(newState);
        });
        this.FadeOutAnimation.Completed += completed;
        this.FadeOutAnimation.Begin();
      }
      else
        this.GoToVisualStateImpl(newState, useTransitions);
    }

    private void GoToVisualStateImpl(SetupTwoFactor.CodeEntryState newState, bool useTransitions = true)
    {
      this.HiddenCode.Text = string.Empty;
      this.CodeInput.Visibility = Visibility.Collapsed;
      this.EmailPart1.Text = string.Empty;
      this.EmailInput.Visibility = Visibility.Collapsed;
      this.ForwardButton.IsEnabled = false;
      switch (newState)
      {
        case SetupTwoFactor.CodeEntryState.EnterCode:
        case SetupTwoFactor.CodeEntryState.VerifyCode:
        case SetupTwoFactor.CodeEntryState.ChangeCode:
        case SetupTwoFactor.CodeEntryState.VerifyChangeCode:
          this.ExplanationText.Text = newState == SetupTwoFactor.CodeEntryState.EnterCode || newState == SetupTwoFactor.CodeEntryState.ChangeCode ? AppResources.EnterPasscode : AppResources.ConfirmPasscode;
          this.CodeInput.Visibility = Visibility.Visible;
          this.HiddenCode.Focus();
          break;
        case SetupTwoFactor.CodeEntryState.EnterEmail:
        case SetupTwoFactor.CodeEntryState.ChangeEmail:
          this.SkipButton.Visibility = newState == SetupTwoFactor.CodeEntryState.ChangeEmail ? Visibility.Collapsed : Visibility.Visible;
          goto case SetupTwoFactor.CodeEntryState.VerifyEnterEmail;
        case SetupTwoFactor.CodeEntryState.VerifyEnterEmail:
          this.ExplanationText.Text = AppResources.AddEmailDescription;
          this.EmailHeader.Text = newState == SetupTwoFactor.CodeEntryState.VerifyEnterEmail ? AppResources.ConfirmEmail : AppResources.EnterEmail;
          this.EmailInput.Visibility = Visibility.Visible;
          this.EmailPart1.Focus();
          break;
      }
      if (!useTransitions)
        return;
      this.FadeInAnimation.Begin();
    }

    private void Next_Click(object sender, EventArgs e)
    {
      switch (this.CodeState)
      {
        case SetupTwoFactor.CodeEntryState.EnterCode:
        case SetupTwoFactor.CodeEntryState.ChangeCode:
          this.code = this.HiddenCode.Text;
          this.CodeState = this.CodeState == SetupTwoFactor.CodeEntryState.EnterCode ? SetupTwoFactor.CodeEntryState.VerifyCode : SetupTwoFactor.CodeEntryState.VerifyChangeCode;
          break;
        case SetupTwoFactor.CodeEntryState.VerifyCode:
        case SetupTwoFactor.CodeEntryState.VerifyChangeCode:
          if (this.HiddenCode.Text == this.code)
          {
            if (this.CodeState == SetupTwoFactor.CodeEntryState.VerifyCode)
            {
              this.CodeState = SetupTwoFactor.CodeEntryState.EnterEmail;
              break;
            }
            this.email = Settings.TwoFactorAuthEmail;
            this.SendCode();
            break;
          }
          this.ErrorText.Visibility = Visibility.Visible;
          this.ErrorText.Text = AppResources.PasscodeMismatch;
          this.ForwardButton.IsEnabled = false;
          this.Focus();
          break;
        case SetupTwoFactor.CodeEntryState.EnterEmail:
        case SetupTwoFactor.CodeEntryState.ChangeEmail:
          this.email = this.EmailPart1.Text;
          this.CodeState = SetupTwoFactor.CodeEntryState.VerifyEnterEmail;
          break;
        case SetupTwoFactor.CodeEntryState.VerifyEnterEmail:
          if (string.IsNullOrEmpty(this.EmailPart1.Text) || this.EmailPart1.Text != this.email)
          {
            this.ErrorText.Visibility = Visibility.Visible;
            this.ErrorText.Text = AppResources.EmailMismatch;
            this.Focus();
            break;
          }
          this.SendCode();
          break;
      }
    }

    private void SkipButton_Click(object sender, RoutedEventArgs e)
    {
      UIUtils.Decision(AppResources.OmitEmailWarning, AppResources.SkipThisStepLower, AppResources.CancelButton, " ").Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        this.EmailPart1.Text = string.Empty;
        this.email = (string) null;
        this.SendCode();
      }));
    }

    private void SendCode()
    {
      this.indicator.Acquire();
      TwoFactorAuthentication.SendSetupCode(this.code, this.email, (Action<bool>) (promptReset => this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.indicator.Release();
        if (promptReset)
          NavUtils.NavigateHome();
        else
          NavUtils.GoBack();
      }))), (Action<int>) (err => this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.indicator.Release();
        UIUtils.ShowMessageBox(AppResources.TwoStepVerification, AppResources.TwoStepSendError).Subscribe<Unit>();
      }))));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/TwoFactor/SetupTwoFactor.xaml", UriKind.Relative));
      this.FadeOutAnimation = (Storyboard) this.FindName("FadeOutAnimation");
      this.FadeInAnimation = (Storyboard) this.FindName("FadeInAnimation");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ExplanationText = (TextBlock) this.FindName("ExplanationText");
      this.CodeInput = (Grid) this.FindName("CodeInput");
      this.HiddenCode = (TextBox) this.FindName("HiddenCode");
      this.VisibleCode = (TextBlock) this.FindName("VisibleCode");
      this.EmailInput = (StackPanel) this.FindName("EmailInput");
      this.EmailHeader = (TextBlock) this.FindName("EmailHeader");
      this.EmailPart1 = (TextBox) this.FindName("EmailPart1");
      this.SkipButton = (Button) this.FindName("SkipButton");
      this.ErrorText = (TextBlock) this.FindName("ErrorText");
    }

    public enum CodeEntryState
    {
      EnterCode,
      VerifyCode,
      ChangeCode,
      VerifyChangeCode,
      EnterEmail,
      VerifyEnterEmail,
      ChangeEmail,
    }
  }
}
