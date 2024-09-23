// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatusViewControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WhatsApp.CommonOps;
using WhatsApp.Streaming;
using WhatsApp.UtilsFrontend;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class WaStatusViewControl : 
    UserControl,
    IDisposable,
    IVideoPlaybackDownloadCallbacks,
    IVideoStreamSourceBufferingCallbacks,
    IVideoStreamSourceErrorCallbacks
  {
    public const int StatusThumbDecodeEdge = 800;
    public const int StatusImageDecodeEdge = 800;
    private const double InitialFitTolerance = 0.15;
    private const int MaxLinkRenderLength = 18;
    private const double DefaultViewTime = 4.5;
    private const int MinGifViewTime = 6;
    private const int MinGifLoops = 3;
    private WaStatusThread statusThread;
    private WaStatus[] threadStatuses;
    private WaStatus currentStatus;
    private bool isCurrentStatusRendered;
    private MessageViewModel currentStatusMsgVm;
    private int currentStatusIndex = -1;
    private int? finishedGifLoops;
    private int remainingTimeout;
    private bool startFromUnviewed;
    private LinkPreviewPanel linkPreviewPanel;
    private StatusViewProgressBar progressBar;
    private Image verifiedIcon;
    private IDisposable profilePicSub;
    private IDisposable contentSub;
    private IDisposable statusesFetchSub;
    private IDisposable timerSub;
    private IDisposable sbSub;
    private IDisposable mediaTypeChangeSub;
    private Subject<int> exitSubj = new Subject<int>();
    private Subject<Unit> flickedUpSubj = new Subject<Unit>();
    private Subject<Pair<WaStatus, bool>> statusLoadedSubj = new Subject<Pair<WaStatus, bool>>();
    private Subject<bool> linkPreviewShownSubj = new Subject<bool>();
    private Subject<bool> playbackStateSubj = new Subject<bool>();
    private bool isPausedByTouch;
    private bool isPausedByOther;
    private bool isSoundMuted;
    private VideoPlayback videoPlayback;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    internal Image ImageView;
    internal Canvas VideoViewCanvas;
    internal MediaElement VideoView;
    internal ProgressBar LoadingProgress;
    internal RichTextBlock CenterTextBlock;
    internal ZoomBox HeaderPanelZoomBox;
    internal Grid HeaderPanel;
    internal Rectangle HeaderPanelGradientPanel;
    internal Image ProfilePicture;
    internal StackPanel NamePanel;
    internal TextBlock NameBlock;
    internal TextBlock InfoBlock;
    internal Button MuteButton;
    internal Image MuteButtonIcon;
    internal Rectangle LinkPreviewMask;
    internal ZoomBox BottomPanelZoomBox;
    internal Grid CaptionPanel;
    internal RichTextBlock CaptionBlock;
    internal Grid LinkPreviewContainer;
    private bool _contentLoaded;

    public MessageViewModel CurrentStatusMessageViewModel => this.currentStatusMsgVm;

    private bool IsPaused => this.isPausedByTouch || this.isPausedByOther;

    public bool IsSoundMuted => this.isSoundMuted;

    public WaStatusViewControl()
    {
      this.InitializeComponent();
      this.HeaderPanelZoomBox.ZoomFactor = this.BottomPanelZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.ContentPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ContentPanel_ManipulationStarted);
      this.ContentPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.ContentPanel_ManipulationDelta);
      this.ContentPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.ContentPanel_ManipulationCompleted);
    }

    public void Dispose()
    {
      if (this.progressBar != null)
        this.progressBar.Stop();
      this.profilePicSub.SafeDispose();
      this.profilePicSub = (IDisposable) null;
      this.contentSub.SafeDispose();
      this.contentSub = (IDisposable) null;
      this.statusesFetchSub.SafeDispose();
      this.statusesFetchSub = (IDisposable) null;
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.sbSub.SafeDispose();
      this.sbSub = (IDisposable) null;
      this.videoPlayback.SafeDispose();
      this.videoPlayback = (VideoPlayback) null;
    }

    public IObservable<int> ExitObservable() => (IObservable<int>) this.exitSubj;

    public IObservable<Unit> FlickedUpObservable() => (IObservable<Unit>) this.flickedUpSubj;

    public IObservable<Pair<WaStatus, bool>> StatusLoadedObservable()
    {
      return (IObservable<Pair<WaStatus, bool>>) this.statusLoadedSubj;
    }

    public IObservable<bool> LinkPreviewShownObservable()
    {
      return (IObservable<bool>) this.linkPreviewShownSubj;
    }

    public IObservable<bool> PlaybackStateObservable()
    {
      return (IObservable<bool>) this.playbackStateSubj;
    }

    public void RenderStatusThread(WaStatusThread thread, bool startFromUnviewed, bool muteSound)
    {
      if ((this.statusThread = thread) == null)
      {
        this.exitSubj.OnNext(0);
      }
      else
      {
        this.startFromUnviewed = startFromUnviewed;
        this.isSoundMuted = muteSound;
        string jid = thread.Jid;
        this.profilePicSub.SafeDispose();
        this.ProfilePicture.Source = (System.Windows.Media.ImageSource) null;
        this.profilePicSub = ChatPictureStore.Get(jid, false, false, true, ChatPictureStore.SubMode.GetCurrent).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
        {
          this.profilePicSub.SafeDispose();
          this.profilePicSub = (IDisposable) null;
          BitmapSource image = picState?.Image;
          this.ProfilePicture.Source = (System.Windows.Media.ImageSource) image;
          this.ProfilePicture.Visibility = (image != null).ToVisibility();
        }));
        this.NameBlock.Text = JidHelper.GetDisplayNameForContactJid(jid);
        this.statusesFetchSub.SafeDispose();
        this.statusesFetchSub = thread.LoadThreadAsync(false, WaStatus.Expiration).SubscribeOn<WaStatus[]>((IScheduler) AppState.Worker).ObserveOnDispatcher<WaStatus[]>().Subscribe<WaStatus[]>((Action<WaStatus[]>) (statuses =>
        {
          if (statuses == null || !((IEnumerable<WaStatus>) statuses).Any<WaStatus>())
          {
            WaStatus[] waStatusArray;
            if (thread.LatestStatus != null)
              waStatusArray = new WaStatus[1]
              {
                thread.LatestStatus
              };
            else
              waStatusArray = new WaStatus[0];
            statuses = waStatusArray;
          }
          Log.l("statusv3", "render thread | jid:{0},statuses:{1}", (object) thread.Jid, (object) statuses.Length);
          if (this.progressBar != null)
            this.HeaderPanel.Children.Remove((UIElement) this.progressBar);
          if (((IEnumerable<WaStatus>) statuses).Any<WaStatus>())
          {
            this.threadStatuses = statuses;
            double num = 8.0 * ResolutionHelper.ZoomMultiplier;
            this.progressBar = new StatusViewProgressBar(statuses.Length)
            {
              VerticalAlignment = VerticalAlignment.Top,
              Margin = new Thickness(num, num, num, 0.0),
              CacheMode = (CacheMode) new BitmapCache()
            };
            this.HeaderPanel.Children.Add((UIElement) this.progressBar);
            int i = 0;
            if (startFromUnviewed)
            {
              i = ((IEnumerable<WaStatus>) statuses).Count<WaStatus>((Func<WaStatus, bool>) (s => s.IsViewed));
              if (i >= statuses.Length)
                i = statuses.Length - 1;
            }
            if (this.RenderStatus(i))
              return;
            this.exitSubj.OnNext(0);
          }
          else
            this.exitSubj.OnNext(0);
        }), (Action) (() =>
        {
          this.statusesFetchSub.SafeDispose();
          this.statusesFetchSub = (IDisposable) null;
        }));
      }
    }

    private bool RenderStatus(int i)
    {
      if (this.currentStatusIndex == i)
        return true;
      WaStatus[] threadStatuses = this.threadStatuses;
      WaStatus status = threadStatuses != null ? ((IEnumerable<WaStatus>) threadStatuses).ElementAtOrDefault<WaStatus>(i) : (WaStatus) null;
      this.currentStatusIndex = status == null ? -1 : i;
      if (this.currentStatusIndex >= 0 && this.progressBar != null)
        this.progressBar.FillTill(this.currentStatusIndex);
      return this.RenderStatus(status);
    }

    private bool RenderStatus(WaStatus status)
    {
      Log.d("statusv3", "Rendering status in WaStatusViewControl");
      this.ShowHeaderPanel(true, true);
      this.HeaderPanelGradientPanel.Opacity = 1.0;
      this.ContentPanel.Background = (Brush) UIUtils.BlackBrush;
      this.CenterTextBlock.LinkBackground = (Brush) null;
      this.ImageView.Source = (System.Windows.Media.ImageSource) null;
      this.ImageView.Opacity = 0.0;
      this.contentSub.SafeDispose();
      this.contentSub = (IDisposable) null;
      this.VideoView.Stop();
      this.VideoView.Source = (Uri) null;
      this.VideoViewCanvas.Opacity = 0.0;
      this.CenterTextBlock.Visibility = Visibility.Collapsed;
      this.CenterTextBlock.FontSize = (double) Application.Current.Resources[(object) "PhoneFontSizeNormal"] * ResolutionHelper.ZoomMultiplier;
      this.CenterTextBlock.FontFamily = UIUtils.FontFamilyNormal;
      this.LoadingProgress.Visibility = Visibility.Collapsed;
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.mediaTypeChangeSub.SafeDispose();
      this.mediaTypeChangeSub = (IDisposable) null;
      this.InfoBlock.Text = "";
      this.currentStatus = status;
      this.isCurrentStatusRendered = false;
      this.isPausedByOther = this.isPausedByTouch = false;
      this.videoPlayback.SafeDispose();
      this.videoPlayback = (VideoPlayback) null;
      this.VideoView.IsMuted = true;
      this.MuteButton.Visibility = Visibility.Collapsed;
      this.currentStatusMsgVm = (MessageViewModel) null;
      if (status == null)
        return false;
      Message m = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => m = db.GetMessageById(status.MessageId)));
      MessageViewModel msgVm = MessageViewModel.Create(m);
      if (msgVm?.Message == null)
        return false;
      this.currentStatusMsgVm = msgVm;
      this.mediaTypeChangeSub = m.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (p => p.PropertyName == "MediaWaType")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ => this.RenderStatus(status)));
      if (JidHelper.IsPsaJid(status.Jid))
      {
        if (this.verifiedIcon == null)
        {
          double resource = (double) Application.Current.Resources[(object) "PhoneFontSizeLarge"];
          Image image = new Image();
          image.Source = (System.Windows.Media.ImageSource) AssetStore.InlineVerified;
          image.Margin = new Thickness(6.0, 0.0, 0.0, 0.0);
          image.HorizontalAlignment = HorizontalAlignment.Left;
          image.VerticalAlignment = VerticalAlignment.Center;
          image.Width = resource;
          image.Height = resource;
          this.verifiedIcon = image;
          this.NamePanel.Children.Add((UIElement) this.verifiedIcon);
        }
        this.verifiedIcon.Visibility = Visibility.Visible;
      }
      else
      {
        if (this.verifiedIcon != null)
          this.verifiedIcon.Visibility = Visibility.Collapsed;
        this.InfoBlock.Text = DateTimeUtils.FormatLastSeen(DateTimeUtils.FunTimeToPhoneTime(status.Timestamp));
      }
      if (string.IsNullOrEmpty(m.MediaCaption) || m.IsTextStatus())
      {
        this.CaptionPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.CaptionBlock.Text = new RichTextBlock.TextSet()
        {
          Text = m.MediaCaption
        };
        this.CaptionPanel.Visibility = Visibility.Visible;
      }
      this.finishedGifLoops = new int?();
      bool second = false;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
          this.RenderAsImage(msgVm);
          second = true;
          break;
        case FunXMPP.FMessage.Type.Video:
          this.RenderAsVideo(msgVm);
          second = true;
          break;
        case FunXMPP.FMessage.Type.ExtendedText:
          this.RenderAsText(msgVm);
          second = true;
          break;
        case FunXMPP.FMessage.Type.Gif:
          this.finishedGifLoops = new int?(0);
          this.RenderAsVideo(msgVm);
          second = true;
          break;
        case FunXMPP.FMessage.Type.CipherText:
          this.RenderAsUndecrypted();
          break;
        default:
          this.RenderAsUnsupported();
          break;
      }
      this.statusLoadedSubj.OnNext(new Pair<WaStatus, bool>(status, second));
      wam_enum_status_view_post_origin origin = this.startFromUnviewed ? wam_enum_status_view_post_origin.RECENT_STORIES : wam_enum_status_view_post_origin.PREVIOUS_STORIES;
      if (JidHelper.IsSelfJid(status.Jid))
        origin = wam_enum_status_view_post_origin.MY_STATUS;
      FieldStats.ReportFsStatusViewEvent(m.MediaWaType, origin, wam_enum_status_view_post_result.OK);
      return true;
    }

    private int GetViewTime(MessageViewModel msgVm)
    {
      string str = msgVm.Message.Data ?? msgVm.Message.MediaCaption;
      int length = str != null ? str.Length : 0;
      return (int) (0.5 + 4.5 * (length < 89 ? 1.0 : Math.Sqrt((double) (length / 89))));
    }

    private void RenderAsText(MessageViewModel msgVm)
    {
      double maxHeight = ResolutionHelper.GetRenderSize().Height - 168.0;
      if (this.ContentPanel.Children.Contains((UIElement) this.CenterTextBlock))
      {
        this.ContentPanel.Children.Remove((UIElement) this.CenterTextBlock);
        AdaptiveRichTextBlockWrapper textBlockWrapper = new AdaptiveRichTextBlockWrapper(this.CenterTextBlock);
        textBlockWrapper.CacheMode = (CacheMode) new BitmapCache();
        textBlockWrapper.Margin = new Thickness(0.0);
        textBlockWrapper.MaxHeight = maxHeight;
        textBlockWrapper.VerticalAlignment = VerticalAlignment.Center;
        textBlockWrapper.HorizontalAlignment = HorizontalAlignment.Center;
        this.ContentPanel.Children.Add((UIElement) textBlockWrapper);
      }
      this.HeaderPanelGradientPanel.Opacity = 0.0;
      uint? backgroundArgb = (uint?) msgVm.Message.InternalProperties?.ExtendedTextPropertiesField?.BackgroundArgb;
      Color? nullable = new Color?();
      if (backgroundArgb.HasValue)
      {
        nullable = new Color?(Color.FromArgb((byte) (backgroundArgb.Value >> 24 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 16 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 8 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value & (uint) byte.MaxValue)));
        this.ContentPanel.Background = (Brush) new SolidColorBrush(nullable.Value);
      }
      string textForDisplay = msgVm.Message.GetTextForDisplay();
      double initialFontSize = 72.0 * ResolutionHelper.ZoomMultiplier;
      int length = textForDisplay.Length;
      if (length > 200)
        initialFontSize = 18.0 * ResolutionHelper.ZoomMultiplier;
      else if (length > 50)
        initialFontSize = 36.0 * ResolutionHelper.ZoomMultiplier;
      WaStatusViewControl.RenderTextStatus(this.CenterTextBlock, msgVm.Message, textForDisplay, nullable ?? UIUtils.BackgroundBrush.Color, initialFontSize, maxHeight, (Action<Message>) (msg => this.ShowLinkPreview(msg)), (Action<string>) (linkStr => this.ShowLinkPreview(new WebPageMetadata()
      {
        OriginalUrl = linkStr,
        CanonicalUrl = linkStr
      })));
      this.CenterTextBlock.Visibility = Visibility.Visible;
      this.MarkAsSeen(msgVm.Message);
      int viewTime = this.GetViewTime(msgVm);
      this.StartProgress(viewTime);
      this.StartTimer(viewTime * 1000);
      this.isCurrentStatusRendered = true;
    }

    public static string ShrinkLink(string linkStr)
    {
      string str = (string) null;
      try
      {
        str = new Uri(linkStr).Host;
      }
      catch (Exception ex)
      {
      }
      if (linkStr.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) || linkStr.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
      {
        int startIndex = linkStr.IndexOf("//") + 2;
        linkStr = linkStr.Substring(startIndex);
      }
      int length = 18;
      if (!string.IsNullOrEmpty(str))
      {
        int num = linkStr.IndexOf(str);
        if (num >= 0)
          length = num + str.Length + 3;
      }
      if (linkStr.Length > length)
        linkStr = string.Format("{0}…", (object) linkStr.Substring(0, length));
      return linkStr;
    }

    public static void RenderTextStatus(
      RichTextBlock richTextBlock,
      Message msg,
      string renderStr,
      Color bgColor,
      double initialFontSize,
      double maxHeight,
      Action<Message> onLinkPreviewTap,
      Action<string> onRegularLinkTap)
    {
      Color linkBackgroundColor = WaStatusHelper.GetLinkBackgroundColor(bgColor);
      richTextBlock.LinkBackground = (Brush) new SolidColorBrush(linkBackgroundColor);
      FontFamily fontFamily = (FontFamily) null;
      int? font = (int?) msg.InternalProperties?.ExtendedTextPropertiesField?.Font;
      if (font.HasValue)
      {
        switch (font.Value)
        {
          case 0:
            fontFamily = new FontFamily("MS Sans Serif");
            break;
          case 1:
            fontFamily = new FontFamily("MS Serif");
            break;
          case 2:
            fontFamily = Application.Current.Resources[(object) "Norican"] as FontFamily;
            break;
          case 3:
            fontFamily = Application.Current.Resources[(object) "BryndanWrite"] as FontFamily;
            break;
          case 5:
            fontFamily = Application.Current.Resources[(object) "Oswald"] as FontFamily;
            break;
        }
        richTextBlock.FontFamily = fontFamily ?? UIUtils.FontFamilyNormal;
      }
      IEnumerable<WaRichText.Chunk> chunks = (IEnumerable<WaRichText.Chunk>) null;
      UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(msg);
      string matchedText = uriMessageWrapper.MatchedText;
      if (string.IsNullOrEmpty(matchedText))
      {
        LinkDetector.Result result = LinkDetector.GetMatches(renderStr, new WaRichText.DetectionArgs(WaRichText.Formats.Link)).FirstOrDefault<LinkDetector.Result>((Func<LinkDetector.Result, bool>) (m => m.type == 1));
        string linkStr = result?.Value;
        if (!string.IsNullOrEmpty(linkStr))
        {
          string renderLinkText = WaStatusViewControl.ShrinkLink(linkStr);
          if (result.Index > 0 && renderStr.Substring(result.Index - 1, 1) != "\n")
          {
            renderStr = renderStr.Substring(0, result.Index) + "\n" + renderStr.Substring(result.Index);
            ++result.Index;
          }
          chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            new WaRichText.Chunk(result.Index, result.Length, WaRichText.Formats.Link, linkStr, (Func<string>) (() => renderLinkText), (Action) (() => onRegularLinkTap(linkStr)))
          };
        }
      }
      else
      {
        int num = renderStr.IndexOf(matchedText);
        if (num >= 0)
        {
          string renderLinkText = WaStatusViewControl.ShrinkLink(matchedText);
          if (num > 0 && renderStr.Substring(num - 1, 1) != "\n")
          {
            renderStr = renderStr.Substring(0, num) + "\n" + renderStr.Substring(num);
            ++num;
          }
          chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            new WaRichText.Chunk(num, matchedText.Length, WaRichText.Formats.Link, uriMessageWrapper.CanonicalUrl ?? matchedText, (Func<string>) (() => renderLinkText), (Action) (() => onLinkPreviewTap(msg)))
          };
        }
      }
      WaRichText.Formats applicableFormats = WaRichText.Formats.TextFormattings | WaRichText.Formats.Emoji;
      IEnumerable<LinkDetector.Result> matches = LinkDetector.GetMatches(renderStr, new WaRichText.DetectionArgs(applicableFormats));
      richTextBlock.FontSize = initialFontSize;
      richTextBlock.Text = new RichTextBlock.TextSet()
      {
        Text = renderStr,
        SerializedFormatting = matches,
        PartialFormattings = chunks
      };
    }

    private void ShowHeaderPanel(bool show, bool skipAnimation = false)
    {
      if (skipAnimation)
      {
        this.HeaderPanelZoomBox.Opacity = show ? 1.0 : 0.0;
      }
      else
      {
        Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(show ? WaAnimations.FadeType.FadeIn : WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(show ? 150.0 : 250.0), (DependencyObject) this.HeaderPanelZoomBox));
        this.sbSub.SafeDispose();
        this.sbSub = Storyboarder.PerformWithDisposable(storyboard, (DependencyObject) null, false, (Action) (() =>
        {
          this.sbSub = (IDisposable) null;
          this.HeaderPanelZoomBox.Opacity = show ? 1.0 : 0.0;
        }), (Action) null, "toggle header panel");
      }
    }

    private void ShowLinkPreview(Message m)
    {
      UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(m);
      if (string.IsNullOrEmpty(uriMessageWrapper.MatchedText))
        return;
      BitmapSource thumbnail = m.GetThumbnail(MessageExtensions.ThumbPreference.OnlySmall);
      this.ShowLinkPreview(new WebPageMetadata()
      {
        Description = uriMessageWrapper.Description,
        OriginalUrl = uriMessageWrapper.MatchedText,
        Title = uriMessageWrapper.Title,
        CanonicalUrl = uriMessageWrapper.CanonicalUrl,
        Thumbnail = thumbnail == null ? (WriteableBitmap) null : new WriteableBitmap(thumbnail)
      });
    }

    private void ShowLinkPreview(WebPageMetadata data)
    {
      this.Pause(false);
      if (this.linkPreviewPanel == null)
      {
        SolidColorBrush bgBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 176, (byte) 176, (byte) 176));
        this.LinkPreviewContainer.Background = (Brush) bgBrush;
        this.linkPreviewPanel = new LinkPreviewPanel(1.0, UIUtils.BlackBrush, bgBrush);
        this.linkPreviewPanel.DismissButton.Visibility = Visibility.Collapsed;
        this.linkPreviewPanel.ShowLinkPanel(true, (Brush) UIUtils.WhiteBrush);
        this.LinkPreviewContainer.Children.Add((UIElement) this.linkPreviewPanel);
      }
      this.linkPreviewPanel.Update(data);
      this.LinkPreviewContainer.Visibility = this.LinkPreviewMask.Visibility = Visibility.Visible;
      this.linkPreviewShownSubj.OnNext(true);
    }

    private void DismissLinkPreview()
    {
      this.LinkPreviewContainer.Visibility = this.LinkPreviewMask.Visibility = Visibility.Collapsed;
      this.Resume();
      this.linkPreviewShownSubj.OnNext(false);
    }

    private void RenderAsImage(MessageViewModel msgVm)
    {
      this.contentSub.SafeDispose();
      this.contentSub = this.GetImageObservable(msgVm.Message).SubscribeOn<Pair<BitmapSource, bool>>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<Pair<BitmapSource, bool>>().Subscribe<Pair<BitmapSource, bool>>((Action<Pair<BitmapSource, bool>>) (p =>
      {
        BitmapSource first = p.First;
        int num = p.Second ? 1 : 0;
        bool flag = false;
        if (first.PixelHeight > first.PixelWidth && Math.Abs((double) first.PixelHeight / (double) first.PixelWidth / (this.LayoutRoot.ActualHeight / this.LayoutRoot.ActualWidth) - 1.0) < 0.15)
          flag = true;
        this.ImageView.Source = (System.Windows.Media.ImageSource) first;
        this.ImageView.Opacity = first == null ? 0.0 : 1.0;
        this.ImageView.Stretch = flag ? Stretch.UniformToFill : Stretch.Uniform;
        if (num == 0)
          return;
        this.MarkAsSeen(msgVm.Message);
        int viewTime = this.GetViewTime(msgVm);
        this.StartProgress(viewTime);
        this.StartTimer(viewTime * 1000);
        this.isCurrentStatusRendered = true;
      }), (Action) (() =>
      {
        this.contentSub.SafeDispose();
        this.contentSub = (IDisposable) null;
      }));
    }

    private void RenderAsVideo(MessageViewModel msgVm)
    {
      Log.d("statusv3", "Rendering the video status");
      this.videoPlayback.SafeDispose();
      this.videoPlayback = new VideoPlayback(msgVm.Message);
      this.VideoViewCanvas.Opacity = 1.0;
      this.videoPlayback.SetSourceOnMediaElement(this.VideoView, videoPlaybackDownloadCallbacks: (IVideoPlaybackDownloadCallbacks) this, videoStreamSourceBufferingCallbacks: (IVideoStreamSourceBufferingCallbacks) this, videoStreamSourceErrorCallbacks: (IVideoStreamSourceErrorCallbacks) this);
      this.LoadingProgress.Visibility = Visibility.Visible;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        BitmapSource blurredThumbnail = this.videoPlayback?.GetBlurredThumbnail();
        if (blurredThumbnail != null)
        {
          Log.d("statusv3", "Set the thumbnail on the video");
          this.ImageView.Source = (System.Windows.Media.ImageSource) blurredThumbnail;
          this.ImageView.Opacity = 1.0;
        }
        else if (this.videoPlayback == null)
          Log.d("statusv3", "Not setting the thumbnail as the videoPlayback was disposed");
        else
          Log.d("statusv3", "Not setting the thumbnail since it was null");
      }));
    }

    private void UpdateVideoRotation(int angle, double thumbnailRatio)
    {
      Log.d("statusv3", string.Format("Updating the video rotation with angle {0} and thumbnail ratio {1}", (object) angle, (object) thumbnailRatio));
      CompositeTransform renderTransform = (CompositeTransform) this.VideoView.RenderTransform;
      renderTransform.Rotation = (double) -angle + 0.05;
      Size renderSize = ResolutionHelper.GetRenderSize();
      this.VideoView.Width = renderSize.Width;
      this.VideoView.Height = renderSize.Height;
      if (angle == 90 || angle == 270)
      {
        double num = thumbnailRatio >= 1.0 ? 1.0 / thumbnailRatio : renderSize.Height / renderSize.Width;
        Log.d("statusv3", string.Format("The status scale was {0}", (object) num));
        renderTransform.ScaleX = renderTransform.ScaleY = num;
      }
      else
        Log.d("statusv3", "Not scaling the video since it was in landscape mode");
    }

    private IObservable<Pair<BitmapSource, bool>> GetImageObservable(Message m)
    {
      return Observable.Create<Pair<BitmapSource, bool>>((Func<IObserver<Pair<BitmapSource, bool>>, Action>) (observer =>
      {
        object subLock = new object();
        IDisposable localUriSub = (IDisposable) null;
        bool disposed = false;
        bool downloadAndTrack = false;
        Stream imgStream = (Stream) null;
        int decodeEdge = 800;
        bool isFileAvailable = false;
        if (m.LocalFileUri == null)
          downloadAndTrack = true;
        else
          imgStream = m.GetImageStream();
        if (imgStream == null)
        {
          bool isLargeSize = false;
          imgStream = (Stream) m.GetThumbnailStream(true, out isLargeSize);
          decodeEdge = 800;
        }
        else
          isFileAvailable = true;
        if (imgStream == null)
        {
          if (!downloadAndTrack)
            observer.OnCompleted();
        }
        else
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            using (imgStream)
            {
              WriteableBitmap writeableBitmap = m.KeyFromMe ? BitmapUtils.CreateBitmap(imgStream, decodeEdge, decodeEdge) : BitmapUtils.CreateBitmap(imgStream);
              if (writeableBitmap != null)
              {
                if (!isFileAvailable)
                  writeableBitmap = writeableBitmap.Blur();
                observer.OnNext(new Pair<BitmapSource, bool>((BitmapSource) writeableBitmap, isFileAvailable));
              }
            }
            if (downloadAndTrack)
              return;
            observer.OnCompleted();
          }));
        if (downloadAndTrack)
        {
          lock (subLock)
          {
            if (!disposed)
              localUriSub = m.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (p => p.PropertyName == "LocalFileUri")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ =>
              {
                Stream imageStream = m.GetImageStream();
                if (imageStream != null)
                {
                  using (imageStream)
                  {
                    WriteableBitmap first = m.KeyFromMe ? BitmapUtils.CreateBitmap(imageStream, 800, 800) : BitmapUtils.CreateBitmap(imageStream);
                    if (first != null)
                      observer.OnNext(new Pair<BitmapSource, bool>((BitmapSource) first, true));
                  }
                }
                observer.OnCompleted();
              }));
          }
          this.Dispatcher.BeginInvoke((Action) (() => ViewMessage.Download(m)));
        }
        return (Action) (() =>
        {
          lock (subLock)
          {
            disposed = true;
            localUriSub.SafeDispose();
            localUriSub = (IDisposable) null;
          }
        });
      }));
    }

    private void RenderAsUndecrypted()
    {
      this.CenterTextBlock.Text = new RichTextBlock.TextSet()
      {
        Text = AppResources.MessagePendingDecrypt
      };
      this.CenterTextBlock.Visibility = Visibility.Visible;
      this.isCurrentStatusRendered = true;
    }

    private void RenderAsUnsupported()
    {
      this.CenterTextBlock.Text = new RichTextBlock.TextSet()
      {
        Text = AppResources.UnsupportedMessage
      };
      this.CenterTextBlock.Visibility = Visibility.Visible;
      this.isCurrentStatusRendered = true;
    }

    private void StartProgress(int duration)
    {
      if (this.progressBar == null)
        return;
      int currentStatusIndex = this.currentStatusIndex;
      if (duration > 0)
        this.progressBar.Begin(currentStatusIndex, duration);
      if (!this.IsPaused)
        return;
      this.progressBar.Pause();
    }

    private void StartTimer(int timeoutInMs)
    {
      int currIndex = this.currentStatusIndex;
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.remainingTimeout = Math.Max(0, timeoutInMs);
      Log.d("statusv3", string.Format("Remaining timeout is {0}", (object) this.remainingTimeout));
      if (this.IsPaused || timeoutInMs <= 0)
        return;
      DateTime dtStart = DateTime.Now;
      this.timerSub = Observable.Interval(TimeSpan.FromMilliseconds(550.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        this.remainingTimeout = timeoutInMs - (int) (DateTime.Now - dtStart).TotalMilliseconds;
        if (this.remainingTimeout > 0)
          return;
        this.timerSub.SafeDispose();
        this.timerSub = (IDisposable) null;
        if (this.finishedGifLoops.HasValue && this.finishedGifLoops.Value < 3 || this.RenderStatus(currIndex + 1))
          return;
        this.exitSubj.OnNext(1);
      }), (Action) (() =>
      {
        this.timerSub.SafeDispose();
        this.timerSub = (IDisposable) null;
      }));
    }

    private void MarkAsSeen(Message m)
    {
      if (m == null || m.KeyFromMe || m.IsReadByTarget())
        return;
      int msgId = m.MessageID;
      AppState.Worker.Enqueue((Action) (() =>
      {
        List<PersistentAction> persistActions = new List<PersistentAction>();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Message messageById3 = db.GetMessageById(msgId);
          if (messageById3 == null)
            return;
          messageById3.Status = FunXMPP.FMessage.Status.ReadByTarget;
          string senderJid = messageById3.GetSenderJid();
          WaStatus waStatus3 = db.GetWaStatus(senderJid, messageById3.MessageID);
          if (waStatus3 != null)
          {
            waStatus3.IsViewed = true;
            if (JidHelper.IsPsaJid(senderJid) && Settings.IsStatusPSAUnseen)
            {
              Settings.IsStatusPSAUnseen = false;
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              WaStatus[] statuses = db.GetStatuses("0@s.whatsapp.net", false, true, new TimeSpan?());
              foreach (WaStatus waStatus4 in statuses)
              {
                waStatus4.Timestamp = currentServerTimeUtc;
                Message messageById4 = db.GetMessageById(waStatus4.MessageId);
                if (messageById4 != null)
                  messageById4.FunTimestamp = new DateTime?(currentServerTimeUtc);
              }
            }
          }
          ReceiptSpec[] receipts = new ReceiptSpec[1]
          {
            new ReceiptSpec()
            {
              Jid = messageById3.KeyRemoteJid,
              Id = messageById3.KeyId,
              Participant = messageById3.RemoteResource,
              IsCipherText = false
            }
          };
          ReadReceipts.Send(db, receipts);
          int limit = 2;
          WaStatus[] statuses2 = db.GetStatuses(new string[1]
          {
            senderJid
          }, (string[]) null, true, true, new TimeSpan?(WaStatus.Expiration), limit, new int?(waStatus3.StatusId));
          if (((IEnumerable<WaStatus>) statuses2).Any<WaStatus>())
          {
            foreach (Message message in ((IEnumerable<WaStatus>) statuses2).Select<WaStatus, Message>((Func<WaStatus, Message>) (s => db.GetMessageById(s.MessageId))).Where<Message>((Func<Message, bool>) (msgToDownload => ((int) msgToDownload?.InternalProperties?.MediaPropertiesField?.AutoDownloadEligible ?? 0) == 0)).ToArray<Message>())
            {
              MessageProperties forMessage = MessageProperties.GetForMessage(message);
              forMessage.EnsureMediaProperties.AutoDownloadEligible = new bool?(true);
              forMessage.Save();
              PersistentAction a = PersistentAction.AutoDownload(messageById3);
              db.StorePersistentAction(a);
              persistActions.Add(a);
              Log.l("statusv3", "download status on deck | {0}", (object) message.LogInfo());
            }
          }
          int num = limit - statuses2.Length;
          db.GetJidInfo(senderJid, CreateOptions.CreateToDbIfNotFound).StatusAutoDownloadQuota = num;
          Log.d("statusv3", "jid:{0}, status auto download quota left:{1}", (object) messageById3.KeyRemoteJid, (object) num);
          db.SubmitChanges();
          AppState.QrPersistentAction.NotifySeen(messageById3.KeyRemoteJid, messageById3.KeyId, messageById3.KeyFromMe, senderJid);
        }));
        if (!persistActions.Any<PersistentAction>())
          return;
        persistActions.ForEach((Action<PersistentAction>) (a => AppState.AttemptPersistentAction(a)));
      }));
    }

    private void RenderPrevStatus()
    {
      if (this.RenderStatus(this.currentStatusIndex - 1))
        return;
      this.exitSubj.OnNext(-1);
    }

    private void RenderNextStatus()
    {
      if (this.RenderStatus(this.currentStatusIndex + 1))
        return;
      this.exitSubj.OnNext(1);
    }

    public void Pause(bool pauseByTouch)
    {
      if (pauseByTouch)
        this.isPausedByTouch = true;
      else
        this.isPausedByOther = true;
      Message message = this.CurrentStatusMessageViewModel?.Message;
      if (message != null && (message.MediaWaType == FunXMPP.FMessage.Type.Video || message.MediaWaType == FunXMPP.FMessage.Type.Gif) && this.VideoView.CanPause)
        this.VideoView.Pause();
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      if (this.progressBar != null)
        this.progressBar.Pause();
      this.playbackStateSubj.OnNext(false);
      if (!pauseByTouch)
        return;
      IDisposable sub = (IDisposable) null;
      sub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).Take<long>(1).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
        if (!this.isPausedByTouch)
          return;
        this.ShowHeaderPanel(false);
      }));
    }

    public void Resume()
    {
      if (!this.IsPaused)
        return;
      this.isPausedByOther = this.isPausedByTouch = false;
      Message message = this.CurrentStatusMessageViewModel?.Message;
      if (message != null && (message.MediaWaType == FunXMPP.FMessage.Type.Video || message.MediaWaType == FunXMPP.FMessage.Type.Gif))
        this.VideoView.Play();
      this.StartTimer(this.remainingTimeout);
      if (this.progressBar != null)
        this.progressBar.Resume();
      this.playbackStateSubj.OnNext(true);
      this.ShowHeaderPanel(true);
    }

    private void ContentPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      Log.d("statusv3", "status view | manipulation started");
      if (this.statusesFetchSub != null)
        Log.l("StatusView", "ContentPanel_ManipulationStarted request ignored, status still being built");
      else
        this.Pause(true);
    }

    private void ContentPanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
    }

    private void ContentPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      Log.d("statusv3", "status view | manipulation completed | paused:{0},by touch:{1},by other:{2}", (object) this.IsPaused, (object) this.isPausedByTouch, (object) this.isPausedByOther);
      if (this.statusesFetchSub != null)
      {
        Log.l("StatusView", "ContentPanel_ManipulationCompleted request ignored, status still being built");
      }
      else
      {
        bool flag = false;
        double x = e.FinalVelocities.LinearVelocity.X;
        double num1 = Math.Abs(x);
        double y = e.FinalVelocities.LinearVelocity.Y;
        double num2 = Math.Abs(y);
        if (num1 > num2)
        {
          if (num1 > 1000.0)
          {
            if (x > 0.0)
              this.exitSubj.OnNext(-1);
            else
              this.exitSubj.OnNext(1);
            flag = true;
          }
        }
        else if (num2 > 1000.0)
        {
          if (y > 0.0)
            this.exitSubj.OnNext(0);
          else
            this.flickedUpSubj.OnNext(new Unit());
          flag = true;
        }
        if (flag)
          return;
        this.Resume();
      }
    }

    private void ContentPanel_Tap(object sender, GestureEventArgs e)
    {
      Log.d("statusv3", "status view | tapped | paused:{0},by touch:{1},by other:{2}", (object) this.IsPaused, (object) this.isPausedByTouch, (object) this.isPausedByOther);
      if (this.statusesFetchSub != null)
      {
        Log.l("StatusView", "ContentPanel_Tap request ignored, status still being built");
      }
      else
      {
        if (this.IsPaused)
          return;
        if (e.GetPosition((UIElement) this.ContentPanel).X < this.ContentPanel.ActualWidth * 0.25)
        {
          this.RenderPrevStatus();
        }
        else
        {
          if (!this.isCurrentStatusRendered)
            this.MarkAsSeen(this.CurrentStatusMessageViewModel?.Message);
          this.RenderNextStatus();
        }
      }
    }

    private void MuteButton_Click(object sender, RoutedEventArgs e)
    {
      this.VideoView.IsMuted = this.isSoundMuted = !this.VideoView.IsMuted;
      this.MuteButtonIcon.Source = this.IsSoundMuted ? (System.Windows.Media.ImageSource) AssetStore.VolumeMuteIconWhite : (System.Windows.Media.ImageSource) AssetStore.VolumeIconWhite;
    }

    private void VideoView_MediaOpened(object sender, RoutedEventArgs e)
    {
      Log.l("statusv3", "Video opened");
      this.VideoView.Play();
      this.VideoViewCanvas.Opacity = 1.0;
      this.LoadingProgress.Visibility = Visibility.Collapsed;
      if (!Assert.IsTrue(this.CurrentStatusMessageViewModel != null, "CurrentStatusMessageViewModel cannot be null after the video has opened"))
        return;
      if (this.videoPlayback == null)
      {
        Log.l("statusv3", "Returning from MediaOpened as videoPlayback is null");
      }
      else
      {
        this.ImageView.Source = (System.Windows.Media.ImageSource) null;
        this.ImageView.Opacity = 0.0;
        this.UpdateVideoRotation(this.videoPlayback.GetRotationAngle(), this.CurrentStatusMessageViewModel.Message.GetThumbnailRatio());
        int num1 = (int) ((double) this.CurrentStatusMessageViewModel.Message.MediaDurationSeconds + 0.5);
        int duration = this.CurrentStatusMessageViewModel.Message.MediaWaType == FunXMPP.FMessage.Type.Gif ? Math.Max(6, num1 * 3) : num1;
        bool flag = this.CurrentStatusMessageViewModel.Message.MediaWaType == FunXMPP.FMessage.Type.Gif;
        int num2 = flag ? duration : 0;
        this.StartProgress(duration);
        this.StartTimer(num2 * 1000);
        this.VideoView.IsMuted = this.IsSoundMuted;
        this.MuteButtonIcon.Source = this.IsSoundMuted ? (System.Windows.Media.ImageSource) AssetStore.VolumeMuteIconWhite : (System.Windows.Media.ImageSource) AssetStore.VolumeIconWhite;
        this.MuteButton.Visibility = (!flag).ToVisibility();
        this.MarkAsSeen(this.CurrentStatusMessageViewModel.Message);
        this.isCurrentStatusRendered = true;
        Log.d("statusv3", "Finished opening the media");
      }
    }

    private void VideoView_MediaEnded(object sender, RoutedEventArgs e)
    {
      Log.l("statusv3", "Received MediaEnded event");
      bool flag = false;
      if (this.finishedGifLoops.HasValue)
      {
        int? finishedGifLoops = this.finishedGifLoops;
        int num = 1;
        this.finishedGifLoops = finishedGifLoops.HasValue ? new int?(finishedGifLoops.GetValueOrDefault() + num) : new int?();
        if (this.finishedGifLoops.Value < 3 || this.timerSub != null)
          flag = true;
      }
      if (flag)
        this.VideoView.Play();
      else
        this.RenderNextStatus();
    }

    public void UpdateVideoDownloadProgress(double progress)
    {
      Log.d("statusv3", string.Format("UpdateVideoDownloadProgress: {0}%", (object) (progress * 100.0)));
    }

    public void VideoDownloadComplete(string localFileUrl)
    {
      Log.d("statusv3", string.Format("Video download complete: {0}", (object) localFileUrl));
      this.Dispatcher.BeginInvoke((Action) (() => this.VideoView.Source = new Uri(MediaStorage.GetAbsolutePath(localFileUrl))));
    }

    public void StartedBuffering()
    {
      Log.d("statusv3", "Video started buffering");
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.LoadingProgress.Visibility = Visibility.Visible;
        this.Pause(false);
      }));
    }

    public void BufferingProgressChanged(double progress)
    {
    }

    public void FinishedBuffering()
    {
      Log.d("statusv3", "Video finished buffering");
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.LoadingProgress.Visibility = Visibility.Collapsed;
        this.Resume();
      }));
    }

    public void CouldNotStream()
    {
      Log.l("statusv3", "Could not stream video");
      Assert.IsTrue(this.videoPlayback != null, "VideoPlayback cannot be null while attempting to stream");
      this.videoPlayback?.SwitchToDownload(this.Dispatcher);
    }

    private void LinkPreviewMask_Tap(object sender, GestureEventArgs e)
    {
      Log.d("statusv3", "status view | link preview mask tap");
      this.DismissLinkPreview();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/WaStatusViewControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.ImageView = (Image) this.FindName("ImageView");
      this.VideoViewCanvas = (Canvas) this.FindName("VideoViewCanvas");
      this.VideoView = (MediaElement) this.FindName("VideoView");
      this.LoadingProgress = (ProgressBar) this.FindName("LoadingProgress");
      this.CenterTextBlock = (RichTextBlock) this.FindName("CenterTextBlock");
      this.HeaderPanelZoomBox = (ZoomBox) this.FindName("HeaderPanelZoomBox");
      this.HeaderPanel = (Grid) this.FindName("HeaderPanel");
      this.HeaderPanelGradientPanel = (Rectangle) this.FindName("HeaderPanelGradientPanel");
      this.ProfilePicture = (Image) this.FindName("ProfilePicture");
      this.NamePanel = (StackPanel) this.FindName("NamePanel");
      this.NameBlock = (TextBlock) this.FindName("NameBlock");
      this.InfoBlock = (TextBlock) this.FindName("InfoBlock");
      this.MuteButton = (Button) this.FindName("MuteButton");
      this.MuteButtonIcon = (Image) this.FindName("MuteButtonIcon");
      this.LinkPreviewMask = (Rectangle) this.FindName("LinkPreviewMask");
      this.BottomPanelZoomBox = (ZoomBox) this.FindName("BottomPanelZoomBox");
      this.CaptionPanel = (Grid) this.FindName("CaptionPanel");
      this.CaptionBlock = (RichTextBlock) this.FindName("CaptionBlock");
      this.LinkPreviewContainer = (Grid) this.FindName("LinkPreviewContainer");
    }

    public struct VideoResult
    {
      public string Filepath;
      public System.Windows.Media.ImageSource Thumbnail;
      public Matrix? Rotation;
      public double? DurationSeconds;
    }
  }
}
