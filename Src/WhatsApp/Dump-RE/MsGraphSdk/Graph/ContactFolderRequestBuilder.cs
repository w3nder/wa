// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderRequestBuilder : 
    EntityRequestBuilder,
    IContactFolderRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ContactFolderRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IContactFolderRequest Request() => this.Request((IEnumerable<Option>) null);

    public IContactFolderRequest Request(IEnumerable<Option> options)
    {
      return (IContactFolderRequest) new ContactFolderRequest(this.RequestUrl, this.Client, options);
    }

    public IContactFolderContactsCollectionRequestBuilder Contacts
    {
      get
      {
        return (IContactFolderContactsCollectionRequestBuilder) new ContactFolderContactsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("contacts"), this.Client);
      }
    }

    public IContactFolderChildFoldersCollectionRequestBuilder ChildFolders
    {
      get
      {
        return (IContactFolderChildFoldersCollectionRequestBuilder) new ContactFolderChildFoldersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("childFolders"), this.Client);
      }
    }
  }
}
