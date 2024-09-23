// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ContactFolderChildFoldersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ContactFolderChildFoldersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IContactFolderChildFoldersCollectionRequestBuilder
  {
    public ContactFolderChildFoldersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IContactFolderChildFoldersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IContactFolderChildFoldersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IContactFolderChildFoldersCollectionRequest) new ContactFolderChildFoldersCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IContactFolderRequestBuilder this[string id]
    {
      get
      {
        return (IContactFolderRequestBuilder) new ContactFolderRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
