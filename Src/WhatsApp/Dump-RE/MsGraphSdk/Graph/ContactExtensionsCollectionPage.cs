// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactExtensionsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactExtensionsCollectionPage : 
    CollectionPage<Extension>,
    IContactExtensionsCollectionPage,
    ICollectionPage<Extension>,
    IList<Extension>,
    ICollection<Extension>,
    IEnumerable<Extension>,
    IEnumerable
  {
    public IContactExtensionsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IContactExtensionsCollectionRequest) new ContactExtensionsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
