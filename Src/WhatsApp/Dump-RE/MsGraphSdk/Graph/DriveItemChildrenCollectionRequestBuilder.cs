// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemChildrenCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemChildrenCollectionRequestBuilder : 
    BaseRequestBuilder,
    IDriveItemChildrenCollectionRequestBuilder
  {
    public DriveItemChildrenCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveItemChildrenCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDriveItemChildrenCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IDriveItemChildrenCollectionRequest) new DriveItemChildrenCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDriveItemRequestBuilder this[string id]
    {
      get
      {
        return (IDriveItemRequestBuilder) new DriveItemRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
