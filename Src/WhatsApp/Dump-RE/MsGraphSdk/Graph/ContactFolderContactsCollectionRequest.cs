// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderContactsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderContactsCollectionRequest : 
    BaseRequest,
    IContactFolderContactsCollectionRequest,
    IBaseRequest
  {
    public ContactFolderContactsCollectionRequest(
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

    public Task<IContactFolderContactsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IContactFolderContactsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ContactFolderContactsCollectionResponse collectionResponse = await this.SendAsync<ContactFolderContactsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IContactFolderContactsCollectionPage) null;
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

    public IContactFolderContactsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IContactFolderContactsCollectionRequest) this;
    }

    public IContactFolderContactsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IContactFolderContactsCollectionRequest) this;
    }

    public IContactFolderContactsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IContactFolderContactsCollectionRequest) this;
    }

    public IContactFolderContactsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IContactFolderContactsCollectionRequest) this;
    }

    public IContactFolderContactsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IContactFolderContactsCollectionRequest) this;
    }

    public IContactFolderContactsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IContactFolderContactsCollectionRequest) this;
    }
  }
}
