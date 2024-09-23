// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.BaseHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http.Headers
{
  internal abstract class BaseHeaderParser : HttpHeaderParser
  {
    protected BaseHeaderParser(bool supportsMultipleValues)
      : base(supportsMultipleValues)
    {
    }

    protected abstract int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue);

    public override sealed bool TryParseValue(
      string value,
      object storeValue,
      ref int index,
      out object parsedValue)
    {
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(value) || index == value.Length)
        return this.SupportsMultipleValues;
      bool separatorFound = false;
      int orWhitespaceIndex1 = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, index, this.SupportsMultipleValues, out separatorFound);
      if (separatorFound && !this.SupportsMultipleValues)
        return false;
      if (orWhitespaceIndex1 == value.Length)
      {
        if (this.SupportsMultipleValues)
          index = orWhitespaceIndex1;
        return this.SupportsMultipleValues;
      }
      object parsedValue1 = (object) null;
      int parsedValueLength = this.GetParsedValueLength(value, orWhitespaceIndex1, storeValue, out parsedValue1);
      if (parsedValueLength == 0)
        return false;
      int startIndex = orWhitespaceIndex1 + parsedValueLength;
      int orWhitespaceIndex2 = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, startIndex, this.SupportsMultipleValues, out separatorFound);
      if (separatorFound && !this.SupportsMultipleValues || !separatorFound && orWhitespaceIndex2 < value.Length)
        return false;
      index = orWhitespaceIndex2;
      parsedValue = parsedValue1;
      return true;
    }
  }
}
