// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.MetadataPropertyHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies metadata property handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum MetadataPropertyHandling
  {
    /// <summary>
    /// Read metadata properties located at the start of a JSON object.
    /// </summary>
    Default,
    /// <summary>
    /// Read metadata properties located anywhere in a JSON object. Note that this setting will impact performance.
    /// </summary>
    ReadAhead,
    /// <summary>Do not try to read metadata properties.</summary>
    Ignore,
  }
}
