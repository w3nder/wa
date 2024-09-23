// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ResponseSummary
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
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class ResponseSummary : INotifyPropertyChanged
  {
    private AuthenticationResultCode AuthenticationResultCodeField;
    private string CopyrightField;
    private string FaultReasonField;
    private ResponseStatusCode StatusCodeField;
    private string TraceIdField;

    [DataMember]
    internal AuthenticationResultCode AuthenticationResultCode
    {
      get => this.AuthenticationResultCodeField;
      set
      {
        if (this.AuthenticationResultCodeField.Equals((object) value))
          return;
        this.AuthenticationResultCodeField = value;
        this.RaisePropertyChanged(nameof (AuthenticationResultCode));
      }
    }

    [DataMember]
    internal string Copyright
    {
      get => this.CopyrightField;
      set
      {
        if (object.ReferenceEquals((object) this.CopyrightField, (object) value))
          return;
        this.CopyrightField = value;
        this.RaisePropertyChanged(nameof (Copyright));
      }
    }

    [DataMember]
    internal string FaultReason
    {
      get => this.FaultReasonField;
      set
      {
        if (object.ReferenceEquals((object) this.FaultReasonField, (object) value))
          return;
        this.FaultReasonField = value;
        this.RaisePropertyChanged(nameof (FaultReason));
      }
    }

    [DataMember]
    internal ResponseStatusCode StatusCode
    {
      get => this.StatusCodeField;
      set
      {
        if (this.StatusCodeField.Equals((object) value))
          return;
        this.StatusCodeField = value;
        this.RaisePropertyChanged(nameof (StatusCode));
      }
    }

    [DataMember]
    internal string TraceId
    {
      get => this.TraceIdField;
      set
      {
        if (object.ReferenceEquals((object) this.TraceIdField, (object) value))
          return;
        this.TraceIdField = value;
        this.RaisePropertyChanged(nameof (TraceId));
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
