// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemThumbnailsCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemThumbnailsCollectionPage : 
    CollectionPage<ThumbnailSet>,
    IDriveItemThumbnailsCollectionPage,
    ICollectionPage<ThumbnailSet>,
    IList<ThumbnailSet>,
    ICollection<ThumbnailSet>,
    IEnumerable<ThumbnailSet>,
    IEnumerable
  {
    public IDriveItemThumbnailsCollectionRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IDriveItemThumbnailsCollectionRequest) new DriveItemThumbnailsCollectionRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
