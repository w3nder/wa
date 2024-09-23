// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCalendarsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCalendarsCollectionPage : 
    CollectionPage<Calendar>,
    IUserCalendarsCollectionPage,
    ICollectionPage<Calendar>,
    IList<Calendar>,
    ICollection<Calendar>,
    IEnumerable<Calendar>,
    IEnumerable
  {
    public IUserCalendarsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IUserCalendarsCollectionRequest) new UserCalendarsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
