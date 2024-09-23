// Decompiled with JetBrains decompiler
// Type: WhatsApp.ToLowerConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace WhatsApp
{
  public class ToLowerConverter : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string str)
        value = (object) str.ToLangFriendlyLower();
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
