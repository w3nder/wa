// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.Rectangle
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "Rectangle", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [DebuggerStepThrough]
  internal class Rectangle : ShapeBase
  {
    private Location NortheastField;
    private Location SouthwestField;

    [DataMember]
    internal Location Northeast
    {
      get => this.NortheastField;
      set
      {
        if (object.ReferenceEquals((object) this.NortheastField, (object) value))
          return;
        this.NortheastField = value;
        this.RaisePropertyChanged(nameof (Northeast));
      }
    }

    [DataMember]
    internal Location Southwest
    {
      get => this.SouthwestField;
      set
      {
        if (object.ReferenceEquals((object) this.SouthwestField, (object) value))
          return;
        this.SouthwestField = value;
        this.RaisePropertyChanged(nameof (Southwest));
      }
    }
  }
}
