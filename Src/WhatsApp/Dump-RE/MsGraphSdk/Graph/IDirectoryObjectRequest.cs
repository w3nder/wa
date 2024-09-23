// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryObjectRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryObjectRequest : IBaseRequest
  {
    Task<DirectoryObject> CreateAsync(DirectoryObject directoryObjectToCreate);

    Task<DirectoryObject> CreateAsync(
      DirectoryObject directoryObjectToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<DirectoryObject> GetAsync();

    Task<DirectoryObject> GetAsync(CancellationToken cancellationToken);

    Task<DirectoryObject> UpdateAsync(DirectoryObject directoryObjectToUpdate);

    Task<DirectoryObject> UpdateAsync(
      DirectoryObject directoryObjectToUpdate,
      CancellationToken cancellationToken);
  }
}
