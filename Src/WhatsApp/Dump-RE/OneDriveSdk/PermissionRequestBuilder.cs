// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.PermissionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class PermissionRequestBuilder : 
    BaseRequestBuilder,
    IPermissionRequestBuilder,
    IBaseRequestBuilder
  {
    public PermissionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IPermissionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IPermissionRequest Request(IEnumerable<Option> options)
    {
      return (IPermissionRequest) new PermissionRequest(this.RequestUrl, this.Client, options);
    }
  }
}
