// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMailFoldersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMailFoldersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserMailFoldersCollectionRequestBuilder
  {
    public IMailFolderRequestBuilder DeletedItems
    {
      get
      {
        return (IMailFolderRequestBuilder) new MailFolderRequestBuilder(this.AppendSegmentToRequestUrl(nameof (DeletedItems)), this.Client);
      }
    }

    public IMailFolderRequestBuilder Drafts
    {
      get
      {
        return (IMailFolderRequestBuilder) new MailFolderRequestBuilder(this.AppendSegmentToRequestUrl(nameof (Drafts)), this.Client);
      }
    }

    public IMailFolderRequestBuilder Inbox
    {
      get
      {
        return (IMailFolderRequestBuilder) new MailFolderRequestBuilder(this.AppendSegmentToRequestUrl(nameof (Inbox)), this.Client);
      }
    }

    public IMailFolderRequestBuilder SentItems
    {
      get
      {
        return (IMailFolderRequestBuilder) new MailFolderRequestBuilder(this.AppendSegmentToRequestUrl(nameof (SentItems)), this.Client);
      }
    }

    public UserMailFoldersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserMailFoldersCollectionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserMailFoldersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserMailFoldersCollectionRequest) new UserMailFoldersCollectionRequest(this.RequestUrl, this.Client, options);
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
