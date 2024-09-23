// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonSerializerInternalReader
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal class JsonSerializerInternalReader : JsonSerializerInternalBase
  {
    public JsonSerializerInternalReader(JsonSerializer serializer)
      : base(serializer)
    {
    }

    public void Populate(JsonReader reader, object target)
    {
      ValidationUtils.ArgumentNotNull(target, nameof (target));
      Type type = target.GetType();
      JsonContract contract1 = this.Serializer._contractResolver.ResolveContract(type);
      if (reader.TokenType == JsonToken.None)
        reader.Read();
      if (reader.TokenType == JsonToken.StartArray)
      {
        JsonArrayContract contract2 = contract1.ContractType == JsonContractType.Array ? (JsonArrayContract) contract1 : throw JsonSerializationException.Create(reader, "Cannot populate JSON array onto type '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type));
        this.PopulateList(contract2.ShouldCreateWrapper ? (IList) contract2.CreateWrapper(target) : (IList) target, reader, contract2, (JsonProperty) null, (string) null);
      }
      else
      {
        if (reader.TokenType != JsonToken.StartObject)
          throw JsonSerializationException.Create(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reader.TokenType));
        this.CheckedRead(reader);
        string id = (string) null;
        if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
        {
          this.CheckedRead(reader);
          id = reader.Value != null ? reader.Value.ToString() : (string) null;
          this.CheckedRead(reader);
        }
        if (contract1.ContractType == JsonContractType.Dictionary)
        {
          JsonDictionaryContract contract3 = (JsonDictionaryContract) contract1;
          this.PopulateDictionary(contract3.ShouldCreateWrapper ? (IDictionary) contract3.CreateWrapper(target) : (IDictionary) target, reader, contract3, (JsonProperty) null, id);
        }
        else
        {
          if (contract1.ContractType != JsonContractType.Object)
            throw JsonSerializationException.Create(reader, "Cannot populate JSON object onto type '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type));
          this.PopulateObject(target, reader, (JsonObjectContract) contract1, (JsonProperty) null, id);
        }
      }
    }

    private JsonContract GetContractSafe(Type type)
    {
      return type == null ? (JsonContract) null : this.Serializer._contractResolver.ResolveContract(type);
    }

    public object Deserialize(JsonReader reader, Type objectType, bool checkAdditionalContent)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      JsonContract contractSafe = this.GetContractSafe(objectType);
      try
      {
        JsonConverter converter = this.GetConverter(contractSafe, (JsonConverter) null, (JsonContainerContract) null, (JsonProperty) null);
        if (reader.TokenType == JsonToken.None && !this.ReadForType(reader, contractSafe, converter != null))
        {
          if (contractSafe != null && !contractSafe.IsNullable)
            throw JsonSerializationException.Create(reader, "No JSON content found and type '{0}' is not nullable.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contractSafe.UnderlyingType));
          return (object) null;
        }
        object obj = converter == null || !converter.CanRead ? this.CreateValueInternal(reader, objectType, contractSafe, (JsonProperty) null, (JsonContainerContract) null, (JsonProperty) null, (object) null) : this.DeserializeConvertable(converter, reader, objectType, (object) null);
        if (checkAdditionalContent && reader.Read() && reader.TokenType != JsonToken.Comment)
          throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
        return obj;
      }
      catch (Exception ex)
      {
        if (this.IsErrorHandled((object) null, contractSafe, (object) null, reader as IJsonLineInfo, reader.Path, ex))
        {
          this.HandleError(reader, false, 0);
          return (object) null;
        }
        this.ClearErrorContext();
        throw;
      }
    }

    private JsonSerializerProxy GetInternalSerializer()
    {
      if (this.InternalSerializer == null)
        this.InternalSerializer = new JsonSerializerProxy(this);
      return this.InternalSerializer;
    }

    private JToken CreateJToken(JsonReader reader, JsonContract contract)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      if (contract != null)
      {
        if (contract.UnderlyingType == typeof (JRaw))
          return (JToken) JRaw.Create(reader);
        if (reader.TokenType == JsonToken.Null && contract.UnderlyingType != typeof (JValue) && contract.UnderlyingType != typeof (JToken))
          return (JToken) null;
      }
      using (JTokenWriter jtokenWriter = new JTokenWriter())
      {
        jtokenWriter.WriteToken(reader);
        return jtokenWriter.Token;
      }
    }

    private JToken CreateJObject(JsonReader reader)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      using (JTokenWriter jtokenWriter = new JTokenWriter())
      {
        jtokenWriter.WriteStartObject();
        do
        {
          if (reader.TokenType == JsonToken.PropertyName)
          {
            string str = (string) reader.Value;
            do
              ;
            while (reader.Read() && reader.TokenType == JsonToken.Comment);
            if (!this.CheckPropertyName(reader, str))
            {
              jtokenWriter.WritePropertyName(str);
              jtokenWriter.WriteToken(reader, true, true);
            }
          }
          else if (reader.TokenType != JsonToken.Comment)
          {
            jtokenWriter.WriteEndObject();
            return jtokenWriter.Token;
          }
        }
        while (reader.Read());
        throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
      }
    }

    private object CreateValueInternal(
      JsonReader reader,
      Type objectType,
      JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerMember,
      object existingValue)
    {
      if (contract != null && contract.ContractType == JsonContractType.Linq)
        return (object) this.CreateJToken(reader, contract);
      do
      {
        switch (reader.TokenType)
        {
          case JsonToken.StartObject:
            return this.CreateObject(reader, objectType, contract, member, containerContract, containerMember, existingValue);
          case JsonToken.StartArray:
            return this.CreateList(reader, objectType, contract, member, existingValue, (string) null);
          case JsonToken.StartConstructor:
            string str = reader.Value.ToString();
            return this.EnsureType(reader, (object) str, CultureInfo.InvariantCulture, contract, objectType);
          case JsonToken.Comment:
            continue;
          case JsonToken.Raw:
            return (object) new JRaw((object) (string) reader.Value);
          case JsonToken.Integer:
          case JsonToken.Float:
          case JsonToken.Boolean:
          case JsonToken.Date:
          case JsonToken.Bytes:
            return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
          case JsonToken.String:
            string s = (string) reader.Value;
            if (JsonSerializerInternalReader.CoerceEmptyStringToNull(objectType, contract, s))
              return (object) null;
            return objectType == typeof (byte[]) ? (object) Convert.FromBase64String(s) : this.EnsureType(reader, (object) s, CultureInfo.InvariantCulture, contract, objectType);
          case JsonToken.Null:
          case JsonToken.Undefined:
            return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
          default:
            throw JsonSerializationException.Create(reader, "Unexpected token while deserializing object: " + (object) reader.TokenType);
        }
      }
      while (reader.Read());
      throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
    }

    private static bool CoerceEmptyStringToNull(Type objectType, JsonContract contract, string s)
    {
      return string.IsNullOrEmpty(s) && objectType != null && objectType != typeof (string) && objectType != typeof (object) && contract != null && contract.IsNullable;
    }

    internal string GetExpectedDescription(JsonContract contract)
    {
      switch (contract.ContractType)
      {
        case JsonContractType.Object:
        case JsonContractType.Dictionary:
          return "JSON object (e.g. {\"name\":\"value\"})";
        case JsonContractType.Array:
          return "JSON array (e.g. [1,2,3])";
        case JsonContractType.Primitive:
          return "JSON primitive value (e.g. string, number, boolean, null)";
        case JsonContractType.String:
          return "JSON string value";
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private JsonConverter GetConverter(
      JsonContract contract,
      JsonConverter memberConverter,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      JsonConverter converter = (JsonConverter) null;
      if (memberConverter != null)
        converter = memberConverter;
      else if (containerProperty != null && containerProperty.ItemConverter != null)
        converter = containerProperty.ItemConverter;
      else if (containerContract != null && containerContract.ItemConverter != null)
        converter = containerContract.ItemConverter;
      else if (contract != null)
      {
        if (contract.Converter != null)
        {
          converter = contract.Converter;
        }
        else
        {
          JsonConverter matchingConverter;
          if ((matchingConverter = this.Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
            converter = matchingConverter;
          else if (contract.InternalConverter != null)
            converter = contract.InternalConverter;
        }
      }
      return converter;
    }

    private object CreateObject(
      JsonReader reader,
      Type objectType,
      JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerMember,
      object existingValue)
    {
      Type objectType1 = objectType;
      string id;
      if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
      {
        this.CheckedRead(reader);
        id = (string) null;
      }
      else if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
      {
        if (!(reader is JTokenReader reader1))
        {
          reader1 = (JTokenReader) JToken.ReadFrom(reader).CreateReader();
          reader1.Culture = reader.Culture;
          reader1.DateFormatString = reader.DateFormatString;
          reader1.DateParseHandling = reader.DateParseHandling;
          reader1.DateTimeZoneHandling = reader.DateTimeZoneHandling;
          reader1.FloatParseHandling = reader.FloatParseHandling;
          reader1.SupportMultipleContent = reader.SupportMultipleContent;
          this.CheckedRead((JsonReader) reader1);
          reader = (JsonReader) reader1;
        }
        object newValue;
        if (this.ReadMetadataPropertiesToken(reader1, ref objectType1, ref contract, member, containerContract, containerMember, existingValue, out newValue, out id))
          return newValue;
      }
      else
      {
        this.CheckedRead(reader);
        object newValue;
        if (this.ReadMetadataProperties(reader, ref objectType1, ref contract, member, containerContract, containerMember, existingValue, out newValue, out id))
          return newValue;
      }
      if (this.HasNoDefinedType(contract))
        return (object) this.CreateJObject(reader);
      switch (contract.ContractType)
      {
        case JsonContractType.Object:
          bool createdFromNonDefaultCreator1 = false;
          JsonObjectContract jsonObjectContract = (JsonObjectContract) contract;
          object newObject = existingValue == null || objectType1 != objectType && !objectType1.IsAssignableFrom(existingValue.GetType()) ? this.CreateNewObject(reader, jsonObjectContract, member, containerMember, id, out createdFromNonDefaultCreator1) : existingValue;
          return createdFromNonDefaultCreator1 ? newObject : this.PopulateObject(newObject, reader, jsonObjectContract, member, id);
        case JsonContractType.Primitive:
          JsonPrimitiveContract contract1 = (JsonPrimitiveContract) contract;
          if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
          {
            this.CheckedRead(reader);
            if (reader.TokenType == JsonToken.StartObject)
              throw JsonSerializationException.Create(reader, "Unexpected token when deserializing primitive value: " + (object) reader.TokenType);
            object valueInternal = this.CreateValueInternal(reader, objectType1, (JsonContract) contract1, member, (JsonContainerContract) null, (JsonProperty) null, existingValue);
            this.CheckedRead(reader);
            return valueInternal;
          }
          break;
        case JsonContractType.Dictionary:
          JsonDictionaryContract contract2 = (JsonDictionaryContract) contract;
          object obj;
          if (existingValue == null)
          {
            bool createdFromNonDefaultCreator2;
            IDictionary newDictionary = this.CreateNewDictionary(reader, contract2, out createdFromNonDefaultCreator2);
            if (createdFromNonDefaultCreator2)
            {
              if (id != null)
                throw JsonSerializationException.Create(reader, "Cannot preserve reference to readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
              if (contract.OnSerializingCallbacks.Count > 0)
                throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
              if (contract.OnErrorCallbacks.Count > 0)
                throw JsonSerializationException.Create(reader, "Cannot call OnError on readonly list, or dictionary created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
              if (!contract2.HasParametrizedCreator)
                throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size dictionary: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
            }
            this.PopulateDictionary(newDictionary, reader, contract2, member, id);
            if (createdFromNonDefaultCreator2)
              return contract2.ParametrizedCreator(new object[1]
              {
                (object) newDictionary
              });
            if (newDictionary is IWrappedDictionary)
              return ((IWrappedDictionary) newDictionary).UnderlyingDictionary;
            obj = (object) newDictionary;
          }
          else
            obj = this.PopulateDictionary(contract2.ShouldCreateWrapper ? (IDictionary) contract2.CreateWrapper(existingValue) : (IDictionary) existingValue, reader, contract2, member, id);
          return obj;
      }
      string message = ("Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object." + Environment.NewLine).FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType1, (object) this.GetExpectedDescription(contract));
      throw JsonSerializationException.Create(reader, message);
    }

    private bool ReadMetadataPropertiesToken(
      JTokenReader reader,
      ref Type objectType,
      ref JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerMember,
      object existingValue,
      out object newValue,
      out string id)
    {
      id = (string) null;
      newValue = (object) null;
      if (reader.TokenType == JsonToken.StartObject)
      {
        JObject currentToken = (JObject) reader.CurrentToken;
        JToken lineInfo1 = currentToken["$ref"];
        if (lineInfo1 != null)
        {
          JToken jtoken = lineInfo1.Type == JTokenType.String || lineInfo1.Type == JTokenType.Null ? (JToken) lineInfo1.Parent : throw JsonSerializationException.Create((IJsonLineInfo) lineInfo1, lineInfo1.Path, "JSON reference {0} property must have a string or null value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) "$ref"), (Exception) null);
          JToken lineInfo2 = (JToken) null;
          if (jtoken.Next != null)
            lineInfo2 = jtoken.Next;
          else if (jtoken.Previous != null)
            lineInfo2 = jtoken.Previous;
          string reference = (string) lineInfo1;
          if (reference != null)
          {
            if (lineInfo2 != null)
              throw JsonSerializationException.Create((IJsonLineInfo) lineInfo2, lineInfo2.Path, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) "$ref"), (Exception) null);
            newValue = this.Serializer.GetReferenceResolver().ResolveReference((object) this, reference);
            if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
              this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) reader, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reference, (object) newValue.GetType())), (Exception) null);
            reader.Skip();
            return true;
          }
        }
        JToken jtoken1 = currentToken["$type"];
        if (jtoken1 != null)
        {
          string qualifiedTypeName = (string) jtoken1;
          JsonReader reader1 = jtoken1.CreateReader();
          this.CheckedRead(reader1);
          this.ResolveTypeName(reader1, ref objectType, ref contract, member, containerContract, containerMember, qualifiedTypeName);
          if (currentToken["$value"] != null)
          {
            while (true)
            {
              this.CheckedRead((JsonReader) reader);
              if (reader.TokenType != JsonToken.PropertyName || !((string) reader.Value == "$value"))
              {
                this.CheckedRead((JsonReader) reader);
                reader.Skip();
              }
              else
                break;
            }
            return false;
          }
        }
        JToken jtoken2 = currentToken["$id"];
        if (jtoken2 != null)
          id = (string) jtoken2;
        JToken jtoken3 = currentToken["$values"];
        if (jtoken3 != null)
        {
          JsonReader reader2 = jtoken3.CreateReader();
          this.CheckedRead(reader2);
          newValue = this.CreateList(reader2, objectType, contract, member, existingValue, id);
          reader.Skip();
          return true;
        }
      }
      this.CheckedRead((JsonReader) reader);
      return false;
    }

    private bool ReadMetadataProperties(
      JsonReader reader,
      ref Type objectType,
      ref JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerMember,
      object existingValue,
      out object newValue,
      out string id)
    {
      id = (string) null;
      newValue = (object) null;
      if (reader.TokenType == JsonToken.PropertyName)
      {
        string str = reader.Value.ToString();
        if (str.Length > 0 && str[0] == '$')
        {
          bool flag;
          do
          {
            string a = reader.Value.ToString();
            if (string.Equals(a, "$ref", StringComparison.Ordinal))
            {
              this.CheckedRead(reader);
              if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
                throw JsonSerializationException.Create(reader, "JSON reference {0} property must have a string or null value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) "$ref"));
              string reference = reader.Value != null ? reader.Value.ToString() : (string) null;
              this.CheckedRead(reader);
              if (reference != null)
              {
                if (reader.TokenType == JsonToken.PropertyName)
                  throw JsonSerializationException.Create(reader, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) "$ref"));
                newValue = this.Serializer.GetReferenceResolver().ResolveReference((object) this, reference);
                if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
                  this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reference, (object) newValue.GetType())), (Exception) null);
                return true;
              }
              flag = true;
            }
            else if (string.Equals(a, "$type", StringComparison.Ordinal))
            {
              this.CheckedRead(reader);
              string qualifiedTypeName = reader.Value.ToString();
              this.ResolveTypeName(reader, ref objectType, ref contract, member, containerContract, containerMember, qualifiedTypeName);
              this.CheckedRead(reader);
              flag = true;
            }
            else if (string.Equals(a, "$id", StringComparison.Ordinal))
            {
              this.CheckedRead(reader);
              id = reader.Value != null ? reader.Value.ToString() : (string) null;
              this.CheckedRead(reader);
              flag = true;
            }
            else
            {
              if (string.Equals(a, "$values", StringComparison.Ordinal))
              {
                this.CheckedRead(reader);
                object list = this.CreateList(reader, objectType, contract, member, existingValue, id);
                this.CheckedRead(reader);
                newValue = list;
                return true;
              }
              flag = false;
            }
          }
          while (flag && reader.TokenType == JsonToken.PropertyName);
        }
      }
      return false;
    }

    private void ResolveTypeName(
      JsonReader reader,
      ref Type objectType,
      ref JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerMember,
      string qualifiedTypeName)
    {
      if (((int) member?.TypeNameHandling ?? (int) containerContract?.ItemTypeNameHandling ?? (int) containerMember?.ItemTypeNameHandling ?? (int) this.Serializer._typeNameHandling) == 0)
        return;
      string typeName;
      string assemblyName;
      ReflectionUtils.SplitFullyQualifiedTypeName(qualifiedTypeName, out typeName, out assemblyName);
      Type type;
      try
      {
        type = this.Serializer._binder.BindToType(assemblyName, typeName);
      }
      catch (Exception ex)
      {
        throw JsonSerializationException.Create(reader, "Error resolving type specified in JSON '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) qualifiedTypeName), ex);
      }
      if (type == null)
        throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' was not resolved.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) qualifiedTypeName));
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
        this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved type '{0}' to {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) qualifiedTypeName, (object) type)), (Exception) null);
      objectType = objectType == null || objectType.IsAssignableFrom(type) ? type : throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type.AssemblyQualifiedName, (object) objectType.AssemblyQualifiedName));
      contract = this.GetContractSafe(type);
    }

    private JsonArrayContract EnsureArrayContract(
      JsonReader reader,
      Type objectType,
      JsonContract contract)
    {
      if (contract == null)
        throw JsonSerializationException.Create(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType));
      if (!(contract is JsonArrayContract jsonArrayContract))
      {
        string message = ("Cannot deserialize the current JSON array (e.g. [1,2,3]) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array." + Environment.NewLine).FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType, (object) this.GetExpectedDescription(contract));
        throw JsonSerializationException.Create(reader, message);
      }
      return jsonArrayContract;
    }

    private void CheckedRead(JsonReader reader)
    {
      if (!reader.Read())
        throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
    }

    private object CreateList(
      JsonReader reader,
      Type objectType,
      JsonContract contract,
      JsonProperty member,
      object existingValue,
      string id)
    {
      if (this.HasNoDefinedType(contract))
        return (object) this.CreateJToken(reader, contract);
      JsonArrayContract contract1 = this.EnsureArrayContract(reader, objectType, contract);
      object list1;
      if (existingValue == null)
      {
        bool createdFromNonDefaultCreator;
        IList list2 = this.CreateNewList(reader, contract1, out createdFromNonDefaultCreator);
        if (createdFromNonDefaultCreator)
        {
          if (id != null)
            throw JsonSerializationException.Create(reader, "Cannot preserve reference to array or readonly list, or list created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
          if (contract.OnSerializingCallbacks.Count > 0)
            throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
          if (contract.OnErrorCallbacks.Count > 0)
            throw JsonSerializationException.Create(reader, "Cannot call OnError on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
          if (!contract1.HasParametrizedCreator && !contract1.IsArray)
            throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size list: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
        }
        if (!contract1.IsMultidimensionalArray)
          this.PopulateList(list2, reader, contract1, member, id);
        else
          this.PopulateMultidimensionalArray(list2, reader, contract1, member, id);
        if (createdFromNonDefaultCreator)
        {
          if (contract1.IsMultidimensionalArray)
            list2 = (IList) CollectionUtils.ToMultidimensionalArray(list2, contract1.CollectionItemType, contract.CreatedType.GetArrayRank());
          else if (contract1.IsArray)
          {
            Array instance = Array.CreateInstance(contract1.CollectionItemType, list2.Count);
            list2.CopyTo(instance, 0);
            list2 = (IList) instance;
          }
          else
            return contract1.ParametrizedCreator(new object[1]
            {
              (object) list2
            });
        }
        else if (list2 is IWrappedCollection)
          return ((IWrappedCollection) list2).UnderlyingCollection;
        list1 = (object) list2;
      }
      else
      {
        if (!contract1.CanDeserialize)
          throw JsonSerializationException.Create(reader, "Cannot populate list type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.CreatedType));
        list1 = this.PopulateList(contract1.ShouldCreateWrapper ? (IList) contract1.CreateWrapper(existingValue) : (IList) existingValue, reader, contract1, member, id);
      }
      return list1;
    }

    private bool HasNoDefinedType(JsonContract contract)
    {
      return contract == null || contract.UnderlyingType == typeof (object) || contract.ContractType == JsonContractType.Linq;
    }

    private object EnsureType(
      JsonReader reader,
      object value,
      CultureInfo culture,
      JsonContract contract,
      Type targetType)
    {
      if (targetType == null || ReflectionUtils.GetObjectType(value) == targetType)
        return value;
      if (value == null)
      {
        if (contract.IsNullable)
          return (object) null;
      }
      try
      {
        if (!contract.IsConvertable)
          return ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
        JsonPrimitiveContract primitiveContract = (JsonPrimitiveContract) contract;
        if (contract.IsEnum)
        {
          if (value is string)
            return Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
          if (ConvertUtils.IsInteger((object) primitiveContract.TypeCode))
            return Enum.ToObject(contract.NonNullableUnderlyingType, value);
        }
        return Convert.ChangeType(value, contract.NonNullableUnderlyingType, (IFormatProvider) culture);
      }
      catch (Exception ex)
      {
        throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) MiscellaneousUtils.FormatValueForPrint(value), (object) targetType), ex);
      }
    }

    private bool SetPropertyValue(
      JsonProperty property,
      JsonConverter propertyConverter,
      JsonContainerContract containerContract,
      JsonProperty containerProperty,
      JsonReader reader,
      object target)
    {
      bool useExistingValue;
      object currentValue;
      JsonContract propertyContract;
      bool gottenCurrentValue;
      if (this.CalculatePropertyDetails(property, ref propertyConverter, containerContract, containerProperty, reader, target, out useExistingValue, out currentValue, out propertyContract, out gottenCurrentValue))
        return false;
      object obj;
      if (propertyConverter != null && propertyConverter.CanRead)
      {
        if (!gottenCurrentValue && target != null && property.Readable)
          currentValue = property.ValueProvider.GetValue(target);
        obj = this.DeserializeConvertable(propertyConverter, reader, property.PropertyType, currentValue);
      }
      else
        obj = this.CreateValueInternal(reader, property.PropertyType, propertyContract, property, containerContract, containerProperty, useExistingValue ? currentValue : (object) null);
      if (useExistingValue && obj == currentValue || !this.ShouldSetPropertyValue(property, obj))
        return useExistingValue;
      property.ValueProvider.SetValue(target, obj);
      if (property.SetIsSpecified != null)
      {
        if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
          this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "IsSpecified for property '{0}' on {1} set to true.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) property.PropertyName, (object) property.DeclaringType)), (Exception) null);
        property.SetIsSpecified(target, (object) true);
      }
      return true;
    }

    private bool CalculatePropertyDetails(
      JsonProperty property,
      ref JsonConverter propertyConverter,
      JsonContainerContract containerContract,
      JsonProperty containerProperty,
      JsonReader reader,
      object target,
      out bool useExistingValue,
      out object currentValue,
      out JsonContract propertyContract,
      out bool gottenCurrentValue)
    {
      currentValue = (object) null;
      useExistingValue = false;
      propertyContract = (JsonContract) null;
      gottenCurrentValue = false;
      if (property.Ignored)
        return true;
      JsonToken tokenType = reader.TokenType;
      if (property.PropertyContract == null)
        property.PropertyContract = this.GetContractSafe(property.PropertyType);
      if (property.ObjectCreationHandling.GetValueOrDefault(this.Serializer._objectCreationHandling) != ObjectCreationHandling.Replace && (tokenType == JsonToken.StartArray || tokenType == JsonToken.StartObject) && property.Readable)
      {
        currentValue = property.ValueProvider.GetValue(target);
        gottenCurrentValue = true;
        if (currentValue != null)
        {
          propertyContract = this.GetContractSafe(currentValue.GetType());
          useExistingValue = !propertyContract.IsReadOnlyOrFixedSize && !propertyContract.UnderlyingType.IsValueType();
        }
      }
      if (!property.Writable && !useExistingValue || property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) == NullValueHandling.Ignore && tokenType == JsonToken.Null || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && !this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && JsonTokenUtils.IsPrimitiveToken(tokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.GetResolvedDefaultValue()))
        return true;
      if (currentValue == null)
      {
        propertyContract = property.PropertyContract;
      }
      else
      {
        propertyContract = this.GetContractSafe(currentValue.GetType());
        if (propertyContract != property.PropertyContract)
          propertyConverter = this.GetConverter(propertyContract, property.MemberConverter, containerContract, containerProperty);
      }
      return false;
    }

    private void AddReference(JsonReader reader, string id, object value)
    {
      try
      {
        if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
          this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Read object reference Id '{0}' for {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) id, (object) value.GetType())), (Exception) null);
        this.Serializer.GetReferenceResolver().AddReference((object) this, id, value);
      }
      catch (Exception ex)
      {
        throw JsonSerializationException.Create(reader, "Error reading object reference '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) id), ex);
      }
    }

    private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
    {
      return (value & flag) == flag;
    }

    private bool ShouldSetPropertyValue(JsonProperty property, object value)
    {
      return (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) != NullValueHandling.Ignore || value != null) && (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) || !MiscellaneousUtils.ValueEquals(value, property.GetResolvedDefaultValue())) && property.Writable;
    }

    private IList CreateNewList(
      JsonReader reader,
      JsonArrayContract contract,
      out bool createdFromNonDefaultCreator)
    {
      if (!contract.CanDeserialize)
        throw JsonSerializationException.Create(reader, "Cannot create and populate list type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.CreatedType));
      if (contract.IsReadOnlyOrFixedSize)
      {
        createdFromNonDefaultCreator = true;
        IList list = contract.CreateTemporaryCollection();
        if (contract.ShouldCreateWrapper)
          list = (IList) contract.CreateWrapper((object) list);
        return list;
      }
      if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
      {
        object list = contract.DefaultCreator();
        if (contract.ShouldCreateWrapper)
          list = (object) contract.CreateWrapper(list);
        createdFromNonDefaultCreator = false;
        return (IList) list;
      }
      if (contract.HasParametrizedCreator)
      {
        createdFromNonDefaultCreator = true;
        return contract.CreateTemporaryCollection();
      }
      if (!contract.IsInstantiable)
        throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
      throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
    }

    private IDictionary CreateNewDictionary(
      JsonReader reader,
      JsonDictionaryContract contract,
      out bool createdFromNonDefaultCreator)
    {
      if (contract.IsReadOnlyOrFixedSize)
      {
        createdFromNonDefaultCreator = true;
        return contract.CreateTemporaryDictionary();
      }
      if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
      {
        object dictionary = contract.DefaultCreator();
        if (contract.ShouldCreateWrapper)
          dictionary = (object) contract.CreateWrapper(dictionary);
        createdFromNonDefaultCreator = false;
        return (IDictionary) dictionary;
      }
      if (contract.HasParametrizedCreator)
      {
        createdFromNonDefaultCreator = true;
        return contract.CreateTemporaryDictionary();
      }
      if (!contract.IsInstantiable)
        throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
      throw JsonSerializationException.Create(reader, "Unable to find a default constructor to use for type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType));
    }

    private void OnDeserializing(JsonReader reader, JsonContract contract, object value)
    {
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType)), (Exception) null);
      contract.InvokeOnDeserializing(value, this.Serializer._context);
    }

    private void OnDeserialized(JsonReader reader, JsonContract contract, object value)
    {
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType)), (Exception) null);
      contract.InvokeOnDeserialized(value, this.Serializer._context);
    }

    private object PopulateDictionary(
      IDictionary dictionary,
      JsonReader reader,
      JsonDictionaryContract contract,
      JsonProperty containerProperty,
      string id)
    {
      object currentObject = dictionary is IWrappedDictionary wrappedDictionary ? wrappedDictionary.UnderlyingDictionary : (object) dictionary;
      if (id != null)
        this.AddReference(reader, id, currentObject);
      this.OnDeserializing(reader, (JsonContract) contract, currentObject);
      int depth = reader.Depth;
      if (contract.KeyContract == null)
        contract.KeyContract = this.GetContractSafe(contract.DictionaryKeyType);
      if (contract.ItemContract == null)
        contract.ItemContract = this.GetContractSafe(contract.DictionaryValueType);
      JsonConverter converter = contract.ItemConverter ?? this.GetConverter(contract.ItemContract, (JsonConverter) null, (JsonContainerContract) contract, containerProperty);
      PrimitiveTypeCode primitiveTypeCode = contract.KeyContract is JsonPrimitiveContract ? ((JsonPrimitiveContract) contract.KeyContract).TypeCode : PrimitiveTypeCode.Empty;
      bool flag = false;
      do
      {
        switch (reader.TokenType)
        {
          case JsonToken.PropertyName:
            object obj1 = reader.Value;
            if (!this.CheckPropertyName(reader, obj1.ToString()))
            {
              try
              {
                try
                {
                  DateParseHandling dateParseHandling;
                  switch (primitiveTypeCode)
                  {
                    case PrimitiveTypeCode.DateTime:
                    case PrimitiveTypeCode.DateTimeNullable:
                      dateParseHandling = DateParseHandling.DateTime;
                      break;
                    case PrimitiveTypeCode.DateTimeOffset:
                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                      dateParseHandling = DateParseHandling.DateTimeOffset;
                      break;
                    default:
                      dateParseHandling = DateParseHandling.None;
                      break;
                  }
                  object dt;
                  obj1 = dateParseHandling == DateParseHandling.None || !DateTimeUtils.TryParseDateTime(obj1.ToString(), dateParseHandling, reader.DateTimeZoneHandling, reader.DateFormatString, reader.Culture, out dt) ? this.EnsureType(reader, obj1, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType) : dt;
                }
                catch (Exception ex)
                {
                  throw JsonSerializationException.Create(reader, "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, reader.Value, (object) contract.DictionaryKeyType), ex);
                }
                if (!this.ReadForType(reader, contract.ItemContract, converter != null))
                  throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
                object obj2 = converter == null || !converter.CanRead ? this.CreateValueInternal(reader, contract.DictionaryValueType, contract.ItemContract, (JsonProperty) null, (JsonContainerContract) contract, containerProperty, (object) null) : this.DeserializeConvertable(converter, reader, contract.DictionaryValueType, (object) null);
                dictionary[obj1] = obj2;
                goto case JsonToken.Comment;
              }
              catch (Exception ex)
              {
                if (this.IsErrorHandled(currentObject, (JsonContract) contract, obj1, reader as IJsonLineInfo, reader.Path, ex))
                {
                  this.HandleError(reader, true, depth);
                  goto case JsonToken.Comment;
                }
                else
                  throw;
              }
            }
            else
              goto case JsonToken.Comment;
          case JsonToken.Comment:
            continue;
          case JsonToken.EndObject:
            flag = true;
            goto case JsonToken.Comment;
          default:
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + (object) reader.TokenType);
        }
      }
      while (!flag && reader.Read());
      if (!flag)
        this.ThrowUnexpectedEndException(reader, (JsonContract) contract, currentObject, "Unexpected end when deserializing object.");
      this.OnDeserialized(reader, (JsonContract) contract, currentObject);
      return currentObject;
    }

    private object PopulateMultidimensionalArray(
      IList list,
      JsonReader reader,
      JsonArrayContract contract,
      JsonProperty containerProperty,
      string id)
    {
      int arrayRank = contract.UnderlyingType.GetArrayRank();
      if (id != null)
        this.AddReference(reader, id, (object) list);
      this.OnDeserializing(reader, (JsonContract) contract, (object) list);
      JsonContract contractSafe = this.GetContractSafe(contract.CollectionItemType);
      JsonConverter converter = this.GetConverter(contractSafe, (JsonConverter) null, (JsonContainerContract) contract, containerProperty);
      int? nullable1 = new int?();
      Stack<IList> listStack = new Stack<IList>();
      listStack.Push(list);
      IList list1 = list;
      bool flag = false;
      do
      {
        int depth = reader.Depth;
        if (listStack.Count == arrayRank)
        {
          try
          {
            if (this.ReadForType(reader, contractSafe, converter != null))
            {
              switch (reader.TokenType)
              {
                case JsonToken.Comment:
                  break;
                case JsonToken.EndArray:
                  listStack.Pop();
                  list1 = listStack.Peek();
                  nullable1 = new int?();
                  break;
                default:
                  object obj = converter == null || !converter.CanRead ? this.CreateValueInternal(reader, contract.CollectionItemType, contractSafe, (JsonProperty) null, (JsonContainerContract) contract, containerProperty, (object) null) : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, (object) null);
                  list1.Add(obj);
                  break;
              }
            }
            else
              break;
          }
          catch (Exception ex)
          {
            JsonPosition position1 = reader.GetPosition(depth);
            if (this.IsErrorHandled((object) list, (JsonContract) contract, (object) position1.Position, reader as IJsonLineInfo, reader.Path, ex))
            {
              this.HandleError(reader, true, depth);
              if (nullable1.HasValue)
              {
                int? nullable2 = nullable1;
                int position2 = position1.Position;
                if ((nullable2.GetValueOrDefault() != position2 ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
                  throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
              }
              nullable1 = new int?(position1.Position);
            }
            else
              throw;
          }
        }
        else if (reader.Read())
        {
          switch (reader.TokenType)
          {
            case JsonToken.StartArray:
              IList list2 = (IList) new List<object>();
              list1.Add((object) list2);
              listStack.Push(list2);
              list1 = list2;
              break;
            case JsonToken.Comment:
              break;
            case JsonToken.EndArray:
              listStack.Pop();
              if (listStack.Count > 0)
              {
                list1 = listStack.Peek();
                break;
              }
              flag = true;
              break;
            default:
              throw JsonSerializationException.Create(reader, "Unexpected token when deserializing multidimensional array: " + (object) reader.TokenType);
          }
        }
        else
          break;
      }
      while (!flag);
      if (!flag)
        this.ThrowUnexpectedEndException(reader, (JsonContract) contract, (object) list, "Unexpected end when deserializing array.");
      this.OnDeserialized(reader, (JsonContract) contract, (object) list);
      return (object) list;
    }

    private void ThrowUnexpectedEndException(
      JsonReader reader,
      JsonContract contract,
      object currentObject,
      string message)
    {
      try
      {
        throw JsonSerializationException.Create(reader, message);
      }
      catch (Exception ex)
      {
        if (this.IsErrorHandled(currentObject, contract, (object) null, reader as IJsonLineInfo, reader.Path, ex))
          this.HandleError(reader, false, 0);
        else
          throw;
      }
    }

    private object PopulateList(
      IList list,
      JsonReader reader,
      JsonArrayContract contract,
      JsonProperty containerProperty,
      string id)
    {
      object currentObject = list is IWrappedCollection wrappedCollection ? wrappedCollection.UnderlyingCollection : (object) list;
      if (id != null)
        this.AddReference(reader, id, currentObject);
      if (list.IsFixedSize)
      {
        reader.Skip();
        return currentObject;
      }
      this.OnDeserializing(reader, (JsonContract) contract, currentObject);
      int depth = reader.Depth;
      if (contract.ItemContract == null)
        contract.ItemContract = this.GetContractSafe(contract.CollectionItemType);
      JsonConverter converter = this.GetConverter(contract.ItemContract, (JsonConverter) null, (JsonContainerContract) contract, containerProperty);
      int? nullable1 = new int?();
      bool flag = false;
      do
      {
        try
        {
          if (this.ReadForType(reader, contract.ItemContract, converter != null))
          {
            switch (reader.TokenType)
            {
              case JsonToken.Comment:
                break;
              case JsonToken.EndArray:
                flag = true;
                break;
              default:
                object obj = converter == null || !converter.CanRead ? this.CreateValueInternal(reader, contract.CollectionItemType, contract.ItemContract, (JsonProperty) null, (JsonContainerContract) contract, containerProperty, (object) null) : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, (object) null);
                list.Add(obj);
                break;
            }
          }
          else
            break;
        }
        catch (Exception ex)
        {
          JsonPosition position1 = reader.GetPosition(depth);
          if (this.IsErrorHandled(currentObject, (JsonContract) contract, (object) position1.Position, reader as IJsonLineInfo, reader.Path, ex))
          {
            this.HandleError(reader, true, depth);
            if (nullable1.HasValue)
            {
              int? nullable2 = nullable1;
              int position2 = position1.Position;
              if ((nullable2.GetValueOrDefault() != position2 ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
                throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
            }
            nullable1 = new int?(position1.Position);
          }
          else
            throw;
        }
      }
      while (!flag);
      if (!flag)
        this.ThrowUnexpectedEndException(reader, (JsonContract) contract, currentObject, "Unexpected end when deserializing array.");
      this.OnDeserialized(reader, (JsonContract) contract, currentObject);
      return currentObject;
    }

    private object CreateObjectUsingCreatorWithParameters(
      JsonReader reader,
      JsonObjectContract contract,
      JsonProperty containerProperty,
      ObjectConstructor<object> creator,
      string id)
    {
      ValidationUtils.ArgumentNotNull((object) creator, nameof (creator));
      Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary1 = contract.HasRequiredOrDefaultValueProperties || this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate) ? contract.Properties.ToDictionary<JsonProperty, JsonProperty, JsonSerializerInternalReader.PropertyPresence>((Func<JsonProperty, JsonProperty>) (m => m), (Func<JsonProperty, JsonSerializerInternalReader.PropertyPresence>) (m => JsonSerializerInternalReader.PropertyPresence.None)) : (Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence>) null;
      Type underlyingType = contract.UnderlyingType;
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
      {
        string str = string.Join(", ", contract.CreatorParameters.Select<JsonProperty, string>((Func<JsonProperty, string>) (p => p.PropertyName)).ToArray<string>());
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using creator with parameters: {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType, (object) str)), (Exception) null);
      }
      IDictionary<string, object> extensionData;
      IDictionary<JsonProperty, object> dictionary2 = this.ResolvePropertyAndCreatorValues(contract, containerProperty, reader, underlyingType, out extensionData);
      object[] objArray = new object[contract.CreatorParameters.Count];
      IDictionary<JsonProperty, object> dictionary3 = (IDictionary<JsonProperty, object>) new Dictionary<JsonProperty, object>();
      foreach (KeyValuePair<JsonProperty, object> keyValuePair in (IEnumerable<KeyValuePair<JsonProperty, object>>) dictionary2)
      {
        JsonProperty key1 = keyValuePair.Key;
        JsonProperty jsonProperty = !contract.CreatorParameters.Contains(key1) ? contract.CreatorParameters.ForgivingCaseSensitiveFind<JsonProperty>((Func<JsonProperty, string>) (p => p.PropertyName), key1.UnderlyingName) : key1;
        if (jsonProperty != null)
        {
          int index = contract.CreatorParameters.IndexOf(jsonProperty);
          objArray[index] = keyValuePair.Value;
        }
        else
          dictionary3.Add(keyValuePair);
        if (dictionary1 != null)
        {
          JsonProperty key2 = dictionary1.Keys.ForgivingCaseSensitiveFind<JsonProperty>((Func<JsonProperty, string>) (p => p.PropertyName), key1.PropertyName);
          if (key2 != null)
          {
            object s = keyValuePair.Value;
            JsonSerializerInternalReader.PropertyPresence propertyPresence = s != null ? (!(s is string) ? JsonSerializerInternalReader.PropertyPresence.Value : (JsonSerializerInternalReader.CoerceEmptyStringToNull(key1.PropertyType, key1.PropertyContract, (string) s) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value)) : JsonSerializerInternalReader.PropertyPresence.Null;
            dictionary1[key2] = propertyPresence;
          }
        }
      }
      if (dictionary1 != null)
      {
        foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in dictionary1)
        {
          JsonProperty property = keyValuePair.Key;
          JsonSerializerInternalReader.PropertyPresence propertyPresence = keyValuePair.Value;
          if (!property.Ignored && (propertyPresence == JsonSerializerInternalReader.PropertyPresence.None || propertyPresence == JsonSerializerInternalReader.PropertyPresence.Null))
          {
            if (property.PropertyContract == null)
              property.PropertyContract = this.GetContractSafe(property.PropertyType);
            if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate))
            {
              int index = contract.CreatorParameters.IndexOf<JsonProperty>((Func<JsonProperty, bool>) (p => p.PropertyName == property.PropertyName));
              if (index != -1)
                objArray[index] = this.EnsureType(reader, property.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, property.PropertyContract, property.PropertyType);
            }
          }
        }
      }
      object creatorWithParameters = creator(objArray);
      if (id != null)
        this.AddReference(reader, id, creatorWithParameters);
      this.OnDeserializing(reader, (JsonContract) contract, creatorWithParameters);
      foreach (KeyValuePair<JsonProperty, object> keyValuePair in (IEnumerable<KeyValuePair<JsonProperty, object>>) dictionary3)
      {
        JsonProperty key = keyValuePair.Key;
        object obj1 = keyValuePair.Value;
        if (this.ShouldSetPropertyValue(key, obj1))
          key.ValueProvider.SetValue(creatorWithParameters, obj1);
        else if (!key.Writable && obj1 != null)
        {
          JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(key.PropertyType);
          if (jsonContract.ContractType == JsonContractType.Array)
          {
            JsonArrayContract jsonArrayContract = (JsonArrayContract) jsonContract;
            object list = key.ValueProvider.GetValue(creatorWithParameters);
            if (list != null)
            {
              IWrappedCollection wrapper = jsonArrayContract.CreateWrapper(list);
              foreach (object obj2 in (IEnumerable) jsonArrayContract.CreateWrapper(obj1))
                wrapper.Add(obj2);
            }
          }
          else if (jsonContract.ContractType == JsonContractType.Dictionary)
          {
            JsonDictionaryContract dictionaryContract = (JsonDictionaryContract) jsonContract;
            object dictionary4 = key.ValueProvider.GetValue(creatorWithParameters);
            if (dictionary4 != null)
            {
              IDictionary dictionary5 = dictionaryContract.ShouldCreateWrapper ? (IDictionary) dictionaryContract.CreateWrapper(dictionary4) : (IDictionary) dictionary4;
              foreach (DictionaryEntry dictionaryEntry in dictionaryContract.ShouldCreateWrapper ? (IDictionary) dictionaryContract.CreateWrapper(obj1) : (IDictionary) obj1)
                dictionary5.Add(dictionaryEntry.Key, dictionaryEntry.Value);
            }
          }
        }
      }
      if (extensionData != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) extensionData)
          contract.ExtensionDataSetter(creatorWithParameters, keyValuePair.Key, keyValuePair.Value);
      }
      this.EndObject(creatorWithParameters, reader, contract, reader.Depth, dictionary1);
      this.OnDeserialized(reader, (JsonContract) contract, creatorWithParameters);
      return creatorWithParameters;
    }

    private object DeserializeConvertable(
      JsonConverter converter,
      JsonReader reader,
      Type objectType,
      object existingValue)
    {
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0} with converter {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType, (object) converter.GetType())), (Exception) null);
      object obj = converter.ReadJson(reader, objectType, existingValue, (JsonSerializer) this.GetInternalSerializer());
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0} with converter {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType, (object) converter.GetType())), (Exception) null);
      return obj;
    }

    private IDictionary<JsonProperty, object> ResolvePropertyAndCreatorValues(
      JsonObjectContract contract,
      JsonProperty containerProperty,
      JsonReader reader,
      Type objectType,
      out IDictionary<string, object> extensionData)
    {
      extensionData = contract.ExtensionDataSetter != null ? (IDictionary<string, object>) new Dictionary<string, object>() : (IDictionary<string, object>) null;
      IDictionary<JsonProperty, object> dictionary = (IDictionary<JsonProperty, object>) new Dictionary<JsonProperty, object>();
      bool flag = false;
      do
      {
        switch (reader.TokenType)
        {
          case JsonToken.PropertyName:
            string str = reader.Value.ToString();
            JsonProperty jsonProperty = contract.CreatorParameters.GetClosestMatchProperty(str) ?? contract.Properties.GetClosestMatchProperty(str);
            if (jsonProperty != null)
            {
              if (jsonProperty.PropertyContract == null)
                jsonProperty.PropertyContract = this.GetContractSafe(jsonProperty.PropertyType);
              JsonConverter converter = this.GetConverter(jsonProperty.PropertyContract, jsonProperty.MemberConverter, (JsonContainerContract) contract, containerProperty);
              if (!this.ReadForType(reader, jsonProperty.PropertyContract, converter != null))
                throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str));
              if (!jsonProperty.Ignored)
              {
                object obj = converter == null || !converter.CanRead ? this.CreateValueInternal(reader, jsonProperty.PropertyType, jsonProperty.PropertyContract, jsonProperty, (JsonContainerContract) contract, containerProperty, (object) null) : this.DeserializeConvertable(converter, reader, jsonProperty.PropertyType, (object) null);
                dictionary[jsonProperty] = obj;
                goto case JsonToken.Comment;
              }
            }
            else
            {
              if (!reader.Read())
                throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str));
              if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
                this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str, (object) contract.UnderlyingType)), (Exception) null);
              if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
                throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str, (object) objectType.Name));
            }
            if (extensionData != null)
            {
              object valueInternal = this.CreateValueInternal(reader, (Type) null, (JsonContract) null, (JsonProperty) null, (JsonContainerContract) contract, containerProperty, (object) null);
              extensionData[str] = valueInternal;
              goto case JsonToken.Comment;
            }
            else
            {
              reader.Skip();
              goto case JsonToken.Comment;
            }
          case JsonToken.Comment:
            continue;
          case JsonToken.EndObject:
            flag = true;
            goto case JsonToken.Comment;
          default:
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + (object) reader.TokenType);
        }
      }
      while (!flag && reader.Read());
      return dictionary;
    }

    private bool ReadForType(JsonReader reader, JsonContract contract, bool hasConverter)
    {
      if (hasConverter)
        return reader.Read();
      switch (contract != null ? (int) contract.InternalReadType : 0)
      {
        case 0:
          while (reader.Read())
          {
            if (reader.TokenType != JsonToken.Comment)
              return true;
          }
          return false;
        case 1:
          reader.ReadAsInt32();
          break;
        case 2:
          reader.ReadAsBytes();
          break;
        case 3:
          reader.ReadAsString();
          break;
        case 4:
          reader.ReadAsDecimal();
          break;
        case 5:
          reader.ReadAsDateTime();
          break;
        case 6:
          reader.ReadAsDateTimeOffset();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return reader.TokenType != JsonToken.None;
    }

    public object CreateNewObject(
      JsonReader reader,
      JsonObjectContract objectContract,
      JsonProperty containerMember,
      JsonProperty containerProperty,
      string id,
      out bool createdFromNonDefaultCreator)
    {
      object newObject = (object) null;
      if (objectContract.OverrideCreator != null)
      {
        if (objectContract.CreatorParameters.Count > 0)
        {
          createdFromNonDefaultCreator = true;
          return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.OverrideCreator, id);
        }
        newObject = objectContract.OverrideCreator(new object[0]);
      }
      else if (objectContract.DefaultCreator != null && (!objectContract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor || objectContract.ParametrizedCreator == null))
        newObject = objectContract.DefaultCreator();
      else if (objectContract.ParametrizedCreator != null)
      {
        createdFromNonDefaultCreator = true;
        return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.ParametrizedCreator, id);
      }
      if (newObject == null)
      {
        if (!objectContract.IsInstantiable)
          throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectContract.UnderlyingType));
        throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectContract.UnderlyingType));
      }
      createdFromNonDefaultCreator = false;
      return newObject;
    }

    private object PopulateObject(
      object newObject,
      JsonReader reader,
      JsonObjectContract contract,
      JsonProperty member,
      string id)
    {
      this.OnDeserializing(reader, (JsonContract) contract, newObject);
      Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary = contract.HasRequiredOrDefaultValueProperties || this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate) ? contract.Properties.ToDictionary<JsonProperty, JsonProperty, JsonSerializerInternalReader.PropertyPresence>((Func<JsonProperty, JsonProperty>) (m => m), (Func<JsonProperty, JsonSerializerInternalReader.PropertyPresence>) (m => JsonSerializerInternalReader.PropertyPresence.None)) : (Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence>) null;
      if (id != null)
        this.AddReference(reader, id, newObject);
      int depth = reader.Depth;
      bool flag = false;
      do
      {
        switch (reader.TokenType)
        {
          case JsonToken.PropertyName:
            string str = reader.Value.ToString();
            if (!this.CheckPropertyName(reader, str))
            {
              try
              {
                JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(str);
                if (closestMatchProperty == null)
                {
                  if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
                    this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str, (object) contract.UnderlyingType)), (Exception) null);
                  if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
                    throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str, (object) contract.UnderlyingType.Name));
                  if (reader.Read())
                  {
                    this.SetExtensionData(contract, member, reader, str, newObject);
                    goto case JsonToken.Comment;
                  }
                  else
                    goto case JsonToken.Comment;
                }
                else
                {
                  if (closestMatchProperty.PropertyContract == null)
                    closestMatchProperty.PropertyContract = this.GetContractSafe(closestMatchProperty.PropertyType);
                  JsonConverter propertyConverter = (JsonConverter) null;
                  if (!closestMatchProperty.Ignored)
                    propertyConverter = this.GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter, (JsonContainerContract) contract, member);
                  if (!this.ReadForType(reader, closestMatchProperty.PropertyContract, propertyConverter != null))
                    throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str));
                  this.SetPropertyPresence(reader, closestMatchProperty, dictionary);
                  if (!this.SetPropertyValue(closestMatchProperty, propertyConverter, (JsonContainerContract) contract, member, reader, newObject))
                  {
                    this.SetExtensionData(contract, member, reader, str, newObject);
                    goto case JsonToken.Comment;
                  }
                  else
                    goto case JsonToken.Comment;
                }
              }
              catch (Exception ex)
              {
                if (this.IsErrorHandled(newObject, (JsonContract) contract, (object) str, reader as IJsonLineInfo, reader.Path, ex))
                {
                  this.HandleError(reader, true, depth);
                  goto case JsonToken.Comment;
                }
                else
                  throw;
              }
            }
            else
              goto case JsonToken.Comment;
          case JsonToken.Comment:
            continue;
          case JsonToken.EndObject:
            flag = true;
            goto case JsonToken.Comment;
          default:
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + (object) reader.TokenType);
        }
      }
      while (!flag && reader.Read());
      if (!flag)
        this.ThrowUnexpectedEndException(reader, (JsonContract) contract, newObject, "Unexpected end when deserializing object.");
      this.EndObject(newObject, reader, contract, depth, dictionary);
      this.OnDeserialized(reader, (JsonContract) contract, newObject);
      return newObject;
    }

    private bool CheckPropertyName(JsonReader reader, string memberName)
    {
      if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
      {
        switch (memberName)
        {
          case "$id":
          case "$ref":
          case "$type":
          case "$values":
            reader.Skip();
            return true;
        }
      }
      return false;
    }

    private void SetExtensionData(
      JsonObjectContract contract,
      JsonProperty member,
      JsonReader reader,
      string memberName,
      object o)
    {
      if (contract.ExtensionDataSetter != null)
      {
        try
        {
          object valueInternal = this.CreateValueInternal(reader, (Type) null, (JsonContract) null, (JsonProperty) null, (JsonContainerContract) contract, member, (object) null);
          contract.ExtensionDataSetter(o, memberName, valueInternal);
        }
        catch (Exception ex)
        {
          throw JsonSerializationException.Create(reader, "Error setting value in extension data for type '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType), ex);
        }
      }
      else
        reader.Skip();
    }

    private void EndObject(
      object newObject,
      JsonReader reader,
      JsonObjectContract contract,
      int initialDepth,
      Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> propertiesPresence)
    {
      if (propertiesPresence == null)
        return;
      foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in propertiesPresence)
      {
        JsonProperty key = keyValuePair.Key;
        JsonSerializerInternalReader.PropertyPresence propertyPresence = keyValuePair.Value;
        switch (propertyPresence)
        {
          case JsonSerializerInternalReader.PropertyPresence.None:
          case JsonSerializerInternalReader.PropertyPresence.Null:
            try
            {
              Required required = (Required) ((int) key._required ?? (int) contract.ItemRequired ?? 0);
              switch (propertyPresence)
              {
                case JsonSerializerInternalReader.PropertyPresence.None:
                  if (required == Required.AllowNull || required == Required.Always)
                    throw JsonSerializationException.Create(reader, "Required property '{0}' not found in JSON.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) key.PropertyName));
                  if (!key.Ignored)
                  {
                    if (key.PropertyContract == null)
                      key.PropertyContract = this.GetContractSafe(key.PropertyType);
                    if (this.HasFlag(key.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate))
                    {
                      if (key.Writable)
                      {
                        key.ValueProvider.SetValue(newObject, this.EnsureType(reader, key.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, key.PropertyContract, key.PropertyType));
                        continue;
                      }
                      continue;
                    }
                    continue;
                  }
                  continue;
                case JsonSerializerInternalReader.PropertyPresence.Null:
                  if (required == Required.Always)
                    throw JsonSerializationException.Create(reader, "Required property '{0}' expects a value but got null.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) key.PropertyName));
                  continue;
                default:
                  continue;
              }
            }
            catch (Exception ex)
            {
              if (this.IsErrorHandled(newObject, (JsonContract) contract, (object) key.PropertyName, reader as IJsonLineInfo, reader.Path, ex))
              {
                this.HandleError(reader, true, initialDepth);
                continue;
              }
              throw;
            }
          default:
            continue;
        }
      }
    }

    private void SetPropertyPresence(
      JsonReader reader,
      JsonProperty property,
      Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
    {
      if (property == null || requiredProperties == null)
        return;
      JsonSerializerInternalReader.PropertyPresence propertyPresence;
      switch (reader.TokenType)
      {
        case JsonToken.String:
          propertyPresence = JsonSerializerInternalReader.CoerceEmptyStringToNull(property.PropertyType, property.PropertyContract, (string) reader.Value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value;
          break;
        case JsonToken.Null:
        case JsonToken.Undefined:
          propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
          break;
        default:
          propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
          break;
      }
      requiredProperties[property] = propertyPresence;
    }

    private void HandleError(JsonReader reader, bool readPastError, int initialDepth)
    {
      this.ClearErrorContext();
      if (!readPastError)
        return;
      reader.Skip();
      do
        ;
      while (reader.Depth > initialDepth + 1 && reader.Read());
    }

    internal enum PropertyPresence
    {
      None,
      Null,
      Value,
    }
  }
}
