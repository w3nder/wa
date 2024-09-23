// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemPermissionsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemPermissionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IDriveItemPermissionsCollectionRequestBuilder
  {
    public DriveItemPermissionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveItemPermissionsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDriveItemPermissionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IDriveItemPermissionsCollectionRequest) new DriveItemPermissionsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IPermissionRequestBuilder this[string id]
    {
      get
      {
        return (IPermissionRequestBuilder) new PermissionRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
