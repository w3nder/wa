// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemDeltaRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemDeltaRequestBuilder : 
    BaseGetMethodRequestBuilder<IDriveItemDeltaRequest>,
    IDriveItemDeltaRequestBuilder
  {
    public DriveItemDeltaRequestBuilder(string requestUrl, IBaseClient client, string token)
      : base(requestUrl, client)
    {
      this.SetParameter(nameof (token), (object) token, true);
    }

    public DriveItemDeltaRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected override IDriveItemDeltaRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IDriveItemDeltaRequest) new DriveItemDeltaRequest(functionUrl, this.Client, options);
    }
  }
}
