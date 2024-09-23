// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.Decoders.GeneralAppIdDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS.Expanded.Decoders
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  internal sealed class GeneralAppIdDecoder
  {
    private BitArray information;
    private CurrentParsingState current = new CurrentParsingState();
    private StringBuilder buffer = new StringBuilder();

    internal GeneralAppIdDecoder(BitArray information) => this.information = information;

    internal string decodeAllCodes(StringBuilder buff, int initialPosition)
    {
      int pos = initialPosition;
      string remaining = (string) null;
      while (true)
      {
        DecodedInformation decodedInformation = this.decodeGeneralPurposeField(pos, remaining);
        string inGeneralPurpose = FieldParser.parseFieldsInGeneralPurpose(decodedInformation.getNewString());
        if (inGeneralPurpose != null)
          buff.Append(inGeneralPurpose);
        remaining = !decodedInformation.isRemaining() ? (string) null : decodedInformation.getRemainingValue().ToString();
        if (pos != decodedInformation.NewPosition)
          pos = decodedInformation.NewPosition;
        else
          break;
      }
      return buff.ToString();
    }

    private bool isStillNumeric(int pos)
    {
      if (pos + 7 > this.information.Size)
        return pos + 4 <= this.information.Size;
      for (int i = pos; i < pos + 3; ++i)
      {
        if (this.information[i])
          return true;
      }
      return this.information[pos + 3];
    }

    private DecodedNumeric decodeNumeric(int pos)
    {
      if (pos + 7 > this.information.Size)
      {
        int valueFromBitArray = this.extractNumericValueFromBitArray(pos, 4);
        return valueFromBitArray == 0 ? new DecodedNumeric(this.information.Size, DecodedNumeric.FNC1, DecodedNumeric.FNC1) : new DecodedNumeric(this.information.Size, valueFromBitArray - 1, DecodedNumeric.FNC1);
      }
      int valueFromBitArray1 = this.extractNumericValueFromBitArray(pos, 7);
      int firstDigit = (valueFromBitArray1 - 8) / 11;
      int secondDigit = (valueFromBitArray1 - 8) % 11;
      return new DecodedNumeric(pos + 7, firstDigit, secondDigit);
    }

    internal int extractNumericValueFromBitArray(int pos, int bits)
    {
      return GeneralAppIdDecoder.extractNumericValueFromBitArray(this.information, pos, bits);
    }

    internal static int extractNumericValueFromBitArray(BitArray information, int pos, int bits)
    {
      int valueFromBitArray = 0;
      for (int index = 0; index < bits; ++index)
      {
        if (information[pos + index])
          valueFromBitArray |= 1 << bits - index - 1;
      }
      return valueFromBitArray;
    }

    internal DecodedInformation decodeGeneralPurposeField(int pos, string remaining)
    {
      this.buffer.Length = 0;
      if (remaining != null)
        this.buffer.Append(remaining);
      this.current.setPosition(pos);
      DecodedInformation blocks = this.parseBlocks();
      return blocks != null && blocks.isRemaining() ? new DecodedInformation(this.current.getPosition(), this.buffer.ToString(), blocks.getRemainingValue()) : new DecodedInformation(this.current.getPosition(), this.buffer.ToString());
    }

    private DecodedInformation parseBlocks()
    {
      int position;
      BlockParsedResult blockParsedResult;
      bool flag;
      do
      {
        position = this.current.getPosition();
        if (this.current.isAlpha())
        {
          blockParsedResult = this.parseAlphaBlock();
          flag = blockParsedResult.isFinished();
        }
        else if (this.current.isIsoIec646())
        {
          blockParsedResult = this.parseIsoIec646Block();
          flag = blockParsedResult.isFinished();
        }
        else
        {
          blockParsedResult = this.parseNumericBlock();
          flag = blockParsedResult.isFinished();
        }
      }
      while ((position != this.current.getPosition() || flag) && !flag);
      return blockParsedResult.getDecodedInformation();
    }

    private BlockParsedResult parseNumericBlock()
    {
      while (this.isStillNumeric(this.current.getPosition()))
      {
        DecodedNumeric decodedNumeric = this.decodeNumeric(this.current.getPosition());
        this.current.setPosition(decodedNumeric.NewPosition);
        if (decodedNumeric.isFirstDigitFNC1())
          return new BlockParsedResult(!decodedNumeric.isSecondDigitFNC1() ? new DecodedInformation(this.current.getPosition(), this.buffer.ToString(), decodedNumeric.getSecondDigit()) : new DecodedInformation(this.current.getPosition(), this.buffer.ToString()), true);
        this.buffer.Append(decodedNumeric.getFirstDigit());
        if (decodedNumeric.isSecondDigitFNC1())
          return new BlockParsedResult(new DecodedInformation(this.current.getPosition(), this.buffer.ToString()), true);
        this.buffer.Append(decodedNumeric.getSecondDigit());
      }
      if (this.isNumericToAlphaNumericLatch(this.current.getPosition()))
      {
        this.current.setAlpha();
        this.current.incrementPosition(4);
      }
      return new BlockParsedResult(false);
    }

    private BlockParsedResult parseIsoIec646Block()
    {
      while (this.isStillIsoIec646(this.current.getPosition()))
      {
        DecodedChar decodedChar = this.decodeIsoIec646(this.current.getPosition());
        this.current.setPosition(decodedChar.NewPosition);
        if (decodedChar.isFNC1())
          return new BlockParsedResult(new DecodedInformation(this.current.getPosition(), this.buffer.ToString()), true);
        this.buffer.Append(decodedChar.getValue());
      }
      if (this.isAlphaOr646ToNumericLatch(this.current.getPosition()))
      {
        this.current.incrementPosition(3);
        this.current.setNumeric();
      }
      else if (this.isAlphaTo646ToAlphaLatch(this.current.getPosition()))
      {
        if (this.current.getPosition() + 5 < this.information.Size)
          this.current.incrementPosition(5);
        else
          this.current.setPosition(this.information.Size);
        this.current.setAlpha();
      }
      return new BlockParsedResult(false);
    }

    private BlockParsedResult parseAlphaBlock()
    {
      while (this.isStillAlpha(this.current.getPosition()))
      {
        DecodedChar decodedChar = this.decodeAlphanumeric(this.current.getPosition());
        this.current.setPosition(decodedChar.NewPosition);
        if (decodedChar.isFNC1())
          return new BlockParsedResult(new DecodedInformation(this.current.getPosition(), this.buffer.ToString()), true);
        this.buffer.Append(decodedChar.getValue());
      }
      if (this.isAlphaOr646ToNumericLatch(this.current.getPosition()))
      {
        this.current.incrementPosition(3);
        this.current.setNumeric();
      }
      else if (this.isAlphaTo646ToAlphaLatch(this.current.getPosition()))
      {
        if (this.current.getPosition() + 5 < this.information.Size)
          this.current.incrementPosition(5);
        else
          this.current.setPosition(this.information.Size);
        this.current.setIsoIec646();
      }
      return new BlockParsedResult(false);
    }

    private bool isStillIsoIec646(int pos)
    {
      if (pos + 5 > this.information.Size)
        return false;
      int valueFromBitArray1 = this.extractNumericValueFromBitArray(pos, 5);
      if (valueFromBitArray1 >= 5 && valueFromBitArray1 < 16)
        return true;
      if (pos + 7 > this.information.Size)
        return false;
      int valueFromBitArray2 = this.extractNumericValueFromBitArray(pos, 7);
      if (valueFromBitArray2 >= 64 && valueFromBitArray2 < 116)
        return true;
      if (pos + 8 > this.information.Size)
        return false;
      int valueFromBitArray3 = this.extractNumericValueFromBitArray(pos, 8);
      return valueFromBitArray3 >= 232 && valueFromBitArray3 < 253;
    }

    private DecodedChar decodeIsoIec646(int pos)
    {
      int valueFromBitArray1 = this.extractNumericValueFromBitArray(pos, 5);
      if (valueFromBitArray1 == 15)
        return new DecodedChar(pos + 5, DecodedChar.FNC1);
      if (valueFromBitArray1 >= 5 && valueFromBitArray1 < 15)
        return new DecodedChar(pos + 5, (char) (48 + valueFromBitArray1 - 5));
      int valueFromBitArray2 = this.extractNumericValueFromBitArray(pos, 7);
      if (valueFromBitArray2 >= 64 && valueFromBitArray2 < 90)
        return new DecodedChar(pos + 7, (char) (valueFromBitArray2 + 1));
      if (valueFromBitArray2 >= 90 && valueFromBitArray2 < 116)
        return new DecodedChar(pos + 7, (char) (valueFromBitArray2 + 7));
      int valueFromBitArray3 = this.extractNumericValueFromBitArray(pos, 8);
      char ch;
      switch (valueFromBitArray3)
      {
        case 232:
          ch = '!';
          break;
        case 233:
          ch = '"';
          break;
        case 234:
          ch = '%';
          break;
        case 235:
          ch = '&';
          break;
        case 236:
          ch = '\'';
          break;
        case 237:
          ch = '(';
          break;
        case 238:
          ch = ')';
          break;
        case 239:
          ch = '*';
          break;
        case 240:
          ch = '+';
          break;
        case 241:
          ch = ',';
          break;
        case 242:
          ch = '-';
          break;
        case 243:
          ch = '.';
          break;
        case 244:
          ch = '/';
          break;
        case 245:
          ch = ':';
          break;
        case 246:
          ch = ';';
          break;
        case 247:
          ch = '<';
          break;
        case 248:
          ch = '=';
          break;
        case 249:
          ch = '>';
          break;
        case 250:
          ch = '?';
          break;
        case 251:
          ch = '_';
          break;
        case 252:
          ch = ' ';
          break;
        default:
          throw new ArgumentException("Decoding invalid ISO/IEC 646 value: " + (object) valueFromBitArray3);
      }
      return new DecodedChar(pos + 8, ch);
    }

    private bool isStillAlpha(int pos)
    {
      if (pos + 5 > this.information.Size)
        return false;
      int valueFromBitArray1 = this.extractNumericValueFromBitArray(pos, 5);
      if (valueFromBitArray1 >= 5 && valueFromBitArray1 < 16)
        return true;
      if (pos + 6 > this.information.Size)
        return false;
      int valueFromBitArray2 = this.extractNumericValueFromBitArray(pos, 6);
      return valueFromBitArray2 >= 16 && valueFromBitArray2 < 63;
    }

    private DecodedChar decodeAlphanumeric(int pos)
    {
      int valueFromBitArray1 = this.extractNumericValueFromBitArray(pos, 5);
      if (valueFromBitArray1 == 15)
        return new DecodedChar(pos + 5, DecodedChar.FNC1);
      if (valueFromBitArray1 >= 5 && valueFromBitArray1 < 15)
        return new DecodedChar(pos + 5, (char) (48 + valueFromBitArray1 - 5));
      int valueFromBitArray2 = this.extractNumericValueFromBitArray(pos, 6);
      if (valueFromBitArray2 >= 32 && valueFromBitArray2 < 58)
        return new DecodedChar(pos + 6, (char) (valueFromBitArray2 + 33));
      char ch;
      switch (valueFromBitArray2)
      {
        case 58:
          ch = '*';
          break;
        case 59:
          ch = ',';
          break;
        case 60:
          ch = '-';
          break;
        case 61:
          ch = '.';
          break;
        case 62:
          ch = '/';
          break;
        default:
          throw new InvalidOperationException("Decoding invalid alphanumeric value: " + (object) valueFromBitArray2);
      }
      return new DecodedChar(pos + 6, ch);
    }

    private bool isAlphaTo646ToAlphaLatch(int pos)
    {
      if (pos + 1 > this.information.Size)
        return false;
      for (int index = 0; index < 5 && index + pos < this.information.Size; ++index)
      {
        if (index == 2)
        {
          if (!this.information[pos + 2])
            return false;
        }
        else if (this.information[pos + index])
          return false;
      }
      return true;
    }

    private bool isAlphaOr646ToNumericLatch(int pos)
    {
      if (pos + 3 > this.information.Size)
        return false;
      for (int i = pos; i < pos + 3; ++i)
      {
        if (this.information[i])
          return false;
      }
      return true;
    }

    private bool isNumericToAlphaNumericLatch(int pos)
    {
      if (pos + 1 > this.information.Size)
        return false;
      for (int index = 0; index < 4 && index + pos < this.information.Size; ++index)
      {
        if (this.information[pos + index])
          return false;
      }
      return true;
    }
  }
}
