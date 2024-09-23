// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCreateLinkRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemCreateLinkRequestBuilder : 
    BasePostMethodRequestBuilder<IItemCreateLinkRequest>,
    IItemCreateLinkRequestBuilder
  {
    public ItemCreateLinkRequestBuilder(string requestUrl, IBaseClient client, string type)
      : base(requestUrl, client)
    {
      this.SetParameter<string>(nameof (type), type, false);
    }

    protected override IItemCreateLinkRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      ItemCreateLinkRequest request = new ItemCreateLinkRequest(functionUrl, this.Client, options);
      if (this.HasParameter("type"))
        request.RequestBody.Type = this.GetParameter<string>("type");
      return (IItemCreateLinkRequest) request;
    }
  }
}
