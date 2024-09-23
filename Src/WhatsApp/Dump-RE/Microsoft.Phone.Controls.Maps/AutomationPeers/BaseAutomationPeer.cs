// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AutomationPeers.BaseAutomationPeer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.AutomationPeers
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class BaseAutomationPeer : FrameworkElementAutomationPeer
  {
    private readonly string className;

    public BaseAutomationPeer(FrameworkElement element, string className)
      : base(element)
    {
      this.className = className;
    }

    protected override string GetClassNameCore() => this.className;

    protected override string GetNameCore() => this.className;

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Custom;
    }
  }
}
