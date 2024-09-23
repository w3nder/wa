// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConnectionHelp
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.Controls;

#nullable disable
namespace WhatsApp
{
  public class ConnectionHelp : PhoneApplicationPage
  {
    private string[] languages;
    private DateTime started;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextAsWebControl Content;
    internal Button Contact;
    private bool _contentLoaded;

    public static int ConnectionFAQTimeRead { get; set; }

    public ConnectionHelp()
    {
      ConnectionHelp.ConnectionFAQTimeRead = 0;
      this.started = DateTime.Now;
      this.languages = new string[43]
      {
        "az",
        "id",
        "ms",
        "da",
        "cs",
        "ca",
        "de",
        "et",
        "en",
        "es",
        "fr",
        "hr",
        "it",
        "lv",
        "lt",
        "hu",
        "nl",
        "nb",
        "pl",
        "pt_br",
        "ro",
        "sk",
        "sl",
        "fi",
        "vi",
        "tr",
        "el",
        "bg",
        "mk",
        "ru",
        "sr",
        "uk",
        "he",
        "ar",
        "fa",
        "hi",
        "gu",
        "ta",
        "th",
        "zh_cn",
        "zh_tw",
        "ja",
        "ko"
      };
      this.InitializeComponent();
      string lang;
      AppState.GetLangAndLocale(out lang, out string _);
      if (!((IEnumerable<string>) this.languages).Contains<string>(lang))
      {
        if (((IEnumerable<string>) this.languages).Contains<string>(lang.Substring(0, 2)))
          lang = lang.Substring(0, 2);
        switch (lang)
        {
          case "pt":
            lang = "pt_br";
            break;
          case "sp":
            lang = "es";
            break;
          case "zh":
            lang = "zh_cn";
            break;
          default:
            lang = "en";
            break;
        }
      }
      this.Content.TextFile = "Resources/ConnectionFaqPages/" + lang + ".txt";
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
      ConnectionHelp.ConnectionFAQTimeRead = Math.Min((int) (DateTime.Now - this.started).TotalSeconds, 1);
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddBool("PageReplace", true);
      uriParams.AddString("context", ContactSupportHelper.AppendPhoneNumberIfNotLoggedIn(nameof (ConnectionHelp)));
      NavUtils.NavigateToPage(this.NavigationService, "ContactSupportPage", uriParams);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ConnectionHelp.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.Content = (TextAsWebControl) this.FindName("Content");
      this.Contact = (Button) this.FindName("Contact");
    }
  }
}
