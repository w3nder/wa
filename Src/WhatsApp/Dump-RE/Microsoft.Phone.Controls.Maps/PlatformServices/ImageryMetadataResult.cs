// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryMetadataResult
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
  [DebuggerStepThrough]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [KnownType(typeof (ImageryMetadataBirdseyeResult))]
  [DataContract(Name = "ImageryMetadataResult", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  internal class ImageryMetadataResult : INotifyPropertyChanged
  {
    private SizeOfint ImageSizeField;
    private string ImageUriField;
    private Collection<string> ImageUriSubdomainsField;
    private Collection<ImageryProvider> ImageryProvidersField;
    private RangeOfdateTime VintageField;
    private RangeOfint ZoomRangeField;

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
    internal string ImageUri
    {
      get => this.ImageUriField;
      set
      {
        if (object.ReferenceEquals((object) this.ImageUriField, (object) value))
          return;
        this.ImageUriField = value;
        this.RaisePropertyChanged(nameof (ImageUri));
      }
    }

    [DataMember]
    internal Collection<string> ImageUriSubdomains
    {
      get => this.ImageUriSubdomainsField;
      set
      {
        if (object.ReferenceEquals((object) this.ImageUriSubdomainsField, (object) value))
          return;
        this.ImageUriSubdomainsField = value;
        this.RaisePropertyChanged(nameof (ImageUriSubdomains));
      }
    }

    [DataMember]
    internal Collection<ImageryProvider> ImageryProviders
    {
      get => this.ImageryProvidersField;
      set
      {
        if (object.ReferenceEquals((object) this.ImageryProvidersField, (object) value))
          return;
        this.ImageryProvidersField = value;
        this.RaisePropertyChanged(nameof (ImageryProviders));
      }
    }

    [DataMember]
    internal RangeOfdateTime Vintage
    {
      get => this.VintageField;
      set
      {
        if (object.ReferenceEquals((object) this.VintageField, (object) value))
          return;
        this.VintageField = value;
        this.RaisePropertyChanged(nameof (Vintage));
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
