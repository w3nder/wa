// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderMessagesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderMessagesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IMailFolderMessagesCollectionRequestBuilder
  {
    public MailFolderMessagesCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMailFolderMessagesCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IMailFolderMessagesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IMailFolderMessagesCollectionRequest) new MailFolderMessagesCollectionRequest(this.RequestUrl, this.Client, options);
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
