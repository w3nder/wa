// Decompiled with JetBrains decompiler
// Type: WhatsApp.FormatInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.RegularExpressions;


namespace WhatsApp
{
  public class FormatInfo
  {
    private Regex numberPattern_;
    private Regex[] leadingDigitsPatterns_;
    private string numberFormat_;

    public FormatInfo(string pattern, string format, string[] leadingDigitsPatterns = null)
    {
      this.numberPattern_ = new Regex(pattern);
      this.numberFormat_ = format;
      this.leadingDigitsPatterns_ = leadingDigitsPatterns == null ? new Regex[0] : ((IEnumerable<string>) leadingDigitsPatterns).Select<string, Regex>((Func<string, Regex>) (p => new Regex(p))).ToArray<Regex>();
    }

    public Regex NumberPattern => this.numberPattern_;

    public string NumberFormat => this.numberFormat_;

    public Regex[] LeadingDigitsPatterns => this.leadingDigitsPatterns_;
  }
}
