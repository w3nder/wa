// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OutlookItemRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class OutlookItemRequest : BaseRequest, IOutlookItemRequest, IBaseRequest
  {
    public OutlookItemRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<OutlookItem> CreateAsync(OutlookItem outlookItemToCreate)
    {
      return this.CreateAsync(outlookItemToCreate, CancellationToken.None);
    }

    public async Task<OutlookItem> CreateAsync(
      OutlookItem outlookItemToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      OutlookItem outlookItemToInitialize = await this.SendAsync<OutlookItem>((object) outlookItemToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(outlookItemToInitialize);
      return outlookItemToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      OutlookItem outlookItem = await this.SendAsync<OutlookItem>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<OutlookItem> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<OutlookItem> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      OutlookItem outlookItemToInitialize = await this.SendAsync<OutlookItem>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(outlookItemToInitialize);
      return outlookItemToInitialize;
    }

    public Task<OutlookItem> UpdateAsync(OutlookItem outlookItemToUpdate)
    {
      return this.UpdateAsync(outlookItemToUpdate, CancellationToken.None);
    }

    public async Task<OutlookItem> UpdateAsync(
      OutlookItem outlookItemToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      OutlookItem outlookItemToInitialize = await this.SendAsync<OutlookItem>((object) outlookItemToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(outlookItemToInitialize);
      return outlookItemToInitialize;
    }

    public IOutlookItemRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IOutlookItemRequest) this;
    }

    public IOutlookItemRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IOutlookItemRequest) this;
    }

    private void InitializeCollectionProperties(OutlookItem outlookItemToInitialize)
    {
    }
  }
}
