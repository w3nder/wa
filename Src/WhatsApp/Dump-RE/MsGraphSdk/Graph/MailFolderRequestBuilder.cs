// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderRequestBuilder : 
    EntityRequestBuilder,
    IMailFolderRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public MailFolderRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IMailFolderRequest Request() => this.Request((IEnumerable<Option>) null);

    public IMailFolderRequest Request(IEnumerable<Option> options)
    {
      return (IMailFolderRequest) new MailFolderRequest(this.RequestUrl, this.Client, options);
    }

    public IMailFolderMessagesCollectionRequestBuilder Messages
    {
      get
      {
        return (IMailFolderMessagesCollectionRequestBuilder) new MailFolderMessagesCollectionRequestBuilder(this.AppendSegmentToRequestUrl("messages"), this.Client);
      }
    }

    public IMailFolderChildFoldersCollectionRequestBuilder ChildFolders
    {
      get
      {
        return (IMailFolderChildFoldersCollectionRequestBuilder) new MailFolderChildFoldersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("childFolders"), this.Client);
      }
    }

    public IMailFolderCopyRequestBuilder Copy(string DestinationId = null)
    {
      return (IMailFolderCopyRequestBuilder) new MailFolderCopyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.copy"), this.Client, DestinationId);
    }

    public IMailFolderMoveRequestBuilder Move(string DestinationId = null)
    {
      return (IMailFolderMoveRequestBuilder) new MailFolderMoveRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.move"), this.Client, DestinationId);
    }
  }
}
