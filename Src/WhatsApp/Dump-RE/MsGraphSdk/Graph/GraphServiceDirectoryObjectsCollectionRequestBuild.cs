// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDirectoryObjectsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDirectoryObjectsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceDirectoryObjectsCollectionRequestBuilder
  {
    public GraphServiceDirectoryObjectsCollectionRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceDirectoryObjectsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceDirectoryObjectsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceDirectoryObjectsCollectionRequest) new GraphServiceDirectoryObjectsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectRequestBuilder) new DirectoryObjectRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
