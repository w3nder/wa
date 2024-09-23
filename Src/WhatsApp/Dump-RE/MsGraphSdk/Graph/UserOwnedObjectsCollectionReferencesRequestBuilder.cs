// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserOwnedObjectsCollectionReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserOwnedObjectsCollectionReferencesRequestBuilder : 
    BaseRequestBuilder,
    IUserOwnedObjectsCollectionReferencesRequestBuilder
  {
    public UserOwnedObjectsCollectionReferencesRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserOwnedObjectsCollectionReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserOwnedObjectsCollectionReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IUserOwnedObjectsCollectionReferencesRequest) new UserOwnedObjectsCollectionReferencesRequest(this.RequestUrl, this.Client, options);
    }
  }
}
