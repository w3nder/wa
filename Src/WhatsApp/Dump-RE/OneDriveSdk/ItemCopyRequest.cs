// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCopyRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemCopyRequest : BaseRequest, IItemCopyRequest, IBaseRequest
  {
    public ItemCopyRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new ItemCopyRequestBody();
    }

    public ItemCopyRequestBody RequestBody { get; private set; }

    public Task<IAsyncMonitor<Item>> PostAsync() => this.PostAsync(CancellationToken.None);

    public async Task<IAsyncMonitor<Item>> PostAsync(CancellationToken cancellationToken)
    {
      IAsyncMonitor<Item> asyncMonitor;
      using (HttpResponseMessage httpResponseMessage = await this.SendRequestAsync((object) this.RequestBody, cancellationToken).ConfigureAwait(false))
        asyncMonitor = (IAsyncMonitor<Item>) new AsyncMonitor<Item>(this.Client, httpResponseMessage.Headers.Location.ToString());
      return asyncMonitor;
    }

    public IItemCopyRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemCopyRequest) this;
    }

    public IItemCopyRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemCopyRequest) this;
    }
  }
}
