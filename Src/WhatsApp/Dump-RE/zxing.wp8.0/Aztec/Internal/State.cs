// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.State
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>
  /// State represents all information about a sequence necessary to generate the current output.
  /// Note that a state is immutable.
  /// </summary>
  internal sealed class State
  {
    public static readonly State INITIAL_STATE = new State(Token.EMPTY, 0, 0, 0);
    private readonly int mode;
    private readonly Token token;
    private readonly int binaryShiftByteCount;
    private readonly int bitCount;

    public State(Token token, int mode, int binaryBytes, int bitCount)
    {
      this.token = token;
      this.mode = mode;
      this.binaryShiftByteCount = binaryBytes;
      this.bitCount = bitCount;
    }

    public int Mode => this.mode;

    public Token Token => this.token;

    public int BinaryShiftByteCount => this.binaryShiftByteCount;

    public int BitCount => this.bitCount;

    /// <summary>
    /// Create a new state representing this state with a latch to a (not
    /// necessary different) mode, and then a code.
    /// </summary>
    public State latchAndAppend(int mode, int value)
    {
      int bitCount1 = this.bitCount;
      Token token = this.token;
      if (mode != this.mode)
      {
        int num = HighLevelEncoder.LATCH_TABLE[this.mode][mode];
        token = token.add(num & (int) ushort.MaxValue, num >> 16);
        bitCount1 += num >> 16;
      }
      int bitCount2 = mode == 2 ? 4 : 5;
      return new State(token.add(value, bitCount2), mode, 0, bitCount1 + bitCount2);
    }

    /// <summary>
    /// Create a new state representing this state, with a temporary shift
    /// to a different mode to output a single value.
    /// </summary>
    public State shiftAndAppend(int mode, int value)
    {
      Token token = this.token;
      int bitCount = this.mode == 2 ? 4 : 5;
      return new State(token.add(HighLevelEncoder.SHIFT_TABLE[this.mode][mode], bitCount).add(value, 5), this.mode, 0, this.bitCount + bitCount + 5);
    }

    /// <summary>
    /// Create a new state representing this state, but an additional character
    /// output in Binary Shift mode.
    /// </summary>
    public State addBinaryShiftChar(int index)
    {
      Token token = this.token;
      int mode = this.mode;
      int bitCount = this.bitCount;
      if (this.mode == 4 || this.mode == 2)
      {
        int num = HighLevelEncoder.LATCH_TABLE[mode][0];
        token = token.add(num & (int) ushort.MaxValue, num >> 16);
        bitCount += num >> 16;
        mode = 0;
      }
      int num1 = this.binaryShiftByteCount == 0 || this.binaryShiftByteCount == 31 ? 18 : (this.binaryShiftByteCount == 62 ? 9 : 8);
      State state = new State(token, mode, this.binaryShiftByteCount + 1, bitCount + num1);
      if (state.binaryShiftByteCount == 2078)
        state = state.endBinaryShift(index + 1);
      return state;
    }

    /// <summary>
    /// Create the state identical to this one, but we are no longer in
    /// Binary Shift mode.
    /// </summary>
    public State endBinaryShift(int index)
    {
      return this.binaryShiftByteCount == 0 ? this : new State(this.token.addBinaryShift(index - this.binaryShiftByteCount, this.binaryShiftByteCount), this.mode, 0, this.bitCount);
    }

    /// <summary>
    /// Returns true if "this" state is better (or equal) to be in than "that"
    /// state under all possible circumstances.
    /// </summary>
    public bool isBetterThanOrEqualTo(State other)
    {
      int num = this.bitCount + (HighLevelEncoder.LATCH_TABLE[this.mode][other.mode] >> 16);
      if (other.binaryShiftByteCount > 0 && (this.binaryShiftByteCount == 0 || this.binaryShiftByteCount > other.binaryShiftByteCount))
        num += 10;
      return num <= other.bitCount;
    }

    public BitArray toBitArray(byte[] text)
    {
      LinkedList<Token> linkedList = new LinkedList<Token>();
      for (Token token = this.endBinaryShift(text.Length).token; token != null; token = token.Previous)
        linkedList.AddFirst(token);
      BitArray bitArray = new BitArray();
      foreach (Token token in linkedList)
        token.appendTo(bitArray, text);
      return bitArray;
    }

    public override string ToString()
    {
      return string.Format("{0} bits={1} bytes={2}", (object) HighLevelEncoder.MODE_NAMES[this.mode], (object) this.bitCount, (object) this.binaryShiftByteCount);
    }
  }
}
