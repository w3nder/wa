// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpWebResponse
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  internal class HttpWebResponse : WebResponse
  {
    private static Version defaultVersion = new Version(0, 0);
    private System.Net.HttpWebResponse actualResponse;

    private HttpWebResponse(System.Net.HttpWebResponse response, DecompressionMethods decompressionMethods)
      : base((System.Net.WebResponse) response, decompressionMethods)
    {
      this.actualResponse = response;
    }

    public CookieCollection Cookies => this.actualResponse.Cookies;

    public string Method => this.actualResponse.Method;

    public Version ProtocolVersion => HttpWebResponse.defaultVersion;

    public HttpStatusCode StatusCode => this.actualResponse.StatusCode;

    public string StatusDescription => this.actualResponse.StatusDescription;

    public static WebResponse CreateDecorator(
      System.Net.WebResponse webResponse,
      DecompressionMethods decompressionMethods)
    {
      WebResponse decorator = (WebResponse) HttpWebResponse.CreateDecorator(webResponse as System.Net.HttpWebResponse, decompressionMethods);
      if (decorator != null)
        return decorator;
      return webResponse == null ? (WebResponse) null : new WebResponse(webResponse, decompressionMethods);
    }

    public static HttpWebResponse CreateDecorator(
      System.Net.HttpWebResponse httpWebResponse,
      DecompressionMethods decompressionMethods)
    {
      return httpWebResponse == null ? (HttpWebResponse) null : new HttpWebResponse(httpWebResponse, decompressionMethods);
    }
  }
}
