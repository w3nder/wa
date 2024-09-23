// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class LocationMessageViewModel : MessageViewModel
  {
    public const int LocationThumbnailOverlayWidth = 239;
    public const int LocationThumbnailOverlayHeight = 111;
    private bool wasPending;
    private Message.PlaceDetails placeDetails;
    private bool largeThumbFetchAttempted;
    protected IDisposable largeThumbSub;
    private static BitmapImage mapLoadingImage;

    protected override bool ShouldShowFooterInAccentColor => this.Message.IsCoordinateLocation();

    public override Thickness FooterMargin
    {
      get => new Thickness(0.0, 0.0, 15.0 * this.zoomMultiplier, 15.0 * this.zoomMultiplier);
    }

    public override bool ShouldUseFooterProtection => this.Message.IsCoordinateLocation();

    public Message.PlaceDetails PlaceDetails
    {
      get => this.placeDetails ?? (this.placeDetails = this.Message.ParsePlaceDetails());
    }

    public string PlaceTitleStr
    {
      get
      {
        if (this.Message.IsCoordinateLocation())
          return (string) null;
        return this.PlaceDetails?.Name;
      }
    }

    public string PlaceAddressStr
    {
      get
      {
        return !this.Message.IsCoordinateLocation() ? string.Format("{0}{1}", (object) this.PlaceDetails?.Address, (object) this.FooterSpaceHolder) : (string) null;
      }
    }

    public VerticalAlignment PlaceDetailsVerticalAlignment
    {
      get
      {
        return !string.IsNullOrEmpty(this.PlaceDetails?.Address) ? VerticalAlignment.Top : VerticalAlignment.Center;
      }
    }

    public LocationMessageViewModel(Message m)
      : base(m)
    {
      this.wasPending = m.Status == FunXMPP.FMessage.Status.Pending;
    }

    public override void Cleanup()
    {
      this.largeThumbSub.SafeDispose();
      this.largeThumbSub = (IDisposable) null;
      base.Cleanup();
    }

    public void ClearPlaceDetails() => this.placeDetails = (Message.PlaceDetails) null;

    protected override IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservableImpl(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      Message msg = this.Message;
      IDisposable largeThumbGenDisp = (IDisposable) null;
      return Observable.Create<MessageViewModel.ThumbnailState>((Func<IObserver<MessageViewModel.ThumbnailState>, Action>) (observer =>
      {
        bool isLargeSize = false;
        MemoryStream thumbStream = msg.GetThumbnailStream(false, out isLargeSize);
        if (thumbStream == null || !isLargeSize)
        {
          if (LocationMessageViewModel.mapLoadingImage == null)
            LocationMessageViewModel.mapLoadingImage = ImageStore.GetStockIcon("/Images/map_loading.png");
          MessageViewModel.InvokeAsync((Action) (() => observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) LocationMessageViewModel.mapLoadingImage, msg.KeyId, false))));
          this.GenerateLargeThumbnailAsync();
        }
        else
          MessageViewModel.InvokeAsync((Action) (() =>
          {
            BitmapSource thumb = (BitmapSource) null;
            using (thumbStream)
            {
              BitmapImage bi = new BitmapImage()
              {
                CreateOptions = BitmapCreateOptions.BackgroundCreation
              };
              bi.SetSource((Stream) thumbStream);
              thumb = (BitmapSource) bi;
              if (!this.Message.IsCoordinateLocation())
                bi.ImageOpened += (EventHandler<RoutedEventArgs>) ((sender, e) =>
                {
                  if ((double) bi.PixelWidth >= this.ThumbnailWidth * 0.8)
                    return;
                  this.GenerateLargeThumbnailAsync();
                });
            }
            this.CacheThumbnail((System.Windows.Media.ImageSource) thumb, true);
            observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) thumb, msg.KeyId, true));
          }));
        return (Action) (() => largeThumbGenDisp.SafeDispose());
      }));
    }

    public override void GenerateLargeThumbnailAsync()
    {
      if (this.Message.Status == FunXMPP.FMessage.Status.Pending || this.largeThumbFetchAttempted)
        return;
      this.largeThumbFetchAttempted = true;
      int num1 = (int) this.ThumbnailWidth * 2;
      int num2 = (int) this.ThumbnailHeight * 2;
      this.largeThumbSub = WebServices.Instance.GetMapThumbnail(this.Message.Latitude, this.Message.Longitude, num1, num2, num1, num2).Take<WriteableBitmap>(1).ObserveOnDispatcher<WriteableBitmap>().SubscribeOn<WriteableBitmap>(WAThreadPool.Scheduler).Subscribe<WriteableBitmap>((Action<WriteableBitmap>) (thumb => this.OnLargeThumbnailCreated(thumb)), (Action<Exception>) (onError => this.DisplayCroppedMessageThumbnail()), (Action) (() =>
      {
        this.largeThumbSub.SafeDispose();
        this.largeThumbSub = (IDisposable) null;
      }));
    }

    public void DisplayCroppedMessageThumbnail()
    {
      Message message = this.Message;
      bool isLargeSize = false;
      MemoryStream thumbStream = message.GetThumbnailStream(false, out isLargeSize);
      if (thumbStream != null)
        MessageViewModel.InvokeAsync((Action) (() =>
        {
          using (thumbStream)
          {
            BitmapImage source = new BitmapImage();
            source.SetSource((Stream) thumbStream);
            WriteableBitmap bitmap = new WriteableBitmap((BitmapSource) source);
            Size cropSize = new Size((double) Math.Min(340, source.PixelWidth), (double) Math.Min(160, source.PixelHeight));
            System.Windows.Point cropPos = new System.Windows.Point(((double) source.PixelWidth - cropSize.Width) / 2.0, ((double) source.PixelHeight - cropSize.Height) / 2.0);
            this.CacheThumbnail((System.Windows.Media.ImageSource) bitmap.Crop(cropPos, cropSize), false);
            this.NotifyThumbnailChanged("displaying cropped message thumb");
          }
        }));
      else
        Log.l(this.LogHeader, "DisplayCroppedMessageThumbnail has no thumbnail");
    }

    protected override Size GetTargetThumbnailSizeImpl()
    {
      double defaultContentWidth = MessageViewModel.DefaultContentWidth;
      return new Size(defaultContentWidth, defaultContentWidth * 320.0 / 680.0);
    }

    protected override void OnMessageStatusChanged()
    {
      base.OnMessageStatusChanged();
      if (!this.Message.KeyFromMe)
        return;
      if (this.wasPending)
      {
        this.NotifyThumbnailChanged("loc msg no longer pending");
        if (!this.Message.IsCoordinateLocation())
        {
          this.ClearPlaceDetails();
          this.Notify("PlaceDetailsChanged");
        }
      }
      this.wasPending = this.Message.Status == FunXMPP.FMessage.Status.Pending;
    }
  }
}
