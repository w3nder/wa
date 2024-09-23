// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceDirectoryRoleTemplatesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceDirectoryRoleTemplatesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceDirectoryRoleTemplatesCollectionRequestBuilder
  {
    public GraphServiceDirectoryRoleTemplatesCollectionRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceDirectoryRoleTemplatesCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceDirectoryRoleTemplatesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceDirectoryRoleTemplatesCollectionRequest) new GraphServiceDirectoryRoleTemplatesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryRoleTemplateRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryRoleTemplateRequestBuilder) new DirectoryRoleTemplateRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
