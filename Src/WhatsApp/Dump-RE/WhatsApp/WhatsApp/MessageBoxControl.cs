// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageBoxControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class MessageBoxControl : UserControl
  {
    internal Grid LayoutRoot;
    internal Image IconImage;
    internal TextBlock TitleBlock;
    internal RichTextBlock ContentBlock;
    internal Grid ButtonsPanel;
    private bool _contentLoaded;

    public MessageBoxControl() => this.InitializeComponent();

    public static void Show(
      string title,
      string body,
      IEnumerable<string> buttonTitles,
      Action<int> onClick,
      bool forceVertical = false)
    {
      RichTextBlock.TextSet body1 = new RichTextBlock.TextSet()
      {
        Text = body
      };
      MessageBoxControl.Show((BitmapImage) null, title, body1, buttonTitles, onClick, forceVertical);
    }

    public static void Show(
      BitmapImage icon,
      string title,
      RichTextBlock.TextSet body,
      IEnumerable<string> buttonTitles,
      Action<int> onClick,
      bool forceVertical = false)
    {
      MessageBoxControl.ShowImpl(icon, title, body, (Paragraph) null, buttonTitles, onClick, forceVertical);
    }

    public static void Show(
      BitmapImage icon,
      string title,
      Paragraph body,
      IEnumerable<string> buttonTitles,
      Action<int> onClick,
      bool forceVertical = false)
    {
      MessageBoxControl.ShowImpl(icon, title, (RichTextBlock.TextSet) null, body, buttonTitles, onClick);
    }

    private static void ShowImpl(
      BitmapImage icon,
      string title,
      RichTextBlock.TextSet bodyRichText,
      Paragraph bodyParagrah,
      IEnumerable<string> buttonLabels,
      Action<int> onClick,
      bool forceVertical = false)
    {
      MessageBoxControl control = new MessageBoxControl();
      bool flag = App.CurrentApp.RootFrame.FlowDirection == FlowDirection.RightToLeft;
      control.LayoutRoot.FlowDirection = flag ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
      if (icon == null)
      {
        control.IconImage.Visibility = Visibility.Collapsed;
      }
      else
      {
        control.IconImage.Source = (System.Windows.Media.ImageSource) icon;
        control.IconImage.Visibility = Visibility.Visible;
      }
      if (title == null)
      {
        control.TitleBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        control.TitleBlock.Text = string.IsNullOrEmpty(title) ? " " : title;
        control.TitleBlock.Visibility = Visibility.Visible;
      }
      if (bodyRichText != null)
        control.ContentBlock.Text = bodyRichText;
      else if (bodyParagrah != null)
      {
        control.ContentBlock.Blocks.Clear();
        control.ContentBlock.Blocks.Add((Block) bodyParagrah);
      }
      List<Button> buttonList = new List<Button>();
      int num1 = buttonLabels.Count<string>();
      if (num1 == 2 && !forceVertical)
      {
        control.ButtonsPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        control.ButtonsPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        Button button1 = new Button();
        button1.Content = (object) (buttonLabels.ElementAtOrDefault<string>(0) ?? "");
        button1.HorizontalAlignment = HorizontalAlignment.Stretch;
        button1.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
        Button element1 = button1;
        element1.Click += (RoutedEventHandler) ((sender, e) => onClick(0));
        Grid.SetColumn((FrameworkElement) element1, 0);
        Button button2 = new Button();
        button2.Content = (object) (buttonLabels.ElementAtOrDefault<string>(1) ?? "");
        button2.HorizontalAlignment = HorizontalAlignment.Stretch;
        button2.Margin = new Thickness(0.0, 0.0, -12.0, 0.0);
        Button element2 = button2;
        element2.Click += (RoutedEventHandler) ((sender, e) => onClick(1));
        Grid.SetColumn((FrameworkElement) element2, 1);
        control.ButtonsPanel.Children.Add((UIElement) element1);
        control.ButtonsPanel.Children.Add((UIElement) element2);
      }
      else
      {
        for (int index = 0; index < num1; ++index)
        {
          control.ButtonsPanel.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Auto)
          });
          Button button = new Button();
          button.Content = (object) (buttonLabels.ElementAtOrDefault<string>(index) ?? "");
          button.HorizontalAlignment = HorizontalAlignment.Left;
          button.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
          button.MinWidth = 144.0;
          Button element = button;
          int indexSnap = index;
          element.Click += (RoutedEventHandler) ((sender, e) => onClick(indexSnap));
          Grid.SetRow((FrameworkElement) element, index);
          control.ButtonsPanel.Children.Add((UIElement) element);
        }
      }
      Popup popup = new Popup();
      popup.Child = (UIElement) control;
      popup.Closed += (EventHandler) ((sender, args) => onClick(-1));
      PopupManager mgr = new PopupManager(popup, true);
      Action updateHeight = (Action) (() =>
      {
        control.UpdateLayout();
        EventHandler layoutUpdated = (EventHandler) null;
        layoutUpdated = (EventHandler) ((s, args) =>
        {
          double num4 = control.LayoutRoot.Children.Cast<FrameworkElement>().Where<FrameworkElement>((Func<FrameworkElement, bool>) (p => p.Visibility == Visibility.Visible)).Sum<FrameworkElement>((Func<FrameworkElement, double>) (e => e.ActualHeight + e.Margin.Top + e.Margin.Bottom));
          double height = control.Height;
          double num5;
          if (num4 > height && (num5 = height - (num4 - control.ContentBlock.ActualHeight)) > 0.0)
          {
            control.ContentBlock.Height = num5;
            num4 = height;
          }
          else
            control.ContentBlock.Height = control.ContentBlock.ActualHeight;
          control.Height = num4;
          control.LayoutUpdated -= layoutUpdated;
        });
        control.LayoutUpdated += layoutUpdated;
      });
      mgr.OrientationChanged += (EventHandler<EventArgs>) ((sender, args) =>
      {
        if (mgr.Orientation.IsLandscape())
        {
          control.TitleBlock.Margin = new Thickness(72.0, 24.0, 72.0, 24.0);
          control.ContentBlock.Margin = new Thickness(60.0, 0.0, 60.0, 0.0);
          control.ButtonsPanel.Margin = new Thickness(72.0, 12.0, 72.0, 12.0);
        }
        else
        {
          control.TitleBlock.Margin = new Thickness(24.0);
          control.ContentBlock.Margin = new Thickness(12.0, 0.0, 12.0, 0.0);
          control.ButtonsPanel.Margin = new Thickness(24.0, 12.0, 24.0, 12.0);
        }
        updateHeight();
      });
      Action<int> snap = onClick;
      onClick = (Action<int>) (i =>
      {
        Action<int> action = Interlocked.Exchange<Action<int>>(ref snap, (Action<int>) null);
        if (action == null)
          return;
        try
        {
          action(i);
        }
        finally
        {
          popup.IsOpen = false;
        }
      });
      if (Application.Current.RootVisual is PhoneApplicationFrame rootVisual)
      {
        PhoneApplicationPage page = rootVisual.Content as PhoneApplicationPage;
        if (page != null)
        {
          page.IsEnabled = false;
          popup.Closed += (EventHandler) ((sender, args) => page.IsEnabled = true);
        }
      }
      CompositeDisposable subscriptions = new CompositeDisposable(new IDisposable[3]
      {
        SysTrayHelper.SysTrayKeeper.Instance.RequestHide(),
        AppState.MuteUIUpdates.Subscribe(),
        App.CurrentApp.RootFrame.GetNavigatingAsync().Take<NavigatingCancelEventArgs>(1).Subscribe<NavigatingCancelEventArgs>((Action<NavigatingCancelEventArgs>) (_ => onClick(-1)))
      });
      popup.Closed += (EventHandler) ((sender, args) => subscriptions.SafeDispose());
      mgr.Show();
      updateHeight();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MessageBoxControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.IconImage = (Image) this.FindName("IconImage");
      this.TitleBlock = (TextBlock) this.FindName("TitleBlock");
      this.ContentBlock = (RichTextBlock) this.FindName("ContentBlock");
      this.ButtonsPanel = (Grid) this.FindName("ButtonsPanel");
    }
  }
}
