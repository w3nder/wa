// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupRejectedSendersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupRejectedSendersCollectionRequest : 
    BaseRequest,
    IGroupRejectedSendersCollectionRequest,
    IBaseRequest
  {
    public GroupRejectedSendersCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DirectoryObject> AddAsync(DirectoryObject directoryObject)
    {
      return this.AddAsync(directoryObject, CancellationToken.None);
    }

    public Task<DirectoryObject> AddAsync(
      DirectoryObject directoryObject,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<DirectoryObject>((object) directoryObject, cancellationToken);
    }

    public Task<IGroupRejectedSendersCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGroupRejectedSendersCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GroupRejectedSendersCollectionResponse collectionResponse = await this.SendAsync<GroupRejectedSendersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGroupRejectedSendersCollectionPage) null;
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

    public IGroupRejectedSendersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGroupRejectedSendersCollectionRequest) this;
    }

    public IGroupRejectedSendersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGroupRejectedSendersCollectionRequest) this;
    }
  }
}
