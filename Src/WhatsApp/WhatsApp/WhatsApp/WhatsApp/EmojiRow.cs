// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiRow
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.Controls;


namespace WhatsApp
{
  public class EmojiRow : UserControl
  {
    public static readonly DependencyProperty ItemsContextProperty = DependencyProperty.Register(nameof (ItemsContext), typeof (object), typeof (EmojiRow), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as EmojiRow).ItemsContextChanged(e.NewValue))));
    private static double scaleFactor = 0.0;
    public const int EmojiSizeForPicker = 48;
    private static EmojiRow.ManipulationState CurrentState = EmojiRow.ManipulationState.None;
    private static Grid CurrentLayoutRoot;
    private static Grid CurrentGrid;
    private static EmojiRowContext CurrentRowContext;
    private static Emoji.EmojiChar CurrentEmojiChar;
    private static string CurrentEmojiCode;
    private static int CurrentColumn;
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public EmojiRow() => this.InitializeComponent();

    public static double ScaleFactor
    {
      get
      {
        if (EmojiRow.scaleFactor == 0.0)
        {
          EmojiRow.scaleFactor = (double) Application.Current.Host.Content.ScaleFactor / 100.0;
          if (EmojiRow.scaleFactor > 1.2)
            EmojiRow.scaleFactor = 1.2;
        }
        return EmojiRow.scaleFactor;
      }
    }

    public object ItemsContext
    {
      get => this.GetValue(EmojiRow.ItemsContextProperty);
      set => this.SetValue(EmojiRow.ItemsContextProperty, value);
    }

    public void ItemsContextChanged(object newContext)
    {
      this.InitializeEmojis((EmojiRowContext) newContext);
    }

    public void InitializeEmojis(EmojiRowContext context)
    {
      for (int i = 0; i < this.LayoutRoot.Children.Count; i++)
      {
        if (i < ((IEnumerable<Emoji.EmojiChar>) context.chars).Count<Emoji.EmojiChar>() && context.chars[i] != null)
        {
          Emoji.EmojiChar.Args args = context.args[i];
          if (args != null)
          {
            Grid child1 = this.LayoutRoot.Children[i] as Grid;
            Rectangle child2 = child1.Children[0] as Rectangle;
            ImageBrush fill = child2.Fill as ImageBrush;
            CompositeTransform transform = fill.Transform as CompositeTransform;
            child1.Visibility = Visibility.Visible;
            child1.Background = EmojiPicker.InactiveBrush;
            child1.Tag = (object) args.EmojiChar;
            transform.TranslateX = -args.X;
            transform.TranslateY = -args.Y;
            fill.ImageSource = (System.Windows.Media.ImageSource) args.BaseImage;
            child2.RenderTransform = (Transform) new ScaleTransform()
            {
              ScaleX = (48.0 / args.Width / EmojiRow.ScaleFactor),
              ScaleY = (48.0 / args.Height / EmojiRow.ScaleFactor)
            };
            child2.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            child2.Width = args.Width;
            child2.Height = args.Height;
          }
          else
          {
            Emoji.EmojiChar emojiChar = context.chars[i];
            if (emojiChar != null)
              emojiChar.Image.Subscribe<Emoji.EmojiChar.Args>((Action<Emoji.EmojiChar.Args>) (returnArgs =>
              {
                Grid child3 = this.LayoutRoot.Children[i] as Grid;
                Rectangle child4 = child3.Children[0] as Rectangle;
                ImageBrush fill = child4.Fill as ImageBrush;
                CompositeTransform transform = fill.Transform as CompositeTransform;
                child3.Visibility = Visibility.Visible;
                child3.Background = EmojiPicker.InactiveBrush;
                child3.Tag = (object) returnArgs.EmojiChar;
                transform.TranslateX = -returnArgs.X;
                transform.TranslateY = -returnArgs.Y;
                fill.ImageSource = (System.Windows.Media.ImageSource) returnArgs.BaseImage;
                child4.RenderTransform = (Transform) new ScaleTransform()
                {
                  ScaleX = (48.0 / returnArgs.Width / EmojiRow.ScaleFactor),
                  ScaleY = (48.0 / returnArgs.Height / EmojiRow.ScaleFactor)
                };
                child4.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                child4.Width = returnArgs.Width;
                child4.Height = returnArgs.Height;
              }));
          }
        }
        else
          this.LayoutRoot.Children[i].Visibility = Visibility.Collapsed;
      }
      int num = ((IEnumerable<Emoji.EmojiChar>) context.chars).Count<Emoji.EmojiChar>() - this.LayoutRoot.ColumnDefinitions.Count<ColumnDefinition>();
      if (num < 0)
      {
        for (int index = 0; index < Math.Abs(num); ++index)
          this.LayoutRoot.ColumnDefinitions.RemoveAt(0);
      }
      else
      {
        if (num <= 0)
          return;
        for (int index = 0; index < Math.Abs(num); ++index)
          this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
      }
    }

    private void Emoji_Tap(object sender, GestureEventArgs e)
    {
      this.Emoji_ManipulationInit(sender);
      if (EmojiRow.CurrentColumn == -1 || !EmojiRow.CurrentRowContext.tapActions[EmojiRow.CurrentColumn] || !Emoji.IsVariantSelector(EmojiRow.CurrentEmojiCode))
        return;
      EmojiRow.CurrentState = EmojiRow.ManipulationState.HeldAndNoDelta;
      EmojiRow.CurrentGrid.Background = EmojiPicker.ActiveBrush;
      EmojiPopup popupSelector = EmojiRow.CurrentRowContext.viewmodel.PopupSelector;
      popupSelector.IsOpen = true;
      popupSelector.SetEmojiChar(EmojiRow.CurrentEmojiChar);
      popupSelector.SetButtonCount(1, 0);
      int pivotPosition = EmojiRow.CurrentRowContext.viewmodel.GetPivotPosition(EmojiRow.CurrentColumn);
      if (pivotPosition < 0)
      {
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
      }
      else
      {
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.SetCurrentSelection(-1);
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.ShowOverlay();
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.SetButtonCount(6, pivotPosition);
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IndexOffset = pivotPosition;
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.StartAnimation();
      }
    }

    private void Emoji_Hold(object sender, GestureEventArgs e)
    {
      if (string.IsNullOrEmpty(EmojiRow.CurrentEmojiCode))
      {
        EmojiRow.Emoji_ManipulationCleanup();
      }
      else
      {
        if (!Emoji.IsVariantSelector(EmojiRow.CurrentEmojiCode) || EmojiRow.CurrentState == EmojiRow.ManipulationState.HeldAndNoDelta)
          return;
        EmojiRow.CurrentState = EmojiRow.ManipulationState.HeldAndNoDelta;
        EmojiRow.CurrentGrid.Background = EmojiPicker.ActiveBrush;
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = true;
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.FindFingerOffset(e);
        int currentColumn = EmojiRow.GetCurrentColumn();
        int pivotPosition = EmojiRow.CurrentRowContext.viewmodel.GetPivotPosition(currentColumn);
        if (pivotPosition < 0)
        {
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
        }
        else
        {
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.ShowOverlay();
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.SetButtonCount(6, pivotPosition);
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.SetCurrentSelection(Emoji.GetVariationSelectorIndex(EmojiRow.CurrentEmojiChar));
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IndexOffset = pivotPosition;
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.StartAnimation();
        }
      }
    }

    private void Emoji_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.Emoji_ManipulationInit(sender);
      EmojiRow.CurrentRowContext.viewmodel.EmojiManipulationStarted = true;
      if (EmojiRow.CurrentRowContext.viewmodel.PopupSelector != null && EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen)
      {
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
        EmojiRow.CurrentRowContext.action(EmojiRow.CurrentEmojiChar);
      }
      EmojiRow.CurrentGrid.Background = EmojiPicker.ActiveBrush;
      EmojiPopup.StartPoint = e.ManipulationOrigin;
      System.Windows.Point anchorpoint = EmojiRow.CurrentGrid.TransformToVisual((UIElement) EmojiRow.CurrentRowContext.viewmodel.EmojiGridContainer).Transform(new System.Windows.Point(0.0, 0.0));
      EmojiRow.CurrentRowContext.viewmodel.ShowVariantSelector(anchorpoint, EmojiRow.CurrentGrid, EmojiRow.CurrentGrid.ActualWidth);
      EmojiRow.CurrentRowContext.viewmodel.PopupSelector.SetEmojiChar(EmojiRow.CurrentEmojiChar);
    }

    private void Emoji_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      switch (EmojiRow.CurrentState)
      {
        case EmojiRow.ManipulationState.None:
          if (EmojiRow.CurrentRowContext == null || EmojiRow.CurrentRowContext.viewmodel.PopupSelector == null)
            break;
          EmojiRow.CurrentState = EmojiRow.ManipulationState.Swipping;
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
          break;
        case EmojiRow.ManipulationState.HeldAndNoDelta:
          e.Handled = true;
          EmojiRow.CurrentState = EmojiRow.ManipulationState.Routing;
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.Routing = true;
          goto case EmojiRow.ManipulationState.Routing;
        case EmojiRow.ManipulationState.Routing:
          e.Handled = true;
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.Overlay_ManipulationDelta(sender, e);
          break;
        case EmojiRow.ManipulationState.Swipping:
          break;
        default:
          this.ErrorCleanup();
          break;
      }
    }

    private void Emoji_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      switch (EmojiRow.CurrentState)
      {
        case EmojiRow.ManipulationState.None:
          if (EmojiRow.CurrentRowContext != null)
          {
            if (EmojiRow.CurrentRowContext.viewmodel.PopupSelector != null && EmojiRow.CurrentColumn != -1 && !EmojiRow.CurrentRowContext.tapActions[EmojiRow.CurrentColumn])
            {
              EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
              EmojiRow.CurrentRowContext.action(EmojiRow.CurrentEmojiChar);
              EmojiRow.Emoji_ManipulationCleanup();
            }
            else
              EmojiRow.CurrentRowContext.viewmodel.RefocusOnSearchTextBox();
          }
          e.Handled = true;
          break;
        case EmojiRow.ManipulationState.HeldAndNoDelta:
          if (EmojiRow.CurrentRowContext != null)
          {
            if (EmojiRow.CurrentRowContext.viewmodel.PopupSelector != null && EmojiRow.CurrentColumn != -1 && !EmojiRow.CurrentRowContext.tapActions[EmojiRow.CurrentColumn])
            {
              EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
              EmojiRow.CurrentRowContext.action(EmojiRow.CurrentEmojiChar);
              EmojiRow.Emoji_ManipulationCleanup();
            }
            else
              EmojiRow.CurrentRowContext.viewmodel.RefocusOnSearchTextBox();
          }
          e.Handled = true;
          break;
        case EmojiRow.ManipulationState.Routing:
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.Overlay_ManipulationCompleted(sender, e);
          EmojiRow.Emoji_ManipulationCleanup();
          e.Handled = true;
          break;
        default:
          this.ErrorCleanup();
          break;
      }
    }

    public static void Emoji_ManipulationCleanup()
    {
      if (EmojiRow.CurrentRowContext != null && EmojiRow.CurrentRowContext.viewmodel != null)
      {
        EmojiRow.CurrentRowContext.viewmodel.RefocusOnSearchTextBox();
        if (EmojiRow.CurrentRowContext.viewmodel.PopupSelector != null)
        {
          EmojiRow.CurrentRowContext.viewmodel.PopupSelector.Routing = true;
          EmojiRow.CurrentRowContext.viewmodel.PopupOpen = false;
        }
      }
      if (EmojiRow.CurrentGrid != null)
        EmojiRow.CurrentGrid.Background = EmojiPicker.InactiveBrush;
      EmojiRow.CurrentState = EmojiRow.ManipulationState.None;
      EmojiRow.CurrentRowContext = (EmojiRowContext) null;
      EmojiRow.CurrentLayoutRoot = (Grid) null;
      EmojiRow.CurrentEmojiChar = (Emoji.EmojiChar) null;
      EmojiRow.CurrentEmojiCode = (string) null;
      EmojiRow.CurrentGrid = (Grid) null;
      EmojiRow.CurrentColumn = -1;
    }

    private void Emoji_ManipulationInit(object sender)
    {
      EmojiRow.CurrentState = EmojiRow.ManipulationState.None;
      if ((EmojiRow.CurrentRowContext = this.ItemsContext as EmojiRowContext) == null)
        this.ErrorCleanup();
      else if ((EmojiRow.CurrentGrid = sender as Grid) == null)
        this.ErrorCleanup();
      else if ((EmojiRow.CurrentEmojiChar = EmojiRow.CurrentGrid.Tag as Emoji.EmojiChar) == null)
      {
        this.ErrorCleanup();
      }
      else
      {
        EmojiRow.CurrentLayoutRoot = this.LayoutRoot;
        EmojiRow.CurrentEmojiCode = EmojiRow.CurrentEmojiChar.codepoints;
        EmojiRow.CurrentColumn = EmojiRow.GetCurrentColumn();
      }
    }

    public static int GetCurrentColumn()
    {
      int currentColumn = 0;
      foreach (Emoji.EmojiChar emojiChar in EmojiRow.CurrentRowContext.chars)
      {
        if (EmojiRow.CurrentEmojiChar == emojiChar)
          return currentColumn;
        ++currentColumn;
      }
      return -1;
    }

    public static void EmojiUpdateImage(int offset)
    {
      if (EmojiRow.CurrentEmojiChar == null)
        EmojiRow.Emoji_ManipulationCleanup();
      Emoji.EmojiChar emojiChar = Emoji.GetEmojiChar(Emoji.GetSingleEmojiVariant(EmojiRow.CurrentEmojiCode, offset));
      if (emojiChar == null)
        EmojiRow.Emoji_ManipulationCleanup();
      EmojiRow.CurrentGrid.Tag = (object) emojiChar;
      EmojiRow.CurrentRowContext.chars[EmojiRow.CurrentColumn] = emojiChar;
      EmojiRow.CurrentRowContext.tapActions[EmojiRow.CurrentColumn] = false;
      emojiChar.Image.Subscribe<Emoji.EmojiChar.Args>((Action<Emoji.EmojiChar.Args>) (args =>
      {
        Grid child1 = EmojiRow.CurrentLayoutRoot.Children[EmojiRow.CurrentColumn] as Grid;
        Rectangle child2 = child1.Children[0] as Rectangle;
        ImageBrush fill = child2.Fill as ImageBrush;
        CompositeTransform transform = fill.Transform as CompositeTransform;
        child1.Visibility = Visibility.Visible;
        child1.Background = EmojiPicker.InactiveBrush;
        child1.Tag = (object) args.EmojiChar;
        transform.TranslateX = -args.X;
        transform.TranslateY = -args.Y;
        fill.ImageSource = (System.Windows.Media.ImageSource) args.BaseImage;
        child2.RenderTransform = (Transform) new ScaleTransform()
        {
          ScaleX = (48.0 / args.Width / EmojiRow.ScaleFactor),
          ScaleY = (48.0 / args.Height / EmojiRow.ScaleFactor)
        };
        child2.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        child2.Width = args.Width;
        child2.Height = args.Height;
      }));
    }

    private void ErrorCleanup()
    {
      if (EmojiRow.CurrentRowContext != null && EmojiRow.CurrentRowContext.viewmodel != null && EmojiRow.CurrentRowContext.viewmodel.PopupSelector != null)
        EmojiRow.CurrentRowContext.viewmodel.PopupSelector.IsOpen = false;
      EmojiRow.Emoji_ManipulationCleanup();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/EmojiRow.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
    }

    private enum ManipulationState
    {
      None,
      HeldAndNoDelta,
      Routing,
      HeldAndSelecting,
      Swipping,
    }
  }
}
