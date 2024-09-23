// Decompiled with JetBrains decompiler
// Type: WhatsApp.CharRange
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class CharRange
  {
    private CharRange.Range[] ranges;

    public CharRange(ushort[] buffer)
    {
      this.ranges = new CharRange.Range[buffer.Length / 2];
      int index1 = 0;
      int index2 = 0;
      while (index1 < this.ranges.Length)
      {
        this.ranges[index1].First = (char) buffer[index2];
        this.ranges[index1].Last = (char) buffer[index2 + 1];
        ++index1;
        index2 += 2;
      }
    }

    public bool Contains(char ch)
    {
      return this.ranges.BinarySearch<CharRange.Range, char>(ch, (Func<char, CharRange.Range, int>) ((key, range) =>
      {
        if ((int) range.First <= (int) key && (int) key <= (int) range.Last)
          return 0;
        return (int) key < (int) range.First ? -1 : 1;
      })) >= 0;
    }

    private struct Range
    {
      public char First;
      public char Last;
    }
  }
}
