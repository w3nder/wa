// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserWithReferenceRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserWithReferenceRequest : BaseRequest, IUserWithReferenceRequest, IBaseRequest
  {
    public UserWithReferenceRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<User> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<User> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      return await this.SendAsync<User>((object) null, cancellationToken).ConfigureAwait(false);
    }
  }
}
