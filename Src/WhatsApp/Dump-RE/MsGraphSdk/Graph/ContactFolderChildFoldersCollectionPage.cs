// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderChildFoldersCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderChildFoldersCollectionPage : 
    CollectionPage<ContactFolder>,
    IContactFolderChildFoldersCollectionPage,
    ICollectionPage<ContactFolder>,
    IList<ContactFolder>,
    ICollection<ContactFolder>,
    IEnumerable<ContactFolder>,
    IEnumerable
  {
    public IContactFolderChildFoldersCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IContactFolderChildFoldersCollectionRequest) new ContactFolderChildFoldersCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
