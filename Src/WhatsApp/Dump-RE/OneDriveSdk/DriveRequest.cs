// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.DriveRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class DriveRequest : BaseRequest, IDriveRequest, IBaseRequest
  {
    public DriveRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.SdkVersionHeaderPrefix = "onedrive";
    }

    public Task<Drive> CreateAsync(Drive driveToCreate)
    {
      return this.CreateAsync(driveToCreate, CancellationToken.None);
    }

    public async Task<Drive> CreateAsync(Drive driveToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Drive driveToInitialize = await this.SendAsync<Drive>((object) driveToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveToInitialize);
      return driveToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Drive drive = await this.SendAsync<Drive>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Drive> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Drive> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Drive driveToInitialize = await this.SendAsync<Drive>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveToInitialize);
      return driveToInitialize;
    }

    public Task<Drive> UpdateAsync(Drive driveToUpdate)
    {
      return this.UpdateAsync(driveToUpdate, CancellationToken.None);
    }

    public async Task<Drive> UpdateAsync(Drive driveToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Drive driveToInitialize = await this.SendAsync<Drive>((object) driveToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(driveToInitialize);
      return driveToInitialize;
    }

    public IDriveRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IDriveRequest) this;
    }

    public IDriveRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IDriveRequest) this;
    }

    private void InitializeCollectionProperties(Drive driveToInitialize)
    {
      if (driveToInitialize == null || driveToInitialize.AdditionalData == null)
        return;
      if (driveToInitialize.Items != null && driveToInitialize.Items.CurrentPage != null)
      {
        driveToInitialize.Items.AdditionalData = driveToInitialize.AdditionalData;
        object obj;
        driveToInitialize.AdditionalData.TryGetValue("items@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          driveToInitialize.Items.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (driveToInitialize.Shared != null && driveToInitialize.Shared.CurrentPage != null)
      {
        driveToInitialize.Shared.AdditionalData = driveToInitialize.AdditionalData;
        object obj;
        driveToInitialize.AdditionalData.TryGetValue("shared@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          driveToInitialize.Shared.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (driveToInitialize.Special == null || driveToInitialize.Special.CurrentPage == null)
        return;
      driveToInitialize.Special.AdditionalData = driveToInitialize.AdditionalData;
      object obj1;
      driveToInitialize.AdditionalData.TryGetValue("special@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      driveToInitialize.Special.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
