// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonSerializerInternalWriter
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
using System.IO;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
  {
    private JsonContract _rootContract;
    private int _rootLevel;
    private readonly List<object> _serializeStack = new List<object>();

    public JsonSerializerInternalWriter(JsonSerializer serializer)
      : base(serializer)
    {
    }

    public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
    {
      if (jsonWriter == null)
        throw new ArgumentNullException(nameof (jsonWriter));
      this._rootContract = objectType != null ? this.Serializer._contractResolver.ResolveContract(objectType) : (JsonContract) null;
      this._rootLevel = this._serializeStack.Count + 1;
      JsonContract contractSafe = this.GetContractSafe(value);
      try
      {
        if (this.ShouldWriteReference(value, (JsonProperty) null, contractSafe, (JsonContainerContract) null, (JsonProperty) null))
          this.WriteReference(jsonWriter, value);
        else
          this.SerializeValue(jsonWriter, value, contractSafe, (JsonProperty) null, (JsonContainerContract) null, (JsonProperty) null);
      }
      catch (Exception ex)
      {
        if (this.IsErrorHandled((object) null, contractSafe, (object) null, (IJsonLineInfo) null, jsonWriter.Path, ex))
        {
          this.HandleError(jsonWriter, 0);
        }
        else
        {
          this.ClearErrorContext();
          throw;
        }
      }
      finally
      {
        this._rootContract = (JsonContract) null;
      }
    }

    private JsonSerializerProxy GetInternalSerializer()
    {
      if (this.InternalSerializer == null)
        this.InternalSerializer = new JsonSerializerProxy(this);
      return this.InternalSerializer;
    }

    private JsonContract GetContractSafe(object value)
    {
      return value == null ? (JsonContract) null : this.Serializer._contractResolver.ResolveContract(value.GetType());
    }

    private void SerializePrimitive(
      JsonWriter writer,
      object value,
      JsonPrimitiveContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      if (contract.TypeCode == PrimitiveTypeCode.Bytes && this.ShouldWriteType(TypeNameHandling.Objects, (JsonContract) contract, member, containerContract, containerProperty))
      {
        writer.WriteStartObject();
        this.WriteTypeProperty(writer, contract.CreatedType);
        writer.WritePropertyName("$value", false);
        JsonWriter.WriteValue(writer, contract.TypeCode, value);
        writer.WriteEndObject();
      }
      else
        JsonWriter.WriteValue(writer, contract.TypeCode, value);
    }

    private void SerializeValue(
      JsonWriter writer,
      object value,
      JsonContract valueContract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        JsonConverter converter = member?.Converter ?? containerProperty?.ItemConverter ?? containerContract?.ItemConverter ?? valueContract.Converter ?? this.Serializer.GetMatchingConverter(valueContract.UnderlyingType) ?? valueContract.InternalConverter;
        if (converter != null && converter.CanWrite)
        {
          this.SerializeConvertable(writer, converter, value, valueContract, containerContract, containerProperty);
        }
        else
        {
          switch (valueContract.ContractType)
          {
            case JsonContractType.Object:
              this.SerializeObject(writer, value, (JsonObjectContract) valueContract, member, containerContract, containerProperty);
              break;
            case JsonContractType.Array:
              JsonArrayContract contract1 = (JsonArrayContract) valueContract;
              if (!contract1.IsMultidimensionalArray)
              {
                this.SerializeList(writer, (IEnumerable) value, contract1, member, containerContract, containerProperty);
                break;
              }
              this.SerializeMultidimensionalArray(writer, (Array) value, contract1, member, containerContract, containerProperty);
              break;
            case JsonContractType.Primitive:
              this.SerializePrimitive(writer, value, (JsonPrimitiveContract) valueContract, member, containerContract, containerProperty);
              break;
            case JsonContractType.String:
              this.SerializeString(writer, value, (JsonStringContract) valueContract);
              break;
            case JsonContractType.Dictionary:
              JsonDictionaryContract contract2 = (JsonDictionaryContract) valueContract;
              this.SerializeDictionary(writer, value is IDictionary ? (IDictionary) value : (IDictionary) contract2.CreateWrapper(value), contract2, member, containerContract, containerProperty);
              break;
            case JsonContractType.Linq:
              ((JToken) value).WriteTo(writer, this.Serializer.Converters.ToArray<JsonConverter>());
              break;
          }
        }
      }
    }

    private bool? ResolveIsReference(
      JsonContract contract,
      JsonProperty property,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      bool? nullable = new bool?();
      if (property != null)
        nullable = property.IsReference;
      if (!nullable.HasValue && containerProperty != null)
        nullable = containerProperty.ItemIsReference;
      if (!nullable.HasValue && collectionContract != null)
        nullable = collectionContract.ItemIsReference;
      if (!nullable.HasValue)
        nullable = contract.IsReference;
      return nullable;
    }

    private bool ShouldWriteReference(
      object value,
      JsonProperty property,
      JsonContract valueContract,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      if (value == null || valueContract.ContractType == JsonContractType.Primitive || valueContract.ContractType == JsonContractType.String)
        return false;
      bool? nullable = this.ResolveIsReference(valueContract, property, collectionContract, containerProperty);
      if (!nullable.HasValue)
        nullable = valueContract.ContractType != JsonContractType.Array ? new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects)) : new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays));
      return nullable.Value && this.Serializer.GetReferenceResolver().IsReferenced((object) this, value);
    }

    private bool ShouldWriteProperty(object memberValue, JsonProperty property)
    {
      return (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) != NullValueHandling.Ignore || memberValue != null) && (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || !MiscellaneousUtils.ValueEquals(memberValue, property.GetResolvedDefaultValue()));
    }

    private bool CheckForCircularReference(
      JsonWriter writer,
      object value,
      JsonProperty property,
      JsonContract contract,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      if (value == null || contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
        return true;
      ReferenceLoopHandling? nullable = new ReferenceLoopHandling?();
      if (property != null)
        nullable = property.ReferenceLoopHandling;
      if (!nullable.HasValue && containerProperty != null)
        nullable = containerProperty.ItemReferenceLoopHandling;
      if (!nullable.HasValue && containerContract != null)
        nullable = containerContract.ItemReferenceLoopHandling;
      if (this.Serializer._equalityComparer != null ? this._serializeStack.Contains(value, this.Serializer._equalityComparer) : this._serializeStack.Contains(value))
      {
        string str = "Self referencing loop detected";
        if (property != null)
          str += " for property '{0}'".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) property.PropertyName);
        string message = str + " with type '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) value.GetType());
        switch (nullable.GetValueOrDefault(this.Serializer._referenceLoopHandling))
        {
          case ReferenceLoopHandling.Error:
            throw JsonSerializationException.Create((IJsonLineInfo) null, writer.ContainerPath, message, (Exception) null);
          case ReferenceLoopHandling.Ignore:
            if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
              this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, message + ". Skipping serializing self referenced value."), (Exception) null);
            return false;
          case ReferenceLoopHandling.Serialize:
            if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
              this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, message + ". Serializing self referenced value."), (Exception) null);
            return true;
        }
      }
      return true;
    }

    private void WriteReference(JsonWriter writer, object value)
    {
      string reference = this.GetReference(writer, value);
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Writing object reference to Id '{0}' for {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reference, (object) value.GetType())), (Exception) null);
      writer.WriteStartObject();
      writer.WritePropertyName("$ref", false);
      writer.WriteValue(reference);
      writer.WriteEndObject();
    }

    private string GetReference(JsonWriter writer, object value)
    {
      try
      {
        return this.Serializer.GetReferenceResolver().GetReference((object) this, value);
      }
      catch (Exception ex)
      {
        throw JsonSerializationException.Create((IJsonLineInfo) null, writer.ContainerPath, "Error writing object reference for '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) value.GetType()), ex);
      }
    }

    internal static bool TryConvertToString(object value, Type type, out string s)
    {
      if (value is Type)
      {
        s = ((Type) value).AssemblyQualifiedName;
        return true;
      }
      s = (string) null;
      return false;
    }

    private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
    {
      this.OnSerializing(writer, (JsonContract) contract, value);
      string s;
      JsonSerializerInternalWriter.TryConvertToString(value, contract.UnderlyingType, out s);
      writer.WriteValue(s);
      this.OnSerialized(writer, (JsonContract) contract, value);
    }

    private void OnSerializing(JsonWriter writer, JsonContract contract, object value)
    {
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Started serializing {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType)), (Exception) null);
      contract.InvokeOnSerializing(value, this.Serializer._context);
    }

    private void OnSerialized(JsonWriter writer, JsonContract contract, object value)
    {
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
        this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Finished serializing {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) contract.UnderlyingType)), (Exception) null);
      contract.InvokeOnSerialized(value, this.Serializer._context);
    }

    private void SerializeObject(
      JsonWriter writer,
      object value,
      JsonObjectContract contract,
      JsonProperty member,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      this.OnSerializing(writer, (JsonContract) contract, value);
      this._serializeStack.Add(value);
      this.WriteObjectStart(writer, value, (JsonContract) contract, member, collectionContract, containerProperty);
      int top = writer.Top;
      for (int index = 0; index < contract.Properties.Count; ++index)
      {
        JsonProperty property = contract.Properties[index];
        try
        {
          JsonContract memberContract;
          object memberValue;
          if (this.CalculatePropertyValues(writer, value, (JsonContainerContract) contract, member, property, out memberContract, out memberValue))
          {
            property.WritePropertyName(writer);
            this.SerializeValue(writer, memberValue, memberContract, property, (JsonContainerContract) contract, member);
          }
        }
        catch (Exception ex)
        {
          if (this.IsErrorHandled(value, (JsonContract) contract, (object) property.PropertyName, (IJsonLineInfo) null, writer.ContainerPath, ex))
            this.HandleError(writer, top);
          else
            throw;
        }
      }
      if (contract.ExtensionDataGetter != null)
      {
        IEnumerable<KeyValuePair<object, object>> keyValuePairs = contract.ExtensionDataGetter(value);
        if (keyValuePairs != null)
        {
          foreach (KeyValuePair<object, object> keyValuePair in keyValuePairs)
          {
            JsonContract contractSafe1 = this.GetContractSafe(keyValuePair.Key);
            JsonContract contractSafe2 = this.GetContractSafe(keyValuePair.Value);
            string propertyName = this.GetPropertyName(writer, keyValuePair.Key, contractSafe1, out bool _);
            if (this.ShouldWriteReference(keyValuePair.Value, (JsonProperty) null, contractSafe2, (JsonContainerContract) contract, member))
            {
              writer.WritePropertyName(propertyName);
              this.WriteReference(writer, keyValuePair.Value);
            }
            else if (this.CheckForCircularReference(writer, keyValuePair.Value, (JsonProperty) null, contractSafe2, (JsonContainerContract) contract, member))
            {
              writer.WritePropertyName(propertyName);
              this.SerializeValue(writer, keyValuePair.Value, contractSafe2, (JsonProperty) null, (JsonContainerContract) contract, member);
            }
          }
        }
      }
      writer.WriteEndObject();
      this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
      this.OnSerialized(writer, (JsonContract) contract, value);
    }

    private bool CalculatePropertyValues(
      JsonWriter writer,
      object value,
      JsonContainerContract contract,
      JsonProperty member,
      JsonProperty property,
      out JsonContract memberContract,
      out object memberValue)
    {
      if (!property.Ignored && property.Readable && this.ShouldSerialize(writer, property, value) && this.IsSpecified(writer, property, value))
      {
        if (property.PropertyContract == null)
          property.PropertyContract = this.Serializer._contractResolver.ResolveContract(property.PropertyType);
        memberValue = property.ValueProvider.GetValue(value);
        memberContract = property.PropertyContract.IsSealed ? property.PropertyContract : this.GetContractSafe(memberValue);
        if (this.ShouldWriteProperty(memberValue, property))
        {
          if (this.ShouldWriteReference(memberValue, property, memberContract, contract, member))
          {
            property.WritePropertyName(writer);
            this.WriteReference(writer, memberValue);
            return false;
          }
          if (!this.CheckForCircularReference(writer, memberValue, property, memberContract, contract, member))
            return false;
          if (memberValue == null)
          {
            JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
            if (((int) property._required ?? (int) jsonObjectContract?.ItemRequired ?? 0) == 2)
              throw JsonSerializationException.Create((IJsonLineInfo) null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a value.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) property.PropertyName), (Exception) null);
          }
          return true;
        }
      }
      memberContract = (JsonContract) null;
      memberValue = (object) null;
      return false;
    }

    private void WriteObjectStart(
      JsonWriter writer,
      object value,
      JsonContract contract,
      JsonProperty member,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      writer.WriteStartObject();
      if (((int) this.ResolveIsReference(contract, member, collectionContract, containerProperty) ?? (this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects) ? 1 : 0)) != 0 && (member == null || member.Writable))
        this.WriteReferenceIdProperty(writer, contract.UnderlyingType, value);
      if (!this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionContract, containerProperty))
        return;
      this.WriteTypeProperty(writer, contract.UnderlyingType);
    }

    private void WriteReferenceIdProperty(JsonWriter writer, Type type, object value)
    {
      string reference = this.GetReference(writer, value);
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
        this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Writing object reference Id '{0}' for {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reference, (object) type)), (Exception) null);
      writer.WritePropertyName("$id", false);
      writer.WriteValue(reference);
    }

    private void WriteTypeProperty(JsonWriter writer, Type type)
    {
      string typeName = ReflectionUtils.GetTypeName(type, this.Serializer._typeNameAssemblyFormat, this.Serializer._binder);
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
        this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Writing type name '{0}' for {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) typeName, (object) type)), (Exception) null);
      writer.WritePropertyName("$type", false);
      writer.WriteValue(typeName);
    }

    private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
    {
      return (value & flag) == flag;
    }

    private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
    {
      return (value & flag) == flag;
    }

    private bool HasFlag(TypeNameHandling value, TypeNameHandling flag) => (value & flag) == flag;

    private void SerializeConvertable(
      JsonWriter writer,
      JsonConverter converter,
      object value,
      JsonContract contract,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      if (this.ShouldWriteReference(value, (JsonProperty) null, contract, collectionContract, containerProperty))
      {
        this.WriteReference(writer, value);
      }
      else
      {
        if (!this.CheckForCircularReference(writer, value, (JsonProperty) null, contract, collectionContract, containerProperty))
          return;
        this._serializeStack.Add(value);
        if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
          this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Started serializing {0} with converter {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) value.GetType(), (object) converter.GetType())), (Exception) null);
        converter.WriteJson(writer, value, (JsonSerializer) this.GetInternalSerializer());
        if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
          this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "Finished serializing {0} with converter {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) value.GetType(), (object) converter.GetType())), (Exception) null);
        this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
      }
    }

    private void SerializeList(
      JsonWriter writer,
      IEnumerable values,
      JsonArrayContract contract,
      JsonProperty member,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      object obj1 = values is IWrappedCollection wrappedCollection ? wrappedCollection.UnderlyingCollection : (object) values;
      this.OnSerializing(writer, (JsonContract) contract, obj1);
      this._serializeStack.Add(obj1);
      bool flag = this.WriteStartArray(writer, obj1, contract, member, collectionContract, containerProperty);
      writer.WriteStartArray();
      int top = writer.Top;
      int keyValue = 0;
      foreach (object obj2 in values)
      {
        try
        {
          JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(obj2);
          if (this.ShouldWriteReference(obj2, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
            this.WriteReference(writer, obj2);
          else if (this.CheckForCircularReference(writer, obj2, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
            this.SerializeValue(writer, obj2, jsonContract, (JsonProperty) null, (JsonContainerContract) contract, member);
        }
        catch (Exception ex)
        {
          if (this.IsErrorHandled(obj1, (JsonContract) contract, (object) keyValue, (IJsonLineInfo) null, writer.ContainerPath, ex))
            this.HandleError(writer, top);
          else
            throw;
        }
        finally
        {
          ++keyValue;
        }
      }
      writer.WriteEndArray();
      if (flag)
        writer.WriteEndObject();
      this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
      this.OnSerialized(writer, (JsonContract) contract, obj1);
    }

    private void SerializeMultidimensionalArray(
      JsonWriter writer,
      Array values,
      JsonArrayContract contract,
      JsonProperty member,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      this.OnSerializing(writer, (JsonContract) contract, (object) values);
      this._serializeStack.Add((object) values);
      bool flag = this.WriteStartArray(writer, (object) values, contract, member, collectionContract, containerProperty);
      this.SerializeMultidimensionalArray(writer, values, contract, member, writer.Top, new int[0]);
      if (flag)
        writer.WriteEndObject();
      this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
      this.OnSerialized(writer, (JsonContract) contract, (object) values);
    }

    private void SerializeMultidimensionalArray(
      JsonWriter writer,
      Array values,
      JsonArrayContract contract,
      JsonProperty member,
      int initialDepth,
      int[] indices)
    {
      int length = indices.Length;
      int[] indices1 = new int[length + 1];
      for (int index = 0; index < length; ++index)
        indices1[index] = indices[index];
      writer.WriteStartArray();
      for (int keyValue = 0; keyValue < values.GetLength(length); ++keyValue)
      {
        indices1[length] = keyValue;
        if (indices1.Length == values.Rank)
        {
          object obj = values.GetValue(indices1);
          try
          {
            JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(obj);
            if (this.ShouldWriteReference(obj, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
              this.WriteReference(writer, obj);
            else if (this.CheckForCircularReference(writer, obj, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
              this.SerializeValue(writer, obj, jsonContract, (JsonProperty) null, (JsonContainerContract) contract, member);
          }
          catch (Exception ex)
          {
            if (this.IsErrorHandled((object) values, (JsonContract) contract, (object) keyValue, (IJsonLineInfo) null, writer.ContainerPath, ex))
              this.HandleError(writer, initialDepth + 1);
            else
              throw;
          }
        }
        else
          this.SerializeMultidimensionalArray(writer, values, contract, member, initialDepth + 1, indices1);
      }
      writer.WriteEndArray();
    }

    private bool WriteStartArray(
      JsonWriter writer,
      object values,
      JsonArrayContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      bool flag1 = ((int) this.ResolveIsReference((JsonContract) contract, member, containerContract, containerProperty) ?? (this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays) ? 1 : 0)) != 0 && (member == null || member.Writable);
      bool flag2 = this.ShouldWriteType(TypeNameHandling.Arrays, (JsonContract) contract, member, containerContract, containerProperty);
      bool flag3 = flag1 || flag2;
      if (flag3)
      {
        writer.WriteStartObject();
        if (flag1)
          this.WriteReferenceIdProperty(writer, contract.UnderlyingType, values);
        if (flag2)
          this.WriteTypeProperty(writer, values.GetType());
        writer.WritePropertyName("$values", false);
      }
      if (contract.ItemContract == null)
        contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.CollectionItemType ?? typeof (object));
      return flag3;
    }

    private bool ShouldWriteDynamicProperty(object memberValue)
    {
      return (this.Serializer._nullValueHandling != NullValueHandling.Ignore || memberValue != null) && (!this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Ignore) || memberValue != null && !MiscellaneousUtils.ValueEquals(memberValue, ReflectionUtils.GetDefaultValue(memberValue.GetType())));
    }

    private bool ShouldWriteType(
      TypeNameHandling typeNameHandlingFlag,
      JsonContract contract,
      JsonProperty member,
      JsonContainerContract containerContract,
      JsonProperty containerProperty)
    {
      TypeNameHandling typeNameHandling = (TypeNameHandling) ((int) member?.TypeNameHandling ?? (int) containerProperty?.ItemTypeNameHandling ?? (int) containerContract?.ItemTypeNameHandling ?? (int) this.Serializer._typeNameHandling);
      if (this.HasFlag(typeNameHandling, typeNameHandlingFlag))
        return true;
      if (this.HasFlag(typeNameHandling, TypeNameHandling.Auto))
      {
        if (member != null)
        {
          if (contract.UnderlyingType != member.PropertyContract.CreatedType)
            return true;
        }
        else if (containerContract != null)
        {
          if (containerContract.ItemContract == null || contract.UnderlyingType != containerContract.ItemContract.CreatedType)
            return true;
        }
        else if (this._rootContract != null && this._serializeStack.Count == this._rootLevel && contract.UnderlyingType != this._rootContract.CreatedType)
          return true;
      }
      return false;
    }

    private void SerializeDictionary(
      JsonWriter writer,
      IDictionary values,
      JsonDictionaryContract contract,
      JsonProperty member,
      JsonContainerContract collectionContract,
      JsonProperty containerProperty)
    {
      object currentObject = values is IWrappedDictionary wrappedDictionary ? wrappedDictionary.UnderlyingDictionary : (object) values;
      this.OnSerializing(writer, (JsonContract) contract, currentObject);
      this._serializeStack.Add(currentObject);
      this.WriteObjectStart(writer, currentObject, (JsonContract) contract, member, collectionContract, containerProperty);
      if (contract.ItemContract == null)
        contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryValueType ?? typeof (object));
      if (contract.KeyContract == null)
        contract.KeyContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryKeyType ?? typeof (object));
      int top = writer.Top;
      foreach (DictionaryEntry dictionaryEntry in values)
      {
        bool escape;
        string propertyName = this.GetPropertyName(writer, dictionaryEntry.Key, contract.KeyContract, out escape);
        string str = contract.DictionaryKeyResolver != null ? contract.DictionaryKeyResolver(propertyName) : propertyName;
        try
        {
          object obj = dictionaryEntry.Value;
          JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(obj);
          if (this.ShouldWriteReference(obj, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
          {
            writer.WritePropertyName(str, escape);
            this.WriteReference(writer, obj);
          }
          else if (this.CheckForCircularReference(writer, obj, (JsonProperty) null, jsonContract, (JsonContainerContract) contract, member))
          {
            writer.WritePropertyName(str, escape);
            this.SerializeValue(writer, obj, jsonContract, (JsonProperty) null, (JsonContainerContract) contract, member);
          }
        }
        catch (Exception ex)
        {
          if (this.IsErrorHandled(currentObject, (JsonContract) contract, (object) str, (IJsonLineInfo) null, writer.ContainerPath, ex))
            this.HandleError(writer, top);
          else
            throw;
        }
      }
      writer.WriteEndObject();
      this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
      this.OnSerialized(writer, (JsonContract) contract, currentObject);
    }

    private string GetPropertyName(
      JsonWriter writer,
      object name,
      JsonContract contract,
      out bool escape)
    {
      if (contract.ContractType == JsonContractType.Primitive)
      {
        JsonPrimitiveContract primitiveContract = (JsonPrimitiveContract) contract;
        if (primitiveContract.TypeCode == PrimitiveTypeCode.DateTime || primitiveContract.TypeCode == PrimitiveTypeCode.DateTimeNullable)
        {
          escape = false;
          StringWriter writer1 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
          DateTimeUtils.WriteDateTimeString((TextWriter) writer1, (DateTime) name, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
          return writer1.ToString();
        }
        if (primitiveContract.TypeCode == PrimitiveTypeCode.DateTimeOffset || primitiveContract.TypeCode == PrimitiveTypeCode.DateTimeOffsetNullable)
        {
          escape = false;
          StringWriter writer2 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
          DateTimeUtils.WriteDateTimeOffsetString((TextWriter) writer2, (DateTimeOffset) name, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
          return writer2.ToString();
        }
        escape = true;
        return Convert.ToString(name, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      string s;
      if (JsonSerializerInternalWriter.TryConvertToString(name, name.GetType(), out s))
      {
        escape = true;
        return s;
      }
      escape = true;
      return name.ToString();
    }

    private void HandleError(JsonWriter writer, int initialDepth)
    {
      this.ClearErrorContext();
      if (writer.WriteState == WriteState.Property)
        writer.WriteNull();
      while (writer.Top > initialDepth)
        writer.WriteEnd();
    }

    private bool ShouldSerialize(JsonWriter writer, JsonProperty property, object target)
    {
      if (property.ShouldSerialize == null)
        return true;
      bool flag = property.ShouldSerialize(target);
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
        this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "ShouldSerialize result for property '{0}' on {1}: {2}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) property.PropertyName, (object) property.DeclaringType, (object) flag)), (Exception) null);
      return flag;
    }

    private bool IsSpecified(JsonWriter writer, JsonProperty property, object target)
    {
      if (property.GetIsSpecified == null)
        return true;
      bool flag = property.GetIsSpecified(target);
      if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
        this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage((IJsonLineInfo) null, writer.Path, "IsSpecified result for property '{0}' on {1}: {2}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) property.PropertyName, (object) property.DeclaringType, (object) flag)), (Exception) null);
      return flag;
    }
  }
}
