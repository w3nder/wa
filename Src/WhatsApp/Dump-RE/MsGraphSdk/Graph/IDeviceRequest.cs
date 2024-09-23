// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDeviceRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDeviceRequest : IBaseRequest
  {
    Task<Device> CreateAsync(Device deviceToCreate);

    Task<Device> CreateAsync(Device deviceToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Device> GetAsync();

    Task<Device> GetAsync(CancellationToken cancellationToken);

    Task<Device> UpdateAsync(Device deviceToUpdate);

    Task<Device> UpdateAsync(Device deviceToUpdate, CancellationToken cancellationToken);
  }
}
