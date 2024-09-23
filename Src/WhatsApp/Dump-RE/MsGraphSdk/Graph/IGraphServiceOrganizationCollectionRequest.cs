// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceOrganizationCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGraphServiceOrganizationCollectionRequest : IBaseRequest
  {
    Task<Organization> AddAsync(Organization organization);

    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken);

    Task<IGraphServiceOrganizationCollectionPage> GetAsync();

    Task<IGraphServiceOrganizationCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGraphServiceOrganizationCollectionRequest Top(int value);

    IGraphServiceOrganizationCollectionRequest OrderBy(string value);
  }
}
