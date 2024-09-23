// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemSearchCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (InterfaceConverter<DriveItemSearchCollectionPage>))]
  public interface IDriveItemSearchCollectionPage : 
    ICollectionPage<DriveItem>,
    IList<DriveItem>,
    ICollection<DriveItem>,
    IEnumerable<DriveItem>,
    IEnumerable
  {
    IDriveItemSearchRequest NextPageRequest { get; }

    void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString);
  }
}
