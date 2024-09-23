// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.TypeNameHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies type name handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  [Flags]
  public enum TypeNameHandling
  {
    /// <summary>
    /// Do not include the .NET type name when serializing types.
    /// </summary>
    None = 0,
    /// <summary>
    /// Include the .NET type name when serializing into a JSON object structure.
    /// </summary>
    Objects = 1,
    /// <summary>
    /// Include the .NET type name when serializing into a JSON array structure.
    /// </summary>
    Arrays = 2,
    /// <summary>Always include the .NET type name when serializing.</summary>
    All = Arrays | Objects, // 0x00000003
    /// <summary>
    /// Include the .NET type name when the type of the object being serialized is not the same as its declared type.
    /// </summary>
    Auto = 4,
  }
}
