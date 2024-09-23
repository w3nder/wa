// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ToastPrompt
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Clarity.Phone.Extensions;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ToastPrompt : PopUp<string, PopUpResult>
  {
    private const string ToastImageName = "ToastImage";
    protected Image ToastImage;
    private readonly Timer _timer;
    private TranslateTransform _translate;
    public static readonly DependencyProperty MillisecondsUntilHiddenProperty = DependencyProperty.Register(nameof (MillisecondsUntilHidden), typeof (int), typeof (ToastPrompt), new PropertyMetadata((object) 4000));
    public static readonly DependencyProperty IsTimerEnabledProperty = DependencyProperty.Register(nameof (IsTimerEnabled), typeof (bool), typeof (ToastPrompt), new PropertyMetadata((object) true));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (ToastPrompt), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof (Message), typeof (string), typeof (ToastPrompt), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof (ImageSource), typeof (ImageSource), typeof (ToastPrompt), new PropertyMetadata(new PropertyChangedCallback(ToastPrompt.OnImageSource)));
    public static readonly DependencyProperty TextOrientationProperty = DependencyProperty.Register(nameof (TextOrientation), typeof (Orientation), typeof (ToastPrompt), new PropertyMetadata((object) (Orientation) 1));
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof (TextWrapping), typeof (TextWrapping), typeof (ToastPrompt), new PropertyMetadata((object) (TextWrapping) 1, new PropertyChangedCallback(ToastPrompt.OnTextWrapping)));

    public ToastPrompt()
    {
      this.DefaultStyleKey = (object) typeof (ToastPrompt);
      this.IsAppBarVisible = true;
      this.IsBackKeyOverride = true;
      this.IsCalculateFrameVerticalOffset = true;
      this.Overlay = (Brush) Application.Current.Resources[(object) "TransparentBrush"];
      this.AnimationType = DialogService.AnimationTypes.SlideHorizontal;
      ((UIElement) this).ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ToastPrompt_ManipulationStarted);
      ((UIElement) this).ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.ToastPrompt_ManipulationDelta);
      ((UIElement) this).ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.ToastPrompt_ManipulationCompleted);
      this._timer = new Timer(new TimerCallback(this._timer_Tick));
      this.Opened += new EventHandler(this.ToastPrompt_Opened);
    }

    private void ToastPrompt_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (e.TotalManipulation.Translation.X > 200.0 || e.FinalVelocities.LinearVelocity.X > 1000.0)
        this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
        {
          PopUpResult = PopUpResult.UserDismissed
        });
      else if (e.TotalManipulation.Translation.X < 20.0)
      {
        this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
        {
          PopUpResult = PopUpResult.Ok
        });
      }
      else
      {
        this._translate.X = 0.0;
        this.StartTimer();
      }
    }

    private void ToastPrompt_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      this._translate.X += e.DeltaManipulation.Translation.X;
      if (this._translate.X >= 0.0)
        return;
      this._translate.X = 0.0;
    }

    private void StartTimer()
    {
      if (this._timer == null)
        return;
      this._timer.Change(TimeSpan.FromMilliseconds((double) this.MillisecondsUntilHidden), TimeSpan.FromMilliseconds(-1.0));
    }

    private void PauseTimer()
    {
      if (this._timer == null)
        return;
      this._timer.Change(TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
    }

    private void ToastPrompt_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.PauseTimer();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this._translate = new TranslateTransform();
      ((UIElement) this).RenderTransform = (Transform) this._translate;
      this.ToastImage = this.GetTemplateChild("ToastImage") as Image;
      if (this.ToastImage != null && this.ImageSource != null)
      {
        this.ToastImage.Source = this.ImageSource;
        this.SetImageVisibility(this.ImageSource);
      }
      this.SetTextOrientation(this.TextWrapping);
    }

    public override void Show()
    {
      if (!this.IsTimerEnabled)
        return;
      base.Show();
    }

    private void ToastPrompt_Opened(object sender, EventArgs e) => this.StartTimer();

    private void _timer_Tick(object state)
    {
      ((DependencyObject) this).Dispatcher.BeginInvoke((Action) (() => this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
      {
        PopUpResult = PopUpResult.NoResponse
      })));
    }

    public override void OnCompleted(PopUpEventArgs<string, PopUpResult> result)
    {
      this.PauseTimer();
      base.OnCompleted(result);
    }

    public int MillisecondsUntilHidden
    {
      get => (int) ((DependencyObject) this).GetValue(ToastPrompt.MillisecondsUntilHiddenProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToastPrompt.MillisecondsUntilHiddenProperty, (object) value);
      }
    }

    public bool IsTimerEnabled
    {
      get => (bool) ((DependencyObject) this).GetValue(ToastPrompt.IsTimerEnabledProperty);
      set => ((DependencyObject) this).SetValue(ToastPrompt.IsTimerEnabledProperty, (object) value);
    }

    public string Title
    {
      get => (string) ((DependencyObject) this).GetValue(ToastPrompt.TitleProperty);
      set => ((DependencyObject) this).SetValue(ToastPrompt.TitleProperty, (object) value);
    }

    public string Message
    {
      get => (string) ((DependencyObject) this).GetValue(ToastPrompt.MessageProperty);
      set => ((DependencyObject) this).SetValue(ToastPrompt.MessageProperty, (object) value);
    }

    public ImageSource ImageSource
    {
      get => (ImageSource) ((DependencyObject) this).GetValue(ToastPrompt.ImageSourceProperty);
      set => ((DependencyObject) this).SetValue(ToastPrompt.ImageSourceProperty, (object) value);
    }

    public Orientation TextOrientation
    {
      get => (Orientation) ((DependencyObject) this).GetValue(ToastPrompt.TextOrientationProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToastPrompt.TextOrientationProperty, (object) value);
      }
    }

    public TextWrapping TextWrapping
    {
      get => (TextWrapping) ((DependencyObject) this).GetValue(ToastPrompt.TextWrappingProperty);
      set => ((DependencyObject) this).SetValue(ToastPrompt.TextWrappingProperty, (object) value);
    }

    private static void OnTextWrapping(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is ToastPrompt toastPrompt) || toastPrompt.ToastImage == null)
        return;
      toastPrompt.SetTextOrientation((TextWrapping) e.NewValue);
    }

    private static void OnImageSource(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is ToastPrompt toastPrompt) || toastPrompt.ToastImage == null)
        return;
      toastPrompt.SetImageVisibility(e.NewValue as ImageSource);
    }

    private void SetImageVisibility(ImageSource source)
    {
      ((UIElement) this.ToastImage).Visibility = source == null ? (Visibility) 1 : (Visibility) 0;
    }

    private void SetTextOrientation(TextWrapping value)
    {
      if (value != 2)
        return;
      this.TextOrientation = (Orientation) 0;
    }
  }
}
