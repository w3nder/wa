// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactRequest : BaseRequest, IContactRequest, IBaseRequest
  {
    public ContactRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Contact> CreateAsync(Contact contactToCreate)
    {
      return this.CreateAsync(contactToCreate, CancellationToken.None);
    }

    public async Task<Contact> CreateAsync(
      Contact contactToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Contact contactToInitialize = await this.SendAsync<Contact>((object) contactToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactToInitialize);
      return contactToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Contact contact = await this.SendAsync<Contact>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Contact> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Contact> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Contact contactToInitialize = await this.SendAsync<Contact>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactToInitialize);
      return contactToInitialize;
    }

    public Task<Contact> UpdateAsync(Contact contactToUpdate)
    {
      return this.UpdateAsync(contactToUpdate, CancellationToken.None);
    }

    public async Task<Contact> UpdateAsync(
      Contact contactToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Contact contactToInitialize = await this.SendAsync<Contact>((object) contactToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactToInitialize);
      return contactToInitialize;
    }

    public IContactRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IContactRequest) this;
    }

    public IContactRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IContactRequest) this;
    }

    private void InitializeCollectionProperties(Contact contactToInitialize)
    {
      if (contactToInitialize == null || contactToInitialize.AdditionalData == null || contactToInitialize.Extensions == null || contactToInitialize.Extensions.CurrentPage == null)
        return;
      contactToInitialize.Extensions.AdditionalData = contactToInitialize.AdditionalData;
      object obj;
      contactToInitialize.AdditionalData.TryGetValue("extensions@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      contactToInitialize.Extensions.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
