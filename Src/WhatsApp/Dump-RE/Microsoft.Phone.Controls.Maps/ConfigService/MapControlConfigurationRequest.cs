// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.MapControlConfigurationRequest
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [DebuggerStepThrough]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "MapControlConfigurationRequest", Namespace = "http://dev.virtualearth.net/webservices/v1/mapcontrolconfiguration")]
  internal class MapControlConfigurationRequest : RequestBase
  {
    private string SectionNameField;
    private string VersionField;

    [DataMember]
    internal string SectionName
    {
      get => this.SectionNameField;
      set
      {
        if (object.ReferenceEquals((object) this.SectionNameField, (object) value))
          return;
        this.SectionNameField = value;
        this.RaisePropertyChanged(nameof (SectionName));
      }
    }

    [DataMember]
    internal string Version
    {
      get => this.VersionField;
      set
      {
        if (object.ReferenceEquals((object) this.VersionField, (object) value))
          return;
        this.VersionField = value;
        this.RaisePropertyChanged(nameof (Version));
      }
    }
  }
}
