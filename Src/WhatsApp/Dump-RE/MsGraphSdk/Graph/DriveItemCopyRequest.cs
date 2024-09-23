// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemCopyRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemCopyRequest : BaseRequest, IDriveItemCopyRequest, IBaseRequest
  {
    public DriveItemCopyRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new DriveItemCopyRequestBody();
    }

    public DriveItemCopyRequestBody RequestBody { get; private set; }

    public Task<DriveItem> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<DriveItem> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<DriveItem>((object) this.RequestBody, cancellationToken);
    }

    public IDriveItemCopyRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemCopyRequest) this;
    }

    public IDriveItemCopyRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemCopyRequest) this;
    }
  }
}
