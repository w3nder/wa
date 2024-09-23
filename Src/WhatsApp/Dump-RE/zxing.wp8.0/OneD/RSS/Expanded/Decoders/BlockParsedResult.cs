// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.BlockParsedResult
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
  internal sealed class BlockParsedResult
  {
    private DecodedInformation decodedInformation;
    private bool finished;

    internal BlockParsedResult(bool finished)
      : this((DecodedInformation) null, finished)
    {
    }

    internal BlockParsedResult(DecodedInformation information, bool finished)
    {
      this.finished = finished;
      this.decodedInformation = information;
    }

    internal DecodedInformation getDecodedInformation() => this.decodedInformation;

    internal bool isFinished() => this.finished;
  }
}
