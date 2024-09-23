// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.DecodedNumeric
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal sealed class DecodedNumeric : DecodedObject
  {
    private readonly int firstDigit;
    private readonly int secondDigit;
    internal static int FNC1 = 10;

    internal DecodedNumeric(int newPosition, int firstDigit, int secondDigit)
      : base(newPosition)
    {
      this.firstDigit = firstDigit >= 0 && firstDigit <= 10 && secondDigit >= 0 && secondDigit <= 10 ? firstDigit : throw FormatException.Instance;
      this.secondDigit = secondDigit;
    }

    internal int getFirstDigit() => this.firstDigit;

    internal int getSecondDigit() => this.secondDigit;

    internal int getValue() => this.firstDigit * 10 + this.secondDigit;

    internal bool isFirstDigitFNC1() => this.firstDigit == DecodedNumeric.FNC1;

    internal bool isSecondDigitFNC1() => this.secondDigit == DecodedNumeric.FNC1;

    internal bool isAnyFNC1()
    {
      return this.firstDigit == DecodedNumeric.FNC1 || this.secondDigit == DecodedNumeric.FNC1;
    }
  }
}
