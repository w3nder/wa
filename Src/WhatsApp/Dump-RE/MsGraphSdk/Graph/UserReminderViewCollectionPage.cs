// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserReminderViewCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserReminderViewCollectionPage : 
    CollectionPage<Reminder>,
    IUserReminderViewCollectionPage,
    ICollectionPage<Reminder>,
    IList<Reminder>,
    ICollection<Reminder>,
    IEnumerable<Reminder>,
    IEnumerable
  {
    public IUserReminderViewRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IUserReminderViewRequest) new UserReminderViewRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
