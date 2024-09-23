// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemVersionsCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemVersionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IItemVersionsCollectionRequestBuilder
  {
    public ItemVersionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IItemVersionsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IItemVersionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IItemVersionsCollectionRequest) new ItemVersionsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IItemRequestBuilder this[string id]
    {
      get
      {
        return (IItemRequestBuilder) new ItemRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
