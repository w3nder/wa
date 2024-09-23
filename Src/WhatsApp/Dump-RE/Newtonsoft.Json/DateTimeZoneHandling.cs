// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.DateTimeZoneHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how to treat the time value when converting between string and <see cref="T:System.DateTime" />.
  /// </summary>
  public enum DateTimeZoneHandling
  {
    /// <summary>
    /// Treat as local time. If the <see cref="T:System.DateTime" /> object represents a Coordinated Universal Time (UTC), it is converted to the local time.
    /// </summary>
    Local,
    /// <summary>
    /// Treat as a UTC. If the <see cref="T:System.DateTime" /> object represents a local time, it is converted to a UTC.
    /// </summary>
    Utc,
    /// <summary>
    /// Treat as a local time if a <see cref="T:System.DateTime" /> is being converted to a string.
    /// If a string is being converted to <see cref="T:System.DateTime" />, convert to a local time if a time zone is specified.
    /// </summary>
    Unspecified,
    /// <summary>
    /// Time zone information should be preserved when converting.
    /// </summary>
    RoundtripKind,
  }
}
