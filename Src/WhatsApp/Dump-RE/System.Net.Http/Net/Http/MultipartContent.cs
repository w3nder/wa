// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MultipartContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Provides a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
  public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
  {
    private const string crlf = "\r\n";
    private List<HttpContent> nestedContent;
    private string boundary;
    private int nextContentIndex;
    private Stream outputStream;
    private TaskCompletionSource<object> tcs;

    public MultipartContent()
      : this("mixed", MultipartContent.GetDefaultBoundary())
    {
    }

    public MultipartContent(string subtype)
      : this(subtype, MultipartContent.GetDefaultBoundary())
    {
    }

    public MultipartContent(string subtype, string boundary)
    {
      if (string.IsNullOrWhiteSpace(subtype))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (subtype));
      Contract.EndContractBlock();
      MultipartContent.ValidateBoundary(boundary);
      this.boundary = boundary;
      string str = boundary;
      if (!str.StartsWith("\"", StringComparison.Ordinal))
        str = "\"" + str + "\"";
      this.Headers.ContentType = new MediaTypeHeaderValue("multipart/" + subtype)
      {
        Parameters = {
          new NameValueHeaderValue(nameof (boundary), str)
        }
      };
      this.nestedContent = new List<HttpContent>();
    }

    private static void ValidateBoundary(string boundary)
    {
      if (string.IsNullOrWhiteSpace(boundary))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (boundary));
      if (boundary.Length > 70)
        throw new ArgumentOutOfRangeException(nameof (boundary), (object) boundary, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_field_too_long, (object) 70));
      if (boundary.EndsWith(" ", StringComparison.Ordinal))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) boundary), nameof (boundary));
      Contract.EndContractBlock();
      string str = "'()+_,-./:=? ";
      foreach (char ch in boundary)
      {
        if (('0' > ch || ch > '9') && ('a' > ch || ch > 'z') && ('A' > ch || ch > 'Z') && str.IndexOf(ch) < 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) boundary), nameof (boundary));
      }
    }

    private static string GetDefaultBoundary() => Guid.NewGuid().ToString();

    public virtual void Add(HttpContent content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      Contract.EndContractBlock();
      this.nestedContent.Add(content);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (HttpContent httpContent in this.nestedContent)
          httpContent.Dispose();
        this.nestedContent.Clear();
      }
      base.Dispose(disposing);
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerator`1" />.</returns>
    public IEnumerator<HttpContent> GetEnumerator()
    {
      return (IEnumerator<HttpContent>) this.nestedContent.GetEnumerator();
    }

    /// <returns>Returns <see cref="T:System.Collections.IEnumerator" />.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.nestedContent.GetEnumerator();

    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.</returns>
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      Contract.Assert(stream != null);
      Contract.Assert(this.outputStream == null, "Opperation already in progress");
      Contract.Assert(this.tcs == null, "Opperation already in progress");
      Contract.Assert(this.nextContentIndex == 0, "Opperation already in progress");
      TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
      this.tcs = completionSource;
      this.outputStream = stream;
      this.nextContentIndex = 0;
      MultipartContent.EncodeStringToStreamAsync(this.outputStream, "--" + this.boundary + "\r\n").ContinueWithStandard(new Action<Task>(this.WriteNextContentHeadersAsync));
      return (Task) completionSource.Task;
    }

    private void WriteNextContentHeadersAsync(Task task)
    {
      if (task.IsFaulted)
      {
        this.HandleAsyncException(nameof (WriteNextContentHeadersAsync), ((Exception) task.Exception).GetBaseException());
      }
      else
      {
        try
        {
          if (this.nextContentIndex >= this.nestedContent.Count)
          {
            this.WriteTerminatingBoundaryAsync();
          }
          else
          {
            string str = "\r\n--" + this.boundary + "\r\n";
            StringBuilder stringBuilder = new StringBuilder();
            if (this.nextContentIndex != 0)
              stringBuilder.Append(str);
            foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) this.nestedContent[this.nextContentIndex].Headers)
              stringBuilder.Append(header.Key + ": " + string.Join(", ", header.Value) + "\r\n");
            stringBuilder.Append("\r\n");
            MultipartContent.EncodeStringToStreamAsync(this.outputStream, stringBuilder.ToString()).ContinueWithStandard(new Action<Task>(this.WriteNextContentAsync));
          }
        }
        catch (Exception ex)
        {
          this.HandleAsyncException(nameof (WriteNextContentHeadersAsync), ex);
        }
      }
    }

    private void WriteNextContentAsync(Task task)
    {
      if (task.IsFaulted)
      {
        this.HandleAsyncException(nameof (WriteNextContentAsync), ((Exception) task.Exception).GetBaseException());
      }
      else
      {
        try
        {
          HttpContent httpContent = this.nestedContent[this.nextContentIndex];
          ++this.nextContentIndex;
          httpContent.CopyToAsync(this.outputStream).ContinueWithStandard(new Action<Task>(this.WriteNextContentHeadersAsync));
        }
        catch (Exception ex)
        {
          this.HandleAsyncException(nameof (WriteNextContentAsync), ex);
        }
      }
    }

    private void WriteTerminatingBoundaryAsync()
    {
      try
      {
        MultipartContent.EncodeStringToStreamAsync(this.outputStream, "\r\n--" + this.boundary + "--\r\n").ContinueWithStandard((Action<Task>) (task =>
        {
          if (task.IsFaulted)
            this.HandleAsyncException(nameof (WriteTerminatingBoundaryAsync), ((Exception) task.Exception).GetBaseException());
          else
            this.CleanupAsync().TrySetResult((object) null);
        }));
      }
      catch (Exception ex)
      {
        this.HandleAsyncException(nameof (WriteTerminatingBoundaryAsync), ex);
      }
    }

    private static Task EncodeStringToStreamAsync(Stream stream, string input)
    {
      byte[] bytes = HttpRuleParser.DefaultHttpEncoding.GetBytes(input);
      return Task.Factory.FromAsync<byte[], int, int>(new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginWrite), new Action<IAsyncResult>(stream.EndWrite), bytes, 0, bytes.Length, (object) null);
    }

    private TaskCompletionSource<object> CleanupAsync()
    {
      Contract.Requires(this.tcs != null, "Operation already cleaned up");
      TaskCompletionSource<object> tcs = this.tcs;
      this.outputStream = (Stream) null;
      this.nextContentIndex = 0;
      this.tcs = (TaskCompletionSource<object>) null;
      return tcs;
    }

    private void HandleAsyncException(string method, Exception ex)
    {
      if (Logging.On)
        Logging.Exception(Logging.Http, (object) this, method, ex);
      this.CleanupAsync().TrySetException(ex);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    protected internal override bool TryComputeLength(out long length)
    {
      long num1 = 0;
      long encodedLength = (long) MultipartContent.GetEncodedLength("\r\n--" + this.boundary + "\r\n");
      long num2 = num1 + (long) MultipartContent.GetEncodedLength("--" + this.boundary + "\r\n");
      bool flag = true;
      foreach (HttpContent httpContent in this.nestedContent)
      {
        if (flag)
          flag = false;
        else
          num2 += encodedLength;
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) httpContent.Headers)
          num2 += (long) MultipartContent.GetEncodedLength(header.Key + ": " + string.Join(", ", header.Value) + "\r\n");
        num2 += (long) "\r\n".Length;
        long length1 = 0;
        if (!httpContent.TryComputeLength(out length1))
        {
          length = 0L;
          return false;
        }
        num2 += length1;
      }
      long num3 = num2 + (long) MultipartContent.GetEncodedLength("\r\n--" + this.boundary + "--\r\n");
      length = num3;
      return true;
    }

    private static int GetEncodedLength(string input)
    {
      return HttpRuleParser.DefaultHttpEncoding.GetByteCount(input);
    }
  }
}
