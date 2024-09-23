// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceDrivesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGraphServiceDrivesCollectionRequest : IBaseRequest
  {
    Task<Drive> AddAsync(Drive drive);

    Task<Drive> AddAsync(Drive drive, CancellationToken cancellationToken);

    Task<IGraphServiceDrivesCollectionPage> GetAsync();

    Task<IGraphServiceDrivesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGraphServiceDrivesCollectionRequest Expand(string value);

    IGraphServiceDrivesCollectionRequest Select(string value);

    IGraphServiceDrivesCollectionRequest Top(int value);

    IGraphServiceDrivesCollectionRequest Filter(string value);

    IGraphServiceDrivesCollectionRequest Skip(int value);

    IGraphServiceDrivesCollectionRequest OrderBy(string value);
  }
}
