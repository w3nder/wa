// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.DecodedInformation
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
  internal sealed class DecodedInformation : DecodedObject
  {
    private string newString;
    private int remainingValue;
    private bool remaining;

    internal DecodedInformation(int newPosition, string newString)
      : base(newPosition)
    {
      this.newString = newString;
      this.remaining = false;
      this.remainingValue = 0;
    }

    internal DecodedInformation(int newPosition, string newString, int remainingValue)
      : base(newPosition)
    {
      this.remaining = true;
      this.remainingValue = remainingValue;
      this.newString = newString;
    }

    internal string getNewString() => this.newString;

    internal bool isRemaining() => this.remaining;

    internal int getRemainingValue() => this.remainingValue;
  }
}
