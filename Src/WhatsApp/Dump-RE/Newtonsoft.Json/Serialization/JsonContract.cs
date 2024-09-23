// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonContract
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public abstract class JsonContract
  {
    internal bool IsNullable;
    internal bool IsConvertable;
    internal bool IsEnum;
    internal Type NonNullableUnderlyingType;
    internal ReadType InternalReadType;
    internal JsonContractType ContractType;
    internal bool IsReadOnlyOrFixedSize;
    internal bool IsSealed;
    internal bool IsInstantiable;
    private List<SerializationCallback> _onDeserializedCallbacks;
    private IList<SerializationCallback> _onDeserializingCallbacks;
    private IList<SerializationCallback> _onSerializedCallbacks;
    private IList<SerializationCallback> _onSerializingCallbacks;
    private IList<SerializationErrorCallback> _onErrorCallbacks;
    private Type _createdType;

    /// <summary>Gets the underlying type for the contract.</summary>
    /// <value>The underlying type for the contract.</value>
    public Type UnderlyingType { get; private set; }

    /// <summary>Gets or sets the type created during deserialization.</summary>
    /// <value>The type created during deserialization.</value>
    public Type CreatedType
    {
      get => this._createdType;
      set
      {
        this._createdType = value;
        this.IsSealed = this._createdType.IsSealed();
        this.IsInstantiable = !this._createdType.IsInterface() && !this._createdType.IsAbstract();
      }
    }

    /// <summary>
    /// Gets or sets whether this type contract is serialized as a reference.
    /// </summary>
    /// <value>Whether this type contract is serialized as a reference.</value>
    public bool? IsReference { get; set; }

    /// <summary>
    /// Gets or sets the default <see cref="T:Newtonsoft.Json.JsonConverter" /> for this contract.
    /// </summary>
    /// <value>The converter.</value>
    public JsonConverter Converter { get; set; }

    internal JsonConverter InternalConverter { get; set; }

    /// <summary>
    /// Gets or sets all methods called immediately after deserialization of the object.
    /// </summary>
    /// <value>The methods called immediately after deserialization of the object.</value>
    public IList<SerializationCallback> OnDeserializedCallbacks
    {
      get
      {
        if (this._onDeserializedCallbacks == null)
          this._onDeserializedCallbacks = new List<SerializationCallback>();
        return (IList<SerializationCallback>) this._onDeserializedCallbacks;
      }
    }

    /// <summary>
    /// Gets or sets all methods called during deserialization of the object.
    /// </summary>
    /// <value>The methods called during deserialization of the object.</value>
    public IList<SerializationCallback> OnDeserializingCallbacks
    {
      get
      {
        if (this._onDeserializingCallbacks == null)
          this._onDeserializingCallbacks = (IList<SerializationCallback>) new List<SerializationCallback>();
        return this._onDeserializingCallbacks;
      }
    }

    /// <summary>
    /// Gets or sets all methods called after serialization of the object graph.
    /// </summary>
    /// <value>The methods called after serialization of the object graph.</value>
    public IList<SerializationCallback> OnSerializedCallbacks
    {
      get
      {
        if (this._onSerializedCallbacks == null)
          this._onSerializedCallbacks = (IList<SerializationCallback>) new List<SerializationCallback>();
        return this._onSerializedCallbacks;
      }
    }

    /// <summary>
    /// Gets or sets all methods called before serialization of the object.
    /// </summary>
    /// <value>The methods called before serialization of the object.</value>
    public IList<SerializationCallback> OnSerializingCallbacks
    {
      get
      {
        if (this._onSerializingCallbacks == null)
          this._onSerializingCallbacks = (IList<SerializationCallback>) new List<SerializationCallback>();
        return this._onSerializingCallbacks;
      }
    }

    /// <summary>
    /// Gets or sets all method called when an error is thrown during the serialization of the object.
    /// </summary>
    /// <value>The methods called when an error is thrown during the serialization of the object.</value>
    public IList<SerializationErrorCallback> OnErrorCallbacks
    {
      get
      {
        if (this._onErrorCallbacks == null)
          this._onErrorCallbacks = (IList<SerializationErrorCallback>) new List<SerializationErrorCallback>();
        return this._onErrorCallbacks;
      }
    }

    /// <summary>
    /// Gets or sets the method called immediately after deserialization of the object.
    /// </summary>
    /// <value>The method called immediately after deserialization of the object.</value>
    [Obsolete("This property is obsolete and has been replaced by the OnDeserializedCallbacks collection.")]
    public MethodInfo OnDeserialized
    {
      get
      {
        return this.OnDeserializedCallbacks.Count <= 0 ? (MethodInfo) null : this.OnDeserializedCallbacks[0].Method();
      }
      set
      {
        this.OnDeserializedCallbacks.Clear();
        this.OnDeserializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
      }
    }

    /// <summary>
    /// Gets or sets the method called during deserialization of the object.
    /// </summary>
    /// <value>The method called during deserialization of the object.</value>
    [Obsolete("This property is obsolete and has been replaced by the OnDeserializingCallbacks collection.")]
    public MethodInfo OnDeserializing
    {
      get
      {
        return this.OnDeserializingCallbacks.Count <= 0 ? (MethodInfo) null : this.OnDeserializingCallbacks[0].Method();
      }
      set
      {
        this.OnDeserializingCallbacks.Clear();
        this.OnDeserializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
      }
    }

    /// <summary>
    /// Gets or sets the method called after serialization of the object graph.
    /// </summary>
    /// <value>The method called after serialization of the object graph.</value>
    [Obsolete("This property is obsolete and has been replaced by the OnSerializedCallbacks collection.")]
    public MethodInfo OnSerialized
    {
      get
      {
        return this.OnSerializedCallbacks.Count <= 0 ? (MethodInfo) null : this.OnSerializedCallbacks[0].Method();
      }
      set
      {
        this.OnSerializedCallbacks.Clear();
        this.OnSerializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
      }
    }

    /// <summary>
    /// Gets or sets the method called before serialization of the object.
    /// </summary>
    /// <value>The method called before serialization of the object.</value>
    [Obsolete("This property is obsolete and has been replaced by the OnSerializingCallbacks collection.")]
    public MethodInfo OnSerializing
    {
      get
      {
        return this.OnSerializingCallbacks.Count <= 0 ? (MethodInfo) null : this.OnSerializingCallbacks[0].Method();
      }
      set
      {
        this.OnSerializingCallbacks.Clear();
        this.OnSerializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
      }
    }

    /// <summary>
    /// Gets or sets the method called when an error is thrown during the serialization of the object.
    /// </summary>
    /// <value>The method called when an error is thrown during the serialization of the object.</value>
    [Obsolete("This property is obsolete and has been replaced by the OnErrorCallbacks collection.")]
    public MethodInfo OnError
    {
      get
      {
        return this.OnErrorCallbacks.Count <= 0 ? (MethodInfo) null : this.OnErrorCallbacks[0].Method();
      }
      set
      {
        this.OnErrorCallbacks.Clear();
        this.OnErrorCallbacks.Add(JsonContract.CreateSerializationErrorCallback(value));
      }
    }

    /// <summary>
    /// Gets or sets the default creator method used to create the object.
    /// </summary>
    /// <value>The default creator method used to create the object.</value>
    public Func<object> DefaultCreator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the default creator is non public.
    /// </summary>
    /// <value><c>true</c> if the default object creator is non-public; otherwise, <c>false</c>.</value>
    public bool DefaultCreatorNonPublic { get; set; }

    internal JsonContract(Type underlyingType)
    {
      ValidationUtils.ArgumentNotNull((object) underlyingType, nameof (underlyingType));
      this.UnderlyingType = underlyingType;
      this.IsNullable = ReflectionUtils.IsNullable(underlyingType);
      this.NonNullableUnderlyingType = !this.IsNullable || !ReflectionUtils.IsNullableType(underlyingType) ? underlyingType : Nullable.GetUnderlyingType(underlyingType);
      this.CreatedType = this.NonNullableUnderlyingType;
      this.IsConvertable = ConvertUtils.IsConvertible(this.NonNullableUnderlyingType);
      this.IsEnum = this.NonNullableUnderlyingType.IsEnum();
      if (this.NonNullableUnderlyingType == typeof (byte[]))
        this.InternalReadType = ReadType.ReadAsBytes;
      else if (this.NonNullableUnderlyingType == typeof (int))
        this.InternalReadType = ReadType.ReadAsInt32;
      else if (this.NonNullableUnderlyingType == typeof (Decimal))
        this.InternalReadType = ReadType.ReadAsDecimal;
      else if (this.NonNullableUnderlyingType == typeof (string))
        this.InternalReadType = ReadType.ReadAsString;
      else if (this.NonNullableUnderlyingType == typeof (DateTime))
        this.InternalReadType = ReadType.ReadAsDateTime;
      else if (this.NonNullableUnderlyingType == typeof (DateTimeOffset))
        this.InternalReadType = ReadType.ReadAsDateTimeOffset;
      else
        this.InternalReadType = ReadType.Read;
    }

    internal void InvokeOnSerializing(object o, StreamingContext context)
    {
      if (this._onSerializingCallbacks == null)
        return;
      foreach (SerializationCallback serializingCallback in (IEnumerable<SerializationCallback>) this._onSerializingCallbacks)
        serializingCallback(o, context);
    }

    internal void InvokeOnSerialized(object o, StreamingContext context)
    {
      if (this._onSerializedCallbacks == null)
        return;
      foreach (SerializationCallback serializedCallback in (IEnumerable<SerializationCallback>) this._onSerializedCallbacks)
        serializedCallback(o, context);
    }

    internal void InvokeOnDeserializing(object o, StreamingContext context)
    {
      if (this._onDeserializingCallbacks == null)
        return;
      foreach (SerializationCallback deserializingCallback in (IEnumerable<SerializationCallback>) this._onDeserializingCallbacks)
        deserializingCallback(o, context);
    }

    internal void InvokeOnDeserialized(object o, StreamingContext context)
    {
      if (this._onDeserializedCallbacks == null)
        return;
      foreach (SerializationCallback deserializedCallback in this._onDeserializedCallbacks)
        deserializedCallback(o, context);
    }

    internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
    {
      if (this._onErrorCallbacks == null)
        return;
      foreach (SerializationErrorCallback onErrorCallback in (IEnumerable<SerializationErrorCallback>) this._onErrorCallbacks)
        onErrorCallback(o, context, errorContext);
    }

    internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
    {
      return (SerializationCallback) ((o, context) => callbackMethodInfo.Invoke(o, new object[1]
      {
        (object) context
      }));
    }

    internal static SerializationErrorCallback CreateSerializationErrorCallback(
      MethodInfo callbackMethodInfo)
    {
      return (SerializationErrorCallback) ((o, context, econtext) => callbackMethodInfo.Invoke(o, new object[2]
      {
        (object) context,
        (object) econtext
      }));
    }
  }
}
