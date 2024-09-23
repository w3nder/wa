// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.SendCode
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp.verify
{
  public class SendCode : PhoneApplicationPage
  {
    private const string LogHeader = "reg";
    private string codeType;
    private PhoneNumberVerificationState codeSentState;
    private IDisposable codeReqSub;
    private bool goBackOnNavigatedTo;
    private string simNumber;
    private string suggested;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal TextBlock ExplanationBlock1;
    internal TextBlock PhoneNumberBlock;
    internal TextBlock SuggestionBlock;
    internal TextBlock ExplanationBlock2;
    internal ProgressBar ProgressBar;
    internal Grid Buttons;
    internal Button EditButton;
    internal Button ContinueButton;
    private bool _contentLoaded;

    public SendCode()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.VerificationTitle;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.EditButton.Content = (object) AppResources.EditPhone;
      this.ContinueButton.Content = (object) AppResources.Continue;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.goBackOnNavigatedTo)
      {
        this.ReturnToPageStart();
      }
      else
      {
        this.PhoneNumberBlock.Text = PhoneNumberFormatter.FormatInternationalNumber(Settings.CountryCode + Settings.PhoneNumber);
        this.codeType = this.NavigationContext.QueryString["type"];
        Log.l("reg", "code type {0}", (object) this.codeType);
        if (this.codeType == "sms")
        {
          this.ExplanationBlock1.Text = AppResources.VerificationSendingText;
          this.ExplanationBlock2.Text = AppResources.VerificationSendingExplanation;
          this.codeSentState = PhoneNumberVerificationState.ServerSentSms;
        }
        else if (this.codeType == "voice")
        {
          this.PageTitle.SmallTitle = AppResources.VoiceCallTitle;
          this.ExplanationBlock1.Text = AppResources.VerificationSendingVoiceCall;
          this.ExplanationBlock2.Text = AppResources.VerificationCallingExplanation;
          this.codeSentState = PhoneNumberVerificationState.ServerSentVoice;
        }
        else
        {
          this.ExplanationBlock1.Text = AppResources.GenericError;
          this.ExplanationBlock2.Visibility = Visibility.Collapsed;
          this.PhoneNumberBlock.Visibility = Visibility.Collapsed;
        }
        if (Settings.OldChatID != null)
          return;
        this.simNumber = PhoneNumberEntry.GetSIMPhoneNumber();
        int index = -1;
        this.suggested = PhoneNumberEntry.GetSuggestion(Settings.PhoneNumber, Settings.CountryCode, this.simNumber, out index);
        if (string.IsNullOrEmpty(this.suggested))
          return;
        Log.l("reg", "phone number mistype detected: {0} to {1}", (object) PhoneNumberFormatter.FormatInternationalNumber(Settings.CountryCode + Settings.PhoneNumber), (object) this.suggested);
        PhoneNumberEntry.SuggestReplacement(this.SuggestionBlock, this.suggested, index);
      }
    }

    private void SuggestionBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (string.IsNullOrEmpty(this.suggested))
        return;
      Settings.PhoneNumber = this.suggested.ExtractDigits().Substring(Settings.CountryCode.Length);
      this.NavigationService.Navigate(UriUtils.CreatePageUri("PhoneNumberEntry", "ClearStack=true"));
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (Settings.OldChatID != null)
      {
        e.Cancel = true;
        AccountManagement.AbortChangePhoneNumber();
        this.NavigationService.Navigate(UriUtils.CreatePageUri("ChangeNumberEntryPage", "ClearStack=true"));
      }
      else
      {
        e.Cancel = true;
        this.NavigationService.Navigate(UriUtils.CreatePageUri("PhoneNumberEntry", "ClearStack=true"));
      }
      base.OnBackKeyPress(e);
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
      if (Settings.OldChatID == null)
      {
        Log.l("reg", "send code | user hit edit number button");
        NavUtils.NavigateToPage(this.NavigationService, "PhoneNumberEntry", "ClearStack=true");
      }
      else
      {
        Log.l("reg", "send code | user hit cancel change number button");
        AccountManagement.AbortChangePhoneNumber();
        NavUtils.NavigateHome(this.NavigationService);
      }
    }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
      Log.l("reg", "requesting {0} code for {1} {2}", (object) this.codeType, (object) Settings.CountryCode, (object) Settings.PhoneNumber);
      if (this.codeReqSub != null)
      {
        Log.l("reg", "code request skipped | already in progress");
      }
      else
      {
        Action<bool> setEnabled = (Action<bool>) (enabled => this.ContinueButton.IsEnabled = this.EditButton.IsEnabled = enabled);
        setEnabled(false);
        this.codeReqSub = Registration.RequestCode(Settings.CountryCode, Settings.PhoneNumber, Settings.RecoveryToken, this.codeType).Timeout<Registration.RegResult>(TimeSpan.FromSeconds(30.0)).ObserveOnDispatcher<Registration.RegResult>().Finally<Registration.RegResult>((Action) (() =>
        {
          this.codeReqSub.SafeDispose();
          this.codeReqSub = (IDisposable) null;
        })).Subscribe<Registration.RegResult>((Action<Registration.RegResult>) (res =>
        {
          if (res.RegistrationOk)
          {
            Log.l("reg", "reg ok");
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.VerifiedPendingBackupCheck;
          }
          else if (res.CodeSent)
          {
            Log.l("reg", "code sent");
            Settings.PhoneNumberVerificationState = this.codeSentState;
          }
          int? waitMinutes = res.WaitMinutes;
          if (waitMinutes.HasValue)
          {
            DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
            waitMinutes = res.WaitMinutes;
            Settings.PhoneNumberVerificationRetryUtc = currentServerTimeUtc + TimeSpan.FromMinutes((double) waitMinutes.Value);
            Settings.PhoneNumberVerificationTimeoutUtc = currentServerTimeUtc + TimeSpan.FromMinutes(5.0);
            Log.l("reg", "retry at {0}", (object) Settings.PhoneNumberVerificationRetryUtc);
          }
          if (res.ErrorString != null)
          {
            Log.l("reg", "got error: {0}", (object) res.ErrorString);
            if (res.HasSupportAction())
            {
              UIUtils.MessageBox((string) null, res.ErrorString, (IEnumerable<string>) new string[2]
              {
                AppResources.Cancel,
                res.ActionTitle
              }, (Action<int>) (buttonIdx =>
              {
                if (buttonIdx == 1)
                {
                  res.PerformSupportAction();
                  this.goBackOnNavigatedTo = true;
                }
                else
                  this.ReturnToPageStart();
              }));
              return;
            }
            if (res.Reason == "old_version")
            {
              Log.l("reg", "prompted to upgrade");
              NavUtils.NavigateToPage(this.NavigationService, "UpdateVersion", "ClearStack=true");
              return;
            }
            if (res.Reason == "too_recent")
            {
              Log.l("reg", "code has been requested recently");
              int num = (int) MessageBox.Show(res.ErrorString);
              if (this.codeSentState == PhoneNumberVerificationState.ServerSentSms)
                Settings.PhoneNumberVerificationState = this.codeSentState;
            }
            else
            {
              int num1 = (int) MessageBox.Show(res.ErrorString);
            }
          }
          this.ReturnToPageStart();
        }), (Action<Exception>) (ex =>
        {
          Log.l(ex, "reg | request code");
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            int num = (int) MessageBox.Show(AppResources.CouldNotGetCode);
            setEnabled(true);
          }));
        }));
      }
    }

    private void ReturnToPageStart()
    {
      NavUtils.NavigateToPage(this.NavigationService, "VerifyStart", "ClearStack=true", "verify");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/verify/SendCode.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.ExplanationBlock1 = (TextBlock) this.FindName("ExplanationBlock1");
      this.PhoneNumberBlock = (TextBlock) this.FindName("PhoneNumberBlock");
      this.SuggestionBlock = (TextBlock) this.FindName("SuggestionBlock");
      this.ExplanationBlock2 = (TextBlock) this.FindName("ExplanationBlock2");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
      this.Buttons = (Grid) this.FindName("Buttons");
      this.EditButton = (Button) this.FindName("EditButton");
      this.ContinueButton = (Button) this.FindName("ContinueButton");
    }
  }
}
