﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupRejectedSendersCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (InterfaceConverter<GroupRejectedSendersCollectionPage>))]
  public interface IGroupRejectedSendersCollectionPage : 
    ICollectionPage<DirectoryObject>,
    IList<DirectoryObject>,
    ICollection<DirectoryObject>,
    IEnumerable<DirectoryObject>,
    IEnumerable
  {
    IGroupRejectedSendersCollectionRequest NextPageRequest { get; }

    void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString);
  }
}
