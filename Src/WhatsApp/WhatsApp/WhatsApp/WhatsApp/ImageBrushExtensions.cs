// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageBrushExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public static class ImageBrushExtensions
  {
    public static readonly DependencyProperty ImageBrushExtensionsStateProperty = DependencyProperty.RegisterAttached("ImageBrushExtensionsState", typeof (ImageBrushExtensions.State), typeof (ImageBrushExtensions.State), new PropertyMetadata((object) null, (PropertyChangedCallback) ((sender, args) => { })));

    public static void SetImageSourceSafe(this ImageBrush brush, System.Windows.Media.ImageSource src)
    {
      if (!(brush.GetValue(ImageBrushExtensions.ImageBrushExtensionsStateProperty) is ImageBrushExtensions.State state))
      {
        state = new ImageBrushExtensions.State();
        brush.SetValue(ImageBrushExtensions.ImageBrushExtensionsStateProperty, (object) state);
      }
      state.SetImageSource(brush, src);
    }

    public static void ClearImageSource(this ImageBrush brush)
    {
      if (brush.GetValue(ImageBrushExtensions.ImageBrushExtensionsStateProperty) is ImageBrushExtensions.State state)
      {
        state.Clear();
        brush.SetValue(ImageBrushExtensions.ImageBrushExtensionsStateProperty, (object) null);
      }
      brush.ImageSource = (System.Windows.Media.ImageSource) null;
    }

    public class State
    {
      private bool brushLoaded = true;
      private System.Windows.Media.ImageSource nextSource;
      private bool hasNextSource;
      private LinkedList<Action> cancelOps = new LinkedList<Action>();

      public void SetImageSource(ImageBrush brush, System.Windows.Media.ImageSource src)
      {
        if (!this.brushLoaded)
        {
          this.nextSource = src;
          this.hasNextSource = true;
        }
        else if (src != null)
        {
          List<Action> dtors = new List<Action>();
          Action cleanup = (Action) (() =>
          {
            dtors.ForEach((Action<Action>) (a => a()));
            dtors.Clear();
            this.brushLoaded = true;
            if (!this.hasNextSource)
              return;
            System.Windows.Media.ImageSource nextSource = this.nextSource;
            this.nextSource = (System.Windows.Media.ImageSource) null;
            this.hasNextSource = false;
            this.SetImageSource(brush, nextSource);
          });
          EventHandler<ExceptionRoutedEventArgs> failedHandler = (EventHandler<ExceptionRoutedEventArgs>) ((sender, args) => cleanup());
          EventHandler<RoutedEventArgs> successHandler = (EventHandler<RoutedEventArgs>) ((sender, args) => cleanup());
          brush.ImageFailed += failedHandler;
          brush.ImageOpened += successHandler;
          dtors.Add((Action) (() => brush.ImageFailed -= failedHandler));
          dtors.Add((Action) (() => brush.ImageOpened -= successHandler));
          LinkedListNode<Action> node = this.cancelOps.AddLast(cleanup);
          dtors.Add((Action) (() => this.cancelOps.Remove(node)));
          if (src is BitmapImage bitmapImage)
            src = (System.Windows.Media.ImageSource) new BitmapImage(bitmapImage.UriSource);
          this.brushLoaded = false;
          brush.ImageSource = src;
        }
        else
          brush.ImageSource = src;
      }

      public void Clear()
      {
        this.hasNextSource = false;
        this.nextSource = (System.Windows.Media.ImageSource) null;
        while (this.cancelOps.Any<Action>())
        {
          foreach (Action action in this.cancelOps.AsRemoveSafeEnumerator<Action>())
            action();
        }
      }
    }
  }
}
