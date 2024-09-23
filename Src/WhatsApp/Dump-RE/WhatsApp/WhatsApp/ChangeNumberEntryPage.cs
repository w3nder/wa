// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeNumberEntryPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class ChangeNumberEntryPage : PhoneApplicationPage
  {
    private CountryInfo codemap = CountryInfo.Instance;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal ProgressBar ProgressBar;
    internal PhoneNumberEntryControl OldNumber;
    internal PhoneNumberEntryControl NewNumber;
    private bool _contentLoaded;

    public ChangeNumberEntryPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.OldNumber.TitleText = AppResources.EnterOldNumber;
      this.NewNumber.TitleText = AppResources.EnterNewNumber;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    private void OnLoaded(object sender, EventArgs e) => this.OldNumber.PhoneNumberBox.Focus();

    private void Submit_Click(object sender, EventArgs e)
    {
      CountryInfoItem cii = this.OldNumber.GetCii();
      CountryInfoItem newCii = this.NewNumber.GetCii();
      if (cii == null || newCii == null)
      {
        int num1 = (int) MessageBox.Show(string.Format(AppResources.InvalidCountryCodeWithCode, cii == null ? (object) this.OldNumber.CountryCode : (object) this.NewNumber.CountryCode));
      }
      else
      {
        string newNumber = (string) null;
        string phoneNumber;
        try
        {
          phoneNumber = this.OldNumber.PhoneNumber;
          newNumber = this.NewNumber.PhoneNumber;
        }
        catch (PhoneNumberEntryException ex)
        {
          int num2 = (int) MessageBox.Show(ex.Message);
          return;
        }
        Log.l("change number", "inputs | old:{0} new:{1}", (object) phoneNumber, (object) newNumber);
        FunXMPP.Connection connection = AppState.ClientInstance.GetConnection();
        string oldChatId = Settings.ChatID;
        this.ProgressBar.Visibility = Visibility.Visible;
        string countryCode = this.OldNumber.CountryCode;
        string phone = phoneNumber;
        connection.SendNormalizePhoneNumber(countryCode, phone).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (chatId =>
        {
          this.ProgressBar.Visibility = Visibility.Collapsed;
          Log.l("change number", "normalized {0}", (object) chatId);
          if (chatId == oldChatId)
          {
            ChangeNumberNotifyPage.Start(chatId, newNumber, newCii.PhoneCountryCode, newCii.PhoneCountryCode != Settings.CountryCode);
          }
          else
          {
            Log.l("change number", "entry mismatch | normalized:{0}, actual:{1}", (object) chatId, (object) oldChatId);
            int num3 = (int) MessageBox.Show(AppResources.PhoneNumberMismatch);
          }
        }), (Action<Exception>) (ex =>
        {
          this.ProgressBar.Visibility = Visibility.Collapsed;
          Log.SendCrashLog(ex, "Change number: normalize");
          int num4 = (int) MessageBox.Show(string.Format(AppResources.ClockSkew, (object) DateTime.Now));
        }));
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHANGE_NUMBER);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChangeNumberEntryPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
      this.OldNumber = (PhoneNumberEntryControl) this.FindName("OldNumber");
      this.NewNumber = (PhoneNumberEntryControl) this.FindName("NewNumber");
    }
  }
}
