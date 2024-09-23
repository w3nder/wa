// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemDeltaRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemDeltaRequest : BaseRequest, IDriveItemDeltaRequest, IBaseRequest
  {
    public DriveItemDeltaRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "GET";
    }

    public Task<IDriveItemDeltaCollectionPage> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<IDriveItemDeltaCollectionPage> GetAsync(CancellationToken cancellationToken)
    {
      DriveItemDeltaCollectionResponse collectionResponse = await this.SendAsync<DriveItemDeltaCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IDriveItemDeltaCollectionPage) null;
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

    public IDriveItemDeltaRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemDeltaRequest) this;
    }

    public IDriveItemDeltaRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemDeltaRequest) this;
    }

    public IDriveItemDeltaRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IDriveItemDeltaRequest) this;
    }

    public IDriveItemDeltaRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IDriveItemDeltaRequest) this;
    }

    public IDriveItemDeltaRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IDriveItemDeltaRequest) this;
    }

    public IDriveItemDeltaRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IDriveItemDeltaRequest) this;
    }
  }
}
