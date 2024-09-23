// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactExtensionsCollectionRequest : 
    BaseRequest,
    IContactExtensionsCollectionRequest,
    IBaseRequest
  {
    public ContactExtensionsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Extension> AddAsync(Extension extension)
    {
      return this.AddAsync(extension, CancellationToken.None);
    }

    public Task<Extension> AddAsync(Extension extension, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      extension.ODataType = "#" + StringHelper.ConvertTypeToLowerCamelCase(extension.GetType().FullName);
      return this.SendAsync<Extension>((object) extension, cancellationToken);
    }

    public Task<IContactExtensionsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IContactExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ContactExtensionsCollectionResponse collectionResponse = await this.SendAsync<ContactExtensionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IContactExtensionsCollectionPage) null;
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

    public IContactExtensionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IContactExtensionsCollectionRequest) this;
    }

    public IContactExtensionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IContactExtensionsCollectionRequest) this;
    }

    public IContactExtensionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IContactExtensionsCollectionRequest) this;
    }

    public IContactExtensionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IContactExtensionsCollectionRequest) this;
    }

    public IContactExtensionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IContactExtensionsCollectionRequest) this;
    }

    public IContactExtensionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IContactExtensionsCollectionRequest) this;
    }
  }
}
