// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.SubscriptionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class SubscriptionRequestBuilder : 
    EntityRequestBuilder,
    ISubscriptionRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public SubscriptionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public ISubscriptionRequest Request() => this.Request((IEnumerable<Option>) null);

    public ISubscriptionRequest Request(IEnumerable<Option> options)
    {
      return (ISubscriptionRequest) new SubscriptionRequest(this.RequestUrl, this.Client, options);
    }
  }
}
