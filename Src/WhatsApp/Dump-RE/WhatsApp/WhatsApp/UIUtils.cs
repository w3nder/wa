// Decompiled with JetBrains decompiler
// Type: WhatsApp.UIUtils
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public static class UIUtils
  {
    private static Dictionary<Uri, LinkedList<IObserver<JournalEntryRemovedEventArgs>>> removedFromJournalDict = new Dictionary<Uri, LinkedList<IObserver<JournalEntryRemovedEventArgs>>>();
    private static IDisposable removedFromJournalSub = (IDisposable) null;
    private static double sIPHeightPortrait = 0.0;
    private static double sIPHeightAdjustmentForWP10 = 0.0;
    public const double SystemTraySizePortraitUnzoomed = 32.0;
    public const double SystemTraySizeLandscapeUnzoomed = 72.0;
    public static SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
    public static SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
    public static SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
    public static SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
    public static SolidColorBrush SelectionBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 58, (byte) 117, (byte) 242));
    public static SolidColorBrush VerifiedBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 37, (byte) 211, (byte) 102));
    public static Brush SubtleBrushWhite = (Brush) new SolidColorBrush(Color.FromArgb((byte) 153, byte.MaxValue, byte.MaxValue, byte.MaxValue));
    public static Brush SubtleBrushGray = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 153, (byte) 153, (byte) 153));
    public static Brush SubtleBrush = Application.Current.Resources[(object) "PhoneSubtleBrush"] as Brush;
    public static FontFamily FontFamilySemiLight = Application.Current.Resources[(object) "PhoneFontFamilySemiLight"] as FontFamily;
    public static FontFamily FontFamilyNormal = Application.Current.Resources[(object) "PhoneFontFamilyNormal"] as FontFamily;
    public static Style TextStyleExtraLarge = Application.Current.Resources[(object) "PhoneTextExtraLargeStyle"] as Style;
    public static Style TextStyleNormal = Application.Current.Resources[(object) "PhoneTextNormalStyle"] as Style;
    public static Style TextStyleSubtle = Application.Current.Resources[(object) "PhoneTextSubtleStyle"] as Style;
    public static double FontSizeSmall = (double) Application.Current.Resources[(object) "PhoneFontSizeSmall"];
    public static double FontSizeMediumLarge = (double) Application.Current.Resources[(object) "PhoneFontSizeMediumLarge"];
    public static double FontSizeLarge = (double) Application.Current.Resources[(object) "PhoneFontSizeLarge"];
    private static SolidColorBrush foregroundBrush_ = (SolidColorBrush) null;
    private static string foregroundColorCode = (string) null;
    private static SolidColorBrush backgroundBrush_ = (SolidColorBrush) null;
    private static SolidColorBrush accentBrush_ = (SolidColorBrush) null;
    private static SolidColorBrush translucentAccentBrush_ = (SolidColorBrush) null;
    private static string accentColorCode = (string) null;
    private static SolidColorBrush darkAccentBrush_ = (SolidColorBrush) null;
    private static SolidColorBrush darkAccentMetadataBrush_ = (SolidColorBrush) null;
    private static Brush phoneChromeBrush = (Brush) null;
    private static Brush phoneInactiveBrush = (Brush) null;

    public static IEnumerable<UIElement> GetVisualTreeAncestors(UIElement target)
    {
      node = target;
      while (VisualTreeHelper.GetParent((DependencyObject) node) is UIElement node)
        yield return node;
    }

    public static IEnumerable<DependencyObject> GetVisualTreeChildren(DependencyObject obj)
    {
      int n = 0;
      try
      {
        n = VisualTreeHelper.GetChildrenCount(obj);
      }
      catch (Exception ex)
      {
      }
      for (int i = 0; i < n; ++i)
        yield return VisualTreeHelper.GetChild(obj, i);
    }

    public static IEnumerable<DependencyObject> GetAllVisualTreeChildren(DependencyObject obj)
    {
      yield return obj;
      int n = 0;
      try
      {
        n = VisualTreeHelper.GetChildrenCount(obj);
      }
      catch (Exception ex)
      {
      }
      for (int i = 0; i < n; ++i)
      {
        foreach (DependencyObject allVisualTreeChild in UIUtils.GetAllVisualTreeChildren(VisualTreeHelper.GetChild(obj, i)))
          yield return allVisualTreeChild;
      }
    }

    public static IEnumerable<DependencyObject> FindInVisualTree(
      DependencyObject obj,
      Func<DependencyObject, bool> pred)
    {
      LinkedList<DependencyObject> lookupQueue = new LinkedList<DependencyObject>();
      lookupQueue.AddLast(obj);
      while (lookupQueue.Any<DependencyObject>())
      {
        DependencyObject dependencyObject = lookupQueue.First.Value;
        lookupQueue.RemoveFirst();
        foreach (DependencyObject child in UIUtils.GetVisualTreeChildren(dependencyObject))
        {
          if (pred(child))
            yield return child;
          else
            lookupQueue.AddLast(child);
        }
      }
    }

    public static List<T> FindAllDisplayedInVisualTree<T>(
      DependencyObject obj,
      Func<DependencyObject, bool> select)
      where T : DependencyObject
    {
      List<T> displayedInVisualTree = new List<T>();
      LinkedList<DependencyObject> source = new LinkedList<DependencyObject>();
      source.AddLast(obj);
      while (source.Any<DependencyObject>())
      {
        DependencyObject dependencyObject1 = source.First.Value;
        source.RemoveFirst();
        foreach (DependencyObject dependencyObject2 in UIUtils.GetVisualTreeChildren(dependencyObject1).ToList<DependencyObject>())
        {
          if (select(dependencyObject2))
          {
            if (dependencyObject2 is T obj1)
              displayedInVisualTree.Add(obj1);
          }
          else if ((!(dependencyObject2 is UIElement uiElement) || uiElement.Visibility != Visibility.Collapsed) && (!(dependencyObject2 is PivotItem pivotItem) || !(pivotItem.Parent is Pivot parent1) || (PivotItem) parent1.SelectedItem == pivotItem) && (!(dependencyObject1 is ListPickerItem listPickerItem) || !(listPickerItem.Parent is ListPicker parent2) || parent2.ListPickerMode != ListPickerMode.Normal || parent2.SelectionMode != SelectionMode.Single || listPickerItem == parent2.SelectedItem))
            source.AddLast(dependencyObject2);
        }
      }
      return displayedInVisualTree;
    }

    public static DependencyObject FindFirstInVisualTree(
      DependencyObject obj,
      Func<DependencyObject, bool> pred)
    {
      if (obj == null)
        return (DependencyObject) null;
      LinkedList<DependencyObject> source = new LinkedList<DependencyObject>();
      source.AddLast(obj);
      while (source.Any<DependencyObject>())
      {
        DependencyObject dependencyObject = source.First.Value;
        source.RemoveFirst();
        foreach (DependencyObject visualTreeChild in UIUtils.GetVisualTreeChildren(dependencyObject))
        {
          if (pred(visualTreeChild))
            return visualTreeChild;
          source.AddLast(visualTreeChild);
        }
      }
      return (DependencyObject) null;
    }

    public static T FindFirstInVisualTreeByType<T>(DependencyObject obj) where T : DependencyObject
    {
      return UIUtils.FindFirstInVisualTree(obj, (Func<DependencyObject, bool>) (argObj => (object) (argObj as T) != null)) as T;
    }

    public static VisualStateGroup FindVisualState(FrameworkElement elem, string name)
    {
      if (elem != null)
      {
        foreach (VisualStateGroup visualStateGroup in (IEnumerable) VisualStateManager.GetVisualStateGroups(elem))
        {
          if (visualStateGroup.Name == name)
            return visualStateGroup;
        }
      }
      return (VisualStateGroup) null;
    }

    public static IEnumerable<Popup> GetOpenPopups() => VisualTreeHelper.GetOpenPopups();

    public static IObservable<JournalEntryRemovedEventArgs> RemovedFromJournalObservable(Uri uri)
    {
      return Observable.Create<JournalEntryRemovedEventArgs>((Func<IObserver<JournalEntryRemovedEventArgs>, Action>) (observer =>
      {
        UIUtils.ThrowIfOffDispatcher();
        LinkedList<IObserver<JournalEntryRemovedEventArgs>> list;
        if (!UIUtils.removedFromJournalDict.TryGetValue(uri, out list))
        {
          list = new LinkedList<IObserver<JournalEntryRemovedEventArgs>>();
          UIUtils.removedFromJournalDict.Add(uri, list);
        }
        LinkedListNode<IObserver<JournalEntryRemovedEventArgs>> node = list.AddLast(observer);
        if (UIUtils.removedFromJournalSub == null)
        {
          PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
          frame.JournalEntryRemoved += new EventHandler<JournalEntryRemovedEventArgs>(UIUtils.frame_JournalEntryRemoved);
          UIUtils.removedFromJournalSub = (IDisposable) new DisposableAction((Action) (() => frame.JournalEntryRemoved -= new EventHandler<JournalEntryRemovedEventArgs>(UIUtils.frame_JournalEntryRemoved)));
        }
        return (Action) (() =>
        {
          UIUtils.ThrowIfOffDispatcher();
          list.Remove(node);
          if (list.Any<IObserver<JournalEntryRemovedEventArgs>>())
            return;
          UIUtils.removedFromJournalDict.Remove(uri);
          if (UIUtils.removedFromJournalDict.Count != 0 || UIUtils.removedFromJournalDict == null)
            return;
          UIUtils.removedFromJournalSub.Dispose();
          UIUtils.removedFromJournalSub = (IDisposable) null;
        });
      }));
    }

    private static void frame_JournalEntryRemoved(object sender, JournalEntryRemovedEventArgs e)
    {
      LinkedList<IObserver<JournalEntryRemovedEventArgs>> list = (LinkedList<IObserver<JournalEntryRemovedEventArgs>>) null;
      if (!UIUtils.removedFromJournalDict.TryGetValue(e.Entry.Source, out list))
        return;
      foreach (IObserver<JournalEntryRemovedEventArgs> observer in list.AsRemoveSafeEnumerator<IObserver<JournalEntryRemovedEventArgs>>())
        observer.OnNext(e);
    }

    public static void MessageBox(
      string title,
      string message,
      IEnumerable<string> buttonTitles,
      Action<int> onClick,
      bool forceVertical = false)
    {
      if (title == null)
        title = AppResources.MessageBoxYesNoTitle;
      if (message.Length < (int) byte.MaxValue && buttonTitles.Count<string>() <= 2)
      {
        if (!string.IsNullOrEmpty(title))
        {
          try
          {
            Guide.BeginShowMessageBox(title, Emoji.ConvertToTextOnly(message, (byte[]) null), buttonTitles, 0, MessageBoxIcon.None, (AsyncCallback) (asyncRes =>
            {
              int? res = Guide.EndShowMessageBox(asyncRes);
              Deployment.Current.Dispatcher.BeginInvoke((Action) (() => onClick(res ?? -1)));
            }), (object) null);
            return;
          }
          catch (GuideAlreadyVisibleException ex)
          {
            Action perform = (Action) null;
            perform = (Action) (() =>
            {
              if (Guide.IsVisible)
                WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), perform);
              else
                UIUtils.MessageBox(title, message, buttonTitles, onClick);
            });
            WAThreadPool.QueueUserWorkItem(perform);
            return;
          }
        }
      }
      MessageBoxControl.Show(title, message, buttonTitles, onClick, forceVertical);
    }

    public static IObservable<Unit> ShowMessageBox(string title, string message)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        UIUtils.MessageBox(title, message, (IEnumerable<string>) new string[1]
        {
          AppResources.OK
        }, (Action<int>) (_ =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<bool> Decision(
      string message,
      string positive = null,
      string negative = null,
      string title = null)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        UIUtils.MessageBox(title, message, (IEnumerable<string>) new string[2]
        {
          positive ?? AppResources.Yes,
          negative ?? AppResources.No
        }, (Action<int>) (mbRes =>
        {
          observer.OnNext(mbRes == 0);
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<bool> Decision(
      RichTextBlock.TextSet message,
      string positive = null,
      string negative = null,
      string title = null)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        MessageBoxControl.Show((BitmapImage) null, title ?? AppResources.MessageBoxYesNoTitle, message, (IEnumerable<string>) new string[2]
        {
          positive ?? AppResources.Yes,
          negative ?? AppResources.No
        }, (Action<int>) (mbRes =>
        {
          observer.OnNext(mbRes == 0);
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<bool> Decision(
      this IObservable<bool> source,
      string message,
      string positive = null,
      string negative = null,
      string title = null)
    {
      return Observable.CreateWithDisposable<bool>((Func<IObserver<bool>, IDisposable>) (observer => source.Subscribe<bool>((Action<bool>) (srcRes =>
      {
        if (srcRes)
        {
          UIUtils.Decision(message, positive, negative, title).Subscribe(observer);
        }
        else
        {
          observer.OnNext(srcRes);
          observer.OnCompleted();
        }
      }))));
    }

    public static IObservable<int> Decisions(string message, string[] options, string title = null)
    {
      return Observable.Create<int>((Func<IObserver<int>, Action>) (observer =>
      {
        UIUtils.MessageBox(title, message, (IEnumerable<string>) options, (Action<int>) (mbRes =>
        {
          observer.OnNext(mbRes);
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<int> Decisions(
      this IObservable<bool> source,
      string message,
      string[] options,
      string title = null)
    {
      return Observable.CreateWithDisposable<int>((Func<IObserver<int>, IDisposable>) (observer => source.Subscribe<bool>((Action<bool>) (srcRes =>
      {
        if (srcRes)
        {
          UIUtils.Decisions(message, options, title).Subscribe(observer);
        }
        else
        {
          observer.OnNext(options.Length - 1);
          observer.OnCompleted();
        }
      }))));
    }

    public static void ShowMessageBoxWithGeneralLearnMore(string message, string faqArticleNumber)
    {
      string faqUrlGeneral = WaWebUrls.GetFaqUrlGeneral(faqArticleNumber);
      UIUtils.ShowMessageBoxWithLearnMore(message, faqUrlGeneral);
    }

    public static void ShowMessageBoxWithLearnMore(string message, string url)
    {
      UIUtils.MessageBox(" ", message, (IEnumerable<string>) new string[2]
      {
        AppResources.OkLower,
        AppResources.LearnMoreButton
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        new WebBrowserTask() { Uri = new Uri(url) }.Show();
      }));
    }

    public static void LockOrientation(PhoneApplicationPage page, bool toLock)
    {
      if (page == null)
        return;
      if (toLock)
        page.SupportedOrientations = page.Orientation.IsLandscape() ? SupportedPageOrientation.Landscape : SupportedPageOrientation.Portrait;
      else
        page.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
    }

    public static double SIPHeightPortrait
    {
      get
      {
        if (UIUtils.sIPHeightPortrait > 0.0)
          return UIUtils.sIPHeightPortrait;
        object propertyValue = (object) 0;
        Settings.ScreenResolutionKind resolution = ResolutionHelper.GetResolution();
        switch (resolution)
        {
          case Settings.ScreenResolutionKind.WXGA:
          case Settings.ScreenResolutionKind.HD1080p:
            UIUtils.sIPHeightPortrait = !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue) || (double) propertyValue <= 375.0 ? 408.0 : 404.0;
            break;
          case Settings.ScreenResolutionKind.HD720p:
            UIUtils.sIPHeightPortrait = !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue) || (double) propertyValue <= 250.0 ? 408.0 : 402.0;
            break;
          default:
            UIUtils.sIPHeightPortrait = 408.0;
            break;
        }
        Log.d("UIUtils EmojiLog", "ScreenResolutionKind={0}, RawRpiX={1}, SIP HeightPorait={2}", (object) resolution, propertyValue, (object) UIUtils.sIPHeightPortrait);
        return UIUtils.sIPHeightPortrait;
      }
    }

    public static double SIPHeightAdjustmentForWP10
    {
      get
      {
        if (!AppState.IsWP10OrLater)
          return 0.0;
        if (UIUtils.sIPHeightAdjustmentForWP10 > 0.0)
          return UIUtils.sIPHeightAdjustmentForWP10;
        int num = 100;
        object propertyValue1;
        if (DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out propertyValue1))
          num = (int) (((Size) propertyValue1).Width / 4.8);
        object propertyValue2 = (object) 0;
        switch (num)
        {
          case 100:
            UIUtils.sIPHeightAdjustmentForWP10 = !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue2) || (double) propertyValue2 <= 200.0 ? -58.0 : -31.0;
            break;
          case 150:
            UIUtils.sIPHeightAdjustmentForWP10 = !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue2) || (double) propertyValue2 <= 260.0 ? -58.0 : -25.0;
            break;
          case 160:
            UIUtils.sIPHeightAdjustmentForWP10 = !DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue2) || (double) propertyValue2 <= 270.0 ? -47.0 : -53.0;
            break;
          default:
            UIUtils.sIPHeightAdjustmentForWP10 = -58.0;
            break;
        }
        Log.d("UIUtils EmojiLog", "ScaleFactor={0}, RawRpiX={1}, SIP Adjustment={2}", (object) num, propertyValue2, (object) UIUtils.sIPHeightAdjustmentForWP10);
        return UIUtils.sIPHeightAdjustmentForWP10;
      }
    }

    public static double SIPHeightLandscape
    {
      get
      {
        switch (ResolutionHelper.GetResolution())
        {
          case Settings.ScreenResolutionKind.WXGA:
            return 328.0;
          case Settings.ScreenResolutionKind.HD720p:
            object propertyValue1;
            return DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue1) && (double) propertyValue1 > 250.0 ? 324.0 : 328.0;
          case Settings.ScreenResolutionKind.HD1080p:
            object propertyValue2;
            return DeviceExtendedProperties.TryGetValue("RawDpiX", out propertyValue2) && (double) propertyValue2 > 375.0 ? 324.0 : 328.0;
          default:
            return 328.0;
        }
      }
    }

    public static double SystemTraySizePortrait => 32.0 * ResolutionHelper.ZoomMultiplier;

    public static double SystemTraySizeLandscape => 72.0 * ResolutionHelper.ZoomMultiplier;

    public static double PageSideMargin => 24.0 * ResolutionHelper.ZoomMultiplier;

    public static double AngleBetween2Lines(PinchContactPoints line1, PinchContactPoints line2)
    {
      if (line1 == null || line2 == null)
        return 0.0;
      System.Windows.Point point = line1.PrimaryContact;
      double y1 = point.Y;
      point = line1.SecondaryContact;
      double y2 = point.Y;
      double y3 = y1 - y2;
      point = line1.PrimaryContact;
      double x1 = point.X;
      point = line1.SecondaryContact;
      double x2 = point.X;
      double x3 = x1 - x2;
      double num1 = Math.Atan2(y3, x3);
      point = line2.PrimaryContact;
      double y4 = point.Y;
      point = line2.SecondaryContact;
      double y5 = point.Y;
      double y6 = y4 - y5;
      point = line2.PrimaryContact;
      double x4 = point.X;
      point = line2.SecondaryContact;
      double x5 = point.X;
      double x6 = x4 - x5;
      double num2 = Math.Atan2(y6, x6);
      return (num1 - num2) * 180.0 / Math.PI;
    }

    public static bool IsRightToLeft()
    {
      return new CultureInfo(AppResources.CultureString).IsRightToLeft();
    }

    public static void EnableWakeLock(bool enable)
    {
      PhoneApplicationService.Current.UserIdleDetectionMode = enable ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
    }

    public static void EnableAppBar(ApplicationBar appbar, bool enable)
    {
      if (appbar == null)
        return;
      new AppBarWrapper((IApplicationBar) appbar).IsEnabled = enable;
    }

    public static void UpdateAppBarButton(
      ApplicationBarIconButton button,
      string text,
      string imagePath = null,
      EventHandler removeHandler = null,
      EventHandler addHandler = null)
    {
      if (button == null)
        return;
      if (!string.IsNullOrEmpty(text))
        button.Text = text;
      if (!string.IsNullOrEmpty(imagePath))
        button.IconUri = new Uri(imagePath, UriKind.Relative);
      if (removeHandler != null)
        button.Click -= removeHandler;
      if (addHandler == null)
        return;
      button.Click += addHandler;
    }

    private static void ThrowIfOffDispatcher()
    {
      if (!Deployment.Current.Dispatcher.CheckAccess())
        throw new UnauthorizedAccessException("Invalid cross-thread access");
    }

    public static SolidColorBrush ForegroundBrush
    {
      get
      {
        return UIUtils.foregroundBrush_ ?? (UIUtils.foregroundBrush_ = Application.Current.Resources[(object) "PhoneForegroundBrush"] as SolidColorBrush);
      }
    }

    public static string ForegroundColorCode
    {
      get
      {
        if (UIUtils.foregroundColorCode == null)
          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => UIUtils.foregroundColorCode = UIUtils.ForegroundBrush.Color.ToString()));
        return UIUtils.foregroundColorCode;
      }
    }

    public static SolidColorBrush BackgroundBrush
    {
      get
      {
        return UIUtils.backgroundBrush_ ?? (UIUtils.backgroundBrush_ = Application.Current.Resources[(object) "PhoneBackgroundBrush"] as SolidColorBrush);
      }
    }

    public static SolidColorBrush AccentBrush
    {
      get
      {
        return UIUtils.accentBrush_ ?? (UIUtils.accentBrush_ = Application.Current.Resources[(object) "PhoneAccentBrush"] as SolidColorBrush);
      }
    }

    public static SolidColorBrush TranslucentAccentBrush
    {
      get
      {
        if (UIUtils.translucentAccentBrush_ == null)
        {
          UIUtils.translucentAccentBrush_ = new SolidColorBrush(UIUtils.AccentBrush.Color);
          UIUtils.translucentAccentBrush_.Opacity = 0.5;
        }
        return UIUtils.translucentAccentBrush_;
      }
    }

    public static string AccentColorCode
    {
      get
      {
        if (UIUtils.accentColorCode == null)
          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => UIUtils.accentColorCode = UIUtils.AccentBrush.Color.ToString()));
        return UIUtils.accentColorCode;
      }
    }

    public static SolidColorBrush DarkAccentBrush
    {
      get
      {
        if (UIUtils.darkAccentBrush_ == null)
        {
          Color color = UIUtils.AccentBrush.Color;
          double num = 0.65;
          UIUtils.darkAccentBrush_ = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) ((double) color.R * num), (byte) ((double) color.G * num), (byte) ((double) color.B * num)));
        }
        return UIUtils.darkAccentBrush_;
      }
    }

    public static SolidColorBrush DarkAccentMetadataBrush
    {
      get
      {
        if (UIUtils.darkAccentMetadataBrush_ == null)
        {
          Color color = UIUtils.DarkAccentBrush.Color;
          UIUtils.darkAccentMetadataBrush_ = new SolidColorBrush(Color.FromArgb((byte) 165, color.R, color.G, color.B));
        }
        return UIUtils.darkAccentMetadataBrush_;
      }
    }

    public static Brush PhoneChromeBrush
    {
      get
      {
        return UIUtils.phoneChromeBrush ?? (UIUtils.phoneChromeBrush = (Brush) (Application.Current.Resources[(object) nameof (PhoneChromeBrush)] as SolidColorBrush));
      }
    }

    public static Brush InactiveBrush { get; } = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 102, (byte) 102, (byte) 102));

    public static Brush PhoneInactiveBrush
    {
      get
      {
        return UIUtils.phoneInactiveBrush ?? (UIUtils.phoneInactiveBrush = Application.Current.Resources[(object) nameof (PhoneInactiveBrush)] as Brush);
      }
    }

    public static Visibility DebugOnlyVisibility => Visibility.Collapsed;

    public static bool IsScrollViewerNearBottom(
      double svExtentHeight,
      double svViewportHeight,
      double svVerticalOffset,
      double threshold = 50.0)
    {
      return svExtentHeight - threshold <= svViewportHeight + svVerticalOffset;
    }

    public static bool IsScrolledNearBottom(this ScrollViewer sv, double threshold = 50.0)
    {
      return UIUtils.IsScrollViewerNearBottom(sv.ExtentHeight, sv.ViewportHeight, sv.VerticalOffset, threshold);
    }

    public static bool IsScrolledToBottom(this ScrollViewer sv)
    {
      return UIUtils.IsScrollViewerNearBottom(sv.ExtentHeight, sv.ViewportHeight, sv.VerticalOffset, 0.0);
    }

    public static void ScrollToBottom(this ScrollViewer sv)
    {
      double scrollableHeight = sv.ScrollableHeight;
      if (double.IsNaN(scrollableHeight) || double.IsInfinity(scrollableHeight))
        return;
      sv.ScrollToVerticalOffset(scrollableHeight);
    }

    public static Color HexToColor(string hex)
    {
      hex = hex.Replace("#", string.Empty);
      int startIndex = -1;
      if (hex.Length == 8)
        startIndex = 2;
      else if (hex.Length == 6)
        startIndex = 0;
      return Color.FromArgb(byte.MaxValue, (byte) Convert.ToUInt32(hex.Substring(startIndex, 2), 16), (byte) Convert.ToUInt32(hex.Substring(startIndex + 2, 2), 16), (byte) Convert.ToUInt32(hex.Substring(startIndex + 4, 2), 16));
    }

    public static Color ToSolidColor(this Color color)
    {
      Color solidColor = color;
      if (solidColor.A < byte.MaxValue)
      {
        double num = (double) solidColor.A / (double) byte.MaxValue;
        solidColor.A = byte.MaxValue;
        solidColor.R = (byte) ((double) solidColor.R * num);
        solidColor.G = (byte) ((double) solidColor.G * num);
        solidColor.B = (byte) ((double) solidColor.B * num);
      }
      return solidColor;
    }

    public static Rectangle CreateFadingGradient(Color baseColor, System.Windows.Point startPoint, System.Windows.Point endPoint)
    {
      Rectangle fadingGradient = new Rectangle();
      fadingGradient.Stretch = Stretch.Fill;
      fadingGradient.IsHitTestVisible = false;
      fadingGradient.Fill = (Brush) UIUtils.CreateFadingGradientBrush(baseColor, startPoint, endPoint);
      return fadingGradient;
    }

    public static LinearGradientBrush CreateFadingGradientBrush(
      Color baseColor,
      System.Windows.Point startPoint,
      System.Windows.Point endPoint)
    {
      Color color1 = baseColor;
      Color color2 = baseColor with { A = 0 };
      LinearGradientBrush fadingGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = color1,
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = color2,
        Offset = 1.0
      });
      fadingGradientBrush.GradientStops = gradientStopCollection;
      fadingGradientBrush.StartPoint = startPoint;
      fadingGradientBrush.EndPoint = endPoint;
      return fadingGradientBrush;
    }

    public static Paragraph TryCreateLinkInParagraph(string source, Action action)
    {
      Paragraph linkInParagraph = new Paragraph();
      string str1 = "<a>";
      string str2 = "</a>";
      int num1 = source.IndexOf(str1);
      if (num1 >= 0)
      {
        int num2 = source.IndexOf(str2, num1);
        if (num2 >= 0)
        {
          linkInParagraph.Inlines.Add(source.Substring(0, num1));
          Hyperlink hyperlink1 = new Hyperlink();
          hyperlink1.Foreground = (Brush) UIUtils.AccentBrush;
          hyperlink1.TextDecorations = (TextDecorationCollection) null;
          hyperlink1.Command = (ICommand) new ActionCommand(action);
          hyperlink1.MouseOverTextDecorations = (TextDecorationCollection) null;
          hyperlink1.FontWeight = FontWeights.SemiBold;
          Hyperlink hyperlink2 = hyperlink1;
          hyperlink2.Inlines.Add(source.Substring(num1 + str1.Length, num2 - num1 - str1.Length));
          linkInParagraph.Inlines.Add((Inline) hyperlink2);
          string text = source.Substring(num2 + str2.Length);
          linkInParagraph.Inlines.Add(text);
        }
      }
      else
        linkInParagraph.Inlines.Add(source);
      return linkInParagraph;
    }

    public class DelegateCommand : ICommand
    {
      private Func<object, bool> canExecute_;
      private Action<object> executeAction_;

      public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute = null)
      {
        this.executeAction_ = executeAction != null ? executeAction : throw new ArgumentNullException(nameof (executeAction));
        this.canExecute_ = canExecute;
      }

      public bool CanExecute(object parameter)
      {
        bool flag = true;
        Func<object, bool> canExecute = this.canExecute_;
        if (canExecute != null)
          flag = canExecute(parameter);
        return flag;
      }

      public event EventHandler CanExecuteChanged;

      public void RaiseCanExecuteChanged()
      {
        EventHandler canExecuteChanged = this.CanExecuteChanged;
        if (canExecuteChanged == null)
          return;
        canExecuteChanged((object) this, new EventArgs());
      }

      public void Execute(object parameter) => this.executeAction_(parameter);
    }

    public class FrameworkElementWrapper
    {
      private FrameworkElement elem_;
      private System.Windows.Point position_;

      public FrameworkElementWrapper(FrameworkElement elem) => this.elem_ = elem;

      public bool Visible
      {
        get => this.elem_.Visibility == Visibility.Visible;
        set => this.elem_.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }

      public double Width
      {
        get => this.elem_.Width;
        set => this.elem_.Width = value;
      }

      public double Height
      {
        get => this.elem_.Height;
        set => this.elem_.Height = value;
      }

      public System.Windows.Point Position
      {
        get => this.position_;
        set
        {
          if (UIUtils.FrameworkElementWrapper.NearlyEquals(this.position_.X, value.X) && UIUtils.FrameworkElementWrapper.NearlyEquals(this.position_.Y, value.Y))
            return;
          this.position_ = value;
          this.OnPositionChanged(true, true);
        }
      }

      public double Left
      {
        get => this.position_.X;
        set
        {
          if (UIUtils.FrameworkElementWrapper.NearlyEquals(this.position_.X, value))
            return;
          this.position_.X = value;
          this.OnPositionChanged(true, false);
        }
      }

      public double Top
      {
        get => this.position_.Y;
        set
        {
          if (UIUtils.FrameworkElementWrapper.NearlyEquals(this.position_.Y, value))
            return;
          this.position_.Y = value;
          this.OnPositionChanged(false, true);
        }
      }

      private static bool NearlyEquals(double a, double b) => Math.Abs(a - b) < 0.0001;

      private void OnPositionChanged(bool posXChanged, bool posYChanged)
      {
        if (this.elem_ == null || !(this.elem_.RenderTransform is TranslateTransform renderTransform))
          return;
        if (posXChanged)
          renderTransform.X = this.Left;
        if (!posYChanged)
          return;
        renderTransform.Y = this.Top;
      }
    }
  }
}
