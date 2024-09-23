// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.RequestBase
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
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "RequestBase", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [KnownType(typeof (MapUriRequest))]
  [KnownType(typeof (ImageryMetadataRequest))]
  internal class RequestBase : INotifyPropertyChanged
  {
    private Credentials CredentialsField;
    private string CultureField;
    private ExecutionOptions ExecutionOptionsField;
    private UserProfile UserProfileField;

    [DataMember]
    internal Credentials Credentials
    {
      get => this.CredentialsField;
      set
      {
        if (object.ReferenceEquals((object) this.CredentialsField, (object) value))
          return;
        this.CredentialsField = value;
        this.RaisePropertyChanged(nameof (Credentials));
      }
    }

    [DataMember]
    internal string Culture
    {
      get => this.CultureField;
      set
      {
        if (object.ReferenceEquals((object) this.CultureField, (object) value))
          return;
        this.CultureField = value;
        this.RaisePropertyChanged(nameof (Culture));
      }
    }

    [DataMember]
    internal ExecutionOptions ExecutionOptions
    {
      get => this.ExecutionOptionsField;
      set
      {
        if (object.ReferenceEquals((object) this.ExecutionOptionsField, (object) value))
          return;
        this.ExecutionOptionsField = value;
        this.RaisePropertyChanged(nameof (ExecutionOptions));
      }
    }

    [DataMember]
    internal UserProfile UserProfile
    {
      get => this.UserProfileField;
      set
      {
        if (object.ReferenceEquals((object) this.UserProfileField, (object) value))
          return;
        this.UserProfileField = value;
        this.RaisePropertyChanged(nameof (UserProfile));
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
