// Decompiled with JetBrains decompiler
// Type: WhatsApp.UriUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public static class UriUtils
  {
    private static string xamlExtension = ".xaml";

    private static void AddFormat(StringBuilder fmtSb, List<object> fmtArgs, object o)
    {
      fmtSb.Append('{');
      fmtSb.Append(fmtArgs.Count.ToString());
      fmtSb.Append('}');
      fmtArgs.Add(o);
    }

    public static string CreatePageUriStr(string pageName, string parameters, string folderName = "Pages")
    {
      List<object> fmtArgs = new List<object>();
      StringBuilder fmtSb = new StringBuilder();
      if (folderName != null)
      {
        fmtSb.Append('/');
        UriUtils.AddFormat(fmtSb, fmtArgs, (object) folderName);
      }
      if (!pageName.EndsWith(UriUtils.xamlExtension))
        pageName += UriUtils.xamlExtension;
      fmtSb.Append('/');
      UriUtils.AddFormat(fmtSb, fmtArgs, (object) pageName);
      if (parameters != null && !string.IsNullOrEmpty(parameters.Trim()))
      {
        fmtSb.Append('?');
        UriUtils.AddFormat(fmtSb, fmtArgs, (object) parameters);
      }
      return string.Format(fmtSb.ToString(), fmtArgs.ToArray());
    }

    public static string CreatePageUriStr(
      string pageName,
      WaUriParams uriParams = null,
      string folderName = "Pages")
    {
      return UriUtils.CreatePageUriStr(pageName, uriParams == null ? "" : uriParams.ToUriString(), folderName);
    }

    public static Uri CreatePageUri(
      string pageName,
      WaUriParams uriParams,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      return new Uri(UriUtils.CreatePageUriStr(pageName, uriParams, folderName), uriKind);
    }

    public static Uri CreatePageUri(
      string pageName,
      string parameters = null,
      string folderName = "Pages",
      UriKind uriKind = UriKind.Relative)
    {
      return new Uri(UriUtils.CreatePageUriStr(pageName, parameters, folderName), uriKind);
    }

    public static Dictionary<string, string> ParsePageParams(Uri uri)
    {
      int num = uri.OriginalString.IndexOf('?');
      return UriUtils.ParsePageParams(num < 0 ? "" : uri.OriginalString.Substring(num + 1));
    }

    public static Dictionary<string, string> ParsePageParams(string paramsStr)
    {
      Dictionary<string, string> pageParams = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(paramsStr))
      {
        if (paramsStr.StartsWith("?"))
          paramsStr = paramsStr.Substring(1);
        string str1 = paramsStr;
        char[] chArray = new char[1]{ '&' };
        foreach (string str2 in str1.Split(chArray))
        {
          int length = str2.IndexOf('=');
          string url1;
          string url2;
          if (length < 0)
          {
            url1 = str2;
            url2 = "1";
          }
          else
          {
            url1 = str2.Substring(0, length);
            url2 = str2.Substring(length + 1);
          }
          pageParams[HttpUtility.UrlDecode(url1)] = HttpUtility.UrlDecode(url2);
        }
      }
      return pageParams;
    }

    public static string AppendLanguageAndLocaleToUrl(string baseUrl)
    {
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      return baseUrl + "?lg=" + lang + "&lc=" + locale;
    }

    public static string ExtractPageNameFromUrl(string pageUrl)
    {
      if (string.IsNullOrEmpty(pageUrl))
        return (string) null;
      int length = pageUrl.IndexOf(UriUtils.xamlExtension);
      if (length <= 0)
        return (string) null;
      string str = pageUrl.Substring(0, length);
      int startIndex = str.LastIndexOf('/');
      return startIndex <= 0 ? (string) null : str.Substring(startIndex);
    }
  }
}
