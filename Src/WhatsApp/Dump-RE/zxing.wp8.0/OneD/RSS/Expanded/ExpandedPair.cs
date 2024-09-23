// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.ExpandedPair
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS.Expanded
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// </summary>
  internal sealed class ExpandedPair
  {
    internal bool MayBeLast { get; private set; }

    internal DataCharacter LeftChar { get; private set; }

    internal DataCharacter RightChar { get; private set; }

    internal FinderPattern FinderPattern { get; private set; }

    internal ExpandedPair(
      DataCharacter leftChar,
      DataCharacter rightChar,
      FinderPattern finderPattern,
      bool mayBeLast)
    {
      this.LeftChar = leftChar;
      this.RightChar = rightChar;
      this.FinderPattern = finderPattern;
      this.MayBeLast = mayBeLast;
    }

    public bool MustBeLast => this.RightChar == null;

    public override string ToString()
    {
      return "[ " + (object) this.LeftChar + " , " + (object) this.RightChar + " : " + (this.FinderPattern == null ? (object) "null" : (object) this.FinderPattern.Value.ToString()) + " ]";
    }

    public override bool Equals(object o)
    {
      if (!(o is ExpandedPair))
        return false;
      ExpandedPair expandedPair = (ExpandedPair) o;
      return ExpandedPair.EqualsOrNull((object) this.LeftChar, (object) expandedPair.LeftChar) && ExpandedPair.EqualsOrNull((object) this.RightChar, (object) expandedPair.RightChar) && ExpandedPair.EqualsOrNull((object) this.FinderPattern, (object) expandedPair.FinderPattern);
    }

    private static bool EqualsOrNull(object o1, object o2)
    {
      return o1 != null ? o1.Equals(o2) : o2 == null;
    }

    public override int GetHashCode()
    {
      return ExpandedPair.hashNotNull((object) this.LeftChar) ^ ExpandedPair.hashNotNull((object) this.RightChar) ^ ExpandedPair.hashNotNull((object) this.FinderPattern);
    }

    private static int hashNotNull(object o) => o != null ? o.GetHashCode() : 0;
  }
}
