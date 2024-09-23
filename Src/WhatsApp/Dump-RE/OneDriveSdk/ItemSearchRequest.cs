// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemSearchRequest
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
  public class ItemSearchRequest : BaseRequest, IItemSearchRequest, IBaseRequest
  {
    public ItemSearchRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "GET";
    }

    public Task<IItemSearchCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IItemSearchCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      ItemSearchCollectionResponse collectionResponse = await this.SendAsync<ItemSearchCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IItemSearchCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      return collectionResponse.Value;
    }

    public IItemSearchRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemSearchRequest) this;
    }

    public IItemSearchRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemSearchRequest) this;
    }

    public IItemSearchRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IItemSearchRequest) this;
    }

    public IItemSearchRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IItemSearchRequest) this;
    }

    public IItemSearchRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IItemSearchRequest) this;
    }

    public IItemSearchRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IItemSearchRequest) this;
    }
  }
}
