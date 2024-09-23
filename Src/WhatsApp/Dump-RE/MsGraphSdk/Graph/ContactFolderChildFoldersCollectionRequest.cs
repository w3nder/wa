// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderChildFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderChildFoldersCollectionRequest : 
    BaseRequest,
    IContactFolderChildFoldersCollectionRequest,
    IBaseRequest
  {
    public ContactFolderChildFoldersCollectionRequest(
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

    public Task<IContactFolderChildFoldersCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IContactFolderChildFoldersCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ContactFolderChildFoldersCollectionResponse collectionResponse = await this.SendAsync<ContactFolderChildFoldersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IContactFolderChildFoldersCollectionPage) null;
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

    public IContactFolderChildFoldersCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }

    public IContactFolderChildFoldersCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }

    public IContactFolderChildFoldersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }

    public IContactFolderChildFoldersCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }

    public IContactFolderChildFoldersCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }

    public IContactFolderChildFoldersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IContactFolderChildFoldersCollectionRequest) this;
    }
  }
}
