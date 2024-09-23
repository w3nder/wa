// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserContactsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserContactsCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserContactsCollectionRequestBuilder
  {
    public UserContactsCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserContactsCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserContactsCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserContactsCollectionRequest) new UserContactsCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IContactRequestBuilder this[string id]
    {
      get
      {
        return (IContactRequestBuilder) new ContactRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
