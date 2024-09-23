// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectGetMemberGroupsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectGetMemberGroupsRequest : 
    BaseRequest,
    IDirectoryObjectGetMemberGroupsRequest,
    IBaseRequest
  {
    public DirectoryObjectGetMemberGroupsRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new DirectoryObjectGetMemberGroupsRequestBody();
    }

    public DirectoryObjectGetMemberGroupsRequestBody RequestBody { get; private set; }

    public Task<IDirectoryObjectGetMemberGroupsCollectionPage> PostAsync()
    {
      return this.PostAsync(CancellationToken.None);
    }

    public async Task<IDirectoryObjectGetMemberGroupsCollectionPage> PostAsync(
      CancellationToken cancellationToken)
    {
      DirectoryObjectGetMemberGroupsCollectionResponse collectionResponse = await this.SendAsync<DirectoryObjectGetMemberGroupsCollectionResponse>((object) this.RequestBody, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDirectoryObjectGetMemberGroupsCollectionPage) null;
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

    public IDirectoryObjectGetMemberGroupsRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }

    public IDirectoryObjectGetMemberGroupsRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }

    public IDirectoryObjectGetMemberGroupsRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }

    public IDirectoryObjectGetMemberGroupsRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }

    public IDirectoryObjectGetMemberGroupsRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }

    public IDirectoryObjectGetMemberGroupsRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDirectoryObjectGetMemberGroupsRequest) this;
    }
  }
}
