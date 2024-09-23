// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.DateHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class DateHeaderParser : HttpHeaderParser
  {
    internal static readonly DateHeaderParser Parser = new DateHeaderParser();

    private DateHeaderParser()
      : base(false)
    {
    }

    public override string ToString(object value)
    {
      Contract.Assert(value is DateTimeOffset);
      return HttpRuleParser.DateToString((DateTimeOffset) value);
    }

    public override bool TryParseValue(
      string value,
      object storeValue,
      ref int index,
      out object parsedValue)
    {
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(value) || index == value.Length)
        return false;
      string input = value;
      if (index > 0)
        input = value.Substring(index);
      DateTimeOffset result;
      if (!HttpRuleParser.TryStringToDate(input, out result))
        return false;
      index = value.Length;
      parsedValue = (object) result;
      return true;
    }
  }
}
