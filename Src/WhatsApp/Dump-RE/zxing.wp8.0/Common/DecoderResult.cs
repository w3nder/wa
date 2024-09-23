// Decompiled with JetBrains decompiler
// Type: ZXing.Common.DecoderResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ZXing.Common
{
  /// <summary>
  /// Encapsulates the result of decoding a matrix of bits. This typically
  /// applies to 2D barcode formats. For now it contains the raw bytes obtained,
  /// as well as a String interpretation of those bytes, if applicable.
  /// <author>Sean Owen</author>
  /// </summary>
  public sealed class DecoderResult
  {
    public byte[] RawBytes { get; private set; }

    public string Text { get; private set; }

    public IList<byte[]> ByteSegments { get; private set; }

    public string ECLevel { get; private set; }

    public bool StructuredAppend
    {
      get => this.StructuredAppendParity >= 0 && this.StructuredAppendSequenceNumber >= 0;
    }

    public int ErrorsCorrected { get; set; }

    public int StructuredAppendSequenceNumber { get; private set; }

    public int Erasures { get; set; }

    public int StructuredAppendParity { get; private set; }

    /// <summary>Miscellanseous data value for the various decoders</summary>
    /// <value>The other.</value>
    public object Other { get; set; }

    public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel)
      : this(rawBytes, text, byteSegments, ecLevel, -1, -1)
    {
    }

    public DecoderResult(
      byte[] rawBytes,
      string text,
      IList<byte[]> byteSegments,
      string ecLevel,
      int saSequence,
      int saParity)
    {
      this.RawBytes = rawBytes != null || text != null ? rawBytes : throw new ArgumentException();
      this.Text = text;
      this.ByteSegments = byteSegments;
      this.ECLevel = ecLevel;
      this.StructuredAppendParity = saParity;
      this.StructuredAppendSequenceNumber = saSequence;
    }
  }
}
