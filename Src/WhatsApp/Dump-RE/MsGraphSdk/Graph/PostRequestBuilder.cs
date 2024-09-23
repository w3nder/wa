// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PostRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class PostRequestBuilder : 
    OutlookItemRequestBuilder,
    IPostRequestBuilder,
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public PostRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IPostRequest Request() => this.Request((IEnumerable<Option>) null);

    public IPostRequest Request(IEnumerable<Option> options)
    {
      return (IPostRequest) new PostRequest(this.RequestUrl, this.Client, options);
    }

    public IPostExtensionsCollectionRequestBuilder Extensions
    {
      get
      {
        return (IPostExtensionsCollectionRequestBuilder) new PostExtensionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("extensions"), this.Client);
      }
    }

    public IPostRequestBuilder InReplyTo
    {
      get
      {
        return (IPostRequestBuilder) new PostRequestBuilder(this.AppendSegmentToRequestUrl("inReplyTo"), this.Client);
      }
    }

    public IPostAttachmentsCollectionRequestBuilder Attachments
    {
      get
      {
        return (IPostAttachmentsCollectionRequestBuilder) new PostAttachmentsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("attachments"), this.Client);
      }
    }

    public IPostForwardRequestBuilder Forward(IEnumerable<Recipient> ToRecipients, string Comment = null)
    {
      return (IPostForwardRequestBuilder) new PostForwardRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.forward"), this.Client, ToRecipients, Comment);
    }

    public IPostReplyRequestBuilder Reply(Post Post)
    {
      return (IPostReplyRequestBuilder) new PostReplyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.reply"), this.Client, Post);
    }
  }
}
