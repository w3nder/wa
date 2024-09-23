// Decompiled with JetBrains decompiler
// Type: WhatsApp.VideoFrame
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class VideoFrame : IDisposable
  {
    internal Stream Stream;
    internal Mp4Atom.OrientationMatrix Matrix;
    internal FRAME_ATTRIBUTES FrameInfo;
    internal int Stride;
    internal int? ScaleFactor;
    public long Timestamp;
    private MatrixTransform tranform;
    private WriteableBitmap bitmap;

    private MatrixTransform Transform
    {
      get
      {
        if (this.tranform != null || this.Matrix == null)
          return this.tranform;
        this.tranform = new MatrixTransform()
        {
          Matrix = this.Matrix.Matrix
        };
        return this.tranform;
      }
    }

    public WriteableBitmap Bitmap
    {
      get
      {
        WriteableBitmap bitmap = this.bitmap ?? (this.bitmap = this.DecodeAndApplyTransform());
        this.Stream.SafeDispose();
        this.Stream = (Stream) null;
        return bitmap;
      }
    }

    public WriteableBitmap GetScaledBitmap()
    {
      if (this.bitmap != null)
        return this.bitmap;
      Log.l(nameof (GetScaledBitmap), "Running {0}", (object) this.Timestamp);
      int width = (int) this.FrameInfo.Width;
      int height = (int) this.FrameInfo.Height;
      Action<int[]> action = new Action<int[]>(this.Decode);
      if (this.ScaleFactor.HasValue)
      {
        int sf = this.ScaleFactor.Value;
        Func<int, int> func = (Func<int, int>) (input => Math.Max(1, input / sf));
        width = func(width);
        height = func(height);
        IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        using (MemoryStream destination = new MemoryStream())
        {
          this.Stream.CopyTo((Stream) destination);
          instance.Put(destination.GetBuffer(), 0, (int) destination.Length);
        }
        NativeInterfaces.MediaMisc.ScaleImage(instance, (int) this.FrameInfo.Width, (int) this.FrameInfo.Height, this.Stride, width, height);
        byte[] inputBuffer = instance.Get();
        instance.Reset();
        action = (Action<int[]>) (b =>
        {
          int stride = width * 4;
          int off = -stride;
          this.Decode(b, width, height, inputBuffer, (Func<int>) (() => off += stride));
        });
      }
      int[] buffer = new int[width * height];
      action(buffer);
      WriteableBitmap wb = (WriteableBitmap) null;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
      {
        wb = new WriteableBitmap(width, height);
        Array.Copy((Array) buffer, (Array) wb.Pixels, Math.Min(buffer.Length, wb.Pixels.Length));
        wb.Invalidate();
        wb = this.ApplyTransform(wb);
      }));
      return wb;
    }

    private void Decode(int[] pixels)
    {
      byte[] line = this.Stride >= 0 ? new byte[Math.Abs(this.Stride)] : throw new Exception("Negative stride not supported for now.");
      this.Decode(pixels, (int) this.FrameInfo.Width, (int) this.FrameInfo.Height, line, (Func<int>) (() =>
      {
        if (line.Length != this.Stream.Read(line, 0, line.Length))
          throw new IOException("unexpected EOF");
        return 0;
      }));
    }

    private void Decode(int[] pixels, int width, int height, byte[] line, Func<int> getLine)
    {
      int num1 = 0;
      for (int index1 = 0; index1 < height; ++index1)
      {
        int num2 = getLine();
        for (int index2 = 0; index2 < width; ++index2)
        {
          uint num3 = (uint) ((int) line[num2 + 0] | (int) line[num2 + 1] << 8 | (int) line[num2 + 2] << 16 | -16777216);
          pixels[num1++] = (int) num3;
          num2 += 4;
        }
      }
    }

    private WriteableBitmap Decode()
    {
      if (this.Stream == null)
        return (WriteableBitmap) null;
      WriteableBitmap writeableBitmap = new WriteableBitmap((int) this.FrameInfo.Width, (int) this.FrameInfo.Height);
      this.Decode(writeableBitmap.Pixels);
      writeableBitmap.Invalidate();
      return writeableBitmap;
    }

    private WriteableBitmap DecodeAndApplyTransform() => this.ApplyTransform(this.Decode());

    private WriteableBitmap ApplyTransform(WriteableBitmap bitmap)
    {
      if (bitmap == null)
        return (WriteableBitmap) null;
      MatrixTransform matrixTransform = this.Transform;
      if (matrixTransform != null)
      {
        bool flag1 = false;
        System.Windows.Media.Matrix matrix = matrixTransform.Matrix;
        if (matrix.OffsetX == 0.0 && matrix.OffsetY == 0.0)
        {
          double pixelWidth = (double) bitmap.PixelWidth;
          double pixelHeight = (double) bitmap.PixelHeight;
          double[][] numArray1 = new double[4][]
          {
            new double[6]{ 1.0, 0.0, 0.0, 1.0, 0.0, 0.0 },
            new double[6]{ 0.0, 1.0, -1.0, 0.0, pixelHeight, 0.0 },
            new double[6]
            {
              -1.0,
              0.0,
              0.0,
              -1.0,
              pixelWidth,
              pixelHeight
            },
            new double[6]{ 0.0, -1.0, 1.0, 0.0, 0.0, pixelWidth }
          };
          double[] numArray2 = new double[4]
          {
            matrix.M11,
            matrix.M12,
            matrix.M21,
            matrix.M22
          };
          foreach (double[] numArray3 in numArray1)
          {
            bool flag2 = false;
            for (int index = 0; index < numArray2.Length; ++index)
            {
              if (numArray2[index] != numArray3[index])
              {
                flag2 = true;
                break;
              }
            }
            if (!flag2)
            {
              matrix.OffsetX = numArray3[4];
              matrix.OffsetY = numArray3[5];
              matrixTransform.Matrix = matrix;
              flag1 = true;
              break;
            }
          }
          if (!flag1)
            matrixTransform = (MatrixTransform) null;
        }
      }
      if (matrixTransform != null)
        bitmap = bitmap.Transform((System.Windows.Media.Transform) matrixTransform);
      return bitmap;
    }

    public void Dispose()
    {
      this.Stream.SafeDispose();
      this.Stream = (Stream) null;
      this.bitmap = (WriteableBitmap) null;
    }
  }
}
