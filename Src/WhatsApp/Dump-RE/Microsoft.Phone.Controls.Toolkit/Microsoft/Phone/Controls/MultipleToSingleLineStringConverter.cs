// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MultipleToSingleLineStringConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class MultipleToSingleLineStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string str = value as string;
      return string.IsNullOrEmpty(str) ? (object) string.Empty : (object) str.Replace(Environment.NewLine, " ");
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
