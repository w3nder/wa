// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserOwnedDevicesCollectionWithReferencesRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserOwnedDevicesCollectionWithReferencesRequest : 
    BaseRequest,
    IUserOwnedDevicesCollectionWithReferencesRequest,
    IBaseRequest
  {
    public UserOwnedDevicesCollectionWithReferencesRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<IUserOwnedDevicesCollectionWithReferencesPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IUserOwnedDevicesCollectionWithReferencesPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      UserOwnedDevicesCollectionWithReferencesResponse referencesResponse = await this.SendAsync<UserOwnedDevicesCollectionWithReferencesResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (referencesResponse == null || referencesResponse.Value == null || referencesResponse.Value.CurrentPage == null)
        return (IUserOwnedDevicesCollectionWithReferencesPage) null;
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

    public IUserOwnedDevicesCollectionWithReferencesRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IUserOwnedDevicesCollectionWithReferencesRequest) this;
    }

    public IUserOwnedDevicesCollectionWithReferencesRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IUserOwnedDevicesCollectionWithReferencesRequest) this;
    }
  }
}
