// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.DriveRecentRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class DriveRecentRequestBuilder : 
    BaseGetMethodRequestBuilder<IDriveRecentRequest>,
    IDriveRecentRequestBuilder
  {
    public DriveRecentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
      this.passParametersInQueryString = true;
    }

    protected override IDriveRecentRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IDriveRecentRequest) new DriveRecentRequest(functionUrl, this.Client, options);
    }
  }
}
