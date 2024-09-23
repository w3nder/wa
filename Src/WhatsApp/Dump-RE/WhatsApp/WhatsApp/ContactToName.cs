// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactToName
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#nullable disable
namespace WhatsApp
{
  public class ContactToName : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      string source = (string) null;
      if (value is Contact contact)
      {
        source = contact.DisplayName;
        if (source != null && UIUtils.IsRightToLeft() && !source.Any<char>((Func<char, bool>) (c => char.IsLetter(c))))
          value = (object) ("\u202A" + source + "\u202C");
      }
      return (object) source;
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
