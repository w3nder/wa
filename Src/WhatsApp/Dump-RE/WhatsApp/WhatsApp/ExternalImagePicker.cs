// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExternalImagePicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class ExternalImagePicker : UserControl
  {
    private const int thumbnailSize = 96;
    private string _initialSearchTerm;
    private string _aspect;
    private int _minimumSize;
    private string _deleteConfirmation;
    private string _owningPage;
    private bool _tapped;
    private IObserver<ExternalImagePicker.ExternalImagePickerArgs> _observer;
    private DispatcherTimer _timer = new DispatcherTimer();
    private PageOrientation _orientation;
    internal Viewbox ZoomZoom;
    internal Button WallpaperGalleryButton;
    internal RoundButton WG_ButtonIcon;
    internal TextBlock WG_ButtonText;
    internal Button TakePhotoButton;
    internal RoundButton TP_ButtonIcon;
    internal TextBlock TP_ButtonText;
    internal Button TakeGroupPhotoButton;
    internal RoundButton TGP_ButtonIcon;
    internal TextBlock TGP_ButtonText;
    internal Button ChooseFromAlbumsButton;
    internal RoundButton CFA_ButtonIcon;
    internal TextBlock CFA_ButtonText;
    internal Button SearchWebButton;
    internal RoundButton SW_ButtonIcon;
    internal TextBlock SW_ButtonText;
    internal Button DeleteButton;
    internal RoundButton D_ButtonIcon;
    internal TextBlock D_ButtonText;
    private bool _contentLoaded;

    public ExternalImagePicker()
    {
      this.InitializeComponent();
      this.WG_ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-gallery-hr.png");
      this.WG_ButtonText.Text = AppResources.WallpaperGallery;
      this.TP_ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/camera-icon.png");
      this.TP_ButtonText.Text = AppResources.SetPictureFromCamera;
      this.CFA_ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/gallery-icon.png");
      this.CFA_ButtonText.Text = AppResources.SetPictureFromChooser;
      this.SW_ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/icon-search-web.png");
      this.SW_ButtonText.Text = AppResources.SearchWeb;
      this.D_ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/assets/dark/delete.png");
      this.D_ButtonText.Text = AppResources.Remove;
      this.TGP_ButtonIcon.ButtonIcon = this.TP_ButtonIcon.ButtonIcon;
      this.TGP_ButtonText.Text = this.TP_ButtonText.Text;
      this._timer.Interval = TimeSpan.FromMilliseconds(500.0);
      this._timer.Tick += new EventHandler(this._timer_Tick);
    }

    private void _timer_Tick(object sender, EventArgs e)
    {
      this._tapped = false;
      this._timer.Stop();
    }

    public PageOrientation Orientation
    {
      get => this._orientation;
      set
      {
        this._orientation = value;
        if (this._orientation.IsLandscape())
          this.ZoomZoom.Stretch = Stretch.Uniform;
        else
          this.ZoomZoom.Stretch = Stretch.None;
      }
    }

    public ExternalImagePicker.ExternalImagePickerActions EnabledActions
    {
      set
      {
        if ((value & ExternalImagePicker.ExternalImagePickerActions.WallpaperGallery) != (ExternalImagePicker.ExternalImagePickerActions) 0)
          this.WallpaperGalleryButton.Visibility = Visibility.Visible;
        else
          this.WallpaperGalleryButton.Visibility = Visibility.Collapsed;
        if ((value & ExternalImagePicker.ExternalImagePickerActions.TakePhoto) != (ExternalImagePicker.ExternalImagePickerActions) 0)
        {
          this.TakePhotoButton.Visibility = Visibility.Visible;
          this.TakeGroupPhotoButton.Visibility = Visibility.Collapsed;
        }
        else
          this.TakePhotoButton.Visibility = Visibility.Collapsed;
        if ((value & ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums) != (ExternalImagePicker.ExternalImagePickerActions) 0)
          this.ChooseFromAlbumsButton.Visibility = Visibility.Visible;
        else
          this.TakePhotoButton.Visibility = Visibility.Collapsed;
        if ((value & ExternalImagePicker.ExternalImagePickerActions.SearchWeb) != (ExternalImagePicker.ExternalImagePickerActions) 0)
          this.SearchWebButton.Visibility = Visibility.Visible;
        else
          this.SearchWebButton.Visibility = Visibility.Collapsed;
        if ((value & ExternalImagePicker.ExternalImagePickerActions.Delete) != (ExternalImagePicker.ExternalImagePickerActions) 0)
          this.DeleteButton.Visibility = Visibility.Visible;
        else
          this.DeleteButton.Visibility = Visibility.Collapsed;
        if ((value & ExternalImagePicker.ExternalImagePickerActions.TakeGroupPhoto) != (ExternalImagePicker.ExternalImagePickerActions) 0)
        {
          this.TakeGroupPhotoButton.Visibility = Visibility.Visible;
          this.TakePhotoButton.Visibility = Visibility.Collapsed;
        }
        else
          this.TakeGroupPhotoButton.Visibility = Visibility.Collapsed;
      }
    }

    public SolidColorBrush Foreground
    {
      set
      {
        this.WG_ButtonIcon.ButtonBrush = value;
        this.WG_ButtonText.Foreground = (Brush) value;
        this.TP_ButtonIcon.ButtonBrush = value;
        this.TP_ButtonText.Foreground = (Brush) value;
        this.CFA_ButtonIcon.ButtonBrush = value;
        this.CFA_ButtonText.Foreground = (Brush) value;
        this.SW_ButtonIcon.ButtonBrush = value;
        this.SW_ButtonText.Foreground = (Brush) value;
        this.D_ButtonIcon.ButtonBrush = value;
        this.D_ButtonText.Foreground = (Brush) value;
        this.TGP_ButtonIcon.ButtonBrush = value;
        this.TGP_ButtonText.Foreground = (Brush) value;
      }
    }

    private void WallpaperGallery_Tap(object sender, EventArgs e)
    {
      if (this._tapped)
        return;
      this._tapped = true;
      this._timer.Start();
      WallpaperGalleryPickerPage.Start().Subscribe<string>((Action<string>) (args => this._observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
      {
        FullPath = args,
        OnComplete = (Action) (() => NavUtils.GoBack()),
        ActionSource = ExternalImagePicker.ExternalImagePickerActions.WallpaperGallery
      })));
    }

    private void TakePhoto_Tap(object sender, EventArgs e) => this.TakePhoto(sender, e, false);

    private void TakeGroupPhoto_Tap(object sender, EventArgs e) => this.TakePhoto(sender, e, true);

    private void TakePhoto(object sender, EventArgs e, bool isGroupPhoto)
    {
      if (this._tapped)
        return;
      this._tapped = true;
      this._timer.Start();
      CameraPage.TakePictureOnly = true;
      TakePicture.Launch(TakePicture.Mode.Regular, false, Settings.ImageMaxEdge, Settings.ImageMaxEdge, true, isGroupPhoto).Subscribe<TakePicture.CapturedPictureArgs>((Action<TakePicture.CapturedPictureArgs>) (args =>
      {
        NavUtils.GoBack();
        this._observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
        {
          Bitmap = args.Bitmap,
          TempImageStream = (Stream) args.TempFileStream,
          ActionSource = ExternalImagePicker.ExternalImagePickerActions.TakePhoto
        });
      }), (Action<Exception>) (err => this._observer.OnError(err)));
    }

    private void ChooseFromAlbums_Tap(object sender, EventArgs e)
    {
      if (this._tapped)
        return;
      this._tapped = true;
      this._timer.Start();
      MediaPickerPage.StartPhotoPicker(false).Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
      {
        string str = (string) null;
        WriteableBitmap writeableBitmap = (WriteableBitmap) null;
        if (args.SharingState.SelectedItems.FirstOrDefault<MediaSharingState.IItem>() is MediaPickerState.Item obj2)
        {
          writeableBitmap = obj2.GetBitmap(new Size((double) Settings.ImageMaxEdge, (double) Settings.ImageMaxEdge), true, true, false);
          str = obj2.GetFullPath();
        }
        args.NavService.JumpBackTo(this._owningPage, fallbackToHome: true);
        if (writeableBitmap == null)
          return;
        this._observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
        {
          FullPath = str,
          Bitmap = writeableBitmap,
          ActionSource = ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums
        });
      }));
    }

    private void SearchWeb_Click(object sender, EventArgs e)
    {
      if (this._tapped)
        return;
      this._tapped = true;
      this._timer.Start();
      WebPhotoPickerPage.Start(((Page) App.CurrentApp.RootFrame.Content).NavigationService, this._initialSearchTerm, "Large", this._aspect, this._minimumSize).Subscribe<WebPhotoPickerPage.WebPhotoPickerArgs>((Action<WebPhotoPickerPage.WebPhotoPickerArgs>) (args => this._observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
      {
        Bitmap = args.Bitmap,
        OnComplete = args.OnComplete,
        ActionSource = ExternalImagePicker.ExternalImagePickerActions.SearchWeb,
        FullPath = args.SourcePageUrl
      })), (Action<Exception>) (ex => this._observer.OnError(ex)));
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      if (this._tapped)
        return;
      this._tapped = true;
      this._timer.Start();
      string remove = AppResources.Remove;
      Observable.Return<bool>(true).Decision(this._deleteConfirmation, remove, AppResources.Keep).Subscribe<bool>((Action<bool>) (accept =>
      {
        this._tapped = false;
        if (!accept)
          return;
        this._observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
        {
          Bitmap = (WriteableBitmap) null,
          ActionSource = ExternalImagePicker.ExternalImagePickerActions.Delete
        });
      }));
    }

    public IObservable<ExternalImagePicker.ExternalImagePickerArgs> Start(
      string initialSearchTerm,
      string aspectRatio,
      int minimumSize,
      string deleteConfirmation,
      string owningPage)
    {
      return Observable.Create<ExternalImagePicker.ExternalImagePickerArgs>((Func<IObserver<ExternalImagePicker.ExternalImagePickerArgs>, Action>) (observer =>
      {
        this._initialSearchTerm = initialSearchTerm;
        this._aspect = aspectRatio;
        this._minimumSize = minimumSize;
        this._deleteConfirmation = deleteConfirmation;
        this._owningPage = owningPage;
        this._observer = observer;
        return (Action) (() => { });
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ExternalImagePicker.xaml", UriKind.Relative));
      this.ZoomZoom = (Viewbox) this.FindName("ZoomZoom");
      this.WallpaperGalleryButton = (Button) this.FindName("WallpaperGalleryButton");
      this.WG_ButtonIcon = (RoundButton) this.FindName("WG_ButtonIcon");
      this.WG_ButtonText = (TextBlock) this.FindName("WG_ButtonText");
      this.TakePhotoButton = (Button) this.FindName("TakePhotoButton");
      this.TP_ButtonIcon = (RoundButton) this.FindName("TP_ButtonIcon");
      this.TP_ButtonText = (TextBlock) this.FindName("TP_ButtonText");
      this.TakeGroupPhotoButton = (Button) this.FindName("TakeGroupPhotoButton");
      this.TGP_ButtonIcon = (RoundButton) this.FindName("TGP_ButtonIcon");
      this.TGP_ButtonText = (TextBlock) this.FindName("TGP_ButtonText");
      this.ChooseFromAlbumsButton = (Button) this.FindName("ChooseFromAlbumsButton");
      this.CFA_ButtonIcon = (RoundButton) this.FindName("CFA_ButtonIcon");
      this.CFA_ButtonText = (TextBlock) this.FindName("CFA_ButtonText");
      this.SearchWebButton = (Button) this.FindName("SearchWebButton");
      this.SW_ButtonIcon = (RoundButton) this.FindName("SW_ButtonIcon");
      this.SW_ButtonText = (TextBlock) this.FindName("SW_ButtonText");
      this.DeleteButton = (Button) this.FindName("DeleteButton");
      this.D_ButtonIcon = (RoundButton) this.FindName("D_ButtonIcon");
      this.D_ButtonText = (TextBlock) this.FindName("D_ButtonText");
    }

    public class ExternalImagePickerArgs
    {
      public Action OnComplete;
      public Stream TempImageStream;
      public WriteableBitmap Bitmap;
      public byte[] Bytes;
      public string FullPath;
      public ExternalImagePicker.ExternalImagePickerActions ActionSource;
    }

    [Flags]
    public enum ExternalImagePickerActions
    {
      WallpaperGallery = 1,
      TakePhoto = 2,
      ChooseFromAlbums = 4,
      SearchWeb = 8,
      Delete = 16, // 0x00000010
      TakeGroupPhoto = 32, // 0x00000020
    }
  }
}
