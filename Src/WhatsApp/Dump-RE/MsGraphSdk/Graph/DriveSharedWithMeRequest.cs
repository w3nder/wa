// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveSharedWithMeRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveSharedWithMeRequest : BaseRequest, IDriveSharedWithMeRequest, IBaseRequest
  {
    public DriveSharedWithMeRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "GET";
    }

    public Task<IDriveSharedWithMeCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IDriveSharedWithMeCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      DriveSharedWithMeCollectionResponse collectionResponse = await this.SendAsync<DriveSharedWithMeCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveSharedWithMeCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      return collectionResponse.Value;
    }

    public IDriveSharedWithMeRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveSharedWithMeRequest) this;
    }

    public IDriveSharedWithMeRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveSharedWithMeRequest) this;
    }

    public IDriveSharedWithMeRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveSharedWithMeRequest) this;
    }

    public IDriveSharedWithMeRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveSharedWithMeRequest) this;
    }

    public IDriveSharedWithMeRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveSharedWithMeRequest) this;
    }

    public IDriveSharedWithMeRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveSharedWithMeRequest) this;
    }
  }
}
