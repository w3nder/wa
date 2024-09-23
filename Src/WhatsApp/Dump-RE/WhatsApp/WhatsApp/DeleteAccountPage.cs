// Decompiled with JetBrains decompiler
// Type: WhatsApp.DeleteAccountPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class DeleteAccountPage : PhoneApplicationPage
  {
    private const string LogHeader = "delete account";
    private GlobalProgressIndicator progress;
    private ApplicationBarIconButton appbarbutton;
    private string feedback;
    internal StackPanel IntroPanel;
    internal ListBox EffectsListBox;
    internal PhoneNumberEntryControl PhoneNumberBox;
    internal StackPanel FeedbackPanel;
    internal TextBox FeedbackBox;
    internal Grid ConfirmPanel;
    internal WhiteBlackImage SadPhone;
    private bool _contentLoaded;

    public DeleteAccountPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.appbarbutton = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      this.progress = new GlobalProgressIndicator((DependencyObject) this);
      this.SadPhone.Image = ImageStore.SadPhoneIcon;
      this.EffectsListBox.ItemsSource = (IEnumerable) ((IEnumerable<string>) new string[3]
      {
        AppResources.DeleteAccountEffects0,
        AppResources.DeleteAccountEffects1,
        AppResources.DeleteAccountEffects2
      }).Select<string, string>((Func<string, string>) (s => string.Format("• {0}", (object) s))).ToArray<string>();
      this.PhoneNumberBox.TitleText = AppResources.DeleteAccountDetails1;
    }

    private void Continue_Click(object sender, EventArgs e)
    {
      Action cleanup = (Action) (() => this.appbarbutton.IsEnabled = true);
      this.appbarbutton.IsEnabled = false;
      string phoneNumber = (string) null;
      try
      {
        phoneNumber = this.PhoneNumberBox.PhoneNumber;
      }
      catch (PhoneNumberEntryException ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
        cleanup();
        return;
      }
      FunXMPP.Connection connection = AppState.ClientInstance.GetConnection();
      string cc = this.PhoneNumberBox.GetCii().PhoneCountryCode;
      string cc1 = cc;
      string phone = phoneNumber;
      connection.SendNormalizePhoneNumber(cc1, phone).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (chatID =>
      {
        if (chatID == Settings.ChatID)
        {
          this.CollectFeedback();
        }
        else
        {
          Log.l("delete account", "user entered wrong number ({0} {1})", (object) cc, (object) phoneNumber);
          int num = (int) MessageBox.Show(AppResources.DeleteAccountMismatch);
        }
        cleanup();
      }), (Action<Exception>) (ex =>
      {
        Log.l("delete account", "error: {0}", ex.InnerException == null ? (object) ex.Message : (object) ex.InnerException.Message);
        int num = (int) MessageBox.Show(AppResources.DeleteAccountError);
        cleanup();
      }));
    }

    private void FeedbackNext_Click(object sender, EventArgs e)
    {
      this.appbarbutton.IsEnabled = false;
      this.feedback = (this.FeedbackBox.Text ?? "").Trim();
      if (this.feedback.Length == 0 || this.feedback.Length > 4)
      {
        this.ShowConfirm();
      }
      else
      {
        int num = (int) MessageBox.Show(AppResources.InsufficientDetail);
      }
      this.appbarbutton.IsEnabled = true;
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      this.appbarbutton.IsEnabled = false;
      this.DeleteAccount();
    }

    private void CollectFeedback()
    {
      this.IntroPanel.Visibility = Visibility.Collapsed;
      this.FeedbackPanel.Visibility = Visibility.Visible;
      this.ConfirmPanel.Visibility = Visibility.Collapsed;
      this.FeedbackBox.Focus();
      UIUtils.UpdateAppBarButton(this.appbarbutton, (string) null, removeHandler: new EventHandler(this.Continue_Click), addHandler: new EventHandler(this.FeedbackNext_Click));
    }

    private void ShowConfirm()
    {
      Log.l("delete account", "feedback: {0}", (object) (this.feedback ?? ""));
      this.IntroPanel.Visibility = Visibility.Collapsed;
      this.FeedbackPanel.Visibility = Visibility.Collapsed;
      this.ConfirmPanel.Visibility = Visibility.Visible;
      UIUtils.UpdateAppBarButton(this.appbarbutton, AppResources.Delete, "Images/assets/dark/delete.png", new EventHandler(this.FeedbackNext_Click), new EventHandler(this.Delete_Click));
    }

    private void DeleteAccount()
    {
      this.IsEnabled = false;
      this.progress.Acquire();
      Action exitPage = (Action) (() =>
      {
        this.progress.Release();
        NavUtils.GoBack();
      });
      Action showErrorAndExit = (Action) (() => UIUtils.ShowMessageBox("", AppResources.DeleteAccountError).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => exitPage())));
      Action showSuccessAndExit = (Action) (() => UIUtils.ShowMessageBox("", AppResources.DeleteAccountSuccess).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.progress.Release();
        NavUtils.NavigateHome();
      })));
      UIUtils.Decision(AppResources.DeleteAccountConfirm).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (confirmed)
        {
          string lang;
          string locale;
          AppState.GetLangAndLocale(out lang, out locale);
          App.CurrentApp.Connection.SendDeleteAccount(this.feedback, lang, locale, (Action) (() =>
          {
            try
            {
              App.CurrentApp.Reset();
              App.AccountJustDeleted = true;
              ConversionRecordHelper.ClearConversionRecords();
              FieldStats.ReportDeleteNumber();
              FieldStatsRunner.TrySendStats(FieldStatsRunner.ForceLevel.AlwaysSend);
              this.Dispatcher.BeginInvoke((Action) (() => showSuccessAndExit()));
            }
            catch (Exception ex)
            {
              Log.l("delete account", "failed with {0}", ex.InnerException == null ? (object) ex.Message : (object) ex.InnerException.Message);
              this.Dispatcher.BeginInvoke((Action) (() => showErrorAndExit()));
            }
          }), (Action<int>) (err => this.Dispatcher.BeginInvoke((Action) (() => showErrorAndExit()))));
        }
        else
          exitPage();
      }));
    }

    private void EffectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.EffectsListBox.SelectedItem = (object) null;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.DELETE_ACCOUNT);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/DeleteAccountPage.xaml", UriKind.Relative));
      this.IntroPanel = (StackPanel) this.FindName("IntroPanel");
      this.EffectsListBox = (ListBox) this.FindName("EffectsListBox");
      this.PhoneNumberBox = (PhoneNumberEntryControl) this.FindName("PhoneNumberBox");
      this.FeedbackPanel = (StackPanel) this.FindName("FeedbackPanel");
      this.FeedbackBox = (TextBox) this.FindName("FeedbackBox");
      this.ConfirmPanel = (Grid) this.FindName("ConfirmPanel");
      this.SadPhone = (WhiteBlackImage) this.FindName("SadPhone");
    }
  }
}
