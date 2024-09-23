// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryProvider
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [DebuggerStepThrough]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "ImageryProvider", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  internal class ImageryProvider : INotifyPropertyChanged
  {
    private string AttributionField;
    private Collection<CoverageArea> CoverageAreasField;

    [DataMember]
    internal string Attribution
    {
      get => this.AttributionField;
      set
      {
        if (object.ReferenceEquals((object) this.AttributionField, (object) value))
          return;
        this.AttributionField = value;
        this.RaisePropertyChanged(nameof (Attribution));
      }
    }

    [DataMember]
    internal Collection<CoverageArea> CoverageAreas
    {
      get => this.CoverageAreasField;
      set
      {
        if (object.ReferenceEquals((object) this.CoverageAreasField, (object) value))
          return;
        this.CoverageAreasField = value;
        this.RaisePropertyChanged(nameof (CoverageAreas));
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
