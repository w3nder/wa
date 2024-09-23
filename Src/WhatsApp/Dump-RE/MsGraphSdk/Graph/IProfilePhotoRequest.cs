// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IProfilePhotoRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IProfilePhotoRequest : IBaseRequest
  {
    Task<ProfilePhoto> CreateAsync(ProfilePhoto profilePhotoToCreate);

    Task<ProfilePhoto> CreateAsync(
      ProfilePhoto profilePhotoToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ProfilePhoto> GetAsync();

    Task<ProfilePhoto> GetAsync(CancellationToken cancellationToken);

    Task<ProfilePhoto> UpdateAsync(ProfilePhoto profilePhotoToUpdate);

    Task<ProfilePhoto> UpdateAsync(
      ProfilePhoto profilePhotoToUpdate,
      CancellationToken cancellationToken);

    IProfilePhotoRequest Expand(string value);

    IProfilePhotoRequest Select(string value);
  }
}
