// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonArrayContract
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public class JsonArrayContract : JsonContainerContract
  {
    private readonly Type _genericCollectionDefinitionType;
    private Type _genericWrapperType;
    private ObjectConstructor<object> _genericWrapperCreator;
    private Func<object> _genericTemporaryCollectionCreator;
    private readonly ConstructorInfo _parametrizedConstructor;
    private ObjectConstructor<object> _parametrizedCreator;

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the collection items.
    /// </summary>
    /// <value>The <see cref="T:System.Type" /> of the collection items.</value>
    public Type CollectionItemType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the collection type is a multidimensional array.
    /// </summary>
    /// <value><c>true</c> if the collection type is a multidimensional array; otherwise, <c>false</c>.</value>
    public bool IsMultidimensionalArray { get; private set; }

    internal bool IsArray { get; private set; }

    internal bool ShouldCreateWrapper { get; private set; }

    internal bool CanDeserialize { get; private set; }

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
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> class.
    /// </summary>
    /// <param name="underlyingType">The underlying type for the contract.</param>
    public JsonArrayContract(Type underlyingType)
      : base(underlyingType)
    {
      this.ContractType = JsonContractType.Array;
      this.IsArray = this.CreatedType.IsArray;
      bool flag;
      if (this.IsArray)
      {
        this.CollectionItemType = ReflectionUtils.GetCollectionItemType(this.UnderlyingType);
        this.IsReadOnlyOrFixedSize = true;
        this._genericCollectionDefinitionType = typeof (List<>).MakeGenericType(this.CollectionItemType);
        flag = true;
        this.IsMultidimensionalArray = this.IsArray && this.UnderlyingType.GetArrayRank() > 1;
      }
      else if (typeof (IList).IsAssignableFrom(underlyingType))
      {
        this.CollectionItemType = !ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (ICollection<>), out this._genericCollectionDefinitionType) ? ReflectionUtils.GetCollectionItemType(underlyingType) : this._genericCollectionDefinitionType.GetGenericArguments()[0];
        if (underlyingType == typeof (IList))
          this.CreatedType = typeof (List<object>);
        if (this.CollectionItemType != null)
          this._parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
        this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof (ReadOnlyCollection<>));
        flag = true;
      }
      else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (ICollection<>), out this._genericCollectionDefinitionType))
      {
        this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
        if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof (ICollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IList<>)))
          this.CreatedType = typeof (List<>).MakeGenericType(this.CollectionItemType);
        this._parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
        flag = true;
        this.ShouldCreateWrapper = true;
      }
      else
      {
        Type implementingType;
        if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IEnumerable<>), out implementingType))
        {
          this.CollectionItemType = implementingType.GetGenericArguments()[0];
          if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof (IEnumerable<>)))
            this.CreatedType = typeof (List<>).MakeGenericType(this.CollectionItemType);
          this._parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
          if (!this.HasParametrizedCreator && underlyingType.Name == "FSharpList`1")
          {
            FSharpUtils.EnsureInitialized(underlyingType.Assembly());
            this._parametrizedCreator = FSharpUtils.CreateSeq(this.CollectionItemType);
          }
          if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
          {
            this._genericCollectionDefinitionType = implementingType;
            this.IsReadOnlyOrFixedSize = false;
            this.ShouldCreateWrapper = false;
            flag = true;
          }
          else
          {
            this._genericCollectionDefinitionType = typeof (List<>).MakeGenericType(this.CollectionItemType);
            this.IsReadOnlyOrFixedSize = true;
            this.ShouldCreateWrapper = true;
            flag = this.HasParametrizedCreator;
          }
        }
        else
        {
          flag = false;
          this.ShouldCreateWrapper = true;
        }
      }
      this.CanDeserialize = flag;
    }

    internal IWrappedCollection CreateWrapper(object list)
    {
      if (this._genericWrapperCreator == null)
      {
        this._genericWrapperType = typeof (CollectionWrapper<>).MakeGenericType(this.CollectionItemType);
        Type type;
        if (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof (List<>)) || this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
          type = typeof (ICollection<>).MakeGenericType(this.CollectionItemType);
        else
          type = this._genericCollectionDefinitionType;
        this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) this._genericWrapperType.GetConstructor(new Type[1]
        {
          type
        }));
      }
      return (IWrappedCollection) this._genericWrapperCreator(new object[1]
      {
        list
      });
    }

    internal IList CreateTemporaryCollection()
    {
      if (this._genericTemporaryCollectionCreator == null)
        this._genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(typeof (List<>).MakeGenericType(this.IsMultidimensionalArray ? typeof (object) : this.CollectionItemType));
      return (IList) this._genericTemporaryCollectionCreator();
    }
  }
}
