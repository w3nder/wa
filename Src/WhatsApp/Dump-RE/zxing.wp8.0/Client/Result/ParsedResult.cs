// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> <p>Abstract class representing the result of decoding a barcode, as more than
  /// a String -- as some type of structured data. This might be a subclass which represents
  /// a URL, or an e-mail address. {@link ResultParser#parseResult(Result)} will turn a raw
  /// decoded string into the most appropriate type of structured representation.</p>
  /// 
  /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
  /// on exception-based mechanisms during parsing.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public abstract class ParsedResult
  {
    protected string displayResultValue;

    public virtual ParsedResultType Type { get; private set; }

    public virtual string DisplayResult => this.displayResultValue;

    protected ParsedResult(ParsedResultType type) => this.Type = type;

    public override string ToString() => this.DisplayResult;

    public override bool Equals(object obj)
    {
      return obj is ParsedResult parsedResult && parsedResult.Type.Equals((object) this.Type) && parsedResult.DisplayResult.Equals(this.DisplayResult);
    }

    public override int GetHashCode() => this.Type.GetHashCode() + this.DisplayResult.GetHashCode();

    public static void maybeAppend(string value, StringBuilder result)
    {
      if (string.IsNullOrEmpty(value))
        return;
      if (result.Length > 0)
        result.Append('\n');
      result.Append(value);
    }

    public static void maybeAppend(string[] values, StringBuilder result)
    {
      if (values == null)
        return;
      foreach (string str in values)
        ParsedResult.maybeAppend(str, result);
    }
  }
}
