// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCreateSessionRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemCreateSessionRequestBuilder : 
    BasePostMethodRequestBuilder<IItemCreateSessionRequest>,
    IItemCreateSessionRequestBuilder
  {
    public ItemCreateSessionRequestBuilder(
      string requestUrl,
      IBaseClient client,
      ChunkedUploadSessionDescriptor item)
      : base(requestUrl, client)
    {
      this.SetParameter<ChunkedUploadSessionDescriptor>(nameof (item), item, true);
    }

    protected override IItemCreateSessionRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      ItemCreateSessionRequest request = new ItemCreateSessionRequest(functionUrl, this.Client, options);
      if (this.HasParameter("item"))
        request.RequestBody.Item = this.GetParameter<ChunkedUploadSessionDescriptor>("item");
      return (IItemCreateSessionRequest) request;
    }
  }
}
