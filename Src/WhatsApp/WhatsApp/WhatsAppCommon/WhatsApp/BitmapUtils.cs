// Decompiled with JetBrains decompiler
// Type: WhatsApp.BitmapUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public static class BitmapUtils
  {
    public static WriteableBitmap CreateBitmap(Stream stream)
    {
      return BitmapUtils.CreateBitmap(stream, 0, 0);
    }

    public static WriteableBitmap CreateBitmap(
      Stream stream,
      int maxPixelWidth,
      int maxPixelHeight)
    {
      if (stream == null)
        return (WriteableBitmap) null;
      WriteableBitmap bitmap;
      try
      {
        stream.Position = 0L;
        bitmap = maxPixelWidth <= 0 || maxPixelHeight <= 0 ? JpegUtils.DecodeJpegWithOrientation(stream) : JpegUtils.DecodeJpegWithOrientation(stream, maxPixelWidth, maxPixelHeight);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "bitmap utils, create from stream");
        bitmap = (WriteableBitmap) null;
      }
      return bitmap;
    }

    public static BitmapSource CreateBitmap(byte[] bytes)
    {
      return (BitmapSource) BitmapUtils.CreateBitmap(bytes, 0, 0);
    }

    public static WriteableBitmap CreateBitmap(byte[] bytes, int maxPixelWidth, int maxPixelHeight)
    {
      if (bytes == null)
        return (WriteableBitmap) null;
      using (MemoryStream memoryStream = new MemoryStream(bytes))
        return BitmapUtils.CreateBitmap((Stream) memoryStream, maxPixelWidth, maxPixelHeight);
    }

    public static WriteableBitmap CreateBitmap(
      string base64,
      int maxPixelWidth,
      int maxPixelHeight)
    {
      return BitmapUtils.CreateBitmap(Convert.FromBase64String(base64), maxPixelWidth, maxPixelHeight);
    }

    public static WriteableBitmap LoadFromFile(
      string filepath,
      int maxPixelWidth,
      int maxPixelHeight)
    {
      if (string.IsNullOrEmpty(filepath))
        return (WriteableBitmap) null;
      WriteableBitmap writeableBitmap = (WriteableBitmap) null;
      using (IMediaStorage mediaStorage = MediaStorage.Create(filepath))
      {
        try
        {
          using (Stream stream = mediaStorage.OpenFile(filepath))
            writeableBitmap = BitmapUtils.CreateBitmap(stream, maxPixelWidth, maxPixelHeight);
        }
        catch (Exception ex)
        {
          writeableBitmap = (WriteableBitmap) null;
          if (mediaStorage.FileExists(filepath))
            Log.LogException(ex, "open stored image");
          else
            Log.WriteLineDebug("image store: file doesn't exist | path={0}", (object) filepath);
        }
      }
      return writeableBitmap;
    }
  }
}
