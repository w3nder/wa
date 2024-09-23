// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.SerializationBinder
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Allows users to control class loading and mandate what class to load.
  /// </summary>
  public abstract class SerializationBinder
  {
    /// <summary>
    /// When overridden in a derived class, controls the binding of a serialized object to a type.
    /// </summary>
    /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object.</param>
    /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object</param>
    /// <returns>The type of the object the formatter creates a new instance of.</returns>
    public abstract Type BindToType(string assemblyName, string typeName);

    /// <summary>
    /// When overridden in a derived class, controls the binding of a serialized object to a type.
    /// </summary>
    /// <param name="serializedType">The type of the object the formatter creates a new instance of.</param>
    /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object.</param>
    /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object.</param>
    public virtual void BindToName(
      Type serializedType,
      out string assemblyName,
      out string typeName)
    {
      assemblyName = (string) null;
      typeName = (string) null;
    }
  }
}
