// Decompiled with JetBrains decompiler
// Type: WhatsApp.CharGroupings
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Globalization;


namespace WhatsApp
{
  public class CharGroupings
  {
    public const string Globe = "\uD83C\uDF10";
    private static CharGroupingImpl instance;

    public static CharGroupingImpl Instance
    {
      get => CharGroupings.instance ?? (CharGroupings.instance = CharGroupings.Create());
    }

    public static CharGroupingImpl Create(CultureInfo culture = null)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string lang;
      string locale;
      culture.GetLangAndLocale(out lang, out locale);
      lang = lang.ToLower();
      string lower = locale.ToLower();
      switch (lang)
      {
        case "es":
        case "sr":
        case "hr":
        case "hu":
          return (CharGroupingImpl) new CharGroupingsLegacy(culture);
        case "zh":
          if (!(lower == "tw"))
            break;
          goto case "es";
      }
      return (CharGroupingImpl) new CharGroupingFromSystem(culture);
    }
  }
}
