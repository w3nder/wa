// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ExtensionDataSetter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Sets extension data for an object during deserialization.
  /// </summary>
  /// <param name="o">The object to set extension data on.</param>
  /// <param name="key">The extension data key.</param>
  /// <param name="value">The extension data value.</param>
  public delegate void ExtensionDataSetter(object o, string key, object value);
}
