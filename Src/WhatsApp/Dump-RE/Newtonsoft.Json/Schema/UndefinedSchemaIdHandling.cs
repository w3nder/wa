// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.UndefinedSchemaIdHandling
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
  /// Specifies undefined schema Id handling options for the <see cref="T:Newtonsoft.Json.Schema.JsonSchemaGenerator" />.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public enum UndefinedSchemaIdHandling
  {
    /// <summary>Do not infer a schema Id.</summary>
    None,
    /// <summary>Use the .NET type name as the schema Id.</summary>
    UseTypeName,
    /// <summary>
    /// Use the assembly qualified .NET type name as the schema Id.
    /// </summary>
    UseAssemblyQualifiedName,
  }
}
