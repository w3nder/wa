// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemCreateLinkRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemCreateLinkRequest : BaseRequest, IDriveItemCreateLinkRequest, IBaseRequest
  {
    public DriveItemCreateLinkRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new DriveItemCreateLinkRequestBody();
    }

    public DriveItemCreateLinkRequestBody RequestBody { get; private set; }

    public Task<Permission> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<Permission> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<Permission>((object) this.RequestBody, cancellationToken);
    }

    public IDriveItemCreateLinkRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemCreateLinkRequest) this;
    }

    public IDriveItemCreateLinkRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemCreateLinkRequest) this;
    }
  }
}
