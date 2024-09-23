// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactExtensionsCollectionRequest : IBaseRequest
  {
    Task<Extension> AddAsync(Extension extension);

    Task<Extension> AddAsync(Extension extension, CancellationToken cancellationToken);

    Task<IContactExtensionsCollectionPage> GetAsync();

    Task<IContactExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IContactExtensionsCollectionRequest Expand(string value);

    IContactExtensionsCollectionRequest Select(string value);

    IContactExtensionsCollectionRequest Top(int value);

    IContactExtensionsCollectionRequest Filter(string value);

    IContactExtensionsCollectionRequest Skip(int value);

    IContactExtensionsCollectionRequest OrderBy(string value);
  }
}
