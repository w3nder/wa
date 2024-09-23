// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.ExpandedRow
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.OneD.RSS.Expanded
{
  /// <summary>
  /// One row of an RSS Expanded Stacked symbol, consisting of 1+ expanded pairs.
  /// </summary>
  internal sealed class ExpandedRow
  {
    internal ExpandedRow(List<ExpandedPair> pairs, int rowNumber, bool wasReversed)
    {
      this.Pairs = new List<ExpandedPair>((IEnumerable<ExpandedPair>) pairs);
      this.RowNumber = rowNumber;
      this.IsReversed = wasReversed;
    }

    internal List<ExpandedPair> Pairs { get; private set; }

    internal int RowNumber { get; private set; }

    /// <summary>
    /// Did this row of the image have to be reversed (mirrored) to recognize the pairs?
    /// </summary>
    internal bool IsReversed { get; private set; }

    internal bool IsEquivalent(List<ExpandedPair> otherPairs)
    {
      return this.Pairs.Equals((object) otherPairs);
    }

    public override string ToString() => "{ " + (object) this.Pairs + " }";

    /// <summary>
    /// Two rows are equal if they contain the same pairs in the same order.
    /// </summary>
    public override bool Equals(object o)
    {
      if (!(o is ExpandedRow))
        return false;
      ExpandedRow expandedRow = (ExpandedRow) o;
      return this.Pairs.Equals((object) expandedRow.Pairs) && this.IsReversed == expandedRow.IsReversed;
    }

    public override int GetHashCode() => this.Pairs.GetHashCode() ^ this.IsReversed.GetHashCode();
  }
}
