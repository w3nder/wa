// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemContentRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemContentRequest : BaseRequest, IItemContentRequest, IBaseRequest
  {
    public ItemContentRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
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

    public Task<T> PutAsync<T>(Stream content) where T : Item
    {
      return this.PutAsync<T>(content, CancellationToken.None, HttpCompletionOption.ResponseContentRead);
    }

    public Task<T> PutAsync<T>(
      Stream content,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
      where T : Item
    {
      this.ContentType = "application/octet-stream";
      this.Method = "PUT";
      return this.SendAsync<T>((object) content, cancellationToken, completionOption);
    }
  }
}
