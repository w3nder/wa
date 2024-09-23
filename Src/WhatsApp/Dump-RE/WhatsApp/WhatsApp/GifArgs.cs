// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifArgs
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class GifArgs
  {
    private VideoFrameGrabber gifFrameGrabber;

    public Queue<WriteableBitmap> GifFrames { get; set; }

    public VideoFrameGrabber GifFrameGrabber
    {
      get => this.gifFrameGrabber;
      set
      {
        this.gifFrameGrabber = value;
        if (this.gifFrameGrabber == null)
          return;
        this.Duration = this.GifFrameGrabber.DurationTicks;
      }
    }

    public long Duration { get; set; }

    public WhatsApp.TimeCrop? TimeCrop { get; set; }

    public MessageProperties.MediaProperties.Attribution GifAttribution { get; set; }

    ~GifArgs() => this.GifFrameGrabber.SafeDispose();
  }
}
