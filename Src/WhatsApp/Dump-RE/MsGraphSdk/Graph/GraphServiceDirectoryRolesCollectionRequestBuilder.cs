// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDirectoryRolesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDirectoryRolesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceDirectoryRolesCollectionRequestBuilder
  {
    public GraphServiceDirectoryRolesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceDirectoryRolesCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceDirectoryRolesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceDirectoryRolesCollectionRequest) new GraphServiceDirectoryRolesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryRoleRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryRoleRequestBuilder) new DirectoryRoleRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
