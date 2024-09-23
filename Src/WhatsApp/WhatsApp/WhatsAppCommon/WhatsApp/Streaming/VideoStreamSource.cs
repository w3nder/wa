// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.VideoStreamSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;
using WhatsAppNative;


namespace WhatsApp.Streaming
{
  public class VideoStreamSource : 
    MediaStreamSource,
    IStreamingMp4DemuxCallbacks,
    IMp4UtilsMetadataReceiver,
    IDisposable
  {
    private const string LogHeader = "VideoStreamSource";
    private const double oneSecond = 10000000.0;
    private const ulong nonTimestampedNALFrame = 18446744073709551615;
    private const string videoCodec = "H264";
    private const long bufferTime = 30000000;
    private const int initialBufferEstimateTime = 1000;
    private const string videoHeader = "Video";
    private const string audioHeader = "Audio";
    private const int waveFormatExAACFormatTag = 255;
    private const int audioCBSize = 0;
    private Dictionary<MediaSampleAttributeKeys, string> sampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>();
    private MediaStreamDescription videoStreamDescription;
    private MediaStreamDescription audioStreamDescription;
    private Stream videoStream;
    private Stream audioStream;
    private object videoLock = new object();
    private object audioLock = new object();
    private int rotationAngle;
    private float framesPerSecond;
    private long duration;
    private int width;
    private int height;
    private List<MediaStreamSample> videoSamples = new List<MediaStreamSample>();
    private Dictionary<long, int> seekableVideoSamples = new Dictionary<long, int>();
    private List<long> seekableVideoTimes = new List<long>();
    private long lastParsedVideoSampleTimestamp;
    private int videoSamplesReadIndex;
    private long seekTargetTime;
    private bool sentFirstVideoSample;
    private int audioNumberOfChannels;
    private int audioSamplesPerSecond;
    private int audioAverageBytesPerSecond;
    private int audioBlockAlign;
    private int audioBitsPerSample;
    private List<MediaStreamSample> audioSamples = new List<MediaStreamSample>();
    private List<long> seekableAudioSamples = new List<long>();
    private int audioSamplesReadIndex;
    private long audioBytesUntilNextFrame;
    private long audioStreamParseOffset;
    private long audioStreamLastAACFrameStartOffset;
    private long audioStreamLastAACFrameTimestamp;
    private bool sentFirstAudioSample;
    private bool didRequestOpenMedia;
    private bool allBytesReceived;
    private long bufferStartTime;
    private long bufferTargetTime;
    private bool videoStreamIsBuffering;
    private bool audioStreamIsBuffering;
    public IVideoStreamSourceBufferingCallbacks BufferingCallbacks;
    private IVideoStreamSourceErrorCallbacks errorCallbacks;
    private bool? videoIsStreamable;
    private VideoStreamSource.ReadSamplesThread readVideoSamplesThread;
    private VideoStreamSource.ReadSamplesThread readAudioSamplesThread;
    private Func<string> errorMessageFetcher;
    private System.Threading.Timer bufferTimer;
    private DateTime? sendStartTime;

    public VideoStreamSource(Func<string> errorMessageFetcher = null)
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        this.videoStream = (Stream) nativeMediaStorage.GetTempFile();
        this.audioStream = (Stream) nativeMediaStorage.GetTempFile();
      }
      this.AudioBufferLength = 15;
      this.errorMessageFetcher = errorMessageFetcher;
    }

    public bool DidRequestOpenMedia => this.didRequestOpenMedia;

    public IVideoStreamSourceErrorCallbacks ErrorCallbacks
    {
      set
      {
        this.errorCallbacks = value;
        bool? videoIsStreamable = this.videoIsStreamable;
        bool flag = false;
        if ((videoIsStreamable.GetValueOrDefault() == flag ? (videoIsStreamable.HasValue ? 1 : 0) : 0) == 0)
          return;
        this.errorCallbacks.CouldNotStream();
      }
      get => this.errorCallbacks;
    }

    public void OnVideoBytes(
      byte[] h264,
      int offset,
      int length,
      bool isSeekPoint,
      ulong timestamp)
    {
      lock (this.videoLock)
      {
        if (this.videoStream == null || this.videoSamples == null)
          return;
        if (!this.AssertValidNALUnit(h264, offset, length))
        {
          string str = string.Format("Received invalid NAL unit (sample #{0}) : [ ", (object) this.videoSamples.Count);
          for (int index = offset; index < offset + length; ++index)
            str += string.Format("{0} ", (object) h264[index]);
          Log.SendCrashLog((Exception) new ArgumentException("Invalid NAL unit received in VideoStreamSource"), str + "]");
        }
        else
        {
          this.videoStream.Write(h264, offset, length);
          long timestamp1 = timestamp == ulong.MaxValue ? this.lastParsedVideoSampleTimestamp : (long) timestamp;
          Assert.IsTrue(this.lastParsedVideoSampleTimestamp <= timestamp1, "Should not travel back in time while parsing video frames");
          this.lastParsedVideoSampleTimestamp = timestamp1;
          this.videoSamples.Add(new MediaStreamSample(this.videoStreamDescription, this.videoStream, this.videoStream.Length - (long) length, (long) length, timestamp1, (IDictionary<MediaSampleAttributeKeys, string>) this.CreateVideoSampleAttributes(isSeekPoint)));
          if (isSeekPoint && !this.seekableVideoSamples.ContainsKey((long) timestamp))
          {
            this.seekableVideoSamples.Add((long) timestamp, this.videoSamples.Count - 1);
            this.seekableVideoTimes.Add((long) timestamp);
          }
          if (!this.videoStreamIsBuffering || this.bufferTimer != null)
            return;
          if (!this.sentFirstVideoSample)
          {
            this.sentFirstVideoSample = true;
            Log.d(nameof (VideoStreamSource), "Sending first video sample after buffering ended");
          }
          if (this.readVideoSamplesThread != null)
            ++this.readVideoSamplesThread.readAfterBuffering;
          this.readVideoSamplesThread?.RequestSample();
        }
      }
    }

    public void OnAudioBytes(
      byte[] aac,
      int offset,
      int length,
      bool isSeekPoint,
      ulong timestamp)
    {
      lock (this.audioLock)
      {
        if (this.audioStream == null || this.audioSamples == null)
          return;
        this.audioStream.Write(aac, offset, length);
        this.StripADTSHeaders(timestamp);
        if (!this.audioStreamIsBuffering || this.bufferTimer != null)
          return;
        if (!this.sentFirstAudioSample)
        {
          this.sentFirstAudioSample = true;
          Log.d(nameof (VideoStreamSource), "Sending first audio sample after buffering ended");
        }
        if (this.readAudioSamplesThread != null)
          ++this.readAudioSamplesThread.readAfterBuffering;
        this.readAudioSamplesThread?.RequestSample();
      }
    }

    public void OnVideoMetadata(
      int Width,
      int Height,
      float Fps,
      float Duration,
      int RotationAngle)
    {
      Log.l(nameof (VideoStreamSource), string.Format("Video metadata received: Dimensions ({0} x {1}), {2} FPS, {3}s, Rotation {4} degrees", (object) Width, (object) Height, (object) Fps, (object) Duration, (object) RotationAngle));
      this.width = Width;
      this.height = Height;
      this.framesPerSecond = Fps;
      this.rotationAngle = 360 - RotationAngle;
      long val2 = (long) ((double) Duration * 10000000.0);
      this.duration = this.duration <= 0L ? val2 : Math.Min(this.duration, val2);
      Assert.IsTrue(this.videoStreamDescription == null, "Video stream description should not be initialized twice");
      this.videoStreamDescription = this.CreateVideoStreamDescription();
    }

    public void OnAudioMetadata(
      int sampleRate,
      int channels,
      int bitsPerSample,
      int avgBytesPerSec,
      int blockAlign,
      float duration)
    {
      Log.l(nameof (VideoStreamSource), string.Format("Audio metadata received: Sample rate ({0}), {1} channels, {2} bits per sample, {3} bytes per second, block align {4}, duration {5}", (object) sampleRate, (object) channels, (object) bitsPerSample, (object) avgBytesPerSec, (object) blockAlign, (object) duration));
      this.audioSamplesPerSecond = sampleRate;
      this.audioNumberOfChannels = channels;
      this.audioBitsPerSample = bitsPerSample;
      this.audioAverageBytesPerSecond = avgBytesPerSec;
      this.audioBlockAlign = blockAlign;
      long val2 = (long) ((double) duration * 10000000.0);
      this.duration = this.duration <= 0L ? val2 : Math.Min(this.duration, val2);
      Assert.IsTrue(this.audioStreamDescription == null, "Audio stream description should not be initialized twice");
      this.audioStreamDescription = this.CreateAudioStreamDescription();
    }

    public void OnMetadataComplete()
    {
      Log.l(nameof (VideoStreamSource), nameof (OnMetadataComplete));
      this.videoStreamIsBuffering = true;
      if (this.AudioStreamExists())
        this.audioStreamIsBuffering = true;
      this.UpdateBufferTargetTime(new long?(this.duration));
      if (this.didRequestOpenMedia)
      {
        this.videoIsStreamable = new bool?(true);
        this.StartStreamReaderThreads();
        this.ReportOpenMediaCompleted((IDictionary<MediaSourceAttributesKeys, string>) this.CreateMediaSourceAttributes(), (IEnumerable<MediaStreamDescription>) this.CreateAvailableMediaStreams());
        Log.l(nameof (VideoStreamSource), "ReportOpenMediaCompleted called from OnMetadataComplete");
      }
      this.BufferingCallbacks?.StartedBuffering();
      this.bufferTimer = new System.Threading.Timer(new TimerCallback(this.EstimateBufferTime), (object) this, 1000, -1);
    }

    public void OnError(Exception e)
    {
      Log.LogException(e, "Receiving streamed video");
      if (e is Mp4CannotStreamException)
      {
        Log.l(nameof (VideoStreamSource), "The video is not streamable");
        this.errorCallbacks?.CouldNotStream();
        this.videoIsStreamable = new bool?(false);
      }
      else
      {
        Func<string> errorMessageFetcher = this.errorMessageFetcher;
        string errorDescription = errorMessageFetcher != null ? errorMessageFetcher() : (string) null;
        if (errorDescription != null)
          this.ErrorOccurred(errorDescription);
        else if (e is HttpStatusException)
        {
          Log.l(nameof (VideoStreamSource), "HttpStatusException: {0}", (object) e.GetFriendlyMessage());
          this.ErrorOccurred(AppResources.VideoNetworkError);
        }
        else
          this.ErrorOccurred(AppResources.VideoError);
      }
    }

    public void OnEndOfFile()
    {
      Log.l(nameof (VideoStreamSource), "Stream fully received");
      this.allBytesReceived = true;
      if (this.bufferTimer != null)
        return;
      bool flag = false;
      lock (this.videoLock)
      {
        if (this.videoStreamIsBuffering)
        {
          if (!this.sentFirstVideoSample)
          {
            if (!flag)
            {
              flag = true;
              this.ExitBufferingState();
            }
            Log.d(nameof (VideoStreamSource), "Video sample being sent after EOF");
            if (this.readVideoSamplesThread != null)
              ++this.readVideoSamplesThread.readAfterEof;
            this.readVideoSamplesThread?.RequestSample();
          }
        }
      }
      lock (this.audioLock)
      {
        if (!this.audioStreamIsBuffering || !this.AudioStreamExists() || this.sentFirstAudioSample)
          return;
        if (!flag)
          this.ExitBufferingState();
        Log.d(nameof (VideoStreamSource), "Audio sample being sent after EOF");
        if (this.readAudioSamplesThread != null)
          ++this.readAudioSamplesThread.readAfterEof;
        this.readAudioSamplesThread?.RequestSample();
      }
    }

    private void EstimateBufferTime(object _)
    {
      this.bufferTimer.SafeDispose();
      this.bufferTimer = (System.Threading.Timer) null;
      double num1 = 1.0;
      double num2 = (double) this.GetTotalBufferedTime() / (10000000.0 * num1);
      if (num2 >= num1)
      {
        Log.l(nameof (VideoStreamSource), string.Format("Buffered {0}s of the video in {1}s - exiting buffering", (object) num2, (object) num1));
        this.BufferingCallbacks?.BufferingProgressChanged(1.0);
        this.ExitBufferingState();
        if (!this.allBytesReceived)
          return;
        lock (this.videoLock)
        {
          if (!this.sentFirstVideoSample)
          {
            this.sentFirstVideoSample = true;
            Assert.IsTrue(this.readVideoSamplesThread != null, "Video samples thread was null after estimating buffer progress time and all bytes were received");
            if (this.readVideoSamplesThread != null)
              ++this.readVideoSamplesThread.readAfterBufferingProgress;
            this.readVideoSamplesThread?.RequestSample();
            Log.d(nameof (VideoStreamSource), "Sent first video sample after estimating buffering time");
          }
        }
        lock (this.audioLock)
        {
          if (this.sentFirstAudioSample || !this.AudioStreamExists())
            return;
          this.sentFirstAudioSample = true;
          Assert.IsTrue(this.readAudioSamplesThread != null, "Audio samples thread was null after estimating buffer progress time and all bytes were received");
          if (this.readAudioSamplesThread != null)
            ++this.readAudioSamplesThread.readAfterBufferingProgress;
          this.readAudioSamplesThread?.RequestSample();
          Log.d(nameof (VideoStreamSource), "Sent first audio sample after estimating buffering time");
        }
      }
      else
      {
        double num3 = (double) this.duration * ((num1 - num2) / num1);
        Log.l(nameof (VideoStreamSource), string.Format("Buffered {0}s of the video in {1}s - will buffer {2} of the video before playing", (object) num2, (object) num1, (object) (num3 / 10000000.0)));
        this.UpdateBufferTargetTime(new long?((long) num3));
      }
    }

    private List<MediaStreamDescription> CreateAvailableMediaStreams()
    {
      List<MediaStreamDescription> availableMediaStreams = new List<MediaStreamDescription>();
      availableMediaStreams.Add(this.videoStreamDescription);
      if (this.AudioStreamExists())
        availableMediaStreams.Add(this.audioStreamDescription);
      return availableMediaStreams;
    }

    private void StartStreamReaderThreads()
    {
      this.readVideoSamplesThread.SafeDispose();
      this.readVideoSamplesThread = new VideoStreamSource.ReadSamplesThread("Video", new Action(this.ReadVideoStream));
      this.readVideoSamplesThread.Start();
      if (!this.AudioStreamExists())
        return;
      this.readAudioSamplesThread.SafeDispose();
      this.readAudioSamplesThread = new VideoStreamSource.ReadSamplesThread("Audio", new Action(this.ReadAudioStream));
      this.readAudioSamplesThread.Start();
    }

    private bool AssertValidNALUnit(byte[] h264, int offset, int length)
    {
      return Assert.IsTrue(h264.Length >= offset + length, "The NAL unit byte array must at least be long enough to fit the NAL unit") && Assert.IsTrue(length > 3, "The NAL unit must at least be long enough to fit the start code") && Assert.IsTrue(h264[offset] == (byte) 0 && h264[offset + 1] == (byte) 0, "The first two bytes must be 0") && Assert.IsTrue(h264[offset + 2] == (byte) 1 || h264[offset + 2] == (byte) 0 && h264[offset + 3] == (byte) 1, "The third or fourth byte must be 1");
    }

    private Dictionary<MediaSampleAttributeKeys, string> CreateVideoSampleAttributes(bool isSeekable)
    {
      return new Dictionary<MediaSampleAttributeKeys, string>()
      {
        {
          MediaSampleAttributeKeys.KeyFrameFlag,
          isSeekable.ToString()
        },
        {
          MediaSampleAttributeKeys.DRMAlgorithmID,
          ContentKeyType.Unprotected.ToString()
        },
        {
          MediaSampleAttributeKeys.FrameWidth,
          this.width.ToString()
        },
        {
          MediaSampleAttributeKeys.FrameHeight,
          this.height.ToString()
        }
      };
    }

    private MediaStreamDescription CreateVideoStreamDescription()
    {
      return new MediaStreamDescription(MediaStreamType.Video, (IDictionary<MediaStreamAttributeKeys, string>) new Dictionary<MediaStreamAttributeKeys, string>()
      {
        {
          MediaStreamAttributeKeys.VideoFourCC,
          "H264"
        },
        {
          MediaStreamAttributeKeys.Width,
          this.width.ToString()
        },
        {
          MediaStreamAttributeKeys.Height,
          this.height.ToString()
        }
      });
    }

    private MediaStreamDescription CreateAudioStreamDescription()
    {
      Dictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
      string str = this.ConstructWaveFormatExString((int) byte.MaxValue, this.audioNumberOfChannels, this.audioSamplesPerSecond, this.audioAverageBytesPerSecond, this.audioBlockAlign, this.audioBitsPerSample, 0);
      mediaStreamAttributes.Add(MediaStreamAttributeKeys.CodecPrivateData, str);
      return new MediaStreamDescription(MediaStreamType.Audio, (IDictionary<MediaStreamAttributeKeys, string>) mediaStreamAttributes);
    }

    private bool AudioStreamExists() => this.audioStreamDescription != null;

    private string ConstructWaveFormatExString(
      int wFormatTag,
      int nChannels,
      int nSamplesPerSec,
      int nAvgBytesPerSec,
      int nBlockAlign,
      int wBitsPerSample,
      int cbSize)
    {
      return this.CreateLittleEndianHexString(wFormatTag, 4) + this.CreateLittleEndianHexString(nChannels, 4) + this.CreateLittleEndianHexString(nSamplesPerSec, 8) + this.CreateLittleEndianHexString(nAvgBytesPerSec, 8) + this.CreateLittleEndianHexString(nBlockAlign, 4) + this.CreateLittleEndianHexString(wBitsPerSample, 4) + this.CreateLittleEndianHexString(cbSize, 4);
    }

    private string CreateLittleEndianHexString(int value, int length)
    {
      Assert.IsTrue(length % 2 == 0, "Little endian strings should have even length");
      string str = value.ToString(string.Format("X{0}", (object) length));
      string littleEndianHexString = "";
      for (int startIndex = 0; startIndex < length; startIndex += 2)
        littleEndianHexString = str.Substring(startIndex, 2) + littleEndianHexString;
      return littleEndianHexString;
    }

    private Dictionary<MediaSourceAttributesKeys, string> CreateMediaSourceAttributes()
    {
      string str = this.duration.ToString();
      return new Dictionary<MediaSourceAttributesKeys, string>()
      {
        {
          MediaSourceAttributesKeys.CanSeek,
          true.ToString()
        },
        {
          MediaSourceAttributesKeys.Duration,
          str
        }
      };
    }

    protected override void OpenMediaAsync()
    {
      Log.l(nameof (VideoStreamSource), "MediaElement requested opening media");
      this.didRequestOpenMedia = true;
      if (!this.videoIsStreamable.HasValue)
        return;
      this.StartStreamReaderThreads();
      this.ReportOpenMediaCompleted((IDictionary<MediaSourceAttributesKeys, string>) this.CreateMediaSourceAttributes(), (IEnumerable<MediaStreamDescription>) this.CreateAvailableMediaStreams());
      Log.l(nameof (VideoStreamSource), "ReportOpenMediaCompleted called from OpenMediaAsync");
    }

    protected override void SeekAsync(long seekToTime)
    {
      Log.l(nameof (VideoStreamSource), string.Format("MediaElement requested seeking to {0}s", (object) ((double) seekToTime / 10000000.0).ToString()));
      bool flag = this.AudioStreamExists();
      if (this.videoStream == null || this.videoSamples == null)
        Log.l(nameof (VideoStreamSource), "Ignoring seeking since the video stream was null");
      else if (flag && (this.audioStream == null || this.audioSamples == null))
      {
        Log.l(nameof (VideoStreamSource), "Ignoring seeking since the audio stream was null");
      }
      else
      {
        long num1 = 0;
        int index = 0;
        int num2;
        long lastSeekableVideoTimestamp;
        if (seekToTime == 0L)
        {
          Log.l(nameof (VideoStreamSource), "Seeking to the beginning of the video");
          lastSeekableVideoTimestamp = 0L;
          num2 = 0;
          num1 = 0L;
          index = 0;
        }
        else
        {
          lastSeekableVideoTimestamp = this.seekableVideoTimes.FindLast((Predicate<long>) (time => time <= seekToTime));
          num2 = Math.Max(this.seekableVideoSamples[lastSeekableVideoTimestamp] - 2, 0);
          if (flag)
          {
            index = Math.Max(this.seekableAudioSamples.FindLastIndex((Predicate<long>) (timestamp => timestamp <= lastSeekableVideoTimestamp)), 0);
            num1 = this.seekableAudioSamples[index];
          }
        }
        Interlocked.Exchange(ref this.videoSamplesReadIndex, num2);
        if (flag)
          Interlocked.Exchange(ref this.audioSamplesReadIndex, index);
        Interlocked.Exchange(ref this.seekTargetTime, seekToTime);
        if (this.GetTotalBufferedTime() < seekToTime)
        {
          this.videoStreamIsBuffering = true;
          if (flag)
            this.audioStreamIsBuffering = true;
          this.UpdateBufferTargetTime(new long?(seekToTime + 30000000L));
        }
        Log.l(nameof (VideoStreamSource), string.Format("Seeked video to {0}s", (object) ((double) lastSeekableVideoTimestamp / 10000000.0)));
        if (flag)
          Log.l(nameof (VideoStreamSource), string.Format("Seeked audio to {0}s", (object) ((double) num1 / 10000000.0)));
        this.ReportSeekCompleted(seekToTime);
      }
    }

    protected override void GetSampleAsync(MediaStreamType mediaStreamType)
    {
      switch (mediaStreamType)
      {
        case MediaStreamType.Audio:
          if (this.readAudioSamplesThread != null)
            ++this.readAudioSamplesThread.readAfterRequestsFromMediaElement;
          this.readAudioSamplesThread?.RequestSample();
          break;
        case MediaStreamType.Video:
          if (this.readVideoSamplesThread != null)
            ++this.readVideoSamplesThread.readAfterRequestsFromMediaElement;
          this.readVideoSamplesThread?.RequestSample();
          break;
        default:
          Assert.IsTrue(false, "VideoStreamSource does not support streaming scripts");
          break;
      }
    }

    protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
    {
      Log.l(nameof (VideoStreamSource), "SwitchMediaStreamAsync called - doing nothing");
    }

    protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
    {
      throw new NotImplementedException();
    }

    protected override void CloseMedia()
    {
      Log.d(nameof (VideoStreamSource), "MediaElement called CloseMedia");
      this.Dispose();
    }

    private void ReadStream(
      ref Stream stream,
      ref List<MediaStreamSample> samples,
      MediaStreamDescription streamDescription,
      ref object streamLock,
      ref int samplesReadIndex,
      ref bool isBuffering,
      ref VideoStreamSource.ReadSamplesThread readSamplesThread)
    {
      MediaStreamSample mediaStreamSample1 = (MediaStreamSample) null;
      double? nullable1 = new double?();
      int num1 = -1;
      int num2 = -1;
      lock (streamLock)
      {
        if (stream == null || samples == null || readSamplesThread == null)
        {
          Log.l(nameof (VideoStreamSource), "Attempted to read the stream but it was null - the MediaElement callbacks will not be called");
          return;
        }
        long position = stream.Position;
        int num3 = samplesReadIndex;
        string str = stream == this.videoStream ? "Video" : "Audio";
        bool flag1 = this.IsInBufferingState();
        if (!this.sendStartTime.HasValue)
          this.sendStartTime = new DateTime?(DateTime.Now);
        if (isBuffering && !this.allBytesReceived)
        {
          nullable1 = new double?(Math.Max(0.0, this.GetBufferingProgress()));
          double? nullable2 = nullable1;
          double num4 = 1.0;
          if ((nullable2.GetValueOrDefault() < num4 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
          {
            this.BufferingCallbacks?.BufferingProgressChanged(nullable1.Value);
            isBuffering = true;
            return;
          }
          nullable1 = new double?();
        }
        Assert.IsTrue(samples != null);
        num2 = samples.Count;
        if (num3 >= samples.Count)
        {
          if (this.allBytesReceived)
          {
            Assert.IsTrue(readSamplesThread != null);
            readSamplesThread?.CancelBufferingUpdates();
            mediaStreamSample1 = this.CreateEndOfStreamSample(streamDescription);
            isBuffering = false;
            Log.l(nameof (VideoStreamSource), string.Format("{0} End of stream sample sent", (object) str));
            Log.d(nameof (VideoStreamSource), string.Format("{0} Took {1}s for MediaElement to accept all the data", (object) str, (object) DateTime.Now.Subtract(this.sendStartTime.GetValueOrDefault()).TotalSeconds));
          }
          else
          {
            nullable1 = new double?(0.0);
            isBuffering = true;
            this.UpdateBufferTargetTime();
          }
        }
        else
        {
          Assert.IsTrue(samples != null);
          MediaStreamSample mediaStreamSample2 = samples[num3];
          num1 = num3;
          MemoryStream memoryStream = new MemoryStream((int) mediaStreamSample2.Count);
          memoryStream.SetLength(mediaStreamSample2.Count);
          stream.Position = mediaStreamSample2.Offset;
          stream.Read(memoryStream.GetBuffer(), 0, (int) mediaStreamSample2.Count);
          mediaStreamSample1 = new MediaStreamSample(mediaStreamSample2.MediaStreamDescription, (Stream) memoryStream, 0L, (long) (int) memoryStream.Length, mediaStreamSample2.Timestamp, mediaStreamSample2.Attributes);
          Assert.IsTrue(readSamplesThread != null);
          readSamplesThread?.CancelBufferingUpdates();
          Interlocked.CompareExchange(ref samplesReadIndex, num3 + 1, num3);
          isBuffering = false;
        }
        bool flag2 = this.IsInBufferingState();
        if (!flag1 & flag2)
          this.BufferingCallbacks?.StartedBuffering();
        else if (flag1 && !flag2)
          this.BufferingCallbacks?.FinishedBuffering();
        Assert.IsTrue(stream != null);
        if (stream != null)
          stream.Position = position;
      }
      Assert.IsTrue(mediaStreamSample1 == null || !nullable1.HasValue, "Cannot report both a sample AND progress");
      Assert.IsTrue(mediaStreamSample1 != null || nullable1.HasValue, "Must report either a sample or the buffering progress to the MediaElement");
      string context1 = (stream == this.videoStream ? (object) "Video" : (object) "Audio").ToString() + " req: " + (object) readSamplesThread.readAfterRequestsFromMediaElement;
      this.ExtraLoggingCountCollection(readSamplesThread);
      try
      {
        if (mediaStreamSample1 != null)
        {
          context1 += " completed";
          ++readSamplesThread.sampleSuppliedCount;
          this.ReportGetSampleCompleted(mediaStreamSample1);
          mediaStreamSample1.Stream.SafeDispose();
        }
        else
        {
          if (!nullable1.HasValue)
            return;
          context1 += " progress";
          ++readSamplesThread.progressSuppliedCount;
          this.ReportGetSampleProgress(nullable1.Value);
        }
      }
      catch (ArgumentException ex)
      {
        string str = stream == this.videoStream ? "Video" : "Audio";
        this.Extralogging(context1, mediaStreamSample1, readSamplesThread);
        string context2 = string.Format("{0} - Reporting a sample ({1}/{2}): timestamp {3}, duration {4}, offset {5}, length {6}", (object) str, (object) num1, (object) num2, (object) mediaStreamSample1.Timestamp, (object) mediaStreamSample1.Duration, (object) mediaStreamSample1.Offset, (object) mediaStreamSample1.Count);
        Log.LogException((Exception) ex, context2);
        throw;
      }
      catch (NullReferenceException ex)
      {
        this.Extralogging(context1, mediaStreamSample1, readSamplesThread);
        Log.l(nameof (VideoStreamSource), "Attempted to send a sample when none was asked for");
        Log.l(nameof (VideoStreamSource), string.Format("Reporting a sample ({0}/{1}): timestamp {2}, duration {3}, offset {4}, length {5}", (object) num1, (object) num2, (object) mediaStreamSample1.Timestamp, (object) mediaStreamSample1.Duration, (object) mediaStreamSample1.Offset, (object) mediaStreamSample1.Count));
      }
    }

    private void ExtraLoggingCountCollection(VideoStreamSource.ReadSamplesThread readThread)
    {
      if (readThread == null)
        return;
      readThread.prevCounts[0] = readThread.requestCount;
      readThread.prevCounts[1] = readThread.requestActionedCount;
      readThread.prevCounts[2] = readThread.progressSuppliedCount;
      readThread.prevCounts[3] = readThread.sampleSuppliedCount;
      readThread.prevCounts[4] = readThread.readAfterBuffering;
      readThread.prevCounts[5] = readThread.readAfterBufferingProgress;
      readThread.prevCounts[6] = readThread.readAfterEof;
      readThread.prevCounts[7] = readThread.readAfterRequestsFromMediaElement;
    }

    private void Extralogging(
      string context,
      MediaStreamSample sample,
      VideoStreamSource.ReadSamplesThread readThread)
    {
      string str1 = "?";
      string str2 = "?";
      try
      {
        if (sample != null)
          str2 = string.Format("Sample time stamp {0}", (object) sample.Timestamp);
        if (sample.Stream != null)
        {
          sample.Stream.Position = 0L;
          long count = Math.Min(sample.Stream.Length, 8L);
          byte[] buffer = new byte[count];
          sample.Stream.Read(buffer, 0, (int) count);
          str1 = BitConverter.ToString(buffer);
          sample.Stream.Position = 0L;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception getting bytes");
      }
      Log.l(nameof (VideoStreamSource), string.Format("{0}, {1}, {2}", (object) context, (object) str2, (object) str1));
      if (readThread == null)
        return;
      Log.l(nameof (VideoStreamSource), "Counts: {0}, {1}, {2}, {3}", (object) readThread.requestCount, (object) readThread.requestActionedCount, (object) readThread.progressSuppliedCount, (object) readThread.sampleSuppliedCount);
      Log.l(nameof (VideoStreamSource), "Prev Counts: {0}, {1}, {2}, {3}", (object) readThread.prevCounts[0], (object) readThread.prevCounts[1], (object) readThread.prevCounts[2], (object) readThread.prevCounts[3]);
      Log.l(nameof (VideoStreamSource), "Read Counts: {0}, {1}, {2}, {3}", (object) readThread.readAfterBuffering, (object) readThread.readAfterBufferingProgress, (object) readThread.readAfterEof, (object) readThread.readAfterRequestsFromMediaElement);
      Log.l(nameof (VideoStreamSource), "Prev Read Counts: {0}, {1}, {2}, {3}", (object) readThread.prevCounts[4], (object) readThread.prevCounts[5], (object) readThread.prevCounts[6], (object) readThread.prevCounts[7]);
    }

    private long LastTimestamp(List<MediaStreamSample> samples)
    {
      if (samples == null || samples.Count == 0)
        return 0;
      MediaStreamSample sample = samples[samples.Count - 1];
      return sample == null ? 0L : sample.Timestamp;
    }

    private double GetBufferingProgress()
    {
      long num1 = Interlocked.Read(ref this.bufferStartTime);
      long num2 = Interlocked.Read(ref this.bufferTargetTime);
      return 1.0 - (double) (num2 - this.GetTotalBufferedTime()) / (double) (num2 - num1);
    }

    private bool IsInBufferingState()
    {
      if (this.videoStreamIsBuffering)
        return true;
      return this.AudioStreamExists() && this.audioStreamIsBuffering;
    }

    private void UpdateBufferTargetTime(long? targetTime = null)
    {
      long val2 = (long) ((double) this.duration - 2.0 / ((double) this.framesPerSecond > 0.0 ? (double) this.framesPerSecond : 5.0) * 10000000.0);
      long totalBufferedTime = this.GetTotalBufferedTime();
      long num = Math.Min(targetTime ?? totalBufferedTime + 30000000L, val2);
      Interlocked.Exchange(ref this.bufferStartTime, totalBufferedTime);
      Interlocked.Exchange(ref this.bufferTargetTime, num);
      Log.l(nameof (VideoStreamSource), string.Format("Stream starting to buffer to {0}s (max {1}s)", (object) ((double) num / 10000000.0), (object) ((double) val2 / 10000000.0)));
    }

    private void ReadVideoStream()
    {
      this.ReadStream(ref this.videoStream, ref this.videoSamples, this.videoStreamDescription, ref this.videoLock, ref this.videoSamplesReadIndex, ref this.videoStreamIsBuffering, ref this.readVideoSamplesThread);
    }

    private void ReadAudioStream()
    {
      this.ReadStream(ref this.audioStream, ref this.audioSamples, this.audioStreamDescription, ref this.audioLock, ref this.audioSamplesReadIndex, ref this.audioStreamIsBuffering, ref this.readAudioSamplesThread);
    }

    private void StripADTSHeaders(ulong timestamp)
    {
      if (this.audioStream == null || this.audioSamples == null)
        return;
      long position = this.audioStream.Position;
      if (this.audioBytesUntilNextFrame != 0L && this.audioStream.Length - this.audioStreamParseOffset > this.audioBytesUntilNextFrame)
      {
        this.audioStreamParseOffset += this.audioBytesUntilNextFrame;
        this.audioBytesUntilNextFrame = 0L;
        this.audioSamples.Add(new MediaStreamSample(this.audioStreamDescription, this.audioStream, this.audioStreamLastAACFrameStartOffset, this.audioStreamParseOffset - this.audioStreamLastAACFrameStartOffset, this.audioStreamLastAACFrameTimestamp, (IDictionary<MediaSampleAttributeKeys, string>) this.sampleAttributes));
        this.seekableAudioSamples.Add(this.audioStreamLastAACFrameTimestamp);
      }
      while (this.audioBytesUntilNextFrame == 0L)
      {
        if (this.audioStream.Length - this.audioStreamParseOffset < 6L)
        {
          this.audioStream.Position = position;
          return;
        }
        this.audioStream.Position = this.audioStreamParseOffset;
        byte[] buffer = new byte[4];
        this.audioStream.Read(buffer, 2, 2);
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) buffer);
        uint num1 = BitConverter.ToUInt32(buffer, 0) & (uint) ushort.MaxValue;
        if (((int) (num1 >> 4) & 4095) != 4095)
        {
          Log.l(nameof (VideoStreamSource), "ADTS header malformed - it must begin with 0xfff. Exiting.");
          this.audioStream.Position = position;
          return;
        }
        bool flag = false;
        if (((int) num1 & 1) == 0)
        {
          Log.l(nameof (VideoStreamSource), "CRC bit set to 0 - CRC protection bytes are appended at the end");
          flag = true;
        }
        this.audioStream.Read(buffer, 0, 4);
        if (BitConverter.IsLittleEndian)
          Array.Reverse((Array) buffer);
        uint num2 = BitConverter.ToUInt32(buffer, 0) >> 5 & 8191U;
        this.audioStream.Position += flag ? 3L : 1L;
        this.audioBytesUntilNextFrame = (long) num2 - (flag ? 9L : 7L);
        this.audioStreamParseOffset = this.audioStream.Position;
        this.audioStreamLastAACFrameStartOffset = this.audioStream.Position;
        this.audioStreamLastAACFrameTimestamp = (long) timestamp;
        if (this.audioBytesUntilNextFrame > this.audioStream.Length - this.audioStream.Position)
        {
          this.audioSamples.Add(new MediaStreamSample(this.audioStreamDescription, this.audioStream, this.audioStreamLastAACFrameStartOffset, this.audioBytesUntilNextFrame, this.audioStreamLastAACFrameTimestamp, (IDictionary<MediaSampleAttributeKeys, string>) this.sampleAttributes));
          this.audioStream.Position += this.audioBytesUntilNextFrame;
          this.audioStreamParseOffset = this.audioStream.Position;
          this.audioBytesUntilNextFrame = 0L;
          this.seekableAudioSamples.Add(this.audioStreamLastAACFrameTimestamp);
        }
      }
      this.audioStream.Position = position;
    }

    private MediaStreamSample CreateEndOfStreamSample(MediaStreamDescription mediaStreamDescription)
    {
      return new MediaStreamSample(mediaStreamDescription, (Stream) null, 0L, 0L, 0L, (IDictionary<MediaSampleAttributeKeys, string>) new Dictionary<MediaSampleAttributeKeys, string>());
    }

    public int GetRotationAngle() => this.rotationAngle % 360 + (this.rotationAngle < 0 ? 360 : 0);

    public void ExitBufferingState()
    {
      Log.l(nameof (VideoStreamSource), string.Format("Exiting buffering state - {0}", this.IsInBufferingState() ? (object) "was buffering" : (object) "was not buffering"));
      Interlocked.Exchange(ref this.bufferStartTime, 0L);
      Interlocked.Exchange(ref this.bufferTargetTime, 0L);
    }

    public long GetTotalBufferedTime()
    {
      if (this.videoSamples == null)
        return 0;
      return !this.AudioStreamExists() || this.audioSamples == null ? this.LastTimestamp(this.videoSamples) : Math.Min(this.LastTimestamp(this.videoSamples), this.LastTimestamp(this.audioSamples));
    }

    public long GetVideoPosition()
    {
      if (this.videoSamples == null || this.videoSamples.Count == 0 || this.videoSamplesReadIndex >= this.videoSamples.Count || this.videoSamplesReadIndex < 0)
        return 0;
      MediaStreamSample videoSample = this.videoSamples[this.videoSamplesReadIndex];
      return Math.Max(videoSample != null ? videoSample.Timestamp : 0L, this.seekTargetTime);
    }

    public void Dispose()
    {
      Log.d(nameof (VideoStreamSource), "Disposing VideoStreamSource");
      Stream d1 = (Stream) null;
      Stream d2 = (Stream) null;
      VideoStreamSource.ReadSamplesThread d3 = (VideoStreamSource.ReadSamplesThread) null;
      VideoStreamSource.ReadSamplesThread d4 = (VideoStreamSource.ReadSamplesThread) null;
      lock (this.videoLock)
      {
        d1 = this.videoStream;
        d3 = this.readVideoSamplesThread;
        this.videoSamples = (List<MediaStreamSample>) null;
        this.videoStream = (Stream) null;
        this.readVideoSamplesThread = (VideoStreamSource.ReadSamplesThread) null;
      }
      Log.d(nameof (VideoStreamSource), "Disposed the video stream");
      lock (this.audioLock)
      {
        d2 = this.audioStream;
        d4 = this.readAudioSamplesThread;
        this.audioSamples = (List<MediaStreamSample>) null;
        this.audioStream = (Stream) null;
        this.readAudioSamplesThread = (VideoStreamSource.ReadSamplesThread) null;
      }
      Log.d(nameof (VideoStreamSource), "Disposed the audio stream");
      d1.SafeDispose();
      d2.SafeDispose();
      d3.SafeDispose();
      d4.SafeDispose();
      this.bufferTimer.SafeDispose();
      this.bufferTimer = (System.Threading.Timer) null;
    }

    private class ReadSamplesThread : IDisposable
    {
      private long numberOfThreads;
      private Thread thread;
      private ManualResetEventSlim shouldReadSampleEvent = new ManualResetEventSlim(false);
      private Action readStream;
      private string threadLogHeader;
      public long requestCount;
      public long cancelCount;
      public long requestActionedCount;
      public long sampleSuppliedCount;
      public long progressSuppliedCount;
      public long readAfterRequestsFromMediaElement;
      public long readAfterBufferingProgress;
      public long readAfterEof;
      public long readAfterBuffering;
      public long[] prevCounts = new long[8];

      public ReadSamplesThread(string streamType, Action readStream)
      {
        this.thread = new Thread(new ThreadStart(this.RunLoop));
        this.threadLogHeader = string.Format("ReadSamplesThread {0}", (object) streamType);
        this.readStream = readStream;
      }

      public void Start()
      {
        Log.d(this.threadLogHeader, "Starting ReadSamplesThread");
        Assert.IsTrue(Interlocked.Exchange(ref this.numberOfThreads, 1L) == 0L, "You should not be starting the thread multiple times");
        Assert.IsTrue(this.thread != null, "The ReadSamples thread should not have been disposed before starting");
        this.thread?.Start();
      }

      public void RequestSample()
      {
        ++this.requestCount;
        this.shouldReadSampleEvent.Set();
      }

      public void CancelBufferingUpdates()
      {
        ++this.cancelCount;
        this.shouldReadSampleEvent.Reset();
      }

      public void Dispose()
      {
        Log.d(this.threadLogHeader, "Disposing ReadSamplesThread");
        Interlocked.Exchange(ref this.numberOfThreads, 0L);
        this.thread = (Thread) null;
        this.readStream = (Action) null;
        this.shouldReadSampleEvent.Set();
      }

      private void RunLoop()
      {
        Log.d(this.threadLogHeader, "Starting loop");
        while (true)
        {
          Action readStream;
          do
          {
            this.shouldReadSampleEvent.WaitOne();
            this.shouldReadSampleEvent.Reset();
            if (Interlocked.Read(ref this.numberOfThreads) <= 0L)
            {
              Log.d(this.threadLogHeader, "Exiting loop");
              return;
            }
            ++this.requestActionedCount;
            readStream = this.readStream;
          }
          while (readStream == null);
          readStream();
        }
      }
    }
  }
}
