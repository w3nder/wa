// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceSubscriptionsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceSubscriptionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceSubscriptionsCollectionRequestBuilder
  {
    public GraphServiceSubscriptionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceSubscriptionsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceSubscriptionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceSubscriptionsCollectionRequest) new GraphServiceSubscriptionsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public ISubscriptionRequestBuilder this[string id]
    {
      get
      {
        return (ISubscriptionRequestBuilder) new SubscriptionRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
