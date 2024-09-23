// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageExtensionsCollectionRequest : 
    BaseRequest,
    IMessageExtensionsCollectionRequest,
    IBaseRequest
  {
    public MessageExtensionsCollectionRequest(
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

    public Task<IMessageExtensionsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IMessageExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      MessageExtensionsCollectionResponse collectionResponse = await this.SendAsync<MessageExtensionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IMessageExtensionsCollectionPage) null;
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

    public IMessageExtensionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMessageExtensionsCollectionRequest) this;
    }

    public IMessageExtensionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMessageExtensionsCollectionRequest) this;
    }

    public IMessageExtensionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IMessageExtensionsCollectionRequest) this;
    }

    public IMessageExtensionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IMessageExtensionsCollectionRequest) this;
    }

    public IMessageExtensionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IMessageExtensionsCollectionRequest) this;
    }

    public IMessageExtensionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IMessageExtensionsCollectionRequest) this;
    }
  }
}
