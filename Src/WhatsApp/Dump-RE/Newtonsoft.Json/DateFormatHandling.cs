// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.DateFormatHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how dates are formatted when writing JSON text.
  /// </summary>
  public enum DateFormatHandling
  {
    /// <summary>
    /// Dates are written in the ISO 8601 format, e.g. "2012-03-21T05:40Z".
    /// </summary>
    IsoDateFormat,
    /// <summary>
    /// Dates are written in the Microsoft JSON format, e.g. "\/Date(1198908717056)\/".
    /// </summary>
    MicrosoftDateFormat,
  }
}
