// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.Location
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [KnownType(typeof (GeocodeLocation))]
  [KnownType(typeof (UserLocation))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "Location", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [DebuggerStepThrough]
  internal class Location : INotifyPropertyChanged
  {
    private double AltitudeField;
    private double LatitudeField;
    private double LongitudeField;

    [DataMember]
    internal double Altitude
    {
      get => this.AltitudeField;
      set
      {
        if (this.AltitudeField.Equals(value))
          return;
        this.AltitudeField = value;
        this.RaisePropertyChanged(nameof (Altitude));
      }
    }

    [DataMember]
    internal double Latitude
    {
      get => this.LatitudeField;
      set
      {
        if (this.LatitudeField.Equals(value))
          return;
        this.LatitudeField = value;
        this.RaisePropertyChanged(nameof (Latitude));
      }
    }

    [DataMember]
    internal double Longitude
    {
      get => this.LongitudeField;
      set
      {
        if (this.LongitudeField.Equals(value))
          return;
        this.LongitudeField = value;
        this.RaisePropertyChanged(nameof (Longitude));
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
