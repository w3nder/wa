// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.IJsonLineInfo
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Provides an interface to enable a class to return line and position information.
  /// </summary>
  public interface IJsonLineInfo
  {
    /// <summary>
    /// Gets a value indicating whether the class can return line information.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if LineNumber and LinePosition can be provided; otherwise, <c>false</c>.
    /// </returns>
    bool HasLineInfo();

    /// <summary>Gets the current line number.</summary>
    /// <value>The current line number or 0 if no line information is available (for example, HasLineInfo returns false).</value>
    int LineNumber { get; }

    /// <summary>Gets the current line position.</summary>
    /// <value>The current line position or 0 if no line information is available (for example, HasLineInfo returns false).</value>
    int LinePosition { get; }
  }
}
