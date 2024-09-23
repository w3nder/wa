// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.MemoryCounter
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Microsoft.Phone.Info;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class MemoryCounter : Control
  {
    private const float ByteToMega = 1048576f;
    private readonly DispatcherTimer _timer;
    private bool _threwException;
    public static readonly DependencyProperty UpdateIntervalProperty = DependencyProperty.Register(nameof (UpdateInterval), typeof (int), typeof (MemoryCounter), new PropertyMetadata((object) 1000, new PropertyChangedCallback(MemoryCounter.OnUpdateIntervalChanged)));
    public static readonly DependencyProperty CurrentMemoryProperty = DependencyProperty.Register(nameof (CurrentMemory), typeof (string), typeof (MemoryCounter), new PropertyMetadata((object) "0"));
    public static readonly DependencyProperty PeakMemoryProperty = DependencyProperty.Register(nameof (PeakMemory), typeof (string), typeof (MemoryCounter), new PropertyMetadata((object) "0"));

    public MemoryCounter()
    {
      if (Debugger.IsAttached)
      {
        this.DefaultStyleKey = (object) typeof (MemoryCounter);
        ((FrameworkElement) this).DataContext = (object) this;
        this._timer = new DispatcherTimer()
        {
          Interval = TimeSpan.FromMilliseconds((double) this.UpdateInterval)
        };
        this._timer.Tick += new EventHandler(this.timer_Tick);
        this._timer.Start();
      }
      else
        ((UIElement) this).Visibility = (Visibility) 1;
    }

    public int UpdateInterval
    {
      get => (int) ((DependencyObject) this).GetValue(MemoryCounter.UpdateIntervalProperty);
      set
      {
        ((DependencyObject) this).SetValue(MemoryCounter.UpdateIntervalProperty, (object) value);
      }
    }

    private static void OnUpdateIntervalChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      if (!Debugger.IsAttached)
        return;
      MemoryCounter memoryCounter = (MemoryCounter) o;
      if (memoryCounter == null || memoryCounter._timer == null)
        return;
      memoryCounter._timer.Interval = TimeSpan.FromMilliseconds((double) (int) e.NewValue);
    }

    public string CurrentMemory
    {
      get => (string) ((DependencyObject) this).GetValue(MemoryCounter.CurrentMemoryProperty);
      set
      {
        ((DependencyObject) this).SetValue(MemoryCounter.CurrentMemoryProperty, (object) value);
      }
    }

    public string PeakMemory
    {
      get => (string) ((DependencyObject) this).GetValue(MemoryCounter.PeakMemoryProperty);
      set => ((DependencyObject) this).SetValue(MemoryCounter.PeakMemoryProperty, (object) value);
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      if (!Debugger.IsAttached)
      {
        if (!this._threwException)
          return;
      }
      try
      {
        this.CurrentMemory = ((float) DeviceStatus.ApplicationCurrentMemoryUsage / 1048576f).ToString("#.00");
        this.PeakMemory = ((float) DeviceStatus.ApplicationPeakMemoryUsage / 1048576f).ToString("#.00");
      }
      catch (Exception ex)
      {
        this._threwException = true;
        this._timer.Stop();
      }
    }
  }
}
