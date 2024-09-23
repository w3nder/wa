// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.IAttributeProvider
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>Provides methods to get attributes.</summary>
  public interface IAttributeProvider
  {
    /// <summary>
    /// Returns a collection of all of the attributes, or an empty collection if there are no attributes.
    /// </summary>
    /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    /// <returns>A collection of <see cref="T:System.Attribute" />s, or an empty collection.</returns>
    IList<Attribute> GetAttributes(bool inherit);

    /// <summary>
    /// Returns a collection of attributes, identified by type, or an empty collection if there are no attributes.
    /// </summary>
    /// <param name="attributeType">The type of the attributes.</param>
    /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    /// <returns>A collection of <see cref="T:System.Attribute" />s, or an empty collection.</returns>
    IList<Attribute> GetAttributes(Type attributeType, bool inherit);
  }
}
