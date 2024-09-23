// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryMetadataRequest
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
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "ImageryMetadataRequest", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class ImageryMetadataRequest : RequestBase
  {
    private ImageryMetadataOptions OptionsField;
    private MapStyle StyleField;

    [DataMember]
    internal ImageryMetadataOptions Options
    {
      get => this.OptionsField;
      set
      {
        if (object.ReferenceEquals((object) this.OptionsField, (object) value))
          return;
        this.OptionsField = value;
        this.RaisePropertyChanged(nameof (Options));
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
  }
}
