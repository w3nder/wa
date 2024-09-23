// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupAcceptedSendersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupAcceptedSendersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IGroupAcceptedSendersCollectionRequestBuilder
  {
    public GroupAcceptedSendersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGroupAcceptedSendersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGroupAcceptedSendersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IGroupAcceptedSendersCollectionRequest) new GroupAcceptedSendersCollectionRequest(this.RequestUrl, this.Client, options);
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
