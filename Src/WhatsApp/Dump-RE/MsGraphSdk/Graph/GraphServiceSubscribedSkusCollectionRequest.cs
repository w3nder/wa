// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceSubscribedSkusCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceSubscribedSkusCollectionRequest : 
    BaseRequest,
    IGraphServiceSubscribedSkusCollectionRequest,
    IBaseRequest
  {
    public GraphServiceSubscribedSkusCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<SubscribedSku> AddAsync(SubscribedSku subscribedSku)
    {
      return this.AddAsync(subscribedSku, CancellationToken.None);
    }

    public Task<SubscribedSku> AddAsync(
      SubscribedSku subscribedSku,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<SubscribedSku>((object) subscribedSku, cancellationToken);
    }

    public Task<IGraphServiceSubscribedSkusCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceSubscribedSkusCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceSubscribedSkusCollectionResponse collectionResponse = await this.SendAsync<GraphServiceSubscribedSkusCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceSubscribedSkusCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
      }
      return collectionResponse.Value;
    }

    public IGraphServiceSubscribedSkusCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceSubscribedSkusCollectionRequest) this;
    }
  }
}
