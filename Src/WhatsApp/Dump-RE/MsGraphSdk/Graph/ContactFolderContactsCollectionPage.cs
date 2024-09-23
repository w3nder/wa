// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderContactsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderContactsCollectionPage : 
    CollectionPage<Contact>,
    IContactFolderContactsCollectionPage,
    ICollectionPage<Contact>,
    IList<Contact>,
    ICollection<Contact>,
    IEnumerable<Contact>,
    IEnumerable
  {
    public IContactFolderContactsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IContactFolderContactsCollectionRequest) new ContactFolderContactsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
