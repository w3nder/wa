// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonToken
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>Specifies the type of JSON token.</summary>
  public enum JsonToken
  {
    /// <summary>
    /// This is returned by the <see cref="T:Newtonsoft.Json.JsonReader" /> if a <see cref="M:Newtonsoft.Json.JsonReader.Read" /> method has not been called.
    /// </summary>
    None,
    /// <summary>An object start token.</summary>
    StartObject,
    /// <summary>An array start token.</summary>
    StartArray,
    /// <summary>A constructor start token.</summary>
    StartConstructor,
    /// <summary>An object property name.</summary>
    PropertyName,
    /// <summary>A comment.</summary>
    Comment,
    /// <summary>Raw JSON.</summary>
    Raw,
    /// <summary>An integer.</summary>
    Integer,
    /// <summary>A float.</summary>
    Float,
    /// <summary>A string.</summary>
    String,
    /// <summary>A boolean.</summary>
    Boolean,
    /// <summary>A null token.</summary>
    Null,
    /// <summary>An undefined token.</summary>
    Undefined,
    /// <summary>An object end token.</summary>
    EndObject,
    /// <summary>An array end token.</summary>
    EndArray,
    /// <summary>A constructor end token.</summary>
    EndConstructor,
    /// <summary>A Date.</summary>
    Date,
    /// <summary>Byte data.</summary>
    Bytes,
  }
}
