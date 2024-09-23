// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRequestException
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base class for exceptions thrown by the <see cref="T:System.Net.Http.HttpClient" /> and <see cref="T:System.Net.Http.HttpMessageHandler" /> classes.</summary>
  [Serializable]
  public class HttpRequestException : Exception
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class.</summary>
    public HttpRequestException()
      : this((string) null, (Exception) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class with a specific message that describes the current exception.</summary>
    /// <param name="message">A message that describes the current exception.</param>
    public HttpRequestException(string message)
      : this(message, (Exception) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class with a specific message that describes the current exception and an inner exception.</summary>
    /// <param name="message">A message that describes the current exception.</param>
    /// <param name="inner">The inner exception.</param>
    public HttpRequestException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
