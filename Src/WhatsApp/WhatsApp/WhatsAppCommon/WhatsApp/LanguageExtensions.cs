// Decompiled with JetBrains decompiler
// Type: WhatsApp.LanguageExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Globalization;


namespace WhatsApp
{
  public static class LanguageExtensions
  {
    public static void GetLangAndLocale(this CultureInfo that, out string lang, out string locale)
    {
      string name = that.Name;
      int length = name.IndexOf('-');
      if (length > 0)
      {
        int num = name.LastIndexOf('-');
        lang = name.Substring(0, length);
        locale = name.Substring(num + 1);
      }
      else
      {
        lang = name;
        string[] strArray = new string[18]
        {
          "ar",
          "SA",
          "cs",
          "CZ",
          "da",
          "DK",
          "el",
          "GR",
          "he",
          "IL",
          "ja",
          "JP",
          "ko",
          "KR",
          "sv",
          "SE",
          "sr",
          "RS"
        };
        locale = lang.ToUpper();
        for (int index = 0; index + 1 < strArray.Length; index += 2)
        {
          if (lang == strArray[index])
          {
            locale = strArray[index + 1];
            break;
          }
        }
      }
      if (!(locale == "029"))
        return;
      locale = "US";
    }

    public static bool IsVariationSelector(this char ch) => ch >= '︀' && ch <= '️';
  }
}
