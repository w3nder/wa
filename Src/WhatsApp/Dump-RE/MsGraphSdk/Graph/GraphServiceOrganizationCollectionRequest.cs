// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceOrganizationCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceOrganizationCollectionRequest : 
    BaseRequest,
    IGraphServiceOrganizationCollectionRequest,
    IBaseRequest
  {
    public GraphServiceOrganizationCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Organization> AddAsync(Organization organization)
    {
      return this.AddAsync(organization, CancellationToken.None);
    }

    public Task<Organization> AddAsync(
      Organization organization,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Organization>((object) organization, cancellationToken);
    }

    public Task<IGraphServiceOrganizationCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceOrganizationCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceOrganizationCollectionResponse collectionResponse = await this.SendAsync<GraphServiceOrganizationCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceOrganizationCollectionPage) null;
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

    public IGraphServiceOrganizationCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGraphServiceOrganizationCollectionRequest) this;
    }

    public IGraphServiceOrganizationCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceOrganizationCollectionRequest) this;
    }
  }
}
