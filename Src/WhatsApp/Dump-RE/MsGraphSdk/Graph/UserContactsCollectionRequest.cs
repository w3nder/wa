// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserContactsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserContactsCollectionRequest : 
    BaseRequest,
    IUserContactsCollectionRequest,
    IBaseRequest
  {
    public UserContactsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Contact> AddAsync(Contact contact)
    {
      return this.AddAsync(contact, CancellationToken.None);
    }

    public Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Contact>((object) contact, cancellationToken);
    }

    public Task<IUserContactsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IUserContactsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserContactsCollectionResponse collectionResponse = await this.SendAsync<UserContactsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserContactsCollectionPage) null;
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

    public IUserContactsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserContactsCollectionRequest) this;
    }

    public IUserContactsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserContactsCollectionRequest) this;
    }

    public IUserContactsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserContactsCollectionRequest) this;
    }

    public IUserContactsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserContactsCollectionRequest) this;
    }

    public IUserContactsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserContactsCollectionRequest) this;
    }

    public IUserContactsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserContactsCollectionRequest) this;
    }
  }
}
