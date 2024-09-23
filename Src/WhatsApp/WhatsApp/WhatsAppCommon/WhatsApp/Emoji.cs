// Decompiled with JetBrains decompiler
// Type: WhatsApp.Emoji
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using WhatsApp.RegularExpressions;


namespace WhatsApp
{
  public sealed class Emoji
  {
    public const char EmojiInvisibleSpacer = '\u200B';
    public const string EmojiInvisibleSpacerRegex = "(\u200B+)";
    public static List<string> TopFifty = new List<string>()
    {
      "\uD83D\uDE02",
      "\uD83D\uDE0D",
      "\uD83D\uDE18",
      "\uD83E\uDD23",
      "❤",
      "\uD83D\uDE00",
      "\uD83D\uDE0A",
      "\uD83D\uDE0E",
      "\uD83D\uDE01",
      "\uD83D\uDE09",
      "\uD83E\uDD14",
      "\uD83D\uDE2D",
      "\uD83D\uDE05",
      "\uD83E\uDD17",
      "\uD83D\uDE0B",
      "\uD83D\uDE1C",
      "\uD83D\uDC4F",
      "\uD83D\uDE0F",
      "\uD83D\uDE04",
      "\uD83D\uDC4D",
      "\uD83D\uDE03",
      "\uD83D\uDE25",
      "\uD83D\uDE42",
      "☺",
      "\uD83D\uDC96",
      "\uD83D\uDE21",
      "\uD83D\uDE06",
      "\uD83D\uDE2A",
      "\uD83D\uDC4C",
      "\uD83D\uDE4F",
      "\uD83D\uDC95",
      "\uD83C\uDF82",
      "\uD83D\uDE44",
      "\uD83D\uDE1D",
      "\uD83D\uDE32",
      "\uD83C\uDF89",
      "\uD83D\uDE12",
      "\uD83D\uDE14",
      "\uD83D\uDCAA",
      "\uD83D\uDE31",
      "\uD83D\uDE1B",
      "✌",
      "\uD83D\uDE1A",
      "\uD83D\uDE2E",
      "\uD83D\uDC94",
      "\uD83E\uDD24",
      "\uD83D\uDC9E",
      "\uD83D\uDE0C",
      "\uD83D\uDC99",
      "\uD83D\uDC8B"
    };
    public const int MaxImagesPerSpriteSheet = 100;
    public const string EmojiRootPath = "/Images/emojis/";
    public const int EmojiSize = 64;
    public const int PadBetweenEmojis = 1;
    public const int OuterPadding = 1;
    public const int SpriteSheetNumColumns = 10;
    public const int SpriteSheetNumRows = 10;
    public const int EmojiMappingsCount = 2403;
    private static string[][] softbankCodepoints = (string[][]) null;
    public static string[] emojisWithVariationSelectorCodepoints = (string[]) null;
    public const string SkinModifierStart = "\uD83C\uDFFB";
    private static Regex flagMacro = (Regex) null;
    private static Regex invisibleRegex = (Regex) null;
    private static Regex longEmojiRegex = (Regex) null;
    private static Dictionary<string, string> coupleOffset = (Dictionary<string, string>) null;
    private static Dictionary<string, string> reverseCoupleOffset = (Dictionary<string, string>) null;

    public static bool IsSkintonePrefix(string codepoint)
    {
      switch (Emoji.FilterVariationSelectors(codepoint))
      {
        case "☝":
        case "✊":
        case "✋":
        case "✌":
        case "✍":
        case "\uD83C\uDF85":
        case "\uD83C\uDFC3":
        case "\uD83C\uDFC4":
        case "\uD83C\uDFC7":
        case "\uD83C\uDFCA":
        case "\uD83C\uDFCB":
        case "\uD83C\uDFCC":
        case "\uD83D\uDC42":
        case "\uD83D\uDC43":
        case "\uD83D\uDC46":
        case "\uD83D\uDC47":
        case "\uD83D\uDC48":
        case "\uD83D\uDC49":
        case "\uD83D\uDC4A":
        case "\uD83D\uDC4B":
        case "\uD83D\uDC4C":
        case "\uD83D\uDC4D":
        case "\uD83D\uDC4E":
        case "\uD83D\uDC4F":
        case "\uD83D\uDC50":
        case "\uD83D\uDC66":
        case "\uD83D\uDC67":
        case "\uD83D\uDC68":
        case "\uD83D\uDC69":
        case "\uD83D\uDC6E":
        case "\uD83D\uDC6F":
        case "\uD83D\uDC70":
        case "\uD83D\uDC71":
        case "\uD83D\uDC72":
        case "\uD83D\uDC73":
        case "\uD83D\uDC74":
        case "\uD83D\uDC75":
        case "\uD83D\uDC76":
        case "\uD83D\uDC77":
        case "\uD83D\uDC78":
        case "\uD83D\uDC7C":
        case "\uD83D\uDC81":
        case "\uD83D\uDC82":
        case "\uD83D\uDC83":
        case "\uD83D\uDC85":
        case "\uD83D\uDC86":
        case "\uD83D\uDC87":
        case "\uD83D\uDCAA":
        case "\uD83D\uDD74":
        case "\uD83D\uDD75":
        case "\uD83D\uDD7A":
        case "\uD83D\uDD90":
        case "\uD83D\uDD95":
        case "\uD83D\uDD96":
        case "\uD83D\uDE45":
        case "\uD83D\uDE46":
        case "\uD83D\uDE47":
        case "\uD83D\uDE4B":
        case "\uD83D\uDE4C":
        case "\uD83D\uDE4D":
        case "\uD83D\uDE4E":
        case "\uD83D\uDE4F":
        case "\uD83D\uDEA3":
        case "\uD83D\uDEB4":
        case "\uD83D\uDEB5":
        case "\uD83D\uDEB6":
        case "\uD83D\uDEC0":
        case "\uD83D\uDECC":
        case "\uD83E\uDD18":
        case "\uD83E\uDD19":
        case "\uD83E\uDD1A":
        case "\uD83E\uDD1B":
        case "\uD83E\uDD1C":
        case "\uD83E\uDD1D":
        case "\uD83E\uDD1E":
        case "\uD83E\uDD1F":
        case "\uD83E\uDD26":
        case "\uD83E\uDD30":
        case "\uD83E\uDD31":
        case "\uD83E\uDD32":
        case "\uD83E\uDD33":
        case "\uD83E\uDD34":
        case "\uD83E\uDD35":
        case "\uD83E\uDD36":
        case "\uD83E\uDD37":
        case "\uD83E\uDD38":
        case "\uD83E\uDD39":
        case "\uD83E\uDD3D":
        case "\uD83E\uDD3E":
        case "\uD83E\uDDD1":
        case "\uD83E\uDDD2":
        case "\uD83E\uDDD3":
        case "\uD83E\uDDD4":
        case "\uD83E\uDDD5":
        case "\uD83E\uDDD6":
        case "\uD83E\uDDD7":
        case "\uD83E\uDDD8":
        case "\uD83E\uDDD9":
        case "\uD83E\uDDDA":
        case "\uD83E\uDDDB":
        case "\uD83E\uDDDC":
        case "\uD83E\uDDDD":
          return true;
        default:
          return false;
      }
    }

    private static int TryGetEmojiIndex(string str, ref int offset, int len)
    {
      int emojiIndex = -1;
      int num1 = offset;
      bool filterVariationSelectors1 = false;
      len += offset;
      int num2 = Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors1);
      bool filterVariationSelectors2 = false;
      switch (num2)
      {
        case 35:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 1;
            break;
          }
          break;
        case 42:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 2;
            break;
          }
          break;
        case 48:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 3;
            break;
          }
          break;
        case 49:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 4;
            break;
          }
          break;
        case 50:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 5;
            break;
          }
          break;
        case 51:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 6;
            break;
          }
          break;
        case 52:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 7;
            break;
          }
          break;
        case 53:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 8;
            break;
          }
          break;
        case 54:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 9;
            break;
          }
          break;
        case 55:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 10;
            break;
          }
          break;
        case 56:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 11;
            break;
          }
          break;
        case 57:
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8419)
          {
            num1 = offset;
            emojiIndex = 12;
            break;
          }
          break;
        case 169:
          num1 = offset;
          emojiIndex = 13;
          break;
        case 174:
          num1 = offset;
          emojiIndex = 14;
          break;
        case 8252:
          num1 = offset;
          emojiIndex = 2202;
          break;
        case 8265:
          num1 = offset;
          emojiIndex = 2203;
          break;
        case 8482:
          num1 = offset;
          emojiIndex = 2204;
          break;
        case 8505:
          num1 = offset;
          emojiIndex = 2205;
          break;
        case 8596:
          num1 = offset;
          emojiIndex = 2206;
          break;
        case 8597:
          num1 = offset;
          emojiIndex = 2207;
          break;
        case 8598:
          num1 = offset;
          emojiIndex = 2208;
          break;
        case 8599:
          num1 = offset;
          emojiIndex = 2209;
          break;
        case 8600:
          num1 = offset;
          emojiIndex = 2210;
          break;
        case 8601:
          num1 = offset;
          emojiIndex = 2211;
          break;
        case 8617:
          num1 = offset;
          emojiIndex = 2212;
          break;
        case 8618:
          num1 = offset;
          emojiIndex = 2213;
          break;
        case 8986:
          num1 = offset;
          emojiIndex = 2214;
          break;
        case 8987:
          num1 = offset;
          emojiIndex = 2215;
          break;
        case 9000:
          num1 = offset;
          emojiIndex = 2216;
          break;
        case 9167:
          num1 = offset;
          emojiIndex = 2217;
          break;
        case 9193:
          num1 = offset;
          emojiIndex = 2218;
          break;
        case 9194:
          num1 = offset;
          emojiIndex = 2219;
          break;
        case 9195:
          num1 = offset;
          emojiIndex = 2220;
          break;
        case 9196:
          num1 = offset;
          emojiIndex = 2221;
          break;
        case 9197:
          num1 = offset;
          emojiIndex = 2222;
          break;
        case 9198:
          num1 = offset;
          emojiIndex = 2223;
          break;
        case 9199:
          num1 = offset;
          emojiIndex = 2224;
          break;
        case 9200:
          num1 = offset;
          emojiIndex = 2225;
          break;
        case 9201:
          num1 = offset;
          emojiIndex = 2226;
          break;
        case 9202:
          num1 = offset;
          emojiIndex = 2227;
          break;
        case 9203:
          num1 = offset;
          emojiIndex = 2228;
          break;
        case 9208:
          num1 = offset;
          emojiIndex = 2229;
          break;
        case 9209:
          num1 = offset;
          emojiIndex = 2230;
          break;
        case 9210:
          num1 = offset;
          emojiIndex = 2231;
          break;
        case 9410:
          num1 = offset;
          emojiIndex = 2232;
          break;
        case 9642:
          num1 = offset;
          emojiIndex = 2233;
          break;
        case 9643:
          num1 = offset;
          emojiIndex = 2234;
          break;
        case 9654:
          num1 = offset;
          emojiIndex = 2235;
          break;
        case 9664:
          num1 = offset;
          emojiIndex = 2236;
          break;
        case 9723:
          num1 = offset;
          emojiIndex = 2237;
          break;
        case 9724:
          num1 = offset;
          emojiIndex = 2238;
          break;
        case 9725:
          num1 = offset;
          emojiIndex = 2239;
          break;
        case 9726:
          num1 = offset;
          emojiIndex = 2240;
          break;
        case 9728:
          num1 = offset;
          emojiIndex = 2241;
          break;
        case 9729:
          num1 = offset;
          emojiIndex = 2242;
          break;
        case 9730:
          num1 = offset;
          emojiIndex = 2243;
          break;
        case 9731:
          num1 = offset;
          emojiIndex = 2244;
          break;
        case 9732:
          num1 = offset;
          emojiIndex = 2245;
          break;
        case 9742:
          num1 = offset;
          emojiIndex = 2246;
          break;
        case 9745:
          num1 = offset;
          emojiIndex = 2247;
          break;
        case 9748:
          num1 = offset;
          emojiIndex = 2248;
          break;
        case 9749:
          num1 = offset;
          emojiIndex = 2249;
          break;
        case 9752:
          num1 = offset;
          emojiIndex = 2250;
          break;
        case 9757:
          num1 = offset;
          emojiIndex = 2256;
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
          {
            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
            {
              case 57339:
                num1 = offset;
                emojiIndex = 2251;
                break;
              case 57340:
                num1 = offset;
                emojiIndex = 2252;
                break;
              case 57341:
                num1 = offset;
                emojiIndex = 2253;
                break;
              case 57342:
                num1 = offset;
                emojiIndex = 2254;
                break;
              case 57343:
                num1 = offset;
                emojiIndex = 2255;
                break;
            }
          }
          else
            break;
          break;
        case 9760:
          num1 = offset;
          emojiIndex = 2257;
          break;
        case 9762:
          num1 = offset;
          emojiIndex = 2258;
          break;
        case 9763:
          num1 = offset;
          emojiIndex = 2259;
          break;
        case 9766:
          num1 = offset;
          emojiIndex = 2260;
          break;
        case 9770:
          num1 = offset;
          emojiIndex = 2261;
          break;
        case 9774:
          num1 = offset;
          emojiIndex = 2262;
          break;
        case 9775:
          num1 = offset;
          emojiIndex = 2263;
          break;
        case 9784:
          num1 = offset;
          emojiIndex = 2264;
          break;
        case 9785:
          num1 = offset;
          emojiIndex = 2265;
          break;
        case 9786:
          num1 = offset;
          emojiIndex = 2266;
          break;
        case 9792:
          num1 = offset;
          emojiIndex = 2267;
          break;
        case 9794:
          num1 = offset;
          emojiIndex = 2268;
          break;
        case 9800:
          num1 = offset;
          emojiIndex = 2269;
          break;
        case 9801:
          num1 = offset;
          emojiIndex = 2270;
          break;
        case 9802:
          num1 = offset;
          emojiIndex = 2271;
          break;
        case 9803:
          num1 = offset;
          emojiIndex = 2272;
          break;
        case 9804:
          num1 = offset;
          emojiIndex = 2273;
          break;
        case 9805:
          num1 = offset;
          emojiIndex = 2274;
          break;
        case 9806:
          num1 = offset;
          emojiIndex = 2275;
          break;
        case 9807:
          num1 = offset;
          emojiIndex = 2276;
          break;
        case 9808:
          num1 = offset;
          emojiIndex = 2277;
          break;
        case 9809:
          num1 = offset;
          emojiIndex = 2278;
          break;
        case 9810:
          num1 = offset;
          emojiIndex = 2279;
          break;
        case 9811:
          num1 = offset;
          emojiIndex = 2280;
          break;
        case 9824:
          num1 = offset;
          emojiIndex = 2281;
          break;
        case 9827:
          num1 = offset;
          emojiIndex = 2282;
          break;
        case 9829:
          num1 = offset;
          emojiIndex = 2283;
          break;
        case 9830:
          num1 = offset;
          emojiIndex = 2284;
          break;
        case 9832:
          num1 = offset;
          emojiIndex = 2285;
          break;
        case 9851:
          num1 = offset;
          emojiIndex = 2286;
          break;
        case 9855:
          num1 = offset;
          emojiIndex = 2287;
          break;
        case 9874:
          num1 = offset;
          emojiIndex = 2288;
          break;
        case 9875:
          num1 = offset;
          emojiIndex = 2289;
          break;
        case 9876:
          num1 = offset;
          emojiIndex = 2290;
          break;
        case 9877:
          num1 = offset;
          emojiIndex = 2291;
          break;
        case 9878:
          num1 = offset;
          emojiIndex = 2292;
          break;
        case 9879:
          num1 = offset;
          emojiIndex = 2293;
          break;
        case 9881:
          num1 = offset;
          emojiIndex = 2294;
          break;
        case 9883:
          num1 = offset;
          emojiIndex = 2295;
          break;
        case 9884:
          num1 = offset;
          emojiIndex = 2296;
          break;
        case 9888:
          num1 = offset;
          emojiIndex = 2297;
          break;
        case 9889:
          num1 = offset;
          emojiIndex = 2298;
          break;
        case 9898:
          num1 = offset;
          emojiIndex = 2299;
          break;
        case 9899:
          num1 = offset;
          emojiIndex = 2300;
          break;
        case 9904:
          num1 = offset;
          emojiIndex = 2301;
          break;
        case 9905:
          num1 = offset;
          emojiIndex = 2302;
          break;
        case 9917:
          num1 = offset;
          emojiIndex = 2303;
          break;
        case 9918:
          num1 = offset;
          emojiIndex = 2304;
          break;
        case 9924:
          num1 = offset;
          emojiIndex = 2305;
          break;
        case 9925:
          num1 = offset;
          emojiIndex = 2306;
          break;
        case 9928:
          num1 = offset;
          emojiIndex = 2307;
          break;
        case 9934:
          num1 = offset;
          emojiIndex = 2308;
          break;
        case 9935:
          num1 = offset;
          emojiIndex = 2309;
          break;
        case 9937:
          num1 = offset;
          emojiIndex = 2310;
          break;
        case 9939:
          num1 = offset;
          emojiIndex = 2311;
          break;
        case 9940:
          num1 = offset;
          emojiIndex = 2312;
          break;
        case 9961:
          num1 = offset;
          emojiIndex = 2313;
          break;
        case 9962:
          num1 = offset;
          emojiIndex = 2314;
          break;
        case 9968:
          num1 = offset;
          emojiIndex = 2315;
          break;
        case 9969:
          num1 = offset;
          emojiIndex = 2316;
          break;
        case 9970:
          num1 = offset;
          emojiIndex = 2317;
          break;
        case 9971:
          num1 = offset;
          emojiIndex = 2318;
          break;
        case 9972:
          num1 = offset;
          emojiIndex = 2319;
          break;
        case 9973:
          num1 = offset;
          emojiIndex = 2320;
          break;
        case 9975:
          num1 = offset;
          emojiIndex = 2321;
          break;
        case 9976:
          num1 = offset;
          emojiIndex = 2322;
          break;
        case 9977:
          num1 = offset;
          emojiIndex = 2335;
          switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
          {
            case 8205:
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 9792:
                  num1 = offset;
                  emojiIndex = 2333;
                  break;
                case 9794:
                  num1 = offset;
                  emojiIndex = 2334;
                  break;
              }
              break;
            case 55356:
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 57339:
                  if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                  {
                    switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                    {
                      case 9792:
                        num1 = offset;
                        emojiIndex = 2323;
                        break;
                      case 9794:
                        num1 = offset;
                        emojiIndex = 2324;
                        break;
                    }
                  }
                  else
                    break;
                  break;
                case 57340:
                  if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                  {
                    switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                    {
                      case 9792:
                        num1 = offset;
                        emojiIndex = 2325;
                        break;
                      case 9794:
                        num1 = offset;
                        emojiIndex = 2326;
                        break;
                    }
                  }
                  else
                    break;
                  break;
                case 57341:
                  if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                  {
                    switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                    {
                      case 9792:
                        num1 = offset;
                        emojiIndex = 2327;
                        break;
                      case 9794:
                        num1 = offset;
                        emojiIndex = 2328;
                        break;
                    }
                  }
                  else
                    break;
                  break;
                case 57342:
                  if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                  {
                    switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                    {
                      case 9792:
                        num1 = offset;
                        emojiIndex = 2329;
                        break;
                      case 9794:
                        num1 = offset;
                        emojiIndex = 2330;
                        break;
                    }
                  }
                  else
                    break;
                  break;
                case 57343:
                  if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                  {
                    switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                    {
                      case 9792:
                        num1 = offset;
                        emojiIndex = 2331;
                        break;
                      case 9794:
                        num1 = offset;
                        emojiIndex = 2332;
                        break;
                    }
                  }
                  else
                    break;
                  break;
              }
              break;
          }
          break;
        case 9978:
          num1 = offset;
          emojiIndex = 2336;
          break;
        case 9981:
          num1 = offset;
          emojiIndex = 2337;
          break;
        case 9986:
          num1 = offset;
          emojiIndex = 2338;
          break;
        case 9989:
          num1 = offset;
          emojiIndex = 2339;
          break;
        case 9992:
          num1 = offset;
          emojiIndex = 2340;
          break;
        case 9993:
          num1 = offset;
          emojiIndex = 2341;
          break;
        case 9994:
          num1 = offset;
          emojiIndex = 2347;
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
          {
            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
            {
              case 57339:
                num1 = offset;
                emojiIndex = 2342;
                break;
              case 57340:
                num1 = offset;
                emojiIndex = 2343;
                break;
              case 57341:
                num1 = offset;
                emojiIndex = 2344;
                break;
              case 57342:
                num1 = offset;
                emojiIndex = 2345;
                break;
              case 57343:
                num1 = offset;
                emojiIndex = 2346;
                break;
            }
          }
          else
            break;
          break;
        case 9995:
          num1 = offset;
          emojiIndex = 2353;
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
          {
            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
            {
              case 57339:
                num1 = offset;
                emojiIndex = 2348;
                break;
              case 57340:
                num1 = offset;
                emojiIndex = 2349;
                break;
              case 57341:
                num1 = offset;
                emojiIndex = 2350;
                break;
              case 57342:
                num1 = offset;
                emojiIndex = 2351;
                break;
              case 57343:
                num1 = offset;
                emojiIndex = 2352;
                break;
            }
          }
          else
            break;
          break;
        case 9996:
          num1 = offset;
          emojiIndex = 2359;
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
          {
            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
            {
              case 57339:
                num1 = offset;
                emojiIndex = 2354;
                break;
              case 57340:
                num1 = offset;
                emojiIndex = 2355;
                break;
              case 57341:
                num1 = offset;
                emojiIndex = 2356;
                break;
              case 57342:
                num1 = offset;
                emojiIndex = 2357;
                break;
              case 57343:
                num1 = offset;
                emojiIndex = 2358;
                break;
            }
          }
          else
            break;
          break;
        case 9997:
          num1 = offset;
          emojiIndex = 2365;
          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
          {
            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
            {
              case 57339:
                num1 = offset;
                emojiIndex = 2360;
                break;
              case 57340:
                num1 = offset;
                emojiIndex = 2361;
                break;
              case 57341:
                num1 = offset;
                emojiIndex = 2362;
                break;
              case 57342:
                num1 = offset;
                emojiIndex = 2363;
                break;
              case 57343:
                num1 = offset;
                emojiIndex = 2364;
                break;
            }
          }
          else
            break;
          break;
        case 9999:
          num1 = offset;
          emojiIndex = 2366;
          break;
        case 10002:
          num1 = offset;
          emojiIndex = 2367;
          break;
        case 10004:
          num1 = offset;
          emojiIndex = 2368;
          break;
        case 10006:
          num1 = offset;
          emojiIndex = 2369;
          break;
        case 10013:
          num1 = offset;
          emojiIndex = 2370;
          break;
        case 10017:
          num1 = offset;
          emojiIndex = 2371;
          break;
        case 10024:
          num1 = offset;
          emojiIndex = 2372;
          break;
        case 10035:
          num1 = offset;
          emojiIndex = 2373;
          break;
        case 10036:
          num1 = offset;
          emojiIndex = 2374;
          break;
        case 10052:
          num1 = offset;
          emojiIndex = 2375;
          break;
        case 10055:
          num1 = offset;
          emojiIndex = 2376;
          break;
        case 10060:
          num1 = offset;
          emojiIndex = 2377;
          break;
        case 10062:
          num1 = offset;
          emojiIndex = 2378;
          break;
        case 10067:
          num1 = offset;
          emojiIndex = 2379;
          break;
        case 10068:
          num1 = offset;
          emojiIndex = 2380;
          break;
        case 10069:
          num1 = offset;
          emojiIndex = 2381;
          break;
        case 10071:
          num1 = offset;
          emojiIndex = 2382;
          break;
        case 10083:
          num1 = offset;
          emojiIndex = 2383;
          break;
        case 10084:
          num1 = offset;
          emojiIndex = 2384;
          break;
        case 10133:
          num1 = offset;
          emojiIndex = 2385;
          break;
        case 10134:
          num1 = offset;
          emojiIndex = 2386;
          break;
        case 10135:
          num1 = offset;
          emojiIndex = 2387;
          break;
        case 10145:
          num1 = offset;
          emojiIndex = 2388;
          break;
        case 10160:
          num1 = offset;
          emojiIndex = 2389;
          break;
        case 10175:
          num1 = offset;
          emojiIndex = 2390;
          break;
        case 10548:
          num1 = offset;
          emojiIndex = 2391;
          break;
        case 10549:
          num1 = offset;
          emojiIndex = 2392;
          break;
        case 11013:
          num1 = offset;
          emojiIndex = 2393;
          break;
        case 11014:
          num1 = offset;
          emojiIndex = 2394;
          break;
        case 11015:
          num1 = offset;
          emojiIndex = 2395;
          break;
        case 11035:
          num1 = offset;
          emojiIndex = 2396;
          break;
        case 11036:
          num1 = offset;
          emojiIndex = 2397;
          break;
        case 11088:
          num1 = offset;
          emojiIndex = 2398;
          break;
        case 11093:
          num1 = offset;
          emojiIndex = 2399;
          break;
        case 12336:
          num1 = offset;
          emojiIndex = 2400;
          break;
        case 12349:
          num1 = offset;
          emojiIndex = 2401;
          break;
        case 12951:
          num1 = offset;
          emojiIndex = 2402;
          break;
        case 12953:
          num1 = offset;
          emojiIndex = 2403;
          break;
        case 55356:
          switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
          {
            case 56324:
              num1 = offset;
              emojiIndex = 15;
              break;
            case 56527:
              num1 = offset;
              emojiIndex = 16;
              break;
            case 56688:
              num1 = offset;
              emojiIndex = 17;
              break;
            case 56689:
              num1 = offset;
              emojiIndex = 18;
              break;
            case 56702:
              num1 = offset;
              emojiIndex = 19;
              break;
            case 56703:
              num1 = offset;
              emojiIndex = 20;
              break;
            case 56718:
              num1 = offset;
              emojiIndex = 21;
              break;
            case 56721:
              num1 = offset;
              emojiIndex = 22;
              break;
            case 56722:
              num1 = offset;
              emojiIndex = 23;
              break;
            case 56723:
              num1 = offset;
              emojiIndex = 24;
              break;
            case 56724:
              num1 = offset;
              emojiIndex = 25;
              break;
            case 56725:
              num1 = offset;
              emojiIndex = 26;
              break;
            case 56726:
              num1 = offset;
              emojiIndex = 27;
              break;
            case 56727:
              num1 = offset;
              emojiIndex = 28;
              break;
            case 56728:
              num1 = offset;
              emojiIndex = 29;
              break;
            case 56729:
              num1 = offset;
              emojiIndex = 30;
              break;
            case 56730:
              num1 = offset;
              emojiIndex = 31;
              break;
            case 56806:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56808:
                    num1 = offset;
                    emojiIndex = 32;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 33;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 34;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 35;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 36;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 37;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 38;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 39;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 40;
                    break;
                  case 56822:
                    num1 = offset;
                    emojiIndex = 41;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 42;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 43;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 44;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 45;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 46;
                    break;
                  case 56829:
                    num1 = offset;
                    emojiIndex = 47;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 48;
                    break;
                }
              }
              else
                break;
              break;
            case 56807:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 49;
                    break;
                  case 56807:
                    num1 = offset;
                    emojiIndex = 50;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 51;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 52;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 53;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 54;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 55;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 56;
                    break;
                  case 56815:
                    num1 = offset;
                    emojiIndex = 57;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 58;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 59;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 60;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 61;
                    break;
                  case 56822:
                    num1 = offset;
                    emojiIndex = 62;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 63;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 64;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 65;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 66;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 67;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 68;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 69;
                    break;
                }
              }
              else
                break;
              break;
            case 56808:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 70;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 71;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 72;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 73;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 74;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 75;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 76;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 77;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 78;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 79;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 80;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 81;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 82;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 83;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 84;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 85;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 86;
                    break;
                  case 56829:
                    num1 = offset;
                    emojiIndex = 87;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 88;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 89;
                    break;
                }
              }
              else
                break;
              break;
            case 56809:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56810:
                    num1 = offset;
                    emojiIndex = 90;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 91;
                    break;
                  case 56815:
                    num1 = offset;
                    emojiIndex = 92;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 93;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 94;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 95;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 96;
                    break;
                }
              }
              else
                break;
              break;
            case 56810:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 97;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 98;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 99;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 100;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 101;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 102;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 103;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 104;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 105;
                    break;
                }
              }
              else
                break;
              break;
            case 56811:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56814:
                    num1 = offset;
                    emojiIndex = 106;
                    break;
                  case 56815:
                    num1 = offset;
                    emojiIndex = 107;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 108;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 109;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 110;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 111;
                    break;
                }
              }
              else
                break;
              break;
            case 56812:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 112;
                    break;
                  case 56807:
                    num1 = offset;
                    emojiIndex = 113;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 114;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 115;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 116;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 117;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 118;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 119;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 120;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 121;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 122;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 123;
                    break;
                  case 56822:
                    num1 = offset;
                    emojiIndex = 124;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 125;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 126;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = (int) sbyte.MaxValue;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 128;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 129;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 130;
                    break;
                }
              }
              else
                break;
              break;
            case 56813:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56816:
                    num1 = offset;
                    emojiIndex = 131;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 132;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 133;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 134;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 135;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 136;
                    break;
                }
              }
              else
                break;
              break;
            case 56814:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56808:
                    num1 = offset;
                    emojiIndex = 137;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 138;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 139;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 140;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 141;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 142;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 143;
                    break;
                  case 56822:
                    num1 = offset;
                    emojiIndex = 144;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 145;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 146;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 147;
                    break;
                }
              }
              else
                break;
              break;
            case 56815:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56810:
                    num1 = offset;
                    emojiIndex = 148;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 149;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 150;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 151;
                    break;
                }
              }
              else
                break;
              break;
            case 56816:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56810:
                    num1 = offset;
                    emojiIndex = 152;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 153;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 154;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 155;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 156;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 157;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 158;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 159;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 160;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 161;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 162;
                    break;
                }
              }
              else
                break;
              break;
            case 56817:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 163;
                    break;
                  case 56807:
                    num1 = offset;
                    emojiIndex = 164;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 165;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 166;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 167;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 168;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 169;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 170;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 171;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 172;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 173;
                    break;
                }
              }
              else
                break;
              break;
            case 56818:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 174;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 175;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 176;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 177;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 178;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 179;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 180;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 181;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 182;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 183;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 184;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 185;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 186;
                    break;
                  case 56822:
                    num1 = offset;
                    emojiIndex = 187;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 188;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 189;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 190;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 191;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 192;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 193;
                    break;
                  case 56829:
                    num1 = offset;
                    emojiIndex = 194;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 195;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 196;
                    break;
                }
              }
              else
                break;
              break;
            case 56819:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 197;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 198;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 199;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 200;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 201;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 202;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 203;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 204;
                    break;
                  case 56821:
                    num1 = offset;
                    emojiIndex = 205;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 206;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 207;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 208;
                    break;
                }
              }
              else
                break;
              break;
            case 56820:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56818)
              {
                num1 = offset;
                emojiIndex = 209;
                break;
              }
              break;
            case 56821:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 210;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 211;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 212;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 213;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 214;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 215;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 216;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 217;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 218;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 219;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 220;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 221;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 222;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 223;
                    break;
                }
              }
              else
                break;
              break;
            case 56822:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56806)
              {
                num1 = offset;
                emojiIndex = 224;
                break;
              }
              break;
            case 56823:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56810:
                    num1 = offset;
                    emojiIndex = 225;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 226;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 227;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 228;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 229;
                    break;
                }
              }
              else
                break;
              break;
            case 56824:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 230;
                    break;
                  case 56807:
                    num1 = offset;
                    emojiIndex = 231;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 232;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 233;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 234;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 235;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 236;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 237;
                    break;
                  case 56815:
                    num1 = offset;
                    emojiIndex = 238;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 239;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 240;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 241;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 242;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 243;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 244;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 245;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 246;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 247;
                    break;
                  case 56829:
                    num1 = offset;
                    emojiIndex = 248;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 249;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 250;
                    break;
                }
              }
              else
                break;
              break;
            case 56825:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 251;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 252;
                    break;
                  case 56809:
                    num1 = offset;
                    emojiIndex = 253;
                    break;
                  case 56811:
                    num1 = offset;
                    emojiIndex = 254;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = (int) byte.MaxValue;
                    break;
                  case 56813:
                    num1 = offset;
                    emojiIndex = 256;
                    break;
                  case 56815:
                    num1 = offset;
                    emojiIndex = 257;
                    break;
                  case 56816:
                    num1 = offset;
                    emojiIndex = 258;
                    break;
                  case 56817:
                    num1 = offset;
                    emojiIndex = 259;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 260;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 261;
                    break;
                  case 56820:
                    num1 = offset;
                    emojiIndex = 262;
                    break;
                  case 56823:
                    num1 = offset;
                    emojiIndex = 263;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 264;
                    break;
                  case 56827:
                    num1 = offset;
                    emojiIndex = 265;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 266;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 267;
                    break;
                }
              }
              else
                break;
              break;
            case 56826:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 268;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 269;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 270;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 271;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 272;
                    break;
                  case 56830:
                    num1 = offset;
                    emojiIndex = 273;
                    break;
                  case 56831:
                    num1 = offset;
                    emojiIndex = 274;
                    break;
                }
              }
              else
                break;
              break;
            case 56827:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 275;
                    break;
                  case 56808:
                    num1 = offset;
                    emojiIndex = 276;
                    break;
                  case 56810:
                    num1 = offset;
                    emojiIndex = 277;
                    break;
                  case 56812:
                    num1 = offset;
                    emojiIndex = 278;
                    break;
                  case 56814:
                    num1 = offset;
                    emojiIndex = 279;
                    break;
                  case 56819:
                    num1 = offset;
                    emojiIndex = 280;
                    break;
                  case 56826:
                    num1 = offset;
                    emojiIndex = 281;
                    break;
                }
              }
              else
                break;
              break;
            case 56828:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56811:
                    num1 = offset;
                    emojiIndex = 282;
                    break;
                  case 56824:
                    num1 = offset;
                    emojiIndex = 283;
                    break;
                }
              }
              else
                break;
              break;
            case 56829:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56816)
              {
                num1 = offset;
                emojiIndex = 284;
                break;
              }
              break;
            case 56830:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56810:
                    num1 = offset;
                    emojiIndex = 285;
                    break;
                  case 56825:
                    num1 = offset;
                    emojiIndex = 286;
                    break;
                }
              }
              else
                break;
              break;
            case 56831:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56806:
                    num1 = offset;
                    emojiIndex = 287;
                    break;
                  case 56818:
                    num1 = offset;
                    emojiIndex = 288;
                    break;
                  case 56828:
                    num1 = offset;
                    emojiIndex = 289;
                    break;
                }
              }
              else
                break;
              break;
            case 56833:
              num1 = offset;
              emojiIndex = 290;
              break;
            case 56834:
              num1 = offset;
              emojiIndex = 291;
              break;
            case 56858:
              num1 = offset;
              emojiIndex = 292;
              break;
            case 56879:
              num1 = offset;
              emojiIndex = 293;
              break;
            case 56882:
              num1 = offset;
              emojiIndex = 294;
              break;
            case 56883:
              num1 = offset;
              emojiIndex = 295;
              break;
            case 56884:
              num1 = offset;
              emojiIndex = 296;
              break;
            case 56885:
              num1 = offset;
              emojiIndex = 297;
              break;
            case 56886:
              num1 = offset;
              emojiIndex = 298;
              break;
            case 56887:
              num1 = offset;
              emojiIndex = 299;
              break;
            case 56888:
              num1 = offset;
              emojiIndex = 300;
              break;
            case 56889:
              num1 = offset;
              emojiIndex = 301;
              break;
            case 56890:
              num1 = offset;
              emojiIndex = 302;
              break;
            case 56912:
              num1 = offset;
              emojiIndex = 303;
              break;
            case 56913:
              num1 = offset;
              emojiIndex = 304;
              break;
            case 57088:
              num1 = offset;
              emojiIndex = 305;
              break;
            case 57089:
              num1 = offset;
              emojiIndex = 306;
              break;
            case 57090:
              num1 = offset;
              emojiIndex = 307;
              break;
            case 57091:
              num1 = offset;
              emojiIndex = 308;
              break;
            case 57092:
              num1 = offset;
              emojiIndex = 309;
              break;
            case 57093:
              num1 = offset;
              emojiIndex = 310;
              break;
            case 57094:
              num1 = offset;
              emojiIndex = 311;
              break;
            case 57095:
              num1 = offset;
              emojiIndex = 312;
              break;
            case 57096:
              num1 = offset;
              emojiIndex = 313;
              break;
            case 57097:
              num1 = offset;
              emojiIndex = 314;
              break;
            case 57098:
              num1 = offset;
              emojiIndex = 315;
              break;
            case 57099:
              num1 = offset;
              emojiIndex = 316;
              break;
            case 57100:
              num1 = offset;
              emojiIndex = 317;
              break;
            case 57101:
              num1 = offset;
              emojiIndex = 318;
              break;
            case 57102:
              num1 = offset;
              emojiIndex = 319;
              break;
            case 57103:
              num1 = offset;
              emojiIndex = 320;
              break;
            case 57104:
              num1 = offset;
              emojiIndex = 321;
              break;
            case 57105:
              num1 = offset;
              emojiIndex = 322;
              break;
            case 57106:
              num1 = offset;
              emojiIndex = 323;
              break;
            case 57107:
              num1 = offset;
              emojiIndex = 324;
              break;
            case 57108:
              num1 = offset;
              emojiIndex = 325;
              break;
            case 57109:
              num1 = offset;
              emojiIndex = 326;
              break;
            case 57110:
              num1 = offset;
              emojiIndex = 327;
              break;
            case 57111:
              num1 = offset;
              emojiIndex = 328;
              break;
            case 57112:
              num1 = offset;
              emojiIndex = 329;
              break;
            case 57113:
              num1 = offset;
              emojiIndex = 330;
              break;
            case 57114:
              num1 = offset;
              emojiIndex = 331;
              break;
            case 57115:
              num1 = offset;
              emojiIndex = 332;
              break;
            case 57116:
              num1 = offset;
              emojiIndex = 333;
              break;
            case 57117:
              num1 = offset;
              emojiIndex = 334;
              break;
            case 57118:
              num1 = offset;
              emojiIndex = 335;
              break;
            case 57119:
              num1 = offset;
              emojiIndex = 336;
              break;
            case 57120:
              num1 = offset;
              emojiIndex = 337;
              break;
            case 57121:
              num1 = offset;
              emojiIndex = 338;
              break;
            case 57124:
              num1 = offset;
              emojiIndex = 339;
              break;
            case 57125:
              num1 = offset;
              emojiIndex = 340;
              break;
            case 57126:
              num1 = offset;
              emojiIndex = 341;
              break;
            case 57127:
              num1 = offset;
              emojiIndex = 342;
              break;
            case 57128:
              num1 = offset;
              emojiIndex = 343;
              break;
            case 57129:
              num1 = offset;
              emojiIndex = 344;
              break;
            case 57130:
              num1 = offset;
              emojiIndex = 345;
              break;
            case 57131:
              num1 = offset;
              emojiIndex = 346;
              break;
            case 57132:
              num1 = offset;
              emojiIndex = 347;
              break;
            case 57133:
              num1 = offset;
              emojiIndex = 348;
              break;
            case 57134:
              num1 = offset;
              emojiIndex = 349;
              break;
            case 57135:
              num1 = offset;
              emojiIndex = 350;
              break;
            case 57136:
              num1 = offset;
              emojiIndex = 351;
              break;
            case 57137:
              num1 = offset;
              emojiIndex = 352;
              break;
            case 57138:
              num1 = offset;
              emojiIndex = 353;
              break;
            case 57139:
              num1 = offset;
              emojiIndex = 354;
              break;
            case 57140:
              num1 = offset;
              emojiIndex = 355;
              break;
            case 57141:
              num1 = offset;
              emojiIndex = 356;
              break;
            case 57142:
              num1 = offset;
              emojiIndex = 357;
              break;
            case 57143:
              num1 = offset;
              emojiIndex = 358;
              break;
            case 57144:
              num1 = offset;
              emojiIndex = 359;
              break;
            case 57145:
              num1 = offset;
              emojiIndex = 360;
              break;
            case 57146:
              num1 = offset;
              emojiIndex = 361;
              break;
            case 57147:
              num1 = offset;
              emojiIndex = 362;
              break;
            case 57148:
              num1 = offset;
              emojiIndex = 363;
              break;
            case 57149:
              num1 = offset;
              emojiIndex = 364;
              break;
            case 57150:
              num1 = offset;
              emojiIndex = 365;
              break;
            case 57151:
              num1 = offset;
              emojiIndex = 366;
              break;
            case 57152:
              num1 = offset;
              emojiIndex = 367;
              break;
            case 57153:
              num1 = offset;
              emojiIndex = 368;
              break;
            case 57154:
              num1 = offset;
              emojiIndex = 369;
              break;
            case 57155:
              num1 = offset;
              emojiIndex = 370;
              break;
            case 57156:
              num1 = offset;
              emojiIndex = 371;
              break;
            case 57157:
              num1 = offset;
              emojiIndex = 372;
              break;
            case 57158:
              num1 = offset;
              emojiIndex = 373;
              break;
            case 57159:
              num1 = offset;
              emojiIndex = 374;
              break;
            case 57160:
              num1 = offset;
              emojiIndex = 375;
              break;
            case 57161:
              num1 = offset;
              emojiIndex = 376;
              break;
            case 57162:
              num1 = offset;
              emojiIndex = 377;
              break;
            case 57163:
              num1 = offset;
              emojiIndex = 378;
              break;
            case 57164:
              num1 = offset;
              emojiIndex = 379;
              break;
            case 57165:
              num1 = offset;
              emojiIndex = 380;
              break;
            case 57166:
              num1 = offset;
              emojiIndex = 381;
              break;
            case 57167:
              num1 = offset;
              emojiIndex = 382;
              break;
            case 57168:
              num1 = offset;
              emojiIndex = 383;
              break;
            case 57169:
              num1 = offset;
              emojiIndex = 384;
              break;
            case 57170:
              num1 = offset;
              emojiIndex = 385;
              break;
            case 57171:
              num1 = offset;
              emojiIndex = 386;
              break;
            case 57172:
              num1 = offset;
              emojiIndex = 387;
              break;
            case 57173:
              num1 = offset;
              emojiIndex = 388;
              break;
            case 57174:
              num1 = offset;
              emojiIndex = 389;
              break;
            case 57175:
              num1 = offset;
              emojiIndex = 390;
              break;
            case 57176:
              num1 = offset;
              emojiIndex = 391;
              break;
            case 57177:
              num1 = offset;
              emojiIndex = 392;
              break;
            case 57178:
              num1 = offset;
              emojiIndex = 393;
              break;
            case 57179:
              num1 = offset;
              emojiIndex = 394;
              break;
            case 57180:
              num1 = offset;
              emojiIndex = 395;
              break;
            case 57181:
              num1 = offset;
              emojiIndex = 396;
              break;
            case 57182:
              num1 = offset;
              emojiIndex = 397;
              break;
            case 57183:
              num1 = offset;
              emojiIndex = 398;
              break;
            case 57184:
              num1 = offset;
              emojiIndex = 399;
              break;
            case 57185:
              num1 = offset;
              emojiIndex = 400;
              break;
            case 57186:
              num1 = offset;
              emojiIndex = 401;
              break;
            case 57187:
              num1 = offset;
              emojiIndex = 402;
              break;
            case 57188:
              num1 = offset;
              emojiIndex = 403;
              break;
            case 57189:
              num1 = offset;
              emojiIndex = 404;
              break;
            case 57190:
              num1 = offset;
              emojiIndex = 405;
              break;
            case 57191:
              num1 = offset;
              emojiIndex = 406;
              break;
            case 57192:
              num1 = offset;
              emojiIndex = 407;
              break;
            case 57193:
              num1 = offset;
              emojiIndex = 408;
              break;
            case 57194:
              num1 = offset;
              emojiIndex = 409;
              break;
            case 57195:
              num1 = offset;
              emojiIndex = 410;
              break;
            case 57196:
              num1 = offset;
              emojiIndex = 411;
              break;
            case 57197:
              num1 = offset;
              emojiIndex = 412;
              break;
            case 57198:
              num1 = offset;
              emojiIndex = 413;
              break;
            case 57199:
              num1 = offset;
              emojiIndex = 414;
              break;
            case 57200:
              num1 = offset;
              emojiIndex = 415;
              break;
            case 57201:
              num1 = offset;
              emojiIndex = 416;
              break;
            case 57202:
              num1 = offset;
              emojiIndex = 417;
              break;
            case 57203:
              num1 = offset;
              emojiIndex = 418;
              break;
            case 57204:
              num1 = offset;
              emojiIndex = 419;
              break;
            case 57205:
              num1 = offset;
              emojiIndex = 420;
              break;
            case 57206:
              num1 = offset;
              emojiIndex = 421;
              break;
            case 57207:
              num1 = offset;
              emojiIndex = 422;
              break;
            case 57208:
              num1 = offset;
              emojiIndex = 423;
              break;
            case 57209:
              num1 = offset;
              emojiIndex = 424;
              break;
            case 57210:
              num1 = offset;
              emojiIndex = 425;
              break;
            case 57211:
              num1 = offset;
              emojiIndex = 426;
              break;
            case 57212:
              num1 = offset;
              emojiIndex = 427;
              break;
            case 57213:
              num1 = offset;
              emojiIndex = 428;
              break;
            case 57214:
              num1 = offset;
              emojiIndex = 429;
              break;
            case 57215:
              num1 = offset;
              emojiIndex = 430;
              break;
            case 57216:
              num1 = offset;
              emojiIndex = 431;
              break;
            case 57217:
              num1 = offset;
              emojiIndex = 432;
              break;
            case 57218:
              num1 = offset;
              emojiIndex = 433;
              break;
            case 57219:
              num1 = offset;
              emojiIndex = 434;
              break;
            case 57220:
              num1 = offset;
              emojiIndex = 435;
              break;
            case 57221:
              num1 = offset;
              emojiIndex = 441;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 436;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 437;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 438;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 439;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 440;
                    break;
                }
              }
              else
                break;
              break;
            case 57222:
              num1 = offset;
              emojiIndex = 442;
              break;
            case 57223:
              num1 = offset;
              emojiIndex = 443;
              break;
            case 57224:
              num1 = offset;
              emojiIndex = 444;
              break;
            case 57225:
              num1 = offset;
              emojiIndex = 445;
              break;
            case 57226:
              num1 = offset;
              emojiIndex = 446;
              break;
            case 57227:
              num1 = offset;
              emojiIndex = 447;
              break;
            case 57228:
              num1 = offset;
              emojiIndex = 448;
              break;
            case 57229:
              num1 = offset;
              emojiIndex = 449;
              break;
            case 57230:
              num1 = offset;
              emojiIndex = 450;
              break;
            case 57231:
              num1 = offset;
              emojiIndex = 451;
              break;
            case 57232:
              num1 = offset;
              emojiIndex = 452;
              break;
            case 57233:
              num1 = offset;
              emojiIndex = 453;
              break;
            case 57234:
              num1 = offset;
              emojiIndex = 454;
              break;
            case 57235:
              num1 = offset;
              emojiIndex = 455;
              break;
            case 57238:
              num1 = offset;
              emojiIndex = 456;
              break;
            case 57239:
              num1 = offset;
              emojiIndex = 457;
              break;
            case 57241:
              num1 = offset;
              emojiIndex = 458;
              break;
            case 57242:
              num1 = offset;
              emojiIndex = 459;
              break;
            case 57243:
              num1 = offset;
              emojiIndex = 460;
              break;
            case 57246:
              num1 = offset;
              emojiIndex = 461;
              break;
            case 57247:
              num1 = offset;
              emojiIndex = 462;
              break;
            case 57248:
              num1 = offset;
              emojiIndex = 463;
              break;
            case 57249:
              num1 = offset;
              emojiIndex = 464;
              break;
            case 57250:
              num1 = offset;
              emojiIndex = 465;
              break;
            case 57251:
              num1 = offset;
              emojiIndex = 466;
              break;
            case 57252:
              num1 = offset;
              emojiIndex = 467;
              break;
            case 57253:
              num1 = offset;
              emojiIndex = 468;
              break;
            case 57254:
              num1 = offset;
              emojiIndex = 469;
              break;
            case 57255:
              num1 = offset;
              emojiIndex = 470;
              break;
            case 57256:
              num1 = offset;
              emojiIndex = 471;
              break;
            case 57257:
              num1 = offset;
              emojiIndex = 472;
              break;
            case 57258:
              num1 = offset;
              emojiIndex = 473;
              break;
            case 57259:
              num1 = offset;
              emojiIndex = 474;
              break;
            case 57260:
              num1 = offset;
              emojiIndex = 475;
              break;
            case 57261:
              num1 = offset;
              emojiIndex = 476;
              break;
            case 57262:
              num1 = offset;
              emojiIndex = 477;
              break;
            case 57263:
              num1 = offset;
              emojiIndex = 478;
              break;
            case 57264:
              num1 = offset;
              emojiIndex = 479;
              break;
            case 57265:
              num1 = offset;
              emojiIndex = 480;
              break;
            case 57266:
              num1 = offset;
              emojiIndex = 481;
              break;
            case 57267:
              num1 = offset;
              emojiIndex = 482;
              break;
            case 57268:
              num1 = offset;
              emojiIndex = 483;
              break;
            case 57269:
              num1 = offset;
              emojiIndex = 484;
              break;
            case 57270:
              num1 = offset;
              emojiIndex = 485;
              break;
            case 57271:
              num1 = offset;
              emojiIndex = 486;
              break;
            case 57272:
              num1 = offset;
              emojiIndex = 487;
              break;
            case 57273:
              num1 = offset;
              emojiIndex = 488;
              break;
            case 57274:
              num1 = offset;
              emojiIndex = 489;
              break;
            case 57275:
              num1 = offset;
              emojiIndex = 490;
              break;
            case 57276:
              num1 = offset;
              emojiIndex = 491;
              break;
            case 57277:
              num1 = offset;
              emojiIndex = 492;
              break;
            case 57278:
              num1 = offset;
              emojiIndex = 493;
              break;
            case 57279:
              num1 = offset;
              emojiIndex = 494;
              break;
            case 57280:
              num1 = offset;
              emojiIndex = 495;
              break;
            case 57281:
              num1 = offset;
              emojiIndex = 496;
              break;
            case 57282:
              num1 = offset;
              emojiIndex = 497;
              break;
            case 57283:
              num1 = offset;
              emojiIndex = 509;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 508;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 509;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 499;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 498;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 499;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 501;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 500;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 501;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 503;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 502;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 503;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 505;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 504;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 505;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 507;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 506;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 507;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57284:
              num1 = offset;
              emojiIndex = 521;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 520;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 521;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 511;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 510;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 511;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 513;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 512;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 513;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 515;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 514;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 515;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 517;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 516;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 517;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 519;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 518;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 519;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57285:
              num1 = offset;
              emojiIndex = 522;
              break;
            case 57286:
              num1 = offset;
              emojiIndex = 523;
              break;
            case 57287:
              num1 = offset;
              emojiIndex = 529;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 524;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 525;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 526;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 527;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 528;
                    break;
                }
              }
              else
                break;
              break;
            case 57288:
              num1 = offset;
              emojiIndex = 530;
              break;
            case 57289:
              num1 = offset;
              emojiIndex = 531;
              break;
            case 57290:
              num1 = offset;
              emojiIndex = 543;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 542;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 543;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 533;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 532;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 533;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 535;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 534;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 535;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 537;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 536;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 537;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 539;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 538;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 539;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 541;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 540;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 541;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57291:
              num1 = offset;
              emojiIndex = 555;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 554;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 555;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 545;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 544;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 545;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 547;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 546;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 547;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 549;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 548;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 549;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 551;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 550;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 551;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 553;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 552;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 553;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57292:
              num1 = offset;
              emojiIndex = 567;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 566;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 567;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 557;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 556;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 557;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 559;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 558;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 559;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 561;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 560;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 561;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 563;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 562;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 563;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 565;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 564;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 565;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57293:
              num1 = offset;
              emojiIndex = 568;
              break;
            case 57294:
              num1 = offset;
              emojiIndex = 569;
              break;
            case 57295:
              num1 = offset;
              emojiIndex = 570;
              break;
            case 57296:
              num1 = offset;
              emojiIndex = 571;
              break;
            case 57297:
              num1 = offset;
              emojiIndex = 572;
              break;
            case 57298:
              num1 = offset;
              emojiIndex = 573;
              break;
            case 57299:
              num1 = offset;
              emojiIndex = 574;
              break;
            case 57300:
              num1 = offset;
              emojiIndex = 575;
              break;
            case 57301:
              num1 = offset;
              emojiIndex = 576;
              break;
            case 57302:
              num1 = offset;
              emojiIndex = 577;
              break;
            case 57303:
              num1 = offset;
              emojiIndex = 578;
              break;
            case 57304:
              num1 = offset;
              emojiIndex = 579;
              break;
            case 57305:
              num1 = offset;
              emojiIndex = 580;
              break;
            case 57306:
              num1 = offset;
              emojiIndex = 581;
              break;
            case 57307:
              num1 = offset;
              emojiIndex = 582;
              break;
            case 57308:
              num1 = offset;
              emojiIndex = 583;
              break;
            case 57309:
              num1 = offset;
              emojiIndex = 584;
              break;
            case 57310:
              num1 = offset;
              emojiIndex = 585;
              break;
            case 57311:
              num1 = offset;
              emojiIndex = 586;
              break;
            case 57312:
              num1 = offset;
              emojiIndex = 587;
              break;
            case 57313:
              num1 = offset;
              emojiIndex = 588;
              break;
            case 57314:
              num1 = offset;
              emojiIndex = 589;
              break;
            case 57315:
              num1 = offset;
              emojiIndex = 590;
              break;
            case 57316:
              num1 = offset;
              emojiIndex = 591;
              break;
            case 57317:
              num1 = offset;
              emojiIndex = 592;
              break;
            case 57318:
              num1 = offset;
              emojiIndex = 593;
              break;
            case 57319:
              num1 = offset;
              emojiIndex = 594;
              break;
            case 57320:
              num1 = offset;
              emojiIndex = 595;
              break;
            case 57321:
              num1 = offset;
              emojiIndex = 596;
              break;
            case 57322:
              num1 = offset;
              emojiIndex = 597;
              break;
            case 57323:
              num1 = offset;
              emojiIndex = 598;
              break;
            case 57324:
              num1 = offset;
              emojiIndex = 599;
              break;
            case 57325:
              num1 = offset;
              emojiIndex = 600;
              break;
            case 57326:
              num1 = offset;
              emojiIndex = 601;
              break;
            case 57327:
              num1 = offset;
              emojiIndex = 602;
              break;
            case 57328:
              num1 = offset;
              emojiIndex = 603;
              break;
            case 57331:
              num1 = offset;
              emojiIndex = 605;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 57096)
              {
                num1 = offset;
                emojiIndex = 604;
                break;
              }
              break;
            case 57332:
              num1 = offset;
              emojiIndex = 610;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 56423:
                    if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56418 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128)
                    {
                      switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                      {
                        case 56421:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56430 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56423 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56447)
                          {
                            num1 = offset;
                            emojiIndex = 606;
                            break;
                          }
                          break;
                        case 56435:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56419 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56436 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56447)
                          {
                            num1 = offset;
                            emojiIndex = 607;
                            break;
                          }
                          break;
                        case 56439:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56428 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56435 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56447)
                          {
                            num1 = offset;
                            emojiIndex = 608;
                            break;
                          }
                          break;
                      }
                    }
                    else
                      break;
                    break;
                  case 56437:
                    if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56435 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56436 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56440 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56128 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56447)
                    {
                      num1 = offset;
                      emojiIndex = 609;
                      break;
                    }
                    break;
                }
              }
              else
                break;
              break;
            case 57333:
              num1 = offset;
              emojiIndex = 611;
              break;
            case 57335:
              num1 = offset;
              emojiIndex = 612;
              break;
            case 57336:
              num1 = offset;
              emojiIndex = 613;
              break;
            case 57337:
              num1 = offset;
              emojiIndex = 614;
              break;
            case 57338:
              num1 = offset;
              emojiIndex = 615;
              break;
            case 57339:
              num1 = offset;
              emojiIndex = 616;
              break;
            case 57340:
              num1 = offset;
              emojiIndex = 617;
              break;
            case 57341:
              num1 = offset;
              emojiIndex = 618;
              break;
            case 57342:
              num1 = offset;
              emojiIndex = 619;
              break;
            case 57343:
              num1 = offset;
              emojiIndex = 620;
              break;
          }
          break;
        case 55357:
          switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
          {
            case 56320:
              num1 = offset;
              emojiIndex = 621;
              break;
            case 56321:
              num1 = offset;
              emojiIndex = 622;
              break;
            case 56322:
              num1 = offset;
              emojiIndex = 623;
              break;
            case 56323:
              num1 = offset;
              emojiIndex = 624;
              break;
            case 56324:
              num1 = offset;
              emojiIndex = 625;
              break;
            case 56325:
              num1 = offset;
              emojiIndex = 626;
              break;
            case 56326:
              num1 = offset;
              emojiIndex = 627;
              break;
            case 56327:
              num1 = offset;
              emojiIndex = 628;
              break;
            case 56328:
              num1 = offset;
              emojiIndex = 629;
              break;
            case 56329:
              num1 = offset;
              emojiIndex = 630;
              break;
            case 56330:
              num1 = offset;
              emojiIndex = 631;
              break;
            case 56331:
              num1 = offset;
              emojiIndex = 632;
              break;
            case 56332:
              num1 = offset;
              emojiIndex = 633;
              break;
            case 56333:
              num1 = offset;
              emojiIndex = 634;
              break;
            case 56334:
              num1 = offset;
              emojiIndex = 635;
              break;
            case 56335:
              num1 = offset;
              emojiIndex = 636;
              break;
            case 56336:
              num1 = offset;
              emojiIndex = 637;
              break;
            case 56337:
              num1 = offset;
              emojiIndex = 638;
              break;
            case 56338:
              num1 = offset;
              emojiIndex = 639;
              break;
            case 56339:
              num1 = offset;
              emojiIndex = 640;
              break;
            case 56340:
              num1 = offset;
              emojiIndex = 641;
              break;
            case 56341:
              num1 = offset;
              emojiIndex = 642;
              break;
            case 56342:
              num1 = offset;
              emojiIndex = 643;
              break;
            case 56343:
              num1 = offset;
              emojiIndex = 644;
              break;
            case 56344:
              num1 = offset;
              emojiIndex = 645;
              break;
            case 56345:
              num1 = offset;
              emojiIndex = 646;
              break;
            case 56346:
              num1 = offset;
              emojiIndex = 647;
              break;
            case 56347:
              num1 = offset;
              emojiIndex = 648;
              break;
            case 56348:
              num1 = offset;
              emojiIndex = 649;
              break;
            case 56349:
              num1 = offset;
              emojiIndex = 650;
              break;
            case 56350:
              num1 = offset;
              emojiIndex = 651;
              break;
            case 56351:
              num1 = offset;
              emojiIndex = 652;
              break;
            case 56352:
              num1 = offset;
              emojiIndex = 653;
              break;
            case 56353:
              num1 = offset;
              emojiIndex = 654;
              break;
            case 56354:
              num1 = offset;
              emojiIndex = 655;
              break;
            case 56355:
              num1 = offset;
              emojiIndex = 656;
              break;
            case 56356:
              num1 = offset;
              emojiIndex = 657;
              break;
            case 56357:
              num1 = offset;
              emojiIndex = 658;
              break;
            case 56358:
              num1 = offset;
              emojiIndex = 659;
              break;
            case 56359:
              num1 = offset;
              emojiIndex = 660;
              break;
            case 56360:
              num1 = offset;
              emojiIndex = 661;
              break;
            case 56361:
              num1 = offset;
              emojiIndex = 662;
              break;
            case 56362:
              num1 = offset;
              emojiIndex = 663;
              break;
            case 56363:
              num1 = offset;
              emojiIndex = 664;
              break;
            case 56364:
              num1 = offset;
              emojiIndex = 665;
              break;
            case 56365:
              num1 = offset;
              emojiIndex = 666;
              break;
            case 56366:
              num1 = offset;
              emojiIndex = 667;
              break;
            case 56367:
              num1 = offset;
              emojiIndex = 668;
              break;
            case 56368:
              num1 = offset;
              emojiIndex = 669;
              break;
            case 56369:
              num1 = offset;
              emojiIndex = 670;
              break;
            case 56370:
              num1 = offset;
              emojiIndex = 671;
              break;
            case 56371:
              num1 = offset;
              emojiIndex = 672;
              break;
            case 56372:
              num1 = offset;
              emojiIndex = 673;
              break;
            case 56373:
              num1 = offset;
              emojiIndex = 674;
              break;
            case 56374:
              num1 = offset;
              emojiIndex = 675;
              break;
            case 56375:
              num1 = offset;
              emojiIndex = 676;
              break;
            case 56376:
              num1 = offset;
              emojiIndex = 677;
              break;
            case 56377:
              num1 = offset;
              emojiIndex = 678;
              break;
            case 56378:
              num1 = offset;
              emojiIndex = 679;
              break;
            case 56379:
              num1 = offset;
              emojiIndex = 680;
              break;
            case 56380:
              num1 = offset;
              emojiIndex = 681;
              break;
            case 56381:
              num1 = offset;
              emojiIndex = 682;
              break;
            case 56382:
              num1 = offset;
              emojiIndex = 683;
              break;
            case 56383:
              num1 = offset;
              emojiIndex = 684;
              break;
            case 56384:
              num1 = offset;
              emojiIndex = 685;
              break;
            case 56385:
              num1 = offset;
              emojiIndex = 687;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56808)
              {
                num1 = offset;
                emojiIndex = 686;
                break;
              }
              break;
            case 56386:
              num1 = offset;
              emojiIndex = 693;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 688;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 689;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 690;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 691;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 692;
                    break;
                }
              }
              else
                break;
              break;
            case 56387:
              num1 = offset;
              emojiIndex = 699;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 694;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 695;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 696;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 697;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 698;
                    break;
                }
              }
              else
                break;
              break;
            case 56388:
              num1 = offset;
              emojiIndex = 700;
              break;
            case 56389:
              num1 = offset;
              emojiIndex = 701;
              break;
            case 56390:
              num1 = offset;
              emojiIndex = 707;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 702;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 703;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 704;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 705;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 706;
                    break;
                }
              }
              else
                break;
              break;
            case 56391:
              num1 = offset;
              emojiIndex = 713;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 708;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 709;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 710;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 711;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 712;
                    break;
                }
              }
              else
                break;
              break;
            case 56392:
              num1 = offset;
              emojiIndex = 719;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 714;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 715;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 716;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 717;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 718;
                    break;
                }
              }
              else
                break;
              break;
            case 56393:
              num1 = offset;
              emojiIndex = 725;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 720;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 721;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 722;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 723;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 724;
                    break;
                }
              }
              else
                break;
              break;
            case 56394:
              num1 = offset;
              emojiIndex = 731;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 726;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 727;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 728;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 729;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 730;
                    break;
                }
              }
              else
                break;
              break;
            case 56395:
              num1 = offset;
              emojiIndex = 737;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 732;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 733;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 734;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 735;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 736;
                    break;
                }
              }
              else
                break;
              break;
            case 56396:
              num1 = offset;
              emojiIndex = 743;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 738;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 739;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 740;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 741;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 742;
                    break;
                }
              }
              else
                break;
              break;
            case 56397:
              num1 = offset;
              emojiIndex = 749;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 744;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 745;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 746;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 747;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 748;
                    break;
                }
              }
              else
                break;
              break;
            case 56398:
              num1 = offset;
              emojiIndex = 755;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 750;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 751;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 752;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 753;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 754;
                    break;
                }
              }
              else
                break;
              break;
            case 56399:
              num1 = offset;
              emojiIndex = 761;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 756;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 757;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 758;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 759;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 760;
                    break;
                }
              }
              else
                break;
              break;
            case 56400:
              num1 = offset;
              emojiIndex = 767;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 762;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 763;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 764;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 765;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 766;
                    break;
                }
              }
              else
                break;
              break;
            case 56401:
              num1 = offset;
              emojiIndex = 768;
              break;
            case 56402:
              num1 = offset;
              emojiIndex = 769;
              break;
            case 56403:
              num1 = offset;
              emojiIndex = 770;
              break;
            case 56404:
              num1 = offset;
              emojiIndex = 771;
              break;
            case 56405:
              num1 = offset;
              emojiIndex = 772;
              break;
            case 56406:
              num1 = offset;
              emojiIndex = 773;
              break;
            case 56407:
              num1 = offset;
              emojiIndex = 774;
              break;
            case 56408:
              num1 = offset;
              emojiIndex = 775;
              break;
            case 56409:
              num1 = offset;
              emojiIndex = 776;
              break;
            case 56410:
              num1 = offset;
              emojiIndex = 777;
              break;
            case 56411:
              num1 = offset;
              emojiIndex = 778;
              break;
            case 56412:
              num1 = offset;
              emojiIndex = 779;
              break;
            case 56413:
              num1 = offset;
              emojiIndex = 780;
              break;
            case 56414:
              num1 = offset;
              emojiIndex = 781;
              break;
            case 56415:
              num1 = offset;
              emojiIndex = 782;
              break;
            case 56416:
              num1 = offset;
              emojiIndex = 783;
              break;
            case 56417:
              num1 = offset;
              emojiIndex = 784;
              break;
            case 56418:
              num1 = offset;
              emojiIndex = 785;
              break;
            case 56419:
              num1 = offset;
              emojiIndex = 786;
              break;
            case 56420:
              num1 = offset;
              emojiIndex = 787;
              break;
            case 56421:
              num1 = offset;
              emojiIndex = 788;
              break;
            case 56422:
              num1 = offset;
              emojiIndex = 794;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 789;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 790;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 791;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 792;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 793;
                    break;
                }
              }
              else
                break;
              break;
            case 56423:
              num1 = offset;
              emojiIndex = 800;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 795;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 796;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 797;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 798;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 799;
                    break;
                }
              }
              else
                break;
              break;
            case 56424:
              num1 = offset;
              emojiIndex = 919;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9877:
                      num1 = offset;
                      emojiIndex = 914;
                      break;
                    case 9878:
                      num1 = offset;
                      emojiIndex = 915;
                      break;
                    case 9992:
                      num1 = offset;
                      emojiIndex = 916;
                      break;
                    case 10084:
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 56424:
                            num1 = offset;
                            emojiIndex = 917;
                            break;
                          case 56459:
                            if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56424)
                            {
                              num1 = offset;
                              emojiIndex = 918;
                              break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 55356:
                      switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                      {
                        case 57150:
                          num1 = offset;
                          emojiIndex = 886;
                          break;
                        case 57203:
                          num1 = offset;
                          emojiIndex = 887;
                          break;
                        case 57235:
                          num1 = offset;
                          emojiIndex = 888;
                          break;
                        case 57252:
                          num1 = offset;
                          emojiIndex = 889;
                          break;
                        case 57256:
                          num1 = offset;
                          emojiIndex = 890;
                          break;
                        case 57323:
                          num1 = offset;
                          emojiIndex = 891;
                          break;
                        case 57325:
                          num1 = offset;
                          emojiIndex = 892;
                          break;
                      }
                      break;
                    case 55357:
                      switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                      {
                        case 56422:
                          num1 = offset;
                          emojiIndex = 894;
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56422)
                          {
                            num1 = offset;
                            emojiIndex = 893;
                            break;
                          }
                          break;
                        case 56423:
                          num1 = offset;
                          emojiIndex = 897;
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                          {
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56422:
                                num1 = offset;
                                emojiIndex = 895;
                                break;
                              case 56423:
                                num1 = offset;
                                emojiIndex = 896;
                                break;
                            }
                          }
                          else
                            break;
                          break;
                        case 56424:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                          {
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56422:
                                num1 = offset;
                                emojiIndex = 899;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56422)
                                {
                                  num1 = offset;
                                  emojiIndex = 898;
                                  break;
                                }
                                break;
                              case 56423:
                                num1 = offset;
                                emojiIndex = 902;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                                {
                                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                                  {
                                    case 56422:
                                      num1 = offset;
                                      emojiIndex = 900;
                                      break;
                                    case 56423:
                                      num1 = offset;
                                      emojiIndex = 901;
                                      break;
                                  }
                                }
                                else
                                  break;
                                break;
                            }
                          }
                          else
                            break;
                          break;
                        case 56425:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                          {
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56422:
                                num1 = offset;
                                emojiIndex = 904;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56422)
                                {
                                  num1 = offset;
                                  emojiIndex = 903;
                                  break;
                                }
                                break;
                              case 56423:
                                num1 = offset;
                                emojiIndex = 907;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                                {
                                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                                  {
                                    case 56422:
                                      num1 = offset;
                                      emojiIndex = 905;
                                      break;
                                    case 56423:
                                      num1 = offset;
                                      emojiIndex = 906;
                                      break;
                                  }
                                }
                                else
                                  break;
                                break;
                            }
                          }
                          else
                            break;
                          break;
                        case 56507:
                          num1 = offset;
                          emojiIndex = 908;
                          break;
                        case 56508:
                          num1 = offset;
                          emojiIndex = 909;
                          break;
                        case 56615:
                          num1 = offset;
                          emojiIndex = 910;
                          break;
                        case 56620:
                          num1 = offset;
                          emojiIndex = 911;
                          break;
                        case 56960:
                          num1 = offset;
                          emojiIndex = 912;
                          break;
                        case 56978:
                          num1 = offset;
                          emojiIndex = 913;
                          break;
                      }
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 817;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 814;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 815;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 816;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 801;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 802;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 803;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 804;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 805;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 806;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 807;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 808;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 809;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 810;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 811;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 812;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 813;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 834;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 831;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 832;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 833;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 818;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 819;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 820;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 821;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 822;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 823;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 824;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 825;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 826;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 827;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 828;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 829;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 830;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 851;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 848;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 849;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 850;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 835;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 836;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 837;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 838;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 839;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 840;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 841;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 842;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 843;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 844;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 845;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 846;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 847;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 868;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 865;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 866;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 867;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 852;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 853;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 854;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 855;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 856;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 857;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 858;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 859;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 860;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 861;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 862;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 863;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 864;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 885;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 882;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 883;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 884;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 869;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 870;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 871;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 872;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 873;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 874;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 875;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 876;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 877;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 878;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 879;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 880;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 881;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56425:
              num1 = offset;
              emojiIndex = 1035;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9877:
                      num1 = offset;
                      emojiIndex = 1028;
                      break;
                    case 9878:
                      num1 = offset;
                      emojiIndex = 1029;
                      break;
                    case 9992:
                      num1 = offset;
                      emojiIndex = 1030;
                      break;
                    case 10084:
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 56424:
                            num1 = offset;
                            emojiIndex = 1031;
                            break;
                          case 56425:
                            num1 = offset;
                            emojiIndex = 1032;
                            break;
                          case 56459:
                            if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                            {
                              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                              {
                                case 56424:
                                  num1 = offset;
                                  emojiIndex = 1033;
                                  break;
                                case 56425:
                                  num1 = offset;
                                  emojiIndex = 1034;
                                  break;
                              }
                            }
                            else
                              break;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 55356:
                      switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                      {
                        case 57150:
                          num1 = offset;
                          emojiIndex = 1005;
                          break;
                        case 57203:
                          num1 = offset;
                          emojiIndex = 1006;
                          break;
                        case 57235:
                          num1 = offset;
                          emojiIndex = 1007;
                          break;
                        case 57252:
                          num1 = offset;
                          emojiIndex = 1008;
                          break;
                        case 57256:
                          num1 = offset;
                          emojiIndex = 1009;
                          break;
                        case 57323:
                          num1 = offset;
                          emojiIndex = 1010;
                          break;
                        case 57325:
                          num1 = offset;
                          emojiIndex = 1011;
                          break;
                      }
                      break;
                    case 55357:
                      switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                      {
                        case 56422:
                          num1 = offset;
                          emojiIndex = 1013;
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56422)
                          {
                            num1 = offset;
                            emojiIndex = 1012;
                            break;
                          }
                          break;
                        case 56423:
                          num1 = offset;
                          emojiIndex = 1016;
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                          {
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56422:
                                num1 = offset;
                                emojiIndex = 1014;
                                break;
                              case 56423:
                                num1 = offset;
                                emojiIndex = 1015;
                                break;
                            }
                          }
                          else
                            break;
                          break;
                        case 56425:
                          if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                          {
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56422:
                                num1 = offset;
                                emojiIndex = 1018;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 56422)
                                {
                                  num1 = offset;
                                  emojiIndex = 1017;
                                  break;
                                }
                                break;
                              case 56423:
                                num1 = offset;
                                emojiIndex = 1021;
                                if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205 && Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55357)
                                {
                                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                                  {
                                    case 56422:
                                      num1 = offset;
                                      emojiIndex = 1019;
                                      break;
                                    case 56423:
                                      num1 = offset;
                                      emojiIndex = 1020;
                                      break;
                                  }
                                }
                                else
                                  break;
                                break;
                            }
                          }
                          else
                            break;
                          break;
                        case 56507:
                          num1 = offset;
                          emojiIndex = 1022;
                          break;
                        case 56508:
                          num1 = offset;
                          emojiIndex = 1023;
                          break;
                        case 56615:
                          num1 = offset;
                          emojiIndex = 1024;
                          break;
                        case 56620:
                          num1 = offset;
                          emojiIndex = 1025;
                          break;
                        case 56960:
                          num1 = offset;
                          emojiIndex = 1026;
                          break;
                        case 56978:
                          num1 = offset;
                          emojiIndex = 1027;
                          break;
                      }
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 936;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 933;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 934;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 935;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 920;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 921;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 922;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 923;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 924;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 925;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 926;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 927;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 928;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 929;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 930;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 931;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 932;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 953;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 950;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 951;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 952;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 937;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 938;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 939;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 940;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 941;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 942;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 943;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 944;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 945;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 946;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 947;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 948;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 949;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 970;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 967;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 968;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 969;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 954;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 955;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 956;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 957;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 958;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 959;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 960;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 961;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 962;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 963;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 964;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 965;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 966;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 987;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 984;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 985;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 986;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 971;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 972;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 973;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 974;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 975;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 976;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 977;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 978;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 979;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 980;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 981;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 982;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 983;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1004;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9877:
                            num1 = offset;
                            emojiIndex = 1001;
                            break;
                          case 9878:
                            num1 = offset;
                            emojiIndex = 1002;
                            break;
                          case 9992:
                            num1 = offset;
                            emojiIndex = 1003;
                            break;
                          case 55356:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 57150:
                                num1 = offset;
                                emojiIndex = 988;
                                break;
                              case 57203:
                                num1 = offset;
                                emojiIndex = 989;
                                break;
                              case 57235:
                                num1 = offset;
                                emojiIndex = 990;
                                break;
                              case 57252:
                                num1 = offset;
                                emojiIndex = 991;
                                break;
                              case 57256:
                                num1 = offset;
                                emojiIndex = 992;
                                break;
                              case 57323:
                                num1 = offset;
                                emojiIndex = 993;
                                break;
                              case 57325:
                                num1 = offset;
                                emojiIndex = 994;
                                break;
                            }
                            break;
                          case 55357:
                            switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                            {
                              case 56507:
                                num1 = offset;
                                emojiIndex = 995;
                                break;
                              case 56508:
                                num1 = offset;
                                emojiIndex = 996;
                                break;
                              case 56615:
                                num1 = offset;
                                emojiIndex = 997;
                                break;
                              case 56620:
                                num1 = offset;
                                emojiIndex = 998;
                                break;
                              case 56960:
                                num1 = offset;
                                emojiIndex = 999;
                                break;
                              case 56978:
                                num1 = offset;
                                emojiIndex = 1000;
                                break;
                            }
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56426:
              num1 = offset;
              emojiIndex = 1036;
              break;
            case 56427:
              num1 = offset;
              emojiIndex = 1037;
              break;
            case 56428:
              num1 = offset;
              emojiIndex = 1038;
              break;
            case 56429:
              num1 = offset;
              emojiIndex = 1039;
              break;
            case 56430:
              num1 = offset;
              emojiIndex = 1051;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1050;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1051;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1041;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1040;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1041;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1043;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1042;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1043;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1045;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1044;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1045;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1047;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1046;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1047;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1049;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1048;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1049;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56431:
              num1 = offset;
              emojiIndex = 1062;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1062;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1063;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1052;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1052;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1053;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1054;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1054;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1055;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1056;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1056;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1057;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1058;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1058;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1059;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1060;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1060;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1061;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56432:
              num1 = offset;
              emojiIndex = 1069;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1064;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1065;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1066;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1067;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1068;
                    break;
                }
              }
              else
                break;
              break;
            case 56433:
              num1 = offset;
              emojiIndex = 1081;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1080;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1081;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1071;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1070;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1071;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1073;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1072;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1073;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1075;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1074;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1075;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1077;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1076;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1077;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1079;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1078;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1079;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56434:
              num1 = offset;
              emojiIndex = 1087;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1082;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1083;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1084;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1085;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1086;
                    break;
                }
              }
              else
                break;
              break;
            case 56435:
              num1 = offset;
              emojiIndex = 1099;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1098;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1099;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1089;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1088;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1089;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1091;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1090;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1091;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1093;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1092;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1093;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1095;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1094;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1095;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1097;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1096;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1097;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56436:
              num1 = offset;
              emojiIndex = 1105;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1100;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1101;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1102;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1103;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1104;
                    break;
                }
              }
              else
                break;
              break;
            case 56437:
              num1 = offset;
              emojiIndex = 1111;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1106;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1107;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1108;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1109;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1110;
                    break;
                }
              }
              else
                break;
              break;
            case 56438:
              num1 = offset;
              emojiIndex = 1117;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1112;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1113;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1114;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1115;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1116;
                    break;
                }
              }
              else
                break;
              break;
            case 56439:
              num1 = offset;
              emojiIndex = 1129;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1128;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1129;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1119;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1118;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1119;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1121;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1120;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1121;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1123;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1122;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1123;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1125;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1124;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1125;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1127;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1126;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1127;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56440:
              num1 = offset;
              emojiIndex = 1135;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1130;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1131;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1132;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1133;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1134;
                    break;
                }
              }
              else
                break;
              break;
            case 56441:
              num1 = offset;
              emojiIndex = 1136;
              break;
            case 56442:
              num1 = offset;
              emojiIndex = 1137;
              break;
            case 56443:
              num1 = offset;
              emojiIndex = 1138;
              break;
            case 56444:
              num1 = offset;
              emojiIndex = 1144;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1139;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1140;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1141;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1142;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1143;
                    break;
                }
              }
              else
                break;
              break;
            case 56445:
              num1 = offset;
              emojiIndex = 1145;
              break;
            case 56446:
              num1 = offset;
              emojiIndex = 1146;
              break;
            case 56447:
              num1 = offset;
              emojiIndex = 1147;
              break;
            case 56448:
              num1 = offset;
              emojiIndex = 1148;
              break;
            case 56449:
              num1 = offset;
              emojiIndex = 1159;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1159;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1160;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1149;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1149;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1150;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1151;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1151;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1152;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1153;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1153;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1154;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1155;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1155;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1156;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1157;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1157;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1158;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56450:
              num1 = offset;
              emojiIndex = 1172;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1171;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1172;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1162;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1161;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1162;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1164;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1163;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1164;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1166;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1165;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1166;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1168;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1167;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1168;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1170;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1169;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1170;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56451:
              num1 = offset;
              emojiIndex = 1178;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1173;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1174;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1175;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1176;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1177;
                    break;
                }
              }
              else
                break;
              break;
            case 56452:
              num1 = offset;
              emojiIndex = 1179;
              break;
            case 56453:
              num1 = offset;
              emojiIndex = 1185;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1180;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1181;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1182;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1183;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1184;
                    break;
                }
              }
              else
                break;
              break;
            case 56454:
              num1 = offset;
              emojiIndex = 1196;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1196;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1197;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1186;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1186;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1187;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1188;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1188;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1189;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1190;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1190;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1191;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1192;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1192;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1193;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1194;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1194;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1195;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56455:
              num1 = offset;
              emojiIndex = 1208;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1208;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1209;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1198;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1198;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1199;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1200;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1200;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1201;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1202;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1202;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1203;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1204;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1204;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1205;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1206;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1206;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1207;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56456:
              num1 = offset;
              emojiIndex = 1210;
              break;
            case 56457:
              num1 = offset;
              emojiIndex = 1211;
              break;
            case 56458:
              num1 = offset;
              emojiIndex = 1212;
              break;
            case 56459:
              num1 = offset;
              emojiIndex = 1213;
              break;
            case 56460:
              num1 = offset;
              emojiIndex = 1214;
              break;
            case 56461:
              num1 = offset;
              emojiIndex = 1215;
              break;
            case 56462:
              num1 = offset;
              emojiIndex = 1216;
              break;
            case 56463:
              num1 = offset;
              emojiIndex = 1217;
              break;
            case 56464:
              num1 = offset;
              emojiIndex = 1218;
              break;
            case 56465:
              num1 = offset;
              emojiIndex = 1219;
              break;
            case 56466:
              num1 = offset;
              emojiIndex = 1220;
              break;
            case 56467:
              num1 = offset;
              emojiIndex = 1221;
              break;
            case 56468:
              num1 = offset;
              emojiIndex = 1222;
              break;
            case 56469:
              num1 = offset;
              emojiIndex = 1223;
              break;
            case 56470:
              num1 = offset;
              emojiIndex = 1224;
              break;
            case 56471:
              num1 = offset;
              emojiIndex = 1225;
              break;
            case 56472:
              num1 = offset;
              emojiIndex = 1226;
              break;
            case 56473:
              num1 = offset;
              emojiIndex = 1227;
              break;
            case 56474:
              num1 = offset;
              emojiIndex = 1228;
              break;
            case 56475:
              num1 = offset;
              emojiIndex = 1229;
              break;
            case 56476:
              num1 = offset;
              emojiIndex = 1230;
              break;
            case 56477:
              num1 = offset;
              emojiIndex = 1231;
              break;
            case 56478:
              num1 = offset;
              emojiIndex = 1232;
              break;
            case 56479:
              num1 = offset;
              emojiIndex = 1233;
              break;
            case 56480:
              num1 = offset;
              emojiIndex = 1234;
              break;
            case 56481:
              num1 = offset;
              emojiIndex = 1235;
              break;
            case 56482:
              num1 = offset;
              emojiIndex = 1236;
              break;
            case 56483:
              num1 = offset;
              emojiIndex = 1237;
              break;
            case 56484:
              num1 = offset;
              emojiIndex = 1238;
              break;
            case 56485:
              num1 = offset;
              emojiIndex = 1239;
              break;
            case 56486:
              num1 = offset;
              emojiIndex = 1240;
              break;
            case 56487:
              num1 = offset;
              emojiIndex = 1241;
              break;
            case 56488:
              num1 = offset;
              emojiIndex = 1242;
              break;
            case 56489:
              num1 = offset;
              emojiIndex = 1243;
              break;
            case 56490:
              num1 = offset;
              emojiIndex = 1249;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1244;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1245;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1246;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1247;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1248;
                    break;
                }
              }
              else
                break;
              break;
            case 56491:
              num1 = offset;
              emojiIndex = 1250;
              break;
            case 56492:
              num1 = offset;
              emojiIndex = 1251;
              break;
            case 56493:
              num1 = offset;
              emojiIndex = 1252;
              break;
            case 56494:
              num1 = offset;
              emojiIndex = 1253;
              break;
            case 56495:
              num1 = offset;
              emojiIndex = 1254;
              break;
            case 56496:
              num1 = offset;
              emojiIndex = 1255;
              break;
            case 56497:
              num1 = offset;
              emojiIndex = 1256;
              break;
            case 56498:
              num1 = offset;
              emojiIndex = 1257;
              break;
            case 56499:
              num1 = offset;
              emojiIndex = 1258;
              break;
            case 56500:
              num1 = offset;
              emojiIndex = 1259;
              break;
            case 56501:
              num1 = offset;
              emojiIndex = 1260;
              break;
            case 56502:
              num1 = offset;
              emojiIndex = 1261;
              break;
            case 56503:
              num1 = offset;
              emojiIndex = 1262;
              break;
            case 56504:
              num1 = offset;
              emojiIndex = 1263;
              break;
            case 56505:
              num1 = offset;
              emojiIndex = 1264;
              break;
            case 56506:
              num1 = offset;
              emojiIndex = 1265;
              break;
            case 56507:
              num1 = offset;
              emojiIndex = 1266;
              break;
            case 56508:
              num1 = offset;
              emojiIndex = 1267;
              break;
            case 56509:
              num1 = offset;
              emojiIndex = 1268;
              break;
            case 56510:
              num1 = offset;
              emojiIndex = 1269;
              break;
            case 56511:
              num1 = offset;
              emojiIndex = 1270;
              break;
            case 56512:
              num1 = offset;
              emojiIndex = 1271;
              break;
            case 56513:
              num1 = offset;
              emojiIndex = 1272;
              break;
            case 56514:
              num1 = offset;
              emojiIndex = 1273;
              break;
            case 56515:
              num1 = offset;
              emojiIndex = 1274;
              break;
            case 56516:
              num1 = offset;
              emojiIndex = 1275;
              break;
            case 56517:
              num1 = offset;
              emojiIndex = 1276;
              break;
            case 56518:
              num1 = offset;
              emojiIndex = 1277;
              break;
            case 56519:
              num1 = offset;
              emojiIndex = 1278;
              break;
            case 56520:
              num1 = offset;
              emojiIndex = 1279;
              break;
            case 56521:
              num1 = offset;
              emojiIndex = 1280;
              break;
            case 56522:
              num1 = offset;
              emojiIndex = 1281;
              break;
            case 56523:
              num1 = offset;
              emojiIndex = 1282;
              break;
            case 56524:
              num1 = offset;
              emojiIndex = 1283;
              break;
            case 56525:
              num1 = offset;
              emojiIndex = 1284;
              break;
            case 56526:
              num1 = offset;
              emojiIndex = 1285;
              break;
            case 56527:
              num1 = offset;
              emojiIndex = 1286;
              break;
            case 56528:
              num1 = offset;
              emojiIndex = 1287;
              break;
            case 56529:
              num1 = offset;
              emojiIndex = 1288;
              break;
            case 56530:
              num1 = offset;
              emojiIndex = 1289;
              break;
            case 56531:
              num1 = offset;
              emojiIndex = 1290;
              break;
            case 56532:
              num1 = offset;
              emojiIndex = 1291;
              break;
            case 56533:
              num1 = offset;
              emojiIndex = 1292;
              break;
            case 56534:
              num1 = offset;
              emojiIndex = 1293;
              break;
            case 56535:
              num1 = offset;
              emojiIndex = 1294;
              break;
            case 56536:
              num1 = offset;
              emojiIndex = 1295;
              break;
            case 56537:
              num1 = offset;
              emojiIndex = 1296;
              break;
            case 56538:
              num1 = offset;
              emojiIndex = 1297;
              break;
            case 56539:
              num1 = offset;
              emojiIndex = 1298;
              break;
            case 56540:
              num1 = offset;
              emojiIndex = 1299;
              break;
            case 56541:
              num1 = offset;
              emojiIndex = 1300;
              break;
            case 56542:
              num1 = offset;
              emojiIndex = 1301;
              break;
            case 56543:
              num1 = offset;
              emojiIndex = 1302;
              break;
            case 56544:
              num1 = offset;
              emojiIndex = 1303;
              break;
            case 56545:
              num1 = offset;
              emojiIndex = 1304;
              break;
            case 56546:
              num1 = offset;
              emojiIndex = 1305;
              break;
            case 56547:
              num1 = offset;
              emojiIndex = 1306;
              break;
            case 56548:
              num1 = offset;
              emojiIndex = 1307;
              break;
            case 56549:
              num1 = offset;
              emojiIndex = 1308;
              break;
            case 56550:
              num1 = offset;
              emojiIndex = 1309;
              break;
            case 56551:
              num1 = offset;
              emojiIndex = 1310;
              break;
            case 56552:
              num1 = offset;
              emojiIndex = 1311;
              break;
            case 56553:
              num1 = offset;
              emojiIndex = 1312;
              break;
            case 56554:
              num1 = offset;
              emojiIndex = 1313;
              break;
            case 56555:
              num1 = offset;
              emojiIndex = 1314;
              break;
            case 56556:
              num1 = offset;
              emojiIndex = 1315;
              break;
            case 56557:
              num1 = offset;
              emojiIndex = 1316;
              break;
            case 56558:
              num1 = offset;
              emojiIndex = 1317;
              break;
            case 56559:
              num1 = offset;
              emojiIndex = 1318;
              break;
            case 56560:
              num1 = offset;
              emojiIndex = 1319;
              break;
            case 56561:
              num1 = offset;
              emojiIndex = 1320;
              break;
            case 56562:
              num1 = offset;
              emojiIndex = 1321;
              break;
            case 56563:
              num1 = offset;
              emojiIndex = 1322;
              break;
            case 56564:
              num1 = offset;
              emojiIndex = 1323;
              break;
            case 56565:
              num1 = offset;
              emojiIndex = 1324;
              break;
            case 56566:
              num1 = offset;
              emojiIndex = 1325;
              break;
            case 56567:
              num1 = offset;
              emojiIndex = 1326;
              break;
            case 56568:
              num1 = offset;
              emojiIndex = 1327;
              break;
            case 56569:
              num1 = offset;
              emojiIndex = 1328;
              break;
            case 56570:
              num1 = offset;
              emojiIndex = 1329;
              break;
            case 56571:
              num1 = offset;
              emojiIndex = 1330;
              break;
            case 56572:
              num1 = offset;
              emojiIndex = 1331;
              break;
            case 56573:
              num1 = offset;
              emojiIndex = 1332;
              break;
            case 56575:
              num1 = offset;
              emojiIndex = 1333;
              break;
            case 56576:
              num1 = offset;
              emojiIndex = 1334;
              break;
            case 56577:
              num1 = offset;
              emojiIndex = 1335;
              break;
            case 56578:
              num1 = offset;
              emojiIndex = 1336;
              break;
            case 56579:
              num1 = offset;
              emojiIndex = 1337;
              break;
            case 56580:
              num1 = offset;
              emojiIndex = 1338;
              break;
            case 56581:
              num1 = offset;
              emojiIndex = 1339;
              break;
            case 56582:
              num1 = offset;
              emojiIndex = 1340;
              break;
            case 56583:
              num1 = offset;
              emojiIndex = 1341;
              break;
            case 56584:
              num1 = offset;
              emojiIndex = 1342;
              break;
            case 56585:
              num1 = offset;
              emojiIndex = 1343;
              break;
            case 56586:
              num1 = offset;
              emojiIndex = 1344;
              break;
            case 56587:
              num1 = offset;
              emojiIndex = 1345;
              break;
            case 56588:
              num1 = offset;
              emojiIndex = 1346;
              break;
            case 56589:
              num1 = offset;
              emojiIndex = 1347;
              break;
            case 56590:
              num1 = offset;
              emojiIndex = 1348;
              break;
            case 56591:
              num1 = offset;
              emojiIndex = 1349;
              break;
            case 56592:
              num1 = offset;
              emojiIndex = 1350;
              break;
            case 56593:
              num1 = offset;
              emojiIndex = 1351;
              break;
            case 56594:
              num1 = offset;
              emojiIndex = 1352;
              break;
            case 56595:
              num1 = offset;
              emojiIndex = 1353;
              break;
            case 56596:
              num1 = offset;
              emojiIndex = 1354;
              break;
            case 56597:
              num1 = offset;
              emojiIndex = 1355;
              break;
            case 56598:
              num1 = offset;
              emojiIndex = 1356;
              break;
            case 56599:
              num1 = offset;
              emojiIndex = 1357;
              break;
            case 56600:
              num1 = offset;
              emojiIndex = 1358;
              break;
            case 56601:
              num1 = offset;
              emojiIndex = 1359;
              break;
            case 56602:
              num1 = offset;
              emojiIndex = 1360;
              break;
            case 56603:
              num1 = offset;
              emojiIndex = 1361;
              break;
            case 56604:
              num1 = offset;
              emojiIndex = 1362;
              break;
            case 56605:
              num1 = offset;
              emojiIndex = 1363;
              break;
            case 56606:
              num1 = offset;
              emojiIndex = 1364;
              break;
            case 56607:
              num1 = offset;
              emojiIndex = 1365;
              break;
            case 56608:
              num1 = offset;
              emojiIndex = 1366;
              break;
            case 56609:
              num1 = offset;
              emojiIndex = 1367;
              break;
            case 56610:
              num1 = offset;
              emojiIndex = 1368;
              break;
            case 56611:
              num1 = offset;
              emojiIndex = 1369;
              break;
            case 56612:
              num1 = offset;
              emojiIndex = 1370;
              break;
            case 56613:
              num1 = offset;
              emojiIndex = 1371;
              break;
            case 56614:
              num1 = offset;
              emojiIndex = 1372;
              break;
            case 56615:
              num1 = offset;
              emojiIndex = 1373;
              break;
            case 56616:
              num1 = offset;
              emojiIndex = 1374;
              break;
            case 56617:
              num1 = offset;
              emojiIndex = 1375;
              break;
            case 56618:
              num1 = offset;
              emojiIndex = 1376;
              break;
            case 56619:
              num1 = offset;
              emojiIndex = 1377;
              break;
            case 56620:
              num1 = offset;
              emojiIndex = 1378;
              break;
            case 56621:
              num1 = offset;
              emojiIndex = 1379;
              break;
            case 56622:
              num1 = offset;
              emojiIndex = 1380;
              break;
            case 56623:
              num1 = offset;
              emojiIndex = 1381;
              break;
            case 56624:
              num1 = offset;
              emojiIndex = 1382;
              break;
            case 56625:
              num1 = offset;
              emojiIndex = 1383;
              break;
            case 56626:
              num1 = offset;
              emojiIndex = 1384;
              break;
            case 56627:
              num1 = offset;
              emojiIndex = 1385;
              break;
            case 56628:
              num1 = offset;
              emojiIndex = 1386;
              break;
            case 56629:
              num1 = offset;
              emojiIndex = 1387;
              break;
            case 56630:
              num1 = offset;
              emojiIndex = 1388;
              break;
            case 56631:
              num1 = offset;
              emojiIndex = 1389;
              break;
            case 56632:
              num1 = offset;
              emojiIndex = 1390;
              break;
            case 56633:
              num1 = offset;
              emojiIndex = 1391;
              break;
            case 56634:
              num1 = offset;
              emojiIndex = 1392;
              break;
            case 56635:
              num1 = offset;
              emojiIndex = 1393;
              break;
            case 56636:
              num1 = offset;
              emojiIndex = 1394;
              break;
            case 56637:
              num1 = offset;
              emojiIndex = 1395;
              break;
            case 56649:
              num1 = offset;
              emojiIndex = 1396;
              break;
            case 56650:
              num1 = offset;
              emojiIndex = 1397;
              break;
            case 56651:
              num1 = offset;
              emojiIndex = 1398;
              break;
            case 56652:
              num1 = offset;
              emojiIndex = 1399;
              break;
            case 56653:
              num1 = offset;
              emojiIndex = 1400;
              break;
            case 56654:
              num1 = offset;
              emojiIndex = 1401;
              break;
            case 56656:
              num1 = offset;
              emojiIndex = 1402;
              break;
            case 56657:
              num1 = offset;
              emojiIndex = 1403;
              break;
            case 56658:
              num1 = offset;
              emojiIndex = 1404;
              break;
            case 56659:
              num1 = offset;
              emojiIndex = 1405;
              break;
            case 56660:
              num1 = offset;
              emojiIndex = 1406;
              break;
            case 56661:
              num1 = offset;
              emojiIndex = 1407;
              break;
            case 56662:
              num1 = offset;
              emojiIndex = 1408;
              break;
            case 56663:
              num1 = offset;
              emojiIndex = 1409;
              break;
            case 56664:
              num1 = offset;
              emojiIndex = 1410;
              break;
            case 56665:
              num1 = offset;
              emojiIndex = 1411;
              break;
            case 56666:
              num1 = offset;
              emojiIndex = 1412;
              break;
            case 56667:
              num1 = offset;
              emojiIndex = 1413;
              break;
            case 56668:
              num1 = offset;
              emojiIndex = 1414;
              break;
            case 56669:
              num1 = offset;
              emojiIndex = 1415;
              break;
            case 56670:
              num1 = offset;
              emojiIndex = 1416;
              break;
            case 56671:
              num1 = offset;
              emojiIndex = 1417;
              break;
            case 56672:
              num1 = offset;
              emojiIndex = 1418;
              break;
            case 56673:
              num1 = offset;
              emojiIndex = 1419;
              break;
            case 56674:
              num1 = offset;
              emojiIndex = 1420;
              break;
            case 56675:
              num1 = offset;
              emojiIndex = 1421;
              break;
            case 56676:
              num1 = offset;
              emojiIndex = 1422;
              break;
            case 56677:
              num1 = offset;
              emojiIndex = 1423;
              break;
            case 56678:
              num1 = offset;
              emojiIndex = 1424;
              break;
            case 56679:
              num1 = offset;
              emojiIndex = 1425;
              break;
            case 56687:
              num1 = offset;
              emojiIndex = 1426;
              break;
            case 56688:
              num1 = offset;
              emojiIndex = 1427;
              break;
            case 56691:
              num1 = offset;
              emojiIndex = 1428;
              break;
            case 56692:
              num1 = offset;
              emojiIndex = 1434;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1429;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1430;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1431;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1432;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1433;
                    break;
                }
              }
              else
                break;
              break;
            case 56693:
              num1 = offset;
              emojiIndex = 1446;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1445;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1446;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1436;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1435;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1436;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1438;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1437;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1438;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1440;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1439;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1440;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1442;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1441;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1442;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1444;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1443;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1444;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56694:
              num1 = offset;
              emojiIndex = 1447;
              break;
            case 56695:
              num1 = offset;
              emojiIndex = 1448;
              break;
            case 56696:
              num1 = offset;
              emojiIndex = 1449;
              break;
            case 56697:
              num1 = offset;
              emojiIndex = 1450;
              break;
            case 56698:
              num1 = offset;
              emojiIndex = 1456;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1451;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1452;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1453;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1454;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1455;
                    break;
                }
              }
              else
                break;
              break;
            case 56711:
              num1 = offset;
              emojiIndex = 1457;
              break;
            case 56714:
              num1 = offset;
              emojiIndex = 1458;
              break;
            case 56715:
              num1 = offset;
              emojiIndex = 1459;
              break;
            case 56716:
              num1 = offset;
              emojiIndex = 1460;
              break;
            case 56717:
              num1 = offset;
              emojiIndex = 1461;
              break;
            case 56720:
              num1 = offset;
              emojiIndex = 1467;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1462;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1463;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1464;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1465;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1466;
                    break;
                }
              }
              else
                break;
              break;
            case 56725:
              num1 = offset;
              emojiIndex = 1473;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1468;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1469;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1470;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1471;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1472;
                    break;
                }
              }
              else
                break;
              break;
            case 56726:
              num1 = offset;
              emojiIndex = 1479;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1474;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1475;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1476;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1477;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1478;
                    break;
                }
              }
              else
                break;
              break;
            case 56740:
              num1 = offset;
              emojiIndex = 1480;
              break;
            case 56741:
              num1 = offset;
              emojiIndex = 1481;
              break;
            case 56744:
              num1 = offset;
              emojiIndex = 1482;
              break;
            case 56753:
              num1 = offset;
              emojiIndex = 1483;
              break;
            case 56754:
              num1 = offset;
              emojiIndex = 1484;
              break;
            case 56764:
              num1 = offset;
              emojiIndex = 1485;
              break;
            case 56770:
              num1 = offset;
              emojiIndex = 1486;
              break;
            case 56771:
              num1 = offset;
              emojiIndex = 1487;
              break;
            case 56772:
              num1 = offset;
              emojiIndex = 1488;
              break;
            case 56785:
              num1 = offset;
              emojiIndex = 1489;
              break;
            case 56786:
              num1 = offset;
              emojiIndex = 1490;
              break;
            case 56787:
              num1 = offset;
              emojiIndex = 1491;
              break;
            case 56796:
              num1 = offset;
              emojiIndex = 1492;
              break;
            case 56797:
              num1 = offset;
              emojiIndex = 1493;
              break;
            case 56798:
              num1 = offset;
              emojiIndex = 1494;
              break;
            case 56801:
              num1 = offset;
              emojiIndex = 1495;
              break;
            case 56803:
              num1 = offset;
              emojiIndex = 1496;
              break;
            case 56808:
              num1 = offset;
              emojiIndex = 1497;
              break;
            case 56815:
              num1 = offset;
              emojiIndex = 1498;
              break;
            case 56819:
              num1 = offset;
              emojiIndex = 1499;
              break;
            case 56826:
              num1 = offset;
              emojiIndex = 1500;
              break;
            case 56827:
              num1 = offset;
              emojiIndex = 1501;
              break;
            case 56828:
              num1 = offset;
              emojiIndex = 1502;
              break;
            case 56829:
              num1 = offset;
              emojiIndex = 1503;
              break;
            case 56830:
              num1 = offset;
              emojiIndex = 1504;
              break;
            case 56831:
              num1 = offset;
              emojiIndex = 1505;
              break;
            case 56832:
              num1 = offset;
              emojiIndex = 1506;
              break;
            case 56833:
              num1 = offset;
              emojiIndex = 1507;
              break;
            case 56834:
              num1 = offset;
              emojiIndex = 1508;
              break;
            case 56835:
              num1 = offset;
              emojiIndex = 1509;
              break;
            case 56836:
              num1 = offset;
              emojiIndex = 1510;
              break;
            case 56837:
              num1 = offset;
              emojiIndex = 1511;
              break;
            case 56838:
              num1 = offset;
              emojiIndex = 1512;
              break;
            case 56839:
              num1 = offset;
              emojiIndex = 1513;
              break;
            case 56840:
              num1 = offset;
              emojiIndex = 1514;
              break;
            case 56841:
              num1 = offset;
              emojiIndex = 1515;
              break;
            case 56842:
              num1 = offset;
              emojiIndex = 1516;
              break;
            case 56843:
              num1 = offset;
              emojiIndex = 1517;
              break;
            case 56844:
              num1 = offset;
              emojiIndex = 1518;
              break;
            case 56845:
              num1 = offset;
              emojiIndex = 1519;
              break;
            case 56846:
              num1 = offset;
              emojiIndex = 1520;
              break;
            case 56847:
              num1 = offset;
              emojiIndex = 1521;
              break;
            case 56848:
              num1 = offset;
              emojiIndex = 1522;
              break;
            case 56849:
              num1 = offset;
              emojiIndex = 1523;
              break;
            case 56850:
              num1 = offset;
              emojiIndex = 1524;
              break;
            case 56851:
              num1 = offset;
              emojiIndex = 1525;
              break;
            case 56852:
              num1 = offset;
              emojiIndex = 1526;
              break;
            case 56853:
              num1 = offset;
              emojiIndex = 1527;
              break;
            case 56854:
              num1 = offset;
              emojiIndex = 1528;
              break;
            case 56855:
              num1 = offset;
              emojiIndex = 1529;
              break;
            case 56856:
              num1 = offset;
              emojiIndex = 1530;
              break;
            case 56857:
              num1 = offset;
              emojiIndex = 1531;
              break;
            case 56858:
              num1 = offset;
              emojiIndex = 1532;
              break;
            case 56859:
              num1 = offset;
              emojiIndex = 1533;
              break;
            case 56860:
              num1 = offset;
              emojiIndex = 1534;
              break;
            case 56861:
              num1 = offset;
              emojiIndex = 1535;
              break;
            case 56862:
              num1 = offset;
              emojiIndex = 1536;
              break;
            case 56863:
              num1 = offset;
              emojiIndex = 1537;
              break;
            case 56864:
              num1 = offset;
              emojiIndex = 1538;
              break;
            case 56865:
              num1 = offset;
              emojiIndex = 1539;
              break;
            case 56866:
              num1 = offset;
              emojiIndex = 1540;
              break;
            case 56867:
              num1 = offset;
              emojiIndex = 1541;
              break;
            case 56868:
              num1 = offset;
              emojiIndex = 1542;
              break;
            case 56869:
              num1 = offset;
              emojiIndex = 1543;
              break;
            case 56870:
              num1 = offset;
              emojiIndex = 1544;
              break;
            case 56871:
              num1 = offset;
              emojiIndex = 1545;
              break;
            case 56872:
              num1 = offset;
              emojiIndex = 1546;
              break;
            case 56873:
              num1 = offset;
              emojiIndex = 1547;
              break;
            case 56874:
              num1 = offset;
              emojiIndex = 1548;
              break;
            case 56875:
              num1 = offset;
              emojiIndex = 1549;
              break;
            case 56876:
              num1 = offset;
              emojiIndex = 1550;
              break;
            case 56877:
              num1 = offset;
              emojiIndex = 1551;
              break;
            case 56878:
              num1 = offset;
              emojiIndex = 1552;
              break;
            case 56879:
              num1 = offset;
              emojiIndex = 1553;
              break;
            case 56880:
              num1 = offset;
              emojiIndex = 1554;
              break;
            case 56881:
              num1 = offset;
              emojiIndex = 1555;
              break;
            case 56882:
              num1 = offset;
              emojiIndex = 1556;
              break;
            case 56883:
              num1 = offset;
              emojiIndex = 1557;
              break;
            case 56884:
              num1 = offset;
              emojiIndex = 1558;
              break;
            case 56885:
              num1 = offset;
              emojiIndex = 1559;
              break;
            case 56886:
              num1 = offset;
              emojiIndex = 1560;
              break;
            case 56887:
              num1 = offset;
              emojiIndex = 1561;
              break;
            case 56888:
              num1 = offset;
              emojiIndex = 1562;
              break;
            case 56889:
              num1 = offset;
              emojiIndex = 1563;
              break;
            case 56890:
              num1 = offset;
              emojiIndex = 1564;
              break;
            case 56891:
              num1 = offset;
              emojiIndex = 1565;
              break;
            case 56892:
              num1 = offset;
              emojiIndex = 1566;
              break;
            case 56893:
              num1 = offset;
              emojiIndex = 1567;
              break;
            case 56894:
              num1 = offset;
              emojiIndex = 1568;
              break;
            case 56895:
              num1 = offset;
              emojiIndex = 1569;
              break;
            case 56896:
              num1 = offset;
              emojiIndex = 1570;
              break;
            case 56897:
              num1 = offset;
              emojiIndex = 1571;
              break;
            case 56898:
              num1 = offset;
              emojiIndex = 1572;
              break;
            case 56899:
              num1 = offset;
              emojiIndex = 1573;
              break;
            case 56900:
              num1 = offset;
              emojiIndex = 1574;
              break;
            case 56901:
              num1 = offset;
              emojiIndex = 1585;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1585;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1586;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1575;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1575;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1576;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1577;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1577;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1578;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1579;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1579;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1580;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1581;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1581;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1582;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1583;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1583;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1584;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56902:
              num1 = offset;
              emojiIndex = 1597;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1597;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1598;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1587;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1587;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1588;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1589;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1589;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1590;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1591;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1591;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1592;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1593;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1593;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1594;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1595;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1595;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1596;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56903:
              num1 = offset;
              emojiIndex = 1610;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1609;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1610;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1600;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1599;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1600;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1602;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1601;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1602;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1604;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1603;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1604;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1606;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1605;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1606;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1608;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1607;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1608;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56904:
              num1 = offset;
              emojiIndex = 1611;
              break;
            case 56905:
              num1 = offset;
              emojiIndex = 1612;
              break;
            case 56906:
              num1 = offset;
              emojiIndex = 1613;
              break;
            case 56907:
              num1 = offset;
              emojiIndex = 1624;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1624;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1625;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1614;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1614;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1615;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1616;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1616;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1617;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1618;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1618;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1619;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1620;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1620;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1621;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1622;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1622;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1623;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56908:
              num1 = offset;
              emojiIndex = 1631;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1626;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1627;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1628;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1629;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1630;
                    break;
                }
              }
              else
                break;
              break;
            case 56909:
              num1 = offset;
              emojiIndex = 1642;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1642;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1643;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1632;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1632;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1633;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1634;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1634;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1635;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1636;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1636;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1637;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1638;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1638;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1639;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1640;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1640;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1641;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56910:
              num1 = offset;
              emojiIndex = 1654;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1654;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1655;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1644;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1644;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1645;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1646;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1646;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1647;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1648;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1648;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1649;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1650;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1650;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1651;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1652;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1652;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1653;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56911:
              num1 = offset;
              emojiIndex = 1661;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1656;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1657;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1658;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1659;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1660;
                    break;
                }
              }
              else
                break;
              break;
            case 56960:
              num1 = offset;
              emojiIndex = 1662;
              break;
            case 56961:
              num1 = offset;
              emojiIndex = 1663;
              break;
            case 56962:
              num1 = offset;
              emojiIndex = 1664;
              break;
            case 56963:
              num1 = offset;
              emojiIndex = 1665;
              break;
            case 56964:
              num1 = offset;
              emojiIndex = 1666;
              break;
            case 56965:
              num1 = offset;
              emojiIndex = 1667;
              break;
            case 56966:
              num1 = offset;
              emojiIndex = 1668;
              break;
            case 56967:
              num1 = offset;
              emojiIndex = 1669;
              break;
            case 56968:
              num1 = offset;
              emojiIndex = 1670;
              break;
            case 56969:
              num1 = offset;
              emojiIndex = 1671;
              break;
            case 56970:
              num1 = offset;
              emojiIndex = 1672;
              break;
            case 56971:
              num1 = offset;
              emojiIndex = 1673;
              break;
            case 56972:
              num1 = offset;
              emojiIndex = 1674;
              break;
            case 56973:
              num1 = offset;
              emojiIndex = 1675;
              break;
            case 56974:
              num1 = offset;
              emojiIndex = 1676;
              break;
            case 56975:
              num1 = offset;
              emojiIndex = 1677;
              break;
            case 56976:
              num1 = offset;
              emojiIndex = 1678;
              break;
            case 56977:
              num1 = offset;
              emojiIndex = 1679;
              break;
            case 56978:
              num1 = offset;
              emojiIndex = 1680;
              break;
            case 56979:
              num1 = offset;
              emojiIndex = 1681;
              break;
            case 56980:
              num1 = offset;
              emojiIndex = 1682;
              break;
            case 56981:
              num1 = offset;
              emojiIndex = 1683;
              break;
            case 56982:
              num1 = offset;
              emojiIndex = 1684;
              break;
            case 56983:
              num1 = offset;
              emojiIndex = 1685;
              break;
            case 56984:
              num1 = offset;
              emojiIndex = 1686;
              break;
            case 56985:
              num1 = offset;
              emojiIndex = 1687;
              break;
            case 56986:
              num1 = offset;
              emojiIndex = 1688;
              break;
            case 56987:
              num1 = offset;
              emojiIndex = 1689;
              break;
            case 56988:
              num1 = offset;
              emojiIndex = 1690;
              break;
            case 56989:
              num1 = offset;
              emojiIndex = 1691;
              break;
            case 56990:
              num1 = offset;
              emojiIndex = 1692;
              break;
            case 56991:
              num1 = offset;
              emojiIndex = 1693;
              break;
            case 56992:
              num1 = offset;
              emojiIndex = 1694;
              break;
            case 56993:
              num1 = offset;
              emojiIndex = 1695;
              break;
            case 56994:
              num1 = offset;
              emojiIndex = 1696;
              break;
            case 56995:
              num1 = offset;
              emojiIndex = 1708;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1707;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1708;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1698;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1697;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1698;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1700;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1699;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1700;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1702;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1701;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1702;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1704;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1703;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1704;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1706;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1705;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1706;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56996:
              num1 = offset;
              emojiIndex = 1709;
              break;
            case 56997:
              num1 = offset;
              emojiIndex = 1710;
              break;
            case 56998:
              num1 = offset;
              emojiIndex = 1711;
              break;
            case 56999:
              num1 = offset;
              emojiIndex = 1712;
              break;
            case 57000:
              num1 = offset;
              emojiIndex = 1713;
              break;
            case 57001:
              num1 = offset;
              emojiIndex = 1714;
              break;
            case 57002:
              num1 = offset;
              emojiIndex = 1715;
              break;
            case 57003:
              num1 = offset;
              emojiIndex = 1716;
              break;
            case 57004:
              num1 = offset;
              emojiIndex = 1717;
              break;
            case 57005:
              num1 = offset;
              emojiIndex = 1718;
              break;
            case 57006:
              num1 = offset;
              emojiIndex = 1719;
              break;
            case 57007:
              num1 = offset;
              emojiIndex = 1720;
              break;
            case 57008:
              num1 = offset;
              emojiIndex = 1721;
              break;
            case 57009:
              num1 = offset;
              emojiIndex = 1722;
              break;
            case 57010:
              num1 = offset;
              emojiIndex = 1723;
              break;
            case 57011:
              num1 = offset;
              emojiIndex = 1724;
              break;
            case 57012:
              num1 = offset;
              emojiIndex = 1736;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1735;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1736;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1726;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1725;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1726;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1728;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1727;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1728;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1730;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1729;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1730;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1732;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1731;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1732;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1734;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1733;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1734;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57013:
              num1 = offset;
              emojiIndex = 1748;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1747;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1748;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1738;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1737;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1738;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1740;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1739;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1740;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1742;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1741;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1742;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1744;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1743;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1744;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1746;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1745;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1746;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57014:
              num1 = offset;
              emojiIndex = 1760;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1759;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1760;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1750;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1749;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1750;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1752;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1751;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1752;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1754;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1753;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1754;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1756;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1755;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1756;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1758;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1757;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1758;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 57015:
              num1 = offset;
              emojiIndex = 1761;
              break;
            case 57016:
              num1 = offset;
              emojiIndex = 1762;
              break;
            case 57017:
              num1 = offset;
              emojiIndex = 1763;
              break;
            case 57018:
              num1 = offset;
              emojiIndex = 1764;
              break;
            case 57019:
              num1 = offset;
              emojiIndex = 1765;
              break;
            case 57020:
              num1 = offset;
              emojiIndex = 1766;
              break;
            case 57021:
              num1 = offset;
              emojiIndex = 1767;
              break;
            case 57022:
              num1 = offset;
              emojiIndex = 1768;
              break;
            case 57023:
              num1 = offset;
              emojiIndex = 1769;
              break;
            case 57024:
              num1 = offset;
              emojiIndex = 1775;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1770;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1771;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1772;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1773;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1774;
                    break;
                }
              }
              else
                break;
              break;
            case 57025:
              num1 = offset;
              emojiIndex = 1776;
              break;
            case 57026:
              num1 = offset;
              emojiIndex = 1777;
              break;
            case 57027:
              num1 = offset;
              emojiIndex = 1778;
              break;
            case 57028:
              num1 = offset;
              emojiIndex = 1779;
              break;
            case 57029:
              num1 = offset;
              emojiIndex = 1780;
              break;
            case 57035:
              num1 = offset;
              emojiIndex = 1781;
              break;
            case 57036:
              num1 = offset;
              emojiIndex = 1787;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1782;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1783;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1784;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1785;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1786;
                    break;
                }
              }
              else
                break;
              break;
            case 57037:
              num1 = offset;
              emojiIndex = 1788;
              break;
            case 57038:
              num1 = offset;
              emojiIndex = 1789;
              break;
            case 57039:
              num1 = offset;
              emojiIndex = 1790;
              break;
            case 57040:
              num1 = offset;
              emojiIndex = 1791;
              break;
            case 57041:
              num1 = offset;
              emojiIndex = 1792;
              break;
            case 57042:
              num1 = offset;
              emojiIndex = 1793;
              break;
            case 57056:
              num1 = offset;
              emojiIndex = 1794;
              break;
            case 57057:
              num1 = offset;
              emojiIndex = 1795;
              break;
            case 57058:
              num1 = offset;
              emojiIndex = 1796;
              break;
            case 57059:
              num1 = offset;
              emojiIndex = 1797;
              break;
            case 57060:
              num1 = offset;
              emojiIndex = 1798;
              break;
            case 57061:
              num1 = offset;
              emojiIndex = 1799;
              break;
            case 57065:
              num1 = offset;
              emojiIndex = 1800;
              break;
            case 57067:
              num1 = offset;
              emojiIndex = 1801;
              break;
            case 57068:
              num1 = offset;
              emojiIndex = 1802;
              break;
            case 57072:
              num1 = offset;
              emojiIndex = 1803;
              break;
            case 57075:
              num1 = offset;
              emojiIndex = 1804;
              break;
            case 57076:
              num1 = offset;
              emojiIndex = 1805;
              break;
            case 57077:
              num1 = offset;
              emojiIndex = 1806;
              break;
            case 57078:
              num1 = offset;
              emojiIndex = 1807;
              break;
            case 57079:
              num1 = offset;
              emojiIndex = 1808;
              break;
            case 57080:
              num1 = offset;
              emojiIndex = 1809;
              break;
          }
          break;
        case 55358:
          switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
          {
            case 56592:
              num1 = offset;
              emojiIndex = 1810;
              break;
            case 56593:
              num1 = offset;
              emojiIndex = 1811;
              break;
            case 56594:
              num1 = offset;
              emojiIndex = 1812;
              break;
            case 56595:
              num1 = offset;
              emojiIndex = 1813;
              break;
            case 56596:
              num1 = offset;
              emojiIndex = 1814;
              break;
            case 56597:
              num1 = offset;
              emojiIndex = 1815;
              break;
            case 56598:
              num1 = offset;
              emojiIndex = 1816;
              break;
            case 56599:
              num1 = offset;
              emojiIndex = 1817;
              break;
            case 56600:
              num1 = offset;
              emojiIndex = 1823;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1818;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1819;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1820;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1821;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1822;
                    break;
                }
              }
              else
                break;
              break;
            case 56601:
              num1 = offset;
              emojiIndex = 1829;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1824;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1825;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1826;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1827;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1828;
                    break;
                }
              }
              else
                break;
              break;
            case 56602:
              num1 = offset;
              emojiIndex = 1835;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1830;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1831;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1832;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1833;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1834;
                    break;
                }
              }
              else
                break;
              break;
            case 56603:
              num1 = offset;
              emojiIndex = 1841;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1836;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1837;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1838;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1839;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1840;
                    break;
                }
              }
              else
                break;
              break;
            case 56604:
              num1 = offset;
              emojiIndex = 1847;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1842;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1843;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1844;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1845;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1846;
                    break;
                }
              }
              else
                break;
              break;
            case 56605:
              num1 = offset;
              emojiIndex = 1853;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1848;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1849;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1850;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1851;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1852;
                    break;
                }
              }
              else
                break;
              break;
            case 56606:
              num1 = offset;
              emojiIndex = 1859;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1854;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1855;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1856;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1857;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1858;
                    break;
                }
              }
              else
                break;
              break;
            case 56607:
              num1 = offset;
              emojiIndex = 1865;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1860;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1861;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1862;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1863;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1864;
                    break;
                }
              }
              else
                break;
              break;
            case 56608:
              num1 = offset;
              emojiIndex = 1866;
              break;
            case 56609:
              num1 = offset;
              emojiIndex = 1867;
              break;
            case 56610:
              num1 = offset;
              emojiIndex = 1868;
              break;
            case 56611:
              num1 = offset;
              emojiIndex = 1869;
              break;
            case 56612:
              num1 = offset;
              emojiIndex = 1870;
              break;
            case 56613:
              num1 = offset;
              emojiIndex = 1871;
              break;
            case 56614:
              num1 = offset;
              emojiIndex = 1882;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1882;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1883;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1872;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1872;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1873;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1874;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1874;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1875;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1876;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1876;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1877;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1878;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1878;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1879;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1880;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1880;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1881;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56615:
              num1 = offset;
              emojiIndex = 1884;
              break;
            case 56616:
              num1 = offset;
              emojiIndex = 1885;
              break;
            case 56617:
              num1 = offset;
              emojiIndex = 1886;
              break;
            case 56618:
              num1 = offset;
              emojiIndex = 1887;
              break;
            case 56619:
              num1 = offset;
              emojiIndex = 1888;
              break;
            case 56620:
              num1 = offset;
              emojiIndex = 1889;
              break;
            case 56621:
              num1 = offset;
              emojiIndex = 1890;
              break;
            case 56622:
              num1 = offset;
              emojiIndex = 1891;
              break;
            case 56623:
              num1 = offset;
              emojiIndex = 1892;
              break;
            case 56624:
              num1 = offset;
              emojiIndex = 1898;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1893;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1894;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1895;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1896;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1897;
                    break;
                }
              }
              else
                break;
              break;
            case 56625:
              num1 = offset;
              emojiIndex = 1904;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1899;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1900;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1901;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1902;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1903;
                    break;
                }
              }
              else
                break;
              break;
            case 56626:
              num1 = offset;
              emojiIndex = 1910;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1905;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1906;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1907;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1908;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1909;
                    break;
                }
              }
              else
                break;
              break;
            case 56627:
              num1 = offset;
              emojiIndex = 1916;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1911;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1912;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1913;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1914;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1915;
                    break;
                }
              }
              else
                break;
              break;
            case 56628:
              num1 = offset;
              emojiIndex = 1922;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1917;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1918;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1919;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1920;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1921;
                    break;
                }
              }
              else
                break;
              break;
            case 56629:
              num1 = offset;
              emojiIndex = 1928;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1923;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1924;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1925;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1926;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1927;
                    break;
                }
              }
              else
                break;
              break;
            case 56630:
              num1 = offset;
              emojiIndex = 1934;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 1929;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 1930;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 1931;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 1932;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 1933;
                    break;
                }
              }
              else
                break;
              break;
            case 56631:
              num1 = offset;
              emojiIndex = 1945;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1945;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1946;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1935;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1935;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1936;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1937;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1937;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1938;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1939;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1939;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1940;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1941;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1941;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1942;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1943;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1943;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1944;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56632:
              num1 = offset;
              emojiIndex = 1957;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1957;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1958;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1947;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1947;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1948;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1949;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1949;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1950;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1951;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1951;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1952;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1953;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1953;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1954;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1955;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1955;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1956;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56633:
              num1 = offset;
              emojiIndex = 1970;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1969;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1970;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1960;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1959;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1960;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1962;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1961;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1962;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1964;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1963;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1964;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1966;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1965;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1966;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1968;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1967;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1968;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56634:
              num1 = offset;
              emojiIndex = 1971;
              break;
            case 56636:
              num1 = offset;
              emojiIndex = 1974;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 9792:
                    num1 = offset;
                    emojiIndex = 1972;
                    break;
                  case 9794:
                    num1 = offset;
                    emojiIndex = 1973;
                    break;
                }
              }
              else
                break;
              break;
            case 56637:
              num1 = offset;
              emojiIndex = 1986;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1985;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1986;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1976;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1975;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1976;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1978;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1977;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1978;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1980;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1979;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1980;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1982;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1981;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1982;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1984;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1983;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1984;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56638:
              num1 = offset;
              emojiIndex = 1998;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 1997;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 1998;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 1988;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1987;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1988;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 1990;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1989;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1990;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 1992;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1991;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1992;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 1994;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1993;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1994;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 1996;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 1995;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 1996;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56640:
              num1 = offset;
              emojiIndex = 1999;
              break;
            case 56641:
              num1 = offset;
              emojiIndex = 2000;
              break;
            case 56642:
              num1 = offset;
              emojiIndex = 2001;
              break;
            case 56643:
              num1 = offset;
              emojiIndex = 2002;
              break;
            case 56644:
              num1 = offset;
              emojiIndex = 2003;
              break;
            case 56645:
              num1 = offset;
              emojiIndex = 2004;
              break;
            case 56647:
              num1 = offset;
              emojiIndex = 2005;
              break;
            case 56648:
              num1 = offset;
              emojiIndex = 2006;
              break;
            case 56649:
              num1 = offset;
              emojiIndex = 2007;
              break;
            case 56650:
              num1 = offset;
              emojiIndex = 2008;
              break;
            case 56651:
              num1 = offset;
              emojiIndex = 2009;
              break;
            case 56652:
              num1 = offset;
              emojiIndex = 2010;
              break;
            case 56656:
              num1 = offset;
              emojiIndex = 2011;
              break;
            case 56657:
              num1 = offset;
              emojiIndex = 2012;
              break;
            case 56658:
              num1 = offset;
              emojiIndex = 2013;
              break;
            case 56659:
              num1 = offset;
              emojiIndex = 2014;
              break;
            case 56660:
              num1 = offset;
              emojiIndex = 2015;
              break;
            case 56661:
              num1 = offset;
              emojiIndex = 2016;
              break;
            case 56662:
              num1 = offset;
              emojiIndex = 2017;
              break;
            case 56663:
              num1 = offset;
              emojiIndex = 2018;
              break;
            case 56664:
              num1 = offset;
              emojiIndex = 2019;
              break;
            case 56665:
              num1 = offset;
              emojiIndex = 2020;
              break;
            case 56666:
              num1 = offset;
              emojiIndex = 2021;
              break;
            case 56667:
              num1 = offset;
              emojiIndex = 2022;
              break;
            case 56668:
              num1 = offset;
              emojiIndex = 2023;
              break;
            case 56669:
              num1 = offset;
              emojiIndex = 2024;
              break;
            case 56670:
              num1 = offset;
              emojiIndex = 2025;
              break;
            case 56671:
              num1 = offset;
              emojiIndex = 2026;
              break;
            case 56672:
              num1 = offset;
              emojiIndex = 2027;
              break;
            case 56673:
              num1 = offset;
              emojiIndex = 2028;
              break;
            case 56674:
              num1 = offset;
              emojiIndex = 2029;
              break;
            case 56675:
              num1 = offset;
              emojiIndex = 2030;
              break;
            case 56676:
              num1 = offset;
              emojiIndex = 2031;
              break;
            case 56677:
              num1 = offset;
              emojiIndex = 2032;
              break;
            case 56678:
              num1 = offset;
              emojiIndex = 2033;
              break;
            case 56679:
              num1 = offset;
              emojiIndex = 2034;
              break;
            case 56680:
              num1 = offset;
              emojiIndex = 2035;
              break;
            case 56681:
              num1 = offset;
              emojiIndex = 2036;
              break;
            case 56682:
              num1 = offset;
              emojiIndex = 2037;
              break;
            case 56683:
              num1 = offset;
              emojiIndex = 2038;
              break;
            case 56704:
              num1 = offset;
              emojiIndex = 2039;
              break;
            case 56705:
              num1 = offset;
              emojiIndex = 2040;
              break;
            case 56706:
              num1 = offset;
              emojiIndex = 2041;
              break;
            case 56707:
              num1 = offset;
              emojiIndex = 2042;
              break;
            case 56708:
              num1 = offset;
              emojiIndex = 2043;
              break;
            case 56709:
              num1 = offset;
              emojiIndex = 2044;
              break;
            case 56710:
              num1 = offset;
              emojiIndex = 2045;
              break;
            case 56711:
              num1 = offset;
              emojiIndex = 2046;
              break;
            case 56712:
              num1 = offset;
              emojiIndex = 2047;
              break;
            case 56713:
              num1 = offset;
              emojiIndex = 2048;
              break;
            case 56714:
              num1 = offset;
              emojiIndex = 2049;
              break;
            case 56715:
              num1 = offset;
              emojiIndex = 2050;
              break;
            case 56716:
              num1 = offset;
              emojiIndex = 2051;
              break;
            case 56717:
              num1 = offset;
              emojiIndex = 2052;
              break;
            case 56718:
              num1 = offset;
              emojiIndex = 2053;
              break;
            case 56719:
              num1 = offset;
              emojiIndex = 2054;
              break;
            case 56720:
              num1 = offset;
              emojiIndex = 2055;
              break;
            case 56721:
              num1 = offset;
              emojiIndex = 2056;
              break;
            case 56722:
              num1 = offset;
              emojiIndex = 2057;
              break;
            case 56723:
              num1 = offset;
              emojiIndex = 2058;
              break;
            case 56724:
              num1 = offset;
              emojiIndex = 2059;
              break;
            case 56725:
              num1 = offset;
              emojiIndex = 2060;
              break;
            case 56726:
              num1 = offset;
              emojiIndex = 2061;
              break;
            case 56727:
              num1 = offset;
              emojiIndex = 2062;
              break;
            case 56768:
              num1 = offset;
              emojiIndex = 2063;
              break;
            case 56784:
              num1 = offset;
              emojiIndex = 2064;
              break;
            case 56785:
              num1 = offset;
              emojiIndex = 2070;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 2065;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 2066;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 2067;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 2068;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 2069;
                    break;
                }
              }
              else
                break;
              break;
            case 56786:
              num1 = offset;
              emojiIndex = 2076;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 2071;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 2072;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 2073;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 2074;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 2075;
                    break;
                }
              }
              else
                break;
              break;
            case 56787:
              num1 = offset;
              emojiIndex = 2082;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 2077;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 2078;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 2079;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 2080;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 2081;
                    break;
                }
              }
              else
                break;
              break;
            case 56788:
              num1 = offset;
              emojiIndex = 2088;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 2083;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 2084;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 2085;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 2086;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 2087;
                    break;
                }
              }
              else
                break;
              break;
            case 56789:
              num1 = offset;
              emojiIndex = 2094;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 55356)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 57339:
                    num1 = offset;
                    emojiIndex = 2089;
                    break;
                  case 57340:
                    num1 = offset;
                    emojiIndex = 2090;
                    break;
                  case 57341:
                    num1 = offset;
                    emojiIndex = 2091;
                    break;
                  case 57342:
                    num1 = offset;
                    emojiIndex = 2092;
                    break;
                  case 57343:
                    num1 = offset;
                    emojiIndex = 2093;
                    break;
                }
              }
              else
                break;
              break;
            case 56790:
              num1 = offset;
              emojiIndex = 2105;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2105;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2106;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2095;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2095;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2096;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2097;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2097;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2098;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2099;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2099;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2100;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2101;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2101;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2102;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2103;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2103;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2104;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56791:
              num1 = offset;
              emojiIndex = 2117;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2117;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2118;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2107;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2107;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2108;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2109;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2109;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2110;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2111;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2111;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2112;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2113;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2113;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2114;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2115;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2115;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2116;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56792:
              num1 = offset;
              emojiIndex = 2129;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2129;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2130;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2119;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2119;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2120;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2121;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2121;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2122;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2123;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2123;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2124;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2125;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2125;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2126;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2127;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2127;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2128;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56793:
              num1 = offset;
              emojiIndex = 2141;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2141;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2142;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2131;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2131;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2132;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2133;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2133;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2134;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2135;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2135;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2136;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2137;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2137;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2138;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2139;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2139;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2140;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56794:
              num1 = offset;
              emojiIndex = 2153;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2153;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2154;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2143;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2143;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2144;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2145;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2145;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2146;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2147;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2147;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2148;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2149;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2149;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2150;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2151;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2151;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2152;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56795:
              num1 = offset;
              emojiIndex = 2165;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2165;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2166;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2155;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2155;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2156;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2157;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2157;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2158;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2159;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2159;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2160;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2161;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2161;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2162;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2163;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2163;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2164;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56796:
              num1 = offset;
              emojiIndex = 2177;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2177;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2178;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2167;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2167;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2168;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2169;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2169;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2170;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2171;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2171;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2172;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2173;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2173;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2174;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2175;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2175;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2176;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56797:
              num1 = offset;
              emojiIndex = 2190;
              switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
              {
                case 8205:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 9792:
                      num1 = offset;
                      emojiIndex = 2189;
                      break;
                    case 9794:
                      num1 = offset;
                      emojiIndex = 2190;
                      break;
                  }
                  break;
                case 55356:
                  switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                  {
                    case 57339:
                      num1 = offset;
                      emojiIndex = 2180;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2179;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2180;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57340:
                      num1 = offset;
                      emojiIndex = 2182;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2181;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2182;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57341:
                      num1 = offset;
                      emojiIndex = 2184;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2183;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2184;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57342:
                      num1 = offset;
                      emojiIndex = 2186;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2185;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2186;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                    case 57343:
                      num1 = offset;
                      emojiIndex = 2188;
                      if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
                      {
                        switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                        {
                          case 9792:
                            num1 = offset;
                            emojiIndex = 2187;
                            break;
                          case 9794:
                            num1 = offset;
                            emojiIndex = 2188;
                            break;
                        }
                      }
                      else
                        break;
                      break;
                  }
                  break;
              }
              break;
            case 56798:
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 9792:
                    num1 = offset;
                    emojiIndex = 2191;
                    break;
                  case 9794:
                    num1 = offset;
                    emojiIndex = 2192;
                    break;
                }
              }
              else
                break;
              break;
            case 56799:
              num1 = offset;
              emojiIndex = 2194;
              if (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2) == 8205)
              {
                switch (Emoji.ReadNextEmojiChar(str, len, ref offset, filterVariationSelectors2))
                {
                  case 9792:
                    num1 = offset;
                    emojiIndex = 2193;
                    break;
                  case 9794:
                    num1 = offset;
                    emojiIndex = 2194;
                    break;
                }
              }
              else
                break;
              break;
            case 56800:
              num1 = offset;
              emojiIndex = 2195;
              break;
            case 56801:
              num1 = offset;
              emojiIndex = 2196;
              break;
            case 56802:
              num1 = offset;
              emojiIndex = 2197;
              break;
            case 56803:
              num1 = offset;
              emojiIndex = 2198;
              break;
            case 56804:
              num1 = offset;
              emojiIndex = 2199;
              break;
            case 56805:
              num1 = offset;
              emojiIndex = 2200;
              break;
            case 56806:
              num1 = offset;
              emojiIndex = 2201;
              break;
          }
          break;
      }
      offset = num1;
      if (emojiIndex > 0)
        Emoji.ReadLastEmojiChar(str, len, ref offset);
      return emojiIndex;
    }

    private static int ReadNextEmojiChar(
      string str,
      int bufferLen,
      ref int offset,
      bool filterVariationSelectors)
    {
      int ch = -1;
      while (offset < bufferLen)
      {
        ch = (int) str[offset++];
        if (filterVariationSelectors && Emoji.IsVariantSelector((uint) ch))
          ch = -1;
        else if (Emoji.IsTextOrEmojiVariantSelector((uint) ch))
          ch = -1;
        else
          break;
      }
      return ch;
    }

    private static void ReadLastEmojiChar(string str, int bufferLen, ref int offset)
    {
      if (offset == 0)
        return;
      while (offset < bufferLen && Emoji.IsVariantSelector((uint) str[offset]))
        ++offset;
    }

    private static bool TryGetIndex(string str, out int index)
    {
      int offset = 0;
      index = Emoji.TryGetEmojiIndex(str, ref offset, str.Length);
      if (index < 0 || offset == 0 || offset != str.Length)
        index = 0;
      return index > 0;
    }

    public static IEnumerable<LinkDetector.Result> GetAllEmojiMatches(
      string str,
      int offset,
      int len)
    {
      List<LinkDetector.Result> allEmojiMatches = new List<LinkDetector.Result>();
      int num = offset + len;
      while (offset < num)
      {
        int index = offset;
        if (Emoji.TryGetEmojiIndex(str, ref offset, len) > 0)
        {
          int length = offset - index;
          allEmojiMatches.Add(new LinkDetector.Result(index, length, 2, str));
          len -= length;
        }
        else
        {
          offset = index + 1;
          --len;
        }
      }
      return (IEnumerable<LinkDetector.Result>) allEmojiMatches;
    }

    private static string FilterVariationSelectors(string str)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      for (int index = 0; index < str.Length; ++index)
      {
        if (str[index] == '️' || str[index] == '︎')
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          stringBuilder.Append(str, startIndex, index - startIndex);
          startIndex = index + 1;
        }
      }
      if (stringBuilder != null && startIndex < str.Length)
        stringBuilder.Append(str, startIndex, str.Length - startIndex);
      return stringBuilder?.ToString() ?? str;
    }

    public static Emoji.EmojiChar Mappings(string codepoint)
    {
      int index;
      if (Emoji.TryGetIndex(codepoint, out index))
      {
        string str = codepoint;
        string genderFromEnd = Emoji.ExtractGenderFromEnd(str);
        if (genderFromEnd != null)
          str = str.Replace(genderFromEnd, "");
        string occupation = genderFromEnd == null ? Emoji.ExtractOccupation(str) : (string) null;
        if (occupation != null)
          str = str.Replace(occupation, "");
        string skintone = str.Length >= 2 ? Emoji.ExtractSkintone(str) : (string) null;
        if (skintone != null)
          str = str.Replace(skintone, "");
        return new Emoji.EmojiChar()
        {
          index = index,
          emojiType = Emoji.IsSkintonePrefix(str) ? Emoji.EmojiType.Skintones : Emoji.EmojiType.Regular,
          codepoints = codepoint
        };
      }
      Emoji.LogCodepoint("EmojiChar missing", codepoint);
      if (codepoint.Length == 2)
      {
        string codepoint1 = Emoji.AddGenderIfMaybeMissing(codepoint);
        if (codepoint1 != null)
          return Emoji.Mappings(codepoint1);
      }
      return (Emoji.EmojiChar) null;
    }

    public static string AddGenderIfMaybeMissing(string codepoint)
    {
      switch (codepoint)
      {
        case "⛹":
        case "\uD83C\uDFC3":
        case "\uD83C\uDFC4":
        case "\uD83C\uDFCA":
        case "\uD83C\uDFCB":
        case "\uD83C\uDFCC":
        case "\uD83D\uDC6E":
        case "\uD83D\uDC71":
        case "\uD83D\uDC73":
        case "\uD83D\uDC77":
        case "\uD83D\uDC82":
        case "\uD83D\uDD75":
        case "\uD83D\uDE47":
        case "\uD83D\uDEA3":
        case "\uD83D\uDEB4":
        case "\uD83D\uDEB5":
        case "\uD83D\uDEB6":
          return codepoint + "\u200D♂";
        case "\uD83D\uDC6F":
        case "\uD83D\uDC81":
        case "\uD83D\uDC86":
        case "\uD83D\uDC87":
        case "\uD83D\uDE45":
        case "\uD83D\uDE46":
        case "\uD83D\uDE4B":
        case "\uD83D\uDE4D":
        case "\uD83D\uDE4E":
          return codepoint + "\u200D♀";
        default:
          return (string) null;
      }
    }

    public static string[][] SoftbankCodepoints
    {
      get
      {
        if (Emoji.softbankCodepoints == null)
          Emoji.softbankCodepoints = new string[6][]
          {
            new string[90]
            {
              "\uD83D\uDC66",
              "\uD83D\uDC67",
              "\uD83D\uDC8B",
              "\uD83D\uDC68",
              "\uD83D\uDC69",
              "\uD83D\uDC55",
              "\uD83D\uDC5F",
              "\uD83D\uDCF7",
              "☎",
              "\uD83D\uDCF1",
              "\uD83D\uDCE0",
              "\uD83D\uDCBB",
              "\uD83D\uDC4A",
              "\uD83D\uDC4D",
              "☝",
              "✊",
              "✌",
              "✋",
              "\uD83C\uDFBF",
              "⛳",
              "\uD83C\uDFBE",
              "⚾",
              "\uD83C\uDFC4",
              "⚽",
              "\uD83D\uDC1F",
              "\uD83D\uDC34",
              "\uD83D\uDE97",
              "⛵",
              "✈",
              "\uD83D\uDE83",
              "\uD83D\uDE85",
              "❓",
              "❗",
              "❤",
              "\uD83D\uDC94",
              "\uD83D\uDD50",
              "\uD83D\uDD51",
              "\uD83D\uDD52",
              "\uD83D\uDD53",
              "\uD83D\uDD54",
              "\uD83D\uDD55",
              "\uD83D\uDD56",
              "\uD83D\uDD57",
              "\uD83D\uDD58",
              "\uD83D\uDD59",
              "\uD83D\uDD5A",
              "\uD83D\uDD5B",
              "\uD83C\uDF38",
              "\uD83D\uDD31",
              "\uD83C\uDF39",
              "\uD83C\uDF84",
              "\uD83D\uDC8D",
              "\uD83D\uDC8E",
              "\uD83C\uDFE0",
              "⛪",
              "\uD83C\uDFE2",
              "\uD83D\uDE89",
              "⛽",
              "\uD83D\uDDFB",
              "\uD83C\uDFA4",
              "\uD83C\uDFA5",
              "\uD83C\uDFB5",
              "\uD83D\uDD11",
              "\uD83C\uDFB7",
              "\uD83C\uDFB8",
              "\uD83C\uDFBA",
              "\uD83C\uDF74",
              "\uD83C\uDF78",
              "☕",
              "\uD83C\uDF70",
              "\uD83C\uDF7A",
              "⛄",
              "☁",
              "☀",
              "☔",
              "\uD83C\uDF19",
              "\uD83C\uDF04",
              "\uD83D\uDC7C",
              "\uD83D\uDC31",
              "\uD83D\uDC2F",
              "\uD83D\uDC3B",
              "\uD83D\uDC36",
              "\uD83D\uDC2D",
              "\uD83D\uDC33",
              "\uD83D\uDC27",
              "\uD83D\uDE0A",
              "\uD83D\uDE03",
              "\uD83D\uDE1E",
              "\uD83D\uDE20",
              "\uD83D\uDCA9"
            },
            new string[90]
            {
              "\uD83D\uDCEB",
              "\uD83D\uDCEE",
              "\uD83D\uDCE9",
              "\uD83D\uDCF2",
              "\uD83D\uDE1C",
              "\uD83D\uDE0D",
              "\uD83D\uDE31",
              "\uD83D\uDE13",
              "\uD83D\uDC35",
              "\uD83D\uDC19",
              "\uD83D\uDC37",
              "\uD83D\uDC7D",
              "\uD83D\uDE80",
              "\uD83D\uDC51",
              "\uD83D\uDCA1",
              "\uD83C\uDF40",
              "\uD83D\uDC8F",
              "\uD83C\uDF81",
              "\uD83D\uDD2B",
              "\uD83D\uDD0D",
              "\uD83C\uDFC3",
              "\uD83D\uDD28",
              "\uD83C\uDF86",
              "\uD83C\uDF41",
              "\uD83C\uDF42",
              "\uD83D\uDC7F",
              "\uD83D\uDC7B",
              "\uD83D\uDC80",
              "\uD83D\uDD25",
              "\uD83D\uDCBC",
              "\uD83D\uDCBA",
              "\uD83C\uDF54",
              "⛲",
              "⛺",
              "♨",
              "\uD83C\uDFA1",
              "\uD83C\uDFAB",
              "\uD83D\uDCBF",
              "\uD83D\uDCC0",
              "\uD83D\uDCFB",
              "\uD83D\uDCFC",
              "\uD83D\uDCFA",
              "\uD83D\uDC7E",
              "〽",
              "\uD83C\uDC04",
              "\uD83C\uDD9A",
              "\uD83D\uDCB0",
              "\uD83C\uDFAF",
              "\uD83C\uDFC6",
              "\uD83C\uDFC1",
              "\uD83C\uDFB0",
              "\uD83D\uDC0E",
              "\uD83D\uDEA4",
              "\uD83D\uDEB2",
              "\uD83D\uDEA7",
              "\uD83D\uDEB9",
              "\uD83D\uDEBA",
              "\uD83D\uDEBC",
              "\uD83D\uDC89",
              "\uD83D\uDCA4",
              "⚡",
              "\uD83D\uDC60",
              "\uD83D\uDEC0",
              "\uD83D\uDEBD",
              "\uD83D\uDD0A",
              "\uD83D\uDCE2",
              "\uD83C\uDF8C",
              "\uD83D\uDD12",
              "\uD83D\uDD13",
              "\uD83C\uDF06",
              "\uD83C\uDF73",
              "\uD83D\uDCD6",
              "\uD83D\uDCB1",
              "\uD83D\uDCB9",
              "\uD83D\uDCE1",
              "\uD83D\uDCAA",
              "\uD83C\uDFE6",
              "\uD83D\uDEA5",
              "\uD83C\uDD7F",
              "\uD83D\uDE8F",
              "\uD83D\uDEBB",
              "\uD83D\uDC6E",
              "\uD83C\uDFE3",
              "\uD83C\uDFE7",
              "\uD83C\uDFE5",
              "\uD83C\uDFEA",
              "\uD83C\uDFEB",
              "\uD83C\uDFE8",
              "\uD83D\uDE8C",
              "\uD83D\uDE95"
            },
            new string[83]
            {
              "\uD83D\uDEB6",
              "\uD83D\uDEA2",
              "\uD83C\uDE01",
              "\uD83D\uDC9F",
              "✴",
              "✳",
              "\uD83D\uDD1E",
              "\uD83D\uDEAD",
              "\uD83D\uDD30",
              "♿",
              "\uD83D\uDCF6",
              "♥",
              "♦",
              "♠",
              "♣",
              "#⃣",
              "➿",
              "\uD83C\uDD95",
              "\uD83C\uDD99",
              "\uD83C\uDD92",
              "\uD83C\uDE36",
              "\uD83C\uDE1A",
              "\uD83C\uDE37",
              "\uD83C\uDE38",
              "\uD83D\uDD34",
              "\uD83D\uDD32",
              "\uD83D\uDD33",
              "1⃣",
              "2⃣",
              "3⃣",
              "4⃣",
              "5⃣",
              "6⃣",
              "7⃣",
              "8⃣",
              "9⃣",
              "0⃣",
              "\uD83C\uDE50",
              "\uD83C\uDE39",
              "\uD83C\uDE02",
              "\uD83C\uDD94",
              "\uD83C\uDE35",
              "\uD83C\uDE33",
              "\uD83C\uDE2F",
              "\uD83C\uDE3A",
              "\uD83D\uDC46",
              "\uD83D\uDC47",
              "\uD83D\uDC48",
              "\uD83D\uDC49",
              "⬆",
              "⬇",
              "➡",
              "⬅",
              "↗",
              "↖",
              "↘",
              "↙",
              "▶",
              "◀",
              "⏩",
              "⏪",
              "\uD83D\uDD2F",
              "♈",
              "♉",
              "♊",
              "♋",
              "♌",
              "♍",
              "♎",
              "♏",
              "♐",
              "♑",
              "♒",
              "♓",
              "⛎",
              "\uD83D\uDD1D",
              "\uD83C\uDD97",
              "©",
              "®",
              "\uD83D\uDCF3",
              "\uD83D\uDCF4",
              "⚠",
              "\uD83D\uDC81"
            },
            new string[77]
            {
              "\uD83D\uDCDD",
              "\uD83D\uDC54",
              "\uD83C\uDF3A",
              "\uD83C\uDF37",
              "\uD83C\uDF3B",
              "\uD83D\uDC90",
              "\uD83C\uDF34",
              "\uD83C\uDF35",
              "\uD83D\uDEBE",
              "\uD83C\uDFA7",
              "\uD83C\uDF76",
              "\uD83C\uDF7B",
              "㊗",
              "\uD83D\uDEAC",
              "\uD83D\uDC8A",
              "\uD83C\uDF88",
              "\uD83D\uDCA3",
              "\uD83C\uDF89",
              "✂",
              "\uD83C\uDF80",
              "㊙",
              "\uD83D\uDCBD",
              "\uD83D\uDCE3",
              "\uD83D\uDC52",
              "\uD83D\uDC57",
              "\uD83D\uDC61",
              "\uD83D\uDC62",
              "\uD83D\uDC84",
              "\uD83D\uDC85",
              "\uD83D\uDC86",
              "\uD83D\uDC87",
              "\uD83D\uDC88",
              "\uD83D\uDC58",
              "\uD83D\uDC59",
              "\uD83D\uDC5C",
              "\uD83C\uDFAC",
              "\uD83D\uDD14",
              "\uD83C\uDFB6",
              "\uD83D\uDC93",
              "\uD83D\uDC97",
              "\uD83D\uDC98",
              "\uD83D\uDC99",
              "\uD83D\uDC9A",
              "\uD83D\uDC9B",
              "\uD83D\uDC9C",
              "✨",
              "⭐",
              "\uD83D\uDCA8",
              "\uD83D\uDCA6",
              "⭕",
              "❌",
              "\uD83D\uDCA2",
              "\uD83C\uDF1F",
              "❔",
              "❕",
              "\uD83C\uDF75",
              "\uD83C\uDF5E",
              "\uD83C\uDF66",
              "\uD83C\uDF5F",
              "\uD83C\uDF61",
              "\uD83C\uDF58",
              "\uD83C\uDF5A",
              "\uD83C\uDF5D",
              "\uD83C\uDF5C",
              "\uD83C\uDF5B",
              "\uD83C\uDF59",
              "\uD83C\uDF62",
              "\uD83C\uDF63",
              "\uD83C\uDF4E",
              "\uD83C\uDF4A",
              "\uD83C\uDF53",
              "\uD83C\uDF49",
              "\uD83C\uDF45",
              "\uD83C\uDF46",
              "\uD83C\uDF82",
              "\uD83C\uDF71",
              "\uD83C\uDF72"
            },
            new string[76]
            {
              "\uD83D\uDE25",
              "\uD83D\uDE0F",
              "\uD83D\uDE14",
              "\uD83D\uDE01",
              "\uD83D\uDE09",
              "\uD83D\uDE23",
              "\uD83D\uDE16",
              "\uD83D\uDE2A",
              "\uD83D\uDE1D",
              "\uD83D\uDE0C",
              "\uD83D\uDE28",
              "\uD83D\uDE37",
              "\uD83D\uDE33",
              "\uD83D\uDE12",
              "\uD83D\uDE30",
              "\uD83D\uDE32",
              "\uD83D\uDE2D",
              "\uD83D\uDE02",
              "\uD83D\uDE22",
              "☺",
              "\uD83D\uDE04",
              "\uD83D\uDE21",
              "\uD83D\uDE1A",
              "\uD83D\uDE18",
              "\uD83D\uDC40",
              "\uD83D\uDC43",
              "\uD83D\uDC42",
              "\uD83D\uDC44",
              "\uD83D\uDE4F",
              "\uD83D\uDC4B",
              "\uD83D\uDC4F",
              "\uD83D\uDC4C",
              "\uD83D\uDC4E",
              "\uD83D\uDC50",
              "\uD83D\uDE45",
              "\uD83D\uDE46",
              "\uD83D\uDC91",
              "\uD83D\uDE47",
              "\uD83D\uDE4C",
              "\uD83D\uDC6B",
              "\uD83D\uDC6F",
              "\uD83C\uDFC0",
              "\uD83C\uDFC8",
              "\uD83C\uDFB1",
              "\uD83C\uDFCA",
              "\uD83D\uDE99",
              "\uD83D\uDE9A",
              "\uD83D\uDE92",
              "\uD83D\uDE91",
              "\uD83D\uDE93",
              "\uD83C\uDFA2",
              "\uD83D\uDE87",
              "\uD83D\uDE84",
              "\uD83C\uDF8D",
              "\uD83D\uDC9D",
              "\uD83C\uDF8E",
              "\uD83C\uDF93",
              "\uD83C\uDF92",
              "\uD83C\uDF8F",
              "\uD83C\uDF02",
              "\uD83D\uDC92",
              "\uD83C\uDF0A",
              "\uD83C\uDF67",
              "\uD83C\uDF87",
              "\uD83D\uDC1A",
              "\uD83C\uDF90",
              "\uD83C\uDF00",
              "\uD83C\uDF3E",
              "\uD83C\uDF83",
              "\uD83C\uDF91",
              "\uD83C\uDF43",
              "\uD83C\uDF85",
              "\uD83C\uDF05",
              "\uD83C\uDF07",
              "\uD83C\uDF03",
              "\uD83C\uDF08"
            },
            new string[55]
            {
              "\uD83C\uDFE9",
              "\uD83C\uDFA8",
              "\uD83C\uDFA9",
              "\uD83C\uDFEC",
              "\uD83C\uDFEF",
              "\uD83C\uDFF0",
              "\uD83C\uDFA6",
              "\uD83C\uDFED",
              "\uD83D\uDDFC",
              string.Empty,
              "\uD83C\uDDEF\uD83C\uDDF5",
              "\uD83C\uDDFA\uD83C\uDDF8",
              "\uD83C\uDDEB\uD83C\uDDF7",
              "\uD83C\uDDE9\uD83C\uDDEA",
              "\uD83C\uDDEE\uD83C\uDDF9",
              "\uD83C\uDDEC\uD83C\uDDE7",
              "\uD83C\uDDEA\uD83C\uDDF8",
              "\uD83C\uDDF7\uD83C\uDDFA",
              "\uD83C\uDDE8\uD83C\uDDF3",
              "\uD83C\uDDF0\uD83C\uDDF7",
              "\uD83D\uDC71",
              "\uD83D\uDC72",
              "\uD83D\uDC73",
              "\uD83D\uDC74",
              "\uD83D\uDC75",
              "\uD83D\uDC76",
              "\uD83D\uDC77",
              "\uD83D\uDC78",
              "\uD83D\uDDFD",
              "\uD83D\uDC82",
              "\uD83D\uDC83",
              "\uD83D\uDC2C",
              "\uD83D\uDC26",
              "\uD83D\uDC20",
              "\uD83D\uDC24",
              "\uD83D\uDC39",
              "\uD83D\uDC1B",
              "\uD83D\uDC18",
              "\uD83D\uDC28",
              "\uD83D\uDC12",
              "\uD83D\uDC11",
              "\uD83D\uDC3A",
              "\uD83D\uDC2E",
              "\uD83D\uDC30",
              "\uD83D\uDC0D",
              "\uD83D\uDC14",
              "\uD83D\uDC17",
              "\uD83D\uDC2B",
              "\uD83D\uDC38",
              "\uD83C\uDD70",
              "\uD83C\uDD71",
              "\uD83C\uDD8E",
              "\uD83C\uDD7E",
              "\uD83D\uDC63",
              "™"
            }
          };
        return Emoji.softbankCodepoints;
      }
    }

    public static bool TryConvertSoftbankCodepoint(
      char softbankCodepointCandidate,
      out string actualCodepoint)
    {
      actualCodepoint = string.Empty;
      if (((int) softbankCodepointCandidate & 61440) != 57344)
        return false;
      int index1 = ((int) softbankCodepointCandidate & 3840) >> 8;
      if (index1 > 5)
        return false;
      string[] softbankCodepoint = Emoji.SoftbankCodepoints[index1];
      int index2 = ((int) softbankCodepointCandidate & (int) byte.MaxValue) - 1;
      if (index2 > softbankCodepoint.Length - 1 || index2 < 0)
        return false;
      actualCodepoint = softbankCodepoint[index2];
      return true;
    }

    public static Emoji.EmojiChar GetEmojiChar(string unicodeCodepoints)
    {
      return !string.IsNullOrEmpty(unicodeCodepoints) ? Emoji.Mappings(unicodeCodepoints) : (Emoji.EmojiChar) null;
    }

    public static string ConvertToUnicode(string text)
    {
      string unicode = (string) null;
      if (text == null)
        return unicode;
      StringBuilder stringBuilder = new StringBuilder();
      int length = text.Length;
      int startIndex = 0;
      for (int index = 0; index < length; ++index)
      {
        int softbankCodepointCandidate = (int) text[index];
        string str = (string) null;
        ref string local = ref str;
        if (Emoji.TryConvertSoftbankCodepoint((char) softbankCodepointCandidate, out local))
        {
          if (index > startIndex)
            stringBuilder.Append(text, startIndex, index - startIndex);
          stringBuilder.Append(str);
          startIndex = index + 1;
        }
        else if (Emoji.IsInvalidEmojiAndVariationPair(text, index))
        {
          if (index > startIndex)
            stringBuilder.Append(text, startIndex, index - startIndex);
          startIndex = index + 1;
        }
      }
      if (stringBuilder.Length > 0)
      {
        if (startIndex < length)
          stringBuilder.Append(text, startIndex, length - startIndex);
        unicode = stringBuilder.ToString();
      }
      return unicode ?? text;
    }

    public static string[] EmojisWithVariationSelectorCodepoints
    {
      get
      {
        if (Emoji.emojisWithVariationSelectorCodepoints == null)
          Emoji.emojisWithVariationSelectorCodepoints = new string[4]
          {
            "\uD83D\uDC68\u200D❤️\u200D\uD83D\uDC8B\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤️\u200D\uD83D\uDC8B\u200D\uD83D\uDC69",
            "\uD83D\uDC68\u200D❤️\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤️\u200D\uD83D\uDC69"
          };
        return Emoji.emojisWithVariationSelectorCodepoints;
      }
    }

    public static bool IsInvalidEmojiAndVariationPair(string s, int offset)
    {
      if (s[offset] < '︀' || s[offset] > '️')
        return false;
      string unicodeCodepoints = (string) null;
      try
      {
        if (!char.IsSurrogate(s, offset - 1))
          unicodeCodepoints = s.Substring(offset - 1, 1);
        else if (char.IsSurrogatePair(s, offset - 2))
          unicodeCodepoints = s.Substring(offset - 2, 2);
      }
      catch (Exception ex)
      {
        return false;
      }
      return unicodeCodepoints != null && Emoji.IsEmoji(unicodeCodepoints) && !Emoji.IsVariantSelectorCodepointPartOfEmoji(s, offset);
    }

    public static bool IsVariantSelectorCodepointPartOfEmoji(
      string text,
      int indexOfVariantSelectorCodepoint)
    {
      foreach (string selectorCodepoint1 in Emoji.EmojisWithVariationSelectorCodepoints)
      {
        int selectorCodepoint2 = Emoji.GetIndexOfVariantSelectorCodepoint(selectorCodepoint1);
        bool flag = true;
        int index = indexOfVariantSelectorCodepoint - selectorCodepoint2;
        if (index >= 0 && index + selectorCodepoint1.Length <= text.Length)
        {
          foreach (int num in selectorCodepoint1)
          {
            if (num != (int) text[index])
            {
              flag = false;
              break;
            }
            ++index;
          }
          if (flag)
            return true;
        }
      }
      return false;
    }

    public static int GetIndexOfVariantSelectorCodepoint(string codepoints)
    {
      int selectorCodepoint = 0;
      foreach (char codepoint in codepoints)
      {
        if (codepoint >= '︀' && codepoint <= '️')
          return selectorCodepoint;
        ++selectorCodepoint;
      }
      return -1;
    }

    public static string ToCountryEmoji(string regionString)
    {
      List<uint> chars = new List<uint>();
      foreach (char c in regionString)
      {
        if (char.IsLetter(c))
        {
          char ch = char.IsUpper(c) ? char.ToLower(c) : c;
          chars.Add((uint) (127462 + ((int) ch - 97)));
        }
      }
      return new string(UTF32.ToUtf16((IEnumerable<uint>) chars).ToArray<char>());
    }

    public static Emoji.EmojiSkinModifier ToSkinModifierEnum(string unicodeCodepoint)
    {
      if (unicodeCodepoint == null || unicodeCodepoint.Length != 2)
        return Emoji.EmojiSkinModifier.None;
      int num = (int) unicodeCodepoint[1] - (int) "\uD83C\uDFFB"[1];
      return num >= 0 && num < 6 ? (Emoji.EmojiSkinModifier) (num + 1) : Emoji.EmojiSkinModifier.None;
    }

    public static string ToSkinModifierString(Emoji.EmojiSkinModifier e)
    {
      int num = (int) e;
      if (num == 0 || num > 5)
        return (string) null;
      char ch1 = "\uD83C\uDFFB"[0];
      char ch2 = (char) ((int) "\uD83C\uDFFB"[1] + num - 1);
      return ch1.ToString() + ch2.ToString();
    }

    public static bool IsEmoji(string unicodeCodepoints)
    {
      return Emoji.Mappings(unicodeCodepoints) != null;
    }

    public static bool IsRegionalIndicator(uint ch) => ch >= 127462U && ch <= 127487U;

    public static bool IsCoupleIndicator(uint ch) => ch == 128104U || ch == 128105U;

    public static bool IsVariantSelector(uint ch) => ch > 65024U && ch <= 65039U;

    public static bool IsTextOrEmojiVariantSelector(uint ch) => ch > 65024U && ch <= 65039U;

    public static bool IsRegionalIndicatorAtPosition(string text, int position)
    {
      if (position + 4 > text.Length || !char.IsSurrogatePair(text[position], text[position + 1]) || !char.IsSurrogatePair(text[position + 2], text[position + 3]))
        return false;
      int utf32_1 = (int) UTF32.ToUtf32(text[position], text[position + 1]);
      uint utf32_2 = UTF32.ToUtf32(text[position + 2], text[position + 3]);
      return Emoji.IsRegionalIndicator((uint) utf32_1) && Emoji.IsRegionalIndicator(utf32_2);
    }

    public static bool IsKeyCapCharacterAtPosition(string text, int position)
    {
      return position + 1 < text.Length && text[position + 1] == '⃣';
    }

    public static bool IsSkinModifierCharacterAtPosition(string text, int position)
    {
      if (position + 2 > text.Length)
        return false;
      int num = (int) text[position];
      char ch = text[position + 1];
      return num == 55356 && ch >= '\uDFFB' && ch <= '\uDFFF';
    }

    public static List<string> getAllEmojiVariants(string codepoints)
    {
      Emoji.EmojiChar emojiChar = Emoji.Mappings(codepoints);
      if (emojiChar == null || emojiChar.emojiType != Emoji.EmojiType.Skintones)
        return (List<string>) null;
      List<string> allEmojiVariants = new List<string>();
      allEmojiVariants.Add(Emoji.GetBaseEmoji(codepoints));
      for (int index = 1; index <= 5; ++index)
      {
        string str = Emoji.AddSkinTone(codepoints, index);
        allEmojiVariants.Add(str);
      }
      return allEmojiVariants;
    }

    public static string ExtractSkintone(string glyph)
    {
      if (glyph.Length < 2)
        return (string) null;
      string skintone = glyph.Substring(glyph.Length - 2);
      if (skintone == "\uD83C\uDFFB" || skintone == "\uD83C\uDFFC" || skintone == "\uD83C\uDFFD" || skintone == "\uD83C\uDFFE" || skintone == "\uD83C\uDFFF")
        return skintone;
      if (glyph.Length < 5)
        return (string) null;
      string str = glyph.Substring(2);
      return str == "\uD83C\uDFFB" || str == "\uD83C\uDFFC" || str == "\uD83C\uDFFD" || str == "\uD83C\uDFFE" || str == "\uD83C\uDFFF" ? str : (string) null;
    }

    public static string ExtractOccupation(string glyph)
    {
      int startIndex = glyph.LastIndexOf('\u200D');
      if (startIndex < 0 || startIndex >= glyph.Length - 1)
        return (string) null;
      switch (glyph.Substring(startIndex + 1))
      {
        case "⚕":
        case "⚖":
        case "✈":
        case "\uD83C\uDF3E":
        case "\uD83C\uDF73":
        case "\uD83C\uDF93":
        case "\uD83C\uDFA4":
        case "\uD83C\uDFA8":
        case "\uD83C\uDFEB":
        case "\uD83C\uDFED":
        case "\uD83D\uDCBB":
        case "\uD83D\uDCBC":
        case "\uD83D\uDD27":
        case "\uD83D\uDD2C":
        case "\uD83D\uDE80":
        case "\uD83D\uDE92":
          return glyph.Substring(startIndex);
        default:
          return (string) null;
      }
    }

    public static string ExtractGenderFromEnd(string glyph)
    {
      string genderFromEnd = (string) null;
      if (glyph.Length > 2)
      {
        switch (glyph[glyph.Length - 1])
        {
          case '♀':
          case '♂':
            if (glyph[glyph.Length - 2] == '\u200D')
            {
              genderFromEnd = glyph.Substring(glyph.Length - 2, 2);
              break;
            }
            break;
          default:
            return genderFromEnd;
        }
      }
      return genderFromEnd;
    }

    public static string ExtractGender(string glyph)
    {
      string gender = (string) null;
      for (int index = glyph.Length - 1; index > 1; --index)
      {
        switch (glyph[index])
        {
          case '♀':
          case '♂':
            if (glyph[index - 1] == '\u200D')
            {
              gender = glyph.Substring(index - 1, 2);
              index = 0;
              break;
            }
            break;
        }
      }
      Log.d(nameof (ExtractGender), "returning {0}", (object) (gender == null ? -1 : gender.Length));
      return gender;
    }

    public static string GetBaseEmoji(string glyph, bool preserveGender = true, bool preserveOccupation = true)
    {
      if (Emoji.IsVariantSelector(glyph))
      {
        string genderFromEnd = preserveGender ? Emoji.ExtractGenderFromEnd(glyph) : (string) null;
        string occupation = preserveOccupation ? Emoji.ExtractOccupation(glyph) : (string) null;
        if (char.IsSurrogate(glyph[0]))
        {
          char ch = glyph[0];
          string str1 = ch.ToString();
          ch = glyph[1];
          string str2 = ch.ToString();
          glyph = str1 + str2;
        }
        else
          glyph = glyph[0].ToString() ?? "";
        if (genderFromEnd != null)
          glyph += genderFromEnd;
        if (occupation != null)
          glyph += occupation;
      }
      return glyph;
    }

    public static string AddSkinTone(string glyph, int index)
    {
      return Emoji.GetBaseEmoji(glyph, false, false) + Emoji.ToSkinModifierString((Emoji.EmojiSkinModifier) index) + Emoji.ExtractGender(glyph) + Emoji.ExtractOccupation(glyph);
    }

    public static string ExtractModifier(string glyph)
    {
      string glyph1 = glyph;
      string genderFromEnd = Emoji.ExtractGenderFromEnd(glyph1);
      if (genderFromEnd != null)
        glyph1 = glyph1.Substring(0, glyph1.Length - genderFromEnd.Length);
      string occupation = genderFromEnd == null ? Emoji.ExtractOccupation(glyph1) : (string) null;
      if (occupation != null)
        glyph1 = glyph1.Substring(0, glyph1.Length - occupation.Length);
      return (glyph1.Length >= 2 ? Emoji.ExtractSkintone(glyph1) : (string) null) ?? "";
    }

    public static string GetSingleEmojiVariant(string codepoints, int index)
    {
      Emoji.EmojiChar emojiChar = Emoji.Mappings(codepoints);
      return emojiChar != null && emojiChar.emojiType == Emoji.EmojiType.Skintones ? Emoji.AddSkinTone(Emoji.GetBaseEmoji(codepoints), index) : (string) null;
    }

    public static bool IsVariantSelector(string codepoints)
    {
      Emoji.EmojiChar emojiChar = Emoji.Mappings(codepoints);
      return emojiChar != null && emojiChar.emojiType == Emoji.EmojiType.Skintones;
    }

    public static int GetVariationSelectorIndex(Emoji.EmojiChar emojiChar)
    {
      if (emojiChar.emojiType != Emoji.EmojiType.Skintones)
        return 0;
      switch (emojiChar.codepoints.Length)
      {
        case 1:
        case 2:
          return 0;
        default:
          return (int) Emoji.ToSkinModifierEnum(Emoji.ExtractModifier(emojiChar.codepoints));
      }
    }

    private static Regex FlagMacro
    {
      get
      {
        if (Emoji.flagMacro == null)
          Emoji.flagMacro = new Regex("\\*[a-z]{2}\\*");
        return Emoji.flagMacro;
      }
    }

    private static string ConvertMacros(string rawInput)
    {
      return Emoji.FlagMacro.Replace(rawInput, (Func<Match, string>) (m =>
      {
        string countryEmoji = Emoji.ToCountryEmoji(m.Value.Substring(1, 2));
        return Emoji.IsEmoji(countryEmoji) ? countryEmoji : m.Value;
      }));
    }

    public static Regex InvisibleRegex
    {
      get
      {
        if (Emoji.invisibleRegex == null)
          Emoji.invisibleRegex = new Regex("(\u200B+)");
        return Emoji.invisibleRegex;
      }
    }

    public static string ConvertToRichText(string rawInput)
    {
      try
      {
        IList<Match> matchList = Emoji.InvisibleRegex.Matches(rawInput);
        int startIndex = 0;
        StringBuilder stringBuilder1 = (StringBuilder) null;
        StringBuilder stringBuilder2 = (StringBuilder) null;
        foreach (Match match in (IEnumerable<Match>) matchList)
        {
          string str = (string) null;
          if (match.Index == 0)
          {
            Log.p("emoji:", "got zero width space not part of emoji");
          }
          else
          {
            if (stringBuilder2 == null)
              stringBuilder2 = new StringBuilder();
            stringBuilder2.Length = 0;
            Emoji.EmojiChar emojiChar = (Emoji.EmojiChar) null;
            for (int index = match.Index - 1; index >= 0 && stringBuilder2.Length < 16; --index)
            {
              stringBuilder2.Insert(0, rawInput[index]);
              str = stringBuilder2.ToString();
              emojiChar = Emoji.Mappings(str);
              if (emojiChar != null && index > 0)
              {
                if (rawInput[--index] == '\u200D')
                  stringBuilder2.Insert(0, rawInput[index]);
                else
                  break;
              }
            }
            if (emojiChar != null)
            {
              if (str == "\uD83D\uDC91" || str == "\uD83D\uDC8F" || str == "\uD83D\uDC6A" || emojiChar.emojiType == Emoji.EmojiType.Skintones)
              {
                if (stringBuilder1 == null)
                  stringBuilder1 = new StringBuilder();
                if (match.Index - startIndex - str.Length > 0)
                  stringBuilder1.Append(rawInput, startIndex, match.Index - startIndex - str.Length);
                startIndex = match.Index + match.Length;
                stringBuilder1.Append(Emoji.GetCodepointsFromInputTextBox(str, match.Length));
              }
            }
            else
              Log.p("emoji:", "got zero width space not part of emoji - {0}", (object) str);
          }
        }
        if (stringBuilder1 == null)
          return rawInput;
        if (rawInput.Length - startIndex > 0)
          stringBuilder1.Append(rawInput, startIndex, rawInput.Length - startIndex);
        return stringBuilder1.ToString();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "parsing text back to original");
        return rawInput;
      }
    }

    private static Regex LongEmojiRegex
    {
      get
      {
        if (Emoji.longEmojiRegex == null)
          Emoji.longEmojiRegex = new Regex(((IEnumerable<string>) ((IEnumerable<string>) new string[313]
          {
            "\uD83D\uDE4B\uD83C\uDFFB",
            "\uD83D\uDE4B\uD83C\uDFFC",
            "\uD83D\uDE4B\uD83C\uDFFD",
            "\uD83D\uDE4B\uD83C\uDFFE",
            "\uD83D\uDE4B\uD83C\uDFFF",
            "\uD83D\uDC4C\uD83C\uDFFB",
            "\uD83D\uDC4C\uD83C\uDFFC",
            "\uD83D\uDC4C\uD83C\uDFFD",
            "\uD83D\uDC4C\uD83C\uDFFE",
            "\uD83D\uDC4C\uD83C\uDFFF",
            "\uD83D\uDE4D\uD83C\uDFFB",
            "\uD83D\uDE4D\uD83C\uDFFC",
            "\uD83D\uDE4D\uD83C\uDFFD",
            "\uD83D\uDE4D\uD83C\uDFFE",
            "\uD83D\uDE4D\uD83C\uDFFF",
            "\uD83D\uDC43\uD83C\uDFFB",
            "\uD83D\uDC43\uD83C\uDFFC",
            "\uD83D\uDC43\uD83C\uDFFD",
            "\uD83D\uDC43\uD83C\uDFFE",
            "\uD83D\uDC43\uD83C\uDFFF",
            "\uD83D\uDC68\uD83C\uDFFB",
            "\uD83D\uDC68\uD83C\uDFFC",
            "\uD83D\uDC68\uD83C\uDFFD",
            "\uD83D\uDC68\uD83C\uDFFE",
            "\uD83D\uDC68\uD83C\uDFFF",
            "✌\uD83C\uDFFB",
            "✌\uD83C\uDFFC",
            "✌\uD83C\uDFFD",
            "✌\uD83C\uDFFE",
            "✌\uD83C\uDFFF",
            "\uD83D\uDE4C\uD83C\uDFFB",
            "\uD83D\uDE4C\uD83C\uDFFC",
            "\uD83D\uDE4C\uD83C\uDFFD",
            "\uD83D\uDE4C\uD83C\uDFFE",
            "\uD83D\uDE4C\uD83C\uDFFF",
            "\uD83C\uDFC3\uD83C\uDFFB",
            "\uD83C\uDFC3\uD83C\uDFFC",
            "\uD83C\uDFC3\uD83C\uDFFD",
            "\uD83C\uDFC3\uD83C\uDFFE",
            "\uD83C\uDFC3\uD83C\uDFFF",
            "\uD83D\uDEC0\uD83C\uDFFB",
            "\uD83D\uDEC0\uD83C\uDFFC",
            "\uD83D\uDEC0\uD83C\uDFFD",
            "\uD83D\uDEC0\uD83C\uDFFE",
            "\uD83D\uDEC0\uD83C\uDFFF",
            "\uD83D\uDEB6\uD83C\uDFFB",
            "\uD83D\uDEB6\uD83C\uDFFC",
            "\uD83D\uDEB6\uD83C\uDFFD",
            "\uD83D\uDEB6\uD83C\uDFFE",
            "\uD83D\uDEB6\uD83C\uDFFF",
            "\uD83D\uDC4A\uD83C\uDFFB",
            "\uD83D\uDC4A\uD83C\uDFFC",
            "\uD83D\uDC4A\uD83C\uDFFD",
            "\uD83D\uDC4A\uD83C\uDFFE",
            "\uD83D\uDC4A\uD83C\uDFFF",
            "\uD83D\uDE4F\uD83C\uDFFB",
            "\uD83D\uDE4F\uD83C\uDFFC",
            "\uD83D\uDE4F\uD83C\uDFFD",
            "\uD83D\uDE4F\uD83C\uDFFE",
            "\uD83D\uDE4F\uD83C\uDFFF",
            "✊\uD83C\uDFFB",
            "✊\uD83C\uDFFC",
            "✊\uD83C\uDFFD",
            "✊\uD83C\uDFFE",
            "✊\uD83C\uDFFF",
            "\uD83D\uDC49\uD83C\uDFFB",
            "\uD83D\uDC49\uD83C\uDFFC",
            "\uD83D\uDC49\uD83C\uDFFD",
            "\uD83D\uDC49\uD83C\uDFFE",
            "\uD83D\uDC49\uD83C\uDFFF",
            "\uD83D\uDC7C\uD83C\uDFFB",
            "\uD83D\uDC7C\uD83C\uDFFC",
            "\uD83D\uDC7C\uD83C\uDFFD",
            "\uD83D\uDC7C\uD83C\uDFFE",
            "\uD83D\uDC7C\uD83C\uDFFF",
            "\uD83D\uDC85\uD83C\uDFFB",
            "\uD83D\uDC85\uD83C\uDFFC",
            "\uD83D\uDC85\uD83C\uDFFD",
            "\uD83D\uDC85\uD83C\uDFFE",
            "\uD83D\uDC85\uD83C\uDFFF",
            "\uD83D\uDC73\uD83C\uDFFB",
            "\uD83D\uDC73\uD83C\uDFFC",
            "\uD83D\uDC73\uD83C\uDFFD",
            "\uD83D\uDC73\uD83C\uDFFE",
            "\uD83D\uDC73\uD83C\uDFFF",
            "\uD83D\uDC75\uD83C\uDFFB",
            "\uD83D\uDC75\uD83C\uDFFC",
            "\uD83D\uDC75\uD83C\uDFFD",
            "\uD83D\uDC75\uD83C\uDFFE",
            "\uD83D\uDC75\uD83C\uDFFF",
            "☝\uD83C\uDFFB",
            "☝\uD83C\uDFFC",
            "☝\uD83C\uDFFD",
            "☝\uD83C\uDFFE",
            "☝\uD83C\uDFFF",
            "\uD83D\uDE47\uD83C\uDFFB",
            "\uD83D\uDE47\uD83C\uDFFC",
            "\uD83D\uDE47\uD83C\uDFFD",
            "\uD83D\uDE47\uD83C\uDFFE",
            "\uD83D\uDE47\uD83C\uDFFF",
            "\uD83D\uDC4B\uD83C\uDFFB",
            "\uD83D\uDC4B\uD83C\uDFFC",
            "\uD83D\uDC4B\uD83C\uDFFD",
            "\uD83D\uDC4B\uD83C\uDFFE",
            "\uD83D\uDC4B\uD83C\uDFFF",
            "\uD83D\uDC71\uD83C\uDFFB",
            "\uD83D\uDC71\uD83C\uDFFC",
            "\uD83D\uDC71\uD83C\uDFFD",
            "\uD83D\uDC71\uD83C\uDFFE",
            "\uD83D\uDC71\uD83C\uDFFF",
            "\uD83D\uDEA3\uD83C\uDFFB",
            "\uD83D\uDEA3\uD83C\uDFFC",
            "\uD83D\uDEA3\uD83C\uDFFD",
            "\uD83D\uDEA3\uD83C\uDFFE",
            "\uD83D\uDEA3\uD83C\uDFFF",
            "\uD83D\uDC4E\uD83C\uDFFB",
            "\uD83D\uDC4E\uD83C\uDFFC",
            "\uD83D\uDC4E\uD83C\uDFFD",
            "\uD83D\uDC4E\uD83C\uDFFE",
            "\uD83D\uDC4E\uD83C\uDFFF",
            "\uD83D\uDC50\uD83C\uDFFB",
            "\uD83D\uDC50\uD83C\uDFFC",
            "\uD83D\uDC50\uD83C\uDFFD",
            "\uD83D\uDC50\uD83C\uDFFE",
            "\uD83D\uDC50\uD83C\uDFFF",
            "\uD83D\uDEB4\uD83C\uDFFB",
            "\uD83D\uDEB4\uD83C\uDFFC",
            "\uD83D\uDEB4\uD83C\uDFFD",
            "\uD83D\uDEB4\uD83C\uDFFE",
            "\uD83D\uDEB4\uD83C\uDFFF",
            "\uD83D\uDC66\uD83C\uDFFB",
            "\uD83D\uDC66\uD83C\uDFFC",
            "\uD83D\uDC66\uD83C\uDFFD",
            "\uD83D\uDC66\uD83C\uDFFE",
            "\uD83D\uDC66\uD83C\uDFFF",
            "\uD83D\uDC83\uD83C\uDFFB",
            "\uD83D\uDC83\uD83C\uDFFC",
            "\uD83D\uDC83\uD83C\uDFFD",
            "\uD83D\uDC83\uD83C\uDFFE",
            "\uD83D\uDC83\uD83C\uDFFF",
            "\uD83D\uDC87\uD83C\uDFFB",
            "\uD83D\uDC87\uD83C\uDFFC",
            "\uD83D\uDC87\uD83C\uDFFD",
            "\uD83D\uDC87\uD83C\uDFFE",
            "\uD83D\uDC87\uD83C\uDFFF",
            "\uD83D\uDC81\uD83C\uDFFB",
            "\uD83D\uDC81\uD83C\uDFFC",
            "\uD83D\uDC81\uD83C\uDFFD",
            "\uD83D\uDC81\uD83C\uDFFE",
            "\uD83D\uDC81\uD83C\uDFFF",
            "\uD83D\uDE45\uD83C\uDFFB",
            "\uD83D\uDE45\uD83C\uDFFC",
            "\uD83D\uDE45\uD83C\uDFFD",
            "\uD83D\uDE45\uD83C\uDFFE",
            "\uD83D\uDE45\uD83C\uDFFF",
            "✋\uD83C\uDFFB",
            "✋\uD83C\uDFFC",
            "✋\uD83C\uDFFD",
            "✋\uD83C\uDFFE",
            "✋\uD83C\uDFFF",
            "\uD83D\uDC46\uD83C\uDFFB",
            "\uD83D\uDC46\uD83C\uDFFC",
            "\uD83D\uDC46\uD83C\uDFFD",
            "\uD83D\uDC46\uD83C\uDFFE",
            "\uD83D\uDC46\uD83C\uDFFF",
            "\uD83D\uDC86\uD83C\uDFFB",
            "\uD83D\uDC86\uD83C\uDFFC",
            "\uD83D\uDC86\uD83C\uDFFD",
            "\uD83D\uDC86\uD83C\uDFFE",
            "\uD83D\uDC86\uD83C\uDFFF",
            "\uD83D\uDEB5\uD83C\uDFFB",
            "\uD83D\uDEB5\uD83C\uDFFC",
            "\uD83D\uDEB5\uD83C\uDFFD",
            "\uD83D\uDEB5\uD83C\uDFFE",
            "\uD83D\uDEB5\uD83C\uDFFF",
            "\uD83D\uDC82\uD83C\uDFFB",
            "\uD83D\uDC82\uD83C\uDFFC",
            "\uD83D\uDC82\uD83C\uDFFD",
            "\uD83D\uDC82\uD83C\uDFFE",
            "\uD83D\uDC82\uD83C\uDFFF",
            "\uD83D\uDC4F\uD83C\uDFFB",
            "\uD83D\uDC4F\uD83C\uDFFC",
            "\uD83D\uDC4F\uD83C\uDFFD",
            "\uD83D\uDC4F\uD83C\uDFFE",
            "\uD83D\uDC4F\uD83C\uDFFF",
            "\uD83C\uDFC7\uD83C\uDFFB",
            "\uD83C\uDFC7\uD83C\uDFFC",
            "\uD83C\uDFC7\uD83C\uDFFD",
            "\uD83C\uDFC7\uD83C\uDFFE",
            "\uD83C\uDFC7\uD83C\uDFFF",
            "\uD83C\uDFCA\uD83C\uDFFB",
            "\uD83C\uDFCA\uD83C\uDFFC",
            "\uD83C\uDFCA\uD83C\uDFFD",
            "\uD83C\uDFCA\uD83C\uDFFE",
            "\uD83C\uDFCA\uD83C\uDFFF",
            "\uD83D\uDC78\uD83C\uDFFB",
            "\uD83D\uDC78\uD83C\uDFFC",
            "\uD83D\uDC78\uD83C\uDFFD",
            "\uD83D\uDC78\uD83C\uDFFE",
            "\uD83D\uDC78\uD83C\uDFFF",
            "\uD83D\uDE46\uD83C\uDFFB",
            "\uD83D\uDE46\uD83C\uDFFC",
            "\uD83D\uDE46\uD83C\uDFFD",
            "\uD83D\uDE46\uD83C\uDFFE",
            "\uD83D\uDE46\uD83C\uDFFF",
            "\uD83C\uDF85\uD83C\uDFFB",
            "\uD83C\uDF85\uD83C\uDFFC",
            "\uD83C\uDF85\uD83C\uDFFD",
            "\uD83C\uDF85\uD83C\uDFFE",
            "\uD83C\uDF85\uD83C\uDFFF",
            "\uD83D\uDC72\uD83C\uDFFB",
            "\uD83D\uDC72\uD83C\uDFFC",
            "\uD83D\uDC72\uD83C\uDFFD",
            "\uD83D\uDC72\uD83C\uDFFE",
            "\uD83D\uDC72\uD83C\uDFFF",
            "\uD83D\uDC67\uD83C\uDFFB",
            "\uD83D\uDC67\uD83C\uDFFC",
            "\uD83D\uDC67\uD83C\uDFFD",
            "\uD83D\uDC67\uD83C\uDFFE",
            "\uD83D\uDC67\uD83C\uDFFF",
            "\uD83D\uDC70\uD83C\uDFFB",
            "\uD83D\uDC70\uD83C\uDFFC",
            "\uD83D\uDC70\uD83C\uDFFD",
            "\uD83D\uDC70\uD83C\uDFFE",
            "\uD83D\uDC70\uD83C\uDFFF",
            "\uD83D\uDC6E\uD83C\uDFFB",
            "\uD83D\uDC6E\uD83C\uDFFC",
            "\uD83D\uDC6E\uD83C\uDFFD",
            "\uD83D\uDC6E\uD83C\uDFFE",
            "\uD83D\uDC6E\uD83C\uDFFF",
            "\uD83D\uDC76\uD83C\uDFFB",
            "\uD83D\uDC76\uD83C\uDFFC",
            "\uD83D\uDC76\uD83C\uDFFD",
            "\uD83D\uDC76\uD83C\uDFFE",
            "\uD83D\uDC76\uD83C\uDFFF",
            "\uD83D\uDC42\uD83C\uDFFB",
            "\uD83D\uDC42\uD83C\uDFFC",
            "\uD83D\uDC42\uD83C\uDFFD",
            "\uD83D\uDC42\uD83C\uDFFE",
            "\uD83D\uDC42\uD83C\uDFFF",
            "\uD83D\uDC47\uD83C\uDFFB",
            "\uD83D\uDC47\uD83C\uDFFC",
            "\uD83D\uDC47\uD83C\uDFFD",
            "\uD83D\uDC47\uD83C\uDFFE",
            "\uD83D\uDC47\uD83C\uDFFF",
            "\uD83D\uDD95\uD83C\uDFFB",
            "\uD83D\uDD95\uD83C\uDFFC",
            "\uD83D\uDD95\uD83C\uDFFD",
            "\uD83D\uDD95\uD83C\uDFFE",
            "\uD83D\uDD95\uD83C\uDFFF",
            "\uD83D\uDC69\uD83C\uDFFB",
            "\uD83D\uDC69\uD83C\uDFFC",
            "\uD83D\uDC69\uD83C\uDFFD",
            "\uD83D\uDC69\uD83C\uDFFE",
            "\uD83D\uDC69\uD83C\uDFFF",
            "\uD83D\uDE4E\uD83C\uDFFB",
            "\uD83D\uDE4E\uD83C\uDFFC",
            "\uD83D\uDE4E\uD83C\uDFFD",
            "\uD83D\uDE4E\uD83C\uDFFE",
            "\uD83D\uDE4E\uD83C\uDFFF",
            "\uD83D\uDC77\uD83C\uDFFB",
            "\uD83D\uDC77\uD83C\uDFFC",
            "\uD83D\uDC77\uD83C\uDFFD",
            "\uD83D\uDC77\uD83C\uDFFE",
            "\uD83D\uDC77\uD83C\uDFFF",
            "\uD83D\uDD96\uD83C\uDFFB",
            "\uD83D\uDD96\uD83C\uDFFC",
            "\uD83D\uDD96\uD83C\uDFFD",
            "\uD83D\uDD96\uD83C\uDFFE",
            "\uD83D\uDD96\uD83C\uDFFF",
            "\uD83D\uDC4D\uD83C\uDFFB",
            "\uD83D\uDC4D\uD83C\uDFFC",
            "\uD83D\uDC4D\uD83C\uDFFD",
            "\uD83D\uDC4D\uD83C\uDFFE",
            "\uD83D\uDC4D\uD83C\uDFFF",
            "\uD83C\uDFC4\uD83C\uDFFB",
            "\uD83C\uDFC4\uD83C\uDFFC",
            "\uD83C\uDFC4\uD83C\uDFFD",
            "\uD83C\uDFC4\uD83C\uDFFE",
            "\uD83C\uDFC4\uD83C\uDFFF",
            "\uD83D\uDCAA\uD83C\uDFFB",
            "\uD83D\uDCAA\uD83C\uDFFC",
            "\uD83D\uDCAA\uD83C\uDFFD",
            "\uD83D\uDCAA\uD83C\uDFFE",
            "\uD83D\uDCAA\uD83C\uDFFF",
            "\uD83D\uDC48\uD83C\uDFFB",
            "\uD83D\uDC48\uD83C\uDFFC",
            "\uD83D\uDC48\uD83C\uDFFD",
            "\uD83D\uDC48\uD83C\uDFFE",
            "\uD83D\uDC48\uD83C\uDFFF",
            "\uD83D\uDC74\uD83C\uDFFB",
            "\uD83D\uDC74\uD83C\uDFFC",
            "\uD83D\uDC74\uD83C\uDFFD",
            "\uD83D\uDC74\uD83C\uDFFE",
            "\uD83D\uDC74\uD83C\uDFFF",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D❤\u200D\uD83D\uDC8B\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤\u200D\uD83D\uDC8B\u200D\uD83D\uDC69",
            "\uD83D\uDC68\u200D❤\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤\u200D\uD83D\uDC69"
          }).OrderByDescending<string, int>((Func<string, int>) (s => s.Length)).ToArray<string>()).ToRegexString());
        return Emoji.longEmojiRegex;
      }
    }

    public static IEnumerable<LinkDetector.Result> GetLongEmojiMatches(string s)
    {
      foreach (Match match in (IEnumerable<Match>) Emoji.LongEmojiRegex.Matches(s))
        yield return LinkDetector.Convert(match, 2);
    }

    public static string ConvertToTextOnly(
      string rawInput,
      byte[] textPerformanceHintBytes,
      bool convertToUnicode = true)
    {
      if (string.IsNullOrEmpty(rawInput))
      {
        Log.l(nameof (Emoji), "Found nothing to convert to text: {0}", (object) rawInput);
        return "";
      }
      IEnumerable<LinkDetector.Result> textPerformanceHint = (textPerformanceHintBytes != null ? (IEnumerable<LinkDetector.Result>) LinkDetector.Result.Deserialize(rawInput, textPerformanceHintBytes) ?? LinkDetector.GetEmojiMatches(rawInput) : Emoji.GetLongEmojiMatches(rawInput)) ?? (IEnumerable<LinkDetector.Result>) new LinkDetector.Result[0];
      return Emoji.ConvertToTextOnly(rawInput, textPerformanceHint, convertToUnicode);
    }

    public static string ConvertToTextOnly(
      string rawInput,
      IEnumerable<LinkDetector.Result> textPerformanceHint,
      bool convertToUnicode = true)
    {
      if (string.IsNullOrEmpty(rawInput))
        return rawInput;
      if (textPerformanceHint == null)
        return Emoji.ConvertToTextOnly(rawInput, (byte[]) null, convertToUnicode);
      try
      {
        if (convertToUnicode)
          rawInput = Emoji.ConvertToUnicode(rawInput);
        int startIndex = 0;
        StringBuilder stringBuilder = (StringBuilder) null;
        foreach (LinkDetector.Result result in textPerformanceHint)
        {
          if ((result.type & 2) != 0)
          {
            if (result.Index + result.Length <= rawInput.Length)
            {
              Emoji.EmojiChar emojiChar = Emoji.Mappings(result.Value);
              if (emojiChar != null && (emojiChar.emojiType == Emoji.EmojiType.Couples || emojiChar.emojiType == Emoji.EmojiType.Skintones))
              {
                if (stringBuilder == null)
                  stringBuilder = new StringBuilder();
                if (result.Index - startIndex > 0)
                  stringBuilder.Append(rawInput, startIndex, result.Index - startIndex);
                stringBuilder.Append(Emoji.GetCodepointsForInputTextBox(emojiChar));
                startIndex = result.Index + result.Length;
              }
            }
            else
              break;
          }
        }
        if (stringBuilder == null)
          return rawInput;
        if (rawInput.Length - startIndex > 0)
          stringBuilder.Append(rawInput, startIndex, rawInput.Length - startIndex);
        return stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "to text only display");
        return rawInput;
      }
    }

    public static string GetCodepointsForInputTextBox(Emoji.EmojiChar emojiChar)
    {
      if (emojiChar.emojiType == Emoji.EmojiType.Couples)
      {
        string str = Emoji.CoupleOffset[emojiChar.codepoints];
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(str, 0, 2);
        int num = (int) str[2];
        for (int index = 0; index < num; ++index)
          stringBuilder.Append('\u200B');
        return stringBuilder.ToString();
      }
      if (emojiChar.emojiType != Emoji.EmojiType.Skintones || emojiChar.codepoints.Length == 1 || emojiChar.codepoints.Length == 2)
        return emojiChar.codepoints;
      Emoji.EmojiSkinModifier skinModifierEnum = Emoji.ToSkinModifierEnum(Emoji.ExtractModifier(emojiChar.codepoints));
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append(Emoji.GetBaseEmoji(emojiChar.codepoints));
      for (int index = 0; (Emoji.EmojiSkinModifier) index < skinModifierEnum; ++index)
        stringBuilder1.Append('\u200B');
      return stringBuilder1.ToString();
    }

    public static string GetCodepointsFromInputTextBox(string baseUnicode, int modifierLength)
    {
      Emoji.EmojiChar emojiChar = Emoji.Mappings(baseUnicode);
      if (emojiChar != null)
      {
        if (baseUnicode == "\uD83D\uDC91" || baseUnicode == "\uD83D\uDC8F" || baseUnicode == "\uD83D\uDC6A")
        {
          if (baseUnicode == "\uD83D\uDC91" && modifierLength > 2)
            Log.p("emoji", "\\U001f46a emoji saw a modifierLength greater than 2");
          if (baseUnicode == "\uD83D\uDC8F" && modifierLength > 2)
            Log.p("emoji", "\\U001f46a emoji saw a modifierLength greater than 2");
          if (baseUnicode == "\uD83D\uDC6A" && modifierLength > 14)
            Log.p("emoji", "\\U001f46a emoji saw a modifierLength greater than 14");
          string str;
          return Emoji.ReverseCoupleOffset.TryGetValue(baseUnicode + ((char) modifierLength).ToString(), out str) ? str : baseUnicode;
        }
        if (emojiChar.emojiType == Emoji.EmojiType.Skintones)
          return Emoji.AddSkinTone(baseUnicode, modifierLength);
      }
      return baseUnicode;
    }

    public static Dictionary<string, string> CoupleOffset
    {
      get
      {
        if (Emoji.coupleOffset == null)
        {
          string[] strArray1 = new string[14]
          {
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC68\u200D\uD83D\uDC68\u200D\uD83D\uDC67\u200D\uD83D\uDC67",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC66\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC66",
            "\uD83D\uDC69\u200D\uD83D\uDC69\u200D\uD83D\uDC67\u200D\uD83D\uDC67"
          };
          string[] strArray2 = new string[2]
          {
            "\uD83D\uDC68\u200D❤️\u200D\uD83D\uDC8B\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤️\u200D\uD83D\uDC8B\u200D\uD83D\uDC69"
          };
          string[] strArray3 = new string[2]
          {
            "\uD83D\uDC68\u200D❤️\u200D\uD83D\uDC68",
            "\uD83D\uDC69\u200D❤️\u200D\uD83D\uDC69"
          };
          Emoji.coupleOffset = new Dictionary<string, string>();
          for (int index = 0; index < strArray1.Length; ++index)
            Emoji.coupleOffset.Add(strArray1[index], "\uD83D\uDC6A" + ((char) (index + 1)).ToString());
          for (int index = 0; index < strArray2.Length; ++index)
            Emoji.coupleOffset.Add(strArray2[index], "\uD83D\uDC8F" + ((char) (index + 1)).ToString());
          for (int index = 0; index < strArray3.Length; ++index)
            Emoji.coupleOffset.Add(strArray3[index], "\uD83D\uDC91" + ((char) (index + 1)).ToString());
        }
        return Emoji.coupleOffset;
      }
    }

    public static Dictionary<string, string> ReverseCoupleOffset
    {
      get
      {
        if (Emoji.reverseCoupleOffset == null)
        {
          Emoji.reverseCoupleOffset = new Dictionary<string, string>();
          foreach (KeyValuePair<string, string> keyValuePair in Emoji.CoupleOffset)
            Emoji.reverseCoupleOffset.Add(keyValuePair.Value, keyValuePair.Key);
        }
        return Emoji.reverseCoupleOffset;
      }
    }

    private static void PrintChar(char c)
    {
      string str = ((int) c).ToString("X8");
      Log.l("    emojis", c.ToString() + "  " + str);
    }

    public static void PrintCodePoint(string s)
    {
    }

    public static void LogCodepoint(string context, string codepoint)
    {
      char[] charArray1 = codepoint.ToCharArray();
      string str = "";
      foreach (char ch in charArray1)
        str = str + ((int) ch).ToString("X5") + ", ";
      char[] charArray2 = codepoint.ToCharArray();
      for (int index = 0; index < codepoint.Length - 1; index += 2)
      {
        uint utf32 = UTF32.ToUtf32(charArray2[index], charArray2[index + 1]);
        str = str + utf32.ToString("X5") + ".";
      }
      Log.d("Codepoint", context + " codepoint {0}, chars: {1}", (object) codepoint, (object) str);
    }

    public class EmojiChar
    {
      public int index;
      public Emoji.EmojiType emojiType;
      public string codepoints;
      private static string[] imagePaths;

      private static string[] ImagePaths
      {
        get
        {
          if (Emoji.EmojiChar.imagePaths == null)
          {
            int length = (int) Math.Ceiling(24.03);
            Emoji.EmojiChar.imagePaths = new string[length];
            for (int index = 0; index < length; ++index)
              Emoji.EmojiChar.imagePaths[index] = string.Format("/Images/emojis/fullset/fullset-{0}.-{1}.png", (object) index.ToString("D2"), (object) 64);
          }
          return Emoji.EmojiChar.imagePaths;
        }
      }

      public string GetImagePath()
      {
        int num = this.index - 1;
        return num < 0 || num >= 2403 ? (string) null : Emoji.EmojiChar.ImagePaths[num / 100];
      }

      public int GetImageIndex() => (this.index - 1) % 100;

      public IObservable<Emoji.EmojiChar.Args> Image
      {
        get
        {
          return Observable.Create<Emoji.EmojiChar.Args>((Func<IObserver<Emoji.EmojiChar.Args>, Action>) (observer =>
          {
            int imageIndex = this.GetImageIndex();
            observer.OnNext(new Emoji.EmojiChar.Args()
            {
              EmojiChar = this,
              BaseImage = (BitmapSource) ImageStore.GetStockIcon(this.GetImagePath()),
              X = (double) (imageIndex % 10 * 65 + 1),
              Y = (double) (imageIndex / 10 * 65 + 1),
              Width = 64.0,
              Height = 64.0
            });
            return (Action) (() => { });
          }));
        }
      }

      public class Args
      {
        public Emoji.EmojiChar EmojiChar;
        public BitmapSource BaseImage;
        public double X;
        public double Y;
        public double Width;
        public double Height;
      }
    }

    public enum PickerCategory
    {
      Recent,
      SmileysAndPeople,
      AnimalsAndNature,
      FoodAndDrink,
      Activity,
      TravelAndPlaces,
      Objects,
      Symbols,
      Flags,
      None,
    }

    public enum EmojiType
    {
      Regular,
      Skintones,
      Couples,
    }

    public enum EmojiSkinModifier
    {
      None,
      Type_12,
      Type_3,
      Type_4,
      Type_5,
      Type_6,
    }
  }
}
