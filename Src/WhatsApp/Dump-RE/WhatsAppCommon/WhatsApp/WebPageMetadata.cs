// Decompiled with JetBrains decompiler
// Type: WhatsApp.WebPageMetadata
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class WebPageMetadata : WaDisposable
  {
    private IDisposable thumbSub;
    private bool loadThumbAttempted;
    private Subject<BitmapSource> pendingThumbSubject;

    public string OriginalUrl { get; set; }

    public string MatchedText { get; set; }

    public string CanonicalUrl { get; set; }

    public string Description { get; set; }

    public string Title { get; set; }

    public string ThumbnailUrl { get; set; }

    public WriteableBitmap Thumbnail { get; set; }

    public byte[] ThumbnailBytes
    {
      get
      {
        WriteableBitmap thumbnail = this.Thumbnail;
        if (thumbnail == null)
          return (byte[]) null;
        try
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            double num = 100.0 / (double) Math.Max(thumbnail.PixelWidth, thumbnail.PixelHeight);
            thumbnail.SaveJpegWithMaxSize((Stream) memoryStream, (int) ((double) thumbnail.PixelWidth * num), (int) ((double) thumbnail.PixelHeight * num), 0, Settings.JpegQuality, 48128);
            memoryStream.Position = 0L;
            return memoryStream.ToArray();
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get link preview thumb bytes");
        }
        return (byte[]) null;
      }
    }

    public IObservable<BitmapSource> LoadThumbnail()
    {
      if (this.Thumbnail != null)
        return Observable.Return<BitmapSource>((BitmapSource) this.Thumbnail);
      if (this.loadThumbAttempted)
        return (IObservable<BitmapSource>) this.pendingThumbSubject ?? (IObservable<BitmapSource>) (this.pendingThumbSubject = new Subject<BitmapSource>());
      this.loadThumbAttempted = true;
      Func<Exception, IObservable<byte[]>> log = (Func<Exception, IObservable<byte[]>>) (ex =>
      {
        Log.LogException(ex, "trying to check for thumbnail");
        return Observable.Return<byte[]>((byte[]) null);
      });
      return Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
      {
        bool cancelled = false;
        this.thumbSub = NativeWeb.SimpleGet(this.ThumbnailUrl, AppState.GetUrlPreviewUserAgent(), NativeWeb.Options.CacheDns).Select<Stream, byte[]>((Func<Stream, byte[]>) (stream =>
        {
          Log.d("link preview", "thumbnail got response {0}", (object) cancelled);
          if (cancelled)
            return (byte[]) null;
          byte[] buffer = (byte[]) null;
          using (stream)
          {
            buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
          }
          return buffer;
        })).SubscribeOn<byte[]>((IScheduler) AppState.Worker).Timeout<byte[]>(TimeSpan.FromSeconds(30.0)).Catch<byte[], Exception>(log).Take<byte[]>(1).Where<byte[]>((Func<byte[], bool>) (_ => _ != null)).DecodeJpeg(new Size?(new Size(168.0, 168.0))).ObserveOnDispatcher<WriteableBitmap>().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (bitmap =>
        {
          Log.d("link preview", "thumbnail got bitmap {0}", (object) cancelled);
          if (cancelled)
            return;
          int num = Math.Min(Math.Min(bitmap.PixelWidth, bitmap.PixelHeight), 140);
          int x = (bitmap.PixelWidth - num) / 2;
          int y = (bitmap.PixelHeight - num) / 2;
          WriteableBitmap writeableBitmap = bitmap.Crop(x, y, num, num);
          this.Thumbnail = writeableBitmap;
          observer.OnNext((BitmapSource) writeableBitmap);
          if (this.pendingThumbSubject != null)
            this.pendingThumbSubject.OnNext((BitmapSource) writeableBitmap);
          this.thumbSub.SafeDispose();
          this.thumbSub = (IDisposable) null;
        }), (Action<Exception>) (ex => Log.d("link preview", "Exception getting thumbnail {0}", (object) ex.ToString())));
        return (Action) (() =>
        {
          cancelled = true;
          this.thumbSub.SafeDispose();
          this.thumbSub = (IDisposable) null;
        });
      }));
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }
  }
}
