// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonMergeSettings
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>Specifies the settings used when merging JSON.</summary>
  public class JsonMergeSettings
  {
    private MergeArrayHandling _mergeArrayHandling;

    /// <summary>
    /// Gets or sets the method used when merging JSON arrays.
    /// </summary>
    /// <value>The method used when merging JSON arrays.</value>
    public MergeArrayHandling MergeArrayHandling
    {
      get => this._mergeArrayHandling;
      set
      {
        this._mergeArrayHandling = value >= MergeArrayHandling.Concat && value <= MergeArrayHandling.Merge ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }
  }
}
