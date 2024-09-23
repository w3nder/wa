// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DeviceRegisteredUsersCollectionWithReferencesPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DeviceRegisteredUsersCollectionWithReferencesPage : 
    CollectionPage<DirectoryObject>,
    IDeviceRegisteredUsersCollectionWithReferencesPage,
    ICollectionPage<DirectoryObject>,
    IList<DirectoryObject>,
    ICollection<DirectoryObject>,
    IEnumerable<DirectoryObject>,
    IEnumerable
  {
    public IDeviceRegisteredUsersCollectionWithReferencesRequest NextPageRequest { get; private set; }

    public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
    {
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      this.NextPageRequest = (IDeviceRegisteredUsersCollectionWithReferencesRequest) new DeviceRegisteredUsersCollectionWithReferencesRequest(nextPageLinkString, client, (IEnumerable<Option>) null);
    }
  }
}
