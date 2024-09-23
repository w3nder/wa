// Decompiled with JetBrains decompiler
// Type: BigIntegerLibrary.Base10BigInteger
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace BigIntegerLibrary
{
  /// <summary>
  /// Integer inefficiently represented internally using base-10 digits, in order to allow a
  /// visual representation as a base-10 string. Only for internal use.
  /// </summary>
  internal sealed class Base10BigInteger
  {
    /// <summary>
    /// 10 numeration base for string representation, very inefficient for computations.
    /// </summary>
    private const long NumberBase = 10;
    /// <summary>
    /// Maximum size for numbers is up to 10240 binary digits or approximately (safe to use) 3000 decimal digits.
    /// The maximum size is, in fact, double the previously specified amount, in order to accommodate operations'
    /// overflow.
    /// </summary>
    private const int MaxSize = 6400;
    /// Integer constants
    private static readonly Base10BigInteger Zero = new Base10BigInteger();
    private static readonly Base10BigInteger One = new Base10BigInteger(1L);
    /// <summary>The array of digits of the number.</summary>
    private Base10BigInteger.DigitContainer digits;
    /// <summary>The actual number of digits of the number.</summary>
    private int size;
    /// <summary>The number sign.</summary>
    private Sign sign;

    /// <summary>Sets the number sign.</summary>
    internal Sign NumberSign
    {
      set => this.sign = value;
    }

    /// <summary>
    /// Default constructor, intializing the Base10BigInteger with zero.
    /// </summary>
    public Base10BigInteger()
    {
      this.digits = new Base10BigInteger.DigitContainer();
      this.size = 1;
      this.digits[this.size] = 0L;
      this.sign = Sign.Positive;
    }

    /// <summary>
    /// Constructor creating a new Base10BigInteger as a conversion of a regular base-10 long.
    /// </summary>
    /// <param name="n">The base-10 long to be converted</param>
    public Base10BigInteger(long n)
    {
      this.digits = new Base10BigInteger.DigitContainer();
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
          this.digits[this.size] = n % 10L;
          n /= 10L;
          ++this.size;
        }
      }
    }

    /// <summary>
    /// Constructor creating a new Base10BigInteger as a copy of an existing Base10BigInteger.
    /// </summary>
    /// <param name="n">The Base10BigInteger to be copied</param>
    public Base10BigInteger(Base10BigInteger n)
    {
      this.digits = new Base10BigInteger.DigitContainer();
      this.size = n.size;
      this.sign = n.sign;
      for (int index = 0; index < n.size; ++index)
        this.digits[index] = n.digits[index];
    }

    /// <summary>
    /// Determines whether the specified Base10BigInteger is equal to the current Base10BigInteger.
    /// </summary>
    /// <param name="other">The Base10BigInteger to compare with the current Base10BigInteger</param>
    /// <returns>True if the specified Base10BigInteger is equal to the current Base10BigInteger,
    /// false otherwise</returns>
    public bool Equals(Base10BigInteger other)
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
    /// Determines whether the specified System.Object is equal to the current Base10BigInteger.
    /// </summary>
    /// <param name="o">The System.Object to compare with the current Base10BigInteger</param>
    /// <returns>True if the specified System.Object is equal to the current Base10BigInteger,
    /// false otherwise</returns>
    public override bool Equals(object o)
    {
      return (object) (o as Base10BigInteger) != null && this.Equals((Base10BigInteger) o);
    }

    /// <summary>
    /// Serves as a hash function for the Base10BigInteger type.
    /// </summary>
    /// <returns>A hash code for the current Base10BigInteger</returns>
    public override int GetHashCode()
    {
      int hashCode = 0;
      for (int index = 0; index < this.size; ++index)
        hashCode += (int) this.digits[index];
      return hashCode;
    }

    /// <summary>
    /// String representation of the current Base10BigInteger, converted to its base-10 representation.
    /// </summary>
    /// <returns>The string representation of the current Base10BigInteger</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder;
      if (this.sign == Sign.Negative)
      {
        stringBuilder = new StringBuilder(this.size + 1);
        stringBuilder.Append('-');
      }
      else
        stringBuilder = new StringBuilder(this.size);
      for (int index = this.size - 1; index >= 0; --index)
        stringBuilder.Append(this.digits[index]);
      return stringBuilder.ToString();
    }

    /// <summary>Base10BigInteger inverse with respect to addition.</summary>
    /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
    /// <returns>The Base10BigInteger inverse with respect to addition</returns>
    public static Base10BigInteger Opposite(Base10BigInteger n)
    {
      Base10BigInteger base10BigInteger = new Base10BigInteger(n);
      if (base10BigInteger != Base10BigInteger.Zero)
        base10BigInteger.sign = base10BigInteger.sign != Sign.Positive ? Sign.Positive : Sign.Negative;
      return base10BigInteger;
    }

    /// <summary>Greater test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &gt; b, false otherwise</returns>
    public static bool Greater(Base10BigInteger a, Base10BigInteger b)
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

    /// <summary>Greater or equal test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &gt;= b, false otherwise</returns>
    public static bool GreaterOrEqual(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Greater(a, b) || object.Equals((object) a, (object) b);
    }

    /// <summary>Smaller test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &lt; b, false otherwise</returns>
    public static bool Smaller(Base10BigInteger a, Base10BigInteger b)
    {
      return !Base10BigInteger.GreaterOrEqual(a, b);
    }

    /// <summary>Smaller or equal test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &lt;= b, false otherwise</returns>
    public static bool SmallerOrEqual(Base10BigInteger a, Base10BigInteger b)
    {
      return !Base10BigInteger.Greater(a, b);
    }

    /// <summary>Computes the absolute value of a Base10BigInteger.</summary>
    /// <param name="n">The Base10BigInteger whose absolute value is to be computed</param>
    /// <returns>The absolute value of the given BigInteger</returns>
    public static Base10BigInteger Abs(Base10BigInteger n)
    {
      return new Base10BigInteger(n)
      {
        sign = Sign.Positive
      };
    }

    /// <summary>Addition operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the addition</returns>
    public static Base10BigInteger Addition(Base10BigInteger a, Base10BigInteger b)
    {
      Base10BigInteger base10BigInteger = (Base10BigInteger) null;
      if (a.sign == Sign.Positive && b.sign == Sign.Positive)
      {
        base10BigInteger = !(a >= b) ? Base10BigInteger.Add(b, a) : Base10BigInteger.Add(a, b);
        base10BigInteger.sign = Sign.Positive;
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Negative)
      {
        base10BigInteger = !(a <= b) ? Base10BigInteger.Add(-b, -a) : Base10BigInteger.Add(-a, -b);
        base10BigInteger.sign = Sign.Negative;
      }
      if (a.sign == Sign.Positive && b.sign == Sign.Negative)
      {
        if (a >= -b)
        {
          base10BigInteger = Base10BigInteger.Subtract(a, -b);
          base10BigInteger.sign = Sign.Positive;
        }
        else
        {
          base10BigInteger = Base10BigInteger.Subtract(-b, a);
          base10BigInteger.sign = Sign.Negative;
        }
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Positive)
      {
        if (-a <= b)
        {
          base10BigInteger = Base10BigInteger.Subtract(b, -a);
          base10BigInteger.sign = Sign.Positive;
        }
        else
        {
          base10BigInteger = Base10BigInteger.Subtract(-a, b);
          base10BigInteger.sign = Sign.Negative;
        }
      }
      return base10BigInteger;
    }

    /// <summary>Subtraction operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the subtraction</returns>
    public static Base10BigInteger Subtraction(Base10BigInteger a, Base10BigInteger b)
    {
      Base10BigInteger base10BigInteger = (Base10BigInteger) null;
      if (a.sign == Sign.Positive && b.sign == Sign.Positive)
      {
        if (a >= b)
        {
          base10BigInteger = Base10BigInteger.Subtract(a, b);
          base10BigInteger.sign = Sign.Positive;
        }
        else
        {
          base10BigInteger = Base10BigInteger.Subtract(b, a);
          base10BigInteger.sign = Sign.Negative;
        }
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Negative)
      {
        if (a <= b)
        {
          base10BigInteger = Base10BigInteger.Subtract(-a, -b);
          base10BigInteger.sign = Sign.Negative;
        }
        else
        {
          base10BigInteger = Base10BigInteger.Subtract(-b, -a);
          base10BigInteger.sign = Sign.Positive;
        }
      }
      if (a.sign == Sign.Positive && b.sign == Sign.Negative)
      {
        base10BigInteger = !(a >= -b) ? Base10BigInteger.Add(-b, a) : Base10BigInteger.Add(a, -b);
        base10BigInteger.sign = Sign.Positive;
      }
      if (a.sign == Sign.Negative && b.sign == Sign.Positive)
      {
        base10BigInteger = !(-a >= b) ? Base10BigInteger.Add(b, -a) : Base10BigInteger.Add(-a, b);
        base10BigInteger.sign = Sign.Negative;
      }
      return base10BigInteger;
    }

    /// <summary>Multiplication operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the multiplication</returns>
    public static Base10BigInteger Multiplication(Base10BigInteger a, Base10BigInteger b)
    {
      if (a == Base10BigInteger.Zero || b == Base10BigInteger.Zero)
        return Base10BigInteger.Zero;
      Base10BigInteger base10BigInteger = Base10BigInteger.Multiply(Base10BigInteger.Abs(a), Base10BigInteger.Abs(b));
      base10BigInteger.sign = a.sign != b.sign ? Sign.Negative : Sign.Positive;
      return base10BigInteger;
    }

    /// <summary>
    /// Implicit conversion operator from long to Base10BigInteger.
    /// </summary>
    /// <param name="n">The long to be converted to a Base10BigInteger</param>
    /// <returns>The Base10BigInteger converted from the given long</returns>
    public static implicit operator Base10BigInteger(long n) => new Base10BigInteger(n);

    /// <summary>Equality test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a == b, false otherwise</returns>
    public static bool operator ==(Base10BigInteger a, Base10BigInteger b)
    {
      return object.Equals((object) a, (object) b);
    }

    /// <summary>Inequality test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a != b, false otherwise</returns>
    public static bool operator !=(Base10BigInteger a, Base10BigInteger b)
    {
      return !object.Equals((object) a, (object) b);
    }

    /// <summary>Greater test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &gt; b, false otherwise</returns>
    public static bool operator >(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Greater(a, b);
    }

    /// <summary>Smaller test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &lt; b, false otherwise</returns>
    public static bool operator <(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Smaller(a, b);
    }

    /// <summary>Greater or equal test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &gt;= b, false otherwise</returns>
    public static bool operator >=(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.GreaterOrEqual(a, b);
    }

    /// <summary>Smaller or equal test between two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>True if a &lt;= b, false otherwise</returns>
    public static bool operator <=(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.SmallerOrEqual(a, b);
    }

    /// <summary>Base10BigInteger inverse with respect to addition.</summary>
    /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
    /// <returns>The Base10BigInteger inverse with respect to addition</returns>
    public static Base10BigInteger operator -(Base10BigInteger n) => Base10BigInteger.Opposite(n);

    /// <summary>Addition operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the addition</returns>
    public static Base10BigInteger operator +(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Addition(a, b);
    }

    /// <summary>Subtraction operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the subtraction</returns>
    public static Base10BigInteger operator -(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Subtraction(a, b);
    }

    /// <summary>Multiplication operation of two Base10BigIntegers.</summary>
    /// <param name="a">The 1st Base10BigInteger</param>
    /// <param name="b">The 2nd Base10BigInteger</param>
    /// <returns>The Base10BigInteger result of the multiplication</returns>
    public static Base10BigInteger operator *(Base10BigInteger a, Base10BigInteger b)
    {
      return Base10BigInteger.Multiplication(a, b);
    }

    /// <summary>Incremetation by one operation of a Base10BigInteger.</summary>
    /// <param name="n">The Base10BigInteger to be incremented by one</param>
    /// <returns>The Base10BigInteger result of incrementing by one</returns>
    public static Base10BigInteger operator ++(Base10BigInteger n) => n + Base10BigInteger.One;

    /// <summary>Decremetation by one operation of a Base10BigInteger.</summary>
    /// <param name="n">The Base10BigInteger to be decremented by one</param>
    /// <returns>The Base10BigInteger result of decrementing by one</returns>
    public static Base10BigInteger operator --(Base10BigInteger n) => n - Base10BigInteger.One;

    /// <summary>
    /// Adds two BigNumbers a and b, where a &gt;= b, a, b non-negative.
    /// </summary>
    private static Base10BigInteger Add(Base10BigInteger a, Base10BigInteger b)
    {
      Base10BigInteger base10BigInteger = new Base10BigInteger(a);
      long num1 = 0;
      for (int index = 0; index < b.size; ++index)
      {
        long num2 = base10BigInteger.digits[index] + b.digits[index] + num1;
        base10BigInteger.digits[index] = num2 % 10L;
        num1 = num2 / 10L;
      }
      for (int size = b.size; size < a.size && num1 > 0L; ++size)
      {
        long num3 = base10BigInteger.digits[size] + num1;
        base10BigInteger.digits[size] = num3 % 10L;
        num1 = num3 / 10L;
      }
      if (num1 > 0L)
      {
        base10BigInteger.digits[base10BigInteger.size] = num1 % 10L;
        ++base10BigInteger.size;
        long num4 = num1 / 10L;
      }
      return base10BigInteger;
    }

    /// <summary>
    /// Subtracts the Base10BigInteger b from the Base10BigInteger a, where a &gt;= b, a, b non-negative.
    /// </summary>
    private static Base10BigInteger Subtract(Base10BigInteger a, Base10BigInteger b)
    {
      Base10BigInteger base10BigInteger = new Base10BigInteger(a);
      long num1 = 0;
      bool flag = true;
      for (int index = 0; index < b.size; ++index)
      {
        long num2 = base10BigInteger.digits[index] - b.digits[index] - num1;
        if (num2 < 0L)
        {
          num1 = 1L;
          num2 += 10L;
        }
        else
          num1 = 0L;
        base10BigInteger.digits[index] = num2;
      }
      for (int size = b.size; size < a.size && num1 > 0L; ++size)
      {
        long num3 = base10BigInteger.digits[size] - num1;
        if (num3 < 0L)
        {
          num1 = 1L;
          num3 += 10L;
        }
        else
          num1 = 0L;
        base10BigInteger.digits[size] = num3;
      }
      while (base10BigInteger.size - 1 > 0 && flag)
      {
        if (base10BigInteger.digits[base10BigInteger.size - 1] == 0L)
          --base10BigInteger.size;
        else
          flag = false;
      }
      return base10BigInteger;
    }

    /// <summary>Multiplies two Base10BigIntegers.</summary>
    private static Base10BigInteger Multiply(Base10BigInteger a, Base10BigInteger b)
    {
      long num1 = 0;
      Base10BigInteger base10BigInteger = new Base10BigInteger();
      base10BigInteger.size = a.size + b.size - 1;
      for (int index = 0; index < base10BigInteger.size + 1; ++index)
        base10BigInteger.digits[index] = 0L;
      for (int index1 = 0; index1 < a.size; ++index1)
      {
        if (a.digits[index1] != 0L)
        {
          for (int index2 = 0; index2 < b.size; ++index2)
          {
            if (b.digits[index2] != 0L)
              base10BigInteger.digits[index1 + index2] += a.digits[index1] * b.digits[index2];
          }
        }
      }
      for (int index = 0; index < base10BigInteger.size; ++index)
      {
        long num2 = base10BigInteger.digits[index] + num1;
        base10BigInteger.digits[index] = num2 % 10L;
        num1 = num2 / 10L;
      }
      if (num1 > 0L)
      {
        base10BigInteger.digits[base10BigInteger.size] = num1 % 10L;
        ++base10BigInteger.size;
        long num3 = num1 / 10L;
      }
      return base10BigInteger;
    }

    private class DigitContainer
    {
      private const int ChunkSize = 32;
      private const int ChunkSizeDivisionShift = 5;
      private const int ChunkCount = 200;
      private readonly long[][] digits;

      public DigitContainer() => this.digits = new long[200][];

      public long this[int index]
      {
        get
        {
          long[] digit = this.digits[index >> 5];
          return digit != null ? digit[index % 32] : 0L;
        }
        set
        {
          int index1 = index >> 5;
          (this.digits[index1] ?? (this.digits[index1] = new long[32]))[index % 32] = value;
        }
      }
    }
  }
}
