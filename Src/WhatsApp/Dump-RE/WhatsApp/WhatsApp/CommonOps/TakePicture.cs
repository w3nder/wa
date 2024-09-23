// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.TakePicture
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone;
using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class TakePicture
  {
    public static IObservable<TakePicture.CapturedPictureArgs> Launch(
      TakePicture.Mode mode,
      bool saveTempFile)
    {
      return TakePicture.Launch(mode, saveTempFile, 0, 0);
    }

    public static IObservable<TakePicture.CapturedPictureArgs> Launch(
      TakePicture.Mode mode,
      bool saveTempFile,
      int imgMaxWidth,
      int imgMaxHeight,
      bool isProfile = false,
      bool isGroupProfile = false)
    {
      return Observable.CreateWithDisposable<TakePicture.CapturedPictureArgs>((Func<IObserver<TakePicture.CapturedPictureArgs>, IDisposable>) (observer => CameraPage.Start(mode, imgMaxWidth, imgMaxHeight, isProfile, isGroupProfile).Subscribe<TakePicture.CapturedPictureArgs>((Action<TakePicture.CapturedPictureArgs>) (e => observer.OnNext(e)))));
    }

    public static TakePicture.CapturedPictureArgs ProcessChosenPhoto(
      Stream originalPhotoStream,
      bool saveTempFile,
      int maxWidth,
      int maxHeight,
      int? orientation = null)
    {
      MemoryStream memoryStream = new MemoryStream();
      originalPhotoStream.CopyTo((Stream) memoryStream);
      memoryStream.Position = 0L;
      NativeStream nativeStream = (NativeStream) null;
      if (saveTempFile)
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            nativeStream = nativeMediaStorage.GetTempFile();
            memoryStream.CopyTo((Stream) nativeStream);
            memoryStream.Position = 0L;
          }
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "saving captured photo to temp file");
        }
      }
      WriteableBitmap writeableBitmap = PictureDecoder.DecodeJpeg((Stream) memoryStream);
      if (orientation.HasValue)
      {
        try
        {
          ushort? jpegOrientation = JpegUtils.GetJpegOrientation((Stream) nativeStream);
          int? nullable = jpegOrientation.HasValue ? new int?((int) jpegOrientation.GetValueOrDefault()) : new int?();
          int num = orientation.Value;
          if ((nullable.GetValueOrDefault() == num ? (!nullable.HasValue ? 1 : 0) : 1) == 0)
          {
            if (!CameraPage.IsTakingProfilePicture)
              goto label_12;
          }
          writeableBitmap = JpegUtils.ApplyJpegOrientation((BitmapSource) writeableBitmap, orientation);
          writeableBitmap.SaveJpeg((Stream) nativeStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, orientation.Value, Settings.JpegQuality);
          nativeStream.Position = 0L;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "to jpeg stream");
        }
      }
label_12:
      memoryStream.SafeDispose();
      return new TakePicture.CapturedPictureArgs(nativeStream, writeableBitmap);
    }

    public enum Mode
    {
      Regular,
      StatusOnly,
      ProfilePicture,
      GroupPicture,
    }

    public class CapturedPictureArgs
    {
      public MediaPickerState.Item CameraRollItem;
      public WaVideoArgs VideoArgs;

      public NativeStream TempFileStream { get; private set; }

      public WriteableBitmap Bitmap { get; set; }

      public System.Windows.Point ZoomScale { get; set; }

      public CapturedPictureArgs(
        NativeStream tempFileStream,
        WriteableBitmap bitmap,
        WaVideoArgs videoargs = null)
      {
        this.TempFileStream = tempFileStream;
        this.Bitmap = bitmap;
        this.VideoArgs = videoargs;
      }

      public CapturedPictureArgs(MediaPickerState.Item CameraRoll)
      {
        this.CameraRollItem = CameraRoll;
      }
    }
  }
}
