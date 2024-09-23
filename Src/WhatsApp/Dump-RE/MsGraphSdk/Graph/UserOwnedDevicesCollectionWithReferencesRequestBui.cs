// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserOwnedDevicesCollectionWithReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserOwnedDevicesCollectionWithReferencesRequestBuilder : 
    BaseRequestBuilder,
    IUserOwnedDevicesCollectionWithReferencesRequestBuilder
  {
    public UserOwnedDevicesCollectionWithReferencesRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserOwnedDevicesCollectionWithReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserOwnedDevicesCollectionWithReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IUserOwnedDevicesCollectionWithReferencesRequest) new UserOwnedDevicesCollectionWithReferencesRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectWithReferenceRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }

    public IUserOwnedDevicesCollectionReferencesRequestBuilder References
    {
      get
      {
        return (IUserOwnedDevicesCollectionReferencesRequestBuilder) new UserOwnedDevicesCollectionReferencesRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
