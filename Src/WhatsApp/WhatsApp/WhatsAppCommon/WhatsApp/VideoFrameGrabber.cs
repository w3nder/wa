// Decompiled with JetBrains decompiler
// Type: WhatsApp.VideoFrameGrabber
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Windows.Media;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public class VideoFrameGrabber : IDisposable
  {
    private Stream stream;
    private Mp4Atom.OrientationMatrix matrix;
    private IVideoUtils native;
    private FRAME_ATTRIBUTES frameInfo;
    private int? scaleFactor;
    private Func<Stream> streamFactory;
    private bool disposeStream = true;
    private static readonly Pair<Matrix, int>[] wellKnownMatrices = new Pair<Matrix, int>[4]
    {
      new Pair<Matrix, int>(new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0), 0),
      new Pair<Matrix, int>(new Matrix(0.0, -1.0, 1.0, 0.0, 0.0, 0.0), 90),
      new Pair<Matrix, int>(new Matrix(-1.0, 0.0, 0.0, -1.0, 0.0, 0.0), 180),
      new Pair<Matrix, int>(new Matrix(0.0, 1.0, -1.0, 0.0, 0.0, 0.0), 270)
    };

    public FRAME_ATTRIBUTES FrameInfo => this.frameInfo;

    public long DurationTicks => this.native.GetDuration();

    public double DurationSeconds => (double) this.DurationTicks * 1E-07;

    public VideoFrameGrabber(string path, int rotationAngle = -1, int? scaleFactor = null)
      : this(MediaStorage.OpenFile(MediaStorage.GetAbsolutePath(path)), rotationAngle, scaleFactor)
    {
    }

    public VideoFrameGrabber(
      Stream stream,
      int rotationAngle = -1,
      int? scaleFactor = null,
      Func<Stream> streamFactory = null,
      bool disposeStream = true)
    {
      this.disposeStream = disposeStream;
      try
      {
        this.stream = stream;
        if (streamFactory == null)
          streamFactory = (Func<Stream>) (() =>
          {
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
              return (Stream) nativeMediaStorage.GetTempFile();
          });
        this.streamFactory = streamFactory;
        this.native = NativeInterfaces.MediaMisc.OpenVideo(this.stream.ToWaStream(true), true);
        this.frameInfo = this.native.GetFrameAttributes();
        this.scaleFactor = scaleFactor;
        this.matrix = VideoFrameGrabber.MatrixForAngle(rotationAngle) ?? VideoFrameGrabber.ReadRotationMatrix(this.stream);
      }
      catch (Exception ex)
      {
        this.Dispose();
        Log.LogException(ex, "VideoFrameGrabber failed");
        throw;
      }
    }

    public int GetCurrentOrientationAngle()
    {
      return this.matrix == null ? 0 : VideoFrameGrabber.GetAngleForMatrix(this.matrix.Matrix);
    }

    public static int GetAngleForMatrix(Matrix m)
    {
      foreach (Pair<Matrix, int> wellKnownMatrix in VideoFrameGrabber.wellKnownMatrices)
      {
        if (m == wellKnownMatrix.First)
          return wellKnownMatrix.Second;
      }
      Log.d("matrix", "Orientation Matrix not in wellKnownMatrices");
      foreach (Pair<Matrix, int> wellKnownMatrix in VideoFrameGrabber.wellKnownMatrices)
      {
        if (m.M11 == wellKnownMatrix.First.M11 && m.M12 == wellKnownMatrix.First.M12 && m.M21 == wellKnownMatrix.First.M21 && m.M22 == wellKnownMatrix.First.M22)
          return wellKnownMatrix.Second;
      }
      Log.l("matrix", "Orientation Matrix not recognized. Setting to identity");
      return 0;
    }

    public static Mp4Atom.OrientationMatrix MatrixForAngle(int angle)
    {
      Matrix? nullable = new Matrix?();
      switch (angle)
      {
        case 0:
          nullable = new Matrix?(new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0));
          break;
        case 90:
          nullable = new Matrix?(new Matrix(0.0, -1.0, 1.0, 0.0, 0.0, 0.0));
          break;
        case 180:
          nullable = new Matrix?(new Matrix(-1.0, 0.0, 0.0, -1.0, 0.0, 0.0));
          break;
        case 270:
          nullable = new Matrix?(new Matrix(0.0, 1.0, -1.0, 0.0, 0.0, 0.0));
          break;
      }
      if (!nullable.HasValue)
        return (Mp4Atom.OrientationMatrix) null;
      Mp4Atom.OrientationMatrix orientationMatrix = new Mp4Atom.OrientationMatrix();
      orientationMatrix.SetMatrix(nullable.Value);
      return orientationMatrix;
    }

    public void Dispose()
    {
      if (this.disposeStream)
        this.stream.SafeDispose();
      this.stream = (Stream) null;
      IVideoUtils snap = this.native;
      this.native = (IVideoUtils) null;
      if (snap == null)
        return;
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        Log.WriteLineDebug("disposing video object...");
        snap.Dispose();
        Log.WriteLineDebug("video object disposed");
      }));
    }

    public void Seek(long ticks) => this.native.Seek(ticks);

    public VideoFrame ReadFrame()
    {
      VideoFrame r = new VideoFrame();
      Stream stream = this.streamFactory();
      try
      {
        if (this.native.GetFrame((ISampleSink) new ManagedSink()
        {
          OnBytes = (Action<byte[]>) (b => stream.Write(b, 0, b.Length)),
          OnTimestamp = (Action<long>) (ts => r.Timestamp = ts)
        }) && stream.Length == 0L)
        {
          stream.SafeDispose();
          return (VideoFrame) null;
        }
        stream.Position = 0L;
        r.Matrix = this.matrix;
        r.FrameInfo = this.frameInfo;
        r.Stream = stream;
        r.Stride = this.native.GetStride();
        r.ScaleFactor = this.scaleFactor;
        stream = (Stream) null;
        return r;
      }
      finally
      {
        stream.SafeDispose();
      }
    }

    private static Mp4Atom.OrientationMatrix ReadRotationMatrix(Stream file)
    {
      long? nullable = new long?();
      Mp4Atom.OrientationMatrix r = (Mp4Atom.OrientationMatrix) null;
      try
      {
        nullable = new long?(file.Position);
        file.Position = 0L;
        Mp4Atom.GetOrientationMatrices(file, file.Length, (Mp4Atom.MatrixParserCallback) ((name, offset, m, cancel) =>
        {
          Matrix matrix = m.Matrix;
          Log.WriteLineDebug("Found transform matrix in [{0}]: [[{1} {2}] [{3} {4}] [{5} {6}]]", (object) name, (object) matrix.M11, (object) matrix.M12, (object) matrix.M21, (object) matrix.M22, (object) matrix.OffsetX, (object) matrix.OffsetY);
          if (m.IsIdentity)
            return;
          r = m;
        }));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "get transform matrix");
      }
      finally
      {
        if (nullable.HasValue)
          file.Position = nullable.Value;
      }
      return r;
    }

    public static void WriteRotationMatrix(Stream file, Matrix newRotation)
    {
      try
      {
        Mp4Atom.GetOrientationMatrices(file, file.Length, (Mp4Atom.MatrixParserCallback) ((name, offset, m, cancel) =>
        {
          Matrix matrix = m.Matrix;
          Log.WriteLineDebug("Found transform matrix in [{0}]: [[{1} {2}] [{3} {4}] [{5} {6}]]", (object) name, (object) matrix.M11, (object) matrix.M12, (object) matrix.M21, (object) matrix.M22, (object) matrix.OffsetX, (object) matrix.OffsetY);
          if (!(name != "mvhd"))
            return;
          m.SetMatrix(newRotation);
          m.WriteMatrixToStream(file, offset);
        }));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "write transform matrix");
      }
    }

    public static void WriteRotationMatrix(string filename, Matrix newRotation)
    {
      using (IMediaStorage mediaStorage = MediaStorage.Create(filename))
      {
        using (Stream file = mediaStorage.OpenFile(filename, access: FileAccess.ReadWrite))
          VideoFrameGrabber.WriteRotationMatrix(file, newRotation);
      }
    }
  }
}
