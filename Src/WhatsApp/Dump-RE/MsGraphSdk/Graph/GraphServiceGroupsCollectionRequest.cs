// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceGroupsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceGroupsCollectionRequest : 
    BaseRequest,
    IGraphServiceGroupsCollectionRequest,
    IBaseRequest
  {
    public GraphServiceGroupsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Group> AddAsync(Group group) => this.AddAsync(group, CancellationToken.None);

    public Task<Group> AddAsync(Group group, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Group>((object) group, cancellationToken);
    }

    public Task<IGraphServiceGroupsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceGroupsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceGroupsCollectionResponse collectionResponse = await this.SendAsync<GraphServiceGroupsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceGroupsCollectionPage) null;
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

    public IGraphServiceGroupsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGraphServiceGroupsCollectionRequest) this;
    }

    public IGraphServiceGroupsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceGroupsCollectionRequest) this;
    }
  }
}
