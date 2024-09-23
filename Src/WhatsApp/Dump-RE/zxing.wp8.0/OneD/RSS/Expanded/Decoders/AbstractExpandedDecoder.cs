// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.AbstractExpandedDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  public abstract class AbstractExpandedDecoder
  {
    private readonly BitArray information;
    private readonly GeneralAppIdDecoder generalDecoder;

    internal AbstractExpandedDecoder(BitArray information)
    {
      this.information = information;
      this.generalDecoder = new GeneralAppIdDecoder(information);
    }

    /// <summary>Gets the information.</summary>
    /// <returns></returns>
    protected BitArray getInformation() => this.information;

    internal GeneralAppIdDecoder getGeneralDecoder() => this.generalDecoder;

    /// <summary>Parses the information.</summary>
    /// <returns></returns>
    public abstract string parseInformation();

    /// <summary>Creates the decoder.</summary>
    /// <param name="information">The information.</param>
    /// <returns></returns>
    public static AbstractExpandedDecoder createDecoder(BitArray information)
    {
      if (information[1])
        return (AbstractExpandedDecoder) new AI01AndOtherAIs(information);
      if (!information[2])
        return (AbstractExpandedDecoder) new AnyAIDecoder(information);
      switch (GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 4))
      {
        case 4:
          return (AbstractExpandedDecoder) new AI013103decoder(information);
        case 5:
          return (AbstractExpandedDecoder) new AI01320xDecoder(information);
        default:
          switch (GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 5))
          {
            case 12:
              return (AbstractExpandedDecoder) new AI01392xDecoder(information);
            case 13:
              return (AbstractExpandedDecoder) new AI01393xDecoder(information);
            default:
              switch (GeneralAppIdDecoder.extractNumericValueFromBitArray(information, 1, 7))
              {
                case 56:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "310", "11");
                case 57:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "320", "11");
                case 58:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "310", "13");
                case 59:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "320", "13");
                case 60:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "310", "15");
                case 61:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "320", "15");
                case 62:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "310", "17");
                case 63:
                  return (AbstractExpandedDecoder) new AI013x0x1xDecoder(information, "320", "17");
                default:
                  throw new InvalidOperationException("unknown decoder: " + (object) information);
              }
          }
      }
    }
  }
}
