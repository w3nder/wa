// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.Credentials
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DataContract(Name = "Credentials", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [DebuggerStepThrough]
  internal class Credentials : INotifyPropertyChanged
  {
    private string ApplicationIdField;
    private string TokenField;

    [DataMember]
    internal string ApplicationId
    {
      get => this.ApplicationIdField;
      set
      {
        if (object.ReferenceEquals((object) this.ApplicationIdField, (object) value))
          return;
        this.ApplicationIdField = value;
        this.RaisePropertyChanged(nameof (ApplicationId));
      }
    }

    [DataMember]
    internal string Token
    {
      get => this.TokenField;
      set
      {
        if (object.ReferenceEquals((object) this.TokenField, (object) value))
          return;
        this.TokenField = value;
        this.RaisePropertyChanged(nameof (Token));
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
