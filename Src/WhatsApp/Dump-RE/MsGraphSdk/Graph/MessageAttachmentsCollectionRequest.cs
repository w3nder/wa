// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageAttachmentsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageAttachmentsCollectionRequest : 
    BaseRequest,
    IMessageAttachmentsCollectionRequest,
    IBaseRequest
  {
    public MessageAttachmentsCollectionRequest(
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

    public Task<IMessageAttachmentsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IMessageAttachmentsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      MessageAttachmentsCollectionResponse collectionResponse = await this.SendAsync<MessageAttachmentsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IMessageAttachmentsCollectionPage) null;
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

    public IMessageAttachmentsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMessageAttachmentsCollectionRequest) this;
    }

    public IMessageAttachmentsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMessageAttachmentsCollectionRequest) this;
    }

    public IMessageAttachmentsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IMessageAttachmentsCollectionRequest) this;
    }

    public IMessageAttachmentsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IMessageAttachmentsCollectionRequest) this;
    }

    public IMessageAttachmentsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IMessageAttachmentsCollectionRequest) this;
    }

    public IMessageAttachmentsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IMessageAttachmentsCollectionRequest) this;
    }
  }
}
