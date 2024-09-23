// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryRoleMembersCollectionReferencesRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryRoleMembersCollectionReferencesRequestBuilder : 
    BaseRequestBuilder,
    IDirectoryRoleMembersCollectionReferencesRequestBuilder
  {
    public DirectoryRoleMembersCollectionReferencesRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryRoleMembersCollectionReferencesRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDirectoryRoleMembersCollectionReferencesRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryRoleMembersCollectionReferencesRequest) new DirectoryRoleMembersCollectionReferencesRequest(this.RequestUrl, this.Client, options);
    }
  }
}
