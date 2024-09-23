// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.Logo
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using System;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class Logo : Overlay
  {
    public Logo() => this.DefaultStyleKey = (object) typeof (Logo);

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.FireTemplateApplied();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (Logo));
    }
  }
}
