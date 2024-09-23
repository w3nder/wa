// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceGroupsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceGroupsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceGroupsCollectionRequestBuilder
  {
    public GraphServiceGroupsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceGroupsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceGroupsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceGroupsCollectionRequest) new GraphServiceGroupsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IGroupRequestBuilder this[string id]
    {
      get
      {
        return (IGroupRequestBuilder) new GroupRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
