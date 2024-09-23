// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpResponseMessage
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Represents a HTTP response message.</summary>
  public class HttpResponseMessage : IDisposable
  {
    private const HttpStatusCode defaultStatusCode = HttpStatusCode.OK;
    private HttpStatusCode statusCode;
    private HttpResponseHeaders headers;
    private string reasonPhrase;
    private HttpRequestMessage requestMessage;
    private Version version;
    private HttpContent content;
    private bool disposed;

    /// <summary>Gets or sets the HTTP message version. </summary>
    /// <returns>Returns <see cref="T:System.Version" />.The HTTP message version. The default is 1.1. </returns>
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

    /// <summary>Gets or sets the content of a HTTP response message. </summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpContent" />.The content of the HTTP response message.</returns>
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

    /// <summary>Gets or sets the status code of the HTTP response.</summary>
    /// <returns>Returns <see cref="T:System.Net.HttpStatusCode" />.The status code of the HTTP response.</returns>
    public HttpStatusCode StatusCode
    {
      get => this.statusCode;
      set
      {
        if (value < (HttpStatusCode) 0 || value > (HttpStatusCode) 999)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.CheckDisposed();
        this.statusCode = value;
      }
    }

    /// <summary>Gets or sets the reason phrase which typically is sent by servers together with the status code. </summary>
    /// <returns>Returns <see cref="T:System.String" />.The reason phrase sent by the server.</returns>
    public string ReasonPhrase
    {
      get
      {
        return this.reasonPhrase != null ? this.reasonPhrase : HttpStatusDescription.Get(this.StatusCode);
      }
      set
      {
        if (value != null && this.ContainsNewLineCharacter(value))
          throw new FormatException(SR.net_http_reasonphrase_format_error);
        this.CheckDisposed();
        this.reasonPhrase = value;
      }
    }

    /// <summary>Gets the collection of HTTP response headers. </summary>
    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpResponseHeaders" />.The collection of HTTP response headers.</returns>
    public HttpResponseHeaders Headers
    {
      get
      {
        if (this.headers == null)
          this.headers = new HttpResponseHeaders();
        return this.headers;
      }
    }

    /// <summary>Gets or sets the request message which led to this response message.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpRequestMessage" />.The request message which led to this response message.</returns>
    public HttpRequestMessage RequestMessage
    {
      get => this.requestMessage;
      set
      {
        this.CheckDisposed();
        if (Logging.On && value != null)
          Logging.Associate(Logging.Http, (object) this, (object) value);
        this.requestMessage = value;
      }
    }

    /// <summary>Gets a value that indicates if the HTTP response was successful.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.A value that indicates if the HTTP response was successful. true if <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" /> was in the range 200-299; otherwise false.</returns>
    public bool IsSuccessStatusCode
    {
      get => this.statusCode >= HttpStatusCode.OK && this.statusCode <= (HttpStatusCode) 299;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class.</summary>
    public HttpResponseMessage()
      : this(HttpStatusCode.OK)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class with a specific <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" />.</summary>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    public HttpResponseMessage(HttpStatusCode statusCode)
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) ("StatusCode: " + (object) (int) statusCode + ", ReasonPhrase: '" + this.reasonPhrase + "'"));
      this.statusCode = statusCode >= (HttpStatusCode) 0 && statusCode <= (HttpStatusCode) 999 ? statusCode : throw new ArgumentOutOfRangeException(nameof (statusCode));
      this.version = HttpUtilities.DefaultVersion;
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    /// <summary>Throws an exception if the <see cref="P:System.Net.Http.HttpResponseMessage.IsSuccessStatusCode" /> property for the HTTP response is false.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpResponseMessage" />.The HTTP response message if the call is successful.</returns>
    public HttpResponseMessage EnsureSuccessStatusCode()
    {
      if (!this.IsSuccessStatusCode)
      {
        if (this.content != null)
          this.content.Dispose();
        throw new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_message_not_success_statuscode, (object) (int) this.statusCode, (object) this.ReasonPhrase));
      }
      return this;
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>Returns <see cref="T:System.String" />.A string representation of the current object.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("StatusCode: ");
      stringBuilder.Append((int) this.statusCode);
      stringBuilder.Append(", ReasonPhrase: '");
      stringBuilder.Append(this.ReasonPhrase ?? "<null>");
      stringBuilder.Append("', Version: ");
      stringBuilder.Append((object) this.version);
      stringBuilder.Append(", Content: ");
      stringBuilder.Append(this.content == null ? "<null>" : this.content.GetType().FullName);
      stringBuilder.Append(", Headers:\r\n");
      stringBuilder.Append(HeaderUtilities.DumpHeaders((HttpHeaders) this.headers, this.content == null ? (HttpHeaders) null : (HttpHeaders) this.content.Headers));
      return stringBuilder.ToString();
    }

    private bool ContainsNewLineCharacter(string value)
    {
      foreach (char ch in value)
      {
        switch (ch)
        {
          case '\n':
          case '\r':
            return true;
          default:
            continue;
        }
      }
      return false;
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" /> and optionally disposes of the managed resources.</summary>
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

    /// <summary>Releases the unmanaged resources and disposes of unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" />.</summary>
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
