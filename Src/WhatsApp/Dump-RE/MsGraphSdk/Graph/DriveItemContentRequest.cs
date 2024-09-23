// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemContentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemContentRequest : BaseRequest, IDriveItemContentRequest, IBaseRequest
  {
    public DriveItemContentRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Stream> GetAsync()
    {
      return this.GetAsync(CancellationToken.None, HttpCompletionOption.ResponseContentRead);
    }

    public Task<Stream> GetAsync(
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      this.Method = "GET";
      return this.SendStreamRequestAsync((object) null, cancellationToken, completionOption);
    }

    public Task<T> PutAsync<T>(Stream content) where T : DriveItem
    {
      return this.PutAsync<T>(content, CancellationToken.None, HttpCompletionOption.ResponseContentRead);
    }

    public Task<T> PutAsync<T>(
      Stream content,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
      where T : DriveItem
    {
      this.ContentType = "application/octet-stream";
      this.Method = "PUT";
      return this.SendAsync<T>((object) content, cancellationToken, completionOption);
    }
  }
}
