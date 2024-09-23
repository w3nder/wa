// Decompiled with JetBrains decompiler
// Type: ZXing.Common.ReedSolomon.ReedSolomonEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ZXing.Common.ReedSolomon
{
  /// <summary>
  /// Implements Reed-Solomon encoding, as the name implies.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>William Rucklidge</author>
  public sealed class ReedSolomonEncoder
  {
    private readonly GenericGF field;
    private readonly IList<GenericGFPoly> cachedGenerators;

    public ReedSolomonEncoder(GenericGF field)
    {
      this.field = field;
      this.cachedGenerators = (IList<GenericGFPoly>) new List<GenericGFPoly>();
      this.cachedGenerators.Add(new GenericGFPoly(field, new int[1]
      {
        1
      }));
    }

    private GenericGFPoly buildGenerator(int degree)
    {
      if (degree >= this.cachedGenerators.Count)
      {
        GenericGFPoly genericGfPoly1 = this.cachedGenerators[this.cachedGenerators.Count - 1];
        for (int count = this.cachedGenerators.Count; count <= degree; ++count)
        {
          GenericGFPoly genericGfPoly2 = genericGfPoly1.multiply(new GenericGFPoly(this.field, new int[2]
          {
            1,
            this.field.exp(count - 1 + this.field.GeneratorBase)
          }));
          this.cachedGenerators.Add(genericGfPoly2);
          genericGfPoly1 = genericGfPoly2;
        }
      }
      return this.cachedGenerators[degree];
    }

    public void encode(int[] toEncode, int ecBytes)
    {
      if (ecBytes == 0)
        throw new ArgumentException("No error correction bytes");
      int length = toEncode.Length - ecBytes;
      if (length <= 0)
        throw new ArgumentException("No data bytes provided");
      GenericGFPoly other = this.buildGenerator(ecBytes);
      int[] numArray = new int[length];
      Array.Copy((Array) toEncode, 0, (Array) numArray, 0, length);
      int[] coefficients = new GenericGFPoly(this.field, numArray).multiplyByMonomial(ecBytes, 1).divide(other)[1].Coefficients;
      int num = ecBytes - coefficients.Length;
      for (int index = 0; index < num; ++index)
        toEncode[length + index] = 0;
      Array.Copy((Array) coefficients, 0, (Array) toEncode, length + num, coefficients.Length);
    }
  }
}
