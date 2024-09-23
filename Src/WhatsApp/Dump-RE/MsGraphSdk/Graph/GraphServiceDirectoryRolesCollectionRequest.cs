// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDirectoryRolesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDirectoryRolesCollectionRequest : 
    BaseRequest,
    IGraphServiceDirectoryRolesCollectionRequest,
    IBaseRequest
  {
    public GraphServiceDirectoryRolesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryRole> AddAsync(DirectoryRole directoryRole)
    {
      return this.AddAsync(directoryRole, CancellationToken.None);
    }

    public Task<DirectoryRole> AddAsync(
      DirectoryRole directoryRole,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<DirectoryRole>((object) directoryRole, cancellationToken);
    }

    public Task<IGraphServiceDirectoryRolesCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceDirectoryRolesCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceDirectoryRolesCollectionResponse collectionResponse = await this.SendAsync<GraphServiceDirectoryRolesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceDirectoryRolesCollectionPage) null;
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

    public IGraphServiceDirectoryRolesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceDirectoryRolesCollectionRequest) this;
    }
  }
}
