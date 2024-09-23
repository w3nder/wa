// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.Inflater
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal class Inflater
  {
    private static readonly byte[] extraLengthBits = new byte[29]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 0
    };
    private static readonly int[] lengthBase = new int[29]
    {
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      10,
      11,
      13,
      15,
      17,
      19,
      23,
      27,
      31,
      35,
      43,
      51,
      59,
      67,
      83,
      99,
      115,
      131,
      163,
      195,
      227,
      258
    };
    private static readonly int[] distanceBasePosition = new int[32]
    {
      1,
      2,
      3,
      4,
      5,
      7,
      9,
      13,
      17,
      25,
      33,
      49,
      65,
      97,
      129,
      193,
      257,
      385,
      513,
      769,
      1025,
      1537,
      2049,
      3073,
      4097,
      6145,
      8193,
      12289,
      16385,
      24577,
      0,
      0
    };
    private static readonly byte[] codeOrder = new byte[19]
    {
      (byte) 16,
      (byte) 17,
      (byte) 18,
      (byte) 0,
      (byte) 8,
      (byte) 7,
      (byte) 9,
      (byte) 6,
      (byte) 10,
      (byte) 5,
      (byte) 11,
      (byte) 4,
      (byte) 12,
      (byte) 3,
      (byte) 13,
      (byte) 2,
      (byte) 14,
      (byte) 1,
      (byte) 15
    };
    private static readonly byte[] staticDistanceTreeTable = new byte[32]
    {
      (byte) 0,
      (byte) 16,
      (byte) 8,
      (byte) 24,
      (byte) 4,
      (byte) 20,
      (byte) 12,
      (byte) 28,
      (byte) 2,
      (byte) 18,
      (byte) 10,
      (byte) 26,
      (byte) 6,
      (byte) 22,
      (byte) 14,
      (byte) 30,
      (byte) 1,
      (byte) 17,
      (byte) 9,
      (byte) 25,
      (byte) 5,
      (byte) 21,
      (byte) 13,
      (byte) 29,
      (byte) 3,
      (byte) 19,
      (byte) 11,
      (byte) 27,
      (byte) 7,
      (byte) 23,
      (byte) 15,
      (byte) 31
    };
    private OutputWindow output;
    private InputBuffer input;
    private HuffmanTree literalLengthTree;
    private HuffmanTree distanceTree;
    private InflaterState state;
    private bool hasFormatReader;
    private int bfinal;
    private BlockType blockType;
    private byte[] blockLengthBuffer = new byte[4];
    private int blockLength;
    private int length;
    private int distanceCode;
    private int extraBits;
    private int loopCounter;
    private int literalLengthCodeCount;
    private int distanceCodeCount;
    private int codeLengthCodeCount;
    private int codeArraySize;
    private int lengthCode;
    private byte[] codeList;
    private byte[] codeLengthTreeCodeLength;
    private HuffmanTree codeLengthTree;
    private IFileFormatReader formatReader;

    public Inflater()
    {
      this.output = new OutputWindow();
      this.input = new InputBuffer();
      this.codeList = new byte[320];
      this.codeLengthTreeCodeLength = new byte[19];
      this.Reset();
    }

    internal void SetFileFormatReader(IFileFormatReader reader)
    {
      this.formatReader = reader;
      this.hasFormatReader = true;
      this.Reset();
    }

    private void Reset()
    {
      if (this.hasFormatReader)
        this.state = InflaterState.ReadingHeader;
      else
        this.state = InflaterState.ReadingBFinal;
    }

    public void SetInput(byte[] inputBytes, int offset, int length)
    {
      this.input.SetInput(inputBytes, offset, length);
    }

    public bool Finished()
    {
      return this.state == InflaterState.Done || this.state == InflaterState.VerifyingFooter;
    }

    public int AvailableOutput => this.output.AvailableBytes;

    public bool NeedsInput() => this.input.NeedsInput();

    public int Inflate(byte[] bytes, int offset, int length)
    {
      int num = 0;
      do
      {
        int bytesToCopy = this.output.CopyTo(bytes, offset, length);
        if (bytesToCopy > 0)
        {
          if (this.hasFormatReader)
            this.formatReader.UpdateWithBytesRead(bytes, offset, bytesToCopy);
          offset += bytesToCopy;
          num += bytesToCopy;
          length -= bytesToCopy;
        }
      }
      while (length != 0 && !this.Finished() && this.Decode());
      if (this.state == InflaterState.VerifyingFooter && this.output.AvailableBytes == 0)
        this.formatReader.Validate();
      return num;
    }

    private bool Decode()
    {
      bool flag1 = false;
      if (this.Finished())
        return true;
      if (this.hasFormatReader)
      {
        if (this.state == InflaterState.ReadingHeader)
        {
          if (!this.formatReader.ReadHeader(this.input))
            return false;
          this.state = InflaterState.ReadingBFinal;
        }
        else if (this.state == InflaterState.StartReadingFooter || this.state == InflaterState.ReadingFooter)
        {
          if (!this.formatReader.ReadFooter(this.input))
            return false;
          this.state = InflaterState.VerifyingFooter;
          return true;
        }
      }
      if (this.state == InflaterState.ReadingBFinal)
      {
        if (!this.input.EnsureBitsAvailable(1))
          return false;
        this.bfinal = this.input.GetBits(1);
        this.state = InflaterState.ReadingBType;
      }
      if (this.state == InflaterState.ReadingBType)
      {
        if (!this.input.EnsureBitsAvailable(2))
        {
          this.state = InflaterState.ReadingBType;
          return false;
        }
        this.blockType = (BlockType) this.input.GetBits(2);
        if (this.blockType == BlockType.Dynamic)
          this.state = InflaterState.ReadingNumLitCodes;
        else if (this.blockType == BlockType.Static)
        {
          this.literalLengthTree = HuffmanTree.StaticLiteralLengthTree;
          this.distanceTree = HuffmanTree.StaticDistanceTree;
          this.state = InflaterState.DecodeTop;
        }
        else
        {
          if (this.blockType != BlockType.Uncompressed)
            throw new InvalidDataException(Resources.UnknownBlockType);
          this.state = InflaterState.UncompressedAligning;
        }
      }
      bool flag2;
      if (this.blockType == BlockType.Dynamic)
        flag2 = this.state >= InflaterState.DecodeTop ? this.DecodeBlock(out flag1) : this.DecodeDynamicBlockHeader();
      else if (this.blockType == BlockType.Static)
      {
        flag2 = this.DecodeBlock(out flag1);
      }
      else
      {
        if (this.blockType != BlockType.Uncompressed)
          throw new InvalidDataException(Resources.UnknownBlockType);
        flag2 = this.DecodeUncompressedBlock(out flag1);
      }
      if (flag1 && this.bfinal != 0)
        this.state = !this.hasFormatReader ? InflaterState.Done : InflaterState.StartReadingFooter;
      return flag2;
    }

    private bool DecodeUncompressedBlock(out bool end_of_block)
    {
      end_of_block = false;
      while (true)
      {
        switch (this.state)
        {
          case InflaterState.UncompressedAligning:
            this.input.SkipToByteBoundary();
            this.state = InflaterState.UncompressedByte1;
            goto case InflaterState.UncompressedByte1;
          case InflaterState.UncompressedByte1:
          case InflaterState.UncompressedByte2:
          case InflaterState.UncompressedByte3:
          case InflaterState.UncompressedByte4:
            int bits = this.input.GetBits(8);
            if (bits >= 0)
            {
              this.blockLengthBuffer[(int) (this.state - 16)] = (byte) bits;
              if (this.state == InflaterState.UncompressedByte4)
              {
                this.blockLength = (int) this.blockLengthBuffer[0] + (int) this.blockLengthBuffer[1] * 256;
                if ((int) (ushort) this.blockLength != (int) (ushort) ~((int) this.blockLengthBuffer[2] + (int) this.blockLengthBuffer[3] * 256))
                  goto label_7;
              }
              ++this.state;
              continue;
            }
            goto label_4;
          case InflaterState.DecodingUncompressed:
            goto label_9;
          default:
            goto label_14;
        }
      }
label_4:
      return false;
label_7:
      throw new InvalidDataException(Resources.InvalidBlockLength);
label_9:
      this.blockLength -= this.output.CopyFrom(this.input, this.blockLength);
      if (this.blockLength == 0)
      {
        this.state = InflaterState.ReadingBFinal;
        end_of_block = true;
        return true;
      }
      return this.output.FreeBytes == 0;
label_14:
      throw new InvalidDataException(Resources.UnknownState);
    }

    private bool DecodeBlock(out bool end_of_block_code_seen)
    {
      end_of_block_code_seen = false;
      int freeBytes = this.output.FreeBytes;
      while (freeBytes > 258)
      {
        switch (this.state)
        {
          case InflaterState.DecodeTop:
            int nextSymbol = this.literalLengthTree.GetNextSymbol(this.input);
            if (nextSymbol < 0)
              return false;
            if (nextSymbol < 256)
            {
              this.output.Write((byte) nextSymbol);
              --freeBytes;
              continue;
            }
            if (nextSymbol == 256)
            {
              end_of_block_code_seen = true;
              this.state = InflaterState.ReadingBFinal;
              return true;
            }
            int index = nextSymbol - 257;
            if (index < 8)
            {
              index += 3;
              this.extraBits = 0;
            }
            else if (index == 28)
            {
              index = 258;
              this.extraBits = 0;
            }
            else
            {
              if (index < 0 || index >= Inflater.extraLengthBits.Length)
                throw new InvalidDataException(Resources.GenericInvalidData);
              this.extraBits = (int) Inflater.extraLengthBits[index];
            }
            this.length = index;
            goto case InflaterState.HaveInitialLength;
          case InflaterState.HaveInitialLength:
            if (this.extraBits > 0)
            {
              this.state = InflaterState.HaveInitialLength;
              int bits = this.input.GetBits(this.extraBits);
              if (bits < 0)
                return false;
              if (this.length < 0 || this.length >= Inflater.lengthBase.Length)
                throw new InvalidDataException(Resources.GenericInvalidData);
              this.length = Inflater.lengthBase[this.length] + bits;
            }
            this.state = InflaterState.HaveFullLength;
            goto case InflaterState.HaveFullLength;
          case InflaterState.HaveFullLength:
            if (this.blockType == BlockType.Dynamic)
            {
              this.distanceCode = this.distanceTree.GetNextSymbol(this.input);
            }
            else
            {
              this.distanceCode = this.input.GetBits(5);
              if (this.distanceCode >= 0)
                this.distanceCode = (int) Inflater.staticDistanceTreeTable[this.distanceCode];
            }
            if (this.distanceCode < 0)
              return false;
            this.state = InflaterState.HaveDistCode;
            goto case InflaterState.HaveDistCode;
          case InflaterState.HaveDistCode:
            int distance;
            if (this.distanceCode > 3)
            {
              this.extraBits = this.distanceCode - 2 >> 1;
              int bits = this.input.GetBits(this.extraBits);
              if (bits < 0)
                return false;
              distance = Inflater.distanceBasePosition[this.distanceCode] + bits;
            }
            else
              distance = this.distanceCode + 1;
            this.output.WriteLengthDistance(this.length, distance);
            freeBytes -= this.length;
            this.state = InflaterState.DecodeTop;
            continue;
          default:
            throw new InvalidDataException(Resources.UnknownState);
        }
      }
      return true;
    }

    private bool DecodeDynamicBlockHeader()
    {
      switch (this.state)
      {
        case InflaterState.ReadingNumLitCodes:
          this.literalLengthCodeCount = this.input.GetBits(5);
          if (this.literalLengthCodeCount < 0)
            return false;
          this.literalLengthCodeCount += 257;
          this.state = InflaterState.ReadingNumDistCodes;
          goto case InflaterState.ReadingNumDistCodes;
        case InflaterState.ReadingNumDistCodes:
          this.distanceCodeCount = this.input.GetBits(5);
          if (this.distanceCodeCount < 0)
            return false;
          ++this.distanceCodeCount;
          this.state = InflaterState.ReadingNumCodeLengthCodes;
          goto case InflaterState.ReadingNumCodeLengthCodes;
        case InflaterState.ReadingNumCodeLengthCodes:
          this.codeLengthCodeCount = this.input.GetBits(4);
          if (this.codeLengthCodeCount < 0)
            return false;
          this.codeLengthCodeCount += 4;
          this.loopCounter = 0;
          this.state = InflaterState.ReadingCodeLengthCodes;
          goto case InflaterState.ReadingCodeLengthCodes;
        case InflaterState.ReadingCodeLengthCodes:
          for (; this.loopCounter < this.codeLengthCodeCount; ++this.loopCounter)
          {
            int bits = this.input.GetBits(3);
            if (bits < 0)
              return false;
            this.codeLengthTreeCodeLength[(int) Inflater.codeOrder[this.loopCounter]] = (byte) bits;
          }
          for (int codeLengthCodeCount = this.codeLengthCodeCount; codeLengthCodeCount < Inflater.codeOrder.Length; ++codeLengthCodeCount)
            this.codeLengthTreeCodeLength[(int) Inflater.codeOrder[codeLengthCodeCount]] = (byte) 0;
          this.codeLengthTree = new HuffmanTree(this.codeLengthTreeCodeLength);
          this.codeArraySize = this.literalLengthCodeCount + this.distanceCodeCount;
          this.loopCounter = 0;
          this.state = InflaterState.ReadingTreeCodesBefore;
          goto case InflaterState.ReadingTreeCodesBefore;
        case InflaterState.ReadingTreeCodesBefore:
        case InflaterState.ReadingTreeCodesAfter:
          while (this.loopCounter < this.codeArraySize)
          {
            if (this.state == InflaterState.ReadingTreeCodesBefore && (this.lengthCode = this.codeLengthTree.GetNextSymbol(this.input)) < 0)
              return false;
            if (this.lengthCode <= 15)
            {
              this.codeList[this.loopCounter++] = (byte) this.lengthCode;
            }
            else
            {
              if (!this.input.EnsureBitsAvailable(7))
              {
                this.state = InflaterState.ReadingTreeCodesAfter;
                return false;
              }
              if (this.lengthCode == 16)
              {
                byte num1 = this.loopCounter != 0 ? this.codeList[this.loopCounter - 1] : throw new InvalidDataException();
                int num2 = this.input.GetBits(2) + 3;
                if (this.loopCounter + num2 > this.codeArraySize)
                  throw new InvalidDataException();
                for (int index = 0; index < num2; ++index)
                  this.codeList[this.loopCounter++] = num1;
              }
              else if (this.lengthCode == 17)
              {
                int num = this.input.GetBits(3) + 3;
                if (this.loopCounter + num > this.codeArraySize)
                  throw new InvalidDataException();
                for (int index = 0; index < num; ++index)
                  this.codeList[this.loopCounter++] = (byte) 0;
              }
              else
              {
                int num = this.input.GetBits(7) + 11;
                if (this.loopCounter + num > this.codeArraySize)
                  throw new InvalidDataException();
                for (int index = 0; index < num; ++index)
                  this.codeList[this.loopCounter++] = (byte) 0;
              }
            }
            this.state = InflaterState.ReadingTreeCodesBefore;
          }
          byte[] numArray1 = new byte[288];
          byte[] numArray2 = new byte[32];
          Array.Copy((Array) this.codeList, (Array) numArray1, this.literalLengthCodeCount);
          Array.Copy((Array) this.codeList, this.literalLengthCodeCount, (Array) numArray2, 0, this.distanceCodeCount);
          this.literalLengthTree = numArray1[256] != (byte) 0 ? new HuffmanTree(numArray1) : throw new InvalidDataException();
          this.distanceTree = new HuffmanTree(numArray2);
          this.state = InflaterState.DecodeTop;
          return true;
        default:
          throw new InvalidDataException(Resources.UnknownState);
      }
    }
  }
}
