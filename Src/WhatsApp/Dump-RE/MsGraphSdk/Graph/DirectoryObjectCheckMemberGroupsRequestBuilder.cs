// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectCheckMemberGroupsRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectCheckMemberGroupsRequestBuilder : 
    BasePostMethodRequestBuilder<IDirectoryObjectCheckMemberGroupsRequest>,
    IDirectoryObjectCheckMemberGroupsRequestBuilder
  {
    public DirectoryObjectCheckMemberGroupsRequestBuilder(
      string requestUrl,
      IBaseClient client,
      IEnumerable<string> groupIds)
      : base(requestUrl, client)
    {
      this.SetParameter<IEnumerable<string>>(nameof (groupIds), groupIds, false);
    }

    protected override IDirectoryObjectCheckMemberGroupsRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      DirectoryObjectCheckMemberGroupsRequest request = new DirectoryObjectCheckMemberGroupsRequest(functionUrl, this.Client, options);
      if (this.HasParameter("groupIds"))
        request.RequestBody.GroupIds = this.GetParameter<IEnumerable<string>>("groupIds");
      return (IDirectoryObjectCheckMemberGroupsRequest) request;
    }
  }
}
