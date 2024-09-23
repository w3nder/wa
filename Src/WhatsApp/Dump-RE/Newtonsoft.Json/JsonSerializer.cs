﻿// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonSerializer
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Serializes and deserializes objects into and from the JSON format.
  /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> enables you to control how objects are encoded into JSON.
  /// </summary>
  public class JsonSerializer
  {
    internal TypeNameHandling _typeNameHandling;
    internal FormatterAssemblyStyle _typeNameAssemblyFormat;
    internal PreserveReferencesHandling _preserveReferencesHandling;
    internal ReferenceLoopHandling _referenceLoopHandling;
    internal MissingMemberHandling _missingMemberHandling;
    internal ObjectCreationHandling _objectCreationHandling;
    internal NullValueHandling _nullValueHandling;
    internal DefaultValueHandling _defaultValueHandling;
    internal ConstructorHandling _constructorHandling;
    internal MetadataPropertyHandling _metadataPropertyHandling;
    internal JsonConverterCollection _converters;
    internal IContractResolver _contractResolver;
    internal ITraceWriter _traceWriter;
    internal IEqualityComparer _equalityComparer;
    internal SerializationBinder _binder;
    internal StreamingContext _context;
    private IReferenceResolver _referenceResolver;
    private Formatting? _formatting;
    private DateFormatHandling? _dateFormatHandling;
    private DateTimeZoneHandling? _dateTimeZoneHandling;
    private DateParseHandling? _dateParseHandling;
    private FloatFormatHandling? _floatFormatHandling;
    private FloatParseHandling? _floatParseHandling;
    private StringEscapeHandling? _stringEscapeHandling;
    private CultureInfo _culture;
    private int? _maxDepth;
    private bool _maxDepthSet;
    private bool? _checkAdditionalContent;
    private string _dateFormatString;
    private bool _dateFormatStringSet;

    /// <summary>
    /// Occurs when the <see cref="T:Newtonsoft.Json.JsonSerializer" /> errors during serialization and deserialization.
    /// </summary>
    public virtual event EventHandler<ErrorEventArgs> Error;

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
    /// </summary>
    public virtual IReferenceResolver ReferenceResolver
    {
      get => this.GetReferenceResolver();
      set
      {
        this._referenceResolver = value != null ? value : throw new ArgumentNullException(nameof (value), "Reference resolver cannot be null.");
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.SerializationBinder" /> used by the serializer when resolving type names.
    /// </summary>
    public virtual SerializationBinder Binder
    {
      get => this._binder;
      set
      {
        this._binder = value != null ? value : throw new ArgumentNullException(nameof (value), "Serialization binder cannot be null.");
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.ITraceWriter" /> used by the serializer when writing trace messages.
    /// </summary>
    /// <value>The trace writer.</value>
    public virtual ITraceWriter TraceWriter
    {
      get => this._traceWriter;
      set => this._traceWriter = value;
    }

    /// <summary>
    /// Gets or sets the equality comparer used by the serializer when comparing references.
    /// </summary>
    /// <value>The equality comparer.</value>
    public virtual IEqualityComparer EqualityComparer
    {
      get => this._equalityComparer;
      set => this._equalityComparer = value;
    }

    /// <summary>
    /// Gets or sets how type name writing and reading is handled by the serializer.
    /// </summary>
    public virtual TypeNameHandling TypeNameHandling
    {
      get => this._typeNameHandling;
      set
      {
        this._typeNameHandling = value >= TypeNameHandling.None && value <= TypeNameHandling.Auto ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets or sets how a type name assembly is written and resolved by the serializer.
    /// </summary>
    /// <value>The type name assembly format.</value>
    public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
    {
      get => this._typeNameAssemblyFormat;
      set
      {
        this._typeNameAssemblyFormat = value >= FormatterAssemblyStyle.Simple && value <= FormatterAssemblyStyle.Full ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets or sets how object references are preserved by the serializer.
    /// </summary>
    public virtual PreserveReferencesHandling PreserveReferencesHandling
    {
      get => this._preserveReferencesHandling;
      set
      {
        this._preserveReferencesHandling = value >= PreserveReferencesHandling.None && value <= PreserveReferencesHandling.All ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Get or set how reference loops (e.g. a class referencing itself) is handled.
    /// </summary>
    public virtual ReferenceLoopHandling ReferenceLoopHandling
    {
      get => this._referenceLoopHandling;
      set
      {
        this._referenceLoopHandling = value >= ReferenceLoopHandling.Error && value <= ReferenceLoopHandling.Serialize ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Get or set how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
    /// </summary>
    public virtual MissingMemberHandling MissingMemberHandling
    {
      get => this._missingMemberHandling;
      set
      {
        this._missingMemberHandling = value >= MissingMemberHandling.Ignore && value <= MissingMemberHandling.Error ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Get or set how null values are handled during serialization and deserialization.
    /// </summary>
    public virtual NullValueHandling NullValueHandling
    {
      get => this._nullValueHandling;
      set
      {
        this._nullValueHandling = value >= NullValueHandling.Include && value <= NullValueHandling.Ignore ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Get or set how null default are handled during serialization and deserialization.
    /// </summary>
    public virtual DefaultValueHandling DefaultValueHandling
    {
      get => this._defaultValueHandling;
      set
      {
        this._defaultValueHandling = value >= DefaultValueHandling.Include && value <= DefaultValueHandling.IgnoreAndPopulate ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets or sets how objects are created during deserialization.
    /// </summary>
    /// <value>The object creation handling.</value>
    public virtual ObjectCreationHandling ObjectCreationHandling
    {
      get => this._objectCreationHandling;
      set
      {
        this._objectCreationHandling = value >= ObjectCreationHandling.Auto && value <= ObjectCreationHandling.Replace ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets or sets how constructors are used during deserialization.
    /// </summary>
    /// <value>The constructor handling.</value>
    public virtual ConstructorHandling ConstructorHandling
    {
      get => this._constructorHandling;
      set
      {
        this._constructorHandling = value >= ConstructorHandling.Default && value <= ConstructorHandling.AllowNonPublicDefaultConstructor ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets or sets how metadata properties are used during deserialization.
    /// </summary>
    /// <value>The metadata properties handling.</value>
    public virtual MetadataPropertyHandling MetadataPropertyHandling
    {
      get => this._metadataPropertyHandling;
      set
      {
        this._metadataPropertyHandling = value >= MetadataPropertyHandling.Default && value <= MetadataPropertyHandling.Ignore ? value : throw new ArgumentOutOfRangeException(nameof (value));
      }
    }

    /// <summary>
    /// Gets a collection <see cref="T:Newtonsoft.Json.JsonConverter" /> that will be used during serialization.
    /// </summary>
    /// <value>Collection <see cref="T:Newtonsoft.Json.JsonConverter" /> that will be used during serialization.</value>
    public virtual JsonConverterCollection Converters
    {
      get
      {
        if (this._converters == null)
          this._converters = new JsonConverterCollection();
        return this._converters;
      }
    }

    /// <summary>
    /// Gets or sets the contract resolver used by the serializer when
    /// serializing .NET objects to JSON and vice versa.
    /// </summary>
    public virtual IContractResolver ContractResolver
    {
      get => this._contractResolver;
      set => this._contractResolver = value ?? DefaultContractResolver.Instance;
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext" /> used by the serializer when invoking serialization callback methods.
    /// </summary>
    /// <value>The context.</value>
    public virtual StreamingContext Context
    {
      get => this._context;
      set => this._context = value;
    }

    /// <summary>Indicates how JSON text output is formatted.</summary>
    public virtual Formatting Formatting
    {
      get => this._formatting ?? Formatting.None;
      set => this._formatting = new Formatting?(value);
    }

    /// <summary>Get or set how dates are written to JSON text.</summary>
    public virtual DateFormatHandling DateFormatHandling
    {
      get => this._dateFormatHandling ?? DateFormatHandling.IsoDateFormat;
      set => this._dateFormatHandling = new DateFormatHandling?(value);
    }

    /// <summary>
    /// Get or set how <see cref="T:System.DateTime" /> time zones are handling during serialization and deserialization.
    /// </summary>
    public virtual DateTimeZoneHandling DateTimeZoneHandling
    {
      get => this._dateTimeZoneHandling ?? DateTimeZoneHandling.RoundtripKind;
      set => this._dateTimeZoneHandling = new DateTimeZoneHandling?(value);
    }

    /// <summary>
    /// Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
    /// </summary>
    public virtual DateParseHandling DateParseHandling
    {
      get => this._dateParseHandling ?? DateParseHandling.DateTime;
      set => this._dateParseHandling = new DateParseHandling?(value);
    }

    /// <summary>
    /// Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
    /// </summary>
    public virtual FloatParseHandling FloatParseHandling
    {
      get => this._floatParseHandling ?? FloatParseHandling.Double;
      set => this._floatParseHandling = new FloatParseHandling?(value);
    }

    /// <summary>
    /// Get or set how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
    /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
    /// are written as JSON text.
    /// </summary>
    public virtual FloatFormatHandling FloatFormatHandling
    {
      get => this._floatFormatHandling ?? FloatFormatHandling.String;
      set => this._floatFormatHandling = new FloatFormatHandling?(value);
    }

    /// <summary>
    /// Get or set how strings are escaped when writing JSON text.
    /// </summary>
    public virtual StringEscapeHandling StringEscapeHandling
    {
      get => this._stringEscapeHandling ?? StringEscapeHandling.Default;
      set => this._stringEscapeHandling = new StringEscapeHandling?(value);
    }

    /// <summary>
    /// Get or set how <see cref="T:System.DateTime" /> and <see cref="T:System.DateTimeOffset" /> values are formatted when writing JSON text, and the expected date format when reading JSON text.
    /// </summary>
    public virtual string DateFormatString
    {
      get => this._dateFormatString ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
      set
      {
        this._dateFormatString = value;
        this._dateFormatStringSet = true;
      }
    }

    /// <summary>
    /// Gets or sets the culture used when reading JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
    /// </summary>
    public virtual CultureInfo Culture
    {
      get => this._culture ?? JsonSerializerSettings.DefaultCulture;
      set => this._culture = value;
    }

    /// <summary>
    /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
    /// </summary>
    public virtual int? MaxDepth
    {
      get => this._maxDepth;
      set
      {
        int? nullable = value;
        if ((nullable.GetValueOrDefault() > 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
          throw new ArgumentException("Value must be positive.", nameof (value));
        this._maxDepth = value;
        this._maxDepthSet = true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether there will be a check for additional JSON content after deserializing an object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if there will be a check for additional JSON content after deserializing an object; otherwise, <c>false</c>.
    /// </value>
    public virtual bool CheckAdditionalContent
    {
      get => this._checkAdditionalContent ?? false;
      set => this._checkAdditionalContent = new bool?(value);
    }

    internal bool IsCheckAdditionalContentSet() => this._checkAdditionalContent.HasValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializer" /> class.
    /// </summary>
    public JsonSerializer()
    {
      this._referenceLoopHandling = ReferenceLoopHandling.Error;
      this._missingMemberHandling = MissingMemberHandling.Ignore;
      this._nullValueHandling = NullValueHandling.Include;
      this._defaultValueHandling = DefaultValueHandling.Include;
      this._objectCreationHandling = ObjectCreationHandling.Auto;
      this._preserveReferencesHandling = PreserveReferencesHandling.None;
      this._constructorHandling = ConstructorHandling.Default;
      this._typeNameHandling = TypeNameHandling.None;
      this._metadataPropertyHandling = MetadataPropertyHandling.Default;
      this._context = JsonSerializerSettings.DefaultContext;
      this._binder = (SerializationBinder) DefaultSerializationBinder.Instance;
      this._culture = JsonSerializerSettings.DefaultCulture;
      this._contractResolver = DefaultContractResolver.Instance;
    }

    /// <summary>
    /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings.
    /// </returns>
    public static JsonSerializer Create() => new JsonSerializer();

    /// <summary>
    /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings.
    /// </summary>
    /// <param name="settings">The settings to be applied to the <see cref="T:Newtonsoft.Json.JsonSerializer" />.</param>
    /// <returns>
    /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings.
    /// </returns>
    public static JsonSerializer Create(JsonSerializerSettings settings)
    {
      JsonSerializer serializer = JsonSerializer.Create();
      if (settings != null)
        JsonSerializer.ApplySerializerSettings(serializer, settings);
      return serializer;
    }

    /// <summary>
    /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings.
    /// </returns>
    public static JsonSerializer CreateDefault()
    {
      Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
      return JsonSerializer.Create(defaultSettings != null ? defaultSettings() : (JsonSerializerSettings) null);
    }

    /// <summary>
    /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings.
    /// </summary>
    /// <param name="settings">The settings to be applied to the <see cref="T:Newtonsoft.Json.JsonSerializer" />.</param>
    /// <returns>
    /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings.
    /// </returns>
    public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
    {
      JsonSerializer serializer = JsonSerializer.CreateDefault();
      if (settings != null)
        JsonSerializer.ApplySerializerSettings(serializer, settings);
      return serializer;
    }

    private static void ApplySerializerSettings(
      JsonSerializer serializer,
      JsonSerializerSettings settings)
    {
      if (!CollectionUtils.IsNullOrEmpty<JsonConverter>((ICollection<JsonConverter>) settings.Converters))
      {
        for (int index = 0; index < settings.Converters.Count; ++index)
          serializer.Converters.Insert(index, settings.Converters[index]);
      }
      if (settings._typeNameHandling.HasValue)
        serializer.TypeNameHandling = settings.TypeNameHandling;
      if (settings._metadataPropertyHandling.HasValue)
        serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
      if (settings._typeNameAssemblyFormat.HasValue)
        serializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
      if (settings._preserveReferencesHandling.HasValue)
        serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
      if (settings._referenceLoopHandling.HasValue)
        serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
      if (settings._missingMemberHandling.HasValue)
        serializer.MissingMemberHandling = settings.MissingMemberHandling;
      if (settings._objectCreationHandling.HasValue)
        serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
      if (settings._nullValueHandling.HasValue)
        serializer.NullValueHandling = settings.NullValueHandling;
      if (settings._defaultValueHandling.HasValue)
        serializer.DefaultValueHandling = settings.DefaultValueHandling;
      if (settings._constructorHandling.HasValue)
        serializer.ConstructorHandling = settings.ConstructorHandling;
      if (settings._context.HasValue)
        serializer.Context = settings.Context;
      if (settings._checkAdditionalContent.HasValue)
        serializer._checkAdditionalContent = settings._checkAdditionalContent;
      if (settings.Error != null)
        serializer.Error += settings.Error;
      if (settings.ContractResolver != null)
        serializer.ContractResolver = settings.ContractResolver;
      if (settings.ReferenceResolverProvider != null)
        serializer.ReferenceResolver = settings.ReferenceResolverProvider();
      if (settings.TraceWriter != null)
        serializer.TraceWriter = settings.TraceWriter;
      if (settings.EqualityComparer != null)
        serializer.EqualityComparer = settings.EqualityComparer;
      if (settings.Binder != null)
        serializer.Binder = settings.Binder;
      if (settings._formatting.HasValue)
        serializer._formatting = settings._formatting;
      if (settings._dateFormatHandling.HasValue)
        serializer._dateFormatHandling = settings._dateFormatHandling;
      if (settings._dateTimeZoneHandling.HasValue)
        serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
      if (settings._dateParseHandling.HasValue)
        serializer._dateParseHandling = settings._dateParseHandling;
      if (settings._dateFormatStringSet)
      {
        serializer._dateFormatString = settings._dateFormatString;
        serializer._dateFormatStringSet = settings._dateFormatStringSet;
      }
      if (settings._floatFormatHandling.HasValue)
        serializer._floatFormatHandling = settings._floatFormatHandling;
      if (settings._floatParseHandling.HasValue)
        serializer._floatParseHandling = settings._floatParseHandling;
      if (settings._stringEscapeHandling.HasValue)
        serializer._stringEscapeHandling = settings._stringEscapeHandling;
      if (settings._culture != null)
        serializer._culture = settings._culture;
      if (!settings._maxDepthSet)
        return;
      serializer._maxDepth = settings._maxDepth;
      serializer._maxDepthSet = settings._maxDepthSet;
    }

    /// <summary>Populates the JSON values onto the target object.</summary>
    /// <param name="reader">The <see cref="T:System.IO.TextReader" /> that contains the JSON structure to reader values from.</param>
    /// <param name="target">The target object to populate values onto.</param>
    public void Populate(TextReader reader, object target)
    {
      this.Populate((JsonReader) new JsonTextReader(reader), target);
    }

    /// <summary>Populates the JSON values onto the target object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> that contains the JSON structure to reader values from.</param>
    /// <param name="target">The target object to populate values onto.</param>
    public void Populate(JsonReader reader, object target) => this.PopulateInternal(reader, target);

    internal virtual void PopulateInternal(JsonReader reader, object target)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      ValidationUtils.ArgumentNotNull(target, nameof (target));
      CultureInfo previousCulture;
      DateTimeZoneHandling? previousDateTimeZoneHandling;
      DateParseHandling? previousDateParseHandling;
      FloatParseHandling? previousFloatParseHandling;
      int? previousMaxDepth;
      string previousDateFormatString;
      this.SetupReader(reader, out previousCulture, out previousDateTimeZoneHandling, out previousDateParseHandling, out previousFloatParseHandling, out previousMaxDepth, out previousDateFormatString);
      TraceJsonReader traceJsonReader = this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose ? (TraceJsonReader) null : new TraceJsonReader(reader);
      new JsonSerializerInternalReader(this).Populate((JsonReader) traceJsonReader ?? reader, target);
      if (traceJsonReader != null)
        this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), (Exception) null);
      this.ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);
    }

    /// <summary>
    /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> that contains the JSON structure to deserialize.</param>
    /// <returns>The <see cref="T:System.Object" /> being deserialized.</returns>
    public object Deserialize(JsonReader reader) => this.Deserialize(reader, (Type) null);

    /// <summary>
    /// Deserializes the JSON structure contained by the specified <see cref="T:System.IO.StringReader" />
    /// into an instance of the specified type.
    /// </summary>
    /// <param name="reader">The <see cref="T:System.IO.TextReader" /> containing the object.</param>
    /// <param name="objectType">The <see cref="T:System.Type" /> of object being deserialized.</param>
    /// <returns>The instance of <paramref name="objectType" /> being deserialized.</returns>
    public object Deserialize(TextReader reader, Type objectType)
    {
      return this.Deserialize((JsonReader) new JsonTextReader(reader), objectType);
    }

    /// <summary>
    /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />
    /// into an instance of the specified type.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the object.</param>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <returns>The instance of <typeparamref name="T" /> being deserialized.</returns>
    public T Deserialize<T>(JsonReader reader) => (T) this.Deserialize(reader, typeof (T));

    /// <summary>
    /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />
    /// into an instance of the specified type.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the object.</param>
    /// <param name="objectType">The <see cref="T:System.Type" /> of object being deserialized.</param>
    /// <returns>The instance of <paramref name="objectType" /> being deserialized.</returns>
    public object Deserialize(JsonReader reader, Type objectType)
    {
      return this.DeserializeInternal(reader, objectType);
    }

    internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
    {
      ValidationUtils.ArgumentNotNull((object) reader, nameof (reader));
      CultureInfo previousCulture;
      DateTimeZoneHandling? previousDateTimeZoneHandling;
      DateParseHandling? previousDateParseHandling;
      FloatParseHandling? previousFloatParseHandling;
      int? previousMaxDepth;
      string previousDateFormatString;
      this.SetupReader(reader, out previousCulture, out previousDateTimeZoneHandling, out previousDateParseHandling, out previousFloatParseHandling, out previousMaxDepth, out previousDateFormatString);
      TraceJsonReader traceJsonReader = this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose ? (TraceJsonReader) null : new TraceJsonReader(reader);
      object obj = new JsonSerializerInternalReader(this).Deserialize((JsonReader) traceJsonReader ?? reader, objectType, this.CheckAdditionalContent);
      if (traceJsonReader != null)
        this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), (Exception) null);
      this.ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);
      return obj;
    }

    private void SetupReader(
      JsonReader reader,
      out CultureInfo previousCulture,
      out DateTimeZoneHandling? previousDateTimeZoneHandling,
      out DateParseHandling? previousDateParseHandling,
      out FloatParseHandling? previousFloatParseHandling,
      out int? previousMaxDepth,
      out string previousDateFormatString)
    {
      if (this._culture != null && !this._culture.Equals((object) reader.Culture))
      {
        previousCulture = reader.Culture;
        reader.Culture = this._culture;
      }
      else
        previousCulture = (CultureInfo) null;
      if (this._dateTimeZoneHandling.HasValue)
      {
        DateTimeZoneHandling timeZoneHandling1 = reader.DateTimeZoneHandling;
        DateTimeZoneHandling? timeZoneHandling2 = this._dateTimeZoneHandling;
        if ((timeZoneHandling1 != timeZoneHandling2.GetValueOrDefault() ? 1 : (!timeZoneHandling2.HasValue ? 1 : 0)) != 0)
        {
          previousDateTimeZoneHandling = new DateTimeZoneHandling?(reader.DateTimeZoneHandling);
          reader.DateTimeZoneHandling = this._dateTimeZoneHandling.Value;
          goto label_7;
        }
      }
      previousDateTimeZoneHandling = new DateTimeZoneHandling?();
label_7:
      if (this._dateParseHandling.HasValue)
      {
        DateParseHandling dateParseHandling1 = reader.DateParseHandling;
        DateParseHandling? dateParseHandling2 = this._dateParseHandling;
        if ((dateParseHandling1 != dateParseHandling2.GetValueOrDefault() ? 1 : (!dateParseHandling2.HasValue ? 1 : 0)) != 0)
        {
          previousDateParseHandling = new DateParseHandling?(reader.DateParseHandling);
          reader.DateParseHandling = this._dateParseHandling.Value;
          goto label_11;
        }
      }
      previousDateParseHandling = new DateParseHandling?();
label_11:
      if (this._floatParseHandling.HasValue)
      {
        FloatParseHandling floatParseHandling1 = reader.FloatParseHandling;
        FloatParseHandling? floatParseHandling2 = this._floatParseHandling;
        if ((floatParseHandling1 != floatParseHandling2.GetValueOrDefault() ? 1 : (!floatParseHandling2.HasValue ? 1 : 0)) != 0)
        {
          previousFloatParseHandling = new FloatParseHandling?(reader.FloatParseHandling);
          reader.FloatParseHandling = this._floatParseHandling.Value;
          goto label_15;
        }
      }
      previousFloatParseHandling = new FloatParseHandling?();
label_15:
      if (this._maxDepthSet)
      {
        int? maxDepth1 = reader.MaxDepth;
        int? maxDepth2 = this._maxDepth;
        if ((maxDepth1.GetValueOrDefault() != maxDepth2.GetValueOrDefault() ? 1 : (maxDepth1.HasValue != maxDepth2.HasValue ? 1 : 0)) != 0)
        {
          previousMaxDepth = reader.MaxDepth;
          reader.MaxDepth = this._maxDepth;
          goto label_19;
        }
      }
      previousMaxDepth = new int?();
label_19:
      if (this._dateFormatStringSet && reader.DateFormatString != this._dateFormatString)
      {
        previousDateFormatString = reader.DateFormatString;
        reader.DateFormatString = this._dateFormatString;
      }
      else
        previousDateFormatString = (string) null;
      if (!(reader is JsonTextReader jsonTextReader) || !(this._contractResolver is DefaultContractResolver contractResolver))
        return;
      jsonTextReader.NameTable = contractResolver.GetState().NameTable;
    }

    private void ResetReader(
      JsonReader reader,
      CultureInfo previousCulture,
      DateTimeZoneHandling? previousDateTimeZoneHandling,
      DateParseHandling? previousDateParseHandling,
      FloatParseHandling? previousFloatParseHandling,
      int? previousMaxDepth,
      string previousDateFormatString)
    {
      if (previousCulture != null)
        reader.Culture = previousCulture;
      if (previousDateTimeZoneHandling.HasValue)
        reader.DateTimeZoneHandling = previousDateTimeZoneHandling.Value;
      if (previousDateParseHandling.HasValue)
        reader.DateParseHandling = previousDateParseHandling.Value;
      if (previousFloatParseHandling.HasValue)
        reader.FloatParseHandling = previousFloatParseHandling.Value;
      if (this._maxDepthSet)
        reader.MaxDepth = previousMaxDepth;
      if (this._dateFormatStringSet)
        reader.DateFormatString = previousDateFormatString;
      if (!(reader is JsonTextReader jsonTextReader))
        return;
      jsonTextReader.NameTable = (PropertyNameTable) null;
    }

    /// <summary>
    /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
    /// to a <c>Stream</c> using the specified <see cref="T:System.IO.TextWriter" />.
    /// </summary>
    /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> used to write the JSON structure.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
    public void Serialize(TextWriter textWriter, object value)
    {
      this.Serialize((JsonWriter) new JsonTextWriter(textWriter), value);
    }

    /// <summary>
    /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
    /// to a <c>Stream</c> using the specified <see cref="T:System.IO.TextWriter" />.
    /// </summary>
    /// <param name="jsonWriter">The <see cref="T:Newtonsoft.Json.JsonWriter" /> used to write the JSON structure.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
    /// <param name="objectType">
    /// The type of the value being serialized.
    /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is Auto to write out the type name if the type of the value does not match.
    /// Specifing the type is optional.
    /// </param>
    public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
    {
      this.SerializeInternal(jsonWriter, value, objectType);
    }

    /// <summary>
    /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
    /// to a <c>Stream</c> using the specified <see cref="T:System.IO.TextWriter" />.
    /// </summary>
    /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> used to write the JSON structure.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
    /// <param name="objectType">
    /// The type of the value being serialized.
    /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is Auto to write out the type name if the type of the value does not match.
    /// Specifing the type is optional.
    /// </param>
    public void Serialize(TextWriter textWriter, object value, Type objectType)
    {
      this.Serialize((JsonWriter) new JsonTextWriter(textWriter), value, objectType);
    }

    /// <summary>
    /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
    /// to a <c>Stream</c> using the specified <see cref="T:Newtonsoft.Json.JsonWriter" />.
    /// </summary>
    /// <param name="jsonWriter">The <see cref="T:Newtonsoft.Json.JsonWriter" /> used to write the JSON structure.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
    public void Serialize(JsonWriter jsonWriter, object value)
    {
      this.SerializeInternal(jsonWriter, value, (Type) null);
    }

    internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
    {
      ValidationUtils.ArgumentNotNull((object) jsonWriter, nameof (jsonWriter));
      Formatting? nullable1 = new Formatting?();
      if (this._formatting.HasValue)
      {
        Formatting formatting1 = jsonWriter.Formatting;
        Formatting? formatting2 = this._formatting;
        if ((formatting1 != formatting2.GetValueOrDefault() ? 1 : (!formatting2.HasValue ? 1 : 0)) != 0)
        {
          nullable1 = new Formatting?(jsonWriter.Formatting);
          jsonWriter.Formatting = this._formatting.Value;
        }
      }
      DateFormatHandling? nullable2 = new DateFormatHandling?();
      if (this._dateFormatHandling.HasValue)
      {
        DateFormatHandling dateFormatHandling1 = jsonWriter.DateFormatHandling;
        DateFormatHandling? dateFormatHandling2 = this._dateFormatHandling;
        if ((dateFormatHandling1 != dateFormatHandling2.GetValueOrDefault() ? 1 : (!dateFormatHandling2.HasValue ? 1 : 0)) != 0)
        {
          nullable2 = new DateFormatHandling?(jsonWriter.DateFormatHandling);
          jsonWriter.DateFormatHandling = this._dateFormatHandling.Value;
        }
      }
      DateTimeZoneHandling? nullable3 = new DateTimeZoneHandling?();
      if (this._dateTimeZoneHandling.HasValue)
      {
        DateTimeZoneHandling timeZoneHandling1 = jsonWriter.DateTimeZoneHandling;
        DateTimeZoneHandling? timeZoneHandling2 = this._dateTimeZoneHandling;
        if ((timeZoneHandling1 != timeZoneHandling2.GetValueOrDefault() ? 1 : (!timeZoneHandling2.HasValue ? 1 : 0)) != 0)
        {
          nullable3 = new DateTimeZoneHandling?(jsonWriter.DateTimeZoneHandling);
          jsonWriter.DateTimeZoneHandling = this._dateTimeZoneHandling.Value;
        }
      }
      FloatFormatHandling? nullable4 = new FloatFormatHandling?();
      if (this._floatFormatHandling.HasValue)
      {
        FloatFormatHandling floatFormatHandling1 = jsonWriter.FloatFormatHandling;
        FloatFormatHandling? floatFormatHandling2 = this._floatFormatHandling;
        if ((floatFormatHandling1 != floatFormatHandling2.GetValueOrDefault() ? 1 : (!floatFormatHandling2.HasValue ? 1 : 0)) != 0)
        {
          nullable4 = new FloatFormatHandling?(jsonWriter.FloatFormatHandling);
          jsonWriter.FloatFormatHandling = this._floatFormatHandling.Value;
        }
      }
      StringEscapeHandling? nullable5 = new StringEscapeHandling?();
      if (this._stringEscapeHandling.HasValue)
      {
        StringEscapeHandling stringEscapeHandling1 = jsonWriter.StringEscapeHandling;
        StringEscapeHandling? stringEscapeHandling2 = this._stringEscapeHandling;
        if ((stringEscapeHandling1 != stringEscapeHandling2.GetValueOrDefault() ? 1 : (!stringEscapeHandling2.HasValue ? 1 : 0)) != 0)
        {
          nullable5 = new StringEscapeHandling?(jsonWriter.StringEscapeHandling);
          jsonWriter.StringEscapeHandling = this._stringEscapeHandling.Value;
        }
      }
      CultureInfo cultureInfo = (CultureInfo) null;
      if (this._culture != null && !this._culture.Equals((object) jsonWriter.Culture))
      {
        cultureInfo = jsonWriter.Culture;
        jsonWriter.Culture = this._culture;
      }
      string str = (string) null;
      if (this._dateFormatStringSet && jsonWriter.DateFormatString != this._dateFormatString)
      {
        str = jsonWriter.DateFormatString;
        jsonWriter.DateFormatString = this._dateFormatString;
      }
      TraceJsonWriter traceJsonWriter = this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose ? (TraceJsonWriter) null : new TraceJsonWriter(jsonWriter);
      new JsonSerializerInternalWriter(this).Serialize((JsonWriter) traceJsonWriter ?? jsonWriter, value, objectType);
      if (traceJsonWriter != null)
        this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonWriter.GetSerializedJsonMessage(), (Exception) null);
      if (nullable1.HasValue)
        jsonWriter.Formatting = nullable1.Value;
      if (nullable2.HasValue)
        jsonWriter.DateFormatHandling = nullable2.Value;
      if (nullable3.HasValue)
        jsonWriter.DateTimeZoneHandling = nullable3.Value;
      if (nullable4.HasValue)
        jsonWriter.FloatFormatHandling = nullable4.Value;
      if (nullable5.HasValue)
        jsonWriter.StringEscapeHandling = nullable5.Value;
      if (this._dateFormatStringSet)
        jsonWriter.DateFormatString = str;
      if (cultureInfo == null)
        return;
      jsonWriter.Culture = cultureInfo;
    }

    internal IReferenceResolver GetReferenceResolver()
    {
      if (this._referenceResolver == null)
        this._referenceResolver = (IReferenceResolver) new DefaultReferenceResolver();
      return this._referenceResolver;
    }

    internal JsonConverter GetMatchingConverter(Type type)
    {
      return JsonSerializer.GetMatchingConverter((IList<JsonConverter>) this._converters, type);
    }

    internal static JsonConverter GetMatchingConverter(
      IList<JsonConverter> converters,
      Type objectType)
    {
      if (converters != null)
      {
        for (int index = 0; index < converters.Count; ++index)
        {
          JsonConverter converter = converters[index];
          if (converter.CanConvert(objectType))
            return converter;
        }
      }
      return (JsonConverter) null;
    }

    internal void OnError(ErrorEventArgs e)
    {
      EventHandler<ErrorEventArgs> error = this.Error;
      if (error == null)
        return;
      error((object) this, e);
    }
  }
}
