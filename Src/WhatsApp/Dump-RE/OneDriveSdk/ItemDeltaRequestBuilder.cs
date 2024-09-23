// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemDeltaRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemDeltaRequestBuilder : 
    BaseGetMethodRequestBuilder<IItemDeltaRequest>,
    IItemDeltaRequestBuilder
  {
    public ItemDeltaRequestBuilder(string requestUrl, IBaseClient client, string token)
      : base(requestUrl, client)
    {
      this.passParametersInQueryString = true;
      this.SetParameter(nameof (token), (object) token, true);
    }

    protected override IItemDeltaRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IItemDeltaRequest) new ItemDeltaRequest(functionUrl, this.Client, options);
    }
  }
}
