// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AttachmentRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class AttachmentRequestBuilder : 
    EntityRequestBuilder,
    IAttachmentRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public AttachmentRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IAttachmentRequest Request() => this.Request((IEnumerable<Option>) null);

    public IAttachmentRequest Request(IEnumerable<Option> options)
    {
      return (IAttachmentRequest) new AttachmentRequest(this.RequestUrl, this.Client, options);
    }
  }
}
