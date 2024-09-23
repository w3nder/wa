// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ScanFilter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath
{
  internal class ScanFilter : PathFilter
  {
    public string Name { get; set; }

    public override IEnumerable<JToken> ExecuteFilter(
      IEnumerable<JToken> current,
      bool errorWhenNoMatch)
    {
      using (IEnumerator<JToken> enumerator = current.GetEnumerator())
      {
label_17:
        while (enumerator.MoveNext())
        {
          JToken root = enumerator.Current;
          if (this.Name == null)
            yield return root;
          JToken value = root;
          JToken container = root;
          while (true)
          {
            if (container != null && container.HasValues)
            {
              value = container.First;
            }
            else
            {
              while (value != null && value != root && value == value.Parent.Last)
                value = (JToken) value.Parent;
              if (value != null && value != root)
                value = value.Next;
              else
                goto label_17;
            }
            if (value is JProperty e)
            {
              if (e.Name == this.Name)
                yield return e.Value;
            }
            else if (this.Name == null)
              yield return value;
            container = (JToken) (value as JContainer);
          }
        }
      }
    }
  }
}
