// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ExpandedProductResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// Parses strings of digits that represent a RSS Extended code.
  /// </summary>
  /// <author>Antonio Manuel Benjumea Conde, Servinform, S.A.</author>
  /// <author>Agustín Delgado, Servinform, S.A.</author>
  public class ExpandedProductResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      if (result.BarcodeFormat != BarcodeFormat.RSS_EXPANDED)
        return (ParsedResult) null;
      string text = result.Text;
      string productID = (string) null;
      string sscc = (string) null;
      string lotNumber = (string) null;
      string productionDate = (string) null;
      string packagingDate = (string) null;
      string bestBeforeDate = (string) null;
      string expirationDate = (string) null;
      string weight = (string) null;
      string weightType = (string) null;
      string weightIncrement = (string) null;
      string price = (string) null;
      string priceIncrement = (string) null;
      string priceCurrency = (string) null;
      Dictionary<string, string> uncommonAIs = new Dictionary<string, string>();
      int i1 = 0;
      while (i1 < text.Length)
      {
        string aivalue = ExpandedProductResultParser.findAIvalue(i1, text);
        if (aivalue == null)
          return (ParsedResult) null;
        int i2 = i1 + (aivalue.Length + 2);
        string str = ExpandedProductResultParser.findValue(i2, text);
        i1 = i2 + str.Length;
        if ("00".Equals(aivalue))
          sscc = str;
        else if ("01".Equals(aivalue))
          productID = str;
        else if ("10".Equals(aivalue))
          lotNumber = str;
        else if ("11".Equals(aivalue))
          productionDate = str;
        else if ("13".Equals(aivalue))
          packagingDate = str;
        else if ("15".Equals(aivalue))
          bestBeforeDate = str;
        else if ("17".Equals(aivalue))
          expirationDate = str;
        else if ("3100".Equals(aivalue) || "3101".Equals(aivalue) || "3102".Equals(aivalue) || "3103".Equals(aivalue) || "3104".Equals(aivalue) || "3105".Equals(aivalue) || "3106".Equals(aivalue) || "3107".Equals(aivalue) || "3108".Equals(aivalue) || "3109".Equals(aivalue))
        {
          weight = str;
          weightType = ExpandedProductParsedResult.KILOGRAM;
          weightIncrement = aivalue.Substring(3);
        }
        else if ("3200".Equals(aivalue) || "3201".Equals(aivalue) || "3202".Equals(aivalue) || "3203".Equals(aivalue) || "3204".Equals(aivalue) || "3205".Equals(aivalue) || "3206".Equals(aivalue) || "3207".Equals(aivalue) || "3208".Equals(aivalue) || "3209".Equals(aivalue))
        {
          weight = str;
          weightType = ExpandedProductParsedResult.POUND;
          weightIncrement = aivalue.Substring(3);
        }
        else if ("3920".Equals(aivalue) || "3921".Equals(aivalue) || "3922".Equals(aivalue) || "3923".Equals(aivalue))
        {
          price = str;
          priceIncrement = aivalue.Substring(3);
        }
        else if ("3930".Equals(aivalue) || "3931".Equals(aivalue) || "3932".Equals(aivalue) || "3933".Equals(aivalue))
        {
          if (str.Length < 4)
            return (ParsedResult) null;
          price = str.Substring(3);
          priceCurrency = str.Substring(0, 3);
          priceIncrement = aivalue.Substring(3);
        }
        else
          uncommonAIs[aivalue] = str;
      }
      return (ParsedResult) new ExpandedProductParsedResult(text, productID, sscc, lotNumber, productionDate, packagingDate, bestBeforeDate, expirationDate, weight, weightType, weightIncrement, price, priceIncrement, priceCurrency, (IDictionary<string, string>) uncommonAIs);
    }

    private static string findAIvalue(int i, string rawText)
    {
      if (rawText[i] != '(')
        return (string) null;
      string str = rawText.Substring(i + 1);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < str.Length; ++index)
      {
        char ch = str[index];
        switch (ch)
        {
          case ')':
            return stringBuilder.ToString();
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            stringBuilder.Append(ch);
            continue;
          default:
            return (string) null;
        }
      }
      return stringBuilder.ToString();
    }

    private static string findValue(int i, string rawText)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string rawText1 = rawText.Substring(i);
      for (int index = 0; index < rawText1.Length; ++index)
      {
        char ch = rawText1[index];
        if (ch == '(')
        {
          if (ExpandedProductResultParser.findAIvalue(index, rawText1) == null)
            stringBuilder.Append('(');
          else
            break;
        }
        else
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }
  }
}
