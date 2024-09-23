// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DeviceRegisteredOwnersCollectionWithReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DeviceRegisteredOwnersCollectionWithReferencesRequestBuilder : 
    BaseRequestBuilder,
    IDeviceRegisteredOwnersCollectionWithReferencesRequestBuilder
  {
    public DeviceRegisteredOwnersCollectionWithReferencesRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDeviceRegisteredOwnersCollectionWithReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDeviceRegisteredOwnersCollectionWithReferencesRequest Request(
      IEnumerable<Option> options)
    {
      return (IDeviceRegisteredOwnersCollectionWithReferencesRequest) new DeviceRegisteredOwnersCollectionWithReferencesRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectWithReferenceRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }

    public IDeviceRegisteredOwnersCollectionReferencesRequestBuilder References
    {
      get
      {
        return (IDeviceRegisteredOwnersCollectionReferencesRequestBuilder) new DeviceRegisteredOwnersCollectionReferencesRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
