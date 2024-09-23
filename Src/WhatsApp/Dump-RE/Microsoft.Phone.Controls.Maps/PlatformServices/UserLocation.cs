// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.UserLocation
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
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [DataContract(Name = "UserLocation", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  internal class UserLocation : Location
  {
    private Confidence ConfidenceField;

    [DataMember]
    internal Confidence Confidence
    {
      get => this.ConfidenceField;
      set
      {
        if (this.ConfidenceField.Equals((object) value))
          return;
        this.ConfidenceField = value;
        this.RaisePropertyChanged(nameof (Confidence));
      }
    }
  }
}
