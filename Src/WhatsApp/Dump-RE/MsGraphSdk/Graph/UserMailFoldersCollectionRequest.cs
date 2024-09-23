// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserMailFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserMailFoldersCollectionRequest : 
    BaseRequest,
    IUserMailFoldersCollectionRequest,
    IBaseRequest
  {
    public UserMailFoldersCollectionRequest(
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

    public Task<IUserMailFoldersCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IUserMailFoldersCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserMailFoldersCollectionResponse collectionResponse = await this.SendAsync<UserMailFoldersCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IUserMailFoldersCollectionPage) null;
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

    public IUserMailFoldersCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IUserMailFoldersCollectionRequest) this;
    }

    public IUserMailFoldersCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IUserMailFoldersCollectionRequest) this;
    }

    public IUserMailFoldersCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserMailFoldersCollectionRequest) this;
    }

    public IUserMailFoldersCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IUserMailFoldersCollectionRequest) this;
    }

    public IUserMailFoldersCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IUserMailFoldersCollectionRequest) this;
    }

    public IUserMailFoldersCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserMailFoldersCollectionRequest) this;
    }
  }
}
