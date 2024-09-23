// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventAttachmentsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class EventAttachmentsCollectionRequest : 
    BaseRequest,
    IEventAttachmentsCollectionRequest,
    IBaseRequest
  {
    public EventAttachmentsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Attachment> AddAsync(Attachment attachment)
    {
      return this.AddAsync(attachment, CancellationToken.None);
    }

    public Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      attachment.ODataType = "#" + StringHelper.ConvertTypeToLowerCamelCase(attachment.GetType().FullName);
      return this.SendAsync<Attachment>((object) attachment, cancellationToken);
    }

    public Task<IEventAttachmentsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IEventAttachmentsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      EventAttachmentsCollectionResponse collectionResponse = await this.SendAsync<EventAttachmentsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IEventAttachmentsCollectionPage) null;
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

    public IEventAttachmentsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IEventAttachmentsCollectionRequest) this;
    }

    public IEventAttachmentsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IEventAttachmentsCollectionRequest) this;
    }

    public IEventAttachmentsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IEventAttachmentsCollectionRequest) this;
    }

    public IEventAttachmentsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IEventAttachmentsCollectionRequest) this;
    }

    public IEventAttachmentsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IEventAttachmentsCollectionRequest) this;
    }

    public IEventAttachmentsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IEventAttachmentsCollectionRequest) this;
    }
  }
}
