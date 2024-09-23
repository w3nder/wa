// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JTokenType
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>Specifies the type of token.</summary>
  public enum JTokenType
  {
    /// <summary>No token type has been set.</summary>
    None,
    /// <summary>A JSON object.</summary>
    Object,
    /// <summary>A JSON array.</summary>
    Array,
    /// <summary>A JSON constructor.</summary>
    Constructor,
    /// <summary>A JSON object property.</summary>
    Property,
    /// <summary>A comment.</summary>
    Comment,
    /// <summary>An integer value.</summary>
    Integer,
    /// <summary>A float value.</summary>
    Float,
    /// <summary>A string value.</summary>
    String,
    /// <summary>A boolean value.</summary>
    Boolean,
    /// <summary>A null value.</summary>
    Null,
    /// <summary>An undefined value.</summary>
    Undefined,
    /// <summary>A date value.</summary>
    Date,
    /// <summary>A raw JSON value.</summary>
    Raw,
    /// <summary>A collection of bytes value.</summary>
    Bytes,
    /// <summary>A Guid value.</summary>
    Guid,
    /// <summary>A Uri value.</summary>
    Uri,
    /// <summary>A TimeSpan value.</summary>
    TimeSpan,
  }
}
