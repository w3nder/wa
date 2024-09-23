// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.ConstructorHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies how constructors are used when initializing objects during deserialization by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum ConstructorHandling
  {
    /// <summary>
    /// First attempt to use the public default constructor, then fall back to single paramatized constructor, then the non-public default constructor.
    /// </summary>
    Default,
    /// <summary>
    /// Json.NET will use a non-public default constructor before falling back to a paramatized constructor.
    /// </summary>
    AllowNonPublicDefaultConstructor,
  }
}
