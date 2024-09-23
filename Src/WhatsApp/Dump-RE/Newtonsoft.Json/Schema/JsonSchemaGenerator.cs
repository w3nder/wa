// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaGenerator
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// Generates a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from a specified <see cref="T:System.Type" />.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public class JsonSchemaGenerator
  {
    private IContractResolver _contractResolver;
    private JsonSchemaResolver _resolver;
    private readonly IList<JsonSchemaGenerator.TypeSchema> _stack = (IList<JsonSchemaGenerator.TypeSchema>) new List<JsonSchemaGenerator.TypeSchema>();
    private JsonSchema _currentSchema;

    /// <summary>
    /// Gets or sets how undefined schemas are handled by the serializer.
    /// </summary>
    public UndefinedSchemaIdHandling UndefinedSchemaIdHandling { get; set; }

    /// <summary>Gets or sets the contract resolver.</summary>
    /// <value>The contract resolver.</value>
    public IContractResolver ContractResolver
    {
      get
      {
        return this._contractResolver == null ? DefaultContractResolver.Instance : this._contractResolver;
      }
      set => this._contractResolver = value;
    }

    private JsonSchema CurrentSchema => this._currentSchema;

    private void Push(JsonSchemaGenerator.TypeSchema typeSchema)
    {
      this._currentSchema = typeSchema.Schema;
      this._stack.Add(typeSchema);
      this._resolver.LoadedSchemas.Add(typeSchema.Schema);
    }

    private JsonSchemaGenerator.TypeSchema Pop()
    {
      JsonSchemaGenerator.TypeSchema typeSchema1 = this._stack[this._stack.Count - 1];
      this._stack.RemoveAt(this._stack.Count - 1);
      JsonSchemaGenerator.TypeSchema typeSchema2 = this._stack.LastOrDefault<JsonSchemaGenerator.TypeSchema>();
      this._currentSchema = typeSchema2 == null ? (JsonSchema) null : typeSchema2.Schema;
      return typeSchema1;
    }

    /// <summary>
    /// Generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified type.
    /// </summary>
    /// <param name="type">The type to generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> generated from the specified type.</returns>
    public JsonSchema Generate(Type type) => this.Generate(type, new JsonSchemaResolver(), false);

    /// <summary>
    /// Generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified type.
    /// </summary>
    /// <param name="type">The type to generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from.</param>
    /// <param name="resolver">The <see cref="T:Newtonsoft.Json.Schema.JsonSchemaResolver" /> used to resolve schema references.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> generated from the specified type.</returns>
    public JsonSchema Generate(Type type, JsonSchemaResolver resolver)
    {
      return this.Generate(type, resolver, false);
    }

    /// <summary>
    /// Generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified type.
    /// </summary>
    /// <param name="type">The type to generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from.</param>
    /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> will be nullable.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> generated from the specified type.</returns>
    public JsonSchema Generate(Type type, bool rootSchemaNullable)
    {
      return this.Generate(type, new JsonSchemaResolver(), rootSchemaNullable);
    }

    /// <summary>
    /// Generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from the specified type.
    /// </summary>
    /// <param name="type">The type to generate a <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> from.</param>
    /// <param name="resolver">The <see cref="T:Newtonsoft.Json.Schema.JsonSchemaResolver" /> used to resolve schema references.</param>
    /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> will be nullable.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Schema.JsonSchema" /> generated from the specified type.</returns>
    public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      ValidationUtils.ArgumentNotNull((object) resolver, nameof (resolver));
      this._resolver = resolver;
      return this.GenerateInternal(type, !rootSchemaNullable ? Required.Always : Required.Default, false);
    }

    private string GetTitle(Type type)
    {
      JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) type);
      return cachedAttribute != null && !string.IsNullOrEmpty(cachedAttribute.Title) ? cachedAttribute.Title : (string) null;
    }

    private string GetDescription(Type type)
    {
      JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) type);
      return cachedAttribute != null && !string.IsNullOrEmpty(cachedAttribute.Description) ? cachedAttribute.Description : (string) null;
    }

    private string GetTypeId(Type type, bool explicitOnly)
    {
      JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) type);
      if (cachedAttribute != null && !string.IsNullOrEmpty(cachedAttribute.Id))
        return cachedAttribute.Id;
      if (explicitOnly)
        return (string) null;
      switch (this.UndefinedSchemaIdHandling)
      {
        case UndefinedSchemaIdHandling.UseTypeName:
          return type.FullName;
        case UndefinedSchemaIdHandling.UseAssemblyQualifiedName:
          return type.AssemblyQualifiedName;
        default:
          return (string) null;
      }
    }

    private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      string typeId1 = this.GetTypeId(type, false);
      string typeId2 = this.GetTypeId(type, true);
      if (!string.IsNullOrEmpty(typeId1))
      {
        JsonSchema schema = this._resolver.GetSchema(typeId1);
        if (schema != null)
        {
          if (valueRequired != Required.Always && !JsonSchemaGenerator.HasFlag(schema.Type, JsonSchemaType.Null))
          {
            JsonSchema jsonSchema = schema;
            JsonSchemaType? type1 = jsonSchema.Type;
            jsonSchema.Type = type1.HasValue ? new JsonSchemaType?(type1.GetValueOrDefault() | JsonSchemaType.Null) : new JsonSchemaType?();
          }
          if (required)
          {
            bool? required1 = schema.Required;
            if ((!required1.GetValueOrDefault() ? 1 : (!required1.HasValue ? 1 : 0)) != 0)
              schema.Required = new bool?(true);
          }
          return schema;
        }
      }
      if (this._stack.Any<JsonSchemaGenerator.TypeSchema>((Func<JsonSchemaGenerator.TypeSchema, bool>) (tc => tc.Type == type)))
        throw new JsonException("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type));
      JsonContract contract = this.ContractResolver.ResolveContract(type);
      JsonConverter jsonConverter;
      if ((jsonConverter = contract.Converter) != null || (jsonConverter = contract.InternalConverter) != null)
      {
        JsonSchema schema = jsonConverter.GetSchema();
        if (schema != null)
          return schema;
      }
      this.Push(new JsonSchemaGenerator.TypeSchema(type, new JsonSchema()));
      if (typeId2 != null)
        this.CurrentSchema.Id = typeId2;
      if (required)
        this.CurrentSchema.Required = new bool?(true);
      this.CurrentSchema.Title = this.GetTitle(type);
      this.CurrentSchema.Description = this.GetDescription(type);
      if (jsonConverter != null)
      {
        this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
      }
      else
      {
        switch (contract.ContractType)
        {
          case JsonContractType.Object:
            this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
            this.CurrentSchema.Id = this.GetTypeId(type, false);
            this.GenerateObjectSchema(type, (JsonObjectContract) contract);
            break;
          case JsonContractType.Array:
            this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Array, valueRequired));
            this.CurrentSchema.Id = this.GetTypeId(type, false);
            JsonArrayAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>((object) type);
            bool flag = cachedAttribute == null || cachedAttribute.AllowNullItems;
            Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
            if (collectionItemType != null)
            {
              this.CurrentSchema.Items = (IList<JsonSchema>) new List<JsonSchema>();
              this.CurrentSchema.Items.Add(this.GenerateInternal(collectionItemType, !flag ? Required.Always : Required.Default, false));
              break;
            }
            break;
          case JsonContractType.Primitive:
            this.CurrentSchema.Type = new JsonSchemaType?(this.GetJsonSchemaType(type, valueRequired));
            JsonSchemaType? type2 = this.CurrentSchema.Type;
            if ((type2.GetValueOrDefault() != JsonSchemaType.Integer ? 0 : (type2.HasValue ? 1 : 0)) != 0 && type.IsEnum() && !type.IsDefined(typeof (FlagsAttribute), true))
            {
              this.CurrentSchema.Enum = (IList<JToken>) new List<JToken>();
              using (IEnumerator<EnumValue<long>> enumerator = EnumUtils.GetNamesAndValues<long>(type).GetEnumerator())
              {
                while (enumerator.MoveNext())
                  this.CurrentSchema.Enum.Add(JToken.FromObject((object) enumerator.Current.Value));
                break;
              }
            }
            else
              break;
          case JsonContractType.String:
            this.CurrentSchema.Type = new JsonSchemaType?(!ReflectionUtils.IsNullable(contract.UnderlyingType) ? JsonSchemaType.String : this.AddNullType(JsonSchemaType.String, valueRequired));
            break;
          case JsonContractType.Dictionary:
            this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
            Type keyType;
            Type valueType;
            ReflectionUtils.GetDictionaryKeyValueTypes(type, out keyType, out valueType);
            if (keyType != null && this.ContractResolver.ResolveContract(keyType).ContractType == JsonContractType.Primitive)
            {
              this.CurrentSchema.AdditionalProperties = this.GenerateInternal(valueType, Required.Default, false);
              break;
            }
            break;
          case JsonContractType.Linq:
            this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
            break;
          default:
            throw new JsonException("Unexpected contract type: {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract));
        }
      }
      return this.Pop().Schema;
    }

    private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
    {
      return valueRequired != Required.Always ? type | JsonSchemaType.Null : type;
    }

    private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
    {
      return (value & flag) == flag;
    }

    private void GenerateObjectSchema(Type type, JsonObjectContract contract)
    {
      this.CurrentSchema.Properties = (IDictionary<string, JsonSchema>) new Dictionary<string, JsonSchema>();
      foreach (JsonProperty property in (Collection<JsonProperty>) contract.Properties)
      {
        if (!property.Ignored)
        {
          NullValueHandling? nullValueHandling = property.NullValueHandling;
          bool flag = (nullValueHandling.GetValueOrDefault() != NullValueHandling.Ignore ? 0 : (nullValueHandling.HasValue ? 1 : 0)) != 0 || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) || property.ShouldSerialize != null || property.GetIsSpecified != null;
          JsonSchema jsonSchema = this.GenerateInternal(property.PropertyType, property.Required, !flag);
          if (property.DefaultValue != null)
            jsonSchema.Default = JToken.FromObject(property.DefaultValue);
          this.CurrentSchema.Properties.Add(property.PropertyName, jsonSchema);
        }
      }
      if (!type.IsSealed())
        return;
      this.CurrentSchema.AllowAdditionalProperties = false;
    }

    internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
    {
      if (!value.HasValue)
        return true;
      JsonSchemaType? nullable1 = value;
      JsonSchemaType jsonSchemaType1 = flag;
      JsonSchemaType? nullable2 = nullable1.HasValue ? new JsonSchemaType?(nullable1.GetValueOrDefault() & jsonSchemaType1) : new JsonSchemaType?();
      JsonSchemaType jsonSchemaType2 = flag;
      if (nullable2.GetValueOrDefault() == jsonSchemaType2 && nullable2.HasValue)
        return true;
      if (flag == JsonSchemaType.Integer)
      {
        JsonSchemaType? nullable3 = value;
        JsonSchemaType? nullable4 = nullable3.HasValue ? new JsonSchemaType?(nullable3.GetValueOrDefault() & JsonSchemaType.Float) : new JsonSchemaType?();
        if ((nullable4.GetValueOrDefault() != JsonSchemaType.Float ? 0 : (nullable4.HasValue ? 1 : 0)) != 0)
          return true;
      }
      return false;
    }

    private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
    {
      JsonSchemaType jsonSchemaType = JsonSchemaType.None;
      if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
      {
        jsonSchemaType = JsonSchemaType.Null;
        if (ReflectionUtils.IsNullableType(type))
          type = Nullable.GetUnderlyingType(type);
      }
      PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(type);
      switch (typeCode)
      {
        case PrimitiveTypeCode.Empty:
        case PrimitiveTypeCode.Object:
          return jsonSchemaType | JsonSchemaType.String;
        case PrimitiveTypeCode.Char:
          return jsonSchemaType | JsonSchemaType.String;
        case PrimitiveTypeCode.Boolean:
          return jsonSchemaType | JsonSchemaType.Boolean;
        case PrimitiveTypeCode.SByte:
        case PrimitiveTypeCode.Int16:
        case PrimitiveTypeCode.UInt16:
        case PrimitiveTypeCode.Int32:
        case PrimitiveTypeCode.Byte:
        case PrimitiveTypeCode.UInt32:
        case PrimitiveTypeCode.Int64:
        case PrimitiveTypeCode.UInt64:
        case PrimitiveTypeCode.BigInteger:
          return jsonSchemaType | JsonSchemaType.Integer;
        case PrimitiveTypeCode.Single:
        case PrimitiveTypeCode.Double:
        case PrimitiveTypeCode.Decimal:
          return jsonSchemaType | JsonSchemaType.Float;
        case PrimitiveTypeCode.DateTime:
        case PrimitiveTypeCode.DateTimeOffset:
          return jsonSchemaType | JsonSchemaType.String;
        case PrimitiveTypeCode.Guid:
        case PrimitiveTypeCode.TimeSpan:
        case PrimitiveTypeCode.Uri:
        case PrimitiveTypeCode.String:
        case PrimitiveTypeCode.Bytes:
          return jsonSchemaType | JsonSchemaType.String;
        case PrimitiveTypeCode.DBNull:
          return jsonSchemaType | JsonSchemaType.Null;
        default:
          throw new JsonException("Unexpected type code '{0}' for type '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) typeCode, (object) type));
      }
    }

    private class TypeSchema
    {
      public Type Type { get; private set; }

      public JsonSchema Schema { get; private set; }

      public TypeSchema(Type type, JsonSchema schema)
      {
        ValidationUtils.ArgumentNotNull((object) type, nameof (type));
        ValidationUtils.ArgumentNotNull((object) schema, nameof (schema));
        this.Type = type;
        this.Schema = schema;
      }
    }
  }
}
