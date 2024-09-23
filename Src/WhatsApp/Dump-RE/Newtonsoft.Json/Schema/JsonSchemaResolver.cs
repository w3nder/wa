// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaResolver
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// Resolves <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from an id.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public class JsonSchemaResolver
  {
    /// <summary>Gets or sets the loaded schemas.</summary>
    /// <value>The loaded schemas.</value>
    public IList<JsonSchema> LoadedSchemas { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Schema.JsonSchemaResolver" /> class.
    /// </summary>
    public JsonSchemaResolver() => this.LoadedSchemas = (IList<JsonSchema>) new List<JsonSchema>();

    /// <summary>
    /// Gets a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> for the specified reference.
    /// </summary>
    /// <param name="reference">The id.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> for the specified reference.</returns>
    public virtual JsonSchema GetSchema(string reference)
    {
      return this.LoadedSchemas.SingleOrDefault<JsonSchema>((Func<JsonSchema, bool>) (s => string.Equals(s.Id, reference, StringComparison.Ordinal))) ?? this.LoadedSchemas.SingleOrDefault<JsonSchema>((Func<JsonSchema, bool>) (s => string.Equals(s.Location, reference, StringComparison.Ordinal)));
    }
  }
}
