// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class MediaTypeHeaderParser : BaseHeaderParser
  {
    private bool supportsMultipleValues;
    private Func<MediaTypeHeaderValue> mediaTypeCreator;
    internal static readonly MediaTypeHeaderParser SingleValueParser = new MediaTypeHeaderParser(false, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaType));
    internal static readonly MediaTypeHeaderParser SingleValueWithQualityParser = new MediaTypeHeaderParser(false, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaTypeWithQuality));
    internal static readonly MediaTypeHeaderParser MultipleValuesParser = new MediaTypeHeaderParser(true, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaTypeWithQuality));

    private MediaTypeHeaderParser(
      bool supportsMultipleValues,
      Func<MediaTypeHeaderValue> mediaTypeCreator)
      : base(supportsMultipleValues)
    {
      Contract.Requires(mediaTypeCreator != null);
      this.supportsMultipleValues = supportsMultipleValues;
      this.mediaTypeCreator = mediaTypeCreator;
    }

    protected override int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue)
    {
      MediaTypeHeaderValue parsedValue1 = (MediaTypeHeaderValue) null;
      int mediaTypeLength = MediaTypeHeaderValue.GetMediaTypeLength(value, startIndex, this.mediaTypeCreator, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return mediaTypeLength;
    }

    private static MediaTypeHeaderValue CreateMediaType() => new MediaTypeHeaderValue();

    private static MediaTypeHeaderValue CreateMediaTypeWithQuality()
    {
      return (MediaTypeHeaderValue) new MediaTypeWithQualityHeaderValue();
    }
  }
}
