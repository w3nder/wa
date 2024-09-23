// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MultipartFormDataContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Net.Http.Headers;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Provides a container for content encoded using multipart/form-data MIME type.</summary>
  public class MultipartFormDataContent : MultipartContent
  {
    private const string formData = "form-data";

    public MultipartFormDataContent()
      : base("form-data")
    {
    }

    public MultipartFormDataContent(string boundary)
      : base("form-data", boundary)
    {
    }

    public override void Add(HttpContent content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      Contract.EndContractBlock();
      if (content.Headers.ContentDisposition == null)
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
      base.Add(content);
    }

    public void Add(HttpContent content, string name)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
      Contract.EndContractBlock();
      this.AddInternal(content, name, (string) null);
    }

    public void Add(HttpContent content, string name, string fileName)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (fileName));
      Contract.EndContractBlock();
      this.AddInternal(content, name, fileName);
    }

    private void AddInternal(HttpContent content, string name, string fileName)
    {
      if (content.Headers.ContentDisposition == null)
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
          Name = name,
          FileName = fileName,
          FileNameStar = fileName
        };
      base.Add(content);
    }
  }
}
