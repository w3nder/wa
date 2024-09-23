// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMessageInvoker
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  public class HttpMessageInvoker : IDisposable
  {
    private volatile bool disposed;
    private bool disposeHandler;
    private HttpMessageHandler handler;

    public HttpMessageInvoker(HttpMessageHandler handler)
      : this(handler, true)
    {
    }

    public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) handler);
      if (handler == null)
        throw new ArgumentNullException(nameof (handler));
      if (Logging.On)
        Logging.Associate(Logging.Http, (object) this, (object) handler);
      this.handler = handler;
      this.disposeHandler = disposeHandler;
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    public virtual Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      this.CheckDisposed();
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, nameof (SendAsync), (object) (Logging.GetObjectLogHash((object) request).ToString() + ": " + (object) request));
      Task<HttpResponseMessage> retObject = this.handler.SendAsync(request, cancellationToken);
      if (Logging.On)
        Logging.Exit(Logging.Http, (object) this, nameof (SendAsync), (object) retObject);
      return retObject;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (!this.disposeHandler)
        return;
      this.handler.Dispose();
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }
  }
}
