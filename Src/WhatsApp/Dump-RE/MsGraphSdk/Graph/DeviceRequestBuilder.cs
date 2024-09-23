// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DeviceRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DeviceRequestBuilder : 
    DirectoryObjectRequestBuilder,
    IDeviceRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public DeviceRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDeviceRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDeviceRequest Request(IEnumerable<Option> options)
    {
      return (IDeviceRequest) new DeviceRequest(this.RequestUrl, this.Client, options);
    }

    public IDeviceRegisteredOwnersCollectionWithReferencesRequestBuilder RegisteredOwners
    {
      get
      {
        return (IDeviceRegisteredOwnersCollectionWithReferencesRequestBuilder) new DeviceRegisteredOwnersCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("registeredOwners"), this.Client);
      }
    }

    public IDeviceRegisteredUsersCollectionWithReferencesRequestBuilder RegisteredUsers
    {
      get
      {
        return (IDeviceRegisteredUsersCollectionWithReferencesRequestBuilder) new DeviceRegisteredUsersCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("registeredUsers"), this.Client);
      }
    }
  }
}
