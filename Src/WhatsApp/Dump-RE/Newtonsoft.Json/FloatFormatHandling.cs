// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.FloatFormatHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies float format handling options when writing special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
  /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" /> with <see cref="T:Newtonsoft.Json.JsonWriter" />.
  /// </summary>
  public enum FloatFormatHandling
  {
    /// <summary>
    /// Write special floating point values as strings in JSON, e.g. "NaN", "Infinity", "-Infinity".
    /// </summary>
    String,
    /// <summary>
    /// Write special floating point values as symbols in JSON, e.g. NaN, Infinity, -Infinity.
    /// Note that this will produce non-valid JSON.
    /// </summary>
    Symbol,
    /// <summary>
    /// Write special floating point values as the property's default value in JSON, e.g. 0.0 for a <see cref="T:System.Double" /> property, null for a <see cref="T:System.Nullable`1" /> property.
    /// </summary>
    DefaultValue,
  }
}
