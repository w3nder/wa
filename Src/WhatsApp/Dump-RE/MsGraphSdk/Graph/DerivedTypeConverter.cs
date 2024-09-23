// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DerivedTypeConverter
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Microsoft.Graph
{
  public class DerivedTypeConverter : JsonConverter
  {
    internal static readonly ConcurrentDictionary<string, Type> TypeMappingCache = new ConcurrentDictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public override bool CanConvert(Type objectType) => true;

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jObject = JObject.Load(reader);
      JToken jtoken = jObject.GetValue("@odata.type");
      object target;
      if (jtoken != null)
      {
        string titleCase = StringHelper.ConvertTypeToTitleCase(jtoken.ToString().TrimStart('#'));
        Type type = (Type) null;
        if (DerivedTypeConverter.TypeMappingCache.TryGetValue(titleCase, out type))
        {
          target = this.Create(type);
        }
        else
        {
          Assembly assembly = IntrospectionExtensions.GetTypeInfo(objectType).Assembly;
          target = this.Create(titleCase, assembly);
        }
        if (target == null)
          target = this.Create(objectType.AssemblyQualifiedName, (Assembly) null);
        if (target != null && type == null)
          DerivedTypeConverter.TypeMappingCache.TryAdd(titleCase, target.GetType());
      }
      else
        target = this.Create(objectType.AssemblyQualifiedName, (Assembly) null);
      if (target == null)
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.GeneralException,
          Message = string.Format(ErrorConstants.Messages.UnableToCreateInstanceOfTypeFormatString, (object) objectType.AssemblyQualifiedName)
        });
      using (JsonReader objectReader = this.GetObjectReader(reader, jObject))
      {
        serializer.Populate(objectReader, target);
        return target;
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    private object Create(string typeString, Assembly typeAssembly)
    {
      return this.Create(typeAssembly == null ? Type.GetType(typeString) : typeAssembly.GetType(typeString));
    }

    private object Create(Type type)
    {
      if (type == null)
        return (object) null;
      try
      {
        return IntrospectionExtensions.GetTypeInfo(type).DeclaredConstructors.FirstOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (constructor => !((IEnumerable<ParameterInfo>) constructor.GetParameters()).Any<ParameterInfo>() && !constructor.IsStatic))?.Invoke(new object[0]);
      }
      catch (Exception ex)
      {
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.GeneralException,
          Message = string.Format(ErrorConstants.Messages.UnableToCreateInstanceOfTypeFormatString, (object) type.FullName)
        }, ex);
      }
    }

    private JsonReader GetObjectReader(JsonReader originalReader, JObject jObject)
    {
      JsonReader reader = jObject.CreateReader();
      reader.Culture = originalReader.Culture;
      reader.DateParseHandling = originalReader.DateParseHandling;
      reader.DateTimeZoneHandling = originalReader.DateTimeZoneHandling;
      reader.FloatParseHandling = originalReader.FloatParseHandling;
      reader.CloseInput = false;
      return reader;
    }
  }
}
