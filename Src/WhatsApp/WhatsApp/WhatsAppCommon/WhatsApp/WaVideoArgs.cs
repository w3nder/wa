// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaVideoArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class WaVideoArgs : EventArgs
  {
    public string FileExtension;
    public string ContentType;
    public Stream Stream;
    private string previewPlayPath;
    public MessageProperties.MediaProperties.Attribution GifAttribution;
    public string FullPath;
    public int Duration;
    public WriteableBitmap Thumbnail;
    public WriteableBitmap LargeThumbnail;
    public string Caption;
    public int OrientationAngle;
    public bool IsCameraVideo;
    public WhatsApp.CropRectangle? CropRectangle;
    public Triad<System.Windows.Point, Size, int> AbsoluteCropData;
    public bool LoopingPlayback;
    public bool C2cStarted;
    private CodecInfo? codecInfo;
    private WhatsApp.TimeCrop? timeCrop;
    public int? VideoMaxBitrate;
    private TranscodeReason? transcodeReason;

    public string PreviewPlayPath
    {
      get
      {
        if (this.previewPlayPath != null)
          return this.previewPlayPath;
        return this.FullPath == null ? (string) null : NativeMediaStorage.MakeUri(this.FullPath);
      }
      set
      {
        if (this.FullPath == null || this.previewPlayPath != NativeMediaStorage.MakeUri(this.FullPath))
          this.previewPlayPath = value;
        else
          this.previewPlayPath = (string) null;
      }
    }

    public Message QuotedMessage { get; set; }

    public string QuotedChat { get; set; }

    private long FileSize
    {
      get
      {
        if (this.Stream != null)
          return this.Stream.Length;
        string previewPlayPath = this.PreviewPlayPath;
        using (IMediaStorage mediaStorage = MediaStorage.Create(previewPlayPath))
        {
          using (Stream stream = mediaStorage.OpenFile(previewPlayPath))
            return stream.Length;
        }
      }
    }

    public CodecInfo CodecInfo
    {
      get
      {
        if (this.codecInfo.HasValue)
          return this.codecInfo.Value;
        CodecInfo codecInfo = CodecDetector.GetCodecInfo(this.PreviewPlayPath);
        this.codecInfo = new CodecInfo?(codecInfo);
        return codecInfo;
      }
      set => this.codecInfo = new CodecInfo?(value);
    }

    public bool ShouldTranscode => this.TranscodeReason != 0;

    public WhatsApp.TimeCrop? TimeCrop
    {
      get => this.timeCrop;
      set
      {
        this.timeCrop = value;
        this.transcodeReason = new TranscodeReason?();
      }
    }

    public TranscodeReason TranscodeReason
    {
      get
      {
        if (this.transcodeReason.HasValue)
          return this.transcodeReason.Value;
        TranscodeReason transcodeReason = TranscodeReason.None;
        if (this.CodecInfo == CodecInfo.NeedsTranscode)
          transcodeReason |= TranscodeReason.BadCodec;
        if (this.CodecInfo == CodecInfo.NeedsRemux)
          transcodeReason |= TranscodeReason.BadContainer;
        if (this.TimeCrop.HasValue)
          transcodeReason |= TranscodeReason.TimeCrop;
        if (this.CropRectangle.HasValue)
          transcodeReason |= TranscodeReason.FrameCrop;
        if (this.FileSize > (long) Settings.MaxMediaSize)
          transcodeReason |= TranscodeReason.FileSize;
        if (this.GifAttribution <= MessageProperties.MediaProperties.Attribution.NONE)
        {
          try
          {
            using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(this.PreviewPlayPath))
            {
              if ((double) (this.FileSize * 8L) / videoFrameGrabber.DurationSeconds / 1000.0 > (double) (this.VideoMaxBitrate ?? Settings.VideoMaxBitrate))
                transcodeReason |= TranscodeReason.MaxBitrate;
            }
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "frame decoder");
          }
        }
        this.transcodeReason = new TranscodeReason?(transcodeReason);
        return transcodeReason;
      }
      set => this.transcodeReason = new TranscodeReason?(value);
    }

    public void CleanUp()
    {
      try
      {
        if (this.previewPlayPath != null)
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            storeForApplication.DeleteFile(this.previewPlayPath);
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "failed to delete video preview file");
      }
      this.previewPlayPath = (string) null;
    }
  }
}
