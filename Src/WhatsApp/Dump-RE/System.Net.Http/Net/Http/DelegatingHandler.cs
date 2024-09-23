// Decompiled with JetBrains decompiler
// Type: System.Net.Http.DelegatingHandler
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.</summary>
  public abstract class DelegatingHandler : HttpMessageHandler
  {
    private HttpMessageHandler innerHandler;
    private volatile bool operationStarted;
    private volatile bool disposed;

    public HttpMessageHandler InnerHandler
    {
      get => this.innerHandler;
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.CheckDisposedOrStarted();
        if (Logging.On)
          Logging.Associate(Logging.Http, (object) this, (object) value);
        this.innerHandler = value;
      }
    }

    protected DelegatingHandler()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.DelegatingHandler" /> class with a specific inner handler.</summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    protected DelegatingHandler(HttpMessageHandler innerHandler)
    {
      this.InnerHandler = innerHandler;
    }

    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    protected internal override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request), SR.net_http_handler_norequest);
      this.SetOperationStarted();
      return this.innerHandler.SendAsync(request, cancellationToken);
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources. </param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.disposed = true;
        if (this.innerHandler != null)
          this.innerHandler.Dispose();
      }
      base.Dispose(disposing);
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private void CheckDisposedOrStarted()
    {
      this.CheckDisposed();
      if (this.operationStarted)
        throw new InvalidOperationException(SR.net_http_operation_started);
    }

    private void SetOperationStarted()
    {
      this.CheckDisposed();
      if (this.innerHandler == null)
        throw new InvalidOperationException(SR.net_http_handler_not_assigned);
      if (this.operationStarted)
        return;
      this.operationStarted = true;
    }
  }
}
