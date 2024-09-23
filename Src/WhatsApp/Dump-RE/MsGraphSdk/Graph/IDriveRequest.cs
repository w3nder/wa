// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveRequest : IBaseRequest
  {
    Task<Drive> CreateAsync(Drive driveToCreate);

    Task<Drive> CreateAsync(Drive driveToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Drive> GetAsync();

    Task<Drive> GetAsync(CancellationToken cancellationToken);

    Task<Drive> UpdateAsync(Drive driveToUpdate);

    Task<Drive> UpdateAsync(Drive driveToUpdate, CancellationToken cancellationToken);

    IDriveRequest Expand(string value);

    IDriveRequest Select(string value);
  }
}
