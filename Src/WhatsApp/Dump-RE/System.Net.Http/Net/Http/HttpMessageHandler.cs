// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMessageHandler
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base type for HTTP message handlers.</summary>
  public abstract class HttpMessageHandler : IDisposable
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpMessageHandler" /> class.</summary>
    protected HttpMessageHandler()
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) null);
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    protected internal abstract Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken);

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpMessageHandler" /> and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
