// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryMetadataOptions
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
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "ImageryMetadataOptions", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  [DebuggerStepThrough]
  internal class ImageryMetadataOptions : INotifyPropertyChanged
  {
    private Heading HeadingField;
    private Location LocationField;
    private bool ReturnImageryProvidersField;
    private UriScheme UriSchemeField;
    private int? ZoomLevelField;

    [DataMember]
    internal Heading Heading
    {
      get => this.HeadingField;
      set
      {
        if (object.ReferenceEquals((object) this.HeadingField, (object) value))
          return;
        this.HeadingField = value;
        this.RaisePropertyChanged(nameof (Heading));
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

    [DataMember]
    internal bool ReturnImageryProviders
    {
      get => this.ReturnImageryProvidersField;
      set
      {
        if (this.ReturnImageryProvidersField.Equals(value))
          return;
        this.ReturnImageryProvidersField = value;
        this.RaisePropertyChanged(nameof (ReturnImageryProviders));
      }
    }

    [DataMember]
    internal UriScheme UriScheme
    {
      get => this.UriSchemeField;
      set
      {
        if (this.UriSchemeField.Equals((object) value))
          return;
        this.UriSchemeField = value;
        this.RaisePropertyChanged(nameof (UriScheme));
      }
    }

    [DataMember]
    internal int? ZoomLevel
    {
      get => this.ZoomLevelField;
      set
      {
        if (this.ZoomLevelField.Equals((object) value))
          return;
        this.ZoomLevelField = value;
        this.RaisePropertyChanged(nameof (ZoomLevel));
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
