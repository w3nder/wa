// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRequestMessage
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Represents a HTTP request message.</summary>
  public class HttpRequestMessage : IDisposable
  {
    private const int messageAlreadySent = 1;
    private const int messageNotYetSent = 0;
    private int sendStatus;
    private HttpMethod method;
    private Uri requestUri;
    private HttpRequestHeaders headers;
    private Version version;
    private HttpContent content;
    private bool disposed;
    private IDictionary<string, object> properties;

    /// <summary>Gets or sets the HTTP message version.</summary>
    /// <returns>Returns <see cref="T:System.Version" />.The HTTP message version. The default is 1.1.</returns>
    public Version Version
    {
      get => this.version;
      set
      {
        if (value == (Version) null)
          throw new ArgumentNullException(nameof (value));
        this.CheckDisposed();
        this.version = value;
      }
    }

    /// <summary>Gets or sets the contents of the HTTP message. </summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpContent" />.The content of a message</returns>
    public HttpContent Content
    {
      get => this.content;
      set
      {
        this.CheckDisposed();
        if (Logging.On)
        {
          if (value == null)
            Logging.PrintInfo(Logging.Http, (object) this, SR.net_http_log_content_null);
          else
            Logging.Associate(Logging.Http, (object) this, (object) value);
        }
        this.content = value;
      }
    }

    /// <summary>Gets or sets the HTTP method used by the HTTP request message.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.The HTTP method used by the request message. The default is the GET method.</returns>
    public HttpMethod Method
    {
      get => this.method;
      set
      {
        if (value == (HttpMethod) null)
          throw new ArgumentNullException(nameof (value));
        this.CheckDisposed();
        this.method = value;
      }
    }

    /// <summary>Gets or sets the <see cref="T:System.Uri" /> used for the HTTP request.</summary>
    /// <returns>Returns <see cref="T:System.Uri" />.The <see cref="T:System.Uri" /> used for the HTTP request.</returns>
    public Uri RequestUri
    {
      get => this.requestUri;
      set
      {
        if (value != (Uri) null && value.IsAbsoluteUri && !HttpUtilities.IsHttpUri(value))
          throw new ArgumentException(SR.net_http_client_http_baseaddress_required, nameof (value));
        this.CheckDisposed();
        this.requestUri = value;
      }
    }

    /// <summary>Gets the collection of HTTP request headers.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpRequestHeaders" />.The collection of HTTP request headers.</returns>
    public HttpRequestHeaders Headers
    {
      get
      {
        if (this.headers == null)
          this.headers = new HttpRequestHeaders();
        return this.headers;
      }
    }

    /// <summary>Gets a set of properties for the HTTP request.</summary>
    /// <returns>Returns <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    public IDictionary<string, object> Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = (IDictionary<string, object>) new Dictionary<string, object>();
        return this.properties;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class.</summary>
    public HttpRequestMessage()
      : this(HttpMethod.Get, (Uri) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The <see cref="T:System.Uri" /> to request.</param>
    public HttpRequestMessage(HttpMethod method, Uri requestUri)
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) ("Method: " + (object) method + ", Uri: '" + (object) requestUri + "'"));
      this.InitializeValues(method, requestUri);
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">A string that represents the request  <see cref="T:System.Uri" />.</param>
    public HttpRequestMessage(HttpMethod method, string requestUri)
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) ("Method: " + (object) method + ", Uri: '" + requestUri + "'"));
      if (string.IsNullOrEmpty(requestUri))
        this.InitializeValues(method, (Uri) null);
      else
        this.InitializeValues(method, new Uri(requestUri, UriKind.RelativeOrAbsolute));
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>Returns <see cref="T:System.String" />.A string representation of the current object.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Method: ");
      stringBuilder.Append((object) this.method);
      stringBuilder.Append(", RequestUri: '");
      stringBuilder.Append(this.requestUri == (Uri) null ? "<null>" : this.requestUri.ToString());
      stringBuilder.Append("', Version: ");
      stringBuilder.Append((object) this.version);
      stringBuilder.Append(", Content: ");
      stringBuilder.Append(this.content == null ? "<null>" : this.content.GetType().FullName);
      stringBuilder.Append(", Headers:\r\n");
      stringBuilder.Append(HeaderUtilities.DumpHeaders((HttpHeaders) this.headers, this.content == null ? (HttpHeaders) null : (HttpHeaders) this.content.Headers));
      return stringBuilder.ToString();
    }

    private void InitializeValues(HttpMethod method, Uri requestUri)
    {
      if (method == (HttpMethod) null)
        throw new ArgumentNullException(nameof (method));
      if (requestUri != (Uri) null && requestUri.IsAbsoluteUri && !HttpUtilities.IsHttpUri(requestUri))
        throw new ArgumentException(SR.net_http_client_http_baseaddress_required, nameof (requestUri));
      this.method = method;
      this.requestUri = requestUri;
      this.version = HttpUtilities.DefaultVersion;
    }

    internal bool MarkAsSent() => Interlocked.Exchange(ref this.sendStatus, 1) == 0;

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" /> and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (this.content == null)
        return;
      this.content.Dispose();
    }

    /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" />.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }
  }
}
