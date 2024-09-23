// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OrganizationRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class OrganizationRequestBuilder : 
    DirectoryObjectRequestBuilder,
    IOrganizationRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public OrganizationRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IOrganizationRequest Request() => this.Request((IEnumerable<Option>) null);

    public IOrganizationRequest Request(IEnumerable<Option> options)
    {
      return (IOrganizationRequest) new OrganizationRequest(this.RequestUrl, this.Client, options);
    }
  }
}
