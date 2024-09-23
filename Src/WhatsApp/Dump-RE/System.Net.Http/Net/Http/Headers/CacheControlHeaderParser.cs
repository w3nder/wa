// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.CacheControlHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class CacheControlHeaderParser : BaseHeaderParser
  {
    internal static readonly CacheControlHeaderParser Parser = new CacheControlHeaderParser();

    private CacheControlHeaderParser()
      : base(true)
    {
    }

    protected override int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue)
    {
      CacheControlHeaderValue parsedValue1 = storeValue as CacheControlHeaderValue;
      Contract.Assert(storeValue == null || parsedValue1 != null, "'storeValue' is not of type CacheControlHeaderValue");
      int cacheControlLength = CacheControlHeaderValue.GetCacheControlLength(value, startIndex, parsedValue1, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return cacheControlLength;
    }
  }
}
