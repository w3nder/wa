// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceOrganizationCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceOrganizationCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceOrganizationCollectionRequestBuilder
  {
    public GraphServiceOrganizationCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceOrganizationCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceOrganizationCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceOrganizationCollectionRequest) new GraphServiceOrganizationCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IOrganizationRequestBuilder this[string id]
    {
      get
      {
        return (IOrganizationRequestBuilder) new OrganizationRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
