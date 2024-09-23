// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonConverterAttribute
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> to use the specified <see cref="T:Newtonsoft.Json.JsonConverter" /> when serializing the member or class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Struct, AllowMultiple = false)]
  public sealed class JsonConverterAttribute : Attribute
  {
    private readonly Type _converterType;

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the converter.
    /// </summary>
    /// <value>The <see cref="T:System.Type" /> of the converter.</value>
    public Type ConverterType => this._converterType;

    /// <summary>
    /// The parameter list to use when constructing the JsonConverter described by ConverterType.
    /// If null, the default constructor is used.
    /// </summary>
    public object[] ConverterParameters { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonConverterAttribute" /> class.
    /// </summary>
    /// <param name="converterType">Type of the converter.</param>
    public JsonConverterAttribute(Type converterType)
    {
      this._converterType = converterType != null ? converterType : throw new ArgumentNullException(nameof (converterType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonConverterAttribute" /> class.
    /// </summary>
    /// <param name="converterType">Type of the converter.</param>
    /// <param name="converterParameters">Parameter list to use when constructing the JsonConverter. Can be null.</param>
    public JsonConverterAttribute(Type converterType, params object[] converterParameters)
      : this(converterType)
    {
      this.ConverterParameters = converterParameters;
    }
  }
}
