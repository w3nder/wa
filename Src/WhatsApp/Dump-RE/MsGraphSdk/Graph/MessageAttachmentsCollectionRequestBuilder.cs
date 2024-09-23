// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageAttachmentsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageAttachmentsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IMessageAttachmentsCollectionRequestBuilder
  {
    public MessageAttachmentsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMessageAttachmentsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IMessageAttachmentsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IMessageAttachmentsCollectionRequest) new MessageAttachmentsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IAttachmentRequestBuilder this[string id]
    {
      get
      {
        return (IAttachmentRequestBuilder) new AttachmentRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
