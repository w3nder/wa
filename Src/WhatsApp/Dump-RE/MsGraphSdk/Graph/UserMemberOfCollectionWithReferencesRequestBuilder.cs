// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMemberOfCollectionWithReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMemberOfCollectionWithReferencesRequestBuilder : 
    BaseRequestBuilder,
    IUserMemberOfCollectionWithReferencesRequestBuilder
  {
    public UserMemberOfCollectionWithReferencesRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserMemberOfCollectionWithReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserMemberOfCollectionWithReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IUserMemberOfCollectionWithReferencesRequest) new UserMemberOfCollectionWithReferencesRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectWithReferenceRequestBuilder this[string id]
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }

    public IUserMemberOfCollectionReferencesRequestBuilder References
    {
      get
      {
        return (IUserMemberOfCollectionReferencesRequestBuilder) new UserMemberOfCollectionReferencesRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
