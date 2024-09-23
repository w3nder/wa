// Decompiled with JetBrains decompiler
// Type: WhatsApp.JpegUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone;
using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class JpegUtils
  {
    private const ushort JPEG_MARKER_MASK = 65280;
    private const ushort JPEG_SOF0 = 65472;
    private const ushort JPEG_DHT = 65476;
    private const ushort JPEG_SOI = 65496;
    private static byte[] JPEG_SOI_BYTES = new byte[2]
    {
      byte.MaxValue,
      (byte) 216
    };
    private const ushort JPEG_EOI = 65497;
    private static byte[] JPEG_EOI_BYTES = new byte[2]
    {
      byte.MaxValue,
      (byte) 217
    };
    private const ushort JPEG_SOS = 65498;
    private static byte[] JPEG_SOS_BYTES = new byte[2]
    {
      byte.MaxValue,
      (byte) 218
    };
    private const ushort JPEG_DQT = 65499;
    private const ushort JPEG_DRI = 65501;
    private const ushort JPEG_APP_MARKER_MASK = 65504;
    private const ushort JPEG_APP0 = 65504;
    private const ushort JPEG_APP1 = 65505;
    private const ushort JPEG_APPD = 65517;
    private static byte[] EXIF_SIG = new byte[5]
    {
      (byte) 69,
      (byte) 120,
      (byte) 105,
      (byte) 102,
      (byte) 0
    };
    private static byte[] JFIF_SIG = new byte[5]
    {
      (byte) 74,
      (byte) 70,
      (byte) 73,
      (byte) 70,
      (byte) 0
    };
    private static byte[] JFXX_SIG = new byte[5]
    {
      (byte) 74,
      (byte) 70,
      (byte) 88,
      (byte) 88,
      (byte) 0
    };

    public static ushort? GetJpegOrientation(Stream stream)
    {
      try
      {
        stream.Position = 0L;
        BinaryReader binaryReader = new BinaryReader(stream);
        if (JpegUtils.Swap(binaryReader.ReadUInt16()) != (ushort) 65496)
          return new ushort?();
        ushort num1;
        ushort num2;
        while (true)
        {
          num1 = JpegUtils.Swap(binaryReader.ReadUInt16());
          if (((int) num1 & 65280) == 65280)
          {
            if (num1 != (ushort) 65496)
            {
              if (num1 != (ushort) 65497)
              {
                num2 = JpegUtils.Swap(binaryReader.ReadUInt16());
                if (num2 > (ushort) 2)
                {
                  switch (num1)
                  {
                    case 65504:
                      stream.Position += (long) ((int) num2 - 2);
                      continue;
                    case 65505:
                      goto label_11;
                    default:
                      goto label_27;
                  }
                }
                else
                  goto label_9;
              }
              else
                goto label_7;
            }
            else
              goto label_5;
          }
          else
            break;
        }
        return new ushort?();
label_5:
        return new ushort?();
label_7:
        return new ushort?();
label_9:
        throw new IOException(string.Format("Invalid JPEG marker length found looking for Exif: {0} {1}", (object) num1, (object) num2));
label_11:
        byte[] buffer = new byte[JpegUtils.EXIF_SIG.Length];
        stream.Read(buffer, 0, buffer.Length);
        for (int index = 0; index < buffer.Length; ++index)
        {
          if ((int) JpegUtils.EXIF_SIG[index] != (int) buffer[index])
            return new ushort?();
        }
        Func<ushort, ushort> func1 = (Func<ushort, ushort>) (i => JpegUtils.Swap(i));
        Func<uint, uint> func2 = (Func<uint, uint>) (i => JpegUtils.Swap(i));
        ++stream.Position;
        long position = stream.Position;
        switch (binaryReader.ReadUInt16())
        {
          case 18761:
            func1 = (Func<ushort, ushort>) (i => i);
            func2 = (Func<uint, uint>) (i => i);
            goto case 19789;
          case 19789:
            int num3 = (int) func1(binaryReader.ReadUInt16());
            uint num4 = func2(binaryReader.ReadUInt32());
            long num5 = position + (long) num4;
            do
            {
              stream.Position = num5;
              ushort num6 = func1(binaryReader.ReadUInt16());
              while (num6-- != (ushort) 0)
              {
                ushort num7 = func1(binaryReader.ReadUInt16());
                int num8 = (int) func1(binaryReader.ReadUInt16());
                int num9 = (int) func2(binaryReader.ReadUInt32());
                if (num7 == (ushort) 274)
                  return new ushort?(func1(binaryReader.ReadUInt16()));
                int num10 = (int) func2(binaryReader.ReadUInt32());
              }
              num5 = position + (long) func2(binaryReader.ReadUInt32());
            }
            while (num5 != position);
            return new ushort?();
          default:
            return new ushort?();
        }
label_27:
        return new ushort?();
      }
      catch (Exception ex)
      {
        return new ushort?();
      }
      finally
      {
        stream.Position = 0L;
      }
    }

    public static bool StripApplicationData(Stream imageStream, Stream outputStream)
    {
      try
      {
        bool flag1 = false;
        imageStream.Position = 0L;
        BinaryReader binaryReader = new BinaryReader(imageStream);
        if (JpegUtils.Swap(binaryReader.ReadUInt16()) != (ushort) 65496)
        {
          Log.l("JPEG Strip", "Non JPEG Stream found when stripping");
          return false;
        }
        byte[] buffer = new byte[4096];
        Func<Stream, Stream, int, int> func1 = (Func<Stream, Stream, int, int>) ((jpegStream, strippedStream, length) =>
        {
          int val1;
          int count;
          for (val1 = length; val1 > 0 && (count = jpegStream.Read(buffer, 0, Math.Min(val1, buffer.Length))) > 0; val1 -= count)
            strippedStream.Write(buffer, 0, count);
          return val1;
        });
        Func<Stream, int, int, bool> func2 = (Func<Stream, int, int, bool>) ((strippedStream, marker, size) =>
        {
          byte[] bytes1 = BitConverter.GetBytes(marker);
          byte[] bytes2 = BitConverter.GetBytes(size);
          int offset = 0;
          if (BitConverter.IsLittleEndian)
          {
            Array.Reverse((Array) bytes1);
            Array.Reverse((Array) bytes2);
            offset = 2;
          }
          strippedStream.Write(bytes1, offset, 2);
          strippedStream.Write(bytes2, offset, 2);
          return true;
        });
        outputStream.Write(JpegUtils.JPEG_SOI_BYTES, 0, 2);
        int num1;
        do
        {
          ushort num2;
          do
          {
            num2 = JpegUtils.Swap(binaryReader.ReadUInt16());
            if (((int) num2 & 65280) != 65280)
              return false;
            switch (num2)
            {
              case 65496:
                outputStream.Write(JpegUtils.JPEG_SOI_BYTES, 0, 2);
                continue;
              case 65497:
                outputStream.Write(JpegUtils.JPEG_EOI_BYTES, 0, 2);
                return true;
              case 65498:
                outputStream.Write(JpegUtils.JPEG_SOS_BYTES, 0, 2);
                continue;
              default:
                goto label_10;
            }
          }
          while (func1(imageStream, outputStream, (int) (imageStream.Length - 2L - imageStream.Position)) == 0);
          Log.l("JPEG Strip", "Incomplete JPEG data written at end when stripping");
          return false;
label_10:
          ushort num3 = JpegUtils.Swap(binaryReader.ReadUInt16());
          if (num3 <= (ushort) 2 || imageStream.Position + (long) num3 >= imageStream.Length - 2L)
          {
            Log.l("JPEG Strip", "Invalid JPEG marker size found when stripping: {0} {1}", (object) num2, (object) num3);
            return false;
          }
          num1 = (int) num3 - 2;
          if (((int) num2 & 65504) == 65504)
          {
            switch (num2)
            {
              case 65504:
              case 65505:
                if (num1 < 5)
                  return false;
                byte[] buffer1 = new byte[5];
                if (imageStream.Read(buffer1, 0, 5) != 5)
                  return false;
                int num4 = num1 - 5;
                if (num2 == (ushort) 65504)
                {
                  bool flag2 = true;
                  for (int index = 0; index < 5; ++index)
                  {
                    if ((int) JpegUtils.JFIF_SIG[index] != (int) buffer1[index])
                    {
                      flag2 = false;
                      break;
                    }
                  }
                  if (flag2)
                  {
                    int num5 = func2(outputStream, (int) num2, (int) num3) ? 1 : 0;
                    outputStream.Write(JpegUtils.JFIF_SIG, 0, 5);
                    if (func1(imageStream, outputStream, num4) != 0)
                    {
                      Log.l("JPEG String", "Incomplete JFIF marker data output while stripping");
                      return false;
                    }
                    continue;
                  }
                  for (int index = 0; index < 5; ++index)
                  {
                    if ((int) JpegUtils.JFXX_SIG[index] != (int) buffer1[index])
                      return false;
                  }
                  imageStream.Position += (long) num4;
                  continue;
                }
                if (!flag1)
                {
                  for (int index = 0; index < 5; ++index)
                  {
                    if ((int) JpegUtils.EXIF_SIG[index] != (int) buffer1[index])
                      return false;
                  }
                  flag1 = true;
                }
                imageStream.Position += (long) num4;
                continue;
              case 65517:
                imageStream.Position += (long) num1;
                continue;
              default:
                return false;
            }
          }
          else
          {
            int num6 = func2(outputStream, (int) num2, (int) num3) ? 1 : 0;
          }
        }
        while (func1(imageStream, outputStream, num1) == 0);
        Log.l("JPEG String", "Incomplete JPEG marker data output while stripping");
        return false;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception stripping jpeg");
        return false;
      }
      finally
      {
        try
        {
          imageStream.Position = 0L;
          outputStream.Position = 0L;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception positioning stream after stripping");
        }
      }
    }

    private static ushort Swap(ushort i)
    {
      return (ushort) ((uint) (((int) i & (int) byte.MaxValue) << 8) | ((uint) i & 65280U) >> 8);
    }

    private static uint Swap(uint i)
    {
      return (uint) (((int) i & (int) byte.MaxValue) << 24 | ((int) i & 65280) << 8) | (i & 16711680U) >> 8 | i >> 24;
    }

    public static WriteableBitmap GetBitmapFromStream(
      Stream imgStream,
      Size? maxSize,
      System.Windows.Point? zoomScale,
      Size? RelativeCropSize,
      System.Windows.Point? RelativeCropPos,
      int? orientationToApply,
      int rotatedTimes)
    {
      WriteableBitmap bitmapFromStream;
      try
      {
        bitmapFromStream = JpegUtils.DecodeJpeg(imgStream, maxSize);
        if (orientationToApply.HasValue)
          bitmapFromStream = JpegUtils.ApplyJpegOrientation((BitmapSource) bitmapFromStream, new int?(orientationToApply.Value));
        System.Windows.Point point;
        if (zoomScale.HasValue)
        {
          double width = Math.Abs((double) bitmapFromStream.PixelWidth / zoomScale.Value.X);
          double pixelHeight = (double) bitmapFromStream.PixelHeight;
          point = zoomScale.Value;
          double y = point.Y;
          double height = Math.Abs(pixelHeight / y);
          bitmapFromStream = bitmapFromStream.Crop(new System.Windows.Point(((double) bitmapFromStream.PixelWidth - width) / 2.0, ((double) bitmapFromStream.PixelHeight - height) / 2.0), new Size(width, height));
        }
        if (RelativeCropPos.HasValue && RelativeCropSize.HasValue)
        {
          System.Windows.Point location;
          ref System.Windows.Point local = ref location;
          point = RelativeCropPos.Value;
          double x = point.X * (double) bitmapFromStream.PixelWidth;
          point = RelativeCropPos.Value;
          double y = point.Y * (double) bitmapFromStream.PixelHeight;
          local = new System.Windows.Point(x, y);
          Size size = new Size(RelativeCropSize.Value.Width * (double) bitmapFromStream.PixelWidth, RelativeCropSize.Value.Height * (double) bitmapFromStream.PixelHeight);
          bitmapFromStream = bitmapFromStream.Crop(new Rect(location, size));
        }
        int num = rotatedTimes % 4;
        if (num > 0)
          bitmapFromStream = bitmapFromStream.Rotate(num * 90);
      }
      catch (Exception ex)
      {
        bitmapFromStream = (WriteableBitmap) null;
        Log.LogException(ex, "get bitmap from pic info | picker");
      }
      return bitmapFromStream;
    }

    private static void IterateTopToBottom(
      WriteableBitmap src,
      WriteableBitmap dst,
      JpegUtils.XAxisProc onLine)
    {
      int dstOff = 0;
      int pixelWidth = src.PixelWidth;
      int pixelHeight = src.PixelHeight;
      int length = src.Pixels.Length;
      for (int srcOffset = 0; srcOffset < length; srcOffset += pixelWidth)
        onLine(src, srcOffset, dst, ref dstOff);
    }

    private static void IterateBottomToTop(
      WriteableBitmap src,
      WriteableBitmap dst,
      JpegUtils.XAxisProc onLine)
    {
      int dstOff = 0;
      int pixelWidth = src.PixelWidth;
      int pixelHeight = src.PixelHeight;
      for (int srcOffset = src.Pixels.Length - pixelWidth; srcOffset >= 0; srcOffset -= pixelWidth)
        onLine(src, srcOffset, dst, ref dstOff);
    }

    private static void IterateLeftToRight(
      WriteableBitmap src,
      int srcOff,
      WriteableBitmap dst,
      ref int dstOff)
    {
      int num = srcOff + src.PixelWidth;
      int[] pixels1 = src.Pixels;
      int[] pixels2 = dst.Pixels;
      while (srcOff < num)
        pixels2[dstOff++] = pixels1[srcOff++];
      int[] numArray1;
      int[] numArray2 = numArray1 = (int[]) null;
    }

    private static void IterateRightToLeft(
      WriteableBitmap src,
      int srcOff,
      WriteableBitmap dst,
      ref int dstOff)
    {
      int num = srcOff;
      srcOff += src.PixelWidth;
      int[] pixels1 = src.Pixels;
      int[] pixels2 = dst.Pixels;
      while (srcOff > num)
        pixels2[dstOff++] = pixels1[--srcOff];
      int[] numArray1;
      int[] numArray2 = numArray1 = (int[]) null;
    }

    private static void IterateLeftToRightWithTranspose(
      WriteableBitmap src,
      int srcOff,
      WriteableBitmap dst,
      ref int dstOff)
    {
      int num = srcOff + src.PixelWidth;
      int[] pixels1 = src.Pixels;
      int[] pixels2 = dst.Pixels;
      int index = dstOff;
      int pixelWidth = dst.PixelWidth;
      while (srcOff < num)
      {
        pixels2[index] = pixels1[srcOff++];
        index += pixelWidth;
      }
      ++dstOff;
      int[] numArray1;
      int[] numArray2 = numArray1 = (int[]) null;
    }

    private static void IterateRightToLeftWithTranspose(
      WriteableBitmap src,
      int srcOff,
      WriteableBitmap dst,
      ref int dstOff)
    {
      int num = srcOff;
      srcOff += src.PixelWidth;
      int[] pixels1 = src.Pixels;
      int[] pixels2 = dst.Pixels;
      int index = dstOff;
      int pixelWidth = dst.PixelWidth;
      while (srcOff > num)
      {
        pixels2[index] = pixels1[--srcOff];
        index += pixelWidth;
      }
      ++dstOff;
      int[] numArray1;
      int[] numArray2 = numArray1 = (int[]) null;
    }

    private static Func<WriteableBitmap, WriteableBitmap> TransformForOrientation(int? orientation)
    {
      JpegUtils.YAxisProc yAxis = (JpegUtils.YAxisProc) null;
      JpegUtils.XAxisProc xAxis = (JpegUtils.XAxisProc) null;
      bool dimensionSwap = false;
      switch (orientation ?? 1)
      {
        case 2:
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateTopToBottom);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateRightToLeft);
          break;
        case 3:
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateBottomToTop);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateRightToLeft);
          break;
        case 4:
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateBottomToTop);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateLeftToRight);
          break;
        case 5:
          dimensionSwap = true;
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateTopToBottom);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateLeftToRightWithTranspose);
          break;
        case 6:
          dimensionSwap = true;
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateBottomToTop);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateLeftToRightWithTranspose);
          break;
        case 7:
          dimensionSwap = true;
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateBottomToTop);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateRightToLeftWithTranspose);
          break;
        case 8:
          dimensionSwap = true;
          yAxis = new JpegUtils.YAxisProc(JpegUtils.IterateTopToBottom);
          xAxis = new JpegUtils.XAxisProc(JpegUtils.IterateRightToLeftWithTranspose);
          break;
        default:
          return (Func<WriteableBitmap, WriteableBitmap>) null;
      }
      return (Func<WriteableBitmap, WriteableBitmap>) (src =>
      {
        int pixelWidth = src.PixelWidth;
        int pixelHeight = src.PixelHeight;
        if (dimensionSwap)
          Utils.Swap<int>(ref pixelWidth, ref pixelHeight);
        WriteableBitmap dst = new WriteableBitmap(pixelWidth, pixelHeight);
        yAxis(src, dst, xAxis);
        src = (WriteableBitmap) null;
        return dst;
      });
    }

    public static WriteableBitmap ApplyJpegOrientation(BitmapSource source, int? orientation)
    {
      if (source == null)
        return (WriteableBitmap) null;
      Func<WriteableBitmap, WriteableBitmap> func = JpegUtils.TransformForOrientation(orientation);
      if (!(source is WriteableBitmap writeableBitmap1))
        writeableBitmap1 = new WriteableBitmap(source);
      WriteableBitmap writeableBitmap2 = writeableBitmap1;
      source = (BitmapSource) null;
      if (func == null)
        return writeableBitmap2;
      WriteableBitmap writeableBitmap3 = func(writeableBitmap2);
      writeableBitmap3.Invalidate();
      return writeableBitmap3;
    }

    public static int ExifCodeForRotation(int clockwiseRotatedTimes)
    {
      switch (clockwiseRotatedTimes % 4)
      {
        case 1:
          return 6;
        case 2:
          return 3;
        case 3:
          return 8;
        default:
          return 1;
      }
    }

    public static int? AngleForRotation(int rot)
    {
      int num;
      switch (rot)
      {
        case 1:
          num = 0;
          break;
        case 3:
          num = 2;
          break;
        case 6:
          num = 1;
          break;
        case 8:
          num = 3;
          break;
        default:
          return new int?();
      }
      return new int?(num * 90);
    }

    public static WriteableBitmap ApplyRotation(BitmapSource source, int clockwiseRotatedTimes)
    {
      int num = JpegUtils.ExifCodeForRotation(clockwiseRotatedTimes);
      if (num != 1)
        return JpegUtils.ApplyJpegOrientation(source, new int?(num));
      return source is WriteableBitmap writeableBitmap ? writeableBitmap : new WriteableBitmap(source);
    }

    public static WriteableBitmap DecodeJpegWithOrientation(Stream stream)
    {
      return JpegUtils.ApplyJpegOrientation((BitmapSource) JpegUtils.DecodeJpeg(stream), new int?((int) JpegUtils.GetJpegOrientation(stream) ?? 1));
    }

    public static WriteableBitmap DecodeJpegWithOrientation(
      Stream stream,
      int maxWidth,
      int maxHeight)
    {
      return JpegUtils.ApplyJpegOrientation((BitmapSource) JpegUtils.DecodeJpeg(stream, maxWidth, maxHeight), new int?((int) JpegUtils.GetJpegOrientation(stream) ?? 1));
    }

    public static WriteableBitmap DecodeJpeg(Stream stream) => PictureDecoder.DecodeJpeg(stream);

    public static WriteableBitmap DecodeJpeg(Stream stream, int maxWidth, int maxHeight)
    {
      return PictureDecoder.DecodeJpeg(stream, maxWidth, maxHeight);
    }

    public static WriteableBitmap DecodeJpeg(Stream stream, Size? maxSize)
    {
      return maxSize.HasValue ? JpegUtils.DecodeJpeg(stream, (int) maxSize.Value.Width, (int) maxSize.Value.Height) : JpegUtils.DecodeJpeg(stream);
    }

    public static void SaveJpeg(
      this WriteableBitmap bitmap,
      Stream targetStream,
      int targetWidth,
      int targetHeight,
      int orientation,
      int quality)
    {
      int[] destination = bitmap.Pixels;
      int Width = bitmap.PixelWidth;
      int Height = bitmap.PixelHeight;
      bitmap = (WriteableBitmap) null;
      IByteBuffer bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      if (targetWidth != Width || targetHeight != Height)
      {
        using (new DisposableAction((Action) (() => bb.Reset())))
        {
          GCHandle handle = GCHandle.Alloc((object) destination, GCHandleType.Pinned);
          uint Buffer = (uint) (int) handle.AddrOfPinnedObject();
          Action a = (Action) (() => handle.Free());
          bb.PutZeroCopy(Buffer, (uint) (destination.Length * 4), a.AsComAction());
          destination = (int[]) null;
          NativeInterfaces.MediaMisc.ScaleImage(bb, Width, Height, Width * 4, targetWidth, targetHeight);
          Width = targetWidth;
          Height = targetHeight;
          destination = new int[Width * Height];
          Marshal.Copy((IntPtr) (long) bb.GetPointer(), destination, 0, bb.GetLength() / 4);
        }
      }
      JpegEncoderParams @params = new JpegEncoderParams();
      @params.SourceWidth = Width;
      @params.SourceHeight = Height;
      @params.Quality = quality;
      @params.UseWaProgJpeg = false;
      IMp4Utils utils = NativeInterfaces.Mp4Utils;
      using (new DisposableAction((Action) (() => utils.DeleteThreadLocalStorage())))
      {
        using (new DisposableAction((Action) (() => bb.Reset())))
        {
          GCHandle handle = GCHandle.Alloc((object) destination, GCHandleType.Pinned);
          uint Buffer = (uint) (int) handle.AddrOfPinnedObject();
          Action a = (Action) (() => handle.Free());
          bb.PutZeroCopy(Buffer, (uint) (destination.Length * 4), a.AsComAction());
          NativeInterfaces.Misc.EncodeJpeg(bb, @params, (IJpegWriteCallback) new JpegUtils.WriteWrapper()
          {
            OnWrite = new Action<byte[], int, int>(targetStream.Write)
          });
        }
      }
    }

    public static void SaveJpegWithMaxSize(
      this WriteableBitmap bitmap,
      Stream stream,
      int targetWidth,
      int targetHeight,
      int orientation,
      int quality,
      int maxSize)
    {
      do
      {
        stream.Position = 0L;
        stream.SetLength(0L);
        bitmap.SaveJpeg(stream, targetWidth, targetHeight, orientation, quality);
        Log.d("jpeg", "{0}x{1}, quality {2} => {3} bytes (maximum {4})", (object) targetWidth, (object) targetHeight, (object) quality, (object) stream.Length, (object) maxSize);
        quality -= 10;
      }
      while (quality > 10 && maxSize > 0 && stream.Length > (long) maxSize);
    }

    public static IObservable<WriteableBitmap> DecodeJpeg(
      this IObservable<byte[]> bytes,
      Size? maxSize = null)
    {
      return Observable.CreateWithDisposable<WriteableBitmap>((Func<IObserver<WriteableBitmap>, IDisposable>) (observer => bytes.ObserveOnDispatcher<byte[]>().Subscribe<byte[]>((Action<byte[]>) (b =>
      {
        using (MemoryStream memoryStream = new MemoryStream(b))
          observer.OnNext(maxSize.HasValue ? JpegUtils.DecodeJpegWithOrientation((Stream) memoryStream, (int) maxSize.Value.Width, (int) maxSize.Value.Height) : JpegUtils.DecodeJpegWithOrientation((Stream) memoryStream));
      }), (Action<Exception>) (e =>
      {
        Log.d(e, "Exception in DecodeJpeg");
        observer.OnError(e);
      }), (Action) (() => observer.OnCompleted()))));
    }

    private delegate void YAxisProc(
      WriteableBitmap src,
      WriteableBitmap dst,
      JpegUtils.XAxisProc proc);

    private delegate void XAxisProc(
      WriteableBitmap src,
      int srcOffset,
      WriteableBitmap dst,
      ref int dstOff);

    private class WriteWrapper : IJpegWriteCallback
    {
      public Action<byte[], int, int> OnWrite;

      public void Write(byte[] buffer) => this.OnWrite(buffer, 0, buffer.Length);
    }
  }
}
