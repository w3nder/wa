// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDrivesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDrivesCollectionRequest : 
    BaseRequest,
    IGraphServiceDrivesCollectionRequest,
    IBaseRequest
  {
    public GraphServiceDrivesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Drive> AddAsync(Drive drive) => this.AddAsync(drive, CancellationToken.None);

    public Task<Drive> AddAsync(Drive drive, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Drive>((object) drive, cancellationToken);
    }

    public Task<IGraphServiceDrivesCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceDrivesCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceDrivesCollectionResponse collectionResponse = await this.SendAsync<GraphServiceDrivesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceDrivesCollectionPage) null;
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

    public IGraphServiceDrivesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IGraphServiceDrivesCollectionRequest) this;
    }

    public IGraphServiceDrivesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IGraphServiceDrivesCollectionRequest) this;
    }

    public IGraphServiceDrivesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGraphServiceDrivesCollectionRequest) this;
    }

    public IGraphServiceDrivesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IGraphServiceDrivesCollectionRequest) this;
    }

    public IGraphServiceDrivesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IGraphServiceDrivesCollectionRequest) this;
    }

    public IGraphServiceDrivesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceDrivesCollectionRequest) this;
    }
  }
}
