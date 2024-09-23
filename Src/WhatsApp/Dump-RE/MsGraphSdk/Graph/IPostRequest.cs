// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IPostRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IPostRequest : IBaseRequest
  {
    Task<Post> CreateAsync(Post postToCreate);

    Task<Post> CreateAsync(Post postToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Post> GetAsync();

    Task<Post> GetAsync(CancellationToken cancellationToken);

    Task<Post> UpdateAsync(Post postToUpdate);

    Task<Post> UpdateAsync(Post postToUpdate, CancellationToken cancellationToken);

    IPostRequest Expand(string value);

    IPostRequest Select(string value);
  }
}
