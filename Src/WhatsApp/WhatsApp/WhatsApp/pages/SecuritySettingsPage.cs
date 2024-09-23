// Decompiled with JetBrains decompiler
// Type: WhatsApp.SecuritySettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace WhatsApp
{
  public class SecuritySettingsPage : PhoneApplicationPage
  {
    internal Localizable Localizable;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal Image Padlock;
    internal RichTextBox EndToEndText;
    internal ToggleSwitch ShowSecurityToggle;
    internal TextBlock ShowSecurityExplanation;
    private bool _contentLoaded;

    public SecuritySettingsPage()
    {
      this.InitializeComponent();
      this.Padlock.Source = (System.Windows.Media.ImageSource) AssetStore.SecurityPadlockIcon;
      this.ShowSecurityToggle.IsChecked = new bool?(Settings.E2EVerificationEnabled);
      Paragraph paragraph = new Paragraph();
      this.ShowSecurityExplanation.Text = AppResources.ShowSecurityExplanation;
      paragraph.Inlines.Add(AppResources.EndToEndEncryptedText + " ");
      Hyperlink hyperlink1 = new Hyperlink();
      hyperlink1.Foreground = (Brush) UIUtils.AccentBrush;
      hyperlink1.TextDecorations = (TextDecorationCollection) null;
      hyperlink1.Command = (ICommand) new ActionCommand((Action) (() => new WebBrowserTask()
      {
        Uri = new Uri(UriUtils.AppendLanguageAndLocaleToUrl("https://www.whatsapp.com/security/"))
      }.Show()));
      Hyperlink hyperlink2 = hyperlink1;
      hyperlink2.Inlines.Add(AppResources.LearnMoreSecurity);
      paragraph.Inlines.Add((Inline) hyperlink2);
      this.EndToEndText.Blocks.Add((Block) paragraph);
    }

    private void ShowSecurityToggle_Checked(object sender, RoutedEventArgs e)
    {
      Settings.E2EVerificationEnabled = true;
    }

    private void ShowSecurityToggle_Unchecked(object sender, RoutedEventArgs e)
    {
      Settings.E2EVerificationEnabled = false;
    }

    private void LearnMoreSecurity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri(UriUtils.AppendLanguageAndLocaleToUrl("https://www.whatsapp.com/security/"))
      }.Show();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/SecuritySettingsPage.xaml", UriKind.Relative));
      this.Localizable = (Localizable) this.FindName("Localizable");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.Padlock = (Image) this.FindName("Padlock");
      this.EndToEndText = (RichTextBox) this.FindName("EndToEndText");
      this.ShowSecurityToggle = (ToggleSwitch) this.FindName("ShowSecurityToggle");
      this.ShowSecurityExplanation = (TextBlock) this.FindName("ShowSecurityExplanation");
    }
  }
}
