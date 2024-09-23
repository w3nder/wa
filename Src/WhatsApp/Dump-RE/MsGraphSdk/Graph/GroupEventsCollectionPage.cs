// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupEventsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupEventsCollectionPage : 
    CollectionPage<Event>,
    IGroupEventsCollectionPage,
    ICollectionPage<Event>,
    IList<Event>,
    ICollection<Event>,
    IEnumerable<Event>,
    IEnumerable
  {
    public IGroupEventsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IGroupEventsCollectionRequest) new GroupEventsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
