// Decompiled with JetBrains decompiler
// Type: WhatsApp.UtilsFrontend.VideoPlayback
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WhatsApp.CommonOps;
using WhatsApp.Events;
using WhatsApp.Pages;
using WhatsApp.Streaming;


namespace WhatsApp.UtilsFrontend
{
  public class VideoPlayback : IDisposable
  {
    private const string LogHeader = "VideoPlayback";
    private const int maxThumbnailSize = 800;
    private TimeSpan streamSourceSetRetryTime = TimeSpan.FromMilliseconds(500.0);
    private Message message;
    private VideoPlayback.VideoContainerType containerType;
    private VideoPlay videoPlayerEvent;
    private MediaElement mediaElement;
    private IVideoStreamSourceBufferingCallbacks videoStreamSourceBufferingCallbacks;
    private IVideoStreamSourceErrorCallbacks videoStreamSourceErrorCallbacks;
    private MessageViewModel messageViewModel;
    private IDisposable messageSubscriber;
    private VideoStreamSource videoStreamSource;
    private StreamingMp4Demux streamingMp4Demux;
    private IStreamingFileSource streamingFileSource;
    private int streamSourceSetAttempt;
    public IVideoPlaybackDownloadCallbacks downloadCallbacks;

    public VideoPlayback(Message message)
    {
      Assert.IsTrue(message != null, "Cannot play a video from a null message");
      this.message = message;
    }

    public void LaunchVideoPlayer(VideoPlay fsEvent, VideoOpenMode videoOpenMode = VideoOpenMode.Default)
    {
      Log.d(nameof (VideoPlayback), "Launching video player");
      Assert.IsTrue(fsEvent != null, "The video player event must be passed to open the video player");
      this.containerType = VideoPlayback.VideoContainerType.VideoPlayer;
      this.videoPlayerEvent = fsEvent;
      this.OpenVideo(videoOpenMode);
    }

    public void SetSourceOnMediaElement(
      MediaElement mediaElement,
      VideoOpenMode videoOpenMode = VideoOpenMode.Default,
      IVideoPlaybackDownloadCallbacks videoPlaybackDownloadCallbacks = null,
      IVideoStreamSourceBufferingCallbacks videoStreamSourceBufferingCallbacks = null,
      IVideoStreamSourceErrorCallbacks videoStreamSourceErrorCallbacks = null)
    {
      Log.d(nameof (VideoPlayback), "Setting the source on a MediaElement");
      Assert.IsTrue(mediaElement != null, "Cannot play a video in a null MediaElement");
      this.containerType = VideoPlayback.VideoContainerType.MediaElement;
      this.mediaElement = mediaElement;
      if (videoOpenMode != VideoOpenMode.OpenFromFile)
      {
        Assert.IsTrue(videoPlaybackDownloadCallbacks != null, "Must have download callbacks when streaming video");
        Assert.IsTrue(videoStreamSourceBufferingCallbacks != null, "Must have buffering callbacks when streaming video");
        Assert.IsTrue(videoStreamSourceErrorCallbacks != null, "Must have error callbacks when streaming video");
        this.downloadCallbacks = videoPlaybackDownloadCallbacks;
        this.videoStreamSourceBufferingCallbacks = videoStreamSourceBufferingCallbacks;
        this.videoStreamSourceErrorCallbacks = videoStreamSourceErrorCallbacks;
      }
      this.OpenVideo(videoOpenMode);
    }

    public int GetRotationAngle()
    {
      if (this.videoStreamSource != null)
        return this.videoStreamSource.GetRotationAngle();
      if (this.message.LocalFileExists())
        return VideoFrameGrabber.GetAngleForMatrix(this.message.GetVideoRotationMatrix());
      Log.l(nameof (VideoPlayback), "Could not get a rotation angle for video");
      return 0;
    }

    public BitmapSource GetBlurredThumbnail()
    {
      MemoryStream thumbnailStream = this.message.GetThumbnailStream(true, out bool _);
      if (thumbnailStream == null)
        return (BitmapSource) null;
      WriteableBitmap bitmap = BitmapUtils.CreateBitmap((Stream) thumbnailStream, 800, 800);
      return bitmap == null ? (BitmapSource) null : (BitmapSource) bitmap.Blur();
    }

    private void OpenVideo(VideoOpenMode videoOpenMode)
    {
      if ((videoOpenMode == VideoOpenMode.OpenFromFile || videoOpenMode == VideoOpenMode.StreamFromFile) && !Assert.IsTrue(this.message.LocalFileExists(), "The video file must exist if opening the video from a file"))
        return;
      switch (videoOpenMode)
      {
        case VideoOpenMode.Default:
          this.OpenVideoFromFileOrStreamOrDownload(this.message);
          break;
        case VideoOpenMode.OpenFromFile:
          this.OpenVideoFromFile(this.message.LocalFileUri, VideoPlayback.CreateKey(this.message));
          break;
        case VideoOpenMode.StreamFromFile:
          this.OpenVideoStreamingFromFile(this.message.LocalFileUri, VideoPlayback.CreateKey(this.message));
          break;
        case VideoOpenMode.StreamFromNetwork:
          this.OpenVideoStreamingFromNetwork(this.message);
          break;
        default:
          Assert.Failed("Added a VideoOpenMode and forgot to add the case in OpenVideo");
          break;
      }
    }

    private void OpenVideoFromFileOrStreamOrDownload(Message msg)
    {
      if (msg.LocalFileExists())
      {
        Log.d(nameof (VideoPlayback), "Opening video from file since it has already been downloaded");
        this.OpenVideoFromFile(msg.LocalFileUri, VideoPlayback.CreateKey(msg));
      }
      else
      {
        Log.d(nameof (VideoPlayback), "Streaming video as it has not yet been downloaded");
        this.OpenVideoStreamingFromNetwork(msg);
      }
    }

    private void OpenVideoFromFile(string localFileURL, FunXMPP.FMessage.Key associatedMsgKey)
    {
      Log.l(nameof (VideoPlayback), string.Format("Opening video from a file (non-streaming): {0}", (object) localFileURL));
      if (this.containerType == VideoPlayback.VideoContainerType.VideoPlayer)
      {
        VideoPlayerPage.Start(this, this.videoPlayerEvent, localFileURL, associatedMsgKey);
      }
      else
      {
        if (this.containerType != VideoPlayback.VideoContainerType.MediaElement || !Assert.IsTrue(this.mediaElement != null, "Must have a MediaElement reference if opening the video on a MediaElement"))
          return;
        this.mediaElement.Source = new Uri(MediaStorage.GetAbsolutePath(localFileURL));
      }
    }

    private void OpenVideoStreamingFromFile(
      string localFileURL,
      FunXMPP.FMessage.Key associatedMsgKey)
    {
      Log.l(nameof (VideoPlayback), "Opening video streaming from a file");
      this.OpenStreamingVideo((IStreamingFileSource) new StreamingFileSource(localFileURL), associatedMsgKey);
    }

    private void OpenVideoStreamingFromNetwork(Message msg)
    {
      Log.l(nameof (VideoPlayback), "Opening video streaming from network");
      StreamingDownload streamingDownload = new StreamingDownload(msg);
      IStreamingFileSource fileSource = streamingDownload.FileSource;
      streamingDownload.Start();
      this.OpenStreamingVideo(fileSource, VideoPlayback.CreateKey(msg));
    }

    private void OpenStreamingVideo(
      IStreamingFileSource streamingFileSource,
      FunXMPP.FMessage.Key associatedMsgKey)
    {
      this.streamingFileSource = streamingFileSource;
      this.videoStreamSource = new VideoStreamSource(new Func<string>(this.GetErrorMessage));
      this.streamingMp4Demux = new StreamingMp4Demux(streamingFileSource, (IStreamingMp4DemuxCallbacks) this.videoStreamSource);
      this.streamingMp4Demux.Start();
      if (this.containerType == VideoPlayback.VideoContainerType.VideoPlayer)
      {
        Log.l(nameof (VideoPlayback), "Starting to stream in the video player");
        VideoPlayerPage.Start(this, this.videoPlayerEvent, this.videoStreamSource, associatedMsgKey);
      }
      else
      {
        Log.l(nameof (VideoPlayback), "Starting to stream in the MediaElement");
        if (!Assert.IsTrue(this.mediaElement != null, "Must have a MediaElement reference to set a source on the MediaElement") || !Assert.IsTrue(this.videoStreamSourceBufferingCallbacks != null, "Cannot set a null buffering callback on a VideoStreamSource") || !Assert.IsTrue(this.videoStreamSourceErrorCallbacks != null, "Cannot set a null error callback on a VideoStreamSource"))
          return;
        this.videoStreamSource.BufferingCallbacks = this.videoStreamSourceBufferingCallbacks;
        this.videoStreamSource.ErrorCallbacks = this.videoStreamSourceErrorCallbacks;
        this.SetVideoStreamSourceOnMediaElementUntilSuccessful();
      }
    }

    private void SetVideoStreamSourceOnMediaElementUntilSuccessful()
    {
      if (this.mediaElement == null || this.videoStreamSource == null)
        Log.d(nameof (VideoPlayback), "MediaElement or VideoStreamSource was null while attempting to set the streaming source");
      else if (this.videoStreamSource.DidRequestOpenMedia)
      {
        Log.d(nameof (VideoPlayback), "VideoStreamSource recognized that opening the video was requested");
      }
      else
      {
        ++this.streamSourceSetAttempt;
        Log.d(nameof (VideoPlayback), string.Format("Setting stream source in the MediaElement (attempt #{0}) (opacity {1}) (visibility {2})", (object) this.streamSourceSetAttempt, (object) this.mediaElement.Opacity, (object) this.mediaElement.Visibility));
        Dispatcher dispatcher = this.mediaElement.Dispatcher;
        dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.mediaElement == null || this.videoStreamSource == null)
          {
            Log.d(nameof (VideoPlayback), "MediaElement or VideoStreamSource was null while attempting to set the streaming source - returning");
          }
          else
          {
            this.mediaElement.SetSource((MediaStreamSource) this.videoStreamSource);
            dispatcher.RunAfterDelay(this.streamSourceSetRetryTime, new Action(this.SetVideoStreamSourceOnMediaElementUntilSuccessful));
          }
        }));
      }
    }

    private string GetErrorMessage() => this.message.GetErrorString();

    public void SwitchToDownload(Dispatcher dispatcher)
    {
      Log.l(nameof (VideoPlayback), "Switching from streaming to downloading");
      this.DisposeStreamingInstances();
      if (this.message.LocalFileExists())
      {
        Log.l(nameof (VideoPlayback), "The file we're trying to download already existed");
        this.SwitchVideoPlayerToDownloadedFile();
      }
      else
      {
        Log.l(nameof (VideoPlayback), "Starting to download the file");
        this.message.NotifyTransfer = true;
        this.messageViewModel = MessageViewModel.Create(this.message);
        this.messageViewModel.InitLazyResources();
        this.messageSubscriber = this.messageViewModel.GetObservable().ObserveOnDispatcherIfNeeded<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>(new Action<KeyValuePair<string, object>>(this.OnViewModelNotified));
        dispatcher.BeginInvoke((Action) (() => ViewMessage.Download(this.message)));
      }
    }

    private void OnViewModelNotified(KeyValuePair<string, object> p)
    {
      switch (p.Key)
      {
        case "TransferProgressChanged":
          this.OnTransferProgressChanged();
          break;
        case "LocalFileUriChanged":
          this.OnLocalFileUriChanged();
          break;
      }
    }

    private void OnTransferProgressChanged()
    {
      Log.d(nameof (VideoPlayback), string.Format("OnTransferProgressChanged: {0}", (object) this.message.TransferValue));
      this.downloadCallbacks?.UpdateVideoDownloadProgress(this.message.TransferValue);
    }

    private void OnLocalFileUriChanged()
    {
      Log.d(nameof (VideoPlayback), string.Format("OnLocalFileUriChanged: {0}", (object) this.message.LocalFileUri));
      if (!this.message.LocalFileExists())
        return;
      this.DisposeMessageSubscription();
      this.downloadCallbacks?.VideoDownloadComplete(this.message.LocalFileUri);
    }

    private void SwitchVideoPlayerToDownloadedFile()
    {
      if (!Assert.IsTrue(this.downloadCallbacks != null, "Video finished downloading but downloadCallbacks was null") || !Assert.IsTrue(this.message.LocalFileUri != null, "Video finished downloading but LocalFileUri was null"))
        return;
      this.downloadCallbacks?.VideoDownloadComplete(this.message.LocalFileUri);
    }

    public void Dispose()
    {
      Log.l(nameof (VideoPlayback), "Disposing video wrapper");
      this.DisposeMessageSubscription();
      this.DisposeStreamingInstances();
    }

    private void DisposeMessageSubscription()
    {
      Log.d(nameof (VideoPlayback), "Disposing message subscription");
      this.messageSubscriber.SafeDispose();
      this.messageSubscriber = (IDisposable) null;
      if (this.messageViewModel != null)
        this.messageViewModel.DisposeLazyResources();
      this.messageViewModel.SafeDispose();
      this.messageViewModel = (MessageViewModel) null;
    }

    private void DisposeStreamingInstances()
    {
      Log.d(nameof (VideoPlayback), "Disposing streaming instances");
      this.streamingMp4Demux.SafeDispose();
      this.streamingMp4Demux = (StreamingMp4Demux) null;
      this.streamingFileSource.SafeDispose();
      this.streamingFileSource = (IStreamingFileSource) null;
      this.videoStreamSource.SafeDispose();
      this.videoStreamSource = (VideoStreamSource) null;
    }

    private static FunXMPP.FMessage.Key CreateKey(Message msg)
    {
      return new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId);
    }

    private enum VideoContainerType
    {
      VideoPlayer,
      MediaElement,
    }
  }
}
