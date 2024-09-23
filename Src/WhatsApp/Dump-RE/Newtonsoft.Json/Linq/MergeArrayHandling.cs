// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.MergeArrayHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>Specifies how JSON arrays are merged together.</summary>
  public enum MergeArrayHandling
  {
    /// <summary>Concatenate arrays.</summary>
    Concat,
    /// <summary>Union arrays, skipping items that already exist.</summary>
    Union,
    /// <summary>Replace all array items.</summary>
    Replace,
    /// <summary>Merge array items together, matched by index.</summary>
    Merge,
  }
}
