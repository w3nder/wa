// Decompiled with JetBrains decompiler
// Type: WhatsApp.Localizable
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Globalization;


namespace WhatsApp
{
  public class Localizable
  {
    private static AppResources instance = AppState.GetAppResources();

    public static AppResources Strings => Localizable.instance;

    public static string GetString(string key) => AppState.GetString(key);

    private static string GetNeutralString(string key) => AppState.GetNeutralString(key);

    public static void LocalizeAppBar(PhoneApplicationPage page)
    {
      Localizable.LocalizeAppBar(page.ApplicationBar as ApplicationBar);
    }

    private static string RemapAppBarForRtl(string uri)
    {
      string str = (string) null;
      switch (uri)
      {
        case "/Images/next.png":
          str = "/Images/prev.png";
          break;
        case "/Images/prev.png":
          str = "/Images/next.png";
          break;
      }
      return str;
    }

    public static string StringForKey(string key)
    {
      string neutralString = Localizable.GetString(key);
      if (string.IsNullOrEmpty(neutralString))
        neutralString = Localizable.GetNeutralString(key);
      string langFriendlyLower = neutralString.ToLangFriendlyLower();
      if (langFriendlyLower == null)
        Log.l("appbar", "localization failed | key:{0}", (object) key);
      return langFriendlyLower;
    }

    public static void LocalizeAppBar(ApplicationBar bar)
    {
      if (bar == null)
        return;
      Action<ApplicationBarIconButton> action = (Action<ApplicationBarIconButton>) (b => { });
      if (CultureInfo.CurrentUICulture.IsRightToLeft())
        action = (Action<ApplicationBarIconButton>) (appBarButton =>
        {
          string uriString;
          if (!(appBarButton.IconUri != (Uri) null) || (uriString = Localizable.RemapAppBarForRtl(appBarButton.IconUri.OriginalString)) == null)
            return;
          appBarButton.IconUri = new Uri(uriString, UriKind.Relative);
        });
      ApplicationBar applicationBar = bar;
      if (applicationBar == null)
        return;
      foreach (object button in (IEnumerable) applicationBar.Buttons)
      {
        if (button is ApplicationBarIconButton applicationBarIconButton)
        {
          string str = Localizable.StringForKey(applicationBarIconButton.Text);
          if (str != null)
            applicationBarIconButton.Text = str;
          action(applicationBarIconButton);
        }
      }
      foreach (object menuItem in (IEnumerable) applicationBar.MenuItems)
      {
        if (menuItem is ApplicationBarMenuItem applicationBarMenuItem)
        {
          string str = Localizable.StringForKey(applicationBarMenuItem.Text);
          if (str != null)
            applicationBarMenuItem.Text = str;
        }
        if (menuItem is LocalizableAppBarMenuItem localizableAppBarMenuItem)
          localizableAppBarMenuItem.Text = string.Format(applicationBarMenuItem.Text, (object[]) localizableAppBarMenuItem.Args);
      }
    }
  }
}
