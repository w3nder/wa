// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectCheckMemberGroupsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectCheckMemberGroupsRequest : 
    BaseRequest,
    IDirectoryObjectCheckMemberGroupsRequest,
    IBaseRequest
  {
    public DirectoryObjectCheckMemberGroupsRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new DirectoryObjectCheckMemberGroupsRequestBody();
    }

    public DirectoryObjectCheckMemberGroupsRequestBody RequestBody { get; private set; }

    public Task<IDirectoryObjectCheckMemberGroupsCollectionPage> PostAsync()
    {
      return this.PostAsync(CancellationToken.None);
    }

    public async Task<IDirectoryObjectCheckMemberGroupsCollectionPage> PostAsync(
      CancellationToken cancellationToken)
    {
      DirectoryObjectCheckMemberGroupsCollectionResponse collectionResponse = await this.SendAsync<DirectoryObjectCheckMemberGroupsCollectionResponse>((object) this.RequestBody, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDirectoryObjectCheckMemberGroupsCollectionPage) null;
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

    public IDirectoryObjectCheckMemberGroupsRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }

    public IDirectoryObjectCheckMemberGroupsRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }

    public IDirectoryObjectCheckMemberGroupsRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }

    public IDirectoryObjectCheckMemberGroupsRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }

    public IDirectoryObjectCheckMemberGroupsRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }

    public IDirectoryObjectCheckMemberGroupsRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDirectoryObjectCheckMemberGroupsRequest) this;
    }
  }
}
