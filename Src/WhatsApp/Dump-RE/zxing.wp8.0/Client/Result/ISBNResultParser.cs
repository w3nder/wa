// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ISBNResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>Parses strings of digits that represent a ISBN.</summary>
  /// <author>jbreiden@google.com (Jeff Breidenbach)</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public class ISBNResultParser : ResultParser
  {
    /// <summary>
    /// See <a href="http://www.bisg.org/isbn-13/for.dummies.html">ISBN-13 For Dummies</a>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public override ParsedResult parse(ZXing.Result result)
    {
      if (result.BarcodeFormat != BarcodeFormat.EAN_13)
        return (ParsedResult) null;
      string text = result.Text;
      if (text.Length != 13)
        return (ParsedResult) null;
      return !text.StartsWith("978") && !text.StartsWith("979") ? (ParsedResult) null : (ParsedResult) new ISBNParsedResult(text);
    }
  }
}
