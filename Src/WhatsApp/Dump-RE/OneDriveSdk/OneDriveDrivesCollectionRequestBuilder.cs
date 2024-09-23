// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OneDriveDrivesCollectionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class OneDriveDrivesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IOneDriveDrivesCollectionRequestBuilder
  {
    public OneDriveDrivesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IOneDriveDrivesCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IOneDriveDrivesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IOneDriveDrivesCollectionRequest) new OneDriveDrivesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IDriveRequestBuilder this[string id]
    {
      get
      {
        return (IDriveRequestBuilder) new DriveRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
