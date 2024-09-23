// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchema
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// An in-memory representation of a JSON Schema.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public class JsonSchema
  {
    private readonly string _internalId = Guid.NewGuid().ToString("N");

    /// <summary>Gets or sets the id.</summary>
    public string Id { get; set; }

    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; }

    /// <summary>Gets or sets whether the object is required.</summary>
    public bool? Required { get; set; }

    /// <summary>Gets or sets whether the object is read only.</summary>
    public bool? ReadOnly { get; set; }

    /// <summary>Gets or sets whether the object is visible to users.</summary>
    public bool? Hidden { get; set; }

    /// <summary>Gets or sets whether the object is transient.</summary>
    public bool? Transient { get; set; }

    /// <summary>Gets or sets the description of the object.</summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the types of values allowed by the object.
    /// </summary>
    /// <value>The type.</value>
    public JsonSchemaType? Type { get; set; }

    /// <summary>Gets or sets the pattern.</summary>
    /// <value>The pattern.</value>
    public string Pattern { get; set; }

    /// <summary>Gets or sets the minimum length.</summary>
    /// <value>The minimum length.</value>
    public int? MinimumLength { get; set; }

    /// <summary>Gets or sets the maximum length.</summary>
    /// <value>The maximum length.</value>
    public int? MaximumLength { get; set; }

    /// <summary>
    /// Gets or sets a number that the value should be divisble by.
    /// </summary>
    /// <value>A number that the value should be divisble by.</value>
    public double? DivisibleBy { get; set; }

    /// <summary>Gets or sets the minimum.</summary>
    /// <value>The minimum.</value>
    public double? Minimum { get; set; }

    /// <summary>Gets or sets the maximum.</summary>
    /// <value>The maximum.</value>
    public double? Maximum { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether the value can not equal the number defined by the "minimum" attribute.
    /// </summary>
    /// <value>A flag indicating whether the value can not equal the number defined by the "minimum" attribute.</value>
    public bool? ExclusiveMinimum { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether the value can not equal the number defined by the "maximum" attribute.
    /// </summary>
    /// <value>A flag indicating whether the value can not equal the number defined by the "maximum" attribute.</value>
    public bool? ExclusiveMaximum { get; set; }

    /// <summary>Gets or sets the minimum number of items.</summary>
    /// <value>The minimum number of items.</value>
    public int? MinimumItems { get; set; }

    /// <summary>Gets or sets the maximum number of items.</summary>
    /// <value>The maximum number of items.</value>
    public int? MaximumItems { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of items.
    /// </summary>
    /// <value>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of items.</value>
    public IList<JsonSchema> Items { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items in an array are validated using the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> instance at their array position from <see cref="P:Newtonsoft.Json.Schema.JsonSchema.Items" />.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if items are validated using their array position; otherwise, <c>false</c>.
    /// </value>
    public bool PositionalItemsValidation { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of additional items.
    /// </summary>
    /// <value>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of additional items.</value>
    public JsonSchema AdditionalItems { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether additional items are allowed.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if additional items are allowed; otherwise, <c>false</c>.
    /// </value>
    public bool AllowAdditionalItems { get; set; }

    /// <summary>Gets or sets whether the array items must be unique.</summary>
    public bool UniqueItems { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of properties.
    /// </summary>
    /// <value>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of properties.</value>
    public IDictionary<string, JsonSchema> Properties { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of additional properties.
    /// </summary>
    /// <value>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> of additional properties.</value>
    public JsonSchema AdditionalProperties { get; set; }

    /// <summary>Gets or sets the pattern properties.</summary>
    /// <value>The pattern properties.</value>
    public IDictionary<string, JsonSchema> PatternProperties { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether additional properties are allowed.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if additional properties are allowed; otherwise, <c>false</c>.
    /// </value>
    public bool AllowAdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the required property if this property is present.
    /// </summary>
    /// <value>The required property if this property is present.</value>
    public string Requires { get; set; }

    /// <summary>
    /// Gets or sets the a collection of valid enum values allowed.
    /// </summary>
    /// <value>A collection of valid enum values allowed.</value>
    public IList<JToken> Enum { get; set; }

    /// <summary>Gets or sets disallowed types.</summary>
    /// <value>The disallow types.</value>
    public JsonSchemaType? Disallow { get; set; }

    /// <summary>Gets or sets the default value.</summary>
    /// <value>The default value.</value>
    public JToken Default { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> that this schema extends.
    /// </summary>
    /// <value>The collection of <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> that this schema extends.</value>
    public IList<JsonSchema> Extends { get; set; }

    /// <summary>Gets or sets the format.</summary>
    /// <value>The format.</value>
    public string Format { get; set; }

    internal string Location { get; set; }

    internal string InternalId => this._internalId;

    internal string DeferredReference { get; set; }

    internal bool ReferencesResolved { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> class.
    /// </summary>
    public JsonSchema()
    {
      this.AllowAdditionalProperties = true;
      this.AllowAdditionalItems = true;
    }

    /// <summary>
    /// Reads a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the JSON Schema to read.</param>
    /// <returns>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> object representing the JSON Schema.</returns>
    public static JsonSchema Read(JsonReader reader)
    {
      return JsonSchema.Read(reader, new JsonSchemaResolver());
    }

    /// <summary>
    /// Reads a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the JSON Schema to read.</param>
    /// <param name="resolver">The <see cref="T:Newtonsoft.Json.Schema.JsonSchemaResolver" /> to use when resolving schema references.</param>
    /// <returns>The <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> object representing the JSON Schema.</returns>
    public static JsonSchema Read(JsonReader reader, JsonSchemaResolver resolver)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      ValidationUtils.ArgumentNotNull((object) resolver, nameof (resolver));
      return new JsonSchemaBuilder(resolver).Read(reader);
    }

    /// <summary>
    /// Load a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from a string that contains schema JSON.
    /// </summary>
    /// <param name="json">A <see cref="T:System.String" /> that contains JSON.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> populated from the string that contains JSON.</returns>
    public static JsonSchema Parse(string json) => JsonSchema.Parse(json, new JsonSchemaResolver());

    /// <summary>Parses the specified json.</summary>
    /// <param name="json">The json.</param>
    /// <param name="resolver">The resolver.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> populated from the string that contains JSON.</returns>
    public static JsonSchema Parse(string json, JsonSchemaResolver resolver)
    {
      ValidationUtils.ArgumentNotNull((object) json, nameof (json));
      using (JsonReader reader = (JsonReader) new JsonTextReader((TextReader) new StringReader(json)))
        return JsonSchema.Read(reader, resolver);
    }

    /// <summary>
    /// Writes this schema to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
    /// </summary>
    /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
    public void WriteTo(JsonWriter writer) => this.WriteTo(writer, new JsonSchemaResolver());

    /// <summary>
    /// Writes this schema to a <see cref="T:Newtonsoft.Json.JsonWriter" /> using the specified <see cref="T:Newtonsoft.Json.Schema.JsonSchemaResolver" />.
    /// </summary>
    /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
    /// <param name="resolver">The resolver used.</param>
    public void WriteTo(JsonWriter writer, JsonSchemaResolver resolver)
    {
      ValidationUtils.ArgumentNotNull((object) writer, nameof (writer));
      ValidationUtils.ArgumentNotNull((object) resolver, nameof (resolver));
      new JsonSchemaWriter(writer, resolver).WriteSchema(this);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
      StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      JsonTextWriter writer = new JsonTextWriter((TextWriter) stringWriter);
      writer.Formatting = Formatting.Indented;
      this.WriteTo((JsonWriter) writer);
      return stringWriter.ToString();
    }
  }
}
