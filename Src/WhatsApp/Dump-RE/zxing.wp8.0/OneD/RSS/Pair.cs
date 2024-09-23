// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Pair
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS
{
  internal sealed class Pair : DataCharacter
  {
    public FinderPattern FinderPattern { get; private set; }

    public int Count { get; private set; }

    internal Pair(int value, int checksumPortion, FinderPattern finderPattern)
      : base(value, checksumPortion)
    {
      this.FinderPattern = finderPattern;
    }

    public void incrementCount() => ++this.Count;
  }
}
