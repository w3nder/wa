// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemDeltaRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemDeltaRequest : BaseRequest, IItemDeltaRequest, IBaseRequest
  {
    public ItemDeltaRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "GET";
    }

    public Task<IItemDeltaCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemDeltaCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      ItemDeltaCollectionResponse collectionResponse = await this.SendAsync<ItemDeltaCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemDeltaCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      collectionResponse.Value.Token = collectionResponse.Token;
      collectionResponse.Value.DeltaLink = collectionResponse.DeltaLink;
      return collectionResponse.Value;
    }

    public IItemDeltaRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemDeltaRequest) this;
    }

    public IItemDeltaRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemDeltaRequest) this;
    }

    public IItemDeltaRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemDeltaRequest) this;
    }

    public IItemDeltaRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemDeltaRequest) this;
    }

    public IItemDeltaRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemDeltaRequest) this;
    }

    public IItemDeltaRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemDeltaRequest) this;
    }
  }
}
