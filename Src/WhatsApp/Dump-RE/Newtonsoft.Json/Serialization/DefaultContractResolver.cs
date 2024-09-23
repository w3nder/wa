// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.DefaultContractResolver
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Used by <see cref="T:Newtonsoft.Json.JsonSerializer" /> to resolves a <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for a given <see cref="T:System.Type" />.
  /// </summary>
  public class DefaultContractResolver : IContractResolver
  {
    private static readonly IContractResolver _instance = (IContractResolver) new DefaultContractResolver(true);
    private static readonly JsonConverter[] BuiltInConverters = new JsonConverter[4]
    {
      (JsonConverter) new DiscriminatedUnionConverter(),
      (JsonConverter) new KeyValuePairConverter(),
      (JsonConverter) new BsonObjectIdConverter(),
      (JsonConverter) new RegexConverter()
    };
    private static readonly object TypeContractCacheLock = new object();
    private static readonly DefaultContractResolverState _sharedState = new DefaultContractResolverState();
    private readonly DefaultContractResolverState _instanceState = new DefaultContractResolverState();
    private readonly bool _sharedCache;

    internal static IContractResolver Instance => DefaultContractResolver._instance;

    /// <summary>
    /// Gets a value indicating whether members are being get and set using dynamic code generation.
    /// This value is determined by the runtime permissions available.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if using dynamic code generation; otherwise, <c>false</c>.
    /// </value>
    public bool DynamicCodeGeneration => JsonTypeReflector.DynamicCodeGeneration;

    /// <summary>Gets or sets the default members search flags.</summary>
    /// <value>The default members search flags.</value>
    [Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
    public BindingFlags DefaultMembersSearchFlags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether compiler generated members should be serialized.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if serialized compiler generated members; otherwise, <c>false</c>.
    /// </value>
    public bool SerializeCompilerGeneratedMembers { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> class.
    /// </summary>
    public DefaultContractResolver()
    {
      this.DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> class.
    /// </summary>
    /// <param name="shareCache">
    /// If set to <c>true</c> the <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> will use a cached shared with other resolvers of the same type.
    /// Sharing the cache will significantly improve performance with multiple resolver instances because expensive reflection will only
    /// happen once. This setting can cause unexpected behavior if different instances of the resolver are suppose to produce different
    /// results. When set to false it is highly recommended to reuse <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> instances with the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </param>
    [Obsolete("DefaultContractResolver(bool) is obsolete. Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance.")]
    public DefaultContractResolver(bool shareCache)
      : this()
    {
      this._sharedCache = shareCache;
    }

    internal DefaultContractResolverState GetState()
    {
      return this._sharedCache ? DefaultContractResolver._sharedState : this._instanceState;
    }

    /// <summary>Resolves the contract for a given type.</summary>
    /// <param name="type">The type to resolve a contract for.</param>
    /// <returns>The contract for a given type.</returns>
    public virtual JsonContract ResolveContract(Type type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      DefaultContractResolverState state = this.GetState();
      ResolverContractKey key = new ResolverContractKey(this.GetType(), type);
      Dictionary<ResolverContractKey, JsonContract> contractCache1 = state.ContractCache;
      JsonContract contract;
      if (contractCache1 == null || !contractCache1.TryGetValue(key, out contract))
      {
        contract = this.CreateContract(type);
        lock (DefaultContractResolver.TypeContractCacheLock)
        {
          Dictionary<ResolverContractKey, JsonContract> contractCache2 = state.ContractCache;
          Dictionary<ResolverContractKey, JsonContract> dictionary = contractCache2 != null ? new Dictionary<ResolverContractKey, JsonContract>((IDictionary<ResolverContractKey, JsonContract>) contractCache2) : new Dictionary<ResolverContractKey, JsonContract>();
          dictionary[key] = contract;
          state.ContractCache = dictionary;
        }
      }
      return contract;
    }

    /// <summary>Gets the serializable members for the type.</summary>
    /// <param name="objectType">The type to get serializable members for.</param>
    /// <returns>The serializable members for the type.</returns>
    protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
    {
      bool ignoreSerializableAttribute = true;
      MemberSerialization memberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, ignoreSerializableAttribute);
      List<MemberInfo> list1 = ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where<MemberInfo>((Func<MemberInfo, bool>) (m => !ReflectionUtils.IsIndexedProperty(m))).ToList<MemberInfo>();
      List<MemberInfo> source = new List<MemberInfo>();
      if (memberSerialization != MemberSerialization.Fields)
      {
        DataContractAttribute contractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
        List<MemberInfo> list2 = ReflectionUtils.GetFieldsAndProperties(objectType, this.DefaultMembersSearchFlags).Where<MemberInfo>((Func<MemberInfo, bool>) (m => !ReflectionUtils.IsIndexedProperty(m))).ToList<MemberInfo>();
        foreach (MemberInfo memberInfo in list1)
        {
          if (this.SerializeCompilerGeneratedMembers || !memberInfo.IsDefined(typeof (CompilerGeneratedAttribute), true))
          {
            if (list2.Contains(memberInfo))
              source.Add(memberInfo);
            else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>((object) memberInfo) != null)
              source.Add(memberInfo);
            else if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>((object) memberInfo) != null)
              source.Add(memberInfo);
            else if (contractAttribute != null && JsonTypeReflector.GetAttribute<DataMemberAttribute>((object) memberInfo) != null)
              source.Add(memberInfo);
            else if (memberSerialization == MemberSerialization.Fields && memberInfo.MemberType() == MemberTypes.Field)
              source.Add(memberInfo);
          }
        }
        if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out Type _))
          source = source.Where<MemberInfo>(new Func<MemberInfo, bool>(this.ShouldSerializeEntityMember)).ToList<MemberInfo>();
      }
      else
      {
        foreach (MemberInfo memberInfo in list1)
        {
          if (memberInfo is FieldInfo fieldInfo && !fieldInfo.IsStatic)
            source.Add(memberInfo);
        }
      }
      return source;
    }

    private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
    {
      return !(memberInfo is PropertyInfo propertyInfo) || !propertyInfo.PropertyType.IsGenericType() || !(propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1");
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> for the given type.</returns>
    protected virtual JsonObjectContract CreateObjectContract(Type objectType)
    {
      JsonObjectContract contract = new JsonObjectContract(objectType);
      this.InitializeContract((JsonContract) contract);
      bool ignoreSerializableAttribute = true;
      contract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(contract.NonNullableUnderlyingType, ignoreSerializableAttribute);
      contract.Properties.AddRange<JsonProperty>((IEnumerable<JsonProperty>) this.CreateProperties(contract.NonNullableUnderlyingType, contract.MemberSerialization));
      JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>((object) contract.NonNullableUnderlyingType);
      if (cachedAttribute != null)
        contract.ItemRequired = cachedAttribute._itemRequired;
      if (contract.IsInstantiable)
      {
        ConstructorInfo attributeConstructor = this.GetAttributeConstructor(contract.NonNullableUnderlyingType);
        if (attributeConstructor != null)
        {
          contract.OverrideConstructor = attributeConstructor;
          contract.CreatorParameters.AddRange<JsonProperty>((IEnumerable<JsonProperty>) this.CreateConstructorParameters(attributeConstructor, contract.Properties));
        }
        else if (contract.MemberSerialization != MemberSerialization.Fields && (contract.DefaultCreator == null || contract.DefaultCreatorNonPublic))
        {
          ConstructorInfo parametrizedConstructor = this.GetParametrizedConstructor(contract.NonNullableUnderlyingType);
          if (parametrizedConstructor != null)
          {
            contract.ParametrizedConstructor = parametrizedConstructor;
            contract.CreatorParameters.AddRange<JsonProperty>((IEnumerable<JsonProperty>) this.CreateConstructorParameters(parametrizedConstructor, contract.Properties));
          }
        }
      }
      MemberInfo dataMemberForType = this.GetExtensionDataMemberForType(contract.NonNullableUnderlyingType);
      if (dataMemberForType != null)
        DefaultContractResolver.SetExtensionDataDelegates(contract, dataMemberForType);
      return contract;
    }

    private MemberInfo GetExtensionDataMemberForType(Type type)
    {
      return this.GetClassHierarchyForType(type).SelectMany<Type, MemberInfo>((Func<Type, IEnumerable<MemberInfo>>) (baseType =>
      {
        IList<MemberInfo> initial = (IList<MemberInfo>) new List<MemberInfo>();
        initial.AddRange<MemberInfo>((IEnumerable<MemberInfo>) baseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
        initial.AddRange<MemberInfo>((IEnumerable<MemberInfo>) baseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
        return (IEnumerable<MemberInfo>) initial;
      })).LastOrDefault<MemberInfo>((Func<MemberInfo, bool>) (m =>
      {
        switch (m.MemberType())
        {
          case MemberTypes.Property:
          case MemberTypes.Field:
            if (!m.IsDefined(typeof (JsonExtensionDataAttribute), false))
              return false;
            if (!ReflectionUtils.CanReadMemberValue(m, true))
              throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' must have a getter.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), (object) m.Name));
            Type implementingType;
            if (ReflectionUtils.ImplementsGenericDefinition(ReflectionUtils.GetMemberUnderlyingType(m), typeof (IDictionary<,>), out implementingType))
            {
              Type genericArgument1 = implementingType.GetGenericArguments()[0];
              Type genericArgument2 = implementingType.GetGenericArguments()[1];
              if (genericArgument1.IsAssignableFrom(typeof (string)) && genericArgument2.IsAssignableFrom(typeof (JToken)))
                return true;
            }
            throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), (object) m.Name));
          default:
            return false;
        }
      }));
    }

    private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
    {
      JsonExtensionDataAttribute attribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>((object) member);
      if (attribute == null)
        return;
      Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
      Type implementingType;
      ReflectionUtils.ImplementsGenericDefinition(memberUnderlyingType, typeof (IDictionary<,>), out implementingType);
      Type genericArgument1 = implementingType.GetGenericArguments()[0];
      Type genericArgument2 = implementingType.GetGenericArguments()[1];
      bool isJTokenValueType = typeof (JToken).IsAssignableFrom(genericArgument2);
      Type type;
      if (ReflectionUtils.IsGenericDefinition(memberUnderlyingType, typeof (IDictionary<,>)))
        type = typeof (Dictionary<,>).MakeGenericType(genericArgument1, genericArgument2);
      else
        type = memberUnderlyingType;
      Func<object, object> getExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
      Action<object, object> setExtensionDataDictionary = ReflectionUtils.CanSetMemberValue(member, true, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member) : (Action<object, object>) null;
      Func<object> createExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
      MethodCall<object, object> setExtensionDataDictionaryValue = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) memberUnderlyingType.GetMethod("Add", new Type[2]
      {
        genericArgument1,
        genericArgument2
      }));
      ExtensionDataSetter extensionDataSetter = (ExtensionDataSetter) ((o, key, value) =>
      {
        object target = getExtensionDataDictionary(o);
        if (target == null)
        {
          if (setExtensionDataDictionary == null)
            throw new JsonSerializationException("Cannot set value onto extension data member '{0}'. The extension data collection is null and it cannot be set.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) member.Name));
          target = createExtensionDataDictionary();
          setExtensionDataDictionary(o, target);
        }
        if (isJTokenValueType && !(value is JToken))
          value = value != null ? (object) JToken.FromObject(value) : (object) JValue.CreateNull();
        object obj = setExtensionDataDictionaryValue(target, new object[2]
        {
          (object) key,
          value
        });
      });
      ObjectConstructor<object> createEnumerableWrapper = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) ((IEnumerable<ConstructorInfo>) typeof (DefaultContractResolver.DictionaryEnumerator<,>).MakeGenericType(genericArgument1, genericArgument2).GetConstructors()).First<ConstructorInfo>());
      ExtensionDataGetter extensionDataGetter = (ExtensionDataGetter) (o =>
      {
        object obj = getExtensionDataDictionary(o);
        if (obj == null)
          return (IEnumerable<KeyValuePair<object, object>>) null;
        return (IEnumerable<KeyValuePair<object, object>>) createEnumerableWrapper(new object[1]
        {
          obj
        });
      });
      if (attribute.ReadData)
        contract.ExtensionDataSetter = extensionDataSetter;
      if (!attribute.WriteData)
        return;
      contract.ExtensionDataGetter = extensionDataGetter;
    }

    private ConstructorInfo GetAttributeConstructor(Type objectType)
    {
      IList<ConstructorInfo> list = (IList<ConstructorInfo>) ((IEnumerable<ConstructorInfo>) objectType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)).Where<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => c.IsDefined(typeof (JsonConstructorAttribute), true))).ToList<ConstructorInfo>();
      if (list.Count > 1)
        throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
      if (list.Count == 1)
        return list[0];
      if (objectType != typeof (Version))
        return (ConstructorInfo) null;
      return objectType.GetConstructor(new Type[4]
      {
        typeof (int),
        typeof (int),
        typeof (int),
        typeof (int)
      });
    }

    private ConstructorInfo GetParametrizedConstructor(Type objectType)
    {
      IList<ConstructorInfo> list = (IList<ConstructorInfo>) ((IEnumerable<ConstructorInfo>) objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)).ToList<ConstructorInfo>();
      return list.Count == 1 ? list[0] : (ConstructorInfo) null;
    }

    /// <summary>Creates the constructor parameters.</summary>
    /// <param name="constructor">The constructor to create properties for.</param>
    /// <param name="memberProperties">The type's member properties.</param>
    /// <returns>Properties for the given <see cref="T:System.Reflection.ConstructorInfo" />.</returns>
    protected virtual IList<JsonProperty> CreateConstructorParameters(
      ConstructorInfo constructor,
      JsonPropertyCollection memberProperties)
    {
      ParameterInfo[] parameters = constructor.GetParameters();
      JsonPropertyCollection constructorParameters = new JsonPropertyCollection(constructor.DeclaringType);
      foreach (ParameterInfo parameterInfo in parameters)
      {
        JsonProperty matchingMemberProperty = parameterInfo.Name != null ? memberProperties.GetClosestMatchProperty(parameterInfo.Name) : (JsonProperty) null;
        if (matchingMemberProperty != null && matchingMemberProperty.PropertyType != parameterInfo.ParameterType)
          matchingMemberProperty = (JsonProperty) null;
        if (matchingMemberProperty != null || parameterInfo.Name != null)
        {
          JsonProperty constructorParameter = this.CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);
          if (constructorParameter != null)
            constructorParameters.AddProperty(constructorParameter);
        }
      }
      return (IList<JsonProperty>) constructorParameters;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.ParameterInfo" />.
    /// </summary>
    /// <param name="matchingMemberProperty">The matching member property.</param>
    /// <param name="parameterInfo">The constructor parameter.</param>
    /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.ParameterInfo" />.</returns>
    protected virtual JsonProperty CreatePropertyFromConstructorParameter(
      JsonProperty matchingMemberProperty,
      ParameterInfo parameterInfo)
    {
      JsonProperty property = new JsonProperty();
      property.PropertyType = parameterInfo.ParameterType;
      property.AttributeProvider = (IAttributeProvider) new ReflectionAttributeProvider((object) parameterInfo);
      this.SetPropertySettingsFromAttributes(property, (object) parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out bool _);
      property.Readable = false;
      property.Writable = true;
      if (matchingMemberProperty != null)
      {
        property.PropertyName = property.PropertyName != parameterInfo.Name ? property.PropertyName : matchingMemberProperty.PropertyName;
        property.Converter = property.Converter ?? matchingMemberProperty.Converter;
        property.MemberConverter = property.MemberConverter ?? matchingMemberProperty.MemberConverter;
        if (!property._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
          property.DefaultValue = matchingMemberProperty.DefaultValue;
        JsonProperty jsonProperty1 = property;
        Required? required = property._required;
        Required? nullable1 = required.HasValue ? new Required?(required.GetValueOrDefault()) : matchingMemberProperty._required;
        jsonProperty1._required = nullable1;
        JsonProperty jsonProperty2 = property;
        bool? isReference = property.IsReference;
        bool? nullable2 = isReference.HasValue ? new bool?(isReference.GetValueOrDefault()) : matchingMemberProperty.IsReference;
        jsonProperty2.IsReference = nullable2;
        JsonProperty jsonProperty3 = property;
        NullValueHandling? nullValueHandling = property.NullValueHandling;
        NullValueHandling? nullable3 = nullValueHandling.HasValue ? new NullValueHandling?(nullValueHandling.GetValueOrDefault()) : matchingMemberProperty.NullValueHandling;
        jsonProperty3.NullValueHandling = nullable3;
        JsonProperty jsonProperty4 = property;
        DefaultValueHandling? defaultValueHandling = property.DefaultValueHandling;
        DefaultValueHandling? nullable4 = defaultValueHandling.HasValue ? new DefaultValueHandling?(defaultValueHandling.GetValueOrDefault()) : matchingMemberProperty.DefaultValueHandling;
        jsonProperty4.DefaultValueHandling = nullable4;
        JsonProperty jsonProperty5 = property;
        ReferenceLoopHandling? referenceLoopHandling = property.ReferenceLoopHandling;
        ReferenceLoopHandling? nullable5 = referenceLoopHandling.HasValue ? new ReferenceLoopHandling?(referenceLoopHandling.GetValueOrDefault()) : matchingMemberProperty.ReferenceLoopHandling;
        jsonProperty5.ReferenceLoopHandling = nullable5;
        JsonProperty jsonProperty6 = property;
        ObjectCreationHandling? creationHandling = property.ObjectCreationHandling;
        ObjectCreationHandling? nullable6 = creationHandling.HasValue ? new ObjectCreationHandling?(creationHandling.GetValueOrDefault()) : matchingMemberProperty.ObjectCreationHandling;
        jsonProperty6.ObjectCreationHandling = nullable6;
        JsonProperty jsonProperty7 = property;
        TypeNameHandling? typeNameHandling = property.TypeNameHandling;
        TypeNameHandling? nullable7 = typeNameHandling.HasValue ? new TypeNameHandling?(typeNameHandling.GetValueOrDefault()) : matchingMemberProperty.TypeNameHandling;
        jsonProperty7.TypeNameHandling = nullable7;
      }
      return property;
    }

    /// <summary>
    /// Resolves the default <see cref="T:Newtonsoft.Json.JsonConverter" /> for the contract.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The contract's default <see cref="T:Newtonsoft.Json.JsonConverter" />.</returns>
    protected virtual JsonConverter ResolveContractConverter(Type objectType)
    {
      return JsonTypeReflector.GetJsonConverter((object) objectType);
    }

    private Func<object> GetDefaultCreator(Type createdType)
    {
      return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
    }

    [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Runtime.Serialization.DataContractAttribute.#get_IsReference()")]
    private void InitializeContract(JsonContract contract)
    {
      JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) contract.NonNullableUnderlyingType);
      if (cachedAttribute != null)
      {
        contract.IsReference = cachedAttribute._isReference;
      }
      else
      {
        DataContractAttribute contractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);
        if (contractAttribute != null && contractAttribute.IsReference)
          contract.IsReference = new bool?(true);
      }
      contract.Converter = this.ResolveContractConverter(contract.NonNullableUnderlyingType);
      contract.InternalConverter = JsonSerializer.GetMatchingConverter((IList<JsonConverter>) DefaultContractResolver.BuiltInConverters, contract.NonNullableUnderlyingType);
      if (contract.IsInstantiable && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
      {
        contract.DefaultCreator = this.GetDefaultCreator(contract.CreatedType);
        contract.DefaultCreatorNonPublic = !contract.CreatedType.IsValueType() && ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null;
      }
      this.ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
    }

    private void ResolveCallbackMethods(JsonContract contract, Type t)
    {
      List<SerializationCallback> onSerializing;
      List<SerializationCallback> onSerialized;
      List<SerializationCallback> onDeserializing;
      List<SerializationCallback> onDeserialized;
      List<SerializationErrorCallback> onError;
      this.GetCallbackMethodsForType(t, out onSerializing, out onSerialized, out onDeserializing, out onDeserialized, out onError);
      if (onSerializing != null)
        contract.OnSerializingCallbacks.AddRange<SerializationCallback>((IEnumerable<SerializationCallback>) onSerializing);
      if (onSerialized != null)
        contract.OnSerializedCallbacks.AddRange<SerializationCallback>((IEnumerable<SerializationCallback>) onSerialized);
      if (onDeserializing != null)
        contract.OnDeserializingCallbacks.AddRange<SerializationCallback>((IEnumerable<SerializationCallback>) onDeserializing);
      if (onDeserialized != null)
        contract.OnDeserializedCallbacks.AddRange<SerializationCallback>((IEnumerable<SerializationCallback>) onDeserialized);
      if (onError == null)
        return;
      contract.OnErrorCallbacks.AddRange<SerializationErrorCallback>((IEnumerable<SerializationErrorCallback>) onError);
    }

    private void GetCallbackMethodsForType(
      Type type,
      out List<SerializationCallback> onSerializing,
      out List<SerializationCallback> onSerialized,
      out List<SerializationCallback> onDeserializing,
      out List<SerializationCallback> onDeserialized,
      out List<SerializationErrorCallback> onError)
    {
      onSerializing = (List<SerializationCallback>) null;
      onSerialized = (List<SerializationCallback>) null;
      onDeserializing = (List<SerializationCallback>) null;
      onDeserialized = (List<SerializationCallback>) null;
      onError = (List<SerializationErrorCallback>) null;
      foreach (Type t in this.GetClassHierarchyForType(type))
      {
        MethodInfo currentCallback1 = (MethodInfo) null;
        MethodInfo currentCallback2 = (MethodInfo) null;
        MethodInfo currentCallback3 = (MethodInfo) null;
        MethodInfo currentCallback4 = (MethodInfo) null;
        MethodInfo currentCallback5 = (MethodInfo) null;
        bool flag1 = DefaultContractResolver.ShouldSkipSerializing(t);
        bool flag2 = DefaultContractResolver.ShouldSkipDeserialized(t);
        foreach (MethodInfo method in t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
          if (!method.ContainsGenericParameters)
          {
            Type prevAttributeType = (Type) null;
            ParameterInfo[] parameters = method.GetParameters();
            if (!flag1 && DefaultContractResolver.IsValidCallback(method, parameters, typeof (OnSerializingAttribute), currentCallback1, ref prevAttributeType))
            {
              onSerializing = onSerializing ?? new List<SerializationCallback>();
              onSerializing.Add(JsonContract.CreateSerializationCallback(method));
              currentCallback1 = method;
            }
            if (DefaultContractResolver.IsValidCallback(method, parameters, typeof (OnSerializedAttribute), currentCallback2, ref prevAttributeType))
            {
              onSerialized = onSerialized ?? new List<SerializationCallback>();
              onSerialized.Add(JsonContract.CreateSerializationCallback(method));
              currentCallback2 = method;
            }
            if (DefaultContractResolver.IsValidCallback(method, parameters, typeof (OnDeserializingAttribute), currentCallback3, ref prevAttributeType))
            {
              onDeserializing = onDeserializing ?? new List<SerializationCallback>();
              onDeserializing.Add(JsonContract.CreateSerializationCallback(method));
              currentCallback3 = method;
            }
            if (!flag2 && DefaultContractResolver.IsValidCallback(method, parameters, typeof (OnDeserializedAttribute), currentCallback4, ref prevAttributeType))
            {
              onDeserialized = onDeserialized ?? new List<SerializationCallback>();
              onDeserialized.Add(JsonContract.CreateSerializationCallback(method));
              currentCallback4 = method;
            }
            if (DefaultContractResolver.IsValidCallback(method, parameters, typeof (OnErrorAttribute), currentCallback5, ref prevAttributeType))
            {
              onError = onError ?? new List<SerializationErrorCallback>();
              onError.Add(JsonContract.CreateSerializationErrorCallback(method));
              currentCallback5 = method;
            }
          }
        }
      }
    }

    private static bool ShouldSkipDeserialized(Type t)
    {
      return t.Name == "FSharpSet`1" || t.Name == "FSharpMap`2";
    }

    private static bool ShouldSkipSerializing(Type t)
    {
      return t.Name == "FSharpSet`1" || t.Name == "FSharpMap`2";
    }

    private List<Type> GetClassHierarchyForType(Type type)
    {
      List<Type> hierarchyForType = new List<Type>();
      for (Type type1 = type; type1 != null && type1 != typeof (object); type1 = type1.BaseType())
        hierarchyForType.Add(type1);
      hierarchyForType.Reverse();
      return hierarchyForType;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonDictionaryContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonDictionaryContract" /> for the given type.</returns>
    protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
      JsonDictionaryContract contract = new JsonDictionaryContract(objectType);
      this.InitializeContract((JsonContract) contract);
      contract.DictionaryKeyResolver = new Func<string, string>(this.ResolveDictionaryKey);
      return contract;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> for the given type.</returns>
    protected virtual JsonArrayContract CreateArrayContract(Type objectType)
    {
      JsonArrayContract contract = new JsonArrayContract(objectType);
      this.InitializeContract((JsonContract) contract);
      return contract;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" /> for the given type.</returns>
    protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
    {
      JsonPrimitiveContract contract = new JsonPrimitiveContract(objectType);
      this.InitializeContract((JsonContract) contract);
      return contract;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonLinqContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonLinqContract" /> for the given type.</returns>
    protected virtual JsonLinqContract CreateLinqContract(Type objectType)
    {
      JsonLinqContract contract = new JsonLinqContract(objectType);
      this.InitializeContract((JsonContract) contract);
      return contract;
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonStringContract" /> for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonStringContract" /> for the given type.</returns>
    protected virtual JsonStringContract CreateStringContract(Type objectType)
    {
      JsonStringContract contract = new JsonStringContract(objectType);
      this.InitializeContract((JsonContract) contract);
      return contract;
    }

    /// <summary>
    /// Determines which contract type is created for the given type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for the given type.</returns>
    protected virtual JsonContract CreateContract(Type objectType)
    {
      if (DefaultContractResolver.IsJsonPrimitiveType(objectType))
        return (JsonContract) this.CreatePrimitiveContract(objectType);
      Type type = ReflectionUtils.EnsureNotNullableType(objectType);
      switch (JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>((object) type))
      {
        case JsonObjectAttribute _:
          return (JsonContract) this.CreateObjectContract(objectType);
        case JsonArrayAttribute _:
          return (JsonContract) this.CreateArrayContract(objectType);
        case JsonDictionaryAttribute _:
          return (JsonContract) this.CreateDictionaryContract(objectType);
        default:
          if (type == typeof (JToken) || type.IsSubclassOf(typeof (JToken)))
            return (JsonContract) this.CreateLinqContract(objectType);
          if (CollectionUtils.IsDictionaryType(type))
            return (JsonContract) this.CreateDictionaryContract(objectType);
          if (typeof (IEnumerable).IsAssignableFrom(type))
            return (JsonContract) this.CreateArrayContract(objectType);
          if (DefaultContractResolver.CanConvertToString(type))
            return (JsonContract) this.CreateStringContract(objectType);
          return DefaultContractResolver.IsIConvertible(type) ? (JsonContract) this.CreatePrimitiveContract(type) : (JsonContract) this.CreateObjectContract(objectType);
      }
    }

    internal static bool IsJsonPrimitiveType(Type t)
    {
      PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);
      return typeCode != PrimitiveTypeCode.Empty && typeCode != PrimitiveTypeCode.Object;
    }

    internal static bool IsIConvertible(Type t)
    {
      return (typeof (IConvertible).IsAssignableFrom(t) || ReflectionUtils.IsNullableType(t) && typeof (IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t))) && !typeof (JToken).IsAssignableFrom(t);
    }

    internal static bool CanConvertToString(Type type)
    {
      return type == typeof (Type) || type.IsSubclassOf(typeof (Type));
    }

    private static bool IsValidCallback(
      MethodInfo method,
      ParameterInfo[] parameters,
      Type attributeType,
      MethodInfo currentCallback,
      ref Type prevAttributeType)
    {
      if (!method.IsDefined(attributeType, false))
        return false;
      if (currentCallback != null)
        throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) method, (object) currentCallback, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) attributeType));
      if (prevAttributeType != null)
        throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) prevAttributeType, (object) attributeType, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) method));
      if (method.IsVirtual)
        throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) method, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) attributeType));
      if (method.ReturnType != typeof (void))
        throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) method));
      if (attributeType == typeof (OnErrorAttribute))
      {
        if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof (StreamingContext) || parameters[1].ParameterType != typeof (ErrorContext))
          throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) method, (object) typeof (StreamingContext), (object) typeof (ErrorContext)));
      }
      else if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof (StreamingContext))
        throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), (object) method, (object) typeof (StreamingContext)));
      prevAttributeType = attributeType;
      return true;
    }

    internal static string GetClrTypeFullName(Type type)
    {
      if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
        return type.FullName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) type.Namespace, (object) type.Name);
    }

    /// <summary>
    /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
    /// </summary>
    /// <param name="type">The type to create properties for.</param>
    /// 
    ///             /// <param name="memberSerialization">The member serialization mode for the type.</param>
    /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
    protected virtual IList<JsonProperty> CreateProperties(
      Type type,
      MemberSerialization memberSerialization)
    {
      List<MemberInfo> serializableMembers = this.GetSerializableMembers(type);
      if (serializableMembers == null)
        throw new JsonSerializationException("Null collection of seralizable members returned.");
      JsonPropertyCollection source = new JsonPropertyCollection(type);
      foreach (MemberInfo member in serializableMembers)
      {
        JsonProperty property = this.CreateProperty(member, memberSerialization);
        if (property != null)
        {
          DefaultContractResolverState state = this.GetState();
          lock (state.NameTable)
            property.PropertyName = state.NameTable.Add(property.PropertyName);
          source.AddProperty(property);
        }
      }
      return (IList<JsonProperty>) source.OrderBy<JsonProperty, int>((Func<JsonProperty, int>) (p => p.Order ?? -1)).ToList<JsonProperty>();
    }

    /// <summary>
    /// Creates the <see cref="T:Newtonsoft.Json.Serialization.IValueProvider" /> used by the serializer to get and set values from a member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>The <see cref="T:Newtonsoft.Json.Serialization.IValueProvider" /> used by the serializer to get and set values from a member.</returns>
    protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
    {
      return (IValueProvider) new ReflectionValueProvider(member);
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
    /// </summary>
    /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
    /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
    /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
    protected virtual JsonProperty CreateProperty(
      MemberInfo member,
      MemberSerialization memberSerialization)
    {
      JsonProperty property = new JsonProperty();
      property.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
      property.DeclaringType = member.DeclaringType;
      property.ValueProvider = this.CreateMemberValueProvider(member);
      property.AttributeProvider = (IAttributeProvider) new ReflectionAttributeProvider((object) member);
      bool allowNonPublicAccess;
      this.SetPropertySettingsFromAttributes(property, (object) member, member.Name, member.DeclaringType, memberSerialization, out allowNonPublicAccess);
      if (memberSerialization != MemberSerialization.Fields)
      {
        property.Readable = ReflectionUtils.CanReadMemberValue(member, allowNonPublicAccess);
        property.Writable = ReflectionUtils.CanSetMemberValue(member, allowNonPublicAccess, property.HasMemberAttribute);
      }
      else
      {
        property.Readable = true;
        property.Writable = true;
      }
      property.ShouldSerialize = this.CreateShouldSerializeTest(member);
      this.SetIsSpecifiedActions(property, member, allowNonPublicAccess);
      return property;
    }

    private void SetPropertySettingsFromAttributes(
      JsonProperty property,
      object attributeProvider,
      string name,
      Type declaringType,
      MemberSerialization memberSerialization,
      out bool allowNonPublicAccess)
    {
      DataContractAttribute contractAttribute = JsonTypeReflector.GetDataContractAttribute(declaringType);
      MemberInfo memberInfo = attributeProvider as MemberInfo;
      DataMemberAttribute dataMemberAttribute = contractAttribute == null || memberInfo == null ? (DataMemberAttribute) null : JsonTypeReflector.GetDataMemberAttribute(memberInfo);
      JsonPropertyAttribute attribute1 = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
      JsonRequiredAttribute attribute2 = JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(attributeProvider);
      string propertyName = attribute1 == null || attribute1.PropertyName == null ? (dataMemberAttribute == null || dataMemberAttribute.Name == null ? name : dataMemberAttribute.Name) : attribute1.PropertyName;
      property.PropertyName = this.ResolvePropertyName(propertyName);
      property.UnderlyingName = name;
      bool flag1 = false;
      if (attribute1 != null)
      {
        property._required = attribute1._required;
        property.Order = attribute1._order;
        property.DefaultValueHandling = attribute1._defaultValueHandling;
        flag1 = true;
      }
      else if (dataMemberAttribute != null)
      {
        property._required = new Required?(dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default);
        property.Order = dataMemberAttribute.Order != -1 ? new int?(dataMemberAttribute.Order) : new int?();
        property.DefaultValueHandling = !dataMemberAttribute.EmitDefaultValue ? new DefaultValueHandling?(DefaultValueHandling.Ignore) : new DefaultValueHandling?();
        flag1 = true;
      }
      if (attribute2 != null)
      {
        property._required = new Required?(Required.Always);
        flag1 = true;
      }
      property.HasMemberAttribute = flag1;
      bool flag2 = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null || JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null;
      if (memberSerialization != MemberSerialization.OptIn)
      {
        bool flag3 = JsonTypeReflector.GetAttribute<IgnoreDataMemberAttribute>(attributeProvider) != null;
        property.Ignored = flag2 || flag3;
      }
      else
        property.Ignored = flag2 || !flag1;
      property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
      property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider);
      DefaultValueAttribute attribute3 = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
      if (attribute3 != null)
        property.DefaultValue = attribute3.Value;
      property.NullValueHandling = (NullValueHandling?) attribute1?._nullValueHandling;
      property.ReferenceLoopHandling = (ReferenceLoopHandling?) attribute1?._referenceLoopHandling;
      property.ObjectCreationHandling = (ObjectCreationHandling?) attribute1?._objectCreationHandling;
      property.TypeNameHandling = (TypeNameHandling?) attribute1?._typeNameHandling;
      property.IsReference = (bool?) attribute1?._isReference;
      property.ItemIsReference = (bool?) attribute1?._itemIsReference;
      property.ItemConverter = attribute1 == null || attribute1.ItemConverterType == null ? (JsonConverter) null : JsonTypeReflector.CreateJsonConverterInstance(attribute1.ItemConverterType, attribute1.ItemConverterParameters);
      property.ItemReferenceLoopHandling = (ReferenceLoopHandling?) attribute1?._itemReferenceLoopHandling;
      property.ItemTypeNameHandling = (TypeNameHandling?) attribute1?._itemTypeNameHandling;
      allowNonPublicAccess = false;
      if ((this.DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
        allowNonPublicAccess = true;
      if (flag1)
        allowNonPublicAccess = true;
      if (memberSerialization != MemberSerialization.Fields)
        return;
      allowNonPublicAccess = true;
    }

    private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
    {
      MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, ReflectionUtils.EmptyTypes);
      if (method == null || method.ReturnType != typeof (bool))
        return (Predicate<object>) null;
      MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) method);
      return (Predicate<object>) (o => (bool) shouldSerializeCall(o, new object[0]));
    }

    private void SetIsSpecifiedActions(
      JsonProperty property,
      MemberInfo member,
      bool allowNonPublicAccess)
    {
      MemberInfo memberInfo = (MemberInfo) member.DeclaringType.GetProperty(member.Name + "Specified") ?? (MemberInfo) member.DeclaringType.GetField(member.Name + "Specified");
      if (memberInfo == null || ReflectionUtils.GetMemberUnderlyingType(memberInfo) != typeof (bool))
        return;
      Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(memberInfo);
      property.GetIsSpecified = (Predicate<object>) (o => (bool) specifiedPropertyGet(o));
      if (!ReflectionUtils.CanSetMemberValue(memberInfo, allowNonPublicAccess, false))
        return;
      property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(memberInfo);
    }

    /// <summary>Resolves the name of the property.</summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Resolved name of the property.</returns>
    protected virtual string ResolvePropertyName(string propertyName) => propertyName;

    /// <summary>
    /// Resolves the key of the dictionary. By default <see cref="M:Newtonsoft.Json.Serialization.DefaultContractResolver.ResolvePropertyName(System.String)" /> is used to resolve dictionary keys.
    /// </summary>
    /// <param name="dictionaryKey">Key of the dictionary.</param>
    /// <returns>Resolved key of the dictionary.</returns>
    protected virtual string ResolveDictionaryKey(string dictionaryKey)
    {
      return this.ResolvePropertyName(dictionaryKey);
    }

    /// <summary>Gets the resolved name of the property.</summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Name of the property.</returns>
    public string GetResolvedPropertyName(string propertyName)
    {
      return this.ResolvePropertyName(propertyName);
    }

    internal struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : 
      IEnumerable<KeyValuePair<object, object>>,
      IEnumerable,
      IEnumerator<KeyValuePair<object, object>>,
      IEnumerator,
      IDisposable
    {
      private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

      public DictionaryEnumerator(
        IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
      {
        ValidationUtils.ArgumentNotNull((object) e, nameof (e));
        this._e = e.GetEnumerator();
      }

      public bool MoveNext() => this._e.MoveNext();

      public void Reset() => this._e.Reset();

      public KeyValuePair<object, object> Current
      {
        get
        {
          return new KeyValuePair<object, object>((object) this._e.Current.Key, (object) this._e.Current.Value);
        }
      }

      public void Dispose() => this._e.Dispose();

      object IEnumerator.Current => (object) this.Current;

      public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
      {
        return (IEnumerator<KeyValuePair<object, object>>) this;
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this;
    }
  }
}
