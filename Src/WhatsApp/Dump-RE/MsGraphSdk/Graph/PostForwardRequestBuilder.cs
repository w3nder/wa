// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PostForwardRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class PostForwardRequestBuilder : 
    BasePostMethodRequestBuilder<IPostForwardRequest>,
    IPostForwardRequestBuilder
  {
    public PostForwardRequestBuilder(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Recipient> ToRecipients,
      string Comment)
      : base(requestUrl, client)
    {
      this.SetParameter<IEnumerable<Recipient>>("toRecipients", ToRecipients, false);
      this.SetParameter<string>("comment", Comment, true);
    }

    protected override IPostForwardRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      PostForwardRequest request = new PostForwardRequest(functionUrl, this.Client, options);
      if (this.HasParameter("toRecipients"))
        request.RequestBody.ToRecipients = this.GetParameter<IEnumerable<Recipient>>("toRecipients");
      if (this.HasParameter("comment"))
        request.RequestBody.Comment = this.GetParameter<string>("comment");
      return (IPostForwardRequest) request;
    }
  }
}
