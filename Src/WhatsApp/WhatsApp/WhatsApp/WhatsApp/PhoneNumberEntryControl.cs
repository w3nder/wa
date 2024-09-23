// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneNumberEntryControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhatsAppNative;


namespace WhatsApp
{
  public class PhoneNumberEntryControl : UserControl
  {
    internal Grid LayoutRoot;
    internal TextBlock TitleBlock;
    internal TextBox CountryCodeBox;
    internal TextBox PhoneNumberBox;
    private bool _contentLoaded;

    public string TitleText
    {
      get => this.TitleBlock.Text;
      set
      {
        string str = value;
        this.TitleBlock.Text = str ?? "";
        this.TitleBlock.Visibility = (!string.IsNullOrEmpty(str)).ToVisibility();
      }
    }

    public string CountryCode
    {
      get
      {
        return this.CountryCodeBox.Text != null ? this.CountryCodeBox.Text.ExtractDigits() : (string) null;
      }
      set
      {
        this.CountryCodeBox.Text = value;
        this.FormatCountryCode(this.CountryCodeBox);
      }
    }

    public string PhoneNumber
    {
      get
      {
        CountryInfoItem cii = this.GetCii();
        if (cii == null)
          throw new PhoneNumberEntryException(AppResources.InvalidCountryCodeText);
        if (string.IsNullOrEmpty(this.PhoneNumberBox.Text))
          throw new PhoneNumberEntryException(string.Format(AppResources.PhoneNumberTooShort, (object) cii.FullName));
        string str = cii.ApplyLeadingDigitsFilter(this.PhoneNumberBox.Text.ExtractDigits());
        if (cii.AllowedLengths.Count > 0 && !cii.AllowedLengths.Contains(str.Length))
        {
          if (str.Length < cii.AllowedLengths.First<int>())
            throw new PhoneNumberEntryException(string.Format(AppResources.PhoneNumberTooShort, (object) cii.FullName));
          if (str.Length > cii.AllowedLengths.Last<int>())
            throw new PhoneNumberEntryException(string.Format(AppResources.PhoneNumberTooLong, (object) cii.FullName));
          throw new PhoneNumberEntryException(string.Format(AppResources.InvalidPhoneNumberLengthForCountry, (object) cii.FullName));
        }
        return this.PhoneNumberBox.Text != null ? this.PhoneNumberBox.Text.ExtractDigits() : (string) null;
      }
      set => this.PhoneNumberBox.Text = value;
    }

    public PhoneNumberEntryControl()
    {
      this.InitializeComponent();
      PhoneNumberFormatter.InstallAsYouTypeFormatter(this.CountryCodeBox, this.PhoneNumberBox);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      CountryInfoItem countryInfoItem = (CountryInfoItem) null;
      try
      {
        countryInfoItem = CountryInfo.Instance.GetCountryForMcc(NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.MccMnc).Mcc);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "change phone number: cannot load cell info");
      }
      if (countryInfoItem == null)
      {
        string name = CultureInfo.CurrentCulture.Name;
        countryInfoItem = CountryInfo.Instance.GetCountryInfoForISOCountryCode(name.Substring(name.IndexOf('-') + 1));
      }
      this.CountryCodeBox.Text = !string.IsNullOrEmpty(Settings.CountryCode) ? Settings.CountryCode : (countryInfoItem == null ? "" : countryInfoItem.PhoneCountryCode);
      this.FormatCountryCode(this.CountryCodeBox);
    }

    public CountryInfoItem GetCii()
    {
      string digits = this.CountryCodeBox.Text.ExtractDigits();
      CountryInfoItem cii = (CountryInfoItem) null;
      if (!string.IsNullOrEmpty(digits))
        cii = CountryInfo.Instance.GetCountryInfoForCountryCode(digits);
      return cii;
    }

    private void FormatCountryCode(TextBox codeBox)
    {
      if (codeBox == null || codeBox.Text == null)
        return;
      string digits = codeBox.Text.ExtractDigits();
      if (codeBox.Text.StartsWith("+"))
        return;
      codeBox.Text = "+" + digits;
    }

    private void CountryCode_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!(sender is TextBox codeBox) || string.IsNullOrEmpty(codeBox.Text))
        return;
      int num = codeBox.SelectionStart == 0 ? 1 : 0;
      this.FormatCountryCode(codeBox);
      if (num == 0)
        return;
      codeBox.Select(codeBox.Text.Length, 0);
    }

    private void CountryCode_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!(sender is TextBox textBox))
        return;
      textBox.TextChanged -= new TextChangedEventHandler(this.CountryCode_TextChanged);
      textBox.TextChanged += new TextChangedEventHandler(this.CountryCode_TextChanged);
    }

    private void CountryCode_LostFocus(object sender, RoutedEventArgs e)
    {
      if (!(sender is TextBox codeBox))
        return;
      codeBox.TextChanged -= new TextChangedEventHandler(this.CountryCode_TextChanged);
      this.FormatCountryCode(codeBox);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/PhoneNumberEntryControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitleBlock = (TextBlock) this.FindName("TitleBlock");
      this.CountryCodeBox = (TextBox) this.FindName("CountryCodeBox");
      this.PhoneNumberBox = (TextBox) this.FindName("PhoneNumberBox");
    }
  }
}
