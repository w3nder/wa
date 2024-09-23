// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectReferenceRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectReferenceRequestBuilder : 
    BaseRequestBuilder,
    IDirectoryObjectReferenceRequestBuilder
  {
    public DirectoryObjectReferenceRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryObjectReferenceRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDirectoryObjectReferenceRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryObjectReferenceRequest) new DirectoryObjectReferenceRequest(this.RequestUrl, this.Client, options);
    }
  }
}
