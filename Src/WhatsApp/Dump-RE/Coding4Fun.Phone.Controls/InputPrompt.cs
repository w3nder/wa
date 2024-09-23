// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.InputPrompt
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class InputPrompt : UserPrompt
  {
    private const string InputBoxName = "inputBox";
    protected TextBox InputBox;
    public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(nameof (InputScope), typeof (InputScope), typeof (InputPrompt), (PropertyMetadata) null);
    public static readonly DependencyProperty IsSubmitOnEnterKeyProperty = DependencyProperty.Register(nameof (IsSubmitOnEnterKey), typeof (bool), typeof (InputPrompt), new PropertyMetadata((object) true, new PropertyChangedCallback(InputPrompt.OnIsSubmitOnEnterKeyPropertyChanged)));

    public InputPrompt() => this.DefaultStyleKey = (object) typeof (InputPrompt);

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.InputBox = this.GetTemplateChild("inputBox") as TextBox;
      if (this.InputBox == null)
        return;
      Binding binding = new Binding()
      {
        Source = (object) this.InputBox,
        Path = new PropertyPath("Text", new object[0])
      };
      ((FrameworkElement) this).SetBinding(UserPrompt.ValueProperty, binding);
      this.HookUpEventForIsSubmitOnEnterKey();
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.DelayInputSelect));
    }

    private void DelayInputSelect(object value)
    {
      Thread.Sleep(250);
      ((DependencyObject) this).Dispatcher.BeginInvoke((Action) (() =>
      {
        ((Control) this.InputBox).Focus();
        this.InputBox.SelectAll();
      }));
    }

    public InputScope InputScope
    {
      get => (InputScope) ((DependencyObject) this).GetValue(InputPrompt.InputScopeProperty);
      set => ((DependencyObject) this).SetValue(InputPrompt.InputScopeProperty, (object) value);
    }

    public bool IsSubmitOnEnterKey
    {
      get => (bool) ((DependencyObject) this).GetValue(InputPrompt.IsSubmitOnEnterKeyProperty);
      set
      {
        ((DependencyObject) this).SetValue(InputPrompt.IsSubmitOnEnterKeyProperty, (object) value);
      }
    }

    private static void OnIsSubmitOnEnterKeyPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is InputPrompt inputPrompt))
        return;
      inputPrompt.HookUpEventForIsSubmitOnEnterKey();
    }

    private void HookUpEventForIsSubmitOnEnterKey()
    {
      this.InputBox = this.GetTemplateChild("inputBox") as TextBox;
      if (this.InputBox == null)
        return;
      ((UIElement) this.InputBox).KeyDown -= new KeyEventHandler(this.InputBoxKeyDown);
      if (!this.IsSubmitOnEnterKey)
        return;
      ((UIElement) this.InputBox).KeyDown += new KeyEventHandler(this.InputBoxKeyDown);
    }

    private void InputBoxKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != 3)
        return;
      this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
      {
        Result = this.Value,
        PopUpResult = PopUpResult.Ok
      });
    }
  }
}
