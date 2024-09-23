// Decompiled with JetBrains decompiler
// Type: WhatsApp.CompatibilityShims.LongListSelector
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp.CompatibilityShims
{
  public class LongListSelector : Microsoft.Phone.Controls.LongListSelector
  {
    private bool overlapScrollBar_;
    private ScrollBar verticalScrollBar;
    private ViewportControl viewportControl;
    private Storyboard prettyStoryboard;
    private DoubleAnimation prettyAnimation;
    private bool preventLayoutCycle;
    private bool viewportEventAttached;
    private double viewportOffsetAtCycle;
    private Dictionary<object, WeakReference<DependencyObject>> realizedItems = new Dictionary<object, WeakReference<DependencyObject>>();

    public LongListSelector()
    {
      this.IsFlatList = false;
      this.LayoutUpdated += new EventHandler(this.LongListSelector_LayoutUpdated);
      this.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.LongListSelector_ItemRealized);
    }

    public bool IsFlatList
    {
      get => !this.IsGroupingEnabled;
      set
      {
        bool flag = !value;
        if (this.IsGroupingEnabled == flag)
          return;
        if (flag)
        {
          this.GroupHeaderTemplate = XamlReader.Load("\r\n                            <DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                          xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\r\n                                <StackPanel HorizontalAlignment=\"Left\">\r\n                                    <Border Margin=\"{StaticResource PhoneTouchTargetOverhang}\" Background=\"{StaticResource PhoneAccentBrush}\" Width=\"80\" Height=\"80\">\r\n                                        <TextBlock Text=\"{Binding Key}\" Style=\"{StaticResource PhoneTextExtraLargeStyle}\" Foreground=\"White\" TextOptions.DisplayColorEmoji=\"False\"/>\r\n                                    </Border>\r\n                                </StackPanel>\r\n                            </DataTemplate>") as DataTemplate;
          this.JumpListStyle = XamlReader.Load("\r\n                            <Style xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                   xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                                   xmlns:phone=\"clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone\"\r\n                                   xmlns:toolkit=\"clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit\"\r\n                                   TargetType=\"phone:LongListSelector\">\r\n                                <Setter Property=\"GridCellSize\" Value=\"111,111\"/>\r\n                                <Setter Property=\"LayoutMode\" Value=\"Grid\"/>\r\n                                <Setter Property=\"Margin\" Value=\"18,12,0,0\"/>\r\n                                <Setter Property=\"ItemTemplate\">\r\n                                    <Setter.Value>\r\n                                        <DataTemplate>\r\n                                            <Button Style=\"{StaticResource BorderlessButton}\"\r\n                                                    toolkit:TiltEffect.IsTiltEnabled=\"True\"\r\n                                                    Margin=\"6\"\r\n                                                    Background=\"{StaticResource PhoneAccentBrush}\"\r\n                                                    HorizontalContentAlignment=\"Left\"\r\n                                                    VerticalContentAlignment=\"Bottom\"\r\n                                                    Padding=\"11,0,0,0\">\r\n                                                <Button.Content>\r\n                                                    <TextBlock Text=\"{Binding Key}\" Foreground=\"White\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Bottom\" FontSize=\"48\" FontFamily=\"{StaticResource PhoneFontFamilySemiBold}\" TextOptions.DisplayColorEmoji=\"False\"/>\r\n                                                </Button.Content>\r\n                                            </Button>\r\n                                        </DataTemplate>\r\n                                    </Setter.Value>\r\n                                </Setter>\r\n                            </Style>") as Style;
        }
        else
        {
          this.JumpListStyle = (Style) null;
          this.GroupHeaderTemplate = (DataTemplate) null;
        }
        this.IsGroupingEnabled = flag;
      }
    }

    public bool OverlapScrollBar
    {
      set
      {
        this.overlapScrollBar_ = value;
        if (this.verticalScrollBar == null)
          return;
        this.OverlapScrollBarImpl();
      }
    }

    public bool IsScrollRequired
    {
      get
      {
        return this.viewportControl != null && this.viewportControl.Bounds.Height > this.viewportControl.ActualHeight;
      }
    }

    private void OverlapScrollBarImpl()
    {
      if (this.verticalScrollBar == null)
        return;
      Grid.SetColumn((FrameworkElement) this.verticalScrollBar, 0);
      this.verticalScrollBar.HorizontalAlignment = HorizontalAlignment.Right;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.verticalScrollBar = this.GetTemplateChild("VerticalScrollBar") as ScrollBar;
      this.viewportControl = this.GetTemplateChild("ViewportControl") as ViewportControl;
      if (!this.overlapScrollBar_)
        return;
      this.OverlapScrollBarImpl();
    }

    public double ViewportFinalY
    {
      get
      {
        return this.prettyStoryboard != null && this.prettyStoryboard.GetCurrentState() == ClockState.Active ? this.prettyAnimation.To.GetValueOrDefault() : this.viewportControl.Viewport.Y;
      }
    }

    public void ScrollToPretty(double yOffset)
    {
      if (this.ManipulationState != System.Windows.Controls.Primitives.ManipulationState.Idle)
        return;
      this.viewportControl.SetViewportOrigin(new System.Windows.Point(this.viewportControl.Viewport.X, yOffset));
    }

    private void prettyStoryboard_Completed(object sender, EventArgs e)
    {
      this.prettyStoryboard.Pause();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (!this.preventLayoutCycle)
        return base.MeasureOverride(availableSize);
      this.viewportOffsetAtCycle = this.viewportControl.Viewport.Top;
      if (!this.viewportEventAttached)
      {
        this.viewportControl.ViewportChanged += new EventHandler<ViewportChangedEventArgs>(this.ViewportControl_ViewportChanged);
        this.viewportEventAttached = true;
      }
      return availableSize;
    }

    private void ViewportControl_ViewportChanged(object sender, ViewportChangedEventArgs e)
    {
      if (Math.Abs(this.viewportControl.Viewport.Top - this.viewportOffsetAtCycle) <= 250.0)
        return;
      this.InvalidateMeasure();
      if (!this.viewportEventAttached)
        return;
      this.viewportControl.ViewportChanged -= new EventHandler<ViewportChangedEventArgs>(this.ViewportControl_ViewportChanged);
      this.viewportEventAttached = false;
    }

    private void LongListSelector_LayoutUpdated(object sender, EventArgs e)
    {
      this.preventLayoutCycle = false;
      this.realizedItems.Clear();
    }

    private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind != LongListSelectorItemKind.Item || this.preventLayoutCycle)
        return;
      object content = e.Container.Content;
      DependencyObject child = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0);
      DependencyObject target = (DependencyObject) null;
      if (this.realizedItems.ContainsKey(content))
        this.realizedItems[content].TryGetTarget(out target);
      if (target != null && target != child)
        this.preventLayoutCycle = true;
      this.realizedItems[content] = new WeakReference<DependencyObject>(child);
    }
  }
}
