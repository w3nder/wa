// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ConversationRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ConversationRequest : BaseRequest, IConversationRequest, IBaseRequest
  {
    public ConversationRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Conversation> CreateAsync(Conversation conversationToCreate)
    {
      return this.CreateAsync(conversationToCreate, CancellationToken.None);
    }

    public async Task<Conversation> CreateAsync(
      Conversation conversationToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Conversation conversationToInitialize = await this.SendAsync<Conversation>((object) conversationToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationToInitialize);
      return conversationToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Conversation conversation = await this.SendAsync<Conversation>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Conversation> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Conversation> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Conversation conversationToInitialize = await this.SendAsync<Conversation>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationToInitialize);
      return conversationToInitialize;
    }

    public Task<Conversation> UpdateAsync(Conversation conversationToUpdate)
    {
      return this.UpdateAsync(conversationToUpdate, CancellationToken.None);
    }

    public async Task<Conversation> UpdateAsync(
      Conversation conversationToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Conversation conversationToInitialize = await this.SendAsync<Conversation>((object) conversationToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(conversationToInitialize);
      return conversationToInitialize;
    }

    public IConversationRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IConversationRequest) this;
    }

    public IConversationRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IConversationRequest) this;
    }

    private void InitializeCollectionProperties(Conversation conversationToInitialize)
    {
      if (conversationToInitialize == null || conversationToInitialize.AdditionalData == null || conversationToInitialize.Threads == null || conversationToInitialize.Threads.CurrentPage == null)
        return;
      conversationToInitialize.Threads.AdditionalData = conversationToInitialize.AdditionalData;
      object obj;
      conversationToInitialize.AdditionalData.TryGetValue("threads@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      conversationToInitialize.Threads.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
