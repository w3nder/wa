// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.DefaultSerializationBinder
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// The default serialization binder used when resolving and loading classes from type names.
  /// </summary>
  public class DefaultSerializationBinder : SerializationBinder
  {
    internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();
    private readonly ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type> _typeCache = new ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type>(new Func<DefaultSerializationBinder.TypeNameKey, Type>(DefaultSerializationBinder.GetTypeFromTypeNameKey));

    private static Type GetTypeFromTypeNameKey(DefaultSerializationBinder.TypeNameKey typeNameKey)
    {
      string assemblyName = typeNameKey.AssemblyName;
      string typeName = typeNameKey.TypeName;
      if (assemblyName == null)
        return Type.GetType(typeName);
      Assembly assembly = Assembly.Load(assemblyName);
      return (assembly != null ? assembly.GetType(typeName) : throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) assemblyName))) ?? throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) typeName, (object) assembly.FullName));
    }

    /// <summary>
    /// When overridden in a derived class, controls the binding of a serialized object to a type.
    /// </summary>
    /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object.</param>
    /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object.</param>
    /// <returns>
    /// The type of the object the formatter creates a new instance of.
    /// </returns>
    public override Type BindToType(string assemblyName, string typeName)
    {
      return this._typeCache.Get(new DefaultSerializationBinder.TypeNameKey(assemblyName, typeName));
    }

    /// <summary>
    /// When overridden in a derived class, controls the binding of a serialized object to a type.
    /// </summary>
    /// <param name="serializedType">The type of the object the formatter creates a new instance of.</param>
    /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object. </param>
    /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object. </param>
    public override void BindToName(
      Type serializedType,
      out string assemblyName,
      out string typeName)
    {
      assemblyName = serializedType.Assembly.FullName;
      typeName = serializedType.FullName;
    }

    internal struct TypeNameKey : IEquatable<DefaultSerializationBinder.TypeNameKey>
    {
      internal readonly string AssemblyName;
      internal readonly string TypeName;

      public TypeNameKey(string assemblyName, string typeName)
      {
        this.AssemblyName = assemblyName;
        this.TypeName = typeName;
      }

      public override int GetHashCode()
      {
        return (this.AssemblyName != null ? this.AssemblyName.GetHashCode() : 0) ^ (this.TypeName != null ? this.TypeName.GetHashCode() : 0);
      }

      public override bool Equals(object obj)
      {
        return obj is DefaultSerializationBinder.TypeNameKey other && this.Equals(other);
      }

      public bool Equals(DefaultSerializationBinder.TypeNameKey other)
      {
        return this.AssemblyName == other.AssemblyName && this.TypeName == other.TypeName;
      }
    }
  }
}
