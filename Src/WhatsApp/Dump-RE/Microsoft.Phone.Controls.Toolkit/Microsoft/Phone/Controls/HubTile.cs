// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.HubTile
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(Name = "Expanded", GroupName = "ImageState")]
  [TemplatePart(Name = "MessageBlock", Type = typeof (TextBlock))]
  [TemplatePart(Name = "TitlePanel", Type = typeof (Panel))]
  [TemplateVisualState(Name = "Semiexpanded", GroupName = "ImageState")]
  [TemplateVisualState(Name = "Collapsed", GroupName = "ImageState")]
  [TemplateVisualState(Name = "Flipped", GroupName = "ImageState")]
  [TemplatePart(Name = "NotificationBlock", Type = typeof (TextBlock))]
  [TemplatePart(Name = "BackTitleBlock", Type = typeof (TextBlock))]
  public class HubTile : Control
  {
    private const string ImageStates = "ImageState";
    private const string Expanded = "Expanded";
    private const string Semiexpanded = "Semiexpanded";
    private const string Collapsed = "Collapsed";
    private const string Flipped = "Flipped";
    private const string NotificationBlock = "NotificationBlock";
    private const string MessageBlock = "MessageBlock";
    private const string BackTitleBlock = "BackTitleBlock";
    private const string TitlePanel = "TitlePanel";
    private TextBlock _notificationBlock;
    private TextBlock _messageBlock;
    private Panel _titlePanel;
    private TextBlock _backTitleBlock;
    internal int _stallingCounter;
    internal bool _canDrop;
    internal bool _canFlip;
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof (Source), typeof (ImageSource), typeof (HubTile), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (HubTile), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(HubTile.OnTitleChanged)));
    public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register(nameof (Notification), typeof (string), typeof (HubTile), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(HubTile.OnBackContentChanged)));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof (Message), typeof (string), typeof (HubTile), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(HubTile.OnBackContentChanged)));
    public static readonly DependencyProperty DisplayNotificationProperty = DependencyProperty.Register(nameof (DisplayNotification), typeof (bool), typeof (HubTile), new PropertyMetadata((object) false, new PropertyChangedCallback(HubTile.OnBackContentChanged)));
    public static readonly DependencyProperty IsFrozenProperty = DependencyProperty.Register(nameof (IsFrozen), typeof (bool), typeof (HubTile), new PropertyMetadata((object) false, new PropertyChangedCallback(HubTile.OnIsFrozenChanged)));
    public static readonly DependencyProperty GroupTagProperty = DependencyProperty.Register(nameof (GroupTag), typeof (string), typeof (HubTile), new PropertyMetadata((object) string.Empty));
    private static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof (State), typeof (ImageState), typeof (HubTile), new PropertyMetadata((object) ImageState.Expanded, new PropertyChangedCallback(HubTile.OnImageStateChanged)));
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof (Size), typeof (TileSize), typeof (HubTile), new PropertyMetadata((object) TileSize.Default, new PropertyChangedCallback(HubTile.OnSizeChanged)));

    public ImageSource Source
    {
      get => (ImageSource) this.GetValue(HubTile.SourceProperty);
      set => this.SetValue(HubTile.SourceProperty, (object) value);
    }

    public string Title
    {
      get => (string) this.GetValue(HubTile.TitleProperty);
      set => this.SetValue(HubTile.TitleProperty, (object) value);
    }

    private static void OnTitleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      HubTile hubTile = (HubTile) obj;
      if (string.IsNullOrEmpty((string) e.NewValue))
      {
        hubTile._canDrop = false;
        hubTile.State = ImageState.Expanded;
      }
      else
        hubTile._canDrop = true;
    }

    public string Notification
    {
      get => (string) this.GetValue(HubTile.NotificationProperty);
      set => this.SetValue(HubTile.NotificationProperty, (object) value);
    }

    private static void OnBackContentChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      HubTile hubTile = (HubTile) obj;
      if (!string.IsNullOrEmpty(hubTile.Notification) && hubTile.DisplayNotification || !string.IsNullOrEmpty(hubTile.Message) && !hubTile.DisplayNotification)
      {
        hubTile._canFlip = true;
      }
      else
      {
        hubTile._canFlip = false;
        hubTile.State = ImageState.Expanded;
      }
    }

    public string Message
    {
      get => (string) this.GetValue(HubTile.MessageProperty);
      set => this.SetValue(HubTile.MessageProperty, (object) value);
    }

    public bool DisplayNotification
    {
      get => (bool) this.GetValue(HubTile.DisplayNotificationProperty);
      set => this.SetValue(HubTile.DisplayNotificationProperty, (object) value);
    }

    public bool IsFrozen
    {
      get => (bool) this.GetValue(HubTile.IsFrozenProperty);
      set => this.SetValue(HubTile.IsFrozenProperty, (object) value);
    }

    private static void OnIsFrozenChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      HubTile tile = (HubTile) obj;
      if ((bool) e.NewValue)
        HubTileService.FreezeHubTile(tile);
      else
        HubTileService.UnfreezeHubTile(tile);
    }

    public string GroupTag
    {
      get => (string) this.GetValue(HubTile.GroupTagProperty);
      set => this.SetValue(HubTile.GroupTagProperty, (object) value);
    }

    internal ImageState State
    {
      get => (ImageState) this.GetValue(HubTile.StateProperty);
      set => this.SetValue(HubTile.StateProperty, (object) value);
    }

    private static void OnImageStateChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((HubTile) obj).UpdateVisualState();
    }

    public TileSize Size
    {
      get => (TileSize) this.GetValue(HubTile.SizeProperty);
      set => this.SetValue(HubTile.SizeProperty, (object) value);
    }

    private static void OnSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      HubTile tile = (HubTile) obj;
      switch (tile.Size)
      {
        case TileSize.Default:
          tile.Width = 173.0;
          tile.Height = 173.0;
          break;
        case TileSize.Small:
          tile.Width = 99.0;
          tile.Height = 99.0;
          break;
        case TileSize.Medium:
          tile.Width = 210.0;
          tile.Height = 210.0;
          break;
        case TileSize.Large:
          tile.Width = 432.0;
          tile.Height = 210.0;
          break;
      }
      tile.SizeChanged += new SizeChangedEventHandler(HubTile.OnHubTileSizeChanged);
      HubTileService.FinalizeReference(tile);
    }

    private static void OnHubTileSizeChanged(object sender, SizeChangedEventArgs e)
    {
      HubTile tile = (HubTile) sender;
      tile.SizeChanged -= new SizeChangedEventHandler(HubTile.OnHubTileSizeChanged);
      if (tile.State != ImageState.Expanded)
      {
        tile.State = ImageState.Expanded;
        VisualStateManager.GoToState((Control) tile, "Expanded", false);
      }
      else if (tile._titlePanel != null && tile._titlePanel.RenderTransform is CompositeTransform renderTransform)
        renderTransform.TranslateY = -tile.Height;
      HubTileService.InitializeReference(tile);
    }

    private void UpdateVisualState()
    {
      string stateName;
      if (this.Size != TileSize.Small)
      {
        switch (this.State)
        {
          case ImageState.Expanded:
            stateName = "Expanded";
            break;
          case ImageState.Semiexpanded:
            stateName = "Semiexpanded";
            break;
          case ImageState.Collapsed:
            stateName = "Collapsed";
            break;
          case ImageState.Flipped:
            stateName = "Flipped";
            break;
          default:
            stateName = "Expanded";
            break;
        }
      }
      else
        stateName = "Expanded";
      VisualStateManager.GoToState((Control) this, stateName, true);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this._notificationBlock = this.GetTemplateChild("NotificationBlock") as TextBlock;
      this._messageBlock = this.GetTemplateChild("MessageBlock") as TextBlock;
      this._backTitleBlock = this.GetTemplateChild("BackTitleBlock") as TextBlock;
      this._titlePanel = this.GetTemplateChild("TitlePanel") as Panel;
      if (this._notificationBlock != null)
        this._notificationBlock.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Source = (object) this,
          Path = new PropertyPath("DisplayNotification", new object[0]),
          Converter = (IValueConverter) new VisibilityConverter(),
          ConverterParameter = (object) false
        });
      if (this._messageBlock != null)
        this._messageBlock.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Source = (object) this,
          Path = new PropertyPath("DisplayNotification", new object[0]),
          Converter = (IValueConverter) new VisibilityConverter(),
          ConverterParameter = (object) true
        });
      if (this._backTitleBlock != null)
        this._backTitleBlock.SetBinding(TextBlock.TextProperty, new Binding()
        {
          Source = (object) this,
          Path = new PropertyPath("Title", new object[0]),
          Converter = (IValueConverter) new MultipleToSingleLineStringConverter()
        });
      this.UpdateVisualState();
    }

    public HubTile()
    {
      this.DefaultStyleKey = (object) typeof (HubTile);
      this.Loaded += new RoutedEventHandler(this.HubTile_Loaded);
      this.Unloaded += new RoutedEventHandler(this.HubTile_Unloaded);
    }

    private void HubTile_Loaded(object sender, RoutedEventArgs e)
    {
      HubTileService.InitializeReference(this);
    }

    private void HubTile_Unloaded(object sender, RoutedEventArgs e)
    {
      HubTileService.FinalizeReference(this);
    }
  }
}
