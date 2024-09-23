// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OneDriveSharesCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class OneDriveSharesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IOneDriveSharesCollectionRequestBuilder
  {
    public OneDriveSharesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IOneDriveSharesCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IOneDriveSharesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IOneDriveSharesCollectionRequest) new OneDriveSharesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IShareRequestBuilder this[string id]
    {
      get
      {
        return (IShareRequestBuilder) new ShareRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
