// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EventExtensionsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class EventExtensionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IEventExtensionsCollectionRequestBuilder
  {
    public EventExtensionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IEventExtensionsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IEventExtensionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IEventExtensionsCollectionRequest) new EventExtensionsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IExtensionRequestBuilder this[string id]
    {
      get
      {
        return (IExtensionRequestBuilder) new ExtensionRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
