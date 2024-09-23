// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.Pushpin
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
  [DebuggerStepThrough]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "Pushpin", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  internal class Pushpin : INotifyPropertyChanged
  {
    private string IconStyleField;
    private string LabelField;
    private Location LocationField;

    [DataMember]
    internal string IconStyle
    {
      get => this.IconStyleField;
      set
      {
        if (object.ReferenceEquals((object) this.IconStyleField, (object) value))
          return;
        this.IconStyleField = value;
        this.RaisePropertyChanged(nameof (IconStyle));
      }
    }

    [DataMember]
    internal string Label
    {
      get => this.LabelField;
      set
      {
        if (object.ReferenceEquals((object) this.LabelField, (object) value))
          return;
        this.LabelField = value;
        this.RaisePropertyChanged(nameof (Label));
      }
    }

    [DataMember]
    internal Location Location
    {
      get => this.LocationField;
      set
      {
        if (object.ReferenceEquals((object) this.LocationField, (object) value))
          return;
        this.LocationField = value;
        this.RaisePropertyChanged(nameof (Location));
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
