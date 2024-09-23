// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Resolves member mappings for a type, camel casing property names.
  /// </summary>
  public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver" /> class.
    /// </summary>
    public CamelCasePropertyNamesContractResolver()
      : base(true)
    {
    }

    /// <summary>Resolves the name of the property.</summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The property name camel cased.</returns>
    protected override string ResolvePropertyName(string propertyName)
    {
      return StringUtils.ToCamelCase(propertyName);
    }
  }
}
