// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.DateParseHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON text.
  /// </summary>
  public enum DateParseHandling
  {
    /// <summary>
    /// Date formatted strings are not parsed to a date type and are read as strings.
    /// </summary>
    None,
    /// <summary>
    /// Date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed to <see cref="F:Newtonsoft.Json.DateParseHandling.DateTime" />.
    /// </summary>
    DateTime,
    /// <summary>
    /// Date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed to <see cref="F:Newtonsoft.Json.DateParseHandling.DateTimeOffset" />.
    /// </summary>
    DateTimeOffset,
  }
}
