// Decompiled with JetBrains decompiler
// Type: WhatsApp.BidiLanguageExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;


namespace WhatsApp
{
  public static class BidiLanguageExtensions
  {
    private static string[] RtlLocales = new string[4]
    {
      "ar",
      "he",
      "fa",
      "ur"
    };

    public static bool IsRightToLeft(this CultureInfo cult)
    {
      string lang;
      cult.GetLangAndLocale(out lang, out string _);
      return ((IEnumerable<string>) BidiLanguageExtensions.RtlLocales).Contains<string>(lang);
    }

    public static void AdjustFlowDirection(this InlineCollection inlines)
    {
      if (!new CultureInfo(AppResources.CultureString).IsRightToLeft())
        return;
      foreach (Inline inline in (PresentationFrameworkCollection<Inline>) inlines)
      {
        if (inline is Run run && !BidiLanguageExtensions.HasRtlChars(run.Text))
          run.FlowDirection = FlowDirection.LeftToRight;
      }
    }

    public static bool HasRtlChars(string str)
    {
      for (int index = 0; index < str.Length; ++index)
      {
        if (Bidi.GetCharCategory(str[index]) == Bidi.CharClass.StrongRtl)
          return true;
      }
      return false;
    }

    public static FlowDirection? getFlowDirection(string str)
    {
      if (str != null)
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (Bidi.GetCharCategory(str[index]) == Bidi.CharClass.StrongRtl)
            return new FlowDirection?(FlowDirection.RightToLeft);
          if (Bidi.GetCharCategory(str[index]) == Bidi.CharClass.StrongLtr)
            return new FlowDirection?(FlowDirection.LeftToRight);
        }
      }
      return new FlowDirection?(new CultureInfo(AppResources.CultureString).IsRightToLeft() ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
    }
  }
}
