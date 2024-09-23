// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.FieldParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal static class FieldParser
  {
    private static readonly object VARIABLE_LENGTH = new object();
    private static readonly IDictionary<string, object[]> TWO_DIGIT_DATA_LENGTH = (IDictionary<string, object[]>) new Dictionary<string, object[]>()
    {
      {
        "00",
        new object[1]{ (object) 18 }
      },
      {
        "01",
        new object[1]{ (object) 14 }
      },
      {
        "02",
        new object[1]{ (object) 14 }
      },
      {
        "10",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 20 }
      },
      {
        "11",
        new object[1]{ (object) 6 }
      },
      {
        "12",
        new object[1]{ (object) 6 }
      },
      {
        "13",
        new object[1]{ (object) 6 }
      },
      {
        "15",
        new object[1]{ (object) 6 }
      },
      {
        "17",
        new object[1]{ (object) 6 }
      },
      {
        "20",
        new object[1]{ (object) 2 }
      },
      {
        "21",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 20 }
      },
      {
        "22",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 29 }
      },
      {
        "30",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 8 }
      },
      {
        "37",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 8 }
      },
      {
        "90",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "91",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "92",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "93",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "94",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "95",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "96",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "97",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "98",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "99",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      }
    };
    private static readonly IDictionary<string, object[]> THREE_DIGIT_DATA_LENGTH = (IDictionary<string, object[]>) new Dictionary<string, object[]>()
    {
      {
        "240",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "241",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "242",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 6 }
      },
      {
        "250",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "251",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "253",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 17 }
      },
      {
        "254",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 20 }
      },
      {
        "400",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "401",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "402",
        new object[1]{ (object) 17 }
      },
      {
        "403",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "410",
        new object[1]{ (object) 13 }
      },
      {
        "411",
        new object[1]{ (object) 13 }
      },
      {
        "412",
        new object[1]{ (object) 13 }
      },
      {
        "413",
        new object[1]{ (object) 13 }
      },
      {
        "414",
        new object[1]{ (object) 13 }
      },
      {
        "420",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 20 }
      },
      {
        "421",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 15 }
      },
      {
        "422",
        new object[1]{ (object) 3 }
      },
      {
        "423",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 15 }
      },
      {
        "424",
        new object[1]{ (object) 3 }
      },
      {
        "425",
        new object[1]{ (object) 3 }
      },
      {
        "426",
        new object[1]{ (object) 3 }
      }
    };
    private static readonly IDictionary<string, object[]> THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH = (IDictionary<string, object[]>) new Dictionary<string, object[]>()
    {
      {
        "310",
        new object[1]{ (object) 6 }
      },
      {
        "311",
        new object[1]{ (object) 6 }
      },
      {
        "312",
        new object[1]{ (object) 6 }
      },
      {
        "313",
        new object[1]{ (object) 6 }
      },
      {
        "314",
        new object[1]{ (object) 6 }
      },
      {
        "315",
        new object[1]{ (object) 6 }
      },
      {
        "316",
        new object[1]{ (object) 6 }
      },
      {
        "320",
        new object[1]{ (object) 6 }
      },
      {
        "321",
        new object[1]{ (object) 6 }
      },
      {
        "322",
        new object[1]{ (object) 6 }
      },
      {
        "323",
        new object[1]{ (object) 6 }
      },
      {
        "324",
        new object[1]{ (object) 6 }
      },
      {
        "325",
        new object[1]{ (object) 6 }
      },
      {
        "326",
        new object[1]{ (object) 6 }
      },
      {
        "327",
        new object[1]{ (object) 6 }
      },
      {
        "328",
        new object[1]{ (object) 6 }
      },
      {
        "329",
        new object[1]{ (object) 6 }
      },
      {
        "330",
        new object[1]{ (object) 6 }
      },
      {
        "331",
        new object[1]{ (object) 6 }
      },
      {
        "332",
        new object[1]{ (object) 6 }
      },
      {
        "333",
        new object[1]{ (object) 6 }
      },
      {
        "334",
        new object[1]{ (object) 6 }
      },
      {
        "335",
        new object[1]{ (object) 6 }
      },
      {
        "336",
        new object[1]{ (object) 6 }
      },
      {
        "340",
        new object[1]{ (object) 6 }
      },
      {
        "341",
        new object[1]{ (object) 6 }
      },
      {
        "342",
        new object[1]{ (object) 6 }
      },
      {
        "343",
        new object[1]{ (object) 6 }
      },
      {
        "344",
        new object[1]{ (object) 6 }
      },
      {
        "345",
        new object[1]{ (object) 6 }
      },
      {
        "346",
        new object[1]{ (object) 6 }
      },
      {
        "347",
        new object[1]{ (object) 6 }
      },
      {
        "348",
        new object[1]{ (object) 6 }
      },
      {
        "349",
        new object[1]{ (object) 6 }
      },
      {
        "350",
        new object[1]{ (object) 6 }
      },
      {
        "351",
        new object[1]{ (object) 6 }
      },
      {
        "352",
        new object[1]{ (object) 6 }
      },
      {
        "353",
        new object[1]{ (object) 6 }
      },
      {
        "354",
        new object[1]{ (object) 6 }
      },
      {
        "355",
        new object[1]{ (object) 6 }
      },
      {
        "356",
        new object[1]{ (object) 6 }
      },
      {
        "357",
        new object[1]{ (object) 6 }
      },
      {
        "360",
        new object[1]{ (object) 6 }
      },
      {
        "361",
        new object[1]{ (object) 6 }
      },
      {
        "362",
        new object[1]{ (object) 6 }
      },
      {
        "363",
        new object[1]{ (object) 6 }
      },
      {
        "364",
        new object[1]{ (object) 6 }
      },
      {
        "365",
        new object[1]{ (object) 6 }
      },
      {
        "366",
        new object[1]{ (object) 6 }
      },
      {
        "367",
        new object[1]{ (object) 6 }
      },
      {
        "368",
        new object[1]{ (object) 6 }
      },
      {
        "369",
        new object[1]{ (object) 6 }
      },
      {
        "390",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 15 }
      },
      {
        "391",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 18 }
      },
      {
        "392",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 15 }
      },
      {
        "393",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 18 }
      },
      {
        "703",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      }
    };
    private static readonly IDictionary<string, object[]> FOUR_DIGIT_DATA_LENGTH = (IDictionary<string, object[]>) new Dictionary<string, object[]>()
    {
      {
        "7001",
        new object[1]{ (object) 13 }
      },
      {
        "7002",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "7003",
        new object[1]{ (object) 10 }
      },
      {
        "8001",
        new object[1]{ (object) 14 }
      },
      {
        "8002",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 20 }
      },
      {
        "8003",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "8004",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "8005",
        new object[1]{ (object) 6 }
      },
      {
        "8006",
        new object[1]{ (object) 18 }
      },
      {
        "8007",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 30 }
      },
      {
        "8008",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 12 }
      },
      {
        "8018",
        new object[1]{ (object) 18 }
      },
      {
        "8020",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 25 }
      },
      {
        "8100",
        new object[1]{ (object) 6 }
      },
      {
        "8101",
        new object[1]{ (object) 10 }
      },
      {
        "8102",
        new object[1]{ (object) 2 }
      },
      {
        "8110",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 70 }
      },
      {
        "8200",
        new object[2]{ FieldParser.VARIABLE_LENGTH, (object) 70 }
      }
    };

    internal static string parseFieldsInGeneralPurpose(string rawInformation)
    {
      if (string.IsNullOrEmpty(rawInformation))
        return (string) null;
      if (rawInformation.Length < 2)
        return (string) null;
      string key1 = rawInformation.Substring(0, 2);
      if (FieldParser.TWO_DIGIT_DATA_LENGTH.ContainsKey(key1))
      {
        object[] objArray = FieldParser.TWO_DIGIT_DATA_LENGTH[key1];
        return objArray[0] == FieldParser.VARIABLE_LENGTH ? FieldParser.processVariableAI(2, (int) objArray[1], rawInformation) : FieldParser.processFixedAI(2, (int) objArray[0], rawInformation);
      }
      if (rawInformation.Length < 3)
        return (string) null;
      string key2 = rawInformation.Substring(0, 3);
      if (FieldParser.THREE_DIGIT_DATA_LENGTH.ContainsKey(key2))
      {
        object[] objArray = FieldParser.THREE_DIGIT_DATA_LENGTH[key2];
        return objArray[0] == FieldParser.VARIABLE_LENGTH ? FieldParser.processVariableAI(3, (int) objArray[1], rawInformation) : FieldParser.processFixedAI(3, (int) objArray[0], rawInformation);
      }
      if (FieldParser.THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH.ContainsKey(key2))
      {
        object[] objArray = FieldParser.THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH[key2];
        return objArray[0] == FieldParser.VARIABLE_LENGTH ? FieldParser.processVariableAI(4, (int) objArray[1], rawInformation) : FieldParser.processFixedAI(4, (int) objArray[0], rawInformation);
      }
      if (rawInformation.Length < 4)
        return (string) null;
      string key3 = rawInformation.Substring(0, 4);
      if (!FieldParser.FOUR_DIGIT_DATA_LENGTH.ContainsKey(key3))
        return (string) null;
      object[] objArray1 = FieldParser.FOUR_DIGIT_DATA_LENGTH[key3];
      return objArray1[0] == FieldParser.VARIABLE_LENGTH ? FieldParser.processVariableAI(4, (int) objArray1[1], rawInformation) : FieldParser.processFixedAI(4, (int) objArray1[0], rawInformation);
    }

    private static string processFixedAI(int aiSize, int fieldSize, string rawInformation)
    {
      if (rawInformation.Length < aiSize)
        return (string) null;
      string str1 = rawInformation.Substring(0, aiSize);
      if (rawInformation.Length < aiSize + fieldSize)
        return (string) null;
      string str2 = rawInformation.Substring(aiSize, fieldSize);
      string rawInformation1 = rawInformation.Substring(aiSize + fieldSize);
      string str3 = '('.ToString() + str1 + (object) ')' + str2;
      string inGeneralPurpose = FieldParser.parseFieldsInGeneralPurpose(rawInformation1);
      return inGeneralPurpose != null ? str3 + inGeneralPurpose : str3;
    }

    private static string processVariableAI(
      int aiSize,
      int variableFieldSize,
      string rawInformation)
    {
      string str1 = rawInformation.Substring(0, aiSize);
      int startIndex = rawInformation.Length >= aiSize + variableFieldSize ? aiSize + variableFieldSize : rawInformation.Length;
      string str2 = rawInformation.Substring(aiSize, startIndex - aiSize);
      string rawInformation1 = rawInformation.Substring(startIndex);
      string str3 = '('.ToString() + str1 + (object) ')' + str2;
      string inGeneralPurpose = FieldParser.parseFieldsInGeneralPurpose(rawInformation1);
      return inGeneralPurpose != null ? str3 + inGeneralPurpose : str3;
    }
  }
}
