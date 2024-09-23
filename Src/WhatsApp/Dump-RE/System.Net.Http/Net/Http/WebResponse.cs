// Decompiled with JetBrains decompiler
// Type: System.Net.Http.WebResponse
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Globalization;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace System.Net.Http
{
  internal class WebResponse : IDisposable
  {
    private System.Net.WebResponse actualResponse;
    private Stream responseStream;

    public WebResponse(System.Net.WebResponse response, DecompressionMethods decompressionMethods)
    {
      this.actualResponse = response;
      this.ContentLength = this.actualResponse.ContentLength == (long) uint.MaxValue ? -1L : this.actualResponse.ContentLength;
      if (this.ContentLength == 0L)
      {
        string s = this.Headers["Content-Length"];
        if (s != null)
        {
          int num1 = s.IndexOf(':');
          if (num1 != -1)
            s = s.Substring(num1 + 1);
          long result;
          if (!long.TryParse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
          {
            result = -1L;
            int num2 = s.LastIndexOf(',');
            if (num2 != -1 && !long.TryParse(s.Substring(num2 + 1), NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
              result = -1L;
          }
          if (result > 0L)
            this.ContentLength = result;
        }
      }
      this.responseStream = this.actualResponse.GetResponseStream() ?? (Stream) new MemoryStream();
      if (decompressionMethods == DecompressionMethods.None)
        return;
      string header = this.Headers["Content-Encoding"];
      if (header == null || !Capabilities.SupportsAutomaticDecompression)
        return;
      if ((decompressionMethods & DecompressionMethods.GZip) != DecompressionMethods.None && header.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) != -1)
      {
        this.responseStream = (Stream) new GZipStream(this.responseStream, CompressionMode.Decompress, true);
        this.ContentLength = -1L;
        this.Headers["Content-Encoding"] = (string) null;
      }
      else
      {
        if ((decompressionMethods & DecompressionMethods.Deflate) == DecompressionMethods.None || header.IndexOf("deflate", StringComparison.OrdinalIgnoreCase) == -1)
          return;
        this.responseStream = (Stream) new DeflateStream(this.responseStream, CompressionMode.Decompress, true);
        this.ContentLength = -1L;
        this.Headers["Content-Encoding"] = (string) null;
      }
    }

    public long ContentLength { get; private set; }

    public string ContentType => this.actualResponse.ContentType;

    public WebHeaderCollection Headers
    {
      get
      {
        try
        {
          return this.actualResponse.Headers;
        }
        catch (NotImplementedException ex)
        {
          return new WebHeaderCollection();
        }
      }
    }

    public Uri ResponseUri => this.actualResponse.ResponseUri;

    public bool SupportsHeaders => this.actualResponse.SupportsHeaders;

    public Stream GetResponseStream() => this.responseStream;

    public void Dispose()
    {
      if (this.responseStream != null && this.responseStream != this.actualResponse.GetResponseStream())
      {
        this.responseStream.Dispose();
        this.responseStream = (Stream) null;
      }
      this.actualResponse.Dispose();
    }
  }
}
