// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.BaseRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public class BaseRequestBuilder
  {
    public BaseRequestBuilder(string requestUrl, IBaseClient client)
    {
      this.Client = client;
      this.RequestUrl = requestUrl;
    }

    public IBaseClient Client { get; private set; }

    public string RequestUrl { get; internal set; }

    public string AppendSegmentToRequestUrl(string urlSegment)
    {
      return string.Format("{0}/{1}", (object) this.RequestUrl, (object) urlSegment);
    }
  }
}
