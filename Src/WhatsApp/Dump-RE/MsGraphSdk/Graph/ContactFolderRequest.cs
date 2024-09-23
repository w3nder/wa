// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderRequest : BaseRequest, IContactFolderRequest, IBaseRequest
  {
    public ContactFolderRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ContactFolder> CreateAsync(ContactFolder contactFolderToCreate)
    {
      return this.CreateAsync(contactFolderToCreate, CancellationToken.None);
    }

    public async Task<ContactFolder> CreateAsync(
      ContactFolder contactFolderToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ContactFolder contactFolderToInitialize = await this.SendAsync<ContactFolder>((object) contactFolderToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactFolderToInitialize);
      return contactFolderToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ContactFolder contactFolder = await this.SendAsync<ContactFolder>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ContactFolder> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ContactFolder> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ContactFolder contactFolderToInitialize = await this.SendAsync<ContactFolder>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactFolderToInitialize);
      return contactFolderToInitialize;
    }

    public Task<ContactFolder> UpdateAsync(ContactFolder contactFolderToUpdate)
    {
      return this.UpdateAsync(contactFolderToUpdate, CancellationToken.None);
    }

    public async Task<ContactFolder> UpdateAsync(
      ContactFolder contactFolderToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ContactFolder contactFolderToInitialize = await this.SendAsync<ContactFolder>((object) contactFolderToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(contactFolderToInitialize);
      return contactFolderToInitialize;
    }

    public IContactFolderRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IContactFolderRequest) this;
    }

    public IContactFolderRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IContactFolderRequest) this;
    }

    private void InitializeCollectionProperties(ContactFolder contactFolderToInitialize)
    {
      if (contactFolderToInitialize == null || contactFolderToInitialize.AdditionalData == null)
        return;
      if (contactFolderToInitialize.Contacts != null && contactFolderToInitialize.Contacts.CurrentPage != null)
      {
        contactFolderToInitialize.Contacts.AdditionalData = contactFolderToInitialize.AdditionalData;
        object obj;
        contactFolderToInitialize.AdditionalData.TryGetValue("contacts@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          contactFolderToInitialize.Contacts.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (contactFolderToInitialize.ChildFolders == null || contactFolderToInitialize.ChildFolders.CurrentPage == null)
        return;
      contactFolderToInitialize.ChildFolders.AdditionalData = contactFolderToInitialize.AdditionalData;
      object obj1;
      contactFolderToInitialize.AdditionalData.TryGetValue("childFolders@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      contactFolderToInitialize.ChildFolders.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
