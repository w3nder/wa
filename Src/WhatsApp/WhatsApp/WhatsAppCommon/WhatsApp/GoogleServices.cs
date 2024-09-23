// Decompiled with JetBrains decompiler
// Type: WhatsApp.GoogleServices
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class GoogleServices : IMapThumbnailFetch
  {
    public IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceSize,
      int cropSize)
    {
      return this.GetMapThumbnail(latitude, longitude, sourceSize, sourceSize, cropSize, cropSize);
    }

    public IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceWidth,
      int sourceHeight,
      int cropWidth,
      int cropHeight)
    {
      sourceHeight = sourceHeight / 2 + 40;
      sourceWidth /= 2;
      string uri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/maps/api/staticmap?center={0},{1}&zoom=15&size={2}x{3}&scale=2&visual_refresh=true&sensor=true&format=png8&mobile=true&markers=color:red%7Csize:mid%7C{0},{1}&&maptype={4}&client=gme-whatsappinc", (object) latitude, (object) longitude, (object) sourceWidth, (object) sourceHeight, Settings.MapCartographicModeRoad ? (object) "roadmap" : (object) "hybrid");
      byte[] hash;
      using (HMACSHA1 hmacshA1 = new HMACSHA1(Convert.FromBase64String(NativeInterfaces.Misc.GetString(11).Replace('-', '+').Replace('_', '/'))))
        hash = hmacshA1.ComputeHash(Encoding.UTF8.GetBytes(uri));
      uri = "https://maps.googleapis.com" + uri + "&signature=" + Convert.ToBase64String(hash, 0, 20).ToUrlSafeBase64String();
      return Observable.Defer<WriteableBitmap>((Func<IObservable<WriteableBitmap>>) (() => this.WebRequest(uri).Cache(WebServices.GetCachePath("googMap", latitude, longitude, (double) sourceWidth, (double) sourceHeight, (string) null)).DecodeJpeg().Select<WriteableBitmap, WriteableBitmap>((Func<WriteableBitmap, WriteableBitmap>) (src =>
      {
        if (sourceWidth > cropWidth || sourceHeight > cropHeight)
        {
          if (cropWidth > sourceWidth)
            cropWidth = sourceWidth;
          if (cropHeight > sourceHeight)
            cropHeight = sourceHeight;
          WriteableBitmap writeableBitmap = new WriteableBitmap(cropWidth, cropHeight);
          writeableBitmap.Render((UIElement) new Image()
          {
            Source = (ImageSource) src,
            Width = (double) src.PixelWidth,
            Height = (double) src.PixelHeight
          }, (Transform) new TranslateTransform()
          {
            X = (double) (-(sourceWidth - cropWidth) / 2),
            Y = (double) (-(sourceHeight - cropHeight) / 2)
          });
          writeableBitmap.Invalidate();
          src = writeableBitmap;
        }
        return src;
      }))));
    }

    public IObservable<byte[]> WebRequest(string uri)
    {
      return Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync()));
    }

    public class OverQuota : Exception
    {
    }
  }
}
