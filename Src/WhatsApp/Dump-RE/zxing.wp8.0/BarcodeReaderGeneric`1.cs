// Decompiled with JetBrains decompiler
// Type: ZXing.BarcodeReaderGeneric`1
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Multi;
using ZXing.Multi.QrCode;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A smart class to decode the barcode inside a bitmap object
  /// </summary>
  public class BarcodeReaderGeneric<T> : IBarcodeReaderGeneric<T>, IMultipleBarcodeReaderGeneric<T>
  {
    private static readonly Func<LuminanceSource, Binarizer> defaultCreateBinarizer = (Func<LuminanceSource, Binarizer>) (luminanceSource => (Binarizer) new HybridBinarizer(luminanceSource));
    protected static readonly Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> defaultCreateRGBLuminanceSource = (Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource>) ((rawBytes, width, height, format) => (LuminanceSource) new RGBLuminanceSource(rawBytes, width, height, format));
    private Reader reader;
    private readonly Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> createRGBLuminanceSource;
    private readonly Func<T, LuminanceSource> createLuminanceSource;
    private readonly Func<LuminanceSource, Binarizer> createBinarizer;
    private bool usePreviousState;
    private DecodingOptions options;

    /// <summary>Gets or sets the options.</summary>
    /// <value>The options.</value>
    public DecodingOptions Options
    {
      get => this.options ?? (this.options = new DecodingOptions());
      set => this.options = value;
    }

    /// <summary>
    /// Gets the reader which should be used to find and decode the barcode.
    /// </summary>
    /// <value>The reader.</value>
    protected Reader Reader => this.reader ?? (this.reader = (Reader) new MultiFormatReader());

    /// <summary>
    /// Gets or sets a method which is called if an important point is found
    /// </summary>
    /// <value>The result point callback.</value>
    public event Action<ResultPoint> ResultPointFound
    {
      add
      {
        if (!this.Options.Hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
          this.Options.Hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK] = (object) new ResultPointCallback(this.OnResultPointFound);
        this.explicitResultPointFound += value;
        this.usePreviousState = false;
      }
      remove
      {
        this.explicitResultPointFound -= value;
        if (this.explicitResultPointFound == null)
          this.Options.Hints.Remove(DecodeHintType.NEED_RESULT_POINT_CALLBACK);
        this.usePreviousState = false;
      }
    }

    private event Action<ResultPoint> explicitResultPointFound;

    /// <summary>event is executed if a result was found via decode</summary>
    public event Action<Result> ResultFound;

    /// <summary>
    /// Gets or sets a flag which cause a deeper look into the bitmap
    /// </summary>
    /// <value>
    ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
    /// </value>
    [Obsolete("Please use the Options.TryHarder property instead.")]
    public bool TryHarder
    {
      get => this.Options.TryHarder;
      set => this.Options.TryHarder = value;
    }

    /// <summary>Image is a pure monochrome image of a barcode.</summary>
    /// <value>
    ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
    /// </value>
    [Obsolete("Please use the Options.PureBarcode property instead.")]
    public bool PureBarcode
    {
      get => this.Options.PureBarcode;
      set => this.Options.PureBarcode = value;
    }

    /// <summary>
    /// Specifies what character encoding to use when decoding, where applicable (type String)
    /// </summary>
    /// <value>The character set.</value>
    [Obsolete("Please use the Options.CharacterSet property instead.")]
    public string CharacterSet
    {
      get => this.Options.CharacterSet;
      set => this.Options.CharacterSet = value;
    }

    /// <summary>
    /// Image is known to be of one of a few possible formats.
    /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
    /// </summary>
    /// <value>The possible formats.</value>
    [Obsolete("Please use the Options.PossibleFormats property instead.")]
    public IList<BarcodeFormat> PossibleFormats
    {
      get => this.Options.PossibleFormats;
      set => this.Options.PossibleFormats = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the image should be automatically rotated.
    /// Rotation is supported for 90, 180 and 270 degrees
    /// </summary>
    /// <value>
    ///   <c>true</c> if image should be rotated; otherwise, <c>false</c>.
    /// </value>
    public bool AutoRotate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the image should be automatically inverted
    /// if no result is found in the original image.
    /// ATTENTION: Please be carefully because it slows down the decoding process if it is used
    /// </summary>
    /// <value>
    ///   <c>true</c> if image should be inverted; otherwise, <c>false</c>.
    /// </value>
    public bool TryInverted { get; set; }

    /// <summary>
    /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
    /// If null a platform specific default LuminanceSource is used
    /// </summary>
    /// <value>The function to create a luminance source object.</value>
    protected Func<T, LuminanceSource> CreateLuminanceSource => this.createLuminanceSource;

    /// <summary>
    /// Optional: Gets or sets the function to create a binarizer object for a luminance source.
    /// If null then HybridBinarizer is used
    /// </summary>
    /// <value>The function to create a binarizer object.</value>
    protected Func<LuminanceSource, Binarizer> CreateBinarizer
    {
      get => this.createBinarizer ?? BarcodeReaderGeneric<T>.defaultCreateBinarizer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReaderGeneric`1" /> class.
    /// </summary>
    public BarcodeReaderGeneric()
      : this((Reader) new MultiFormatReader(), (Func<T, LuminanceSource>) null, BarcodeReaderGeneric<T>.defaultCreateBinarizer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReaderGeneric`1" /> class.
    /// </summary>
    /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
    /// If null then MultiFormatReader is used</param>
    /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
    /// If null, an exception is thrown when Decode is called</param>
    /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
    /// If null then HybridBinarizer is used</param>
    public BarcodeReaderGeneric(
      Reader reader,
      Func<T, LuminanceSource> createLuminanceSource,
      Func<LuminanceSource, Binarizer> createBinarizer)
      : this(reader, createLuminanceSource, createBinarizer, (Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource>) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReaderGeneric`1" /> class.
    /// </summary>
    /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
    /// If null then MultiFormatReader is used</param>
    /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
    /// If null, an exception is thrown when Decode is called</param>
    /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
    /// If null then HybridBinarizer is used</param>
    /// <param name="createRGBLuminanceSource">Sets the function to create a luminance source object for a rgb array.
    /// If null the RGBLuminanceSource is used. The handler is only called when Decode with a byte[] array is called.</param>
    public BarcodeReaderGeneric(
      Reader reader,
      Func<T, LuminanceSource> createLuminanceSource,
      Func<LuminanceSource, Binarizer> createBinarizer,
      Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> createRGBLuminanceSource)
    {
      this.reader = reader ?? (Reader) new MultiFormatReader();
      this.createLuminanceSource = createLuminanceSource;
      this.createBinarizer = createBinarizer ?? BarcodeReaderGeneric<T>.defaultCreateBinarizer;
      this.createRGBLuminanceSource = createRGBLuminanceSource ?? BarcodeReaderGeneric<T>.defaultCreateRGBLuminanceSource;
      this.Options.ValueChanged += (Action<object, EventArgs>) ((o, args) => this.usePreviousState = false);
      this.usePreviousState = false;
    }

    /// <summary>Decodes the specified barcode bitmap.</summary>
    /// <param name="barcodeBitmap">The barcode bitmap.</param>
    /// <returns>the result data or null</returns>
    public Result Decode(T barcodeBitmap)
    {
      if (this.CreateLuminanceSource == null)
        throw new InvalidOperationException("You have to declare a luminance source delegate.");
      return (object) barcodeBitmap != null ? this.Decode(this.CreateLuminanceSource(barcodeBitmap)) : throw new ArgumentNullException(nameof (barcodeBitmap));
    }

    /// <summary>
    /// Tries to decode a barcode within an image which is given by a luminance source.
    /// That method gives a chance to prepare a luminance source completely before calling
    /// the time consuming decoding method. On the other hand there is a chance to create
    /// a luminance source which is independent from external resources (like Bitmap objects)
    /// and the decoding call can be made in a background thread.
    /// </summary>
    /// <param name="luminanceSource">The luminance source.</param>
    /// <returns></returns>
    public virtual Result Decode(LuminanceSource luminanceSource)
    {
      Result result = (Result) null;
      BinaryBitmap image1 = new BinaryBitmap(this.CreateBinarizer(luminanceSource));
      MultiFormatReader reader = this.Reader as MultiFormatReader;
      int num1 = 0;
      int num2 = 1;
      if (this.AutoRotate)
      {
        this.Options.Hints[DecodeHintType.TRY_HARDER_WITHOUT_ROTATION] = (object) true;
        num2 = 4;
      }
      else if (this.Options.Hints.ContainsKey(DecodeHintType.TRY_HARDER_WITHOUT_ROTATION))
        this.Options.Hints.Remove(DecodeHintType.TRY_HARDER_WITHOUT_ROTATION);
      for (; num1 < num2; ++num1)
      {
        if (this.usePreviousState && reader != null)
        {
          result = reader.decodeWithState(image1);
        }
        else
        {
          result = this.Reader.decode(image1, this.Options.Hints);
          this.usePreviousState = true;
        }
        if (result == null && this.TryInverted && luminanceSource.InversionSupported)
        {
          BinaryBitmap image2 = new BinaryBitmap(this.CreateBinarizer(luminanceSource.invert()));
          if (this.usePreviousState && reader != null)
          {
            result = reader.decodeWithState(image2);
          }
          else
          {
            result = this.Reader.decode(image2, this.Options.Hints);
            this.usePreviousState = true;
          }
        }
        if (result == null && luminanceSource.RotateSupported && this.AutoRotate)
          image1 = new BinaryBitmap(this.CreateBinarizer(luminanceSource.rotateCounterClockwise()));
        else
          break;
      }
      if (result != null)
      {
        if (result.ResultMetadata == null)
          result.putMetadata(ResultMetadataType.ORIENTATION, (object) (num1 * 90));
        else
          result.ResultMetadata[ResultMetadataType.ORIENTATION] = result.ResultMetadata.ContainsKey(ResultMetadataType.ORIENTATION) ? (object) (((int) result.ResultMetadata[ResultMetadataType.ORIENTATION] + num1 * 90) % 360) : (object) (num1 * 90);
        this.OnResultFound(result);
      }
      return result;
    }

    /// <summary>Decodes the specified barcode bitmap.</summary>
    /// <param name="barcodeBitmap">The barcode bitmap.</param>
    /// <returns>the result data or null</returns>
    public Result[] DecodeMultiple(T barcodeBitmap)
    {
      if (this.CreateLuminanceSource == null)
        throw new InvalidOperationException("You have to declare a luminance source delegate.");
      return (object) barcodeBitmap != null ? this.DecodeMultiple(this.CreateLuminanceSource(barcodeBitmap)) : throw new ArgumentNullException(nameof (barcodeBitmap));
    }

    /// <summary>
    /// Tries to decode barcodes within an image which is given by a luminance source.
    /// That method gives a chance to prepare a luminance source completely before calling
    /// the time consuming decoding method. On the other hand there is a chance to create
    /// a luminance source which is independent from external resources (like Bitmap objects)
    /// and the decoding call can be made in a background thread.
    /// </summary>
    /// <param name="luminanceSource">The luminance source.</param>
    /// <returns></returns>
    public virtual Result[] DecodeMultiple(LuminanceSource luminanceSource)
    {
      Result[] resultArray = (Result[]) null;
      BinaryBitmap image1 = new BinaryBitmap(this.CreateBinarizer(luminanceSource));
      int num1 = 0;
      int num2 = 1;
      if (this.AutoRotate)
      {
        this.Options.Hints[DecodeHintType.TRY_HARDER_WITHOUT_ROTATION] = (object) true;
        num2 = 4;
      }
      IList<BarcodeFormat> possibleFormats = this.Options.PossibleFormats;
      MultipleBarcodeReader multipleBarcodeReader = possibleFormats == null || possibleFormats.Count != 1 || !possibleFormats.Contains(BarcodeFormat.QR_CODE) ? (MultipleBarcodeReader) new GenericMultipleBarcodeReader(this.Reader) : (MultipleBarcodeReader) new QRCodeMultiReader();
      for (; num1 < num2; ++num1)
      {
        resultArray = multipleBarcodeReader.decodeMultiple(image1, this.Options.Hints);
        if (resultArray == null && this.TryInverted && luminanceSource.InversionSupported)
        {
          BinaryBitmap image2 = new BinaryBitmap(this.CreateBinarizer(luminanceSource.invert()));
          resultArray = multipleBarcodeReader.decodeMultiple(image2, this.Options.Hints);
        }
        if (resultArray == null && luminanceSource.RotateSupported && this.AutoRotate)
          image1 = new BinaryBitmap(this.CreateBinarizer(luminanceSource.rotateCounterClockwise()));
        else
          break;
      }
      if (resultArray != null)
      {
        foreach (Result result in resultArray)
        {
          if (result.ResultMetadata == null)
            result.putMetadata(ResultMetadataType.ORIENTATION, (object) (num1 * 90));
          else
            result.ResultMetadata[ResultMetadataType.ORIENTATION] = result.ResultMetadata.ContainsKey(ResultMetadataType.ORIENTATION) ? (object) (((int) result.ResultMetadata[ResultMetadataType.ORIENTATION] + num1 * 90) % 360) : (object) (num1 * 90);
        }
        this.OnResultsFound((IEnumerable<Result>) resultArray);
      }
      return resultArray;
    }

    protected void OnResultsFound(IEnumerable<Result> results)
    {
      if (this.ResultFound == null)
        return;
      foreach (Result result in results)
        this.ResultFound(result);
    }

    protected void OnResultFound(Result result)
    {
      if (this.ResultFound == null)
        return;
      this.ResultFound(result);
    }

    protected void OnResultPointFound(ResultPoint resultPoint)
    {
      if (this.explicitResultPointFound == null)
        return;
      this.explicitResultPointFound(resultPoint);
    }

    /// <summary>Decodes the specified barcode bitmap.</summary>
    /// <param name="rawRGB">The image as byte[] array.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <returns>the result data or null</returns>
    public Result Decode(
      byte[] rawRGB,
      int width,
      int height,
      RGBLuminanceSource.BitmapFormat format)
    {
      if (rawRGB == null)
        throw new ArgumentNullException(nameof (rawRGB));
      return this.Decode(this.createRGBLuminanceSource(rawRGB, width, height, format));
    }

    /// <summary>Decodes the specified barcode bitmap.</summary>
    /// <param name="rawRGB">The image as byte[] array.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <returns>the result data or null</returns>
    public Result[] DecodeMultiple(
      byte[] rawRGB,
      int width,
      int height,
      RGBLuminanceSource.BitmapFormat format)
    {
      if (rawRGB == null)
        throw new ArgumentNullException(nameof (rawRGB));
      return this.DecodeMultiple(this.createRGBLuminanceSource(rawRGB, width, height, format));
    }
  }
}
