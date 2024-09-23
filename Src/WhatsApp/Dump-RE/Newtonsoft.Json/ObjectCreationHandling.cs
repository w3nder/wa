// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.ObjectCreationHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how object creation is handled by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum ObjectCreationHandling
  {
    /// <summary>
    /// Reuse existing objects, create new objects when needed.
    /// </summary>
    Auto,
    /// <summary>Only reuse existing objects.</summary>
    Reuse,
    /// <summary>Always create new objects.</summary>
    Replace,
  }
}
