// Decompiled with JetBrains decompiler
// Type: BigIntegerLibrary.BigIntegerException
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing;

#nullable disable
namespace BigIntegerLibrary
{
  /// <summary>BigInteger-related exception class.</summary>
  [Serializable]
  public sealed class BigIntegerException : Exception
  {
    /// <summary>BigIntegerException constructor.</summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception</param>
    public BigIntegerException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
