// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectReferenceRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectReferenceRequest : 
    BaseRequest,
    IDirectoryObjectReferenceRequest,
    IBaseRequest
  {
    public DirectoryObjectReferenceRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      DirectoryObject directoryObject = await this.SendAsync<DirectoryObject>((object) null, cancellationToken).ConfigureAwait(false);
    }
  }
}
