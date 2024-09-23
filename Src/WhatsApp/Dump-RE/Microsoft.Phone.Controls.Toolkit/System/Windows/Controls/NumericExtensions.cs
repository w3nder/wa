// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.NumericExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Runtime.InteropServices;

#nullable disable
namespace System.Windows.Controls
{
  internal static class NumericExtensions
  {
    public static bool IsNaN(this double value)
    {
      NumericExtensions.NanUnion nanUnion = new NumericExtensions.NanUnion()
      {
        FloatingValue = value
      };
      switch (nanUnion.IntegerValue & 18442240474082181120UL)
      {
        case 9218868437227405312:
        case 18442240474082181120:
          return (nanUnion.IntegerValue & 4503599627370495UL) != 0UL;
        default:
          return false;
      }
    }

    public static bool IsGreaterThan(double left, double right)
    {
      return left > right && !NumericExtensions.AreClose(left, right);
    }

    public static bool AreClose(double left, double right)
    {
      if (left == right)
        return true;
      double num1 = (Math.Abs(left) + Math.Abs(right) + 10.0) * 2.2204460492503131E-16;
      double num2 = left - right;
      return -num1 < num2 && num1 > num2;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct NanUnion
    {
      [FieldOffset(0)]
      internal double FloatingValue;
      [FieldOffset(0)]
      internal ulong IntegerValue;
    }
  }
}
