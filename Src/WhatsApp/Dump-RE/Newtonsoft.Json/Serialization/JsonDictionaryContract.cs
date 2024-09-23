// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonDictionaryContract
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public class JsonDictionaryContract : JsonContainerContract
  {
    private readonly Type _genericCollectionDefinitionType;
    private Type _genericWrapperType;
    private ObjectConstructor<object> _genericWrapperCreator;
    private Func<object> _genericTemporaryDictionaryCreator;
    private readonly ConstructorInfo _parametrizedConstructor;
    private ObjectConstructor<object> _parametrizedCreator;

    /// <summary>Gets or sets the property name resolver.</summary>
    /// <value>The property name resolver.</value>
    [Obsolete("PropertyNameResolver is obsolete. Use DictionaryKeyResolver instead.")]
    public Func<string, string> PropertyNameResolver
    {
      get => this.DictionaryKeyResolver;
      set => this.DictionaryKeyResolver = value;
    }

    /// <summary>Gets or sets the dictionary key resolver.</summary>
    /// <value>The dictionary key resolver.</value>
    public Func<string, string> DictionaryKeyResolver { get; set; }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the dictionary keys.
    /// </summary>
    /// <value>The <see cref="T:System.Type" /> of the dictionary keys.</value>
    public Type DictionaryKeyType { get; private set; }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the dictionary values.
    /// </summary>
    /// <value>The <see cref="T:System.Type" /> of the dictionary values.</value>
    public Type DictionaryValueType { get; private set; }

    internal JsonContract KeyContract { get; set; }

    internal bool ShouldCreateWrapper { get; private set; }

    internal ObjectConstructor<object> ParametrizedCreator
    {
      get
      {
        if (this._parametrizedCreator == null)
          this._parametrizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) this._parametrizedConstructor);
        return this._parametrizedCreator;
      }
    }

    internal bool HasParametrizedCreator
    {
      get => this._parametrizedCreator != null || this._parametrizedConstructor != null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonDictionaryContract" /> class.
    /// </summary>
    /// <param name="underlyingType">The underlying type for the contract.</param>
    public JsonDictionaryContract(Type underlyingType)
      : base(underlyingType)
    {
      this.ContractType = JsonContractType.Dictionary;
      Type keyType;
      Type valueType;
      if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IDictionary<,>), out this._genericCollectionDefinitionType))
      {
        keyType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
        valueType = this._genericCollectionDefinitionType.GetGenericArguments()[1];
        if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof (IDictionary<,>)))
          this.CreatedType = typeof (Dictionary<,>).MakeGenericType(keyType, valueType);
      }
      else
      {
        ReflectionUtils.GetDictionaryKeyValueTypes(this.UnderlyingType, out keyType, out valueType);
        if (this.UnderlyingType == typeof (IDictionary))
          this.CreatedType = typeof (Dictionary<object, object>);
      }
      if (keyType != null && valueType != null)
      {
        this._parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(this.CreatedType, typeof (KeyValuePair<,>).MakeGenericType(keyType, valueType));
        if (!this.HasParametrizedCreator && underlyingType.Name == "FSharpMap`2")
        {
          FSharpUtils.EnsureInitialized(underlyingType.Assembly());
          this._parametrizedCreator = FSharpUtils.CreateMap(keyType, valueType);
        }
      }
      this.ShouldCreateWrapper = !typeof (IDictionary).IsAssignableFrom(this.CreatedType);
      this.DictionaryKeyType = keyType;
      this.DictionaryValueType = valueType;
    }

    internal IWrappedDictionary CreateWrapper(object dictionary)
    {
      if (this._genericWrapperCreator == null)
      {
        this._genericWrapperType = typeof (DictionaryWrapper<,>).MakeGenericType(this.DictionaryKeyType, this.DictionaryValueType);
        this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) this._genericWrapperType.GetConstructor(new Type[1]
        {
          this._genericCollectionDefinitionType
        }));
      }
      return (IWrappedDictionary) this._genericWrapperCreator(new object[1]
      {
        dictionary
      });
    }

    internal IDictionary CreateTemporaryDictionary()
    {
      if (this._genericTemporaryDictionaryCreator == null)
        this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(typeof (Dictionary<,>).MakeGenericType(this.DictionaryKeyType, this.DictionaryValueType));
      return (IDictionary) this._genericTemporaryDictionaryCreator();
    }
  }
}
