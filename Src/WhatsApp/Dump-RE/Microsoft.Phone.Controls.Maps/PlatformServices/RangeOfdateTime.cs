// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.RangeOfdateTime
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DataContract(Name = "RangeOfdateTime", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  internal class RangeOfdateTime : INotifyPropertyChanged
  {
    private DateTime FromField;
    private DateTime ToField;

    [DataMember]
    internal DateTime From
    {
      get => this.FromField;
      set
      {
        if (this.FromField.Equals(value))
          return;
        this.FromField = value;
        this.RaisePropertyChanged(nameof (From));
      }
    }

    [DataMember]
    internal DateTime To
    {
      get => this.ToField;
      set
      {
        if (this.ToField.Equals(value))
          return;
        this.ToField = value;
        this.RaisePropertyChanged(nameof (To));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
