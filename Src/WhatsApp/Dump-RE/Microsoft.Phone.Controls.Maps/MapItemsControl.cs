// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.MapItemsControl
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapItemsControl : ItemsControl, IProjectable
  {
    private ItemsPresenter ip;
    private MapLayer ml;

    public MapItemsControl() => this.DefaultStyleKey = (object) typeof (MapItemsControl);

    public MapBase ParentMap
    {
      get => this.Parent is IProjectable parent ? parent.ParentMap : this.Parent as MapBase;
    }

    public void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
      if (updateLevel == ProjectionUpdateLevel.None)
        return;
      this.InvalidateArrange();
      this.InvalidateMeasure();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (VisualTreeHelper.GetChildrenCount((DependencyObject) this) <= 0)
        return;
      this.ip = VisualTreeHelper.GetChild((DependencyObject) this, 0) as ItemsPresenter;
      if (this.ip == null)
        return;
      this.ip.LayoutUpdated += new EventHandler(this.ip_LayoutUpdated);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.ml != null)
        this.ml.InvalidateArrange();
      return base.ArrangeOverride(finalSize);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.ml != null)
        this.ml.InvalidateMeasure();
      return base.MeasureOverride(availableSize);
    }

    private void ip_LayoutUpdated(object sender, EventArgs e)
    {
      this.ip.LayoutUpdated -= new EventHandler(this.ip_LayoutUpdated);
      if (VisualTreeHelper.GetChildrenCount((DependencyObject) this.ip) <= 0)
        return;
      this.ml = VisualTreeHelper.GetChild((DependencyObject) this.ip, 0) as MapLayer;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (MapItemsControl));
    }
  }
}
