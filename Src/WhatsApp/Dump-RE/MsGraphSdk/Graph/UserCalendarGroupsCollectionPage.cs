// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarGroupsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarGroupsCollectionPage : 
    CollectionPage<CalendarGroup>,
    IUserCalendarGroupsCollectionPage,
    ICollectionPage<CalendarGroup>,
    IList<CalendarGroup>,
    ICollection<CalendarGroup>,
    IEnumerable<CalendarGroup>,
    IEnumerable
  {
    public IUserCalendarGroupsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IUserCalendarGroupsCollectionRequest) new UserCalendarGroupsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
