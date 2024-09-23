// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemRequest : BaseRequest, IDriveItemRequest, IBaseRequest
  {
    public DriveItemRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<DriveItem> CreateAsync(DriveItem driveItemToCreate)
    {
      return this.CreateAsync(driveItemToCreate, CancellationToken.None);
    }

    public async Task<DriveItem> CreateAsync(
      DriveItem driveItemToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      DriveItem driveItemToInitialize = await this.SendAsync<DriveItem>((object) driveItemToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveItemToInitialize);
      return driveItemToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      DriveItem driveItem = await this.SendAsync<DriveItem>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<DriveItem> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<DriveItem> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      DriveItem driveItemToInitialize = await this.SendAsync<DriveItem>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveItemToInitialize);
      return driveItemToInitialize;
    }

    public Task<DriveItem> UpdateAsync(DriveItem driveItemToUpdate)
    {
      return this.UpdateAsync(driveItemToUpdate, CancellationToken.None);
    }

    public async Task<DriveItem> UpdateAsync(
      DriveItem driveItemToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      DriveItem driveItemToInitialize = await this.SendAsync<DriveItem>((object) driveItemToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveItemToInitialize);
      return driveItemToInitialize;
    }

    public IDriveItemRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveItemRequest) this;
    }

    public IDriveItemRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveItemRequest) this;
    }

    private void InitializeCollectionProperties(DriveItem driveItemToInitialize)
    {
      if (driveItemToInitialize == null || driveItemToInitialize.AdditionalData == null)
        return;
      if (driveItemToInitialize.Permissions != null && driveItemToInitialize.Permissions.CurrentPage != null)
      {
        driveItemToInitialize.Permissions.AdditionalData = driveItemToInitialize.AdditionalData;
        object obj;
        driveItemToInitialize.AdditionalData.TryGetValue("permissions@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          driveItemToInitialize.Permissions.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (driveItemToInitialize.Children != null && driveItemToInitialize.Children.CurrentPage != null)
      {
        driveItemToInitialize.Children.AdditionalData = driveItemToInitialize.AdditionalData;
        object obj;
        driveItemToInitialize.AdditionalData.TryGetValue("children@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          driveItemToInitialize.Children.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (driveItemToInitialize.Thumbnails == null || driveItemToInitialize.Thumbnails.CurrentPage == null)
        return;
      driveItemToInitialize.Thumbnails.AdditionalData = driveItemToInitialize.AdditionalData;
      object obj1;
      driveItemToInitialize.AdditionalData.TryGetValue("thumbnails@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      driveItemToInitialize.Thumbnails.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
