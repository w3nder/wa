// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneNumberEntry
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using WhatsAppNative;


namespace WhatsApp
{
  public class PhoneNumberEntry : PhoneApplicationPage
  {
    private const string LogHeader = "phone number entry";
    private CountryInfoItem currentCountry;
    private List<CountryInfoViewModel> countryInfoViewModels;
    private string simNumber;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal TextBlock JustDeletedInstruction;
    internal TextBlock CountryName;
    internal TextBox CountryCode;
    internal TextBox PhoneNumber;
    internal TextBlock SuggestionTooltip;
    internal RichTextBlock GdprTooltipBlock;
    internal Grid CountryPickerPanel;
    internal WhatsApp.CompatibilityShims.LongListSelector CountryNameSelector;
    internal ListBox TopCountries;
    private bool _contentLoaded;

    public PhoneNumberEntry()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.EnterYourPhoneNumber;
      this.JustDeletedInstruction.Visibility = App.AccountJustDeleted.ToVisibility();
      this.JustDeletedInstruction.Text = AppResources.DeleteAccountInstruction;
      this.simNumber = PhoneNumberEntry.GetSIMPhoneNumber();
      this.PhoneNumber.TextChanged += new TextChangedEventHandler(this.PhoneNumber_TextChanged);
      if (!string.IsNullOrEmpty(Settings.PhoneNumber) && Settings.OldChatID == null)
        this.PhoneNumber.Text = Settings.PhoneNumber;
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      PhoneNumberFormatter.InstallAsYouTypeFormatter(this.CountryCode, this.PhoneNumber);
    }

    private void NormalizeCountryCode()
    {
      string code = this.CountryCode.Text.ExtractDigits() ?? "";
      if (code != this.CountryCode.Text)
      {
        this.CountryCode.TextChanged -= new TextChangedEventHandler(this.CountryCode_TextChanged);
        this.CountryCode.Text = code;
        this.CountryCode.SelectionStart = code.Length;
        this.CountryCode.TextChanged += new TextChangedEventHandler(this.CountryCode_TextChanged);
      }
      CountryInfoItem countryInfoItem;
      if (this.currentCountry?.PhoneCountryCode == code)
        countryInfoItem = this.currentCountry;
      else
        this.currentCountry = countryInfoItem = CountryInfo.Instance.GetCountryInfoForCountryCode(code);
      this.CountryName.Text = countryInfoItem?.FullName ?? AppResources.SelectCountryPrompt;
      if (countryInfoItem != null && GdprTos.IsEEA(countryInfoItem.PhoneCountryCode))
      {
        string numberEntryTooltip = AppResources.GdprTosPhoneNumberEntryTooltip;
        KeyValuePair<WaRichText.Formats, string>[] formattings = new KeyValuePair<WaRichText.Formats, string>[2]
        {
          new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlAge),
          new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlWorkingWithFb)
        };
        this.GdprTooltipBlock.Text = new RichTextBlock.TextSet()
        {
          Text = numberEntryTooltip,
          PartialFormattings = (IEnumerable<WaRichText.Chunk>) GdprTos.GetRichTextFormattings(numberEntryTooltip, formattings)
        };
        this.GdprTooltipBlock.Visibility = Visibility.Visible;
      }
      else
        this.GdprTooltipBlock.Visibility = Visibility.Collapsed;
    }

    public static string GetSIMPhoneNumber()
    {
      string str = (string) null;
      try
      {
        str = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.PhoneNumber).SimPhoneNumber.ExtractDigits();
      }
      catch (Exception ex)
      {
        Log.l(ex, "loading SIM phone number");
      }
      Log.l("phone number entry", "detect phone number from sim: {0}", string.IsNullOrEmpty(str) ? (object) "n/a" : (object) str);
      return str ?? "";
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      string str = Settings.CountryCode;
      if (string.IsNullOrEmpty(str))
      {
        CountryInfoItem countryInfoItem = (CountryInfoItem) null;
        try
        {
          countryInfoItem = CountryInfo.Instance.GetCountryForMcc(NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.MccMnc).Mcc);
        }
        catch (Exception ex)
        {
          Log.l(ex, "phone number entry: cannot load cell info");
        }
        if (countryInfoItem == null)
        {
          string name = CultureInfo.CurrentCulture.Name;
          countryInfoItem = CountryInfo.Instance.GetCountryInfoForISOCountryCode(name.Substring(name.IndexOf('-') + 1));
        }
        str = countryInfoItem?.PhoneCountryCode ?? "";
      }
      this.CountryCode.Text = str;
      this.SetCountryListSource();
      this.NormalizeCountryCode();
    }

    private void SetCountryListSource()
    {
      this.countryInfoViewModels = new List<CountryInfoViewModel>();
      CountryInfoViewModel[] array1 = ((IEnumerable<string>) new string[6]
      {
        "IN",
        "BR",
        "IT",
        "RU",
        "DE",
        "AR"
      }).Select<string, CountryInfoViewModel>((Func<string, CountryInfoViewModel>) (cc => new CountryInfoViewModel(CountryInfo.Instance.GetCountryInfoForISOCountryCode(cc))
      {
        IsTop = true
      })).ToArray<CountryInfoViewModel>();
      CountryInfoViewModel[] array2 = CountryInfo.Instance.GetSortedCountryInfos().Where<CountryInfoItem>((Func<CountryInfoItem, bool>) (c => c != null)).OrderBy<CountryInfoItem, string>((Func<CountryInfoItem, string>) (c => c.FullName)).Select<CountryInfoItem, CountryInfoViewModel>((Func<CountryInfoItem, CountryInfoViewModel>) (c => new CountryInfoViewModel(c))).ToArray<CountryInfoViewModel>();
      this.countryInfoViewModels.AddRange((IEnumerable<CountryInfoViewModel>) array1);
      this.countryInfoViewModels.AddRange((IEnumerable<CountryInfoViewModel>) array2);
      IEnumerable<IList<CountryInfoViewModel>> source = ((IEnumerable<CountryInfoViewModel>) array2).GroupBy<CountryInfoViewModel, string>((Func<CountryInfoViewModel, string>) (x => x.FullName.ToGroupChar())).Select<IGrouping<string, CountryInfoViewModel>, IList<CountryInfoViewModel>>((Func<IGrouping<string, CountryInfoViewModel>, IList<CountryInfoViewModel>>) (cg => cg.ToGoodGrouping<string, CountryInfoViewModel>()));
      this.TopCountries.ItemsSource = (IEnumerable) array1;
      this.CountryNameSelector.ItemsSource = (IList) source.ToList<IList<CountryInfoViewModel>>();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.CountryPickerPanel.Visibility == Visibility.Visible)
      {
        this.ShowCountryList(false);
        e.Cancel = true;
      }
      base.OnBackKeyPress(e);
    }

    private void Next_Click(object sender, EventArgs e)
    {
      string digits = this.PhoneNumber.Text.ExtractDigits();
      if (string.IsNullOrEmpty(this.CountryCode.Text.ExtractDigits()))
      {
        if (string.IsNullOrEmpty(digits))
        {
          int num1 = (int) MessageBox.Show(AppResources.EnterCountryCodeAndPhoneNumber, AppResources.EnterPhoneNumberTitle, MessageBoxButton.OK);
        }
        else
        {
          int num2 = (int) MessageBox.Show(AppResources.EnterCountryCode, AppResources.EnterCountryCodeTitle, MessageBoxButton.OK);
        }
      }
      else
      {
        this.NormalizeCountryCode();
        CountryInfoItem currentCountry = this.currentCountry;
        if (currentCountry == null)
        {
          Log.l("phone number entry", "country code not valid: {0}", (object) this.CountryCode.Text);
          int num3 = (int) MessageBox.Show(AppResources.InvalidCountryCodeText, AppResources.InvalidCountryCodeTitle, MessageBoxButton.OK);
        }
        else
        {
          string str = currentCountry.ApplyLeadingDigitsFilter(digits);
          if (currentCountry.AllowedLengths.Count > 0 && !currentCountry.AllowedLengths.Contains(str.Length))
          {
            Log.l("phone number entry", "length not valid: {0} {1}", (object) this.CountryCode.Text, (object) this.PhoneNumber.Text);
            if (str.Length == 0)
            {
              int num4 = (int) MessageBox.Show(AppResources.EnterPhoneNumber, AppResources.EnterPhoneNumberTitle, MessageBoxButton.OK);
            }
            else if (str.Length < currentCountry.AllowedLengths.First<int>())
            {
              int num5 = (int) MessageBox.Show(string.Format(AppResources.PhoneNumberTooShort, (object) currentCountry.FullName), AppResources.PhoneNumberTooShortTitle, MessageBoxButton.OK);
            }
            else if (str.Length > currentCountry.AllowedLengths.Last<int>())
            {
              int num6 = (int) MessageBox.Show(string.Format(AppResources.PhoneNumberTooLong, (object) currentCountry.FullName), AppResources.PhoneNumberTooLongTitle, MessageBoxButton.OK);
            }
            else
            {
              int num7 = (int) MessageBox.Show(string.Format(AppResources.InvalidPhoneNumberLengthForCountry, (object) currentCountry.FullName), AppResources.IncorrectPhoneNumberTitle, MessageBoxButton.OK);
            }
          }
          else if (string.IsNullOrEmpty(str))
          {
            Log.l("phone number entry", "empty number");
          }
          else
          {
            App.AccountJustDeleted = false;
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
            Settings.CountryCode = currentCountry.PhoneCountryCode;
            Settings.PhoneNumber = str;
            Settings.Delete(Settings.Key.CodeEntryWaitToRetryUtc);
            NavUtils.NavigateHome();
          }
        }
      }
    }

    private void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.SuggestionTooltip.Text = "";
      string digits1 = this.PhoneNumber.Text.ExtractDigits();
      string digits2 = this.CountryCode.Text.ExtractDigits();
      if (digits1.Length == this.simNumber.Length || digits1.Length + digits2.Length == this.simNumber.Length)
      {
        int index = -1;
        string suggestion = PhoneNumberEntry.GetSuggestion(digits1, digits2, this.simNumber, out index);
        if (string.IsNullOrEmpty(suggestion))
          return;
        Log.l("phone number entry", "phone number mistype detected: {0} to {1}", (object) PhoneNumberFormatter.FormatInternationalNumber(digits2 + digits1), (object) suggestion);
        PhoneNumberEntry.SuggestReplacement(this.SuggestionTooltip, suggestion, index);
      }
      else
      {
        this.SuggestionTooltip.Visibility = Visibility.Collapsed;
        this.SuggestionTooltip.Text = "";
      }
    }

    public static string GetSuggestion(
      string numberInput,
      string ccDigits,
      string simNumber,
      out int index)
    {
      numberInput = numberInput.ExtractDigits();
      ccDigits = ccDigits.ExtractDigits();
      simNumber = simNumber.ExtractDigits();
      index = -1;
      if (numberInput.Length + ccDigits.Length != simNumber.Length && numberInput.Length != simNumber.Length)
        return (string) null;
      string suggestion = PhoneNumberFormatter.FormatInternationalNumber(simNumber);
      int length = suggestion.IndexOf(" ");
      if (length == -1)
        return (string) null;
      string digits = suggestion.Substring(0, length).ExtractDigits();
      string str1 = suggestion.Substring(length + 1);
      string str2 = str1.ExtractDigits();
      if (ccDigits != digits)
      {
        str2 = simNumber;
        suggestion = PhoneNumberFormatter.FormatInternationalNumber(ccDigits + str2);
        length = suggestion.IndexOf(" ");
        if (length == -1)
          return (string) null;
        str1 = suggestion.Substring(length + 1);
      }
      if (numberInput.Length == str2.Length)
      {
        int num1 = 0;
        int num2 = -1;
        for (int index1 = 0; index1 < numberInput.Length; ++index1)
        {
          if ((int) numberInput[index1] != (int) str2[index1])
          {
            ++num1;
            num2 = index1;
          }
        }
        if (num1 == 1)
        {
          int index2 = 0;
          int num3 = 0;
          while (num3 <= num2)
          {
            if (char.IsDigit(str1[index2]))
              ++num3;
            ++index2;
          }
          int num4 = index2 + length;
          index = num4;
          return suggestion;
        }
      }
      return (string) null;
    }

    public static void SuggestReplacement(
      TextBlock tooltip,
      string suggestedNumber,
      int mistypedIndex)
    {
      string registrationMistypedTooltip = AppResources.RegistrationMistypedTooltip;
      int length = registrationMistypedTooltip.IndexOf("{");
      tooltip.Inlines.Add(registrationMistypedTooltip.Substring(0, length));
      InlineCollection inlines1 = tooltip.Inlines;
      Run run1 = new Run();
      run1.Text = suggestedNumber.Substring(0, mistypedIndex);
      run1.TextDecorations = TextDecorations.Underline;
      inlines1.Add((Inline) run1);
      InlineCollection inlines2 = tooltip.Inlines;
      Run run2 = new Run();
      run2.Text = suggestedNumber.Substring(mistypedIndex, 1);
      run2.FontWeight = FontWeights.Bold;
      run2.Foreground = (Brush) new SolidColorBrush(Colors.Red);
      run2.TextDecorations = TextDecorations.Underline;
      inlines2.Add((Inline) run2);
      InlineCollection inlines3 = tooltip.Inlines;
      Run run3 = new Run();
      run3.Text = suggestedNumber.Substring(mistypedIndex + 1);
      run3.TextDecorations = TextDecorations.Underline;
      inlines3.Add((Inline) run3);
      if (length + 3 < registrationMistypedTooltip.Length)
        tooltip.Inlines.Add(registrationMistypedTooltip.Substring(length + 3));
      tooltip.Visibility = Visibility.Visible;
    }

    private void SuggestionTooltip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      int length = this.CountryCode.Text.ExtractDigits().Length;
      this.PhoneNumber.Text = this.simNumber.Length == this.PhoneNumber.Text.ExtractDigits().Length ? this.simNumber : this.simNumber.Substring(length);
      this.SuggestionTooltip.Visibility = Visibility.Collapsed;
      this.SuggestionTooltip.Text = "";
    }

    private void CountryCode_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.NormalizeCountryCode();
    }

    private void ShowCountryList(bool show)
    {
      if (show)
      {
        this.PageTitle.SmallTitle = AppResources.SelectCountryTitle;
        this.CountryPickerPanel.Visibility = Visibility.Visible;
        ((CompositeTransform) this.CountryPickerPanel.RenderTransform).TranslateY = 200.0;
        this.CountryPickerPanel.Opacity = 0.0;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          WaAnimations.AnimateTo((FrameworkElement) this.CountryPickerPanel, new double?(), new double?(), new double?(200.0), new double?(0.0), new double?(), new double?(), new double?(), new double?(), (Action) null);
          WaAnimations.PerformFade((DependencyObject) this.CountryPickerPanel, WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(150.0), (Action) (() => this.CountryPickerPanel.Opacity = 1.0));
        }));
      }
      else
      {
        this.PageTitle.SmallTitle = AppResources.EnterYourPhoneNumber;
        WaAnimations.AnimateTo((FrameworkElement) this.CountryPickerPanel, new double?(), new double?(), new double?(0.0), new double?(200.0), new double?(), new double?(), new double?(), new double?(), (Action) null);
        this.CountryPickerPanel.Opacity = 1.0;
        WaAnimations.PerformFade((DependencyObject) this.CountryPickerPanel, WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(150.0), (Action) (() =>
        {
          this.CountryPickerPanel.Visibility = Visibility.Collapsed;
          this.CountryPickerPanel.Opacity = 0.0;
        }));
      }
      this.ApplicationBar.IsVisible = !show;
    }

    private void ScrollToCountryName(CountryInfoViewModel vm)
    {
      this.CountryNameSelector.UpdateLayout();
      if (vm.IsTop)
        this.CountryNameSelector.ScrollTo(this.CountryNameSelector.ListHeader);
      else
        this.CountryNameSelector.ScrollTo((object) vm);
    }

    private void CountryName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ShowCountryList(true);
      if (this.currentCountry != null)
      {
        string isoCode = this.currentCountry.IsoCode;
        CountryInfoViewModel vm = (CountryInfoViewModel) null;
        foreach (CountryInfoViewModel countryInfoViewModel in this.countryInfoViewModels)
        {
          if (countryInfoViewModel.Item != null && countryInfoViewModel.Item.IsoCode == isoCode)
          {
            countryInfoViewModel.IsSelected = true;
            if (vm == null)
              vm = countryInfoViewModel;
          }
          else
            countryInfoViewModel.IsSelected = false;
        }
        if (vm == null)
          return;
        this.ScrollToCountryName(vm);
        this.CountryNameSelector.LayoutUpdated += new EventHandler(this.CountryList_LayoutUpdated);
      }
      else
      {
        foreach (CountryInfoViewModel countryInfoViewModel in this.countryInfoViewModels)
          countryInfoViewModel.IsSelected = false;
        CountryInfoViewModel vm = this.countryInfoViewModels.FirstOrDefault<CountryInfoViewModel>();
        if (vm == null)
          return;
        this.ScrollToCountryName(vm);
      }
    }

    private void CountryList_LayoutUpdated(object sender, EventArgs e)
    {
      this.CountryNameSelector.LayoutUpdated -= new EventHandler(this.CountryList_LayoutUpdated);
      ViewportControl logicalChildByType = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ViewportControl>(this.CountryNameSelector, false);
      Rect viewport = logicalChildByType.Viewport;
      double num = (logicalChildByType.ActualHeight - 72.0) / 4.0;
      if ((double) (this.countryInfoViewModels.Count - this.countryInfoViewModels.FindIndex((Predicate<CountryInfoViewModel>) (x => x.FullName == this.currentCountry.FullName)) + 4) * 72.0 <= viewport.Height)
        return;
      logicalChildByType.SetViewportOrigin(new System.Windows.Point(viewport.X, viewport.Y - num));
    }

    private void CountryNameSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!((this.TopCountries.SelectedItem ?? this.CountryNameSelector.SelectedItem) is CountryInfoViewModel countryInfoViewModel))
        return;
      this.CountryNameSelector.SelectedItem = this.TopCountries.SelectedItem = (object) null;
      countryInfoViewModel.IsSelected = true;
      this.ShowCountryList(false);
      this.currentCountry = countryInfoViewModel.Item;
      this.CountryCode.Text = countryInfoViewModel.PhoneCountryCode;
      this.NormalizeCountryCode();
    }

    private void CountryCode_GotFocus(object sender, RoutedEventArgs e)
    {
      this.CountryCode.TextChanged -= new TextChangedEventHandler(this.CountryCode_TextChanged);
      this.CountryCode.TextChanged += new TextChangedEventHandler(this.CountryCode_TextChanged);
    }

    private void CountryCode_LostFocus(object sender, RoutedEventArgs e)
    {
      this.CountryCode.TextChanged -= new TextChangedEventHandler(this.CountryCode_TextChanged);
      this.NormalizeCountryCode();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/PhoneNumberEntry.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.JustDeletedInstruction = (TextBlock) this.FindName("JustDeletedInstruction");
      this.CountryName = (TextBlock) this.FindName("CountryName");
      this.CountryCode = (TextBox) this.FindName("CountryCode");
      this.PhoneNumber = (TextBox) this.FindName("PhoneNumber");
      this.SuggestionTooltip = (TextBlock) this.FindName("SuggestionTooltip");
      this.GdprTooltipBlock = (RichTextBlock) this.FindName("GdprTooltipBlock");
      this.CountryPickerPanel = (Grid) this.FindName("CountryPickerPanel");
      this.CountryNameSelector = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("CountryNameSelector");
      this.TopCountries = (ListBox) this.FindName("TopCountries");
    }
  }
}
