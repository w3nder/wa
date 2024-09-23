// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageRequest : BaseRequest, IMessageRequest, IBaseRequest
  {
    public MessageRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Message> CreateAsync(Message messageToCreate)
    {
      return this.CreateAsync(messageToCreate, CancellationToken.None);
    }

    public async Task<Message> CreateAsync(
      Message messageToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Message messageToInitialize = await this.SendAsync<Message>((object) messageToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(messageToInitialize);
      return messageToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Message message = await this.SendAsync<Message>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Message> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Message> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Message messageToInitialize = await this.SendAsync<Message>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(messageToInitialize);
      return messageToInitialize;
    }

    public Task<Message> UpdateAsync(Message messageToUpdate)
    {
      return this.UpdateAsync(messageToUpdate, CancellationToken.None);
    }

    public async Task<Message> UpdateAsync(
      Message messageToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Message messageToInitialize = await this.SendAsync<Message>((object) messageToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(messageToInitialize);
      return messageToInitialize;
    }

    public IMessageRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMessageRequest) this;
    }

    public IMessageRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMessageRequest) this;
    }

    private void InitializeCollectionProperties(Message messageToInitialize)
    {
      if (messageToInitialize == null || messageToInitialize.AdditionalData == null)
        return;
      if (messageToInitialize.Extensions != null && messageToInitialize.Extensions.CurrentPage != null)
      {
        messageToInitialize.Extensions.AdditionalData = messageToInitialize.AdditionalData;
        object obj;
        messageToInitialize.AdditionalData.TryGetValue("extensions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          messageToInitialize.Extensions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (messageToInitialize.Attachments == null || messageToInitialize.Attachments.CurrentPage == null)
        return;
      messageToInitialize.Attachments.AdditionalData = messageToInitialize.AdditionalData;
      object obj1;
      messageToInitialize.AdditionalData.TryGetValue("attachments@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      messageToInitialize.Attachments.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
