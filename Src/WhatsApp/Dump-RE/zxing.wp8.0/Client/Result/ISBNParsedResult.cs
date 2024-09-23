// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ISBNParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>jbreiden@google.com (Jeff Breidenbach)</author>
  public sealed class ISBNParsedResult : ParsedResult
  {
    internal ISBNParsedResult(string isbn)
      : base(ParsedResultType.ISBN)
    {
      this.ISBN = isbn;
      this.displayResultValue = isbn;
    }

    public string ISBN { get; private set; }
  }
}
