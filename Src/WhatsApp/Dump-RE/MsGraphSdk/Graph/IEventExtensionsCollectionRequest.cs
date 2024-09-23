// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IEventExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IEventExtensionsCollectionRequest : IBaseRequest
  {
    Task<Extension> AddAsync(Extension extension);

    Task<Extension> AddAsync(Extension extension, CancellationToken cancellationToken);

    Task<IEventExtensionsCollectionPage> GetAsync();

    Task<IEventExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IEventExtensionsCollectionRequest Expand(string value);

    IEventExtensionsCollectionRequest Select(string value);

    IEventExtensionsCollectionRequest Top(int value);

    IEventExtensionsCollectionRequest Filter(string value);

    IEventExtensionsCollectionRequest Skip(int value);

    IEventExtensionsCollectionRequest OrderBy(string value);
  }
}
