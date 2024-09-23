// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupAcceptedSendersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupAcceptedSendersCollectionRequest : 
    BaseRequest,
    IGroupAcceptedSendersCollectionRequest,
    IBaseRequest
  {
    public GroupAcceptedSendersCollectionRequest(
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

    public Task<IGroupAcceptedSendersCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IGroupAcceptedSendersCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      GroupAcceptedSendersCollectionResponse collectionResponse = await this.SendAsync<GroupAcceptedSendersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IGroupAcceptedSendersCollectionPage) null;
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

    public IGroupAcceptedSendersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IGroupAcceptedSendersCollectionRequest) this;
    }

    public IGroupAcceptedSendersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IGroupAcceptedSendersCollectionRequest) this;
    }
  }
}
