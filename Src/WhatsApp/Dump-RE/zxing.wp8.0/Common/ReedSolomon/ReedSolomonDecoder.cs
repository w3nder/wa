// Decompiled with JetBrains decompiler
// Type: ZXing.Common.ReedSolomon.ReedSolomonDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common.ReedSolomon
{
  /// <summary> <p>Implements Reed-Solomon decoding, as the name implies.</p>
  /// 
  /// <p>The algorithm will not be explained here, but the following references were helpful
  /// in creating this implementation:</p>
  /// 
  /// <ul>
  /// <li>Bruce Maggs.
  /// <a href="http://www.cs.cmu.edu/afs/cs.cmu.edu/project/pscico-guyb/realworld/www/rs_decode.ps">
  /// "Decoding Reed-Solomon Codes"</a> (see discussion of Forney's Formula)</li>
  /// <li>J.I. Hall. <a href="www.mth.msu.edu/~jhall/classes/codenotes/GRS.pdf">
  /// "Chapter 5. Generalized Reed-Solomon Codes"</a>
  /// (see discussion of Euclidean algorithm)</li>
  /// </ul>
  /// 
  /// <p>Much credit is due to William Rucklidge since portions of this code are an indirect
  /// port of his C++ Reed-Solomon implementation.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>William Rucklidge</author>
  /// <author>sanfordsquires</author>
  public sealed class ReedSolomonDecoder
  {
    private readonly GenericGF field;

    public ReedSolomonDecoder(GenericGF field) => this.field = field;

    /// <summary>
    ///   <p>Decodes given set of received codewords, which include both data and error-correction
    /// codewords. Really, this means it uses Reed-Solomon to detect and correct errors, in-place,
    /// in the input.</p>
    /// </summary>
    /// <param name="received">data and error-correction codewords</param>
    /// <param name="twoS">number of error-correction codewords available</param>
    /// <returns>false: decoding fails</returns>
    public bool decode(int[] received, int twoS)
    {
      GenericGFPoly genericGfPoly = new GenericGFPoly(this.field, received);
      int[] coefficients = new int[twoS];
      bool flag = true;
      for (int index = 0; index < twoS; ++index)
      {
        int at = genericGfPoly.evaluateAt(this.field.exp(index + this.field.GeneratorBase));
        coefficients[coefficients.Length - 1 - index] = at;
        if (at != 0)
          flag = false;
      }
      if (flag)
        return true;
      GenericGFPoly b = new GenericGFPoly(this.field, coefficients);
      GenericGFPoly[] genericGfPolyArray = this.runEuclideanAlgorithm(this.field.buildMonomial(twoS, 1), b, twoS);
      if (genericGfPolyArray == null)
        return false;
      int[] errorLocations = this.findErrorLocations(genericGfPolyArray[0]);
      if (errorLocations == null)
        return false;
      int[] errorMagnitudes = this.findErrorMagnitudes(genericGfPolyArray[1], errorLocations);
      for (int index1 = 0; index1 < errorLocations.Length; ++index1)
      {
        int index2 = received.Length - 1 - this.field.log(errorLocations[index1]);
        if (index2 < 0)
          return false;
        received[index2] = GenericGF.addOrSubtract(received[index2], errorMagnitudes[index1]);
      }
      return true;
    }

    internal GenericGFPoly[] runEuclideanAlgorithm(GenericGFPoly a, GenericGFPoly b, int R)
    {
      if (a.Degree < b.Degree)
      {
        GenericGFPoly genericGfPoly = a;
        a = b;
        b = genericGfPoly;
      }
      GenericGFPoly genericGfPoly1 = a;
      GenericGFPoly genericGfPoly2 = b;
      GenericGFPoly other1 = this.field.Zero;
      GenericGFPoly genericGfPoly3 = this.field.One;
      while (genericGfPoly2.Degree >= R / 2)
      {
        GenericGFPoly genericGfPoly4 = genericGfPoly1;
        GenericGFPoly other2 = other1;
        genericGfPoly1 = genericGfPoly2;
        other1 = genericGfPoly3;
        if (genericGfPoly1.isZero)
          return (GenericGFPoly[]) null;
        genericGfPoly2 = genericGfPoly4;
        GenericGFPoly genericGfPoly5 = this.field.Zero;
        int b1 = this.field.inverse(genericGfPoly1.getCoefficient(genericGfPoly1.Degree));
        int degree;
        int coefficient;
        for (; genericGfPoly2.Degree >= genericGfPoly1.Degree && !genericGfPoly2.isZero; genericGfPoly2 = genericGfPoly2.addOrSubtract(genericGfPoly1.multiplyByMonomial(degree, coefficient)))
        {
          degree = genericGfPoly2.Degree - genericGfPoly1.Degree;
          coefficient = this.field.multiply(genericGfPoly2.getCoefficient(genericGfPoly2.Degree), b1);
          genericGfPoly5 = genericGfPoly5.addOrSubtract(this.field.buildMonomial(degree, coefficient));
        }
        genericGfPoly3 = genericGfPoly5.multiply(other1).addOrSubtract(other2);
        if (genericGfPoly2.Degree >= genericGfPoly1.Degree)
          return (GenericGFPoly[]) null;
      }
      int coefficient1 = genericGfPoly3.getCoefficient(0);
      if (coefficient1 == 0)
        return (GenericGFPoly[]) null;
      int scalar = this.field.inverse(coefficient1);
      return new GenericGFPoly[2]
      {
        genericGfPoly3.multiply(scalar),
        genericGfPoly2.multiply(scalar)
      };
    }

    private int[] findErrorLocations(GenericGFPoly errorLocator)
    {
      int degree = errorLocator.Degree;
      if (degree == 1)
        return new int[1]{ errorLocator.getCoefficient(1) };
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

    private int[] findErrorMagnitudes(GenericGFPoly errorEvaluator, int[] errorLocations)
    {
      int length = errorLocations.Length;
      int[] errorMagnitudes = new int[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        int num1 = this.field.inverse(errorLocations[index1]);
        int a = 1;
        for (int index2 = 0; index2 < length; ++index2)
        {
          if (index1 != index2)
          {
            int num2 = this.field.multiply(errorLocations[index2], num1);
            int b = (num2 & 1) == 0 ? num2 | 1 : num2 & -2;
            a = this.field.multiply(a, b);
          }
        }
        errorMagnitudes[index1] = this.field.multiply(errorEvaluator.evaluateAt(num1), this.field.inverse(a));
        if (this.field.GeneratorBase != 0)
          errorMagnitudes[index1] = this.field.multiply(errorMagnitudes[index1], num1);
      }
      return errorMagnitudes;
    }
  }
}
