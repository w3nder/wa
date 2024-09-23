// Decompiled with JetBrains decompiler
// Type: ZXing.Result
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Encapsulates the result of decoding a barcode within an image.
  /// </summary>
  public sealed class Result
  {
    /// <returns>raw text encoded by the barcode, if applicable, otherwise <code>null</code></returns>
    public string Text { get; private set; }

    /// <returns>raw bytes encoded by the barcode, if applicable, otherwise <code>null</code></returns>
    public byte[] RawBytes { get; private set; }

    /// <returns>
    /// points related to the barcode in the image. These are typically points
    /// identifying finder patterns or the corners of the barcode. The exact meaning is
    /// specific to the type of barcode that was decoded.
    /// </returns>
    public ResultPoint[] ResultPoints { get; private set; }

    /// <returns>{@link BarcodeFormat} representing the format of the barcode that was decoded</returns>
    public BarcodeFormat BarcodeFormat { get; private set; }

    /// <returns>
    /// {@link Hashtable} mapping {@link ResultMetadataType} keys to values. May be
    /// <code>null</code>. This contains optional metadata about what was detected about the barcode,
    /// like orientation.
    /// </returns>
    public IDictionary<ResultMetadataType, object> ResultMetadata { get; private set; }

    /// <summary>Gets the timestamp.</summary>
    public long Timestamp { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Result" /> class.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="rawBytes">The raw bytes.</param>
    /// <param name="resultPoints">The result points.</param>
    /// <param name="format">The format.</param>
    public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format)
      : this(text, rawBytes, resultPoints, format, DateTime.Now.Ticks)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Result" /> class.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="rawBytes">The raw bytes.</param>
    /// <param name="resultPoints">The result points.</param>
    /// <param name="format">The format.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Result(
      string text,
      byte[] rawBytes,
      ResultPoint[] resultPoints,
      BarcodeFormat format,
      long timestamp)
    {
      this.Text = text != null || rawBytes != null ? text : throw new ArgumentException("Text and bytes are null");
      this.RawBytes = rawBytes;
      this.ResultPoints = resultPoints;
      this.BarcodeFormat = format;
      this.ResultMetadata = (IDictionary<ResultMetadataType, object>) null;
      this.Timestamp = timestamp;
    }

    /// <summary>Adds one metadata to the result</summary>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    public void putMetadata(ResultMetadataType type, object value)
    {
      if (this.ResultMetadata == null)
        this.ResultMetadata = (IDictionary<ResultMetadataType, object>) new Dictionary<ResultMetadataType, object>();
      this.ResultMetadata[type] = value;
    }

    /// <summary>Adds a list of metadata to the result</summary>
    /// <param name="metadata">The metadata.</param>
    public void putAllMetadata(IDictionary<ResultMetadataType, object> metadata)
    {
      if (metadata == null)
        return;
      if (this.ResultMetadata == null)
      {
        this.ResultMetadata = metadata;
      }
      else
      {
        foreach (KeyValuePair<ResultMetadataType, object> keyValuePair in (IEnumerable<KeyValuePair<ResultMetadataType, object>>) metadata)
          this.ResultMetadata[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    /// <summary>Adds the result points.</summary>
    /// <param name="newPoints">The new points.</param>
    public void addResultPoints(ResultPoint[] newPoints)
    {
      ResultPoint[] resultPoints = this.ResultPoints;
      if (resultPoints == null)
      {
        this.ResultPoints = newPoints;
      }
      else
      {
        if (newPoints == null || newPoints.Length <= 0)
          return;
        ResultPoint[] destinationArray = new ResultPoint[resultPoints.Length + newPoints.Length];
        Array.Copy((Array) resultPoints, 0, (Array) destinationArray, 0, resultPoints.Length);
        Array.Copy((Array) newPoints, 0, (Array) destinationArray, resultPoints.Length, newPoints.Length);
        this.ResultPoints = destinationArray;
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this.Text == null ? "[" + (object) this.RawBytes.Length + " bytes]" : this.Text;
    }
  }
}
