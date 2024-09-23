// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.GenericMultipleBarcodeReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Multi
{
  /// <summary>
  ///   <p>Attempts to locate multiple barcodes in an image by repeatedly decoding portion of the image.
  /// After one barcode is found, the areas left, above, right and below the barcode's
  /// {@link com.google.zxing.ResultPoint}s are scanned, recursively.</p>
  ///   <p>A caller may want to also employ {@link ByQuadrantReader} when attempting to find multiple
  /// 2D barcodes, like QR Codes, in an image, where the presence of multiple barcodes might prevent
  /// detecting any one of them.</p>
  ///   <p>That is, instead of passing a {@link Reader} a caller might pass
  ///   <code>new ByQuadrantReader(reader)</code>.</p>
  ///   <author>Sean Owen</author>
  /// </summary>
  public sealed class GenericMultipleBarcodeReader : MultipleBarcodeReader, Reader
  {
    private const int MIN_DIMENSION_TO_RECUR = 30;
    private const int MAX_DEPTH = 4;
    private readonly Reader _delegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Multi.GenericMultipleBarcodeReader" /> class.
    /// </summary>
    /// <param name="delegate">The @delegate.</param>
    public GenericMultipleBarcodeReader(Reader @delegate) => this._delegate = @delegate;

    /// <summary>Decodes the multiple.</summary>
    /// <param name="image">The image.</param>
    /// <returns></returns>
    public Result[] decodeMultiple(BinaryBitmap image)
    {
      return this.decodeMultiple(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>Decodes the multiple.</summary>
    /// <param name="image">The image.</param>
    /// <param name="hints">The hints.</param>
    /// <returns></returns>
    public Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      List<Result> results = new List<Result>();
      this.doDecodeMultiple(image, hints, (IList<Result>) results, 0, 0, 0);
      if (results.Count == 0)
        return (Result[]) null;
      int count = results.Count;
      Result[] resultArray = new Result[count];
      for (int index = 0; index < count; ++index)
        resultArray[index] = results[index];
      return resultArray;
    }

    private void doDecodeMultiple(
      BinaryBitmap image,
      IDictionary<DecodeHintType, object> hints,
      IList<Result> results,
      int xOffset,
      int yOffset,
      int currentDepth)
    {
      if (currentDepth > 4)
        return;
      Result result = this._delegate.decode(image, hints);
      if (result == null)
        return;
      bool flag = false;
      for (int index = 0; index < results.Count; ++index)
      {
        if (results[index].Text.Equals(result.Text))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        results.Add(GenericMultipleBarcodeReader.translateResultPoints(result, xOffset, yOffset));
      ResultPoint[] resultPoints = result.ResultPoints;
      if (resultPoints == null || resultPoints.Length == 0)
        return;
      int width1 = image.Width;
      int height1 = image.Height;
      float width2 = (float) width1;
      float height2 = (float) height1;
      float left = 0.0f;
      float top = 0.0f;
      for (int index = 0; index < resultPoints.Length; ++index)
      {
        ResultPoint resultPoint = resultPoints[index];
        float x = resultPoint.X;
        float y = resultPoint.Y;
        if ((double) x < (double) width2)
          width2 = x;
        if ((double) y < (double) height2)
          height2 = y;
        if ((double) x > (double) left)
          left = x;
        if ((double) y > (double) top)
          top = y;
      }
      if ((double) width2 > 30.0)
        this.doDecodeMultiple(image.crop(0, 0, (int) width2, height1), hints, results, xOffset, yOffset, currentDepth + 1);
      if ((double) height2 > 30.0)
        this.doDecodeMultiple(image.crop(0, 0, width1, (int) height2), hints, results, xOffset, yOffset, currentDepth + 1);
      if ((double) left < (double) (width1 - 30))
        this.doDecodeMultiple(image.crop((int) left, 0, width1 - (int) left, height1), hints, results, xOffset + (int) left, yOffset, currentDepth + 1);
      if ((double) top >= (double) (height1 - 30))
        return;
      this.doDecodeMultiple(image.crop(0, (int) top, width1, height1 - (int) top), hints, results, xOffset, yOffset + (int) top, currentDepth + 1);
    }

    private static Result translateResultPoints(Result result, int xOffset, int yOffset)
    {
      ResultPoint[] resultPoints1 = result.ResultPoints;
      ResultPoint[] resultPoints2 = new ResultPoint[resultPoints1.Length];
      for (int index = 0; index < resultPoints1.Length; ++index)
      {
        ResultPoint resultPoint = resultPoints1[index];
        resultPoints2[index] = new ResultPoint(resultPoint.X + (float) xOffset, resultPoint.Y + (float) yOffset);
      }
      Result result1 = new Result(result.Text, result.RawBytes, resultPoints2, result.BarcodeFormat);
      result1.putAllMetadata(result.ResultMetadata);
      return result1;
    }

    /// <summary>
    /// Locates and decodes a barcode in some format within an image.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image) => this._delegate.decode(image);

    /// <summary>
    /// Locates and decodes a barcode in some format within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      return this._delegate.decode(image, hints);
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    public void reset() => this._delegate.reset();
  }
}
