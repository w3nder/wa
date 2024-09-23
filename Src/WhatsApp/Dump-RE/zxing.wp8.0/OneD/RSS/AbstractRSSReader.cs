// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.AbstractRSSReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class AbstractRSSReader : OneDReader
  {
    private const float MIN_FINDER_PATTERN_RATIO = 0.7916667f;
    private const float MAX_FINDER_PATTERN_RATIO = 0.892857134f;
    private static readonly int MAX_AVG_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.20000000298023224);
    private static readonly int MAX_INDIVIDUAL_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.44999998807907104);
    private readonly int[] decodeFinderCounters;
    private readonly int[] dataCharacterCounters;
    private readonly float[] oddRoundingErrors;
    private readonly float[] evenRoundingErrors;
    private readonly int[] oddCounts;
    private readonly int[] evenCounts;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.RSS.AbstractRSSReader" /> class.
    /// </summary>
    protected AbstractRSSReader()
    {
      this.decodeFinderCounters = new int[4];
      this.dataCharacterCounters = new int[8];
      this.oddRoundingErrors = new float[4];
      this.evenRoundingErrors = new float[4];
      this.oddCounts = new int[this.dataCharacterCounters.Length / 2];
      this.evenCounts = new int[this.dataCharacterCounters.Length / 2];
    }

    /// <summary>Gets the decode finder counters.</summary>
    /// <returns></returns>
    protected int[] getDecodeFinderCounters() => this.decodeFinderCounters;

    /// <summary>Gets the data character counters.</summary>
    /// <returns></returns>
    protected int[] getDataCharacterCounters() => this.dataCharacterCounters;

    /// <summary>Gets the odd rounding errors.</summary>
    /// <returns></returns>
    protected float[] getOddRoundingErrors() => this.oddRoundingErrors;

    /// <summary>Gets the even rounding errors.</summary>
    /// <returns></returns>
    protected float[] getEvenRoundingErrors() => this.evenRoundingErrors;

    /// <summary>Gets the odd counts.</summary>
    /// <returns></returns>
    protected int[] getOddCounts() => this.oddCounts;

    /// <summary>Gets the even counts.</summary>
    /// <returns></returns>
    protected int[] getEvenCounts() => this.evenCounts;

    /// <summary>Parses the finder value.</summary>
    /// <param name="counters">The counters.</param>
    /// <param name="finderPatterns">The finder patterns.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected static bool parseFinderValue(int[] counters, int[][] finderPatterns, out int value)
    {
      value = 0;
      while (value < finderPatterns.Length)
      {
        if (OneDReader.patternMatchVariance(counters, finderPatterns[value], AbstractRSSReader.MAX_INDIVIDUAL_VARIANCE) < AbstractRSSReader.MAX_AVG_VARIANCE)
          return true;
        ++value;
      }
      return false;
    }

    /// <summary>Counts the specified array.</summary>
    /// <param name="array">The array.</param>
    /// <returns></returns>
    protected static int count(int[] array)
    {
      int num1 = 0;
      foreach (int num2 in array)
        num1 += num2;
      return num1;
    }

    /// <summary>Increments the specified array.</summary>
    /// <param name="array">The array.</param>
    /// <param name="errors">The errors.</param>
    protected static void increment(int[] array, float[] errors)
    {
      int index1 = 0;
      float error = errors[0];
      for (int index2 = 1; index2 < array.Length; ++index2)
      {
        if ((double) errors[index2] > (double) error)
        {
          error = errors[index2];
          index1 = index2;
        }
      }
      ++array[index1];
    }

    /// <summary>Decrements the specified array.</summary>
    /// <param name="array">The array.</param>
    /// <param name="errors">The errors.</param>
    protected static void decrement(int[] array, float[] errors)
    {
      int index1 = 0;
      float error = errors[0];
      for (int index2 = 1; index2 < array.Length; ++index2)
      {
        if ((double) errors[index2] < (double) error)
        {
          error = errors[index2];
          index1 = index2;
        }
      }
      --array[index1];
    }

    /// <summary>
    /// Determines whether [is finder pattern] [the specified counters].
    /// </summary>
    /// <param name="counters">The counters.</param>
    /// <returns>
    ///   <c>true</c> if [is finder pattern] [the specified counters]; otherwise, <c>false</c>.
    /// </returns>
    protected static bool isFinderPattern(int[] counters)
    {
      int num1 = counters[0] + counters[1];
      int num2 = num1 + counters[2] + counters[3];
      float num3 = (float) num1 / (float) num2;
      if ((double) num3 < 0.79166668653488159 || (double) num3 > 0.8928571343421936)
        return false;
      int num4 = int.MaxValue;
      int num5 = int.MinValue;
      foreach (int counter in counters)
      {
        if (counter > num5)
          num5 = counter;
        if (counter < num4)
          num4 = counter;
      }
      return num5 < 10 * num4;
    }
  }
}
