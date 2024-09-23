// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Serialization.SerializableString
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System.Reflection;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Serialization
{
  [Serializable]
  public class SerializableString : SerializableBase
  {
    public string String { get; set; }

    public SerializableString(string value) => this.String = value;

    public SerializableString()
    {
    }

    protected override object GetValue(FieldInfo field)
    {
      return field.DeclaringType != typeof (SerializableString) ? (object) null : field.GetValue((object) this);
    }

    public bool Equals(SerializableString other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      return object.ReferenceEquals((object) this, (object) other) || object.Equals((object) other.String, (object) this.String);
    }

    public override bool Equals(object other)
    {
      if (object.ReferenceEquals((object) null, other))
        return false;
      if (object.ReferenceEquals((object) this, other))
        return true;
      return other.GetType() == typeof (SerializableString) && this.Equals((SerializableString) other);
    }

    public override int GetHashCode() => this.String == null ? 0 : this.String.GetHashCode();

    protected override void SetValue(FieldInfo field, object value)
    {
      if (field.DeclaringType != typeof (SerializableString))
        return;
      field.SetValue((object) this, value);
    }
  }
}
