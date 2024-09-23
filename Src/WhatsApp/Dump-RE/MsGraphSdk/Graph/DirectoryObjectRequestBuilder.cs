// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectRequestBuilder : 
    EntityRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public DirectoryObjectRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryObjectRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDirectoryObjectRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryObjectRequest) new DirectoryObjectRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectCheckMemberGroupsRequestBuilder CheckMemberGroups(
      IEnumerable<string> groupIds)
    {
      return (IDirectoryObjectCheckMemberGroupsRequestBuilder) new DirectoryObjectCheckMemberGroupsRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.checkMemberGroups"), this.Client, groupIds);
    }

    public IDirectoryObjectGetMemberGroupsRequestBuilder GetMemberGroups(bool? securityEnabledOnly = null)
    {
      return (IDirectoryObjectGetMemberGroupsRequestBuilder) new DirectoryObjectGetMemberGroupsRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.getMemberGroups"), this.Client, securityEnabledOnly);
    }

    public IDirectoryObjectGetMemberObjectsRequestBuilder GetMemberObjects(bool? securityEnabledOnly = null)
    {
      return (IDirectoryObjectGetMemberObjectsRequestBuilder) new DirectoryObjectGetMemberObjectsRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.getMemberObjects"), this.Client, securityEnabledOnly);
    }
  }
}
