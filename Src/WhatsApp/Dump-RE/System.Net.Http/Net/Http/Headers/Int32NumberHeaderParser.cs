// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.Int32NumberHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class Int32NumberHeaderParser : BaseHeaderParser
  {
    internal static readonly Int32NumberHeaderParser Parser = new Int32NumberHeaderParser();

    private Int32NumberHeaderParser()
      : base(false)
    {
    }

    public override string ToString(object value)
    {
      Contract.Assert(value is int);
      return ((int) value).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    protected override int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue)
    {
      parsedValue = (object) null;
      int numberLength = HttpRuleParser.GetNumberLength(value, startIndex, false);
      if (numberLength == 0 || numberLength > 10)
        return 0;
      int result = 0;
      if (!HeaderUtilities.TryParseInt32(value.Substring(startIndex, numberLength), out result))
        return 0;
      parsedValue = (object) result;
      return numberLength;
    }
  }
}
