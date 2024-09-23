// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMessageExtensionsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMessageExtensionsCollectionRequest : IBaseRequest
  {
    Task<Extension> AddAsync(Extension extension);

    Task<Extension> AddAsync(Extension extension, CancellationToken cancellationToken);

    Task<IMessageExtensionsCollectionPage> GetAsync();

    Task<IMessageExtensionsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IMessageExtensionsCollectionRequest Expand(string value);

    IMessageExtensionsCollectionRequest Select(string value);

    IMessageExtensionsCollectionRequest Top(int value);

    IMessageExtensionsCollectionRequest Filter(string value);

    IMessageExtensionsCollectionRequest Skip(int value);

    IMessageExtensionsCollectionRequest OrderBy(string value);
  }
}
