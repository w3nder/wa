// Decompiled with JetBrains decompiler
// Type: System.Net.Http.FormUrlEncodedContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A container for name/value tuples encoded using application/x-www-form-urlencoded MIME type.</summary>
  public class FormUrlEncodedContent : ByteArrayContent
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.FormUrlEncodedContent" /> class with a specific collection of name/value pairs.</summary>
    /// <param name="nameValueCollection">A collection of name/value pairs.</param>
    public FormUrlEncodedContent(
      IEnumerable<KeyValuePair<string, string>> nameValueCollection)
      : base(FormUrlEncodedContent.GetContentByteArray(nameValueCollection))
    {
      this.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
    }

    private static byte[] GetContentByteArray(
      IEnumerable<KeyValuePair<string, string>> nameValueCollection)
    {
      if (nameValueCollection == null)
        throw new ArgumentNullException(nameof (nameValueCollection));
      Contract.EndContractBlock();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> nameValue in nameValueCollection)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append('&');
        stringBuilder.Append(FormUrlEncodedContent.Encode(nameValue.Key));
        stringBuilder.Append('=');
        stringBuilder.Append(FormUrlEncodedContent.Encode(nameValue.Value));
      }
      return HttpRuleParser.DefaultHttpEncoding.GetBytes(stringBuilder.ToString());
    }

    private static string Encode(string data)
    {
      return string.IsNullOrEmpty(data) ? string.Empty : Uri.EscapeDataString(data).Replace("%20", "+");
    }
  }
}
