// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.PreserveReferencesHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies reference handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// Note that references cannot be preserved when a value is set via a non-default constructor such as types that implement ISerializable.
  /// </summary>
  /// <example>
  ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="PreservingObjectReferencesOn" title="Preserve Object References" />
  /// </example>
  [Flags]
  public enum PreserveReferencesHandling
  {
    /// <summary>Do not preserve references when serializing types.</summary>
    None = 0,
    /// <summary>
    /// Preserve references when serializing into a JSON object structure.
    /// </summary>
    Objects = 1,
    /// <summary>
    /// Preserve references when serializing into a JSON array structure.
    /// </summary>
    Arrays = 2,
    /// <summary>Preserve references when serializing.</summary>
    All = Arrays | Objects, // 0x00000003
  }
}
