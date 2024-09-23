// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.AbstractDoCoMoResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> <p>See
  /// <a href="http://www.nttdocomo.co.jp/english/service/imode/make/content/barcode/about/s2.html">
  /// DoCoMo's documentation</a> about the result types represented by subclasses of this class.</p>
  /// 
  /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
  /// on exception-based mechanisms during parsing.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal abstract class AbstractDoCoMoResultParser : ResultParser
  {
    internal static string[] matchDoCoMoPrefixedField(string prefix, string rawText, bool trim)
    {
      return ResultParser.matchPrefixedField(prefix, rawText, ';', trim);
    }

    internal static string matchSingleDoCoMoPrefixedField(string prefix, string rawText, bool trim)
    {
      return ResultParser.matchSinglePrefixedField(prefix, rawText, ';', trim);
    }
  }
}
