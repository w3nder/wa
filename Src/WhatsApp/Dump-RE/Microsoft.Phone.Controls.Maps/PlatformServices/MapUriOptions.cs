// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.MapUriOptions
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
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [DataContract(Name = "MapUriOptions", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  internal class MapUriOptions : INotifyPropertyChanged
  {
    private Collection<string> DisplayLayersField;
    private SizeOfint ImageSizeField;
    private ImageType ImageTypeField;
    private bool PreventIconCollisionField;
    private MapStyle StyleField;
    private UriScheme UriSchemeField;
    private int? ZoomLevelField;

    [DataMember]
    internal Collection<string> DisplayLayers
    {
      get => this.DisplayLayersField;
      set
      {
        if (object.ReferenceEquals((object) this.DisplayLayersField, (object) value))
          return;
        this.DisplayLayersField = value;
        this.RaisePropertyChanged(nameof (DisplayLayers));
      }
    }

    [DataMember]
    internal SizeOfint ImageSize
    {
      get => this.ImageSizeField;
      set
      {
        if (object.ReferenceEquals((object) this.ImageSizeField, (object) value))
          return;
        this.ImageSizeField = value;
        this.RaisePropertyChanged(nameof (ImageSize));
      }
    }

    [DataMember]
    internal ImageType ImageType
    {
      get => this.ImageTypeField;
      set
      {
        if (this.ImageTypeField.Equals((object) value))
          return;
        this.ImageTypeField = value;
        this.RaisePropertyChanged(nameof (ImageType));
      }
    }

    [DataMember]
    internal bool PreventIconCollision
    {
      get => this.PreventIconCollisionField;
      set
      {
        if (this.PreventIconCollisionField.Equals(value))
          return;
        this.PreventIconCollisionField = value;
        this.RaisePropertyChanged(nameof (PreventIconCollision));
      }
    }

    [DataMember]
    internal MapStyle Style
    {
      get => this.StyleField;
      set
      {
        if (this.StyleField.Equals((object) value))
          return;
        this.StyleField = value;
        this.RaisePropertyChanged(nameof (Style));
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
