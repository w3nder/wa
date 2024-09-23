// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IOpenTypeExtensionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IOpenTypeExtensionRequest : IBaseRequest
  {
    Task<OpenTypeExtension> CreateAsync(OpenTypeExtension openTypeExtensionToCreate);

    Task<OpenTypeExtension> CreateAsync(
      OpenTypeExtension openTypeExtensionToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<OpenTypeExtension> GetAsync();

    Task<OpenTypeExtension> GetAsync(CancellationToken cancellationToken);

    Task<OpenTypeExtension> UpdateAsync(OpenTypeExtension openTypeExtensionToUpdate);

    Task<OpenTypeExtension> UpdateAsync(
      OpenTypeExtension openTypeExtensionToUpdate,
      CancellationToken cancellationToken);

    IOpenTypeExtensionRequest Expand(string value);

    IOpenTypeExtensionRequest Select(string value);
  }
}
