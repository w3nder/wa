// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.ZoomBar
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [TemplatePart(Name = "ZoomIn", Type = typeof (Button))]
  [TemplatePart(Name = "ZoomOut", Type = typeof (Button))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class ZoomBar : Overlay
  {
    internal const string ZoomInElementName = "ZoomIn";
    internal const string ZoomOutElementName = "ZoomOut";
    private Button zoomIn;
    private Button zoomOut;

    public event EventHandler<MapCommandEventArgs> ZoomMap;

    public ZoomBar()
    {
      this.DefaultStyleKey = (object) typeof (ZoomBar);
      this.zoomIn = new Button();
      this.zoomOut = new Button();
    }

    ~ZoomBar() => this.ZoomMap = (EventHandler<MapCommandEventArgs>) null;

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.zoomIn = this.GetTemplateChild("ZoomIn") as Button;
      this.zoomOut = this.GetTemplateChild("ZoomOut") as Button;
      if (this.zoomIn != null)
        this.zoomIn.Click += new RoutedEventHandler(this.ZoomIn_Click);
      if (this.zoomOut != null)
        this.zoomOut.Click += new RoutedEventHandler(this.ZoomOut_Click);
      this.FireTemplateApplied();
    }

    private void ZoomIn_Click(object sender, RoutedEventArgs e)
    {
      this.OnZoomMap(new ZoomMapCommand(true));
    }

    private void ZoomOut_Click(object sender, RoutedEventArgs e)
    {
      this.OnZoomMap(new ZoomMapCommand(false));
    }

    protected virtual void OnZoomMap(ZoomMapCommand command)
    {
      EventHandler<MapCommandEventArgs> zoomMap = this.ZoomMap;
      if (command == null || zoomMap == null)
        return;
      zoomMap((object) this, new MapCommandEventArgs((MapCommandBase) command));
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (ZoomBar));
    }
  }
}
