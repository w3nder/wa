// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectGetMemberObjectsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectGetMemberObjectsRequest : 
    BaseRequest,
    IDirectoryObjectGetMemberObjectsRequest,
    IBaseRequest
  {
    public DirectoryObjectGetMemberObjectsRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new DirectoryObjectGetMemberObjectsRequestBody();
    }

    public DirectoryObjectGetMemberObjectsRequestBody RequestBody { get; private set; }

    public Task<IDirectoryObjectGetMemberObjectsCollectionPage> PostAsync()
    {
      return this.PostAsync(CancellationToken.None);
    }

    public async Task<IDirectoryObjectGetMemberObjectsCollectionPage> PostAsync(
      CancellationToken cancellationToken)
    {
      DirectoryObjectGetMemberObjectsCollectionResponse collectionResponse = await this.SendAsync<DirectoryObjectGetMemberObjectsCollectionResponse>((object) this.RequestBody, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDirectoryObjectGetMemberObjectsCollectionPage) null;
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

    public IDirectoryObjectGetMemberObjectsRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }

    public IDirectoryObjectGetMemberObjectsRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }

    public IDirectoryObjectGetMemberObjectsRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }

    public IDirectoryObjectGetMemberObjectsRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }

    public IDirectoryObjectGetMemberObjectsRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }

    public IDirectoryObjectGetMemberObjectsRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDirectoryObjectGetMemberObjectsRequest) this;
    }
  }
}
