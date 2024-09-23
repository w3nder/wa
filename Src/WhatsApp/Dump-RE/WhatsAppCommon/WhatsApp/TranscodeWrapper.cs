// Decompiled with JetBrains decompiler
// Type: WhatsApp.TranscodeWrapper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class TranscodeWrapper
  {
    private static ObservableQueue transcodeQueue;

    public static IObservable<TranscodeWrapper.Result> GetRemuxObservable(
      string path,
      TranscodeArgs args,
      Action<int> onProgress)
    {
      return Observable.Create<TranscodeWrapper.Result>((Func<IObserver<TranscodeWrapper.Result>, Action>) (observer =>
      {
        bool cancel = false;
        object cancelLock = new object();
        IDisposable transcodeSub = (IDisposable) null;
        ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
        {
          TranscodeWrapper.Result res = new TranscodeWrapper.Result();
          string tempFile = (string) null;
          bool flag = true;
          res.Suffix = "mp4";
          Action performCleanup = (Action) (() =>
          {
            res.Stream.SafeDispose();
            if (tempFile == null)
              return;
            try
            {
              using (IMediaStorage mediaStorage = MediaStorage.Create(tempFile))
                mediaStorage.DeleteFile(tempFile);
            }
            catch (Exception ex)
            {
            }
          });
          try
          {
            if (cancel)
              return;
            float framerate = TranscodeWrapper.TryGetFramerate(path);
            path = MediaStorage.GetAbsolutePath(path);
            if (cancel)
              return;
            MediaUpload.CreateTempFileForMp4Truncation(out tempFile, out res.Stream);
            res.Filename = tempFile = MediaStorage.GetAbsolutePath(tempFile);
            IMp4Utils mp4Utils = NativeInterfaces.Mp4Utils;
            string AudioFilename = path;
            string VideoFilename = path;
            string OutputFilename = tempFile;
            int? nullable4 = args.StartMilliseconds;
            double StartTime;
            if (!nullable4.HasValue)
            {
              StartTime = 0.0;
            }
            else
            {
              nullable4 = args.StartMilliseconds;
              StartTime = (double) nullable4.Value / 1000.0;
            }
            nullable4 = args.DurationMilliseconds;
            double Duration;
            if (!nullable4.HasValue)
            {
              Duration = -1.0;
            }
            else
            {
              nullable4 = args.DurationMilliseconds;
              Duration = (double) nullable4.Value / 1000.0;
            }
            double TargetFramerate = (double) framerate;
            mp4Utils.MuxAVStreams(AudioFilename, VideoFilename, OutputFilename, (float) StartTime, (float) Duration, (float) TargetFramerate);
            if (cancel)
              return;
            if (res.Stream.Length > (long) Settings.MaxMediaSize)
            {
              TranscodeArgs transcodeArgs3 = args;
              nullable4 = new int?();
              int? nullable5 = nullable4;
              transcodeArgs3.StartMilliseconds = nullable5;
              TranscodeArgs transcodeArgs4 = args;
              nullable4 = new int?();
              int? nullable6 = nullable4;
              transcodeArgs4.DurationMilliseconds = nullable6;
              args.Flags &= ~TranscodeReason.TimeCrop;
              lock (cancelLock)
              {
                if (cancel)
                  return;
                flag = false;
                transcodeSub = TranscodeWrapper.GetTranscodeObservable(tempFile, args, FunXMPP.FMessage.Type.Video, onProgress).Do<TranscodeWrapper.Result>((Action<TranscodeWrapper.Result>) (__ => performCleanup()), (Action<Exception>) (ex => performCleanup())).Subscribe(observer);
              }
            }
            else
            {
              flag = false;
              observer.OnNext(res);
              observer.OnCompleted();
            }
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            observer.OnCompleted();
          }
          finally
          {
            if (flag)
              performCleanup();
          }
        }));
        return (Action) (() =>
        {
          lock (cancelLock)
          {
            cancel = true;
            transcodeSub.SafeDispose();
            transcodeSub = (IDisposable) null;
          }
        });
      }));
    }

    public static float TryGetFramerate(string path)
    {
      using (Stream file = MediaStorage.OpenFile(path))
        return TranscodeWrapper.TryGetFramerate(file);
    }

    public static float TryGetFramerate(Stream file)
    {
      float framerate = -1f;
      if ((double) framerate < 0.0)
      {
        file.Position = 0L;
        try
        {
          using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(file, disposeStream: false))
          {
            if (videoFrameGrabber.FrameInfo.FrameRatePeriod != 0U)
            {
              framerate = ((float) videoFrameGrabber.FrameInfo.FrameRate + 0.0f) / (float) videoFrameGrabber.FrameInfo.FrameRatePeriod;
              Log.l("transcode", "Got framerate from media foundation: {0}", (object) framerate);
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get framerate");
        }
      }
      if ((double) framerate < 0.0)
      {
        file.Position = 0L;
        try
        {
          IMp4Utils mp4Utils = NativeInterfaces.Mp4Utils;
          using (Mp4MappedStream mp4MappedStream = mp4Utils.MapStream(file))
          {
            ref Mp4UtilsVideoMetdata? local = ref mp4Utils.GetStreamMetadata(mp4MappedStream.Filename).Video;
            framerate = local.HasValue ? local.GetValueOrDefault().Fps : -1f;
            if ((double) framerate >= 0.0)
              Log.l("transcode", "Got framerate from mp4utils: {0}", (object) framerate);
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get framerate");
        }
      }
      file.Position = 0L;
      return framerate;
    }

    public static IObservable<TranscodeWrapper.Result> GetRemuxAudioObservable(
      string path,
      TranscodeArgs args,
      Action<int> onProgress)
    {
      return Observable.Create<TranscodeWrapper.Result>((Func<IObserver<TranscodeWrapper.Result>, Action>) (observer =>
      {
        bool cancel = false;
        object cancelLock = new object();
        IDisposable transcodeSub = (IDisposable) null;
        ManualResetEvent cancelEvent = new ManualResetEvent(false);
        ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
        {
          TranscodeWrapper.Result res = new TranscodeWrapper.Result();
          string tempFile = (string) null;
          string newAudioTrack = (string) null;
          Stream newAudioTrackStream = (Stream) null;
          bool flag = true;
          res.Suffix = "mp4";
          Action action = (Action) (() =>
          {
            Action<string> action2 = (Action<string>) (f =>
            {
              if (f == null)
                return;
              try
              {
                using (IMediaStorage mediaStorage = MediaStorage.Create(f))
                  mediaStorage.DeleteFile(f);
              }
              catch (Exception ex)
              {
              }
            });
            res.Stream.SafeDispose();
            action2(tempFile);
            newAudioTrackStream.SafeDispose();
            action2(newAudioTrack);
          });
          try
          {
            if (cancel)
              return;
            float framerate = TranscodeWrapper.TryGetFramerate(path);
            using (TranscodeWrapper.DemuxResult demuxResult = new TranscodeWrapper.DemuxResult(path))
            {
              if (cancel)
                return;
              lock (cancelLock)
              {
                if (cancel)
                  return;
                transcodeSub = TranscodeWrapper.GetTranscodeObservable(demuxResult.AudioTrack, args, FunXMPP.FMessage.Type.Audio, onProgress).Subscribe<TranscodeWrapper.Result>((Action<TranscodeWrapper.Result>) (innerRes =>
                {
                  try
                  {
                    using (innerRes.Stream)
                    {
                      MediaUpload.CreateTempFileForMp4Truncation(out newAudioTrack, out newAudioTrackStream);
                      innerRes.Stream.CopyTo(newAudioTrackStream);
                    }
                  }
                  catch (Exception ex)
                  {
                    cancel = true;
                  }
                  cancelEvent.Set();
                }), (Action<Exception>) (ex =>
                {
                  cancel = true;
                  cancelEvent.Set();
                }));
              }
              cancelEvent.WaitOne();
              if (cancel)
                return;
              MediaUpload.CreateTempFileForMp4Truncation(out tempFile, out res.Stream);
              res.Filename = tempFile = MediaStorage.GetAbsolutePath(tempFile);
              newAudioTrack = MediaStorage.GetAbsolutePath(newAudioTrack);
              NativeInterfaces.Mp4Utils.MuxAVStreams(newAudioTrack, demuxResult.VideoTrackForMux ?? "", tempFile, 0.0f, -1f, framerate);
            }
            if (cancel)
              return;
            flag = false;
            observer.OnNext(res);
            observer.OnCompleted();
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            observer.OnCompleted();
          }
          finally
          {
            if (flag)
              action();
          }
        }));
        return (Action) (() =>
        {
          lock (cancelLock)
          {
            cancel = true;
            transcodeSub.SafeDispose();
            transcodeSub = (IDisposable) null;
            cancelEvent.Set();
          }
        });
      }));
    }

    public static IObservable<TranscodeWrapper.Result> GetPiecemealTranscodeObservable(
      string path,
      TranscodeArgs args,
      Action<int> onProgress)
    {
      bool audioError = false;
      int audioProgress = 0;
      int videoProgress = 0;
      Action<int> onProgressShim = (Action<int>) (totalPct => Deployment.Current.Dispatcher.BeginInvoke((Action) (() => onProgress(totalPct))));
      Action<int> onProgress1 = (Action<int>) (pct =>
      {
        audioProgress = pct;
        onProgressShim((audioProgress + videoProgress) / 2);
      });
      Action<int> onProgress2 = (Action<int>) (pct =>
      {
        videoProgress = pct;
        if (audioError)
          onProgressShim(videoProgress);
        else
          onProgressShim((audioProgress + videoProgress) / 2);
      });
      IObservable<TranscodeWrapper.Result> source = TranscodeWrapper.GetTranscodeObservable(path, args, FunXMPP.FMessage.Type.Audio, onProgress1, (Func<NativeStream, IVideoUtils>) null, (Func<NativeStream, ISoundSource>) (stream =>
      {
        try
        {
          return TranscodeWrapper.DefaultAudioCtor(stream);
        }
        catch (Exception ex)
        {
          audioError = true;
          throw;
        }
      })).Catch<TranscodeWrapper.Result, Exception>((Func<Exception, IObservable<TranscodeWrapper.Result>>) (ex => !audioError ? TranscodeWrapper.ForwardException<TranscodeWrapper.Result>(ex) : Observable.Return<TranscodeWrapper.Result>((TranscodeWrapper.Result) null)));
      IObservable<TranscodeWrapper.Result> videoPart = TranscodeWrapper.GetTranscodeObservable(path, args, FunXMPP.FMessage.Type.Video, onProgress2, new Func<NativeStream, IVideoUtils>(TranscodeWrapper.DefaultVideoCtor), (Func<NativeStream, ISoundSource>) null);
      Func<TranscodeWrapper.Result, TranscodeWrapper.Result, IObservable<TranscodeWrapper.Result>> mergeResults = (Func<TranscodeWrapper.Result, TranscodeWrapper.Result, IObservable<TranscodeWrapper.Result>>) ((audio, video) => audio == null ? Observable.Return<TranscodeWrapper.Result>(video) : Observable.Create<TranscodeWrapper.Result>((Func<IObserver<TranscodeWrapper.Result>, Action>) (observer =>
      {
        TranscodeWrapper.Result result = new TranscodeWrapper.Result()
        {
          Suffix = "mp4",
          MimeType = "video/mp4",
          RotationPerformed = video.RotationPerformed
        };
        using (Mp4MappedStream mp4MappedStream1 = NativeInterfaces.Mp4Utils.MapStream(audio.Stream))
        {
          using (Mp4MappedStream mp4MappedStream2 = NativeInterfaces.Mp4Utils.MapStream(video.Stream))
          {
            MediaUpload.CreateTempFileForMp4Truncation(out result.Filename, out result.Stream);
            float framerate = TranscodeWrapper.TryGetFramerate(video.Stream);
            NativeInterfaces.Mp4Utils.MuxAVStreams(mp4MappedStream1.Filename, mp4MappedStream2.Filename, MediaStorage.GetAbsolutePath(result.Filename), 0.0f, -1f, framerate);
          }
        }
        observer.OnNext(result);
        observer.OnCompleted();
        return (Action) (() => { });
      })));
      Func<TranscodeWrapper.Result, IObservable<TranscodeWrapper.Result>> collectionSelector = (Func<TranscodeWrapper.Result, IObservable<TranscodeWrapper.Result>>) (audioResult => videoPart);
      return source.SelectMany(collectionSelector, (audioResult, videoResult) => new
      {
        audioResult = audioResult,
        videoResult = videoResult
      }).SelectMany(_param1 => Observable.Defer<TranscodeWrapper.Result>((Func<IObservable<TranscodeWrapper.Result>>) (() => mergeResults(_param1.audioResult, _param1.videoResult))), (_param1, merged) => merged);
    }

    private static IObservable<T> ForwardException<T>(Exception ex)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        observer.OnError(ex);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static IVideoUtils DefaultVideoCtor(NativeStream stream)
    {
      return NativeInterfaces.MediaMisc.OpenVideo(stream.GetNative().Clone(), false);
    }

    public static ISoundSource DefaultAudioCtor(NativeStream stream)
    {
      return NativeInterfaces.Misc.CreateSoundSource((IEnumerable<SoundPlaybackCodec>) CodecDetector.DetectAudioCodec((Stream) stream).Codecs, stream.GetNative());
    }

    public static IObservable<TranscodeWrapper.Result> GetTranscodeObservable(
      string path,
      TranscodeArgs args,
      FunXMPP.FMessage.Type mediaType,
      Action<int> onProgress)
    {
      return mediaType == FunXMPP.FMessage.Type.Video || mediaType == FunXMPP.FMessage.Type.Gif ? TranscodeWrapper.GetPiecemealTranscodeObservable(path, args, onProgress) : TranscodeWrapper.GetTranscodeObservable(path, args, mediaType, onProgress, new Func<NativeStream, IVideoUtils>(TranscodeWrapper.DefaultVideoCtor), new Func<NativeStream, ISoundSource>(TranscodeWrapper.DefaultAudioCtor));
    }

    public static IObservable<TranscodeWrapper.Result> GetTranscodeObservable(
      string path,
      TranscodeArgs args,
      FunXMPP.FMessage.Type mediaType,
      Action<int> onProgress,
      Func<NativeStream, IVideoUtils> videoCtor,
      Func<NativeStream, ISoundSource> audioCtor)
    {
      return Observable.Create<TranscodeWrapper.Result>((Func<IObserver<TranscodeWrapper.Result>, Action>) (observer =>
      {
        bool cancel = false;
        object cancelLock = new object();
        ITranscoder tc = (ITranscoder) null;
        NativeStream dstStream = (NativeStream) null;
        TranscodeWrapper.Result res = new TranscodeWrapper.Result();
        ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
        {
          bool flag4 = false;
          IWAStream o = (IWAStream) null;
          Action action = (Action) (() => { });
          try
          {
            lock (cancelLock)
            {
              if (cancel)
              {
                observer.OnCompleted();
                return;
              }
              tc = (ITranscoder) NativeInterfaces.CreateInstance<Transcoder>();
            }
            path = MediaStorage.GetAbsolutePath(path);
            Log.l("Transcode", "Starting transcode for file: [{0}]", (object) path);
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              using (NativeStream nativeStream = (NativeStream) nativeMediaStorage.OpenFile(path, FileMode.Open, FileAccess.Read))
              {
                dstStream = nativeMediaStorage.GetTempFile();
                GC.Collect();
                int? nullable;
                if (args.XOffset.HasValue && args.YOffset.HasValue && args.Width.HasValue && args.Height.HasValue)
                {
                  ITranscoder transcoder4 = tc;
                  ITranscoder transcoder5 = tc;
                  int x = args.XOffset.Value;
                  nullable = args.YOffset;
                  int y = nullable.Value;
                  nullable = args.Width;
                  int w = nullable.Value;
                  nullable = args.Height;
                  int h = nullable.Value;
                  IImageTransform rectangleTransform = transcoder5.CreateClipRectangleTransform((uint) x, (uint) y, (uint) w, (uint) h);
                  transcoder4.AddImageTransform(rectangleTransform);
                }
                bool flag5 = false;
                bool flag6 = audioCtor != null;
                TranscoderContainerType outputContainer;
                switch (mediaType)
                {
                  case FunXMPP.FMessage.Type.Audio:
                    outputContainer = TranscoderContainerType.Mp4;
                    res.Suffix = "m4a";
                    res.MimeType = "audio/mp4";
                    break;
                  case FunXMPP.FMessage.Type.Video:
                    int num = Settings.MaxVideoEdge;
                    if (!AppState.IsDecentMemoryDevice)
                      num = Math.Min(480, num);
                    tc.AddImageTransform(tc.CreateMaxEdgeTransform((uint) num));
                    outputContainer = TranscoderContainerType.Mp4;
                    res.Suffix = "mp4";
                    res.MimeType = "video/mp4";
                    flag5 = true;
                    break;
                  default:
                    throw new InvalidOperationException("unknown media type: " + mediaType.ToString());
                }
                IVideoUtils videoObject = (IVideoUtils) null;
                ISoundSource audioObject = (ISoundSource) null;
                action = (Action) (() =>
                {
                  if (videoObject != null)
                  {
                    Marshal.ReleaseComObject((object) videoObject);
                    videoObject = (IVideoUtils) null;
                  }
                  if (audioObject == null)
                    return;
                  Marshal.ReleaseComObject((object) audioObject);
                  audioObject = (ISoundSource) null;
                });
                if (flag5)
                  videoObject = videoCtor(nativeStream);
                if (flag6)
                {
                  try
                  {
                    audioObject = audioCtor(nativeStream);
                  }
                  catch (Exception ex)
                  {
                    if (!flag5)
                      throw;
                  }
                }
                tc.Initialize(videoObject, audioObject, outputContainer, dstStream.GetNative());
                action();
                nullable = args.StartMilliseconds;
                if (nullable.HasValue)
                {
                  ITranscoder transcoder = tc;
                  nullable = args.StartMilliseconds;
                  long millis = (long) nullable.Value;
                  transcoder.Seek(millis);
                }
                ITranscoder transcoder6 = tc;
                nullable = args.DurationMilliseconds;
                long durationMillisOrNegative = (long) (nullable ?? -1);
                TranscodeWrapper.TranscoderProgress progress = new TranscodeWrapper.TranscoderProgress(onProgress);
                transcoder6.Transcode(durationMillisOrNegative, (ITranscoderProgress) progress);
              }
            }
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            observer.OnCompleted();
            flag4 = true;
            return;
          }
          finally
          {
            lock (cancelLock)
            {
              if (tc != null)
              {
                Marshal.ReleaseComObject((object) tc);
                tc = (ITranscoder) null;
              }
              if (flag4)
              {
                dstStream.SafeDispose();
                dstStream = (NativeStream) null;
              }
            }
            action();
            if (o != null)
              Marshal.ReleaseComObject((object) o);
          }
          dstStream.Position = 0L;
          res.Stream = (Stream) dstStream;
          observer.OnNext(res);
        }));
        return (Action) (() =>
        {
          lock (cancelLock)
          {
            tc?.Cancel();
            dstStream.SafeDispose();
            dstStream = (NativeStream) null;
            cancel = true;
          }
        });
      }));
    }

    public static void ProcessPendingTranscode(Message msg)
    {
      TranscodeArgs args = (TranscodeArgs) null;
      MessageMiscInfo misc = (MessageMiscInfo) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        misc = msg.GetMiscInfo((SqliteMessagesContext) db);
        if (misc != null && misc.TranscoderData != null)
          args = JsonBase.Deserialize<TranscodeArgs>(misc.TranscoderData, 0, misc.TranscoderData.Length);
        if (args != null || msg.Status != FunXMPP.FMessage.Status.Pending)
          return;
        Log.l("Transcode", "Message was marked pending, but no transcode metadata set.");
        msg.Status = FunXMPP.FMessage.Status.Error;
        db.SubmitChanges();
      }));
      if (msg.Status == FunXMPP.FMessage.Status.Error && AppState.GetConnection().EventHandler.Qr.Session.Active)
        AppState.GetConnection().SendQrReceived(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId), FunXMPP.FMessage.Status.Error);
      if (msg.Status != FunXMPP.FMessage.Status.Pending)
      {
        Log.l("Transcode", "Message was not pending {0}", (object) msg.Status);
      }
      else
      {
        if (args == null)
          return;
        if (AppState.IsBackgroundAgent && BackgroundAgent.agentType != BackgroundAgent.BackgroundAgentType.AudioAgent)
        {
          Log.l("Transcode", "Don't want to transcode from memory-constrained BG agent.  Returning early.");
        }
        else
        {
          Utils.LazyInit<ObservableQueue>(ref TranscodeWrapper.transcodeQueue, (Func<ObservableQueue>) (() => new ObservableQueue()));
          IDisposable transcodeSub = (IDisposable) null;
          IDisposable cancelSub = (IDisposable) null;
          Action onComplete = Utils.IgnoreMultipleInvokes((Action) (() =>
          {
            transcodeSub.SafeDispose();
            cancelSub.SafeDispose();
            msg.ClearPendingMedia();
          }));
          cancelSub = Observable.Merge<Unit>(MessagesContext.Events.DeletedConversationSubject.Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == msg.KeyRemoteJid)).Select<Conversation, Unit>((Func<Conversation, Unit>) (_ => new Unit())), MessagesContext.Events.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.MessageID == msg.MessageID)).Select<Message, Unit>((Func<Message, Unit>) (_ => new Unit())), VoipHandler.CallStartedSubject.Select<Unit, Unit>((Func<Unit, Unit>) (_ => new Unit()))).Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => onComplete()));
          Action<int> onProgress = (Action<int>) (pct => msg.TransferValue = (double) (Math.Min(pct, 100) / 2) / 100.0);
          IObservable<TranscodeWrapper.Result> transcodeObsSnap = Observable.If<TranscodeWrapper.Result>((Func<bool>) (() => (msg.MediaWaType == FunXMPP.FMessage.Type.Video || msg.MediaWaType == FunXMPP.FMessage.Type.Gif) && (args.Flags & ~(TranscodeReason.FileSize | TranscodeReason.BadContainer)) == TranscodeReason.TimeCrop && TranscodeWrapper.CheckValidTimeCrop(msg.LocalFileUri, args) || args.Flags == TranscodeReason.BadContainer), TranscodeWrapper.GetRemuxObservable(msg.LocalFileUri, args, onProgress), Observable.If<TranscodeWrapper.Result>((Func<bool>) (() => (args.Flags & TranscodeReason.TranscodeAudio) != 0), TranscodeWrapper.GetRemuxAudioObservable(msg.LocalFileUri, args, onProgress), TranscodeWrapper.GetTranscodeObservable(msg.LocalFileUri, args, msg.MediaWaType, onProgress))).Do<TranscodeWrapper.Result>((Action<TranscodeWrapper.Result>) (res =>
          {
            using (Stream stream = res.Stream)
              res.Filename = MediaUpload.CopyLocal(stream, string.Format("transcode_{0}_." + res.Suffix, (object) DateTime.Now.ToUnixTime()));
            if (res.RotationPerformed)
              return;
            int? rotation = args.Rotation;
            if (!rotation.HasValue)
              return;
            rotation = args.Rotation;
            int? nullable = JpegUtils.AngleForRotation(rotation.Value);
            if (!nullable.HasValue)
              return;
            Mp4Atom.OrientationMatrix orientationMatrix = VideoFrameGrabber.MatrixForAngle((360 - nullable.Value) % 360);
            if (orientationMatrix == null)
              return;
            VideoFrameGrabber.WriteRotationMatrix(res.Filename, orientationMatrix.Matrix);
          })).ObserveOn<TranscodeWrapper.Result>((IScheduler) AppState.Worker).Do<TranscodeWrapper.Result>((Action<TranscodeWrapper.Result>) (res =>
          {
            string str = MediaDownload.SaveMedia(res.Filename, msg.MediaWaType, isPtt: msg.IsPtt());
            using (IMediaStorage mediaStorage = MediaStorage.Create(res.Filename))
            {
              try
              {
                mediaStorage.DeleteFile(res.Filename);
              }
              catch (Exception ex)
              {
              }
            }
            res.Filename = str;
          })).ObserveInQueue<TranscodeWrapper.Result>(TranscodeWrapper.transcodeQueue).ObserveUntilLeavingFg<TranscodeWrapper.Result>().ObserveOn<TranscodeWrapper.Result>(WAThreadPool.Scheduler).Take<TranscodeWrapper.Result>(1).Do<TranscodeWrapper.Result>((Action<TranscodeWrapper.Result>) (res =>
          {
            bool skip = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              if (msg.Status != FunXMPP.FMessage.Status.Pending)
              {
                skip = true;
              }
              else
              {
                db.LocalFileRelease(msg.LocalFileUri, LocalFileType.MessageMedia);
                msg.LocalFileUri = res.Filename;
                long num = 0;
                try
                {
                  using (IMediaStorage mediaStorage = MediaStorage.Create(res.Filename))
                  {
                    using (Stream stream = mediaStorage.OpenFile(res.Filename))
                    {
                      num = stream.Length;
                      msg.MediaHash = MediaUpload.ComputeHash(stream);
                    }
                  }
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "could get file length");
                }
                db.LocalFileAddRef(res.Filename, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
                msg.MediaSize = num;
                Log.l("transcode", "New file size is {0}", (object) num);
                if (res.MimeType != null)
                {
                  msg.MediaMimeType = res.MimeType;
                  Log.l("transcode", "Setting mime type to {0}", (object) res.MimeType);
                }
                misc.TranscoderData = (byte[]) null;
                msg.Status = !msg.KeyFromMe ? FunXMPP.FMessage.Status.Undefined : FunXMPP.FMessage.Status.Uploading;
                db.SubmitChanges();
              }
            }));
            if (skip || !msg.KeyFromMe)
              return;
            onComplete();
            AppState.Worker.Enqueue((Action) (() => msg.SetPendingMediaSubscription("Upload after transcode", PendingMediaTransfer.TransferTypes.Upload_NotWeb, MediaUpload.SendMediaObservable(msg))));
          }), (Action<Exception>) (ex => Log.LogException(ex, "transcode")), onComplete);
          IObservable<TranscodeWrapper.Result> source = Observable.Create<TranscodeWrapper.Result>((Func<IObserver<TranscodeWrapper.Result>, Action>) (observer =>
          {
            transcodeSub = transcodeObsSnap.Subscribe(observer);
            return onComplete;
          }));
          msg.SetPendingMediaSubscription("Transcode", PendingMediaTransfer.TransferTypes.Transcode, source.Select<TranscodeWrapper.Result, Unit>((Func<TranscodeWrapper.Result, Unit>) (_ => new Unit())));
        }
      }
    }

    private static bool CheckValidTimeCrop(string filePath, TranscodeArgs args)
    {
      ulong[] seq = (ulong[]) null;
      filePath = MediaStorage.GetAbsolutePath(filePath);
      try
      {
        seq = NativeInterfaces.Mp4Utils.GetEditPoints(filePath);
      }
      catch (Exception ex)
      {
      }
      if (seq == null || seq.Length == 0)
        return false;
      long start = (long) (args.StartMilliseconds ?? 0);
      long num1 = 0;
      long num2 = 0;
      if (start != 0L)
        num1 = (long) ((IEnumerable<ulong>) seq).MinOfFunc<ulong, long>((Func<ulong, long>) (editPoint => Math.Abs((long) editPoint - start))) - start;
      int? nullable1 = args.DurationMilliseconds;
      if (nullable1.HasValue)
      {
        nullable1 = args.StartMilliseconds;
        long num3 = (long) (nullable1 ?? 0);
        nullable1 = args.DurationMilliseconds;
        long num4 = (long) nullable1.Value;
        long end = num3 + num4;
        ulong num5 = ((IEnumerable<ulong>) seq).MinOfFunc<ulong, long>((Func<ulong, long>) (editPoint => Math.Abs((long) editPoint - end)));
        num2 = (long) num5 - (long) num5;
      }
      int num6 = 1000;
      if (Math.Abs(num1) > (long) num6 || Math.Abs(num2) > (long) num6)
        return false;
      nullable1 = args.StartMilliseconds;
      if (nullable1.HasValue)
      {
        TranscodeArgs transcodeArgs = args;
        nullable1 = args.StartMilliseconds;
        int? nullable2 = new int?(nullable1.Value + (int) num1);
        transcodeArgs.StartMilliseconds = nullable2;
      }
      nullable1 = args.DurationMilliseconds;
      if (nullable1.HasValue)
      {
        TranscodeArgs transcodeArgs = args;
        nullable1 = args.DurationMilliseconds;
        int? nullable3 = new int?(nullable1.Value - (int) num1 + (int) num2);
        transcodeArgs.DurationMilliseconds = nullable3;
      }
      return true;
    }

    public class Result
    {
      public Stream Stream;
      public string Suffix;
      public string MimeType;
      public string Filename;
      public bool RotationPerformed;
    }

    public class DemuxResult : IDisposable
    {
      private string tmpPath;
      private string inputPath;
      private Dictionary<string, string> allTracksByExtension;
      private string audioTrack;
      private string videoTrack;

      public DemuxResult(string path)
      {
        path = this.inputPath = MediaStorage.GetAbsolutePath(path);
        string isoStoreRelative = string.Format("tmp\\remux-{0}-{1}-{2}", AppState.IsBackgroundAgent ? (object) "bg" : (object) "fg", (object) NativeInterfaces.Misc.GetProcessId(), (object) DateTime.Now.Ticks);
        this.tmpPath = string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) isoStoreRelative);
        Action<Action> action = (Action<Action>) (a =>
        {
          try
          {
            a();
          }
          catch (Exception ex)
          {
          }
        });
        action((Action) (() => NativeInterfaces.Misc.RemoveDirectoryRecursive(this.tmpPath)));
        using (IsolatedStorageFile fs = IsolatedStorageFile.GetUserStoreForApplication())
          action((Action) (() => fs.CreateDirectory(isoStoreRelative)));
        try
        {
          NativeInterfaces.Mp4Utils.ExtractAVStreams(path, this.tmpPath);
        }
        catch (Exception ex)
        {
          this.Dispose();
        }
      }

      private string[] AllTracks
      {
        get
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            return nativeMediaStorage.FindFiles(string.Format("{0}\\*", (object) this.ResultDirectory)).Where<WIN32_FIND_DATA>((Func<WIN32_FIND_DATA, bool>) (d => d.cFileName != "." && d.cFileName != "..")).Select<WIN32_FIND_DATA, string>((Func<WIN32_FIND_DATA, string>) (d => string.Format("{0}\\{1}", (object) this.ResultDirectory, (object) d.cFileName))).ToArray<string>();
        }
      }

      private Dictionary<string, string> AllTracksByExtension
      {
        get
        {
          return Utils.LazyInit<Dictionary<string, string>>(ref this.allTracksByExtension, (Func<Dictionary<string, string>>) (() => ((IEnumerable<string>) this.AllTracks).ToDictionary<string, string>((Func<string, string>) (filename =>
          {
            int num = filename.LastIndexOf('.');
            if (num >= 0)
              filename = filename.Substring(num + 1).ToLowerInvariant();
            return filename;
          }))));
        }
      }

      private IEnumerable<string> FindTracksByExtension(params string[] extensions)
      {
        string[] strArray = extensions;
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str1 = strArray[index];
          string str2 = (string) null;
          if (this.AllTracksByExtension.TryGetValue(str1.ToLowerInvariant(), out str2))
            yield return str2;
        }
        strArray = (string[]) null;
      }

      public string AudioTrack
      {
        get
        {
          return Utils.LazyInit<string>(ref this.audioTrack, (Func<string>) (() => this.FindTracksByExtension("aac", "mp3", "amr", "qcp").OrderBy<string, string>((Func<string, string>) (s => s)).FirstOrDefault<string>()));
        }
      }

      public string VideoTrack
      {
        get
        {
          return Utils.LazyInit<string>(ref this.videoTrack, (Func<string>) (() => this.FindTracksByExtension("h263", "h264", "m4v").OrderBy<string, string>((Func<string, string>) (s => s)).FirstOrDefault<string>()));
        }
      }

      public string VideoTrackForMux
      {
        get
        {
          string videoTrackForMux = this.VideoTrack;
          if (videoTrackForMux != null && !videoTrackForMux.EndsWith(".h264", StringComparison.InvariantCultureIgnoreCase))
            videoTrackForMux = this.inputPath;
          return videoTrackForMux;
        }
      }

      public string ResultDirectory => this.tmpPath;

      public void Dispose()
      {
        ((Action<Action>) (a =>
        {
          try
          {
            a();
          }
          catch (Exception ex)
          {
          }
        }))((Action) (() => NativeInterfaces.Misc.RemoveDirectoryRecursive(this.tmpPath)));
      }
    }

    private class TranscoderProgress : ITranscoderProgress
    {
      private Action<int> onProgress;

      public TranscoderProgress(Action<int> onProgress) => this.onProgress = onProgress;

      public void OnProgress(int pct)
      {
        Log.l("Transcode", "({0}%)", (object) pct);
        if (this.onProgress == null)
          return;
        this.onProgress(pct);
      }
    }
  }
}
