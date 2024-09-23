// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationOverrideRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationOverrideRequest : 
    BaseRequest,
    IInferenceClassificationOverrideRequest,
    IBaseRequest
  {
    public InferenceClassificationOverrideRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<InferenceClassificationOverride> CreateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToCreate)
    {
      return this.CreateAsync(inferenceClassificationOverrideToCreate, CancellationToken.None);
    }

    public async Task<InferenceClassificationOverride> CreateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      InferenceClassificationOverride inferenceClassificationOverrideToInitialize = await this.SendAsync<InferenceClassificationOverride>((object) inferenceClassificationOverrideToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationOverrideToInitialize);
      return inferenceClassificationOverrideToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      InferenceClassificationOverride classificationOverride = await this.SendAsync<InferenceClassificationOverride>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<InferenceClassificationOverride> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<InferenceClassificationOverride> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      InferenceClassificationOverride inferenceClassificationOverrideToInitialize = await this.SendAsync<InferenceClassificationOverride>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationOverrideToInitialize);
      return inferenceClassificationOverrideToInitialize;
    }

    public Task<InferenceClassificationOverride> UpdateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToUpdate)
    {
      return this.UpdateAsync(inferenceClassificationOverrideToUpdate, CancellationToken.None);
    }

    public async Task<InferenceClassificationOverride> UpdateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      InferenceClassificationOverride inferenceClassificationOverrideToInitialize = await this.SendAsync<InferenceClassificationOverride>((object) inferenceClassificationOverrideToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationOverrideToInitialize);
      return inferenceClassificationOverrideToInitialize;
    }

    public IInferenceClassificationOverrideRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IInferenceClassificationOverrideRequest) this;
    }

    public IInferenceClassificationOverrideRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IInferenceClassificationOverrideRequest) this;
    }

    private void InitializeCollectionProperties(
      InferenceClassificationOverride inferenceClassificationOverrideToInitialize)
    {
    }
  }
}
