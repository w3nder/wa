// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemContentRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemContentRequestBuilder : BaseRequestBuilder, IItemContentRequestBuilder
  {
    public ItemContentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IItemContentRequest Request()
    {
      return (IItemContentRequest) new ItemContentRequest(this.RequestUrl, this.Client, (IEnumerable<Option>) null);
    }
  }
}
