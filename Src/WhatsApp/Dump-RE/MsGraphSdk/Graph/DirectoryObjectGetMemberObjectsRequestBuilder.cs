// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectGetMemberObjectsRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectGetMemberObjectsRequestBuilder : 
    BasePostMethodRequestBuilder<IDirectoryObjectGetMemberObjectsRequest>,
    IDirectoryObjectGetMemberObjectsRequestBuilder
  {
    public DirectoryObjectGetMemberObjectsRequestBuilder(
      string requestUrl,
      IBaseClient client,
      bool? securityEnabledOnly)
      : base(requestUrl, client)
    {
      this.SetParameter<bool?>(nameof (securityEnabledOnly), securityEnabledOnly, true);
    }

    protected override IDirectoryObjectGetMemberObjectsRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      DirectoryObjectGetMemberObjectsRequest request = new DirectoryObjectGetMemberObjectsRequest(functionUrl, this.Client, options);
      if (this.HasParameter("securityEnabledOnly"))
        request.RequestBody.SecurityEnabledOnly = this.GetParameter<bool?>("securityEnabledOnly");
      return (IDirectoryObjectGetMemberObjectsRequest) request;
    }
  }
}
