// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IPostExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IPostExtensionsCollectionRequest : IBaseRequest
  {
    Task<Extension> AddAsync(Extension extension);

    Task<Extension> AddAsync(Extension extension, CancellationToken cancellationToken);

    Task<IPostExtensionsCollectionPage> GetAsync();

    Task<IPostExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IPostExtensionsCollectionRequest Expand(string value);

    IPostExtensionsCollectionRequest Select(string value);

    IPostExtensionsCollectionRequest Top(int value);

    IPostExtensionsCollectionRequest Filter(string value);

    IPostExtensionsCollectionRequest Skip(int value);

    IPostExtensionsCollectionRequest OrderBy(string value);
  }
}
