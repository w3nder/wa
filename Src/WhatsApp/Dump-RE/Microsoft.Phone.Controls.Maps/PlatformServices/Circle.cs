// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.Circle
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [DataContract(Name = "Circle", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  internal class Circle : ShapeBase
  {
    private Location CenterField;
    private DistanceUnit DistanceUnitField;
    private double RadiusField;

    [DataMember]
    internal Location Center
    {
      get => this.CenterField;
      set
      {
        if (object.ReferenceEquals((object) this.CenterField, (object) value))
          return;
        this.CenterField = value;
        this.RaisePropertyChanged(nameof (Center));
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
    internal double Radius
    {
      get => this.RadiusField;
      set
      {
        if (this.RadiusField.Equals(value))
          return;
        this.RadiusField = value;
        this.RaisePropertyChanged(nameof (Radius));
      }
    }
  }
}
