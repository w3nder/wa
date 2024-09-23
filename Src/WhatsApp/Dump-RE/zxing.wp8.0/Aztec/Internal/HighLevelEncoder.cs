// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.HighLevelEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>
  /// This produces nearly optimal encodings of text into the first-level of
  /// encoding used by Aztec code.
  /// It uses a dynamic algorithm.  For each prefix of the string, it determines
  /// a set of encodings that could lead to this prefix.  We repeatedly add a
  /// character and generate a new set of optimal encodings until we have read
  /// through the entire input.
  /// @author Frank Yellin
  /// @author Rustam Abdullaev
  /// </summary>
  public sealed class HighLevelEncoder
  {
    internal const int MODE_UPPER = 0;
    internal const int MODE_LOWER = 1;
    internal const int MODE_DIGIT = 2;
    internal const int MODE_MIXED = 3;
    internal const int MODE_PUNCT = 4;
    internal static string[] MODE_NAMES = new string[5]
    {
      "UPPER",
      "LOWER",
      "DIGIT",
      "MIXED",
      "PUNCT"
    };
    internal static readonly int[][] LATCH_TABLE = new int[5][]
    {
      new int[5]{ 0, 327708, 327710, 327709, 656318 },
      new int[5]{ 590318, 0, 327710, 327709, 656318 },
      new int[5]{ 262158, 590300, 0, 590301, 932798 },
      new int[5]{ 327709, 327708, 656318, 0, 327710 },
      new int[5]{ 327711, 656380, 656382, 656381, 0 }
    };
    internal static readonly int[][] CHAR_MAP = new int[5][];
    internal static readonly int[][] SHIFT_TABLE = new int[6][];
    private readonly byte[] text;

    static HighLevelEncoder()
    {
      HighLevelEncoder.CHAR_MAP[0] = new int[256];
      HighLevelEncoder.CHAR_MAP[1] = new int[256];
      HighLevelEncoder.CHAR_MAP[2] = new int[256];
      HighLevelEncoder.CHAR_MAP[3] = new int[256];
      HighLevelEncoder.CHAR_MAP[4] = new int[256];
      HighLevelEncoder.SHIFT_TABLE[0] = new int[6];
      HighLevelEncoder.SHIFT_TABLE[1] = new int[6];
      HighLevelEncoder.SHIFT_TABLE[2] = new int[6];
      HighLevelEncoder.SHIFT_TABLE[3] = new int[6];
      HighLevelEncoder.SHIFT_TABLE[4] = new int[6];
      HighLevelEncoder.SHIFT_TABLE[5] = new int[6];
      HighLevelEncoder.CHAR_MAP[0][32] = 1;
      for (int index = 65; index <= 90; ++index)
        HighLevelEncoder.CHAR_MAP[0][index] = index - 65 + 2;
      HighLevelEncoder.CHAR_MAP[1][32] = 1;
      for (int index = 97; index <= 122; ++index)
        HighLevelEncoder.CHAR_MAP[1][index] = index - 97 + 2;
      HighLevelEncoder.CHAR_MAP[2][32] = 1;
      for (int index = 48; index <= 57; ++index)
        HighLevelEncoder.CHAR_MAP[2][index] = index - 48 + 2;
      HighLevelEncoder.CHAR_MAP[2][44] = 12;
      HighLevelEncoder.CHAR_MAP[2][46] = 13;
      int[] numArray1 = new int[28]
      {
        0,
        32,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        27,
        28,
        29,
        30,
        31,
        64,
        92,
        94,
        95,
        96,
        124,
        126,
        (int) sbyte.MaxValue
      };
      for (int index = 0; index < numArray1.Length; ++index)
        HighLevelEncoder.CHAR_MAP[3][numArray1[index]] = index;
      int[] numArray2 = new int[31]
      {
        0,
        13,
        0,
        0,
        0,
        0,
        33,
        39,
        35,
        36,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        47,
        58,
        59,
        60,
        61,
        62,
        63,
        91,
        93,
        123,
        125
      };
      for (int index = 0; index < numArray2.Length; ++index)
      {
        if (numArray2[index] > 0)
          HighLevelEncoder.CHAR_MAP[4][numArray2[index]] = index;
      }
      foreach (int[] array in HighLevelEncoder.SHIFT_TABLE)
        SupportClass.Fill<int>(array, -1);
      HighLevelEncoder.SHIFT_TABLE[0][4] = 0;
      HighLevelEncoder.SHIFT_TABLE[1][4] = 0;
      HighLevelEncoder.SHIFT_TABLE[1][0] = 28;
      HighLevelEncoder.SHIFT_TABLE[3][4] = 0;
      HighLevelEncoder.SHIFT_TABLE[2][4] = 0;
      HighLevelEncoder.SHIFT_TABLE[2][0] = 15;
    }

    public HighLevelEncoder(byte[] text) => this.text = text;

    /// <summary>
    /// Convert the text represented by this High Level Encoder into a BitArray.
    /// </summary>
    /// <returns></returns>
    public BitArray encode()
    {
      ICollection<State> states = (ICollection<State>) new Collection<State>();
      states.Add(State.INITIAL_STATE);
      for (int index = 0; index < this.text.Length; ++index)
      {
        int num = index + 1 < this.text.Length ? (int) this.text[index + 1] : 0;
        int pairCode;
        switch (this.text[index])
        {
          case 13:
            pairCode = num == 10 ? 2 : 0;
            break;
          case 44:
            pairCode = num == 32 ? 4 : 0;
            break;
          case 46:
            pairCode = num == 32 ? 3 : 0;
            break;
          case 58:
            pairCode = num == 32 ? 5 : 0;
            break;
          default:
            pairCode = 0;
            break;
        }
        if (pairCode > 0)
        {
          states = HighLevelEncoder.updateStateListForPair((IEnumerable<State>) states, index, pairCode);
          ++index;
        }
        else
          states = this.updateStateListForChar((IEnumerable<State>) states, index);
      }
      State state1 = (State) null;
      foreach (State state2 in (IEnumerable<State>) states)
      {
        if (state1 == null)
          state1 = state2;
        else if (state2.BitCount < state1.BitCount)
          state1 = state2;
      }
      return state1.toBitArray(this.text);
    }

    private ICollection<State> updateStateListForChar(IEnumerable<State> states, int index)
    {
      LinkedList<State> result = new LinkedList<State>();
      foreach (State state in states)
        this.updateStateForChar(state, index, (ICollection<State>) result);
      return HighLevelEncoder.simplifyStates((IEnumerable<State>) result);
    }

    private void updateStateForChar(State state, int index, ICollection<State> result)
    {
      char index1 = (char) ((uint) this.text[index] & (uint) byte.MaxValue);
      bool flag = HighLevelEncoder.CHAR_MAP[state.Mode][(int) index1] > 0;
      State state1 = (State) null;
      for (int mode = 0; mode <= 4; ++mode)
      {
        int num = HighLevelEncoder.CHAR_MAP[mode][(int) index1];
        if (num > 0)
        {
          if (state1 == null)
            state1 = state.endBinaryShift(index);
          if (!flag || mode == state.Mode || mode == 2)
          {
            State state2 = state1.latchAndAppend(mode, num);
            result.Add(state2);
          }
          if (!flag && HighLevelEncoder.SHIFT_TABLE[state.Mode][mode] >= 0)
          {
            State state3 = state1.shiftAndAppend(mode, num);
            result.Add(state3);
          }
        }
      }
      if (state.BinaryShiftByteCount <= 0 && HighLevelEncoder.CHAR_MAP[state.Mode][(int) index1] != 0)
        return;
      State state4 = state.addBinaryShiftChar(index);
      result.Add(state4);
    }

    private static ICollection<State> updateStateListForPair(
      IEnumerable<State> states,
      int index,
      int pairCode)
    {
      LinkedList<State> result = new LinkedList<State>();
      foreach (State state in states)
        HighLevelEncoder.updateStateForPair(state, index, pairCode, (ICollection<State>) result);
      return HighLevelEncoder.simplifyStates((IEnumerable<State>) result);
    }

    private static void updateStateForPair(
      State state,
      int index,
      int pairCode,
      ICollection<State> result)
    {
      State state1 = state.endBinaryShift(index);
      result.Add(state1.latchAndAppend(4, pairCode));
      if (state.Mode != 4)
        result.Add(state1.shiftAndAppend(4, pairCode));
      if (pairCode == 3 || pairCode == 4)
      {
        State state2 = state1.latchAndAppend(2, 16 - pairCode).latchAndAppend(2, 1);
        result.Add(state2);
      }
      if (state.BinaryShiftByteCount <= 0)
        return;
      State state3 = state.addBinaryShiftChar(index).addBinaryShiftChar(index + 1);
      result.Add(state3);
    }

    private static ICollection<State> simplifyStates(IEnumerable<State> states)
    {
      LinkedList<State> linkedList = new LinkedList<State>();
      List<State> stateList = new List<State>();
      foreach (State state1 in states)
      {
        bool flag = true;
        stateList.Clear();
        foreach (State other in linkedList)
        {
          if (other.isBetterThanOrEqualTo(state1))
          {
            flag = false;
            break;
          }
          if (state1.isBetterThanOrEqualTo(other))
            stateList.Add(other);
        }
        if (flag)
          linkedList.AddLast(state1);
        foreach (State state2 in stateList)
          linkedList.Remove(state2);
      }
      return (ICollection<State>) linkedList;
    }
  }
}
