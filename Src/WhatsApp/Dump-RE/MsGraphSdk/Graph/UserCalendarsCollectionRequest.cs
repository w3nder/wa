// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarsCollectionRequest : 
    BaseRequest,
    IUserCalendarsCollectionRequest,
    IBaseRequest
  {
    public UserCalendarsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Calendar> AddAsync(Calendar calendar)
    {
      return this.AddAsync(calendar, CancellationToken.None);
    }

    public Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Calendar>((object) calendar, cancellationToken);
    }

    public Task<IUserCalendarsCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IUserCalendarsCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserCalendarsCollectionResponse collectionResponse = await this.SendAsync<UserCalendarsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserCalendarsCollectionPage) null;
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

    public IUserCalendarsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserCalendarsCollectionRequest) this;
    }

    public IUserCalendarsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserCalendarsCollectionRequest) this;
    }

    public IUserCalendarsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserCalendarsCollectionRequest) this;
    }

    public IUserCalendarsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserCalendarsCollectionRequest) this;
    }

    public IUserCalendarsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserCalendarsCollectionRequest) this;
    }

    public IUserCalendarsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserCalendarsCollectionRequest) this;
    }
  }
}
