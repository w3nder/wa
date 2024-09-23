// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonValidatingReader
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// <para>
  /// Represents a reader that provides <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> validation.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public class JsonValidatingReader : JsonReader, IJsonLineInfo
  {
    private readonly JsonReader _reader;
    private readonly Stack<JsonValidatingReader.SchemaScope> _stack;
    private JsonSchema _schema;
    private JsonSchemaModel _model;
    private JsonValidatingReader.SchemaScope _currentScope;
    private static readonly IList<JsonSchemaModel> EmptySchemaList = (IList<JsonSchemaModel>) new List<JsonSchemaModel>();

    /// <summary>
    /// Sets an event handler for receiving schema validation errors.
    /// </summary>
    public event Newtonsoft.Json.Schema.ValidationEventHandler ValidationEventHandler;

    /// <summary>Gets the text value of the current JSON token.</summary>
    /// <value></value>
    public override object Value => this._reader.Value;

    /// <summary>
    /// Gets the depth of the current token in the JSON document.
    /// </summary>
    /// <value>The depth of the current token in the JSON document.</value>
    public override int Depth => this._reader.Depth;

    /// <summary>Gets the path of the current JSON token.</summary>
    public override string Path => this._reader.Path;

    /// <summary>
    /// Gets the quotation mark character used to enclose the value of a string.
    /// </summary>
    /// <value></value>
    public override char QuoteChar
    {
      get => this._reader.QuoteChar;
      protected internal set
      {
      }
    }

    /// <summary>Gets the type of the current JSON token.</summary>
    /// <value></value>
    public override JsonToken TokenType => this._reader.TokenType;

    /// <summary>
    /// Gets the Common Language Runtime (CLR) type for the current JSON token.
    /// </summary>
    /// <value></value>
    public override Type ValueType => this._reader.ValueType;

    private void Push(JsonValidatingReader.SchemaScope scope)
    {
      this._stack.Push(scope);
      this._currentScope = scope;
    }

    private JsonValidatingReader.SchemaScope Pop()
    {
      JsonValidatingReader.SchemaScope schemaScope = this._stack.Pop();
      this._currentScope = this._stack.Count != 0 ? this._stack.Peek() : (JsonValidatingReader.SchemaScope) null;
      return schemaScope;
    }

    private IList<JsonSchemaModel> CurrentSchemas => this._currentScope.Schemas;

    private IList<JsonSchemaModel> CurrentMemberSchemas
    {
      get
      {
        if (this._currentScope == null)
          return (IList<JsonSchemaModel>) new List<JsonSchemaModel>((IEnumerable<JsonSchemaModel>) new JsonSchemaModel[1]
          {
            this._model
          });
        if (this._currentScope.Schemas == null || this._currentScope.Schemas.Count == 0)
          return JsonValidatingReader.EmptySchemaList;
        switch (this._currentScope.TokenType)
        {
          case JTokenType.None:
            return this._currentScope.Schemas;
          case JTokenType.Object:
            if (this._currentScope.CurrentPropertyName == null)
              throw new JsonReaderException("CurrentPropertyName has not been set on scope.");
            IList<JsonSchemaModel> currentMemberSchemas1 = (IList<JsonSchemaModel>) new List<JsonSchemaModel>();
            foreach (JsonSchemaModel currentSchema in (IEnumerable<JsonSchemaModel>) this.CurrentSchemas)
            {
              JsonSchemaModel jsonSchemaModel;
              if (currentSchema.Properties != null && currentSchema.Properties.TryGetValue(this._currentScope.CurrentPropertyName, out jsonSchemaModel))
                currentMemberSchemas1.Add(jsonSchemaModel);
              if (currentSchema.PatternProperties != null)
              {
                foreach (KeyValuePair<string, JsonSchemaModel> patternProperty in (IEnumerable<KeyValuePair<string, JsonSchemaModel>>) currentSchema.PatternProperties)
                {
                  if (Regex.IsMatch(this._currentScope.CurrentPropertyName, patternProperty.Key))
                    currentMemberSchemas1.Add(patternProperty.Value);
                }
              }
              if (currentMemberSchemas1.Count == 0 && currentSchema.AllowAdditionalProperties && currentSchema.AdditionalProperties != null)
                currentMemberSchemas1.Add(currentSchema.AdditionalProperties);
            }
            return currentMemberSchemas1;
          case JTokenType.Array:
            IList<JsonSchemaModel> currentMemberSchemas2 = (IList<JsonSchemaModel>) new List<JsonSchemaModel>();
            foreach (JsonSchemaModel currentSchema in (IEnumerable<JsonSchemaModel>) this.CurrentSchemas)
            {
              if (!currentSchema.PositionalItemsValidation)
              {
                if (currentSchema.Items != null && currentSchema.Items.Count > 0)
                  currentMemberSchemas2.Add(currentSchema.Items[0]);
              }
              else
              {
                if (currentSchema.Items != null && currentSchema.Items.Count > 0 && currentSchema.Items.Count > this._currentScope.ArrayItemCount - 1)
                  currentMemberSchemas2.Add(currentSchema.Items[this._currentScope.ArrayItemCount - 1]);
                if (currentSchema.AllowAdditionalItems && currentSchema.AdditionalItems != null)
                  currentMemberSchemas2.Add(currentSchema.AdditionalItems);
              }
            }
            return currentMemberSchemas2;
          case JTokenType.Constructor:
            return JsonValidatingReader.EmptySchemaList;
          default:
            throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._currentScope.TokenType));
        }
      }
    }

    private void RaiseError(string message, JsonSchemaModel schema)
    {
      IJsonLineInfo jsonLineInfo = (IJsonLineInfo) this;
      this.OnValidationEvent(new JsonSchemaException(jsonLineInfo.HasLineInfo() ? message + " Line {0}, position {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) jsonLineInfo.LineNumber, (object) jsonLineInfo.LinePosition) : message, (Exception) null, this.Path, jsonLineInfo.LineNumber, jsonLineInfo.LinePosition));
    }

    private void OnValidationEvent(JsonSchemaException exception)
    {
      Newtonsoft.Json.Schema.ValidationEventHandler validationEventHandler = this.ValidationEventHandler;
      if (validationEventHandler == null)
        throw exception;
      validationEventHandler((object) this, new ValidationEventArgs(exception));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonValidatingReader" /> class that
    /// validates the content returned from the given <see cref="T:Newtonsoft.Json.JsonReader" />.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from while validating.</param>
    public JsonValidatingReader(JsonReader reader)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      this._reader = reader;
      this._stack = new Stack<JsonValidatingReader.SchemaScope>();
    }

    /// <summary>Gets or sets the schema.</summary>
    /// <value>The schema.</value>
    public JsonSchema Schema
    {
      get => this._schema;
      set
      {
        if (this.TokenType != JsonToken.None)
          throw new InvalidOperationException("Cannot change schema while validating JSON.");
        this._schema = value;
        this._model = (JsonSchemaModel) null;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:Newtonsoft.Json.JsonReader" /> used to construct this <see cref="T:Newtonsoft.Json.JsonValidatingReader" />.
    /// </summary>
    /// <value>The <see cref="T:Newtonsoft.Json.JsonReader" /> specified in the constructor.</value>
    public JsonReader Reader => this._reader;

    private void ValidateNotDisallowed(JsonSchemaModel schema)
    {
      if (schema == null)
        return;
      JsonSchemaType? currentNodeSchemaType = this.GetCurrentNodeSchemaType();
      if (!currentNodeSchemaType.HasValue || !JsonSchemaGenerator.HasFlag(new JsonSchemaType?(schema.Disallow), currentNodeSchemaType.Value))
        return;
      this.RaiseError("Type {0} is disallowed.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) currentNodeSchemaType), schema);
    }

    private JsonSchemaType? GetCurrentNodeSchemaType()
    {
      switch (this._reader.TokenType)
      {
        case JsonToken.StartObject:
          return new JsonSchemaType?(JsonSchemaType.Object);
        case JsonToken.StartArray:
          return new JsonSchemaType?(JsonSchemaType.Array);
        case JsonToken.Integer:
          return new JsonSchemaType?(JsonSchemaType.Integer);
        case JsonToken.Float:
          return new JsonSchemaType?(JsonSchemaType.Float);
        case JsonToken.String:
          return new JsonSchemaType?(JsonSchemaType.String);
        case JsonToken.Boolean:
          return new JsonSchemaType?(JsonSchemaType.Boolean);
        case JsonToken.Null:
          return new JsonSchemaType?(JsonSchemaType.Null);
        default:
          return new JsonSchemaType?();
      }
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />.</returns>
    public override int? ReadAsInt32()
    {
      int? nullable = this._reader.ReadAsInt32();
      this.ValidateCurrentToken();
      return nullable;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Byte" />[].
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Byte" />[] or a null reference if the next JSON token is null.
    /// </returns>
    public override byte[] ReadAsBytes()
    {
      byte[] numArray = this._reader.ReadAsBytes();
      this.ValidateCurrentToken();
      return numArray;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />.</returns>
    public override Decimal? ReadAsDecimal()
    {
      Decimal? nullable = this._reader.ReadAsDecimal();
      this.ValidateCurrentToken();
      return nullable;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.String" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public override string ReadAsString()
    {
      string str = this._reader.ReadAsString();
      this.ValidateCurrentToken();
      return str;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public override DateTime? ReadAsDateTime()
    {
      DateTime? nullable = this._reader.ReadAsDateTime();
      this.ValidateCurrentToken();
      return nullable;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />.</returns>
    public override DateTimeOffset? ReadAsDateTimeOffset()
    {
      DateTimeOffset? nullable = this._reader.ReadAsDateTimeOffset();
      this.ValidateCurrentToken();
      return nullable;
    }

    /// <summary>Reads the next JSON token from the stream.</summary>
    /// <returns>
    /// true if the next token was read successfully; false if there are no more tokens to read.
    /// </returns>
    public override bool Read()
    {
      if (!this._reader.Read())
        return false;
      if (this._reader.TokenType == JsonToken.Comment)
        return true;
      this.ValidateCurrentToken();
      return true;
    }

    private void ValidateCurrentToken()
    {
      if (this._model == null)
      {
        this._model = new JsonSchemaModelBuilder().Build(this._schema);
        if (!JsonTokenUtils.IsStartToken(this._reader.TokenType))
          this.Push(new JsonValidatingReader.SchemaScope(JTokenType.None, this.CurrentMemberSchemas));
      }
      switch (this._reader.TokenType)
      {
        case JsonToken.None:
          break;
        case JsonToken.StartObject:
          this.ProcessValue();
          this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Object, (IList<JsonSchemaModel>) this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateObject)).ToList<JsonSchemaModel>()));
          this.WriteToken(this.CurrentSchemas);
          break;
        case JsonToken.StartArray:
          this.ProcessValue();
          this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Array, (IList<JsonSchemaModel>) this.CurrentMemberSchemas.Where<JsonSchemaModel>(new Func<JsonSchemaModel, bool>(this.ValidateArray)).ToList<JsonSchemaModel>()));
          this.WriteToken(this.CurrentSchemas);
          break;
        case JsonToken.StartConstructor:
          this.ProcessValue();
          this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Constructor, (IList<JsonSchemaModel>) null));
          this.WriteToken(this.CurrentSchemas);
          break;
        case JsonToken.PropertyName:
          this.WriteToken(this.CurrentSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidatePropertyName(enumerator.Current);
            break;
          }
        case JsonToken.Raw:
          this.ProcessValue();
          break;
        case JsonToken.Integer:
          this.ProcessValue();
          this.WriteToken(this.CurrentMemberSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidateInteger(enumerator.Current);
            break;
          }
        case JsonToken.Float:
          this.ProcessValue();
          this.WriteToken(this.CurrentMemberSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidateFloat(enumerator.Current);
            break;
          }
        case JsonToken.String:
          this.ProcessValue();
          this.WriteToken(this.CurrentMemberSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidateString(enumerator.Current);
            break;
          }
        case JsonToken.Boolean:
          this.ProcessValue();
          this.WriteToken(this.CurrentMemberSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidateBoolean(enumerator.Current);
            break;
          }
        case JsonToken.Null:
          this.ProcessValue();
          this.WriteToken(this.CurrentMemberSchemas);
          using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ValidateNull(enumerator.Current);
            break;
          }
        case JsonToken.Undefined:
        case JsonToken.Date:
        case JsonToken.Bytes:
          this.WriteToken(this.CurrentMemberSchemas);
          break;
        case JsonToken.EndObject:
          this.WriteToken(this.CurrentSchemas);
          foreach (JsonSchemaModel currentSchema in (IEnumerable<JsonSchemaModel>) this.CurrentSchemas)
            this.ValidateEndObject(currentSchema);
          this.Pop();
          break;
        case JsonToken.EndArray:
          this.WriteToken(this.CurrentSchemas);
          foreach (JsonSchemaModel currentSchema in (IEnumerable<JsonSchemaModel>) this.CurrentSchemas)
            this.ValidateEndArray(currentSchema);
          this.Pop();
          break;
        case JsonToken.EndConstructor:
          this.WriteToken(this.CurrentSchemas);
          this.Pop();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void WriteToken(IList<JsonSchemaModel> schemas)
    {
      foreach (JsonValidatingReader.SchemaScope schemaScope in this._stack)
      {
        bool flag = schemaScope.TokenType == JTokenType.Array && schemaScope.IsUniqueArray && schemaScope.ArrayItemCount > 0;
        if (flag || schemas.Any<JsonSchemaModel>((Func<JsonSchemaModel, bool>) (s => s.Enum != null)))
        {
          if (schemaScope.CurrentItemWriter == null)
          {
            if (!JsonTokenUtils.IsEndToken(this._reader.TokenType))
              schemaScope.CurrentItemWriter = new JTokenWriter();
            else
              continue;
          }
          schemaScope.CurrentItemWriter.WriteToken(this._reader, false);
          if (schemaScope.CurrentItemWriter.Top == 0 && this._reader.TokenType != JsonToken.PropertyName)
          {
            JToken token = schemaScope.CurrentItemWriter.Token;
            schemaScope.CurrentItemWriter = (JTokenWriter) null;
            if (flag)
            {
              if (schemaScope.UniqueArrayItems.Contains<JToken>(token, (IEqualityComparer<JToken>) JToken.EqualityComparer))
                this.RaiseError("Non-unique array item at index {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) (schemaScope.ArrayItemCount - 1)), schemaScope.Schemas.First<JsonSchemaModel>((Func<JsonSchemaModel, bool>) (s => s.UniqueItems)));
              schemaScope.UniqueArrayItems.Add(token);
            }
            else if (schemas.Any<JsonSchemaModel>((Func<JsonSchemaModel, bool>) (s => s.Enum != null)))
            {
              foreach (JsonSchemaModel schema in (IEnumerable<JsonSchemaModel>) schemas)
              {
                if (schema.Enum != null && !schema.Enum.ContainsValue<JToken>(token, (IEqualityComparer<JToken>) JToken.EqualityComparer))
                {
                  StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
                  token.WriteTo((JsonWriter) new JsonTextWriter((TextWriter) stringWriter));
                  this.RaiseError("Value {0} is not defined in enum.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) stringWriter.ToString()), schema);
                }
              }
            }
          }
        }
      }
    }

    private void ValidateEndObject(JsonSchemaModel schema)
    {
      if (schema == null)
        return;
      Dictionary<string, bool> requiredProperties = this._currentScope.RequiredProperties;
      if (requiredProperties == null)
        return;
      List<string> list = requiredProperties.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (kv => !kv.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (kv => kv.Key)).ToList<string>();
      if (list.Count <= 0)
        return;
      this.RaiseError("Required properties are missing from object: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) string.Join(", ", list.ToArray())), schema);
    }

    private void ValidateEndArray(JsonSchemaModel schema)
    {
      if (schema == null)
        return;
      int arrayItemCount = this._currentScope.ArrayItemCount;
      if (schema.MaximumItems.HasValue)
      {
        int num = arrayItemCount;
        int? maximumItems = schema.MaximumItems;
        if ((num <= maximumItems.GetValueOrDefault() ? 0 : (maximumItems.HasValue ? 1 : 0)) != 0)
          this.RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) arrayItemCount, (object) schema.MaximumItems), schema);
      }
      if (!schema.MinimumItems.HasValue)
        return;
      int num1 = arrayItemCount;
      int? minimumItems = schema.MinimumItems;
      if ((num1 >= minimumItems.GetValueOrDefault() ? 0 : (minimumItems.HasValue ? 1 : 0)) == 0)
        return;
      this.RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) arrayItemCount, (object) schema.MinimumItems), schema);
    }

    private void ValidateNull(JsonSchemaModel schema)
    {
      if (schema == null || !this.TestType(schema, JsonSchemaType.Null))
        return;
      this.ValidateNotDisallowed(schema);
    }

    private void ValidateBoolean(JsonSchemaModel schema)
    {
      if (schema == null || !this.TestType(schema, JsonSchemaType.Boolean))
        return;
      this.ValidateNotDisallowed(schema);
    }

    private void ValidateString(JsonSchemaModel schema)
    {
      if (schema == null || !this.TestType(schema, JsonSchemaType.String))
        return;
      this.ValidateNotDisallowed(schema);
      string input = this._reader.Value.ToString();
      if (schema.MaximumLength.HasValue)
      {
        int length = input.Length;
        int? maximumLength = schema.MaximumLength;
        if ((length <= maximumLength.GetValueOrDefault() ? 0 : (maximumLength.HasValue ? 1 : 0)) != 0)
          this.RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) input, (object) schema.MaximumLength), schema);
      }
      if (schema.MinimumLength.HasValue)
      {
        int length = input.Length;
        int? minimumLength = schema.MinimumLength;
        if ((length >= minimumLength.GetValueOrDefault() ? 0 : (minimumLength.HasValue ? 1 : 0)) != 0)
          this.RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) input, (object) schema.MinimumLength), schema);
      }
      if (schema.Patterns == null)
        return;
      foreach (string pattern in (IEnumerable<string>) schema.Patterns)
      {
        if (!Regex.IsMatch(input, pattern))
          this.RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) input, (object) pattern), schema);
      }
    }

    private void ValidateInteger(JsonSchemaModel schema)
    {
      if (schema == null || !this.TestType(schema, JsonSchemaType.Integer))
        return;
      this.ValidateNotDisallowed(schema);
      object objA = this._reader.Value;
      if (schema.Maximum.HasValue)
      {
        if (JValue.Compare(JTokenType.Integer, objA, (object) schema.Maximum) > 0)
          this.RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, objA, (object) schema.Maximum), schema);
        if (schema.ExclusiveMaximum && JValue.Compare(JTokenType.Integer, objA, (object) schema.Maximum) == 0)
          this.RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, objA, (object) schema.Maximum), schema);
      }
      if (schema.Minimum.HasValue)
      {
        if (JValue.Compare(JTokenType.Integer, objA, (object) schema.Minimum) < 0)
          this.RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, objA, (object) schema.Minimum), schema);
        if (schema.ExclusiveMinimum && JValue.Compare(JTokenType.Integer, objA, (object) schema.Minimum) == 0)
          this.RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, objA, (object) schema.Minimum), schema);
      }
      if (!schema.DivisibleBy.HasValue || JsonValidatingReader.IsZero((double) Convert.ToInt64(objA, (IFormatProvider) CultureInfo.InvariantCulture) % schema.DivisibleBy.Value))
        return;
      this.RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(objA), (object) schema.DivisibleBy), schema);
    }

    private void ProcessValue()
    {
      if (this._currentScope == null || this._currentScope.TokenType != JTokenType.Array)
        return;
      ++this._currentScope.ArrayItemCount;
      foreach (JsonSchemaModel currentSchema in (IEnumerable<JsonSchemaModel>) this.CurrentSchemas)
      {
        if (currentSchema != null && currentSchema.PositionalItemsValidation && !currentSchema.AllowAdditionalItems && (currentSchema.Items == null || this._currentScope.ArrayItemCount - 1 >= currentSchema.Items.Count))
          this.RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._currentScope.ArrayItemCount), currentSchema);
      }
    }

    private void ValidateFloat(JsonSchemaModel schema)
    {
      if (schema == null || !this.TestType(schema, JsonSchemaType.Float))
        return;
      this.ValidateNotDisallowed(schema);
      double dividend = Convert.ToDouble(this._reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (schema.Maximum.HasValue)
      {
        double num1 = dividend;
        double? maximum1 = schema.Maximum;
        if ((num1 <= maximum1.GetValueOrDefault() ? 0 : (maximum1.HasValue ? 1 : 0)) != 0)
          this.RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(dividend), (object) schema.Maximum), schema);
        if (schema.ExclusiveMaximum)
        {
          double num2 = dividend;
          double? maximum2 = schema.Maximum;
          if ((num2 != maximum2.GetValueOrDefault() ? 0 : (maximum2.HasValue ? 1 : 0)) != 0)
            this.RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(dividend), (object) schema.Maximum), schema);
        }
      }
      if (schema.Minimum.HasValue)
      {
        double num3 = dividend;
        double? minimum1 = schema.Minimum;
        if ((num3 >= minimum1.GetValueOrDefault() ? 0 : (minimum1.HasValue ? 1 : 0)) != 0)
          this.RaiseError("Float {0} is less than minimum value of {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(dividend), (object) schema.Minimum), schema);
        if (schema.ExclusiveMinimum)
        {
          double num4 = dividend;
          double? minimum2 = schema.Minimum;
          if ((num4 != minimum2.GetValueOrDefault() ? 0 : (minimum2.HasValue ? 1 : 0)) != 0)
            this.RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(dividend), (object) schema.Minimum), schema);
        }
      }
      if (!schema.DivisibleBy.HasValue || JsonValidatingReader.IsZero(JsonValidatingReader.FloatingPointRemainder(dividend, schema.DivisibleBy.Value)))
        return;
      this.RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonConvert.ToString(dividend), (object) schema.DivisibleBy), schema);
    }

    private static double FloatingPointRemainder(double dividend, double divisor)
    {
      return dividend - Math.Floor(dividend / divisor) * divisor;
    }

    private static bool IsZero(double value) => Math.Abs(value) < 4.4408920985006262E-15;

    private void ValidatePropertyName(JsonSchemaModel schema)
    {
      if (schema == null)
        return;
      string str = Convert.ToString(this._reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (this._currentScope.RequiredProperties.ContainsKey(str))
        this._currentScope.RequiredProperties[str] = true;
      if (!schema.AllowAdditionalProperties && !this.IsPropertyDefinied(schema, str))
        this.RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str), schema);
      this._currentScope.CurrentPropertyName = str;
    }

    private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
    {
      if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
        return true;
      if (schema.PatternProperties != null)
      {
        foreach (string key in (IEnumerable<string>) schema.PatternProperties.Keys)
        {
          if (Regex.IsMatch(propertyName, key))
            return true;
        }
      }
      return false;
    }

    private bool ValidateArray(JsonSchemaModel schema)
    {
      return schema == null || this.TestType(schema, JsonSchemaType.Array);
    }

    private bool ValidateObject(JsonSchemaModel schema)
    {
      return schema == null || this.TestType(schema, JsonSchemaType.Object);
    }

    private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
    {
      if (JsonSchemaGenerator.HasFlag(new JsonSchemaType?(currentSchema.Type), currentType))
        return true;
      this.RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) currentSchema.Type, (object) currentType), currentSchema);
      return false;
    }

    bool IJsonLineInfo.HasLineInfo()
    {
      return this._reader is IJsonLineInfo reader && reader.HasLineInfo();
    }

    int IJsonLineInfo.LineNumber => !(this._reader is IJsonLineInfo reader) ? 0 : reader.LineNumber;

    int IJsonLineInfo.LinePosition
    {
      get => !(this._reader is IJsonLineInfo reader) ? 0 : reader.LinePosition;
    }

    private class SchemaScope
    {
      private readonly JTokenType _tokenType;
      private readonly IList<JsonSchemaModel> _schemas;
      private readonly Dictionary<string, bool> _requiredProperties;

      public string CurrentPropertyName { get; set; }

      public int ArrayItemCount { get; set; }

      public bool IsUniqueArray { get; set; }

      public IList<JToken> UniqueArrayItems { get; set; }

      public JTokenWriter CurrentItemWriter { get; set; }

      public IList<JsonSchemaModel> Schemas => this._schemas;

      public Dictionary<string, bool> RequiredProperties => this._requiredProperties;

      public JTokenType TokenType => this._tokenType;

      public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
      {
        this._tokenType = tokenType;
        this._schemas = schemas;
        this._requiredProperties = schemas.SelectMany<JsonSchemaModel, string>(new Func<JsonSchemaModel, IEnumerable<string>>(this.GetRequiredProperties)).Distinct<string>().ToDictionary<string, string, bool>((Func<string, string>) (p => p), (Func<string, bool>) (p => false));
        if (tokenType != JTokenType.Array || !schemas.Any<JsonSchemaModel>((Func<JsonSchemaModel, bool>) (s => s.UniqueItems)))
          return;
        this.IsUniqueArray = true;
        this.UniqueArrayItems = (IList<JToken>) new List<JToken>();
      }

      private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
      {
        return schema == null || schema.Properties == null ? Enumerable.Empty<string>() : schema.Properties.Where<KeyValuePair<string, JsonSchemaModel>>((Func<KeyValuePair<string, JsonSchemaModel>, bool>) (p => p.Value.Required)).Select<KeyValuePair<string, JsonSchemaModel>, string>((Func<KeyValuePair<string, JsonSchemaModel>, string>) (p => p.Key));
      }
    }
  }
}
