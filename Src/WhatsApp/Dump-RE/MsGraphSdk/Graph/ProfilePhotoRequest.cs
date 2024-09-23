// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ProfilePhotoRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class ProfilePhotoRequest : BaseRequest, IProfilePhotoRequest, IBaseRequest
  {
    public ProfilePhotoRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<ProfilePhoto> CreateAsync(ProfilePhoto profilePhotoToCreate)
    {
      return this.CreateAsync(profilePhotoToCreate, CancellationToken.None);
    }

    public async Task<ProfilePhoto> CreateAsync(
      ProfilePhoto profilePhotoToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      ProfilePhoto profilePhotoToInitialize = await this.SendAsync<ProfilePhoto>((object) profilePhotoToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(profilePhotoToInitialize);
      return profilePhotoToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      ProfilePhoto profilePhoto = await this.SendAsync<ProfilePhoto>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<ProfilePhoto> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<ProfilePhoto> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      ProfilePhoto profilePhotoToInitialize = await this.SendAsync<ProfilePhoto>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(profilePhotoToInitialize);
      return profilePhotoToInitialize;
    }

    public Task<ProfilePhoto> UpdateAsync(ProfilePhoto profilePhotoToUpdate)
    {
      return this.UpdateAsync(profilePhotoToUpdate, CancellationToken.None);
    }

    public async Task<ProfilePhoto> UpdateAsync(
      ProfilePhoto profilePhotoToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      ProfilePhoto profilePhotoToInitialize = await this.SendAsync<ProfilePhoto>((object) profilePhotoToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(profilePhotoToInitialize);
      return profilePhotoToInitialize;
    }

    public IProfilePhotoRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IProfilePhotoRequest) this;
    }

    public IProfilePhotoRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IProfilePhotoRequest) this;
    }

    private void InitializeCollectionProperties(ProfilePhoto profilePhotoToInitialize)
    {
    }
  }
}
