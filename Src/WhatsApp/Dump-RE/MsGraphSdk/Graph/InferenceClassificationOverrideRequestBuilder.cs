// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InferenceClassificationOverrideRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class InferenceClassificationOverrideRequestBuilder : 
    EntityRequestBuilder,
    IInferenceClassificationOverrideRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public InferenceClassificationOverrideRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IInferenceClassificationOverrideRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IInferenceClassificationOverrideRequest Request(IEnumerable<Option> options)
    {
      return (IInferenceClassificationOverrideRequest) new InferenceClassificationOverrideRequest(this.RequestUrl, this.Client, options);
    }
  }
}
