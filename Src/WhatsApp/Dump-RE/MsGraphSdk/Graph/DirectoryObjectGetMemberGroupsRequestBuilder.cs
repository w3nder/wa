// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectGetMemberGroupsRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectGetMemberGroupsRequestBuilder : 
    BasePostMethodRequestBuilder<IDirectoryObjectGetMemberGroupsRequest>,
    IDirectoryObjectGetMemberGroupsRequestBuilder
  {
    public DirectoryObjectGetMemberGroupsRequestBuilder(
      string requestUrl,
      IBaseClient client,
      bool? securityEnabledOnly)
      : base(requestUrl, client)
    {
      this.SetParameter<bool?>(nameof (securityEnabledOnly), securityEnabledOnly, true);
    }

    protected override IDirectoryObjectGetMemberGroupsRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      DirectoryObjectGetMemberGroupsRequest request = new DirectoryObjectGetMemberGroupsRequest(functionUrl, this.Client, options);
      if (this.HasParameter("securityEnabledOnly"))
        request.RequestBody.SecurityEnabledOnly = this.GetParameter<bool?>("securityEnabledOnly");
      return (IDirectoryObjectGetMemberGroupsRequest) request;
    }
  }
}
