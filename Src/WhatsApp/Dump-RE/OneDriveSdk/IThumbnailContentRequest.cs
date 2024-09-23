// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IThumbnailContentRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IThumbnailContentRequest : IBaseRequest
  {
    Task<Stream> GetAsync();

    Task<Stream> GetAsync(
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead);

    Task<T> PutAsync<T>(Stream content) where T : Thumbnail;

    Task<T> PutAsync<T>(
      Stream content,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
      where T : Thumbnail;
  }
}
