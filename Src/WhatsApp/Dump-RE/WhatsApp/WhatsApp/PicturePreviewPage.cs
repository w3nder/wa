// Decompiled with JetBrains decompiler
// Type: WhatsApp.PicturePreviewPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WhatsApp.Events;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class PicturePreviewPage : PhoneApplicationPage
  {
    private static MediaSharingState NextInstanceMediaSharingState = (MediaSharingState) null;
    private static IObserver<MediaSharingArgs> NextInstanceObserver = (IObserver<MediaSharingArgs>) null;
    private static bool NextInstanceIsSinglePhotoPreview = false;
    private MediaSharingState sharingState_;
    private IObserver<MediaSharingArgs> previewObserver_;
    private bool isSinglePhotoPreview_;
    private List<PicturePreviewPage.CropRatioItem> cropRatioItems_;
    private const int timelineOffset = 450;
    private ChatPage.InputMode inputMode_ = ChatPage.InputMode.None;
    private List<PicturePreviewPage.PreviewItem> previewItems_;
    private List<PicturePreviewPage.PreviewItem> thumbListSource_;
    private int currentIndex_ = -1;
    private bool submitted_;
    private bool onback;
    private IDisposable sharingStateItemsChangedSub_;
    public static readonly DependencyProperty RootFrameTranslateYProperty = DependencyProperty.Register(nameof (RootFrameTranslateY), typeof (double), typeof (PicturePreviewPage), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(PicturePreviewPage.OnRootFrameTranslateYChanged)));
    private double screenRenderHeight_ = 800.0;
    private double maxSlideViewHeight_ = 610.0;
    private Size cropSpaceSize_;
    private Size lowResMaxSize_ = new Size(480.0, 480.0);
    private Size highResMaxSize_ = new Size(800.0, 800.0);
    private double keyboardOffset_;
    private CompositeDisposable videoFrameSubs;
    private IDisposable captionFocusChangedSub;
    private TranslateTransform shiftedRootFrameTransformPortrait_ = new TranslateTransform();
    private Brush captionTextBrush_ = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 239, (byte) 239, (byte) 239));
    private Brush captionHintBrush_ = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 167, (byte) 167, (byte) 167));
    private Brush singlePreviewHintBrush_ = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 205, (byte) 205, (byte) 205));
    private bool stopGifPlayback;
    private long lastSartedAt = -1;
    private long frameDelay = 1000000;
    private int CurrentItemOriginalHeight;
    private int CurrentItemOriginalWidth;
    private Grid DrawGrid;
    private Storyboard fadeInSb_;
    private DoubleAnimation fadeInAnimation_;
    private IDisposable fadeInSbSub_;
    private IDisposable pendingSbSub_;
    private bool isCropping_;
    private WriteableBitmap croppingSource_;
    private DateTime? lastButtonClickedAt_;
    private MediaElementWrapper videoPlayerWrapper_;
    private DispatcherTimer playbackTimer;
    private DateTime recordingStartTime;
    private int timerDuration;
    private DispatcherTimer timeCropTimer;
    private bool timeCropManipulation;
    private bool PlayingVideo;
    private bool initialLoaded_;
    private bool ignoreSelectedItemsChangeOnce_;
    private bool isSwitching_;
    private bool isSwitchingEnough_;
    private bool ignoreEmojiKeyboardClosedOnce_;
    private long currentPosition;
    private static Dictionary<wam_enum_media_picker_origin_type, MediaPicker> imagePickerEvents = (Dictionary<wam_enum_media_picker_origin_type, MediaPicker>) null;
    private static Dictionary<wam_enum_media_picker_origin_type, MediaPicker> videoPickerEvents = (Dictionary<wam_enum_media_picker_origin_type, MediaPicker>) null;
    private static long previewStartTimeTicks = 0;
    private static Dictionary<string, PicturePreviewPage.PIDetails> imagePreviewItemKeyCheck = new Dictionary<string, PicturePreviewPage.PIDetails>();
    private static Dictionary<string, PicturePreviewPage.PIDetails> videoPreviewItemKeyCheck = new Dictionary<string, PicturePreviewPage.PIDetails>();
    private const int StateItemNotPresent = 0;
    private const int StateItemAdded = 1;
    private const int StateItemChanged = 2;
    private Stroke NewStroke;
    public static Color StrokeColor = Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, (byte) 0);
    public static Grid ColorPicker;
    private bool strokeStarted;
    private bool force = true;
    private double imageScale = -1.0;
    private EmojiKeyboard emojiKeyboard;
    private bool IsEmojiKeyboardOpen;
    private Dictionary<EmojiTextBox, RichTextBlock> drawingBoxes = new Dictionary<EmojiTextBox, RichTextBlock>();
    private RichTextBlock DrawingModeTextBox;
    private Grid DrawingModeTextGrid;
    private Grid TextColorPickerGlobal;
    private bool TextColorPickerJustTapped;
    private Color TextStrokeColor = Colors.White;
    private int DrawTextFontSize = 56;
    internal Grid LayoutRoot;
    internal Grid SlideViewPanel;
    internal CompositeTransform SliderViewTransform;
    internal ImageSlideViewControl SlideView;
    internal Grid DrawingMode;
    internal CompositeTransform DrawingModeTransform;
    internal Grid InnerLayout;
    internal MediaElement VideoPlayer;
    internal RectangleGeometry VideoCropRectangle;
    internal ImageEditControl EditControl;
    internal Rectangle AppBarGradient;
    internal Grid AppBar;
    internal StackPanel AppBarButtonsPanel;
    internal CompositeTransform AppBarButtonsPanelTransform;
    internal Button UndoButton;
    internal Button DeleteButton;
    internal Button CropButton;
    internal Button RotateButton;
    internal Button GifButton;
    internal Rectangle VideoToggleForeground;
    internal Rectangle GifToggleForeground;
    internal Button EmojiButton;
    internal Button TextButton;
    internal Ellipse TextButtonBackground;
    internal Button PaintButton;
    internal Ellipse PaintButtonBackground;
    internal Grid DurationBox;
    internal Rectangle DurationGradient;
    internal TextBlock Duration;
    internal RoundButton PlayButton;
    internal StackPanel BottomPanel;
    internal StackPanel CaptionAndThumbList;
    internal CompositeTransform CaptionAndThumbListTransform;
    internal Grid AddButton;
    internal Image AddButtonImage;
    internal Rectangle CaptionBackground;
    internal ScrollViewer CaptionPanel;
    internal RichTextBlock CaptionBlock;
    internal Button SubmitButton;
    internal Image SubmitButtonIcon;
    internal Grid ThumbnailsPanel;
    internal ScrollViewer PreviewItemsPanel;
    internal Grid ScrollingPart;
    internal ListBox PreviewItemsList;
    internal TextBlock Recipients;
    internal Grid CropBar;
    internal Grid CropControlPanel;
    internal CompositeTransform CropControlPanelTransform;
    internal Button CropDoneButton;
    internal ListPicker CropAspectRatioPicker;
    internal Canvas VideoTimelinePanel;
    internal ListBox VideoTimelineList;
    internal Border CurrentTimeStrip;
    internal CompositeTransform CurrentTimeStripTranslate;
    internal Grid TimelineLeftHandle;
    internal CompositeTransform LeftHandleTransform;
    internal Rectangle LeftHandleOverlay;
    internal Rectangle LeftHandle;
    internal Grid TimelineHandleRight;
    internal CompositeTransform RightHandleTransform;
    internal Rectangle RightHandleOverlay;
    internal Rectangle RightHandle;
    internal EmojiTextBox CaptionBox;
    private bool _contentLoaded;

    private bool ShouldShowAddButton
    {
      get
      {
        if (this.isSinglePhotoPreview_ || this.sharingState_ == null || this.previewItems_ == null || this.previewItems_.Count > MediaSharingState.MaxSelectedItem)
          return false;
        return this.sharingState_.Mode == MediaSharingState.SharingMode.ChoosePicture || this.sharingState_.Mode == MediaSharingState.SharingMode.ChooseMedia || this.sharingState_.Mode == MediaSharingState.SharingMode.ChooseVideo || this.sharingState_.Mode == MediaSharingState.SharingMode.TakePicture;
      }
    }

    private bool ShouldShowThumbnailRow
    {
      get
      {
        return this.Recipients.Visibility == Visibility.Visible || this.previewItems_ != null && this.previewItems_.Count<PicturePreviewPage.PreviewItem>() > 1;
      }
    }

    private ChatPage.InputMode InputMode
    {
      get => this.inputMode_;
      set
      {
        if (this.inputMode_ == value && value != ChatPage.InputMode.None)
          return;
        this.OnInputModeChanged(this.inputMode_, this.inputMode_ = value);
      }
    }

    private PicturePreviewPage.PreviewItem CurrentItem
    {
      get
      {
        return this.previewItems_ != null ? this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_) : (PicturePreviewPage.PreviewItem) null;
      }
    }

    private PicturePreviewPage.PreviewItem NextItem
    {
      get
      {
        return this.previewItems_ != null ? this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_ + 1) : (PicturePreviewPage.PreviewItem) null;
      }
    }

    private PicturePreviewPage.PreviewItem PrevItem
    {
      get
      {
        return this.previewItems_ != null ? this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_ - 1) : (PicturePreviewPage.PreviewItem) null;
      }
    }

    public double RootFrameTranslateY
    {
      get => (double) this.GetValue(PicturePreviewPage.RootFrameTranslateYProperty);
    }

    private double CaptionBoxReservedHeight => this.CaptionBox.ActualHeight - 15.0;

    private double CurrentSIPOffset
    {
      get
      {
        if (this.InputMode == ChatPage.InputMode.Keyboard)
          return this.keyboardOffset_;
        return this.InputMode == ChatPage.InputMode.Emoji ? UIUtils.SIPHeightPortrait : 0.0;
      }
    }

    public PicturePreviewPage()
    {
      this.InitializeComponent();
      this.sharingState_ = PicturePreviewPage.NextInstanceMediaSharingState;
      this.previewObserver_ = PicturePreviewPage.NextInstanceObserver;
      this.isSinglePhotoPreview_ = PicturePreviewPage.NextInstanceIsSinglePhotoPreview;
      PicturePreviewPage.NextInstanceMediaSharingState = (MediaSharingState) null;
      PicturePreviewPage.NextInstanceObserver = (IObserver<MediaSharingArgs>) null;
      PicturePreviewPage.NextInstanceIsSinglePhotoPreview = false;
      if (this.sharingState_ == null)
        return;
      this.videoPlayerWrapper_ = new MediaElementWrapper(this.VideoPlayer);
      this.videoPlayerWrapper_.MediaEnded += new EventHandler(this.VideoPlayer_MediaEnded);
      this.videoPlayerWrapper_.MediaOpened += new EventHandler(this.VideoPlayer_MediaOpened);
      this.AddButtonImage.Source = this.sharingState_.Mode != MediaSharingState.SharingMode.TakePicture ? (System.Windows.Media.ImageSource) ImageStore.GetStockIcon("/Images/multisend-add.png") : (System.Windows.Media.ImageSource) ImageStore.GetStockIcon("/Images/multisend-add-cam.png");
      this.SubmitButtonIcon.Width = this.SubmitButtonIcon.Height = 36.0 * ResolutionHelper.ZoomMultiplier;
      this.SubmitButtonIcon.Source = (System.Windows.Media.ImageSource) AssetStore.InputBarSendIcon;
      this.SubmitButtonIcon.FlowDirection = App.CurrentApp.RootFrame.FlowDirection;
      Rectangle durationGradient = this.DurationGradient;
      LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection1 = new GradientStopCollection();
      gradientStopCollection1.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 96, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection1.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
        Offset = 1.0
      });
      linearGradientBrush1.GradientStops = gradientStopCollection1;
      linearGradientBrush1.StartPoint = new System.Windows.Point(0.0, 1.0);
      linearGradientBrush1.EndPoint = new System.Windows.Point(0.0, 0.0);
      durationGradient.Fill = (Brush) linearGradientBrush1;
      Rectangle appBarGradient = this.AppBarGradient;
      LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection2 = new GradientStopCollection();
      gradientStopCollection2.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 96, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection2.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
        Offset = 1.0
      });
      linearGradientBrush2.GradientStops = gradientStopCollection2;
      linearGradientBrush2.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush2.EndPoint = new System.Windows.Point(0.0, 1.0);
      appBarGradient.Fill = (Brush) linearGradientBrush2;
      PicturePreviewPage.StrokeColor = Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, (byte) 0);
      this.UpdateRecipientsTextBlock();
      this.ReloadData();
      this.sharingStateItemsChangedSub_ = this.sharingState_.SubscribeToSelectedItemsChange(new Action<MediaSharingState.SelectedItemsChangeCause>(this.ProcessSharingStateSelectedItemsChanged));
      this.SlideView.ImageSwitched += new ImageSlideViewControl.ImageSwitchedHandler(this.SliderView_ItemSwitched);
      this.SlideView.ImageSwitching += new ImageSlideViewControl.ImageSwitchingHandler(this.SliderView_ItemSwitching);
      this.CaptionBox.CounterForeground = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 153, (byte) 153, (byte) 153));
      this.CaptionBox.EmojiKeyboardOpening += new EventHandler(this.CaptionBox_EmojiKeyboardOpening);
      this.CaptionBox.EmojiKeyboardClosed += new EventHandler(this.CaptionBox_EmojiKeyboardClosed);
      this.captionFocusChangedSub = this.CaptionBox.TextBoxFocusChangedObservable().Subscribe<bool>(new Action<bool>(this.CaptionBox_FocusChanged));
      this.CaptionBox.KeyDown += (KeyEventHandler) ((sender, e) =>
      {
        if (e.Key != System.Windows.Input.Key.Enter)
          return;
        e.Handled = true;
        this.OnEnterKeyPressed();
      });
      this.screenRenderHeight_ = ResolutionHelper.GetRenderSize().Height;
      this.UpdateThumbnailVisibility();
      this.cropSpaceSize_ = new Size(ResolutionHelper.GetRenderSize().Width, this.screenRenderHeight_);
      this.EditControl.Height = this.cropSpaceSize_.Height;
      this.EditControl.CropMode = ImageEditControl.CroppingMode.None;
      this.EditControl.RotateMode = ImageEditControl.RotationMode.Rotate90;
      this.PlayButton.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/play.png");
      this.PlayButton.ButtonIconReversed = (BitmapSource) ImageStore.GetStockIcon("/Images/play-active.png");
      this.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) =>
      {
        if (this.InputMode != ChatPage.InputMode.Emoji)
          return;
        this.CaptionBox.CloseEmojiKeyboard();
      });
      if (!this.isSinglePhotoPreview_)
        return;
      this.DeleteButton.Visibility = Visibility.Collapsed;
    }

    private void UpdateThumbnailVisibility()
    {
      if (!this.ShouldShowThumbnailRow)
      {
        this.SlideView.Height = this.maxSlideViewHeight_ = this.screenRenderHeight_;
        this.ThumbnailsPanel.Visibility = Visibility.Collapsed;
        this.CaptionBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb((byte) 96, (byte) 53, (byte) 53, (byte) 53));
      }
      else
      {
        this.SlideView.Height = this.maxSlideViewHeight_ = this.screenRenderHeight_ - 110.0;
        this.ThumbnailsPanel.Visibility = Visibility.Visible;
        this.CaptionBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb((byte) 178, (byte) 53, (byte) 53, (byte) 53));
      }
    }

    public static IObservable<MediaSharingArgs> Start(MediaSharingState state, bool isSingle = false)
    {
      Mms4Helper.MaybeWarmupMms4Route(FunXMPP.FMessage.FunMediaType.Image, true);
      return Observable.Create<MediaSharingArgs>((Func<IObserver<MediaSharingArgs>, Action>) (observer =>
      {
        PicturePreviewPage.NextInstanceMediaSharingState = state;
        PicturePreviewPage.NextInstanceObserver = observer;
        PicturePreviewPage.NextInstanceIsSinglePhotoPreview = isSingle;
        NavUtils.NavigateToPage(nameof (PicturePreviewPage));
        return (Action) (() => { });
      }));
    }

    private void SaveCaptionForCurrentItem()
    {
      PicturePreviewPage.PreviewItem previewItem = this.previewItems_ == null ? (PicturePreviewPage.PreviewItem) null : this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_);
      if (previewItem == null)
        return;
      string str = this.CaptionBox.Text == null ? (string) null : Emoji.ConvertToRichText(this.CaptionBox.Text).Trim();
      previewItem.Caption = string.IsNullOrEmpty(str) ? (string) null : str;
      this.UpdateCaptionPanel(previewItem, true);
    }

    private void UpdateRootFrameShifting(ChatPage.InputMode mode)
    {
      if (mode == ChatPage.InputMode.Emoji || mode == ChatPage.InputMode.Keyboard)
      {
        TranslateTransform transformPortrait = this.shiftedRootFrameTransformPortrait_;
        transformPortrait.Y = -UIUtils.SIPHeightPortrait;
        this.RenderTransform = (Transform) transformPortrait;
      }
      else
      {
        TranslateTransform transformPortrait = this.shiftedRootFrameTransformPortrait_;
        transformPortrait.Y = 0.0;
        this.RenderTransform = (Transform) transformPortrait;
      }
    }

    private void BindRootFrameTranslateY()
    {
      if (!(App.CurrentApp.OriginalRootFrameTransform is TransformGroup rootFrameTransform))
        return;
      Transform child = rootFrameTransform.Children[0];
      if (child == null)
        return;
      this.SetBinding(PicturePreviewPage.RootFrameTranslateYProperty, new Binding("Y")
      {
        Source = (object) child
      });
    }

    private void NotifyObserver(MediaSharingArgs.SharingStatus status = MediaSharingArgs.SharingStatus.None)
    {
      MediaSharingArgs mediaSharingArgs = new MediaSharingArgs(this.sharingState_, status, this.NavigationService);
      PageTransitionAnimation? transition = new PageTransitionAnimation?();
      switch (status)
      {
        case MediaSharingArgs.SharingStatus.Submitted:
          transition = new PageTransitionAnimation?(PageTransitionAnimation.SlideUpFadeOut);
          break;
        case MediaSharingArgs.SharingStatus.Canceled:
          transition = new PageTransitionAnimation?(PageTransitionAnimation.SlideDownFadeOut);
          break;
      }
      if (transition.HasValue)
        mediaSharingArgs.NavTransition = (Action<Action>) (onDone => Storyboarder.Perform(WaAnimations.PageTransition(transition.GetValueOrDefault()), (DependencyObject) this.LayoutRoot, false, onDone, transition.ToString()));
      this.previewObserver_.OnNext(mediaSharingArgs);
      if (status != MediaSharingArgs.SharingStatus.Submitted && status != MediaSharingArgs.SharingStatus.Canceled)
        return;
      this.sendPickerFieldStats(status);
      this.previewObserver_.OnCompleted();
    }

    private WriteableBitmap CropBitmapForThumbnail(WriteableBitmap bitmap)
    {
      if (bitmap != null)
      {
        double height = (double) bitmap.PixelWidth / (double) bitmap.PixelHeight;
        double width = (double) bitmap.PixelHeight / (double) bitmap.PixelWidth;
        if (height < 1.0)
          bitmap = bitmap.CropRelatively(new System.Windows.Point(0.0, (1.0 - height) / 2.0), new Size(1.0, height));
        if (width < 1.0)
          bitmap = bitmap.CropRelatively(new System.Windows.Point((1.0 - width) / 2.0, 0.0), new Size(width, 1.0));
      }
      return bitmap;
    }

    private void SetCurrentIndex(
      int i,
      bool forceUpdate,
      bool setCurrAsync = false,
      bool setPrev = true,
      bool setNext = true)
    {
      if (this.CurrentItem != null && this.CurrentItem.BindedMediaItem != null)
      {
        this.CurrentItem.BindedMediaItem.WriteToDrawingBitmapCache(this.highResMaxSize_);
        WriteableBitmap drawingBitmapCache = this.CurrentItem.BindedMediaItem.DrawingBitmapCache;
        if (drawingBitmapCache != null)
          this.CurrentItem.Thumbnail = (BitmapSource) this.CropBitmapForThumbnail(drawingBitmapCache);
      }
      this.EditControl.ImageSource = (BitmapSource) (this.croppingSource_ = (WriteableBitmap) null);
      i = Math.Min(this.previewItems_.Count - 1, Math.Max(i, 0));
      if (this.currentIndex_ == i && !forceUpdate)
        return;
      this.currentIndex_ = i;
      this.UpdateForCurrentItem(setCurrAsync, setPrev, setNext);
    }

    private void UpdateCaptionPanel(PicturePreviewPage.PreviewItem item, bool moveCursorToEnd = false)
    {
      if (item != null && !string.IsNullOrEmpty(item.Caption))
      {
        this.CaptionBlock.Foreground = this.isSinglePhotoPreview_ ? (Brush) UIUtils.WhiteBrush : this.captionTextBrush_;
        this.CaptionBox.Text = item.Caption;
        this.CaptionBlock.Text = new RichTextBlock.TextSet()
        {
          Text = item.Caption
        };
        if (!moveCursorToEnd)
          return;
        this.CaptionBox.TextBox.SelectionLength = 0;
        this.CaptionBox.TextBox.SelectionStart = item.Caption.Length;
      }
      else
      {
        this.CaptionBlock.Foreground = this.isSinglePhotoPreview_ ? this.singlePreviewHintBrush_ : this.captionHintBrush_;
        this.CaptionBlock.Text = new RichTextBlock.TextSet()
        {
          Text = AppResources.AddACaption
        };
        this.CaptionBox.Text = "";
      }
    }

    public void PlayGif(long delay, long previousFrameTimestamp)
    {
      if (this.stopGifPlayback)
        return;
      long localLastStartedAt = this.lastSartedAt = DateTime.Now.Ticks;
      this.Dispatcher.RunAfterDelay(TimeSpan.FromTicks(delay), (Action) (() =>
      {
        if (localLastStartedAt < this.lastSartedAt)
        {
          Log.l(nameof (PlayGif), "Terminating repeated invocation of gif display processing");
        }
        else
        {
          if (this.CurrentItem != null)
          {
            if (this.CurrentItem.BindedMediaItem.GifInfo != null)
            {
              try
              {
                MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
                if (bindedMediaItem.GifInfo.GifFrames.Count > 0)
                {
                  WriteableBitmap writeableBitmap = bindedMediaItem.GifInfo.GifFrames.Dequeue();
                  if (bindedMediaItem.RelativeCropPos.HasValue)
                    writeableBitmap = writeableBitmap.CropRelatively(bindedMediaItem.RelativeCropPos.Value, bindedMediaItem.RelativeCropSize.Value);
                  if (this.CurrentItem.RotatedTimes > 0)
                    writeableBitmap = writeableBitmap.Rotate(this.CurrentItem.RotatedTimes * 90);
                  this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap);
                }
                VideoFrameGrabber gifFrameGrabber = bindedMediaItem.GifInfo.GifFrameGrabber;
                TimeCrop? timeCrop = bindedMediaItem.GifInfo.TimeCrop;
                long num1;
                if (timeCrop.HasValue)
                {
                  timeCrop = bindedMediaItem.GifInfo.TimeCrop;
                  num1 = timeCrop.Value.StartTime.Ticks;
                }
                else
                  num1 = 0L;
                long ticks1 = num1;
                timeCrop = bindedMediaItem.GifInfo.TimeCrop;
                long num2;
                if (timeCrop.HasValue)
                {
                  timeCrop = bindedMediaItem.GifInfo.TimeCrop;
                  long ticks2 = timeCrop.Value.StartTime.Ticks;
                  timeCrop = bindedMediaItem.GifInfo.TimeCrop;
                  long ticks3 = timeCrop.Value.DesiredDuration.Ticks;
                  num2 = ticks2 + ticks3;
                }
                else
                  num2 = gifFrameGrabber.DurationTicks;
                long num3 = num2;
                VideoFrame videoFrame = gifFrameGrabber.ReadFrame();
                if (videoFrame != null && videoFrame.Timestamp > previousFrameTimestamp)
                  this.frameDelay = videoFrame.Timestamp - previousFrameTimestamp;
                if (videoFrame != null && videoFrame.Timestamp > num3)
                  videoFrame = (VideoFrame) null;
                if (videoFrame == null || videoFrame.Timestamp < ticks1)
                {
                  gifFrameGrabber.Seek(ticks1);
                  while ((videoFrame = gifFrameGrabber.ReadFrame()) != null && videoFrame.Timestamp + this.frameDelay <= ticks1)
                    ;
                }
                if (videoFrame == null || videoFrame.Timestamp > num3)
                {
                  Log.l(nameof (PlayGif), "gif or selected period seems too short {0} {1} {2}", (object) (videoFrame == null), (object) ticks1, (object) num3);
                  if (videoFrame == null)
                  {
                    gifFrameGrabber.Seek(0L);
                    videoFrame = gifFrameGrabber.ReadFrame();
                  }
                  if (videoFrame != null)
                    this.CurrentItem.BindedMediaItem.GifInfo.GifFrames.Enqueue(videoFrame.Bitmap);
                  this.stopGifPlayback = true;
                  return;
                }
                this.CurrentItem.BindedMediaItem.GifInfo.GifFrames.Enqueue(videoFrame.Bitmap);
                this.PlayGif(this.frameDelay, videoFrame.Timestamp);
                return;
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "Exception playing gif");
                this.stopGifPlayback = true;
                return;
              }
            }
          }
          Log.l(nameof (PlayGif), "No gif item found");
          this.stopGifPlayback = true;
        }
      }));
    }

    private void UpdateForCurrentItem(bool setCurrAsync, bool setPrev, bool setNext)
    {
      PicturePreviewPage.PreviewItem currentItem = this.CurrentItem;
      if (currentItem == null)
        return;
      this.StopPlayback();
      this.stopGifPlayback = true;
      this.resetTimeStripImage();
      int num = 0;
      foreach (PicturePreviewPage.PreviewItem previewItem in this.previewItems_)
        previewItem.IsBeingViewed = this.currentIndex_ == num++;
      DateTime? start1 = PerformanceTimer.Start();
      if (setCurrAsync)
      {
        this.SlideView.CenterImage.EnableScaling = false;
        currentItem.GetBitmapObservable(true, this.highResMaxSize_).ObserveOnDispatcher<BitmapSource>().Subscribe<BitmapSource>((Action<BitmapSource>) (bitmap =>
        {
          Log.p("pic preview", "curr item res updated {0}x{1}", (object) bitmap.PixelWidth, (object) bitmap.PixelHeight);
          this.SlideView.CenterImage.EnableScaling = true;
          this.SlideView.SetCenterImageSource(bitmap);
        }));
      }
      else
      {
        this.SlideView.SetCenterImageSource(currentItem.GetBitmapObservable(false, this.highResMaxSize_), false);
        this.SlideView.CenterImage.EnableScaling = true;
      }
      PerformanceTimer.End("set curr image", start1);
      DateTime? start2 = PerformanceTimer.Start();
      if (!AppState.IsLowMemoryDevice)
      {
        if (setPrev)
        {
          PicturePreviewPage.PreviewItem previewItem = this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_ - 1);
          this.SlideView.SetLeftImageSource(previewItem == null ? (IObservable<BitmapSource>) null : previewItem.GetBitmapObservable(true, this.lowResMaxSize_), true);
        }
        if (setNext)
        {
          PicturePreviewPage.PreviewItem previewItem = this.previewItems_.ElementAtOrDefault<PicturePreviewPage.PreviewItem>(this.currentIndex_ + 1);
          this.SlideView.SetRightImageSource(previewItem == null ? (IObservable<BitmapSource>) null : previewItem.GetBitmapObservable(true, this.lowResMaxSize_), true);
        }
      }
      PerformanceTimer.End("set side images", start2);
      if (currentItem.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Gif)
      {
        currentItem.BindedMediaItem.GifInfo = currentItem.BindedMediaItem.GifInfo ?? new GifArgs();
        currentItem.BindedMediaItem.GifInfo.GifFrames = new Queue<WriteableBitmap>();
        Stream stream;
        if (currentItem.BindedMediaItem is GifSharingState.Item)
          stream = ((GifSharingState.Item) currentItem.BindedMediaItem).GetGifStream();
        else if (currentItem.BindedMediaItem is ExternalSharingState.Item)
        {
          stream = ((ExternalSharingState.Item) currentItem.BindedMediaItem).GetGifStream();
        }
        else
        {
          Log.l("Preview", "Unexpected item with Gif");
          stream = this.CurrentItem.BindedMediaItem.ToPicInfo(this.highResMaxSize_).GetImageStream();
        }
        currentItem.BindedMediaItem.GifInfo.GifFrameGrabber = new VideoFrameGrabber(stream, streamFactory: (Func<Stream>) (() => (Stream) new MemoryStream()));
        this.generateMediaThumnails();
        this.DrawingMode.Visibility = this.EmojiButton.Visibility = this.PaintButton.Visibility = this.TextButton.Visibility = this.UndoButton.Visibility = Visibility.Collapsed;
        this.DurationBox.Visibility = this.PlayButton.Visibility = this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.SlideViewPanel.Visibility = Visibility.Visible;
        MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
        VideoFrame videoFrame = this.CurrentItem.BindedMediaItem.GifInfo.GifFrameGrabber.ReadFrame();
        if (videoFrame != null)
          this.CurrentItem.BindedMediaItem.GifInfo.GifFrames.Enqueue(videoFrame.Bitmap);
        this.stopGifPlayback = false;
        this.PlayGif(0L, 0L);
        this.SlideViewPanel.Visibility = Visibility.Visible;
      }
      else if (currentItem.BindedMediaItem.VideoInfo == null)
      {
        this.DurationBox.Visibility = this.PlayButton.Visibility = this.VideoPlayer.Visibility = this.VideoTimelinePanel.Visibility = this.GifButton.Visibility = Visibility.Collapsed;
        this.DrawingMode.Visibility = this.EmojiButton.Visibility = this.PaintButton.Visibility = this.TextButton.Visibility = this.UndoButton.Visibility = Visibility.Visible;
        this.addDrawingPanel();
        this.SlideViewPanel.Visibility = Visibility.Visible;
      }
      else
      {
        WaVideoArgs videoInfo = currentItem.BindedMediaItem.VideoInfo;
        this.DrawingMode.Visibility = this.EmojiButton.Visibility = this.PaintButton.Visibility = this.TextButton.Visibility = this.UndoButton.Visibility = Visibility.Collapsed;
        this.DurationBox.Visibility = this.PlayButton.Visibility = this.SlideViewPanel.Visibility = Visibility.Visible;
        this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.GifButton.Visibility = Visibility.Visible;
        this.UpdateGifButton();
        if (videoInfo.LoopingPlayback)
        {
          this.VideoToggleForeground.Fill = (Brush) UIUtils.WhiteBrush;
          this.GifToggleForeground.Fill = (Brush) UIUtils.AccentBrush;
        }
        else
        {
          this.VideoToggleForeground.Fill = (Brush) UIUtils.AccentBrush;
          this.GifToggleForeground.Fill = (Brush) UIUtils.WhiteBrush;
        }
        try
        {
          this.videoPlayerWrapper_.SetSource(new Uri(videoInfo.PreviewPlayPath));
        }
        catch (UriFormatException ex)
        {
          Log.l("video preview", "exception using preview play back uri: {0}", (object) videoInfo.PreviewPlayPath);
          Log.SendCrashLog((Exception) ex, "video preview Uri format exception", logOnlyForRelease: true);
          return;
        }
        this.Duration.Text = string.Format("{0:00}:{1:00}", (object) (videoInfo.Duration / 60), (object) (videoInfo.Duration % 60));
        if (videoInfo.OrientationAngle == -1)
        {
          string path = (string) null;
          try
          {
            path = this.sharingState_.Mode == MediaSharingState.SharingMode.TakePicture ? NativeMediaStorage.MakeUri(videoInfo.FullPath) : videoInfo.PreviewPlayPath;
            using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(path))
              videoInfo.OrientationAngle = videoFrameGrabber.GetCurrentOrientationAngle();
          }
          catch (Exception ex)
          {
            Log.l("video preview", "unexpected exception determining orientation angle: {0}", (object) path);
            Log.SendCrashLog(ex, "video preview unexpected exception", logOnlyForRelease: true);
            return;
          }
        }
      }
      this.UpdateCaptionPanel(currentItem, true);
      this.PreviewItemsList.GetLayoutUpdatedAsync().Take<Unit>(1).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.PreviewItemsList.ScrollIntoView((object) this.CurrentItem)));
    }

    private void addDrawingPanel()
    {
      if (this.CurrentItem.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Image)
        return;
      try
      {
        if (this.CurrentItem.BindedMediaItem.DrawArgs == null)
        {
          DrawingArgs drawingArgs1 = new DrawingArgs();
          Grid grid = new Grid();
          grid.HorizontalAlignment = HorizontalAlignment.Left;
          grid.VerticalAlignment = VerticalAlignment.Top;
          drawingArgs1.DrawGrid = grid;
          drawingArgs1.UndoList = new Stack<DrawingAction>();
          drawingArgs1.OriginalHeight = this.SlideView.CenterImageSource.PixelHeight;
          drawingArgs1.OriginalWidth = this.SlideView.CenterImageSource.PixelWidth;
          DrawingArgs drawingArgs2 = drawingArgs1;
          InkPresenter inkPresenter1 = new InkPresenter();
          inkPresenter1.VerticalAlignment = VerticalAlignment.Stretch;
          inkPresenter1.HorizontalAlignment = HorizontalAlignment.Stretch;
          inkPresenter1.Background = (Brush) UIUtils.TransparentBrush;
          inkPresenter1.Visibility = Visibility.Collapsed;
          InkPresenter inkPresenter2 = inkPresenter1;
          drawingArgs2.Canvas = inkPresenter2;
          inkPresenter2.MouseLeftButtonDown += new MouseButtonEventHandler(this.DrawingCanvas_MouseLeftButtonDown);
          inkPresenter2.MouseLeftButtonUp += new MouseButtonEventHandler(this.DrawingCanvas_MouseLeftButtonUp);
          inkPresenter2.MouseMove += new MouseEventHandler(this.DrawingCanvas_MouseMove);
          inkPresenter2.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.DrawingCanvas_Tap);
          drawingArgs2.DrawGrid.Children.Add((UIElement) inkPresenter2);
          this.CurrentItem.BindedMediaItem.DrawArgs = drawingArgs2;
        }
        this.DrawGrid = this.CurrentItem.BindedMediaItem.DrawArgs.DrawGrid;
        this.CurrentItemOriginalHeight = this.CurrentItem.BindedMediaItem.DrawArgs.OriginalHeight;
        this.CurrentItemOriginalWidth = this.CurrentItem.BindedMediaItem.DrawArgs.OriginalWidth;
        this.DrawingMode.Children.Clear();
        this.PaintButtonBackground.Fill = (Brush) UIUtils.TransparentBrush;
        Border border = this.newColorPicker((int) (this.screenRenderHeight_ / 2.0));
        border.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ColorPicker_Tap);
        border.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ColorPicker_ManipulationStarted);
        border.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.ColorPicker_ManipulationCompleted);
        border.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.ColorPicker_ManipulationDelta);
        Grid grid1 = new Grid();
        grid1.Height = border.Height;
        grid1.Width = 30.0;
        grid1.HorizontalAlignment = HorizontalAlignment.Right;
        grid1.VerticalAlignment = VerticalAlignment.Center;
        grid1.Visibility = Visibility.Collapsed;
        PicturePreviewPage.ColorPicker = grid1;
        PicturePreviewPage.ColorPicker.Children.Add((UIElement) border);
        this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Width = this.SlideView.CenterImage.Width;
        this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Height = this.SlideView.CenterImage.Height;
        double x = (this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Width - (double) this.CurrentItemOriginalWidth) / 2.0;
        double y = (this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Height - (double) this.CurrentItemOriginalHeight) / 2.0;
        this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect(x, y, (double) this.CurrentItemOriginalWidth, (double) this.CurrentItemOriginalHeight)
        };
        this.DrawingMode.Children.Add((UIElement) this.DrawGrid);
        this.DrawingMode.Children.Add((UIElement) PicturePreviewPage.ColorPicker);
        Grid grid2 = new Grid();
        grid2.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 153, (byte) 0, (byte) 0, (byte) 0));
        grid2.Height = 850.0 - UIUtils.SIPHeightPortrait + 2.0;
        grid2.Visibility = Visibility.Collapsed;
        grid2.VerticalAlignment = VerticalAlignment.Top;
        grid2.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.DrawingModeTextGrid = grid2;
        RichTextBlock richTextBlock = new RichTextBlock();
        richTextBlock.FontSize = (double) this.DrawTextFontSize;
        richTextBlock.TextWrapping = TextWrapping.Wrap;
        richTextBlock.Visibility = Visibility.Visible;
        richTextBlock.VerticalAlignment = VerticalAlignment.Center;
        richTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
        richTextBlock.Text = new RichTextBlock.TextSet()
        {
          Text = ""
        };
        this.DrawingModeTextBox = richTextBlock;
        this.DrawingModeTextGrid.Children.Add((UIElement) this.DrawingModeTextBox);
        this.DrawingMode.Children.Add((UIElement) this.DrawingModeTextGrid);
        this.UndoButton.Visibility = Visibility.Collapsed;
        if (this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Count > 0)
          this.UndoButton.Visibility = Visibility.Visible;
        this.SlideViewPanel.IsHitTestVisible = false;
        this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Visibility = this.DrawingMode.Visibility = Visibility.Visible;
        this.force = true;
        this.SlideView.CenterImage.LayoutUpdated += new EventHandler(this.CenterImage_LayoutUpdated);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "drawing panel failed on creation");
        this.EmojiButton.Visibility = this.PaintButton.Visibility = this.TextButton.Visibility = this.UndoButton.Visibility = Visibility.Collapsed;
      }
    }

    private Border newColorPicker(int height)
    {
      Border border = new Border();
      border.Height = (double) height;
      border.Width = 30.0;
      border.CornerRadius = new CornerRadius(10.0);
      border.VerticalAlignment = VerticalAlignment.Center;
      border.HorizontalAlignment = HorizontalAlignment.Center;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 0),
        Offset = 0.166
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, (byte) 0),
        Offset = 0.333
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, byte.MaxValue),
        Offset = 0.5
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, byte.MaxValue),
        Offset = 0.666
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue),
        Offset = 0.833
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 0, (byte) 0),
        Offset = 1.0
      });
      linearGradientBrush.GradientStops = gradientStopCollection;
      linearGradientBrush.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(0.0, 1.0);
      border.Background = (Brush) linearGradientBrush;
      return border;
    }

    private void ReloadData()
    {
      if (this.sharingState_ == null)
        return;
      this.previewItems_ = this.sharingState_.SelectedItems.Select<MediaSharingState.IItem, PicturePreviewPage.PreviewItem>((Func<MediaSharingState.IItem, PicturePreviewPage.PreviewItem>) (item => new PicturePreviewPage.PreviewItem(item))).Take<PicturePreviewPage.PreviewItem>(MediaSharingState.MaxSelectedItem).ToList<PicturePreviewPage.PreviewItem>();
      this.updatePickerFieldStatsAdd(this.previewItems_);
      if (this.previewItems_.Count == MediaSharingState.MaxSelectedItem)
        this.previewItems_.ElementAt<PicturePreviewPage.PreviewItem>(MediaSharingState.MaxSelectedItem - 1).IsLast = true;
      this.thumbListSource_ = this.previewItems_;
      this.UpdateThumbnailVisibility();
    }

    private void ReloadPageUI(bool setCurrIndex)
    {
      this.AddButton.Visibility = this.ShouldShowAddButton.ToVisibility();
      this.PreviewItemsList.ItemsSource = (IEnumerable) this.thumbListSource_;
      if (!setCurrIndex)
        return;
      this.SetCurrentIndex(this.previewItems_.Count - 1, true);
    }

    private void UpdateGifButton()
    {
      WaVideoArgs videoInfo = this.CurrentItem.BindedMediaItem.VideoInfo;
      int? nullable1 = videoInfo.TimeCrop.HasValue ? new int?((int) videoInfo.TimeCrop.Value.DesiredDuration.TotalSeconds) : videoInfo?.Duration;
      if (videoInfo != null)
      {
        int? nullable2 = nullable1;
        int num = 6;
        if ((nullable2.GetValueOrDefault() <= num ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
        {
          this.GifButton.IsEnabled = true;
          this.GifButton.Opacity = 1.0;
          return;
        }
      }
      this.GifButton.IsEnabled = false;
      this.GifButton.Opacity = 0.4;
      videoInfo.LoopingPlayback = false;
      this.VideoToggleForeground.Fill = (Brush) UIUtils.AccentBrush;
      this.GifToggleForeground.Fill = (Brush) UIUtils.WhiteBrush;
    }

    private void RotateCurrentItem()
    {
      PicturePreviewPage.PreviewItem currentItem = this.CurrentItem;
      if (currentItem == null)
        return;
      this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.ChangedPreviewItem, currentItem);
      if (!(this.SlideView.CenterImageSource is WriteableBitmap writeableBitmap1))
        writeableBitmap1 = new WriteableBitmap(this.SlideView.CenterImageSource);
      WriteableBitmap writeableBitmap2 = writeableBitmap1;
      WriteableBitmap writeableBitmap3 = writeableBitmap2.Rotate(90);
      double num1 = this.CalcImageScaleInViewMode((BitmapSource) writeableBitmap2);
      double num2 = this.CalcImageScaleInViewMode((BitmapSource) writeableBitmap3) / num1;
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.From = new double?(0.0);
      doubleAnimation1.To = new double?(90.0);
      doubleAnimation1.EasingFunction = (IEasingFunction) new SineEase();
      doubleAnimation1.Duration = (System.Windows.Duration) TimeSpan.FromMilliseconds(350.0);
      DoubleAnimation element1 = doubleAnimation1;
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("Rotation", new object[0]));
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.SliderViewTransform);
      DoubleAnimation doubleAnimation2 = new DoubleAnimation();
      doubleAnimation2.From = new double?(1.0);
      doubleAnimation2.To = new double?(num2);
      PowerEase powerEase1 = new PowerEase();
      powerEase1.EasingMode = EasingMode.EaseOut;
      powerEase1.Power = 5.0;
      doubleAnimation2.EasingFunction = (IEasingFunction) powerEase1;
      doubleAnimation2.Duration = (System.Windows.Duration) TimeSpan.FromMilliseconds(350.0);
      DoubleAnimation element2 = doubleAnimation2;
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("ScaleX", new object[0]));
      Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.SliderViewTransform);
      DoubleAnimation doubleAnimation3 = new DoubleAnimation();
      doubleAnimation3.From = new double?(1.0);
      doubleAnimation3.To = new double?(num2);
      PowerEase powerEase2 = new PowerEase();
      powerEase2.EasingMode = EasingMode.EaseOut;
      powerEase2.Power = 5.0;
      doubleAnimation3.EasingFunction = (IEasingFunction) powerEase2;
      doubleAnimation3.Duration = (System.Windows.Duration) TimeSpan.FromMilliseconds(350.0);
      DoubleAnimation element3 = doubleAnimation3;
      Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath("ScaleY", new object[0]));
      Storyboard.SetTarget((Timeline) element3, (DependencyObject) this.SliderViewTransform);
      Storyboard storyboard = new Storyboard();
      storyboard.Children.Add((Timeline) element1);
      storyboard.Children.Add((Timeline) element2);
      storyboard.Children.Add((Timeline) element3);
      this.SlideViewPanel.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      this.SlideView.CenterImage.SetupInitialDisplay();
      this.SlideView.LeftImage.Opacity = this.SlideView.RightImage.Opacity = 0.0;
      this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap3);
      this.SlideViewPanel.RenderTransformOrigin = new System.Windows.Point(0.0, 0.0);
      this.SlideView.LeftImage.Opacity = this.SlideView.RightImage.Opacity = 1.0;
      currentItem.Rotate();
      WaVideoArgs videoInfo = currentItem.BindedMediaItem.VideoInfo;
      if (videoInfo == null)
        return;
      videoInfo.OrientationAngle = this.mod(videoInfo.OrientationAngle - 90, 360);
      videoInfo.LargeThumbnail = writeableBitmap3;
      videoInfo.Thumbnail = writeableBitmap3;
      if (this.VideoPlayer.RenderTransform == null || !(this.VideoPlayer.RenderTransform is CompositeTransform))
        this.VideoPlayer.RenderTransform = (Transform) new CompositeTransform();
      ((CompositeTransform) this.VideoPlayer.RenderTransform).Rotation += 90.0;
      if (this.VideoTimelineList == null)
        return;
      foreach (PicturePreviewPage.VideoFrameItem videoFrameItem in this.VideoTimelineList.ItemsSource)
        videoFrameItem.ThumbRotationAngle += 90;
    }

    private int mod(int x, int m) => (x % m + m) % m;

    private void DeleteCurrentItem()
    {
      this.stopGifPlayback = true;
      if (this.PlayingVideo)
        this.StopPlayback();
      this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.DeletedPreviewItem, this.CurrentItem);
      if (this.previewItems_.Count <= 1)
      {
        if (this.sharingState_.Mode == MediaSharingState.SharingMode.TakePicture)
          this.NotifyObserver();
        else
          NavUtils.GoBack(this.NavigationService);
      }
      if (this.previewItems_.Count <= 0)
        return;
      int currentIndex = this.currentIndex_;
      this.sharingState_.DeleteItem(this.previewItems_[currentIndex].BindedMediaItem);
      this.SetCurrentIndex(currentIndex, true);
    }

    private Storyboard GetFadeInAnimation(double durationInMs = 250.0)
    {
      if (this.fadeInSb_ == null)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = new double?(0.0);
        doubleAnimation.To = new double?(1.0);
        ExponentialEase exponentialEase = new ExponentialEase();
        exponentialEase.Exponent = 2.0;
        exponentialEase.EasingMode = EasingMode.EaseIn;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Opacity", new object[0]));
        this.fadeInSb_ = new Storyboard();
        this.fadeInSb_.Children.Add((Timeline) element);
        this.fadeInAnimation_ = element;
      }
      this.fadeInAnimation_.Duration = (System.Windows.Duration) TimeSpan.FromMilliseconds(durationInMs);
      return this.fadeInSb_;
    }

    private double CalcImageScaleInViewMode(BitmapSource bitmap)
    {
      return Math.Min(this.SlideView.ActualWidth / (double) bitmap.PixelWidth, this.SlideView.ActualHeight / (double) bitmap.PixelHeight);
    }

    private Storyboard GetCropTransition(
      double scaleInCropMode,
      Size spaceForCropMode,
      bool enterCrop)
    {
      TimeSpan duration1 = TimeSpan.FromMilliseconds(150.0);
      int num1 = enterCrop ? 1 : 0;
      TimeSpan duration2 = TimeSpan.FromMilliseconds(100.0);
      StackPanel captionAndThumbList = this.CaptionAndThumbList;
      PowerEase easeFunc1;
      if (!enterCrop)
      {
        easeFunc1 = (PowerEase) null;
      }
      else
      {
        easeFunc1 = new PowerEase();
        easeFunc1.EasingMode = EasingMode.EaseIn;
        easeFunc1.Power = 5.0;
      }
      DoubleAnimation doubleAnimation1 = WaAnimations.Fade((WaAnimations.FadeType) num1, duration2, (DependencyObject) captionAndThumbList, (EasingFunctionBase) easeFunc1);
      if (enterCrop)
        doubleAnimation1.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(50.0));
      double fromY = enterCrop ? 0.0 : 200.0;
      double toY1 = enterCrop ? 200.0 : 0.0;
      TimeSpan duration3 = enterCrop ? duration1 : TimeSpan.FromMilliseconds(350.0);
      CompositeTransform thumbListTransform = this.CaptionAndThumbListTransform;
      EasingFunctionBase easeFunc2;
      if (!enterCrop)
      {
        ExponentialEase exponentialEase = new ExponentialEase();
        exponentialEase.EasingMode = EasingMode.EaseInOut;
        exponentialEase.Exponent = 8.0;
        easeFunc2 = (EasingFunctionBase) exponentialEase;
      }
      else
      {
        easeFunc2 = (EasingFunctionBase) new QuinticEase();
        easeFunc2.EasingMode = EasingMode.EaseIn;
      }
      DoubleAnimation doubleAnimation2 = WaAnimations.VerticalSlide(fromY, toY1, duration3, (DependencyObject) thumbListTransform, easeFunc2, "TranslateY");
      double num2 = this.CalcImageScaleInViewMode(this.SlideView.CenterImageSource);
      double num3 = scaleInCropMode / num2;
      DoubleAnimation doubleAnimation3 = new DoubleAnimation();
      doubleAnimation3.From = new double?(enterCrop ? 1.0 : num3);
      doubleAnimation3.To = new double?(enterCrop ? num3 : 1.0);
      doubleAnimation3.Duration = (System.Windows.Duration) duration1;
      DoubleAnimation element1 = doubleAnimation3;
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.SliderViewTransform);
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("ScaleX", new object[0]));
      DoubleAnimation doubleAnimation4 = new DoubleAnimation();
      doubleAnimation4.From = new double?(enterCrop ? 1.0 : num3);
      doubleAnimation4.To = new double?(enterCrop ? num3 : 1.0);
      doubleAnimation4.Duration = (System.Windows.Duration) duration1;
      DoubleAnimation element2 = doubleAnimation4;
      Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.SliderViewTransform);
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("ScaleY", new object[0]));
      double num4 = this.SlideView.ActualWidth * num3;
      double num5 = this.SlideView.ActualHeight * num3;
      double num6 = (spaceForCropMode.Width - num4) / 2.0;
      double num7 = (spaceForCropMode.Height - num5) / 2.0;
      DoubleAnimation doubleAnimation5 = new DoubleAnimation();
      doubleAnimation5.From = new double?(enterCrop ? 0.0 : num6);
      doubleAnimation5.To = new double?(enterCrop ? num6 : 0.0);
      doubleAnimation5.Duration = (System.Windows.Duration) duration1;
      DoubleAnimation element3 = doubleAnimation5;
      Storyboard.SetTarget((Timeline) element3, (DependencyObject) this.SliderViewTransform);
      Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath("TranslateX", new object[0]));
      DoubleAnimation doubleAnimation6 = new DoubleAnimation();
      doubleAnimation6.From = new double?(enterCrop ? 0.0 : num7);
      doubleAnimation6.To = new double?(enterCrop ? num7 : 0.0);
      doubleAnimation6.Duration = (System.Windows.Duration) duration1;
      DoubleAnimation element4 = doubleAnimation6;
      Storyboard.SetTarget((Timeline) element4, (DependencyObject) this.SliderViewTransform);
      Storyboard.SetTargetProperty((Timeline) element4, new PropertyPath("TranslateY", new object[0]));
      DoubleAnimation doubleAnimation7 = WaAnimations.Fade(enterCrop ? WaAnimations.FadeType.FadeOut : WaAnimations.FadeType.FadeIn, duration1, (DependencyObject) this.AppBarButtonsPanel);
      double toY2 = enterCrop ? -24.0 : 0.0;
      TimeSpan duration4 = duration1;
      CompositeTransform buttonsPanelTransform = this.AppBarButtonsPanelTransform;
      BackEase easeFunc3;
      if (!enterCrop)
      {
        BackEase backEase = new BackEase();
        backEase.Amplitude = 0.3;
        backEase.EasingMode = EasingMode.EaseOut;
        easeFunc3 = backEase;
      }
      else
        easeFunc3 = (BackEase) null;
      DoubleAnimation doubleAnimation8 = WaAnimations.VerticalSlide(0.0, toY2, duration4, (DependencyObject) buttonsPanelTransform, (EasingFunctionBase) easeFunc3, "TranslateY");
      DoubleAnimation doubleAnimation9 = WaAnimations.Fade(enterCrop ? WaAnimations.FadeType.FadeOut : WaAnimations.FadeType.FadeIn, duration1, (DependencyObject) this.CropControlPanel);
      double toY3 = enterCrop ? 0.0 : -24.0;
      TimeSpan duration5 = duration1;
      CompositeTransform controlPanelTransform = this.CropControlPanelTransform;
      BackEase easeFunc4;
      if (!enterCrop)
      {
        easeFunc4 = (BackEase) null;
      }
      else
      {
        easeFunc4 = new BackEase();
        easeFunc4.Amplitude = 0.3;
        easeFunc4.EasingMode = EasingMode.EaseOut;
      }
      DoubleAnimation doubleAnimation10 = WaAnimations.VerticalSlide(0.0, toY3, duration5, (DependencyObject) controlPanelTransform, (EasingFunctionBase) easeFunc4, "TranslateY");
      Storyboard cropTransition = new Storyboard();
      cropTransition.Children.Add((Timeline) doubleAnimation1);
      cropTransition.Children.Add((Timeline) doubleAnimation2);
      cropTransition.Children.Add((Timeline) element1);
      cropTransition.Children.Add((Timeline) element2);
      cropTransition.Children.Add((Timeline) element3);
      cropTransition.Children.Add((Timeline) element4);
      cropTransition.Children.Add((Timeline) doubleAnimation7);
      cropTransition.Children.Add((Timeline) doubleAnimation8);
      cropTransition.Children.Add((Timeline) doubleAnimation9);
      cropTransition.Children.Add((Timeline) doubleAnimation10);
      return cropTransition;
    }

    private Storyboard GetEditControlFadeTransition(bool enterCrop)
    {
      DoubleAnimation doubleAnimation1 = WaAnimations.Fade(enterCrop ? WaAnimations.FadeType.FadeIn : WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(350.0), (DependencyObject) this.EditControl.BackImage);
      DoubleAnimation doubleAnimation2 = WaAnimations.Fade(enterCrop ? WaAnimations.FadeType.FadeIn : WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(350.0), (DependencyObject) this.EditControl.CroppingBorder);
      Storyboard controlFadeTransition = new Storyboard();
      controlFadeTransition.Children.Add((Timeline) doubleAnimation1);
      controlFadeTransition.Children.Add((Timeline) doubleAnimation2);
      return controlFadeTransition;
    }

    private Pair<System.Windows.Point, Size> TranslateCropping(
      System.Windows.Point relativeCropPos,
      Size relativeCropSize,
      int clockwiseRotatedTimes)
    {
      double x;
      double y;
      double width;
      double height;
      switch (clockwiseRotatedTimes % 4)
      {
        case 1:
          x = 1.0 - relativeCropPos.Y - relativeCropSize.Height;
          y = relativeCropPos.X;
          width = relativeCropSize.Height;
          height = relativeCropSize.Width;
          break;
        case 2:
          x = 1.0 - relativeCropPos.X - relativeCropSize.Width;
          y = 1.0 - relativeCropPos.Y - relativeCropSize.Height;
          width = relativeCropSize.Width;
          height = relativeCropSize.Height;
          break;
        case 3:
          x = relativeCropPos.Y;
          y = 1.0 - relativeCropPos.X - relativeCropSize.Width;
          width = relativeCropSize.Height;
          height = relativeCropSize.Width;
          break;
        default:
          x = relativeCropPos.X;
          y = relativeCropPos.Y;
          width = relativeCropSize.Width;
          height = relativeCropSize.Height;
          break;
      }
      return new Pair<System.Windows.Point, Size>(new System.Windows.Point(x, y), new Size(width, height));
    }

    private void EnterCrop()
    {
      this.CloseEmojiKeyboard();
      if (this.PlayingVideo)
        this.videoPlayerWrapper_.Stop();
      this.pendingSbSub_.SafeDispose();
      this.pendingSbSub_ = (IDisposable) null;
      this.DurationBox.Visibility = Visibility.Collapsed;
      this.PlayButton.Visibility = Visibility.Collapsed;
      this.VideoPlayer.Visibility = Visibility.Collapsed;
      this.ThumbnailsPanel.Visibility = Visibility.Collapsed;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      if (bindedMediaItem == null)
        return;
      this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.ChangedPreviewItem, this.CurrentItem);
      if (bindedMediaItem.VideoInfo != null)
        this.VideoTimelinePanel.Visibility = Visibility.Collapsed;
      this.DeleteButton.IsEnabled = this.RotateButton.IsEnabled = false;
      this.isCropping_ = true;
      if (this.croppingSource_ == null || bindedMediaItem.DrawArgs != null && bindedMediaItem.DrawArgs.HasDrawing)
      {
        WriteableBitmap img = this.croppingSource_ = bindedMediaItem.GetBitmap(this.highResMaxSize_, false, withDrawing: true);
        this.EditControl.MinRelativeCropSize = new Size?(new Size(64.0 / (double) img.PixelWidth, 64.0 / (double) img.PixelHeight));
        if (bindedMediaItem.RelativeCropPos.HasValue && bindedMediaItem.RelativeCropSize.HasValue)
        {
          Pair<System.Windows.Point, Size> pair = this.TranslateCropping(bindedMediaItem.RelativeCropPos.Value, bindedMediaItem.RelativeCropSize.Value, bindedMediaItem.RotatedTimes);
          this.EditControl.Setup((BitmapSource) img, ImageEditControl.CroppingMode.Custom, pair.First, pair.Second, new Size?(this.cropSpaceSize_));
        }
        else
          this.EditControl.Setup((BitmapSource) img, ImageEditControl.CroppingMode.Custom, (double) img.PixelWidth / (double) img.PixelHeight, new Size?(this.cropSpaceSize_));
      }
      double currentScale = this.EditControl.CurrentScale;
      this.AppBarButtonsPanel.IsHitTestVisible = false;
      this.SlideView.Opacity = this.SlideView.LeftImage.Opacity = this.SlideView.RightImage.Opacity = this.DrawingMode.Opacity = 0.0;
      Storyboard cropTransition = this.GetCropTransition(currentScale, this.cropSpaceSize_, true);
      this.SlideView.CenterImage.SetupInitialDisplay();
      this.pendingSbSub_ = Storyboarder.PerformWithDisposable(cropTransition, (DependencyObject) null, true, (Action) (() =>
      {
        this.CaptionAndThumbList.Visibility = Visibility.Collapsed;
        this.CropBar.Visibility = Visibility.Visible;
        this.AppBarButtonsPanel.Opacity = 0.0;
        this.EditControl.Visibility = Visibility.Visible;
        this.pendingSbSub_ = (IDisposable) null;
        this.pendingSbSub_ = Storyboarder.PerformWithDisposable(this.GetEditControlFadeTransition(true), (DependencyObject) null, true, (Action) (() =>
        {
          this.EditControl.BackImage.Opacity = this.EditControl.CroppingBorder.Opacity = 1.0;
          this.pendingSbSub_ = (IDisposable) null;
        }), false, "pic preview: enter crop part 2");
      }), false, "pic preview: enter crop part 1");
    }

    private void ExitCrop(bool applyChanges)
    {
      this.pendingSbSub_.SafeDispose();
      this.pendingSbSub_ = (IDisposable) null;
      if (this.CurrentItem?.BindedMediaItem?.GifInfo != null)
        this.VideoTimelinePanel.Visibility = Visibility.Visible;
      else if (this.CurrentItem?.BindedMediaItem.VideoInfo != null)
      {
        this.DurationBox.Visibility = Visibility.Visible;
        this.PlayButton.Visibility = Visibility.Visible;
        this.SlideViewPanel.Visibility = Visibility.Visible;
        this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.VideoTimelinePanel.Visibility = Visibility.Visible;
      }
      this.isCropping_ = false;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem?.BindedMediaItem;
      if (applyChanges)
      {
        Pair<System.Windows.Point, Size> croppingState = this.EditControl.GetCroppingState(true);
        if (croppingState != null && bindedMediaItem != null)
        {
          Pair<System.Windows.Point, Size> pair1 = this.TranslateCropping(croppingState.First, croppingState.Second, 4 - this.CurrentItem.RotatedTimes);
          double num1 = 1E-06;
          System.Windows.Point? relativeCropPos = bindedMediaItem.RelativeCropPos;
          if (relativeCropPos.HasValue && bindedMediaItem.RelativeCropSize.HasValue)
          {
            relativeCropPos = bindedMediaItem.RelativeCropPos;
            if (Math.Abs(relativeCropPos.Value.X - pair1.First.X) <= num1)
            {
              relativeCropPos = bindedMediaItem.RelativeCropPos;
              if (Math.Abs(relativeCropPos.Value.Y - pair1.First.Y) <= num1 && Math.Abs(bindedMediaItem.RelativeCropSize.Value.Width - pair1.Second.Width) <= num1 && Math.Abs(bindedMediaItem.RelativeCropSize.Value.Height - pair1.Second.Height) <= num1)
                goto label_21;
            }
          }
          bindedMediaItem.RelativeCropPos = new System.Windows.Point?(pair1.First);
          bindedMediaItem.RelativeCropSize = new Size?(pair1.Second);
          this.SlideView.SetCenterImageSource((BitmapSource) this.CurrentItem.BindedMediaItem.GetBitmap(this.highResMaxSize_, false).CropRelatively(croppingState.First, croppingState.Second));
          int scale = 94 / Math.Min(this.SlideView.CenterImageSource.PixelWidth, this.SlideView.CenterImageSource.PixelHeight);
          if ((double) scale < 1.0)
          {
            if (bindedMediaItem.DrawArgs != null && bindedMediaItem.DrawArgs.HasDrawing)
            {
              this.CurrentItem.BindedMediaItem.WriteToDrawingBitmapCache(this.highResMaxSize_);
              this.CurrentItem.Thumbnail = (BitmapSource) this.CropBitmapForThumbnail(bindedMediaItem.DrawingBitmapCache);
            }
            else
              this.CurrentItem.Thumbnail = this.SlideView.CenterImageSource;
            if (bindedMediaItem.VideoInfo != null)
            {
              if (!(this.SlideView.CenterImageSource is WriteableBitmap writeableBitmap1))
                writeableBitmap1 = new WriteableBitmap(this.SlideView.CenterImageSource);
              WriteableBitmap writeableBitmap2 = writeableBitmap1;
              bindedMediaItem.VideoInfo.LargeThumbnail = writeableBitmap2;
              bindedMediaItem.VideoInfo.Thumbnail = writeableBitmap2;
            }
          }
          else
          {
            if (!(this.SlideView.CenterImageSource is WriteableBitmap writeableBitmap))
              writeableBitmap = new WriteableBitmap(this.SlideView.CenterImageSource);
            WriteableBitmap bitmap = writeableBitmap;
            this.CurrentItem.Thumbnail = (BitmapSource) bitmap.Scale((double) scale);
            if (bindedMediaItem.VideoInfo != null)
            {
              bindedMediaItem.VideoInfo.LargeThumbnail = bitmap;
              bindedMediaItem.VideoInfo.Thumbnail = bitmap.Scale((double) scale);
            }
          }
label_21:
          if (bindedMediaItem.VideoInfo != null)
          {
            int num2;
            switch (bindedMediaItem.VideoInfo.OrientationAngle)
            {
              case 90:
                num2 = 1;
                break;
              case 180:
                num2 = 2;
                break;
              case 270:
                num2 = 3;
                break;
              default:
                num2 = 0;
                break;
            }
            int clockwiseRotatedTimes = num2;
            bindedMediaItem.VideoInfo.AbsoluteCropData = new Triad<System.Windows.Point, Size, int>();
            bindedMediaItem.VideoInfo.AbsoluteCropData.First = croppingState.First;
            bindedMediaItem.VideoInfo.AbsoluteCropData.Second = croppingState.Second;
            bindedMediaItem.VideoInfo.AbsoluteCropData.Third = clockwiseRotatedTimes;
            Pair<System.Windows.Point, Size> pair2 = this.TranslateCropping(croppingState.First, croppingState.Second, clockwiseRotatedTimes);
            System.Windows.Point first = pair2.First;
            Size second = pair2.Second;
            using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(bindedMediaItem.VideoInfo.PreviewPlayPath))
            {
              this.CurrentItem.BindedMediaItem.VideoInfo.CropRectangle = new CropRectangle?(new CropRectangle()
              {
                Height = (int) (second.Height * (double) videoFrameGrabber.FrameInfo.Height),
                Width = (int) (second.Width * (double) videoFrameGrabber.FrameInfo.Width),
                XOffset = (int) (first.X * (double) videoFrameGrabber.FrameInfo.Width),
                YOffset = (int) (first.Y * (double) videoFrameGrabber.FrameInfo.Height)
              });
              double num3 = this.LayoutRoot.ActualWidth * ((double) videoFrameGrabber.FrameInfo.Height / (double) videoFrameGrabber.FrameInfo.Width);
              double num4 = this.LayoutRoot.ActualWidth;
              if ((double) videoFrameGrabber.FrameInfo.Height > (double) videoFrameGrabber.FrameInfo.Width)
              {
                num4 = this.LayoutRoot.ActualWidth * ((double) videoFrameGrabber.FrameInfo.Width / (double) videoFrameGrabber.FrameInfo.Height);
                num3 = this.LayoutRoot.ActualWidth;
              }
              double width = second.Width * num4;
              double height = second.Height * num3;
              double x = first.X * num4;
              double y = first.Y * num3;
              this.VideoCropRectangle.Rect = new Rect(new System.Windows.Point(x, y), new Size(width, height));
              double num5 = width > height ? this.LayoutRoot.ActualWidth / width : this.LayoutRoot.ActualWidth / height;
              this.VideoPlayer.RenderTransformOrigin = new System.Windows.Point((x + width / 2.0) / num4, (y + height / 2.0) / num3);
              CompositeTransform renderTransform = (CompositeTransform) this.VideoPlayer.RenderTransform;
              renderTransform.Rotation += 0.01;
              renderTransform.TranslateX = (num4 - width) / 2.0 - x;
              renderTransform.TranslateY = (num3 - height) / 2.0 - y;
              renderTransform.ScaleX = num5;
              renderTransform.ScaleY = num5;
              this.VideoPlayer.RenderTransform = (Transform) renderTransform;
            }
          }
        }
        else if (bindedMediaItem != null)
        {
          this.SlideView.SetCenterImageSource((BitmapSource) bindedMediaItem.GetBitmap(this.highResMaxSize_, false));
          bindedMediaItem.RelativeCropPos = new System.Windows.Point?();
          bindedMediaItem.RelativeCropSize = new Size?();
          if (bindedMediaItem.VideoInfo != null)
          {
            CompositeTransform renderTransform = (CompositeTransform) this.VideoPlayer.RenderTransform;
            renderTransform.ScaleX = 1.0;
            renderTransform.ScaleY = 1.0;
            renderTransform.TranslateX = 0.0;
            renderTransform.TranslateY = 0.0;
            this.VideoPlayer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            this.VideoCropRectangle.Rect = new Rect(new System.Windows.Point(0.0, 0.0), new Size(480.0, 800.0));
            bindedMediaItem.VideoInfo.AbsoluteCropData = (Triad<System.Windows.Point, Size, int>) null;
            bindedMediaItem.VideoInfo.CropRectangle = new CropRectangle?();
          }
        }
      }
      else if (bindedMediaItem != null && bindedMediaItem.RelativeCropPos.HasValue && bindedMediaItem.RelativeCropSize.HasValue)
      {
        Pair<System.Windows.Point, Size> pair = this.TranslateCropping(bindedMediaItem.RelativeCropPos.Value, bindedMediaItem.RelativeCropSize.Value, bindedMediaItem.RotatedTimes);
        this.EditControl.SetCropping(pair.First, pair.Second, true);
      }
      else if (this.croppingSource_ != null)
        this.EditControl.SetCropping((double) this.croppingSource_.PixelWidth / (double) this.croppingSource_.PixelHeight);
      this.SlideView.Opacity = 1.0;
      this.EditControl.Visibility = Visibility.Collapsed;
      this.EditControl.BackImage.Opacity = this.EditControl.CroppingBorder.Opacity = 0.0;
      if (this.ShouldShowThumbnailRow)
        this.ThumbnailsPanel.Visibility = Visibility.Visible;
      this.UpdateDrawingPanelSize();
      this.pendingSbSub_ = Storyboarder.PerformWithDisposable(this.GetCropTransition(this.EditControl.CurrentScale, this.cropSpaceSize_, false), onComplete: (Action) (() =>
      {
        this.CaptionAndThumbList.Opacity = this.SlideView.Opacity = this.SlideView.LeftImage.Opacity = this.SlideView.RightImage.Opacity = this.DrawingMode.Opacity = 1.0;
        this.CaptionAndThumbList.IsHitTestVisible = this.SlideView.IsHitTestVisible = true;
        this.CropBar.Visibility = Visibility.Collapsed;
        this.CaptionAndThumbList.Visibility = Visibility.Visible;
        this.AppBarButtonsPanel.Opacity = 1.0;
        this.AppBarButtonsPanel.IsHitTestVisible = true;
        this.DeleteButton.IsEnabled = this.RotateButton.IsEnabled = true;
        this.pendingSbSub_ = (IDisposable) null;
      }), callOnCompleteOnDisposing: true, context: "pic preview: exit crop part 2");
    }

    private static void AdjustForCrop(
      MediaSharingState.IItem currentMediaItem,
      ref CompositeTransform currentTransform)
    {
      if (currentMediaItem == null)
        return;
      if (currentTransform == null)
        return;
      try
      {
        if (!currentMediaItem.RelativeCropPos.HasValue || !currentMediaItem.RelativeCropSize.HasValue)
          return;
        System.Windows.Point point = currentMediaItem.RelativeCropPos.Value;
        Size size = currentMediaItem.RelativeCropSize.Value;
        double originalWidth = (double) currentMediaItem.DrawArgs.OriginalWidth;
        double num1 = originalWidth / 2.0;
        double num2 = size.Width * originalWidth / 2.0 + point.X * originalWidth;
        double originalHeight = (double) currentMediaItem.DrawArgs.OriginalHeight;
        double num3 = originalHeight / 2.0;
        double num4 = size.Height * originalHeight / 2.0 + point.Y * originalHeight - num3;
        double num5 = num1;
        double num6 = num2 - num5;
        switch (currentMediaItem.RotatedTimes % 4)
        {
          case 1:
            double num7 = num4;
            num4 = num6;
            num6 = -num7;
            break;
          case 2:
            num4 = -num4;
            num6 = -num6;
            break;
          case 3:
            double num8 = num4;
            num4 = -num6;
            num6 = num8;
            break;
        }
        currentTransform.TranslateY = num4;
        currentTransform.TranslateX = num6;
        Log.d("PircturePreview", "adjusted for crop {0}", (object) (currentMediaItem.RotatedTimes % 4));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception adjusting for crop");
      }
    }

    private bool ShouldBlockButtonClick(int blockThreshold = 500)
    {
      DateTime now = DateTime.Now;
      if (this.lastButtonClickedAt_.HasValue && now - this.lastButtonClickedAt_.Value < TimeSpan.FromMilliseconds((double) blockThreshold))
        return true;
      this.lastButtonClickedAt_ = new DateTime?(now);
      return false;
    }

    public BitmapImage PlayButtonIcon => ImageStore.GetStockIcon("/Images/play.png");

    public BitmapImage StopButtonIconUri => ImageStore.GetStockIcon("/Images/stop.png");

    private void StartPlaybackTimer()
    {
      if (this.playbackTimer == null)
      {
        this.playbackTimer = new DispatcherTimer();
        this.playbackTimer.Tick += new EventHandler(this.Timer_Tick);
        this.playbackTimer.Interval = TimeSpan.FromMilliseconds(25.0);
      }
      this.recordingStartTime = DateTime.Now;
      this.timerDuration = 0;
      this.playbackTimer.Start();
    }

    private void StopPlaybackTimer()
    {
      if (this.playbackTimer == null)
        return;
      this.playbackTimer.Stop();
    }

    private void StartTimeCropTimer()
    {
      if (this.timeCropTimer == null)
      {
        this.timeCropTimer = new DispatcherTimer();
        this.timeCropTimer.Tick += new EventHandler(this.timeCropTimer_Tick);
        this.timeCropTimer.Interval = TimeSpan.FromMilliseconds(200.0);
      }
      this.timeCropManipulation = false;
      this.timeCropTimer.Start();
    }

    private void StopTimeCropTimer()
    {
      if (this.timeCropTimer == null)
        return;
      this.timeCropTimer.Stop();
    }

    private void timeCropTimer_Tick(object sender, EventArgs e) => this.timeCropManipulation = true;

    private void StartPlayback()
    {
      try
      {
        long num1 = (long) this.RightHandleTransform.TranslateX - 450L;
        long ticks;
        int num2 = (int) (ticks = (long) ((double) this.CurrentItem.BindedMediaItem.VideoInfo.Duration / 1E-07)) / (int) this.LayoutRoot.ActualWidth;
        if (this.VideoPlayer.Position >= new TimeSpan(ticks) + new TimeSpan(num1 * (long) num2) - TimeSpan.FromMilliseconds(5.0))
          this.VideoPlayer.Position = !this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop.HasValue ? new TimeSpan(0L) : this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop.Value.StartTime;
        this.PlayButton.ButtonIcon = (BitmapSource) this.StopButtonIconUri;
        this.PlayingVideo = true;
        if (this.videoPlayerWrapper_ != null && this.CurrentItem != null && this.videoPlayerWrapper_.Duration.TimeSpan == TimeSpan.Zero)
        {
          Log.l("video preview", "play back uri");
          this.videoPlayerWrapper_.Play(new Uri(this.CurrentItem.BindedMediaItem.VideoInfo.PreviewPlayPath));
        }
        else
        {
          Log.l("video preview", "play back");
          this.videoPlayerWrapper_.Play();
        }
        this.StartPlaybackTimer();
        this.VideoPlayer.Visibility = Visibility.Visible;
        this.SlideViewPanel.Visibility = Visibility.Collapsed;
        if (this.CurrentItem.BindedMediaItem.VideoInfo == null)
          return;
        this.CurrentTimeStrip.Opacity = 1.0;
        Storyboard resource = this.Resources[(object) "AnimationList"] as Storyboard;
        DoubleAnimation child = resource.Children[0] as DoubleAnimation;
        child.From = new double?(1.0);
        child.To = new double?(0.0);
        Storyboarder.Perform(resource, false);
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "play just recorded video");
      }
    }

    private void StopPlayback()
    {
      if (!this.PlayingVideo || this.CurrentItem == null)
        return;
      this.StopPlaybackTimer();
      this.PlayingVideo = false;
      this.PlayButton.ButtonIcon = (BitmapSource) this.PlayButtonIcon;
      if (this.CurrentItem.BindedMediaItem.VideoInfo != null && this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop.HasValue)
      {
        this.VideoPlayer.Pause();
        this.VideoPlayer.Position = this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop.Value.StartTime;
      }
      else
        this.videoPlayerWrapper_.Stop();
      this.resetTimeStripImage();
      this.updateDurationText();
      Storyboard resource = this.Resources[(object) "AnimationList"] as Storyboard;
      DoubleAnimation child = resource.Children[0] as DoubleAnimation;
      child.From = new double?(0.0);
      child.To = new double?(1.0);
      Storyboarder.Perform(resource, false);
    }

    private void PausePlayback()
    {
      if (!this.PlayingVideo || this.CurrentItem == null)
        return;
      this.StopPlaybackTimer();
      this.PlayingVideo = false;
      this.PlayButton.ButtonIcon = (BitmapSource) this.PlayButtonIcon;
      this.videoPlayerWrapper_.Pause();
      this.updateDurationText();
      Storyboard resource = this.Resources[(object) "AnimationList"] as Storyboard;
      DoubleAnimation child = resource.Children[0] as DoubleAnimation;
      child.From = new double?(0.0);
      child.To = new double?(1.0);
      Storyboarder.Perform(resource, false);
    }

    private void updateDurationText()
    {
      WaVideoArgs videoInfo = this.CurrentItem.BindedMediaItem.VideoInfo;
      if (videoInfo == null)
        return;
      int num1 = videoInfo.Duration / 60;
      int num2 = videoInfo.Duration % 60;
      TimeCrop? timeCrop1 = videoInfo.TimeCrop;
      if (timeCrop1.HasValue)
      {
        timeCrop1 = videoInfo.TimeCrop;
        TimeCrop timeCrop2 = timeCrop1.Value;
        num1 = timeCrop2.DesiredDuration.Minutes;
        timeCrop1 = videoInfo.TimeCrop;
        timeCrop2 = timeCrop1.Value;
        num2 = timeCrop2.DesiredDuration.Seconds;
      }
      this.Duration.Text = string.Format("{0:00}:{1:00}", (object) num1, (object) num2);
      this.Duration.Opacity = 1.0;
      this.UpdateGifButton();
    }

    private void resetTimeStripImage()
    {
      this.CurrentTimeStrip.Opacity = 0.0;
      this.CurrentTimeStripTranslate.TranslateX = this.LeftHandleTransform.TranslateX + 450.0;
    }

    private void Timer_Tick(object sender, object e)
    {
      try
      {
        TimeSpan timeSpan = DateTime.Now - this.recordingStartTime;
        this.CurrentTimeStripTranslate.TranslateX = (double) (this.VideoPlayer.Position.Ticks / (long) ((int) ((double) this.CurrentItem.BindedMediaItem.VideoInfo.Duration / 1E-07) / (int) this.LayoutRoot.ActualWidth));
        this.timerDuration = (int) timeSpan.TotalSeconds;
        if (this.CurrentTimeStripTranslate.TranslateX <= (double) (long) this.RightHandleTransform.TranslateX)
          return;
        this.StopPlayback();
        this.resetTimeStripImage();
        this.StopPlaybackTimer();
      }
      catch (Exception ex)
      {
        this.StopPlaybackTimer();
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.sharingState_ == null || this.previewItems_ == null || this.previewItems_.Count == 0 || this.previewObserver_ == null)
      {
        Log.WriteLineDebug("picture preview: missing instance args");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
        CameraPage.TakePictureOnly = false;
      }
      else
      {
        if (this.NavigationService.SearchBackStack("CameraPage") == 0)
          this.NavigationService.RemoveBackEntry();
        if (this.sharingState_.Mode == MediaSharingState.SharingMode.TakePicture)
          CameraPage.TakePictureOnly = true;
        this.LayoutRoot.Visibility = Visibility.Visible;
        this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.SlideViewPanel.Visibility = Visibility.Visible;
        this.BindRootFrameTranslateY();
        if (this.videoPlayerWrapper_ != null && this.CurrentItem != null && this.videoPlayerWrapper_.Duration.TimeSpan == TimeSpan.Zero && this.CurrentItem.BindedMediaItem != null && this.CurrentItem.BindedMediaItem.VideoInfo != null)
          this.videoPlayerWrapper_.SetSource(new Uri(this.CurrentItem.BindedMediaItem.VideoInfo.PreviewPlayPath));
        this.InputMode = ChatPage.InputMode.None;
        if (e.NavigationMode == NavigationMode.Back && this.CurrentItem?.BindedMediaItem != null && this.CurrentItem.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Image)
          this.addDrawingPanel();
      }
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.InputMode = ChatPage.InputMode.None;
      this.LayoutRoot.Visibility = Visibility.Collapsed;
      this.ClearValue(PicturePreviewPage.RootFrameTranslateYProperty);
      this.StopPlaybackTimer();
      this.StopPlayback();
      this.stopGifPlayback = true;
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.sharingStateItemsChangedSub_.SafeDispose();
      this.sharingStateItemsChangedSub_ = (IDisposable) null;
      this.captionFocusChangedSub.SafeDispose();
      this.captionFocusChangedSub = (IDisposable) null;
      this.videoPlayerWrapper_?.Detach();
      if (this.previewObserver_ != null)
        this.previewObserver_.OnCompleted();
      this.videoFrameSubs.SafeDispose();
      this.videoFrameSubs = (CompositeDisposable) null;
      if (this.sharingState_ != null && !this.submitted_ && !this.onback && this.sharingState_.SelectedItems.Count<MediaSharingState.IItem>() > 0 && this.sharingState_.Mode == MediaSharingState.SharingMode.TakePicture)
      {
        Log.p("pic preview", "leaving preview page via notification or crash, saving preview page pictures");
        List<MediaSharingState.PicInfo> picInfoList = new List<MediaSharingState.PicInfo>();
        foreach (MediaSharingState.IItem selectedItem in this.sharingState_.SelectedItems)
        {
          if (selectedItem.GetMediaType() == FunXMPP.FMessage.Type.Image)
            picInfoList.Add(selectedItem.ToPicInfo(new Size((double) Settings.ImageMaxEdge, (double) Settings.ImageMaxEdge)));
        }
        int count = picInfoList.Count;
        for (int index = 0; index < count; ++index)
          MediaDownload.SaveMediaToCameraRoll(picInfoList[index].PathForDb, FunXMPP.FMessage.Type.Image, saveAlbum: "Camera Roll");
      }
      CameraPage.TakePictureOnly = false;
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.IsEmojiKeyboardOpen)
      {
        this.CloseEmojiKeyboard();
      }
      else
      {
        if (this.isCropping_)
        {
          e.Cancel = true;
          this.ExitCrop(false);
          return;
        }
        if (!this.CaptionBox.IsEmojiKeyboardOpen && this.sharingState_ != null)
        {
          List<PicturePreviewPage.PreviewItem> previewItems = this.previewItems_;
          PicturePreviewPage.PreviewItem previewItem = previewItems != null ? previewItems.LastOrDefault<PicturePreviewPage.PreviewItem>() : (PicturePreviewPage.PreviewItem) null;
          if (previewItem != null)
          {
            this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.DeletedPreviewItem, previewItem);
            this.previewItems_.Remove(previewItem);
          }
          e.Cancel = true;
          this.onback = true;
          this.Dispatcher.BeginInvoke((Action) (() => this.NotifyObserver(MediaSharingArgs.SharingStatus.Canceled)));
        }
      }
      base.OnBackKeyPress(e);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (this.initialLoaded_ || this.previewItems_ == null)
        return;
      this.initialLoaded_ = true;
      this.ReloadPageUI(true);
    }

    private static void OnRootFrameTranslateYChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      PicturePreviewPage picturePreviewPage = d as PicturePreviewPage;
      if (picturePreviewPage.InputMode != ChatPage.InputMode.Emoji)
        return;
      picturePreviewPage.RenderTransform = (Transform) new TranslateTransform()
      {
        Y = -UIUtils.SIPHeightPortrait
      };
    }

    private void ProcessSharingStateSelectedItemsChanged(
      MediaSharingState.SelectedItemsChangeCause cause)
    {
      if (this.ignoreSelectedItemsChangeOnce_)
      {
        this.ignoreSelectedItemsChangeOnce_ = false;
      }
      else
      {
        this.ReloadData();
        if (this.sharingState_.SelectedCount == 0)
          return;
        this.ReloadPageUI(cause != MediaSharingState.SelectedItemsChangeCause.Delete);
      }
    }

    private void VideoPlayer_MediaOpened(object sender, EventArgs e)
    {
      if (this.CurrentItem == null || this.CurrentItem.BindedMediaItem.VideoInfo == null)
        return;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      int orientationAngle = bindedMediaItem.VideoInfo.OrientationAngle;
      CompositeTransform compositeTransform1 = new CompositeTransform();
      CompositeTransform compositeTransform2 = compositeTransform1;
      int num1;
      switch (orientationAngle)
      {
        case 90:
          num1 = 270;
          break;
        case 270:
          num1 = 90;
          break;
        default:
          num1 = orientationAngle;
          break;
      }
      double num2 = (double) num1;
      compositeTransform2.Rotation = num2;
      if (bindedMediaItem.VideoInfo.CropRectangle.HasValue && bindedMediaItem.VideoInfo.AbsoluteCropData != null)
      {
        Pair<System.Windows.Point, Size> pair = this.TranslateCropping(bindedMediaItem.VideoInfo.AbsoluteCropData.First, bindedMediaItem.VideoInfo.AbsoluteCropData.Second, bindedMediaItem.VideoInfo.AbsoluteCropData.Third);
        System.Windows.Point first = pair.First;
        Size second = pair.Second;
        using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(bindedMediaItem.VideoInfo.PreviewPlayPath))
        {
          double num3 = this.LayoutRoot.ActualWidth * ((double) videoFrameGrabber.FrameInfo.Height / (double) videoFrameGrabber.FrameInfo.Width);
          double num4 = this.LayoutRoot.ActualWidth;
          if ((double) videoFrameGrabber.FrameInfo.Height > (double) videoFrameGrabber.FrameInfo.Width)
          {
            num4 = this.LayoutRoot.ActualWidth * ((double) videoFrameGrabber.FrameInfo.Width / (double) videoFrameGrabber.FrameInfo.Height);
            num3 = this.LayoutRoot.ActualWidth;
          }
          double width = second.Width * num4;
          double height = second.Height * num3;
          double x = first.X * num4;
          double y = first.Y * num3;
          this.VideoCropRectangle.Rect = new Rect(new System.Windows.Point(x, y), new Size(width, height));
          double num5 = width > height ? this.LayoutRoot.ActualWidth / width : this.LayoutRoot.ActualWidth / height;
          this.VideoPlayer.RenderTransformOrigin = new System.Windows.Point((x + width / 2.0) / num4, (y + height / 2.0) / num3);
          compositeTransform1.Rotation += 0.01;
          compositeTransform1.TranslateX = (num4 - width) / 2.0 - x;
          compositeTransform1.TranslateY = (num3 - height) / 2.0 - y;
          compositeTransform1.ScaleX = num5;
          compositeTransform1.ScaleY = num5;
          this.VideoPlayer.RenderTransform = (Transform) compositeTransform1;
        }
      }
      else
      {
        compositeTransform1.ScaleX = 1.0;
        compositeTransform1.ScaleY = 1.0;
        this.VideoPlayer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        this.VideoCropRectangle.Rect = new Rect(new System.Windows.Point(0.0, 0.0), new Size(480.0, 800.0));
      }
      TimeCrop? timeCrop1 = bindedMediaItem.VideoInfo.TimeCrop;
      if (timeCrop1.HasValue)
      {
        int num6 = (int) (long) ((double) this.CurrentItem.BindedMediaItem.VideoInfo.Duration / 1E-07) / (int) this.LayoutRoot.ActualWidth;
        timeCrop1 = bindedMediaItem.VideoInfo.TimeCrop;
        TimeCrop timeCrop2 = timeCrop1.Value;
        this.videoPlayerWrapper_.Play();
        this.videoPlayerWrapper_.Pause();
        MediaElement videoPlayer = this.VideoPlayer;
        timeCrop1 = this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop;
        TimeSpan startTime = timeCrop1.Value.StartTime;
        videoPlayer.Position = startTime;
        this.LeftHandleTransform.TranslateX = (double) (timeCrop2.StartTime.Ticks / (long) num6 - 450L);
        this.RightHandleTransform.TranslateX = this.LeftHandleTransform.TranslateX + (double) (timeCrop2.DesiredDuration.Ticks / (long) num6) + 450.0 - 30.0;
      }
      else
      {
        this.LeftHandleTransform.TranslateX = -450.0;
        this.RightHandleTransform.TranslateX = 450.0;
      }
      this.generateMediaThumnails();
      this.VideoPlayer.RenderTransform = (Transform) compositeTransform1;
    }

    private void generateMediaThumnails()
    {
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      if (this.CurrentItem.BindedMediaItem.MediaTimelineThumbnails == null)
      {
        ObservableCollection<PicturePreviewPage.VideoFrameItem> list = new ObservableCollection<PicturePreviewPage.VideoFrameItem>();
        this.VideoTimelineList.ItemsSource = (IEnumerable) list;
        int orientation = bindedMediaItem.VideoInfo != null ? bindedMediaItem.VideoInfo.OrientationAngle : 0;
        list.Add(new PicturePreviewPage.VideoFrameItem((BitmapSource) null, orientation));
        if (this.videoFrameSubs == null)
          this.videoFrameSubs = new CompositeDisposable(new IDisposable[0]);
        IDisposable sub = (IDisposable) null;
        sub = bindedMediaItem.VideoTimelineThumbsAsync().Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (bitmap =>
        {
          if (list.Count<PicturePreviewPage.VideoFrameItem>() - 1 >= 0)
            list.ElementAt<PicturePreviewPage.VideoFrameItem>(list.Count<PicturePreviewPage.VideoFrameItem>() - 1).FrameThumbnail = (BitmapSource) bitmap;
          list.Add(new PicturePreviewPage.VideoFrameItem((BitmapSource) null, orientation));
        }), (Action) (() =>
        {
          if (this.videoFrameSubs == null)
            return;
          this.videoFrameSubs.Remove(sub);
        }));
        this.videoFrameSubs.Add(sub);
      }
      else
      {
        ObservableCollection<PicturePreviewPage.VideoFrameItem> observableCollection = new ObservableCollection<PicturePreviewPage.VideoFrameItem>();
        foreach (WriteableBitmap timelineThumbnail in bindedMediaItem.MediaTimelineThumbnails)
        {
          int orientationAngle = bindedMediaItem.VideoInfo != null ? bindedMediaItem.VideoInfo.OrientationAngle : 0;
          int num;
          switch (orientationAngle)
          {
            case 90:
              num = 270;
              break;
            case 270:
              num = 90;
              break;
            default:
              num = orientationAngle;
              break;
          }
          int rotation = num;
          PicturePreviewPage.VideoFrameItem videoFrameItem = new PicturePreviewPage.VideoFrameItem((BitmapSource) timelineThumbnail, rotation);
          observableCollection.Add(videoFrameItem);
        }
        this.VideoTimelineList.ItemsSource = (IEnumerable) observableCollection;
      }
      this.VideoTimelinePanel.Visibility = Visibility.Visible;
    }

    private void VideoPlayer_MediaEnded(object sender, EventArgs e) => this.StopPlayback();

    private void PlayButton_Click(object sender, EventArgs e)
    {
      if (this.CurrentItem?.BindedMediaItem?.VideoInfo == null)
        return;
      if (this.PlayingVideo)
        this.PausePlayback();
      else
        this.StartPlayback();
    }

    private void Submit_Click(object sender, EventArgs e)
    {
      this.StopPlayback();
      this.stopGifPlayback = true;
      if (this.isCropping_)
        this.ExitCrop(true);
      else if (!this.submitted_)
      {
        Log.d("preview", "Submit button clicked");
        Action onSubmit = (Action) (() =>
        {
          this.videoPlayerWrapper_.Detach();
          this.submitted_ = true;
          this.EditControl.ImageSource = (BitmapSource) (this.croppingSource_ = (WriteableBitmap) null);
          foreach (PicturePreviewPage.PreviewItem previewItem in this.previewItems_)
            previewItem.BindedMediaItem.IsSent = true;
          if (this.CurrentItem != null && this.CurrentItem.BindedMediaItem != null)
            this.CurrentItem.BindedMediaItem.WriteToDrawingBitmapCache(this.highResMaxSize_);
          this.DrawingMode.Children.Clear();
          this.NotifyObserver(MediaSharingArgs.SharingStatus.Submitted);
        });
        if (this.sharingState_.RecipientJids != null && this.sharingState_.RecipientJids.Length == 1 && JidHelper.IsStatusJid(this.sharingState_.RecipientJids[0]) && Settings.ShowStatusPresendTooltip)
        {
          Settings.ShowStatusPresendTooltip = false;
          UIUtils.Decision(AppResources.StatusFirstTimeNotice, AppResources.Send, AppResources.Cancel, AppResources.StatusOnboardTitleYourStatus).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
          {
            if (!confirmed)
              return;
            onSubmit();
          }));
        }
        else
          onSubmit();
      }
      else
        Log.d("preview", "Submit button ignored - already clicked");
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      if (this.ShouldBlockButtonClick())
        return;
      this.CloseEmojiKeyboard();
      this.EditControl.ImageSource = (BitmapSource) (this.croppingSource_ = (WriteableBitmap) null);
      this.DeleteCurrentItem();
    }

    private void Crop_Click(object sender, EventArgs e)
    {
      if (this.ShouldBlockButtonClick())
        return;
      if (this.isCropping_)
        this.ExitCrop(true);
      else
        this.EnterCrop();
    }

    private void Rotate_Click(object sender, EventArgs e)
    {
      if (this.ShouldBlockButtonClick())
        return;
      this.CloseEmojiKeyboard();
      this.EditControl.ImageSource = (BitmapSource) (this.croppingSource_ = (WriteableBitmap) null);
      this.RotateCurrentItem();
    }

    private void ChangeCropRatio_Click(object sender, EventArgs e)
    {
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem?.BindedMediaItem;
      if (this.ShouldBlockButtonClick(250) || !this.isCropping_ || bindedMediaItem == null || this.croppingSource_ == null)
        return;
      if (this.cropRatioItems_ == null)
      {
        this.cropRatioItems_ = new List<PicturePreviewPage.CropRatioItem>()
        {
          new PicturePreviewPage.CropRatioItem(1.0, 1.0)
          {
            IsDummy = true
          },
          new PicturePreviewPage.CropRatioItem(1.0, 1.0)
          {
            TitleStr = AppResources.OriginalCropRatio,
            UseDashRectStrokeStyle = true,
            RectStrokeThickness = 1.0,
            AspectRatio = (double) this.croppingSource_.PixelWidth / (double) this.croppingSource_.PixelHeight
          },
          new PicturePreviewPage.CropRatioItem(1.0, 1.0)
          {
            TitleStr = AppResources.SquareCropRatio
          },
          new PicturePreviewPage.CropRatioItem(3.0, 4.0),
          new PicturePreviewPage.CropRatioItem(4.0, 3.0),
          new PicturePreviewPage.CropRatioItem(4.0, 6.0),
          new PicturePreviewPage.CropRatioItem(6.0, 4.0),
          new PicturePreviewPage.CropRatioItem(5.0, 7.0),
          new PicturePreviewPage.CropRatioItem(7.0, 5.0),
          new PicturePreviewPage.CropRatioItem(8.0, 10.0),
          new PicturePreviewPage.CropRatioItem(10.0, 8.0),
          new PicturePreviewPage.CropRatioItem(16.0, 9.0)
          {
            ItemMargin = new Thickness(0.0, 0.0, 0.0, 144.0)
          }
        };
        this.CropAspectRatioPicker.ItemsSource = (IEnumerable) this.cropRatioItems_;
      }
      Pair<System.Windows.Point, Size> croppingState = this.EditControl.GetCroppingState(false);
      if (croppingState == null)
      {
        this.cropRatioItems_.ForEach((Action<PicturePreviewPage.CropRatioItem>) (item => item.IsSelected = false));
        this.cropRatioItems_[0].IsSelected = true;
      }
      else
      {
        double currRatio = croppingState.Second.Width / croppingState.Second.Height;
        bool hit = false;
        this.cropRatioItems_.ForEach((Action<PicturePreviewPage.CropRatioItem>) (item =>
        {
          if (!hit && Math.Abs(currRatio - item.AspectRatio) < 0.001)
          {
            item.IsSelected = true;
            hit = true;
          }
          else
            item.IsSelected = false;
        }));
      }
      this.CropAspectRatioPicker.Open();
    }

    private void PreviewItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      PicturePreviewPage.PreviewItem selectedItem = this.PreviewItemsList.SelectedItem as PicturePreviewPage.PreviewItem;
      this.PreviewItemsList.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      int i = 0;
      foreach (PicturePreviewPage.PreviewItem previewItem in this.previewItems_)
      {
        if (selectedItem != previewItem)
          ++i;
        else
          break;
      }
      AppState.Worker.Enqueue((Action) (() => GC.Collect()));
      this.SetCurrentIndex(i, false);
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
      this.NotifyObserver();
      this.DrawingMode.Children.Clear();
    }

    private void CaptionPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.CaptionBox.Opacity = 0.0;
      this.CaptionBox.Visibility = Visibility.Visible;
      this.CaptionBox.MaxLength = 1024;
      this.fadeInSbSub_.SafeDispose();
      this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(350.0), (DependencyObject) this.CaptionBox, true, (Action) (() =>
      {
        if (this.InputMode == ChatPage.InputMode.None)
          return;
        this.CaptionBox.Opacity = 1.0;
        int length = this.CaptionBox.Text == null ? 0 : this.CaptionBox.Text.Length;
        this.CaptionBox.TextBox.SelectionLength = 0;
        this.CaptionBox.TextBox.SelectionStart = length;
      }), false, "pic preview: fade in caption box");
      this.CaptionBox.OpenTextKeyboard();
    }

    private void SliderView_ItemSwitching(int direction, double percentage)
    {
      this.SlideView.CenterImage.EnableScaling = false;
      PicturePreviewPage.PreviewItem currentItem = this.CurrentItem;
      PicturePreviewPage.PreviewItem nextItem = this.NextItem;
      PicturePreviewPage.PreviewItem prevItem = this.PrevItem;
      int num1 = this.isSwitchingEnough_ ? 1 : 0;
      bool flag1;
      bool flag2;
      bool flag3;
      PicturePreviewPage.PreviewItem previewItem;
      if (percentage > 0.5)
      {
        flag1 = false;
        flag2 = direction > 0;
        flag3 = direction < 0;
        previewItem = flag2 ? nextItem : (flag3 ? prevItem : (PicturePreviewPage.PreviewItem) null);
        this.isSwitchingEnough_ = true;
      }
      else
      {
        flag1 = true;
        flag2 = flag3 = false;
        previewItem = currentItem;
        this.isSwitchingEnough_ = false;
      }
      if (currentItem != null)
        currentItem.IsBeingViewed = flag1;
      if (nextItem != null)
        nextItem.IsBeingViewed = flag2;
      if (prevItem != null)
        prevItem.IsBeingViewed = flag3;
      if (!this.isSwitching_)
      {
        if (this.InputMode != ChatPage.InputMode.None && currentItem != null)
          this.SaveCaptionForCurrentItem();
        this.isSwitching_ = true;
      }
      if (this.InputMode != ChatPage.InputMode.None)
        this.CaptionBox.TextForeground.Opacity = Math.Abs(percentage - 0.5) * 2.0;
      int num2 = this.isSwitchingEnough_ ? 1 : 0;
      if (num1 == num2)
        return;
      this.UpdateCaptionPanel(previewItem, true);
    }

    private void SliderView_ItemSwitched(int direction)
    {
      this.isSwitching_ = this.isSwitchingEnough_ = false;
      this.CaptionBox.TextForeground.Opacity = 1.0;
      if (this.previewItems_ == null)
        return;
      if (direction == 0)
      {
        PicturePreviewPage.PreviewItem currentItem = this.CurrentItem;
        if (currentItem == null)
          return;
        currentItem.IsBeingViewed = true;
      }
      else
        this.SetCurrentIndex(this.currentIndex_ + (direction > 0 ? 1 : -1), false, true, direction < 0, direction > 0);
    }

    private void OnEnterKeyPressed()
    {
      Log.p("pic preview", "enter key pressed");
      this.Focus();
    }

    private void CaptionBox_EmojiKeyboardOpening(object sender, EventArgs e)
    {
      Log.p("pic preview", "emoji keyboard opening");
      this.InputMode = ChatPage.InputMode.Emoji;
    }

    private void CaptionBox_EmojiKeyboardClosed(object sender, EventArgs e)
    {
      if (this.ignoreEmojiKeyboardClosedOnce_)
      {
        this.ignoreEmojiKeyboardClosedOnce_ = false;
      }
      else
      {
        Log.p("pic preview", "emoji keyboard closed");
        if (this.InputMode != ChatPage.InputMode.Emoji)
          return;
        this.InputMode = ChatPage.InputMode.None;
      }
    }

    private void CaptionBox_FocusChanged(bool focused)
    {
      Log.p("pic preview", "caption box focus changed: {0}", (object) focused);
      if (focused)
      {
        if (this.InputMode == ChatPage.InputMode.Emoji)
          this.ignoreEmojiKeyboardClosedOnce_ = true;
        this.InputMode = ChatPage.InputMode.Keyboard;
        if (this.CurrentItem?.BindedMediaItem?.GifInfo != null || this.CurrentItem?.BindedMediaItem?.VideoInfo == null)
          return;
        this.DurationBox.Visibility = Visibility.Collapsed;
        this.PlayButton.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.InputMode = ChatPage.InputMode.None;
        if (this.CurrentItem?.BindedMediaItem?.GifInfo != null || this.CurrentItem?.BindedMediaItem?.VideoInfo == null)
          return;
        this.DurationBox.Visibility = Visibility.Visible;
        this.PlayButton.Visibility = Visibility.Visible;
      }
    }

    private void OnInputModeChanged(ChatPage.InputMode oldMode, ChatPage.InputMode newMode)
    {
      Log.p("pic preview", "input mode {0} -> {1}", (object) oldMode, (object) newMode);
      if (newMode != ChatPage.InputMode.Keyboard)
        this.Focus();
      else if (newMode != ChatPage.InputMode.Emoji)
        this.CaptionBox.CloseEmojiKeyboard();
      this.UpdateRootFrameShifting(newMode);
      if (newMode == ChatPage.InputMode.None)
      {
        this.SaveCaptionForCurrentItem();
        this.CaptionBox.Visibility = Visibility.Collapsed;
        this.BottomPanel.Opacity = 0.0;
        this.BottomPanel.Visibility = Visibility.Visible;
        this.fadeInSbSub_.SafeDispose();
        this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(), (DependencyObject) this.BottomPanel, true, (Action) (() =>
        {
          if (this.InputMode != ChatPage.InputMode.None)
            return;
          this.BottomPanel.Opacity = 1.0;
        }), false, "pic preview: fade in bottom panel");
      }
      else
      {
        this.BottomPanel.Visibility = Visibility.Collapsed;
        this.BottomPanel.Opacity = 0.0;
        this.CaptionBox.Visibility = Visibility.Visible;
      }
    }

    private void CropAspectRatioPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      PicturePreviewPage.CropRatioItem selItem = this.CropAspectRatioPicker.SelectedItem as PicturePreviewPage.CropRatioItem;
      this.CropAspectRatioPicker.SelectedItem = (object) this.cropRatioItems_.First<PicturePreviewPage.CropRatioItem>();
      if (selItem == null || selItem.IsDummy)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => this.EditControl.SetCropping(selItem.AspectRatio, transit: true)));
    }

    private void LeftHandle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      e.Handled = true;
      if (this.PlayingVideo)
        this.PausePlayback();
      this.stopGifPlayback = true;
      this.LeftHandle.Fill = (Brush) UIUtils.AccentBrush;
      this.VideoTimelinePanel.Background = (Brush) UIUtils.AccentBrush;
      if (this.CurrentItem.BindedMediaItem.GifInfo != null)
      {
        this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.SlideViewPanel.Visibility = Visibility.Visible;
      }
      else if (this.CurrentItem.BindedMediaItem.VideoInfo != null)
      {
        this.VideoPlayer.Visibility = Visibility.Visible;
        this.SlideViewPanel.Visibility = Visibility.Collapsed;
      }
      this.resetTimeStripImage();
      this.StartTimeCropTimer();
    }

    private void LeftHandle_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      e.Handled = true;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      double num1 = this.LeftHandleTransform.TranslateX + e.DeltaManipulation.Translation.X;
      double num2 = num1 > -450.0 ? num1 : -450.0;
      double num3 = num2 > 450.0 ? 450.0 : num2;
      if (num3 + 450.0 + 50.0 > this.RightHandleTransform.TranslateX)
        return;
      this.LeftHandleTransform.TranslateX = num3;
      if (!this.timeCropManipulation)
        return;
      long pixel = (long) num3 + 450L;
      int interval = (int) (bindedMediaItem.VideoInfo != null ? (long) ((double) bindedMediaItem.VideoInfo.Duration / 1E-07) : (long) (double) bindedMediaItem.GifInfo.Duration) / (int) this.LayoutRoot.ActualWidth;
      this.currentPosition = pixel * (long) interval;
      if (bindedMediaItem.GifInfo != null)
      {
        VideoFrameGrabber gifFrameGrabber = bindedMediaItem.GifInfo.GifFrameGrabber;
        gifFrameGrabber.Seek(this.currentPosition);
        VideoFrame videoFrame = gifFrameGrabber.ReadFrame();
        if (videoFrame != null)
        {
          WriteableBitmap writeableBitmap = videoFrame.Bitmap;
          System.Windows.Point? relativeCropPos1 = bindedMediaItem.RelativeCropPos;
          if (relativeCropPos1.HasValue)
          {
            WriteableBitmap bitmap = writeableBitmap;
            relativeCropPos1 = bindedMediaItem.RelativeCropPos;
            System.Windows.Point relativeCropPos2 = relativeCropPos1.Value;
            Size relativeCropSize = bindedMediaItem.RelativeCropSize.Value;
            writeableBitmap = bitmap.CropRelatively(relativeCropPos2, relativeCropSize);
          }
          if (this.CurrentItem.RotatedTimes > 0)
            writeableBitmap = writeableBitmap.Rotate(this.CurrentItem.RotatedTimes * 90);
          this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap);
        }
      }
      else if (bindedMediaItem.VideoInfo != null)
      {
        TimeSpan t = new TimeSpan(pixel * (long) interval);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if ((double) Math.Abs(this.currentPosition - pixel * (long) interval) >= 0.001)
            return;
          this.VideoPlayer.Position = t;
          this.videoPlayerWrapper_.Pause();
        }));
      }
      this.timeCropManipulation = false;
    }

    private void LeftHandle_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.StopTimeCropTimer();
      this.LeftHandle.Fill = (Brush) UIUtils.BlackBrush;
      this.VideoTimelinePanel.Background = (Brush) UIUtils.BlackBrush;
      if ((long) this.LeftHandleTransform.TranslateX + 450L < 20L)
        this.LeftHandleTransform.TranslateX = -450.0;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      long num1 = (long) this.LeftHandleTransform.TranslateX + 450L;
      int num2 = (bindedMediaItem.VideoInfo != null ? (int) (long) ((double) bindedMediaItem.VideoInfo.Duration / 1E-07) : (int) (long) (double) bindedMediaItem.GifInfo.Duration) / (int) this.LayoutRoot.ActualWidth;
      this.updateVideoCrop();
      if (this.CurrentItem.BindedMediaItem.GifInfo != null)
      {
        this.stopGifPlayback = false;
        VideoFrameGrabber gifFrameGrabber = bindedMediaItem.GifInfo.GifFrameGrabber;
        gifFrameGrabber.Seek(this.currentPosition);
        VideoFrame videoFrame = gifFrameGrabber.ReadFrame();
        if (videoFrame == null)
          return;
        WriteableBitmap writeableBitmap = videoFrame.Bitmap;
        System.Windows.Point? relativeCropPos1 = bindedMediaItem.RelativeCropPos;
        if (relativeCropPos1.HasValue)
        {
          WriteableBitmap bitmap = writeableBitmap;
          relativeCropPos1 = bindedMediaItem.RelativeCropPos;
          System.Windows.Point relativeCropPos2 = relativeCropPos1.Value;
          Size relativeCropSize = bindedMediaItem.RelativeCropSize.Value;
          writeableBitmap = bitmap.CropRelatively(relativeCropPos2, relativeCropSize);
        }
        if (this.CurrentItem.RotatedTimes > 0)
          writeableBitmap = writeableBitmap.Rotate(this.CurrentItem.RotatedTimes * 90);
        this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap);
        this.PlayGif(0L, videoFrame.Timestamp);
      }
      else
      {
        if (this.CurrentItem.BindedMediaItem.VideoInfo == null)
          return;
        this.VideoPlayer.Position = new TimeSpan(num1 * (long) num2);
        this.videoPlayerWrapper_.Pause();
        this.SlideView.SetCenterImageSource(this.CurrentItem.GetBitmapObservable(false, this.highResMaxSize_, true), false);
      }
    }

    private void RightHandle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      e.Handled = true;
      if (this.PlayingVideo)
        this.PausePlayback();
      this.stopGifPlayback = true;
      this.RightHandle.Fill = (Brush) UIUtils.AccentBrush;
      this.VideoTimelinePanel.Background = (Brush) UIUtils.AccentBrush;
      if (this.CurrentItem.BindedMediaItem.GifInfo != null)
      {
        this.VideoPlayer.Visibility = Visibility.Collapsed;
        this.SlideViewPanel.Visibility = Visibility.Visible;
      }
      else if (this.CurrentItem.BindedMediaItem.VideoInfo != null)
      {
        this.VideoPlayer.Visibility = Visibility.Visible;
        this.SlideViewPanel.Visibility = Visibility.Collapsed;
      }
      this.resetTimeStripImage();
      this.StartTimeCropTimer();
    }

    private void RightHandle_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      e.Handled = true;
      double num1 = this.RightHandleTransform.TranslateX + e.DeltaManipulation.Translation.X;
      double num2 = num1 > -450.0 ? num1 : -450.0;
      double num3 = num2 > 450.0 ? 450.0 : num2;
      if (num3 - 450.0 - 50.0 < this.LeftHandleTransform.TranslateX)
        return;
      this.RightHandleTransform.TranslateX = num3;
      if (!this.timeCropManipulation)
        return;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      long pixel = (long) num3 - 450L;
      long ticks = bindedMediaItem.VideoInfo != null ? (long) ((double) bindedMediaItem.VideoInfo.Duration / 1E-07) : (long) (double) bindedMediaItem.GifInfo.Duration;
      int interval = (int) ticks / (int) this.LayoutRoot.ActualWidth;
      this.currentPosition = pixel * (long) interval;
      if (this.CurrentItem.BindedMediaItem.GifInfo != null)
      {
        VideoFrameGrabber gifFrameGrabber = bindedMediaItem.GifInfo.GifFrameGrabber;
        gifFrameGrabber.Seek(ticks + this.currentPosition);
        VideoFrame videoFrame = gifFrameGrabber.ReadFrame();
        if (videoFrame != null)
        {
          WriteableBitmap writeableBitmap = videoFrame.Bitmap;
          System.Windows.Point? relativeCropPos1 = bindedMediaItem.RelativeCropPos;
          if (relativeCropPos1.HasValue)
          {
            WriteableBitmap bitmap = writeableBitmap;
            relativeCropPos1 = bindedMediaItem.RelativeCropPos;
            System.Windows.Point relativeCropPos2 = relativeCropPos1.Value;
            Size relativeCropSize = bindedMediaItem.RelativeCropSize.Value;
            writeableBitmap = bitmap.CropRelatively(relativeCropPos2, relativeCropSize);
          }
          if (this.CurrentItem.RotatedTimes > 0)
            writeableBitmap = writeableBitmap.Rotate(this.CurrentItem.RotatedTimes * 90);
          this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap);
        }
      }
      else if (this.CurrentItem.BindedMediaItem.VideoInfo != null)
      {
        TimeSpan t = new TimeSpan(ticks) + new TimeSpan(pixel * (long) interval);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if ((double) Math.Abs(this.currentPosition - pixel * (long) interval) >= 0.001)
            return;
          this.VideoPlayer.Position = t;
          this.videoPlayerWrapper_.Pause();
        }));
      }
      this.timeCropManipulation = false;
    }

    private void RightHandle_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.StopTimeCropTimer();
      this.RightHandle.Fill = (Brush) UIUtils.BlackBrush;
      this.VideoTimelinePanel.Background = (Brush) UIUtils.BlackBrush;
      if ((long) this.RightHandleTransform.TranslateX - 450L > -20L)
        this.RightHandleTransform.TranslateX = 450.0;
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      long num1 = (long) this.RightHandleTransform.TranslateX - 450L;
      long ticks = bindedMediaItem.VideoInfo != null ? (long) ((double) bindedMediaItem.VideoInfo.Duration / 1E-07) : (long) (double) bindedMediaItem.GifInfo.Duration;
      int num2 = (int) ticks / (int) this.LayoutRoot.ActualWidth;
      this.updateVideoCrop();
      if (this.CurrentItem.BindedMediaItem.GifInfo != null)
      {
        this.stopGifPlayback = false;
        VideoFrameGrabber gifFrameGrabber = bindedMediaItem.GifInfo.GifFrameGrabber;
        gifFrameGrabber.Seek(this.currentPosition);
        VideoFrame videoFrame = gifFrameGrabber.ReadFrame();
        if (videoFrame == null)
          return;
        WriteableBitmap writeableBitmap = videoFrame.Bitmap;
        System.Windows.Point? relativeCropPos1 = bindedMediaItem.RelativeCropPos;
        if (relativeCropPos1.HasValue)
        {
          WriteableBitmap bitmap = writeableBitmap;
          relativeCropPos1 = bindedMediaItem.RelativeCropPos;
          System.Windows.Point relativeCropPos2 = relativeCropPos1.Value;
          Size relativeCropSize = bindedMediaItem.RelativeCropSize.Value;
          writeableBitmap = bitmap.CropRelatively(relativeCropPos2, relativeCropSize);
        }
        if (this.CurrentItem.RotatedTimes > 0)
          writeableBitmap = writeableBitmap.Rotate(this.CurrentItem.RotatedTimes * 90);
        this.SlideView.SetCenterImageSource((BitmapSource) writeableBitmap);
        this.PlayGif(0L, 0L);
      }
      else
      {
        if (bindedMediaItem.VideoInfo == null)
          return;
        this.VideoPlayer.Position = new TimeSpan(ticks) + new TimeSpan(num1 * (long) num2);
        this.videoPlayerWrapper_.Pause();
      }
    }

    private void updateVideoCrop()
    {
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem.BindedMediaItem;
      long num1 = (long) this.LeftHandleTransform.TranslateX + 450L;
      long num2 = (long) this.RightHandleTransform.TranslateX - 450L;
      long ticks = bindedMediaItem.GifInfo != null ? (long) (double) bindedMediaItem.GifInfo.Duration : (long) ((double) bindedMediaItem.VideoInfo.Duration / 1E-07);
      int num3 = (int) ticks / (int) this.LayoutRoot.ActualWidth;
      if (num1 == 0L && num2 == 0L)
      {
        if (bindedMediaItem.GifInfo != null)
          bindedMediaItem.GifInfo.TimeCrop = new TimeCrop?();
        else if (bindedMediaItem.VideoInfo != null)
          this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop = new TimeCrop?();
      }
      else
      {
        TimeCrop timeCrop = new TimeCrop()
        {
          StartTime = new TimeSpan(num1 * (long) num3)
        };
        timeCrop.DesiredDuration = new TimeSpan(ticks) - timeCrop.StartTime + new TimeSpan(num2 * (long) num3);
        if (bindedMediaItem.GifInfo != null)
          bindedMediaItem.GifInfo.TimeCrop = new TimeCrop?(timeCrop);
        else if (bindedMediaItem.VideoInfo != null)
          this.CurrentItem.BindedMediaItem.VideoInfo.TimeCrop = new TimeCrop?(timeCrop);
      }
      this.updateDurationText();
    }

    private void VideoPlayer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.PlayingVideo)
        this.PausePlayback();
      else
        this.StartPlayback();
    }

    private void GifButton_Click(object sender, EventArgs e)
    {
      WaVideoArgs videoInfo = this.CurrentItem?.BindedMediaItem?.VideoInfo;
      if (videoInfo == null)
        return;
      if (this.VideoToggleForeground.Fill == UIUtils.AccentBrush)
      {
        this.VideoToggleForeground.Fill = (Brush) UIUtils.WhiteBrush;
        this.GifToggleForeground.Fill = (Brush) UIUtils.AccentBrush;
        videoInfo.LoopingPlayback = true;
      }
      else
      {
        this.VideoToggleForeground.Fill = (Brush) UIUtils.AccentBrush;
        this.GifToggleForeground.Fill = (Brush) UIUtils.WhiteBrush;
        videoInfo.LoopingPlayback = false;
      }
    }

    private void updatePickerFieldStatsAdd(
      List<PicturePreviewPage.PreviewItem> newPreviewItems)
    {
      if (PicturePreviewPage.previewStartTimeTicks == 0L)
        PicturePreviewPage.previewStartTimeTicks = DateTime.Now.Ticks;
      Dictionary<string, PicturePreviewPage.PIDetails> dictionary1 = new Dictionary<string, PicturePreviewPage.PIDetails>();
      Dictionary<string, PicturePreviewPage.PIDetails> dictionary2 = new Dictionary<string, PicturePreviewPage.PIDetails>();
      foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.imagePreviewItemKeyCheck)
      {
        if (keyValuePair.Value.StateIndicator >= 1)
          dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
      }
      foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.videoPreviewItemKeyCheck)
      {
        if (keyValuePair.Value.StateIndicator >= 1)
          dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
      }
      foreach (PicturePreviewPage.PreviewItem newPreviewItem in newPreviewItems)
      {
        string previewItemKey = this.createPreviewItemKey(newPreviewItem);
        if (previewItemKey != null)
        {
          bool isImage = PicturePreviewPage.isImagePreviewItem(newPreviewItem);
          wam_enum_media_picker_origin_type itemOrigin = PicturePreviewPage.getItemOrigin(newPreviewItem);
          if (!string.IsNullOrEmpty(previewItemKey))
          {
            if ((isImage ? (dictionary1.ContainsKey(previewItemKey) ? 1 : 0) : (dictionary2.ContainsKey(previewItemKey) ? 1 : 0)) == 0)
              this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.AddedPreviewItem, previewItemKey, isImage, itemOrigin);
            else if (isImage)
              dictionary1.Remove(previewItemKey);
            else
              dictionary2.Remove(previewItemKey);
          }
        }
      }
      foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in dictionary1)
        this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.DeletedPreviewItem, keyValuePair.Key, true, keyValuePair.Value.Origin);
      foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in dictionary2)
        this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.DeletedPreviewItem, keyValuePair.Key, false, keyValuePair.Value.Origin);
    }

    private void updatePickerFieldStats(
      PicturePreviewPage.StatToUpdate updateType,
      PicturePreviewPage.PreviewItem previewItem)
    {
      string previewItemKey = this.createPreviewItemKey(previewItem);
      if (previewItemKey == null)
        return;
      this.updatePickerFieldStats(updateType, previewItemKey, PicturePreviewPage.isImagePreviewItem(previewItem), PicturePreviewPage.getItemOrigin(previewItem));
    }

    private void updatePickerFieldStats(
      PicturePreviewPage.StatToUpdate updateType,
      string pKey,
      bool isImage,
      wam_enum_media_picker_origin_type itemOrigin)
    {
      int num = -1;
      try
      {
        MediaPicker fieldStatsToUpdate = this.getFieldStatsToUpdate(isImage, itemOrigin);
        if (pKey == null || fieldStatsToUpdate == null)
          return;
        Dictionary<string, PicturePreviewPage.PIDetails> dictionary = isImage ? PicturePreviewPage.imagePreviewItemKeyCheck : PicturePreviewPage.videoPreviewItemKeyCheck;
        PicturePreviewPage.PIDetails piDetails = dictionary.ContainsKey(pKey) ? dictionary[pKey] : (PicturePreviewPage.PIDetails) null;
        num = piDetails != null ? piDetails.StateIndicator : 0;
        switch (updateType)
        {
          case PicturePreviewPage.StatToUpdate.AddedPreviewItem:
            if (num >= 1)
              break;
            dictionary[pKey] = new PicturePreviewPage.PIDetails(1, itemOrigin);
            break;
          case PicturePreviewPage.StatToUpdate.ChangedPreviewItem:
            if (num >= 2)
              break;
            dictionary[pKey] = new PicturePreviewPage.PIDetails(2, itemOrigin);
            fieldStatsToUpdate.mediaPickerChanged = new long?(fieldStatsToUpdate.mediaPickerChanged.Value + 1L);
            break;
          case PicturePreviewPage.StatToUpdate.DeletedPreviewItem:
            if (num < 1)
              break;
            dictionary.Remove(pKey);
            fieldStatsToUpdate.mediaPickerDeleted = new long?(fieldStatsToUpdate.mediaPickerDeleted.Value + 1L);
            break;
        }
      }
      catch (Exception ex)
      {
        string context = string.Format("Exception updating field stats {0} {1} {2} {3} {4}", (object) updateType, (object) pKey, (object) isImage, (object) itemOrigin, (object) num);
        Log.LogException(ex, context);
      }
    }

    private void sendPickerFieldStats(MediaSharingArgs.SharingStatus status)
    {
      if (status != MediaSharingArgs.SharingStatus.Submitted)
      {
        if (PicturePreviewPage.imagePreviewItemKeyCheck.Count > 0)
        {
          foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.imagePreviewItemKeyCheck)
          {
            MediaPicker fieldStatsToUpdate = this.getFieldStatsToUpdate(true, keyValuePair.Value.Origin);
            fieldStatsToUpdate.mediaPickerSent = new long?(0L);
            if (keyValuePair.Value.StateIndicator >= 1)
              fieldStatsToUpdate.mediaPickerDeleted = new long?(fieldStatsToUpdate.mediaPickerDeleted.Value + 1L);
          }
        }
        if (PicturePreviewPage.videoPreviewItemKeyCheck.Count > 0)
        {
          foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.videoPreviewItemKeyCheck)
          {
            MediaPicker fieldStatsToUpdate = this.getFieldStatsToUpdate(false, keyValuePair.Value.Origin);
            fieldStatsToUpdate.mediaPickerSent = new long?(0L);
            if (keyValuePair.Value.StateIndicator >= 1)
              fieldStatsToUpdate.mediaPickerDeleted = new long?(fieldStatsToUpdate.mediaPickerDeleted.Value + 1L);
          }
        }
      }
      else
      {
        if (PicturePreviewPage.imagePreviewItemKeyCheck.Count > 0)
        {
          foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.imagePreviewItemKeyCheck)
          {
            MediaPicker fieldStatsToUpdate = this.getFieldStatsToUpdate(true, keyValuePair.Value.Origin);
            if (keyValuePair.Value.StateIndicator >= 1)
            {
              fieldStatsToUpdate.mediaPickerSent = new long?(fieldStatsToUpdate.mediaPickerSent.Value + 1L);
              if (keyValuePair.Value.StateIndicator == 1)
                fieldStatsToUpdate.mediaPickerSentUnchanged = new long?(fieldStatsToUpdate.mediaPickerSentUnchanged.Value + 1L);
            }
          }
        }
        if (PicturePreviewPage.videoPreviewItemKeyCheck.Count > 0)
        {
          foreach (KeyValuePair<string, PicturePreviewPage.PIDetails> keyValuePair in PicturePreviewPage.videoPreviewItemKeyCheck)
          {
            MediaPicker fieldStatsToUpdate = this.getFieldStatsToUpdate(false, keyValuePair.Value.Origin);
            if (keyValuePair.Value.StateIndicator >= 1)
            {
              fieldStatsToUpdate.mediaPickerSent = new long?(fieldStatsToUpdate.mediaPickerSent.Value + 1L);
              if (keyValuePair.Value.StateIndicator == 1)
                fieldStatsToUpdate.mediaPickerSentUnchanged = new long?(fieldStatsToUpdate.mediaPickerSentUnchanged.Value + 1L);
            }
          }
        }
      }
      PicturePreviewPage.imagePreviewItemKeyCheck.Clear();
      PicturePreviewPage.videoPreviewItemKeyCheck.Clear();
      long num = PicturePreviewPage.previewStartTimeTicks > 0L ? DateTime.Now.Ticks - PicturePreviewPage.previewStartTimeTicks : 0L;
      long? nullable = num > 0L ? new long?(num / 10000L) : new long?();
      PicturePreviewPage.previewStartTimeTicks = 0L;
      if (PicturePreviewPage.imagePickerEvents != null)
      {
        foreach (MediaPicker mediaPicker in PicturePreviewPage.imagePickerEvents.Values)
        {
          mediaPicker.mediaPickerT = nullable;
          mediaPicker.SaveEvent();
        }
        PicturePreviewPage.imagePickerEvents = (Dictionary<wam_enum_media_picker_origin_type, MediaPicker>) null;
      }
      if (PicturePreviewPage.videoPickerEvents == null)
        return;
      foreach (MediaPicker mediaPicker in PicturePreviewPage.videoPickerEvents.Values)
      {
        mediaPicker.mediaPickerT = nullable;
        mediaPicker.SaveEvent();
      }
      PicturePreviewPage.videoPickerEvents = (Dictionary<wam_enum_media_picker_origin_type, MediaPicker>) null;
    }

    private string createPreviewItemKey(PicturePreviewPage.PreviewItem previewItem)
    {
      if (previewItem == null)
      {
        Log.d("preview fieldstats", "null preview item supplied");
        return (string) null;
      }
      if (previewItem.BindedMediaItem is MediaSharingState.IItemFieldStats bindedMediaItem)
        return bindedMediaItem.FsItemId().ToString();
      Log.d("preview fieldstats", "preview item supplied did not implement fieldstats interface");
      return (string) null;
    }

    private MediaPicker getFieldStatsToUpdate(
      bool isImage,
      wam_enum_media_picker_origin_type itemOrigin)
    {
      MediaPicker fieldStatsToUpdate1 = (MediaPicker) null;
      MediaPicker fieldStatsToUpdate2;
      if (isImage)
      {
        if (PicturePreviewPage.imagePickerEvents == null)
          PicturePreviewPage.imagePickerEvents = new Dictionary<wam_enum_media_picker_origin_type, MediaPicker>();
        if (PicturePreviewPage.imagePickerEvents.TryGetValue(itemOrigin, out fieldStatsToUpdate1))
          return fieldStatsToUpdate1;
        fieldStatsToUpdate2 = new MediaPicker();
        fieldStatsToUpdate2.mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(FunXMPP.FMessage.Type.Image, false));
        fieldStatsToUpdate2.mediaPickerOrigin = new wam_enum_media_picker_origin_type?(itemOrigin);
        PicturePreviewPage.imagePickerEvents.Add(itemOrigin, fieldStatsToUpdate2);
      }
      else
      {
        if (PicturePreviewPage.videoPickerEvents == null)
          PicturePreviewPage.videoPickerEvents = new Dictionary<wam_enum_media_picker_origin_type, MediaPicker>();
        if (PicturePreviewPage.videoPickerEvents.TryGetValue(itemOrigin, out fieldStatsToUpdate1))
          return fieldStatsToUpdate1;
        fieldStatsToUpdate2 = new MediaPicker();
        fieldStatsToUpdate2.mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(FunXMPP.FMessage.Type.Video, false));
        fieldStatsToUpdate2.mediaPickerOrigin = new wam_enum_media_picker_origin_type?(itemOrigin);
        PicturePreviewPage.videoPickerEvents.Add(itemOrigin, fieldStatsToUpdate2);
      }
      fieldStatsToUpdate2.mediaPickerSent = new long?(0L);
      fieldStatsToUpdate2.mediaPickerSentUnchanged = new long?(0L);
      fieldStatsToUpdate2.mediaPickerDeleted = new long?(0L);
      fieldStatsToUpdate2.mediaPickerChanged = new long?(0L);
      return fieldStatsToUpdate2;
    }

    private static bool isImagePreviewItem(PicturePreviewPage.PreviewItem previewItem)
    {
      return previewItem.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Image;
    }

    private static wam_enum_media_picker_origin_type getItemOrigin(
      PicturePreviewPage.PreviewItem previewItem)
    {
      if (previewItem.BindedMediaItem is MediaSharingState.IItemFieldStats bindedMediaItem)
        return bindedMediaItem.FsItemOrigin;
      Log.d("preview fieldstats", "preview item supplied did not implement fieldstats interface");
      return wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_PHOTO_LIBRARY;
    }

    private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseEventArgs e)
    {
      if (this.IsEmojiKeyboardOpen)
      {
        this.CloseEmojiKeyboard();
      }
      else
      {
        try
        {
          if (this.CurrentItem == null || this.CurrentItem.BindedMediaItem == null || this.CurrentItem.BindedMediaItem.DrawArgs == null || PicturePreviewPage.ColorPicker == null || PicturePreviewPage.ColorPicker.Visibility != Visibility.Visible)
            return;
          this.CurrentItem.BindedMediaItem.DrawArgs.HasDrawing = true;
          this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.ChangedPreviewItem, this.CurrentItem);
          this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.CaptureMouse();
          this.NewStroke = new Stroke(new StylusPointCollection()
          {
            e.StylusDevice.GetStylusPoints((UIElement) this.CurrentItem.BindedMediaItem.DrawArgs.Canvas)
          });
          this.NewStroke.DrawingAttributes.Color = PicturePreviewPage.StrokeColor;
          this.NewStroke.DrawingAttributes.Width = 10.0;
          this.NewStroke.DrawingAttributes.Height = 10.0;
          this.CurrentItem.BindedMediaItem.DrawArgs.Canvas.Strokes.Add(this.NewStroke);
          this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Push(new DrawingAction((object) this.NewStroke));
          this.UndoButton.Visibility = Visibility.Visible;
          this.strokeStarted = true;
        }
        catch (Exception ex)
        {
          Log.l("UndoList:" + (object) this.CurrentItem.BindedMediaItem.DrawArgs.UndoList);
          Log.l("ColorPicker:" + (object) PicturePreviewPage.ColorPicker);
          Log.l("Canvas:" + (object) this.CurrentItem.BindedMediaItem.DrawArgs.Canvas);
          Log.SendCrashLog(ex, "drawing canvas mouse down");
        }
      }
    }

    private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseEventArgs e)
    {
      this.strokeStarted = false;
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
    {
      if (this.NewStroke == null || !this.strokeStarted || PicturePreviewPage.ColorPicker.Visibility != Visibility.Visible)
        return;
      this.NewStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints((UIElement) this.CurrentItem.BindedMediaItem.DrawArgs.Canvas));
    }

    private void DrawingCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      e.GetPosition((UIElement) this.DrawGrid);
      this.NewStroke = (Stroke) null;
    }

    private void CenterImage_LayoutUpdated(object sender, EventArgs e)
    {
      if (this.SlideView.CenterImage.ImageScale == this.imageScale && !this.force)
        return;
      this.imageScale = this.SlideView.CenterImage.ImageScale;
      this.force = false;
      this.UpdateDrawingPanelSize();
    }

    private void UpdateDrawingPanelSize()
    {
      MediaSharingState.IItem bindedMediaItem = this.CurrentItem?.BindedMediaItem;
      if (bindedMediaItem == null || bindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Image)
        return;
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      InkPresenter canvas = bindedMediaItem.DrawArgs.Canvas;
      canvas.Width = this.SlideView.CenterImage.Width;
      canvas.Height = this.SlideView.CenterImage.Height;
      this.DrawGrid.Width = this.SlideView.CenterImage.Width;
      this.DrawGrid.Height = this.SlideView.CenterImage.Height;
      double num5 = (canvas.Width - (double) this.CurrentItemOriginalWidth) / 2.0;
      double num6 = (canvas.Height - (double) this.CurrentItemOriginalHeight) / 2.0;
      if (bindedMediaItem.RelativeCropPos.HasValue)
      {
        num2 = bindedMediaItem.RelativeCropPos.Value.Y * (double) this.CurrentItemOriginalHeight;
        num1 = bindedMediaItem.RelativeCropPos.Value.X * (double) this.CurrentItemOriginalWidth;
      }
      if (bindedMediaItem.RelativeCropSize.HasValue)
      {
        Size? relativeCropSize = bindedMediaItem.RelativeCropSize;
        num4 = (1.0 - relativeCropSize.Value.Height) * (double) this.CurrentItemOriginalHeight - num2;
        relativeCropSize = bindedMediaItem.RelativeCropSize;
        num3 = (1.0 - relativeCropSize.Value.Width) * (double) this.CurrentItemOriginalWidth - num1;
      }
      if (bindedMediaItem.RotatedTimes == 1 || bindedMediaItem.RotatedTimes == 3)
        this.DrawGrid.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect(num5 + num1, num6 + num2, (double) this.SlideView.CenterImageSource.PixelHeight, (double) this.SlideView.CenterImageSource.PixelWidth)
        };
      else
        this.DrawGrid.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect(num5 + num1, num6 + num2, (double) this.SlideView.CenterImageSource.PixelWidth, (double) this.SlideView.CenterImageSource.PixelHeight)
        };
      if (this.CurrentItemOriginalWidth > this.CurrentItemOriginalHeight)
      {
        canvas.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        canvas.RenderTransform = (Transform) new CompositeTransform()
        {
          Rotation = 90.0
        };
        double x = (canvas.Width - (double) this.CurrentItemOriginalHeight) / 2.0;
        double y = (canvas.Height - (double) this.CurrentItemOriginalWidth) / 2.0;
        bindedMediaItem.DrawArgs.Canvas.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect(x, y, (double) this.CurrentItemOriginalHeight, (double) this.CurrentItemOriginalWidth)
        };
      }
      else
      {
        double x = (canvas.Width - (double) this.CurrentItemOriginalWidth) / 2.0;
        double y = (canvas.Height - (double) this.CurrentItemOriginalHeight) / 2.0;
        bindedMediaItem.DrawArgs.Canvas.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect(x, y, (double) this.CurrentItemOriginalWidth, (double) this.CurrentItemOriginalHeight)
        };
      }
      this.DrawGrid.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      CompositeTransform compositeTransform = new CompositeTransform();
      compositeTransform.ScaleX = this.SlideView.CenterImage.ImageScale;
      compositeTransform.ScaleY = this.SlideView.CenterImage.ImageScale;
      compositeTransform.TranslateY = (-num2 + num4) * this.SlideView.CenterImage.ImageScale / 2.0;
      compositeTransform.TranslateX = (-num1 + num3) * this.SlideView.CenterImage.ImageScale / 2.0;
      TransformGroup transformGroup = new TransformGroup();
      transformGroup.Children.Add((Transform) compositeTransform);
      transformGroup.Children.Add((Transform) new RotateTransform()
      {
        Angle = (double) (bindedMediaItem.RotatedTimes * 90)
      });
      this.DrawGrid.RenderTransform = (Transform) transformGroup;
    }

    private void ColorPicker_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      PicturePreviewPage.StrokeColor = this.GetColorPickerColor(PicturePreviewPage.ColorPicker, e.ManipulationOrigin.Y);
      this.PaintButtonBackground.Fill = (Brush) new SolidColorBrush()
      {
        Color = PicturePreviewPage.StrokeColor
      };
    }

    private void ColorPicker_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      Grid colorPicker = PicturePreviewPage.ColorPicker;
      System.Windows.Point point = e.ManipulationOrigin;
      double y1 = point.Y;
      point = e.DeltaManipulation.Translation;
      double y2 = point.Y;
      double PositionY = y1 + y2;
      PicturePreviewPage.StrokeColor = this.GetColorPickerColor(colorPicker, PositionY);
      this.PaintButtonBackground.Fill = (Brush) new SolidColorBrush()
      {
        Color = PicturePreviewPage.StrokeColor
      };
    }

    private void ColorPicker_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
    }

    private void ColorPicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      PicturePreviewPage.StrokeColor = this.GetColorPickerColor(PicturePreviewPage.ColorPicker, e.GetPosition((UIElement) PicturePreviewPage.ColorPicker).Y);
      this.PaintButtonBackground.Fill = (Brush) new SolidColorBrush()
      {
        Color = PicturePreviewPage.StrokeColor
      };
    }

    private Color GetColorPickerColor(Grid ColorPicker, double PositionY)
    {
      double num = ColorPicker.Height / 6.0;
      Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, (byte) 0);
      return PositionY >= num ? (PositionY >= num * 2.0 ? (PositionY >= num * 3.0 ? (PositionY >= num * 4.0 ? (PositionY >= num * 5.0 ? Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 0, (byte) (int) (1.0 - (PositionY - num * 5.0) / num * (double) byte.MaxValue)) : Color.FromArgb(byte.MaxValue, (byte) (int) ((PositionY - num * 4.0) / num * (double) byte.MaxValue), (byte) 0, byte.MaxValue)) : Color.FromArgb(byte.MaxValue, (byte) 0, (byte) (int) ((1.0 - (PositionY - num * 3.0) / num) * (double) byte.MaxValue), byte.MaxValue)) : Color.FromArgb(byte.MaxValue, (byte) 0, byte.MaxValue, (byte) (int) ((PositionY - num * 2.0) / num * (double) byte.MaxValue))) : Color.FromArgb(byte.MaxValue, (byte) (int) ((1.0 - (PositionY - num) / num) * (double) byte.MaxValue), byte.MaxValue, (byte) 0)) : Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) (int) (PositionY / num * (double) byte.MaxValue), (byte) 0);
    }

    public void openEmojiPicker()
    {
      if (this.emojiKeyboard == null)
      {
        this.emojiKeyboard = new EmojiKeyboard(new Action<Emoji.EmojiChar>(this.SelectEmoji));
        this.emojiKeyboard.EmojiPicker.Model.onEmojiSelectedAction_ = new Action<Emoji.EmojiChar>(this.SelectEmoji);
        this.emojiKeyboard.OwnerPage = (PhoneApplicationPage) this;
        this.emojiKeyboard.Opened += (EventHandler) ((sender, e) => this.IsEmojiKeyboardOpen = true);
        this.emojiKeyboard.Closed += (EventHandler) ((sender, e) => this.IsEmojiKeyboardOpen = false);
      }
      this.emojiKeyboard.Open();
    }

    public void SelectEmoji(Emoji.EmojiChar emojiChar)
    {
      IObservable<Emoji.EmojiChar.Args> image = emojiChar.Image;
      int fontSize = 72;
      double emojiSize = (double) fontSize * 1.175;
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Margin = new Thickness(emojiSize * 0.08, 0.0, emojiSize * 0.08, -emojiSize * 0.16);
      rectangle1.Width = emojiSize;
      rectangle1.Height = (double) fontSize;
      rectangle1.HorizontalAlignment = HorizontalAlignment.Center;
      rectangle1.VerticalAlignment = VerticalAlignment.Center;
      rectangle1.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      rectangle1.FlowDirection = FlowDirection.LeftToRight;
      Rectangle emojiRectangle = rectangle1;
      CompositeTransform transform = new CompositeTransform()
      {
        ScaleY = emojiSize / (double) fontSize
      };
      TransformGroup transformGroup = new TransformGroup();
      transformGroup.Children.Add((Transform) transform);
      emojiRectangle.RenderTransform = (Transform) transformGroup;
      transform.Rotation = (double) (-this.CurrentItem.BindedMediaItem.RotatedTimes * 90);
      IDisposable sub = (IDisposable) null;
      sub = image.Subscribe<Emoji.EmojiChar.Args>((Action<Emoji.EmojiChar.Args>) (res =>
      {
        PicturePreviewPage.AdjustForCrop(this.CurrentItem.BindedMediaItem, ref transform);
        CompositeTransform compositeTransform = new CompositeTransform()
        {
          ScaleX = emojiSize / res.Width,
          ScaleY = (double) fontSize / res.Height,
          TranslateX = -res.X * emojiSize / res.Width,
          TranslateY = -res.Y * (double) fontSize / res.Height
        };
        Rectangle rectangle2 = emojiRectangle;
        rectangle2.Fill = (Brush) new ImageBrush()
        {
          Transform = (Transform) compositeTransform,
          ImageSource = (System.Windows.Media.ImageSource) res.BaseImage,
          AlignmentX = AlignmentX.Left,
          AlignmentY = AlignmentY.Top,
          Stretch = Stretch.None
        };
        DrawingArgs drawArgs = this.CurrentItem.BindedMediaItem.DrawArgs;
        double startScaleY = transform.ScaleY;
        double startScaleX = transform.ScaleX;
        double startAngle = transform.Rotation;
        emojiRectangle.ManipulationStarted += (EventHandler<ManipulationStartedEventArgs>) ((send, ev) =>
        {
          CompositeTransform previousState = new CompositeTransform()
          {
            TranslateX = transform.TranslateX,
            TranslateY = transform.TranslateY,
            ScaleY = transform.ScaleY,
            ScaleX = transform.ScaleX,
            Rotation = transform.Rotation
          };
          startScaleY = transform.ScaleY;
          startScaleX = transform.ScaleX;
          startAngle = transform.Rotation;
          this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Push(new DrawingAction((object) emojiRectangle, (object) previousState, DrawingAction.DrawingActionType.Transform));
          this.UndoButton.Visibility = Visibility.Visible;
        });
        emojiRectangle.ManipulationDelta += (EventHandler<ManipulationDeltaEventArgs>) ((send, ev) =>
        {
          if (ev.PinchManipulation != null)
          {
            double cumulativeScale = ev.PinchManipulation.CumulativeScale;
            if (transform.ScaleY * cumulativeScale < 10.0 && transform.ScaleY * cumulativeScale > 0.5)
            {
              transform.ScaleY = startScaleY * cumulativeScale;
              transform.ScaleX = startScaleX * cumulativeScale;
            }
            transform.Rotation = startAngle - UIUtils.AngleBetween2Lines(ev.PinchManipulation.Original, ev.PinchManipulation.Current);
          }
          else
          {
            if (ev.DeltaManipulation == null)
              return;
            System.Windows.Point point = new RotateTransform()
            {
              Angle = transform.Rotation
            }.Transform(ev.DeltaManipulation.Translation);
            transform.TranslateX += point.X * transform.ScaleX;
            transform.TranslateY += point.Y * transform.ScaleY;
          }
        });
        drawArgs.HasDrawing = true;
        this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.ChangedPreviewItem, this.CurrentItem);
        this.DrawGrid.Children.Add((UIElement) emojiRectangle);
        this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Push(new DrawingAction((object) emojiRectangle));
        this.UndoButton.Visibility = Visibility.Visible;
        sub.SafeDispose();
        sub = (IDisposable) null;
      }), (Action<Exception>) (ex =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
      this.CloseEmojiKeyboard();
    }

    public void CloseEmojiKeyboard()
    {
      if (this.emojiKeyboard == null)
        return;
      this.emojiKeyboard.Close();
    }

    private void EmojiButton_Click(object sender, EventArgs e)
    {
      if (this.CurrentItem?.BindedMediaItem?.DrawArgs == null)
        return;
      this.openEmojiPicker();
    }

    private void TextButton_Click(object send, EventArgs eventArgs)
    {
      this.CloseEmojiKeyboard();
      if (this.CurrentItem?.BindedMediaItem?.DrawArgs == null)
        return;
      Log.l("text button clicked");
      Grid colorPickerGlobal = this.TextColorPickerGlobal;
      if ((colorPickerGlobal != null ? (colorPickerGlobal.Visibility == Visibility.Visible ? 1 : 0) : 0) != 0)
      {
        this.HideTextColorPicker();
      }
      else
      {
        this.ShowTextColorPicker();
        EmojiTextBox emojiTextBox = new EmojiTextBox();
        emojiTextBox.Margin = new Thickness(6.0, 6.0, 6.0, 6.0);
        emojiTextBox.Visibility = Visibility.Collapsed;
        emojiTextBox.TextWrapping = TextWrapping.Wrap;
        emojiTextBox.ShowTail = false;
        emojiTextBox.CounterLocation = EmojiTextBox.Alignment.TopRight;
        emojiTextBox.VerticalAlignment = VerticalAlignment.Bottom;
        emojiTextBox.MaxHeight = 180.0;
        emojiTextBox.MaxLength = 512;
        emojiTextBox.CacheMode = (CacheMode) new BitmapCache();
        EmojiTextBox textbox = emojiTextBox;
        RichTextBlock richTextBlock = new RichTextBlock();
        richTextBlock.FontSize = (double) this.DrawTextFontSize;
        richTextBlock.TextWrapping = TextWrapping.Wrap;
        richTextBlock.Visibility = Visibility.Visible;
        richTextBlock.VerticalAlignment = VerticalAlignment.Center;
        richTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
        richTextBlock.Foreground = (Brush) new SolidColorBrush(this.TextStrokeColor);
        richTextBlock.Text = new RichTextBlock.TextSet()
        {
          Text = ""
        };
        richTextBlock.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        richTextBlock.CacheMode = (CacheMode) new BitmapCache();
        RichTextBlock displaybox = richTextBlock;
        CompositeTransform transform = new CompositeTransform();
        PicturePreviewPage.AdjustForCrop(this.CurrentItem.BindedMediaItem, ref transform);
        RotateTransform rotateTransform = new RotateTransform()
        {
          Angle = (double) (-this.CurrentItem.BindedMediaItem.RotatedTimes * 90)
        };
        TransformGroup transformGroup = new TransformGroup();
        transformGroup.Children.Add((Transform) transform);
        transformGroup.Children.Add((Transform) rotateTransform);
        displaybox.RenderTransform = (Transform) transformGroup;
        this.DrawingModeTextBox.Foreground = displaybox.Foreground;
        if (this.TextColorPickerGlobal == null)
        {
          Grid grid = new Grid();
          grid.Height = (this.screenRenderHeight_ - UIUtils.SIPHeightPortrait) * 0.8;
          grid.Width = 30.0;
          grid.HorizontalAlignment = HorizontalAlignment.Right;
          grid.VerticalAlignment = VerticalAlignment.Center;
          Grid TextColorPicker = grid;
          Border border = this.newColorPicker((int) TextColorPicker.Height);
          border.ManipulationStarted += (EventHandler<ManipulationStartedEventArgs>) ((sender, e) => this.TextBoxColorChange(TextColorPicker, e.ManipulationOrigin.Y));
          border.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) =>
          {
            this.TextBoxColorChange(TextColorPicker, e.GetPosition((UIElement) TextColorPicker).Y);
            this.TextColorPickerJustTapped = true;
            e.Handled = true;
          });
          border.ManipulationCompleted += (EventHandler<ManipulationCompletedEventArgs>) ((sender, e) => this.TextBoxColorChange(TextColorPicker, e.ManipulationOrigin.Y));
          border.ManipulationDelta += (EventHandler<ManipulationDeltaEventArgs>) ((sender, e) =>
          {
            Grid textColorPicker = TextColorPicker;
            System.Windows.Point point = e.ManipulationOrigin;
            double y1 = point.Y;
            point = e.DeltaManipulation.Translation;
            double y2 = point.Y;
            double y3 = y1 + y2;
            this.TextBoxColorChange(textColorPicker, y3);
          });
          TextColorPicker.Children.Add((UIElement) border);
          this.DrawingModeTextGrid.Children.Add((UIElement) TextColorPicker);
          this.TextColorPickerGlobal = TextColorPicker;
        }
        textbox.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>((Action<TextChangedEventArgs>) (args => this.DrawingModeTextBox.Text = new RichTextBlock.TextSet()
        {
          Text = textbox.Text
        }));
        textbox.TextBoxFocusChangedObservable().Subscribe<bool>((Action<bool>) (focused =>
        {
          if (focused)
            return;
          if (this.TextColorPickerJustTapped)
          {
            textbox.Focus();
            textbox.OpenTextKeyboard();
            this.TextColorPickerJustTapped = false;
          }
          else
          {
            int num = displaybox.Text != this.DrawingModeTextBox.Text ? 1 : (displaybox.Foreground != this.DrawingModeTextBox.Foreground ? 1 : 0);
            RichTextBlock previousState = new RichTextBlock()
            {
              Text = displaybox.Text,
              Foreground = displaybox.Foreground
            };
            displaybox.Text = this.DrawingModeTextBox.Text;
            displaybox.Foreground = this.DrawingModeTextBox.Foreground;
            if (num != 0)
            {
              this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Push(new DrawingAction((object) displaybox, (object) previousState, DrawingAction.DrawingActionType.AlterText));
              this.UndoButton.Visibility = Visibility.Visible;
            }
            this.DrawingModeTextGrid.Visibility = Visibility.Collapsed;
            this.HideTextColorPicker();
          }
        }));
        textbox.KeyDown += (KeyEventHandler) ((sender, ev) =>
        {
          if (ev.Key != System.Windows.Input.Key.Enter)
            return;
          ev.Handled = true;
          this.OnEnterKeyPressed();
        });
        displaybox.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, ev) =>
        {
          textbox.MaxLength = 139;
          this.fadeInSbSub_.SafeDispose();
          this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(350.0), (DependencyObject) textbox, true, (Action) (() =>
          {
            this.DrawingModeTextGrid.Visibility = Visibility.Visible;
            this.DrawingModeTextBox.Text = displaybox.Text;
            this.DrawingModeTextBox.Foreground = displaybox.Foreground;
            textbox.Text = displaybox.Text.Text;
            if (this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Count <= 0)
              return;
            this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Pop();
          }), false, "pic preview: fade in text box");
          textbox.OpenTextKeyboard();
        });
        double startScaleY = transform.ScaleY;
        double startScaleX = transform.ScaleX;
        displaybox.ManipulationStarted += (EventHandler<ManipulationStartedEventArgs>) ((sender, ev) =>
        {
          CompositeTransform previousState = new CompositeTransform()
          {
            TranslateX = transform.TranslateX,
            TranslateY = transform.TranslateY,
            ScaleY = transform.ScaleY,
            ScaleX = transform.ScaleX
          };
          startScaleY = transform.ScaleY;
          startScaleX = transform.ScaleX;
          this.CurrentItem.BindedMediaItem.DrawArgs.UndoList.Push(new DrawingAction((object) displaybox, (object) previousState, DrawingAction.DrawingActionType.Transform));
          this.UndoButton.Visibility = Visibility.Visible;
        });
        displaybox.ManipulationDelta += (EventHandler<ManipulationDeltaEventArgs>) ((sender, ev) =>
        {
          if (ev.PinchManipulation != null)
          {
            double cumulativeScale = ev.PinchManipulation.CumulativeScale;
            if (transform.ScaleY * cumulativeScale >= 10.0 || transform.ScaleY * cumulativeScale <= 0.5)
              return;
            transform.ScaleY = startScaleY * cumulativeScale;
            transform.ScaleX = startScaleX * cumulativeScale;
          }
          else
          {
            if (ev.DeltaManipulation == null)
              return;
            System.Windows.Point translation = ev.DeltaManipulation.Translation;
            transform.TranslateX += translation.X * transform.ScaleX;
            transform.TranslateY += translation.Y * transform.ScaleY;
          }
        });
        this.DrawGrid.Children.Add((UIElement) displaybox);
        this.LayoutRoot.Children.Add((UIElement) textbox);
        this.CurrentItem.BindedMediaItem.DrawArgs.HasDrawing = true;
        this.updatePickerFieldStats(PicturePreviewPage.StatToUpdate.ChangedPreviewItem, this.CurrentItem);
        this.fadeInSbSub_.SafeDispose();
        this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(350.0), (DependencyObject) textbox, true, (Action) (() =>
        {
          this.DrawingModeTextGrid.Visibility = Visibility.Visible;
          this.DrawingModeTextBox.Text = displaybox.Text;
        }), false, "pic preview: fade in text box");
        textbox.OpenTextKeyboard();
      }
    }

    private void TextBoxColorChange(Grid textColorPicker, double y)
    {
      this.TextStrokeColor = this.GetColorPickerColor(textColorPicker, y);
      SolidColorBrush solidColorBrush = new SolidColorBrush(this.TextStrokeColor);
      this.DrawingModeTextBox.Foreground = (Brush) solidColorBrush;
      this.TextButtonBackground.Fill = (Brush) solidColorBrush;
    }

    private void ShowTextColorPicker()
    {
      if (this.TextColorPickerGlobal != null)
        this.TextColorPickerGlobal.Visibility = Visibility.Visible;
      this.TextButtonBackground.Fill = (Brush) new SolidColorBrush(this.TextStrokeColor);
      if (PicturePreviewPage.ColorPicker.Visibility != Visibility.Visible)
        return;
      this.PaintButton_Click((object) null, (EventArgs) null);
    }

    private void HideTextColorPicker()
    {
      this.TextColorPickerGlobal.Visibility = Visibility.Collapsed;
      this.TextButtonBackground.Fill = (Brush) UIUtils.TransparentBrush;
    }

    private void PaintButton_Click(object sender, EventArgs e)
    {
      this.CloseEmojiKeyboard();
      if (PicturePreviewPage.ColorPicker.Visibility == Visibility.Collapsed)
      {
        if (this.CurrentItem?.BindedMediaItem?.DrawArgs == null)
          return;
        PicturePreviewPage.ColorPicker.Visibility = Visibility.Visible;
        this.PaintButtonBackground.Fill = (Brush) new SolidColorBrush()
        {
          Color = PicturePreviewPage.StrokeColor
        };
        Log.l("paint button opened");
      }
      else
      {
        PicturePreviewPage.ColorPicker.Visibility = Visibility.Collapsed;
        this.PaintButtonBackground.Fill = (Brush) UIUtils.TransparentBrush;
      }
    }

    private void UndoButton_Click(object sender, EventArgs e)
    {
      this.CloseEmojiKeyboard();
      DrawingArgs drawArgs = this.CurrentItem?.BindedMediaItem?.DrawArgs;
      if (drawArgs == null || drawArgs.UndoList == null || drawArgs.UndoList.Count <= 0)
        return;
      Log.l("undo button clicked");
      DrawingAction drawingAction = drawArgs.UndoList.Pop();
      if (drawingAction.item is Stroke)
        drawArgs.Canvas.Strokes.Remove((Stroke) drawingAction.item);
      else if (drawingAction.item is Rectangle)
      {
        if (drawingAction.type == DrawingAction.DrawingActionType.Add)
          drawArgs.DrawGrid.Children.Remove((UIElement) drawingAction.item);
        else if (drawingAction.type == DrawingAction.DrawingActionType.Transform)
        {
          CompositeTransform previousState = (CompositeTransform) drawingAction.previousState;
          CompositeTransform child = (CompositeTransform) ((TransformGroup) ((UIElement) drawingAction.item).RenderTransform).Children[0];
          child.TranslateX = previousState.TranslateX;
          child.TranslateY = previousState.TranslateY;
          child.ScaleY = previousState.ScaleY;
          child.ScaleX = previousState.ScaleX;
          child.Rotation = previousState.Rotation;
        }
      }
      else if (drawingAction.item is RichTextBlock)
      {
        if (drawingAction.type == DrawingAction.DrawingActionType.Add)
          drawArgs.DrawGrid.Children.Remove((UIElement) drawingAction.item);
        else if (drawingAction.type == DrawingAction.DrawingActionType.AlterText)
        {
          ((RichTextBlock) drawingAction.item).Text = ((RichTextBlock) drawingAction.previousState).Text;
          ((Control) drawingAction.item).Foreground = ((Control) drawingAction.previousState).Foreground;
        }
        else if (drawingAction.type == DrawingAction.DrawingActionType.Transform)
        {
          CompositeTransform previousState = (CompositeTransform) drawingAction.previousState;
          CompositeTransform child = (CompositeTransform) ((TransformGroup) ((UIElement) drawingAction.item).RenderTransform).Children[0];
          child.TranslateX = previousState.TranslateX;
          child.TranslateY = previousState.TranslateY;
          child.ScaleY = previousState.ScaleY;
          child.ScaleX = previousState.ScaleX;
        }
      }
      if (drawArgs.UndoList.Count != 0)
        return;
      this.UndoButton.Visibility = Visibility.Collapsed;
    }

    private void CropDoneButton_Click(object sender, EventArgs e) => this.ExitCrop(true);

    public void UpdateRecipientsTextBlock()
    {
      string str = (string) null;
      if (this.sharingState_?.RecipientJids != null && ((IEnumerable<string>) this.sharingState_.RecipientJids).Count<string>() > 0)
      {
        if (((IEnumerable<string>) this.sharingState_.RecipientJids).Count<string>() == 1)
        {
          string jid = this.sharingState_.RecipientJids[0];
          JidHelper.JidTypes jidType = JidHelper.GetJidType(jid);
          string recipientName = (string) null;
          switch (jidType)
          {
            case JidHelper.JidTypes.User:
              UserStatus userStatus = UserCache.Get(this.sharingState_.RecipientJids[0], false);
              if (userStatus != null)
              {
                string displayName = userStatus.GetDisplayName(true);
                if (!string.IsNullOrEmpty(displayName))
                {
                  recipientName = displayName;
                  break;
                }
                break;
              }
              break;
            case JidHelper.JidTypes.Group:
            case JidHelper.JidTypes.Broadcast:
              Conversation convo = (Conversation) null;
              MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
              {
                if (string.IsNullOrEmpty(jid))
                  return;
                convo = db.GetConversation(jid, CreateOptions.None, out CreateResult _);
                if (convo == null || !convo.IsGroup() || string.IsNullOrEmpty(convo.GroupSubject))
                  return;
                recipientName = convo.GroupSubject;
              }));
              break;
            case JidHelper.JidTypes.Status:
              recipientName = AppResources.MyStatusV3Title;
              break;
          }
          if (recipientName != null)
            str = string.Format(AppResources.SendingToRecipient, (object) recipientName);
        }
        if (str == null)
          str = Plurals.Instance.GetStringWithIndex(AppResources.SendingToNRecipientsPlural, 0, (object) ((IEnumerable<string>) this.sharingState_.RecipientJids).Count<string>());
      }
      if (str == null)
      {
        this.Recipients.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.Recipients.Visibility = Visibility.Visible;
        this.Recipients.Text = str;
      }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TimeSpentManager.GetInstance().UserAction();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/PicturePreviewPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SlideViewPanel = (Grid) this.FindName("SlideViewPanel");
      this.SliderViewTransform = (CompositeTransform) this.FindName("SliderViewTransform");
      this.SlideView = (ImageSlideViewControl) this.FindName("SlideView");
      this.DrawingMode = (Grid) this.FindName("DrawingMode");
      this.DrawingModeTransform = (CompositeTransform) this.FindName("DrawingModeTransform");
      this.InnerLayout = (Grid) this.FindName("InnerLayout");
      this.VideoPlayer = (MediaElement) this.FindName("VideoPlayer");
      this.VideoCropRectangle = (RectangleGeometry) this.FindName("VideoCropRectangle");
      this.EditControl = (ImageEditControl) this.FindName("EditControl");
      this.AppBarGradient = (Rectangle) this.FindName("AppBarGradient");
      this.AppBar = (Grid) this.FindName("AppBar");
      this.AppBarButtonsPanel = (StackPanel) this.FindName("AppBarButtonsPanel");
      this.AppBarButtonsPanelTransform = (CompositeTransform) this.FindName("AppBarButtonsPanelTransform");
      this.UndoButton = (Button) this.FindName("UndoButton");
      this.DeleteButton = (Button) this.FindName("DeleteButton");
      this.CropButton = (Button) this.FindName("CropButton");
      this.RotateButton = (Button) this.FindName("RotateButton");
      this.GifButton = (Button) this.FindName("GifButton");
      this.VideoToggleForeground = (Rectangle) this.FindName("VideoToggleForeground");
      this.GifToggleForeground = (Rectangle) this.FindName("GifToggleForeground");
      this.EmojiButton = (Button) this.FindName("EmojiButton");
      this.TextButton = (Button) this.FindName("TextButton");
      this.TextButtonBackground = (Ellipse) this.FindName("TextButtonBackground");
      this.PaintButton = (Button) this.FindName("PaintButton");
      this.PaintButtonBackground = (Ellipse) this.FindName("PaintButtonBackground");
      this.DurationBox = (Grid) this.FindName("DurationBox");
      this.DurationGradient = (Rectangle) this.FindName("DurationGradient");
      this.Duration = (TextBlock) this.FindName("Duration");
      this.PlayButton = (RoundButton) this.FindName("PlayButton");
      this.BottomPanel = (StackPanel) this.FindName("BottomPanel");
      this.CaptionAndThumbList = (StackPanel) this.FindName("CaptionAndThumbList");
      this.CaptionAndThumbListTransform = (CompositeTransform) this.FindName("CaptionAndThumbListTransform");
      this.AddButton = (Grid) this.FindName("AddButton");
      this.AddButtonImage = (Image) this.FindName("AddButtonImage");
      this.CaptionBackground = (Rectangle) this.FindName("CaptionBackground");
      this.CaptionPanel = (ScrollViewer) this.FindName("CaptionPanel");
      this.CaptionBlock = (RichTextBlock) this.FindName("CaptionBlock");
      this.SubmitButton = (Button) this.FindName("SubmitButton");
      this.SubmitButtonIcon = (Image) this.FindName("SubmitButtonIcon");
      this.ThumbnailsPanel = (Grid) this.FindName("ThumbnailsPanel");
      this.PreviewItemsPanel = (ScrollViewer) this.FindName("PreviewItemsPanel");
      this.ScrollingPart = (Grid) this.FindName("ScrollingPart");
      this.PreviewItemsList = (ListBox) this.FindName("PreviewItemsList");
      this.Recipients = (TextBlock) this.FindName("Recipients");
      this.CropBar = (Grid) this.FindName("CropBar");
      this.CropControlPanel = (Grid) this.FindName("CropControlPanel");
      this.CropControlPanelTransform = (CompositeTransform) this.FindName("CropControlPanelTransform");
      this.CropDoneButton = (Button) this.FindName("CropDoneButton");
      this.CropAspectRatioPicker = (ListPicker) this.FindName("CropAspectRatioPicker");
      this.VideoTimelinePanel = (Canvas) this.FindName("VideoTimelinePanel");
      this.VideoTimelineList = (ListBox) this.FindName("VideoTimelineList");
      this.CurrentTimeStrip = (Border) this.FindName("CurrentTimeStrip");
      this.CurrentTimeStripTranslate = (CompositeTransform) this.FindName("CurrentTimeStripTranslate");
      this.TimelineLeftHandle = (Grid) this.FindName("TimelineLeftHandle");
      this.LeftHandleTransform = (CompositeTransform) this.FindName("LeftHandleTransform");
      this.LeftHandleOverlay = (Rectangle) this.FindName("LeftHandleOverlay");
      this.LeftHandle = (Rectangle) this.FindName("LeftHandle");
      this.TimelineHandleRight = (Grid) this.FindName("TimelineHandleRight");
      this.RightHandleTransform = (CompositeTransform) this.FindName("RightHandleTransform");
      this.RightHandleOverlay = (Rectangle) this.FindName("RightHandleOverlay");
      this.RightHandle = (Rectangle) this.FindName("RightHandle");
      this.CaptionBox = (EmojiTextBox) this.FindName("CaptionBox");
    }

    public class PreviewItem : PropChangedBase
    {
      private bool isBeingViewed_;

      public bool IsBeingViewed
      {
        get => this.isBeingViewed_;
        set
        {
          if (this.isBeingViewed_ == value)
            return;
          this.isBeingViewed_ = value;
          this.NotifyPropertyChanged("BorderVisibility");
        }
      }

      public IObservable<BitmapSource> GetBitmapObservable(
        bool async,
        Size maxSize,
        bool forceUpdate = false)
      {
        if (async)
          return (IObservable<BitmapSource>) this.BindedMediaItem.GetBitmapAsync(maxSize);
        return this.BindedMediaItem.VideoInfo != null && this.BindedMediaItem.LargeThumb == null | forceUpdate ? (IObservable<BitmapSource>) this.BindedMediaItem.FetchLargeThumbAsync() : Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
        {
          WriteableBitmap bitmap = this.BindedMediaItem.GetBitmap(maxSize);
          observer.OnNext((BitmapSource) bitmap);
          observer.OnCompleted();
          return (Action) (() => { });
        }));
      }

      public bool IsLast { get; set; }

      public Brush Background => (Brush) null;

      public Visibility BorderVisibility => this.IsBeingViewed.ToVisibility();

      public System.Windows.Media.ImageSource PlayButtonIcon
      {
        get
        {
          if (this.BindedMediaItem == null)
            return (System.Windows.Media.ImageSource) null;
          return this.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Video && this.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Gif ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.WhitePlayButton;
        }
      }

      public System.Windows.Media.ImageSource GifIcon
      {
        get
        {
          if (this.BindedMediaItem == null)
            return (System.Windows.Media.ImageSource) null;
          return this.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Gif ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.GifIcon;
        }
      }

      public Visibility PlayOverlayVisibility
      {
        get
        {
          return this.BindedMediaItem == null ? false.ToVisibility() : (this.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Video || this.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Gif).ToVisibility();
        }
      }

      public Visibility GifOverlayVisibility
      {
        get
        {
          return this.BindedMediaItem == null ? false.ToVisibility() : (this.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Gif).ToVisibility();
        }
      }

      public string Caption
      {
        get => this.BindedMediaItem.Caption;
        set => this.BindedMediaItem.Caption = value;
      }

      public MediaSharingState.IItem BindedMediaItem { get; private set; }

      public BitmapSource Thumbnail
      {
        get
        {
          return this.BindedMediaItem != null ? this.BindedMediaItem.GetThumbnail() : (BitmapSource) null;
        }
        set
        {
          this.BindedMediaItem.ThumbnailOverride = value;
          this.NotifyPropertyChanged(nameof (Thumbnail));
        }
      }

      public int ThumbRotationAngle => this.BindedMediaItem != null ? this.RotatedTimes * 90 : 0;

      public Thickness ItemMargin => new Thickness(10.0, 0.0, this.IsLast ? 10.0 : 0.0, 8.0);

      public int RotatedTimes => this.BindedMediaItem.RotatedTimes;

      public PreviewItem(MediaSharingState.IItem item) => this.BindedMediaItem = item;

      public void Rotate()
      {
        this.BindedMediaItem.RotatedTimes = (this.RotatedTimes + 1) % 4;
        this.NotifyPropertyChanged("ThumbRotationAngle");
      }
    }

    public class VideoFrameItem : PropChangedBase
    {
      private BitmapSource frameThumbnail;
      private int thumbRotationAngle;
      private int originalRotationAngle;

      public BitmapSource FrameThumbnail
      {
        get => this.frameThumbnail;
        set
        {
          this.frameThumbnail = value;
          this.NotifyPropertyChanged(nameof (FrameThumbnail));
        }
      }

      public int ThumbRotationAngle
      {
        get => this.thumbRotationAngle;
        set
        {
          if (value == this.thumbRotationAngle)
            return;
          this.thumbRotationAngle = value % 360;
          this.NotifyPropertyChanged(nameof (ThumbRotationAngle));
          this.NotifyPropertyChanged("UserRotationAngle");
        }
      }

      public int UserRotationAngle => this.thumbRotationAngle - this.originalRotationAngle;

      public VideoFrameItem(BitmapSource b, int rotation)
      {
        this.ThumbRotationAngle = rotation;
        this.originalRotationAngle = this.ThumbRotationAngle;
        this.FrameThumbnail = b;
      }
    }

    public class CropRatioItem : WaViewModelBase
    {
      private double width_;
      private double height_;
      private bool isSelected_;
      private string title_;
      private Thickness itemMargin_ = new Thickness(0.0, 12.0, 0.0, 18.0);
      private double? aspectRatio_;

      public bool IsSelected
      {
        get => this.isSelected_;
        set
        {
          if (this.isSelected_ == value)
            return;
          this.isSelected_ = value;
          this.NotifyPropertyChanged("TitleBrush");
        }
      }

      public string TitleStr
      {
        get
        {
          return this.title_ ?? string.Format("{0}x{1}", (object) (int) this.width_, (object) (int) this.height_);
        }
        set => this.title_ = value;
      }

      public Brush TitleBrush
      {
        get => !this.IsSelected ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }

      public double RectWidth => this.width_ > this.height_ ? 44.0 : 32.0;

      public double RectHeight => this.height_ / this.width_ * this.RectWidth;

      public Brush RectStrokeBrush
      {
        get
        {
          return !ImageStore.IsDarkTheme() ? (Brush) new SolidColorBrush(Colors.DarkGray) : (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 99, (byte) 99, (byte) 99));
        }
      }

      public double RectStrokeThickness { get; set; }

      public bool UseDashRectStrokeStyle { get; set; }

      public DoubleCollection RectStrokeDashArray
      {
        get
        {
          if (!this.UseDashRectStrokeStyle)
            return (DoubleCollection) null;
          DoubleCollection rectStrokeDashArray = new DoubleCollection();
          rectStrokeDashArray.Add(2.0);
          return rectStrokeDashArray;
        }
      }

      public Thickness ItemMargin
      {
        get => this.itemMargin_;
        set => this.itemMargin_ = value;
      }

      public Visibility ItemVisibility => (!this.IsDummy).ToVisibility();

      public double AspectRatio
      {
        get
        {
          return this.aspectRatio_ ?? (this.aspectRatio_ = new double?(this.width_ / this.height_)).Value;
        }
        set => this.aspectRatio_ = new double?(value);
      }

      public bool IsDummy { get; set; }

      public CropRatioItem(double w, double h)
      {
        this.width_ = w;
        this.height_ = h;
        this.RectStrokeThickness = 2.0;
      }
    }

    private enum StatToUpdate
    {
      AddedPreviewItem = 1,
      ChangedPreviewItem = 2,
      DeletedPreviewItem = 3,
    }

    private class PIDetails
    {
      public int StateIndicator;
      public wam_enum_media_picker_origin_type Origin;

      public PIDetails(int stateIndicator, wam_enum_media_picker_origin_type origin)
      {
        this.StateIndicator = stateIndicator;
        this.Origin = origin;
      }
    }
  }
}
