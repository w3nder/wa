// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupRequest : IBaseRequest
  {
    Task<Group> CreateAsync(Group groupToCreate);

    Task<Group> CreateAsync(Group groupToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Group> GetAsync();

    Task<Group> GetAsync(CancellationToken cancellationToken);

    Task<Group> UpdateAsync(Group groupToUpdate);

    Task<Group> UpdateAsync(Group groupToUpdate, CancellationToken cancellationToken);
  }
}
