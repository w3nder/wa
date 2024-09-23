// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.FileAttachmentRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class FileAttachmentRequestBuilder : 
    AttachmentRequestBuilder,
    IFileAttachmentRequestBuilder,
    IAttachmentRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public FileAttachmentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IFileAttachmentRequest Request() => this.Request((IEnumerable<Option>) null);

    public IFileAttachmentRequest Request(IEnumerable<Option> options)
    {
      return (IFileAttachmentRequest) new FileAttachmentRequest(this.RequestUrl, this.Client, options);
    }
  }
}
