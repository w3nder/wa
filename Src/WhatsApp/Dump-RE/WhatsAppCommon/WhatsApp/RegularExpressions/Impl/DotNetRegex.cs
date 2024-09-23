// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegularExpressions.Impl.DotNetRegex
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;

#nullable disable
namespace WhatsApp.RegularExpressions.Impl
{
  public class DotNetRegex
  {
    private System.Text.RegularExpressions.Regex regex;

    public DotNetRegex(string expr, WhatsApp.RegularExpressions.RegexOptions options)
    {
      this.regex = new System.Text.RegularExpressions.Regex(expr, (System.Text.RegularExpressions.RegexOptions) options);
    }

    public WhatsApp.RegularExpressions.Match Match(string input, int offset, int length)
    {
      System.Text.RegularExpressions.Match match1 = this.regex.Match(input, offset, length);
      if (match1 == null)
        return (WhatsApp.RegularExpressions.Match) null;
      WhatsApp.RegularExpressions.Match match2 = new WhatsApp.RegularExpressions.Match()
      {
        Success = match1.Success
      };
      if (match1.Success)
      {
        match2.Index = match1.Index;
        match2.Length = match1.Length;
        match2.Value = match1.Value;
        if (match1.Groups != null)
        {
          List<WhatsApp.RegularExpressions.Group> groupList = new List<WhatsApp.RegularExpressions.Group>();
          for (int groupnum = 0; groupnum < match1.Groups.Count; ++groupnum)
            groupList.Add(new WhatsApp.RegularExpressions.Group()
            {
              Value = match1.Groups[groupnum].Value
            });
          match2.Groups = groupList.ToArray();
        }
      }
      return match2;
    }
  }
}
