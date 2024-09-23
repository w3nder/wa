// Decompiled with JetBrains decompiler
// Type: WhatsApp.MpnsTileExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public static class MpnsTileExtensions
  {
    public static void SetWideContent(
      this IconicTileData data,
      IEnumerable<string> contentEnumerable)
    {
      List<string> list = contentEnumerable.Select<string, string>((Func<string, string>) (s =>
      {
        s = s != null ? Emoji.ConvertToTextOnly(s, (byte[]) null).Replace('\n', ' ').EscapeForTile() : "";
        return s;
      })).ToList<string>();
      list.Add("");
      list.Add("");
      list.Add("");
      int num1 = 0;
      IconicTileData iconicTileData1 = data;
      List<string> stringList1 = list;
      int index1 = num1;
      int num2 = index1 + 1;
      string str1 = stringList1[index1];
      iconicTileData1.WideContent1 = str1;
      IconicTileData iconicTileData2 = data;
      List<string> stringList2 = list;
      int index2 = num2;
      int num3 = index2 + 1;
      string str2 = stringList2[index2];
      iconicTileData2.WideContent2 = str2;
      IconicTileData iconicTileData3 = data;
      List<string> stringList3 = list;
      int index3 = num3;
      int num4 = index3 + 1;
      string str3 = stringList3[index3];
      iconicTileData3.WideContent3 = str3;
    }

    public static string EscapeForTile(this string s)
    {
      if (s.Length != 0 && s[0] == '@')
        s = "\u200B" + s;
      return s;
    }
  }
}
