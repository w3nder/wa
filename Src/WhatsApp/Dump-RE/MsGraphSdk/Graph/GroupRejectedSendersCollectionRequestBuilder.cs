// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupRejectedSendersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupRejectedSendersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGroupRejectedSendersCollectionRequestBuilder
  {
    public GroupRejectedSendersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGroupRejectedSendersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGroupRejectedSendersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGroupRejectedSendersCollectionRequest) new GroupRejectedSendersCollectionRequest(this.RequestUrl, this.Client, options);
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
