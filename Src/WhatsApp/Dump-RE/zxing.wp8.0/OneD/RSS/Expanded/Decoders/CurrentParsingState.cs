// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.CurrentParsingState
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// </summary>
  internal sealed class CurrentParsingState
  {
    private int position;
    private CurrentParsingState.State encoding;

    internal CurrentParsingState()
    {
      this.position = 0;
      this.encoding = CurrentParsingState.State.NUMERIC;
    }

    internal int getPosition() => this.position;

    internal void setPosition(int position) => this.position = position;

    internal void incrementPosition(int delta) => this.position += delta;

    internal bool isAlpha() => this.encoding == CurrentParsingState.State.ALPHA;

    internal bool isNumeric() => this.encoding == CurrentParsingState.State.NUMERIC;

    internal bool isIsoIec646() => this.encoding == CurrentParsingState.State.ISO_IEC_646;

    internal void setNumeric() => this.encoding = CurrentParsingState.State.NUMERIC;

    internal void setAlpha() => this.encoding = CurrentParsingState.State.ALPHA;

    internal void setIsoIec646() => this.encoding = CurrentParsingState.State.ISO_IEC_646;

    private enum State
    {
      NUMERIC,
      ALPHA,
      ISO_IEC_646,
    }
  }
}
