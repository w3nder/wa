// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.CoverageArea
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
  [DataContract(Name = "CoverageArea", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  internal class CoverageArea : INotifyPropertyChanged
  {
    private Rectangle BoundingRectangleField;
    private RangeOfint ZoomRangeField;

    [DataMember]
    internal Rectangle BoundingRectangle
    {
      get => this.BoundingRectangleField;
      set
      {
        if (object.ReferenceEquals((object) this.BoundingRectangleField, (object) value))
          return;
        this.BoundingRectangleField = value;
        this.RaisePropertyChanged(nameof (BoundingRectangle));
      }
    }

    [DataMember]
    internal RangeOfint ZoomRange
    {
      get => this.ZoomRangeField;
      set
      {
        if (object.ReferenceEquals((object) this.ZoomRangeField, (object) value))
          return;
        this.ZoomRangeField = value;
        this.RaisePropertyChanged(nameof (ZoomRange));
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
