// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingWithQualityHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a transfer-coding header value with optional quality.</summary>
  public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue, ICloneable
  {
    /// <returns>Returns <see cref="T:System.Double" />.</returns>
    public double? Quality
    {
      get => HeaderUtilities.GetQuality(this.Parameters);
      set => HeaderUtilities.SetQuality(this.Parameters, value);
    }

    internal TransferCodingWithQualityHeaderValue()
    {
    }

    public TransferCodingWithQualityHeaderValue(string value)
      : base(value)
    {
    }

    public TransferCodingWithQualityHeaderValue(string value, double quality)
      : base(value)
    {
      this.Quality = new double?(quality);
    }

    private TransferCodingWithQualityHeaderValue(TransferCodingWithQualityHeaderValue source)
      : base((TransferCodingHeaderValue) source)
    {
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new TransferCodingWithQualityHeaderValue(this);

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.</returns>
    public static TransferCodingWithQualityHeaderValue Parse(string input)
    {
      int index = 0;
      return (TransferCodingWithQualityHeaderValue) TransferCodingHeaderParser.SingleValueWithQualityParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(
      string input,
      out TransferCodingWithQualityHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (TransferCodingWithQualityHeaderValue) null;
      object parsedValue1;
      if (!TransferCodingHeaderParser.SingleValueWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (TransferCodingWithQualityHeaderValue) parsedValue1;
      return true;
    }
  }
}
