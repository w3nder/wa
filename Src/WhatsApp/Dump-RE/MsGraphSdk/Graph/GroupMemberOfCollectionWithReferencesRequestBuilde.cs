// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupMemberOfCollectionWithReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupMemberOfCollectionWithReferencesRequestBuilder : 
    BaseRequestBuilder,
    IGroupMemberOfCollectionWithReferencesRequestBuilder
  {
    public GroupMemberOfCollectionWithReferencesRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGroupMemberOfCollectionWithReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IGroupMemberOfCollectionWithReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IGroupMemberOfCollectionWithReferencesRequest) new GroupMemberOfCollectionWithReferencesRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectWithReferenceRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }

    public IGroupMemberOfCollectionReferencesRequestBuilder References
    {
      get
      {
        return (IGroupMemberOfCollectionReferencesRequestBuilder) new GroupMemberOfCollectionReferencesRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
