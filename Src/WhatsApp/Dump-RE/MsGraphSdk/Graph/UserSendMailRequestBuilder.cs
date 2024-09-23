// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserSendMailRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserSendMailRequestBuilder : 
    BasePostMethodRequestBuilder<IUserSendMailRequest>,
    IUserSendMailRequestBuilder
  {
    public UserSendMailRequestBuilder(
      string requestUrl,
      IBaseClient client,
      Message Message,
      bool? SaveToSentItems)
      : base(requestUrl, client)
    {
      this.SetParameter<Message>("message", Message, false);
      this.SetParameter<bool?>("saveToSentItems", SaveToSentItems, true);
    }

    protected override IUserSendMailRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      UserSendMailRequest request = new UserSendMailRequest(functionUrl, this.Client, options);
      if (this.HasParameter("message"))
        request.RequestBody.Message = this.GetParameter<Message>("message");
      if (this.HasParameter("saveToSentItems"))
        request.RequestBody.SaveToSentItems = this.GetParameter<bool?>("saveToSentItems");
      return (IUserSendMailRequest) request;
    }
  }
}
