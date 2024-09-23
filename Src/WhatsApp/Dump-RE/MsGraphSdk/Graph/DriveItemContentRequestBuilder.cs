// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemContentRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemContentRequestBuilder : BaseRequestBuilder, IDriveItemContentRequestBuilder
  {
    public DriveItemContentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveItemContentRequest Request(IEnumerable<Option> options = null)
    {
      return (IDriveItemContentRequest) new DriveItemContentRequest(this.RequestUrl, this.Client, options);
    }
  }
}
