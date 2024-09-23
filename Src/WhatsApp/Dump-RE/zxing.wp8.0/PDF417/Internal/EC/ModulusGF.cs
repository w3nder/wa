// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.EC.ModulusGF
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.PDF417.Internal.EC
{
  /// <summary>
  /// <p>A field based on powers of a generator integer, modulo some modulus.</p>
  /// @see com.google.zxing.common.reedsolomon.GenericGF
  /// </summary>
  /// <author>Sean Owen</author>
  internal sealed class ModulusGF
  {
    public static ModulusGF PDF417_GF = new ModulusGF(PDF417Common.NUMBER_OF_CODEWORDS, 3);
    private readonly int[] expTable;
    private readonly int[] logTable;
    private readonly int modulus;

    public ModulusPoly Zero { get; private set; }

    public ModulusPoly One { get; private set; }

    public ModulusGF(int modulus, int generator)
    {
      this.modulus = modulus;
      this.expTable = new int[modulus];
      this.logTable = new int[modulus];
      int num = 1;
      for (int index = 0; index < modulus; ++index)
      {
        this.expTable[index] = num;
        num = num * generator % modulus;
      }
      for (int index = 0; index < modulus - 1; ++index)
        this.logTable[this.expTable[index]] = index;
      this.Zero = new ModulusPoly(this, new int[1]);
      this.One = new ModulusPoly(this, new int[1]{ 1 });
    }

    internal ModulusPoly buildMonomial(int degree, int coefficient)
    {
      if (degree < 0)
        throw new ArgumentException();
      if (coefficient == 0)
        return this.Zero;
      int[] coefficients = new int[degree + 1];
      coefficients[0] = coefficient;
      return new ModulusPoly(this, coefficients);
    }

    internal int add(int a, int b) => (a + b) % this.modulus;

    internal int subtract(int a, int b) => (this.modulus + a - b) % this.modulus;

    internal int exp(int a) => this.expTable[a];

    internal int log(int a) => a != 0 ? this.logTable[a] : throw new ArgumentException();

    internal int inverse(int a)
    {
      if (a == 0)
        throw new ArithmeticException();
      return this.expTable[this.modulus - this.logTable[a] - 1];
    }

    internal int multiply(int a, int b)
    {
      return a == 0 || b == 0 ? 0 : this.expTable[(this.logTable[a] + this.logTable[b]) % (this.modulus - 1)];
    }

    internal int Size => this.modulus;
  }
}
