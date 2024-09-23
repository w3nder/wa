// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EventExtensionsCollectionRequest : 
    BaseRequest,
    IEventExtensionsCollectionRequest,
    IBaseRequest
  {
    public EventExtensionsCollectionRequest(
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

    public Task<IEventExtensionsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IEventExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      EventExtensionsCollectionResponse collectionResponse = await this.SendAsync<EventExtensionsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IEventExtensionsCollectionPage) null;
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

    public IEventExtensionsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEventExtensionsCollectionRequest) this;
    }

    public IEventExtensionsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEventExtensionsCollectionRequest) this;
    }

    public IEventExtensionsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IEventExtensionsCollectionRequest) this;
    }

    public IEventExtensionsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IEventExtensionsCollectionRequest) this;
    }

    public IEventExtensionsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IEventExtensionsCollectionRequest) this;
    }

    public IEventExtensionsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IEventExtensionsCollectionRequest) this;
    }
  }
}
