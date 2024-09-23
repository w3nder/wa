// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DeviceRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DeviceRequest : BaseRequest, IDeviceRequest, IBaseRequest
  {
    public DeviceRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Device> CreateAsync(Device deviceToCreate)
    {
      return this.CreateAsync(deviceToCreate, CancellationToken.None);
    }

    public async Task<Device> CreateAsync(
      Device deviceToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Device deviceToInitialize = await this.SendAsync<Device>((object) deviceToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(deviceToInitialize);
      return deviceToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Device device = await this.SendAsync<Device>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Device> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Device> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Device deviceToInitialize = await this.SendAsync<Device>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(deviceToInitialize);
      return deviceToInitialize;
    }

    public Task<Device> UpdateAsync(Device deviceToUpdate)
    {
      return this.UpdateAsync(deviceToUpdate, CancellationToken.None);
    }

    public async Task<Device> UpdateAsync(
      Device deviceToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Device deviceToInitialize = await this.SendAsync<Device>((object) deviceToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(deviceToInitialize);
      return deviceToInitialize;
    }

    private void InitializeCollectionProperties(Device deviceToInitialize)
    {
      if (deviceToInitialize == null || deviceToInitialize.AdditionalData == null)
        return;
      if (deviceToInitialize.RegisteredOwners != null && deviceToInitialize.RegisteredOwners.CurrentPage != null)
      {
        deviceToInitialize.RegisteredOwners.AdditionalData = deviceToInitialize.AdditionalData;
        object obj;
        deviceToInitialize.AdditionalData.TryGetValue("registeredOwners@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          deviceToInitialize.RegisteredOwners.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (deviceToInitialize.RegisteredUsers == null || deviceToInitialize.RegisteredUsers.CurrentPage == null)
        return;
      deviceToInitialize.RegisteredUsers.AdditionalData = deviceToInitialize.AdditionalData;
      object obj1;
      deviceToInitialize.AdditionalData.TryGetValue("registeredUsers@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      deviceToInitialize.RegisteredUsers.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
