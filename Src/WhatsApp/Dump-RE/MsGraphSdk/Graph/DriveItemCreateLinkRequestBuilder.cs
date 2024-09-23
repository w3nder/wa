// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemCreateLinkRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemCreateLinkRequestBuilder : 
    BasePostMethodRequestBuilder<IDriveItemCreateLinkRequest>,
    IDriveItemCreateLinkRequestBuilder
  {
    public DriveItemCreateLinkRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string type,
      string scope)
      : base(requestUrl, client)
    {
      this.SetParameter<string>(nameof (type), type, false);
      this.SetParameter<string>(nameof (scope), scope, true);
    }

    protected override IDriveItemCreateLinkRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      DriveItemCreateLinkRequest request = new DriveItemCreateLinkRequest(functionUrl, this.Client, options);
      if (this.HasParameter("type"))
        request.RequestBody.Type = this.GetParameter<string>("type");
      if (this.HasParameter("scope"))
        request.RequestBody.Scope = this.GetParameter<string>("scope");
      return (IDriveItemCreateLinkRequest) request;
    }
  }
}
