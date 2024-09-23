// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.StringHelper
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Microsoft.Graph
{
  public static class StringHelper
  {
    public static string ConvertTypeToTitleCase(string typeString)
    {
      if (string.IsNullOrEmpty(typeString))
        return typeString;
      return string.Join(".", ((IEnumerable<string>) typeString.Split('.')).Select<string, string>((Func<string, string>) (segment => segment.Substring(0, 1).ToUpperInvariant() + segment.Substring(1))));
    }

    public static string ConvertTypeToLowerCamelCase(string typeString)
    {
      if (string.IsNullOrEmpty(typeString))
        return typeString;
      return string.Join(".", ((IEnumerable<string>) typeString.Split('.')).Select<string, string>((Func<string, string>) (segment => segment.Substring(0, 1).ToLowerInvariant() + segment.Substring(1))));
    }
  }
}
