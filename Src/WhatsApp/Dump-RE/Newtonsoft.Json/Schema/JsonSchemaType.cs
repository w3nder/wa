// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaType
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// The value types allowed by the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" />.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Flags]
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public enum JsonSchemaType
  {
    /// <summary>No type specified.</summary>
    None = 0,
    /// <summary>String type.</summary>
    String = 1,
    /// <summary>Float type.</summary>
    Float = 2,
    /// <summary>Integer type.</summary>
    Integer = 4,
    /// <summary>Boolean type.</summary>
    Boolean = 8,
    /// <summary>Object type.</summary>
    Object = 16, // 0x00000010
    /// <summary>Array type.</summary>
    Array = 32, // 0x00000020
    /// <summary>Null type.</summary>
    Null = 64, // 0x00000040
    /// <summary>Any type.</summary>
    Any = Null | Array | Object | Boolean | Integer | Float | String, // 0x0000007F
  }
}
