// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IBaseRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Net.Http;

#nullable disable
namespace Microsoft.Graph
{
  public interface IBaseRequest
  {
    string ContentType { get; set; }

    IList<HeaderOption> Headers { get; }

    IBaseClient Client { get; }

    string Method { get; }

    string RequestUrl { get; }

    IList<QueryOption> QueryOptions { get; }

    HttpRequestMessage GetHttpRequestMessage();
  }
}
