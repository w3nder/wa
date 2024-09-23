// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemCopyRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemCopyRequestBuilder : 
    BasePostMethodRequestBuilder<IDriveItemCopyRequest>,
    IDriveItemCopyRequestBuilder
  {
    public DriveItemCopyRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string name,
      ItemReference parentReference)
      : base(requestUrl, client)
    {
      this.SetParameter<string>(nameof (name), name, true);
      this.SetParameter<ItemReference>(nameof (parentReference), parentReference, true);
    }

    protected override IDriveItemCopyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      DriveItemCopyRequest request = new DriveItemCopyRequest(functionUrl, this.Client, options);
      if (this.HasParameter("name"))
        request.RequestBody.Name = this.GetParameter<string>("name");
      if (this.HasParameter("parentReference"))
        request.RequestBody.ParentReference = this.GetParameter<ItemReference>("parentReference");
      return (IDriveItemCopyRequest) request;
    }
  }
}
