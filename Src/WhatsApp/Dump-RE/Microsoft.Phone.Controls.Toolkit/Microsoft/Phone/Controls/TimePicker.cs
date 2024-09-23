// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TimePicker
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TimePicker : DateTimePickerBase
  {
    private string _fallbackValueStringFormat;

    public TimePicker()
    {
      this.DefaultStyleKey = (object) typeof (TimePicker);
      this.Value = new DateTime?(DateTime.Now);
    }

    protected override string ValueStringFormatFallback
    {
      get
      {
        if (this._fallbackValueStringFormat == null)
        {
          string str = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Replace(":ss", "");
          string letterIsoLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
          this._fallbackValueStringFormat = "{0:" + (letterIsoLanguageName == "ar" || letterIsoLanguageName == "fa" ? "\u200F" + str : "\u200E" + str) + "}";
        }
        return this._fallbackValueStringFormat;
      }
    }
  }
}
