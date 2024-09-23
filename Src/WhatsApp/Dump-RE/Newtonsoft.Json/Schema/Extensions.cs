// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.Extensions
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// Contains the JSON schema extension methods.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public static class Extensions
  {
    /// <summary>
    /// <para>
    /// Determines whether the <see cref="T:Newtonsoft.Json.Linq.JToken" /> is valid.
    /// </para>
    /// <note type="caution">
    /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
    /// </note>
    /// </summary>
    /// <param name="source">The source <see cref="T:Newtonsoft.Json.Linq.JToken" /> to test.</param>
    /// <param name="schema">The schema to test with.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="T:Newtonsoft.Json.Linq.JToken" /> is valid; otherwise, <c>false</c>.
    /// </returns>
    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public static bool IsValid(this JToken source, JsonSchema schema)
    {
      bool valid = true;
      source.Validate(schema, (ValidationEventHandler) ((sender, args) => valid = false));
      return valid;
    }

    /// <summary>
    /// <para>
    /// Determines whether the <see cref="T:Newtonsoft.Json.Linq.JToken" /> is valid.
    /// </para>
    /// <note type="caution">
    /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
    /// </note>
    /// </summary>
    /// <param name="source">The source <see cref="T:Newtonsoft.Json.Linq.JToken" /> to test.</param>
    /// <param name="schema">The schema to test with.</param>
    /// <param name="errorMessages">When this method returns, contains any error messages generated while validating. </param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="T:Newtonsoft.Json.Linq.JToken" /> is valid; otherwise, <c>false</c>.
    /// </returns>
    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public static bool IsValid(
      this JToken source,
      JsonSchema schema,
      out IList<string> errorMessages)
    {
      IList<string> errors = (IList<string>) new List<string>();
      source.Validate(schema, (ValidationEventHandler) ((sender, args) => errors.Add(args.Message)));
      errorMessages = errors;
      return errorMessages.Count == 0;
    }

    /// <summary>
    /// <para>
    /// Validates the specified <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </para>
    /// <note type="caution">
    /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
    /// </note>
    /// </summary>
    /// <param name="source">The source <see cref="T:Newtonsoft.Json.Linq.JToken" /> to test.</param>
    /// <param name="schema">The schema to test with.</param>
    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public static void Validate(this JToken source, JsonSchema schema)
    {
      source.Validate(schema, (ValidationEventHandler) null);
    }

    /// <summary>
    /// <para>
    /// Validates the specified <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </para>
    /// <note type="caution">
    /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
    /// </note>
    /// </summary>
    /// <param name="source">The source <see cref="T:Newtonsoft.Json.Linq.JToken" /> to test.</param>
    /// <param name="schema">The schema to test with.</param>
    /// <param name="validationEventHandler">The validation event handler.</param>
    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    public static void Validate(
      this JToken source,
      JsonSchema schema,
      ValidationEventHandler validationEventHandler)
    {
      ValidationUtils.ArgumentNotNull((object) source, nameof (source));
      ValidationUtils.ArgumentNotNull((object) schema, nameof (schema));
      using (JsonValidatingReader validatingReader = new JsonValidatingReader(source.CreateReader()))
      {
        validatingReader.Schema = schema;
        if (validationEventHandler != null)
          validatingReader.ValidationEventHandler += validationEventHandler;
        do
          ;
        while (validatingReader.Read());
      }
    }
  }
}
