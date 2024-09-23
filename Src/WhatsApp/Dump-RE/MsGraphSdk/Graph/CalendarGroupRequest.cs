// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarGroupRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarGroupRequest : BaseRequest, ICalendarGroupRequest, IBaseRequest
  {
    public CalendarGroupRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<CalendarGroup> CreateAsync(CalendarGroup calendarGroupToCreate)
    {
      return this.CreateAsync(calendarGroupToCreate, CancellationToken.None);
    }

    public async Task<CalendarGroup> CreateAsync(
      CalendarGroup calendarGroupToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      CalendarGroup calendarGroupToInitialize = await this.SendAsync<CalendarGroup>((object) calendarGroupToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarGroupToInitialize);
      return calendarGroupToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      CalendarGroup calendarGroup = await this.SendAsync<CalendarGroup>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<CalendarGroup> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<CalendarGroup> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      CalendarGroup calendarGroupToInitialize = await this.SendAsync<CalendarGroup>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarGroupToInitialize);
      return calendarGroupToInitialize;
    }

    public Task<CalendarGroup> UpdateAsync(CalendarGroup calendarGroupToUpdate)
    {
      return this.UpdateAsync(calendarGroupToUpdate, CancellationToken.None);
    }

    public async Task<CalendarGroup> UpdateAsync(
      CalendarGroup calendarGroupToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      CalendarGroup calendarGroupToInitialize = await this.SendAsync<CalendarGroup>((object) calendarGroupToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarGroupToInitialize);
      return calendarGroupToInitialize;
    }

    public ICalendarGroupRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (ICalendarGroupRequest) this;
    }

    public ICalendarGroupRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (ICalendarGroupRequest) this;
    }

    private void InitializeCollectionProperties(CalendarGroup calendarGroupToInitialize)
    {
      if (calendarGroupToInitialize == null || calendarGroupToInitialize.AdditionalData == null || calendarGroupToInitialize.Calendars == null || calendarGroupToInitialize.Calendars.CurrentPage == null)
        return;
      calendarGroupToInitialize.Calendars.AdditionalData = calendarGroupToInitialize.AdditionalData;
      object obj;
      calendarGroupToInitialize.AdditionalData.TryGetValue("calendars@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      calendarGroupToInitialize.Calendars.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
