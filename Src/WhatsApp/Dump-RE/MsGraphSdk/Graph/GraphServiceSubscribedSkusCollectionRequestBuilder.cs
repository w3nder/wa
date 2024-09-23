// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceSubscribedSkusCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceSubscribedSkusCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceSubscribedSkusCollectionRequestBuilder
  {
    public GraphServiceSubscribedSkusCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceSubscribedSkusCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceSubscribedSkusCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceSubscribedSkusCollectionRequest) new GraphServiceSubscribedSkusCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public ISubscribedSkuRequestBuilder this[string id]
    {
      get
      {
        return (ISubscribedSkuRequestBuilder) new SubscribedSkuRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
