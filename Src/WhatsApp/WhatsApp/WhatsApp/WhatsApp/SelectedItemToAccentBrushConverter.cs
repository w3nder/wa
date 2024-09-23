// Decompiled with JetBrains decompiler
// Type: WhatsApp.SelectedItemToAccentBrushConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace WhatsApp
{
  public class SelectedItemToAccentBrushConverter : IValueConverter
  {
    public Func<object, bool> IsSelected = (Func<object, bool>) (o => false);
    public Brush DefaultBrush;

    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      if (this.IsSelected(value))
        return Application.Current.Resources[(object) "PhoneAccentBrush"];
      if (this.DefaultBrush == null)
        this.DefaultBrush = (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Colors.White : Colors.Black);
      return (object) this.DefaultBrush;
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
