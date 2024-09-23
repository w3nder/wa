// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceDevicesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGraphServiceDevicesCollectionRequest : IBaseRequest
  {
    Task<Device> AddAsync(Device device);

    Task<Device> AddAsync(Device device, CancellationToken cancellationToken);

    Task<IGraphServiceDevicesCollectionPage> GetAsync();

    Task<IGraphServiceDevicesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGraphServiceDevicesCollectionRequest Top(int value);

    IGraphServiceDevicesCollectionRequest OrderBy(string value);
  }
}
