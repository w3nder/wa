// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.MemberSerialization
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies the member serialization options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  public enum MemberSerialization
  {
    /// <summary>
    /// All public members are serialized by default. Members can be excluded using <see cref="T:Newtonsoft.Json.JsonIgnoreAttribute" /> or <see cref="!:NonSerializedAttribute" />.
    /// This is the default member serialization mode.
    /// </summary>
    OptOut,
    /// <summary>
    /// Only members must be marked with <see cref="T:Newtonsoft.Json.JsonPropertyAttribute" /> or <see cref="T:System.Runtime.Serialization.DataMemberAttribute" /> are serialized.
    /// This member serialization mode can also be set by marking the class with <see cref="T:System.Runtime.Serialization.DataContractAttribute" />.
    /// </summary>
    OptIn,
    /// <summary>
    /// All public and private fields are serialized. Members can be excluded using <see cref="T:Newtonsoft.Json.JsonIgnoreAttribute" /> or <see cref="!:NonSerializedAttribute" />.
    /// This member serialization mode can also be set by marking the class with <see cref="!:SerializableAttribute" />
    /// and setting IgnoreSerializableAttribute on <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> to false.
    /// </summary>
    Fields,
  }
}
