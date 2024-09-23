// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageExtensionsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageExtensionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IMessageExtensionsCollectionRequestBuilder
  {
    public MessageExtensionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMessageExtensionsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IMessageExtensionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IMessageExtensionsCollectionRequest) new MessageExtensionsCollectionRequest(this.RequestUrl, this.Client, options);
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
