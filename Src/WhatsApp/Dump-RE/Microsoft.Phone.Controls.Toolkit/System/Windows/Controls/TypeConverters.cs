// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.TypeConverters
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Properties;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace System.Windows.Controls
{
  internal static class TypeConverters
  {
    internal static bool CanConvertTo<T>(Type destinationType)
    {
      if (destinationType == null)
        throw new ArgumentNullException(nameof (destinationType));
      return destinationType == typeof (string) || destinationType.IsAssignableFrom(typeof (T));
    }

    internal static object ConvertTo(TypeConverter converter, object value, Type destinationType)
    {
      if (destinationType == null)
        throw new ArgumentNullException(nameof (destinationType));
      if (value == null && !destinationType.IsValueType)
        return (object) null;
      return value != null && destinationType.IsAssignableFrom(value.GetType()) ? value : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypeConverters_Convert_CannotConvert, (object) converter.GetType().Name, value != null ? (object) value.GetType().FullName : (object) "(null)", (object) destinationType.GetType().Name));
    }
  }
}
