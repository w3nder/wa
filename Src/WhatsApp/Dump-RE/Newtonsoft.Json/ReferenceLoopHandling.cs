// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.ReferenceLoopHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies reference loop handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum ReferenceLoopHandling
  {
    /// <summary>
    /// Throw a <see cref="T:Newtonsoft.Json.JsonSerializationException" /> when a loop is encountered.
    /// </summary>
    Error,
    /// <summary>Ignore loop references and do not serialize.</summary>
    Ignore,
    /// <summary>Serialize loop references.</summary>
    Serialize,
  }
}
