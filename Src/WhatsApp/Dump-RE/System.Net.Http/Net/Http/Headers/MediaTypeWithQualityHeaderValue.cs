// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a content-type header value with an additional quality.</summary>
  public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue, ICloneable
  {
    /// <returns>Returns <see cref="T:System.Double" />.</returns>
    public double? Quality
    {
      get => HeaderUtilities.GetQuality(this.Parameters);
      set => HeaderUtilities.SetQuality(this.Parameters, value);
    }

    internal MediaTypeWithQualityHeaderValue()
    {
    }

    public MediaTypeWithQualityHeaderValue(string mediaType)
      : base(mediaType)
    {
    }

    public MediaTypeWithQualityHeaderValue(string mediaType, double quality)
      : base(mediaType)
    {
      this.Quality = new double?(quality);
    }

    private MediaTypeWithQualityHeaderValue(MediaTypeWithQualityHeaderValue source)
      : base((MediaTypeHeaderValue) source)
    {
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new MediaTypeWithQualityHeaderValue(this);

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" />.</returns>
    public static MediaTypeWithQualityHeaderValue Parse(string input)
    {
      int index = 0;
      return (MediaTypeWithQualityHeaderValue) MediaTypeHeaderParser.SingleValueWithQualityParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out MediaTypeWithQualityHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (MediaTypeWithQualityHeaderValue) null;
      object parsedValue1;
      if (!MediaTypeHeaderParser.SingleValueWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (MediaTypeWithQualityHeaderValue) parsedValue1;
      return true;
    }
  }
}
