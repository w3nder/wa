// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ResponseBase
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
  [DataContract(Name = "ResponseBase", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [KnownType(typeof (MapUriResponse))]
  [KnownType(typeof (ImageryMetadataResponse))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class ResponseBase : INotifyPropertyChanged
  {
    private Uri BrandLogoUriField;
    private ResponseSummary ResponseSummaryField;

    [DataMember]
    internal Uri BrandLogoUri
    {
      get => this.BrandLogoUriField;
      set
      {
        if (object.ReferenceEquals((object) this.BrandLogoUriField, (object) value))
          return;
        this.BrandLogoUriField = value;
        this.RaisePropertyChanged(nameof (BrandLogoUri));
      }
    }

    [DataMember]
    internal ResponseSummary ResponseSummary
    {
      get => this.ResponseSummaryField;
      set
      {
        if (object.ReferenceEquals((object) this.ResponseSummaryField, (object) value))
          return;
        this.ResponseSummaryField = value;
        this.RaisePropertyChanged(nameof (ResponseSummary));
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
