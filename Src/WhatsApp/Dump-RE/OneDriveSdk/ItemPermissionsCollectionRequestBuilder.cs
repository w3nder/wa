// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemPermissionsCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemPermissionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IItemPermissionsCollectionRequestBuilder
  {
    public ItemPermissionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IItemPermissionsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IItemPermissionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IItemPermissionsCollectionRequest) new ItemPermissionsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IPermissionRequestBuilder this[string id]
    {
      get
      {
        return (IPermissionRequestBuilder) new PermissionRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
