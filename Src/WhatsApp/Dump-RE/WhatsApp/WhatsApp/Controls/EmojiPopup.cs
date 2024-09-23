// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.EmojiPopup
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp.Controls
{
  public class EmojiPopup : UserControl
  {
    private EmojiPickerViewModel ViewModel;
    public static System.Windows.Point StartPoint;
    private static System.Windows.Point Offset;
    private static System.Windows.Point OverlayTopLeft;
    private static System.Windows.Point OverlayGridTopLeft;
    public int CurrentButtonCount;
    public int PivotPosition;
    private int CurrentSelectionIndex = -1;
    public Emoji.EmojiChar EmojiChar;
    public bool Routing;
    private bool isOpen;
    public static double OverlayHeight;
    public static double OverlayWidth;
    private static Brush overlayBrush = (Brush) null;
    public const int MaxButtonCount = 6;
    public static int MarginSize = 0;
    public static int GridWidth = 67;
    public static int GridHeight = 80;
    private static Brush transparentBrush = (Brush) null;
    private static Brush lightGrayBrush = (Brush) null;
    private static Brush blackBrush = (Brush) null;
    private static Thickness FullMargin = new Thickness((double) EmojiPopup.MarginSize, (double) EmojiPopup.MarginSize, (double) EmojiPopup.MarginSize, (double) EmojiPopup.MarginSize);
    private static Thickness DefaultMargin = new Thickness(0.0, (double) EmojiPopup.MarginSize, (double) EmojiPopup.MarginSize, (double) EmojiPopup.MarginSize);
    private static GridLength DefaultGridWidth = new GridLength((double) (EmojiPopup.GridWidth + EmojiPopup.MarginSize));
    private static GridLength DefaultGridAndMarginWidth = new GridLength((double) (EmojiPopup.GridWidth + 2 * EmojiPopup.MarginSize));
    private static GridLength CollapsedGridLength = new GridLength(0.0);
    private Storyboard fanoutStoryboard;
    public int IndexOffset;
    internal Popup PopupSelector;
    internal Canvas Overlay;
    internal Grid OverlayGrid;
    private bool _contentLoaded;

    public bool IsOpen
    {
      get => this.isOpen;
      set
      {
        if (this.isOpen == value)
          return;
        if (value)
          this.ViewModel.PopupOpen = true;
        this.isOpen = value;
        this.PopupSelector.IsOpen = value;
        if (this.CurrentSelectionIndex > 0 && this.CurrentSelectionIndex < 6)
        {
          ((this.OverlayGrid.Children[this.CurrentSelectionIndex] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.InactiveBrush;
          this.CurrentSelectionIndex = -1;
        }
        this.HideOverlay();
        this.Routing = true;
      }
    }

    public static Brush OverlayBrush
    {
      get
      {
        return EmojiPopup.overlayBrush ?? (EmojiPopup.overlayBrush = (Brush) new SolidColorBrush(Color.FromArgb((byte) 100, (byte) 50, (byte) 50, (byte) 50)));
      }
    }

    public static Brush ButtonBorderBrush
    {
      get => !ImageStore.IsDarkTheme() ? EmojiPopup.LightGrayBrush : EmojiPopup.BlackBrush;
    }

    public static Brush TransparentBrush
    {
      get
      {
        return EmojiPopup.transparentBrush ?? (EmojiPopup.transparentBrush = (Brush) new SolidColorBrush(Colors.Transparent));
      }
    }

    public static Brush LightGrayBrush
    {
      get
      {
        return EmojiPopup.lightGrayBrush ?? (EmojiPopup.lightGrayBrush = (Brush) new SolidColorBrush(Colors.LightGray));
      }
    }

    public static Brush BlackBrush
    {
      get
      {
        return EmojiPopup.blackBrush ?? (EmojiPopup.blackBrush = (Brush) new SolidColorBrush(Colors.Black));
      }
    }

    public Storyboard FanoutStoryboard
    {
      get
      {
        if (this.fanoutStoryboard == null)
        {
          this.fanoutStoryboard = new Storyboard();
          for (int index = 0; index < 6; ++index)
          {
            Grid child1 = this.OverlayGrid.Children[index] as Grid;
            UIElement child2 = child1.Children[0];
            DoubleAnimation element = new DoubleAnimation();
            element.To = new double?(0.0);
            this.fanoutStoryboard.Children.Add((Timeline) element);
            Storyboard.SetTarget((Timeline) element, (DependencyObject) child1);
            Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)", new object[0]));
          }
        }
        return this.fanoutStoryboard;
      }
    }

    public EmojiPopup(EmojiPickerViewModel viewModel)
    {
      this.InitializeComponent();
      this.ViewModel = viewModel;
      this.CurrentSelectionIndex = -1;
      this.PopupSelector.Closed += (EventHandler) ((sender, e) =>
      {
        if (this.ViewModel.CurrentEmojiKeyboardItem == null || this.PopupSelector.IsOpen)
          return;
        this.ViewModel.CurrentEmojiKeyboardItem.Background = EmojiPickerViewModel.InactiveBrush;
        this.ViewModel.CurrentEmojiKeyboardItem = (Grid) null;
      });
      this.Overlay.Tap += new EventHandler<GestureEventArgs>(this.Overlay_Tap);
      this.Overlay.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.Overlay_ManipulationStarted);
      this.Overlay.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.Overlay_ManipulationDelta);
      this.Overlay.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.Overlay_ManipulationCompleted);
      for (int index = 0; index < 6; ++index)
      {
        Grid child1 = this.OverlayGrid.Children[index] as Grid;
        Rectangle child2 = child1.Children[0] as Rectangle;
        child1.Background = EmojiPopup.ButtonBorderBrush;
        Canvas.SetZIndex((UIElement) child1, 6 - index);
        child2.Fill = EmojiPickerViewModel.InactiveBrush;
        int j = index;
        child2.Tap += (EventHandler<GestureEventArgs>) ((sender, e) => this.ViewModel.OnEmojiSelectorTapped(this.EmojiChar, j));
        this.OverlayGrid.ColumnDefinitions.Add(new ColumnDefinition());
      }
    }

    public void SetRelativePosition(double x, double y)
    {
      Canvas.SetLeft((UIElement) this.OverlayGrid, x);
      Canvas.SetTop((UIElement) this.OverlayGrid, y);
    }

    public void SetEmojiChar(Emoji.EmojiChar emojiChar) => this.EmojiChar = emojiChar;

    public void SetButtonCount(int count, int pivot)
    {
      this.CurrentButtonCount = count;
      for (int index = this.OverlayGrid.Children.Count - 1; index >= 0; --index)
      {
        if (index < count)
        {
          Grid CurrentGrid = this.OverlayGrid.Children[index] as Grid;
          Rectangle CurrentButton = CurrentGrid.Children[0] as Rectangle;
          int num = (pivot - index) * (EmojiPopup.GridWidth + EmojiPopup.MarginSize);
          if (!(CurrentGrid.RenderTransform is CompositeTransform))
            CurrentGrid.RenderTransform = (Transform) new CompositeTransform();
          ((CompositeTransform) CurrentGrid.RenderTransform).TranslateX = (double) num;
          this.FanoutStoryboard.Children[index].Duration = new Duration(TimeSpan.FromSeconds(Math.Abs((double) num / 1300.0)));
          if (index == 0)
          {
            CurrentButton.Margin = EmojiPopup.FullMargin;
            this.OverlayGrid.ColumnDefinitions[index].Width = EmojiPopup.DefaultGridAndMarginWidth;
          }
          else
          {
            CurrentButton.Margin = EmojiPopup.DefaultMargin;
            this.OverlayGrid.ColumnDefinitions[index].Width = EmojiPopup.DefaultGridWidth;
          }
        }
        else
          this.OverlayGrid.ColumnDefinitions[index].Width = EmojiPopup.CollapsedGridLength;
      }
      List<string> allEmojiVariants = Emoji.getAllEmojiVariants(this.EmojiChar.codepoints);
      for (int i = count - 1; i >= 0; --i)
      {
        Emoji.EmojiChar emojiChar = allEmojiVariants == null || count == 1 ? this.EmojiChar : Emoji.GetEmojiChar(allEmojiVariants[i]);
        IDisposable sub = (IDisposable) null;
        emojiChar.Image.Subscribe<Emoji.EmojiChar.Args>((Action<Emoji.EmojiChar.Args>) (returnArgs =>
        {
          CurrentGrid = (Grid) this.OverlayGrid.Children[i];
          CurrentButton = CurrentGrid.Children[0] as Rectangle;
          Rectangle child = CurrentGrid.Children[1] as Rectangle;
          ImageBrush fill = child.Fill as ImageBrush;
          CompositeTransform transform = fill.Transform as CompositeTransform;
          transform.TranslateX = -returnArgs.X;
          transform.TranslateY = -returnArgs.Y;
          fill.ImageSource = (System.Windows.Media.ImageSource) returnArgs.BaseImage;
          child.RenderTransform = (Transform) new ScaleTransform()
          {
            ScaleX = (48.0 / returnArgs.Width / EmojiRow.ScaleFactor),
            ScaleY = (48.0 / returnArgs.Height / EmojiRow.ScaleFactor)
          };
          child.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
          child.Width = returnArgs.Width;
          child.Height = returnArgs.Height;
          sub.SafeDispose();
          sub = (IDisposable) null;
        }));
      }
    }

    public void SetCurrentSelection(int index)
    {
      if (this.CurrentSelectionIndex >= 0 && this.CurrentSelectionIndex < this.CurrentButtonCount)
      {
        ((this.OverlayGrid.Children[this.CurrentSelectionIndex] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.InactiveBrush;
        this.CurrentSelectionIndex = -1;
      }
      if (index < 0 || index >= this.CurrentButtonCount)
        return;
      ((this.OverlayGrid.Children[index] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.ActiveBrush;
      this.CurrentSelectionIndex = index;
    }

    public void StartAnimation() => this.FanoutStoryboard.Begin();

    public void ShowOverlay() => this.Overlay.Background = EmojiPopup.OverlayBrush;

    public void HideOverlay() => this.Overlay.Background = EmojiPopup.TransparentBrush;

    private void Overlay_Tap(object sender, GestureEventArgs e) => this.IsOpen = !this.IsOpen;

    public void Overlay_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.IndexOffset = 0;
      this.ViewModel.EmojiManipulationStarted = true;
      this.FindFingerOffset(e);
      System.Windows.Point point1 = e.ManipulationContainer.TransformToVisual((UIElement) this.Overlay).Transform(e.ManipulationOrigin);
      System.Windows.Point point2 = new System.Windows.Point(point1.X - EmojiPopup.Offset.X, point1.Y - EmojiPopup.Offset.Y);
      if (point2.Y <= (double) (EmojiPopup.GridHeight + 2 * EmojiPopup.MarginSize))
      {
        int index = (int) Math.Floor(point2.X / (double) (EmojiPopup.GridWidth + EmojiPopup.MarginSize));
        if (index >= 0 && index < this.CurrentButtonCount)
        {
          this.SetCurrentSelection(index);
          return;
        }
      }
      this.CurrentSelectionIndex = -1;
    }

    public void FindFingerOffset(ManipulationStartedEventArgs e)
    {
      EmojiPopup.OverlayTopLeft = e.ManipulationContainer.TransformToVisual((UIElement) this.Overlay).Transform(e.ManipulationOrigin);
      EmojiPopup.OverlayGridTopLeft = e.ManipulationContainer.TransformToVisual((UIElement) this.OverlayGrid).Transform(e.ManipulationOrigin);
      EmojiPopup.Offset = new System.Windows.Point(EmojiPopup.OverlayTopLeft.X - EmojiPopup.OverlayGridTopLeft.X, EmojiPopup.OverlayTopLeft.Y - EmojiPopup.OverlayGridTopLeft.Y);
    }

    public void FindFingerOffset(GestureEventArgs e)
    {
      EmojiPopup.OverlayTopLeft = e.GetPosition((UIElement) this.Overlay);
      EmojiPopup.OverlayGridTopLeft = e.GetPosition((UIElement) this.OverlayGrid);
      EmojiPopup.Offset = new System.Windows.Point(EmojiPopup.OverlayTopLeft.X - EmojiPopup.OverlayGridTopLeft.X, EmojiPopup.OverlayTopLeft.Y - EmojiPopup.OverlayGridTopLeft.Y);
    }

    public void Overlay_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      e.Handled = true;
      System.Windows.Point point1 = e.ManipulationContainer.TransformToVisual((UIElement) this.Overlay).Transform(e.ManipulationOrigin);
      System.Windows.Point point2 = new System.Windows.Point(point1.X - EmojiPopup.Offset.X, point1.Y - EmojiPopup.Offset.Y);
      int index = (int) Math.Floor(point2.X / (double) (EmojiPopup.GridWidth + EmojiPopup.MarginSize)) + this.IndexOffset;
      if (point2.Y > 250.0 || point2.Y < 0.0)
        index = -1;
      if (index == this.CurrentSelectionIndex)
        return;
      if (index >= 0 && index < this.CurrentButtonCount)
      {
        if (this.CurrentSelectionIndex >= 0 && this.CurrentSelectionIndex < this.CurrentButtonCount)
          ((this.OverlayGrid.Children[this.CurrentSelectionIndex] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.InactiveBrush;
        ((this.OverlayGrid.Children[index] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.ActiveBrush;
        this.CurrentSelectionIndex = index;
      }
      else
      {
        if (this.CurrentSelectionIndex >= 0 && this.CurrentSelectionIndex < this.CurrentButtonCount)
          ((this.OverlayGrid.Children[this.CurrentSelectionIndex] as Grid).Children[0] as Rectangle).Fill = EmojiPickerViewModel.InactiveBrush;
        this.CurrentSelectionIndex = -1;
      }
    }

    public void Overlay_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.CurrentSelectionIndex >= 0)
      {
        EmojiRow.EmojiUpdateImage(this.CurrentSelectionIndex);
        this.ViewModel.OnEmojiSelectorTapped(this.EmojiChar, this.CurrentSelectionIndex);
      }
      this.ViewModel.RefocusOnSearchTextBox();
      this.IsOpen = false;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/EmojiPopup.xaml", UriKind.Relative));
      this.PopupSelector = (Popup) this.FindName("PopupSelector");
      this.Overlay = (Canvas) this.FindName("Overlay");
      this.OverlayGrid = (Grid) this.FindName("OverlayGrid");
    }
  }
}
