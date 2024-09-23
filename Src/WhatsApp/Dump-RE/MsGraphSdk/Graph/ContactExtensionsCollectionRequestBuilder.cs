// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactExtensionsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactExtensionsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IContactExtensionsCollectionRequestBuilder
  {
    public ContactExtensionsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IContactExtensionsCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IContactExtensionsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IContactExtensionsCollectionRequest) new ContactExtensionsCollectionRequest(this.RequestUrl, this.Client, options);
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
