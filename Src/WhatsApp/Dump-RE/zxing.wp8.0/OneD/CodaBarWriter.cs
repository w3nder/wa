// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.CodaBarWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// This class renders CodaBar as <see cref="T:System.Boolean" />[].
  /// </summary>
  /// <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
  public sealed class CodaBarWriter : OneDimensionalCodeWriter
  {
    private static readonly char[] START_END_CHARS = new char[4]
    {
      'A',
      'B',
      'C',
      'D'
    };
    private static readonly char[] ALT_START_END_CHARS = new char[4]
    {
      'T',
      'N',
      '*',
      'E'
    };
    private static readonly char[] CHARS_WHICH_ARE_TEN_LENGTH_EACH_AFTER_DECODED = new char[4]
    {
      '/',
      ':',
      '+',
      '.'
    };

    public override bool[] encode(string contents)
    {
      char key = contents.Length >= 2 ? char.ToUpper(contents[0]) : throw new ArgumentException("Codabar should start/end with start/stop symbols");
      char upper = char.ToUpper(contents[contents.Length - 1]);
      bool flag1 = CodaBarReader.arrayContains(CodaBarWriter.START_END_CHARS, key) && CodaBarReader.arrayContains(CodaBarWriter.START_END_CHARS, upper);
      bool flag2 = CodaBarReader.arrayContains(CodaBarWriter.ALT_START_END_CHARS, key) && CodaBarReader.arrayContains(CodaBarWriter.ALT_START_END_CHARS, upper);
      if (!flag1 && !flag2)
        throw new ArgumentException("Codabar should start/end with " + SupportClass.Join<char>(", ", (IEnumerable<char>) CodaBarWriter.START_END_CHARS) + ", or start/end with " + SupportClass.Join<char>(", ", (IEnumerable<char>) CodaBarWriter.ALT_START_END_CHARS));
      int num1 = 20;
      for (int index = 1; index < contents.Length - 1; ++index)
      {
        if (char.IsDigit(contents[index]) || contents[index] == '-' || contents[index] == '$')
        {
          num1 += 9;
        }
        else
        {
          if (!CodaBarReader.arrayContains(CodaBarWriter.CHARS_WHICH_ARE_TEN_LENGTH_EACH_AFTER_DECODED, contents[index]))
            throw new ArgumentException("Cannot encode : '" + (object) contents[index] + (object) '\'');
          num1 += 10;
        }
      }
      bool[] flagArray = new bool[num1 + (contents.Length - 1)];
      int index1 = 0;
      for (int index2 = 0; index2 < contents.Length; ++index2)
      {
        char ch = char.ToUpper(contents[index2]);
        if (index2 == 0 || index2 == contents.Length - 1)
        {
          switch (ch)
          {
            case '*':
              ch = 'C';
              break;
            case 'E':
              ch = 'D';
              break;
            case 'N':
              ch = 'B';
              break;
            case 'T':
              ch = 'A';
              break;
          }
        }
        int num2 = 0;
        for (int index3 = 0; index3 < CodaBarReader.ALPHABET.Length; ++index3)
        {
          if ((int) ch == (int) CodaBarReader.ALPHABET[index3])
          {
            num2 = CodaBarReader.CHARACTER_ENCODINGS[index3];
            break;
          }
        }
        bool flag3 = true;
        int num3 = 0;
        int num4 = 0;
        while (num4 < 7)
        {
          flagArray[index1] = flag3;
          ++index1;
          if ((num2 >> 6 - num4 & 1) == 0 || num3 == 1)
          {
            flag3 = !flag3;
            ++num4;
            num3 = 0;
          }
          else
            ++num3;
        }
        if (index2 < contents.Length - 1)
        {
          flagArray[index1] = false;
          ++index1;
        }
      }
      return flagArray;
    }
  }
}
