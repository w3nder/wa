// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.MissingMemberHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies missing member handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum MissingMemberHandling
  {
    /// <summary>
    /// Ignore a missing member and do not attempt to deserialize it.
    /// </summary>
    Ignore,
    /// <summary>
    /// Throw a <see cref="T:Newtonsoft.Json.JsonSerializationException" /> when a missing member is encountered during deserialization.
    /// </summary>
    Error,
  }
}
