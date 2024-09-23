// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMessagesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMessagesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserMessagesCollectionRequestBuilder
  {
    public UserMessagesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserMessagesCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserMessagesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserMessagesCollectionRequest) new UserMessagesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IMessageRequestBuilder this[string id]
    {
      get
      {
        return (IMessageRequestBuilder) new MessageRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
