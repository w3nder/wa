// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarViewCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarViewCollectionRequest : 
    BaseRequest,
    IUserCalendarViewCollectionRequest,
    IBaseRequest
  {
    public UserCalendarViewCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Event> AddAsync(Event calendarViewEvent)
    {
      return this.AddAsync(calendarViewEvent, CancellationToken.None);
    }

    public Task<Event> AddAsync(Event calendarViewEvent, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<Event>((object) calendarViewEvent, cancellationToken);
    }

    public Task<IUserCalendarViewCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IUserCalendarViewCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserCalendarViewCollectionResponse collectionResponse = await this.SendAsync<UserCalendarViewCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserCalendarViewCollectionPage) null;
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

    public IUserCalendarViewCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserCalendarViewCollectionRequest) this;
    }

    public IUserCalendarViewCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserCalendarViewCollectionRequest) this;
    }

    public IUserCalendarViewCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserCalendarViewCollectionRequest) this;
    }

    public IUserCalendarViewCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserCalendarViewCollectionRequest) this;
    }

    public IUserCalendarViewCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserCalendarViewCollectionRequest) this;
    }

    public IUserCalendarViewCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserCalendarViewCollectionRequest) this;
    }
  }
}
