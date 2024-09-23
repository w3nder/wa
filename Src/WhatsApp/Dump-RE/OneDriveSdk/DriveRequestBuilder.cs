// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.DriveRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class DriveRequestBuilder : BaseRequestBuilder, IDriveRequestBuilder, IBaseRequestBuilder
  {
    public IItemRequestBuilder Root
    {
      get
      {
        return (IItemRequestBuilder) new ItemRequestBuilder(this.AppendSegmentToRequestUrl("root"), this.Client);
      }
    }

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

    public IDriveSharedCollectionRequestBuilder Shared
    {
      get
      {
        return (IDriveSharedCollectionRequestBuilder) new DriveSharedCollectionRequestBuilder(this.AppendSegmentToRequestUrl("shared"), this.Client);
      }
    }

    public IDriveSpecialCollectionRequestBuilder Special
    {
      get
      {
        return (IDriveSpecialCollectionRequestBuilder) new DriveSpecialCollectionRequestBuilder(this.AppendSegmentToRequestUrl("special"), this.Client);
      }
    }

    public IDriveRecentRequestBuilder Recent()
    {
      return (IDriveRecentRequestBuilder) new DriveRecentRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.recent"), this.Client);
    }
  }
}
