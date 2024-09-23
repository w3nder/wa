// Decompiled with JetBrains decompiler
// Type: WhatsApp.NavUtils
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using WhatsApp.RegularExpressions;
using Windows.ApplicationModel;
using Windows.Phone.Management.Deployment;
using Windows.System;

#nullable disable
namespace WhatsApp
{
  public static class NavUtils
  {
    public static bool JumpBack(this PhoneApplicationFrame rootFrame, int backEntryToSkip = 0)
    {
      if (rootFrame == null)
        return false;
      Log.l("nav", "jump back | entries to skip:{0}", (object) backEntryToSkip);
      for (int index = 0; index < backEntryToSkip; ++index)
        rootFrame.RemoveBackEntry();
      rootFrame.GoBack();
      return true;
    }

    public static int SearchBackStack(this PhoneApplicationFrame rootFrame, string pageName)
    {
      return rootFrame.SearchBackStack(pageName, (IEnumerable<string>) new string[0]);
    }

    public static int SearchBackStack(
      this PhoneApplicationFrame rootFrame,
      string pageName,
      string keyword)
    {
      return rootFrame.SearchBackStack(pageName, (IEnumerable<string>) new string[1]
      {
        keyword
      });
    }

    public static int SearchBackStack(
      this PhoneApplicationFrame rootFrame,
      string pageName,
      IEnumerable<string> keywords)
    {
      return NavUtils.SearchBackStackImpl(rootFrame.BackStack, pageName, keywords);
    }

    public static void ClearBackStack()
    {
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (rootFrame == null)
        return;
      while (rootFrame.BackStack.Any<JournalEntry>())
        rootFrame.RemoveBackEntry();
    }

    public static bool JumpBack(this NavigationService nav, int backEntryToSkip = 0)
    {
      if (nav == null)
        return false;
      Log.l(nameof (nav), "jump back with nav service | entries to skip:{0}", (object) backEntryToSkip);
      for (int index = 0; index < backEntryToSkip; ++index)
        nav.RemoveBackEntry();
      nav.GoBack();
      return true;
    }

    public static int SearchBackStack(this NavigationService nav, string pageName)
    {
      return nav.SearchBackStack(pageName, (IEnumerable<string>) new string[0]);
    }

    public static int SearchBackStack(this NavigationService nav, string pageName, string keyword)
    {
      return nav.SearchBackStack(pageName, (IEnumerable<string>) new string[1]
      {
        keyword
      });
    }

    public static int SearchBackStack(
      this NavigationService nav,
      string pageName,
      IEnumerable<string> keywords)
    {
      return NavUtils.SearchBackStackImpl(nav.BackStack, pageName, keywords);
    }

    public static bool JumpBackTo(
      this PhoneApplicationFrame rootFrame,
      string pageName,
      string keyword = null,
      bool fallbackToHome = false)
    {
      PhoneApplicationFrame rootFrame1 = rootFrame;
      string pageName1 = pageName;
      string[] keywords;
      if (!string.IsNullOrEmpty(keyword))
        keywords = new string[1]{ keyword };
      else
        keywords = new string[0];
      int num = fallbackToHome ? 1 : 0;
      return rootFrame1.JumpBackTo(pageName1, (IEnumerable<string>) keywords, num != 0);
    }

    public static bool JumpBackTo(
      this PhoneApplicationFrame rootFrame,
      string pageName,
      IEnumerable<string> keywords,
      bool fallbackToHome = false)
    {
      if (rootFrame == null)
        return false;
      int backEntryToSkip = rootFrame.SearchBackStack(pageName, keywords);
      if (backEntryToSkip >= 0)
      {
        Log.l("nav", "jump back to {0}", (object) pageName);
        return rootFrame.JumpBack(backEntryToSkip);
      }
      if (fallbackToHome)
      {
        Log.l("nav", "jump back to {0} | fallback to home", (object) pageName);
        if (!rootFrame.JumpBackTo("ContactsPage", (IEnumerable<string>) null))
          NavUtils.NavigateHome();
        return true;
      }
      Log.l("nav", "jump back to {0} | no-op", (object) pageName);
      return false;
    }

    public static bool JumpBackTo(
      this NavigationService nav,
      string pageName,
      string keyword = null,
      bool fallbackToHome = false)
    {
      NavigationService nav1 = nav;
      string pageName1 = pageName;
      string[] keywords;
      if (!string.IsNullOrEmpty(keyword))
        keywords = new string[1]{ keyword };
      else
        keywords = new string[0];
      int num = fallbackToHome ? 1 : 0;
      return nav1.JumpBackTo(pageName1, (IEnumerable<string>) keywords, num != 0);
    }

    public static bool JumpBackTo(
      this NavigationService nav,
      string pageName,
      IEnumerable<string> keywords,
      bool fallbackToHome = false)
    {
      if (nav == null)
        return false;
      int backEntryToSkip = nav.SearchBackStack(pageName, keywords);
      if (backEntryToSkip >= 0)
      {
        Log.l(nameof (nav), "jump back to {0} with nav service", (object) pageName);
        return nav.JumpBack(backEntryToSkip);
      }
      if (fallbackToHome)
      {
        Log.l(nameof (nav), "jump back to {0} with nav service | fallback to home", (object) pageName);
        if (!nav.JumpBackTo("ContactsPage", (IEnumerable<string>) null))
          NavUtils.NavigateHome(nav);
        return true;
      }
      Log.l(nameof (nav), "jump back to {0} with nav service | no-op", (object) pageName);
      return false;
    }

    public static void NavigateToChat(
      string jid,
      bool searchBackFirst,
      string clearStackToPage = "ContactsPage",
      WaUriParams additionalParams = null)
    {
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (rootFrame == null || JidHelper.IsStatusJid(jid) || rootFrame.CanGoBack & searchBackFirst && rootFrame.JumpBackTo("ChatPage", jid))
        return;
      if (App.CurrentApp.CurrentPage is ChatPage currentPage)
        currentPage.OnNavigatedFromImpl();
      WaUriParams uriParams = WaUriParams.ForChatPage(jid, (string) null, clearStackTo: clearStackToPage);
      uriParams.AddParams(additionalParams);
      NavUtils.NavigateToPage("ChatPage", uriParams);
    }

    public static void NavigateToChat(
      NavigationService nav,
      string jid,
      bool searchBackFirst,
      string clearStackToPage = "ContactsPage",
      WaUriParams additionalParams = null)
    {
      if (nav == null)
      {
        NavUtils.NavigateToChat(jid, searchBackFirst, clearStackToPage, additionalParams);
      }
      else
      {
        if (JidHelper.IsStatusJid(jid) || nav.CanGoBack & searchBackFirst && nav.JumpBackTo("ChatPage", jid))
          return;
        if (App.CurrentApp.CurrentPage is ChatPage currentPage)
          currentPage.OnNavigatedFromImpl();
        WaUriParams uriParams = WaUriParams.ForChatPage(jid, (string) null, clearStackTo: clearStackToPage);
        uriParams.AddParams(additionalParams);
        NavUtils.NavigateToPage(nav, "ChatPage", uriParams);
      }
    }

    public static bool NavigateBackToChat(bool fallbackToHome = true)
    {
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (rootFrame == null)
        return false;
      if (rootFrame.CanGoBack)
      {
        if (rootFrame.JumpBackTo("ChatPage"))
          return true;
        if (fallbackToHome && rootFrame.JumpBackTo("ContactsPage"))
          return false;
      }
      if (fallbackToHome)
        NavUtils.NavigateHome();
      return false;
    }

    public static bool NavigateBackToChat(NavigationService nav, bool fallbackToHome = true)
    {
      if (nav == null)
        return NavUtils.NavigateBackToChat(fallbackToHome);
      if (nav.CanGoBack)
      {
        if (nav.JumpBackTo("ChatPage"))
          return true;
        if (fallbackToHome && nav.JumpBackTo("ContactsPage"))
          return false;
      }
      if (fallbackToHome)
        NavUtils.NavigateHome(nav);
      return false;
    }

    public static void GoBack(bool fallbackToHome = true)
    {
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      if (rootFrame == null)
        return;
      if (rootFrame.CanGoBack)
      {
        Log.l("nav", "go back");
        rootFrame.GoBack();
      }
      else
      {
        if (!fallbackToHome)
          return;
        Log.l("nav", "can't go back | start fresh");
        NavUtils.NavigateHome();
      }
    }

    public static void GoBack(NavigationService nav, bool fallbackToHome = true)
    {
      if (nav == null)
        NavUtils.GoBack(fallbackToHome);
      else if (nav.CanGoBack)
      {
        Log.d(nameof (nav), "go back with nav service");
        nav.GoBack();
      }
      else
      {
        if (!fallbackToHome)
          return;
        Log.d(nameof (nav), "can't go back | start fresh");
        NavUtils.NavigateHome(nav);
      }
    }

    public static void NavigateToPage(
      string pageName,
      WaUriParams uriParams,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      App.CurrentApp.RootFrame?.Navigate(UriUtils.CreatePageUri(pageName, uriParams, folderName, uriKind));
    }

    public static void NavigateToPage(
      NavigationService nav,
      string pageName,
      WaUriParams uriParams,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      if (nav == null)
        NavUtils.NavigateToPage(pageName, uriParams, folderName, uriKind);
      else
        nav.Navigate(UriUtils.CreatePageUri(pageName, uriParams, folderName, uriKind));
    }

    public static void NavigateToPage(
      string pageName,
      string parameters = null,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      App.CurrentApp.RootFrame?.Navigate(UriUtils.CreatePageUri(pageName, parameters, folderName, uriKind));
    }

    public static void NavigateToPage(
      NavigationService nav,
      string pageName,
      string parameters = null,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      if (nav == null)
        NavUtils.NavigateToPage(pageName, parameters, folderName, uriKind);
      else
        nav.Navigate(UriUtils.CreatePageUri(pageName, parameters, folderName, uriKind));
    }

    private static int SearchBackStackImpl(
      IEnumerable<JournalEntry> backStack,
      string pageName,
      IEnumerable<string> keywords)
    {
      int num = 0;
      bool flag = false;
      Regex regex = (Regex) null;
      if (keywords != null && keywords.Any<string>())
      {
        keywords = (IEnumerable<string>) keywords.Select<string, string>((Func<string, string>) (w => Uri.EscapeDataString(w))).ToArray<string>();
        regex = new Regex(string.Join("\\.*", keywords), RegexOptions.IgnoreCase);
      }
      foreach (JournalEntry back in backStack)
      {
        string originalString = back.Source.OriginalString;
        if (originalString.IndexOf(pageName + ".xaml") != -1 && (regex == null || regex.Match(originalString).Success))
        {
          flag = true;
          break;
        }
        ++num;
      }
      return !flag ? -1 : num;
    }

    public static void NavigateHome(NavigationService nav)
    {
      Uri source = new Uri("/PageSelect?ClearStack=true", UriKind.Relative);
      nav.Navigate(source);
    }

    public static void NavigateHome()
    {
      App.CurrentApp.RootFrame.Navigate(new Uri("/PageSelect?ClearStack=true", UriKind.Relative));
    }

    public static void NavigateExternal(string url, Action onComplete = null, Action<Exception> onError = null)
    {
      Log.l("nav", "to external: {0}", (object) url);
      if (url.StartsWith("app:", StringComparison.InvariantCultureIgnoreCase))
        NativeInterfaces.Misc.LaunchSession(url, CallbackResult.Create(onComplete, onError));
      else
        Launcher.LaunchUriAsync(new Uri(url));
    }

    public static void NavigateExternalByProductId(string productId, string parameters = "")
    {
      Package package1 = (Package) null;
      foreach (Package package2 in InstallationManager.FindPackages())
      {
        if (package2.Id.ProductId.Equals(productId, StringComparison.InvariantCultureIgnoreCase))
        {
          package1 = package2;
          break;
        }
      }
      package1?.Launch(parameters);
    }

    public static void LogBackStack(bool omitInRelease)
    {
      string[] array = App.CurrentApp.RootFrame.BackStack.Select<JournalEntry, string>((Func<JournalEntry, string>) (entry => entry.Source.OriginalString)).ToArray<string>();
      int num = 0;
      foreach (string str in array)
      {
        if (omitInRelease)
          Log.d("backstack", "{0} | {1}", (object) num++, (object) str);
        else
          Log.l("backstack", "{0} | {1}", (object) num++, (object) str);
      }
    }

    public static void VerifyIdentityForJid(string jid)
    {
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString(nameof (jid), jid);
      NavUtils.NavigateToPage("IdentityVerificationPage.xaml", uriParams);
    }

    public enum BackStackOps
    {
      None,
      ReplaceOne,
      ClearAll,
      ClearTo,
    }

    public class BackStackOpParams
    {
      public bool ClearBackStack { get; private set; }

      public string ClearBackStackTo { get; private set; }

      public int BackStackEntriesToRemove { get; private set; }

      private BackStackOpParams()
      {
        this.ClearBackStack = false;
        this.ClearBackStackTo = (string) null;
        this.BackStackEntriesToRemove = 0;
      }

      public bool IsActionable
      {
        get
        {
          return this.ClearBackStack || !string.IsNullOrEmpty(this.ClearBackStackTo) || this.BackStackEntriesToRemove > 0;
        }
      }

      public static NavUtils.BackStackOpParams FromUriParams(WaUriParams uriParams)
      {
        NavUtils.BackStackOpParams backStackOpParams = new NavUtils.BackStackOpParams();
        if (uriParams == null)
          return backStackOpParams;
        bool val1 = false;
        if (uriParams.TryGetBoolValue("ClearStack", out val1) & val1)
        {
          backStackOpParams.ClearBackStack = true;
          return backStackOpParams;
        }
        string val2 = (string) null;
        if (uriParams.TryGetStrValue("clr2", out val2) && !string.IsNullOrEmpty(val2))
          backStackOpParams.ClearBackStackTo = val2;
        int val3 = 0;
        if (uriParams.TryGetIntValue("BackEntryRemoval", out val3) && val3 > 0)
          backStackOpParams.BackStackEntriesToRemove = val3;
        if (((val3 >= 1 ? 0 : (uriParams.TryGetBoolValue("PageReplace", out val1) ? 1 : 0)) & (val1 ? 1 : 0)) != 0)
        {
          int num;
          backStackOpParams.BackStackEntriesToRemove = num = 1;
        }
        return backStackOpParams;
      }
    }
  }
}
