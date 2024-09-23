// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationThreadRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationThreadRequest : BaseRequest, IConversationThreadRequest, IBaseRequest
  {
    public ConversationThreadRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ConversationThread> CreateAsync(ConversationThread conversationThreadToCreate)
    {
      return this.CreateAsync(conversationThreadToCreate, CancellationToken.None);
    }

    public async Task<ConversationThread> CreateAsync(
      ConversationThread conversationThreadToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ConversationThread conversationThreadToInitialize = await this.SendAsync<ConversationThread>((object) conversationThreadToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationThreadToInitialize);
      return conversationThreadToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ConversationThread conversationThread = await this.SendAsync<ConversationThread>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ConversationThread> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ConversationThread> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ConversationThread conversationThreadToInitialize = await this.SendAsync<ConversationThread>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationThreadToInitialize);
      return conversationThreadToInitialize;
    }

    public Task<ConversationThread> UpdateAsync(ConversationThread conversationThreadToUpdate)
    {
      return this.UpdateAsync(conversationThreadToUpdate, CancellationToken.None);
    }

    public async Task<ConversationThread> UpdateAsync(
      ConversationThread conversationThreadToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ConversationThread conversationThreadToInitialize = await this.SendAsync<ConversationThread>((object) conversationThreadToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationThreadToInitialize);
      return conversationThreadToInitialize;
    }

    public IConversationThreadRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IConversationThreadRequest) this;
    }

    public IConversationThreadRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IConversationThreadRequest) this;
    }

    private void InitializeCollectionProperties(ConversationThread conversationThreadToInitialize)
    {
      if (conversationThreadToInitialize == null || conversationThreadToInitialize.AdditionalData == null || conversationThreadToInitialize.Posts == null || conversationThreadToInitialize.Posts.CurrentPage == null)
        return;
      conversationThreadToInitialize.Posts.AdditionalData = conversationThreadToInitialize.AdditionalData;
      object obj;
      conversationThreadToInitialize.AdditionalData.TryGetValue("posts@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      conversationThreadToInitialize.Posts.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
