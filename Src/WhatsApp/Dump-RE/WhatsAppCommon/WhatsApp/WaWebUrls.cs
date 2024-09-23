// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaWebUrls
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class WaWebUrls
  {
    public const string FaqUrl = "https://faq.whatsapp.com";

    public static string FaqUrlGroupE2e => WaWebUrls.GetFaqUrlGeneral("28030015");

    public static string FaqUrlSecurityCodeChanged => WaWebUrls.GetFaqUrlGeneral("28030014");

    public static string FaqUrlE2ePlaceholder => WaWebUrls.GetFaqUrlGeneral("26000015");

    public static string FaqlUrlPushDisconnected
    {
      get => WaWebUrls.GetFaqUrl("/link/push.php", (Dictionary<string, string>) null);
    }

    public static string FaqlUrlLiveLocation => WaWebUrls.GetFaqUrlWp("26000054");

    public static string FaqUrlRecall => WaWebUrls.GetFaqUrlGeneral("26000034");

    public static string FaqUrlRecallWp => WaWebUrls.GetFaqUrlWp("26000070");

    public static string FaqUrlGdprReport => WaWebUrls.GetFaqUrlGeneral("26000110");

    public static string FaqUrlWorkingWithFb => WaWebUrls.GetFaqUrlGeneral("26000112");

    public static string FaqUrlVerification
    {
      get => WaWebUrls.GetFaqUrl("/link/verification.php", WaWebUrls.GetDefaultQuery(true));
    }

    public static string SecurityUrl
    {
      get => WaWebUrls.AppendDefaultQuery("https://www.whatsapp.com/security/", false);
    }

    private static string GdprTosUrlLegal
    {
      get
      {
        Dictionary<string, string> defaultQuery = WaWebUrls.GetDefaultQuery(false);
        defaultQuery["eea"] = "1";
        return WaWebUrls.AppendQuery("https://www.whatsapp.com/legal/", defaultQuery);
      }
    }

    public static string GdprTosUrlTerms
    {
      get => string.Format("{0}#terms-of-service", (object) WaWebUrls.GdprTosUrlLegal);
    }

    public static string GdprTosUrlPrivacyPolicy
    {
      get => string.Format("{0}#privacy-policy", (object) WaWebUrls.GdprTosUrlLegal);
    }

    public static string GdprTosUrlAge
    {
      get => string.Format("{0}#terms-of-service-age", (object) WaWebUrls.GdprTosUrlLegal);
    }

    public static string GdprTosUrlManageAndDeleteInfo
    {
      get
      {
        return string.Format("{0}#privacy-policy-managing-and-deleting-your-information", (object) WaWebUrls.GdprTosUrlLegal);
      }
    }

    public static string GdprTosUrlGlobalOp
    {
      get
      {
        return string.Format("{0}#privacy-policy-our-global-operations", (object) WaWebUrls.GdprTosUrlLegal);
      }
    }

    public static string GdprTosUrlCookies
    {
      get => string.Format("{0}#cookies", (object) WaWebUrls.GdprTosUrlLegal);
    }

    public static string GdprTosUrlGroupE2e
    {
      get => WaWebUrls.GetFaqUrlGeneral("28030015").Replace("eea=0", "eea=1");
    }

    public static string GdprTosUrlWorkingWithFb
    {
      get => WaWebUrls.GetFaqUrlGeneral("26000112").Replace("eea=0", "eea=1");
    }

    public static string FbUrlFacebookCompanies => "https://www.facebook.com/help/111814505650678";

    private static string AppendQuery(string url, Dictionary<string, string> queryDict)
    {
      if (queryDict != null && queryDict.Any<KeyValuePair<string, string>>())
      {
        string str = string.Join("&", queryDict.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => string.Format("{0}={1}", (object) Uri.EscapeUriString(kv.Key), (object) Uri.EscapeUriString(kv.Value)))));
        url = string.Format("{0}{1}{2}", (object) url, url.EndsWith("?") ? (object) "" : (object) "?", (object) str);
      }
      return url;
    }

    private static string AppendDefaultQuery(string url, bool includePlatform)
    {
      return WaWebUrls.AppendQuery(url, WaWebUrls.GetDefaultQuery(includePlatform));
    }

    private static Dictionary<string, string> GetDefaultQuery(bool includePlatform)
    {
      string lang = (string) null;
      string locale = (string) null;
      AppState.GetLangAndLocale(out lang, out locale);
      Dictionary<string, string> defaultQuery = new Dictionary<string, string>();
      if (lang != null)
        defaultQuery["lg"] = lang;
      if (locale != null)
        defaultQuery["lc"] = locale;
      if (includePlatform)
        defaultQuery["platform"] = "wp";
      defaultQuery["eea"] = GdprTos.IsEEA() ? "1" : "0";
      return defaultQuery;
    }

    public static string GetFaqSearchUrl(string searchTerm, bool includeDescription)
    {
      Dictionary<string, string> defaultQuery = WaWebUrls.GetDefaultQuery(true);
      defaultQuery["query"] = searchTerm;
      if (!includeDescription)
        defaultQuery["lite"] = "1";
      return WaWebUrls.GetFaqUrl("/client_search.php", defaultQuery);
    }

    public static string GetFaqUrl(string suffix, Dictionary<string, string> queryDict)
    {
      if (suffix == null)
        suffix = "";
      else if (suffix[0] != '/')
        suffix = string.Format("/{0}", (object) suffix);
      return WaWebUrls.AppendQuery(string.Format("{0}{1}", (object) "https://faq.whatsapp.com", (object) suffix), queryDict);
    }

    public static string GetFaqUrlGeneral(string articleNumber)
    {
      return WaWebUrls.GetFaqUrlImpl("general", articleNumber, false);
    }

    public static string GetFaqUrlWp(string articleNumber)
    {
      return WaWebUrls.GetFaqUrlImpl("wp", articleNumber, false);
    }

    private static string GetFaqUrlImpl(string group, string articleNumber, bool includePlatform)
    {
      return WaWebUrls.GetFaqUrl(string.Format("/{0}/{1}/", (object) group, (object) articleNumber), WaWebUrls.GetDefaultQuery(includePlatform));
    }
  }
}
