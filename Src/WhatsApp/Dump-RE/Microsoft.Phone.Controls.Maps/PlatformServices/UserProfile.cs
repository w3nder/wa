// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.UserProfile
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
  [DataContract(Name = "UserProfile", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  internal class UserProfile : INotifyPropertyChanged
  {
    private Heading CurrentHeadingField;
    private UserLocation CurrentLocationField;
    private DeviceType DeviceTypeField;
    private DistanceUnit DistanceUnitField;
    private string IPAddressField;
    private ShapeBase MapViewField;
    private SizeOfint ScreenSizeField;

    [DataMember]
    internal Heading CurrentHeading
    {
      get => this.CurrentHeadingField;
      set
      {
        if (object.ReferenceEquals((object) this.CurrentHeadingField, (object) value))
          return;
        this.CurrentHeadingField = value;
        this.RaisePropertyChanged(nameof (CurrentHeading));
      }
    }

    [DataMember]
    internal UserLocation CurrentLocation
    {
      get => this.CurrentLocationField;
      set
      {
        if (object.ReferenceEquals((object) this.CurrentLocationField, (object) value))
          return;
        this.CurrentLocationField = value;
        this.RaisePropertyChanged(nameof (CurrentLocation));
      }
    }

    [DataMember]
    internal DeviceType DeviceType
    {
      get => this.DeviceTypeField;
      set
      {
        if (this.DeviceTypeField.Equals((object) value))
          return;
        this.DeviceTypeField = value;
        this.RaisePropertyChanged(nameof (DeviceType));
      }
    }

    [DataMember]
    internal DistanceUnit DistanceUnit
    {
      get => this.DistanceUnitField;
      set
      {
        if (this.DistanceUnitField.Equals((object) value))
          return;
        this.DistanceUnitField = value;
        this.RaisePropertyChanged(nameof (DistanceUnit));
      }
    }

    [DataMember]
    internal string IPAddress
    {
      get => this.IPAddressField;
      set
      {
        if (object.ReferenceEquals((object) this.IPAddressField, (object) value))
          return;
        this.IPAddressField = value;
        this.RaisePropertyChanged(nameof (IPAddress));
      }
    }

    [DataMember]
    internal ShapeBase MapView
    {
      get => this.MapViewField;
      set
      {
        if (object.ReferenceEquals((object) this.MapViewField, (object) value))
          return;
        this.MapViewField = value;
        this.RaisePropertyChanged(nameof (MapView));
      }
    }

    [DataMember]
    internal SizeOfint ScreenSize
    {
      get => this.ScreenSizeField;
      set
      {
        if (object.ReferenceEquals((object) this.ScreenSizeField, (object) value))
          return;
        this.ScreenSizeField = value;
        this.RaisePropertyChanged(nameof (ScreenSize));
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
