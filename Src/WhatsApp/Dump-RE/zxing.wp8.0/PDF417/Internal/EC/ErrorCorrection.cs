// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.EC.ErrorCorrection
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417.Internal.EC
{
  /// <summary>
  /// <p>PDF417 error correction implementation.</p>
  /// <p>This <a href="http://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction#Example">example</a>
  /// is quite useful in understanding the algorithm.</p>
  /// <author>Sean Owen</author>
  /// <see cref="T:ZXing.Common.ReedSolomon.ReedSolomonDecoder" />
  /// </summary>
  public sealed class ErrorCorrection
  {
    private readonly ModulusGF field;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.EC.ErrorCorrection" /> class.
    /// </summary>
    public ErrorCorrection() => this.field = ModulusGF.PDF417_GF;

    /// <summary>Decodes the specified received.</summary>
    /// <param name="received">The received.</param>
    /// <param name="numECCodewords">The num EC codewords.</param>
    /// <param name="erasures">The erasures.</param>
    /// <param name="errorLocationsCount">The error locations count.</param>
    /// <returns></returns>
    public bool decode(
      int[] received,
      int numECCodewords,
      int[] erasures,
      out int errorLocationsCount)
    {
      ModulusPoly modulusPoly1 = new ModulusPoly(this.field, received);
      int[] coefficients = new int[numECCodewords];
      bool flag = false;
      errorLocationsCount = 0;
      for (int a = numECCodewords; a > 0; --a)
      {
        int at = modulusPoly1.evaluateAt(this.field.exp(a));
        coefficients[numECCodewords - a] = at;
        if (at != 0)
          flag = true;
      }
      if (!flag)
        return true;
      ModulusPoly modulusPoly2 = this.field.One;
      foreach (int erasure in erasures)
      {
        ModulusPoly other = new ModulusPoly(this.field, new int[2]
        {
          this.field.subtract(0, this.field.exp(received.Length - 1 - erasure)),
          1
        });
        modulusPoly2 = modulusPoly2.multiply(other);
      }
      ModulusPoly b = new ModulusPoly(this.field, coefficients);
      ModulusPoly[] modulusPolyArray = this.runEuclideanAlgorithm(this.field.buildMonomial(numECCodewords, 1), b, numECCodewords);
      if (modulusPolyArray == null)
        return false;
      ModulusPoly errorLocator = modulusPolyArray[0];
      ModulusPoly errorEvaluator = modulusPolyArray[1];
      if (errorLocator == null || errorEvaluator == null)
        return false;
      int[] errorLocations = this.findErrorLocations(errorLocator);
      if (errorLocations == null)
        return false;
      int[] errorMagnitudes = this.findErrorMagnitudes(errorEvaluator, errorLocator, errorLocations);
      for (int index1 = 0; index1 < errorLocations.Length; ++index1)
      {
        int index2 = received.Length - 1 - this.field.log(errorLocations[index1]);
        if (index2 < 0)
          return false;
        received[index2] = this.field.subtract(received[index2], errorMagnitudes[index1]);
      }
      errorLocationsCount = errorLocations.Length;
      return true;
    }

    /// <summary>
    /// Runs the euclidean algorithm (Greatest Common Divisor) until r's degree is less than R/2
    /// </summary>
    /// <returns>The euclidean algorithm.</returns>
    private ModulusPoly[] runEuclideanAlgorithm(ModulusPoly a, ModulusPoly b, int R)
    {
      if (a.Degree < b.Degree)
      {
        ModulusPoly modulusPoly = a;
        a = b;
        b = modulusPoly;
      }
      ModulusPoly modulusPoly1 = a;
      ModulusPoly modulusPoly2 = b;
      ModulusPoly other1 = this.field.Zero;
      ModulusPoly modulusPoly3 = this.field.One;
      while (modulusPoly2.Degree >= R / 2)
      {
        ModulusPoly modulusPoly4 = modulusPoly1;
        ModulusPoly other2 = other1;
        modulusPoly1 = modulusPoly2;
        other1 = modulusPoly3;
        if (modulusPoly1.isZero)
          return (ModulusPoly[]) null;
        modulusPoly2 = modulusPoly4;
        ModulusPoly modulusPoly5 = this.field.Zero;
        int b1 = this.field.inverse(modulusPoly1.getCoefficient(modulusPoly1.Degree));
        int degree;
        int coefficient;
        for (; modulusPoly2.Degree >= modulusPoly1.Degree && !modulusPoly2.isZero; modulusPoly2 = modulusPoly2.subtract(modulusPoly1.multiplyByMonomial(degree, coefficient)))
        {
          degree = modulusPoly2.Degree - modulusPoly1.Degree;
          coefficient = this.field.multiply(modulusPoly2.getCoefficient(modulusPoly2.Degree), b1);
          modulusPoly5 = modulusPoly5.add(this.field.buildMonomial(degree, coefficient));
        }
        modulusPoly3 = modulusPoly5.multiply(other1).subtract(other2).getNegative();
      }
      int coefficient1 = modulusPoly3.getCoefficient(0);
      if (coefficient1 == 0)
        return (ModulusPoly[]) null;
      int scalar = this.field.inverse(coefficient1);
      return new ModulusPoly[2]
      {
        modulusPoly3.multiply(scalar),
        modulusPoly2.multiply(scalar)
      };
    }

    /// <summary>
    /// Finds the error locations as a direct application of Chien's search
    /// </summary>
    /// <returns>The error locations.</returns>
    /// <param name="errorLocator">Error locator.</param>
    private int[] findErrorLocations(ModulusPoly errorLocator)
    {
      int degree = errorLocator.Degree;
      int[] numArray = new int[degree];
      int index = 0;
      for (int a = 1; a < this.field.Size && index < degree; ++a)
      {
        if (errorLocator.evaluateAt(a) == 0)
        {
          numArray[index] = this.field.inverse(a);
          ++index;
        }
      }
      return index != degree ? (int[]) null : numArray;
    }

    /// <summary>
    /// Finds the error magnitudes by directly applying Forney's Formula
    /// </summary>
    /// <returns>The error magnitudes.</returns>
    /// <param name="errorEvaluator">Error evaluator.</param>
    /// <param name="errorLocator">Error locator.</param>
    /// <param name="errorLocations">Error locations.</param>
    private int[] findErrorMagnitudes(
      ModulusPoly errorEvaluator,
      ModulusPoly errorLocator,
      int[] errorLocations)
    {
      int degree = errorLocator.Degree;
      int[] coefficients = new int[degree];
      for (int index = 1; index <= degree; ++index)
        coefficients[degree - index] = this.field.multiply(index, errorLocator.getCoefficient(index));
      ModulusPoly modulusPoly = new ModulusPoly(this.field, coefficients);
      int length = errorLocations.Length;
      int[] errorMagnitudes = new int[length];
      for (int index = 0; index < length; ++index)
      {
        int a1 = this.field.inverse(errorLocations[index]);
        int a2 = this.field.subtract(0, errorEvaluator.evaluateAt(a1));
        int b = this.field.inverse(modulusPoly.evaluateAt(a1));
        errorMagnitudes[index] = this.field.multiply(a2, b);
      }
      return errorMagnitudes;
    }
  }
}
