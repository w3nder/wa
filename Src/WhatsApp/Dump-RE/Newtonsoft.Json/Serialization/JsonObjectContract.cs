// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonObjectContract
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections.ObjectModel;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public class JsonObjectContract : JsonContainerContract
  {
    private bool? _hasRequiredOrDefaultValueProperties;
    private ConstructorInfo _parametrizedConstructor;
    private ConstructorInfo _overrideConstructor;
    private ObjectConstructor<object> _overrideCreator;
    private ObjectConstructor<object> _parametrizedCreator;

    /// <summary>Gets or sets the object member serialization.</summary>
    /// <value>The member object serialization.</value>
    public MemberSerialization MemberSerialization { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the object's properties are required.
    /// </summary>
    /// <value>
    /// 	A value indicating whether the object's properties are required.
    /// </value>
    public Required? ItemRequired { get; set; }

    /// <summary>Gets the object's properties.</summary>
    /// <value>The object's properties.</value>
    public JsonPropertyCollection Properties { get; private set; }

    /// <summary>
    /// Gets the constructor parameters required for any non-default constructor
    /// </summary>
    [Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
    public JsonPropertyCollection ConstructorParameters => this.CreatorParameters;

    /// <summary>
    /// Gets a collection of <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> instances that define the parameters used with <see cref="P:Newtonsoft.Json.Serialization.JsonObjectContract.OverrideCreator" />.
    /// </summary>
    public JsonPropertyCollection CreatorParameters { get; private set; }

    /// <summary>
    /// Gets or sets the override constructor used to create the object.
    /// This is set when a constructor is marked up using the
    /// JsonConstructor attribute.
    /// </summary>
    /// <value>The override constructor.</value>
    [Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
    public ConstructorInfo OverrideConstructor
    {
      get => this._overrideConstructor;
      set
      {
        this._overrideConstructor = value;
        this._overrideCreator = value != null ? JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) value) : (ObjectConstructor<object>) null;
      }
    }

    /// <summary>
    /// Gets or sets the parametrized constructor used to create the object.
    /// </summary>
    /// <value>The parametrized constructor.</value>
    [Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
    public ConstructorInfo ParametrizedConstructor
    {
      get => this._parametrizedConstructor;
      set
      {
        this._parametrizedConstructor = value;
        this._parametrizedCreator = value != null ? JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) value) : (ObjectConstructor<object>) null;
      }
    }

    /// <summary>
    /// Gets or sets the function used to create the object. When set this function will override <see cref="P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator" />.
    /// This function is called with a collection of arguments which are defined by the <see cref="P:Newtonsoft.Json.Serialization.JsonObjectContract.CreatorParameters" /> collection.
    /// </summary>
    /// <value>The function used to create the object.</value>
    public ObjectConstructor<object> OverrideCreator
    {
      get => this._overrideCreator;
      set
      {
        this._overrideCreator = value;
        this._overrideConstructor = (ConstructorInfo) null;
      }
    }

    internal ObjectConstructor<object> ParametrizedCreator => this._parametrizedCreator;

    /// <summary>Gets or sets the extension data setter.</summary>
    public ExtensionDataSetter ExtensionDataSetter { get; set; }

    /// <summary>Gets or sets the extension data getter.</summary>
    public ExtensionDataGetter ExtensionDataGetter { get; set; }

    internal bool HasRequiredOrDefaultValueProperties
    {
      get
      {
        if (!this._hasRequiredOrDefaultValueProperties.HasValue)
        {
          this._hasRequiredOrDefaultValueProperties = new bool?(false);
          if (this.ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
          {
            this._hasRequiredOrDefaultValueProperties = new bool?(true);
          }
          else
          {
            foreach (JsonProperty property in (Collection<JsonProperty>) this.Properties)
            {
              if (property.Required == Required.Default)
              {
                DefaultValueHandling? defaultValueHandling = property.DefaultValueHandling;
                DefaultValueHandling? nullable = defaultValueHandling.HasValue ? new DefaultValueHandling?(defaultValueHandling.GetValueOrDefault() & DefaultValueHandling.Populate) : new DefaultValueHandling?();
                if ((nullable.GetValueOrDefault() != DefaultValueHandling.Populate ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
                  continue;
              }
              this._hasRequiredOrDefaultValueProperties = new bool?(true);
              break;
            }
          }
        }
        return this._hasRequiredOrDefaultValueProperties.Value;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> class.
    /// </summary>
    /// <param name="underlyingType">The underlying type for the contract.</param>
    public JsonObjectContract(Type underlyingType)
      : base(underlyingType)
    {
      this.ContractType = JsonContractType.Object;
      this.Properties = new JsonPropertyCollection(this.UnderlyingType);
      this.CreatorParameters = new JsonPropertyCollection(this.UnderlyingType);
    }
  }
}
