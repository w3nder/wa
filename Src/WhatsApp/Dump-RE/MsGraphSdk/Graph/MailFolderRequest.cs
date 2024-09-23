// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MailFolderRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MailFolderRequest : BaseRequest, IMailFolderRequest, IBaseRequest
  {
    public MailFolderRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<MailFolder> CreateAsync(MailFolder mailFolderToCreate)
    {
      return this.CreateAsync(mailFolderToCreate, CancellationToken.None);
    }

    public async Task<MailFolder> CreateAsync(
      MailFolder mailFolderToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      MailFolder mailFolderToInitialize = await this.SendAsync<MailFolder>((object) mailFolderToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(mailFolderToInitialize);
      return mailFolderToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      MailFolder mailFolder = await this.SendAsync<MailFolder>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<MailFolder> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<MailFolder> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      MailFolder mailFolderToInitialize = await this.SendAsync<MailFolder>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(mailFolderToInitialize);
      return mailFolderToInitialize;
    }

    public Task<MailFolder> UpdateAsync(MailFolder mailFolderToUpdate)
    {
      return this.UpdateAsync(mailFolderToUpdate, CancellationToken.None);
    }

    public async Task<MailFolder> UpdateAsync(
      MailFolder mailFolderToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      MailFolder mailFolderToInitialize = await this.SendAsync<MailFolder>((object) mailFolderToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(mailFolderToInitialize);
      return mailFolderToInitialize;
    }

    public IMailFolderRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMailFolderRequest) this;
    }

    public IMailFolderRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMailFolderRequest) this;
    }

    private void InitializeCollectionProperties(MailFolder mailFolderToInitialize)
    {
      if (mailFolderToInitialize == null || mailFolderToInitialize.AdditionalData == null)
        return;
      if (mailFolderToInitialize.Messages != null && mailFolderToInitialize.Messages.CurrentPage != null)
      {
        mailFolderToInitialize.Messages.AdditionalData = mailFolderToInitialize.AdditionalData;
        object obj;
        mailFolderToInitialize.AdditionalData.TryGetValue("messages@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          mailFolderToInitialize.Messages.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (mailFolderToInitialize.ChildFolders == null || mailFolderToInitialize.ChildFolders.CurrentPage == null)
        return;
      mailFolderToInitialize.ChildFolders.AdditionalData = mailFolderToInitialize.AdditionalData;
      object obj1;
      mailFolderToInitialize.AdditionalData.TryGetValue("childFolders@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      mailFolderToInitialize.ChildFolders.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
