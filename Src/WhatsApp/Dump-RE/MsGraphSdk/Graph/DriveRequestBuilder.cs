// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveRequestBuilder : 
    EntityRequestBuilder,
    IDriveRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public DriveRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDriveRequest Request(IEnumerable<Option> options)
    {
      return (IDriveRequest) new DriveRequest(this.RequestUrl, this.Client, options);
    }

    public IDriveItemsCollectionRequestBuilder Items
    {
      get
      {
        return (IDriveItemsCollectionRequestBuilder) new DriveItemsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("items"), this.Client);
      }
    }

    public IDriveSpecialCollectionRequestBuilder Special
    {
      get
      {
        return (IDriveSpecialCollectionRequestBuilder) new DriveSpecialCollectionRequestBuilder(this.AppendSegmentToRequestUrl("special"), this.Client);
      }
    }

    public IDriveItemRequestBuilder Root
    {
      get
      {
        return (IDriveItemRequestBuilder) new DriveItemRequestBuilder(this.AppendSegmentToRequestUrl("root"), this.Client);
      }
    }

    public IDriveRecentRequestBuilder Recent()
    {
      return (IDriveRecentRequestBuilder) new DriveRecentRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.recent"), this.Client);
    }

    public IDriveSharedWithMeRequestBuilder SharedWithMe()
    {
      return (IDriveSharedWithMeRequestBuilder) new DriveSharedWithMeRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.sharedWithMe"), this.Client);
    }
  }
}
