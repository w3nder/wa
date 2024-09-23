// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IDriveItemsCollectionRequestBuilder
  {
    public DriveItemsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveItemsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDriveItemsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IDriveItemsCollectionRequest) new DriveItemsCollectionRequest(this.RequestUrl, this.Client, options);
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
