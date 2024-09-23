// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TransferControl
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.LocalizedResources;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "TransferControl", Type = typeof (TransferControl))]
  [TemplateVisualState(Name = "NoProgressBar", GroupName = "ControlStates")]
  [TemplateVisualState(Name = "Hidden", GroupName = "ControlStates")]
  [TemplateVisualState(Name = "Default", GroupName = "ControlStates")]
  public class TransferControl : ContentControl
  {
    private const string ControlPart = "TransferControl";
    private const bool UseTransitions = false;
    private const string ControlStates = "ControlStates";
    private const string DefaultState = "Default";
    private const string NoProgressBarState = "NoProgressBar";
    private const string HiddenState = "Hidden";
    private static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(nameof (AutoHide), typeof (bool), typeof (TransferControl), new PropertyMetadata((object) false));
    private static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (TransferControl), new PropertyMetadata((PropertyChangedCallback) null));
    private static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (TransferControl), new PropertyMetadata((object) null));
    private static readonly DependencyProperty MonitorProperty = DependencyProperty.Register(nameof (Monitor), typeof (TransferMonitor), typeof (TransferControl), new PropertyMetadata((object) null, new PropertyChangedCallback(TransferControl.UpdateMonitor)));
    private static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof (Icon), typeof (Uri), typeof (TransferControl), new PropertyMetadata((object) null));
    private static readonly DependencyProperty IsContextMenuEnabledProperty = DependencyProperty.Register(nameof (IsContextMenuEnabled), typeof (bool), typeof (TransferControl), new PropertyMetadata((object) true));
    private static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register(nameof (ProgressBarStyle), typeof (Style), typeof (TransferControl), new PropertyMetadata((object) null));
    private static readonly DependencyProperty StatusTextBrushProperty = DependencyProperty.Register(nameof (StatusTextBrush), typeof (Brush), typeof (TransferControl), new PropertyMetadata(Application.Current.Resources[(object) "PhoneForegroundBrush"]));

    public bool AutoHide
    {
      get => (bool) this.GetValue(TransferControl.AutoHideProperty);
      set => this.SetValue(TransferControl.AutoHideProperty, (object) value);
    }

    public object Header
    {
      get => (object) (string) this.GetValue(TransferControl.HeaderProperty);
      set => this.SetValue(TransferControl.HeaderProperty, value);
    }

    public DataTemplate HeaderTemplate
    {
      get => (DataTemplate) this.GetValue(TransferControl.HeaderTemplateProperty);
      set => this.SetValue(TransferControl.HeaderTemplateProperty, (object) value);
    }

    public TransferMonitor Monitor
    {
      get => (TransferMonitor) this.GetValue(TransferControl.MonitorProperty);
      set => this.SetValue(TransferControl.MonitorProperty, (object) value);
    }

    public Uri Icon
    {
      get => (Uri) this.GetValue(TransferControl.IconProperty);
      set => this.SetValue(TransferControl.IconProperty, (object) value);
    }

    public bool IsContextMenuEnabled
    {
      get => (bool) this.GetValue(TransferControl.IsContextMenuEnabledProperty);
      set => this.SetValue(TransferControl.IsContextMenuEnabledProperty, (object) value);
    }

    public Style ProgressBarStyle
    {
      get => (Style) this.GetValue(TransferControl.ProgressBarStyleProperty);
      set => this.SetValue(TransferControl.ProgressBarStyleProperty, (object) value);
    }

    public Brush StatusTextBrush
    {
      get => (Brush) this.GetValue(TransferControl.StatusTextBrushProperty);
      set => this.SetValue(TransferControl.StatusTextBrushProperty, (object) value);
    }

    public TransferControl()
    {
      this.DefaultStyleKey = (object) typeof (TransferControl);
      this.UpdateState((object) this, new PropertyChangedEventArgs("State"));
    }

    private static void UpdateMonitor(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is TransferControl transferControl))
        return;
      transferControl.UpdateMonitor(e);
    }

    private void UpdateMonitor(DependencyPropertyChangedEventArgs e)
    {
      if (!(e.NewValue is TransferMonitor newValue))
        return;
      newValue.PropertyChanged -= new PropertyChangedEventHandler(this.UpdateState);
      newValue.PropertyChanged += new PropertyChangedEventHandler(this.UpdateState);
      if (this.Header == null && newValue.Name != null)
        this.Header = (object) newValue.Name;
      this.UpdateState((object) this, new PropertyChangedEventArgs("State"));
    }

    private void UpdateState(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName != "State" || this.Monitor == null)
        return;
      string stateName;
      switch (this.Monitor.State)
      {
        case TransferRequestState.Pending:
        case TransferRequestState.Waiting:
          this.IsContextMenuEnabled = true;
          stateName = "NoProgressBar";
          break;
        case TransferRequestState.Downloading:
        case TransferRequestState.Uploading:
        case TransferRequestState.Paused:
          this.IsContextMenuEnabled = true;
          stateName = "Default";
          break;
        case TransferRequestState.Complete:
        case TransferRequestState.Unknown:
          this.IsContextMenuEnabled = false;
          stateName = this.AutoHide ? "Hidden" : "NoProgressBar";
          break;
        case TransferRequestState.Failed:
          this.IsContextMenuEnabled = false;
          stateName = "NoProgressBar";
          break;
        default:
          return;
      }
      VisualStateManager.GoToState((Control) this, stateName, false);
    }

    public override void OnApplyTemplate()
    {
      if (this.GetTemplateChild("ContextMenuCancel") is MenuItem templateChild)
      {
        templateChild.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, args) =>
        {
          if (this.Monitor == null)
            return;
          this.Monitor.RequestCancel();
        });
        templateChild.Header = (object) ControlResources.Cancel;
      }
      base.OnApplyTemplate();
    }
  }
}
