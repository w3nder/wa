// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Credentials
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DataContract(Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  public class Credentials
  {
    [DataMember]
    public string ApplicationId { get; set; }

    public override bool Equals(object obj)
    {
      return obj is Credentials credentials && this == credentials;
    }

    public override int GetHashCode() => this.ApplicationId.GetHashCode();

    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.ApplicationId) ? this.ApplicationId : string.Empty;
    }
  }
}
