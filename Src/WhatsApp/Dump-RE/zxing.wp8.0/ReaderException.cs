// Decompiled with JetBrains decompiler
// Type: ZXing.ReaderException
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// The general exception class throw when something goes wrong during decoding of a barcode.
  /// This includes, but is not limited to, failing checksums / error correction algorithms, being
  /// unable to locate finder timing patterns, and so on.
  /// </summary>
  /// <author>Sean Owen</author>
  [Serializable]
  public class ReaderException : Exception
  {
    private static readonly ReaderException instance = new ReaderException();

    /// <summary>Gets the instance.</summary>
    public static ReaderException Instance => ReaderException.instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.ReaderException" /> class.
    /// </summary>
    protected ReaderException()
    {
    }
  }
}
