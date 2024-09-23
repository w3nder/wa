// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.PasswordInputPrompt
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class PasswordInputPrompt : InputPrompt
  {
    private readonly StringBuilder _inputText = new StringBuilder();
    public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register(nameof (PasswordChar), typeof (char), typeof (PasswordInputPrompt), new PropertyMetadata((object) '●'));

    public PasswordInputPrompt() => this.DefaultStyleKey = (object) typeof (PasswordInputPrompt);

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (this.InputBox == null)
        return;
      this.InputBox.TextChanged += new TextChangedEventHandler(this.InputBoxTextChanged);
      this.InputBox.SelectionChanged += new RoutedEventHandler(this.InputBoxSelectionChanged);
    }

    private void InputBoxSelectionChanged(object sender, RoutedEventArgs e)
    {
      if (this.InputBox.SelectionLength <= 0)
        return;
      this.InputBox.SelectionLength = 0;
    }

    private void InputBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      int length1 = this.InputBox.Text.Length - this._inputText.Length;
      if (length1 < 0)
      {
        int length2 = length1 * -1;
        int startIndex = this.InputBox.SelectionStart + 1 - length2;
        if (startIndex < 0)
          startIndex = 0;
        this._inputText.Remove(startIndex, length2);
        this.Value = this._inputText.ToString();
      }
      else
      {
        if (length1 <= 0)
          return;
        int selectionStart = this.InputBox.SelectionStart;
        int num = selectionStart - 1;
        string str = this.InputBox.Text.Substring(num, length1);
        this._inputText.Insert(num, str);
        this.Value = this._inputText.ToString();
        if (this.InputBox.Text.Length >= 2)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Insert(0, this.PasswordChar.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.InputBox.Text.Length - 1);
          stringBuilder.Insert(num, str);
          this.InputBox.Text = stringBuilder.ToString();
        }
        this.InputBox.SelectionStart = selectionStart;
      }
    }

    public char PasswordChar
    {
      get => (char) ((DependencyObject) this).GetValue(PasswordInputPrompt.PasswordCharProperty);
      set
      {
        ((DependencyObject) this).SetValue(PasswordInputPrompt.PasswordCharProperty, (object) value);
      }
    }
  }
}
