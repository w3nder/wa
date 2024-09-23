// Decompiled with JetBrains decompiler
// Type: WhatsApp.WebpUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class WebpUtils
  {
    private static readonly IWebp decoder = NativeInterfaces.WebP;

    public static WebpUtils.WebpImage DecodeWebp(Stream stream, bool decodeAnimation = false)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      if (stream == null)
        return (WebpUtils.WebpImage) null;
      WebpUtils.WebpImage webpImage = new WebpUtils.WebpImage();
      List<WebpUtils.ImageFrame> imageFrameList = new List<WebpUtils.ImageFrame>();
      webpImage.Frames = imageFrameList;
      int length = (int) stream.Length;
      try
      {
        byte[] numArray = new byte[length];
        stream.Seek(0L, SeekOrigin.Begin);
        stream.Read(numArray, 0, length);
        IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance1.Put(numArray, 0, numArray.Length);
        byte[] bytes = new byte[WebpUtils.decoder.Initialise(instance1)];
        IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance2.Put(bytes, 0, bytes.Length);
        webpImage.Width = WebpUtils.decoder.GetCanvasWidth();
        webpImage.Height = WebpUtils.decoder.GetCanvasHeight();
        webpImage.LoopCount = WebpUtils.decoder.GetLoopCount();
        webpImage.FrameCount = WebpUtils.decoder.GetFrameCount();
        webpImage.BgColor = WebpUtils.decoder.GetBgColor();
        uint num1 = decodeAnimation ? webpImage.FrameCount : 1U;
        for (int index = 0; (long) index < (long) num1; ++index)
        {
          int num2 = WebpUtils.decoder.Decode(instance2);
          WriteableBitmap bmp = new WriteableBitmap((int) webpImage.Width, (int) webpImage.Height);
          imageFrameList.Add(new WebpUtils.ImageFrame()
          {
            Image = (ImageSource) bmp.FromByteArray(instance2.Get()),
            FrameLengthMillis = num2
          });
        }
        WebpUtils.decoder.Dispose();
        TimeSpan timeSpan = DateTimeOffset.UtcNow.Subtract(utcNow);
        Log.d("WebpDecode", "Image Info: width={0}, height={1}, loopCount={2}, frameCount={3} | decode time={4}ms", (object) webpImage.Width, (object) webpImage.Height, (object) webpImage.LoopCount, (object) webpImage.FrameCount, (object) timeSpan.Milliseconds);
        return webpImage;
      }
      catch (Exception ex)
      {
        Log.d(ex, "decode webp");
        throw;
      }
    }

    public static void AnimateOn(this WebpUtils.WebpImage image, Storyboard stickerStoryboard)
    {
      ObjectAnimationUsingKeyFrames element = new ObjectAnimationUsingKeyFrames();
      Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Source", new object[0]));
      if ((long) image.Frames.Count != (long) image.FrameCount)
      {
        Log.l("Webp DecodeAnimation", "FrameCount not equal to Frames count: FrameCount={0}, Frames.Count={1}", (object) image.FrameCount, (object) image.Frames.Count);
      }
      else
      {
        foreach (WebpUtils.ImageFrame frame in image.Frames)
        {
          DiscreteObjectKeyFrame discreteObjectKeyFrame1 = new DiscreteObjectKeyFrame();
          discreteObjectKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((double) frame.FrameLengthMillis));
          discreteObjectKeyFrame1.Value = (object) frame.Image;
          DiscreteObjectKeyFrame discreteObjectKeyFrame2 = discreteObjectKeyFrame1;
          element.KeyFrames.Add((ObjectKeyFrame) discreteObjectKeyFrame2);
        }
        stickerStoryboard.RepeatBehavior = image.LoopCount == 0U ? RepeatBehavior.Forever : new RepeatBehavior((double) image.LoopCount);
        stickerStoryboard.Children.Clear();
        stickerStoryboard.Children.Add((Timeline) element);
      }
    }

    public enum DecoderStrategy
    {
      Simple,
      Config,
      Animation,
    }

    public class ImageFrame
    {
      public ImageSource Image;
      public int FrameLengthMillis;
    }

    public class WebpImage
    {
      public List<WebpUtils.ImageFrame> Frames;
      public uint Width;
      public uint Height;
      public uint LoopCount;
      public uint FrameCount;
      public uint BgColor;
    }
  }
}
