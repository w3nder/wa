// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Helpers.UploadSessionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Helpers
{
  public class UploadSessionRequest : BaseRequest
  {
    private readonly UploadSession session;

    public UploadSessionRequest(
      UploadSession session,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(session.UploadUrl, client, options)
    {
      this.session = session;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      UploadSession uploadSession = await this.SendAsync<UploadSession>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<UploadSession> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<UploadSession> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      return await this.SendAsync<UploadSession>((object) null, cancellationToken).ConfigureAwait(false);
    }
  }
}
