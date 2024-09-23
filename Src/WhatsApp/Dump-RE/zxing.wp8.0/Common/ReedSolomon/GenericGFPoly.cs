// Decompiled with JetBrains decompiler
// Type: ZXing.Common.ReedSolomon.GenericGFPoly
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Common.ReedSolomon
{
  /// <summary>
  /// <p>Represents a polynomial whose coefficients are elements of a GF.
  /// Instances of this class are immutable.</p>
  /// <p>Much credit is due to William Rucklidge since portions of this code are an indirect
  /// port of his C++ Reed-Solomon implementation.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  internal sealed class GenericGFPoly
  {
    private readonly GenericGF field;
    private readonly int[] coefficients;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.ReedSolomon.GenericGFPoly" /> class.
    /// </summary>
    /// <param name="field">the {@link GenericGF} instance representing the field to use
    /// to perform computations</param>
    /// <param name="coefficients">coefficients as ints representing elements of GF(size), arranged
    /// from most significant (highest-power term) coefficient to least significant</param>
    /// <exception cref="T:System.ArgumentException">if argument is null or empty,
    /// or if leading coefficient is 0 and this is not a
    /// constant polynomial (that is, it is not the monomial "0")</exception>
    internal GenericGFPoly(GenericGF field, int[] coefficients)
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

    internal int[] Coefficients => this.coefficients;

    /// <summary>degree of this polynomial</summary>
    internal int Degree => this.coefficients.Length - 1;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:ZXing.Common.ReedSolomon.GenericGFPoly" /> is zero.
    /// </summary>
    /// <value>true iff this polynomial is the monomial "0"</value>
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
      int a1 = 0;
      if (a == 0)
        return this.getCoefficient(0);
      int length = this.coefficients.Length;
      if (a == 1)
      {
        foreach (int coefficient in this.coefficients)
          a1 = GenericGF.addOrSubtract(a1, coefficient);
        return a1;
      }
      int b = this.coefficients[0];
      for (int index = 1; index < length; ++index)
        b = GenericGF.addOrSubtract(this.field.multiply(a, b), this.coefficients[index]);
      return b;
    }

    internal GenericGFPoly addOrSubtract(GenericGFPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
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
        numArray3[index] = GenericGF.addOrSubtract(numArray1[index - length], sourceArray[index]);
      return new GenericGFPoly(this.field, numArray3);
    }

    internal GenericGFPoly multiply(GenericGFPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
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
          coefficients3[index1 + index2] = GenericGF.addOrSubtract(coefficients3[index1 + index2], this.field.multiply(a, coefficients2[index2]));
      }
      return new GenericGFPoly(this.field, coefficients3);
    }

    internal GenericGFPoly multiply(int scalar)
    {
      if (scalar == 0)
        return this.field.Zero;
      if (scalar == 1)
        return this;
      int length = this.coefficients.Length;
      int[] coefficients = new int[length];
      for (int index = 0; index < length; ++index)
        coefficients[index] = this.field.multiply(this.coefficients[index], scalar);
      return new GenericGFPoly(this.field, coefficients);
    }

    internal GenericGFPoly multiplyByMonomial(int degree, int coefficient)
    {
      if (degree < 0)
        throw new ArgumentException();
      if (coefficient == 0)
        return this.field.Zero;
      int length = this.coefficients.Length;
      int[] coefficients = new int[length + degree];
      for (int index = 0; index < length; ++index)
        coefficients[index] = this.field.multiply(this.coefficients[index], coefficient);
      return new GenericGFPoly(this.field, coefficients);
    }

    internal GenericGFPoly[] divide(GenericGFPoly other)
    {
      if (!this.field.Equals((object) other.field))
        throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
      if (other.isZero)
        throw new ArgumentException("Divide by 0");
      GenericGFPoly genericGfPoly1 = this.field.Zero;
      GenericGFPoly genericGfPoly2 = this;
      int b = this.field.inverse(other.getCoefficient(other.Degree));
      GenericGFPoly other1;
      for (; genericGfPoly2.Degree >= other.Degree && !genericGfPoly2.isZero; genericGfPoly2 = genericGfPoly2.addOrSubtract(other1))
      {
        int degree = genericGfPoly2.Degree - other.Degree;
        int coefficient = this.field.multiply(genericGfPoly2.getCoefficient(genericGfPoly2.Degree), b);
        other1 = other.multiplyByMonomial(degree, coefficient);
        GenericGFPoly other2 = this.field.buildMonomial(degree, coefficient);
        genericGfPoly1 = genericGfPoly1.addOrSubtract(other2);
      }
      return new GenericGFPoly[2]
      {
        genericGfPoly1,
        genericGfPoly2
      };
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(8 * this.Degree);
      for (int degree = this.Degree; degree >= 0; --degree)
      {
        int a = this.getCoefficient(degree);
        if (a != 0)
        {
          if (a < 0)
          {
            stringBuilder.Append(" - ");
            a = -a;
          }
          else if (stringBuilder.Length > 0)
            stringBuilder.Append(" + ");
          if (degree == 0 || a != 1)
          {
            int num = this.field.log(a);
            switch (num)
            {
              case 0:
                stringBuilder.Append('1');
                break;
              case 1:
                stringBuilder.Append('a');
                break;
              default:
                stringBuilder.Append("a^");
                stringBuilder.Append(num);
                break;
            }
          }
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
