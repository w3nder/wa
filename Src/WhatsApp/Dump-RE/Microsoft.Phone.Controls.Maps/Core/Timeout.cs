// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.Timeout
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class Timeout
  {
    private readonly Action action;
    private readonly DispatcherTimer timer = new DispatcherTimer();

    public Timeout(Action action, long timeout)
    {
      this.action = action;
      this.timer.Tick += new EventHandler(this.OnTimeout);
      this.timer.Interval = TimeSpan.FromTicks(timeout);
      this.timer.Start();
    }

    private void OnTimeout(object sender, EventArgs e)
    {
      this.action();
      this.timer.Tick -= new EventHandler(this.OnTimeout);
      this.timer.Stop();
    }

    public void DoItNow()
    {
      if (!this.timer.IsEnabled)
        return;
      this.OnTimeout((object) this, (EventArgs) null);
    }

    public void Cancel()
    {
      this.timer.Tick -= new EventHandler(this.OnTimeout);
      if (!this.timer.IsEnabled)
        return;
      this.timer.Stop();
    }
  }
}
