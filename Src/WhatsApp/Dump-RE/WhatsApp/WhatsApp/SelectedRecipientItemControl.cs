// Decompiled with JetBrains decompiler
// Type: WhatsApp.SelectedRecipientItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class SelectedRecipientItemControl : Grid
  {
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof (ViewModel), typeof (JidItemViewModel), typeof (SelectedRecipientItemControl), new PropertyMetadata(new PropertyChangedCallback(SelectedRecipientItemControl.OnViewModelChanged)));
    protected const int DefaultIconSize = 68;
    protected int? iconSize;
    private const double RemoveButtonSize = 28.0;
    private Grid iconPanel;
    private Image icon;
    private TextBlock nameBlock;
    private Border removePanel;
    private IDisposable iconSub;
    private bool shouldPopIn;

    public event EventHandler RemoveClicked;

    protected void NotifyRemoveClicked()
    {
      if (this.RemoveClicked == null)
        return;
      this.RemoveClicked((object) this, new EventArgs());
    }

    public JidItemViewModel ViewModel
    {
      get => this.GetValue(SelectedRecipientItemControl.ViewModelProperty) as JidItemViewModel;
      set => this.SetValue(SelectedRecipientItemControl.ViewModelProperty, (object) value);
    }

    public virtual int IconSize
    {
      get => this.iconSize ?? 68;
      set
      {
        this.iconSize = new int?(value);
        this.OnIconSizeChanged();
      }
    }

    public SelectedRecipientItemControl() => this.InitComponents();

    public static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is SelectedRecipientItemControl recipientItemControl))
        return;
      recipientItemControl.RefreshContent(e.OldValue as JidItemViewModel, e.NewValue as JidItemViewModel);
    }

    public void RefreshContent(JidItemViewModel oldVm, JidItemViewModel newVm)
    {
      if (newVm == null)
        return;
      this.UpdateComponents(newVm);
    }

    protected virtual void InitComponents()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      int iconSize = this.IconSize;
      this.MaxWidth = (double) iconSize;
      double num = (double) iconSize * 0.5;
      Grid grid1 = new Grid();
      grid1.VerticalAlignment = VerticalAlignment.Top;
      grid1.Width = (double) iconSize;
      grid1.Height = (double) iconSize;
      grid1.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      grid1.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
      grid1.IsHitTestVisible = false;
      this.iconPanel = grid1;
      this.Children.Add((UIElement) this.iconPanel);
      Grid.SetRow((FrameworkElement) this.iconPanel, 0);
      Image image1 = new Image();
      image1.Stretch = Stretch.UniformToFill;
      image1.HorizontalAlignment = HorizontalAlignment.Center;
      image1.VerticalAlignment = VerticalAlignment.Center;
      image1.IsHitTestVisible = false;
      this.icon = image1;
      this.iconPanel.Children.Add((UIElement) this.icon);
      Border border = new Border();
      border.Background = UIUtils.SubtleBrushGray;
      border.VerticalAlignment = VerticalAlignment.Top;
      border.HorizontalAlignment = HorizontalAlignment.Right;
      border.Width = 28.0;
      border.Height = 28.0;
      border.CornerRadius = new CornerRadius(14.0);
      border.RenderTransform = (Transform) new TranslateTransform()
      {
        X = 6.0
      };
      border.IsHitTestVisible = false;
      this.removePanel = border;
      this.Children.Add((UIElement) this.removePanel);
      Grid.SetRow((FrameworkElement) this.removePanel, 0);
      Image image2 = new Image();
      image2.Source = (System.Windows.Media.ImageSource) AssetStore.DismissIconWhite;
      image2.VerticalAlignment = VerticalAlignment.Top;
      image2.HorizontalAlignment = HorizontalAlignment.Right;
      image2.Width = 28.0;
      image2.Height = 28.0;
      this.removePanel.Child = (UIElement) image2;
      Grid grid2 = new Grid();
      grid2.Background = (Brush) UIUtils.TransparentBrush;
      grid2.VerticalAlignment = VerticalAlignment.Top;
      grid2.HorizontalAlignment = HorizontalAlignment.Right;
      grid2.Height = (double) iconSize * 0.7;
      grid2.Width = (double) iconSize * 0.7;
      grid2.RenderTransform = (Transform) new TranslateTransform()
      {
        X = 6.0
      };
      Grid element1 = grid2;
      element1.Tap += new EventHandler<GestureEventArgs>(this.RemovePanel_Tap);
      this.Children.Add((UIElement) element1);
      Grid.SetRow((FrameworkElement) element1, 0);
      Grid grid3 = new Grid();
      grid3.Margin = new Thickness(0.0, 8.0, 0.0, 0.0);
      grid3.HorizontalAlignment = HorizontalAlignment.Stretch;
      grid3.IsHitTestVisible = false;
      Grid element2 = grid3;
      this.Children.Add((UIElement) element2);
      Grid.SetRow((FrameworkElement) element2, 1);
      TextBlock textBlock = new TextBlock();
      textBlock.HorizontalAlignment = HorizontalAlignment.Left;
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.FontSize = 18.0;
      this.nameBlock = textBlock;
      element2.Children.Add((UIElement) this.nameBlock);
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Stretch = Stretch.Fill;
      rectangle1.HorizontalAlignment = HorizontalAlignment.Right;
      rectangle1.VerticalAlignment = VerticalAlignment.Stretch;
      rectangle1.Width = 12.0;
      rectangle1.Fill = (Brush) UIUtils.CreateFadingGradientBrush(UIUtils.BackgroundBrush.Color, new System.Windows.Point(1.0, 0.0), new System.Windows.Point(0.0, 0.0));
      Rectangle rectangle2 = rectangle1;
      element2.Children.Add((UIElement) rectangle2);
    }

    protected virtual void UpdateComponents(JidItemViewModel vm)
    {
      this.icon.Source = (System.Windows.Media.ImageSource) null;
      bool iconSet = false;
      this.iconSub.SafeDispose();
      this.iconSub = vm.GetPictureSourceObservable(true, false).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (iconSrc =>
      {
        this.icon.Source = iconSrc ?? vm.GetDefaultPicture();
        iconSet = true;
      }), (Action) (() =>
      {
        if (!iconSet)
          this.icon.Source = vm.GetDefaultPicture();
        this.iconSub = (IDisposable) null;
      }));
      string titleStr = vm.TitleStr;
      this.nameBlock.Text = !string.IsNullOrEmpty(titleStr) ? titleStr : JidHelper.GetShortDisplayNameForContactJid(vm.Jid);
    }

    protected void OnIconSizeChanged()
    {
      if (this.iconPanel == null)
        return;
      int iconSize = this.IconSize;
      double num = (double) iconSize * 0.5;
      this.iconPanel.Width = this.iconPanel.Height = (double) iconSize;
      this.iconPanel.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
    }

    private void RemovePanel_Tap(object sender, GestureEventArgs e) => this.NotifyRemoveClicked();
  }
}
