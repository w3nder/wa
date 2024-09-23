// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventInstancesCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventInstancesCollectionPage : 
    CollectionPage<Event>,
    IEventInstancesCollectionPage,
    ICollectionPage<Event>,
    IList<Event>,
    ICollection<Event>,
    IEnumerable<Event>,
    IEnumerable
  {
    public IEventInstancesCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IEventInstancesCollectionRequest) new EventInstancesCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
