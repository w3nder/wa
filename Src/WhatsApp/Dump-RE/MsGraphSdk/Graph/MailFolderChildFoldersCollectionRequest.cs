// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderChildFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderChildFoldersCollectionRequest : 
    BaseRequest,
    IMailFolderChildFoldersCollectionRequest,
    IBaseRequest
  {
    public MailFolderChildFoldersCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<MailFolder> AddAsync(MailFolder mailFolder)
    {
      return this.AddAsync(mailFolder, CancellationToken.None);
    }

    public Task<MailFolder> AddAsync(MailFolder mailFolder, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<MailFolder>((object) mailFolder, cancellationToken);
    }

    public Task<IMailFolderChildFoldersCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IMailFolderChildFoldersCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      MailFolderChildFoldersCollectionResponse collectionResponse = await this.SendAsync<MailFolderChildFoldersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IMailFolderChildFoldersCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
      }
      return collectionResponse.Value;
    }

    public IMailFolderChildFoldersCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }

    public IMailFolderChildFoldersCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }

    public IMailFolderChildFoldersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }

    public IMailFolderChildFoldersCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }

    public IMailFolderChildFoldersCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }

    public IMailFolderChildFoldersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IMailFolderChildFoldersCollectionRequest) this;
    }
  }
}
