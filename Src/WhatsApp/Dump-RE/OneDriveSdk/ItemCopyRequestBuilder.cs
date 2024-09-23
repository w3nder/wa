// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCopyRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemCopyRequestBuilder : 
    BasePostMethodRequestBuilder<IItemCopyRequest>,
    IItemCopyRequestBuilder
  {
    public ItemCopyRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string name,
      ItemReference parentReference)
      : base(requestUrl, client)
    {
      this.SetParameter<string>(nameof (name), name, true);
      this.SetParameter<ItemReference>(nameof (parentReference), parentReference, true);
    }

    protected override IItemCopyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      ItemCopyRequest request = new ItemCopyRequest(functionUrl, this.Client, options);
      if (this.HasParameter("name"))
        request.RequestBody.Name = this.GetParameter<string>("name");
      if (this.HasParameter("parentReference"))
        request.RequestBody.ParentReference = this.GetParameter<ItemReference>("parentReference");
      return (IItemCopyRequest) request;
    }
  }
}
