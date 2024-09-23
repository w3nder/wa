// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DirectoryObjectWithReferenceRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DirectoryObjectWithReferenceRequestBuilder : 
    BaseRequestBuilder,
    IDirectoryObjectWithReferenceRequestBuilder
  {
    public DirectoryObjectWithReferenceRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDirectoryObjectWithReferenceRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IDirectoryObjectWithReferenceRequest Request(IEnumerable<Option> options)
    {
      return (IDirectoryObjectWithReferenceRequest) new DirectoryObjectWithReferenceRequest(this.RequestUrl, this.Client, options);
    }

    public IDirectoryObjectReferenceRequestBuilder Reference
    {
      get
      {
        return (IDirectoryObjectReferenceRequestBuilder) new DirectoryObjectReferenceRequestBuilder(this.AppendSegmentToRequestUrl("$ref"), this.Client);
      }
    }
  }
}
