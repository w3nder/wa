// Decompiled with JetBrains decompiler
// Type: WhatsApp.InlineVideoMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WhatsApp
{
  internal class InlineVideoMessageViewPanel : ImageMessageViewPanel
  {
    private static MediaElement MediaElementStatic = (MediaElement) null;
    private static Grid videoCanvasParent = (Grid) null;
    private static Canvas videoCanvas = new Canvas();
    private static Rectangle videoRectangle;
    private static bool MediaElementAttached;
    private static bool VideoRectangleAttached;
    private static bool IsBackgroundMediaPlaying;
    private bool ViewLoaded;
    private Image GifAttribution;

    public InlineVideoMessageViewPanel()
    {
      Image image = new Image();
      image.VerticalAlignment = VerticalAlignment.Bottom;
      image.HorizontalAlignment = HorizontalAlignment.Left;
      image.Margin = new Thickness(4.0, 4.0, 4.0, 4.0);
      image.Width = 61.0;
      image.Height = 20.0;
      this.GifAttribution = image;
      Canvas.SetZIndex((UIElement) this.GifAttribution, 100);
      this.Children.Add((UIElement) this.GifAttribution);
      this.Loaded += new RoutedEventHandler(this.InlineVideoMessageViewPanel_Loaded);
      this.Unloaded += new RoutedEventHandler(this.InlineVideoMessageViewPanel_Unloaded);
    }

    private void InlineVideoMessageViewPanel_Loaded(object sender, RoutedEventArgs e)
    {
      this.ViewLoaded = true;
    }

    private void InlineVideoMessageViewPanel_Unloaded(object sender, RoutedEventArgs e)
    {
      this.ViewLoaded = false;
    }

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.InlineVideo;

    protected override Stretch ThumbnailStretch => Stretch.Uniform;

    public override void Cleanup()
    {
      base.Cleanup();
      if (InlineVideoMessageViewPanel.videoCanvasParent != this)
        return;
      InlineVideoMessageViewPanel.videoCanvas.Visibility = Visibility.Collapsed;
      this.Children.Remove((UIElement) InlineVideoMessageViewPanel.videoCanvas);
      InlineVideoMessageViewPanel.videoCanvasParent = (Grid) null;
      if (InlineVideoMessageViewPanel.MediaElementStatic == null)
        return;
      InlineVideoMessageViewPanel.MediaElementStatic.Stop();
      InlineVideoMessageViewPanel.MediaElementStatic.Source = (Uri) null;
      if (!InlineVideoMessageViewPanel.IsBackgroundMediaPlaying)
        return;
      AudioPlaybackManager.BackgroundMedia.Resume(true);
      InlineVideoMessageViewPanel.IsBackgroundMediaPlaying = false;
    }

    public override void Render(MessageViewModel mvm)
    {
      base.Render(mvm);
      if (!(mvm is InlineVideoMessageViewModel messageViewModel))
        return;
      MessageProperties.MediaProperties.Attribution gifAttribution = messageViewModel.GetGifAttribution();
      if (gifAttribution != MessageProperties.MediaProperties.Attribution.NONE)
        this.GifAttribution.Source = (System.Windows.Media.ImageSource) new BitmapImage()
        {
          CreateOptions = BitmapCreateOptions.None,
          UriSource = new Uri(gifAttribution == MessageProperties.MediaProperties.Attribution.GIPHY ? "/images/giphy.png" : "/images/tenor.png", UriKind.Relative)
        };
      else
        this.GifAttribution.Source = (System.Windows.Media.ImageSource) null;
    }

    protected override void ViewMessage()
    {
      if (!(this.viewModel is InlineVideoMessageViewModel viewModel) || !this.ViewLoaded)
        return;
      if (viewModel.ShouldAttemptDownload)
        WhatsApp.CommonOps.ViewMessage.View(this.viewModel.Message);
      if (!InlineVideoMessageViewPanel.VideoRectangleAttached)
      {
        InlineVideoMessageViewPanel.videoCanvas.Children.Add((UIElement) InlineVideoMessageViewPanel.videoRectangle);
        InlineVideoMessageViewPanel.VideoRectangleAttached = true;
      }
      if (!InlineVideoMessageViewPanel.MediaElementAttached)
      {
        if (InlineVideoMessageViewPanel.MediaElementStatic == null)
        {
          Log.d("msgbubble", string.Format("Created MediaElement from message: {0}", (object) viewModel?.MessageID));
          MediaElement mediaElement = new MediaElement();
          mediaElement.Visibility = Visibility.Collapsed;
          mediaElement.IsMuted = true;
          InlineVideoMessageViewPanel.MediaElementStatic = mediaElement;
          InlineVideoMessageViewPanel.MediaElementStatic.MediaEnded += (RoutedEventHandler) ((s, e) =>
          {
            if (InlineVideoMessageViewPanel.MediaElementStatic == null)
              return;
            this.PlayVideo();
          });
          InlineVideoMessageViewPanel.MediaElementStatic.MediaOpened += (RoutedEventHandler) ((s, e) =>
          {
            if (InlineVideoMessageViewPanel.MediaElementStatic == null)
              return;
            InlineVideoMessageViewPanel.videoCanvas.Visibility = Visibility.Visible;
            this.PlayVideo();
          });
          ((VideoBrush) InlineVideoMessageViewPanel.videoRectangle.Fill).SetSource(InlineVideoMessageViewPanel.MediaElementStatic);
        }
        if (!ChatPageHelper.EnsureCurrentChatPageChild((FrameworkElement) InlineVideoMessageViewPanel.MediaElementStatic))
          return;
        InlineVideoMessageViewPanel.MediaElementAttached = true;
      }
      if (InlineVideoMessageViewPanel.videoCanvasParent != null && InlineVideoMessageViewPanel.videoCanvasParent != this)
      {
        InlineVideoMessageViewPanel.videoCanvas.Visibility = Visibility.Collapsed;
        InlineVideoMessageViewPanel.videoCanvasParent.Children.Remove((UIElement) InlineVideoMessageViewPanel.videoCanvas);
      }
      if (InlineVideoMessageViewPanel.videoCanvasParent != this)
      {
        this.Children.Add((UIElement) InlineVideoMessageViewPanel.videoCanvas);
        InlineVideoMessageViewPanel.videoCanvasParent = (Grid) this;
        if (viewModel.Message.LocalFileExists())
        {
          InlineVideoMessageViewPanel.MediaElementStatic.Source = new Uri(MediaStorage.GetAbsolutePath(viewModel.Message.LocalFileUri), UriKind.Absolute);
          viewModel.GetRotationMatrix().Subscribe<Matrix>(new Action<Matrix>(this.SetRotationMatrix));
        }
        else
          InlineVideoMessageViewPanel.MediaElementStatic.Source = (Uri) null;
      }
      else if (viewModel.Message.LocalFileExists())
      {
        if (InlineVideoMessageViewPanel.MediaElementStatic.Source == (Uri) null)
        {
          InlineVideoMessageViewPanel.MediaElementStatic.Source = new Uri(MediaStorage.GetAbsolutePath(viewModel.Message.LocalFileUri), UriKind.Absolute);
          viewModel.GetRotationMatrix().Subscribe<Matrix>(new Action<Matrix>(this.SetRotationMatrix));
        }
        else
          this.PlayVideo();
      }
      if (AudioPlaybackManager.BackgroundMedia.Stop() != MediaState.Playing)
        return;
      InlineVideoMessageViewPanel.IsBackgroundMediaPlaying = true;
    }

    private void PlayVideo()
    {
      Assert.IsTrue(InlineVideoMessageViewPanel.videoCanvas != null, "Video canvas cannot be null when playing a video");
      Assert.IsTrue(InlineVideoMessageViewPanel.videoRectangle != null, "Video rectangle cannot be null when playing a video");
      Assert.IsTrue(InlineVideoMessageViewPanel.MediaElementStatic != null, "MediaElement cannot be null when playing a video");
      Assert.IsTrue(InlineVideoMessageViewPanel.MediaElementStatic.Source != (Uri) null, "MediaElement must have a source to play video");
      Assert.IsTrue(InlineVideoMessageViewPanel.VideoRectangleAttached, "Video rectangle must be attached to play video");
      Assert.IsTrue(InlineVideoMessageViewPanel.MediaElementAttached, "MediaElement must be attached to play video");
      if (InlineVideoMessageViewPanel.MediaElementStatic.CurrentState != MediaElementState.Paused && InlineVideoMessageViewPanel.MediaElementStatic.CurrentState != MediaElementState.Stopped && InlineVideoMessageViewPanel.MediaElementStatic.CurrentState != MediaElementState.Playing)
      {
        Log.d("msgbubble", string.Format("Returning from playing GIF since MediaElement is not ready to play, State: {0}", (object) InlineVideoMessageViewPanel.MediaElementStatic.CurrentState));
      }
      else
      {
        try
        {
          InlineVideoMessageViewPanel.MediaElementStatic.Play();
        }
        catch (InvalidOperationException ex)
        {
          Log.LogException((Exception) ex, "Called Play on GIF MediaElement");
          throw;
        }
      }
    }

    private void SetRotationMatrix(Matrix m)
    {
      if (!(this.viewModel is InlineVideoMessageViewModel viewModel))
        return;
      CultureInfo cult = new CultureInfo(AppResources.CultureString);
      int angleForMatrix = VideoFrameGrabber.GetAngleForMatrix(m);
      CompositeTransform renderTransform = (CompositeTransform) InlineVideoMessageViewPanel.videoRectangle.RenderTransform;
      renderTransform.Rotation = (double) -angleForMatrix + 0.05;
      switch (angleForMatrix)
      {
        case 90:
          if (!cult.IsRightToLeft())
          {
            renderTransform.CenterX = viewModel.ThumbnailHeight / 2.0;
            renderTransform.CenterY = viewModel.ThumbnailHeight / 2.0;
          }
          else
          {
            renderTransform.CenterX = viewModel.ThumbnailWidth * 5.0 / 6.0;
            renderTransform.CenterY = viewModel.ThumbnailWidth / 2.0;
          }
          InlineVideoMessageViewPanel.videoRectangle.Width = viewModel.ThumbnailHeight;
          InlineVideoMessageViewPanel.videoRectangle.Height = viewModel.ThumbnailWidth;
          break;
        case 180:
          renderTransform.CenterX = viewModel.ThumbnailWidth / 2.0;
          renderTransform.CenterY = viewModel.ThumbnailHeight / 2.0;
          InlineVideoMessageViewPanel.videoRectangle.Width = viewModel.ThumbnailWidth;
          InlineVideoMessageViewPanel.videoRectangle.Height = viewModel.ThumbnailHeight;
          break;
        case 270:
          if (!cult.IsRightToLeft())
          {
            renderTransform.CenterX = viewModel.ThumbnailWidth / 2.0;
            renderTransform.CenterY = viewModel.ThumbnailWidth / 2.0;
          }
          else
          {
            renderTransform.CenterX = viewModel.ThumbnailHeight / 2.0;
            renderTransform.CenterY = viewModel.ThumbnailHeight / 2.0;
          }
          InlineVideoMessageViewPanel.videoRectangle.Width = viewModel.ThumbnailHeight;
          InlineVideoMessageViewPanel.videoRectangle.Height = viewModel.ThumbnailWidth;
          break;
        default:
          InlineVideoMessageViewPanel.videoRectangle.Width = viewModel.ThumbnailWidth;
          InlineVideoMessageViewPanel.videoRectangle.Height = viewModel.ThumbnailHeight;
          break;
      }
    }

    private void ViewMessageAfterDownload()
    {
      if (InlineVideoMessageViewPanel.videoCanvasParent != this)
        return;
      this.ViewMessage();
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      switch (args.Key)
      {
        case "Play":
          this.ViewMessage();
          break;
        case "LocalFileUriChanged":
          this.ViewMessageAfterDownload();
          break;
      }
    }

    public static void OnNavigatedFrom(ChatPage page)
    {
      if (!InlineVideoMessageViewPanel.MediaElementAttached)
        return;
      UIElementCollection children = page.LayoutRoot.Children;
      if (children.Contains((UIElement) InlineVideoMessageViewPanel.MediaElementStatic))
        children.Remove((UIElement) InlineVideoMessageViewPanel.MediaElementStatic);
      InlineVideoMessageViewPanel.MediaElementStatic = (MediaElement) null;
      InlineVideoMessageViewPanel.MediaElementAttached = false;
      if (!InlineVideoMessageViewPanel.IsBackgroundMediaPlaying)
        return;
      AudioPlaybackManager.BackgroundMedia.Resume(true);
      InlineVideoMessageViewPanel.IsBackgroundMediaPlaying = false;
    }

    static InlineVideoMessageViewPanel()
    {
      Rectangle rectangle = new Rectangle();
      rectangle.VerticalAlignment = VerticalAlignment.Stretch;
      rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
      rectangle.Fill = (Brush) new VideoBrush();
      rectangle.FlowDirection = FlowDirection.LeftToRight;
      rectangle.RenderTransform = (Transform) new CompositeTransform()
      {
        Rotation = 0.05
      };
      InlineVideoMessageViewPanel.videoRectangle = rectangle;
      InlineVideoMessageViewPanel.MediaElementAttached = false;
      InlineVideoMessageViewPanel.VideoRectangleAttached = false;
      InlineVideoMessageViewPanel.IsBackgroundMediaPlaying = false;
    }
  }
}
