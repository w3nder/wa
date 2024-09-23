// Decompiled with JetBrains decompiler
// Type: BigIntegerLibrary.BigInteger
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace BigIntegerLibrary
{
  /// <summary>
  /// .NET 2.0 class for handling of very large integers, up to 10240 binary digits or
  /// approximately (safe to use) 3000 decimal digits.
  /// </summary>
  internal sealed class BigInteger : IEquatable<BigInteger>, IComparable, IComparable<BigInteger>
  {
    /// <summary>
    /// 2^16 numeration base for internal computations, in order to benefit the most from the
    /// 32 bit (or 64 bit) integer processor registers.
    /// </summary>
    private const long NumberBase = 65536;
    /// <summary>
    /// Maximum size for numbers is up to 10240 binary digits or approximately (safe to use) 3000 decimal digits.
    /// The maximum size is, in fact, double the previously specified amount, in order to accommodate operations's
    /// overflow.
    /// </summary>
    internal const int MaxSize = 1280;
    /// <summary>
    /// Ratio for the convertion of a BigInteger's size to a binary digits size.
    /// </summary>
    private const int RatioToBinaryDigits = 16;
    /// Integer constants
    public static readonly BigInteger Zero = new BigInteger();
    public static readonly BigInteger One = new BigInteger(1L);
    public static readonly BigInteger Two = new BigInteger(2L);
    public static readonly BigInteger Ten = new BigInteger(10L);
    /// <summary>The array of digits of the number.</summary>
    private BigInteger.DigitContainer digits;
    /// <summary>The actual number of digits of the number.</summary>
    private int size;
    /// <summary>The number sign.</summary>
    private Sign sign;

    /// <summary>
    /// Default constructor, intializing the BigInteger with zero.
    /// </summary>
    public BigInteger()
    {
      this.digits = new BigInteger.DigitContainer();
      this.size = 1;
      this.digits[this.size] = 0L;
      this.sign = Sign.Positive;
    }

    /// <summary>
    /// Constructor creating a new BigInteger as a conversion of a regular base-10 long.
    /// </summary>
    /// <param name="n">The base-10 long to be converted</param>
    public BigInteger(long n)
    {
      this.digits = new BigInteger.DigitContainer();
      this.sign = Sign.Positive;
      if (n == 0L)
      {
        this.size = 1;
        this.digits[this.size] = 0L;
      }
      else
      {
        if (n < 0L)
        {
          n = -n;
          this.sign = Sign.Negative;
        }
        this.size = 0;
        while (n > 0L)
        {
          this.digits[this.size] = n % 65536L;
          n /= 65536L;
          ++this.size;
        }
      }
    }

    /// <summary>
    /// Constructor creating a new BigInteger as a copy of an existing BigInteger.
    /// </summary>
    /// <param name="n">The BigInteger to be copied</param>
    public BigInteger(BigInteger n)
    {
      this.digits = new BigInteger.DigitContainer();
      this.size = n.size;
      this.sign = n.sign;
      for (int index = 0; index < n.size; ++index)
        this.digits[index] = n.digits[index];
    }

    /// <summary>
    /// Constructor creating a BigInteger instance out of a base-10 formatted string.
    /// </summary>
    /// <param name="numberString">The base-10 formatted string.</param>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Invalid numeric string exception</exception>
    public BigInteger(string numberString)
    {
      BigInteger bigInteger = new BigInteger();
      Sign sign = Sign.Positive;
      for (int index = 0; index < numberString.Length; ++index)
      {
        if (numberString[index] < '0' || numberString[index] > '9')
        {
          if (index != 0 || numberString[index] != '-')
            throw new BigIntegerException("Invalid numeric string.", (Exception) null);
          sign = Sign.Negative;
        }
        else
          bigInteger = bigInteger * BigInteger.Ten + (BigInteger) long.Parse(numberString[index].ToString());
      }
      this.sign = sign;
      this.digits = new BigInteger.DigitContainer();
      this.size = bigInteger.size;
      for (int index = 0; index < bigInteger.size; ++index)
        this.digits[index] = bigInteger.digits[index];
    }

    /// <summary>
    /// Constructor creating a positive BigInteger by extracting it's digits from a given byte array.
    /// </summary>
    /// <param name="byteArray">The byte array</param>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">The byte array's content exceeds the maximum size of a BigInteger
    /// exception</exception>
    public BigInteger(byte[] byteArray)
    {
      if (byteArray.Length / 4 > 1280)
        throw new BigIntegerException("The byte array's content exceeds the maximum size of a BigInteger.", (Exception) null);
      this.digits = new BigInteger.DigitContainer();
      this.sign = Sign.Positive;
      for (int index = 0; index < byteArray.Length; index += 2)
      {
        int num = (int) byteArray[index];
        if (index + 1 < byteArray.Length)
          num = (num << 8) + (int) byteArray[index + 1];
        this.digits[this.size++] = (long) num;
      }
      bool flag = true;
      while (this.size - 1 > 0 && flag)
      {
        if (this.digits[this.size - 1] == 0L)
          --this.size;
        else
          flag = false;
      }
    }

    /// <summary>
    /// Determines whether the specified BigInteger is equal to the current BigInteger.
    /// </summary>
    /// <param name="other">The BigInteger to compare with the current BigInteger</param>
    /// <returns>True if the specified BigInteger is equal to the current BigInteger,
    /// false otherwise</returns>
    public bool Equals(BigInteger other)
    {
      if (this.sign != other.sign || this.size != other.size)
        return false;
      for (int index = 0; index < this.size; ++index)
      {
        if (this.digits[index] != other.digits[index])
          return false;
      }
      return true;
    }

    /// <summary>
    /// Determines whether the specified System.Object is equal to the current BigInteger.
    /// </summary>
    /// <param name="o">The System.Object to compare with the current BigInteger</param>
    /// <returns>True if the specified System.Object is equal to the current BigInteger,
    /// false otherwise</returns>
    public override bool Equals(object o)
    {
      return (object) (o as BigInteger) != null && this.Equals((BigInteger) o);
    }

    /// <summary>Serves as a hash function for the BigInteger type.</summary>
    /// <returns>A hash code for the current BigInteger</returns>
    public override int GetHashCode()
    {
      int hashCode = 0;
      for (int index = 0; index < this.size; ++index)
        hashCode += (int) this.digits[index];
      return hashCode;
    }

    /// <summary>
    /// String representation of the current BigInteger, converted to its base-10 representation.
    /// </summary>
    /// <returns>The string representation of the current BigInteger</returns>
    public override string ToString()
    {
      Base10BigInteger base10BigInteger1 = new Base10BigInteger();
      Base10BigInteger base10BigInteger2 = new Base10BigInteger(1L);
      for (int index = 0; index < this.size; ++index)
      {
        base10BigInteger1 += (Base10BigInteger) this.digits[index] * base10BigInteger2;
        base10BigInteger2 *= (Base10BigInteger) 65536L;
      }
      base10BigInteger1.NumberSign = this.sign;
      return base10BigInteger1.ToString();
    }

    /// <summary>Parses the number given by a string</summary>
    /// <param name="str">the number as a string</param>
    /// <returns></returns>
    public static BigInteger Parse(string str) => new BigInteger(str);

    /// <summary>Compares this instance to a specified BigInteger.</summary>
    /// <param name="other">The BigInteger to compare this instance with</param>
    /// <returns>-1 if the current instance is smaller than the given BigInteger,
    /// 0 if the two are equal, 1 otherwise</returns>
    public int CompareTo(BigInteger other)
    {
      if (BigInteger.Greater(this, other))
        return 1;
      return object.Equals((object) this, (object) other) ? 0 : -1;
    }

    /// <summary>Compares this instance to a specified object.</summary>
    /// <param name="obj">The object to compare this instance with</param>
    /// <returns>-1 if the current instance is smaller than the given object,
    /// 0 if the two are equal, 1 otherwise</returns>
    /// <exception cref="T:System.ArgumentException">obj is not a BigInteger exception</exception>
    public int CompareTo(object obj)
    {
      return (object) (obj as BigInteger) != null ? this.CompareTo((BigInteger) obj) : throw new ArgumentException("obj is not a BigInteger.");
    }

    /// <summary>Returns a BigInteger's size in binary digits.</summary>
    /// <param name="n">The BigInteger whose size in binary digits is to be determined</param>
    /// <returns>The BigInteger's size in binary digits</returns>
    public static int SizeInBinaryDigits(BigInteger n) => n.size * 16;

    /// <summary>BigInteger inverse with respect to addition.</summary>
    /// <param name="n">The BigInteger whose opposite is to be computed</param>
    /// <returns>The BigInteger inverse with respect to addition</returns>
    public static BigInteger Opposite(BigInteger n)
    {
      BigInteger bigInteger = new BigInteger(n);
      if (bigInteger != BigInteger.Zero)
        bigInteger.sign = bigInteger.sign != Sign.Positive ? Sign.Positive : Sign.Negative;
      return bigInteger;
    }

    /// <summary>Greater test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &gt; b, false otherwise</returns>
    public static bool Greater(BigInteger a, BigInteger b)
    {
      if (a.sign != b.sign)
        return (a.sign != Sign.Negative || b.sign != Sign.Positive) && a.sign == Sign.Positive && b.sign == Sign.Negative;
      if (a.sign == Sign.Positive)
      {
        if (a.size > b.size)
          return true;
        if (a.size < b.size)
          return false;
        for (int index = a.size - 1; index >= 0; --index)
        {
          if (a.digits[index] > b.digits[index])
            return true;
          if (a.digits[index] < b.digits[index])
            return false;
        }
      }
      else
      {
        if (a.size < b.size)
          return true;
        if (a.size > b.size)
          return false;
        for (int index = a.size - 1; index >= 0; --index)
        {
          if (a.digits[index] < b.digits[index])
            return true;
          if (a.digits[index] > b.digits[index])
            return false;
        }
      }
      return false;
    }

    /// <summary>Greater or equal test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &gt;= b, false otherwise</returns>
    public static bool GreaterOrEqual(BigInteger a, BigInteger b)
    {
      return BigInteger.Greater(a, b) || object.Equals((object) a, (object) b);
    }

    /// <summary>Smaller test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &lt; b, false otherwise</returns>
    public static bool Smaller(BigInteger a, BigInteger b) => !BigInteger.GreaterOrEqual(a, b);

    /// <summary>Smaller or equal test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &lt;= b, false otherwise</returns>
    public static bool SmallerOrEqual(BigInteger a, BigInteger b) => !BigInteger.Greater(a, b);

    /// <summary>Computes the absolute value of a BigInteger.</summary>
    /// <param name="n">The BigInteger whose absolute value is to be computed</param>
    /// <returns>The absolute value of the given BigInteger</returns>
    public static BigInteger Abs(BigInteger n)
    {
      return new BigInteger(n) { sign = Sign.Positive };
    }

    /// <summary>Addition operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the addition</returns>
    public static BigInteger Addition(BigInteger a, BigInteger b)
    {
      BigInteger bigInteger = (BigInteger) null;
      if (a.sign == Sign.Positive && b.sign == Sign.Positive)
      {
        bigInteger = !(a >= b) ? BigInteger.Add(b, a) : BigInteger.Add(a, b);
        bigInteger.sign = Sign.Positive;
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Negative)
      {
        bigInteger = !(a <= b) ? BigInteger.Add(-b, -a) : BigInteger.Add(-a, -b);
        bigInteger.sign = Sign.Negative;
      }
      if (a.sign == Sign.Positive && b.sign == Sign.Negative)
      {
        if (a >= -b)
        {
          bigInteger = BigInteger.Subtract(a, -b);
          bigInteger.sign = Sign.Positive;
        }
        else
        {
          bigInteger = BigInteger.Subtract(-b, a);
          bigInteger.sign = Sign.Negative;
        }
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Positive)
      {
        if (-a <= b)
        {
          bigInteger = BigInteger.Subtract(b, -a);
          bigInteger.sign = Sign.Positive;
        }
        else
        {
          bigInteger = BigInteger.Subtract(-a, b);
          bigInteger.sign = Sign.Negative;
        }
      }
      return bigInteger;
    }

    /// <summary>Subtraction operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the subtraction</returns>
    public static BigInteger Subtraction(BigInteger a, BigInteger b)
    {
      BigInteger bigInteger = (BigInteger) null;
      if (a.sign == Sign.Positive && b.sign == Sign.Positive)
      {
        if (a >= b)
        {
          bigInteger = BigInteger.Subtract(a, b);
          bigInteger.sign = Sign.Positive;
        }
        else
        {
          bigInteger = BigInteger.Subtract(b, a);
          bigInteger.sign = Sign.Negative;
        }
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Negative)
      {
        if (a <= b)
        {
          bigInteger = BigInteger.Subtract(-a, -b);
          bigInteger.sign = Sign.Negative;
        }
        else
        {
          bigInteger = BigInteger.Subtract(-b, -a);
          bigInteger.sign = Sign.Positive;
        }
      }
      if (a.sign == Sign.Positive && b.sign == Sign.Negative)
      {
        bigInteger = !(a >= -b) ? BigInteger.Add(-b, a) : BigInteger.Add(a, -b);
        bigInteger.sign = Sign.Positive;
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Positive)
      {
        bigInteger = !(-a >= b) ? BigInteger.Add(b, -a) : BigInteger.Add(-a, b);
        bigInteger.sign = Sign.Negative;
      }
      return bigInteger;
    }

    /// <summary>Multiplication operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the multiplication</returns>
    public static BigInteger Multiplication(BigInteger a, BigInteger b)
    {
      if (a == BigInteger.Zero || b == BigInteger.Zero)
        return BigInteger.Zero;
      BigInteger bigInteger = BigInteger.Multiply(BigInteger.Abs(a), BigInteger.Abs(b));
      bigInteger.sign = a.sign != b.sign ? Sign.Negative : Sign.Positive;
      return bigInteger;
    }

    /// <summary>
    /// Division operation of two BigIntegers a and b, b != 0.
    /// </summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the division</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot divide by zero exception</exception>
    public static BigInteger Division(BigInteger a, BigInteger b)
    {
      if (b == BigInteger.Zero)
        throw new BigIntegerException("Cannot divide by zero.", (Exception) new DivideByZeroException());
      if (a == BigInteger.Zero || BigInteger.Abs(a) < BigInteger.Abs(b))
        return BigInteger.Zero;
      BigInteger bigInteger = b.size != 1 ? BigInteger.DivideByBigNumber(BigInteger.Abs(a), BigInteger.Abs(b)) : BigInteger.DivideByOneDigitNumber(BigInteger.Abs(a), b.digits[0]);
      bigInteger.sign = a.sign != b.sign ? Sign.Negative : Sign.Positive;
      return bigInteger;
    }

    /// <summary>Modulo operation of two BigIntegers a and b, b != 0.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the modulo</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot divide by zero exception</exception>
    public static BigInteger Modulo(BigInteger a, BigInteger b)
    {
      if (b == BigInteger.Zero)
        throw new BigIntegerException("Cannot divide by zero.", (Exception) new DivideByZeroException());
      return BigInteger.Abs(a) < BigInteger.Abs(b) ? new BigInteger(a) : a - a / b * b;
    }

    /// <summary>
    /// Returns the power of a BigInteger base to a non-negative exponent by using the
    /// fast exponentiation algorithm (right to left binary exponentiation).
    /// </summary>
    /// <param name="a">The BigInteger base</param>
    /// <param name="exponent">The non-negative exponent</param>
    /// <returns>The power of the BigInteger base to the non-negative exponent</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot raise a BigInteger to a negative power exception.</exception>
    public static BigInteger Power(BigInteger a, int exponent)
    {
      if (exponent < 0)
        throw new BigIntegerException("Cannot raise an BigInteger to a negative power.", (Exception) null);
      if (a == BigInteger.Zero)
        return new BigInteger();
      BigInteger bigInteger1 = new BigInteger(1L);
      if (exponent == 0)
        return bigInteger1;
      BigInteger bigInteger2 = new BigInteger(a);
      while (exponent > 0)
      {
        if (exponent % 2 == 1)
          bigInteger1 *= bigInteger2;
        exponent /= 2;
        if (exponent > 0)
          bigInteger2 *= bigInteger2;
      }
      return bigInteger1;
    }

    /// <summary>
    /// Integer square root of the given BigInteger using Newton's numeric method.
    /// </summary>
    /// <param name="n">The BigInteger whose integer square root is to be computed</param>
    /// <returns>The integer square root of the given BigInteger</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot compute the integer square root of a negative number exception</exception>
    public static BigInteger IntegerSqrt(BigInteger n)
    {
      if (n.sign == Sign.Negative)
        throw new BigIntegerException("Cannot compute the integer square root of a negative number.", (Exception) null);
      BigInteger bigInteger1 = new BigInteger(0L);
      BigInteger bigInteger2;
      for (bigInteger2 = new BigInteger(n); BigInteger.Abs(bigInteger2 - bigInteger1) >= BigInteger.One; bigInteger2 = (bigInteger1 + n / bigInteger1) / BigInteger.Two)
        bigInteger1 = bigInteger2;
      return bigInteger2;
    }

    /// <summary>
    /// Euclidean algorithm for computing the greatest common divisor of two non-negative BigIntegers.
    /// </summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The greatest common divisor of the two given BigIntegers</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot compute the Gcd of negative BigIntegers exception</exception>
    public static BigInteger Gcd(BigInteger a, BigInteger b)
    {
      if (a.sign == Sign.Negative || b.sign == Sign.Negative)
        throw new BigIntegerException("Cannot compute the Gcd of negative BigIntegers.", (Exception) null);
      BigInteger bigInteger;
      for (; b > BigInteger.Zero; b = bigInteger)
      {
        bigInteger = a % b;
        a = b;
      }
      return a;
    }

    /// <summary>
    /// Extended Euclidian Gcd algorithm, returning the greatest common divisor of two non-negative BigIntegers,
    /// while also providing u and v, where: a*u + b*v = gcd(a,b).
    /// </summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <param name="u">Output BigInteger parameter, where a*u + b*v = gcd(a,b)</param>
    /// <param name="v">Output BigInteger parameter, where a*u + b*v = gcd(a,b)</param>
    /// <returns>The greatest common divisor of the two given BigIntegers</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot compute the Gcd of negative BigIntegers exception</exception>
    public static BigInteger ExtendedEuclidGcd(
      BigInteger a,
      BigInteger b,
      out BigInteger u,
      out BigInteger v)
    {
      if (a.sign == Sign.Negative || b.sign == Sign.Negative)
        throw new BigIntegerException("Cannot compute the Gcd of negative BigIntegers.", (Exception) null);
      BigInteger n1 = new BigInteger();
      BigInteger n2 = new BigInteger(1L);
      BigInteger n3 = new BigInteger(1L);
      BigInteger n4 = new BigInteger();
      u = new BigInteger();
      v = new BigInteger();
      while (b > BigInteger.Zero)
      {
        BigInteger bigInteger = a / b;
        BigInteger n5 = a - bigInteger * b;
        u = n2 - bigInteger * n1;
        v = n4 - bigInteger * n3;
        a = new BigInteger(b);
        b = new BigInteger(n5);
        n2 = new BigInteger(n1);
        n1 = new BigInteger(u);
        n4 = new BigInteger(n3);
        n3 = new BigInteger(v);
        u = new BigInteger(n2);
        v = new BigInteger(n4);
      }
      return a;
    }

    /// <summary>Computes the modular inverse of a given BigInteger.</summary>
    /// <param name="a">The non-zero BigInteger whose inverse is to be computed</param>
    /// <param name="n">The BigInteger modulus, which must be greater than or equal to 2</param>
    /// <returns>The BigInteger equal to a^(-1) mod n</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Invalid number or modulus exception</exception>
    public static BigInteger ModularInverse(BigInteger a, BigInteger n)
    {
      if (n < BigInteger.Two)
        throw new BigIntegerException("Cannot perform a modulo operation against a BigInteger less than 2.", (Exception) null);
      if (BigInteger.Abs(a) >= n)
        a %= n;
      if (a.sign == Sign.Negative)
        a += n;
      if (a == BigInteger.Zero)
        throw new BigIntegerException("Cannot obtain the modular inverse of 0.", (Exception) null);
      if (BigInteger.Gcd(a, n) != BigInteger.One)
        throw new BigIntegerException("Cannot obtain the modular inverse of a number that is not coprime with the modulus.", (Exception) null);
      BigInteger v;
      BigInteger.ExtendedEuclidGcd(n, a, out BigInteger _, out v);
      if (v.sign == Sign.Negative)
        v += n;
      return v;
    }

    /// <summary>
    /// Returns the power of a BigInteger to a non-negative exponent modulo n, by using the
    /// fast exponentiation algorithm (right to left binary exponentiation) and modulo optimizations.
    /// </summary>
    /// <param name="a">The BigInteger base</param>
    /// <param name="exponent">The non-negative exponent</param>
    /// <param name="n">The modulus, which must be greater than or equal to 2</param>
    /// <returns>The power of the BigInteger to the non-negative exponent</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Invalid exponent or modulus exception</exception>
    public static BigInteger ModularExponentiation(BigInteger a, BigInteger exponent, BigInteger n)
    {
      if (exponent < (BigInteger) 0L)
        throw new BigIntegerException("Cannot raise a BigInteger to a negative power.", (Exception) null);
      if (n < BigInteger.Two)
        throw new BigIntegerException("Cannot perform a modulo operation against a BigInteger less than 2.", (Exception) null);
      if (BigInteger.Abs(a) >= n)
        a %= n;
      if (a.sign == Sign.Negative)
        a += n;
      if (a == BigInteger.Zero)
        return new BigInteger();
      BigInteger bigInteger1 = new BigInteger(1L);
      BigInteger bigInteger2 = new BigInteger(a);
      while (exponent > BigInteger.Zero)
      {
        if (exponent % BigInteger.Two == BigInteger.One)
          bigInteger1 = bigInteger1 * bigInteger2 % n;
        exponent /= BigInteger.Two;
        bigInteger2 = bigInteger2 * bigInteger2 % n;
      }
      return bigInteger1;
    }

    /// <summary>Implicit conversion operator from long to BigInteger.</summary>
    /// <param name="n">The long to be converted to a BigInteger</param>
    /// <returns>The BigInteger converted from the given long</returns>
    public static implicit operator BigInteger(long n) => new BigInteger(n);

    /// <summary>Equality test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a == b, false otherwise</returns>
    public static bool operator ==(BigInteger a, BigInteger b)
    {
      return object.Equals((object) a, (object) b);
    }

    /// <summary>Inequality test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a != b, false otherwise</returns>
    public static bool operator !=(BigInteger a, BigInteger b)
    {
      return !object.Equals((object) a, (object) b);
    }

    /// <summary>Greater test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &gt; b, false otherwise</returns>
    public static bool operator >(BigInteger a, BigInteger b) => BigInteger.Greater(a, b);

    /// <summary>Smaller test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &lt; b, false otherwise</returns>
    public static bool operator <(BigInteger a, BigInteger b) => BigInteger.Smaller(a, b);

    /// <summary>Greater or equal test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &gt;= b, false otherwise</returns>
    public static bool operator >=(BigInteger a, BigInteger b) => BigInteger.GreaterOrEqual(a, b);

    /// <summary>Smaller or equal test between two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>True if a &lt;= b, false otherwise</returns>
    public static bool operator <=(BigInteger a, BigInteger b) => BigInteger.SmallerOrEqual(a, b);

    /// <summary>BigInteger inverse with respect to addition.</summary>
    /// <param name="n">The BigInteger whose opposite is to be computed</param>
    /// <returns>The BigInteger inverse with respect to addition</returns>
    public static BigInteger operator -(BigInteger n) => BigInteger.Opposite(n);

    /// <summary>Addition operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the addition</returns>
    public static BigInteger operator +(BigInteger a, BigInteger b) => BigInteger.Addition(a, b);

    /// <summary>Subtraction operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the subtraction</returns>
    public static BigInteger operator -(BigInteger a, BigInteger b) => BigInteger.Subtraction(a, b);

    /// <summary>Multiplication operation of two BigIntegers.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the multiplication</returns>
    public static BigInteger operator *(BigInteger a, BigInteger b)
    {
      return BigInteger.Multiplication(a, b);
    }

    /// <summary>
    /// Division operation of two BigIntegers a and b, b != 0.
    /// </summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the division</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot divide by zero exception</exception>
    public static BigInteger operator /(BigInteger a, BigInteger b) => BigInteger.Division(a, b);

    /// <summary>Modulo operation of two BigIntegers a and b, b != 0.</summary>
    /// <param name="a">The 1st BigInteger</param>
    /// <param name="b">The 2nd BigInteger</param>
    /// <returns>The BigInteger result of the modulo</returns>
    /// <exception cref="T:BigIntegerLibrary.BigIntegerException">Cannot divide by zero exception</exception>
    public static BigInteger operator %(BigInteger a, BigInteger b) => BigInteger.Modulo(a, b);

    /// <summary>Incremetation by one operation of a BigInteger.</summary>
    /// <param name="n">The BigInteger to be incremented by one</param>
    /// <returns>The BigInteger result of incrementing by one</returns>
    public static BigInteger operator ++(BigInteger n) => n + BigInteger.One;

    /// <summary>Decremetation by one operation of a BigInteger.</summary>
    /// <param name="n">The BigInteger to be decremented by one</param>
    /// <returns>The BigInteger result of decrementing by one</returns>
    public static BigInteger operator --(BigInteger n) => n - BigInteger.One;

    /// <summary>
    /// Adds two BigNumbers a and b, where a &gt;= b, a, b non-negative.
    /// </summary>
    private static BigInteger Add(BigInteger a, BigInteger b)
    {
      BigInteger bigInteger = new BigInteger(a);
      long num1 = 0;
      for (int index = 0; index < b.size; ++index)
      {
        long num2 = bigInteger.digits[index] + b.digits[index] + num1;
        bigInteger.digits[index] = num2 % 65536L;
        num1 = num2 / 65536L;
      }
      for (int size = b.size; size < a.size && num1 > 0L; ++size)
      {
        long num3 = bigInteger.digits[size] + num1;
        bigInteger.digits[size] = num3 % 65536L;
        num1 = num3 / 65536L;
      }
      if (num1 > 0L)
      {
        bigInteger.digits[bigInteger.size] = num1 % 65536L;
        ++bigInteger.size;
        long num4 = num1 / 65536L;
      }
      return bigInteger;
    }

    /// <summary>
    /// Subtracts the BigInteger b from the BigInteger a, where a &gt;= b, a, b non-negative.
    /// </summary>
    private static BigInteger Subtract(BigInteger a, BigInteger b)
    {
      BigInteger bigInteger = new BigInteger(a);
      long num1 = 0;
      bool flag = true;
      for (int index = 0; index < b.size; ++index)
      {
        long num2 = bigInteger.digits[index] - b.digits[index] - num1;
        if (num2 < 0L)
        {
          num1 = 1L;
          num2 += 65536L;
        }
        else
          num1 = 0L;
        bigInteger.digits[index] = num2;
      }
      for (int size = b.size; size < a.size && num1 > 0L; ++size)
      {
        long num3 = bigInteger.digits[size] - num1;
        if (num3 < 0L)
        {
          num1 = 1L;
          num3 += 65536L;
        }
        else
          num1 = 0L;
        bigInteger.digits[size] = num3;
      }
      while (bigInteger.size - 1 > 0 && flag)
      {
        if (bigInteger.digits[bigInteger.size - 1] == 0L)
          --bigInteger.size;
        else
          flag = false;
      }
      return bigInteger;
    }

    /// <summary>Multiplies two BigIntegers.</summary>
    private static BigInteger Multiply(BigInteger a, BigInteger b)
    {
      long num1 = 0;
      BigInteger bigInteger = new BigInteger();
      bigInteger.size = a.size + b.size - 1;
      for (int index = 0; index < bigInteger.size + 1; ++index)
        bigInteger.digits[index] = 0L;
      for (int index1 = 0; index1 < a.size; ++index1)
      {
        if (a.digits[index1] != 0L)
        {
          for (int index2 = 0; index2 < b.size; ++index2)
          {
            if (b.digits[index2] != 0L)
              bigInteger.digits[index1 + index2] += a.digits[index1] * b.digits[index2];
          }
        }
      }
      for (int index = 0; index < bigInteger.size; ++index)
      {
        long num2 = bigInteger.digits[index] + num1;
        bigInteger.digits[index] = num2 % 65536L;
        num1 = num2 / 65536L;
      }
      if (num1 > 0L)
      {
        bigInteger.digits[bigInteger.size] = num1 % 65536L;
        ++bigInteger.size;
        long num3 = num1 / 65536L;
      }
      return bigInteger;
    }

    /// <summary>Divides a BigInteger by a one-digit int.</summary>
    private static BigInteger DivideByOneDigitNumber(BigInteger a, long b)
    {
      BigInteger bigInteger = new BigInteger();
      int index = a.size - 1;
      bigInteger.size = a.size;
      long num = a.digits[index];
      while (index >= 0)
      {
        bigInteger.digits[index] = num / b;
        num %= b;
        --index;
        if (index >= 0)
          num = num * 65536L + a.digits[index];
      }
      if (bigInteger.digits[bigInteger.size - 1] == 0L && bigInteger.size != 1)
        --bigInteger.size;
      return bigInteger;
    }

    /// <summary>Divides a BigInteger by another BigInteger.</summary>
    private static BigInteger DivideByBigNumber(BigInteger a, BigInteger b)
    {
      int size1 = a.size;
      int size2 = b.size;
      long num1 = 65536L / (b.digits[size2 - 1] + 1L);
      BigInteger bigInteger = new BigInteger();
      BigInteger r = a * (BigInteger) num1;
      BigInteger d = b * (BigInteger) num1;
      for (int index = size1 - size2; index >= 0; --index)
      {
        long num2 = BigInteger.Trial(r, d, index, size2);
        BigInteger dq = d * (BigInteger) num2;
        if (BigInteger.DivideByBigNumberSmaller(r, dq, index, size2))
        {
          --num2;
          dq = d * (BigInteger) num2;
        }
        bigInteger.digits[index] = num2;
        BigInteger.Difference(r, dq, index, size2);
      }
      bigInteger.size = size1 - size2 + 1;
      if (bigInteger.size != 1 && bigInteger.digits[bigInteger.size - 1] == 0L)
        --bigInteger.size;
      return bigInteger;
    }

    /// <summary>DivideByBigNumber auxiliary method.</summary>
    private static bool DivideByBigNumberSmaller(BigInteger r, BigInteger dq, int k, int m)
    {
      int index = m;
      int num = 0;
      while (index != num)
      {
        if (r.digits[index + k] != dq.digits[index])
          num = index;
        else
          --index;
      }
      return r.digits[index + k] < dq.digits[index];
    }

    /// <summary>DivideByBigNumber auxilary method.</summary>
    private static void Difference(BigInteger r, BigInteger dq, int k, int m)
    {
      long num1 = 0;
      for (int index = 0; index <= m; ++index)
      {
        long num2 = r.digits[index + k] - dq.digits[index] - num1 + 65536L;
        r.digits[index + k] = num2 % 65536L;
        num1 = 1L - num2 / 65536L;
      }
    }

    /// <summary>DivideByBigNumber auxilary method.</summary>
    private static long Trial(BigInteger r, BigInteger d, int k, int m)
    {
      int index = k + m;
      long num = ((r.digits[index] * 65536L + r.digits[index - 1]) * 65536L + r.digits[index - 2]) / (d.digits[m - 1] * 65536L + d.digits[m - 2]);
      return num < (long) ushort.MaxValue ? (long) (int) num : (long) ushort.MaxValue;
    }

    private class DigitContainer
    {
      private const int ChunkSize = 16;
      private const int ChunkSizeDivisionShift = 4;
      private const int ChunkCount = 80;
      private readonly long[][] digits;

      public DigitContainer() => this.digits = new long[80][];

      public long this[int index]
      {
        get
        {
          long[] digit = this.digits[index >> 4];
          return digit != null ? digit[index % 16] : 0L;
        }
        set
        {
          int index1 = index >> 4;
          (this.digits[index1] ?? (this.digits[index1] = new long[16]))[index % 16] = value;
        }
      }
    }
  }
}
