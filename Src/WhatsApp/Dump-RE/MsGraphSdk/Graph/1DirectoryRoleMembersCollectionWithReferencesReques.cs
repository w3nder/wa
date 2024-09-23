// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRoleMembersCollectionWithReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryRoleMembersCollectionWithReferencesRequestBuilder : 
    BaseRequestBuilder,
    IDirectoryRoleMembersCollectionWithReferencesRequestBuilder
  {
    public DirectoryRoleMembersCollectionWithReferencesRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryRoleMembersCollectionWithReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDirectoryRoleMembersCollectionWithReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryRoleMembersCollectionWithReferencesRequest) new DirectoryRoleMembersCollectionWithReferencesRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectWithReferenceRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }

    public IDirectoryRoleMembersCollectionReferencesRequestBuilder References
    {
      get
      {
        return (IDirectoryRoleMembersCollectionReferencesRequestBuilder) new DirectoryRoleMembersCollectionReferencesRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
