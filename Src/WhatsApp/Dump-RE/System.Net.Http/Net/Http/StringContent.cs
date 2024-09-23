// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StringContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Provides HTTP content based on a string.</summary>
  public class StringContent : ByteArrayContent
  {
    private const string defaultMediaType = "text/plain";

    public StringContent(string content)
      : this(content, (Encoding) null, (string) null)
    {
    }

    public StringContent(string content, Encoding encoding)
      : this(content, encoding, (string) null)
    {
    }

    public StringContent(string content, Encoding encoding, string mediaType)
      : base(StringContent.GetContentByteArray(content, encoding))
    {
      this.Headers.ContentType = new MediaTypeHeaderValue(mediaType == null ? "text/plain" : mediaType)
      {
        CharSet = encoding == null ? HttpContent.DefaultStringEncoding.WebName : encoding.WebName
      };
    }

    private static byte[] GetContentByteArray(string content, Encoding encoding)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (encoding == null)
        encoding = HttpContent.DefaultStringEncoding;
      return encoding.GetBytes(content);
    }
  }
}
