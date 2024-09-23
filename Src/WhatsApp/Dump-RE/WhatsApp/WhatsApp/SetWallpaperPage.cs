// Decompiled with JetBrains decompiler
// Type: WhatsApp.SetWallpaperPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class SetWallpaperPage : PhoneApplicationPage
  {
    private static string NextInstanceJid;
    private string jid;
    private IDisposable pickerSubscription;
    private bool firstNav = true;
    internal Grid LayoutRoot;
    internal WallpaperPanel WallpaperPanel;
    internal Rectangle WallpaperOverlay;
    internal ZoomBox ZoomBox;
    internal PageTitlePanel PageTitle;
    internal TextBlock TooltipBlock;
    internal ExternalImagePicker ImagePicker;
    private bool _contentLoaded;

    private bool IsGlobalWallpaper => this.jid == "";

    public SetWallpaperPage()
    {
      this.InitializeComponent();
      this.ZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.ZoomBox.Margin = new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
      this.jid = SetWallpaperPage.NextInstanceJid;
      SetWallpaperPage.NextInstanceJid = (string) null;
      Log.l("set wallpaper", "set wallpaper for: {0}", this.IsGlobalWallpaper ? (object) "default" : (object) this.jid);
      this.Init();
    }

    public static void Start(string jid = null)
    {
      SetWallpaperPage.NextInstanceJid = jid ?? "";
      NavUtils.NavigateToPage(nameof (SetWallpaperPage));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.jid == null)
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      else if (this.firstNav)
        this.firstNav = false;
      else
        this.UpdatePage();
      base.OnNavigatedTo(e);
    }

    private void Init()
    {
      if (this.IsGlobalWallpaper)
      {
        this.TooltipBlock.Text = AppResources.GlobalWallpaperSetExplain;
        this.PageTitle.SmallTitle = AppResources.SetGlobalWallpaper;
      }
      else
        this.TooltipBlock.Text = AppResources.ChatWallpaperSetExplain;
      if (this.pickerSubscription == null)
        this.pickerSubscription = this.ImagePicker.Start((string) null, "Tall", 480, !this.IsGlobalWallpaper ? (WallpaperStore.DefaultWallpaper != null ? AppResources.RemoveChatWallpaperToDefaultConfirm : AppResources.RemoveChatWallpaperConfirm) : AppResources.RemoveDefaultWallpaperConfirm, nameof (SetWallpaperPage)).Subscribe<ExternalImagePicker.ExternalImagePickerArgs>(new Action<ExternalImagePicker.ExternalImagePickerArgs>(this.OnImageSelected), new Action<Exception>(this.OnError));
      this.UpdatePage();
    }

    private void UpdatePage()
    {
      WallpaperStore.WallpaperState wallpaper = (WallpaperStore.WallpaperState) null;
      if (this.IsGlobalWallpaper)
      {
        wallpaper = WallpaperStore.DefaultWallpaper;
      }
      else
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          convo = db.GetConversation(this.jid, CreateOptions.None);
          if (convo == null)
            return;
          wallpaper = WallpaperStore.Get(db, this.jid, true);
        }));
        this.PageTitle.SmallTitle = convo != null ? Emoji.ConvertToUnicode(convo.GetName(true) ?? "") : AppResources.WallpaperSettings;
      }
      if (wallpaper != null && this.WallpaperPanel.Set(wallpaper))
      {
        this.WallpaperOverlay.Visibility = wallpaper.SolidColor.HasValue.ToVisibility();
        this.PageTitle.TextBrush = this.TooltipBlock.Foreground = (Brush) (this.ImagePicker.Foreground = wallpaper.PreferredForegroundBrush);
        SysTrayHelper.SetForegroundColor((DependencyObject) this, wallpaper.GetPreferredForegroundColor(true));
      }
      else
      {
        this.WallpaperPanel.Clear();
        this.WallpaperOverlay.Visibility = Visibility.Collapsed;
        this.PageTitle.TextBrush = this.TooltipBlock.Foreground = (Brush) (this.ImagePicker.Foreground = UIUtils.ForegroundBrush);
        SysTrayHelper.SetForegroundColor((DependencyObject) this, UIUtils.ForegroundBrush.Color);
      }
      ExternalImagePicker.ExternalImagePickerActions imagePickerActions = ExternalImagePicker.ExternalImagePickerActions.WallpaperGallery | ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums | ExternalImagePicker.ExternalImagePickerActions.SearchWeb;
      if (wallpaper != null)
        imagePickerActions |= ExternalImagePicker.ExternalImagePickerActions.Delete;
      this.ImagePicker.EnabledActions = imagePickerActions;
    }

    public void OnError(Exception ex)
    {
      if (ex is ExceptionWithUserMessage)
      {
        int num1 = (int) MessageBox.Show(ex.Message);
      }
      else
      {
        int num2 = (int) MessageBox.Show(AppResources.WallpaperErrorOccured);
      }
      Log.LogException(ex, "set wallpaper page");
    }

    public void OnImageSelected(ExternalImagePicker.ExternalImagePickerArgs args)
    {
      switch (args.ActionSource)
      {
        case ExternalImagePicker.ExternalImagePickerActions.WallpaperGallery:
          if (!this.IsGlobalWallpaper)
            this.NavigationService.JumpBack(1);
          this.SaveResults(args);
          break;
        case ExternalImagePicker.ExternalImagePickerActions.ChooseFromAlbums:
        case ExternalImagePicker.ExternalImagePickerActions.SearchWeb:
          this.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.MakeCroppable(args).Subscribe<ExternalImagePicker.ExternalImagePickerArgs>((Action<ExternalImagePicker.ExternalImagePickerArgs>) (args2 => this.SaveResults(args2)), new Action<Exception>(this.OnError))));
          break;
        case ExternalImagePicker.ExternalImagePickerActions.Delete:
          this.SaveResults(args);
          if (this.IsGlobalWallpaper)
          {
            this.UpdatePage();
            break;
          }
          NavUtils.GoBack();
          break;
      }
    }

    private void SaveResults(ExternalImagePicker.ExternalImagePickerArgs args)
    {
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        if (args.Bitmap != null)
          WallpaperStore.Set(db, this.IsGlobalWallpaper ? (string) null : this.jid, args.Bitmap);
        else if (args.FullPath != null)
          WallpaperStore.Set(db, this.IsGlobalWallpaper ? (string) null : this.jid, args.FullPath);
        else
          WallpaperStore.Delete(db, this.IsGlobalWallpaper ? (string) null : this.jid);
      }));
      if (args.OnComplete == null)
        return;
      args.OnComplete();
    }

    private IObservable<ExternalImagePicker.ExternalImagePickerArgs> MakeCroppable(
      ExternalImagePicker.ExternalImagePickerArgs args)
    {
      return Observable.CreateWithDisposable<ExternalImagePicker.ExternalImagePickerArgs>((Func<IObserver<ExternalImagePicker.ExternalImagePickerArgs>, IDisposable>) (observer =>
      {
        int number = 480;
        if (args.Bitmap.PixelHeight < number || args.Bitmap.PixelWidth < number)
        {
          observer.OnError((Exception) new ExceptionWithUserMessage(Plurals.Instance.GetString(AppResources.SetProfileFailureReasonTooSmallPlural, number)));
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
        return WallpaperPreviewPage.Start(this.NavigationService, args.Bitmap, this.IsGlobalWallpaper).Subscribe<WallpaperPreviewPage.WallpaperPreviewArgs>((Action<WallpaperPreviewPage.WallpaperPreviewArgs>) (args2 =>
        {
          show();
          if (args.OnComplete != null)
            args.OnComplete();
          observer.OnNext(new ExternalImagePicker.ExternalImagePickerArgs()
          {
            Bitmap = args2.Bitmap,
            OnComplete = args2.OnComplete
          });
        }), onError, onCompleted);
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/SetWallpaperPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.WallpaperPanel = (WallpaperPanel) this.FindName("WallpaperPanel");
      this.WallpaperOverlay = (Rectangle) this.FindName("WallpaperOverlay");
      this.ZoomBox = (ZoomBox) this.FindName("ZoomBox");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
      this.ImagePicker = (ExternalImagePicker) this.FindName("ImagePicker");
    }
  }
}
