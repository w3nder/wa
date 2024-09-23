// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IThumbnailContentRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
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
