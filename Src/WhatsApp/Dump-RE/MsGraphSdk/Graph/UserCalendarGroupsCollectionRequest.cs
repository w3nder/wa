// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarGroupsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarGroupsCollectionRequest : 
    BaseRequest,
    IUserCalendarGroupsCollectionRequest,
    IBaseRequest
  {
    public UserCalendarGroupsCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<CalendarGroup> AddAsync(CalendarGroup calendarGroup)
    {
      return this.AddAsync(calendarGroup, CancellationToken.None);
    }

    public Task<CalendarGroup> AddAsync(
      CalendarGroup calendarGroup,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<CalendarGroup>((object) calendarGroup, cancellationToken);
    }

    public Task<IUserCalendarGroupsCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IUserCalendarGroupsCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserCalendarGroupsCollectionResponse collectionResponse = await this.SendAsync<UserCalendarGroupsCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserCalendarGroupsCollectionPage) null;
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

    public IUserCalendarGroupsCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserCalendarGroupsCollectionRequest) this;
    }

    public IUserCalendarGroupsCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserCalendarGroupsCollectionRequest) this;
    }

    public IUserCalendarGroupsCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserCalendarGroupsCollectionRequest) this;
    }

    public IUserCalendarGroupsCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserCalendarGroupsCollectionRequest) this;
    }

    public IUserCalendarGroupsCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserCalendarGroupsCollectionRequest) this;
    }

    public IUserCalendarGroupsCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserCalendarGroupsCollectionRequest) this;
    }
  }
}
