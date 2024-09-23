// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.StreamingMp4Demux
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Threading;
using WhatsAppNative;


namespace WhatsApp.Streaming
{
  public class StreamingMp4Demux : IDisposable
  {
    private IStreamingFileSource srcObject;
    private Stream src;
    private IStreamingMp4DemuxCallbacks callbacks;
    private Action<Action> sampleSyncAction = (Action<Action>) (a => a());

    public StreamingMp4Demux(IStreamingFileSource src, IStreamingMp4DemuxCallbacks callbacks)
    {
      this.src = (Stream) new StreamingMp4Demux.InputStreamWrapper(this.srcObject = src);
      this.callbacks = callbacks;
    }

    public void Start()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
      {
        Action checkWriteError = (Action) null;
        try
        {
          IMp4Utils mp4Utils = NativeInterfaces.Mp4Utils;
          Stream src = this.src;
          if (src == null)
            this.srcObject?.CheckIoError();
          using (Mp4MappedStream mp4MappedStream = mp4Utils.MapStream(src))
          {
            MediaType mediaType = CodecDetector.DetectMp4Codecs(mp4MappedStream.Filename, false);
            this.srcObject?.CheckIoError();
            if (!this.CanStream(mediaType.VideoStreamType) || !this.CanStream(mediaType.AudioStreamType))
            {
              this.OnError((Exception) new Mp4CannotStreamException(string.Format("Cannot stream privded codecs\nVideo = {0}\nAudio = {1}", (object) mediaType.VideoStreamType.ToString(), (object) mediaType.AudioStreamType.ToString())));
            }
            else
            {
              src.Position = 0L;
              float fps = -1f;
              using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(src, disposeStream: false))
              {
                FRAME_ATTRIBUTES frameInfo = videoFrameGrabber.FrameInfo;
                if (frameInfo.FrameRatePeriod != 0U)
                  fps = (0.0f + (float) frameInfo.FrameRate) / (float) frameInfo.FrameRatePeriod;
              }
              src.Position = 0L;
              Action<Action> action = (Action<Action>) (a => checkWriteError = a);
              mp4Utils.ExtractAVStreamsForStreaming(mp4MappedStream.Filename, fps, (IMp4UtilsMetadataReceiver) this.callbacks, (IMp4UtilsWriteCallback) new StreamingMp4Demux.OutputCallbackWrapper()
              {
                OnBytes = this.Sync(new StreamingMp4Demux.SampleCallback(this.callbacks.OnVideoBytes)),
                Throw = action
              }, (IMp4UtilsWriteCallback) new StreamingMp4Demux.OutputCallbackWrapper()
              {
                OnBytes = this.Sync(new StreamingMp4Demux.SampleCallback(this.callbacks.OnAudioBytes)),
                Throw = action
              });
              this.callbacks.OnEndOfFile();
            }
          }
        }
        catch (Exception ex1)
        {
          Exception ex2 = ex1;
          try
          {
            this.srcObject?.CheckIoError();
            Action action = checkWriteError;
            if (action != null)
              action();
          }
          catch (Exception ex3)
          {
            ex2 = ex3;
          }
          this.OnError(ex2);
        }
      }));
    }

    private void OnError(Exception ex)
    {
      if (this.src == null && ex is OperationCanceledException)
        return;
      if (ex is SidecarVerifierException)
        ex = (Exception) new Mp4CannotStreamException("Sidecar verifier failed", ex);
      this.callbacks.OnError(ex);
    }

    private bool CanStream(Mp4VideoStreamType type) => type == Mp4VideoStreamType.H264;

    private bool CanStream(Mp4AudioStreamType type)
    {
      return type == Mp4AudioStreamType.NotFound || type == Mp4AudioStreamType.Aac;
    }

    private StreamingMp4Demux.SampleCallback Sync(StreamingMp4Demux.SampleCallback input)
    {
      return (StreamingMp4Demux.SampleCallback) ((buf, offset, len, seek, ts) => this.sampleSyncAction((Action) (() => input(buf, offset, len, seek, ts))));
    }

    public void Dispose()
    {
      Interlocked.Exchange<Stream>(ref this.src, (Stream) null).SafeDispose();
      Interlocked.Exchange<Action<Action>>(ref this.sampleSyncAction, (Action<Action>) (a =>
      {
        throw new OperationCanceledException();
      }));
    }

    public delegate void SampleCallback(
      byte[] buffer,
      int offset,
      int length,
      bool isSeekPoint,
      ulong timestamp);

    private class InputStreamWrapper : Stream
    {
      private IStreamingFileSource src;
      private long pos;

      public InputStreamWrapper(IStreamingFileSource src) => this.src = src;

      protected override void Dispose(bool disposing)
      {
        this.src.SafeDispose();
        base.Dispose(disposing);
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int num = this.src.Read(buffer, offset, count);
        if (num > 0)
          this.pos += (long) num;
        return num;
      }

      public override long Length => this.src.GetFullFileSize();

      public override long Position
      {
        get => this.pos;
        set
        {
          if (value < 0L || value > this.Length)
            throw new InvalidOperationException();
          this.src.Seek(value);
          this.pos = value;
        }
      }

      public override long Seek(long pos, SeekOrigin origin)
      {
        switch (origin)
        {
          case SeekOrigin.Begin:
            this.Position = pos;
            break;
          case SeekOrigin.Current:
            this.Position += pos;
            break;
          case SeekOrigin.End:
            this.Position = this.Length + pos;
            break;
          default:
            throw new InvalidOperationException();
        }
        return this.Position;
      }

      public override bool CanSeek => true;

      public override bool CanRead => true;

      public override bool CanWrite => false;

      public override void SetLength(long value) => throw new UnauthorizedAccessException();

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new UnauthorizedAccessException();
      }

      public override void Flush()
      {
      }
    }

    private class OutputCallbackWrapper : IMp4UtilsWriteCallback
    {
      public StreamingMp4Demux.SampleCallback OnBytes;
      public Action<Action> Throw;

      public void Write(byte[] buffer, bool seekPoint, ulong ts)
      {
        try
        {
          StreamingMp4Demux.SampleCallback onBytes = this.OnBytes;
          if (onBytes == null)
            return;
          onBytes(buffer, 0, buffer.Length, seekPoint, ts);
        }
        catch (Exception ex)
        {
          Action<Action> action = this.Throw;
          if (action != null)
            action(ex.GetRethrowAction());
          throw;
        }
      }
    }
  }
}
