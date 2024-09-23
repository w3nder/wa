// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.OpenTypeExtensionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class OpenTypeExtensionRequestBuilder : 
    ExtensionRequestBuilder,
    IOpenTypeExtensionRequestBuilder,
    IExtensionRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public OpenTypeExtensionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IOpenTypeExtensionRequest Request() => this.Request((IEnumerable<Option>) null);

    public IOpenTypeExtensionRequest Request(IEnumerable<Option> options)
    {
      return (IOpenTypeExtensionRequest) new OpenTypeExtensionRequest(this.RequestUrl, this.Client, options);
    }
  }
}
