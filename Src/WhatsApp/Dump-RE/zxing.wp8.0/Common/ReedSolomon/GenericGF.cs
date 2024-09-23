﻿// Decompiled with JetBrains decompiler
// Type: ZXing.Common.ReedSolomon.GenericGF
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common.ReedSolomon
{
  /// <summary>
  ///   <p>This class contains utility methods for performing mathematical operations over
  /// the Galois Fields. Operations use a given primitive polynomial in calculations.</p>
  ///   <p>Throughout this package, elements of the GF are represented as an {@code int}
  /// for convenience and speed (but at the cost of memory).
  ///   </p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class GenericGF
  {
    private const int INITIALIZATION_THRESHOLD = 0;
    public static GenericGF AZTEC_DATA_12 = new GenericGF(4201, 4096, 1);
    public static GenericGF AZTEC_DATA_10 = new GenericGF(1033, 1024, 1);
    public static GenericGF AZTEC_DATA_6 = new GenericGF(67, 64, 1);
    public static GenericGF AZTEC_PARAM = new GenericGF(19, 16, 1);
    public static GenericGF QR_CODE_FIELD_256 = new GenericGF(285, 256, 0);
    public static GenericGF DATA_MATRIX_FIELD_256 = new GenericGF(301, 256, 1);
    public static GenericGF AZTEC_DATA_8 = GenericGF.DATA_MATRIX_FIELD_256;
    public static GenericGF MAXICODE_FIELD_64 = GenericGF.AZTEC_DATA_6;
    private int[] expTable;
    private int[] logTable;
    private GenericGFPoly zero;
    private GenericGFPoly one;
    private readonly int size;
    private readonly int primitive;
    private readonly int generatorBase;
    private bool initialized;

    /// <summary>
    /// Create a representation of GF(size) using the given primitive polynomial.
    /// </summary>
    /// <param name="primitive">irreducible polynomial whose coefficients are represented by
    /// *  the bits of an int, where the least-significant bit represents the constant
    /// *  coefficient</param>
    /// <param name="size">the size of the field</param>
    /// <param name="genBase">the factor b in the generator polynomial can be 0- or 1-based
    /// *  (g(x) = (x+a^b)(x+a^(b+1))...(x+a^(b+2t-1))).
    /// *  In most cases it should be 1, but for QR code it is 0.</param>
    public GenericGF(int primitive, int size, int genBase)
    {
      this.primitive = primitive;
      this.size = size;
      this.generatorBase = genBase;
      if (size > 0)
        return;
      this.initialize();
    }

    private void initialize()
    {
      this.expTable = new int[this.size];
      this.logTable = new int[this.size];
      int num = 1;
      for (int index = 0; index < this.size; ++index)
      {
        this.expTable[index] = num;
        num <<= 1;
        if (num >= this.size)
          num = (num ^ this.primitive) & this.size - 1;
      }
      for (int index = 0; index < this.size - 1; ++index)
        this.logTable[this.expTable[index]] = index;
      this.zero = new GenericGFPoly(this, new int[1]);
      this.one = new GenericGFPoly(this, new int[1]{ 1 });
      this.initialized = true;
    }

    private void checkInit()
    {
      if (this.initialized)
        return;
      this.initialize();
    }

    internal GenericGFPoly Zero
    {
      get
      {
        this.checkInit();
        return this.zero;
      }
    }

    internal GenericGFPoly One
    {
      get
      {
        this.checkInit();
        return this.one;
      }
    }

    /// <summary>Builds the monomial.</summary>
    /// <param name="degree">The degree.</param>
    /// <param name="coefficient">The coefficient.</param>
    /// <returns>the monomial representing coefficient * x^degree</returns>
    internal GenericGFPoly buildMonomial(int degree, int coefficient)
    {
      this.checkInit();
      if (degree < 0)
        throw new ArgumentException();
      if (coefficient == 0)
        return this.zero;
      int[] coefficients = new int[degree + 1];
      coefficients[0] = coefficient;
      return new GenericGFPoly(this, coefficients);
    }

    /// <summary>
    /// Implements both addition and subtraction -- they are the same in GF(size).
    /// </summary>
    /// <returns>sum/difference of a and b</returns>
    internal static int addOrSubtract(int a, int b) => a ^ b;

    /// <summary>Exps the specified a.</summary>
    /// <returns>2 to the power of a in GF(size)</returns>
    internal int exp(int a)
    {
      this.checkInit();
      return this.expTable[a];
    }

    /// <summary>Logs the specified a.</summary>
    /// <param name="a">A.</param>
    /// <returns>base 2 log of a in GF(size)</returns>
    internal int log(int a)
    {
      this.checkInit();
      return a != 0 ? this.logTable[a] : throw new ArgumentException();
    }

    /// <summary>Inverses the specified a.</summary>
    /// <returns>multiplicative inverse of a</returns>
    internal int inverse(int a)
    {
      this.checkInit();
      if (a == 0)
        throw new ArithmeticException();
      return this.expTable[this.size - this.logTable[a] - 1];
    }

    /// <summary>Multiplies the specified a with b.</summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <returns>product of a and b in GF(size)</returns>
    internal int multiply(int a, int b)
    {
      this.checkInit();
      return a == 0 || b == 0 ? 0 : this.expTable[(this.logTable[a] + this.logTable[b]) % (this.size - 1)];
    }

    /// <summary>Gets the size.</summary>
    public int Size => this.size;

    /// <summary>Gets the generator base.</summary>
    public int GeneratorBase => this.generatorBase;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return "GF(0x" + this.primitive.ToString("X") + (object) ',' + (object) this.size + (object) ')';
    }
  }
}
