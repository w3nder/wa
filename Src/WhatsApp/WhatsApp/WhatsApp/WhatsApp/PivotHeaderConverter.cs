// Decompiled with JetBrains decompiler
// Type: WhatsApp.PivotHeaderConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.Windows.Data;


namespace WhatsApp
{
  public class PivotHeaderConverter : IValueConverter
  {
    private bool? active;

    public string Convert(string header)
    {
      return this.Convert((object) header, (System.Type) null, (object) null, (CultureInfo) null) as string;
    }

    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      if (!this.active.HasValue)
      {
        string lang;
        CultureInfo.CurrentUICulture.GetLangAndLocale(out lang, out string _);
        this.active = new bool?(lang.ToLowerInvariant() == "zh");
      }
      if (this.active.Value && value is string str)
        value = (object) (str + " ");
      return value;
    }

    public object ConvertBack(
      object value,
      System.Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
