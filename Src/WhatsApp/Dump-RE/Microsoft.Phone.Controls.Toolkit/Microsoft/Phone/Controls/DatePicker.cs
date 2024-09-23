// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DatePicker
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class DatePicker : DateTimePickerBase
  {
    private string _fallbackValueStringFormat;

    public DatePicker()
    {
      this.DefaultStyleKey = (object) typeof (DatePicker);
      this.Value = new DateTime?(DateTime.Now.Date);
    }

    protected override string ValueStringFormatFallback
    {
      get
      {
        if (this._fallbackValueStringFormat == null)
        {
          string str = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
          if (DateTimePickerBase.DateShouldFlowRTL())
          {
            char[] charArray = str.ToCharArray();
            Array.Reverse((Array) charArray);
            str = new string(charArray);
          }
          this._fallbackValueStringFormat = "{0:" + str + "}";
        }
        return this._fallbackValueStringFormat;
      }
    }
  }
}
