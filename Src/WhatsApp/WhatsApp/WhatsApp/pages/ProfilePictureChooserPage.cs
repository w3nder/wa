// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProfilePictureChooserPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class ProfilePictureChooserPage : PhoneApplicationPage
  {
    private PageOrientation _originalOrientation;
    private int loadedCount;
    private string Jid;
    private string GroupSubject;
    private Conversation convo;
    private IDisposable imageSubscription;
    private IDisposable pickerSubscription;
    private static IObserver<ProfilePictureChooserPage.ProfilePictureChooserArgs> _observer;
    public static System.Windows.Media.ImageSource _preview;
    private static Popup _animationPopup;
    private static bool _animationEnded;
    private static bool _dismissRequested;
    internal Storyboard LoadStoryboard;
    internal Grid LayoutRoot;
    internal Image PreviewImage;
    internal Rectangle PreviewImageShadow;
    internal TextBlock SubjectTitle;
    internal ExternalImagePicker ImagePicker;
    private bool _contentLoaded;

    public ProfilePictureChooserPage()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.ProfilePictureChooserPage_Loaded);
      this.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.ProfilePictureChooserPage_OrientationChanged);
      this._originalOrientation = this.Orientation;
      if (!this.Orientation.IsLandscape())
        return;
      this.OnOrientationChanged(this.Orientation);
    }

    private void ProfilePictureChooserPage_OrientationChanged(
      object sender,
      OrientationChangedEventArgs e)
    {
      this.OnOrientationChanged(e.Orientation);
    }

    private void ProfilePictureChooserPage_Loaded(object sender, RoutedEventArgs e)
    {
      ++this.loadedCount;
      if (this.loadedCount <= 1)
        return;
      this.LoadStoryboard.Begin();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (ProfilePictureChooserPage._observer == null)
        NavUtils.GoBack(this.NavigationService);
      this.NavigationContext.QueryString.TryGetValue("jid", out this.Jid);
      this.NavigationContext.QueryString.TryGetValue("groupSubject", out this.GroupSubject);
      this.Initialize();
      if (ProfilePictureChooserPage._animationPopup != null)
        ((ProfilePictureChooserPage._animationPopup.Child as Canvas).Children[0] as Rectangle).Opacity = 0.0;
      ++this.loadedCount;
      if (this.loadedCount > 1)
        this.LoadStoryboard.Begin();
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back && ProfilePictureChooserPage._animationPopup != null && this._originalOrientation == this.Orientation)
        ProfilePictureChooserPage.PlayExitAnimation(this.PreviewImage.Source);
      base.OnNavigatingFrom(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back && ProfilePictureChooserPage._animationPopup != null)
        ((ProfilePictureChooserPage._animationPopup.Child as Canvas).Children[0] as Rectangle).Opacity = 0.0;
      this.imageSubscription.SafeDispose();
      this.imageSubscription = (IDisposable) null;
      base.OnNavigatedFrom(e);
    }

    private void OnOrientationChanged(PageOrientation orientation)
    {
      if (orientation.IsLandscape())
      {
        this.SubjectTitle.Margin = new Thickness(96.0, 10.0, 24.0, 24.0);
        this.LayoutRoot.RowDefinitions.Clear();
        this.LayoutRoot.ColumnDefinitions.Clear();
        this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = GridLength.Auto
        });
        this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        Grid.SetRow((FrameworkElement) this.ImagePicker, 0);
        Grid.SetColumn((FrameworkElement) this.ImagePicker, 1);
      }
      else
      {
        this.SubjectTitle.Margin = new Thickness(24.0, 44.0, 24.0, 24.0);
        this.LayoutRoot.ColumnDefinitions.Clear();
        this.LayoutRoot.RowDefinitions.Clear();
        this.LayoutRoot.RowDefinitions.Add(new RowDefinition()
        {
          Height = GridLength.Auto
        });
        this.LayoutRoot.RowDefinitions.Add(new RowDefinition()
        {
          Height = new GridLength(1.0, GridUnitType.Star)
        });
        Grid.SetRow((FrameworkElement) this.ImagePicker, 1);
        Grid.SetColumn((FrameworkElement) this.ImagePicker, 0);
      }
      this.ImagePicker.Orientation = orientation;
    }

    private void Initialize()
    {
      ExternalImagePicker.ExternalImagePickerActions imagePickerActions1 = ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums;
      ExternalImagePicker.ExternalImagePickerActions imagePickerActions2 = this.Jid == null || this.Jid.IsGroupJid() ? imagePickerActions1 | ExternalImagePicker.ExternalImagePickerActions.SearchWeb | ExternalImagePicker.ExternalImagePickerActions.TakeGroupPhoto : imagePickerActions1 | ExternalImagePicker.ExternalImagePickerActions.TakePhoto;
      if (this.Jid == null)
      {
        System.Windows.Media.ImageSource preview = ProfilePictureChooserPage._preview;
        if (preview != null)
        {
          this.PreviewImage.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewImage_Tap);
          this.PreviewImage.Source = preview;
          this.PreviewImage.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewImage_Tap);
          this.SetImageSourceAttributes();
          imagePickerActions2 |= ExternalImagePicker.ExternalImagePickerActions.Delete;
        }
        ProfilePictureChooserPage.DismissEntranceAnimation();
      }
      else if (this.Jid.IsGroupJid())
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.convo = db.GetConversation(this.Jid, CreateOptions.None)));
        if (this.convo == null)
          NavUtils.GoBack();
        this.SubjectTitle.Text = Emoji.ConvertToTextOnly(this.convo.GroupSubject ?? "", (byte[]) null).ToUpper();
        string photoId = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(this.Jid, CreateOptions.None);
          if (chatPictureState == null)
            return;
          photoId = chatPictureState.WaPhotoId;
        }));
        if (photoId != null)
          imagePickerActions2 |= ExternalImagePicker.ExternalImagePickerActions.Delete;
        this.SetImageSource(this.convo.Jid);
      }
      else if (this.Jid.IsUserJid())
      {
        string photoId = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(this.Jid, CreateOptions.None);
          if (chatPictureState == null)
            return;
          photoId = chatPictureState.WaPhotoId;
        }));
        if (photoId != null)
          imagePickerActions2 |= ExternalImagePicker.ExternalImagePickerActions.Delete;
        this.SetImageSource(this.Jid);
      }
      if (this.pickerSubscription == null)
      {
        string deleteConfirmation = this.Jid.IsGroupJid() ? AppResources.RemoveGroupIconConfirm : AppResources.RemovePictureConfirm;
        this.pickerSubscription = this.ImagePicker.Start(this.GroupSubject ?? (this.Jid.IsGroupJid() ? this.convo.GroupSubject : (string) null), "Square", 0, deleteConfirmation, nameof (ProfilePictureChooserPage)).Subscribe<ExternalImagePicker.ExternalImagePickerArgs>(new Action<ExternalImagePicker.ExternalImagePickerArgs>(this.OnImageSelected), new Action<Exception>(this.OnError));
      }
      this.ImagePicker.EnabledActions = imagePickerActions2;
    }

    private void SetImageSource(string jid)
    {
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.imageSubscription = ChatPictureStore.Get(jid, true, false, false).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
      {
        bool flag = true;
        this.PreviewImage.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewImage_Tap);
        if (picState.Image == null)
        {
          this.PreviewImage.Source = (System.Windows.Media.ImageSource) AssetStore.GetDefaultChatIcon(jid);
        }
        else
        {
          if (picState.Image is BitmapImage image2 && image2.PixelHeight == 0 && image2.PixelWidth == 0)
          {
            image2.ImageOpened += new EventHandler<RoutedEventArgs>(this.ImageSource_ImageOpened);
            flag = false;
          }
          this.PreviewImage.Source = (System.Windows.Media.ImageSource) picState.Image;
          this.PreviewImage.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewImage_Tap);
        }
        this.SetImageSourceAttributes();
        if (!flag)
          return;
        ProfilePictureChooserPage.DismissEntranceAnimation();
      }));
    }

    private void ImageSource_ImageOpened(object sender, RoutedEventArgs e)
    {
      ProfilePictureChooserPage.DismissEntranceAnimation();
    }

    private void SetImageSourceAttributes()
    {
      this.PreviewImage.Visibility = this.PreviewImageShadow.Visibility = Visibility.Visible;
      if (ImageStore.IsDarkTheme())
        return;
      this.SubjectTitle.Foreground = (Brush) new SolidColorBrush(Colors.White);
    }

    public void OnError(Exception ex)
    {
      if (ex is ExceptionWithUserMessage)
      {
        int num1 = (int) MessageBox.Show(ex.Message);
      }
      else if (this.Jid == Settings.MyJid)
      {
        int num2 = (int) MessageBox.Show(AppResources.SetProfileFailure);
      }
      else if (JidHelper.IsGroupJid(this.Jid))
      {
        int num3 = (int) MessageBox.Show(AppResources.SetGroupIconFailure);
      }
      Log.LogException(ex, "set pic page");
    }

    public void OnImageSelected(ExternalImagePicker.ExternalImagePickerArgs args)
    {
      switch (args.ActionSource)
      {
        case ExternalImagePicker.ExternalImagePickerActions.TakePhoto:
        case ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums:
        case ExternalImagePicker.ExternalImagePickerActions.SearchWeb:
          ((Action) (() => this.MakeCroppable(args).Subscribe<ExternalImagePicker.ExternalImagePickerArgs>((Action<ExternalImagePicker.ExternalImagePickerArgs>) (args2 => this.SaveResults(args2)), new Action<Exception>(this.OnError))))();
          break;
        case ExternalImagePicker.ExternalImagePickerActions.Delete:
          this.SaveResults(args);
          if (ProfilePictureChooserPage._animationPopup != null)
            ((ProfilePictureChooserPage._animationPopup.Child as Canvas).Children[1] as Image).Source = (System.Windows.Media.ImageSource) AssetStore.DefaultGroupIcon;
          NavUtils.GoBack();
          break;
      }
    }

    private void SaveResults(ExternalImagePicker.ExternalImagePickerArgs args)
    {
      byte[] fullSize = args.Bytes;
      byte[] thumbnail = (byte[]) null;
      ProfilePictureChooserPage.ProfilePictureChooserArgs pictureChooserArgs = new ProfilePictureChooserPage.ProfilePictureChooserArgs();
      if (args.Bitmap != null)
      {
        thumbnail = args.Bitmap.ToJpegByteArray(96, 96, -1, new int?(Settings.JpegQuality));
        fullSize = args.Bitmap.ToJpegByteArray(640, 640, -1, new int?(Settings.JpegQuality));
      }
      if (this.Jid == null)
      {
        pictureChooserArgs.GroupThumbSource = (System.Windows.Media.ImageSource) args.Bitmap;
        pictureChooserArgs.GroupImage = fullSize;
        pictureChooserArgs.GroupThumb = thumbnail;
        if (args.OnComplete != null)
          args.OnComplete();
      }
      else
      {
        if (this.Jid.IsUserJid() && fullSize != null)
          FieldStats.ReportProfilePictureUpload((double) fullSize.Length);
        SetChatPhoto.Set(this.Jid, thumbnail, fullSize);
        if (args.OnComplete != null)
          args.OnComplete();
      }
      ProfilePictureChooserPage._observer.OnNext(pictureChooserArgs);
    }

    private IObservable<ExternalImagePicker.ExternalImagePickerArgs> MakeCroppable(
      ExternalImagePicker.ExternalImagePickerArgs args)
    {
      return Observable.CreateWithDisposable<ExternalImagePicker.ExternalImagePickerArgs>((Func<IObserver<ExternalImagePicker.ExternalImagePickerArgs>, IDisposable>) (observer =>
      {
        if (args.Bitmap.PixelHeight < 192 || args.Bitmap.PixelWidth < 192)
        {
          observer.OnError((Exception) new ExceptionWithUserMessage(Plurals.Instance.GetString(AppResources.SetProfileFailureReasonTooSmallPlural, 192)));
          return Disposable.Create((Action) (() => { }));
        }
        Action action = (Action) (() => this.LayoutRoot.Visibility = Visibility.Collapsed);
        Action show = (Action) (() => this.LayoutRoot.Visibility = Visibility.Visible);
        action();
        Action<Exception> onError = (Action<Exception>) (ex =>
        {
          observer.OnError(ex);
          if (args.OnComplete != null)
            args.OnComplete();
          show();
        });
        Action onCompleted = show;
        bool flag = (args.ActionSource & ExternalImagePicker.ExternalImagePickerActions.SearchWeb) != 0;
        return ImageEditPage.Start(new ImageEditPage.ImageEditPageConfigs((BitmapSource) args.Bitmap)
        {
          CropMode = ImageEditControl.CroppingMode.Fixed,
          InitialCropRatio = 1.0,
          MinRelativeCropSize = new Size?(new Size(192.0 / (double) args.Bitmap.PixelWidth, 192.0 / (double) args.Bitmap.PixelHeight)),
          ImageFrom = flag ? ImageEditPage.ImageEditPageConfigs.ImageSource.Bing : ImageEditPage.ImageEditPageConfigs.ImageSource.Unknown,
          ImagePath = flag ? args.FullPath : (string) null
        }).Subscribe<ImageEditPage.ImageEditPageResults>((Action<ImageEditPage.ImageEditPageResults>) (args2 =>
        {
          show();
          if (args.OnComplete != null)
            args.OnComplete();
          if (args.ActionSource == ExternalImagePicker.ExternalImagePickerActions.TakePhoto)
            MediaDownload.SaveMedia(MediaUpload.CopyLocal(args.TempImageStream, MediaUpload.GenerateMediaFilename("jpg")), FunXMPP.FMessage.Type.Image, saveAlbum: "Camera Roll");
          WriteableBitmap writeableBitmap = SetChatPhoto.ProcessChatPictureCropResult(args.Bitmap, args2.RelativeCropSize, args2.RelativeCropPos);
          observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
          {
            Bitmap = writeableBitmap,
            OnComplete = (Action) (() => args2.NavService.JumpBack(1))
          });
        }), onError, onCompleted);
      }));
    }

    private void PreviewImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.Jid == null)
        ViewPicturePage._GroupThumbSource = this.PreviewImage.Source;
      WaUriParams uriParams = (WaUriParams) null;
      if (this.Jid != null)
      {
        uriParams = new WaUriParams();
        uriParams.AddString("jid", this.Jid);
      }
      NavUtils.NavigateToPage("ViewPicturePage", uriParams);
    }

    public static IObservable<ProfilePictureChooserPage.ProfilePictureChooserArgs> Start(
      string jid,
      string groupSubject,
      System.Windows.Media.ImageSource preview)
    {
      return Observable.Create<ProfilePictureChooserPage.ProfilePictureChooserArgs>((Func<IObserver<ProfilePictureChooserPage.ProfilePictureChooserArgs>, Action>) (observer =>
      {
        ProfilePictureChooserPage._observer = observer;
        ProfilePictureChooserPage._preview = preview;
        WaUriParams uriParams = new WaUriParams();
        if (jid != null)
          uriParams.AddString(nameof (jid), jid);
        if (groupSubject != null)
          uriParams.AddString(nameof (groupSubject), groupSubject);
        NavUtils.NavigateToPage(nameof (ProfilePictureChooserPage), uriParams);
        return (Action) (() => { });
      }));
    }

    public static void PlayEntranceAnimation(
      Image sourceImage,
      System.Windows.Point initialPosition,
      System.Windows.Point returnPosition,
      PageOrientation orientation,
      bool hasZoomFactor)
    {
      double val1 = orientation.IsLandscape() ? Application.Current.Host.Content.ActualHeight : Application.Current.Host.Content.ActualWidth;
      double val2 = orientation.IsLandscape() ? Application.Current.Host.Content.ActualWidth : Application.Current.Host.Content.ActualHeight;
      Canvas canvas = new Canvas();
      switch (orientation)
      {
        case PageOrientation.LandscapeLeft:
          canvas.RenderTransform = (Transform) new CompositeTransform()
          {
            Rotation = 90.0,
            TranslateX = 480.0
          };
          break;
        case PageOrientation.LandscapeRight:
          canvas.RenderTransform = (Transform) new CompositeTransform()
          {
            Rotation = -90.0,
            TranslateY = val1
          };
          break;
      }
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Width = val1;
      rectangle1.Height = val2;
      rectangle1.Fill = Application.Current.Resources[(object) "PhoneBackgroundBrush"] as Brush;
      rectangle1.CacheMode = (CacheMode) new BitmapCache();
      Rectangle rectangle2 = rectangle1;
      canvas.Children.Add((UIElement) rectangle2);
      Image image = new Image();
      image.Source = sourceImage.Source;
      image.Width = Math.Min(val1, val2);
      image.Height = Math.Min(val1, val2);
      image.CacheMode = (CacheMode) new BitmapCache();
      image.RenderTransform = (Transform) new CompositeTransform();
      Image target = image;
      canvas.Children.Add((UIElement) target);
      Popup popup = new Popup();
      popup.Child = (UIElement) canvas;
      popup.IsOpen = true;
      System.Windows.Point point = new System.Windows.Point(sourceImage.Width / target.Width, sourceImage.Height / target.Height);
      if (hasZoomFactor)
      {
        point.X /= ResolutionHelper.ZoomFactor;
        point.Y /= ResolutionHelper.ZoomFactor;
      }
      target.Tag = (object) new Rect(returnPosition, new Size(point.X, point.Y));
      ProfilePictureChooserPage._animationPopup = popup;
      ProfilePictureChooserPage._animationEnded = false;
      ProfilePictureChooserPage._dismissRequested = false;
      Action onComplete = (Action) (() =>
      {
        ProfilePictureChooserPage._animationEnded = true;
        if (!ProfilePictureChooserPage._dismissRequested || ProfilePictureChooserPage._animationPopup == null)
          return;
        ProfilePictureChooserPage._animationPopup.IsOpen = false;
      });
      WaAnimations.AnimateTo((FrameworkElement) target, new double?(initialPosition.X), new double?(0.0), new double?(initialPosition.Y), new double?(0.0), new double?(point.X), new double?(1.0), new double?(point.Y), new double?(1.0), onComplete);
    }

    public static void DismissEntranceAnimation()
    {
      ProfilePictureChooserPage._dismissRequested = true;
      if (!ProfilePictureChooserPage._animationEnded || ProfilePictureChooserPage._animationPopup == null)
        return;
      ProfilePictureChooserPage._animationPopup.IsOpen = false;
    }

    public static void PlayExitAnimation(System.Windows.Media.ImageSource sourceImage)
    {
      if (ProfilePictureChooserPage._animationPopup == null)
        return;
      double actualWidth = Application.Current.Host.Content.ActualWidth;
      double actualHeight = Application.Current.Host.Content.ActualHeight;
      ProfilePictureChooserPage._animationPopup.IsOpen = true;
      Canvas child1 = ProfilePictureChooserPage._animationPopup.Child as Canvas;
      Rectangle child2 = child1.Children[0] as Rectangle;
      Image child3 = child1.Children[1] as Image;
      child2.Opacity = 1.0;
      Rect tag = (Rect) child3.Tag;
      Action onComplete = (Action) (() => ProfilePictureChooserPage.ClearPopup());
      WaAnimations.AnimateTo((FrameworkElement) child3, new double?(0.0), new double?(tag.X), new double?(0.0), new double?(tag.Y), new double?(1.0), new double?(tag.Width), new double?(1.0), new double?(tag.Height), onComplete);
    }

    public static void ClearPopup()
    {
      if (ProfilePictureChooserPage._animationPopup == null)
        return;
      ProfilePictureChooserPage._animationPopup.IsOpen = false;
      ProfilePictureChooserPage._animationPopup = (Popup) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ProfilePictureChooserPage.xaml", UriKind.Relative));
      this.LoadStoryboard = (Storyboard) this.FindName("LoadStoryboard");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PreviewImage = (Image) this.FindName("PreviewImage");
      this.PreviewImageShadow = (Rectangle) this.FindName("PreviewImageShadow");
      this.SubjectTitle = (TextBlock) this.FindName("SubjectTitle");
      this.ImagePicker = (ExternalImagePicker) this.FindName("ImagePicker");
    }

    public class ProfilePictureChooserArgs
    {
      public byte[] GroupImage;
      public byte[] GroupThumb;
      public System.Windows.Media.ImageSource GroupThumbSource;
    }
  }
}
