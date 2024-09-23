// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IPostRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IPostRequestBuilder : 
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IPostRequest Request();

    IPostRequest Request(IEnumerable<Option> options);

    IPostExtensionsCollectionRequestBuilder Extensions { get; }

    IPostRequestBuilder InReplyTo { get; }

    IPostAttachmentsCollectionRequestBuilder Attachments { get; }

    IPostForwardRequestBuilder Forward(IEnumerable<Recipient> ToRecipients, string Comment = null);

    IPostReplyRequestBuilder Reply(Post Post);
  }
}
