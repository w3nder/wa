// Decompiled with JetBrains decompiler
// Type: WhatsApp.ValidateCode
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


namespace WhatsApp
{
  public class ValidateCode : PhoneApplicationPage
  {
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
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock ExplanationText;
    internal Grid CodeInput;
    internal TextBox HiddenCode;
    internal TextBlock VisibleCode;
    internal Button ForgotButton;
    internal TextBlock EmailText;
    private bool _contentLoaded;

    public ValidateCode()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.Code_TextChanged((object) null, (TextChangedEventArgs) null);
      this.Loaded += new RoutedEventHandler(this.ValidateCode_Loaded);
    }

    private void ValidateCode_Loaded(object sender, RoutedEventArgs e) => this.HiddenCode.Focus();

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
      if (this.HiddenCode.Text.Length > Settings.CodeLength)
      {
        this.HiddenCode.Text = this.HiddenCode.Text.Substring(0, Settings.CodeLength);
      }
      else
      {
        string hiddenString = this.HiddenCode.Text;
        int length = hiddenString.Length;
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
        if (hiddenString != Settings.TwoFactorAuthCodeLocal)
        {
          UIUtils.ShowMessageBox(AppResources.TwoStepVerification, AppResources.TwoStepVerificationFailed).Subscribe<Unit>();
          this.HiddenCode.Text = "";
          TwoFactorAuthentication.DecrementBackoff();
        }
        else
        {
          AppState.InvokeWhenConnected((Action<FunXMPP.Connection>) (conn => TwoFactorAuthentication.ValidateCode(hiddenString, (Action<bool>) (correctCode =>
          {
            if (correctCode)
              return;
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              UIUtils.ShowMessageBox(AppResources.TwoStepVerification, AppResources.TwoStepVerificationFailed).Subscribe<Unit>();
              NavUtils.NavigateToPage(this.NavigationService, "TwoFactorPage", "ClearStack=true", "Pages/TwoFactor");
            }));
          }))));
          Settings.TwoFactorNextPrompt = new DateTime?(FunRunner.CurrentServerTimeUtc + TwoFactorAuthentication.GetNextBackoff());
          NavUtils.NavigateHome();
        }
      }
    }

    private void CodeInput_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.HiddenCode.Focus();
    }

    private void Forgot_Click(object sender, RoutedEventArgs e)
    {
      UIUtils.Decision(AppResources.TwoStepVerificationDisableWarning, AppResources.Disable, AppResources.CancelButton, " ").Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        TwoFactorAuthentication.NaggedForCodeEntry = true;
        TwoFactorAuthentication.RemoveSetupCode((Action<bool>) (promptReset => { }), (Action<int>) (err =>
        {
          this.Dispatcher.BeginInvokeIfNeeded((Action) (() => UIUtils.ShowMessageBox(AppResources.TwoStepVerification, AppResources.TwoStepRemoveError).Subscribe<Unit>()));
          TwoFactorAuthentication.NaggedForCodeEntry = false;
        }));
        NavUtils.NavigateHome();
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/TwoFactor/ValidateCode.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ExplanationText = (TextBlock) this.FindName("ExplanationText");
      this.CodeInput = (Grid) this.FindName("CodeInput");
      this.HiddenCode = (TextBox) this.FindName("HiddenCode");
      this.VisibleCode = (TextBlock) this.FindName("VisibleCode");
      this.ForgotButton = (Button) this.FindName("ForgotButton");
      this.EmailText = (TextBlock) this.FindName("EmailText");
    }
  }
}
