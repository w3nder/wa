// Decompiled with JetBrains decompiler
// Type: WhatsApp.Plurals
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace WhatsApp
{
  public class Plurals
  {
    private IEnumerable<Func<int, bool>> PluralRules;
    private static Plurals instance;

    private void Return(params Func<int, bool>[] a)
    {
      this.PluralRules = (IEnumerable<Func<int, bool>>) a;
    }

    private bool InRange(int value, int low, int high) => value >= low && value <= high;

    private bool WithinRange(int value, int low, int high) => value > low && value < high;

    public Plurals(CultureInfo ci)
    {
      string lang;
      ci.GetLangAndLocale(out lang, out string _);
      this.CreateRules(lang);
    }

    public Plurals(string lg) => this.CreateRules(lg);

    private void CreateRules(string lg)
    {
      char[] chArray = new char[1]{ ' ' };
      if (((IEnumerable<string>) "az bm fa ig hu ja kde kea ko my ses sg to tr vi wo yo zh bo dz id jv ka km kn ms th".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => true));
      else if (lg == "ar")
        this.Return((Func<int, bool>) (n => n == 0), (Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => n == 2), (Func<int, bool>) (n => this.InRange(n % 100, 3, 10)), (Func<int, bool>) (n => this.InRange(n % 100, 11, 99)));
      else if (((IEnumerable<string>) "bem brx da de el en eo es et fi fo gl he iw it nb nl nn no pt_PT sv af bg bn ca eu fur fy gu ha is ku lb ml mr nah ne om or pa pap ps so sq sw ta te tk ur zu mn gsw chr rm pt".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => n == 1));
      else if (((IEnumerable<string>) "ak am bh fil tl guw hi ln mg nso ti wa".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => n == 0 || n == 1));
      else if (((IEnumerable<string>) "ff fr kab".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => this.WithinRange(n, 0, 2) && n != 2));
      else if (lg == "lv")
        this.Return((Func<int, bool>) (n => n == 0), (Func<int, bool>) (n => n % 10 == 1 && n % 100 != 11));
      else if (((IEnumerable<string>) "ga se sma smi smj smn sms".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => n == 2));
      else if (((IEnumerable<string>) "ro mo".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => n == 1), (Func<int, bool>) (n =>
        {
          if (n == 0)
            return true;
          return n != 1 && this.InRange(n % 100, 1, 19);
        }));
      else if (lg == "lt")
        this.Return((Func<int, bool>) (n => n % 10 == 1 && !this.InRange(n % 100, 11, 19)), (Func<int, bool>) (n => this.InRange(n % 10, 2, 9) && !this.InRange(n % 100, 11, 19)));
      else if (((IEnumerable<string>) "hr ru sr uk be bs sh".Split(chArray)).Contains<string>(lg))
        this.Return((Func<int, bool>) (n => n % 10 == 1 && n % 100 != 11), (Func<int, bool>) (n => this.InRange(n % 10, 2, 4) && !this.InRange(n % 100, 12, 14)), (Func<int, bool>) (n => n % 10 == 0 || this.InRange(n % 10, 5, 9) || this.InRange(n % 100, 11, 14)));
      else if (((IEnumerable<string>) "cs sk".Split(chArray)).Contains<string>(lg))
      {
        this.Return((Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => this.InRange(n, 2, 4)));
      }
      else
      {
        switch (lg)
        {
          case "pl":
            this.Return((Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => this.InRange(n % 100, 2, 4) && !this.InRange(n % 100, 12, 14)), (Func<int, bool>) (n =>
            {
              if (n == 1)
                return false;
              return this.InRange(n % 10, 0, 1) || this.InRange(n % 10, 5, 9) || this.InRange(n % 100, 12, 14);
            }));
            break;
          case "sl":
            this.Return((Func<int, bool>) (n => n % 100 == 1), (Func<int, bool>) (n => n % 100 == 2), (Func<int, bool>) (n => this.InRange(n % 100, 3, 4)));
            break;
          case "mt":
            this.Return((Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => n == 0 || this.InRange(n % 100, 2, 10)), (Func<int, bool>) (n => this.InRange(n % 100, 11, 19)));
            break;
          case "mk":
            this.Return((Func<int, bool>) (n => n % 10 == 1 && n != 11));
            break;
          case "cy":
            this.Return((Func<int, bool>) (n => n == 0), (Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => n == 2), (Func<int, bool>) (n => n == 3), (Func<int, bool>) (n => n == 6));
            break;
          case "lag":
            this.Return((Func<int, bool>) (n => n == 0), (Func<int, bool>) (n => this.WithinRange(n, 0, 2) && n != 0 && n != 2));
            break;
          case "shi":
            this.Return((Func<int, bool>) (n => this.WithinRange(n, 0, 1)), (Func<int, bool>) (n => this.InRange(n, 2, 10)));
            break;
          case "br":
            this.Return((Func<int, bool>) (n => n == 0), (Func<int, bool>) (n => n == 1), (Func<int, bool>) (n => n == 2), (Func<int, bool>) (n => n == 3), (Func<int, bool>) (n => n == 6));
            break;
          default:
            this.Return((Func<int, bool>) (n => true));
            break;
        }
      }
    }

    public static Plurals Instance
    {
      get
      {
        return Plurals.instance ?? (Plurals.instance = new Plurals(new CultureInfo(AppResources.CultureString)));
      }
    }

    public static void ResetInstance() => Plurals.instance = (Plurals) null;

    public int GetPluralCategory(int number)
    {
      int pluralCategory = 0;
      foreach (Func<int, bool> pluralRule in this.PluralRules)
      {
        if (pluralRule(number))
          return pluralCategory;
        ++pluralCategory;
      }
      return pluralCategory;
    }

    public string GetStringWithIndex(string locString, int index, params object[] objs)
    {
      string[] locArray = locString.ParseLocArray();
      if (locArray.Length == 0)
        return locString;
      int index1 = this.GetPluralCategory((int) objs[index]);
      if (index1 >= locArray.Length)
        index1 = locArray.Length - 1;
      return Bidi.Format(locArray[index1], ((IEnumerable<object>) objs).Select<object, string>((Func<object, string>) (o => o is string str ? str : o.ToString())).ToArray<string>());
    }

    public string GetString(string locString, int number)
    {
      return this.GetStringWithIndex(locString, 0, (object) number);
    }
  }
}
