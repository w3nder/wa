// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.EC.ModulusPoly
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.PDF417.Internal.EC
{
  /// <summary>
  /// <see cref="!:com.google.zxing.common.reedsolomon.GenericGFPoly" />
  /// </summary>
  /// <author>Sean Owen</author>
  internal sealed class ModulusPoly
  {
    private readonly ModulusGF field;
    private readonly int[] coefficients;

    public ModulusPoly(ModulusGF field, int[] coefficients)
    {
      if (coefficients.Length == 0)
        throw new ArgumentException();
      this.field = field;
      int length = coefficients.Length;
      if (length > 1 && coefficients[0] == 0)
      {
        int sourceIndex = 1;
        while (sourceIndex < length && coefficients[sourceIndex] == 0)
          ++sourceIndex;
        if (sourceIndex == length)
        {
          this.coefficients = field.Zero.coefficients;
        }
        else
        {
          this.coefficients = new int[length - sourceIndex];
          Array.Copy((Array) coefficients, sourceIndex, (Array) this.coefficients, 0, this.coefficients.Length);
        }
      }
      else
        this.coefficients = coefficients;
    }

    /// <summary>Gets the coefficients.</summary>
    /// <value>The coefficients.</value>
    internal int[] Coefficients => this.coefficients;

    /// <summary>degree of this polynomial</summary>
    internal int Degree => this.coefficients.Length - 1;

    /// <summary>
    /// Gets a value indicating whether this instance is zero.
    /// </summary>
    /// <value>true if this polynomial is the monomial "0"</value>
    internal bool isZero => this.coefficients[0] == 0;

    /// <summary>coefficient of x^degree term in this polynomial</summary>
    /// <param name="degree">The degree.</param>
    /// <returns>coefficient of x^degree term in this polynomial</returns>
    internal int getCoefficient(int degree)
    {
      return this.coefficients[this.coefficients.Length - 1 - degree];
    }

    /// <summary>evaluation of this polynomial at a given point</summary>
    /// <param name="a">A.</param>
    /// <returns>evaluation of this polynomial at a given point</returns>
    internal int evaluateAt(int a)
    {
      if (a == 0)
        return this.getCoefficient(0);
      int length = this.coefficients.Length;
      int a1 = 0;
      if (a == 1)
      {
        foreach (int coefficient in this.coefficients)
          a1 = this.field.add(a1, coefficient);
        return a1;
      }
      int b = this.coefficients[0];
      for (int index = 1; index < length; ++index)
        b = this.field.add(this.field.multiply(a, b), this.coefficients[index]);
      return b;
    }

    /// <summary>Adds another Modulus</summary>
    /// <param name="other">Other.</param>
    internal ModulusPoly add(ModulusPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("ModulusPolys do not have same ModulusGF field");
      if (this.isZero)
        return other;
      if (other.isZero)
        return this;
      int[] numArray1 = this.coefficients;
      int[] sourceArray = other.coefficients;
      if (numArray1.Length > sourceArray.Length)
      {
        int[] numArray2 = numArray1;
        numArray1 = sourceArray;
        sourceArray = numArray2;
      }
      int[] numArray3 = new int[sourceArray.Length];
      int length = sourceArray.Length - numArray1.Length;
      Array.Copy((Array) sourceArray, 0, (Array) numArray3, 0, length);
      for (int index = length; index < sourceArray.Length; ++index)
        numArray3[index] = this.field.add(numArray1[index - length], sourceArray[index]);
      return new ModulusPoly(this.field, numArray3);
    }

    /// <summary>Subtract another Modulus</summary>
    /// <param name="other">Other.</param>
    internal ModulusPoly subtract(ModulusPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("ModulusPolys do not have same ModulusGF field");
      return other.isZero ? this : this.add(other.getNegative());
    }

    /// <summary>Multiply by another Modulus</summary>
    /// <param name="other">Other.</param>
    internal ModulusPoly multiply(ModulusPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("ModulusPolys do not have same ModulusGF field");
      if (this.isZero || other.isZero)
        return this.field.Zero;
      int[] coefficients1 = this.coefficients;
      int length1 = coefficients1.Length;
      int[] coefficients2 = other.coefficients;
      int length2 = coefficients2.Length;
      int[] coefficients3 = new int[length1 + length2 - 1];
      for (int index1 = 0; index1 < length1; ++index1)
      {
        int a = coefficients1[index1];
        for (int index2 = 0; index2 < length2; ++index2)
          coefficients3[index1 + index2] = this.field.add(coefficients3[index1 + index2], this.field.multiply(a, coefficients2[index2]));
      }
      return new ModulusPoly(this.field, coefficients3);
    }

    /// <summary>Returns a Negative version of this instance</summary>
    internal ModulusPoly getNegative()
    {
      int length = this.coefficients.Length;
      int[] coefficients = new int[length];
      for (int index = 0; index < length; ++index)
        coefficients[index] = this.field.subtract(0, this.coefficients[index]);
      return new ModulusPoly(this.field, coefficients);
    }

    /// <summary>Multiply by a Scalar.</summary>
    /// <param name="scalar">Scalar.</param>
    internal ModulusPoly multiply(int scalar)
    {
      if (scalar == 0)
        return this.field.Zero;
      if (scalar == 1)
        return this;
      int length = this.coefficients.Length;
      int[] coefficients = new int[length];
      for (int index = 0; index < length; ++index)
        coefficients[index] = this.field.multiply(this.coefficients[index], scalar);
      return new ModulusPoly(this.field, coefficients);
    }

    /// <summary>Multiplies by a Monomial</summary>
    /// <returns>The by monomial.</returns>
    /// <param name="degree">Degree.</param>
    /// <param name="coefficient">Coefficient.</param>
    internal ModulusPoly multiplyByMonomial(int degree, int coefficient)
    {
      if (degree < 0)
        throw new ArgumentException();
      if (coefficient == 0)
        return this.field.Zero;
      int length = this.coefficients.Length;
      int[] coefficients = new int[length + degree];
      for (int index = 0; index < length; ++index)
        coefficients[index] = this.field.multiply(this.coefficients[index], coefficient);
      return new ModulusPoly(this.field, coefficients);
    }

    /// <summary>Divide by another modulus</summary>
    /// <param name="other">Other.</param>
    internal ModulusPoly[] divide(ModulusPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("ModulusPolys do not have same ModulusGF field");
      if (other.isZero)
        throw new DivideByZeroException();
      ModulusPoly modulusPoly1 = this.field.Zero;
      ModulusPoly modulusPoly2 = this;
      int b = this.field.inverse(other.getCoefficient(other.Degree));
      ModulusPoly other1;
      for (; modulusPoly2.Degree >= other.Degree && !modulusPoly2.isZero; modulusPoly2 = modulusPoly2.subtract(other1))
      {
        int degree = modulusPoly2.Degree - other.Degree;
        int coefficient = this.field.multiply(modulusPoly2.getCoefficient(modulusPoly2.Degree), b);
        other1 = other.multiplyByMonomial(degree, coefficient);
        ModulusPoly other2 = this.field.buildMonomial(degree, coefficient);
        modulusPoly1 = modulusPoly1.add(other2);
      }
      return new ModulusPoly[2]
      {
        modulusPoly1,
        modulusPoly2
      };
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.EC.ModulusPoly" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.EC.ModulusPoly" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(8 * this.Degree);
      for (int degree = this.Degree; degree >= 0; --degree)
      {
        int num = this.getCoefficient(degree);
        if (num != 0)
        {
          if (num < 0)
          {
            stringBuilder.Append(" - ");
            num = -num;
          }
          else if (stringBuilder.Length > 0)
            stringBuilder.Append(" + ");
          if (degree == 0 || num != 1)
            stringBuilder.Append(num);
          switch (degree)
          {
            case 0:
              continue;
            case 1:
              stringBuilder.Append('x');
              continue;
            default:
              stringBuilder.Append("x^");
              stringBuilder.Append(degree);
              continue;
          }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
