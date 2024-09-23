// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationOverridesCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationOverridesCollectionRequestBuilder : 
    BaseRequestBuilder,
    IInferenceClassificationOverridesCollectionRequestBuilder
  {
    public InferenceClassificationOverridesCollectionRequestBuilder(
      string requestUrl,
      IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IInferenceClassificationOverridesCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IInferenceClassificationOverridesCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IInferenceClassificationOverridesCollectionRequest) new InferenceClassificationOverridesCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IInferenceClassificationOverrideRequestBuilder this[string id]
    {
      get
      {
        return (IInferenceClassificationOverrideRequestBuilder) new InferenceClassificationOverrideRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
