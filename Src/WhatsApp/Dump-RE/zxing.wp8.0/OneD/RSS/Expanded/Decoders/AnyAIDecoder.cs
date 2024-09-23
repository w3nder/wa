// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AnyAIDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal sealed class AnyAIDecoder : AbstractExpandedDecoder
  {
    private static int HEADER_SIZE = 5;

    internal AnyAIDecoder(BitArray information)
      : base(information)
    {
    }

    public override string parseInformation()
    {
      StringBuilder buff = new StringBuilder();
      return this.getGeneralDecoder().decodeAllCodes(buff, AnyAIDecoder.HEADER_SIZE);
    }
  }
}
