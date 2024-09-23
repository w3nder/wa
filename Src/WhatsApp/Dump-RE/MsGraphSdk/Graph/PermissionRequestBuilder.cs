// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.PermissionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class PermissionRequestBuilder : 
    EntityRequestBuilder,
    IPermissionRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public PermissionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IPermissionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IPermissionRequest Request(IEnumerable<Option> options)
    {
      return (IPermissionRequest) new PermissionRequest(this.RequestUrl, this.Client, options);
    }
  }
}
