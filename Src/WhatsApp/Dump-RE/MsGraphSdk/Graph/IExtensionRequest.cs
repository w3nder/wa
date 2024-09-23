// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IExtensionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IExtensionRequest : IBaseRequest
  {
    Task<Extension> CreateAsync(Extension extensionToCreate);

    Task<Extension> CreateAsync(Extension extensionToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Extension> GetAsync();

    Task<Extension> GetAsync(CancellationToken cancellationToken);

    Task<Extension> UpdateAsync(Extension extensionToUpdate);

    Task<Extension> UpdateAsync(Extension extensionToUpdate, CancellationToken cancellationToken);

    IExtensionRequest Expand(string value);

    IExtensionRequest Select(string value);
  }
}
