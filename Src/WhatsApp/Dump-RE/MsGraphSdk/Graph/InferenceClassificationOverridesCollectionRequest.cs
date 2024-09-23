// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationOverridesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationOverridesCollectionRequest : 
    BaseRequest,
    IInferenceClassificationOverridesCollectionRequest,
    IBaseRequest
  {
    public InferenceClassificationOverridesCollectionRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<InferenceClassificationOverride> AddAsync(
      InferenceClassificationOverride inferenceClassificationOverride)
    {
      return this.AddAsync(inferenceClassificationOverride, CancellationToken.None);
    }

    public Task<InferenceClassificationOverride> AddAsync(
      InferenceClassificationOverride inferenceClassificationOverride,
      CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      return this.SendAsync<InferenceClassificationOverride>((object) inferenceClassificationOverride, cancellationToken);
    }

    public Task<IInferenceClassificationOverridesCollectionPage> GetAsync()
    {
      return this.GetAsync(CancellationToken.None);
    }

    public async Task<IInferenceClassificationOverridesCollectionPage> GetAsync(
      CancellationToken cancellationToken)
    {
      this.Method = "GET";
      InferenceClassificationOverridesCollectionResponse collectionResponse = await this.SendAsync<InferenceClassificationOverridesCollectionResponse>((object) null, cancellationToken).ConfigureAwait(false);
      if (collectionResponse == null || collectionResponse.Value == null || collectionResponse.Value.CurrentPage == null)
        return (IInferenceClassificationOverridesCollectionPage) null;
      if (collectionResponse.AdditionalData != null)
      {
        object obj;
        collectionResponse.AdditionalData.TryGetValue("@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          collectionResponse.Value.InitializeNextPageRequest(this.Client, nextPageLinkString);
        collectionResponse.Value.AdditionalData = collectionResponse.AdditionalData;
      }
      return collectionResponse.Value;
    }

    public IInferenceClassificationOverridesCollectionRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }

    public IInferenceClassificationOverridesCollectionRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }

    public IInferenceClassificationOverridesCollectionRequest Top(int value)
    {
      this.QueryOptions.Add(new QueryOption("$top", value.ToString()));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }

    public IInferenceClassificationOverridesCollectionRequest Filter(string value)
    {
      this.QueryOptions.Add(new QueryOption("$filter", value));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }

    public IInferenceClassificationOverridesCollectionRequest Skip(int value)
    {
      this.QueryOptions.Add(new QueryOption("$skip", value.ToString()));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }

    public IInferenceClassificationOverridesCollectionRequest OrderBy(string value)
    {
      this.QueryOptions.Add(new QueryOption("$orderby", value));
      return (IInferenceClassificationOverridesCollectionRequest) this;
    }
  }
}
