// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonSerializerSettings
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies the settings on a <see cref="T:Newtonsoft.Json.JsonSerializer" /> object.
  /// </summary>
  public class JsonSerializerSettings
  {
    internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;
    internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;
    internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;
    internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;
    internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;
    internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;
    internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;
    internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;
    internal const MetadataPropertyHandling DefaultMetadataPropertyHandling = MetadataPropertyHandling.Default;
    internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
    internal const Formatting DefaultFormatting = Formatting.None;
    internal const DateFormatHandling DefaultDateFormatHandling = DateFormatHandling.IsoDateFormat;
    internal const DateTimeZoneHandling DefaultDateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
    internal const DateParseHandling DefaultDateParseHandling = DateParseHandling.DateTime;
    internal const FloatParseHandling DefaultFloatParseHandling = FloatParseHandling.Double;
    internal const FloatFormatHandling DefaultFloatFormatHandling = FloatFormatHandling.String;
    internal const StringEscapeHandling DefaultStringEscapeHandling = StringEscapeHandling.Default;
    internal const FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;
    internal const bool DefaultCheckAdditionalContent = false;
    internal const string DefaultDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
    internal static readonly StreamingContext DefaultContext = new StreamingContext();
    internal static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
    internal Formatting? _formatting;
    internal DateFormatHandling? _dateFormatHandling;
    internal DateTimeZoneHandling? _dateTimeZoneHandling;
    internal DateParseHandling? _dateParseHandling;
    internal FloatFormatHandling? _floatFormatHandling;
    internal FloatParseHandling? _floatParseHandling;
    internal StringEscapeHandling? _stringEscapeHandling;
    internal CultureInfo _culture;
    internal bool? _checkAdditionalContent;
    internal int? _maxDepth;
    internal bool _maxDepthSet;
    internal string _dateFormatString;
    internal bool _dateFormatStringSet;
    internal FormatterAssemblyStyle? _typeNameAssemblyFormat;
    internal DefaultValueHandling? _defaultValueHandling;
    internal PreserveReferencesHandling? _preserveReferencesHandling;
    internal NullValueHandling? _nullValueHandling;
    internal ObjectCreationHandling? _objectCreationHandling;
    internal MissingMemberHandling? _missingMemberHandling;
    internal ReferenceLoopHandling? _referenceLoopHandling;
    internal StreamingContext? _context;
    internal ConstructorHandling? _constructorHandling;
    internal TypeNameHandling? _typeNameHandling;
    internal MetadataPropertyHandling? _metadataPropertyHandling;

    /// <summary>
    /// Gets or sets how reference loops (e.g. a class referencing itself) is handled.
    /// </summary>
    /// <value>Reference loop handling.</value>
    public ReferenceLoopHandling ReferenceLoopHandling
    {
      get => this._referenceLoopHandling ?? ReferenceLoopHandling.Error;
      set => this._referenceLoopHandling = new ReferenceLoopHandling?(value);
    }

    /// <summary>
    /// Gets or sets how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
    /// </summary>
    /// <value>Missing member handling.</value>
    public MissingMemberHandling MissingMemberHandling
    {
      get => this._missingMemberHandling ?? MissingMemberHandling.Ignore;
      set => this._missingMemberHandling = new MissingMemberHandling?(value);
    }

    /// <summary>
    /// Gets or sets how objects are created during deserialization.
    /// </summary>
    /// <value>The object creation handling.</value>
    public ObjectCreationHandling ObjectCreationHandling
    {
      get => this._objectCreationHandling ?? ObjectCreationHandling.Auto;
      set => this._objectCreationHandling = new ObjectCreationHandling?(value);
    }

    /// <summary>
    /// Gets or sets how null values are handled during serialization and deserialization.
    /// </summary>
    /// <value>Null value handling.</value>
    public NullValueHandling NullValueHandling
    {
      get => this._nullValueHandling ?? NullValueHandling.Include;
      set => this._nullValueHandling = new NullValueHandling?(value);
    }

    /// <summary>
    /// Gets or sets how null default are handled during serialization and deserialization.
    /// </summary>
    /// <value>The default value handling.</value>
    public DefaultValueHandling DefaultValueHandling
    {
      get => this._defaultValueHandling ?? DefaultValueHandling.Include;
      set => this._defaultValueHandling = new DefaultValueHandling?(value);
    }

    /// <summary>
    /// Gets or sets a collection <see cref="T:Newtonsoft.Json.JsonConverter" /> that will be used during serialization.
    /// </summary>
    /// <value>The converters.</value>
    public IList<JsonConverter> Converters { get; set; }

    /// <summary>
    /// Gets or sets how object references are preserved by the serializer.
    /// </summary>
    /// <value>The preserve references handling.</value>
    public PreserveReferencesHandling PreserveReferencesHandling
    {
      get => this._preserveReferencesHandling ?? PreserveReferencesHandling.None;
      set => this._preserveReferencesHandling = new PreserveReferencesHandling?(value);
    }

    /// <summary>
    /// Gets or sets how type name writing and reading is handled by the serializer.
    /// </summary>
    /// <value>The type name handling.</value>
    public TypeNameHandling TypeNameHandling
    {
      get => this._typeNameHandling ?? TypeNameHandling.None;
      set => this._typeNameHandling = new TypeNameHandling?(value);
    }

    /// <summary>
    /// Gets or sets how metadata properties are used during deserialization.
    /// </summary>
    /// <value>The metadata properties handling.</value>
    public MetadataPropertyHandling MetadataPropertyHandling
    {
      get => this._metadataPropertyHandling ?? MetadataPropertyHandling.Default;
      set => this._metadataPropertyHandling = new MetadataPropertyHandling?(value);
    }

    /// <summary>
    /// Gets or sets how a type name assembly is written and resolved by the serializer.
    /// </summary>
    /// <value>The type name assembly format.</value>
    public FormatterAssemblyStyle TypeNameAssemblyFormat
    {
      get => this._typeNameAssemblyFormat ?? FormatterAssemblyStyle.Simple;
      set => this._typeNameAssemblyFormat = new FormatterAssemblyStyle?(value);
    }

    /// <summary>
    /// Gets or sets how constructors are used during deserialization.
    /// </summary>
    /// <value>The constructor handling.</value>
    public ConstructorHandling ConstructorHandling
    {
      get => this._constructorHandling ?? ConstructorHandling.Default;
      set => this._constructorHandling = new ConstructorHandling?(value);
    }

    /// <summary>
    /// Gets or sets the contract resolver used by the serializer when
    /// serializing .NET objects to JSON and vice versa.
    /// </summary>
    /// <value>The contract resolver.</value>
    public IContractResolver ContractResolver { get; set; }

    /// <summary>
    /// Gets or sets the equality comparer used by the serializer when comparing references.
    /// </summary>
    /// <value>The equality comparer.</value>
    public IEqualityComparer EqualityComparer { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
    /// </summary>
    /// <value>The reference resolver.</value>
    [Obsolete("ReferenceResolver property is obsolete. Use the ReferenceResolverProvider property to set the IReferenceResolver: settings.ReferenceResolverProvider = () => resolver")]
    public IReferenceResolver ReferenceResolver
    {
      get
      {
        return this.ReferenceResolverProvider == null ? (IReferenceResolver) null : this.ReferenceResolverProvider();
      }
      set => this.ReferenceResolverProvider = (Func<IReferenceResolver>) (() => value);
    }

    /// <summary>
    /// Gets or sets a function that creates the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
    /// </summary>
    /// <value>A function that creates the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.</value>
    public Func<IReferenceResolver> ReferenceResolverProvider { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.ITraceWriter" /> used by the serializer when writing trace messages.
    /// </summary>
    /// <value>The trace writer.</value>
    public ITraceWriter TraceWriter { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:Newtonsoft.Json.SerializationBinder" /> used by the serializer when resolving type names.
    /// </summary>
    /// <value>The binder.</value>
    public SerializationBinder Binder { get; set; }

    /// <summary>
    /// Gets or sets the error handler called during serialization and deserialization.
    /// </summary>
    /// <value>The error handler called during serialization and deserialization.</value>
    public EventHandler<ErrorEventArgs> Error { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext" /> used by the serializer when invoking serialization callback methods.
    /// </summary>
    /// <value>The context.</value>
    public StreamingContext Context
    {
      get => this._context ?? JsonSerializerSettings.DefaultContext;
      set => this._context = new StreamingContext?(value);
    }

    /// <summary>
    /// Get or set how <see cref="T:System.DateTime" /> and <see cref="T:System.DateTimeOffset" /> values are formatted when writing JSON text, and the expected date format when reading JSON text.
    /// </summary>
    public string DateFormatString
    {
      get => this._dateFormatString ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
      set
      {
        this._dateFormatString = value;
        this._dateFormatStringSet = true;
      }
    }

    /// <summary>
    /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
    /// </summary>
    public int? MaxDepth
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

    /// <summary>Indicates how JSON text output is formatted.</summary>
    public Formatting Formatting
    {
      get => this._formatting ?? Formatting.None;
      set => this._formatting = new Formatting?(value);
    }

    /// <summary>Get or set how dates are written to JSON text.</summary>
    public DateFormatHandling DateFormatHandling
    {
      get => this._dateFormatHandling ?? DateFormatHandling.IsoDateFormat;
      set => this._dateFormatHandling = new DateFormatHandling?(value);
    }

    /// <summary>
    /// Get or set how <see cref="T:System.DateTime" /> time zones are handling during serialization and deserialization.
    /// </summary>
    public DateTimeZoneHandling DateTimeZoneHandling
    {
      get => this._dateTimeZoneHandling ?? DateTimeZoneHandling.RoundtripKind;
      set => this._dateTimeZoneHandling = new DateTimeZoneHandling?(value);
    }

    /// <summary>
    /// Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
    /// </summary>
    public DateParseHandling DateParseHandling
    {
      get => this._dateParseHandling ?? DateParseHandling.DateTime;
      set => this._dateParseHandling = new DateParseHandling?(value);
    }

    /// <summary>
    /// Get or set how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
    /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
    /// are written as JSON.
    /// </summary>
    public FloatFormatHandling FloatFormatHandling
    {
      get => this._floatFormatHandling ?? FloatFormatHandling.String;
      set => this._floatFormatHandling = new FloatFormatHandling?(value);
    }

    /// <summary>
    /// Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
    /// </summary>
    public FloatParseHandling FloatParseHandling
    {
      get => this._floatParseHandling ?? FloatParseHandling.Double;
      set => this._floatParseHandling = new FloatParseHandling?(value);
    }

    /// <summary>
    /// Get or set how strings are escaped when writing JSON text.
    /// </summary>
    public StringEscapeHandling StringEscapeHandling
    {
      get => this._stringEscapeHandling ?? StringEscapeHandling.Default;
      set => this._stringEscapeHandling = new StringEscapeHandling?(value);
    }

    /// <summary>
    /// Gets or sets the culture used when reading JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
    /// </summary>
    public CultureInfo Culture
    {
      get => this._culture ?? JsonSerializerSettings.DefaultCulture;
      set => this._culture = value;
    }

    /// <summary>
    /// Gets a value indicating whether there will be a check for additional content after deserializing an object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if there will be a check for additional content after deserializing an object; otherwise, <c>false</c>.
    /// </value>
    public bool CheckAdditionalContent
    {
      get => this._checkAdditionalContent ?? false;
      set => this._checkAdditionalContent = new bool?(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> class.
    /// </summary>
    public JsonSerializerSettings()
    {
      this.Converters = (IList<JsonConverter>) new List<JsonConverter>();
    }
  }
}
