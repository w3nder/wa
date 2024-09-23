// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactRequestBuilder : 
    OutlookItemRequestBuilder,
    IContactRequestBuilder,
    IOutlookItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ContactRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IContactRequest Request() => this.Request((IEnumerable<Option>) null);

    public IContactRequest Request(IEnumerable<Option> options)
    {
      return (IContactRequest) new ContactRequest(this.RequestUrl, this.Client, options);
    }

    public IContactExtensionsCollectionRequestBuilder Extensions
    {
      get
      {
        return (IContactExtensionsCollectionRequestBuilder) new ContactExtensionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("extensions"), this.Client);
      }
    }

    public IProfilePhotoRequestBuilder Photo
    {
      get
      {
        return (IProfilePhotoRequestBuilder) new ProfilePhotoRequestBuilder(this.AppendSegmentToRequestUrl("photo"), this.Client);
      }
    }
  }
}
