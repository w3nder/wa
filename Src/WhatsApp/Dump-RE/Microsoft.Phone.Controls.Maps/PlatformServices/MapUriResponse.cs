// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.MapUriResponse
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
  [DataContract(Name = "MapUriResponse", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapUriResponse : ResponseBase
  {
    private string UriField;

    [DataMember]
    internal string Uri
    {
      get => this.UriField;
      set
      {
        if (object.ReferenceEquals((object) this.UriField, (object) value))
          return;
        this.UriField = value;
        this.RaisePropertyChanged(nameof (Uri));
      }
    }
  }
}
