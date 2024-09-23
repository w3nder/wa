// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDirectoryRoleTemplatesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDirectoryRoleTemplatesCollectionRequest : 
    BaseRequest,
    IGraphServiceDirectoryRoleTemplatesCollectionRequest,
    IBaseRequest
  {
    public GraphServiceDirectoryRoleTemplatesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryRoleTemplate> AddAsync(DirectoryRoleTemplate directoryRoleTemplate)
    {
      return this.AddAsync(directoryRoleTemplate, CancellationToken.None);
    }

    public Task<DirectoryRoleTemplate> AddAsync(
      DirectoryRoleTemplate directoryRoleTemplate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<DirectoryRoleTemplate>((object) directoryRoleTemplate, cancellationToken);
    }

    public Task<IGraphServiceDirectoryRoleTemplatesCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGraphServiceDirectoryRoleTemplatesCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GraphServiceDirectoryRoleTemplatesCollectionResponse collectionResponse = await this.SendAsync<GraphServiceDirectoryRoleTemplatesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGraphServiceDirectoryRoleTemplatesCollectionPage) null;
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

    public IGraphServiceDirectoryRoleTemplatesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGraphServiceDirectoryRoleTemplatesCollectionRequest) this;
    }
  }
}
