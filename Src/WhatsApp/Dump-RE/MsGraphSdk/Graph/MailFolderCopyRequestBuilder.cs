// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderCopyRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderCopyRequestBuilder : 
    BasePostMethodRequestBuilder<IMailFolderCopyRequest>,
    IMailFolderCopyRequestBuilder
  {
    public MailFolderCopyRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string DestinationId)
      : base(requestUrl, client)
    {
      this.SetParameter<string>("destinationId", DestinationId, true);
    }

    protected override IMailFolderCopyRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      MailFolderCopyRequest request = new MailFolderCopyRequest(functionUrl, this.Client, options);
      if (this.HasParameter("destinationId"))
        request.RequestBody.DestinationId = this.GetParameter<string>("destinationId");
      return (IMailFolderCopyRequest) request;
    }
  }
}
