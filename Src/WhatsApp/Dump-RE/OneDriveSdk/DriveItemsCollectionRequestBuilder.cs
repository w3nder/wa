// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.DriveItemsCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
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

    public IItemRequestBuilder this[string id]
    {
      get
      {
        return (IItemRequestBuilder) new ItemRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
