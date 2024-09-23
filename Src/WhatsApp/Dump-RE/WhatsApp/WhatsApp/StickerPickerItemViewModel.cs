// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerPickerItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class StickerPickerItemViewModel : WaViewModelBase
  {
    private bool isSelected;

    public Sticker Sticker { get; set; }

    public System.Windows.Media.ImageSource Thumbnail { get; private set; }

    public Action HoldAction { get; set; }

    public Action TappedAction { get; set; }

    public Brush StickerItemBackground
    {
      get => !this.IsSelected ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.AccentBrush;
    }

    public bool IsSelected
    {
      get => this.isSelected;
      set
      {
        if (this.isSelected == value)
          return;
        this.isSelected = value;
        this.NotifyPropertyChanged("StickerItemBackground");
      }
    }

    public StickerPickerItemViewModel(Sticker sticker, System.Windows.Media.ImageSource thumb)
    {
      this.Sticker = sticker;
      this.Thumbnail = thumb;
    }

    public static IObservable<StickerPickerItemViewModel> GetStickerItemObservableImpl(
      Sticker sticker)
    {
      return Observable.Create<StickerPickerItemViewModel>((Func<IObserver<StickerPickerItemViewModel>, Action>) (observer =>
      {
        bool disposed = false;
        MemoryStream thumbStream = sticker.GetThumbnailStream();
        if (thumbStream == null)
        {
          observer.OnNext(new StickerPickerItemViewModel(sticker, (System.Windows.Media.ImageSource) null));
          observer.OnCompleted();
        }
        else
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            DateTime? start = PerformanceTimer.Start();
            if (!disposed)
            {
              StickerPickerItemViewModel pickerItemViewModel;
              using (thumbStream)
              {
                WebpUtils.WebpImage webpImage = WebpUtils.DecodeWebp((Stream) thumbStream, Settings.StickerAnimationEnabled);
                pickerItemViewModel = (int) webpImage.FrameCount <= 0 ? new StickerPickerItemViewModel(sticker, (System.Windows.Media.ImageSource) null) : new StickerPickerItemViewModel(sticker, webpImage.Frames[0].Image);
              }
              thumbStream = (MemoryStream) null;
              observer.OnNext(pickerItemViewModel);
              observer.OnCompleted();
            }
            PerformanceTimer.End("decoding webp image", start);
          }));
        return (Action) (() =>
        {
          disposed = true;
          thumbStream.SafeDispose();
          thumbStream = (MemoryStream) null;
        });
      }));
    }
  }
}
