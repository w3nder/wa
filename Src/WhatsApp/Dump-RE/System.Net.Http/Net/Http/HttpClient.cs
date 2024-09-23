// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpClient
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI. </summary>
  public class HttpClient : HttpMessageInvoker
  {
    private const HttpCompletionOption defaultCompletionOption = HttpCompletionOption.ResponseContentRead;
    private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(100.0);
    private static readonly TimeSpan maxTimeout = TimeSpan.FromMilliseconds((double) int.MaxValue);
    private static readonly TimeSpan infiniteTimeout = TimeSpan.FromMilliseconds(-1.0);
    private volatile bool operationStarted;
    private volatile bool disposed;
    private CancellationTokenSource pendingRequestsCts;
    private HttpRequestHeaders defaultRequestHeaders;
    private Uri baseAddress;
    private TimeSpan timeout;
    private long maxResponseContentBufferSize;
    private TimerThread.Queue timerQueue;
    private static readonly TimerThread.Callback timeoutCallback = new TimerThread.Callback(HttpClient.TimeoutCallback);

    private TimerThread.Queue TimerQueue
    {
      get
      {
        if (this.timerQueue == null)
          this.timerQueue = TimerThread.GetOrCreateQueue((int) this.timeout.TotalMilliseconds);
        return this.timerQueue;
      }
    }

    private static void TimeoutCallback(TimerThread.Timer timer, int timeNoticed, object context)
    {
      try
      {
        ((CancellationTokenSource) context).Cancel();
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (AggregateException ex)
      {
        if (!Logging.On)
          return;
        Logging.Exception(Logging.Http, context, nameof (TimeoutCallback), (Exception) ex);
      }
    }

    /// <summary>Gets the headers which should be sent with each request.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpRequestHeaders" />.The headers which should be sent with each request.</returns>
    public HttpRequestHeaders DefaultRequestHeaders
    {
      get
      {
        if (this.defaultRequestHeaders == null)
          this.defaultRequestHeaders = new HttpRequestHeaders();
        return this.defaultRequestHeaders;
      }
    }

    /// <summary>Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</summary>
    /// <returns>Returns <see cref="T:System.Uri" />.The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</returns>
    public Uri BaseAddress
    {
      get => this.baseAddress;
      set
      {
        HttpClient.CheckBaseAddress(value, nameof (value));
        this.CheckDisposedOrStarted();
        if (Logging.On)
          Logging.PrintInfo(Logging.Http, (object) this, "BaseAddress: '" + (object) this.baseAddress + "'");
        this.baseAddress = value;
      }
    }

    /// <summary>Gets or sets the number of milliseconds to wait before the request times out.</summary>
    /// <returns>Returns <see cref="T:System.TimeSpan" />.The number of milliseconds to wait before the request times out.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The timeout specified is less than or equal to zero and is not <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance. </exception>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
    public TimeSpan Timeout
    {
      get => this.timeout;
      set
      {
        if (value != HttpClient.infiniteTimeout && (value <= TimeSpan.Zero || value > HttpClient.maxTimeout))
          throw new ArgumentOutOfRangeException(nameof (value));
        this.CheckDisposedOrStarted();
        this.timeout = value;
      }
    }

    /// <summary>Gets or sets the maximum number of bytes to buffer when reading the response content.</summary>
    /// <returns>Returns <see cref="T:System.Int32" />.The maximum number of bytes to buffer when reading the response content.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The size specified is less than or equal to zero.</exception>
    /// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance. </exception>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
    public long MaxResponseContentBufferSize
    {
      get => this.maxResponseContentBufferSize;
      set
      {
        if (value <= 0L)
          throw new ArgumentOutOfRangeException(nameof (value));
        if (value > (long) int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
        this.CheckDisposedOrStarted();
        this.maxResponseContentBufferSize = value;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class.</summary>
    public HttpClient()
      : this((HttpMessageHandler) new HttpClientHandler())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class with a specific handler.</summary>
    /// <param name="handler">The HTTP handler stack to use for sending requests. </param>
    public HttpClient(HttpMessageHandler handler)
      : this(handler, true)
    {
    }

    public HttpClient(HttpMessageHandler handler, bool disposeHandler)
      : base(handler, disposeHandler)
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) handler);
      this.timeout = HttpClient.defaultTimeout;
      this.maxResponseContentBufferSize = (long) int.MaxValue;
      this.pendingRequestsCts = new CancellationTokenSource();
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    public Task<string> GetStringAsync(string requestUri)
    {
      return this.GetStringAsync(this.CreateUri(requestUri));
    }

    public Task<string> GetStringAsync(Uri requestUri)
    {
      return this.GetContentAsync<string>(requestUri, HttpCompletionOption.ResponseContentRead, string.Empty, (Func<HttpContent, Task<string>>) (content => content.ReadAsStringAsync()));
    }

    public Task<byte[]> GetByteArrayAsync(string requestUri)
    {
      return this.GetByteArrayAsync(this.CreateUri(requestUri));
    }

    public Task<byte[]> GetByteArrayAsync(Uri requestUri)
    {
      return this.GetContentAsync<byte[]>(requestUri, HttpCompletionOption.ResponseContentRead, HttpUtilities.EmptyByteArray, (Func<HttpContent, Task<byte[]>>) (content => content.ReadAsByteArrayAsync()));
    }

    public Task<Stream> GetStreamAsync(string requestUri)
    {
      return this.GetStreamAsync(this.CreateUri(requestUri));
    }

    public Task<Stream> GetStreamAsync(Uri requestUri)
    {
      return this.GetContentAsync<Stream>(requestUri, HttpCompletionOption.ResponseHeadersRead, Stream.Null, (Func<HttpContent, Task<Stream>>) (content => content.ReadAsStreamAsync()));
    }

    private Task<T> GetContentAsync<T>(
      Uri requestUri,
      HttpCompletionOption completionOption,
      T defaultValue,
      Func<HttpContent, Task<T>> readAs)
    {
      TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
      this.GetAsync(requestUri, completionOption).ContinueWithStandard<HttpResponseMessage>((Action<Task<HttpResponseMessage>>) (requestTask =>
      {
        if (HttpClient.HandleRequestFaultsAndCancelation<T>(requestTask, tcs))
          return;
        HttpResponseMessage result = requestTask.Result;
        if (result.Content == null)
        {
          tcs.TrySetResult(defaultValue);
        }
        else
        {
          try
          {
            readAs(result.Content).ContinueWithStandard<T>((Action<Task<T>>) (contentTask =>
            {
              if (HttpUtilities.HandleFaultsAndCancelation<T>((Task) contentTask, tcs))
                return;
              tcs.TrySetResult(contentTask.Result);
            }));
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }));
      return tcs.Task;
    }

    /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> GetAsync(string requestUri)
    {
      return this.GetAsync(this.CreateUri(requestUri));
    }

    /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> GetAsync(Uri requestUri)
    {
      return this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
    }

    public Task<HttpResponseMessage> GetAsync(
      string requestUri,
      HttpCompletionOption completionOption)
    {
      return this.GetAsync(this.CreateUri(requestUri), completionOption);
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
    {
      return this.GetAsync(requestUri, completionOption, CancellationToken.None);
    }

    public Task<HttpResponseMessage> GetAsync(
      string requestUri,
      CancellationToken cancellationToken)
    {
      return this.GetAsync(this.CreateUri(requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
    {
      return this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(
      string requestUri,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken)
    {
      return this.GetAsync(this.CreateUri(requestUri), completionOption, cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(
      Uri requestUri,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
    }

    /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
      return this.PostAsync(this.CreateUri(requestUri), content);
    }

    /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
    {
      return this.PostAsync(requestUri, content, CancellationToken.None);
    }

    public Task<HttpResponseMessage> PostAsync(
      string requestUri,
      HttpContent content,
      CancellationToken cancellationToken)
    {
      return this.PostAsync(this.CreateUri(requestUri), content, cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsync(
      Uri requestUri,
      HttpContent content,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
      {
        Content = content
      }, cancellationToken);
    }

    /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
    {
      return this.PutAsync(this.CreateUri(requestUri), content);
    }

    /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
    {
      return this.PutAsync(requestUri, content, CancellationToken.None);
    }

    public Task<HttpResponseMessage> PutAsync(
      string requestUri,
      HttpContent content,
      CancellationToken cancellationToken)
    {
      return this.PutAsync(this.CreateUri(requestUri), content, cancellationToken);
    }

    public Task<HttpResponseMessage> PutAsync(
      Uri requestUri,
      HttpContent content,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
      {
        Content = content
      }, cancellationToken);
    }

    /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    public Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
      return this.DeleteAsync(this.CreateUri(requestUri));
    }

    /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
    {
      return this.DeleteAsync(requestUri, CancellationToken.None);
    }

    public Task<HttpResponseMessage> DeleteAsync(
      string requestUri,
      CancellationToken cancellationToken)
    {
      return this.DeleteAsync(this.CreateUri(requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> DeleteAsync(
      Uri requestUri,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
    }

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
      return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
    }

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }

    /// <summary>Send an HTTP request as an asynchronous operation. </summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <exception cref="T:System.InvalidOperationException">This operation will not block. The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      HttpCompletionOption completionOption)
    {
      return this.SendAsync(request, completionOption, CancellationToken.None);
    }

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
    public Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      this.CheckDisposed();
      HttpClient.CheckRequestMessage(request);
      this.SetOperationStarted();
      this.PrepareRequestMessage(request);
      CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.pendingRequestsCts.Token);
      TimerThread.Timer timeoutTimer = this.SetTimeout(linkedCts);
      TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
      try
      {
        base.SendAsync(request, linkedCts.Token).ContinueWithStandard<HttpResponseMessage>((Action<Task<HttpResponseMessage>>) (task =>
        {
          try
          {
            this.DisposeRequestContent(request);
            if (task.IsFaulted)
              this.SetTaskFaulted(request, linkedCts, tcs, ((Exception) task.Exception).GetBaseException(), timeoutTimer);
            else if (task.IsCanceled)
            {
              this.SetTaskCanceled(request, linkedCts, tcs, timeoutTimer);
            }
            else
            {
              HttpResponseMessage result = task.Result;
              if (result == null)
                this.SetTaskFaulted(request, linkedCts, tcs, (Exception) new InvalidOperationException(SR.net_http_handler_noresponse), timeoutTimer);
              else if (result.Content == null || completionOption == HttpCompletionOption.ResponseHeadersRead)
              {
                this.SetTaskCompleted(request, linkedCts, tcs, result, timeoutTimer);
              }
              else
              {
                Contract.Assert(completionOption == HttpCompletionOption.ResponseContentRead, "Unknown completion option.");
                if (request.Method == HttpMethod.Head)
                  this.SetTaskCompleted(request, linkedCts, tcs, result, timeoutTimer);
                else
                  this.StartContentBuffering(request, linkedCts, tcs, result, timeoutTimer);
              }
            }
          }
          catch (Exception ex)
          {
            if (Logging.On)
              Logging.Exception(Logging.Http, (object) this, nameof (SendAsync), ex);
            tcs.TrySetException(ex);
          }
        }));
      }
      catch
      {
        HttpClient.DisposeTimer(timeoutTimer);
        throw;
      }
      return tcs.Task;
    }

    /// <summary>Cancel all pending requests on this instance.</summary>
    public void CancelPendingRequests()
    {
      this.CheckDisposed();
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, nameof (CancelPendingRequests), (object) "");
      CancellationTokenSource cancellationTokenSource = Interlocked.Exchange<CancellationTokenSource>(ref this.pendingRequestsCts, new CancellationTokenSource());
      cancellationTokenSource.Cancel();
      cancellationTokenSource.Dispose();
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, nameof (CancelPendingRequests), (object) "");
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClient" /> and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.disposed = true;
        this.pendingRequestsCts.Cancel();
        this.pendingRequestsCts.Dispose();
      }
      base.Dispose(disposing);
    }

    private void DisposeRequestContent(HttpRequestMessage request)
    {
      Contract.Requires(request != null);
      request.Content?.Dispose();
    }

    private void StartContentBuffering(
      HttpRequestMessage request,
      CancellationTokenSource cancellationTokenSource,
      TaskCompletionSource<HttpResponseMessage> tcs,
      HttpResponseMessage response,
      TimerThread.Timer timeoutTimer)
    {
      response.Content.LoadIntoBufferAsync(this.maxResponseContentBufferSize).ContinueWithStandard((Action<Task>) (contentTask =>
      {
        try
        {
          bool cancellationRequested = cancellationTokenSource.Token.IsCancellationRequested;
          if (contentTask.IsFaulted)
          {
            response.Dispose();
            if (cancellationRequested && ((Exception) contentTask.Exception).GetBaseException() is HttpRequestException)
              this.SetTaskCanceled(request, cancellationTokenSource, tcs, timeoutTimer);
            else
              this.SetTaskFaulted(request, cancellationTokenSource, tcs, ((Exception) contentTask.Exception).GetBaseException(), timeoutTimer);
          }
          else if (contentTask.IsCanceled)
          {
            response.Dispose();
            this.SetTaskCanceled(request, cancellationTokenSource, tcs, timeoutTimer);
          }
          else
            this.SetTaskCompleted(request, cancellationTokenSource, tcs, response, timeoutTimer);
        }
        catch (Exception ex)
        {
          response.Dispose();
          tcs.TrySetException(ex);
          if (!Logging.On)
            return;
          Logging.Exception(Logging.Http, (object) this, "SendAsync", ex);
        }
      }));
    }

    private void SetOperationStarted()
    {
      if (this.operationStarted)
        return;
      this.operationStarted = true;
    }

    private void CheckDisposedOrStarted()
    {
      this.CheckDisposed();
      if (this.operationStarted)
        throw new InvalidOperationException(SR.net_http_operation_started);
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private static void CheckRequestMessage(HttpRequestMessage request)
    {
      if (!request.MarkAsSent())
        throw new InvalidOperationException(SR.net_http_client_request_already_sent);
    }

    private void PrepareRequestMessage(HttpRequestMessage request)
    {
      Uri uri = (Uri) null;
      if (request.RequestUri == (Uri) null && this.baseAddress == (Uri) null)
        throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
      if (request.RequestUri == (Uri) null)
        uri = this.baseAddress;
      else if (!request.RequestUri.IsAbsoluteUri)
      {
        if (this.baseAddress == (Uri) null)
          throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
        uri = UriHelper.CombineUri(this.baseAddress, request.RequestUri);
      }
      if (uri != (Uri) null)
        request.RequestUri = uri;
      if (this.defaultRequestHeaders == null)
        return;
      request.Headers.AddHeaders((HttpHeaders) this.defaultRequestHeaders);
    }

    private static void CheckBaseAddress(Uri baseAddress, string parameterName)
    {
      if (baseAddress == (Uri) null)
        return;
      if (!baseAddress.IsAbsoluteUri)
        throw new ArgumentException(SR.net_http_client_absolute_baseaddress_required, parameterName);
      if (!HttpUtilities.IsHttpUri(baseAddress))
        throw new ArgumentException(SR.net_http_client_http_baseaddress_required, parameterName);
    }

    private void SetTaskFaulted(
      HttpRequestMessage request,
      CancellationTokenSource cancellationTokenSource,
      TaskCompletionSource<HttpResponseMessage> tcs,
      Exception e,
      TimerThread.Timer timeoutTimer)
    {
      this.LogSendError(request, cancellationTokenSource, "SendAsync", e);
      tcs.TrySetException(e);
      HttpClient.DisposeCancellationTokenAndTimer(cancellationTokenSource, timeoutTimer);
    }

    private void SetTaskCanceled(
      HttpRequestMessage request,
      CancellationTokenSource cancellationTokenSource,
      TaskCompletionSource<HttpResponseMessage> tcs,
      TimerThread.Timer timeoutTimer)
    {
      this.LogSendError(request, cancellationTokenSource, "SendAsync", (Exception) null);
      tcs.TrySetCanceled();
      HttpClient.DisposeCancellationTokenAndTimer(cancellationTokenSource, timeoutTimer);
    }

    private void SetTaskCompleted(
      HttpRequestMessage request,
      CancellationTokenSource cancellationTokenSource,
      TaskCompletionSource<HttpResponseMessage> tcs,
      HttpResponseMessage response,
      TimerThread.Timer timeoutTimer)
    {
      if (Logging.On)
        Logging.PrintInfo(Logging.Http, (object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_client_send_completed, (object) Logging.GetObjectLogHash((object) request), (object) Logging.GetObjectLogHash((object) response), (object) response));
      tcs.TrySetResult(response);
      HttpClient.DisposeCancellationTokenAndTimer(cancellationTokenSource, timeoutTimer);
    }

    private static void DisposeCancellationTokenAndTimer(
      CancellationTokenSource cancellationTokenSource,
      TimerThread.Timer timeoutTimer)
    {
      try
      {
        cancellationTokenSource.Dispose();
      }
      catch (ObjectDisposedException ex)
      {
      }
      finally
      {
        HttpClient.DisposeTimer(timeoutTimer);
      }
    }

    private static void DisposeTimer(TimerThread.Timer timeoutTimer) => timeoutTimer?.Dispose();

    private TimerThread.Timer SetTimeout(CancellationTokenSource cancellationTokenSource)
    {
      Contract.Requires(cancellationTokenSource != null);
      TimerThread.Timer timer = (TimerThread.Timer) null;
      if (this.timeout != HttpClient.infiniteTimeout)
        timer = this.TimerQueue.CreateTimer(HttpClient.timeoutCallback, (object) cancellationTokenSource);
      return timer;
    }

    private void LogSendError(
      HttpRequestMessage request,
      CancellationTokenSource cancellationTokenSource,
      string method,
      Exception e)
    {
      Contract.Requires(request != null);
      if (cancellationTokenSource.IsCancellationRequested)
      {
        if (!Logging.On)
          return;
        Logging.PrintError(Logging.Http, (object) this, method, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_client_send_canceled, Logging.GetObjectLogHash((object) request)));
      }
      else
      {
        Contract.Assert(e != null);
        if (!Logging.On)
          return;
        Logging.PrintError(Logging.Http, (object) this, method, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_client_send_error, (object) Logging.GetObjectLogHash((object) request), (object) e));
      }
    }

    private Uri CreateUri(string uri)
    {
      return string.IsNullOrEmpty(uri) ? (Uri) null : new Uri(uri, UriKind.RelativeOrAbsolute);
    }

    private static bool HandleRequestFaultsAndCancelation<T>(
      Task<HttpResponseMessage> task,
      TaskCompletionSource<T> tcs)
    {
      if (HttpUtilities.HandleFaultsAndCancelation<T>((Task) task, tcs))
        return true;
      HttpResponseMessage result = task.Result;
      if (result.IsSuccessStatusCode)
        return false;
      if (result.Content != null)
        result.Content.Dispose();
      tcs.TrySetException((Exception) new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_message_not_success_statuscode, (object) (int) result.StatusCode, (object) result.ReasonPhrase)));
      return true;
    }
  }
}
