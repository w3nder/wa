// Decompiled with JetBrains decompiler
// Type: WhatsApp.ResolutionHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Info;
using System;
using System.Windows;
using Windows.Graphics.Display;

#nullable disable
namespace WhatsApp
{
  public class ResolutionHelper
  {
    private static Settings.ScreenResolutionKind resolution;
    private static bool fetchResolutionAttempted;
    private static int? scaleFactor;
    private static Size? renderSize_;
    private static Size? actualSize_;
    private static ResolutionHelper.ResolutionPlateau? plateau;

    public static void Init()
    {
      Log.l("resolution: {0}", (object) ResolutionHelper.GetResolution());
    }

    public static Settings.ScreenResolutionKind GetResolution()
    {
      if (ResolutionHelper.resolution == Settings.ScreenResolutionKind.Undefined)
      {
        ResolutionHelper.resolution = Settings.ScreenResolution;
        if (ResolutionHelper.resolution == Settings.ScreenResolutionKind.Undefined && !ResolutionHelper.fetchResolutionAttempted)
        {
          ResolutionHelper.fetchResolutionAttempted = true;
          switch (ResolutionHelper.GetScaleFactor())
          {
            case 100:
              ResolutionHelper.resolution = Settings.ScreenResolutionKind.WVGA;
              break;
            case 112:
              ResolutionHelper.resolution = Settings.ScreenResolutionKind.HD720p;
              break;
            case 150:
              ResolutionHelper.resolution = Settings.ScreenResolutionKind.HD720p;
              break;
            case 160:
              ResolutionHelper.resolution = Settings.ScreenResolutionKind.WXGA;
              break;
            case 225:
              ResolutionHelper.resolution = Settings.ScreenResolutionKind.HD1080p;
              break;
          }
          if (ResolutionHelper.resolution != Settings.ScreenResolutionKind.Undefined)
            Settings.ScreenResolution = ResolutionHelper.resolution;
        }
      }
      if (ResolutionHelper.resolution == Settings.ScreenResolutionKind.Undefined)
      {
        Log.d("UIUtils EmojiLog", "ScreenResolutionKind=WVGA");
        return Settings.ScreenResolutionKind.WVGA;
      }
      Log.d("UIUtils EmojiLog", "ScreenResolutionKind={0}", (object) ResolutionHelper.resolution);
      return ResolutionHelper.resolution;
    }

    public static int GetScaleFactor()
    {
      if (ResolutionHelper.scaleFactor.HasValue)
      {
        Log.d("UIUtils EmojiLog", "ScaleFactor={0}", (object) ResolutionHelper.scaleFactor.Value);
        return ResolutionHelper.scaleFactor.Value;
      }
      if (AppState.IsWP10OrLater)
      {
        ResolutionScale resolutionScale = DisplayProperties.ResolutionScale;
        if (resolutionScale <= 150)
        {
          if (resolutionScale != 100)
          {
            if (resolutionScale == 150)
              ResolutionHelper.scaleFactor = new int?(150);
          }
          else
            ResolutionHelper.scaleFactor = new int?(100);
        }
        else if (resolutionScale != 160)
        {
          if (resolutionScale == 225)
            ResolutionHelper.scaleFactor = new int?(225);
        }
        else
          ResolutionHelper.scaleFactor = new int?(160);
      }
      if (!ResolutionHelper.scaleFactor.HasValue)
      {
        object propertyValue;
        if (DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out propertyValue))
          ResolutionHelper.scaleFactor = new int?((int) (((Size) propertyValue).Width / 4.8));
        else
          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => ResolutionHelper.scaleFactor = new int?(Application.Current.Host.Content.ScaleFactor)));
      }
      Log.d("UIUtils EmojiLog", "ScaleFactor={0}", (object) ResolutionHelper.scaleFactor.Value);
      return ResolutionHelper.scaleFactor.Value;
    }

    public static Size GetPixelSize()
    {
      switch (ResolutionHelper.GetResolution())
      {
        case Settings.ScreenResolutionKind.WXGA:
          return new Size(768.0, 1280.0);
        case Settings.ScreenResolutionKind.HD720p:
          return new Size(720.0, 1280.0);
        case Settings.ScreenResolutionKind.HD1080p:
          return new Size(1080.0, 1920.0);
        default:
          return new Size(480.0, 800.0);
      }
    }

    public static Size GetRenderSize()
    {
      if (!ResolutionHelper.renderSize_.HasValue || ResolutionHelper.renderSize_.Value.Height == 0.0)
        ResolutionHelper.renderSize_ = new Size?(Application.Current.RootVisual.RenderSize);
      if (!ResolutionHelper.renderSize_.HasValue || ResolutionHelper.renderSize_.Value.Height == 0.0)
      {
        if (!ResolutionHelper.actualSize_.HasValue || ResolutionHelper.actualSize_.Value.Height == 0.0)
        {
          ResolutionHelper.actualSize_ = new Size?(new Size(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight));
          object[] objArray = new object[2];
          Size size = ResolutionHelper.actualSize_.Value;
          objArray[0] = (object) size.Width;
          size = ResolutionHelper.actualSize_.Value;
          objArray[1] = (object) size.Height;
          Log.l("UiUtils", "GetRenderSize using actual values {0}x{1}", objArray);
        }
        return ResolutionHelper.actualSize_.Value;
      }
      if (ResolutionHelper.actualSize_.HasValue)
      {
        object[] objArray = new object[4];
        Size size = ResolutionHelper.actualSize_.Value;
        objArray[0] = (object) size.Width;
        size = ResolutionHelper.actualSize_.Value;
        objArray[1] = (object) size.Height;
        size = ResolutionHelper.renderSize_.Value;
        objArray[2] = (object) size.Width;
        size = ResolutionHelper.renderSize_.Value;
        objArray[3] = (object) size.Height;
        Log.l("UiUtils", "GetRenderSize was using actual values ({0}x{1}), now {2}x{3}", objArray);
        ResolutionHelper.actualSize_ = new Size?();
      }
      return ResolutionHelper.renderSize_.Value;
    }

    public static double GetDPI()
    {
      object propertyValue;
      return !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue) || (double) propertyValue == 0.0 ? -1.0 : (double) propertyValue;
    }

    public static Size GetPhysicalScreenSize()
    {
      double dpi = ResolutionHelper.GetDPI();
      if (dpi == -1.0)
        return Size.Empty;
      object propertyValue;
      Size size = !DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out propertyValue) ? ResolutionHelper.GetPixelSize() : (Size) propertyValue;
      return new Size(size.Width / dpi, size.Height / dpi);
    }

    public static ResolutionHelper.ResolutionPlateau Plateau
    {
      get
      {
        if (ResolutionHelper.plateau.HasValue)
          return ResolutionHelper.plateau.Value;
        Size physicalScreenSize = ResolutionHelper.GetPhysicalScreenSize();
        Settings.ScreenResolutionKind resolution = ResolutionHelper.GetResolution();
        if (physicalScreenSize == Size.Empty)
        {
          switch (resolution)
          {
            case Settings.ScreenResolutionKind.WXGA:
            case Settings.ScreenResolutionKind.HD720p:
            case Settings.ScreenResolutionKind.HD1080p:
              ResolutionHelper.plateau = new ResolutionHelper.ResolutionPlateau?(ResolutionHelper.ResolutionPlateau.S20);
              break;
          }
        }
        else
        {
          double num = physicalScreenSize.Width * physicalScreenSize.Width + physicalScreenSize.Height * physicalScreenSize.Height;
          if (num > 29.5)
            ResolutionHelper.plateau = new ResolutionHelper.ResolutionPlateau?(ResolutionHelper.ResolutionPlateau.S30);
          else if (num > 19.5)
            ResolutionHelper.plateau = new ResolutionHelper.ResolutionPlateau?(ResolutionHelper.ResolutionPlateau.S20);
        }
        return ResolutionHelper.plateau ?? (ResolutionHelper.plateau = new ResolutionHelper.ResolutionPlateau?(ResolutionHelper.ResolutionPlateau.S10)).Value;
      }
    }

    public static bool ShouldEnableZoomBox
    {
      get
      {
        return ResolutionHelper.Plateau == ResolutionHelper.ResolutionPlateau.S20 || ResolutionHelper.Plateau == ResolutionHelper.ResolutionPlateau.S30;
      }
    }

    public static double ZoomMultiplier
    {
      get
      {
        switch (ResolutionHelper.Plateau)
        {
          case ResolutionHelper.ResolutionPlateau.S20:
            return 0.93;
          case ResolutionHelper.ResolutionPlateau.S30:
            return 0.78;
          default:
            return 1.0;
        }
      }
    }

    public static double ZoomFactor => 1.0 / ResolutionHelper.ZoomMultiplier;

    public static Thickness ZoomedPivotHeaderMargin
    {
      get => new Thickness(24.0 * ResolutionHelper.ZoomMultiplier - 24.0, 0.0, 0.0, 0.0);
    }

    public enum ResolutionPlateau
    {
      S10,
      S20,
      S30,
    }
  }
}
