// Decompiled with JetBrains decompiler
// Type: WhatsApp.DistanceToStringConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace WhatsApp
{
  public class DistanceToStringConverter : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      double number = (double) value;
      string str;
      if (RegionInfo.CurrentRegion.IsMetric)
      {
        str = number <= 900.0 ? Plurals.Instance.GetString(AppResources.MetersPlural, (int) number) : Plurals.Instance.GetString(AppResources.KilometersPlural, (int) number / 1000);
      }
      else
      {
        double num1 = number / 0.30480061;
        double num2 = num1 / 5280.0;
        str = num2 <= 0.9 ? (num2 <= 0.2 ? Plurals.Instance.GetString(AppResources.FeetPlural, System.Convert.ToInt32(num1)) : string.Format("{0} {1}", num2 > 0.6 ? (object) "3/4" : (num2 > 0.35 ? (object) "1/2" : (object) "1/4"), (object) AppResources.MilesFraction)) : Plurals.Instance.GetString(AppResources.MilesPlural, System.Convert.ToInt32(num2));
      }
      return (object) str;
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
