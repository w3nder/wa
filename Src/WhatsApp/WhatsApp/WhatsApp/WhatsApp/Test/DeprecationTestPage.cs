// Decompiled with JetBrains decompiler
// Type: WhatsApp.Test.DeprecationTestPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp.Test
{
  public class DeprecationTestPage : PhoneApplicationPage
  {
    internal Grid LayoutRoot;
    internal StackPanel UnsupportedTextPanel;
    internal TextBlock FinalFlag;
    internal TextBox MessagingYear;
    internal TextBox MessagingMonth;
    internal TextBox MessagingDay;
    internal StackPanel SetValuePanel;
    internal TextBox OfficialYear;
    internal TextBox OfficialMonth;
    internal TextBox OfficialDay;
    internal TextBox ActualYear;
    internal TextBox ActualMonth;
    internal TextBox ActualDay;
    internal TextBlock NagTime;
    internal TextBlock NagFreq;
    private bool _contentLoaded;

    public DeprecationTestPage()
    {
      this.InitializeComponent();
      RichTextBlock richTextBlock1 = new RichTextBlock();
      richTextBlock1.Foreground = UIUtils.SubtleBrushWhite;
      richTextBlock1.TextWrapping = TextWrapping.Wrap;
      richTextBlock1.FontSize = 22.0;
      richTextBlock1.FontStyle = FontStyles.Italic;
      richTextBlock1.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
      richTextBlock1.AllowLinks = true;
      richTextBlock1.EnableScan = false;
      RichTextBlock richTextBlock2 = richTextBlock1;
      richTextBlock2.Text = UnsupportedMessageViewModel.GetUnsupportedMessageTextSet();
      this.UnsupportedTextPanel.Children.Add((UIElement) richTextBlock2);
      this.UpdateServerPropRelatedUi();
      this.UpdateUIWithDates();
      this.UpdateNagRelatedUI();
    }

    private void UpdateUIWithDates()
    {
      DateTime dateTime1 = Settings.DeprecationDateMessaging.Value;
      DateTime dateTime2 = Settings.DeprecationDateOfficial.Value;
      DateTime dateTime3 = Settings.DeprecationDateActual.Value;
      this.MessagingYear.Text = dateTime1.Year.ToString();
      this.MessagingMonth.Text = dateTime1.Month.ToString();
      this.MessagingDay.Text = dateTime1.Day.ToString();
      this.OfficialYear.Text = dateTime2.Year.ToString();
      this.OfficialMonth.Text = dateTime2.Month.ToString();
      this.OfficialDay.Text = dateTime2.Day.ToString();
      this.ActualYear.Text = dateTime3.Year.ToString();
      this.ActualMonth.Text = dateTime3.Month.ToString();
      this.ActualDay.Text = dateTime3.Day.ToString();
    }

    private void UpdateNagRelatedUI()
    {
      TextBlock nagTime = this.NagTime;
      DateTime? deprecationNagTime = Settings.LastDeprecationNagTime;
      ref DateTime? local = ref deprecationNagTime;
      string str = (local.HasValue ? local.GetValueOrDefault().ToShortDateString() : (string) null) ?? "none";
      nagTime.Text = str;
      this.NagFreq.Text = AppState.DeprecationNagFrequency().ToString();
    }

    private void UpdateServerPropRelatedUi()
    {
      this.FinalFlag.Text = AppState.IsFinalRelease() ? "On" : "Off";
    }

    private void ResetLastNag_Click(object sender, RoutedEventArgs e)
    {
      Settings.LastDeprecationNagTime = new DateTime?();
      this.UpdateNagRelatedUI();
    }

    private void PullServerPropButton_Click(object sender, RoutedEventArgs e)
    {
      Settings.ForceServerPropsReload = true;
      AppState.GetConnection().SendGetServerProperties();
      UIUtils.ShowMessageBox("", "please re-enter page").ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => NavUtils.GoBack(this.NavigationService)));
    }

    private void Reset_Messaging_Click(object sender, RoutedEventArgs e)
    {
      Settings.DeprecationDateMessaging = new DateTime?();
      this.UpdateUIWithDates();
    }

    private void Update_Messaging_Click(object sender, RoutedEventArgs e)
    {
      this.UpdateMessaging(new DateTime(int.Parse(this.MessagingYear.Text), int.Parse(this.MessagingMonth.Text), int.Parse(this.MessagingDay.Text), 23, 59, 59));
    }

    private void UpdateMessaging(DateTime newValue)
    {
      Settings.DeprecationDateMessaging = new DateTime?(newValue);
      this.UpdateUIWithDates();
    }

    private void Reset_Actual_Click(object sender, RoutedEventArgs e)
    {
      Settings.DeprecationDateActual = new DateTime?();
      this.UpdateUIWithDates();
    }

    private void Update_Actual_Click(object sender, RoutedEventArgs e)
    {
      DateTime newDate = new DateTime(int.Parse(this.ActualYear.Text), int.Parse(this.ActualMonth.Text), int.Parse(this.ActualDay.Text), 23, 59, 59);
      if (newDate < DateTime.Now.AddMinutes(5.0))
        UIUtils.Decision("Actual date has past or will pass very soon", "OK", "Cancel", "Are you sure?").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (flag =>
        {
          if (!flag)
            return;
          this.UpdateActual(newDate);
        }));
      else
        this.UpdateActual(newDate);
    }

    private void UpdateActual(DateTime newValue)
    {
      Settings.DeprecationDateActual = new DateTime?(newValue);
      this.UpdateUIWithDates();
    }

    private void Reset_Official_Click(object sender, RoutedEventArgs e)
    {
      Settings.DeprecationDateOfficial = new DateTime?();
      this.UpdateUIWithDates();
    }

    private void Update_Official_Click(object sender, RoutedEventArgs e)
    {
      DateTime newDate = new DateTime(int.Parse(this.OfficialYear.Text), int.Parse(this.OfficialMonth.Text), int.Parse(this.OfficialDay.Text), 23, 59, 59);
      if (newDate < DateTime.Now.AddMinutes(5.0))
        UIUtils.Decision("Official date has past or will pass very soon", "OK", "Cancel", "Are you sure?").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (flag =>
        {
          if (!flag)
            return;
          this.UpdateOfficial(newDate);
        }));
      else
        this.UpdateOfficial(newDate);
    }

    private void UpdateOfficial(DateTime newValue)
    {
      Settings.DeprecationDateOfficial = new DateTime?(newValue);
      this.UpdateUIWithDates();
    }

    private void LaunchNag_Click(object sender, RoutedEventArgs e) => ContactsPage.NagDeprecation();

    private void LaunchAbout_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "About", folderName: "Pages/Settings");
    }

    private void LaunchUpdate_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "UpdateVersion");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Test/DeprecationTestPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.UnsupportedTextPanel = (StackPanel) this.FindName("UnsupportedTextPanel");
      this.FinalFlag = (TextBlock) this.FindName("FinalFlag");
      this.MessagingYear = (TextBox) this.FindName("MessagingYear");
      this.MessagingMonth = (TextBox) this.FindName("MessagingMonth");
      this.MessagingDay = (TextBox) this.FindName("MessagingDay");
      this.SetValuePanel = (StackPanel) this.FindName("SetValuePanel");
      this.OfficialYear = (TextBox) this.FindName("OfficialYear");
      this.OfficialMonth = (TextBox) this.FindName("OfficialMonth");
      this.OfficialDay = (TextBox) this.FindName("OfficialDay");
      this.ActualYear = (TextBox) this.FindName("ActualYear");
      this.ActualMonth = (TextBox) this.FindName("ActualMonth");
      this.ActualDay = (TextBox) this.FindName("ActualDay");
      this.NagTime = (TextBlock) this.FindName("NagTime");
      this.NagFreq = (TextBlock) this.FindName("NagFreq");
    }
  }
}
