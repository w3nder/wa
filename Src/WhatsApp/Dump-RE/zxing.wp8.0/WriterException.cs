// Decompiled with JetBrains decompiler
// Type: ZXing.WriterException
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A base class which covers the range of exceptions which may occur when encoding a barcode using
  /// the Writer framework.
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  [Serializable]
  public sealed class WriterException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.WriterException" /> class.
    /// </summary>
    public WriterException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.WriterException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public WriterException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.WriterException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerExc">The inner exc.</param>
    public WriterException(string message, Exception innerExc)
      : base(message, innerExc)
    {
    }
  }
}
