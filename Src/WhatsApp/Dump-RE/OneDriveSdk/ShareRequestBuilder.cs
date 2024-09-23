// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ShareRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ShareRequestBuilder : BaseRequestBuilder, IShareRequestBuilder, IBaseRequestBuilder
  {
    public IItemRequestBuilder Root
    {
      get
      {
        return (IItemRequestBuilder) new ItemRequestBuilder(this.AppendSegmentToRequestUrl("root"), this.Client);
      }
    }

    public ShareRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IShareRequest Request() => this.Request((IEnumerable<Option>) null);

    public IShareRequest Request(IEnumerable<Option> options)
    {
      return (IShareRequest) new ShareRequest(this.RequestUrl, this.Client, options);
    }

    public IShareItemsCollectionRequestBuilder Items
    {
      get
      {
        return (IShareItemsCollectionRequestBuilder) new ShareItemsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("items"), this.Client);
      }
    }
  }
}
