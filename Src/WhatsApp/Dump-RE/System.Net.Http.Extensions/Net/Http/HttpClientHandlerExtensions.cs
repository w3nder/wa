// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpClientHandlerExtensions
// Assembly: System.Net.Http.Extensions, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4532C01-CAE1-4FEB-922A-3FFFB1361F31
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.xml

#nullable disable
namespace System.Net.Http
{
  /// <summary>
  /// Extension methods for <see cref="T:System.Net.Http.HttpClientHandler" /> which expose differences in platform specific capabilities.
  /// </summary>
  public static class HttpClientHandlerExtensions
  {
    private static bool? supportsAllowAutoRedirect;

    /// <summary>
    /// Gets a value that indicates if <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect">HttpClientHandler.AllowAutoRedirect</see> is supported by the handler.
    /// When this property is true and <see cref="P:System.Net.Http.HttpClientHandler.SupportsRedirectConfiguration">HttpClientHandler.SupportsRedirectConfiguration</see> is false, setting <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect">HttpClientHandler.AllowAutoRedirect</see> to true will cause the system default to be used for <see cref="P:System.Net.Http.HttpClientHandler.MaximumAutomaticRedirections">HttpClientHandler.MaximumAutomaticRedirections</see>.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Net.Http.HttpClientHandler" /> to check.</param>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> property; otherwise false.</returns>
    public static bool SupportsAllowAutoRedirect(this HttpClientHandler handler)
    {
      if (!HttpClientHandlerExtensions.supportsAllowAutoRedirect.HasValue)
      {
        bool flag;
        HttpClientHandlerExtensions.supportsAllowAutoRedirect = !(WebRequest.Create("http://www.contoso.com") is System.Net.HttpWebRequest instance) || !HttpWebRequestLightup.Current.TryGetAllowAutoRedirect(instance, out flag) || !HttpWebRequestLightup.Current.TrySetAllowAutoRedirect(instance, !flag) ? new bool?(false) : new bool?(true);
      }
      return HttpClientHandlerExtensions.supportsAllowAutoRedirect.Value;
    }

    /// <summary>
    /// Gets a value that indicates if <see cref="P:System.Net.Http.HttpClientHandler.PreAuthenticate" /> is supported by the handler.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Net.Http.HttpClientHandler" /> to check.</param>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.PreAuthenticate" /> property; otherwise false.</returns>
    public static bool SupportsPreAuthenticate(this HttpClientHandler handler) => false;

    /// <summary>
    /// Gets a value that indicates if <see cref="P:System.Net.Http.HttpClientHandler.ProtocolVersion" />, <see cref="P:System.Net.Http.HttpRequestMessage.ProtocolVersion">HttpRequestMessage.ProtocolVersion</see>, and <see cref="P:System.Net.Http.HttpResponseMessage.ProtocolVersion">HttpResponseMessage.ProtocolVersion</see> are supported by the handler.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Net.Http.HttpClientHandler" /> to check.</param>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.ProtocolVersion" />, <see cref="P:System.Net.Http.HttpRequestMessage.ProtocolVersion">HttpRequestMessage.ProtocolVersion</see>, and <see cref="P:System.Net.Http.HttpResponseMessage.ProtocolVersion">HttpResponseMessage.ProtocolVersion</see> properties; otherwise false.</returns>
    public static bool SupportsProtocolVersion(this HttpClientHandler handler) => false;

    /// <summary>
    /// Gets a value that indicates if <see cref="P:System.Net.Http.HttpRequestMessage.Headers">HttpRequestMessage.Headers</see> with <see cref="P:System.Net.Http.HttpRequestHeaders.TransferEncodingChunked" /> or <see cref="P:System.Net.Http.HttpRequestHeaders.TransferEncoding" /> header value of 'chunked' is supported by the handler.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Net.Http.HttpClientHandler" /> to check.</param>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports setting <see cref="P:System.Net.Http.HttpRequestMessage.Headers">HttpRequestMessage.Headers</see> with <see cref="P:System.Net.Http.HttpRequestHeaders.TransferEncodingChunked" /> or <see cref="P:System.Net.Http.HttpRequestHeaders.TransferEncoding" /> header value of 'chunked'; otherwise false.</returns>
    public static bool SupportsTransferEncodingChunked(this HttpClientHandler handler) => false;

    /// <summary>
    /// Gets a value that indicates if <see cref="P:System.Net.Http.HttpClientHandler.UseProxy" /> is supported by the handler.
    /// When this property is true and <see cref="P:System.Net.Http.HttpClientHandler.SupportsProxy">HttpClientHandler.SupportsProxy</see> is false, setting <see cref="P:System.Net.Http.HttpClientHandler.UseProxy">HttpClientHandler.UseProxy</see> to true will cause the system default to be used for <see cref="P:System.Net.Http.HttpClientHandler.Proxy">HttpClientHandler.Proxy</see>.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Net.Http.HttpClientHandler" /> to check.</param>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.UseProxy" /> property; otherwise false.</returns>
    public static bool SupportsUseProxy(this HttpClientHandler handler) => false;
  }
}
