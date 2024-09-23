// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonTypeReflector
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal static class JsonTypeReflector
  {
    public const string IdPropertyName = "$id";
    public const string RefPropertyName = "$ref";
    public const string TypePropertyName = "$type";
    public const string ValuePropertyName = "$value";
    public const string ArrayValuesPropertyName = "$values";
    public const string ShouldSerializePrefix = "ShouldSerialize";
    public const string SpecifiedPostfix = "Specified";
    private static bool? _dynamicCodeGeneration;
    private static bool? _fullyTrusted;
    private static readonly ThreadSafeStore<Type, Func<object[], JsonConverter>> JsonConverterCreatorCache = new ThreadSafeStore<Type, Func<object[], JsonConverter>>(new Func<Type, Func<object[], JsonConverter>>(JsonTypeReflector.GetJsonConverterCreator));
    private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));
    private static ReflectionObject _metadataTypeAttributeReflectionObject;

    public static T GetCachedAttribute<T>(object attributeProvider) where T : Attribute
    {
      return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
    }

    public static DataContractAttribute GetDataContractAttribute(Type type)
    {
      for (Type type1 = type; type1 != null; type1 = type1.BaseType())
      {
        DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute((object) type1);
        if (attribute != null)
          return attribute;
      }
      return (DataContractAttribute) null;
    }

    public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
    {
      if (memberInfo.MemberType() == MemberTypes.Field)
        return CachedAttributeGetter<DataMemberAttribute>.GetAttribute((object) memberInfo);
      PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
      DataMemberAttribute attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute((object) propertyInfo);
      if (attribute == null && propertyInfo.IsVirtual())
      {
        for (Type type = propertyInfo.DeclaringType; attribute == null && type != null; type = type.BaseType())
        {
          PropertyInfo memberInfoFromType = (PropertyInfo) ReflectionUtils.GetMemberInfoFromType(type, (MemberInfo) propertyInfo);
          if (memberInfoFromType != null && memberInfoFromType.IsVirtual())
            attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute((object) memberInfoFromType);
        }
      }
      return attribute;
    }

    public static MemberSerialization GetObjectMemberSerialization(
      Type objectType,
      bool ignoreSerializableAttribute)
    {
      JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>((object) objectType);
      if (cachedAttribute != null)
        return cachedAttribute.MemberSerialization;
      return JsonTypeReflector.GetDataContractAttribute(objectType) != null ? MemberSerialization.OptIn : MemberSerialization.OptOut;
    }

    public static JsonConverter GetJsonConverter(object attributeProvider)
    {
      JsonConverterAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
      if (cachedAttribute != null)
      {
        Func<object[], JsonConverter> func = JsonTypeReflector.JsonConverterCreatorCache.Get(cachedAttribute.ConverterType);
        if (func != null)
          return func(cachedAttribute.ConverterParameters);
      }
      return (JsonConverter) null;
    }

    /// <summary>
    /// Lookup and create an instance of the JsonConverter type described by the argument.
    /// </summary>
    /// <param name="converterType">The JsonConverter type to create.</param>
    /// <param name="converterArgs">Optional arguments to pass to an initializing constructor of the JsonConverter.
    /// If null, the default constructor is used.</param>
    public static JsonConverter CreateJsonConverterInstance(
      Type converterType,
      object[] converterArgs)
    {
      return JsonTypeReflector.JsonConverterCreatorCache.Get(converterType)(converterArgs);
    }

    /// <summary>
    /// Create a factory function that can be used to create instances of a JsonConverter described by the
    /// argument type.  The returned function can then be used to either invoke the converter's default ctor, or any
    /// parameterized constructors by way of an object array.
    /// </summary>
    private static Func<object[], JsonConverter> GetJsonConverterCreator(Type converterType)
    {
      Func<object> defaultConstructor = ReflectionUtils.HasDefaultConstructor(converterType, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(converterType) : (Func<object>) null;
      return (Func<object[], JsonConverter>) (parameters =>
      {
        try
        {
          if (parameters != null)
          {
            ConstructorInfo constructor = converterType.GetConstructor(((IEnumerable<object>) parameters).Select<object, Type>((Func<object, Type>) (param => param.GetType())).ToArray<Type>());
            if (constructor != null)
              return (JsonConverter) JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) constructor)(parameters);
            throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) converterType));
          }
          if (defaultConstructor == null)
            throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) converterType));
          return (JsonConverter) defaultConstructor();
        }
        catch (Exception ex)
        {
          throw new JsonException("Error creating '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) converterType), ex);
        }
      });
    }

    private static Type GetAssociatedMetadataType(Type type)
    {
      return JsonTypeReflector.AssociatedMetadataTypesCache.Get(type);
    }

    private static Type GetAssociateMetadataTypeFromAttribute(Type type)
    {
      foreach (Attribute attribute in ReflectionUtils.GetAttributes((object) type, (Type) null, true))
      {
        Type type1 = attribute.GetType();
        if (string.Equals(type1.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
        {
          if (JsonTypeReflector._metadataTypeAttributeReflectionObject == null)
            JsonTypeReflector._metadataTypeAttributeReflectionObject = ReflectionObject.Create(type1, "MetadataClassType");
          return (Type) JsonTypeReflector._metadataTypeAttributeReflectionObject.GetValue((object) attribute, "MetadataClassType");
        }
      }
      return (Type) null;
    }

    private static T GetAttribute<T>(Type type) where T : Attribute
    {
      Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(type);
      if (associatedMetadataType != null)
      {
        T attribute = ReflectionUtils.GetAttribute<T>((object) associatedMetadataType, true);
        if ((object) attribute != null)
          return attribute;
      }
      T attribute1 = ReflectionUtils.GetAttribute<T>((object) type, true);
      if ((object) attribute1 != null)
        return attribute1;
      foreach (object attributeProvider in type.GetInterfaces())
      {
        T attribute2 = ReflectionUtils.GetAttribute<T>(attributeProvider, true);
        if ((object) attribute2 != null)
          return attribute2;
      }
      return default (T);
    }

    private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
    {
      Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(memberInfo.DeclaringType);
      if (associatedMetadataType != null)
      {
        MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
        if (memberInfoFromType != null)
        {
          T attribute = ReflectionUtils.GetAttribute<T>((object) memberInfoFromType, true);
          if ((object) attribute != null)
            return attribute;
        }
      }
      T attribute1 = ReflectionUtils.GetAttribute<T>((object) memberInfo, true);
      if ((object) attribute1 != null)
        return attribute1;
      if (memberInfo.DeclaringType != null)
      {
        foreach (Type targetType in memberInfo.DeclaringType.GetInterfaces())
        {
          MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(targetType, memberInfo);
          if (memberInfoFromType != null)
          {
            T attribute2 = ReflectionUtils.GetAttribute<T>((object) memberInfoFromType, true);
            if ((object) attribute2 != null)
              return attribute2;
          }
        }
      }
      return default (T);
    }

    public static T GetAttribute<T>(object provider) where T : Attribute
    {
      switch (provider)
      {
        case Type type:
          return JsonTypeReflector.GetAttribute<T>(type);
        case MemberInfo memberInfo:
          return JsonTypeReflector.GetAttribute<T>(memberInfo);
        default:
          return ReflectionUtils.GetAttribute<T>(provider, true);
      }
    }

    public static bool DynamicCodeGeneration
    {
      [SecuritySafeCritical] get
      {
        if (!JsonTypeReflector._dynamicCodeGeneration.HasValue)
          JsonTypeReflector._dynamicCodeGeneration = new bool?(false);
        return JsonTypeReflector._dynamicCodeGeneration.Value;
      }
    }

    public static bool FullyTrusted
    {
      get
      {
        if (!JsonTypeReflector._fullyTrusted.HasValue)
          JsonTypeReflector._fullyTrusted = new bool?(false);
        return JsonTypeReflector._fullyTrusted.Value;
      }
    }

    public static ReflectionDelegateFactory ReflectionDelegateFactory
    {
      get => ExpressionReflectionDelegateFactory.Instance;
    }
  }
}
