// Decompiled with JetBrains decompiler
// Type: System.Net.IWebProxy
// Assembly: System.Net.Http.Primitives, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 254C62AD-DCB5-4A11-92C4-B88227BACC42
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Primitives.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Primitives.xml

#nullable disable
namespace System.Net
{
  /// <summary>
  /// Provides the base interface for implementation of proxy access for the <see cref="T:System.Net.WebRequest" /> class.
  /// </summary>
  public interface IWebProxy
  {
    /// <summary>
    /// The credentials to submit to the proxy server for authentication.
    /// </summary>
    /// <returns>An <see cref="T:System.Net.ICredentials" /> instance that contains the credentials that are needed to authenticate a request to the proxy server.</returns>
    ICredentials Credentials { get; set; }

    /// <summary>Returns the URI of a proxy.</summary>
    /// <param name="destination">A <see cref="T:System.Uri" /> that specifies the requested Internet resource. </param>
    /// <returns>A <see cref="T:System.Uri" /> instance that contains the URI of the proxy used to contact <paramref name="destination" />.</returns>
    Uri GetProxy(Uri destination);

    /// <summary>
    /// Indicates that the proxy should not be used for the specified host.
    /// </summary>
    /// <param name="host">The <see cref="T:System.Uri" /> of the host to check for proxy use. </param>
    /// <returns>true if the proxy server should not be used for <paramref name="host" />; otherwise, false.</returns>
    bool IsBypassed(Uri host);
  }
}
