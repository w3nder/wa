// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserCreatedObjectsCollectionWithReferencesRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserCreatedObjectsCollectionWithReferencesRequest : 
    BaseRequest,
    IUserCreatedObjectsCollectionWithReferencesRequest,
    IBaseRequest
  {
    public UserCreatedObjectsCollectionWithReferencesRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<IUserCreatedObjectsCollectionWithReferencesPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IUserCreatedObjectsCollectionWithReferencesPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserCreatedObjectsCollectionWithReferencesResponse referencesResponse = await this.SendAsync<UserCreatedObjectsCollectionWithReferencesResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (referencesResponse == null || referencesResponse.Value == null || referencesResponse.Value.CurrentPage == null)
        return (IUserCreatedObjectsCollectionWithReferencesPage) null;
      if (referencesResponse.AdditionalData != null)
      {
        object obj;
        referencesResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          referencesResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
        referencesResponse.Value.AdditionalData = referencesResponse.AdditionalData;
      }
      return referencesResponse.Value;
    }

    public IUserCreatedObjectsCollectionWithReferencesRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserCreatedObjectsCollectionWithReferencesRequest) this;
    }

    public IUserCreatedObjectsCollectionWithReferencesRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserCreatedObjectsCollectionWithReferencesRequest) this;
    }
  }
}
