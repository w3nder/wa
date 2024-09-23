// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemThumbnailsCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemThumbnailsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IItemThumbnailsCollectionRequestBuilder
  {
    public ItemThumbnailsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IItemThumbnailsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IItemThumbnailsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IItemThumbnailsCollectionRequest) new ItemThumbnailsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IThumbnailSetRequestBuilder this[string id]
    {
      get
      {
        return (IThumbnailSetRequestBuilder) new ThumbnailSetRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
