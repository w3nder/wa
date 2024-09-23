// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Serialization.SerializableBase
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Reflection;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Serialization
{
  [Serializable]
  public abstract class SerializableBase : ISerializable
  {
    void ISerializable.Serialize(SerializationInfo info, XmlFormatter formatter)
    {
      Type type = this.GetType();
      info.TypeName = string.Format("{0},{1}", (object) type.FullName, (object) type.Assembly.FullName);
      this.Serialize(info, formatter);
    }

    void ISerializable.Deserialize(SerializationInfo info, XmlFormatter formatter)
    {
      this.Deserialize(info, formatter);
    }

    protected virtual void Serialize(SerializationInfo info, XmlFormatter formatter)
    {
      for (Type type = this.GetType(); type != null; type = type.BaseType)
      {
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
          if (!field.IsNotSerialized && !SerializableBase.IsNonSerialized((ICustomAttributeProvider) field))
          {
            object obj = this.GetValue(field);
            ISerializable serializable = obj as ISerializable;
            info.AddValue(string.Format("{0}!{1}", (object) field.DeclaringType.Name, (object) field.Name), serializable == null ? obj : (object) formatter.SerializeObject((object) serializable));
          }
        }
      }
    }

    private static bool IsNonSerialized(ICustomAttributeProvider field)
    {
      return field.GetCustomAttributes(typeof (NonSerializedAttribute), false).Length > 0;
    }

    protected virtual void Deserialize(SerializationInfo info, XmlFormatter formatter)
    {
      for (Type type = this.GetType(); type != null; type = type.BaseType)
      {
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
          if (!field.IsNotSerialized && !SerializableBase.IsNonSerialized((ICustomAttributeProvider) field))
          {
            object obj = info.GetValue(string.Format("{0}!{1}", (object) field.DeclaringType.Name, (object) field.Name));
            if (!(obj is SerializationInfo serializationInfo))
              this.SetValue(field, Convert.ChangeType(obj, field.FieldType, (IFormatProvider) null));
            else
              this.SetValue(field, (object) formatter.GetObject(serializationInfo.ReferenceId));
          }
        }
      }
    }

    protected abstract void SetValue(FieldInfo field, object value);

    protected abstract object GetValue(FieldInfo field);
  }
}
