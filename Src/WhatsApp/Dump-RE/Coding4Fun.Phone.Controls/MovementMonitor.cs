// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.MovementMonitor
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class MovementMonitor
  {
    protected Rectangle Monitor;
    private double _xOffsetStartValue;
    private double _yOffsetStartValue;

    public event EventHandler<MovementMonitorEventArgs> Movement;

    public void MonitorControl(Panel panel)
    {
      Rectangle rectangle = new Rectangle();
      ((Shape) rectangle).Fill = (Brush) new SolidColorBrush(Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0));
      this.Monitor = rectangle;
      ((DependencyObject) this.Monitor).SetValue(Grid.RowSpanProperty, (object) 2147483646);
      ((DependencyObject) this.Monitor).SetValue(Grid.ColumnSpanProperty, (object) 2147483646);
      ((UIElement) this.Monitor).ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.MonitorManipulationStarted);
      ((UIElement) this.Monitor).ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.MonitorManipulationDelta);
      ((PresentationFrameworkCollection<UIElement>) panel.Children).Add((UIElement) this.Monitor);
    }

    private void MonitorManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.Movement != null)
        this.Movement((object) this, new MovementMonitorEventArgs()
        {
          X = this._xOffsetStartValue + e.CumulativeManipulation.Translation.X,
          Y = this._yOffsetStartValue + e.CumulativeManipulation.Translation.Y
        });
      e.Handled = true;
    }

    private void MonitorManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this._xOffsetStartValue = e.ManipulationOrigin.X;
      this._yOffsetStartValue = e.ManipulationOrigin.Y;
      if (this.Movement != null)
        this.Movement((object) this, new MovementMonitorEventArgs()
        {
          X = this._xOffsetStartValue,
          Y = this._yOffsetStartValue
        });
      e.Handled = true;
    }
  }
}
