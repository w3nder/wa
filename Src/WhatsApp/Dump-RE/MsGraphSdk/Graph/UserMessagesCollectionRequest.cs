// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMessagesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMessagesCollectionRequest : 
    BaseRequest,
    IUserMessagesCollectionRequest,
    IBaseRequest
  {
    public UserMessagesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Message> AddAsync(Message message)
    {
      return this.AddAsync(message, CancellationToken.None);
    }

    public Task<Message> AddAsync(Message message, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Message>((object) message, cancellationToken);
    }

    public Task<IUserMessagesCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IUserMessagesCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserMessagesCollectionResponse collectionResponse = await this.SendAsync<UserMessagesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserMessagesCollectionPage) null;
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

    public IUserMessagesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserMessagesCollectionRequest) this;
    }

    public IUserMessagesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserMessagesCollectionRequest) this;
    }

    public IUserMessagesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserMessagesCollectionRequest) this;
    }

    public IUserMessagesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserMessagesCollectionRequest) this;
    }

    public IUserMessagesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserMessagesCollectionRequest) this;
    }

    public IUserMessagesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserMessagesCollectionRequest) this;
    }
  }
}
