// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class CalendarRequest : BaseRequest, ICalendarRequest, IBaseRequest
  {
    public CalendarRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Calendar> CreateAsync(Calendar calendarToCreate)
    {
      return this.CreateAsync(calendarToCreate, CancellationToken.None);
    }

    public async Task<Calendar> CreateAsync(
      Calendar calendarToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Calendar calendarToInitialize = await this.SendAsync<Calendar>((object) calendarToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarToInitialize);
      return calendarToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Calendar calendar = await this.SendAsync<Calendar>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Calendar> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Calendar> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Calendar calendarToInitialize = await this.SendAsync<Calendar>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarToInitialize);
      return calendarToInitialize;
    }

    public Task<Calendar> UpdateAsync(Calendar calendarToUpdate)
    {
      return this.UpdateAsync(calendarToUpdate, CancellationToken.None);
    }

    public async Task<Calendar> UpdateAsync(
      Calendar calendarToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Calendar calendarToInitialize = await this.SendAsync<Calendar>((object) calendarToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(calendarToInitialize);
      return calendarToInitialize;
    }

    public ICalendarRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (ICalendarRequest) this;
    }

    public ICalendarRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (ICalendarRequest) this;
    }

    private void InitializeCollectionProperties(Calendar calendarToInitialize)
    {
      if (calendarToInitialize == null || calendarToInitialize.AdditionalData == null)
        return;
      if (calendarToInitialize.Events != null && calendarToInitialize.Events.CurrentPage != null)
      {
        calendarToInitialize.Events.AdditionalData = calendarToInitialize.AdditionalData;
        object obj;
        calendarToInitialize.AdditionalData.TryGetValue("events@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          calendarToInitialize.Events.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (calendarToInitialize.CalendarView == null || calendarToInitialize.CalendarView.CurrentPage == null)
        return;
      calendarToInitialize.CalendarView.AdditionalData = calendarToInitialize.AdditionalData;
      object obj1;
      calendarToInitialize.AdditionalData.TryGetValue("calendarView@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      calendarToInitialize.CalendarView.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
