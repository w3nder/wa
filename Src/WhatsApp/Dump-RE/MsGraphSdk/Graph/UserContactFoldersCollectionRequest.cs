// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserContactFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserContactFoldersCollectionRequest : 
    BaseRequest,
    IUserContactFoldersCollectionRequest,
    IBaseRequest
  {
    public UserContactFoldersCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ContactFolder> AddAsync(ContactFolder contactFolder)
    {
      return this.AddAsync(contactFolder, CancellationToken.None);
    }

    public Task<ContactFolder> AddAsync(
      ContactFolder contactFolder,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<ContactFolder>((object) contactFolder, cancellationToken);
    }

    public Task<IUserContactFoldersCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IUserContactFoldersCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserContactFoldersCollectionResponse collectionResponse = await this.SendAsync<UserContactFoldersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserContactFoldersCollectionPage) null;
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

    public IUserContactFoldersCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserContactFoldersCollectionRequest) this;
    }

    public IUserContactFoldersCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserContactFoldersCollectionRequest) this;
    }

    public IUserContactFoldersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserContactFoldersCollectionRequest) this;
    }

    public IUserContactFoldersCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserContactFoldersCollectionRequest) this;
    }

    public IUserContactFoldersCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserContactFoldersCollectionRequest) this;
    }

    public IUserContactFoldersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserContactFoldersCollectionRequest) this;
    }
  }
}
