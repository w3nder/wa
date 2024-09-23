// Decompiled with JetBrains decompiler
// Type: WhatsApp.AttributionControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class AttributionControl : StackPanel
  {
    private List<WebServices.Attribution> attributions = new List<WebServices.Attribution>();

    public void Add(WebServices.Attribution attrib)
    {
      if (attrib == null)
        return;
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.Add(attrib)));
      }
      else
      {
        foreach (WebServices.Attribution attribution in this.attributions)
        {
          if (attribution == attrib)
            return;
        }
        this.attributions.Add(attrib);
        BitmapImage logo = attrib.Logo;
        if (logo != null)
        {
          Image image = new Image();
          image.Source = (System.Windows.Media.ImageSource) logo;
          image.Width = (double) logo.PixelWidth * attrib.LogoScaleFactor;
          image.Height = (double) logo.PixelHeight * attrib.LogoScaleFactor;
          this.Children.Add((UIElement) image);
        }
        else
        {
          if (attrib.Text == null)
            return;
          this.Children.Add((UIElement) new TextBlock()
          {
            Text = attrib.Text
          });
        }
      }
    }

    public void Clear()
    {
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        this.attributions.Clear();
        this.Children.Clear();
      }));
    }
  }
}
