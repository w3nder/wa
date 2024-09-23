// Decompiled with JetBrains decompiler
// Type: WhatsApp.AppAsyncExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Specialized;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp
{
  public static class AppAsyncExtensions
  {
    public static bool IsLandscape(this PageOrientation orientation)
    {
      return (orientation & PageOrientation.Portrait) == PageOrientation.None;
    }

    public static bool IsPortrait(this PageOrientation orientation)
    {
      return (orientation & PageOrientation.Portrait) != 0;
    }

    public static IObservable<SelectionChangedEventArgs> GetSelectionChangedAsync(this Pivot that)
    {
      return Observable.Create<SelectionChangedEventArgs>((Func<IObserver<SelectionChangedEventArgs>, Action>) (o =>
      {
        SelectionChangedEventHandler eh = (SelectionChangedEventHandler) ((sender, a) => o.OnNext(a));
        that.SelectionChanged += eh;
        return (Action) (() => that.SelectionChanged -= eh);
      }));
    }

    public static IObservable<SelectionChangedEventArgs> GetSelectionChangedAsync(this Panorama that)
    {
      return Observable.FromEvent<SelectionChangedEventArgs>((Action<EventHandler<SelectionChangedEventArgs>>) (eh => that.SelectionChanged += eh), (Action<EventHandler<SelectionChangedEventArgs>>) (eh => that.SelectionChanged -= eh)).Select<IEvent<SelectionChangedEventArgs>, SelectionChangedEventArgs>((Func<IEvent<SelectionChangedEventArgs>, SelectionChangedEventArgs>) (ev => ev.EventArgs));
    }

    public static IObservable<Unit> GetLoadedAsync(this FrameworkElement that)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (o =>
      {
        RoutedEventHandler eh = (RoutedEventHandler) ((sender, a) => o.OnNext(new Unit()));
        that.Loaded += eh;
        return (Action) (() => that.Loaded -= eh);
      }));
    }

    public static IObservable<SelectionChangedEventArgs> GetSelectionChangedAsync(this Selector that)
    {
      return Observable.Create<SelectionChangedEventArgs>((Func<IObserver<SelectionChangedEventArgs>, Action>) (o =>
      {
        SelectionChangedEventHandler eh = (SelectionChangedEventHandler) ((sender, a) => o.OnNext(a));
        that.SelectionChanged += eh;
        return (Action) (() => that.SelectionChanged -= eh);
      }));
    }

    public static IObservable<object> GetSelectionChangedAsync(this LongListSelector that)
    {
      return Observable.Create<object>((Func<IObserver<object>, Action>) (o =>
      {
        SelectionChangedEventHandler eh = (SelectionChangedEventHandler) ((sender, a) => o.OnNext(that.SelectedItem));
        that.SelectionChanged += eh;
        return (Action) (() => that.SelectionChanged -= eh);
      }));
    }

    public static IObservable<TextChangedEventArgs> GetTextChangedAsync(this TextBox that)
    {
      return Observable.Create<TextChangedEventArgs>((Func<IObserver<TextChangedEventArgs>, Action>) (o =>
      {
        TextChangedEventHandler eventHandler = (TextChangedEventHandler) ((sender, a) => o.OnNext(a));
        that.TextChanged += eventHandler;
        return (Action) (() => that.TextChanged -= eventHandler);
      }));
    }

    public static IObservable<bool> GetFocusChangeObservable(this Control that)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (o =>
      {
        RoutedEventHandler gainedHandler = (RoutedEventHandler) ((sender, a) => o.OnNext(true));
        RoutedEventHandler lostHandler = (RoutedEventHandler) ((sender, a) => o.OnNext(false));
        that.GotFocus += gainedHandler;
        that.LostFocus += lostHandler;
        return (Action) (() =>
        {
          that.GotFocus -= gainedHandler;
          that.LostFocus -= lostHandler;
        });
      }));
    }

    public static IObservable<Unit> GetLayoutUpdatedAsync(this FrameworkElement that)
    {
      return that.GetLayoutUpdatedAsync((Action) (() => { }));
    }

    public static IObservable<Unit> GetLayoutUpdatedAsync(
      this FrameworkElement that,
      Action onSubscribe)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (o =>
      {
        EventHandler handler = (EventHandler) ((sender, a) => o.OnNext(new Unit()));
        that.LayoutUpdated += handler;
        onSubscribe();
        return (Action) (() => that.LayoutUpdated -= handler);
      }));
    }

    public static IObservable<NavigatingCancelEventArgs> GetNavigatingAsync(
      this PhoneApplicationFrame that)
    {
      return Observable.Create<NavigatingCancelEventArgs>((Func<IObserver<NavigatingCancelEventArgs>, Action>) (o =>
      {
        NavigatingCancelEventHandler eventHandler = (NavigatingCancelEventHandler) ((sender, a) => o.OnNext(a));
        that.Navigating += eventHandler;
        return (Action) (() => that.Navigating -= eventHandler);
      }));
    }

    public static IObservable<NavigationEventArgs> GetNavigatedAsync(this PhoneApplicationFrame that)
    {
      return Observable.Create<NavigationEventArgs>((Func<IObserver<NavigationEventArgs>, Action>) (o =>
      {
        NavigatedEventHandler eventHandler = (NavigatedEventHandler) ((sender, a) => o.OnNext(a));
        that.Navigated += eventHandler;
        return (Action) (() => that.Navigated -= eventHandler);
      }));
    }

    public static IObservable<IEvent<T>> GetShowTaskAsync<T>(this ChooserBase<T> that) where T : TaskEventArgs
    {
      return Observable.FromEvent<T>((Action<EventHandler<T>>) (eh =>
      {
        that.Completed += eh;
        that.Show();
      }), (Action<EventHandler<T>>) (eh => that.Completed -= eh));
    }

    public static IObservable<IEvent<GeoPositionStatusChangedEventArgs>> GetGeoStatusAsync(
      this GeoCoordinateWatcher that)
    {
      return Observable.FromEvent<GeoPositionStatusChangedEventArgs>((Action<EventHandler<GeoPositionStatusChangedEventArgs>>) (eh => that.StatusChanged += eh), (Action<EventHandler<GeoPositionStatusChangedEventArgs>>) (eh => that.StatusChanged -= eh));
    }

    public static IObservable<IEvent<CaptureImageCompletedEventArgs>> GetImageCompletedAsync(
      this CaptureSource that)
    {
      return Observable.FromEvent<CaptureImageCompletedEventArgs>((Action<EventHandler<CaptureImageCompletedEventArgs>>) (eh => that.CaptureImageCompleted += eh), (Action<EventHandler<CaptureImageCompletedEventArgs>>) (eh => that.CaptureImageCompleted -= eh));
    }

    public static IObservable<NotifyCollectionChangedEventArgs> GetCollectionChangedAsnyc(
      this INotifyCollectionChanged source)
    {
      return Observable.Create<NotifyCollectionChangedEventArgs>((Func<IObserver<NotifyCollectionChangedEventArgs>, Action>) (observer =>
      {
        NotifyCollectionChangedEventHandler handler = (NotifyCollectionChangedEventHandler) ((sender, args) => observer.OnNext(args));
        source.CollectionChanged += handler;
        return (Action) (() => source.CollectionChanged -= handler);
      }));
    }

    public static IObservable<Unit> GetCompletedAsync(this Timeline source)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        EventHandler eh = (EventHandler) ((sender, args) => observer.OnNext(new Unit()));
        source.Completed += eh;
        return (Action) (() => Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => source.Completed -= eh)));
      }));
    }

    public static IObservable<PageOrientation> GetOrientationAsync(this PhoneApplicationPage source)
    {
      return Observable.Return<PageOrientation>(source.Orientation).Concat<PageOrientation>(Observable.FromEvent<OrientationChangedEventArgs>((Action<EventHandler<OrientationChangedEventArgs>>) (h => source.OrientationChanged += h), (Action<EventHandler<OrientationChangedEventArgs>>) (h => source.OrientationChanged -= h)).Select<IEvent<OrientationChangedEventArgs>, PageOrientation>((Func<IEvent<OrientationChangedEventArgs>, PageOrientation>) (args => args.EventArgs.Orientation)));
    }

    public static IObservable<T> RepeatInForeground<T>(this IObservable<T> source)
    {
      bool cancelled = false;
      source = source.RepeatWhile<T>((Func<bool>) (() =>
      {
        bool flag = !cancelled && App.Active;
        Log.l("repeat in fg", "cancelled:{0}, active:{1}, returning {2}", (object) cancelled, (object) App.Active, (object) flag);
        return flag;
      }));
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        IDisposable sourceSub = (IDisposable) null;
        Dispatcher Dispatcher = Deployment.Current.Dispatcher;
        IDisposable reconnectSub = Observable.Return<Unit>(new Unit()).Concat<Unit>(App.ApplicationActivatedSubject.Skip<Unit>(1)).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          Log.l("repeat in fg", "subscribing to source observable");
          Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            sourceSub?.Dispose();
            sourceSub = source.Subscribe(observer);
          }));
        }));
        return (Action) (() =>
        {
          Log.l("repeat in fg", "cancelling...");
          Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            cancelled = true;
            if (reconnectSub != null)
            {
              reconnectSub.Dispose();
              reconnectSub = (IDisposable) null;
            }
            if (sourceSub == null)
              return;
            sourceSub.Dispose();
            sourceSub = (IDisposable) null;
          }));
        });
      }));
    }
  }
}
