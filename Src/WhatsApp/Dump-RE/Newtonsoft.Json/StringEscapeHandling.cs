// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.StringEscapeHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how strings are escaped when writing JSON text.
  /// </summary>
  public enum StringEscapeHandling
  {
    /// <summary>Only control characters (e.g. newline) are escaped.</summary>
    Default,
    /// <summary>
    /// All non-ASCII and control characters (e.g. newline) are escaped.
    /// </summary>
    EscapeNonAscii,
    /// <summary>
    /// HTML (&lt;, &gt;, &amp;, ', ") and control characters (e.g. newline) are escaped.
    /// </summary>
    EscapeHtml,
  }
}
