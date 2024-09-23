// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegexSearch
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Linq;
using WhatsApp.RegularExpressions;

#nullable disable
namespace WhatsApp
{
  public class RegexSearch : SearchMethodBase
  {
    public override IEnumerable<SearchMethodBase.Match> LookupImpl(string term)
    {
      return this.LookupWithRegex(new Regex(term, RegexOptions.IgnoreCase));
    }

    public override IEnumerable<SearchMethodBase.Match> LookupImpl(string[] keywords)
    {
      return keywords != null && ((IEnumerable<string>) keywords).Count<string>() > 0 ? this.LookupWithRegex(new Regex(string.Join("\\s+", keywords), RegexOptions.IgnoreCase)) : (IEnumerable<SearchMethodBase.Match>) new SearchMethodBase.Match[0];
    }

    private IEnumerable<SearchMethodBase.Match> LookupWithRegex(Regex regex)
    {
      LinkedList<SearchMethodBase.Match> linkedList = new LinkedList<SearchMethodBase.Match>();
      lock (this.accessLock_)
      {
        foreach (SearchMethodBase.Node node in this.nodes_)
        {
          WhatsApp.RegularExpressions.Match match = regex.Match(node.Text);
          if (match.Success)
            linkedList.AddLast(new SearchMethodBase.Match()
            {
              Tag = node.Tag,
              Start = match.Index,
              End = match.Index + match.Length
            });
        }
      }
      return (IEnumerable<SearchMethodBase.Match>) linkedList;
    }
  }
}
