// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderChildFoldersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderChildFoldersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IMailFolderChildFoldersCollectionRequestBuilder
  {
    public MailFolderChildFoldersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMailFolderChildFoldersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IMailFolderChildFoldersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IMailFolderChildFoldersCollectionRequest) new MailFolderChildFoldersCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IMailFolderRequestBuilder this[string id]
    {
      get
      {
        return (IMailFolderRequestBuilder) new MailFolderRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
