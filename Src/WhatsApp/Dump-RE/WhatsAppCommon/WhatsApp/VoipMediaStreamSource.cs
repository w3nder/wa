// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipMediaStreamSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class VoipMediaStreamSource : MediaStreamSource
  {
    private MediaStreamDescription streamDesc;
    private Dictionary<MediaStreamAttributeKeys, string> mediaAttr;
    private Dictionary<MediaSampleAttributeKeys, string> sampleAttr;
    private int requestCount;
    private AutoResetEvent kickSampleThread = new AutoResetEvent(false);
    private object queueLock = new object();
    private ManualResetEvent queueAvailable = new ManualResetEvent(false);
    private Queue<VoipMediaStreamSource.NAL> frameQueue = new Queue<VoipMediaStreamSource.NAL>();
    private long lastSuppliedTs;
    private Thread sampleSupplyThread;
    private volatile bool shutdown;
    private static int? lastWidth;
    private static int? lastHeight;
    private int droppedFrames;
    private bool waitingForKeyframe = true;
    private const int MAX_FRAMES = 6;
    private const int MAX_WAIT_FOR_FIRST_FRAME_POLL_MS = 5000;
    private const int MAX_WAIT_FOR_NEXT_FRAMES_POLL_MS = 2000;
    private VideoCodec codec;
    private bool openPending;
    private bool videoStalled;
    private int waitForNextFrameRequestMs = 5000;

    public event VoipMediaStreamSource.OrientationChangedHandler OrientationChanged;

    public bool AddSample(
      byte[] buffer,
      VideoCodec codec,
      int width,
      int height,
      long timestamp,
      bool keyframe,
      VideoOrientation? orientation)
    {
      if (this.videoStalled)
        return false;
      lock (this.queueLock)
      {
        if (this.codec == VideoCodec.None)
        {
          this.codec = codec;
          if (this.openPending)
          {
            if (codec == VideoCodec.VPX)
            {
              VoipMediaStreamSource.lastWidth = new int?(width);
              VoipMediaStreamSource.lastHeight = new int?(height);
            }
            else
            {
              VoipMediaStreamSource.lastWidth = new int?();
              VoipMediaStreamSource.lastHeight = new int?();
            }
            this.OpenCompleted();
          }
        }
        else if (this.codec != codec)
        {
          Log.l("voip mss", "Incorrect codec {0}", (object) codec);
          return true;
        }
        if (VoipMediaStreamSource.lastWidth.HasValue)
        {
          int? lastWidth = VoipMediaStreamSource.lastWidth;
          int num1 = width;
          if ((lastWidth.GetValueOrDefault() == num1 ? (!lastWidth.HasValue ? 1 : 0) : 1) != 0)
          {
            lastWidth = VoipMediaStreamSource.lastWidth;
            int num2 = 0;
            if ((lastWidth.GetValueOrDefault() > num2 ? (lastWidth.HasValue ? 1 : 0) : 0) != 0)
            {
              Log.l("voip mss", "Resolution changed {0} -> {1}, restart renderer", (object) VoipMediaStreamSource.lastWidth, (object) width);
              VoipMediaStreamSource.lastWidth = new int?(-1);
            }
            return false;
          }
        }
        if (this.waitingForKeyframe)
        {
          if (!keyframe)
            return true;
          this.waitingForKeyframe = false;
        }
        this.frameQueue.Enqueue(new VoipMediaStreamSource.NAL()
        {
          ts = DateTime.Now.Ticks,
          buffer = buffer,
          keyframe = keyframe,
          orientation = orientation,
          width = width,
          height = height
        });
        int num = 0;
        if (this.frameQueue.Count > 6)
        {
          do
          {
            this.frameQueue.Dequeue();
            ++num;
          }
          while (this.frameQueue.Count > 0 && !this.frameQueue.Peek().keyframe);
          if (this.frameQueue.Count == 0)
          {
            Log.l("voip mss", "dropped all {0} frames. Waiting for keyframe.", (object) num);
            this.waitingForKeyframe = true;
            Voip.Worker.Enqueue((Action) (() =>
            {
              try
              {
                Voip.Instance.RequestKeyframe();
                Voip.Instance.SetPeerVideoFlowControl(0U, (ushort) 0, (ushort) 15);
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "RequestKeyframe", false);
              }
            }));
            return true;
          }
        }
        if (num > 0)
        {
          this.droppedFrames += num;
          Log.l("voip mss", "dropped {0} consecutive frames", (object) this.droppedFrames);
        }
        else
          this.droppedFrames = 0;
        this.queueAvailable.Set();
      }
      return true;
    }

    private VoipMediaStreamSource.NAL DequeueSample()
    {
      while (true)
      {
        this.queueAvailable.WaitOne();
        lock (this.queueLock)
        {
          if (this.shutdown)
            return new VoipMediaStreamSource.NAL();
          if (this.frameQueue.Count == 0)
          {
            this.queueAvailable.Reset();
          }
          else
          {
            VoipMediaStreamSource.NAL nal = this.frameQueue.Dequeue();
            if (this.frameQueue.Count == 0)
              this.queueAvailable.Reset();
            return nal;
          }
        }
      }
    }

    private MediaStreamSample CreateNullSample()
    {
      return new MediaStreamSample(this.streamDesc, (Stream) new MemoryStream(), 0L, 0L, this.lastSuppliedTs, (IDictionary<MediaSampleAttributeKeys, string>) this.sampleAttr);
    }

    private MediaStreamSample CreateSample(VoipMediaStreamSource.NAL data)
    {
      if (this.codec == VideoCodec.H26X)
      {
        if (data.keyframe)
          this.sampleAttr[MediaSampleAttributeKeys.KeyFrameFlag] = true.ToString();
        else
          this.sampleAttr.Remove(MediaSampleAttributeKeys.KeyFrameFlag);
      }
      else
      {
        this.sampleAttr[MediaSampleAttributeKeys.FrameWidth] = data.width.ToString();
        this.sampleAttr[MediaSampleAttributeKeys.FrameHeight] = data.height.ToString();
      }
      return new MediaStreamSample(this.streamDesc, (Stream) new MemoryStream(data.buffer, false), 0L, (long) data.buffer.Length, data.ts, (IDictionary<MediaSampleAttributeKeys, string>) this.sampleAttr);
    }

    protected override void CloseMedia()
    {
      Log.l("VoipMSS", nameof (CloseMedia));
      this.shutdown = true;
      if (this.openPending)
        return;
      this.kickSampleThread.Set();
      this.queueAvailable.Set();
      if (this.sampleSupplyThread == null)
        return;
      Thread thread = this.sampleSupplyThread;
      this.sampleSupplyThread = (Thread) null;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        Log.l("VoipMSS", "sampleSupplyThread waiting...");
        thread.Join();
        Log.l("VoipMSS", "sampleSupplyThread joined");
      }));
    }

    protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
    {
      throw new NotImplementedException();
    }

    protected override void GetSampleAsync(MediaStreamType mediaStreamType)
    {
      if (Interlocked.Increment(ref this.requestCount) > 0)
        return;
      this.kickSampleThread.Set();
    }

    private void SampleSupplyProc()
    {
      while (true)
      {
        if (Interlocked.Decrement(ref this.requestCount) == -1)
        {
          if (!this.videoStalled)
          {
            if (this.kickSampleThread.WaitOne(this.waitForNextFrameRequestMs))
            {
              this.waitForNextFrameRequestMs = 2000;
            }
            else
            {
              Log.l("Video frames requests stalled");
              this.videoStalled = true;
            }
          }
          if (this.videoStalled)
            this.kickSampleThread.WaitOne();
        }
        VoipMediaStreamSource.NAL data = this.DequeueSample();
        if (!this.shutdown)
        {
          if (data.orientation.HasValue)
          {
            try
            {
              VoipMediaStreamSource.OrientationChangedHandler orientationChanged = this.OrientationChanged;
              if (orientationChanged != null)
                orientationChanged(data.orientation.Value);
            }
            catch (Exception ex)
            {
            }
          }
          this.lastSuppliedTs = data.ts;
          this.ReportGetSampleCompleted(this.CreateSample(data));
        }
        else
          break;
      }
      while (Interlocked.Decrement(ref this.requestCount) > -1)
        this.ReportGetSampleCompleted(this.CreateNullSample());
    }

    protected override void OpenMediaAsync()
    {
      Log.l("VoipMSS", nameof (OpenMediaAsync));
      lock (this.queueLock)
      {
        if (this.codec != VideoCodec.None)
          this.OpenCompleted();
        else
          this.openPending = true;
      }
    }

    private void OpenCompleted()
    {
      Log.l("VoipMSS", "OpenMediaAsync Completed");
      int? nullable = VoipMediaStreamSource.lastWidth;
      string str1 = (nullable ?? 320).ToString();
      nullable = VoipMediaStreamSource.lastHeight;
      string str2 = (nullable ?? 240).ToString();
      this.mediaAttr = new Dictionary<MediaStreamAttributeKeys, string>()
      {
        {
          MediaStreamAttributeKeys.VideoFourCC,
          this.codec == VideoCodec.H26X ? "H264" : "YV12"
        },
        {
          MediaStreamAttributeKeys.Width,
          str1
        },
        {
          MediaStreamAttributeKeys.Height,
          str2
        }
      };
      this.sampleAttr = new Dictionary<MediaSampleAttributeKeys, string>()
      {
        {
          MediaSampleAttributeKeys.FrameWidth,
          str1
        },
        {
          MediaSampleAttributeKeys.FrameHeight,
          str2
        }
      };
      this.streamDesc = new MediaStreamDescription(MediaStreamType.Video, (IDictionary<MediaStreamAttributeKeys, string>) this.mediaAttr);
      this.sampleSupplyThread = new Thread(new ThreadStart(this.SampleSupplyProc));
      this.sampleSupplyThread.Start();
      this.ReportOpenMediaCompleted((IDictionary<MediaSourceAttributesKeys, string>) new Dictionary<MediaSourceAttributesKeys, string>()
      {
        {
          MediaSourceAttributesKeys.CanSeek,
          false.ToString()
        }
      }, (IEnumerable<MediaStreamDescription>) new List<MediaStreamDescription>()
      {
        this.streamDesc
      });
      this.openPending = false;
    }

    protected override void SeekAsync(long seekToTime) => this.ReportSeekCompleted(seekToTime);

    protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
    {
      throw new NotImplementedException();
    }

    private struct NAL
    {
      public long ts;
      public bool keyframe;
      public byte[] buffer;
      public int width;
      public int height;
      public VideoOrientation? orientation;
    }

    public delegate void OrientationChangedHandler(VideoOrientation newOrientation);
  }
}
