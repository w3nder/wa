// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ReferenceAttachmentRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ReferenceAttachmentRequestBuilder : 
    AttachmentRequestBuilder,
    IReferenceAttachmentRequestBuilder,
    IAttachmentRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ReferenceAttachmentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IReferenceAttachmentRequest Request() => this.Request((IEnumerable<Option>) null);

    public IReferenceAttachmentRequest Request(IEnumerable<Option> options)
    {
      return (IReferenceAttachmentRequest) new ReferenceAttachmentRequest(this.RequestUrl, this.Client, options);
    }
  }
}
