// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.KeyValuePairConverter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Converters
{
  /// <summary>
  /// Converts a <see cref="T:System.Collections.Generic.KeyValuePair`2" /> to and from JSON.
  /// </summary>
  public class KeyValuePairConverter : JsonConverter
  {
    private const string KeyName = "Key";
    private const string ValueName = "Value";
    private static readonly ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(new Func<Type, ReflectionObject>(KeyValuePairConverter.InitializeReflectionObject));

    private static ReflectionObject InitializeReflectionObject(Type t)
    {
      IList<Type> genericArguments = (IList<Type>) t.GetGenericArguments();
      Type type1 = genericArguments[0];
      Type type2 = genericArguments[1];
      return ReflectionObject.Create(t, (MethodBase) t.GetConstructor(new Type[2]
      {
        type1,
        type2
      }), "Key", "Value");
    }

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(value.GetType());
      DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
      writer.WriteStartObject();
      writer.WritePropertyName(contractResolver != null ? contractResolver.GetResolvedPropertyName("Key") : "Key");
      serializer.Serialize(writer, reflectionObject.GetValue(value, "Key"), reflectionObject.GetType("Key"));
      writer.WritePropertyName(contractResolver != null ? contractResolver.GetResolvedPropertyName("Value") : "Value");
      serializer.Serialize(writer, reflectionObject.GetValue(value, "Value"), reflectionObject.GetType("Value"));
      writer.WriteEndObject();
    }

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      bool flag = ReflectionUtils.IsNullableType(objectType);
      Type key = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
      ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(key);
      if (reader.TokenType == JsonToken.Null)
      {
        if (!flag)
          throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
        return (object) null;
      }
      object obj1 = (object) null;
      object obj2 = (object) null;
      KeyValuePairConverter.ReadAndAssert(reader);
      while (reader.TokenType == JsonToken.PropertyName)
      {
        string a = reader.Value.ToString();
        if (string.Equals(a, "Key", StringComparison.OrdinalIgnoreCase))
        {
          KeyValuePairConverter.ReadAndAssert(reader);
          obj1 = serializer.Deserialize(reader, reflectionObject.GetType("Key"));
        }
        else if (string.Equals(a, "Value", StringComparison.OrdinalIgnoreCase))
        {
          KeyValuePairConverter.ReadAndAssert(reader);
          obj2 = serializer.Deserialize(reader, reflectionObject.GetType("Value"));
        }
        else
          reader.Skip();
        KeyValuePairConverter.ReadAndAssert(reader);
      }
      return reflectionObject.Creator(new object[2]
      {
        obj1,
        obj2
      });
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
      Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
      return type.IsValueType() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>);
    }

    private static void ReadAndAssert(JsonReader reader)
    {
      if (!reader.Read())
        throw JsonSerializationException.Create(reader, "Unexpected end when reading KeyValuePair.");
    }
  }
}
