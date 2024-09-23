// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.MapUriRequest
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [DataContract(Name = "MapUriRequest", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  internal class MapUriRequest : RequestBase
  {
    private Location CenterField;
    private Location MajorRoutesDestinationField;
    private MapUriOptions OptionsField;
    private Collection<Pushpin> PushpinsField;

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
    internal Location MajorRoutesDestination
    {
      get => this.MajorRoutesDestinationField;
      set
      {
        if (object.ReferenceEquals((object) this.MajorRoutesDestinationField, (object) value))
          return;
        this.MajorRoutesDestinationField = value;
        this.RaisePropertyChanged(nameof (MajorRoutesDestination));
      }
    }

    [DataMember]
    internal MapUriOptions Options
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
    internal Collection<Pushpin> Pushpins
    {
      get => this.PushpinsField;
      set
      {
        if (object.ReferenceEquals((object) this.PushpinsField, (object) value))
          return;
        this.PushpinsField = value;
        this.RaisePropertyChanged(nameof (Pushpins));
      }
    }
  }
}
