// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryRoleTemplateRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryRoleTemplateRequest : IBaseRequest
  {
    Task<DirectoryRoleTemplate> CreateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToCreate);

    Task<DirectoryRoleTemplate> CreateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<DirectoryRoleTemplate> GetAsync();

    Task<DirectoryRoleTemplate> GetAsync(CancellationToken cancellationToken);

    Task<DirectoryRoleTemplate> UpdateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToUpdate);

    Task<DirectoryRoleTemplate> UpdateAsync(
      DirectoryRoleTemplate directoryRoleTemplateToUpdate,
      CancellationToken cancellationToken);
  }
}
