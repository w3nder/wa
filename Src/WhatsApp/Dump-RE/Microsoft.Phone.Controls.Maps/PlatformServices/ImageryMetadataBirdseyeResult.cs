// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryMetadataBirdseyeResult
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
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "ImageryMetadataBirdseyeResult", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  internal class ImageryMetadataBirdseyeResult : ImageryMetadataResult
  {
    private Heading HeadingField;
    private int TilesXField;
    private int TilesYField;

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
    internal int TilesX
    {
      get => this.TilesXField;
      set
      {
        if (this.TilesXField.Equals(value))
          return;
        this.TilesXField = value;
        this.RaisePropertyChanged(nameof (TilesX));
      }
    }

    [DataMember]
    internal int TilesY
    {
      get => this.TilesYField;
      set
      {
        if (this.TilesYField.Equals(value))
          return;
        this.TilesYField = value;
        this.RaisePropertyChanged(nameof (TilesY));
      }
    }
  }
}
