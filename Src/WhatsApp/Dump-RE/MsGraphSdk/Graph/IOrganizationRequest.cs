// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IOrganizationRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IOrganizationRequest : IBaseRequest
  {
    Task<Organization> CreateAsync(Organization organizationToCreate);

    Task<Organization> CreateAsync(
      Organization organizationToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Organization> GetAsync();

    Task<Organization> GetAsync(CancellationToken cancellationToken);

    Task<Organization> UpdateAsync(Organization organizationToUpdate);

    Task<Organization> UpdateAsync(
      Organization organizationToUpdate,
      CancellationToken cancellationToken);
  }
}
