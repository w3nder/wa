// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemSearchRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemSearchRequestBuilder : 
    BaseGetMethodRequestBuilder<IDriveItemSearchRequest>,
    IDriveItemSearchRequestBuilder
  {
    public DriveItemSearchRequestBuilder(string requestUrl, IBaseClient client, string q)
      : base(requestUrl, client)
    {
      this.SetParameter(nameof (q), (object) q, true);
    }

    protected override IDriveItemSearchRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IDriveItemSearchRequest) new DriveItemSearchRequest(functionUrl, this.Client, options);
    }
  }
}
