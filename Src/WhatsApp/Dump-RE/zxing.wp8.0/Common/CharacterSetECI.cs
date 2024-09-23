// Decompiled with JetBrains decompiler
// Type: ZXing.Common.CharacterSetECI
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Common
{
  /// <summary> Encapsulates a Character Set ECI, according to "Extended Channel Interpretations" 5.3.1.1
  /// of ISO 18004.
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class CharacterSetECI : ECI
  {
    internal static readonly IDictionary<int, CharacterSetECI> VALUE_TO_ECI = (IDictionary<int, CharacterSetECI>) new Dictionary<int, CharacterSetECI>();
    internal static readonly IDictionary<string, CharacterSetECI> NAME_TO_ECI = (IDictionary<string, CharacterSetECI>) new Dictionary<string, CharacterSetECI>();
    private readonly string encodingName;

    public string EncodingName => this.encodingName;

    static CharacterSetECI()
    {
      CharacterSetECI.addCharacterSet(0, "CP437");
      CharacterSetECI.addCharacterSet(1, new string[2]
      {
        "ISO-8859-1",
        "ISO8859_1"
      });
      CharacterSetECI.addCharacterSet(2, "CP437");
      CharacterSetECI.addCharacterSet(3, new string[2]
      {
        "ISO-8859-1",
        "ISO8859_1"
      });
      CharacterSetECI.addCharacterSet(4, new string[2]
      {
        "ISO-8859-2",
        "ISO8859_2"
      });
      CharacterSetECI.addCharacterSet(5, new string[2]
      {
        "ISO-8859-3",
        "ISO8859_3"
      });
      CharacterSetECI.addCharacterSet(6, new string[2]
      {
        "ISO-8859-4",
        "ISO8859_4"
      });
      CharacterSetECI.addCharacterSet(7, new string[2]
      {
        "ISO-8859-5",
        "ISO8859_5"
      });
      CharacterSetECI.addCharacterSet(8, new string[2]
      {
        "ISO-8859-6",
        "ISO8859_6"
      });
      CharacterSetECI.addCharacterSet(9, new string[2]
      {
        "ISO-8859-7",
        "ISO8859_7"
      });
      CharacterSetECI.addCharacterSet(10, new string[2]
      {
        "ISO-8859-8",
        "ISO8859_8"
      });
      CharacterSetECI.addCharacterSet(11, new string[2]
      {
        "ISO-8859-9",
        "ISO8859_9"
      });
      CharacterSetECI.addCharacterSet(12, new string[3]
      {
        "ISO-8859-4",
        "ISO-8859-10",
        "ISO8859_10"
      });
      CharacterSetECI.addCharacterSet(13, new string[2]
      {
        "ISO-8859-11",
        "ISO8859_11"
      });
      CharacterSetECI.addCharacterSet(15, new string[2]
      {
        "ISO-8859-13",
        "ISO8859_13"
      });
      CharacterSetECI.addCharacterSet(16, new string[3]
      {
        "ISO-8859-1",
        "ISO-8859-14",
        "ISO8859_14"
      });
      CharacterSetECI.addCharacterSet(17, new string[2]
      {
        "ISO-8859-15",
        "ISO8859_15"
      });
      CharacterSetECI.addCharacterSet(18, new string[3]
      {
        "ISO-8859-3",
        "ISO-8859-16",
        "ISO8859_16"
      });
      CharacterSetECI.addCharacterSet(20, new string[2]
      {
        "SJIS",
        "Shift_JIS"
      });
      CharacterSetECI.addCharacterSet(21, new string[2]
      {
        "WINDOWS-1250",
        "CP1250"
      });
      CharacterSetECI.addCharacterSet(22, new string[2]
      {
        "WINDOWS-1251",
        "CP1251"
      });
      CharacterSetECI.addCharacterSet(23, new string[2]
      {
        "WINDOWS-1252",
        "CP1252"
      });
      CharacterSetECI.addCharacterSet(24, new string[2]
      {
        "WINDOWS-1256",
        "CP1256"
      });
      CharacterSetECI.addCharacterSet(25, new string[2]
      {
        "UTF-16BE",
        "UNICODEBIG"
      });
      CharacterSetECI.addCharacterSet(26, new string[2]
      {
        "UTF-8",
        "UTF8"
      });
      CharacterSetECI.addCharacterSet(27, "US-ASCII");
      CharacterSetECI.addCharacterSet(170, "US-ASCII");
      CharacterSetECI.addCharacterSet(28, "BIG5");
      CharacterSetECI.addCharacterSet(29, new string[4]
      {
        "GB18030",
        "GB2312",
        "EUC_CN",
        "GBK"
      });
      CharacterSetECI.addCharacterSet(30, new string[2]
      {
        "EUC-KR",
        "EUC_KR"
      });
    }

    private CharacterSetECI(int value, string encodingName)
      : base(value)
    {
      this.encodingName = encodingName;
    }

    private static void addCharacterSet(int value, string encodingName)
    {
      CharacterSetECI characterSetEci = new CharacterSetECI(value, encodingName);
      CharacterSetECI.VALUE_TO_ECI[value] = characterSetEci;
      CharacterSetECI.NAME_TO_ECI[encodingName] = characterSetEci;
    }

    private static void addCharacterSet(int value, string[] encodingNames)
    {
      CharacterSetECI characterSetEci = new CharacterSetECI(value, encodingNames[0]);
      CharacterSetECI.VALUE_TO_ECI[value] = characterSetEci;
      foreach (string encodingName in encodingNames)
        CharacterSetECI.NAME_TO_ECI[encodingName] = characterSetEci;
    }

    /// <param name="value">character set ECI value</param>
    /// <returns> {@link CharacterSetECI} representing ECI of given value, or null if it is legal but
    /// unsupported
    /// </returns>
    /// <throws>  IllegalArgumentException if ECI value is invalid </throws>
    public static CharacterSetECI getCharacterSetECIByValue(int value)
    {
      return value < 0 || value >= 900 ? (CharacterSetECI) null : CharacterSetECI.VALUE_TO_ECI[value];
    }

    /// <param name="name">character set ECI encoding name</param>
    /// <returns> {@link CharacterSetECI} representing ECI for character encoding, or null if it is legal
    /// but unsupported
    /// </returns>
    public static CharacterSetECI getCharacterSetECIByName(string name)
    {
      return CharacterSetECI.NAME_TO_ECI[name.ToUpper()];
    }
  }
}
