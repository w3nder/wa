// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDevicesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDevicesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceDevicesCollectionRequestBuilder
  {
    public GraphServiceDevicesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceDevicesCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceDevicesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceDevicesCollectionRequest) new GraphServiceDevicesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDeviceRequestBuilder this[string id]
    {
      get
      {
        return (IDeviceRequestBuilder) new DeviceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
