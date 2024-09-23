// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceOrganizationCollectionPage
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (InterfaceConverter<GraphServiceOrganizationCollectionPage>))]
  public interface IGraphServiceOrganizationCollectionPage : 
    ICollectionPage<Organization>,
    IList<Organization>,
    ICollection<Organization>,
    IEnumerable<Organization>,
    IEnumerable
  {
    IGraphServiceOrganizationCollectionRequest NextPageRequest { get; }

    void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString);
  }
}
