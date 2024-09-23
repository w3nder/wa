// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCreateLinkRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemCreateLinkRequest : BaseRequest, IItemCreateLinkRequest, IBaseRequest
  {
    public ItemCreateLinkRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new ItemCreateLinkRequestBody();
    }

    public ItemCreateLinkRequestBody RequestBody { get; private set; }

    public Task<Permission> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<Permission> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<Permission>((object) this.RequestBody, cancellationToken);
    }

    public IItemCreateLinkRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemCreateLinkRequest) this;
    }

    public IItemCreateLinkRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemCreateLinkRequest) this;
    }
  }
}
