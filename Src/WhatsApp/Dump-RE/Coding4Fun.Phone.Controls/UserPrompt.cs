// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.UserPrompt
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public abstract class UserPrompt : ActionPopUp<string, PopUpResult>
  {
    private readonly RoundButton _cancelButton;
    protected internal Action MessageChanged;
    public readonly DependencyProperty IsCancelVisibileProperty = DependencyProperty.Register(nameof (IsCancelVisible), typeof (bool), typeof (UserPrompt), new PropertyMetadata((object) false, new PropertyChangedCallback(UserPrompt.OnCancelButtonVisibilityChanged)));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (string), typeof (UserPrompt), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (UserPrompt), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof (Message), typeof (string), typeof (UserPrompt), new PropertyMetadata((object) "", new PropertyChangedCallback(UserPrompt.OnMesageContentChanged)));

    protected UserPrompt()
    {
      RoundButton roundButton = new RoundButton();
      this._cancelButton = new RoundButton()
      {
        ImageSource = (ImageSource) new BitmapImage(new Uri("/Coding4Fun.Phone.Controls;component/Media/icons/appbar.cancel.rest.png", UriKind.RelativeOrAbsolute))
      };
      ((ButtonBase) roundButton).Click += new RoutedEventHandler(this.ok_Click);
      ((ButtonBase) this._cancelButton).Click += new RoutedEventHandler(this.cancelled_Click);
      this.ActionPopUpButtons.Add((Button) roundButton);
      this.ActionPopUpButtons.Add((Button) this._cancelButton);
      this.SetCancelButtonVisibility(this.IsCancelVisible);
    }

    public bool IsCancelVisible
    {
      get => (bool) ((DependencyObject) this).GetValue(this.IsCancelVisibileProperty);
      set => ((DependencyObject) this).SetValue(this.IsCancelVisibileProperty, (object) value);
    }

    public string Value
    {
      get => (string) ((DependencyObject) this).GetValue(UserPrompt.ValueProperty);
      set => ((DependencyObject) this).SetValue(UserPrompt.ValueProperty, (object) value);
    }

    public string Title
    {
      get => (string) ((DependencyObject) this).GetValue(UserPrompt.TitleProperty);
      set => ((DependencyObject) this).SetValue(UserPrompt.TitleProperty, (object) value);
    }

    public string Message
    {
      get => (string) ((DependencyObject) this).GetValue(UserPrompt.MessageProperty);
      set => ((DependencyObject) this).SetValue(UserPrompt.MessageProperty, (object) value);
    }

    private static void OnMesageContentChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      UserPrompt userPrompt = (UserPrompt) o;
      if (userPrompt == null || e.NewValue == e.OldValue || userPrompt.MessageChanged == null)
        return;
      userPrompt.MessageChanged();
    }

    private static void OnCancelButtonVisibilityChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      UserPrompt userPrompt = (UserPrompt) o;
      if (userPrompt == null || e.NewValue == e.OldValue)
        return;
      userPrompt.SetCancelButtonVisibility((bool) e.NewValue);
    }

    private void SetCancelButtonVisibility(bool value)
    {
      ((UIElement) this._cancelButton).Visibility = value ? (Visibility) 0 : (Visibility) 1;
    }

    private void ok_Click(object sender, RoutedEventArgs e)
    {
      this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
      {
        Result = this.Value,
        PopUpResult = PopUpResult.Ok
      });
    }

    private void cancelled_Click(object sender, RoutedEventArgs e)
    {
      this.OnCompleted(new PopUpEventArgs<string, PopUpResult>()
      {
        PopUpResult = PopUpResult.Cancelled
      });
    }
  }
}
