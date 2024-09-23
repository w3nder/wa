// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.FloatParseHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
  /// </summary>
  public enum FloatParseHandling
  {
    /// <summary>
    /// Floating point numbers are parsed to <see cref="F:Newtonsoft.Json.FloatParseHandling.Double" />.
    /// </summary>
    Double,
    /// <summary>
    /// Floating point numbers are parsed to <see cref="F:Newtonsoft.Json.FloatParseHandling.Decimal" />.
    /// </summary>
    Decimal,
  }
}
