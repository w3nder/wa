// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMethod
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  /// <summary>A helper class for retrieving and comparing standard HTTP methods.</summary>
  public class HttpMethod : IEquatable<HttpMethod>
  {
    private string method;
    private static readonly HttpMethod getMethod = new HttpMethod("GET");
    private static readonly HttpMethod putMethod = new HttpMethod("PUT");
    private static readonly HttpMethod postMethod = new HttpMethod("POST");
    private static readonly HttpMethod deleteMethod = new HttpMethod("DELETE");
    private static readonly HttpMethod headMethod = new HttpMethod("HEAD");
    private static readonly HttpMethod optionsMethod = new HttpMethod("OPTIONS");
    private static readonly HttpMethod traceMethod = new HttpMethod("TRACE");

    /// <summary>Represents an HTTP GET protocol method.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Get => HttpMethod.getMethod;

    /// <summary>Represents an HTTP PUT protocol method that is used to replace an entity identified by a URI.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Put => HttpMethod.putMethod;

    /// <summary>Represents an HTTP POST protocol method that is used to post a new entity as an addition to a URI.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Post => HttpMethod.postMethod;

    /// <summary>Represents an HTTP DELETE protocol method.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Delete => HttpMethod.deleteMethod;

    /// <summary>Represents an HTTP HEAD protocol method. The HEAD method is identical to GET except that the server only returns message-headers in the response, without a message-body.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Head => HttpMethod.headMethod;

    /// <summary>Represents an HTTP OPTIONS protocol method.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Options => HttpMethod.optionsMethod;

    /// <summary>Represents an HTTP TRACE protocol method.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.HttpMethod" />.</returns>
    public static HttpMethod Trace => HttpMethod.traceMethod;

    /// <summary>An HTTP method. </summary>
    /// <returns>Returns <see cref="T:System.String" />.An HTTP method represented as a <see cref="T:System.String" />.</returns>
    public string Method => this.method;

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpMethod" /> class with a specific HTTP method.</summary>
    /// <param name="method">The HTTP method.</param>
    public HttpMethod(string method)
    {
      if (string.IsNullOrEmpty(method))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (method));
      this.method = HttpRuleParser.GetTokenLength(method, 0) == method.Length ? method : throw new FormatException(SR.net_http_httpmethod_format_error);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Equals(HttpMethod other)
    {
      if ((object) other == null)
        return false;
      return object.ReferenceEquals((object) this.method, (object) other.method) || string.Compare(this.method, other.method, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj) => this.Equals(obj as HttpMethod);

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode() => this.method.ToUpperInvariant().GetHashCode();

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>Returns <see cref="T:System.String" />.A string representing the current object.</returns>
    public override string ToString() => this.method.ToString();

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool operator ==(HttpMethod left, HttpMethod right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right == null ? (object) left == null : left.Equals(right);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool operator !=(HttpMethod left, HttpMethod right) => !(left == right);
  }
}
