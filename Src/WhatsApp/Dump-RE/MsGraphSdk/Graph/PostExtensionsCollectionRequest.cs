// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PostExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class PostExtensionsCollectionRequest : 
    BaseRequest,
    IPostExtensionsCollectionRequest,
    IBaseRequest
  {
    public PostExtensionsCollectionRequest(
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

    public Task<IPostExtensionsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IPostExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      PostExtensionsCollectionResponse collectionResponse = await this.SendAsync<PostExtensionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IPostExtensionsCollectionPage) null;
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

    public IPostExtensionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IPostExtensionsCollectionRequest) this;
    }

    public IPostExtensionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IPostExtensionsCollectionRequest) this;
    }

    public IPostExtensionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IPostExtensionsCollectionRequest) this;
    }

    public IPostExtensionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IPostExtensionsCollectionRequest) this;
    }

    public IPostExtensionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IPostExtensionsCollectionRequest) this;
    }

    public IPostExtensionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IPostExtensionsCollectionRequest) this;
    }
  }
}
