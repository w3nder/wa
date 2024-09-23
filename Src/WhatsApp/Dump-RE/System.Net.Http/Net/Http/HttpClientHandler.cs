// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpClientHandler
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base class for HTTP handler implementations.</summary>
  public class HttpClientHandler : HttpMessageHandler
  {
    private static readonly Action<object> onCancel = new Action<object>(HttpClientHandler.OnCancel);
    private readonly Action<object> startRequest;
    private readonly AsyncCallback getRequestStreamCallback;
    private readonly AsyncCallback getResponseCallback;
    private volatile bool operationStarted;
    private volatile bool disposed;
    private long maxRequestContentBufferSize;
    private CookieContainer cookieContainer;
    private bool useCookies;
    private DecompressionMethods automaticDecompression;
    private IWebProxy proxy;
    private bool useProxy;
    private bool preAuthenticate;
    private bool useDefaultCredentials;
    private ICredentials credentials;
    private bool allowAutoRedirect;
    private int maxAutomaticRedirections;
    private string connectionGroupName;
    private ClientCertificateOption clientCertOptions;
    private Uri lastUsedRequestUri;

    /// <summary>Gets a value that indicates whether the handler supports automatic response content decompression.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports automatic response content decompression; otherwise false. The default value is true.</returns>
    public virtual bool SupportsAutomaticDecompression
    {
      get => Capabilities.SupportsAutomaticDecompression;
    }

    /// <summary>Gets a value that indicates whether the handler supports proxy settings.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports proxy settings; otherwise false. The default value is true.</returns>
    public virtual bool SupportsProxy => Capabilities.SupportsProxy;

    /// <summary>Gets a value that indicates whether the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties; otherwise false. The default value is true.</returns>
    public virtual bool SupportsRedirectConfiguration => Capabilities.SupportsRedirectConfiguration;

    /// <summary>Gets or sets a value that indicates whether the handler uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests; otherwise false. The default value is true.</returns>
    public bool UseCookies
    {
      get => this.useCookies;
      set
      {
        this.CheckDisposedOrStarted();
        this.useCookies = value;
      }
    }

    /// <summary>Gets or sets the cookie container used to store server cookies by the handler.</summary>
    /// <returns>Returns <see cref="T:System.Net.CookieContainer" />.The cookie container used to store server cookies by the handler.</returns>
    public CookieContainer CookieContainer
    {
      get => this.cookieContainer;
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        if (!this.UseCookies)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_invalid_enable_first, (object) "UseCookies", (object) "true"));
        this.CheckDisposedOrStarted();
        this.cookieContainer = value;
      }
    }

    public ClientCertificateOption ClientCertificateOptions
    {
      get => this.clientCertOptions;
      set
      {
        if (value != ClientCertificateOption.Manual && value != ClientCertificateOption.Automatic)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.CheckDisposedOrStarted();
        this.clientCertOptions = value;
      }
    }

    /// <summary>Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.</summary>
    /// <returns>Returns <see cref="T:System.Net.DecompressionMethods" />.The automatic decompression method used by the handler. The default value is <see cref="F:System.Net.DecompressionMethods.None" />.</returns>
    public DecompressionMethods AutomaticDecompression
    {
      get => this.automaticDecompression;
      set
      {
        this.CheckDisposedOrStarted();
        this.automaticDecompression = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the handler uses a proxy for requests. </summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the handler should use a proxy for requests; otherwise false. The default value is true.</returns>
    public bool UseProxy
    {
      get => this.useProxy;
      set
      {
        this.CheckDisposedOrStarted();
        this.useProxy = value;
      }
    }

    /// <summary>Gets or sets proxy information used by the handler.</summary>
    /// <returns>Returns <see cref="T:System.Net.IWebProxy" />.The proxy information used by the handler. The default value is null.</returns>
    public IWebProxy Proxy
    {
      get => this.proxy;
      [SecuritySafeCritical] set
      {
        if (!this.UseProxy && value != null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_invalid_enable_first, (object) "UseProxy", (object) "true"));
        this.CheckDisposedOrStarted();
        ExceptionHelper.WebPermissionUnrestricted.Demand();
        this.proxy = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the handler sends an Authorization header with the request.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true for the handler to send an HTTP Authorization header with requests after authentication has taken place; otherwise, false. The default is false.</returns>
    public bool PreAuthenticate
    {
      get => this.preAuthenticate;
      set
      {
        this.CheckDisposedOrStarted();
        this.preAuthenticate = value;
      }
    }

    /// <summary>Gets or sets a value that controls whether default credentials are sent with requests by the handler.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the default credentials are used; otherwise false. The default value is false.</returns>
    public bool UseDefaultCredentials
    {
      get => this.useDefaultCredentials;
      set
      {
        this.CheckDisposedOrStarted();
        this.useDefaultCredentials = value;
      }
    }

    /// <summary>Gets or sets authentication information used by this handler.</summary>
    /// <returns>Returns <see cref="T:System.Net.ICredentials" />.The authentication credentials associated with the handler. The default is null.</returns>
    public ICredentials Credentials
    {
      get => this.credentials;
      set
      {
        this.CheckDisposedOrStarted();
        this.credentials = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the handler should follow redirection responses.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler should follow redirection responses; otherwise false. The default value is true.</returns>
    public bool AllowAutoRedirect
    {
      get => this.allowAutoRedirect;
      set
      {
        this.CheckDisposedOrStarted();
        this.allowAutoRedirect = value;
      }
    }

    /// <summary>Gets or sets the maximum number of redirects that the handler follows.</summary>
    /// <returns>Returns <see cref="T:System.Int32" />.The maximum number of redirection responses that the handler follows. The default value is 50.</returns>
    public int MaxAutomaticRedirections
    {
      get => this.maxAutomaticRedirections;
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.CheckDisposedOrStarted();
        this.maxAutomaticRedirections = value;
      }
    }

    /// <summary>Gets or sets the maximum request content buffer size used by the handler.</summary>
    /// <returns>Returns <see cref="T:System.Int32" />.The maximum request content buffer size in bytes. The default value is 65,536 bytes.</returns>
    public long MaxRequestContentBufferSize
    {
      get => this.maxRequestContentBufferSize;
      set
      {
        if (value < 0L)
          throw new ArgumentOutOfRangeException(nameof (value));
        if (value > (long) int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
        this.CheckDisposedOrStarted();
        this.maxRequestContentBufferSize = value;
      }
    }

    /// <summary>Creates an instance of a <see cref="T:System.Net.Http.HttpClientHandler" /> class.</summary>
    public HttpClientHandler()
    {
      this.startRequest = new Action<object>(this.StartRequest);
      this.getRequestStreamCallback = new AsyncCallback(this.GetRequestStreamCallback);
      this.getResponseCallback = new AsyncCallback(this.GetResponseCallback);
      this.connectionGroupName = RuntimeHelpers.GetHashCode((object) this).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      this.allowAutoRedirect = true;
      this.maxRequestContentBufferSize = (long) int.MaxValue;
      this.automaticDecompression = DecompressionMethods.None;
      this.cookieContainer = new CookieContainer();
      this.credentials = (ICredentials) null;
      this.maxAutomaticRedirections = 5;
      this.preAuthenticate = false;
      this.proxy = (IWebProxy) null;
      this.useProxy = true;
      this.useCookies = true;
      this.useDefaultCredentials = false;
      this.clientCertOptions = ClientCertificateOption.Manual;
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClientHandler" /> and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
        this.disposed = true;
      base.Dispose(disposing);
    }

    private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
    {
      HttpWebRequest prepareWebRequest = new HttpWebRequest(request.RequestUri);
      if (Logging.On)
        Logging.Associate(Logging.Http, (object) request, (object) prepareWebRequest);
      prepareWebRequest.Method = request.Method.Method;
      prepareWebRequest.ProtocolVersion = request.Version;
      this.SetDefaultOptions(prepareWebRequest);
      HttpClientHandler.SetConnectionOptions(prepareWebRequest, request);
      this.SetServicePointOptions(prepareWebRequest, request);
      HttpClientHandler.SetRequestHeaders(prepareWebRequest, request);
      HttpClientHandler.SetContentHeaders(prepareWebRequest, request);
      this.InitializeWebRequest(request, prepareWebRequest);
      return prepareWebRequest;
    }

    internal virtual void InitializeWebRequest(
      HttpRequestMessage request,
      HttpWebRequest webRequest)
    {
    }

    private void SetDefaultOptions(HttpWebRequest webRequest)
    {
      webRequest.Timeout = -1;
      webRequest.AllowAutoRedirect = this.allowAutoRedirect;
      webRequest.AutomaticDecompression = this.automaticDecompression;
      webRequest.PreAuthenticate = this.preAuthenticate;
      if (this.useDefaultCredentials)
        webRequest.UseDefaultCredentials = true;
      else
        webRequest.Credentials = this.credentials;
      if (this.allowAutoRedirect)
        webRequest.MaximumAutomaticRedirections = this.maxAutomaticRedirections;
      if (this.useProxy)
      {
        if (this.proxy != null)
          webRequest.Proxy = this.proxy;
      }
      else
        webRequest.Proxy = (IWebProxy) null;
      if (!this.useCookies)
        return;
      webRequest.CookieContainer = this.cookieContainer;
    }

    private static void SetConnectionOptions(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      if (request.Version <= HttpVersion.Version10)
      {
        bool flag = false;
        foreach (string strA in request.Headers.Connection)
        {
          if (string.Compare(strA, "Keep-Alive", StringComparison.OrdinalIgnoreCase) == 0)
          {
            flag = true;
            break;
          }
        }
        webRequest.KeepAlive = flag;
      }
      else
      {
        bool? connectionClose = request.Headers.ConnectionClose;
        if ((!connectionClose.GetValueOrDefault() ? 0 : (connectionClose.HasValue ? 1 : 0)) == 0)
          return;
        webRequest.KeepAlive = false;
      }
    }

    private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      bool? expectContinue = request.Headers.ExpectContinue;
      if (!expectContinue.HasValue)
        return;
      webRequest.ServicePoint.Expect100Continue = expectContinue.Value;
    }

    private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      WebHeaderCollection headers1 = webRequest.Headers;
      HttpRequestHeaders headers2 = request.Headers;
      bool flag1 = headers2.Contains("Host");
      bool flag2 = headers2.Contains("Expect");
      bool flag3 = headers2.Contains("Transfer-Encoding");
      bool flag4 = headers2.Contains("Connection");
      bool flag5 = headers2.Contains("Accept");
      bool flag6 = headers2.Contains("Range");
      bool flag7 = headers2.Contains("Referer");
      bool flag8 = headers2.Contains("User-Agent");
      bool flag9 = headers2.Contains("Date");
      bool flag10 = headers2.Contains("If-Modified-Since");
      if (flag9)
      {
        DateTimeOffset? date = headers2.Date;
        if (date.HasValue)
          webRequest.Date = date.Value.UtcDateTime;
      }
      if (flag10)
      {
        DateTimeOffset? ifModifiedSince = headers2.IfModifiedSince;
        if (ifModifiedSince.HasValue)
          webRequest.IfModifiedSince = ifModifiedSince.Value.UtcDateTime;
      }
      if (flag6)
      {
        RangeHeaderValue range1 = headers2.Range;
        if (range1 != null)
        {
          foreach (RangeItemHeaderValue range2 in (IEnumerable<RangeItemHeaderValue>) range1.Ranges)
          {
            if (!range2.To.HasValue || !range2.From.HasValue)
            {
              HttpWebRequest httpWebRequest = webRequest;
              long? to = range2.To;
              long? nullable = to.HasValue ? new long?(-to.GetValueOrDefault()) : new long?();
              long range3 = (nullable.HasValue ? new long?(nullable.GetValueOrDefault()) : range2.From).Value;
              httpWebRequest.AddRange(range3);
            }
            else
              webRequest.AddRange(range2.From.Value, range2.To.Value);
          }
        }
      }
      if (flag7)
      {
        Uri referrer = headers2.Referrer;
        if (referrer != (Uri) null)
          webRequest.Referer = referrer.OriginalString;
      }
      if (flag5 && headers2.Accept.Count > 0)
        webRequest.Accept = headers2.Accept.ToString();
      if (flag8 && headers2.UserAgent.Count > 0)
        webRequest.UserAgent = headers2.UserAgent.ToString();
      if (flag1)
      {
        string host = headers2.Host;
        if (host != null)
          webRequest.Host = host;
      }
      if (flag2)
      {
        string stringWithoutSpecial = headers2.Expect.GetHeaderStringWithoutSpecial();
        if (!string.IsNullOrEmpty(stringWithoutSpecial) || !headers2.Expect.IsSpecialValueSet)
          webRequest.Expect = stringWithoutSpecial;
      }
      if (flag3)
      {
        string stringWithoutSpecial = headers2.TransferEncoding.GetHeaderStringWithoutSpecial();
        if (!string.IsNullOrEmpty(stringWithoutSpecial) || !headers2.TransferEncoding.IsSpecialValueSet)
        {
          webRequest.SendChunked = true;
          webRequest.TransferEncoding = stringWithoutSpecial;
          webRequest.SendChunked = false;
        }
      }
      if (flag4)
      {
        string str = string.Join(", ", headers2.Connection.Where<string>((Func<string, bool>) (item => string.Compare(item, "Keep-Alive", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item, "close", StringComparison.OrdinalIgnoreCase) != 0)));
        if (!string.IsNullOrEmpty(str) || !headers2.Connection.IsSpecialValueSet)
          webRequest.Connection = str;
      }
      foreach (KeyValuePair<string, string> headerString in request.Headers.GetHeaderStrings())
      {
        string key = headerString.Key;
        if ((!flag1 || !HttpClientHandler.AreEqual("Host", key)) && (!flag2 || !HttpClientHandler.AreEqual("Expect", key)) && (!flag3 || !HttpClientHandler.AreEqual("Transfer-Encoding", key)) && (!flag5 || !HttpClientHandler.AreEqual("Accept", key)) && (!flag6 || !HttpClientHandler.AreEqual("Range", key)) && (!flag7 || !HttpClientHandler.AreEqual("Referer", key)) && (!flag8 || !HttpClientHandler.AreEqual("User-Agent", key)) && (!flag9 || !HttpClientHandler.AreEqual("Date", key)) && (!flag10 || !HttpClientHandler.AreEqual("If-Modified-Since", key)) && (!flag4 || !HttpClientHandler.AreEqual("Connection", key)))
          headers1.Add(headerString.Key, headerString.Value);
      }
    }

    private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      if (request.Content == null)
        return;
      if (request.Content.Headers.Contains("Content-Length"))
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
        {
          if (string.Compare("Content-Length", header.Key, StringComparison.OrdinalIgnoreCase) != 0)
            HttpClientHandler.SetContentHeader(webRequest, header);
        }
      }
      else
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
          HttpClientHandler.SetContentHeader(webRequest, header);
      }
    }

    private static void SetContentHeader(
      HttpWebRequest webRequest,
      KeyValuePair<string, IEnumerable<string>> header)
    {
      if (string.Compare("Content-Type", header.Key, StringComparison.OrdinalIgnoreCase) == 0)
        webRequest.ContentType = string.Join(", ", header.Value);
      else
        webRequest.Headers.Add(header.Key, string.Join(", ", header.Value));
    }

    /// <summary>Creates an instance of  <see cref="T:System.Net.Http.HttpResponseMessage" /> based on the information provided in the <see cref="T:System.Net.Http.HttpRequestMessage" /> as an operation that will not block.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    protected internal override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request), SR.net_http_handler_norequest);
      this.CheckDisposed();
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, nameof (SendAsync), (object) request);
      this.SetOperationStarted();
      TaskCompletionSource<HttpResponseMessage> completionSource = new TaskCompletionSource<HttpResponseMessage>();
      HttpClientHandler.RequestState state = new HttpClientHandler.RequestState();
      state.tcs = completionSource;
      state.cancellationToken = cancellationToken;
      state.requestMessage = request;
      this.lastUsedRequestUri = request.RequestUri;
      try
      {
        HttpWebRequest prepareWebRequest = this.CreateAndPrepareWebRequest(request);
        state.webRequest = prepareWebRequest;
        cancellationToken.Register(HttpClientHandler.onCancel, (object) prepareWebRequest);
        if (ExecutionContext.IsFlowSuppressed())
        {
          IWebProxy webProxy = (IWebProxy) null;
          if (this.useProxy)
            webProxy = this.proxy ?? HttpWebRequest.DefaultWebProxy;
          if (this.UseDefaultCredentials || this.Credentials != null || webProxy != null && webProxy.Credentials != null)
            this.SafeCaptureIdenity(state);
        }
        Task.Factory.StartNew(this.startRequest, (object) state);
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(state, ex);
      }
      if (Logging.On)
        Logging.Exit(Logging.Http, (object) this, nameof (SendAsync), (object) completionSource.Task);
      return completionSource.Task;
    }

    private void StartRequest(object obj)
    {
      HttpClientHandler.RequestState state = obj as HttpClientHandler.RequestState;
      Contract.Assert(state != null);
      try
      {
        if (state.requestMessage.Content != null)
        {
          this.PrepareAndStartContentUpload(state);
        }
        else
        {
          state.webRequest.ContentLength = 0L;
          this.StartGettingResponse(state);
        }
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(state, ex);
      }
    }

    private void PrepareAndStartContentUpload(HttpClientHandler.RequestState state)
    {
      HttpContent requestContent = state.requestMessage.Content;
      Contract.Assert(requestContent != null);
      try
      {
        bool? transferEncodingChunked = state.requestMessage.Headers.TransferEncodingChunked;
        if ((!transferEncodingChunked.GetValueOrDefault() ? 0 : (transferEncodingChunked.HasValue ? 1 : 0)) != 0)
        {
          state.webRequest.SendChunked = true;
          this.StartGettingRequestStream(state);
        }
        else
        {
          long? contentLength = requestContent.Headers.ContentLength;
          if (contentLength.HasValue)
          {
            state.webRequest.ContentLength = contentLength.Value;
            this.StartGettingRequestStream(state);
          }
          else
          {
            if (this.maxRequestContentBufferSize == 0L)
              throw new HttpRequestException(SR.net_http_handler_nocontentlength);
            requestContent.LoadIntoBufferAsync(this.maxRequestContentBufferSize).ContinueWithStandard((Action<Task>) (task =>
            {
              try
              {
                if (task.IsFaulted)
                {
                  this.HandleAsyncException(state, ((Exception) task.Exception).GetBaseException());
                }
                else
                {
                  contentLength = requestContent.Headers.ContentLength;
                  state.webRequest.ContentLength = contentLength.Value;
                  this.StartGettingRequestStream(state);
                }
              }
              catch (Exception ex)
              {
                this.HandleAsyncException(state, ex);
              }
            }));
          }
        }
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(state, ex);
      }
    }

    private void StartGettingRequestStream(HttpClientHandler.RequestState state)
    {
      if (state.identity != null)
      {
        using (state.identity.Impersonate())
          state.webRequest.BeginGetRequestStream(this.getRequestStreamCallback, (object) state);
      }
      else
        state.webRequest.BeginGetRequestStream(this.getRequestStreamCallback, (object) state);
    }

    private void GetRequestStreamCallback(IAsyncResult ar)
    {
      HttpClientHandler.RequestState state = ar.AsyncState as HttpClientHandler.RequestState;
      Contract.Assert(state != null);
      try
      {
        TransportContext context = (TransportContext) null;
        Stream requestStream = state.webRequest.EndGetRequestStream(ar, out context);
        state.requestStream = requestStream;
        state.requestMessage.Content.CopyToAsync(requestStream, context).ContinueWithStandard((Action<Task>) (task =>
        {
          try
          {
            if (task.IsFaulted)
              this.HandleAsyncException(state, ((Exception) task.Exception).GetBaseException());
            else if (task.IsCanceled)
            {
              state.tcs.TrySetCanceled();
            }
            else
            {
              state.requestStream.Close();
              this.StartGettingResponse(state);
            }
          }
          catch (Exception ex)
          {
            this.HandleAsyncException(state, ex);
          }
        }));
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(state, ex);
      }
    }

    private void StartGettingResponse(HttpClientHandler.RequestState state)
    {
      if (state.identity != null)
      {
        using (state.identity.Impersonate())
          state.webRequest.BeginGetResponse(this.getResponseCallback, (object) state);
      }
      else
        state.webRequest.BeginGetResponse(this.getResponseCallback, (object) state);
    }

    private void GetResponseCallback(IAsyncResult ar)
    {
      HttpClientHandler.RequestState asyncState = ar.AsyncState as HttpClientHandler.RequestState;
      Contract.Assert(asyncState != null);
      try
      {
        HttpWebResponse response = asyncState.webRequest.EndGetResponse(ar) as HttpWebResponse;
        asyncState.tcs.TrySetResult(this.CreateResponseMessage(response, asyncState.requestMessage));
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(asyncState, ex);
      }
    }

    private bool TryGetExceptionResponse(
      WebException webException,
      HttpRequestMessage requestMessage,
      out HttpResponseMessage httpResponseMessage)
    {
      if (webException != null && webException.Response != null)
      {
        HttpWebResponse decorator = HttpWebResponse.CreateDecorator(webException.Response as System.Net.HttpWebResponse, this.automaticDecompression);
        if (decorator != null)
        {
          httpResponseMessage = this.CreateResponseMessage(decorator, requestMessage);
          return true;
        }
      }
      httpResponseMessage = (HttpResponseMessage) null;
      return false;
    }

    private HttpResponseMessage CreateResponseMessage(
      HttpWebResponse webResponse,
      HttpRequestMessage request)
    {
      HttpResponseMessage responseMessage = new HttpResponseMessage(webResponse.StatusCode);
      responseMessage.ReasonPhrase = webResponse.StatusDescription;
      responseMessage.Version = webResponse.ProtocolVersion;
      responseMessage.RequestMessage = request;
      responseMessage.Content = (HttpContent) new StreamContent((Stream) new HttpClientHandler.WebExceptionWrapperStream(webResponse.GetResponseStream()));
      request.RequestUri = webResponse.ResponseUri;
      WebHeaderCollection headers1 = webResponse.Headers;
      HttpContentHeaders headers2 = responseMessage.Content.Headers;
      HttpResponseHeaders headers3 = responseMessage.Headers;
      if (webResponse.ContentLength >= 0L)
        headers2.ContentLength = new long?(webResponse.ContentLength);
      for (int index = 0; index < headers1.Count; ++index)
      {
        string key = headers1.GetKey(index);
        if (string.Compare(key, "Content-Length", StringComparison.OrdinalIgnoreCase) != 0)
        {
          string[] values = headers1.GetValues(index);
          if (!headers3.TryAddWithoutValidation(key, (IEnumerable<string>) values))
            Contract.Assert(headers2.TryAddWithoutValidation(key, (IEnumerable<string>) values), "Invalid header name.");
        }
      }
      return responseMessage;
    }

    private void HandleAsyncException(HttpClientHandler.RequestState state, Exception e)
    {
      if (Logging.On)
        Logging.Exception(Logging.Http, (object) this, "SendAsync", e);
      HttpResponseMessage httpResponseMessage;
      if (this.TryGetExceptionResponse(e as WebException, state.requestMessage, out httpResponseMessage))
        state.tcs.TrySetResult(httpResponseMessage);
      else if (state.cancellationToken.IsCancellationRequested)
      {
        state.tcs.TrySetCanceled();
      }
      else
      {
        switch (e)
        {
          case WebException _:
          case IOException _:
            state.tcs.TrySetException((Exception) new HttpRequestException(SR.net_http_client_execution_error, e));
            break;
          default:
            state.tcs.TrySetException(e);
            break;
        }
      }
    }

    private static void OnCancel(object state)
    {
      HttpWebRequest httpWebRequest = state as HttpWebRequest;
      Contract.Assert(httpWebRequest != null);
      httpWebRequest.Abort();
    }

    private void SetOperationStarted()
    {
      if (this.operationStarted)
        return;
      this.operationStarted = true;
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    internal void CheckDisposedOrStarted()
    {
      this.CheckDisposed();
      if (this.operationStarted)
        throw new InvalidOperationException(SR.net_http_operation_started);
    }

    private static bool AreEqual(string x, string y)
    {
      return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
    }

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
    private void SafeCaptureIdenity(HttpClientHandler.RequestState state)
    {
      state.identity = WindowsIdentity.GetCurrent();
    }

    private class RequestState
    {
      internal HttpWebRequest webRequest;
      internal TaskCompletionSource<HttpResponseMessage> tcs;
      internal CancellationToken cancellationToken;
      internal HttpRequestMessage requestMessage;
      internal Stream requestStream;
      internal WindowsIdentity identity;
    }

    private class WebExceptionWrapperStream : DelegatingStream
    {
      internal WebExceptionWrapperStream(Stream innerStream)
        : base(innerStream)
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        try
        {
          return base.Read(buffer, offset, count);
        }
        catch (WebException ex)
        {
          throw new IOException(SR.net_http_read_error, (Exception) ex);
        }
      }

      public override IAsyncResult BeginRead(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        try
        {
          return base.BeginRead(buffer, offset, count, callback, state);
        }
        catch (WebException ex)
        {
          throw new IOException(SR.net_http_read_error, (Exception) ex);
        }
      }

      public override int EndRead(IAsyncResult asyncResult)
      {
        try
        {
          return base.EndRead(asyncResult);
        }
        catch (WebException ex)
        {
          throw new IOException(SR.net_http_read_error, (Exception) ex);
        }
      }

      public override int ReadByte()
      {
        try
        {
          return base.ReadByte();
        }
        catch (WebException ex)
        {
          throw new IOException(SR.net_http_read_error, (Exception) ex);
        }
      }
    }
  }
}
