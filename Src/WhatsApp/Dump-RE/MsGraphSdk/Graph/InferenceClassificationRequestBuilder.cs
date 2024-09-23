// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationRequestBuilder : 
    EntityRequestBuilder,
    IInferenceClassificationRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public InferenceClassificationRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IInferenceClassificationRequest Request() => this.Request((IEnumerable<Option>) null);

    public IInferenceClassificationRequest Request(IEnumerable<Option> options)
    {
      return (IInferenceClassificationRequest) new InferenceClassificationRequest(this.RequestUrl, this.Client, options);
    }

    public IInferenceClassificationOverridesCollectionRequestBuilder Overrides
    {
      get
      {
        return (IInferenceClassificationOverridesCollectionRequestBuilder) new InferenceClassificationOverridesCollectionRequestBuilder(this.AppendSegmentToRequestUrl("overrides"), this.Client);
      }
    }
  }
}
