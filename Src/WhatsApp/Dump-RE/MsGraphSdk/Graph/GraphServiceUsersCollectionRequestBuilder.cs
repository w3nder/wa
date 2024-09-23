// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceUsersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceUsersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGraphServiceUsersCollectionRequestBuilder
  {
    public GraphServiceUsersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGraphServiceUsersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGraphServiceUsersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGraphServiceUsersCollectionRequest) new GraphServiceUsersCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IUserRequestBuilder this[string id]
    {
      get
      {
        return (IUserRequestBuilder) new UserRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
