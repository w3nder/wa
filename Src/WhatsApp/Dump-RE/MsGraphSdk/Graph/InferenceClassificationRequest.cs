// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationRequest : 
    BaseRequest,
    IInferenceClassificationRequest,
    IBaseRequest
  {
    public InferenceClassificationRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<InferenceClassification> CreateAsync(
      InferenceClassification inferenceClassificationToCreate)
    {
      return this.CreateAsync(inferenceClassificationToCreate, CancellationToken.None);
    }

    public async Task<InferenceClassification> CreateAsync(
      InferenceClassification inferenceClassificationToCreate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      InferenceClassification inferenceClassificationToInitialize = await this.SendAsync<InferenceClassification>((object) inferenceClassificationToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationToInitialize);
      return inferenceClassificationToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      InferenceClassification inferenceClassification = await this.SendAsync<InferenceClassification>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<InferenceClassification> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<InferenceClassification> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      InferenceClassification inferenceClassificationToInitialize = await this.SendAsync<InferenceClassification>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationToInitialize);
      return inferenceClassificationToInitialize;
    }

    public Task<InferenceClassification> UpdateAsync(
      InferenceClassification inferenceClassificationToUpdate)
    {
      return this.UpdateAsync(inferenceClassificationToUpdate, CancellationToken.None);
    }

    public async Task<InferenceClassification> UpdateAsync(
      InferenceClassification inferenceClassificationToUpdate,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      InferenceClassification inferenceClassificationToInitialize = await this.SendAsync<InferenceClassification>((object) inferenceClassificationToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(inferenceClassificationToInitialize);
      return inferenceClassificationToInitialize;
    }

    public IInferenceClassificationRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IInferenceClassificationRequest) this;
    }

    public IInferenceClassificationRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IInferenceClassificationRequest) this;
    }

    private void InitializeCollectionProperties(
      InferenceClassification inferenceClassificationToInitialize)
    {
      if (inferenceClassificationToInitialize == null || inferenceClassificationToInitialize.AdditionalData == null || inferenceClassificationToInitialize.Overrides == null || inferenceClassificationToInitialize.Overrides.CurrentPage == null)
        return;
      inferenceClassificationToInitialize.Overrides.AdditionalData = inferenceClassificationToInitialize.AdditionalData;
      object obj;
      inferenceClassificationToInitialize.AdditionalData.TryGetValue("overrides@odata.nextLink", out obj);
      string nextPageLinkString = obj as string;
      if (string.IsNullOrEmpty(nextPageLinkString))
        return;
      inferenceClassificationToInitialize.Overrides.InitializeNextPageRequest(this.Client, nextPageLinkString);
    }
  }
}
