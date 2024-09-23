// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarCalendarViewCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarCalendarViewCollectionRequest : 
    BaseRequest,
    ICalendarCalendarViewCollectionRequest,
    IBaseRequest
  {
    public CalendarCalendarViewCollectionRequest(
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

    public Task<ICalendarCalendarViewCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<ICalendarCalendarViewCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      CalendarCalendarViewCollectionResponse collectionResponse = await this.SendAsync<CalendarCalendarViewCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (ICalendarCalendarViewCollectionPage) null;
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

    public ICalendarCalendarViewCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (ICalendarCalendarViewCollectionRequest) this;
    }

    public ICalendarCalendarViewCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (ICalendarCalendarViewCollectionRequest) this;
    }

    public ICalendarCalendarViewCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (ICalendarCalendarViewCollectionRequest) this;
    }

    public ICalendarCalendarViewCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (ICalendarCalendarViewCollectionRequest) this;
    }

    public ICalendarCalendarViewCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (ICalendarCalendarViewCollectionRequest) this;
    }

    public ICalendarCalendarViewCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (ICalendarCalendarViewCollectionRequest) this;
    }
  }
}
