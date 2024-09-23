// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Serialization.SerializableDateTime
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Reflection;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Serialization
{
  [Serializable]
  public class SerializableDateTime : SerializableBase
  {
    public DateTime DateTime { get; set; }

    public SerializableDateTime(int year, int month, int day)
    {
      this.DateTime = new DateTime(year, month, day);
    }

    public SerializableDateTime()
    {
    }

    protected override object GetValue(FieldInfo field)
    {
      return field.DeclaringType != typeof (SerializableDateTime) ? (object) null : field.GetValue((object) this);
    }

    protected override void SetValue(FieldInfo field, object value)
    {
      if (field.DeclaringType != typeof (SerializableDateTime))
        return;
      field.SetValue((object) this, value);
    }

    public bool Equals(SerializableDateTime other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      return object.ReferenceEquals((object) this, (object) other) || object.Equals((object) other.DateTime, (object) this.DateTime);
    }

    public override bool Equals(object other)
    {
      if (object.ReferenceEquals((object) null, other))
        return false;
      if (object.ReferenceEquals((object) this, other))
        return true;
      return other.GetType() == typeof (SerializableDateTime) && this.Equals((SerializableDateTime) other);
    }

    public override int GetHashCode() => this.DateTime.GetHashCode();
  }
}
