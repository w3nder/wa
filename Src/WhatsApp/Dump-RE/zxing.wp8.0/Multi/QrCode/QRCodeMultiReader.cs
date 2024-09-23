// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.QrCode.QRCodeMultiReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Multi.QrCode.Internal;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.Multi.QrCode
{
  /// <summary>
  /// This implementation can detect and decode multiple QR Codes in an image.
  /// </summary>
  public sealed class QRCodeMultiReader : QRCodeReader, MultipleBarcodeReader
  {
    private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

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
      foreach (DetectorResult detectorResult in new MultiDetector(image.BlackMatrix).detectMulti(hints))
      {
        DecoderResult decoderResult = this.getDecoder().decode(detectorResult.Bits, hints);
        if (decoderResult != null)
        {
          ResultPoint[] points = detectorResult.Points;
          if (decoderResult.Other is QRCodeDecoderMetaData other)
            other.applyMirroredCorrection(points);
          Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.QR_CODE);
          IList<byte[]> byteSegments = decoderResult.ByteSegments;
          if (byteSegments != null)
            result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, (object) byteSegments);
          string ecLevel = decoderResult.ECLevel;
          if (ecLevel != null)
            result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, (object) ecLevel);
          if (decoderResult.StructuredAppend)
          {
            result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE, (object) decoderResult.StructuredAppendSequenceNumber);
            result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_PARITY, (object) decoderResult.StructuredAppendParity);
          }
          results.Add(result);
        }
      }
      return results.Count == 0 ? (Result[]) null : this.ProcessStructuredAppend(results).ToArray();
    }

    private List<Result> ProcessStructuredAppend(List<Result> results)
    {
      bool flag = false;
      foreach (Result result in results)
      {
        if (result.ResultMetadata.ContainsKey(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return results;
      List<Result> resultList1 = new List<Result>();
      List<Result> resultList2 = new List<Result>();
      foreach (Result result in results)
      {
        resultList1.Add(result);
        if (result.ResultMetadata.ContainsKey(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE))
          resultList2.Add(result);
      }
      resultList2.Sort(new Comparison<Result>(this.SaSequenceSort));
      string empty = string.Empty;
      int length1 = 0;
      int length2 = 0;
      foreach (Result result in resultList2)
      {
        empty += result.Text;
        length1 += result.RawBytes.Length;
        if (result.ResultMetadata.ContainsKey(ResultMetadataType.BYTE_SEGMENTS))
        {
          foreach (byte[] numArray in (IEnumerable<byte[]>) result.ResultMetadata[ResultMetadataType.BYTE_SEGMENTS])
            length2 += numArray.Length;
        }
      }
      byte[] numArray1 = new byte[length1];
      byte[] destinationArray = new byte[length2];
      int destinationIndex1 = 0;
      int destinationIndex2 = 0;
      foreach (Result result in resultList2)
      {
        Array.Copy((Array) result.RawBytes, 0, (Array) numArray1, destinationIndex1, result.RawBytes.Length);
        destinationIndex1 += result.RawBytes.Length;
        if (result.ResultMetadata.ContainsKey(ResultMetadataType.BYTE_SEGMENTS))
        {
          foreach (byte[] sourceArray in (IEnumerable<byte[]>) result.ResultMetadata[ResultMetadataType.BYTE_SEGMENTS])
          {
            Array.Copy((Array) sourceArray, 0, (Array) destinationArray, destinationIndex2, sourceArray.Length);
            destinationIndex2 += sourceArray.Length;
          }
        }
      }
      Result result1 = new Result(empty, numArray1, QRCodeMultiReader.NO_POINTS, BarcodeFormat.QR_CODE);
      if (length2 > 0)
        result1.putMetadata(ResultMetadataType.BYTE_SEGMENTS, (object) new List<byte[]>()
        {
          destinationArray
        });
      resultList1.Add(result1);
      return resultList1;
    }

    private int SaSequenceSort(Result a, Result b)
    {
      return (int) a.ResultMetadata[ResultMetadataType.STRUCTURED_APPEND_SEQUENCE] - (int) b.ResultMetadata[ResultMetadataType.STRUCTURED_APPEND_SEQUENCE];
    }
  }
}
