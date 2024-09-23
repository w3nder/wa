// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserWithReferenceRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserWithReferenceRequestBuilder : BaseRequestBuilder, IUserWithReferenceRequestBuilder
  {
    public UserWithReferenceRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserWithReferenceRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserWithReferenceRequest Request(IEnumerable<Option> options)
    {
      return (IUserWithReferenceRequest) new UserWithReferenceRequest(this.RequestUrl, this.Client, options);
    }

    public IUserReferenceRequestBuilder Reference
    {
      get
      {
        return (IUserReferenceRequestBuilder) new UserReferenceRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
