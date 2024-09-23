// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemCreateSessionRequest
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
  public class ItemCreateSessionRequest : BaseRequest, IItemCreateSessionRequest, IBaseRequest
  {
    public ItemCreateSessionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new ItemCreateSessionRequestBody();
    }

    public ItemCreateSessionRequestBody RequestBody { get; private set; }

    public Task<UploadSession> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<UploadSession> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<UploadSession>((object) this.RequestBody, cancellationToken);
    }

    public IItemCreateSessionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IItemCreateSessionRequest) this;
    }

    public IItemCreateSessionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IItemCreateSessionRequest) this;
    }
  }
}
