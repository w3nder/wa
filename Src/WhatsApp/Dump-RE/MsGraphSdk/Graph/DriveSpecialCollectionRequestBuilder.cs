// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveSpecialCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveSpecialCollectionRequestBuilder : 
    BaseRequestBuilder,
    IDriveSpecialCollectionRequestBuilder
  {
    public IDriveItemRequestBuilder AppRoot
    {
      get
      {
        return (IDriveItemRequestBuilder) new DriveItemRequestBuilder(this.AppendSegmentToRequestUrl("approot"), this.Client);
      }
    }

    public DriveSpecialCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveSpecialCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDriveSpecialCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IDriveSpecialCollectionRequest) new DriveSpecialCollectionRequest(this.RequestUrl, this.Client, options);
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
