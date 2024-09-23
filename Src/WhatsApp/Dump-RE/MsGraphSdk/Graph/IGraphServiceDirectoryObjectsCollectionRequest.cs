// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceDirectoryObjectsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGraphServiceDirectoryObjectsCollectionRequest : IBaseRequest
  {
    Task<DirectoryObject> AddAsync(DirectoryObject directoryObject);

    Task<DirectoryObject> AddAsync(
      DirectoryObject directoryObject,
      CancellationToken cancellationToken);

    Task<IGraphServiceDirectoryObjectsCollectionPage> GetAsync();

    Task<IGraphServiceDirectoryObjectsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGraphServiceDirectoryObjectsCollectionRequest Top(int value);

    IGraphServiceDirectoryObjectsCollectionRequest OrderBy(string value);
  }
}
