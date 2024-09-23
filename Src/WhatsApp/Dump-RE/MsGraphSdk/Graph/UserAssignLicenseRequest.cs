// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserAssignLicenseRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserAssignLicenseRequest : BaseRequest, IUserAssignLicenseRequest, IBaseRequest
  {
    public UserAssignLicenseRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
      this.ContentType = "application/json";
      this.RequestBody = new UserAssignLicenseRequestBody();
    }

    public UserAssignLicenseRequestBody RequestBody { get; private set; }

    public Task<User> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<User> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<User>((object) this.RequestBody, cancellationToken);
    }

    public IUserAssignLicenseRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserAssignLicenseRequest) this;
    }

    public IUserAssignLicenseRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserAssignLicenseRequest) this;
    }
  }
}
