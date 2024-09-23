// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ExtensionDataGetter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Gets extension data for an object during serialization.
  /// </summary>
  /// <param name="o">The object to set extension data on.</param>
  public delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);
}
