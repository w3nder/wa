// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ThumbnailRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ThumbnailRequestBuilder : 
    BaseRequestBuilder,
    IThumbnailRequestBuilder,
    IBaseRequestBuilder
  {
    public ThumbnailRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IThumbnailRequest Request() => this.Request((IEnumerable<Option>) null);

    public IThumbnailRequest Request(IEnumerable<Option> options)
    {
      return (IThumbnailRequest) new ThumbnailRequest(this.RequestUrl, this.Client, options);
    }

    public IThumbnailContentRequestBuilder Content
    {
      get
      {
        return (IThumbnailContentRequestBuilder) new ThumbnailContentRequestBuilder(this.AppendSegmentToRequestUrl("content"), this.Client);
      }
    }
  }
}
