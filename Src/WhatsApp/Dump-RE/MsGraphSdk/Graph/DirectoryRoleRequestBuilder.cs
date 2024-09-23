// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRoleRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryRoleRequestBuilder : 
    DirectoryObjectRequestBuilder,
    IDirectoryRoleRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public DirectoryRoleRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryRoleRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDirectoryRoleRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryRoleRequest) new DirectoryRoleRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryRoleMembersCollectionWithReferencesRequestBuilder Members
    {
      get
      {
        return (IDirectoryRoleMembersCollectionWithReferencesRequestBuilder) new DirectoryRoleMembersCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("members"), this.Client);
      }
    }
  }
}
