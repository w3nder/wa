// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMemberOfCollectionReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMemberOfCollectionReferencesRequestBuilder : 
    BaseRequestBuilder,
    IUserMemberOfCollectionReferencesRequestBuilder
  {
    public UserMemberOfCollectionReferencesRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserMemberOfCollectionReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserMemberOfCollectionReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IUserMemberOfCollectionReferencesRequest) new UserMemberOfCollectionReferencesRequest(this.RequestUrl, this.Client, options);
    }
  }
}
