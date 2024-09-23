// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.TelParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class TelParsedResult : ParsedResult
  {
    public TelParsedResult(string number, string telURI, string title)
      : base(ParsedResultType.TEL)
    {
      this.Number = number;
      this.TelURI = telURI;
      this.Title = title;
      StringBuilder result = new StringBuilder(20);
      ParsedResult.maybeAppend(number, result);
      ParsedResult.maybeAppend(title, result);
      this.displayResultValue = result.ToString();
    }

    public string Number { get; private set; }

    public string TelURI { get; private set; }

    public string Title { get; private set; }
  }
}
