// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UrlHelper
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.Net;

#nullable disable
namespace Microsoft.Graph
{
  public static class UrlHelper
  {
    public static IDictionary<string, string> GetQueryOptions(Uri resultUri)
    {
      string[] strArray1 = (string[]) null;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      int num = resultUri.AbsoluteUri.IndexOf("#", StringComparison.Ordinal);
      if (num > 0 && num < resultUri.AbsoluteUri.Length + 1)
        strArray1 = resultUri.AbsoluteUri.Substring(num + 1).Split('&');
      else if (num < 0 && !string.IsNullOrEmpty(resultUri.Query))
        strArray1 = resultUri.Query.TrimStart('?').Split('&');
      if (strArray1 != null)
      {
        foreach (string str in strArray1)
        {
          if (!string.IsNullOrEmpty(str))
          {
            string[] strArray2 = str.Split('=');
            queryOptions.Add(strArray2[0], WebUtility.UrlDecode(strArray2[1]));
          }
        }
      }
      return (IDictionary<string, string>) queryOptions;
    }
  }
}
